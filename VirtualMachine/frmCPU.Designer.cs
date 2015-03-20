namespace VirtualMachine
{
    partial class frmCPU
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblPC = new System.Windows.Forms.Label();
            this.lblRegA = new System.Windows.Forms.Label();
            this.lblRegB = new System.Windows.Forms.Label();
            this.lblRegC = new System.Windows.Forms.Label();
            this.lblRegD = new System.Windows.Forms.Label();
            this.lblRegE = new System.Windows.Forms.Label();
            this.lblRegF = new System.Windows.Forms.Label();
            this.lblRegG = new System.Windows.Forms.Label();
            this.lblRegH = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstCallStack = new System.Windows.Forms.ListBox();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblArg1 = new System.Windows.Forms.Label();
            this.lblArg2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblClockRate = new System.Windows.Forms.Label();
            this.lblCycleRate = new System.Windows.Forms.Label();
            this.lblLastCycleLength = new System.Windows.Forms.Label();
            this.lblCycleLength = new System.Windows.Forms.Label();
            this.lblMaxCycleLength = new System.Windows.Forms.Label();
            this.lblCycleCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Location = new System.Drawing.Point(12, 65);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(24, 13);
            this.lblPC.TabIndex = 0;
            this.lblPC.Text = "PC:";
            // 
            // lblRegA
            // 
            this.lblRegA.AutoSize = true;
            this.lblRegA.Location = new System.Drawing.Point(12, 88);
            this.lblRegA.Name = "lblRegA";
            this.lblRegA.Size = new System.Drawing.Size(17, 13);
            this.lblRegA.TabIndex = 1;
            this.lblRegA.Text = "A:";
            // 
            // lblRegB
            // 
            this.lblRegB.AutoSize = true;
            this.lblRegB.Location = new System.Drawing.Point(12, 101);
            this.lblRegB.Name = "lblRegB";
            this.lblRegB.Size = new System.Drawing.Size(17, 13);
            this.lblRegB.TabIndex = 2;
            this.lblRegB.Text = "B:";
            // 
            // lblRegC
            // 
            this.lblRegC.AutoSize = true;
            this.lblRegC.Location = new System.Drawing.Point(12, 114);
            this.lblRegC.Name = "lblRegC";
            this.lblRegC.Size = new System.Drawing.Size(17, 13);
            this.lblRegC.TabIndex = 3;
            this.lblRegC.Text = "C:";
            // 
            // lblRegD
            // 
            this.lblRegD.AutoSize = true;
            this.lblRegD.Location = new System.Drawing.Point(12, 127);
            this.lblRegD.Name = "lblRegD";
            this.lblRegD.Size = new System.Drawing.Size(18, 13);
            this.lblRegD.TabIndex = 4;
            this.lblRegD.Text = "D:";
            // 
            // lblRegE
            // 
            this.lblRegE.AutoSize = true;
            this.lblRegE.Location = new System.Drawing.Point(168, 88);
            this.lblRegE.Name = "lblRegE";
            this.lblRegE.Size = new System.Drawing.Size(17, 13);
            this.lblRegE.TabIndex = 5;
            this.lblRegE.Text = "E:";
            // 
            // lblRegF
            // 
            this.lblRegF.AutoSize = true;
            this.lblRegF.Location = new System.Drawing.Point(168, 101);
            this.lblRegF.Name = "lblRegF";
            this.lblRegF.Size = new System.Drawing.Size(16, 13);
            this.lblRegF.TabIndex = 6;
            this.lblRegF.Text = "F:";
            // 
            // lblRegG
            // 
            this.lblRegG.AutoSize = true;
            this.lblRegG.Location = new System.Drawing.Point(168, 114);
            this.lblRegG.Name = "lblRegG";
            this.lblRegG.Size = new System.Drawing.Size(18, 13);
            this.lblRegG.TabIndex = 7;
            this.lblRegG.Text = "G:";
            // 
            // lblRegH
            // 
            this.lblRegH.AutoSize = true;
            this.lblRegH.Location = new System.Drawing.Point(168, 127);
            this.lblRegH.Name = "lblRegH";
            this.lblRegH.Size = new System.Drawing.Size(18, 13);
            this.lblRegH.TabIndex = 8;
            this.lblRegH.Text = "H:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Call Stack";
            // 
            // lstCallStack
            // 
            this.lstCallStack.FormattingEnabled = true;
            this.lstCallStack.Location = new System.Drawing.Point(5, 242);
            this.lstCallStack.Name = "lstCallStack";
            this.lstCallStack.Size = new System.Drawing.Size(326, 160);
            this.lstCallStack.TabIndex = 10;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Location = new System.Drawing.Point(12, 147);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(59, 13);
            this.lblInstruction.TabIndex = 11;
            this.lblInstruction.Text = "Instruction:";
            // 
            // lblArg1
            // 
            this.lblArg1.AutoSize = true;
            this.lblArg1.Location = new System.Drawing.Point(13, 160);
            this.lblArg1.Name = "lblArg1";
            this.lblArg1.Size = new System.Drawing.Size(32, 13);
            this.lblArg1.TabIndex = 12;
            this.lblArg1.Text = "Arg1:";
            // 
            // lblArg2
            // 
            this.lblArg2.AutoSize = true;
            this.lblArg2.Location = new System.Drawing.Point(13, 173);
            this.lblArg2.Name = "lblArg2";
            this.lblArg2.Size = new System.Drawing.Size(32, 13);
            this.lblArg2.TabIndex = 13;
            this.lblArg2.Text = "Arg2:";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblClockRate
            // 
            this.lblClockRate.AutoSize = true;
            this.lblClockRate.Location = new System.Drawing.Point(12, 9);
            this.lblClockRate.Name = "lblClockRate";
            this.lblClockRate.Size = new System.Drawing.Size(63, 13);
            this.lblClockRate.TabIndex = 14;
            this.lblClockRate.Text = "Clock Rate:";
            // 
            // lblCycleRate
            // 
            this.lblCycleRate.AutoSize = true;
            this.lblCycleRate.Location = new System.Drawing.Point(12, 22);
            this.lblCycleRate.Name = "lblCycleRate";
            this.lblCycleRate.Size = new System.Drawing.Size(62, 13);
            this.lblCycleRate.TabIndex = 15;
            this.lblCycleRate.Text = "Cycle Rate:";
            // 
            // lblLastCycleLength
            // 
            this.lblLastCycleLength.AutoSize = true;
            this.lblLastCycleLength.Location = new System.Drawing.Point(195, 9);
            this.lblLastCycleLength.Name = "lblLastCycleLength";
            this.lblLastCycleLength.Size = new System.Drawing.Size(72, 13);
            this.lblLastCycleLength.TabIndex = 17;
            this.lblLastCycleLength.Text = "Cycle Length:";
            // 
            // lblCycleLength
            // 
            this.lblCycleLength.AutoSize = true;
            this.lblCycleLength.Location = new System.Drawing.Point(195, 22);
            this.lblCycleLength.Name = "lblCycleLength";
            this.lblCycleLength.Size = new System.Drawing.Size(72, 13);
            this.lblCycleLength.TabIndex = 16;
            this.lblCycleLength.Text = "Cycle Length:";
            // 
            // lblMaxCycleLength
            // 
            this.lblMaxCycleLength.AutoSize = true;
            this.lblMaxCycleLength.Location = new System.Drawing.Point(195, 35);
            this.lblMaxCycleLength.Name = "lblMaxCycleLength";
            this.lblMaxCycleLength.Size = new System.Drawing.Size(72, 13);
            this.lblMaxCycleLength.TabIndex = 18;
            this.lblMaxCycleLength.Text = "Cycle Length:";
            // 
            // lblCycleCount
            // 
            this.lblCycleCount.AutoSize = true;
            this.lblCycleCount.Location = new System.Drawing.Point(13, 35);
            this.lblCycleCount.Name = "lblCycleCount";
            this.lblCycleCount.Size = new System.Drawing.Size(67, 13);
            this.lblCycleCount.TabIndex = 19;
            this.lblCycleCount.Text = "Cycle Count:";
            // 
            // frmCPU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 409);
            this.Controls.Add(this.lblCycleCount);
            this.Controls.Add(this.lblMaxCycleLength);
            this.Controls.Add(this.lblLastCycleLength);
            this.Controls.Add(this.lblCycleLength);
            this.Controls.Add(this.lblCycleRate);
            this.Controls.Add(this.lblClockRate);
            this.Controls.Add(this.lblArg2);
            this.Controls.Add(this.lblArg1);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lstCallStack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblRegH);
            this.Controls.Add(this.lblRegG);
            this.Controls.Add(this.lblRegF);
            this.Controls.Add(this.lblRegE);
            this.Controls.Add(this.lblRegD);
            this.Controls.Add(this.lblRegC);
            this.Controls.Add(this.lblRegB);
            this.Controls.Add(this.lblRegA);
            this.Controls.Add(this.lblPC);
            this.Name = "frmCPU";
            this.Text = "CPU State";
            this.Load += new System.EventHandler(this.frmCPU_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPC;
        private System.Windows.Forms.Label lblRegA;
        private System.Windows.Forms.Label lblRegB;
        private System.Windows.Forms.Label lblRegC;
        private System.Windows.Forms.Label lblRegD;
        private System.Windows.Forms.Label lblRegE;
        private System.Windows.Forms.Label lblRegF;
        private System.Windows.Forms.Label lblRegG;
        private System.Windows.Forms.Label lblRegH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstCallStack;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Label lblArg1;
        private System.Windows.Forms.Label lblArg2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblClockRate;
        private System.Windows.Forms.Label lblCycleRate;
        private System.Windows.Forms.Label lblLastCycleLength;
        private System.Windows.Forms.Label lblCycleLength;
        private System.Windows.Forms.Label lblMaxCycleLength;
        private System.Windows.Forms.Label lblCycleCount;
    }
}