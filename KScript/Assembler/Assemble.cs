using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using KScript.Hardware;

namespace KScript.Assembler
{
    public class Assemble
    {
        public enum VariableSign
        {
            Default,
            Signed,
            Unsigned
        }

        struct SymbolInfo
        {
            public string Filename { get; private set; }
            public int LineNumber { get; private set; }
            public int PlaceHolderLocation { get; private set; }
            public bool IsLabel { get; private set; }

            public SymbolInfo(string filename, int lineNumber, int placeholderLocation, bool isLabel)
                : this()
            {
                Filename = filename;
                LineNumber = lineNumber;
                PlaceHolderLocation = placeholderLocation;
                IsLabel = isLabel;
            }
        }

        Dictionary<string, byte> instructions, registers;
        Dictionary<string, int> dataTypes;

        Queue<string> files;
        string currentFile;

        List<string> errors;
        List<string> warnings;
        List<byte> output;

        List<byte> dataSegment;
        List<byte> codeSegment;

        // These two represent placeholders and pointers for both variable/constant names and labels. 
        // This is so that function pointers, anonymous functions, and lambda expressions can be created and used easily.
        Dictionary<string, List<SymbolInfo>> symbolPlaceHolders;
        Dictionary<string, int> symbolPointers;

        string[] lines;
        int lineNumber;

        public string CurrentLine { get { return lines[lineNumber].Trim(); } }

        string[] tokens;
        int tokenIndex;

        public string CurrentToken { get { return tokens[tokenIndex]; } }

        public byte[] Result { get { return output.ToArray(); } }
        public string[] Errors { get { return errors.ToArray(); } }
        public string[] Warnings { get { return warnings.ToArray(); } }

        /// <summary>
        /// Loads all instructions and registers from the Instructions enumerator. Instructions can begin with underscore. 
        /// Registers must begin with "reg". Underscores and "reg" are removed from the final names. "_null" is a reserved word and is ignored.
        /// </summary>
        private void LoadInstructions()
        {
            instructions = new Dictionary<string, byte>();
            registers = new Dictionary<string, byte>();
            dataTypes = new Dictionary<string, int>();

            string[] instr = Enum.GetNames(typeof(Instructions));
            for (int i = 0; i < instr.Length; i++)
            {
                if (instr[i] == "_null") continue;

                if (instr[i].StartsWith("reg"))
                {
                    registers.Add(instr[i].Substring(3), (byte)i);
                }
                else
                {
                    instructions.Add(instr[i].StartsWith("_") ? instr[i].Substring(1) : instr[i], (byte)i);
                }
            }

            dataTypes.Add("byte", 1);
            dataTypes.Add("short", 2);
            dataTypes.Add("word", 4);
            dataTypes.Add("long", 8);
            dataTypes.Add("float", 4);
            dataTypes.Add("double", 8);
        }

        public Assemble(string code, string filename)
            : this(code.Split('\n'), filename)
        {
        }

        public Assemble(string[] code, string filename)
        {
            LoadInstructions();

            lines = code;
            files = new Queue<string>();
            currentFile = filename;

            errors = new List<string>();
            warnings = new List<string>();
            output = new List<byte>();
            dataSegment = new List<byte>();
            codeSegment = new List<byte>();
            symbolPlaceHolders = new Dictionary<string, List<SymbolInfo>>();
            symbolPointers = new Dictionary<string, int>();

            do
            {
                lineNumber = -1;
                Preprocessor();
                Asm();
                if (files.Count > 0)
                {
                    currentFile = files.Dequeue();
                    // Load file
                }

            }
            while (files.Count > 0);

            StaticLink();
            output.AddRange(codeSegment);
            output.AddRange(dataSegment);
        }

