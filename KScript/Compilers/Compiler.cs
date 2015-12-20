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
        TokenizerOptions options;
        DirectoryInfo stdLibPath;
        protected SourceFileCollection code;

        protected List<string> errors;
        protected List<string> warnings;

        protected List<string> assemblyCode;
        protected List<byte> byteCode;

        public string[] Errors { get { return errors.ToArray(); } }
        public string[] Warnings { get { return warnings.ToArray(); } }

        public string[] AssemblyCode { get { return assemblyCode.ToArray(); } }
        public byte[] ByteCode { get { return byteCode.ToArray(); } }

        int tokenIndex;

        protected Token CurrentToken 
        { 
            get 
            {
                if (tokenIndex >= 0)
                    return code.Tokens[tokenIndex];
                else if (code.Tokens.Length > 0)
                    return code.Tokens.Last();
                else
                    return new Token("", true, 0, "Unknown");
            } 
        }

        PreprocessorOptions preprocessorOptions;

        public Compiler(FileInfo[] files, DirectoryInfo stdlibPath, TokenizerOptions tokenOptions, PreprocessorOptions flags)
        {
            if ((files == null) | (stdlibPath == null))
            {
                throw new ArgumentNullException();
            }

            if (files.Length == 0)
                throw new ArgumentException();

            if (files.Contains(null))
                throw new ArgumentNullException();

            options = tokenOptions;
            this.stdLibPath = stdlibPath;

            assemblyCode = new List<string>();
            byteCode = new List<byte>();
            errors = new List<string>();
            warnings = new List<string>();  

            tokenIndex = -1;

            preprocessorOptions = flags;

            code = new SourceFileCollection(files, tokenOptions);

            Precompile();

            RunCompiler();
        }

        void RunCompiler()
        {
            Preprocess();

            Compile();

            Postprocess();

            Assemble asm = new Assemble(assemblyCode.ToArray(), "output");

            if (asm.Errors.Length > 0)
            {
                for (int i = 0; i < asm.Errors.Length; i++)
                {
                    Error("ASM: " + asm.Errors[i]);
                }
            }
            if (asm.Warnings.Length > 0)
            {
                for (int i = 0; i < asm.Errors.Length; i++)
                {
                    Error("ASM: " + asm.Errors[i]);
                }
            }

            if (asm.Errors.Length == 0)
                byteCode.AddRange(asm.Result);
        }

        protected bool GetNextToken()
        {
            tokenIndex++;
            if (tokenIndex >= code.Tokens.Length)
                tokenIndex = -1;

            return tokenIndex >= 0;
        }

        /// <summary>
        /// Perform this step before any preprocessing begins. Sets up the compiler environment.
        /// </summary>
        protected abstract void Precompile();

        protected abstract void Preprocess();

        protected abstract void Compile();

        protected abstract void Postprocess();


        protected void IncludeFile(string filePath, bool libraryFile, int index = 0)
        {
            FileInfo file = new FileInfo((libraryFile ? stdLibPath.FullName : Directory.GetCurrentDirectory()) +
                ((libraryFile ? stdLibPath.FullName : Directory.GetCurrentDirectory()).EndsWith("\\") ? filePath : "\\" + filePath));

            if (preprocessorOptions == PreprocessorOptions.IncludeAppend)
            {
                try
                {
                    code.AddFile(file, options);
                }
                catch (FileNotFoundException)
                {
                    errors.Add(string.Format("File \"{0}\" not found.", file.FullName));
                }
            }
            else
            {
                try
                {
                    code.InsertFile(file, index, options);
                }
                catch (FileNotFoundException)
                {
                    errors.Add(string.Format("File \"{0}\" not found.", file.FullName));
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
