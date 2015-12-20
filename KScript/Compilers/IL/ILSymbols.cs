using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers.IL
{
    public partial class ILCompiler : Compiler
    {
        string GenerateUniqueVariableName(string commonName)
        {
            Random rnd = new Random();

            StringBuilder sb = new StringBuilder();

            if (blocks.Peek() != BlockTypes.Global)
            {
                sb.Append(functions.Last().AsmName);
                sb.Append('_');
            }
            sb.Append(commonName);
            sb.Append('_');
            sb.Append(rnd.Next());

            return sb.ToString();
        }

        private bool HasVariable(string commonName)
        {
            foreach (Dictionary<string, string> scope in variableScope)
            {
                if (scope.ContainsKey(commonName)) return true;
            }
            return false;
        }

        private string GetUniqueVariable(string commonName)
        {
            foreach (Dictionary<string, string> scope in variableScope)
            {
                if (scope.ContainsKey(commonName))
                    return scope[commonName];
            }
            return null;
        }
    }
}