        private void Asm()
        {
            VariableSign signed = VariableSign.Default;

            while (GetNextLine())
            {
                if (LineIsLabel())
                {
                    AddLabelPointer();
                    continue;
                }

                GetNextToken();
                AddInstruction();

                bool isInstruction = true;
                switch (CurrentToken)
                {
                    case "inr":
                    case "outr":
                        break;

                    case "cpuid":
                    case "pushi":
                    case "outp":
                    case "inp":
                        AddLiteral();
                        break;

                    case "jtruer":
                    case "jfalser":
                    case "jmpr":
                    case "gotor":
                    case "gototruer":
                    case "gotofalser":
                    case "popi":
                    case "peeki":
                    case "pushr":
                    case "push":
                    case "pop":
                    case "incrr":
                    case "decrr":
                        AddRegister();
                        break;

                    case "jtrue":
                    case "jfalse":
                    case "jmp":
                    case "goto":
                    case "gototrue":
                    case "gotofalse":
                    case "inthnd":
                        AddLabelOrAddress();
                        break;

                    case "mov":
                        AddRegister();
                        AddLiteral();
                        break;

                    case "rmem":
                    case "movp":
                        AddRegister();
                        AddSymbolOrAddress();
                        break;
  
                    case "wvmem":
                    case "wmem":
                        AddSymbolOrAddress();
                        AddRegister();
                        break;

                    case "wmeml":
                        AddSymbolOrAddress();
                        AddLiteral();
                        break;

                    case "rmemr":
                    case "wmemr":
                    case "wvmemr":
                    case "movr":
                        AddRegister();
                        AddRegister();
                        break;

                    case "signed":
                        isInstruction = false;
                        signed = VariableSign.Signed;
                        if (!GetNextToken())
                            Error("Expected type.");
                        goto case "byte";

                    case "unsigned":
                        isInstruction = false;
                        signed = VariableSign.Unsigned;
                        if (!GetNextToken())
                            Error("Expected type.");
                        goto case "byte";
                        
                    case "byte":
                    case "short":
                    case "word":
                    case "long":
                    case "float":
                    case "double":
                        isInstruction = false;
                        AddVariable(signed);
                        break;
                }

                if (GetNextToken() & isInstruction) Error("Too many arguments!");
            }
        }

        private void StaticLink()
        {
            foreach (string symbol in symbolPlaceHolders.Keys)
            {
                foreach (SymbolInfo info in symbolPlaceHolders[symbol])
                {
                    if (!symbolPointers.ContainsKey(symbol))
                        LinkError(info.Filename, info.LineNumber, "Undefined symbol \"" + symbol + "\"");
                    else
                    {
                        int offset = info.IsLabel ? 0 : codeSegment.Count;
                        int address = offset + symbolPointers[symbol];
                        byte[] ptr = BitConverter.GetBytes(address);
                        for (int i = 0; i < 4; i++)
                        {
                            codeSegment[info.PlaceHolderLocation + i] = ptr[i];
                        }
                    }
                }
            }
        }

