using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Interrupts
{
    public enum HardwareInterruptType
    {
        Generic,
        DivideByZero,
        ArithmeticStackUnderflow,
        SegmentationFault,
        DeviceNotReady,
        InputPortReady,
        OutputPortReady,
        DeviceAttached,
        DeviceDetached,
        ButtonDown,
        ButtonUp,
        ButtonStateChanged,
    }

    public class HardwareInterruptEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public HardwareInterruptType InterruptType { get; private set; }

        public HardwareInterruptEventArgs(HardwareInterruptType type)
        {
            InterruptType = type;
            Message = "";
        }

        public HardwareInterruptEventArgs(HardwareInterruptType type, string message)
        {
            InterruptType = type;
            Message = message;
        }
    }
}
