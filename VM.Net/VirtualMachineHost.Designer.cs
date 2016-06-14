namespace VM.Net
{
    partial class VirtualMachineHost
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClockSpeed = new System.Windows.Forms.ToolStripMenuItem();
            this.opt5Hz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt50Hz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt100Hz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt1KHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt10KHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt100KHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt1MHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt10MHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt100MHz = new System.Windows.Forms.ToolStripMenuItem();
            this.opt1GHz = new System.Windows.Forms.ToolStripMenuItem();
            this.optPaused = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRegisters = new System.Windows.Forms.Panel();
            this.lblRegisters = new System.Windows.Forms.Label();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.myVirtualScreen = new VM.Net.VirtualMachine.VirtualScreen();
            this.menuStrip1.SuspendLayout();
            this.pnlRegisters.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mnuSettings});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(641, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "msMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClockSpeed,
            this.optPaused});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(61, 20);
            this.mnuSettings.Text = "&Settings";
            // 
            // mnuClockSpeed
            // 
            this.mnuClockSpeed.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opt5Hz,
            this.opt50Hz,
            this.opt100Hz,
            this.opt1KHz,
            this.opt10KHz,
            this.opt100KHz,
            this.opt1MHz,
            this.opt10MHz,
            this.opt100MHz,
            this.opt1GHz});
            this.mnuClockSpeed.Name = "mnuClockSpeed";
            this.mnuClockSpeed.Size = new System.Drawing.Size(139, 22);
            this.mnuClockSpeed.Text = "Clock Speed";
            // 
            // opt5Hz
            // 
            this.opt5Hz.Name = "opt5Hz";
            this.opt5Hz.Size = new System.Drawing.Size(120, 22);
            this.opt5Hz.Tag = "5";
            this.opt5Hz.Text = "5 Hz";
            this.opt5Hz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt50Hz
            // 
            this.opt50Hz.Name = "opt50Hz";
            this.opt50Hz.Size = new System.Drawing.Size(120, 22);
            this.opt50Hz.Tag = "50";
            this.opt50Hz.Text = "50 Hz";
            this.opt50Hz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt100Hz
            // 
            this.opt100Hz.Name = "opt100Hz";
            this.opt100Hz.Size = new System.Drawing.Size(120, 22);
            this.opt100Hz.Tag = "100";
            this.opt100Hz.Text = "100 Hz";
            this.opt100Hz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt1KHz
            // 
            this.opt1KHz.Name = "opt1KHz";
            this.opt1KHz.Size = new System.Drawing.Size(120, 22);
            this.opt1KHz.Tag = "1000";
            this.opt1KHz.Text = "1 KHz";
            this.opt1KHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt10KHz
            // 
            this.opt10KHz.Name = "opt10KHz";
            this.opt10KHz.Size = new System.Drawing.Size(120, 22);
            this.opt10KHz.Tag = "10000";
            this.opt10KHz.Text = "10 KHz";
            this.opt10KHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt100KHz
            // 
            this.opt100KHz.Name = "opt100KHz";
            this.opt100KHz.Size = new System.Drawing.Size(120, 22);
            this.opt100KHz.Tag = "100000";
            this.opt100KHz.Text = "100 KHz";
            this.opt100KHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt1MHz
            // 
            this.opt1MHz.Name = "opt1MHz";
            this.opt1MHz.Size = new System.Drawing.Size(120, 22);
            this.opt1MHz.Tag = "1000000";
            this.opt1MHz.Text = "1 MHz";
            this.opt1MHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt10MHz
            // 
            this.opt10MHz.Name = "opt10MHz";
            this.opt10MHz.Size = new System.Drawing.Size(120, 22);
            this.opt10MHz.Tag = "10000000";
            this.opt10MHz.Text = "10 MHz";
            this.opt10MHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt100MHz
            // 
            this.opt100MHz.Name = "opt100MHz";
            this.opt100MHz.Size = new System.Drawing.Size(120, 22);
            this.opt100MHz.Tag = "100000000";
            this.opt100MHz.Text = "100 MHz";
            this.opt100MHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // opt1GHz
            // 
            this.opt1GHz.Name = "opt1GHz";
            this.opt1GHz.Size = new System.Drawing.Size(120, 22);
            this.opt1GHz.Tag = "1000000000";
            this.opt1GHz.Text = "1 GHz";
            this.opt1GHz.Click += new System.EventHandler(this.ClockSpeedClicked);
            // 
            // optPaused
            // 
            this.optPaused.Name = "optPaused";
            this.optPaused.Size = new System.Drawing.Size(139, 22);
            this.optPaused.Text = "&Pause";
            this.optPaused.Click += new System.EventHandler(this.optPauseClicked);
            // 
            // pnlRegisters
            // 
            this.pnlRegisters.Controls.Add(this.lblRegisters);
            this.pnlRegisters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRegisters.Location = new System.Drawing.Point(0, 301);
            this.pnlRegisters.Name = "pnlRegisters";
            this.pnlRegisters.Size = new System.Drawing.Size(641, 54);
            this.pnlRegisters.TabIndex = 2;
            // 
            // lblRegisters
            // 
            this.lblRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRegisters.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegisters.Location = new System.Drawing.Point(0, 0);
            this.lblRegisters.Name = "lblRegisters";
            this.lblRegisters.Size = new System.Drawing.Size(641, 54);
            this.lblRegisters.TabIndex = 0;
            this.lblRegisters.Text = "[ ]";
            this.lblRegisters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "vbc";
            this.dlgOpenFile.Filter = "Virtual Byte Code | *.vbc";
            // 
            // myVirtualScreen
            // 
            this.myVirtualScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myVirtualScreen.Location = new System.Drawing.Point(0, 24);
            this.myVirtualScreen.Name = "myVirtualScreen";
            this.myVirtualScreen.ScreenMemoryLocation = ((ushort)(0));
            this.myVirtualScreen.Size = new System.Drawing.Size(641, 331);
            this.myVirtualScreen.TabIndex = 0;
            // 
            // VirtualMachineHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 355);
            this.Controls.Add(this.pnlRegisters);
            this.Controls.Add(this.myVirtualScreen);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VirtualMachineHost";
            this.Text = "VirtualScreenHost";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlRegisters.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VirtualMachine.VirtualScreen myVirtualScreen;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Panel pnlRegisters;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Label lblRegisters;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuClockSpeed;
        private System.Windows.Forms.ToolStripMenuItem opt5Hz;
        private System.Windows.Forms.ToolStripMenuItem opt50Hz;
        private System.Windows.Forms.ToolStripMenuItem opt100Hz;
        private System.Windows.Forms.ToolStripMenuItem opt1KHz;
        private System.Windows.Forms.ToolStripMenuItem opt10KHz;
        private System.Windows.Forms.ToolStripMenuItem opt100KHz;
        private System.Windows.Forms.ToolStripMenuItem opt1MHz;
        private System.Windows.Forms.ToolStripMenuItem opt10MHz;
        private System.Windows.Forms.ToolStripMenuItem opt100MHz;
        private System.Windows.Forms.ToolStripMenuItem opt1GHz;
        private System.Windows.Forms.ToolStripMenuItem optPaused;
    }
}