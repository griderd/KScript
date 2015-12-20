using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KScript.Assembler;
using KScript.Compilers;
using KScript.Compilers.IL;

namespace Compiler
{
    class Program
    {
        static KScript.Compilers.Compiler compiler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// output file
        /// input files[]
        /// </param>
        static void Main(string[] args)
        {
            DirectoryInfo stdlib = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "stdlib"));

            FileInfo[] files = new FileInfo[args.Length - 1];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new FileInfo(args[i + 1]);
            }

            switch (files[0].Extension)
            {
                case ".asm":
                    compiler = new SmartAssembler(files, stdlib);
                    break;
                    
                case ".il":
                    compiler = new ILCompiler(files, stdlib);
                    break;
            }

            if (compiler.Errors.Length == 0)
            {
                File.WriteAllBytes(args[0], compiler.ByteCode);
                File.WriteAllLines(args[0] + ".asm", compiler.AssemblyCode);
            }

            Console.WriteLine("Compilation complete. {0} errors, {1} warnings.", compiler.Errors.Length, compiler.Warnings.Length);

            for (int i = 0; i < compiler.Errors.Length; i++)
            {
                Console.WriteLine(compiler.Errors[i]);
            }
            for (int i = 0; i < compiler.Warnings.Length; i++)
            {
                Console.WriteLine(compiler.Warnings[i]);
            }
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
