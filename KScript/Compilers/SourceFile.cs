using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers
{
    public class SourceFile
    {
        List<string> lines;

        public string[] Lines { get { return lines.ToArray(); } }

        public string FileName { get; private set; }

        public Token[] Tokens { get; private set; }

        TokenizerOptions options;

        public SourceFile(string[] lines, string filename, TokenizerOptions tokenizerOptions)
        {
            if ((lines == null) | (filename == null))
                throw new ArgumentNullException();
            if (lines.Contains(null))
                throw new NullReferenceException();

            FileName = filename;
            this.lines = new List<string>(lines);
            options = tokenizerOptions;
            Tokenize();
        }

        public void RemoveLine(int index)
        {
            lines.RemoveAt(index);
            Tokenize();
        }

        void Tokenize()
        {
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < Lines.Length; i++)
            {
                tokens.AddRange(TokenizeLine(i));
            }

            Tokens = tokens.ToArray();
        }

        Token[] TokenizeLine(int lineIndex)
        {
            if ((lineIndex < 0) | (lineIndex >= Lines.Length))
                throw new ArgumentOutOfRangeException();

            List<Token> tokens = new List<Token>();
            StringBuilder sb = new StringBuilder();

            Action<bool> AddToken = new Action<bool>(eol =>
            {
                if (sb.Length == 0) return;
                tokens.Add(new Token(sb.ToString(), eol, lineIndex, FileName));
                sb.Clear();
            });

            string line = Lines[lineIndex];

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                bool eol = i + 1 == line.Length;
                
                if ((char.IsWhiteSpace(c)) & (options.EndTokenOnWhitespace))
                {
                    AddToken(eol);
                    continue;
                }
                else if ((sb.Length > 0) && (options.MultiCharTokens.Contains(sb.ToString() + c)))
                {
                    sb.Append(c);
                    AddToken(eol);
                    continue;
                }
                else if (options.SingleCharTokens.Contains(c))
                {
                    AddToken(false);
                    sb.Append(c);
                    AddToken(eol);
                    continue;
                }

                sb.Append(c);

                if (options.MultiCharTokens.Contains(sb.ToString()))
                {
                    AddToken(eol);
                    continue;
                }
            }

            if (sb.Length > 0)
                AddToken(true);

            return tokens.ToArray();
        }
    }
}
