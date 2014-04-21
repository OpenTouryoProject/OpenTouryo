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
            this.gbxDataProviders.Location = new System.Drawing.Point(19, 44);
            this.gbxDataProviders.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDataProviders.Name = "gbxDataProviders";
            this.gbxDataProviders.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDataProviders.Size = new System.Drawing.Size(700, 82);
            this.gbxDataProviders.TabIndex = 0;
            this.gbxDataProviders.TabStop = false;
            this.gbxDataProviders.Text = "データプロバイダを選択する";
            // 
            // rbnHiRDB
            // 
            this.rbnHiRDB.AutoSize = true;
            this.rbnHiRDB.Location = new System.Drawing.Point(555, 22);
            this.rbnHiRDB.Margin = new System.Windows.Forms.Padding(4);
            this.rbnHiRDB.Name = "rbnHiRDB";
            this.rbnHiRDB.Size = new System.Drawing.Size(100, 19);
            this.rbnHiRDB.TabIndex = 8;
            this.rbnHiRDB.TabStop = true;
            this.rbnHiRDB.Text = "HiRDB.NET";
            this.rbnHiRDB.UseVisualStyleBackColor = true;
            this.rbnHiRDB.CheckedChanged += new System.EventHandler(this.rbnHiRDB_CheckedChanged);
            // 
            // rbnODB
            // 
            this.rbnODB.AutoSize = true;
            this.rbnODB.Location = new System.Drawing.Point(161, 50);
            this.rbnODB.Margin = new System.Windows.Forms.Padding(4);
            this.rbnODB.Name = "rbnODB";
            this.rbnODB.Size = new System.Drawing.Size(99, 19);
            this.rbnODB.TabIndex = 7;
            this.rbnODB.TabStop = true;
            this.rbnODB.Text = "ODBC.NET";
            this.rbnODB.UseVisualStyleBackColor = true;
            this.rbnODB.CheckedChanged += new System.EventHandler(this.rbnODB_CheckedChanged);
            // 
            // rbnOLE
            // 
            this.rbnOLE.AutoSize = true;
            this.rbnOLE.Location = new System.Drawing.Point(27, 50);
            this.rbnOLE.Margin = new System.Windows.Forms.Padding(4);
            this.rbnOLE.Name = "rbnOLE";
            this.rbnOLE.Size = new System.Drawing.Size(105, 19);
            this.rbnOLE.TabIndex = 6;
            this.rbnOLE.TabStop = true;
            this.rbnOLE.Text = "OLEDB.NET";
            this.rbnOLE.UseVisualStyleBackColor = true;
            this.rbnOLE.CheckedChanged += new System.EventHandler(this.rbnOLE_CheckedChanged);
            // 
            // rbnPstgrs
            // 
            this.rbnPstgrs.AutoSize = true;
            this.rbnPstgrs.Location = new System.Drawing.Point(503, 50);
            this.rbnPstgrs.Margin = new System.Windows.Forms.Padding(4);
            this.rbnPstgrs.Name = "rbnPstgrs";
            this.rbnPstgrs.Size = new System.Drawing.Size(151, 19);
            this.rbnPstgrs.TabIndex = 5;
            this.rbnPstgrs.TabStop = true;
            this.rbnPstgrs.Text = "PostgreSQL Npgsql";
            this.rbnPstgrs.UseVisualStyleBackColor = true;
            this.rbnPstgrs.CheckedChanged += new System.EventHandler(this.rbnPstgrs_CheckedChanged);
            // 
            // rbnMySQL
            // 
            this.rbnMySQL.AutoSize = true;
            this.rbnMySQL.Location = new System.Drawing.Point(289, 50);
            this.rbnMySQL.Margin = new System.Windows.Forms.Padding(4);
            this.rbnMySQL.Name = "rbnMySQL";
            this.rbnMySQL.Size = new System.Drawing.Size(182, 19);
            this.rbnMySQL.TabIndex = 4;
            this.rbnMySQL.TabStop = true;
            this.rbnMySQL.Text = "MySQL Connector/NET";
            this.rbnMySQL.UseVisualStyleBackColor = true;
            this.rbnMySQL.CheckedChanged += new System.EventHandler(this.rdbMySQL_CheckedChanged);
            // 
            // rbnDB2
            // 
            this.rbnDB2.AutoSize = true;
            this.rbnDB2.Location = new System.Drawing.Point(416, 22);
            this.rbnDB2.Margin = new System.Windows.Forms.Padding(4);
            this.rbnDB2.Name = "rbnDB2";
            this.rbnDB2.Size = new System.Drawing.Size(86, 19);
            this.rbnDB2.TabIndex = 3;
            this.rbnDB2.TabStop = true;
            this.rbnDB2.Text = "DB2.NET";
            this.rbnDB2.UseVisualStyleBackColor = true;
            this.rbnDB2.CheckedChanged += new System.EventHandler(this.rdbDB2_CheckedChanged);
            // 
            // rbnODP
            // 
            this.rbnODP.AutoSize = true;
            this.rbnODP.Location = new System.Drawing.Point(227, 22);
            this.rbnODP.Margin = new System.Windows.Forms.Padding(4);
            this.rbnODP.Name = "rbnODP";
            this.rbnODP.Size = new System.Drawing.Size(135, 19);
            this.rbnODP.TabIndex = 2;
            this.rbnODP.Text = "Oracle ODP.NET";
            this.rbnODP.UseVisualStyleBackColor = true;
            this.rbnODP.CheckedChanged += new System.EventHandler(this.rdbODP_CheckedChanged);
            // 
            // rbnSQL
            // 
            this.rbnSQL.AutoSize = true;
            this.rbnSQL.Location = new System.Drawing.Point(27, 22);
            this.rbnSQL.Margin = new System.Windows.Forms.Padding(4);
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.Size = new System.Drawing.Size(145, 19);
            this.rbnSQL.TabIndex = 1;
            this.rbnSQL.TabStop = true;
            this.rbnSQL.Text = "SQL Server Client";
            this.rbnSQL.UseVisualStyleBackColor = true;
            this.rbnSQL.CheckedChanged += new System.EventHandler(this.rdbSQL_CheckedChanged);
            // 
            // btnListTable
            // 
            this.btnListTable.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnListTable.Location = new System.Drawing.Point(19, 206);
            this.btnListTable.Margin = new System.Windows.Forms.Padding(4);
            this.btnListTable.Name = "btnListTable";
            this.btnListTable.Size = new System.Drawing.Size(700, 31);
            this.btnListTable.TabIndex = 5;
            this.btnListTable.Text = "▼ DBに接続しテーブル一覧を取得する ▼";
            this.btnListTable.UseVisualStyleBackColor = true;
            this.btnListTable.Click += new System.EventHandler(this.btnListTable_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 146);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "接続文字列：";
            // 
            // txtConnString
            // 
            this.txtConnString.Location = new System.Drawing.Point(123, 142);
            this.txtConnString.Margin = new System.Windows.Forms.Padding(4);
            this.txtConnString.Name = "txtConnString";
            this.txtConnString.Size = new System.Drawing.Size(596, 22);
            this.txtConnString.TabIndex = 2;
            // 
            // lbxTables
            // 
            this.lbxTables.FormattingEnabled = true;
            this.lbxTables.ItemHeight = 15;
            this.lbxTables.Location = new System.Drawing.Point(19, 245);
            this.lbxTables.Margin = new System.Windows.Forms.Padding(4);
            this.lbxTables.Name = "lbxTables";
            this.lbxTables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxTables.Size = new System.Drawing.Size(700, 334);
            this.lbxTables.TabIndex = 6;
            // 
            // btnDaoDefinitionGen
            // 
            this.btnDaoDefinitionGen.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDaoDefinitionGen.Location = new System.Drawing.Point(426, 626);
            this.btnDaoDefinitionGen.Margin = new System.Windows.Forms.Padding(4);
            this.btnDaoDefinitionGen.Name = "btnDaoDefinitionGen";
            this.btnDaoDefinitionGen.Size = new System.Drawing.Size(293, 31);
            this.btnDaoDefinitionGen.TabIndex = 11;
            this.btnDaoDefinitionGen.Text = "▼ D層定義情報を生成する ▼";
            this.btnDaoDefinitionGen.UseVisualStyleBackColor = true;
            this.btnDaoDefinitionGen.Click += new System.EventHandler(this.btnDaoDefinitionGen_Click);
            // 
            // btnDaoAndSqlGen
            // 
            this.btnDaoAndSqlGen.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDaoAndSqlGen.Location = new System.Drawing.Point(19, 665);
            this.btnDaoAndSqlGen.Margin = new System.Windows.Forms.Padding(4);
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.Size = new System.Drawing.Size(700, 31);
            this.btnDaoAndSqlGen.TabIndex = 12;
            this.btnDaoAndSqlGen.Text = "～ Dao・SQLファイルを生成する ～";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = true;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // btnGetSchemaInfo
            // 
            this.btnGetSchemaInfo.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnGetSchemaInfo.Location = new System.Drawing.Point(406, 167);
            this.btnGetSchemaInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetSchemaInfo.Name = "btnGetSchemaInfo";
            this.btnGetSchemaInfo.Size = new System.Drawing.Size(313, 31);
            this.btnGetSchemaInfo.TabIndex = 4;
            this.btnGetSchemaInfo.Text = "スキーマ情報取得";
            this.btnGetSchemaInfo.UseVisualStyleBackColor = true;
            this.btnGetSchemaInfo.Click += new System.EventHandler(this.btnGetSchemaInfo_Click);
            // 
            // cmbSchemaInfo
            // 
            this.cmbSchemaInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchemaInfo.FormattingEnabled = true;
            this.cmbSchemaInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbSchemaInfo.Location = new System.Drawing.Point(19, 172);
            this.cmbSchemaInfo.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSchemaInfo.Name = "cmbSchemaInfo";
            this.cmbSchemaInfo.Size = new System.Drawing.Size(375, 23);
            this.cmbSchemaInfo.TabIndex = 3;
            // 
            // btnDelTable
            // 
            this.btnDelTable.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelTable.Location = new System.Drawing.Point(19, 587);
            this.btnDelTable.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelTable.Name = "btnDelTable";
            this.btnDelTable.Size = new System.Drawing.Size(213, 31);
            this.btnDelTable.TabIndex = 7;
            this.btnDelTable.Text = "選択したテーブルを削除";
            this.btnDelTable.UseVisualStyleBackColor = true;
            this.btnDelTable.Click += new System.EventHandler(this.btnDelTable_Click);
            // 
            // btnLoadColumn
            // 
            this.btnLoadColumn.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadColumn.Location = new System.Drawing.Point(240, 587);
            this.btnLoadColumn.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadColumn.Name = "btnLoadColumn";
            this.btnLoadColumn.Size = new System.Drawing.Size(178, 31);
            this.btnLoadColumn.TabIndex = 8;
            this.btnLoadColumn.Text = "列情報のロード";
            this.btnLoadColumn.UseVisualStyleBackColor = true;
            this.btnLoadColumn.Click += new System.EventHandler(this.btnLoadColumn_Click);
            // 
            // btnSetPrimaryKey
            // 
            this.btnSetPrimaryKey.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.btnSetPrimaryKey.Location = new System.Drawing.Point(426, 587);
            this.btnSetPrimaryKey.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetPrimaryKey.Name = "btnSetPrimaryKey";
            this.btnSetPrimaryKey.Size = new System.Drawing.Size(293, 31);
            this.btnSetPrimaryKey.TabIndex = 9;
            this.btnSetPrimaryKey.Text = "選択したテーブルの主キーを設定";
            this.btnSetPrimaryKey.UseVisualStyleBackColor = true;
            this.btnSetPrimaryKey.Click += new System.EventHandler(this.btnSetPrimaryKey_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Restart_ToolStripMenuItem,
            this.Close_ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(739, 31);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Restart_ToolStripMenuItem
            // 
            this.Restart_ToolStripMenuItem.Name = "Restart_ToolStripMenuItem";
            this.Restart_ToolStripMenuItem.Size = new System.Drawing.Size(157, 27);
            this.Restart_ToolStripMenuItem.Text = "やり直す（再起動）";
            this.Restart_ToolStripMenuItem.Click += new System.EventHandler(this.Restart_ToolStripMenuItem_Click);
            // 
            // Close_ToolStripMenuItem
            // 
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            this.Close_ToolStripMenuItem.Size = new System.Drawing.Size(67, 27);
            this.Close_ToolStripMenuItem.Text = "閉じる";
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(8, 8);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(1068, 625);
            this.dataGridView1.TabIndex = 13;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // cbxDebug
            // 
            this.cbxDebug.AutoSize = true;
            this.cbxDebug.Location = new System.Drawing.Point(751, 44);
            this.cbxDebug.Margin = new System.Windows.Forms.Padding(4);
            this.cbxDebug.Name = "cbxDebug";
            this.cbxDebug.Size = new System.Drawing.Size(75, 19);
            this.cbxDebug.TabIndex = 14;
            this.cbxDebug.Text = "デバッグ";
            this.cbxDebug.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(751, 71);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1095, 675);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1087, 646);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "タブ１";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1087, 646);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "タブ２";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView2.Location = new System.Drawing.Point(8, 8);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 21;
            this.dataGridView2.Size = new System.Drawing.Size(1068, 625);
            this.dataGridView2.TabIndex = 14;
            this.dataGridView2.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1087, 646);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "タブ３";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView3.Location = new System.Drawing.Point(8, 9);
            this.dataGridView3.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 21;
            this.dataGridView3.Size = new System.Drawing.Size(1068, 625);
            this.dataGridView3.TabIndex = 15;
            this.dataGridView3.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(751, 755);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(1095, 31);
            this.button1.TabIndex = 16;
            this.button1.Text = "グリッドをクリアする";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 634);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "エンコーディング：";
            // 
            // cmbEncoding
            // 
            this.cmbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoding.FormattingEnabled = true;
            this.cmbEncoding.Location = new System.Drawing.Point(125, 631);
            this.cmbEncoding.Name = "cmbEncoding";
            this.cmbEncoding.Size = new System.Drawing.Size(294, 23);
            this.cmbEncoding.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 709);
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
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "棟梁：D層自動生成ツール（墨壺） - D層定義情報ファイル生成";
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

