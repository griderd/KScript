using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KScript.Hardware;

namespace VirtualMachine
{
    public partial class frmALU : Form
    {
        ALU alu;

        public frmALU(ALU alu)
        {
            InitializeComponent();
            this.alu = alu;
        }

        private void frmALU_Load(object sender, EventArgs e)
        {
            RefreshStack();
        }

        private void RefreshStack()
        {
            lstStack.Items.Clear();
            foreach (int x in alu.Stack)
            {
                lstStack.Items.Add(x);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshStack();
        }
    }
}
