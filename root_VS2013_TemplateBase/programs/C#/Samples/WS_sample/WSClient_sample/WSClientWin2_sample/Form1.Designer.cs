namespace WSClientWin2_sample
{
    partial class Form1
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
            this.btnSync = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnOpenForm2 = new System.Windows.Forms.Button();
            this.btnButton1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnASync = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxDialog = new System.Windows.Forms.CheckBox();
            this.cbxWindow = new System.Windows.Forms.CheckBox();
            this.cbxClick = new System.Windows.Forms.CheckBox();
            this.cbxDoClick = new System.Windows.Forms.CheckBox();
            this.btnButton2 = new System.Windows.Forms.Button();
            this.btnHdnBtn1 = new Touryo.Infrastructure.Framework.RichClient.Presentation.HiddenButton();
            this.cbxDoClick2 = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(201, 12);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 5;
            this.btnSync.Text = "同期実行";
            this.btnSync.UseVisualStyleBackColor = true;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(18, 170);
            this.txtStatus.MaxLength = 1000000000;
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatus.Size = new System.Drawing.Size(464, 257);
            this.txtStatus.TabIndex = 6;
            this.txtStatus.TextChanged += new System.EventHandler(this.txtStatus_TextChanged);
            // 
            // btnOpenForm2
            // 
            this.btnOpenForm2.Location = new System.Drawing.Point(363, 12);
            this.btnOpenForm2.Name = "btnOpenForm2";
            this.btnOpenForm2.Size = new System.Drawing.Size(119, 23);
            this.btnOpenForm2.TabIndex = 10;
            this.btnOpenForm2.Text = "Form2を開く";
            this.btnOpenForm2.UseVisualStyleBackColor = true;
            // 
            // btnButton1
            // 
            this.btnButton1.Location = new System.Drawing.Point(19, 41);
            this.btnButton1.Name = "btnButton1";
            this.btnButton1.Size = new System.Drawing.Size(256, 23);
            this.btnButton1.TabIndex = 11;
            this.btnButton1.Text = "メソッドの実装されているボタン";
            this.btnButton1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "テスト（処理中も操作可能１）",
            "テスト（処理中も操作可能２）",
            "テスト（処理中も操作可能３）",
            "テスト（処理中も操作可能４）",
            "テスト（処理中も操作可能５）",
            "テスト（処理中も操作可能６）",
            "テスト（処理中も操作可能７）",
            "テスト（処理中も操作可能８）",
            "テスト（処理中も操作可能９）"});
            this.comboBox1.Location = new System.Drawing.Point(281, 41);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(201, 20);
            this.comboBox1.TabIndex = 12;
            // 
            // btnASync
            // 
            this.btnASync.Location = new System.Drawing.Point(282, 12);
            this.btnASync.Name = "btnASync";
            this.btnASync.Size = new System.Drawing.Size(75, 23);
            this.btnASync.TabIndex = 13;
            this.btnASync.Text = "非同期実行";
            this.btnASync.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(75, 15);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 19);
            this.numericUpDown1.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "実行時間";
            // 
            // cbxDialog
            // 
            this.cbxDialog.AutoSize = true;
            this.cbxDialog.Location = new System.Drawing.Point(18, 126);
            this.cbxDialog.Name = "cbxDialog";
            this.cbxDialog.Size = new System.Drawing.Size(205, 16);
            this.cbxDialog.TabIndex = 16;
            this.cbxDialog.Text = "(2) 結果表示でダイアログを表示する。";
            this.cbxDialog.UseVisualStyleBackColor = true;
            // 
            // cbxWindow
            // 
            this.cbxWindow.AutoSize = true;
            this.cbxWindow.Location = new System.Drawing.Point(18, 104);
            this.cbxWindow.Name = "cbxWindow";
            this.cbxWindow.Size = new System.Drawing.Size(203, 16);
            this.cbxWindow.TabIndex = 17;
            this.cbxWindow.Text = "(1) 結果表示でウィンドウを表示する。";
            this.cbxWindow.UseVisualStyleBackColor = true;
            // 
            // cbxClick
            // 
            this.cbxClick.AutoSize = true;
            this.cbxClick.Location = new System.Drawing.Point(233, 104);
            this.cbxClick.Name = "cbxClick";
            this.cbxClick.Size = new System.Drawing.Size(230, 16);
            this.cbxClick.TabIndex = 18;
            this.cbxClick.Text = "(3) 結果表示でClickイベントを発生させる。";
            this.cbxClick.UseVisualStyleBackColor = true;
            // 
            // cbxDoClick
            // 
            this.cbxDoClick.AutoSize = true;
            this.cbxDoClick.Location = new System.Drawing.Point(233, 126);
            this.cbxDoClick.Name = "cbxDoClick";
            this.cbxDoClick.Size = new System.Drawing.Size(254, 16);
            this.cbxDoClick.TabIndex = 19;
            this.cbxDoClick.Text = "(4) 結果表示でDoClickでイベントを発生させる。";
            this.cbxDoClick.UseVisualStyleBackColor = true;
            // 
            // btnButton2
            // 
            this.btnButton2.Location = new System.Drawing.Point(19, 70);
            this.btnButton2.Name = "btnButton2";
            this.btnButton2.Size = new System.Drawing.Size(256, 23);
            this.btnButton2.TabIndex = 20;
            this.btnButton2.Text = "メソッドの実装されていないボタン";
            this.btnButton2.UseVisualStyleBackColor = true;
            // 
            // btnHdnBtn1
            // 
            this.btnHdnBtn1.Location = new System.Drawing.Point(282, 70);
            this.btnHdnBtn1.Name = "btnHdnBtn1";
            this.btnHdnBtn1.Size = new System.Drawing.Size(200, 23);
            this.btnHdnBtn1.TabIndex = 21;
            this.btnHdnBtn1.Text = "hiddenButton1";
            this.btnHdnBtn1.UseVisualStyleBackColor = true;
            this.btnHdnBtn1.Visible = false;
            // 
            // cbxDoClick2
            // 
            this.cbxDoClick2.AutoSize = true;
            this.cbxDoClick2.Location = new System.Drawing.Point(18, 148);
            this.cbxDoClick2.Name = "cbxDoClick2";
            this.cbxDoClick2.Size = new System.Drawing.Size(458, 16);
            this.cbxDoClick2.TabIndex = 22;
            this.cbxDoClick2.Text = "(5) 結果表示でDoClickでイベントを発生させ、そのイベント内で更に非同期呼び出しを行う。";
            this.cbxDoClick2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbxDoClick2);
            this.panel1.Controls.Add(this.btnSync);
            this.panel1.Controls.Add(this.btnHdnBtn1);
            this.panel1.Controls.Add(this.txtStatus);
            this.panel1.Controls.Add(this.btnButton2);
            this.panel1.Controls.Add(this.btnOpenForm2);
            this.panel1.Controls.Add(this.cbxDoClick);
            this.panel1.Controls.Add(this.btnButton1);
            this.panel1.Controls.Add(this.cbxClick);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.cbxWindow);
            this.panel1.Controls.Add(this.btnASync);
            this.panel1.Controls.Add(this.cbxDialog);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(499, 441);
            this.panel1.TabIndex = 23;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 459);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(119, 19);
            this.textBox1.TabIndex = 24;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(137, 459);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(119, 19);
            this.textBox2.TabIndex = 25;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(267, 459);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(119, 19);
            this.textBox3.TabIndex = 26;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(392, 459);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(119, 19);
            this.textBox4.TabIndex = 27;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 494);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnOpenForm2;
        private System.Windows.Forms.Button btnButton1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnASync;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxDialog;
        private System.Windows.Forms.CheckBox cbxWindow;
        private System.Windows.Forms.CheckBox cbxClick;
        private System.Windows.Forms.CheckBox cbxDoClick;
        private System.Windows.Forms.Button btnButton2;
        private Touryo.Infrastructure.Framework.RichClient.Presentation.HiddenButton btnHdnBtn1;
        private System.Windows.Forms.CheckBox cbxDoClick2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
    }
}
