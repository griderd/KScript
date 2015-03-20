using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KScript.Hardware;
using KScript.Hardware.GraphicsAdapters;
using KScript.Hardware.Peripherals;
using System.Threading;

namespace VirtualMachine
{
    static class Program
    {
        public static SystemBoard systemBoard;
        public static CPU cpu;
        public static RAM ram;
        public static ALU alu;
        public static MonochromeDisplayAdapter mda;
        //public static VideoDevice monitor;
        static Thread t;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            systemBoard = new SystemBoard(CPUMode.Real, 100, 0x10000);
            ram = (RAM)systemBoard[IOPorts.RAM];
            cpu = (CPU)systemBoard[IOPorts.CPU];
            alu = (ALU)systemBoard[IOPorts.ALU];
            mda = (MonochromeDisplayAdapter)systemBoard[IOPorts.Video];

            //monitor = new VideoDevice(mda.PixelResolution.Width, mda.PixelResolution.Height, "Monitor", mda);
            t = new Thread(new ThreadStart(Run));
            t.Start();
            //monitor.Run();
        }

        static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmSystemBoard());            
        }

        public static void Exit()
        {
            //monitor.Close();
        }
    }
}
