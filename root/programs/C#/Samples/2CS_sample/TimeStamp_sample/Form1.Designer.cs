namespace _TimeStamp_sample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTSColType = new System.Windows.Forms.ComboBox();
            this.cmbTableType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVAL = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTS = new System.Windows.Forms.TextBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDynDel = new System.Windows.Forms.Button();
            this.btnDynUpd = new System.Windows.Forms.Button();
            this.btnDynIns = new System.Windows.Forms.Button();
            this.btnDynSel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClearTS = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            // 
            // btnGetAll
            // 
            resources.ApplyResources(this.btnGetAll, "btnGetAll");
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.UseVisualStyleBackColor = true;
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtID
            // 
            resources.ApplyResources(this.txtID, "txtID");
            this.txtID.Name = "txtID";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cmbTSColType
            // 
            this.cmbTSColType.FormattingEnabled = true;
            this.cmbTSColType.Items.AddRange(new object[] {
            resources.GetString("cmbTSColType.Items"),
            resources.GetString("cmbTSColType.Items1")});
            resources.ApplyResources(this.cmbTSColType, "cmbTSColType");
            this.cmbTSColType.Name = "cmbTSColType";
            // 
            // cmbTableType
            // 
            this.cmbTableType.FormattingEnabled = true;
            this.cmbTableType.Items.AddRange(new object[] {
            resources.GetString("cmbTableType.Items"),
            resources.GetString("cmbTableType.Items1"),
            resources.GetString("cmbTableType.Items2")});
            resources.ApplyResources(this.cmbTableType, "cmbTableType");
            this.cmbTableType.Name = "cmbTableType";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtVAL
            // 
            resources.ApplyResources(this.txtVAL, "txtVAL");
            this.txtVAL.Name = "txtVAL";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtTS
            // 
            resources.ApplyResources(this.txtTS, "txtTS");
            this.txtTS.Name = "txtTS";
            this.txtTS.ReadOnly = true;
            // 
            // btnInsert
            // 
            resources.ApplyResources(this.btnInsert, "btnInsert");
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnSelect
            // 
            resources.ApplyResources(this.btnSelect, "btnSelect");
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnDynDel
            // 
            resources.ApplyResources(this.btnDynDel, "btnDynDel");
            this.btnDynDel.Name = "btnDynDel";
            this.btnDynDel.UseVisualStyleBackColor = true;
            this.btnDynDel.Click += new System.EventHandler(this.btnDynDel_Click);
            // 
            // btnDynUpd
            // 
            resources.ApplyResources(this.btnDynUpd, "btnDynUpd");
            this.btnDynUpd.Name = "btnDynUpd";
            this.btnDynUpd.UseVisualStyleBackColor = true;
            this.btnDynUpd.Click += new System.EventHandler(this.btnDynUpd_Click);
            // 
            // btnDynIns
            // 
            resources.ApplyResources(this.btnDynIns, "btnDynIns");
            this.btnDynIns.Name = "btnDynIns";
            this.btnDynIns.UseVisualStyleBackColor = true;
            this.btnDynIns.Click += new System.EventHandler(this.btnDynIns_Click);
            // 
            // btnDynSel
            // 
            resources.ApplyResources(this.btnDynSel, "btnDynSel");
            this.btnDynSel.Name = "btnDynSel";
            this.btnDynSel.UseVisualStyleBackColor = true;
            this.btnDynSel.Click += new System.EventHandler(this.btnDynSel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInsert);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Controls.Add(this.btnDelete);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDynIns);
            this.groupBox2.Controls.Add(this.btnDynSel);
            this.groupBox2.Controls.Add(this.btnDynDel);
            this.groupBox2.Controls.Add(this.btnDynUpd);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnClearTS
            // 
            resources.ApplyResources(this.btnClearTS, "btnClearTS");
            this.btnClearTS.Name = "btnClearTS";
            this.btnClearTS.UseVisualStyleBackColor = true;
            this.btnClearTS.Click += new System.EventHandler(this.btnClearTS_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClearTS);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTS);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtVAL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbTableType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbTSColType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnGetAll);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnGetAll;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTSColType;
        private System.Windows.Forms.ComboBox cmbTableType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVAL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTS;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnDynDel;
        private System.Windows.Forms.Button btnDynUpd;
        private System.Windows.Forms.Button btnDynIns;
        private System.Windows.Forms.Button btnDynSel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClearTS;

    }
}

