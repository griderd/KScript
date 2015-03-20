using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KScript.Compilers.LanguageObjects;

namespace KScript.Compilers.IL
{
    class ILCompiler : Compiler
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
        Stack<List<string>> variableScope;
        List<Function> functions;

        Dictionary<string, string> types;
        string[] keywords = new string[] 
        {
            "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "string",
            "function", "endfunction", "if", "else", "endif", "while", "do"
        };

        public ILCompiler(FileInfo[] files, DirectoryInfo rootPath, DirectoryInfo outputPath, FileInfo outputFile, DirectoryInfo libraryPath)
            : base(files, rootPath, outputPath, outputFile, libraryPath, 
                new TokenizerOptions(true, new char[] { ';', '+', '-', '*', '/', '%', '.', ',', '{', '}', '(', ')', '<', '>', '=', '!'},
                                           new string[] { "++", "--", ">=", "<=", "!=", "==" } ), PreprocessorOptions.IncludeInsert)
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
            blocks.Push(BlockTypes.Global);
            variableScope = new Stack<List<string>>();
            variableScope.Push(new List<string>());
            functions = new List<Function>();
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
                            StartScope();
                            // TODO: Parse WHILE conditional
                        }
                        break;
                }
            }
        }

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

            assemblyCodeFile.Add(functionName + ":");
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

            functions.Add(new Function(functionName, parameterList, returnType));
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
            variableScope.Push(new List<string>());
            blocks.Push(block);
        }

        private void EndScope()
        {
            variableScope.Pop();
            blocks.Pop();
        }

        private bool AddToScope(string variable)
        {
            if (variableScope.Peek().Contains(variable))
                return false;
            variableScope.Peek().Add(variable);
            return true;
        }

        private bool HasVariable(string variable)
        {
            foreach (List<string> scope in variableScope)
            {
                if (scope.Contains(variable)) return true;
            }
            return false;
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
            throw new NotImplementedException();
        }

        protected override void Postprocess()
        {
            throw new NotImplementedException();
        }
    }
}
