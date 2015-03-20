using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSLib.Collections;

namespace KScript.Hardware
{
    /// <summary>
    /// Represents a data store. Can be a cache, RAM, disk, or any other data store.
    /// </summary>
    public abstract class DataStore : Hardware
    {
        protected byte[] data;

        public byte[] Data
        {
            get
            {
                return data;
            }
        }
        public int Length { get { return data.Length; } }
        public int WordLength { get { return data.Length / CPU.WORD_LENGTH; } }
        public int WordModulus { get { return data.Length % CPU.WORD_LENGTH; } }

        #region Request Info
        
        protected int bytesProcessed;
        protected bool read;
        protected int address;
        protected int length;

        public enum DataSize
        {
            Byte = 0,
            Short = 2,
            Integer = 4,
            Long = 6
        }

        protected void ClearRequestInfo()
        {
            bytesProcessed = 0;
            address = 0;
            read = false;
            length = 0;
        }

        #endregion 

        public DataStore(int size, HardwareType type)
            : this(size, Enum.GetName(typeof(HardwareType), type), type)
        {
        }

        public DataStore(int size, string name, HardwareType type)
            : base(name, type)
        {
            data = new byte[size];
            ClearRequestInfo();
        }

        #region Generic I/O

        protected byte[] Read(int address, int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException();
            if (address < 0) throw new ArgumentOutOfRangeException();
            if (address >= data.Length) throw new ArgumentOutOfRangeException();

            byte[] value = new byte[length];
            for (int i = 0; i < length; i++)
            {
                value[i] = data[i + address];
            }

            return value;
        }

        protected void Write(int address, byte[] value)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.Length == 0) throw new ArgumentException();
            if (address < 0) throw new ArgumentOutOfRangeException();
            if (address + value.Length >= data.Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < value.Length; i++)
            {
                data[i + address] = value[i];
            }
        }

        protected int ReadToInteger(int address, int length)
        {
            List<byte> value = new List<byte>(Read(address, length));
            while (value.Count < CPU.WORD_LENGTH)
            {
                value.Add(0);
            }
            return BitConverter.ToInt32(value.ToArray(), 0);
        }

        protected void WriteFromInteger(int value, int address, int length)
        {
            byte[] val = BitConverter.GetBytes(value);
            for (int i = 0; i < length; i++)
            {
                data[i + address] = val[i];
            }
        }

        protected int ReadInteger(int address)
        {
            return BitConverter.ToInt32(data, address);
        }

        protected void WriteInteger(int address, int value)
        {
            byte[] val = BitConverter.GetBytes(value);
            for (int i = 0; i < val.Length; i++)
            {
                data[i + address] = val[i];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }

        #endregion

        #region Port I/O

        protected override void NewDataInPort()
        {
            // Stage 1: Decode
            if (length == 0)
            {
                DecodeIORequest(InPort, out address, out read, out length);
                if (read)
                {
                    if (length <= 4)
                    {
                        OutPort = length == 4 ? ReadInteger(address) : ReadToInteger(address, length);
                        ClearRequestInfo();
                    }
                    else
                    {
                        OutPort = ReadInteger(4);
                        bytesProcessed = 4;
                        address += 4;
                    }
                }
            }
            else if (bytesProcessed < length)   // Stage 2
            {
                int n = length - bytesProcessed;
                if (read)
                {
                    if (n <= 4)
                    {
                        OutPort = n == 4 ? ReadInteger(address) : ReadToInteger(address, n);
                        ClearRequestInfo();
                    }
                    else
                    {
                        OutPort = ReadInteger(address);
                        bytesProcessed += 4;
                        address += 4;
                    }
                }
                else
                {
                    if (n <= 4)
                    {
                        if (n == 4)
                            WriteInteger(address, InPort);
                        else
                            WriteFromInteger(InPort, address, n);
                        ClearRequestInfo();
                    }
                    else
                    {
                        WriteInteger(address, InPort);
                        bytesProcessed += 4;
                        address += 4;
                    }
                }
            }

            ClearInPort();
        }

        #endregion

        #region Request Encoding/Decoding Helpers
        /// <summary>
        /// Encodes an IO request by address, read/write, and size. The value is encoded as a little-endian integer.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="read"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual int EncodeIORequest(int address, bool read, DataSize size)
        {
            if ((address < 0) | (address > 0xFFFFFF)) throw new ArgumentOutOfRangeException();

            int header = read ? 1 : 0;
            header = header | (int)size;

            // Reserved bits go here

            int result = header | (address << 8);
            return result;
        }

        /// <summary>
        /// Decodes an IO request into an address, read/write, and length. The value is interpreted as a little-endian integer.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="address"></param>
        /// <param name="read"></param>
        /// <param name="length"></param>
        public virtual void DecodeIORequest(int request, out int address, out bool read, out int length)
        {
            // I'm using the GSLib.Collections.Bitfield class because I'm lazy.

            Bitfield bits = Bitfield.FromInt32(request);
            read = bits[0];

            length = 0; // to make the compiler happy

            if (!bits[1] & !bits[2])
                length = 1;
            else if (bits[1] & !bits[2])
                length = 2;
            else if (bits[2] & !bits[1])
                length = 4;
            else if (bits[1] & bits[2])
                length = 8;

            // Reserved bits go here

            address = request >> 8;
        }
        #endregion

    }
}