        private void Preprocessor()
        {
            foreach (string line in lines)
            {
                if (line.StartsWith("#include "))
                {
                    string filename = line.Substring("#include ".Length);
                    if (filename.StartsWith("\"") && filename.EndsWith("\""))
                        files.Enqueue(System.IO.Directory.GetCurrentDirectory() + "\\" + filename.Trim('\"'));
                    else if (filename.StartsWith("<") && filename.EndsWith(">"))
                        files.Enqueue(System.IO.Directory.GetCurrentDirectory() + "\\Library\\" + filename.Trim('<', '>'));
                    else
                        Warning("File name must be wrapped in quotes or angle brackets.");
                }
                else if (line.StartsWith("#define "))
                {
                    string identifier = line.Split(' ')[1];
                    string value = line.Substring(("#define " + identifier + " ").Length).Trim();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].Replace(identifier, value);
                    }
                }
            }
        }

        private void LinkError(string filename, int lineNumber, string message)
        {
            errors.Add(String.Format("{0}({1}) LNK: {2}", filename, lineNumber, message));
        }

        private void LinkError(string message)
        {
            errors.Add("LNK: " + message);
        }

        private void Error(string message)
        {
            errors.Add(String.Format("{0}({1}): {2}", currentFile, lineNumber + 1, message));
        }

        private void Warning(string message)
        {
            warnings.Add(String.Format("{0}({1}): {2}", currentFile, lineNumber + 1, message));
        }

        private void AddSymbolPlaceHolder(bool isLabel)
        {
            if (!symbolPlaceHolders.ContainsKey(CurrentToken))
                symbolPlaceHolders.Add(CurrentToken, new List<SymbolInfo>());

            symbolPlaceHolders[CurrentToken].Add(new SymbolInfo(currentFile, lineNumber + 1, codeSegment.Count, isLabel));
            codeSegment.AddRange(new byte[CPU.WORD_LENGTH]);
        }

        private void AddInstruction(Instructions instruction)
        {
            codeSegment.Add((byte)instruction);
        }

        private bool GetNextLine()
        {
            lineNumber++;
            if (lineNumber < lines.Length)
            {
                tokens = CurrentLine.Split(' ');
                tokenIndex = -1;

                if (!LineIsCommentOrEmpty()) return true;
                else return GetNextLine();
            }
            else
            {
                lineNumber--;
                return false;
            }
        }

        private bool LineIsCommentOrEmpty()
        {
            if (CurrentLine.StartsWith(";") | CurrentLine.StartsWith("#") | CurrentLine == "")
                return true;
            else
                return false;
        }

        private bool LineIsLabel()
        {
            return ((tokens.Length == 1) & (CurrentLine.EndsWith(":")));
        }

        private void AddLabelPointer()
        {
            if (!LineIsLabel())
            {
                Error("Internal error at 'void AddLabelPointer()'. Line is not a label.");
                return;
            }

            string name = CurrentLine.Substring(0, CurrentLine.Length - 1);

            if (symbolPointers.ContainsKey(name))
            {
                Error("Symbol \"" + name + "\" already exists.");
            }
            else
            {
                symbolPointers.Add(name, codeSegment.Count);
            }
        }

        private void AddSymbolPointer(string name)
        {
            if (!IsValidSymbolName(name))
            {
                Error("Symbol name invalid. Must start with a letter or underscore and must contain letters, numbers, and underscores.");
            }
            else if (symbolPointers.ContainsKey(name))
            {
                Error("Symbol \"" + name + "\" already exists.");
            }
            else
            {
                symbolPointers.Add(name, dataSegment.Count);
            }
        }

        private bool GetNextToken()
        {
            tokenIndex++;
            if (tokenIndex < tokens.Length)
            {
                return true;
            }
            else
            {
                tokenIndex--;
                return false;
            }
        }

        private void AddInstruction()
        {
            if (dataTypes.ContainsKey(CurrentToken)) return;
            if ((CurrentToken == "unsigned") | (CurrentToken == "signed")) return;

            if (!instructions.ContainsKey(CurrentToken))
                Error("Token is not an instruction.");
            else
                codeSegment.Add(instructions[CurrentToken]);
        }

        /// <summary>
        /// Interprets the next token as a literal and adds it to the code segment.
        /// </summary>
        private void AddLiteral()
        {
            if (!GetNextToken())
            {
                Error("Literal expected.");
                return;
            }

            int value = 0;
            if (!int.TryParse(CurrentToken, out value))
            {
                Error("Could not parse value as word literal.");
                return;
            }

            codeSegment.AddRange(BitConverter.GetBytes(value));
        }

        private void AddRegister()
        {
            if (!GetNextToken())
                Error("Register expected.");
            else if (!registers.ContainsKey(CurrentToken))
                Error("Register name not valid or is reserved.");
            else
                codeSegment.AddRange(BitConverter.GetBytes((int)registers[CurrentToken]));
        }

        private void AddLabel()
        {
            if (!GetNextToken())
            {
                Error("Label expected.");
                return;
            }

            if (symbolPointers.ContainsKey(CurrentToken))
            {
                // If the label exists, add the address the label represents.
                codeSegment.AddRange(BitConverter.GetBytes(symbolPointers[CurrentToken]));
            }
            else
            {
                // Otherwise, mark where this label reference exists and insert placeholder address.
                AddSymbolPlaceHolder(true);
            }
        }

        private void AddSymbol()
        {
            if (!GetNextToken())
            {
                Error("Symbol expected.");
                return;
            }

            // We have to add a placeholder because the data segment goes later. We don't know where this will actually point yet.
            AddSymbolPlaceHolder(false);
        }

        private void AddAddress()
        {
            int address = 0;
            if (!GetNextToken())
            {
                Error("Address expected.");
            }
            else if (!CurrentToken.StartsWith("@"))
            {
                Error("Address expected to start with \"@\".");
            }
            else if (!int.TryParse(CurrentToken.Substring(1), out address))
            {
                Error("Invalid address.");
            }
            else if ((address < 0) | (address > 0xFFFFFF))
            {
                Error("Address is out of 24-bit addressing range.");
            }
            else
            {
                codeSegment.AddRange(BitConverter.GetBytes(address));
            }
        }

        private void AddLabelOrAddress()
        {
            if (!GetNextToken())
            {
                Error("Address or label expected.");
            }
            else if (CurrentToken.StartsWith("@"))
            {
                tokenIndex--;
                AddAddress();
            }
            else
            {
                tokenIndex--;
                AddLabel();
            }
        }

        private void AddSymbolOrAddress()
        {
            if (!GetNextToken())
            {
                Error("Address or label expected.");
            }
            else if (CurrentToken.StartsWith("@"))
            {
                tokenIndex--;
                AddAddress();
            }
            else
            {
                tokenIndex--;
                AddSymbol();
            }
        }

        /// <summary>
        /// Parses a variable declaration, then adds the result to the data segment, and creates a symbol record.
        /// </summary>
        /// <param name="signed">Determines if the variable is signed, unsigned, or uses its default sign.</param>
        private void AddVariable(VariableSign signed)
        {
            string type = CurrentToken;
            string symbol = "";
            
            if (!dataTypes.ContainsKey(type))
            {
                Error("Invalid type.");
                return;
            }

            if (((type == "float") | (type == "double")) & (signed == VariableSign.Unsigned))
            {
                Error("Cannot have an unsigned floating-point.");
                return;
            }

            int length = dataTypes[CurrentToken];

            if (!GetNextToken())
            {
                Error("Symbol or \"array\" expected.");
                return;
            }

            symbol = CurrentToken;

            if (CurrentToken == "array")
            {
                AddArray(type, length, signed);
                return;
            }
            else if (!IsValidSymbolName(symbol))
            {
                Error("Symbol name invalid. Must start with a letter or underscore and must contain only letters, numbers, and underscores.");
                return;
            }
            else
            {
                AddSymbolPointer(symbol);
            }

            if (!GetNextToken())
            {
                Error(string.Format("Variable {0} value not set. Initializing to zero.", symbol));
                return;
            }
            else
            {
                dataSegment.AddRange(GetElement(type, signed));
            }

        }

        /// <summary>
        /// Parses an array from a variable declaration, adds it to the data segment, and adds a symbol.
        /// </summary>
        /// <param name="type">Type of array.</param>
        /// <param name="typeSize">Size of the type.</param>
        /// <param name="signed">Determines whether the type should be signed, unsigned, or use its default sign.</param>
        private void AddArray(string type, int typeSize, VariableSign signed)
        {
            int elementCount = 0;
            int length = 0;
            string name;

            if (!GetNextToken())
            {
                Error("Element count or symbol expected.");
                return;
            }
            else if (int.TryParse(CurrentToken, out elementCount))
            {
                length = typeSize * elementCount;
                if (!GetNextToken())
                {
                    Error("Symbol expected.");
                    return;
                }
            }

            if (!IsValidSymbolName())
            {
                Error("Symbol name invalid. Must start with a letter or underscore and must only contain letters, numbers, and underscores.");
                return;
            }
            else
            {
                name = CurrentToken;
                AddSymbolPointer(name);
            }

            if (!GetNextToken())
            {
                Warning("Value undefined. Values are automatically set to zero.");
                dataSegment.AddRange(new byte[length]);
                return;
            }
            else if (CurrentToken.StartsWith("\""))
            {
                dataSegment.AddRange(GetString());
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    if (CurrentToken.StartsWith("\'") && CurrentToken.EndsWith("\'"))
                        dataSegment.AddRange(GetChar());
                    else
                        dataSegment.AddRange(GetElement(type, signed));
                }
            }
        }

        private byte[] GetElement(string type, VariableSign signed)
        {
            if (signed == VariableSign.Default)
            {
                switch (type)
                {
                    case "byte":
                        return GetByte();
                        
                    case "short":
                        return GetShort();
                        
                    case "word":
                        return GetInt();

                    case "long":
                        return GetLong();

                    case "float":
                        return GetFloat();

                    case "double":
                        return GetDouble();
                }
            }
            else if (signed == VariableSign.Signed)
            {
                switch (type)
                {
                    case "byte":
                        return GetSByte();

                    case "short":
                        return GetShort();

                    case "word":
                        return GetInt();

                    case "long":
                        return GetLong();

                    case "float":
                        return GetFloat();

                    case "double":
                        return GetDouble();
                }
            }
            else if (signed == VariableSign.Unsigned)
            {
                switch (type)
                {
                    case "byte":
                        return GetByte();

                    case "short":
                        return GetUShort();

                    case "word":
                        return GetUInt();

                    case "long":
                        return GetULong();
                }
            }

            return new byte[0];
        }

        private byte[] GetChar()
        {
            return Encoding.UTF8.GetBytes(CurrentToken.Trim('\''));
        }

        private byte[] GetByte()
        {
            byte val;
            if (!byte.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return new byte[] { val };
        }

        private byte[] GetSByte()
        {
            sbyte val;
            if (!sbyte.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return new byte[] { (byte)val };
        }

        private byte[] GetShort()
        {
            short val;
            if (!short.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetUShort()
        {
            ushort val;
            if (!ushort.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetInt()
        {
            int val;
            if (!int.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetUInt()
        {
            uint val;
            if (!uint.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetLong()
        {
            long val;
            if (!long.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetULong()
        {
            ulong val;
            if (!ulong.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetFloat()
        {
            float val;
            if (!float.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetDouble()
        {
            double val;
            if (!double.TryParse(CurrentToken, out val))
            {
                Error("Cannot parse value.");
                return new byte[0];
            }
            else
                return BitConverter.GetBytes(val);
        }

        private byte[] GetString()
        {
            int start = CurrentLine.IndexOf('\"') + 1;
            int length = CurrentLine.LastIndexOf('\"') - start;
            
            string str = CurrentLine.Substring(start, length);

            str = str.Replace("\\a", "\a");
            str = str.Replace("\\b", "\b");
            str = str.Replace("\\f", "\f");
            str = str.Replace("\\n", "\n");
            str = str.Replace("\\r", "\r");
            str = str.Replace("\\v", "\v");
            str = str.Replace("\\'", "\'");
            str = str.Replace("\\\"", "\"");
            str = str.Replace(@"\\", "\\");
            str = str.Replace("\\0", "\0");

            return Encoding.UTF8.GetBytes(str);
        }

        private bool IsValidLabelName()
        {
            return IsValidSymbolName(CurrentToken.Substring(0, CurrentToken.Length - 1));
        }

        private bool IsValidSymbolName()
        {
            return IsValidSymbolName(CurrentToken);
        }

        private bool IsValidSymbolName(string name)
        {
            return Regex.IsMatch(name, "^[a-zA-Z_]+[a-zA-Z0-9]*$");
        }
    }
}
