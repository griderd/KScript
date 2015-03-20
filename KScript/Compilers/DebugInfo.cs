using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KScript.Compilers
{
    public struct DebugInfoItem
    {
        string filename;
        int lineNumber;
        string value;

        public string Filename { get { return filename; } }
        public int LineNumber { get { return lineNumber; } }
        public string Value { get { return value; } }

        public DebugInfoItem(string value, string filename, int lineNumber)
        {
            this.filename = filename;
            this.lineNumber = lineNumber;
            this.value = value;
        }

        public static implicit operator string(DebugInfoItem x)
        {
            return x.Value;
        }
    }

    public class DebugInfoCollection : ICollection<DebugInfoItem>
    {
        List<DebugInfoItem> items;

        public DebugInfoItem this[string value]
        {
            get
            {
                foreach (DebugInfoItem item in items)
                {
                    if (item.Value == value) return item;
                }
                return new DebugInfoItem();
            }
        }

        public DebugInfoItem this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public DebugInfoCollection()
        {
            items = new List<DebugInfoItem>();
        }

        public void Add(DebugInfoItem item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(DebugInfoItem item)
        {
            return items.Contains(item);
        }

        public bool Contains(string value)
        {
            foreach (DebugInfoItem item in items)
            {
                if (item.Value == value) return true;
            }
            return false;
        }

        public void CopyTo(DebugInfoItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(DebugInfoItem item)
        {
            return items.Remove(item);
        }

        public IEnumerator<DebugInfoItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
