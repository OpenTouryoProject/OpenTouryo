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
            this.btnListTable = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.lbxTables = new System.Windows.Forms.ListBox();
            this.btnDaoDefinitionGen = new System.Windows.Forms.Button();
            this.btnDaoAndSqlGen = new System.Windows.Forms.Button();
            this.btnGetSchemaInfo = new System.Windows.Forms.Button();
            this.cmbSchemaInfo = new System.Windows.Forms.ComboBox();
            this.btnDelTable = new System.Windows.Forms.Button();
            this.btnLoadColumn = new System.Windows.Forms.Button();
            this.btnSetPrimaryKey = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Restart_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Close_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cbxDebug = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbEncoding = new System.Windows.Forms.ComboBox();
            this.gbxDataProviders.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxDataProviders
            // 
            this.gbxDataProviders.Controls.Add(this.rbnHiRDB);
            this.gbxDataProviders.Controls.Add(this.rbnODB);
            this.gbxDataProviders.Controls.Add(this.rbnOLE);
            this.gbxDataProviders.Controls.Add(this.rbnPstgrs);
            this.gbxDataProviders.Controls.Add(this.rbnMySQL);
            this.gbxDataProviders.Controls.Add(this.rbnDB2);
            this.gbxDataProviders.Controls.Add(this.rbnODP);
            this.gbxDataProviders.Controls.Add(this.rbnSQL);
            resources.ApplyResources(this.gbxDataProviders, "gbxDataProviders");
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
            this.rbnODP.UseVisualStyleBackColor = true;
            this.rbnODP.CheckedChanged += new System.EventHandler(this.rdbODP_CheckedChanged);
            // 
            // rbnSQL
            // 
            resources.ApplyResources(this.rbnSQL, "rbnSQL");
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.TabStop = true;
            this.rbnSQL.UseVisualStyleBackColor = true;
            this.rbnSQL.CheckedChanged += new System.EventHandler(this.rdbSQL_CheckedChanged);
            // 
            // btnListTable
            // 
            resources.ApplyResources(this.btnListTable, "btnListTable");
            this.btnListTable.Name = "btnListTable";
            this.btnListTable.UseVisualStyleBackColor = true;
            this.btnListTable.Click += new System.EventHandler(this.btnListTable_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtConnString
            // 
            resources.ApplyResources(this.txtConnString, "txtConnString");
            this.txtConnString.Name = "txtConnString";
            // 
            // lbxTables
            // 
            this.lbxTables.FormattingEnabled = true;
            resources.ApplyResources(this.lbxTables, "lbxTables");
            this.lbxTables.Name = "lbxTables";
            this.lbxTables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // btnDaoDefinitionGen
            // 
            resources.ApplyResources(this.btnDaoDefinitionGen, "btnDaoDefinitionGen");
            this.btnDaoDefinitionGen.Name = "btnDaoDefinitionGen";
            this.btnDaoDefinitionGen.UseVisualStyleBackColor = true;
            this.btnDaoDefinitionGen.Click += new System.EventHandler(this.btnDaoDefinitionGen_Click);
            // 
            // btnDaoAndSqlGen
            // 
            resources.ApplyResources(this.btnDaoAndSqlGen, "btnDaoAndSqlGen");
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = true;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // btnGetSchemaInfo
            // 
            resources.ApplyResources(this.btnGetSchemaInfo, "btnGetSchemaInfo");
            this.btnGetSchemaInfo.Name = "btnGetSchemaInfo";
            this.btnGetSchemaInfo.UseVisualStyleBackColor = true;
            this.btnGetSchemaInfo.Click += new System.EventHandler(this.btnGetSchemaInfo_Click);
            // 
            // cmbSchemaInfo
            // 
            this.cmbSchemaInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchemaInfo.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSchemaInfo, "cmbSchemaInfo");
            this.cmbSchemaInfo.Name = "cmbSchemaInfo";
            // 
            // btnDelTable
            // 
            resources.ApplyResources(this.btnDelTable, "btnDelTable");
            this.btnDelTable.Name = "btnDelTable";
            this.btnDelTable.UseVisualStyleBackColor = true;
            this.btnDelTable.Click += new System.EventHandler(this.btnDelTable_Click);
            // 
            // btnLoadColumn
            // 
            resources.ApplyResources(this.btnLoadColumn, "btnLoadColumn");
            this.btnLoadColumn.Name = "btnLoadColumn";
            this.btnLoadColumn.UseVisualStyleBackColor = true;
            this.btnLoadColumn.Click += new System.EventHandler(this.btnLoadColumn_Click);
            // 
            // btnSetPrimaryKey
            // 
            resources.ApplyResources(this.btnSetPrimaryKey, "btnSetPrimaryKey");
            this.btnSetPrimaryKey.Name = "btnSetPrimaryKey";
            this.btnSetPrimaryKey.UseVisualStyleBackColor = true;
            this.btnSetPrimaryKey.Click += new System.EventHandler(this.btnSetPrimaryKey_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Restart_ToolStripMenuItem,
            this.Close_ToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // Restart_ToolStripMenuItem
            // 
            this.Restart_ToolStripMenuItem.Name = "Restart_ToolStripMenuItem";
            resources.ApplyResources(this.Restart_ToolStripMenuItem, "Restart_ToolStripMenuItem");
            this.Restart_ToolStripMenuItem.Click += new System.EventHandler(this.Restart_ToolStripMenuItem_Click);
            // 
            // Close_ToolStripMenuItem
            // 
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            resources.ApplyResources(this.Close_ToolStripMenuItem, "Close_ToolStripMenuItem");
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            resources.ApplyResources(this.dataGridView2, "dataGridView2");
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 21;
            this.dataGridView2.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridView3);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            resources.ApplyResources(this.dataGridView3, "dataGridView3");
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 21;
            this.dataGridView3.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmbEncoding
            // 
            this.cmbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbEncoding, "cmbEncoding");
            this.cmbEncoding.Name = "cmbEncoding";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbEncoding);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cbxDebug);
            this.Controls.Add(this.btnSetPrimaryKey);
            this.Controls.Add(this.btnLoadColumn);
            this.Controls.Add(this.btnDelTable);
            this.Controls.Add(this.cmbSchemaInfo);
            this.Controls.Add(this.btnGetSchemaInfo);
            this.Controls.Add(this.btnDaoAndSqlGen);
            this.Controls.Add(this.btnDaoDefinitionGen);
            this.Controls.Add(this.lbxTables);
            this.Controls.Add(this.txtConnString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnListTable);
            this.Controls.Add(this.gbxDataProviders);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbxDataProviders.ResumeLayout(false);
            this.gbxDataProviders.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxDataProviders;
        private System.Windows.Forms.RadioButton rbnDB2;
        private System.Windows.Forms.RadioButton rbnODP;
        private System.Windows.Forms.RadioButton rbnSQL;
        private System.Windows.Forms.Button btnListTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConnString;
        private System.Windows.Forms.ListBox lbxTables;
        private System.Windows.Forms.Button btnDaoDefinitionGen;
        private System.Windows.Forms.Button btnDaoAndSqlGen;
        private System.Windows.Forms.Button btnGetSchemaInfo;
        private System.Windows.Forms.ComboBox cmbSchemaInfo;
        private System.Windows.Forms.Button btnDelTable;
        private System.Windows.Forms.Button btnLoadColumn;
        private System.Windows.Forms.Button btnSetPrimaryKey;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Restart_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Close_ToolStripMenuItem;
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.RadioButton rbnHiRDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbEncoding;
    }
}

