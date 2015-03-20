using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KScript.Compilers;
using System.IO;
using System.Text.RegularExpressions;

namespace KScript.Assembler
{
    /// <summary>
    /// Represents a dual-pass assembler. The first pass translates the code into "classical" assembly, which is then assembled normally.
    /// </summary>
    public class SmartAssembler : Compiler
    {
        /// <summary>
        /// A dictionary of variables. The key is the variable name, the value is whether the variable is constant. 
        /// Use this dictionary to avoid creating duplicate variables or overwriting constant values.
        /// </summary>
        Dictionary<string, bool> symbols;

        Dictionary<string, string> macros;

        string[] variableKeywords = new string[]
        {
            "signed",
            "unsigned",
            "byte",
            "short",
            "word",
            "long",
            "float",
            "double",
            "string",
            "const"
        };

        string[] registers = new string[] { "a", "b", "c", "d", "e", "f" };

        public SmartAssembler(FileInfo[] files, DirectoryInfo rootPath, DirectoryInfo outputPath, FileInfo outputFile, DirectoryInfo stdlibPath)
            : base(files, rootPath, outputPath, outputFile, stdlibPath,
              new TokenizerOptions(true,
                  new char[] { ',', '{', '}', '[', ']', '\n' }, new string[0]), PreprocessorOptions.IncludeAppend)
        {
            macros = new Dictionary<string, string>();
            symbols = new Dictionary<string, bool>();
        }

        public void Assemble()
        {
            Compile();
        }

        protected override void Preprocess()
        {
            for (int i = 0; i < code.Lines.Length; i++)
            {
                string line = code.Lines[i].Trim();

                if (!line.StartsWith("#")) continue;

                string[] parts = line.Split(' ');

                switch (parts[0])
                {
                    case "#include":
                        if ((parts.Length < 2) || (parts[1] == ""))
                        {
                            PreprocessorError("#INCLUDE directive requires filename.");
                            continue;
                        }

                        IncludeFile(parts[1], false);
                        break;

                    case "#define":
                        if ((parts.Length < 2) || (parts[1] == ""))
                        {
                            PreprocessorError("#DEFINE directive requires a token.");
                            continue;
                        }

                        string replacementString = "";
                        if (parts.Length > 2) replacementString = line.Substring(parts[1].Length + 9, line.Length - (parts.Length + 9));

                        macros.Add(parts[1], replacementString);
                        break;
                }

                code.RemoveLine(i);
                i--;
            }
        }

        private bool IsValidSymbolName()
        {
            return IsValidSymbolName(CurrentToken);
        }

        private bool IsValidSymbolName(string name)
        {
            return Regex.IsMatch(name, "^[a-zA-Z_]+[a-zA-Z0-9]*$");
        }

        private bool IsLiteral()
        {
            int temp;
            double ftemp;

            return int.TryParse(CurrentToken, out temp) | double.TryParse(CurrentToken, out ftemp);
        }

        private bool IsRegister()
        {
            return registers.Contains(CurrentToken);
        }

        protected override void Compile()
        {
            while (GetNextToken())
            {
                if (variableKeywords.Contains(CurrentToken.Value))
                    AddVariable();
                else if (CurrentToken.Value.EndsWith(":"))
                    AddLabel();
                else
                    AddInstruction();
            }
        }

        protected void AddLabel()
        {
            string labelName = CurrentToken.Value.Substring(0, CurrentToken.Value.Length - 1);
            if (symbols.ContainsKey(labelName))
            {
                Error(string.Format("The identifier \"{0}\" already exists.", labelName));
                return;
            }
            else
            {
                symbols.Add(labelName, true);
                assemblyCodeFile.Add(labelName + ':');
            }
        }

