using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSLib.Collections;

namespace KScript.Hardware
{
    public class RAM : DataStore 
    {
        public RAM(int capacity)
            : base(capacity, HardwareType.RAM)
        {
            
        }

        public byte ReadByte(int address)
        {
            if ((address < 0) | (address >= data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return 0;
            }

            return data[address];
        }

        public void WriteByte(int address, byte value)
        {
            if ((address < 0) | (address >= data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return;
            }

            data[address] = value;
        }

        public short ReadShort(int address)
        {
            if ((address < 0) | (address + (CPU.WORD_LENGTH / 2) > data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return 0;
            }

            return BitConverter.ToInt16(Read(address, CPU.WORD_LENGTH / 2), 0);
        }

        public void WriteShort(int address, short value)
        {
            if ((address < 0) | (address + (CPU.WORD_LENGTH / 2) > data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return;
            }

            Write(address, BitConverter.GetBytes(value));
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

        public string ReadString(int address)
        {
            StringBuilder sb = new StringBuilder();

            int addr = address;
            byte b = 0;

            do
            {
                try
                {
                    b = Read(addr, 1)[0];
                }
                catch (ArgumentOutOfRangeException)
                {
                    RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                    b = 0;
                }

                if (b != 0)
                    sb.Append(Encoding.UTF8.GetChars(new byte[] { b }));
            }
            while (b != 0);

            return sb.ToString();
        }

        protected override void _Start()
        {
            Clear();
        }

        protected override void _Stop()
        {
            Clear();
        }
    }
}
