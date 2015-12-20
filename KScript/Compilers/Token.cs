using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers
{
    public struct Token
    {
        /// <summary>
        /// Gets the string value of the token.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the file line number of the token.
        /// </summary>
        public int LineNumber { get { return LineIndex + 1; } }

        /// <summary>
        /// Gets the zero-based line index of the token.
        /// </summary>
        public int LineIndex { get; private set; }

        /// <summary>
        /// Gets the file the token is located in.
        /// </summary>
        public string Filename { get; private set; }

        public Token(string value, int lineIndex, string filename)
            : this()
        {
            Filename = filename;
            LineIndex = lineIndex;
            Value = value;
        }

        public static implicit operator string(Token t)
        {
            return t.Value;
        }
    }
}
