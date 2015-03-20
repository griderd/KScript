using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VirtualMachine
{
    public partial class frmCPU : Form
    {
        public frmCPU()
        {
            InitializeComponent();
        }

        private void frmCPU_Load(object sender, EventArgs e)
        {
        }

        void cpu_StateChanged(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblArg1.Text = "Arg1: " + Program.cpu.Arg1.ToString();
            lblArg2.Text = "Arg2: " + Program.cpu.Arg2.ToString();
            lblInstruction.Text = "Instruction: " + Enum.GetName(typeof(KScript.Hardware.Instructions), Program.cpu.Instruction);
            lblPC.Text = "PC: " + Program.cpu.ProgramCounter;
            lblRegA.Text = "A: " + Program.cpu[KScript.Hardware.Instructions.rega].ToString();
            lblRegB.Text = "B: " + Program.cpu[KScript.Hardware.Instructions.regb].ToString();
            lblRegC.Text = "C: " + Program.cpu[KScript.Hardware.Instructions.regc].ToString();
            lblRegD.Text = "D: " + Program.cpu[KScript.Hardware.Instructions.regd].ToString();
            //lblRegE.Text = "E: " + Program.cpu[KScript.Hardware.Instructions.rege].ToString();
            //lblRegF.Text = "F: " + Program.cpu[KScript.Hardware.Instructions.regf].ToString();
            lblRegG.Text = "G: " + Program.cpu[KScript.Hardware.Instructions.regg].ToString();
            lblRegH.Text = "H: " + Program.cpu[KScript.Hardware.Instructions.regh].ToString();

            lblCycleLength.Text = "Avg Cycle Length: " + Program.cpu.AverageCycleLength.ToString() + " µs";
            lblCycleRate.Text = "Avg Cycle Rate: " + (double)Program.cpu.AverageCycleRate / 1000.0 + " kHz";
            lblClockRate.Text = "Clock Rate: " + (double)Program.cpu.ClockSpeed / 1000.0 + " kHz";
            lblLastCycleLength.Text = "Last Cycle Length: " + Program.cpu.LastCycleLength.ToString() + " µs";
            lblMaxCycleLength.Text = "Max Cycle Length: " + Program.cpu.MaxCycleLength.ToString() + " µs";
            lblCycleCount.Text = "Cycle Count: " + Program.cpu.CycleCount.ToString();

            lstCallStack.Items.Clear();
            foreach (int stackFrame in Program.cpu.CallStack)
            {
                lstCallStack.Items.Add(stackFrame);
            }

        }
    }
}
