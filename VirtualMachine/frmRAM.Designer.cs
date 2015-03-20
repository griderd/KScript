namespace VirtualMachine
{
    partial class frmRAM
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
            this.chkHexDigits = new System.Windows.Forms.CheckBox();
            this.txtHexView = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnDump = new System.Windows.Forms.Button();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // chkHexDigits
            // 
            this.chkHexDigits.AutoSize = true;
            this.chkHexDigits.Checked = true;
            this.chkHexDigits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHexDigits.Location = new System.Drawing.Point(12, 446);
            this.chkHexDigits.Name = "chkHexDigits";
            this.chkHexDigits.Size = new System.Drawing.Size(74, 17);
            this.chkHexDigits.TabIndex = 5;
            this.chkHexDigits.Text = "Hex Digits";
            this.chkHexDigits.UseVisualStyleBackColor = true;
            this.chkHexDigits.CheckedChanged += new System.EventHandler(this.chkHexDigits_CheckedChanged_1);
            // 
            // txtHexView
            // 
            this.txtHexView.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHexView.Location = new System.Drawing.Point(12, 12);
            this.txtHexView.Multiline = true;
            this.txtHexView.Name = "txtHexView";
            this.txtHexView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHexView.Size = new System.Drawing.Size(732, 428);
            this.txtHexView.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(631, 446);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(113, 31);
            this.btnDump.TabIndex = 6;
            this.btnDump.Text = "Dump";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "*.bin";
            // 
            // frmRAM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 485);
            this.Controls.Add(this.btnDump);
            this.Controls.Add(this.chkHexDigits);
            this.Controls.Add(this.txtHexView);
            this.Name = "frmRAM";
            this.Text = "frmRAM";
            this.Load += new System.EventHandler(this.frmRAM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkHexDigits;
        internal System.Windows.Forms.TextBox txtHexView;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
    }
}