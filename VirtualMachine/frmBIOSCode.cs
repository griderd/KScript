using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KScript.Hardware.Firmware;

namespace VirtualMachine
{
    public partial class frmBIOSCode : Form
    {
        public frmBIOSCode()
        {
            InitializeComponent();
        }

        private void frmBIOSCode_Load(object sender, EventArgs e)
        {
            txtCode.Text = BIOS.POSTSource;
            lstErrors.Items.Clear();
            lstWarnings.Items.Clear();

            foreach (string error in BIOS.Errors)
            {
                lstErrors.Items.Add(error);
            }
            foreach (string warning in BIOS.Warnings)
            {
                lstWarnings.Items.Add(warning);
            }

            tabControl1.TabPages[0].Text = "Errors (" + lstErrors.Items.Count.ToString() + ")";
            tabControl1.TabPages[1].Text = "Warnings (" + lstWarnings.Items.Count.ToString() + ")";
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
