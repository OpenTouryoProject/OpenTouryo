namespace DaoGen_Tool
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
            this.gbxDataProviders = new System.Windows.Forms.GroupBox();
            this.rbnHiRDB = new System.Windows.Forms.RadioButton();
            this.rbnODB = new System.Windows.Forms.RadioButton();
            this.rbnOLE = new System.Windows.Forms.RadioButton();
            this.rbnPstgrs = new System.Windows.Forms.RadioButton();
            this.rbnMySQL = new System.Windows.Forms.RadioButton();
            this.rbnDB2 = new System.Windows.Forms.RadioButton();
            this.rbnODP = new System.Windows.Forms.RadioButton();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.lbxTables = new System.Windows.Forms.ListBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cbxDebug = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.gbxSchemaDetails = new System.Windows.Forms.GroupBox();
            this.btnGetSchemaInfo = new System.Windows.Forms.Button();
            this.cmbSchemaInfo = new System.Windows.Forms.ComboBox();
            this.lblSchemaInfo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSetPrimaryKey = new System.Windows.Forms.Button();
            this.btnLoadColumn = new System.Windows.Forms.Button();
            this.btnDelTable = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbEncoding = new System.Windows.Forms.ComboBox();
            this.btnDaoDefinitionGen = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnListTable = new System.Windows.Forms.Button();
            this.btnDaoAndSqlGen = new System.Windows.Forms.Button();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.lnkHelp = new System.Windows.Forms.LinkLabel();
            this.gbxDataProviders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.gbxSchemaDetails.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxDataProviders
            // 
            resources.ApplyResources(this.gbxDataProviders, "gbxDataProviders");
            this.gbxDataProviders.Controls.Add(this.rbnHiRDB);
            this.gbxDataProviders.Controls.Add(this.rbnODB);
            this.gbxDataProviders.Controls.Add(this.rbnOLE);
            this.gbxDataProviders.Controls.Add(this.rbnPstgrs);
            this.gbxDataProviders.Controls.Add(this.rbnMySQL);
            this.gbxDataProviders.Controls.Add(this.rbnDB2);
            this.gbxDataProviders.Controls.Add(this.rbnODP);
            this.gbxDataProviders.Controls.Add(this.rbnSQL);
            this.gbxDataProviders.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbxDataProviders.Name = "gbxDataProviders";
            this.gbxDataProviders.TabStop = false;
            // 
            // rbnHiRDB
            // 
            resources.ApplyResources(this.rbnHiRDB, "rbnHiRDB");
            this.rbnHiRDB.Name = "rbnHiRDB";
            this.rbnHiRDB.TabStop = true;
            this.rbnHiRDB.UseVisualStyleBackColor = true;
            this.rbnHiRDB.CheckedChanged += new System.EventHandler(this.rbnHiRDB_CheckedChanged);
            // 
            // rbnODB
            // 
            resources.ApplyResources(this.rbnODB, "rbnODB");
            this.rbnODB.Name = "rbnODB";
            this.rbnODB.TabStop = true;
            this.rbnODB.UseVisualStyleBackColor = true;
            this.rbnODB.CheckedChanged += new System.EventHandler(this.rbnODB_CheckedChanged);
            // 
            // rbnOLE
            // 
            resources.ApplyResources(this.rbnOLE, "rbnOLE");
            this.rbnOLE.Name = "rbnOLE";
            this.rbnOLE.TabStop = true;
            this.rbnOLE.UseVisualStyleBackColor = true;
            this.rbnOLE.CheckedChanged += new System.EventHandler(this.rbnOLE_CheckedChanged);
            // 
            // rbnPstgrs
            // 
            resources.ApplyResources(this.rbnPstgrs, "rbnPstgrs");
            this.rbnPstgrs.Name = "rbnPstgrs";
            this.rbnPstgrs.TabStop = true;
            this.rbnPstgrs.UseVisualStyleBackColor = true;
            this.rbnPstgrs.CheckedChanged += new System.EventHandler(this.rbnPstgrs_CheckedChanged);
            // 
            // rbnMySQL
            // 
            resources.ApplyResources(this.rbnMySQL, "rbnMySQL");
            this.rbnMySQL.Name = "rbnMySQL";
            this.rbnMySQL.TabStop = true;
            this.rbnMySQL.UseVisualStyleBackColor = true;
            this.rbnMySQL.CheckedChanged += new System.EventHandler(this.rdbMySQL_CheckedChanged);
            // 
            // rbnDB2
            // 
            resources.ApplyResources(this.rbnDB2, "rbnDB2");
            this.rbnDB2.Name = "rbnDB2";
            this.rbnDB2.TabStop = true;
            this.rbnDB2.UseVisualStyleBackColor = true;
            this.rbnDB2.CheckedChanged += new System.EventHandler(this.rdbDB2_CheckedChanged);
            // 
            // rbnODP
            // 
            resources.ApplyResources(this.rbnODP, "rbnODP");
            this.rbnODP.Name = "rbnODP";
            this.rbnODP.TabStop = true;
            this.rbnODP.UseVisualStyleBackColor = true;
            this.rbnODP.CheckedChanged += new System.EventHandler(this.rdbODP_CheckedChanged);
            // 
            // rbnSQL
            // 
            resources.ApplyResources(this.rbnSQL, "rbnSQL");
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.UseVisualStyleBackColor = true;
            this.rbnSQL.CheckedChanged += new System.EventHandler(this.rdbSQL_CheckedChanged);
            // 
            // lblConnectionString
            // 
            resources.ApplyResources(this.lblConnectionString, "lblConnectionString");
            this.lblConnectionString.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnectionString.Name = "lblConnectionString";
            // 
            // txtConnString
            // 
            resources.ApplyResources(this.txtConnString, "txtConnString");
            this.txtConnString.Name = "txtConnString";
            // 
            // lbxTables
            // 
            resources.ApplyResources(this.lbxTables, "lbxTables");
            this.lbxTables.FormattingEnabled = true;
            this.lbxTables.Name = "lbxTables";
            this.lbxTables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // cbxDebug
            // 
            resources.ApplyResources(this.cbxDebug, "cbxDebug");
            this.cbxDebug.Name = "cbxDebug";
            this.cbxDebug.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            resources.ApplyResources(this.dataGridView2, "dataGridView2");
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 21;
            this.dataGridView2.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            resources.ApplyResources(this.dataGridView3, "dataGridView3");
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 21;
            this.dataGridView3.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // gbxSchemaDetails
            // 
            resources.ApplyResources(this.gbxSchemaDetails, "gbxSchemaDetails");
            this.gbxSchemaDetails.Controls.Add(this.btnGetSchemaInfo);
            this.gbxSchemaDetails.Controls.Add(this.cmbSchemaInfo);
            this.gbxSchemaDetails.Controls.Add(this.lblSchemaInfo);
            this.gbxSchemaDetails.Name = "gbxSchemaDetails";
            this.gbxSchemaDetails.TabStop = false;
            // 
            // btnGetSchemaInfo
            // 
            resources.ApplyResources(this.btnGetSchemaInfo, "btnGetSchemaInfo");
            this.btnGetSchemaInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(155)))), ((int)(((byte)(213)))));
            this.btnGetSchemaInfo.FlatAppearance.BorderSize = 0;
            this.btnGetSchemaInfo.ForeColor = System.Drawing.Color.White;
            this.btnGetSchemaInfo.Name = "btnGetSchemaInfo";
            this.btnGetSchemaInfo.UseVisualStyleBackColor = false;
            this.btnGetSchemaInfo.EnabledChanged += new System.EventHandler(this.btnLoadColumn_EnabledChanged);
            this.btnGetSchemaInfo.Click += new System.EventHandler(this.btnGetSchemaInfo_Click);
            // 
            // cmbSchemaInfo
            // 
            resources.ApplyResources(this.cmbSchemaInfo, "cmbSchemaInfo");
            this.cmbSchemaInfo.FormattingEnabled = true;
            this.cmbSchemaInfo.Name = "cmbSchemaInfo";
            // 
            // lblSchemaInfo
            // 
            resources.ApplyResources(this.lblSchemaInfo, "lblSchemaInfo");
            this.lblSchemaInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSchemaInfo.Name = "lblSchemaInfo";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Name = "label4";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btnSetPrimaryKey);
            this.groupBox1.Controls.Add(this.btnLoadColumn);
            this.groupBox1.Controls.Add(this.btnDelTable);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnSetPrimaryKey
            // 
            resources.ApplyResources(this.btnSetPrimaryKey, "btnSetPrimaryKey");
            this.btnSetPrimaryKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(202)))), ((int)(((byte)(207)))));
            this.btnSetPrimaryKey.FlatAppearance.BorderSize = 0;
            this.btnSetPrimaryKey.ForeColor = System.Drawing.Color.White;
            this.btnSetPrimaryKey.Name = "btnSetPrimaryKey";
            this.btnSetPrimaryKey.UseVisualStyleBackColor = false;
            this.btnSetPrimaryKey.EnabledChanged += new System.EventHandler(this.btnLoadColumn_EnabledChanged);
            this.btnSetPrimaryKey.Click += new System.EventHandler(this.btnSetPrimaryKey_Click);
            // 
            // btnLoadColumn
            // 
            resources.ApplyResources(this.btnLoadColumn, "btnLoadColumn");
            this.btnLoadColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(155)))), ((int)(((byte)(213)))));
            this.btnLoadColumn.FlatAppearance.BorderSize = 0;
            this.btnLoadColumn.ForeColor = System.Drawing.Color.White;
            this.btnLoadColumn.Name = "btnLoadColumn";
            this.btnLoadColumn.UseVisualStyleBackColor = false;
            this.btnLoadColumn.EnabledChanged += new System.EventHandler(this.btnLoadColumn_EnabledChanged);
            this.btnLoadColumn.Click += new System.EventHandler(this.btnLoadColumn_Click);
            // 
            // btnDelTable
            // 
            resources.ApplyResources(this.btnDelTable, "btnDelTable");
            this.btnDelTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(155)))), ((int)(((byte)(213)))));
            this.btnDelTable.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDelTable.FlatAppearance.BorderSize = 0;
            this.btnDelTable.ForeColor = System.Drawing.Color.White;
            this.btnDelTable.Name = "btnDelTable";
            this.btnDelTable.UseVisualStyleBackColor = false;
            this.btnDelTable.EnabledChanged += new System.EventHandler(this.btnLoadColumn_EnabledChanged);
            this.btnDelTable.Click += new System.EventHandler(this.btnDelTable_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cmbEncoding
            // 
            resources.ApplyResources(this.cmbEncoding, "cmbEncoding");
            this.cmbEncoding.FormattingEnabled = true;
            this.cmbEncoding.Name = "cmbEncoding";
            // 
            // btnDaoDefinitionGen
            // 
            resources.ApplyResources(this.btnDaoDefinitionGen, "btnDaoDefinitionGen");
            this.btnDaoDefinitionGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(42)))), ((int)(((byte)(53)))));
            this.btnDaoDefinitionGen.FlatAppearance.BorderSize = 0;
            this.btnDaoDefinitionGen.ForeColor = System.Drawing.Color.White;
            this.btnDaoDefinitionGen.Name = "btnDaoDefinitionGen";
            this.btnDaoDefinitionGen.UseVisualStyleBackColor = false;
            this.btnDaoDefinitionGen.EnabledChanged += new System.EventHandler(this.btnDaoDefinitionGen_EnabledChanged);
            this.btnDaoDefinitionGen.Click += new System.EventHandler(this.btnDaoDefinitionGen_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // btnListTable
            // 
            resources.ApplyResources(this.btnListTable, "btnListTable");
            this.btnListTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(42)))), ((int)(((byte)(53)))));
            this.btnListTable.ForeColor = System.Drawing.Color.White;
            this.btnListTable.Name = "btnListTable";
            this.btnListTable.UseVisualStyleBackColor = false;
            this.btnListTable.Click += new System.EventHandler(this.btnListTable_Click);
            // 
            // btnDaoAndSqlGen
            // 
            resources.ApplyResources(this.btnDaoAndSqlGen, "btnDaoAndSqlGen");
            this.btnDaoAndSqlGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(42)))), ((int)(((byte)(53)))));
            this.btnDaoAndSqlGen.ForeColor = System.Drawing.Color.White;
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = false;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // lblStep1
            // 
            resources.ApplyResources(this.lblStep1, "lblStep1");
            this.lblStep1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(123)))), ((int)(((byte)(155)))));
            this.lblStep1.ForeColor = System.Drawing.Color.White;
            this.lblStep1.Name = "lblStep1";
            // 
            // lnkHelp
            // 
            resources.ApplyResources(this.lnkHelp, "lnkHelp");
            this.lnkHelp.ActiveLinkColor = System.Drawing.SystemColors.Highlight;
            this.lnkHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(123)))), ((int)(((byte)(155)))));
            this.lnkHelp.ForeColor = System.Drawing.Color.White;
            this.lnkHelp.LinkColor = System.Drawing.Color.White;
            this.lnkHelp.Name = "lnkHelp";
            this.lnkHelp.TabStop = true;
            this.lnkHelp.VisitedLinkColor = System.Drawing.SystemColors.Highlight;
            this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkHelp);
            this.Controls.Add(this.lblStep1);
            this.Controls.Add(this.btnDaoAndSqlGen);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnDaoDefinitionGen);
            this.Controls.Add(this.cmbEncoding);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnListTable);
            this.Controls.Add(this.gbxSchemaDetails);
            this.Controls.Add(this.gbxDataProviders);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cbxDebug);
            this.Controls.Add(this.lbxTables);
            this.Controls.Add(this.txtConnString);
            this.Controls.Add(this.lblConnectionString);
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbxDataProviders.ResumeLayout(false);
            this.gbxDataProviders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.gbxSchemaDetails.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxDataProviders;
        private System.Windows.Forms.RadioButton rbnDB2;
        private System.Windows.Forms.RadioButton rbnODP;
        private System.Windows.Forms.RadioButton rbnSQL;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox txtConnString;
        private System.Windows.Forms.ListBox lbxTables;
        private System.Windows.Forms.RadioButton rbnMySQL;
        private System.Windows.Forms.RadioButton rbnPstgrs;
        private System.Windows.Forms.RadioButton rbnODB;
        private System.Windows.Forms.RadioButton rbnOLE;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox cbxDebug;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.RadioButton rbnHiRDB;

        private System.Windows.Forms.GroupBox gbxSchemaDetails;
        private System.Windows.Forms.ComboBox cmbSchemaInfo;
        private System.Windows.Forms.Label lblSchemaInfo;
        private System.Windows.Forms.Button btnGetSchemaInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbEncoding;
        private System.Windows.Forms.Button btnDaoDefinitionGen;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnListTable;
        private System.Windows.Forms.Button btnLoadColumn;
        private System.Windows.Forms.Button btnSetPrimaryKey;
        private System.Windows.Forms.Button btnDaoAndSqlGen;
        private System.Windows.Forms.Button btnDelTable;
        private System.Windows.Forms.Label lblStep1;
        private System.Windows.Forms.LinkLabel lnkHelp;


    }
}

