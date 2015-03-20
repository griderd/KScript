using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Hardware
{
    /// <summary>
    /// Passive hardware is always "running." It only needs an input signal to do something.
    /// </summary>
    public abstract class PassiveHardware : Hardware
    {
        protected override void _Start()
        {
            // throw new NotImplementedException();
        }

        protected override void _Stop()
        {
            // throw new NotImplementedException();
        }

        public PassiveHardware(string name, HardwareType type)
            : base(name, type)
        {
            Start();
        }

        public PassiveHardware(HardwareType type)
            : base(type)
        {
            Start();
        }
    }
}