        private void AddVariable()
        {
            string[] signedTypes = new string[] { "short", "word", "long" };
            string[] unsignedTypes = new string[] { "byte" };
            string[] defaultTypes = new string[] { "float", "double", "string" };

            List<string> elements = new List<string>();

            bool isString = false;
            bool constant = false;
            string type = "";
            string name;
            bool array = false;
            int elementCount = 0;

            StringBuilder value = new StringBuilder();

            Assembler.Assemble.VariableSign sign = Assembler.Assemble.VariableSign.Default;

            // Check for constant
            if (CurrentToken.Value == "const")
            {
                constant = true;
                if (!GetNextToken())
                {
                    Error("Expected type.");
                    return;
                }
            }
            
            // Check for signedness or get type
            if (CurrentToken.Value == "signed")
                sign = Assembler.Assemble.VariableSign.Signed;
            else if (CurrentToken.Value == "unsigned")
                sign = Assembler.Assemble.VariableSign.Unsigned;
            else
                type = CurrentToken;

            // Get type
            if (type == "")
            {
                if (!GetNextToken())
                {
                    Error("Type expected.");
                    return;
                }

                if ((signedTypes.Contains(CurrentToken)) |
                    (unsignedTypes.Contains(CurrentToken)) |
                    (defaultTypes.Contains(CurrentToken)))
                {
                    type = CurrentToken;
                }

                if ((defaultTypes.Contains(type)) & (sign != Assembler.Assemble.VariableSign.Default))
                {
                    Warning(String.Format("Type \"{0}\" cannot be given signedness. Resetting signedness to default.", type));
                    sign = Assembler.Assemble.VariableSign.Default;
                }

                // Apply "string" macro
                if (type == "string")
                {
                    isString = true;
                    sign = Assembler.Assemble.VariableSign.Unsigned;
                    type = "byte";
                    array = true;
                }
            }

            // Get variable name or array index character.
            if (!GetNextToken())
            {
                Error("Variable name or '[' expected.");
                return;
            }
            

            if (CurrentToken.Value == "[")
            {
                array = true;

                if (!GetNextToken())
                {
                    Error("Integer literal or ']' expected.");
                    return;
                }

                if (CurrentToken.Value == "]")
                {
                    Warning("No array length specified. The number of elements will be determined from the element list.");
                    return;
                }
                else if (!int.TryParse(CurrentToken, out elementCount))
                {
                    Error("Integer literal or ']' expected.");
                    return;
                }
                else if (elementCount < 1)
                {
                    Error("Element count cannot be less than one. It makes no sense to have a static array of less than one element!");
                    return;
                }
                else if (!GetNextToken())
                {
                    Error("Expected ']'");
                    return;
                }
                else if (CurrentToken != "]")
                {
                    Error("Expected ']'");
                    return;
                }

                if (!GetNextToken())
                {
                    Error("Expected variable name.");
                }
            }

            if (!IsValidSymbolName())
            {
                Error("Variable name is not a valid symbol name.");
                return;
            }
            else if (symbols.ContainsKey(CurrentToken))
            {
                Error(String.Format("The identifier \"{0}\" already exists.", CurrentToken.Value));
                return;
            }
            else
            {
                name = CurrentToken;
            }

            if (!GetNextToken())
            {
                Error("Value or ';' expected.");
                return;
            }

            if (CurrentToken.Value == ";")
            {
                if (!constant)
                    Warning(string.Format("Variable '{0}' not assigned a value. A default value will be assigned.", name));
                else
                    Error(string.Format("Unassigned value to constant variable '{0}'.", name));

                return;
            }
            else if (CurrentToken.Value == "{")
            {
                if (!array)
                {
                    Error(string.Format("Variable '{0}' is not an array and cannot be assigned an array value.", name));
                    return;
                }

                bool onElement = true;

                while (GetNextToken() && CurrentToken.Value != "}")
                {
                    if (CurrentToken == ";")
                    {
                        Error("'}' expected.");
                        return;
                    }
                    else if (!onElement && CurrentToken != ",")
                    {
                        Error("',' expected.");
                        return;
                    }
                    else if (!onElement && CurrentToken == ",")
                    {
                        onElement = true;
                    }
                    else
                    {
                        elements.Add(CurrentToken);
                        onElement = false;
                    }
                }

                if ((elementCount != 0) & (elementCount != elements.Count))
                {
                    Error("The number of elements in the array does not match the number of elements expected.");
                    return;
                }
            }
            else if (CurrentToken.Value.StartsWith("\"") & CurrentToken.Value.EndsWith("\""))
            {
                if (!isString)
                {
                    Error("Cannot assign a string to a non-string value.");
                    return;
                }

                if ((elementCount == 0) |
                    (elementCount == CurrentToken.Value.Length - CurrentToken.Value.Count(new Func<char, bool>(x => { return (x == '\\'); })) - 2))
                {
                    elements.Add(CurrentToken);
                }
                else
                {
                    Error("String length does not match value length.");
                    return;
                }
            }
            else
            {
                elements.Add(CurrentToken);
            }

            if ((!GetNextToken()) || (CurrentToken != ";"))
            {
                Error("Expected ';'.");
                return;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                value.Append(elements[i]);

                if (i < elements.Count - 2)
                    value.Append(' ');
            }


            StringBuilder line = new StringBuilder();

            if (sign == Assembler.Assemble.VariableSign.Signed)
                line.Append("signed ");
            else if (sign == Assembler.Assemble.VariableSign.Unsigned)
                line.Append("unsigned ");

            line.Append(type);
            line.Append(' ');
            if (array) line.Append("array ");
            if (elementCount > 0) { line.Append(elementCount); line.Append(' '); }
            line.Append(name);
            line.Append(' ');
            line.Append(value.ToString());

            assemblyCodeFile.Add(line.ToString());
            symbols.Add(name, constant);
        }

