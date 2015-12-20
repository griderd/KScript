using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KScript.Compilers.LanguageObjects;

namespace KScript.Compilers.IL
{
    public partial class ILCompiler : Compiler
    {
        enum BlockTypes
        {
            Global,
            Function,
            IfBlock,
            ElseBlock,
            DoWhileLoop,
            WhileLoop
        }

        Stack<BlockTypes> blocks;
        Stack<Dictionary<string, string>> variableScope;
        List<Function> functions;

        Dictionary<string, string> types;
        string[] keywords = new string[] 
        {
            "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "string",
            "function", "endfunction", "if", "else", "endif", "while", "do", "endif", "endloop", "proto", "returns",
            "return", "void", "null", "asm", "endasm"
        };

        public ILCompiler(FileInfo[] files, DirectoryInfo libraryPath)
            : base(files, libraryPath, 
                new TokenizerOptions(true, new char[] { ';', '+', '-', '*', '/', '%', '.', ',', '{', '}', '(', ')', '<', '>', '=', '!'},
                                           new string[] { "++", "--", ">=", "<=", "!=", "==" } ), PreprocessorOptions.IncludeInsert)
        {
            
        }

        protected override void Precompile()
        {
            types = new Dictionary<string, string>();
            types.Add("byte", "byte");
            types.Add("sbyte", "signed byte");
            types.Add("short", "short");
            types.Add("ushort", "unsigned short");
            types.Add("int", "word");
            types.Add("uint", "unsigned word");
            types.Add("long", "long");
            types.Add("ulong", "unsigned long");
            types.Add("float", "float");
            types.Add("double", "double");
            types.Add("string", "string");

            blocks = new Stack<BlockTypes>();
            variableScope = new Stack<Dictionary<string, string>>();
            functions = new List<Function>();

            StartScope(BlockTypes.Global);
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
                        if (blocks.Peek() != BlockTypes.Global)
                        {
                            Error("Cannot define function inside a block.");
                            return;
                        }

                        if (!ParseFunction()) return;
                        break;

                    case "endfunction":
                        if (blocks.Peek() != BlockTypes.Function)
                        {
                            if (blocks.Peek() == BlockTypes.Global)
                                Error("endfunction without matching function.");
                            else
                                Error("Cannot close function block before nested blocks are closed.");
                            return;
                        }

                        EndScope();
                        break;

                    case "asm":
                        break;

                    case "if":
                        if (blocks.Peek() == BlockTypes.Global)
                        {
                            Error("Cannot define if block outside a function.");
                            return;
                        }

                        // TODO: Parse IF conditional
                        StartScope(BlockTypes.IfBlock);
                        break;

                    case "else":
                        if (blocks.Peek() != BlockTypes.IfBlock)
                        {
                            Error("Cannot define else block without an if block.");
                            return;
                        }

                        EndScope();
                        StartScope(BlockTypes.ElseBlock);
                        break;

                    case "endif":
                        if ((blocks.Peek() != BlockTypes.IfBlock) & (blocks.Peek() != BlockTypes.ElseBlock))
                        {
                            Error("endif without matching if.");
                            return;
                        }

                        EndScope();
                        break;

                    case "do":
                        if (blocks.Peek() == BlockTypes.Global)
                        {
                            Error("Cannot define do-while block outside a function.");
                            return;
                        }

                        StartScope(BlockTypes.DoWhileLoop);
                        break;

                    case "while":
                        if (blocks.Peek() == BlockTypes.DoWhileLoop)
                        {
                            EndScope();
                            // TODO: Parse DO-WHILE conditional
                        }
                        else if (blocks.Peek() == BlockTypes.Global)
                        {
                            StartScope(BlockTypes.WhileLoop);
                            // TODO: Parse WHILE conditional
                        }
                        break;

                    case "return":
                        ParseExpression(false);
                        AddAsm("ret");
                        break;

                }
            }
        }

        /// <summary>
        /// Parses a function header. This is called when the previous token was "function".
        /// </summary>
        /// <returns>Returns true on success (no errors). Otherwise, returns false.</returns>
        private bool ParseFunction()
        {
            string functionName;
            string returnType;
            Dictionary<string, string> parameterList;

            if (!GetNextToken())
            {
                Error("Missing function name.");
                return false;
            }

            functionName = CurrentToken.Value;

            AddAsm(functionName + ":");
            if (!ParseParameterList(out parameterList))
                return false;

            if (!GetNextToken())
            {
                Error("Expected \"returns\"");
                return false;
            }
            else if (CurrentToken.Value != "returns")
            {
                Error("Expected \"returns\"");
                return false;
            }

            if (!GetNextToken())
            {
                Error("Expected return type.");
                return false;
            }
            else if (!types.ContainsKey(CurrentToken.Value))
            {
                Error("Invalid type.");
                return false;
            }

            returnType = CurrentToken.Value;

            functions.Add(new Function(functionName, parameterList, returnType, types));
            StartScope(BlockTypes.Function);
            
            return true;
        }

        private bool ParseParameterList(out Dictionary<string, string> paramList)
        {
            paramList = new Dictionary<string, string>();
            string paramType = "";
            string paramName = "";

            if (!GetNextToken())
            {
                Error("Missing parameter list.");
                return false;
            }
            else if (CurrentToken.Value != "(")
            {
                Error("Parameter list must begin be enclosed in parentheses.");
                return false;
            }

            while (true)
            {
                if (!GetNextToken())
                {
                    Error("Expected parameter type.");
                    return false;
                }
                else if (CurrentToken.Value == ")")
                {
                    return true;
                }
                else if (!IsValidType())
                {
                    Error("Invalid type.");
                    return false;
                }

                paramType = CurrentToken.Value;

                if (!GetNextToken())
                {
                    Error("Expected parameter name.");
                    return false;
                }
                else if (IsKeyword())
                {
                    Error("Parameter name cannot be a keyword.");
                    return false;
                }

                paramName = CurrentToken.Value;

                if (paramList.ContainsKey(paramName))
                    Error("Parameter declared more than once.");
                paramList.Add(paramName, paramType);

                if (!GetNextToken())
                {
                    Error("Expected comma or closing parenthesis.");
                    return false;
                }
                else if ((CurrentToken.Value != ",") & (CurrentToken.Value != ")"))
                {
                    Error("Expected comma or closing parenthesis.");
                    return false;
                }
                else if (CurrentToken.Value == ")")
                {
                    return true;
                }
            }
        }

        private void StartScope(BlockTypes block)
        {
            variableScope.Push(new Dictionary<string, string>());
            blocks.Push(block);
        }

        private void EndScope()
        {
            variableScope.Pop();
            blocks.Pop();
        }

        private bool AddToScope(string variable)
        {
            if (variableScope.Peek().ContainsKey(variable))
                return false;
            variableScope.Peek().Add(variable, GenerateUniqueVariableName(variable));
            return true;
        }

        private bool IsKeyword(string value)
        {
            return keywords.Contains(value);
        }

        private bool IsKeyword()
        {
            return IsKeyword(CurrentToken.Value);
        }

        private bool IsValidType()
        {
            return IsValidType(CurrentToken.Value);
        }

        private bool IsValidType(string type)
        {
            return types.ContainsKey(type);
        }

        protected override void Preprocess()
        {
            // TODO: Implement preprocessor.
            //throw new NotImplementedException();
        }

        protected override void Postprocess()
        {
            // TODO: Implement postprocessor
            //throw new NotImplementedException();
        }

        void AddAsm(string asm)
        {
            assemblyCode.Add(string.Format("{0}{1}", new string('\t', blocks.Count - 1), asm));
        }
    }
}
