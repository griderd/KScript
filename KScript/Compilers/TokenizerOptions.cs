using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers
{
    public class TokenizerOptions
    {
        public bool EndTokenOnWhitespace { get; private set; }
        public char[] SingleCharTokens { get; private set; }
        public string[] MultiCharTokens { get; private set; }

        public TokenizerOptions(bool endTokenOnWhitespace, char[] singleCharTokens, string[] multiCharTokens)
        {
            if ((singleCharTokens == null) | (multiCharTokens == null))
                throw new ArgumentNullException();
            if (multiCharTokens.Contains(null))
                throw new NullReferenceException();

            EndTokenOnWhitespace = endTokenOnWhitespace;
            SingleCharTokens = singleCharTokens;
            MultiCharTokens = multiCharTokens;
        }
    }
}
