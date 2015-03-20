using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KScript.Hardware;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemBoard board = new SystemBoard(CPUMode.Real, 10, 0x10000);
            board.Start();
            Console.WriteLine("Status Code: {0}", (int)board.State);
            bool noCPU = (board.State & SystemBoardState.NoCPU) == SystemBoardState.NoCPU;
            bool noBIOS = (board.State & SystemBoardState.NoBIOS) == SystemBoardState.NoBIOS;
            bool noALU = (board.State & SystemBoardState.NoALU) == SystemBoardState.NoALU;
            bool noRAM = (board.State & SystemBoardState.NoRAM) == SystemBoardState.NoRAM;
            bool BIOSError = (board.State & SystemBoardState.BIOSError) == SystemBoardState.BIOSError;

            if (noBIOS) Console.WriteLine("- No BIOS");
            if (noCPU) Console.WriteLine("- No CPU");
            if (noALU) Console.WriteLine("- No ALU");
            if (noRAM) Console.WriteLine("- No RAM");
            if (BIOSError) Console.WriteLine("- BIOS Error");
            if (board.State == SystemBoardState.Normal) Console.WriteLine("Status: Normal");

            Console.ReadLine();
        }
    }
}
