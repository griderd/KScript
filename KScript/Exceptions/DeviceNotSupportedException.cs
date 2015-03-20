using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Exceptions
{
    public class DeviceNotSupportedException : Exception
    {
        /// <summary>
        ///  The SystemBoard port on which the device lives.
        /// </summary>
        public int DeviceId { get; private set; }

        public DeviceNotSupportedException(int deviceId)
            : base("The device does not support this operation.")
        {
            DeviceId = deviceId;
        }
    }
}
