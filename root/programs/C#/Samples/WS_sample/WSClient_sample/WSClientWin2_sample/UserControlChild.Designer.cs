namespace WSClientWin2_sample
{
    partial class UserControlChild
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnUCButton1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUCButton1
            // 
            this.btnUCButton1.Location = new System.Drawing.Point(3, 3);
            this.btnUCButton1.Name = "btnUCButton1";
            this.btnUCButton1.Size = new System.Drawing.Size(144, 23);
            this.btnUCButton1.TabIndex = 0;
            this.btnUCButton1.Text = "button1";
            this.btnUCButton1.UseVisualStyleBackColor = true;
            // 
            // UserControlChild
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUCButton1);
            this.Name = "UserControlChild";
            this.Size = new System.Drawing.Size(150, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUCButton1;
    }
}
