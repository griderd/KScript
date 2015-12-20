using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Hardware
{
    /// <summary>
    /// Fixed I/O port list.
    /// </summary>
    public enum IOPorts : byte
    {
        SystemBoard = 0x00,
        BIOS = 0x01,
        RAM = 0x02,
        CPU = 0x03,
        ALU = 0x04,
        FPU = 0x05,
        Video = 0x06,
        Disk1 = 0x07,
        Disk2 = 0x08,
        Sound = 0x09,
        Keyboard = 0x0A
    }

    /// <summary>
    /// System board status flags.
    /// </summary>
    [Flags]
    public enum SystemBoardState : int
    {
        Normal = 0,
        NoBIOS = 1,
        NoRAM = 2,
        NoCPU = 4,
        NoALU = 8,
        BIOSCompiling = 16,
        BIOSError = 32,
        POSTRunning = 64,
        BadCPU = 128,
        BadRAM = 256
    }

    public struct HardwareInfo
    {
        public int port;
        public string name;
    }

    public class SystemBoard : Hardware
    {
        #region Fields
        /// <summary>
        /// Currently installed Hardware objects.
        /// </summary>
        Hardware[] hardware;

        Storage BIOS;
        CPU cpu;
        ALU alu;
        RAM ram;

        /// <summary>
        /// The graphics adapter. 
        /// TODO: Allow user to change GraphicsAdapter type.
        /// </summary>
        GraphicsAdapters.MonochromeDisplayAdapter mda;

        #endregion 

        #region Properties

        /// <summary>
        /// Gets the current state of the system board.
        /// </summary>
        public SystemBoardState State { get; private set; }

        /// <summary>
        /// Gets the hardware at the given port.
        /// </summary>
        /// <param name="port">I/O Port identifier.</param>
        /// <returns>Returns the Hardware object if the hardware is installed. If no hardware exists on the specified port, returns null.</returns>
        public Hardware this[IOPorts port]
        {
            get
            {
                return hardware[(int)port];
            }
        }

        /// <summary>
        /// Gets the hardware at the given port.
        /// </summary>
        /// <param name="port">I/O port identifier. Must be a value between 0 and 255.</param>
        /// <returns>Returns the Hardware object if the hardware is installed. If no hardware exists on the specified port, returns null.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is less than 0 or greater than 255.</exception>
        public Hardware this[int port]
        {
            get
            {
                if ((port < 0) | (port > 255))
                    throw new IndexOutOfRangeException("Port numbers range between 0 and 255.");
                return hardware[port];
            }
        }

        /// <summary>
        /// Gets a list of all hardware currently installed.
        /// </summary>
        public HardwareInfo[] HardwareTable
        {
            get
            {
                List<HardwareInfo> installed = new List<HardwareInfo>();
                for (int i = 0; i < 256; i++)
                {
                    if (this[i] != null)
                    {
                        installed.Add(new HardwareInfo { port = i, name = this[i].Name });
                    }
                }
                return installed.ToArray();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new SystemBoard instance.
        /// </summary>
        /// <param name="mode">The mode the CPU should start in.</param>
        /// <param name="cpuFrequency">The CPU's clock frequency.</param>
        /// <param name="memorySize">The size of the RAM in bytes.</param>
        /// <remarks>If the CPU mode is set to CPU.Debug, the RAM size is limited to 64 kilobytes.</remarks>
        public SystemBoard(CPUMode mode, double cpuFrequency, int memorySize)
            : base(HardwareType.SystemBoard)
        {
            hardware = new Hardware[256];
            BIOS = new Storage(HardwareType.BIOS);
            cpu = new CPU(this, mode, cpuFrequency);
            if (mode == CPUMode.Debug)
                ram = new RAM(0x10000);
            else
                ram = new RAM(memorySize);
                //ram = new RAM(0x1000000);
            alu = new ALU();
            mda = new GraphicsAdapters.MonochromeDisplayAdapter();
            hardware[(int)IOPorts.SystemBoard] = this;
            hardware[(int)IOPorts.BIOS] = BIOS;
            hardware[(int)IOPorts.CPU] = cpu;
            hardware[(int)IOPorts.RAM] = ram;
            hardware[(int)IOPorts.ALU] = alu;
            hardware[(int)IOPorts.Sound] = new Peripherals.AudioDevice();
            hardware[(int)IOPorts.Video] = mda;

        }

        #endregion

        /// <summary>
        /// Steps the CPU through the next instruction.
        /// </summary>
        public void StepCPU()
        {
            cpu.Step();
        }

        /// <summary>
        /// Initializes the attached hardware, assembles the POST software from code,
        /// loads the resulting machine code into memory, and starts the CPU.
        /// </summary>
        protected override void _Start()
        {
            // Start hardware devices
            foreach (Hardware device in hardware)
            {
                if ((device != null) & (device != this)) 
                    device.Start();
            }

            // Assemble POST
            Assembler.Assemble asm = new Assembler.Assemble(Firmware.BIOS.POST, "POST");


            State = (SystemBoardState)0xF;
            if (this[IOPorts.BIOS] != null) State ^= SystemBoardState.NoBIOS;
            if (this[IOPorts.CPU] != null) State ^= SystemBoardState.NoCPU;
            if (this[IOPorts.ALU] != null) State ^= SystemBoardState.NoALU;
            if (this[IOPorts.RAM] != null) State ^= SystemBoardState.NoRAM;

            if (State == SystemBoardState.Normal)
            {
                State = SystemBoardState.BIOSCompiling;
                string[] biosErrors = asm.Errors;
                Firmware.BIOS.Errors = biosErrors;
                Firmware.BIOS.Warnings = asm.Warnings;
                byte[] biosCode = asm.Result;
                if (biosErrors.Length > 0)
                    State = SystemBoardState.BIOSError;
                else
                {
                    State = SystemBoardState.POSTRunning;
                    BIOS.Attach(biosCode);
                    CopyStorageToRAM(IOPorts.BIOS, 0);
                    cpu.Start();
                }
            }
        }

        protected override void _Stop()
        {
            mda.Clear();
            cpu.Stop();
        }

        public bool AttachHardware(Hardware device, byte port)
        {
            if (port > (byte)IOPorts.FPU)
            {
                if (this[port] == null)
                {
                    hardware[port] = device;
                    // TODO: notify the CPU of new hardware
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DetachHardware(byte port)
        {
            if (port > (byte)IOPorts.FPU)
            {
                if (this[port] != null)
                {
                    hardware[port] = null;
                    // TODO: notify the CPU that hardware has been detached

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        protected override void NewDataInPort()
        {
            State = (SystemBoardState)InPort;
            ClearInPort();
        }

        private void CopyStorageToRAM(IOPorts port, int memoryAddressOffset)
        {
            CopyStorageToRAM((int)port, memoryAddressOffset);
        }

        private void CopyStorageToRAM(int storagePort, int memoryAddressOffset)
        {
            Hardware device = this[storagePort];
            if ((device.Type >= HardwareType.RAM) & (device.Type <= HardwareType.RemovableStorage))
            {
                DataStore storage = (DataStore)device;
                int address = 0;
                for (int i = 0; i < storage.WordLength; i++)
                {
                    ram.WriteToPort(storage.EncodeIORequest(address + memoryAddressOffset, false, DataStore.DataSize.Integer));     // Tell RAM to write next word
                    storage.WriteToPort(storage.EncodeIORequest(address, true, DataStore.DataSize.Integer));                        // Tell storage to read value
                    ram.WriteToPort(storage.OutPort);                                                                               // Read value and write to RAM.
                    address += CPU.WORD_LENGTH;                                                                                     // Advance pointer
                }
                for (int i = 0; i < storage.WordModulus; i++)
                {
                    ram.WriteToPort(storage.EncodeIORequest(address + memoryAddressOffset, false, DataStore.DataSize.Byte));
                    storage.WriteToPort(storage.EncodeIORequest(address, true, DataStore.DataSize.Byte));
                    ram.WriteToPort(storage.OutPort);
                    address += 1;
                }
            }
            else
            {
                throw new Exceptions.DeviceNotSupportedException(storagePort);
            }
        }
    }
}
