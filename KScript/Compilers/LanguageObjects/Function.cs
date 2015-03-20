using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers.LanguageObjects
{
    struct Function
    {
        public string Name { get; private set; }
        public string[] ParamNames { get; private set; }
        public string[] ParamTypes { get; private set; }
        public string ReturnType { get; private set; }

        public Function(string name, Dictionary<string, string> parameters, string returnType)
            : this()
        {
            Name = name;
            ParamNames = parameters.Keys.ToArray();
            ParamTypes = parameters.Values.ToArray();
            ReturnType = returnType;
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
    }
}
