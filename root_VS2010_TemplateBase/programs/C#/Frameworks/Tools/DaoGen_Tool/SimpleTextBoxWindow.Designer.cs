namespace DaoGen_Tool
{
    partial class SimpleTextBoxWindow
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
            this.tbxMulti = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbxMulti
            // 
            this.tbxMulti.Location = new System.Drawing.Point(0, 1);
            this.tbxMulti.Multiline = true;
            this.tbxMulti.Name = "tbxMulti";
            this.tbxMulti.ReadOnly = true;
            this.tbxMulti.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxMulti.Size = new System.Drawing.Size(312, 210);
            this.tbxMulti.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(0, 217);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(312, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // SimpleTextBoxWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 253);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tbxMulti);
            this.Name = "SimpleTextBoxWindow";
            this.Text = "SimpleTextBoxWindow";
            this.Load += new System.EventHandler(this.SimpleTextBoxWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxMulti;
        private System.Windows.Forms.Button btnClose;
    }
}