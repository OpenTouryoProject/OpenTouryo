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
            this.label1 = new System.Windows.Forms.Label();
            this.txtCnnstr = new System.Windows.Forms.TextBox();
            this.txtSubSystemId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWorkflowName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFromDearSirUID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFromUserID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.txtWorkflowControlNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtWorkflowReserveArea = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCurrentWorkflowReserveArea = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dtpReplyDeadline = new System.Windows.Forms.DateTimePicker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(16, 289);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 188);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(642, 159);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "新しいワークフローを準備します。";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dtpReplyDeadline);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.txtCurrentWorkflowReserveArea);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.txtWorkflowReserveArea);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(642, 159);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "新しいワークフローを開始します。";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(630, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "新しいワークフローを準備します。";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "接続文字列：";
            // 
            // txtCnnstr
            // 
            this.txtCnnstr.Location = new System.Drawing.Point(157, 10);
            this.txtCnnstr.Name = "txtCnnstr";
            this.txtCnnstr.Size = new System.Drawing.Size(505, 22);
            this.txtCnnstr.TabIndex = 2;
            // 
            // txtSubSystemId
            // 
            this.txtSubSystemId.Location = new System.Drawing.Point(157, 66);
            this.txtSubSystemId.Name = "txtSubSystemId";
            this.txtSubSystemId.Size = new System.Drawing.Size(149, 22);
            this.txtSubSystemId.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "サブシステムID：";
            // 
            // txtWorkflowName
            // 
            this.txtWorkflowName.Location = new System.Drawing.Point(414, 66);
            this.txtWorkflowName.Name = "txtWorkflowName";
            this.txtWorkflowName.Size = new System.Drawing.Size(149, 22);
            this.txtWorkflowName.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(312, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "ワークフロー名：";
            // 
            // txtFromDearSirUID
            // 
            this.txtFromDearSirUID.Location = new System.Drawing.Point(157, 94);
            this.txtFromDearSirUID.Name = "txtFromDearSirUID";
            this.txtFromDearSirUID.Size = new System.Drawing.Size(149, 22);
            this.txtFromDearSirUID.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "From御中ユーザID：";
            // 
            // txtFromUserID
            // 
            this.txtFromUserID.Location = new System.Drawing.Point(414, 94);
            this.txtFromUserID.Name = "txtFromUserID";
            this.txtFromUserID.Size = new System.Drawing.Size(149, 22);
            this.txtFromUserID.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(312, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "FromユーザID：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(16, 133);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(646, 150);
            this.dataGridView1.TabIndex = 11;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 130);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(630, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "新しいワークフローを開始します。";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtWorkflowControlNo
            // 
            this.txtWorkflowControlNo.Location = new System.Drawing.Point(157, 38);
            this.txtWorkflowControlNo.Name = "txtWorkflowControlNo";
            this.txtWorkflowControlNo.Size = new System.Drawing.Size(505, 22);
            this.txtWorkflowControlNo.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(141, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "ワークフロー管理番号：";
            // 
            // txtWorkflowReserveArea
            // 
            this.txtWorkflowReserveArea.Location = new System.Drawing.Point(150, 6);
            this.txtWorkflowReserveArea.Name = "txtWorkflowReserveArea";
            this.txtWorkflowReserveArea.Size = new System.Drawing.Size(486, 22);
            this.txtWorkflowReserveArea.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "WF予約領域：";
            // 
            // txtCurrentWorkflowReserveArea
            // 
            this.txtCurrentWorkflowReserveArea.Location = new System.Drawing.Point(150, 34);
            this.txtCurrentWorkflowReserveArea.Name = "txtCurrentWorkflowReserveArea";
            this.txtCurrentWorkflowReserveArea.Size = new System.Drawing.Size(486, 22);
            this.txtCurrentWorkflowReserveArea.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(138, 15);
            this.label8.TabIndex = 7;
            this.label8.Text = "カレントWF予約領域：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "回答希望日：";
            // 
            // dtpReplyDeadline
            // 
            this.dtpReplyDeadline.Location = new System.Drawing.Point(150, 65);
            this.dtpReplyDeadline.Name = "dtpReplyDeadline";
            this.dtpReplyDeadline.Size = new System.Drawing.Size(200, 22);
            this.dtpReplyDeadline.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 490);
            this.Controls.Add(this.txtWorkflowControlNo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtFromUserID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFromDearSirUID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWorkflowName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSubSystemId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCnnstr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCnnstr;
        private System.Windows.Forms.TextBox txtSubSystemId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWorkflowName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFromDearSirUID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFromUserID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtWorkflowControlNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpReplyDeadline;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCurrentWorkflowReserveArea;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWorkflowReserveArea;
        private System.Windows.Forms.Label label7;
    }
}

