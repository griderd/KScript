using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KScript.Assembler;

namespace KScript.Compilers
{
    [Flags]
    public enum PreprocessorOptions
    {
        /// <summary>
        /// If set to this, all files that are included by the preprocessor are inserted where the include call is made. C/C++ work like this.
        /// </summary>
        IncludeInsert = 0,
        /// <summary>
        /// If set to this, all files that are included are appended to the end of the code.
        /// </summary>
        IncludeAppend = 1
    }

    public abstract class Compiler
    {
        Tokenizer tokenizer;
        TokenizerOptions options;
        DirectoryInfo rootPath, outputPath, stdLibPath;
        FileInfo outputFile;

        protected RawCode code;

        List<string> errors;
        List<string> warnings;

        List<string> assemblyCode;
        List<byte> byteCode;

        protected List<string> assemblyCodeFile;

        protected DebugInfoCollection tokens;
        int tokenIndex;

        protected DebugInfoItem CurrentToken 
        { 
            get 
            {
                if (tokenIndex >= 0)
                    return tokens[tokenIndex];
                else if (tokens.Count > 0)
                    return tokens.Last();
                else
                    return new DebugInfoItem("", "Unknown", 0);
            } 
        }

        PreprocessorOptions preprocessorOptions;

        public Compiler(FileInfo[] files, DirectoryInfo rootPath, DirectoryInfo outputPath, FileInfo outputFile, DirectoryInfo stdlibPath, TokenizerOptions tokenOptions, PreprocessorOptions flags)
        {
            if ((files == null) | (rootPath == null) | (outputPath == null) | 
                (outputFile == null) | (stdlibPath == null))
            {
                throw new ArgumentNullException();
            }

            if (files.Length == 0)
                throw new ArgumentException();

            if (files.Contains(null))
                throw new ArgumentNullException();

            options = tokenOptions;
            this.rootPath = rootPath;
            this.outputFile = outputFile;
            this.outputPath = outputPath;
            this.stdLibPath = stdlibPath;

            assemblyCode = new List<string>();
            byteCode = new List<byte>();
            assemblyCodeFile = new List<string>();

            tokens = new DebugInfoCollection();

            tokenIndex = -1;

            preprocessorOptions = flags;

            code = new RawCode(files);

            RunCompiler();
        }

        void RunCompiler()
        {
            Preprocess();

            tokenizer = new Tokenizer(code.Text, options);
            Compile();

            Postprocess();

            Assemble asm = new Assemble(assemblyCode.ToArray(), "output");
            byteCode.AddRange(asm.Result);
        }

        protected bool GetNextToken()
        {
            tokenIndex++;
            if (tokenIndex >= tokens.Count)
                tokenIndex = -1;

            return tokenIndex >= 0;
        }

        protected abstract void Preprocess();

        protected abstract void Compile();

        protected abstract void Postprocess();

        protected void IncludeFile(string filePath, bool libraryFile, int index = 0)
        {
            FileInfo file = new FileInfo((libraryFile ? stdLibPath.FullName : rootPath.FullName) +
                ((libraryFile ? stdLibPath.FullName : rootPath.FullName).EndsWith("\\") ? filePath : "\\" + filePath));

            if (preprocessorOptions == PreprocessorOptions.IncludeAppend)
            {
                if (!code.AddFile(file))
                {
                    errors.AddRange(code.Errors);
                    code.ClearErrors();
                }
            }
            else
            {
                if (!code.InsertFile(file, index))
                {
                    errors.AddRange(code.Errors);
                    code.ClearErrors();
                }
            }
        }

        protected void LinkError(string filename, int lineNumber, string message)
        {
            errors.Add(String.Format("{0}({1}) LNK: {2}", filename, lineNumber, message));
        }

        protected void LinkError(string message)
        {
            errors.Add("LNK: " + message);
        }

        protected void Error(string message)
        {
            errors.Add(String.Format("{0}({1}): {2}", CurrentToken.Filename, CurrentToken.LineNumber, message));
        }
       
        protected void Warning(string message)
        {
            warnings.Add(String.Format("{0}({1}): {2}", CurrentToken.Filename, CurrentToken.LineNumber, message));
        }

        protected void PreprocessorError(string message)
        {
            errors.Add("PREPROCESSOR: " + message);
        }
    }
} 
