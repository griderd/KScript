using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Compilers.IL
{
    class ILCompiler : Compiler
    {
        public ILCompiler(FileInfo[] files, DirectoryInfo rootPath, DirectoryInfo outputPath, FileInfo outputFile, DirectoryInfo libraryPath)
            : base(files, rootPath, outputPath, outputFile, libraryPath, 
                new TokenizerOptions(true, new char[] { ';', '+', '-', '*', '/', '%', '.', ',', '{', '}', '(', ')', '<', '>', '=', '!'},
                                           new string[] { "++", "--", ">=", "<=", "!=", "==" } ), PreprocessorOptions.IncludeInsert)
        {

        }

        protected override void Compile()
        {
            Func<string, string> GetToken = x =>
            {
                if (!GetNextToken())
                {
                    Error(x);
                    return null;
                }
                else
                    return CurrentToken.Value;
            };

            while (GetNextToken())
            {
                switch (CurrentToken.Value)
                {
                    case "function":
                        if (!CompileFunction()) return;
                        break;
                }
            }
        }

        private bool CompileFunction()
        {
            string functionName;

            if (!GetNextToken())
            {
                Error("Missing function name.");
                return false;
            }

            functionName = CurrentToken.Value;

            assemblyCodeFile.Add(functionName + ":");

            return true;
        }

        protected override void Preprocess()
        {
            throw new NotImplementedException();
        }

        protected override void Postprocess()
        {
            throw new NotImplementedException();
        }
    }
}
