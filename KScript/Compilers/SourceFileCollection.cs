using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Compilers
{
    public class SourceFileCollection
    {
        List<SourceFile> files;

        public SourceFile[] Files { get { return files.ToArray(); } }

        public Token[] Tokens { get; private set;}

        public SourceFileCollection(SourceFile[] files)
        {
            if (files == null)
                throw new ArgumentNullException();
            if (files.Contains(null))
                throw new NullReferenceException();

            this.files = new List<SourceFile>(files);

            BuildTokenArray();
        }

        public SourceFileCollection(FileInfo[] files, TokenizerOptions options)
        {
            if (files == null)
                throw new ArgumentNullException();
            if (files.Contains(null))
                throw new NullReferenceException();

            this.files = new List<SourceFile>();

            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Exists)
                    throw new FileNotFoundException();

                this.files.Add(new SourceFile(File.ReadAllLines(files[i].FullName), files[i].Name, options));
            }

            BuildTokenArray();
        }

        void BuildTokenArray()
        {
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < files.Count; i++)
            {
                tokens.AddRange(files[i].Tokens);
            }

            Tokens = tokens.ToArray();
        }

        public void AddFile(FileInfo file, TokenizerOptions options)
        {
            if (file == null)
                throw new ArgumentNullException();

            if (!file.Exists)
                throw new FileNotFoundException();

            this.files.Add(new SourceFile(File.ReadAllLines(file.FullName), file.Name, options));
        }

        public void InsertFile(FileInfo file, int index, TokenizerOptions options)
        {
            if (file == null)
                throw new ArgumentNullException();

            if (!file.Exists)
                throw new FileNotFoundException();

            if ((index < 0) | (index > files.Count))
                throw new ArgumentOutOfRangeException();

            this.files.Insert(index, new SourceFile(File.ReadAllLines(file.FullName), file.Name, options));
        }
    }
}
