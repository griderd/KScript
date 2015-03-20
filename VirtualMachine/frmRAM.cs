using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KScript.Hardware;
using System.IO;

namespace VirtualMachine
{
    public partial class frmRAM : Form
    {
        RAM memory;
        CPU cpu;

        public frmRAM(RAM memory, CPU cpu)
        {
            InitializeComponent();
            this.memory = memory;
            this.cpu = cpu;
            txtHexView.HideSelection = false;
        }

        private void frmRAM_Load(object sender, EventArgs e)
        {
            RefreshText();
        }

        public void RefreshText()
        {
            this.Text = "RAM - " + memory.Length.ToString() + " bytes (" + (memory.Length / 1024).ToString() + " KB)";
            int select = 0;
            int selLength = 0;

            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < memory.Length; i++)
            {
                if (i % 16 == 0)
                    hex.Append(i.ToString("X3"));
                hex.Append("  ");

                if (cpu.ProgramCounter == i)
                    select = hex.Length;

                if (chkHexDigits.Checked)
                {
                    hex.Append(((int)memory.Data[i]).ToString("X2"));
                    selLength = 2;
                }
                else
                {
                    hex.Append(' ');
                    hex.Append(memory.Data[i]);
                }

                if ((i + 1) % 16 == 0)
                    hex.AppendLine();
            }
            txtHexView.Text = hex.ToString();
            txtHexView.Select(select, selLength);
        }

        private void chkHexDigits_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void chkHexDigits_CheckedChanged_1(object sender, EventArgs e)
        {
            RefreshText();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshText();
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes("ram.bin", memory.Data);
        }
    }
}
