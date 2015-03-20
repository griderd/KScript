using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Hardware
{
    /// <summary>
    /// Represents a hard disk mirrored in reality onto a directory.
    /// </summary>
    class Disk : Storage
    {
        DirectoryInfo diskDirectory;

        public Disk(DirectoryInfo directory)
            : base(HardwareType.HDD)
        {
            if (directory == null) throw new ArgumentNullException();
            if (!directory.Exists) throw new DirectoryNotFoundException();

            
        }
    }
}
