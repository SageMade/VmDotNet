namespace VM.Net
{
    partial class RegisterView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvRegisters = new System.Windows.Forms.DataGridView();
            this.clmRegisterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmRegisterValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegisters)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(277, 290);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // dgvRegisters
            // 
            this.dgvRegisters.AllowUserToAddRows = false;
            this.dgvRegisters.AllowUserToDeleteRows = false;
            this.dgvRegisters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegisters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmRegisterName,
            this.clmRegisterValue});
            this.dgvRegisters.Location = new System.Drawing.Point(13, 13);
            this.dgvRegisters.Name = "dgvRegisters";
            this.dgvRegisters.ReadOnly = true;
            this.dgvRegisters.RowHeadersVisible = false;
            this.dgvRegisters.Size = new System.Drawing.Size(339, 262);
            this.dgvRegisters.TabIndex = 1;
            // 
            // clmRegisterName
            // 
            this.clmRegisterName.HeaderText = "Name";
            this.clmRegisterName.Name = "clmRegisterName";
            this.clmRegisterName.ReadOnly = true;
            // 
            // clmRegisterValue
            // 
            dataGridViewCellStyle1.Format = "X04";
            dataGridViewCellStyle1.NullValue = "0";
            this.clmRegisterValue.DefaultCellStyle = dataGridViewCellStyle1;
            this.clmRegisterValue.HeaderText = "Value";
            this.clmRegisterValue.Name = "clmRegisterValue";
            this.clmRegisterValue.ReadOnly = true;
            // 
            // RegisterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 325);
            this.Controls.Add(this.dgvRegisters);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterView";
            this.Text = "Registers";
            this.Load += new System.EventHandler(this.RegisterVIew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegisters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvRegisters;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmRegisterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmRegisterValue;
    }
}