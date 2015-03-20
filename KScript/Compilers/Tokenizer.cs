using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Compilers
{
    class Tokenizer
    {
        List<string> tokens;
        DebugInfoCollection debugInfo;
        StringBuilder token;

        public string[] Tokens { get { return tokens.ToArray(); } }
        

        public Tokenizer(string sourceCode, TokenizerOptions options)
        {
            tokens = new List<string>();
            debugInfo = new DebugInfoCollection();           

            token = new StringBuilder();

            // TODO: add support for DebugInfo
            for (int i = 0; i < sourceCode.Length; i++)
            {
                char c = sourceCode[i];
                if (((c == '\n') | (c == ' ') | (c == '\t')) & (options.endTokenAtWhitespace))
                {
                    AddToken();
                    continue;
                }
                else if ((token.Length > 0) && (options.multiCharTokens.Contains(token.ToString() + c)))
                {
                    token.Append(c);
                    AddToken();
                    continue;
                }
                else if (options.singleCharTokens.Contains(c))
                {
                    AddToken();
                    continue;
                }

                token.Append(c);

                if (options.multiCharTokens.Contains(token.ToString()))
                {
                    AddToken();
                    continue;
                }
            }

            if (token.Length > 0)
                AddToken();
        }

        void AddToken()
        {
            if (token.Length > 0)
            {
                tokens.Add(token.ToString());
                token.Clear();
            }
        }
    }

    public struct TokenizerOptions
    {
        public bool endTokenAtWhitespace;
        public char[] singleCharTokens;
        public string[] multiCharTokens;

        public TokenizerOptions(bool endTokenAtWhitespace, char[] singleCharTokens, string[] multiCharTokens)
        {
            this.endTokenAtWhitespace = endTokenAtWhitespace;
            this.singleCharTokens = singleCharTokens;
            this.multiCharTokens = multiCharTokens;
        }
    }
}
