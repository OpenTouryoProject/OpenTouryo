namespace WSClientWin2_sample
{
    partial class Form2
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtGuid = new System.Windows.Forms.TextBox();
            this.btnFormList = new System.Windows.Forms.Button();
            this.btnFormCount = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(318, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(78, 52);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 70);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(384, 19);
            this.txtStatus.TabIndex = 1;
            // 
            // txtGuid
            // 
            this.txtGuid.Location = new System.Drawing.Point(12, 95);
            this.txtGuid.Name = "txtGuid";
            this.txtGuid.ReadOnly = true;
            this.txtGuid.Size = new System.Drawing.Size(384, 19);
            this.txtGuid.TabIndex = 2;
            // 
            // btnFormList
            // 
            this.btnFormList.Location = new System.Drawing.Point(12, 12);
            this.btnFormList.Name = "btnFormList";
            this.btnFormList.Size = new System.Drawing.Size(300, 23);
            this.btnFormList.TabIndex = 3;
            this.btnFormList.Text = "Formを識別するIDをリストする。";
            this.btnFormList.UseVisualStyleBackColor = true;
            // 
            // btnFormCount
            // 
            this.btnFormCount.Location = new System.Drawing.Point(12, 41);
            this.btnFormCount.Name = "btnFormCount";
            this.btnFormCount.Size = new System.Drawing.Size(300, 23);
            this.btnFormCount.TabIndex = 4;
            this.btnFormCount.Text = "全Formインスタンス数を表示する。";
            this.btnFormCount.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 126);
            this.Controls.Add(this.btnFormCount);
            this.Controls.Add(this.btnFormList);
            this.Controls.Add(this.txtGuid);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnClose);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtGuid;
        private System.Windows.Forms.Button btnFormList;
        private System.Windows.Forms.Button btnFormCount;
    }
}
