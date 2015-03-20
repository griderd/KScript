using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KScript.Interrupts;

namespace KScript.Hardware
{
    public enum HardwareType
    {
        SystemBoard,
        CPU,
        ALU,
        FPU,
        RAM,
        BIOS,
        HDD,
        MagneticDrive,
        OpticalDrive,
        SSD,
        RemovableStorage,
        VideoDevice,
        AudioDevice,
        HumanInterfaceDevice
    }

    public abstract class Hardware
    {
        public event EventHandler<HardwareInterruptEventArgs> ErrorEvent;
        public event EventHandler<HardwareInterruptEventArgs> PortReady;

        /// <summary>
        /// Name of the device. 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Type of device.
        /// </summary>
        public HardwareType Type { get; private set; }

        /// <summary>
        /// Gets whether the device is running.
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Device-specific on-start initialization code.
        /// </summary>
        protected abstract void _Start();

        /// <summary>
        /// Device-specific on-stop cleanup code.
        /// </summary>
        protected abstract void _Stop();

        /// <summary>
        /// Turns the device on and sets Running to true. Any on-start initialization code should be put in _Start().
        /// </summary>
        public void Start()
        {
            Running = true;
            _Start();
        }

        /// <summary>
        /// Turns the device off and sets Running to false. Any on-stop cleanup code should be put in _Stop().
        /// </summary>
        public void Stop()
        {
            Running = false;
            _Stop();
        }

        /// <summary>
        /// Gets or sets the value of the device's input serial port.
        /// </summary>
        protected int InPort { get; private set; }

        /// <summary>
        /// Gets or sets the value of the device's output serial port.
        /// </summary>
        public int OutPort { get; protected set; }

        public Hardware(HardwareType type)
            : this(Enum.GetName(typeof(HardwareType), type), type)
        {
        }

        public Hardware(string name, HardwareType type)
        {
            if (name == null) throw new ArgumentNullException();
            if (name == "") throw new ArgumentException();

            Name = name;
            Type = type;
        }

        protected void RaiseError(HardwareInterruptType type)
        {
            if (ErrorEvent != null) ErrorEvent(this, new HardwareInterruptEventArgs(type));
        }

        protected void RaisePortReady(bool inPort)
        {
            if (PortReady != null) ErrorEvent(this, new HardwareInterruptEventArgs(inPort ? HardwareInterruptType.InputPortReady : HardwareInterruptType.OutputPortReady));
        }

        /// <summary>
        /// Writes data to the devices IN port. Device must be running to read data.
        /// </summary>
        /// <param name="data">Data to write to the devices IN port. If the value is -1, the device is powered off.</param>
        public void WriteToPort(int data)
        {
            if (!Running) return;

            InPort = data;
            if (data == -1)
                Stop();
            else
                NewDataInPort();
        }

        /// <summary>
        /// This is called when there is new data in the IN port.
        /// </summary>
        protected abstract void NewDataInPort();

        protected void ClearInPort()
        {
            if (Running)
                RaisePortReady(true);
        }

        protected void WriteToOut(int data)
        {
            OutPort = data;
            RaisePortReady(false);
        }
    }
}
