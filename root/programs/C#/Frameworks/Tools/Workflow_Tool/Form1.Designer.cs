namespace Workflow_Tool
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCnnstr = new System.Windows.Forms.Label();
            this.txtCnnstr = new System.Windows.Forms.TextBox();
            this.txtSubSystemId = new System.Windows.Forms.TextBox();
            this.lblSubSystemId = new System.Windows.Forms.Label();
            this.txtWorkflowName = new System.Windows.Forms.TextBox();
            this.lblWorkflowName = new System.Windows.Forms.Label();
            this.txtDearSirUID = new System.Windows.Forms.TextBox();
            this.lblDearSirUID = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.lblUserID = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.txtWorkflowControlNo = new System.Windows.Forms.TextBox();
            this.lblWorkflowControlNo = new System.Windows.Forms.Label();
            this.txtWorkflowReserveArea = new System.Windows.Forms.TextBox();
            this.lblWorkflowReserveArea = new System.Windows.Forms.Label();
            this.txtCurrentWorkflowReserveArea1 = new System.Windows.Forms.TextBox();
            this.lblCurrentWorkflowReserveArea1 = new System.Windows.Forms.Label();
            this.lblReplyDeadline1 = new System.Windows.Forms.Label();
            this.dtpReplyDeadline1 = new System.Windows.Forms.DateTimePicker();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.txtDearSirPTitleId = new System.Windows.Forms.TextBox();
            this.lblDearSirPTitleId = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.dtpReplyDeadline2 = new System.Windows.Forms.DateTimePicker();
            this.lblReplyDeadline2 = new System.Windows.Forms.Label();
            this.txtCurrentWorkflowReserveArea2 = new System.Windows.Forms.TextBox();
            this.lblCurrentWorkflowReserveArea2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Location = new System.Drawing.Point(16, 289);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(835, 188);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 67);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(827, 117);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "(1) 新しいワークフローを準備します。";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dtpReplyDeadline1);
            this.tabPage2.Controls.Add(this.lblReplyDeadline1);
            this.tabPage2.Controls.Add(this.txtCurrentWorkflowReserveArea1);
            this.tabPage2.Controls.Add(this.lblCurrentWorkflowReserveArea1);
            this.tabPage2.Controls.Add(this.txtWorkflowReserveArea);
            this.tabPage2.Controls.Add(this.lblWorkflowReserveArea);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Location = new System.Drawing.Point(4, 67);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(827, 117);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "(2) 新しいワークフローを開始します。";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(821, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "新しいワークフローを準備します。";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblCnnstr
            // 
            this.lblCnnstr.AutoSize = true;
            this.lblCnnstr.Location = new System.Drawing.Point(13, 17);
            this.lblCnnstr.Name = "lblCnnstr";
            this.lblCnnstr.Size = new System.Drawing.Size(90, 15);
            this.lblCnnstr.TabIndex = 1;
            this.lblCnnstr.Text = "接続文字列：";
            // 
            // txtCnnstr
            // 
            this.txtCnnstr.Location = new System.Drawing.Point(157, 10);
            this.txtCnnstr.Name = "txtCnnstr";
            this.txtCnnstr.Size = new System.Drawing.Size(694, 22);
            this.txtCnnstr.TabIndex = 2;
            // 
            // txtSubSystemId
            // 
            this.txtSubSystemId.Location = new System.Drawing.Point(157, 66);
            this.txtSubSystemId.Name = "txtSubSystemId";
            this.txtSubSystemId.Size = new System.Drawing.Size(149, 22);
            this.txtSubSystemId.TabIndex = 4;
            // 
            // lblSubSystemId
            // 
            this.lblSubSystemId.AutoSize = true;
            this.lblSubSystemId.Location = new System.Drawing.Point(13, 69);
            this.lblSubSystemId.Name = "lblSubSystemId";
            this.lblSubSystemId.Size = new System.Drawing.Size(98, 15);
            this.lblSubSystemId.TabIndex = 3;
            this.lblSubSystemId.Text = "サブシステムID：";
            // 
            // txtWorkflowName
            // 
            this.txtWorkflowName.Location = new System.Drawing.Point(414, 66);
            this.txtWorkflowName.Name = "txtWorkflowName";
            this.txtWorkflowName.Size = new System.Drawing.Size(149, 22);
            this.txtWorkflowName.TabIndex = 6;
            // 
            // lblWorkflowName
            // 
            this.lblWorkflowName.AutoSize = true;
            this.lblWorkflowName.Location = new System.Drawing.Point(312, 69);
            this.lblWorkflowName.Name = "lblWorkflowName";
            this.lblWorkflowName.Size = new System.Drawing.Size(96, 15);
            this.lblWorkflowName.TabIndex = 5;
            this.lblWorkflowName.Text = "ワークフロー名：";
            // 
            // txtDearSirUID
            // 
            this.txtDearSirUID.Location = new System.Drawing.Point(157, 94);
            this.txtDearSirUID.Name = "txtDearSirUID";
            this.txtDearSirUID.Size = new System.Drawing.Size(149, 22);
            this.txtDearSirUID.TabIndex = 8;
            // 
            // lblDearSirUID
            // 
            this.lblDearSirUID.AutoSize = true;
            this.lblDearSirUID.Location = new System.Drawing.Point(13, 97);
            this.lblDearSirUID.Name = "lblDearSirUID";
            this.lblDearSirUID.Size = new System.Drawing.Size(95, 15);
            this.lblDearSirUID.TabIndex = 7;
            this.lblDearSirUID.Text = "御中ユーザID：";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(640, 94);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(149, 22);
            this.txtUserID.TabIndex = 10;
            // 
            // lblUserID
            // 
            this.lblUserID.AutoSize = true;
            this.lblUserID.Location = new System.Drawing.Point(569, 97);
            this.lblUserID.Name = "lblUserID";
            this.lblUserID.Size = new System.Drawing.Size(65, 15);
            this.lblUserID.TabIndex = 9;
            this.lblUserID.Text = "ユーザID：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(16, 133);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(835, 150);
            this.dataGridView1.TabIndex = 11;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 91);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(821, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "新しいワークフローを開始します。";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtWorkflowControlNo
            // 
            this.txtWorkflowControlNo.Location = new System.Drawing.Point(157, 38);
            this.txtWorkflowControlNo.Name = "txtWorkflowControlNo";
            this.txtWorkflowControlNo.Size = new System.Drawing.Size(694, 22);
            this.txtWorkflowControlNo.TabIndex = 13;
            // 
            // lblWorkflowControlNo
            // 
            this.lblWorkflowControlNo.AutoSize = true;
            this.lblWorkflowControlNo.Location = new System.Drawing.Point(13, 41);
            this.lblWorkflowControlNo.Name = "lblWorkflowControlNo";
            this.lblWorkflowControlNo.Size = new System.Drawing.Size(141, 15);
            this.lblWorkflowControlNo.TabIndex = 12;
            this.lblWorkflowControlNo.Text = "ワークフロー管理番号：";
            // 
            // txtWorkflowReserveArea
            // 
            this.txtWorkflowReserveArea.Location = new System.Drawing.Point(150, 6);
            this.txtWorkflowReserveArea.Name = "txtWorkflowReserveArea";
            this.txtWorkflowReserveArea.Size = new System.Drawing.Size(671, 22);
            this.txtWorkflowReserveArea.TabIndex = 6;
            // 
            // lblWorkflowReserveArea
            // 
            this.lblWorkflowReserveArea.AutoSize = true;
            this.lblWorkflowReserveArea.Location = new System.Drawing.Point(6, 9);
            this.lblWorkflowReserveArea.Name = "lblWorkflowReserveArea";
            this.lblWorkflowReserveArea.Size = new System.Drawing.Size(94, 15);
            this.lblWorkflowReserveArea.TabIndex = 5;
            this.lblWorkflowReserveArea.Text = "WF予約領域：";
            // 
            // txtCurrentWorkflowReserveArea1
            // 
            this.txtCurrentWorkflowReserveArea1.Location = new System.Drawing.Point(150, 34);
            this.txtCurrentWorkflowReserveArea1.Name = "txtCurrentWorkflowReserveArea1";
            this.txtCurrentWorkflowReserveArea1.Size = new System.Drawing.Size(671, 22);
            this.txtCurrentWorkflowReserveArea1.TabIndex = 8;
            // 
            // lblCurrentWorkflowReserveArea1
            // 
            this.lblCurrentWorkflowReserveArea1.AutoSize = true;
            this.lblCurrentWorkflowReserveArea1.Location = new System.Drawing.Point(6, 37);
            this.lblCurrentWorkflowReserveArea1.Name = "lblCurrentWorkflowReserveArea1";
            this.lblCurrentWorkflowReserveArea1.Size = new System.Drawing.Size(138, 15);
            this.lblCurrentWorkflowReserveArea1.TabIndex = 7;
            this.lblCurrentWorkflowReserveArea1.Text = "カレントWF予約領域：";
            // 
            // lblReplyDeadline1
            // 
            this.lblReplyDeadline1.AutoSize = true;
            this.lblReplyDeadline1.Location = new System.Drawing.Point(6, 65);
            this.lblReplyDeadline1.Name = "lblReplyDeadline1";
            this.lblReplyDeadline1.Size = new System.Drawing.Size(90, 15);
            this.lblReplyDeadline1.TabIndex = 9;
            this.lblReplyDeadline1.Text = "回答希望日：";
            // 
            // dtpReplyDeadline1
            // 
            this.dtpReplyDeadline1.Location = new System.Drawing.Point(150, 65);
            this.dtpReplyDeadline1.Name = "dtpReplyDeadline1";
            this.dtpReplyDeadline1.Size = new System.Drawing.Size(671, 22);
            this.dtpReplyDeadline1.TabIndex = 10;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Location = new System.Drawing.Point(4, 67);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(827, 117);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "(3) ワークフロー依頼を取得します。";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(821, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "ワークフロー依頼を取得します。";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtDearSirPTitleId
            // 
            this.txtDearSirPTitleId.Location = new System.Drawing.Point(414, 94);
            this.txtDearSirPTitleId.Name = "txtDearSirPTitleId";
            this.txtDearSirPTitleId.Size = new System.Drawing.Size(149, 22);
            this.txtDearSirPTitleId.TabIndex = 15;
            // 
            // lblDearSirPTitleId
            // 
            this.lblDearSirPTitleId.AutoSize = true;
            this.lblDearSirPTitleId.Location = new System.Drawing.Point(312, 97);
            this.lblDearSirPTitleId.Name = "lblDearSirPTitleId";
            this.lblDearSirPTitleId.Size = new System.Drawing.Size(95, 15);
            this.lblDearSirPTitleId.TabIndex = 14;
            this.lblDearSirPTitleId.Text = "ユーザ職位ID：";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button4);
            this.tabPage4.Location = new System.Drawing.Point(4, 67);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(827, 117);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "(4) ワークフロー依頼を受付ます。";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(821, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "ワークフロー依頼を受付ます。";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button5);
            this.tabPage5.Location = new System.Drawing.Point(4, 67);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(827, 117);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "(5) 処理中ワークフロー依頼を取得します。";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(3, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(821, 23);
            this.button5.TabIndex = 1;
            this.button5.Text = "処理中ワークフロー依頼を取得します。";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.button6);
            this.tabPage6.Location = new System.Drawing.Point(4, 67);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(827, 117);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "(6) 次のワークフロー依頼を取得します。";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(3, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(821, 23);
            this.button6.TabIndex = 2;
            this.button6.Text = "次のワークフロー依頼を取得します。";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dtpReplyDeadline2);
            this.tabPage7.Controls.Add(this.lblReplyDeadline2);
            this.tabPage7.Controls.Add(this.txtCurrentWorkflowReserveArea2);
            this.tabPage7.Controls.Add(this.lblCurrentWorkflowReserveArea2);
            this.tabPage7.Controls.Add(this.button7);
            this.tabPage7.Location = new System.Drawing.Point(4, 67);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(827, 117);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "(7) ワークフロー承認を依頼します。";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(3, 91);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(821, 23);
            this.button7.TabIndex = 3;
            this.button7.Text = "ワークフロー承認を依頼します。";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // dtpReplyDeadline2
            // 
            this.dtpReplyDeadline2.Location = new System.Drawing.Point(150, 37);
            this.dtpReplyDeadline2.Name = "dtpReplyDeadline2";
            this.dtpReplyDeadline2.Size = new System.Drawing.Size(671, 22);
            this.dtpReplyDeadline2.TabIndex = 14;
            // 
            // lblReplyDeadline2
            // 
            this.lblReplyDeadline2.AutoSize = true;
            this.lblReplyDeadline2.Location = new System.Drawing.Point(6, 37);
            this.lblReplyDeadline2.Name = "lblReplyDeadline2";
            this.lblReplyDeadline2.Size = new System.Drawing.Size(90, 15);
            this.lblReplyDeadline2.TabIndex = 13;
            this.lblReplyDeadline2.Text = "回答希望日：";
            // 
            // txtCurrentWorkflowReserveArea2
            // 
            this.txtCurrentWorkflowReserveArea2.Location = new System.Drawing.Point(150, 6);
            this.txtCurrentWorkflowReserveArea2.Name = "txtCurrentWorkflowReserveArea2";
            this.txtCurrentWorkflowReserveArea2.Size = new System.Drawing.Size(671, 22);
            this.txtCurrentWorkflowReserveArea2.TabIndex = 12;
            // 
            // lblCurrentWorkflowReserveArea2
            // 
            this.lblCurrentWorkflowReserveArea2.AutoSize = true;
            this.lblCurrentWorkflowReserveArea2.Location = new System.Drawing.Point(6, 9);
            this.lblCurrentWorkflowReserveArea2.Name = "lblCurrentWorkflowReserveArea2";
            this.lblCurrentWorkflowReserveArea2.Size = new System.Drawing.Size(138, 15);
            this.lblCurrentWorkflowReserveArea2.TabIndex = 11;
            this.lblCurrentWorkflowReserveArea2.Text = "カレントWF予約領域：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 490);
            this.Controls.Add(this.txtDearSirPTitleId);
            this.Controls.Add(this.lblDearSirPTitleId);
            this.Controls.Add(this.txtWorkflowControlNo);
            this.Controls.Add(this.lblWorkflowControlNo);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.lblUserID);
            this.Controls.Add(this.txtDearSirUID);
            this.Controls.Add(this.lblDearSirUID);
            this.Controls.Add(this.txtWorkflowName);
            this.Controls.Add(this.lblWorkflowName);
            this.Controls.Add(this.txtSubSystemId);
            this.Controls.Add(this.lblSubSystemId);
            this.Controls.Add(this.txtCnnstr);
            this.Controls.Add(this.lblCnnstr);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "ワークフロー☆シミュレータ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblCnnstr;
        private System.Windows.Forms.TextBox txtCnnstr;
        private System.Windows.Forms.TextBox txtSubSystemId;
        private System.Windows.Forms.Label lblSubSystemId;
        private System.Windows.Forms.TextBox txtWorkflowName;
        private System.Windows.Forms.Label lblWorkflowName;
        private System.Windows.Forms.TextBox txtDearSirUID;
        private System.Windows.Forms.Label lblDearSirUID;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label lblUserID;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtWorkflowControlNo;
        private System.Windows.Forms.Label lblWorkflowControlNo;
        private System.Windows.Forms.DateTimePicker dtpReplyDeadline1;
        private System.Windows.Forms.Label lblReplyDeadline1;
        private System.Windows.Forms.TextBox txtCurrentWorkflowReserveArea1;
        private System.Windows.Forms.Label lblCurrentWorkflowReserveArea1;
        private System.Windows.Forms.TextBox txtWorkflowReserveArea;
        private System.Windows.Forms.Label lblWorkflowReserveArea;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtDearSirPTitleId;
        private System.Windows.Forms.Label lblDearSirPTitleId;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.DateTimePicker dtpReplyDeadline2;
        private System.Windows.Forms.Label lblReplyDeadline2;
        private System.Windows.Forms.TextBox txtCurrentWorkflowReserveArea2;
        private System.Windows.Forms.Label lblCurrentWorkflowReserveArea2;
    }
}

