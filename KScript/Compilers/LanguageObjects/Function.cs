using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers.LanguageObjects
{
    class Function
    {
        public string Name { get; private set; }
        public string AsmName { get; private set; }
        public string[] ParamNames { get; private set; }
        public string[] ParamTypes { get; private set; }
        public string ReturnType { get; private set; }
        public string Signature { get; private set; }

        Dictionary<string, string> types;

        public Function(string name, Dictionary<string, string> parameters, string returnType, Dictionary<string, string> languageTypes)
        {
            Name = name;
            ParamNames = parameters.Keys.ToArray();
            ParamTypes = parameters.Values.ToArray();
            ReturnType = returnType;
            Signature = ToString();
            
            GenerateUniqueName();

            types = languageTypes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append('(');

            for (int i = 0; i < ParamTypes.Length; i++)
            {
                sb.Append(ParamTypes[i]);
                if (i + 1 < ParamTypes.Length)
                    sb.Append(',');
            }
            sb.Append("):");
            sb.Append(ReturnType);

            return sb.ToString();
        }

        public string[] GenerateAsmCode()
        {
            List<string> asm = new List<string>();

            StringBuilder line = new StringBuilder();

            line.Append(AsmName);
            line.Append(':');
            asm.Add(line.ToString());
            line.Clear();

            for (int i = 0; i < ParamNames.Length; i++)
            {
                string name = ParamNames[i];
                string type = ParamTypes[i];

                if (!types.ContainsKey(type))
                    throw new Exception("The type \"" + type + "\" is not a fundamental type and cannot be translated into assembly. If your language uses user-generated types, you should translate into the Intermediate Language first.");
                else
                {
                    line.Append(types[type]);
                    line.Append(' ');
                    line.Append(AsmName);
                    line.Append('_');
                    line.Append(name);
                    asm.Add(line.ToString());
                    line.Clear();
                }
            }

            for (int i = ParamNames.Length - 1; i >= 0; i--)
            {
                asm.Add("pop a");
                line.Append("wmem ");
                line.Append(AsmName);
                line.Append('_');
                line.Append(ParamNames[i]);
                asm.Add(line.ToString());
                asm.Clear();
            }


            return asm.ToArray();
        }

        private void GenerateUniqueName()
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append('_');
            sb.Append(rnd.Next());

            AsmName = sb.ToString();
        }
    }
}
