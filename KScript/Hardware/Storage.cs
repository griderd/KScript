using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Hardware
{
    /// <summary>
    /// Represents a permenant storage device, such as a hard disk
    /// </summary>
    public class Storage : DataStore
    {
        public bool Inserted { get; private set; }

        public Storage(HardwareType type)
            : this(Enum.GetName(typeof(HardwareType), type), type)
        {
        }

        public Storage(string name, HardwareType type)
            : base(0, name, type)
        {
        }

        public void Insert(string filename)
        {
            if (!File.Exists(filename))
            {
                RaiseError(Interrupts.HardwareInterruptType.DeviceNotReady);
                Inserted = false;
                return;
            }

            try
            {
                data = File.ReadAllBytes(filename);
                Inserted = true;
            }
            catch
            {
                RaiseError(Interrupts.HardwareInterruptType.DeviceNotReady);
                Inserted = false;
            }
        }

        public void Eject()
        {
            data = new byte[0];
            Inserted = false;
        }

        public void Attach(byte[] rawData)
        {
            if (rawData == null) throw new ArgumentNullException();
            data = new byte[rawData.Length];
            rawData.CopyTo(data, 0);
        }

        public int ReadWord(int address)
        {
            // Check for memory access violation
            if ((address < 0) | (address + CPU.WORD_LENGTH >= data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return 0;
            }

            return BitConverter.ToInt32(data, address);
        }

        public void WriteWord(int address, int value)
        {
            // Check for memory access violation
            if ((address < 0) | (address + CPU.WORD_LENGTH >= data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return;
            }

            Write(address, BitConverter.GetBytes(value));
        }

        protected override void _Start()
        {
            //throw new NotImplementedException();
        }

        protected override void _Stop()
        {
            //throw new NotImplementedException();
        }
    }
}
