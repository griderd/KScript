using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KScript.Compilers
{
    public class RawCode
    {
        List<string> lines;
        List<string> errors;

        public string[] Lines { get { return lines.ToArray(); } }
        public string[] Errors { get { return errors.ToArray(); } }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (string s in lines)
                {
                    sb.AppendLine(s);
                }

                return sb.ToString();
            }
        }

        public void ClearErrors()
        {
            errors.Clear();
        }

        public RawCode()
        {
            lines = new List<string>();
            errors = new List<string>();
        }

        public RawCode(FileInfo[] files)
            : this()
        {
            if (files == null) throw new ArgumentNullException();

            foreach (FileInfo file in files)
            {
                if (file == null) throw new NullReferenceException();
                AddFile(file);
            }
        }

        public RawCode(string[][] files)
            : this()
        {
            if (files == null) throw new ArgumentNullException();

            foreach (string[] file in files)
            {
                if (file == null) throw new NullReferenceException();
                AddFile(file);
            }
        }


        public void AddFile(string[] file)
        {
            if (file == null) throw new ArgumentNullException();
            lines.AddRange(file);
        }

        public bool AddFile(FileInfo file)
        {
            string[] lines;

            if (!GetFile(file, out lines))
            {
                return false;
            }
            else
            {
                this.lines.AddRange(lines);
                return true;
            }
        }

        public void Replace(string originalString, string replacementString)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Replace(originalString, replacementString);
            }
        }

        public void InsertFile(string[] file, int index)
        {
            if (file == null) throw new ArgumentNullException();
            if ((index < 0) | (index >= lines.Count)) throw new ArgumentOutOfRangeException();

            lines.InsertRange(index, file);
        }

        public bool InsertFile(FileInfo file, int index)
        {
            string[] lines;

            if (!GetFile(file, out lines))
            {
                return false;
            }
            else
            {
                InsertFile(lines, index);
                return true;
            }
        }

        public bool RemoveLine(int index)
        {
            if ((index < 0) | (index >= lines.Count)) return false;

            lines.RemoveAt(index);
            return true;
        }

        private bool GetFile(FileInfo file, out string[] lines)
        {
            if (file == null) throw new ArgumentNullException();
            lines = null;

            if (file.Exists)
            {
                try
                {
                    lines = File.ReadAllLines(file.FullName);
                }
                catch (DirectoryNotFoundException)
                {
                    Error("Directory not found.", file);
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    Error("The user does not have permission to open the file, or the file is read-only.", file);
                    return false;
                }
                catch (FileNotFoundException)
                {
                    Error("The file in the specified path was not found.", file);
                    return false;
                }
                catch (System.Security.SecurityException)
                {
                    Error("The user does not have permission to open this file.", file);
                    return false;
                }
                catch (IOException)
                {
                    Error("An I/O error occurred while opening the file.", file);
                    return false;
                }
                return true;
            }
            else
            {
                Error("The file in the specified path was not found.", file);
                return false;
            }
        }

        private void Error(string message, FileInfo file)
        {
            errors.Add(message + " (" + file.FullName + ")");
        }
    }
}