        private void AddInstruction()
        {
            switch (CurrentToken.Value)
            {
                case "popi":
                    if ((!GetNextToken()) || (CurrentToken.Value == "\n"))
                        assemblyCodeFile.Add("popx");
                    else if (!IsRegister())
                    {
                        Error("Expected register or EOL.");
                        return;
                    }
                    else
                        assemblyCodeFile.Add("popi " + CurrentToken.Value);
                    break;

                case "jmp":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("jmpr " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("jmp " + CurrentToken.Value);
                    }
                    break;

                case "jtrue":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("jtruer " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("jtrue " + CurrentToken.Value);
                    }
                    break;

                case "jfalse":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("jfalser " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("jfalse " + CurrentToken.Value);
                    }
                    break;

                case "goto":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("gotor " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("goto " + CurrentToken.Value);
                    }
                    break;

                case "gototrue":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("gototruer " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("gototrue " + CurrentToken.Value);
                    }
                    break;

                case "gotofalse":
                    if (!GetNextToken())
                    {
                        Error("Expected symbol, address, or register.");
                        return;
                    }
                    else if (IsRegister())
                    {
                        assemblyCodeFile.Add("gotofalser " + CurrentToken.Value);
                    }
                    else
                    {
                        assemblyCodeFile.Add("gotofalse " + CurrentToken.Value);
                    }
                    break;

                case "pop":
                    if ((!GetNextToken()) || (CurrentToken.Value == "\n"))
                        assemblyCodeFile.Add("poprx");
                    else if (!IsRegister())
                    {
                        Error("Expected register or EOL.");
                        return;
                    }
                    else
                        assemblyCodeFile.Add("pop " + CurrentToken.Value);
                    break;

                case "mov":
                    {
                        string arg1;

                        if (!GetNextToken())
                        {
                            Error("Expected register.");
                            return;
                        }
                        else if (!IsRegister())
                        {
                            Error(string.Format("\"{0}\" is not a register.", CurrentToken.Value));
                            return;
                        }
                        else
                        {
                            arg1 = CurrentToken.Value;
                        }

                        if (!GetNextToken())
                        {
                            Error("Expected register, symbol, or address.");
                            return;
                        }
                        else if (IsRegister())
                        {
                            assemblyCodeFile.Add(string.Format("movr {0} {1}", arg1, CurrentToken.Value));
                        }
                        else if (IsLiteral())
                        {
                            assemblyCodeFile.Add(string.Format("mov {0} {1}", arg1, CurrentToken.Value));
                        }
                        else
                        {
                            assemblyCodeFile.Add(string.Format("movp {0} {1}", arg1, CurrentToken.Value));
                        }
                    }
                    break;

                case "rmem":
                    {
                        string arg1;

                        if (!GetNextToken())
                        {
                            Error("Expected register.");
                            return;
                        }
                        else if (!IsRegister())
                        {
                            Error(string.Format("\"{0}\" is not a register.", CurrentToken.Value));
                            return;
                        }
                        else
                        {
                            arg1 = CurrentToken.Value;
                        }

                        if (!GetNextToken())
                        {
                            Error("Expected register, symbol, or address.");
                            return;
                        }
                        else if (IsRegister())
                        {
                            assemblyCodeFile.Add(string.Format("rmemr {0} {1}", arg1, CurrentToken.Value));
                        }
                        else
                        {
                            assemblyCodeFile.Add(string.Format("rmem {0} {1}", arg1, CurrentToken.Value));
                        }
                    }
                    break;

            }
        }

        protected override void Postprocess()
        {
            //throw new NotImplementedException();
        }
    }
}
