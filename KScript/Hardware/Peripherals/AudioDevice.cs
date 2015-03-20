using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KScript.Hardware.Peripherals
{
    class AudioDevice : PassiveHardware 
    {
        Thread t;
        List<Tuple<int, int>> signal;

        public AudioDevice()
            : base("System Speaker", HardwareType.AudioDevice)
        {
            signal = new List<Tuple<int, int>>();
        }
        
        protected override void NewDataInPort()
        {
            int length = InPort;
            t = new Thread(Play);
            t.Start(new Tuple<int, int>(800, length));
            ClearInPort();
        }

        void Play(object info)
        {
            Tuple<int, int> i = (Tuple<int, int>)info;
            Console.Beep(i.Item1, i.Item2);
        }
    }
}
