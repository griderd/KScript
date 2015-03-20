using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KScript.Hardware;
using KScript.Hardware.GraphicsAdapters;
using KScript.Hardware.Peripherals;

namespace VirtualMachine
{
    public partial class frmSystemBoard : Form
    {
        frmBIOSCode biosForm;
        frmRAM ramForm;
        frmALU aluForm;
        frmCPU cpuForm;
        GDIVideo monitor;

        public frmSystemBoard()
        {
            InitializeComponent();
            lblStatusCode.Parent = lblCodeBack;
            lblStatusCode.BackColor = Color.Transparent;
            lblStatusCode.Left = 0;
            lblStatusCode.Top = 0;
            tmrStatusCode.Enabled = true;

            monitor = new GDIVideo(Program.mda);
            monitor.Show();
        }

        private void frmSystemBoard_Load(object sender, EventArgs e)
        {
            lblStatusCode.Text = "";
        }

        private void btnPower_Click(object sender, EventArgs e)
        {
            if (!Program.systemBoard.Running)
            {
                if (chkDebugMode.Checked) Program.cpu.ChangeMode(CPUMode.Debug);
                // TODO: set MDA to debug mode
                Program.systemBoard.Start();
            }
            else
            {
                Program.systemBoard.Stop();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (Program.systemBoard.Running)
            {
                Program.systemBoard.Stop();
                tmrReset.Enabled = true;
            }
        }

        private void btnBIOS_Click(object sender, EventArgs e)
        {
            biosForm = new frmBIOSCode();
            biosForm.Show();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            Program.systemBoard.StepCPU();
        }

        private void btnRAM_Click(object sender, EventArgs e)
        {
            ramForm = new frmRAM(Program.ram, Program.cpu);
            ramForm.Show();
        }

        private void btnALU_Click(object sender, EventArgs e)
        {
            aluForm = new frmALU(Program.alu);
            aluForm.Show();
        }

        private void tmrStatusCode_Tick(object sender, EventArgs e)
        {
            if (Program.systemBoard.Running)
                lblStatusCode.Text = ((int)Program.systemBoard.State).ToString("0000");
            else
                lblStatusCode.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.mda.Snapshot();
        }

        private void tmrReset_Tick(object sender, EventArgs e)
        {
            tmrReset.Enabled = false;
            if (chkDebugMode.Checked) Program.cpu.ChangeMode(CPUMode.Debug);
            else Program.cpu.ChangeMode(CPUMode.Real);
            Program.systemBoard.Start();
        }

        private void btnCPU_Click(object sender, EventArgs e)
        {
            cpuForm = new frmCPU();
            cpuForm.Show();
        }

        private void frmSystemBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cpuForm != null) cpuForm.Close();
            if (aluForm != null) aluForm.Close();
            if (biosForm != null) biosForm.Close();
            if (ramForm != null) ramForm.Close();
            Program.Exit();
        }


    }
}
