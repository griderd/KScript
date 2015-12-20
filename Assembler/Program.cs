using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KScript.Assembler;

namespace Assembler
{
    class Program
    {
        static Assemble asm;

        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found.");
                return;
            }

            string[] lines = File.ReadAllLines(args[0]);

            asm = new Assemble(lines, args[0]);

            if (asm.Errors.Length == 0)
                File.WriteAllBytes(args[1], asm.Result);

            Console.WriteLine("Assembly complete. {0} errors, {1} warnings.", asm.Errors.Length, asm.Warnings.Length);
            for (int i = 0; i < asm.Errors.Length; i++)
            {
                Console.WriteLine(asm.Errors[i]);
            }
            for (int i = 0; i < asm.Warnings.Length; i++)
            {
                Console.WriteLine(asm.Warnings[i]);
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
