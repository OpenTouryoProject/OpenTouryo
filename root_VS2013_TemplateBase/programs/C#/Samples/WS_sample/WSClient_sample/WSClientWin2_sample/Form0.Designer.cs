namespace WSClientWin2_sample
{
    partial class Form0
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
            this.btnOpenForm1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenForm3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenForm1
            // 
            this.btnOpenForm1.Location = new System.Drawing.Point(12, 12);
            this.btnOpenForm1.Name = "btnOpenForm1";
            this.btnOpenForm1.Size = new System.Drawing.Size(518, 23);
            this.btnOpenForm1.TabIndex = 0;
            this.btnOpenForm1.Text = "Form1(スレッド制御と画面制御の動作確認サンプル)を開く";
            this.btnOpenForm1.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 70);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(518, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "終了";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnOpenForm3
            // 
            this.btnOpenForm3.Location = new System.Drawing.Point(12, 41);
            this.btnOpenForm3.Name = "btnOpenForm3";
            this.btnOpenForm3.Size = new System.Drawing.Size(518, 23);
            this.btnOpenForm3.TabIndex = 3;
            this.btnOpenForm3.Text = "Form3(種々のコントロールのイベント動作サンプル)を開く";
            this.btnOpenForm3.UseVisualStyleBackColor = true;
            // 
            // Form0
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 103);
            this.Controls.Add(this.btnOpenForm3);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpenForm1);
            this.Name = "Form0";
            this.Text = "Form0";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenForm1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpenForm3;
    }
}
