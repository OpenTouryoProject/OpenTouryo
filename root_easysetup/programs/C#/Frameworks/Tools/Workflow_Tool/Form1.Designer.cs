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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dtpReplyDeadline1 = new System.Windows.Forms.DateTimePicker();
            this.lblReplyDeadline1 = new System.Windows.Forms.Label();
            this.txtCurrentWorkflowReserveArea1 = new System.Windows.Forms.TextBox();
            this.lblCurrentWorkflowReserveArea1 = new System.Windows.Forms.Label();
            this.txtWorkflowReserveArea = new System.Windows.Forms.TextBox();
            this.lblWorkflowReserveArea = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.btnTurnbackIntoFirst = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.dtpReplyDeadline2 = new System.Windows.Forms.DateTimePicker();
            this.lblReplyDeadline2 = new System.Windows.Forms.Label();
            this.txtCurrentWorkflowReserveArea2 = new System.Windows.Forms.TextBox();
            this.lblCurrentWorkflowReserveArea2 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
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
            this.txtWorkflowControlNo = new System.Windows.Forms.TextBox();
            this.lblWorkflowControlNo = new System.Windows.Forms.Label();
            this.txtDearSirPTitleId = new System.Windows.Forms.TextBox();
            this.lblDearSirPTitleId = new System.Windows.Forms.Label();
            this.txtToUserID = new System.Windows.Forms.TextBox();
            this.lblToUserID = new System.Windows.Forms.Label();
            this.btnForcedTermination = new System.Windows.Forms.Button();
            this.btnSwitchPersonInCharge = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dtpReplyDeadline1
            // 
            resources.ApplyResources(this.dtpReplyDeadline1, "dtpReplyDeadline1");
            this.dtpReplyDeadline1.Name = "dtpReplyDeadline1";
            // 
            // lblReplyDeadline1
            // 
            resources.ApplyResources(this.lblReplyDeadline1, "lblReplyDeadline1");
            this.lblReplyDeadline1.Name = "lblReplyDeadline1";
            // 
            // txtCurrentWorkflowReserveArea1
            // 
            resources.ApplyResources(this.txtCurrentWorkflowReserveArea1, "txtCurrentWorkflowReserveArea1");
            this.txtCurrentWorkflowReserveArea1.Name = "txtCurrentWorkflowReserveArea1";
            // 
            // lblCurrentWorkflowReserveArea1
            // 
            resources.ApplyResources(this.lblCurrentWorkflowReserveArea1, "lblCurrentWorkflowReserveArea1");
            this.lblCurrentWorkflowReserveArea1.Name = "lblCurrentWorkflowReserveArea1";
            // 
            // txtWorkflowReserveArea
            // 
            resources.ApplyResources(this.txtWorkflowReserveArea, "txtWorkflowReserveArea");
            this.txtWorkflowReserveArea.Name = "txtWorkflowReserveArea";
            // 
            // lblWorkflowReserveArea
            // 
            resources.ApplyResources(this.lblWorkflowReserveArea, "lblWorkflowReserveArea");
            this.lblWorkflowReserveArea.Name = "lblWorkflowReserveArea";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button3);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button4);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button5);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            resources.ApplyResources(this.button5, "button5");
            this.button5.Name = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.button6);
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            resources.ApplyResources(this.button6, "button6");
            this.button6.Name = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.btnSwitchPersonInCharge);
            this.tabPage7.Controls.Add(this.btnForcedTermination);
            this.tabPage7.Controls.Add(this.btnTurnbackIntoFirst);
            this.tabPage7.Controls.Add(this.checkBox1);
            this.tabPage7.Controls.Add(this.dtpReplyDeadline2);
            this.tabPage7.Controls.Add(this.lblReplyDeadline2);
            this.tabPage7.Controls.Add(this.txtCurrentWorkflowReserveArea2);
            this.tabPage7.Controls.Add(this.lblCurrentWorkflowReserveArea2);
            this.tabPage7.Controls.Add(this.button7);
            resources.ApplyResources(this.tabPage7, "tabPage7");
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // btnTurnbackIntoFirst
            // 
            resources.ApplyResources(this.btnTurnbackIntoFirst, "btnTurnbackIntoFirst");
            this.btnTurnbackIntoFirst.Name = "btnTurnbackIntoFirst";
            this.btnTurnbackIntoFirst.UseVisualStyleBackColor = true;
            this.btnTurnbackIntoFirst.Click += new System.EventHandler(this.btnTurnbackIntoFirst_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // dtpReplyDeadline2
            // 
            resources.ApplyResources(this.dtpReplyDeadline2, "dtpReplyDeadline2");
            this.dtpReplyDeadline2.Name = "dtpReplyDeadline2";
            // 
            // lblReplyDeadline2
            // 
            resources.ApplyResources(this.lblReplyDeadline2, "lblReplyDeadline2");
            this.lblReplyDeadline2.Name = "lblReplyDeadline2";
            // 
            // txtCurrentWorkflowReserveArea2
            // 
            resources.ApplyResources(this.txtCurrentWorkflowReserveArea2, "txtCurrentWorkflowReserveArea2");
            this.txtCurrentWorkflowReserveArea2.Name = "txtCurrentWorkflowReserveArea2";
            // 
            // lblCurrentWorkflowReserveArea2
            // 
            resources.ApplyResources(this.lblCurrentWorkflowReserveArea2, "lblCurrentWorkflowReserveArea2");
            this.lblCurrentWorkflowReserveArea2.Name = "lblCurrentWorkflowReserveArea2";
            // 
            // button7
            // 
            resources.ApplyResources(this.button7, "button7");
            this.button7.Name = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // lblCnnstr
            // 
            resources.ApplyResources(this.lblCnnstr, "lblCnnstr");
            this.lblCnnstr.Name = "lblCnnstr";
            // 
            // txtCnnstr
            // 
            resources.ApplyResources(this.txtCnnstr, "txtCnnstr");
            this.txtCnnstr.Name = "txtCnnstr";
            // 
            // txtSubSystemId
            // 
            resources.ApplyResources(this.txtSubSystemId, "txtSubSystemId");
            this.txtSubSystemId.Name = "txtSubSystemId";
            // 
            // lblSubSystemId
            // 
            resources.ApplyResources(this.lblSubSystemId, "lblSubSystemId");
            this.lblSubSystemId.Name = "lblSubSystemId";
            // 
            // txtWorkflowName
            // 
            resources.ApplyResources(this.txtWorkflowName, "txtWorkflowName");
            this.txtWorkflowName.Name = "txtWorkflowName";
            // 
            // lblWorkflowName
            // 
            resources.ApplyResources(this.lblWorkflowName, "lblWorkflowName");
            this.lblWorkflowName.Name = "lblWorkflowName";
            // 
            // txtDearSirUID
            // 
            resources.ApplyResources(this.txtDearSirUID, "txtDearSirUID");
            this.txtDearSirUID.Name = "txtDearSirUID";
            // 
            // lblDearSirUID
            // 
            resources.ApplyResources(this.lblDearSirUID, "lblDearSirUID");
            this.lblDearSirUID.Name = "lblDearSirUID";
            // 
            // txtUserID
            // 
            resources.ApplyResources(this.txtUserID, "txtUserID");
            this.txtUserID.Name = "txtUserID";
            // 
            // lblUserID
            // 
            resources.ApplyResources(this.lblUserID, "lblUserID");
            this.lblUserID.Name = "lblUserID";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // txtWorkflowControlNo
            // 
            resources.ApplyResources(this.txtWorkflowControlNo, "txtWorkflowControlNo");
            this.txtWorkflowControlNo.Name = "txtWorkflowControlNo";
            // 
            // lblWorkflowControlNo
            // 
            resources.ApplyResources(this.lblWorkflowControlNo, "lblWorkflowControlNo");
            this.lblWorkflowControlNo.Name = "lblWorkflowControlNo";
            // 
            // txtDearSirPTitleId
            // 
            resources.ApplyResources(this.txtDearSirPTitleId, "txtDearSirPTitleId");
            this.txtDearSirPTitleId.Name = "txtDearSirPTitleId";
            // 
            // lblDearSirPTitleId
            // 
            resources.ApplyResources(this.lblDearSirPTitleId, "lblDearSirPTitleId");
            this.lblDearSirPTitleId.Name = "lblDearSirPTitleId";
            // 
            // txtToUserID
            // 
            resources.ApplyResources(this.txtToUserID, "txtToUserID");
            this.txtToUserID.Name = "txtToUserID";
            // 
            // lblToUserID
            // 
            resources.ApplyResources(this.lblToUserID, "lblToUserID");
            this.lblToUserID.Name = "lblToUserID";
            // 
            // btnForcedTermination
            // 
            resources.ApplyResources(this.btnForcedTermination, "btnForcedTermination");
            this.btnForcedTermination.Name = "btnForcedTermination";
            this.btnForcedTermination.UseVisualStyleBackColor = true;
            this.btnForcedTermination.Click += new System.EventHandler(this.btnForcedTermination_Click);
            // 
            // btnSwitchPersonInCharge
            // 
            resources.ApplyResources(this.btnSwitchPersonInCharge, "btnSwitchPersonInCharge");
            this.btnSwitchPersonInCharge.Name = "btnSwitchPersonInCharge";
            this.btnSwitchPersonInCharge.UseVisualStyleBackColor = true;
            this.btnSwitchPersonInCharge.Click += new System.EventHandler(this.btnSwitchPersonInCharge_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtToUserID);
            this.Controls.Add(this.lblToUserID);
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
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtToUserID;
        private System.Windows.Forms.Label lblToUserID;
        private System.Windows.Forms.Button btnTurnbackIntoFirst;
        private System.Windows.Forms.Button btnForcedTermination;
        private System.Windows.Forms.Button btnSwitchPersonInCharge;
    }
}

