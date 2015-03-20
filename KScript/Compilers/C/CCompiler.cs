using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Compilers.C
{
    //class CCompiler : Compiler
    //{
    //    public CCompiler(FileInfo[] files, DirectoryInfo stdLibDirectory, DirectoryInfo outputPath, FileInfo outputFile)
    //        : base(files, (files != null && files.Length > 0) ? files[0].Directory : null, outputPath, outputFile, stdLibDirectory, new TokenizerOptions(
    //                true,
    //                new char[] { ',', ';', '\'', '"', '(', ')', '<', '>', '[', ']', '+', '-', '=', '*', '/', '{', '}', '&', '%', '^', '~', '.', '!' },
    //                new string[] { "++", "--", ">=", "<=", "==", "!=" }), PreprocessorOptions.IncludeInsert)
    //    {
            
    //    }

    //    protected override void Compile()
    //    {

    //    }

    //    protected override void Preprocess()
    //    {
    //        string[] lines = currentCode.Split('\n');
    //        for (int i = 0; i < lines.Length; i++)
    //        {
    //            string line = lines[i].Trim();
    //            if (line == "") continue;

    //            if (line.StartsWith("//"))
    //            {
    //                lines[i] = "";
    //                continue;
    //            }

    //            string[] tokens = line.Split(' ');
    //            switch (tokens[0])
    //            {
    //                case "#include":
    //                    lifoFiles.Push(currentFile);
    //                    if (tokens[1].StartsWith("<") & tokens[1].EndsWith(">"))
    //                    {
    //                        IncludeFile(tokens[1].Substring(1, tokens[1].Length - 2), true);
    //                    }
    //                    else if (tokens[1].StartsWith("\"") & tokens[1].EndsWith("\""))
    //                    {
    //                        IncludeFile(tokens[1].Substring(1, tokens[1].Length - 2), false);
    //                    }
    //                    else
    //                    {
    //                        PreprocessorError("Filename must be enclosed by quotes or angle brackets.");
    //                    }
    //                    break;
    //            }
    //        }
    //    }

    //    protected override void Postprocess()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
