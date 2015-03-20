namespace VirtualMachine
{
    partial class frmSystemBoard
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
            this.lblCodeBack = new System.Windows.Forms.Label();
            this.lblStatusCode = new System.Windows.Forms.Label();
            this.btnPower = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnBIOS = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnCPU = new System.Windows.Forms.Button();
            this.btnALU = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnRAM = new System.Windows.Forms.Button();
            this.tmrStatusCode = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.tmrReset = new System.Windows.Forms.Timer(this.components);
            this.chkDebugMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblCodeBack
            // 
            this.lblCodeBack.AutoSize = true;
            this.lblCodeBack.BackColor = System.Drawing.Color.Black;
            this.lblCodeBack.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCodeBack.Font = new System.Drawing.Font("Digital-7 Mono", 48F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodeBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCodeBack.Location = new System.Drawing.Point(12, 9);
            this.lblCodeBack.Name = "lblCodeBack";
            this.lblCodeBack.Size = new System.Drawing.Size(150, 66);
            this.lblCodeBack.TabIndex = 0;
            this.lblCodeBack.Text = "8888";
            // 
            // lblStatusCode
            // 
            this.lblStatusCode.AutoSize = true;
            this.lblStatusCode.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusCode.Font = new System.Drawing.Font("Digital-7 Mono", 48F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusCode.ForeColor = System.Drawing.Color.Red;
            this.lblStatusCode.Location = new System.Drawing.Point(12, 11);
            this.lblStatusCode.Name = "lblStatusCode";
            this.lblStatusCode.Size = new System.Drawing.Size(148, 64);
            this.lblStatusCode.TabIndex = 1;
            this.lblStatusCode.Text = "0000";
            // 
            // btnPower
            // 
            this.btnPower.Location = new System.Drawing.Point(241, 9);
            this.btnPower.Name = "btnPower";
            this.btnPower.Size = new System.Drawing.Size(69, 66);
            this.btnPower.TabIndex = 2;
            this.btnPower.Text = "PWR";
            this.btnPower.UseVisualStyleBackColor = true;
            this.btnPower.Click += new System.EventHandler(this.btnPower_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(316, 9);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(69, 66);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnBIOS
            // 
            this.btnBIOS.Location = new System.Drawing.Point(12, 90);
            this.btnBIOS.Name = "btnBIOS";
            this.btnBIOS.Size = new System.Drawing.Size(41, 42);
            this.btnBIOS.TabIndex = 4;
            this.btnBIOS.Text = "BIOS";
            this.btnBIOS.UseVisualStyleBackColor = true;
            this.btnBIOS.Click += new System.EventHandler(this.btnBIOS_Click);
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(12, 144);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(77, 54);
            this.btnStep.TabIndex = 5;
            this.btnStep.Text = "CPU Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnCPU
            // 
            this.btnCPU.Location = new System.Drawing.Point(12, 204);
            this.btnCPU.Name = "btnCPU";
            this.btnCPU.Size = new System.Drawing.Size(192, 182);
            this.btnCPU.TabIndex = 6;
            this.btnCPU.Text = "CPU";
            this.btnCPU.UseVisualStyleBackColor = true;
            this.btnCPU.Click += new System.EventHandler(this.btnCPU_Click);
            // 
            // btnALU
            // 
            this.btnALU.Location = new System.Drawing.Point(95, 144);
            this.btnALU.Name = "btnALU";
            this.btnALU.Size = new System.Drawing.Size(109, 54);
            this.btnALU.TabIndex = 7;
            this.btnALU.Text = "ALU";
            this.btnALU.UseVisualStyleBackColor = true;
            this.btnALU.Click += new System.EventHandler(this.btnALU_Click);
            // 
            // btnRAM
            // 
            this.btnRAM.Location = new System.Drawing.Point(372, 144);
            this.btnRAM.Name = "btnRAM";
            this.btnRAM.Size = new System.Drawing.Size(60, 242);
            this.btnRAM.TabIndex = 8;
            this.btnRAM.Text = "RAM";
            this.btnRAM.UseVisualStyleBackColor = true;
            this.btnRAM.Click += new System.EventHandler(this.btnRAM_Click);
            // 
            // tmrStatusCode
            // 
            this.tmrStatusCode.Interval = 200;
            this.tmrStatusCode.Tick += new System.EventHandler(this.tmrStatusCode_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 336);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 50);
            this.button1.TabIndex = 9;
            this.button1.Text = "Screenshot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tmrReset
            // 
            this.tmrReset.Interval = 2500;
            this.tmrReset.Tick += new System.EventHandler(this.tmrReset_Tick);
            // 
            // chkDebugMode
            // 
            this.chkDebugMode.AutoSize = true;
            this.chkDebugMode.Location = new System.Drawing.Point(242, 81);
            this.chkDebugMode.Name = "chkDebugMode";
            this.chkDebugMode.Size = new System.Drawing.Size(124, 17);
            this.chkDebugMode.TabIndex = 10;
            this.chkDebugMode.Text = "Start in Debug Mode";
            this.chkDebugMode.UseVisualStyleBackColor = true;
            // 
            // frmSystemBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 420);
            this.Controls.Add(this.chkDebugMode);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblStatusCode);
            this.Controls.Add(this.btnRAM);
            this.Controls.Add(this.btnALU);
            this.Controls.Add(this.btnCPU);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnBIOS);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnPower);
            this.Controls.Add(this.lblCodeBack);
            this.Name = "frmSystemBoard";
            this.Text = "System Board";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSystemBoard_FormClosed);
            this.Load += new System.EventHandler(this.frmSystemBoard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCodeBack;
        private System.Windows.Forms.Label lblStatusCode;
        private System.Windows.Forms.Button btnPower;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnBIOS;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnCPU;
        private System.Windows.Forms.Button btnALU;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnRAM;
        private System.Windows.Forms.Timer tmrStatusCode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer tmrReset;
        private System.Windows.Forms.CheckBox chkDebugMode;
    }
}

