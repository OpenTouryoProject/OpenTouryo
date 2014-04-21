namespace DaoGen_Tool
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbnMySQL = new System.Windows.Forms.RadioButton();
            this.gbxDataProviders = new System.Windows.Forms.GroupBox();
            this.rbnODB = new System.Windows.Forms.RadioButton();
            this.rbnOLE = new System.Windows.Forms.RadioButton();
            this.rbnPstgrs = new System.Windows.Forms.RadioButton();
            this.rbnDB2 = new System.Windows.Forms.RadioButton();
            this.rbnODP = new System.Windows.Forms.RadioButton();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Close_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSetDaoDefinition = new System.Windows.Forms.TextBox();
            this.btnSetDaoDefinition = new System.Windows.Forms.Button();
            this.lblSetDaoDefinition = new System.Windows.Forms.Label();
            this.lblSetSourceTemplate = new System.Windows.Forms.Label();
            this.btnSetSourceTemplate = new System.Windows.Forms.Button();
            this.txtSetSourceTemplate = new System.Windows.Forms.TextBox();
            this.gbxInput = new System.Windows.Forms.GroupBox();
            this.gbxSqlTemplateEncoding = new System.Windows.Forms.GroupBox();
            this.cmbSTEncoding = new System.Windows.Forms.ComboBox();
            this.gbxDaoTemplateLanguage = new System.Windows.Forms.GroupBox();
            this.rbnDTL_VB = new System.Windows.Forms.RadioButton();
            this.rbnDTL_CS = new System.Windows.Forms.RadioButton();
            this.gbxDaoTemplateEncoding = new System.Windows.Forms.GroupBox();
            this.cmbDTEncoding = new System.Windows.Forms.ComboBox();
            this.cbxDaoDefinitionHeader = new System.Windows.Forms.CheckBox();
            this.gbxDaoDefinitionEncoding = new System.Windows.Forms.GroupBox();
            this.cmbDDEncoding = new System.Windows.Forms.ComboBox();
            this.btnDaoAndSqlGen = new System.Windows.Forms.Button();
            this.gbxOutput = new System.Windows.Forms.GroupBox();
            this.lblSetOutput = new System.Windows.Forms.Label();
            this.btnSetOutput = new System.Windows.Forms.Button();
            this.gbxSQLFileEncoding = new System.Windows.Forms.GroupBox();
            this.cmbSFEncoding = new System.Windows.Forms.ComboBox();
            this.gbxDaoFileEncoding = new System.Windows.Forms.GroupBox();
            this.cmbDFEncoding = new System.Windows.Forms.ComboBox();
            this.txtSetOutput = new System.Windows.Forms.TextBox();
            this.lblFamilyName = new System.Windows.Forms.Label();
            this.txtFamilyName = new System.Windows.Forms.TextBox();
            this.txtPersonalName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTimeStampUpdMethod = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTimeStampColName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxTSIndisp = new System.Windows.Forms.CheckBox();
            this.gbxDTO = new System.Windows.Forms.GroupBox();
            this.cbxOnlyDTO = new System.Windows.Forms.CheckBox();
            this.cbxTypedDataSet = new System.Windows.Forms.CheckBox();
            this.cbxEntity = new System.Windows.Forms.CheckBox();
            this.gbxOracleLikeSetting = new System.Windows.Forms.GroupBox();
            this.txtEscapeChar = new System.Windows.Forms.TextBox();
            this.lblEscapeChar = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkEscapeToNChar = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbLikeStatement = new System.Windows.Forms.ComboBox();
            this.cbxOnlyTableMaintenance = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxTableMaintenance = new System.Windows.Forms.CheckBox();
            this.gbxDataProviders.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbxInput.SuspendLayout();
            this.gbxSqlTemplateEncoding.SuspendLayout();
            this.gbxDaoTemplateLanguage.SuspendLayout();
            this.gbxDaoTemplateEncoding.SuspendLayout();
            this.gbxDaoDefinitionEncoding.SuspendLayout();
            this.gbxOutput.SuspendLayout();
            this.gbxSQLFileEncoding.SuspendLayout();
            this.gbxDaoFileEncoding.SuspendLayout();
            this.gbxDTO.SuspendLayout();
            this.gbxOracleLikeSetting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbnMySQL
            // 
            this.rbnMySQL.AutoSize = true;
            this.rbnMySQL.Location = new System.Drawing.Point(289, 50);
            this.rbnMySQL.Margin = new System.Windows.Forms.Padding(4);
            this.rbnMySQL.Name = "rbnMySQL";
            this.rbnMySQL.Size = new System.Drawing.Size(182, 19);
            this.rbnMySQL.TabIndex = 3;
            this.rbnMySQL.TabStop = true;
            this.rbnMySQL.Text = "MySQL Connector/NET";
            this.rbnMySQL.UseVisualStyleBackColor = true;
            // 
            // gbxDataProviders
            // 
            this.gbxDataProviders.Controls.Add(this.rbnODB);
            this.gbxDataProviders.Controls.Add(this.rbnOLE);
            this.gbxDataProviders.Controls.Add(this.rbnPstgrs);
            this.gbxDataProviders.Controls.Add(this.rbnMySQL);
            this.gbxDataProviders.Controls.Add(this.rbnDB2);
            this.gbxDataProviders.Controls.Add(this.rbnODP);
            this.gbxDataProviders.Controls.Add(this.rbnSQL);
            this.gbxDataProviders.Location = new System.Drawing.Point(4, 7);
            this.gbxDataProviders.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDataProviders.Name = "gbxDataProviders";
            this.gbxDataProviders.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDataProviders.Size = new System.Drawing.Size(700, 82);
            this.gbxDataProviders.TabIndex = 0;
            this.gbxDataProviders.TabStop = false;
            this.gbxDataProviders.Text = "データプロバイダを選択する";
            // 
            // rbnODB
            // 
            this.rbnODB.AutoSize = true;
            this.rbnODB.Location = new System.Drawing.Point(161, 50);
            this.rbnODB.Margin = new System.Windows.Forms.Padding(4);
            this.rbnODB.Name = "rbnODB";
            this.rbnODB.Size = new System.Drawing.Size(99, 19);
            this.rbnODB.TabIndex = 16;
            this.rbnODB.Text = "ODBC.NET";
            this.rbnODB.UseVisualStyleBackColor = true;
            // 
            // rbnOLE
            // 
            this.rbnOLE.AutoSize = true;
            this.rbnOLE.Location = new System.Drawing.Point(27, 50);
            this.rbnOLE.Margin = new System.Windows.Forms.Padding(4);
            this.rbnOLE.Name = "rbnOLE";
            this.rbnOLE.Size = new System.Drawing.Size(105, 19);
            this.rbnOLE.TabIndex = 15;
            this.rbnOLE.Text = "OLEDB.NET";
            this.rbnOLE.UseVisualStyleBackColor = true;
            // 
            // rbnPstgrs
            // 
            this.rbnPstgrs.AutoSize = true;
            this.rbnPstgrs.Location = new System.Drawing.Point(503, 50);
            this.rbnPstgrs.Margin = new System.Windows.Forms.Padding(4);
            this.rbnPstgrs.Name = "rbnPstgrs";
            this.rbnPstgrs.Size = new System.Drawing.Size(151, 19);
            this.rbnPstgrs.TabIndex = 14;
            this.rbnPstgrs.TabStop = true;
            this.rbnPstgrs.Text = "PostgreSQL Npgsql";
            this.rbnPstgrs.UseVisualStyleBackColor = true;
            // 
            // rbnDB2
            // 
            this.rbnDB2.AutoSize = true;
            this.rbnDB2.Location = new System.Drawing.Point(503, 22);
            this.rbnDB2.Margin = new System.Windows.Forms.Padding(4);
            this.rbnDB2.Name = "rbnDB2";
            this.rbnDB2.Size = new System.Drawing.Size(86, 19);
            this.rbnDB2.TabIndex = 2;
            this.rbnDB2.TabStop = true;
            this.rbnDB2.Text = "DB2.NET";
            this.rbnDB2.UseVisualStyleBackColor = true;
            // 
            // rbnODP
            // 
            this.rbnODP.AutoSize = true;
            this.rbnODP.Location = new System.Drawing.Point(289, 22);
            this.rbnODP.Margin = new System.Windows.Forms.Padding(4);
            this.rbnODP.Name = "rbnODP";
            this.rbnODP.Size = new System.Drawing.Size(135, 19);
            this.rbnODP.TabIndex = 1;
            this.rbnODP.TabStop = true;
            this.rbnODP.Text = "Oracle ODP.NET";
            this.rbnODP.UseVisualStyleBackColor = true;
            this.rbnODP.CheckedChanged += new System.EventHandler(this.rbnODP_CheckedChanged);
            // 
            // rbnSQL
            // 
            this.rbnSQL.AutoSize = true;
            this.rbnSQL.Location = new System.Drawing.Point(27, 22);
            this.rbnSQL.Margin = new System.Windows.Forms.Padding(4);
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.Size = new System.Drawing.Size(145, 19);
            this.rbnSQL.TabIndex = 0;
            this.rbnSQL.TabStop = true;
            this.rbnSQL.Text = "SQL Server Client";
            this.rbnSQL.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Close_ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(754, 31);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Close_ToolStripMenuItem
            // 
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            this.Close_ToolStripMenuItem.Size = new System.Drawing.Size(67, 27);
            this.Close_ToolStripMenuItem.Text = "閉じる";
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // txtSetDaoDefinition
            // 
            this.txtSetDaoDefinition.Location = new System.Drawing.Point(159, 25);
            this.txtSetDaoDefinition.Margin = new System.Windows.Forms.Padding(4);
            this.txtSetDaoDefinition.Name = "txtSetDaoDefinition";
            this.txtSetDaoDefinition.Size = new System.Drawing.Size(417, 22);
            this.txtSetDaoDefinition.TabIndex = 1;
            // 
            // btnSetDaoDefinition
            // 
            this.btnSetDaoDefinition.Location = new System.Drawing.Point(585, 22);
            this.btnSetDaoDefinition.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetDaoDefinition.Name = "btnSetDaoDefinition";
            this.btnSetDaoDefinition.Size = new System.Drawing.Size(107, 29);
            this.btnSetDaoDefinition.TabIndex = 2;
            this.btnSetDaoDefinition.Text = "パスを指定";
            this.btnSetDaoDefinition.UseVisualStyleBackColor = true;
            this.btnSetDaoDefinition.Click += new System.EventHandler(this.btnSetDaoDefinition_Click);
            // 
            // lblSetDaoDefinition
            // 
            this.lblSetDaoDefinition.AutoSize = true;
            this.lblSetDaoDefinition.Location = new System.Drawing.Point(8, 29);
            this.lblSetDaoDefinition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSetDaoDefinition.Name = "lblSetDaoDefinition";
            this.lblSetDaoDefinition.Size = new System.Drawing.Size(134, 15);
            this.lblSetDaoDefinition.TabIndex = 0;
            this.lblSetDaoDefinition.Text = "D層定義情報ファイル";
            // 
            // lblSetSourceTemplate
            // 
            this.lblSetSourceTemplate.AutoSize = true;
            this.lblSetSourceTemplate.Location = new System.Drawing.Point(8, 146);
            this.lblSetSourceTemplate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSetSourceTemplate.Name = "lblSetSourceTemplate";
            this.lblSetSourceTemplate.Size = new System.Drawing.Size(160, 15);
            this.lblSetSourceTemplate.TabIndex = 5;
            this.lblSetSourceTemplate.Text = "ソース テンプレート ファイル";
            // 
            // btnSetSourceTemplate
            // 
            this.btnSetSourceTemplate.Location = new System.Drawing.Point(489, 140);
            this.btnSetSourceTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetSourceTemplate.Name = "btnSetSourceTemplate";
            this.btnSetSourceTemplate.Size = new System.Drawing.Size(203, 29);
            this.btnSetSourceTemplate.TabIndex = 7;
            this.btnSetSourceTemplate.Text = "ルート フォルダ パスを指定";
            this.btnSetSourceTemplate.UseVisualStyleBackColor = true;
            this.btnSetSourceTemplate.Click += new System.EventHandler(this.btnSetSourceTemplate_Click);
            // 
            // txtSetSourceTemplate
            // 
            this.txtSetSourceTemplate.Location = new System.Drawing.Point(188, 142);
            this.txtSetSourceTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.txtSetSourceTemplate.Name = "txtSetSourceTemplate";
            this.txtSetSourceTemplate.Size = new System.Drawing.Size(292, 22);
            this.txtSetSourceTemplate.TabIndex = 6;
            // 
            // gbxInput
            // 
            this.gbxInput.Controls.Add(this.gbxSqlTemplateEncoding);
            this.gbxInput.Controls.Add(this.gbxDaoTemplateLanguage);
            this.gbxInput.Controls.Add(this.gbxDaoTemplateEncoding);
            this.gbxInput.Controls.Add(this.cbxDaoDefinitionHeader);
            this.gbxInput.Controls.Add(this.gbxDaoDefinitionEncoding);
            this.gbxInput.Controls.Add(this.btnSetDaoDefinition);
            this.gbxInput.Controls.Add(this.lblSetSourceTemplate);
            this.gbxInput.Controls.Add(this.txtSetDaoDefinition);
            this.gbxInput.Controls.Add(this.btnSetSourceTemplate);
            this.gbxInput.Controls.Add(this.lblSetDaoDefinition);
            this.gbxInput.Controls.Add(this.txtSetSourceTemplate);
            this.gbxInput.Location = new System.Drawing.Point(7, 7);
            this.gbxInput.Margin = new System.Windows.Forms.Padding(4);
            this.gbxInput.Name = "gbxInput";
            this.gbxInput.Padding = new System.Windows.Forms.Padding(4);
            this.gbxInput.Size = new System.Drawing.Size(700, 315);
            this.gbxInput.TabIndex = 1;
            this.gbxInput.TabStop = false;
            this.gbxInput.Text = "入力設定";
            // 
            // gbxSqlTemplateEncoding
            // 
            this.gbxSqlTemplateEncoding.Controls.Add(this.cmbSTEncoding);
            this.gbxSqlTemplateEncoding.Location = new System.Drawing.Point(373, 246);
            this.gbxSqlTemplateEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.gbxSqlTemplateEncoding.Name = "gbxSqlTemplateEncoding";
            this.gbxSqlTemplateEncoding.Padding = new System.Windows.Forms.Padding(4);
            this.gbxSqlTemplateEncoding.Size = new System.Drawing.Size(319, 56);
            this.gbxSqlTemplateEncoding.TabIndex = 10;
            this.gbxSqlTemplateEncoding.TabStop = false;
            this.gbxSqlTemplateEncoding.Text = "SQLファイル入力のエンコーディングを選択する";
            // 
            // cmbSTEncoding
            // 
            this.cmbSTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSTEncoding.FormattingEnabled = true;
            this.cmbSTEncoding.Location = new System.Drawing.Point(7, 26);
            this.cmbSTEncoding.Name = "cmbSTEncoding";
            this.cmbSTEncoding.Size = new System.Drawing.Size(305, 23);
            this.cmbSTEncoding.TabIndex = 2;
            // 
            // gbxDaoTemplateLanguage
            // 
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_VB);
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_CS);
            this.gbxDaoTemplateLanguage.Location = new System.Drawing.Point(28, 179);
            this.gbxDaoTemplateLanguage.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDaoTemplateLanguage.Name = "gbxDaoTemplateLanguage";
            this.gbxDaoTemplateLanguage.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDaoTemplateLanguage.Size = new System.Drawing.Size(319, 56);
            this.gbxDaoTemplateLanguage.TabIndex = 8;
            this.gbxDaoTemplateLanguage.TabStop = false;
            this.gbxDaoTemplateLanguage.Text = "言語を選択する";
            // 
            // rbnDTL_VB
            // 
            this.rbnDTL_VB.AutoSize = true;
            this.rbnDTL_VB.Location = new System.Drawing.Point(195, 22);
            this.rbnDTL_VB.Margin = new System.Windows.Forms.Padding(4);
            this.rbnDTL_VB.Name = "rbnDTL_VB";
            this.rbnDTL_VB.Size = new System.Drawing.Size(47, 19);
            this.rbnDTL_VB.TabIndex = 1;
            this.rbnDTL_VB.TabStop = true;
            this.rbnDTL_VB.Text = "VB";
            this.rbnDTL_VB.UseVisualStyleBackColor = true;
            // 
            // rbnDTL_CS
            // 
            this.rbnDTL_CS.AutoSize = true;
            this.rbnDTL_CS.Location = new System.Drawing.Point(87, 22);
            this.rbnDTL_CS.Margin = new System.Windows.Forms.Padding(4);
            this.rbnDTL_CS.Name = "rbnDTL_CS";
            this.rbnDTL_CS.Size = new System.Drawing.Size(46, 19);
            this.rbnDTL_CS.TabIndex = 0;
            this.rbnDTL_CS.TabStop = true;
            this.rbnDTL_CS.Text = "C#";
            this.rbnDTL_CS.UseVisualStyleBackColor = true;
            // 
            // gbxDaoTemplateEncoding
            // 
            this.gbxDaoTemplateEncoding.Controls.Add(this.cmbDTEncoding);
            this.gbxDaoTemplateEncoding.Location = new System.Drawing.Point(28, 246);
            this.gbxDaoTemplateEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDaoTemplateEncoding.Name = "gbxDaoTemplateEncoding";
            this.gbxDaoTemplateEncoding.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDaoTemplateEncoding.Size = new System.Drawing.Size(319, 56);
            this.gbxDaoTemplateEncoding.TabIndex = 9;
            this.gbxDaoTemplateEncoding.TabStop = false;
            this.gbxDaoTemplateEncoding.Text = "Daoファイル入力のエンコーディングを選択する";
            // 
            // cmbDTEncoding
            // 
            this.cmbDTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDTEncoding.FormattingEnabled = true;
            this.cmbDTEncoding.Location = new System.Drawing.Point(7, 26);
            this.cmbDTEncoding.Name = "cmbDTEncoding";
            this.cmbDTEncoding.Size = new System.Drawing.Size(305, 23);
            this.cmbDTEncoding.TabIndex = 1;
            // 
            // cbxDaoDefinitionHeader
            // 
            this.cbxDaoDefinitionHeader.AutoSize = true;
            this.cbxDaoDefinitionHeader.Location = new System.Drawing.Point(384, 86);
            this.cbxDaoDefinitionHeader.Margin = new System.Windows.Forms.Padding(4);
            this.cbxDaoDefinitionHeader.Name = "cbxDaoDefinitionHeader";
            this.cbxDaoDefinitionHeader.Size = new System.Drawing.Size(245, 19);
            this.cbxDaoDefinitionHeader.TabIndex = 4;
            this.cbxDaoDefinitionHeader.Text = "ヘッダありの場合（１行目を無視する）";
            this.cbxDaoDefinitionHeader.UseVisualStyleBackColor = true;
            // 
            // gbxDaoDefinitionEncoding
            // 
            this.gbxDaoDefinitionEncoding.Controls.Add(this.cmbDDEncoding);
            this.gbxDaoDefinitionEncoding.Location = new System.Drawing.Point(28, 64);
            this.gbxDaoDefinitionEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDaoDefinitionEncoding.Name = "gbxDaoDefinitionEncoding";
            this.gbxDaoDefinitionEncoding.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDaoDefinitionEncoding.Size = new System.Drawing.Size(319, 56);
            this.gbxDaoDefinitionEncoding.TabIndex = 3;
            this.gbxDaoDefinitionEncoding.TabStop = false;
            this.gbxDaoDefinitionEncoding.Text = "入力エンコーディングを選択する";
            // 
            // cmbDDEncoding
            // 
            this.cmbDDEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDDEncoding.FormattingEnabled = true;
            this.cmbDDEncoding.Location = new System.Drawing.Point(7, 22);
            this.cmbDDEncoding.Name = "cmbDDEncoding";
            this.cmbDDEncoding.Size = new System.Drawing.Size(305, 23);
            this.cmbDDEncoding.TabIndex = 0;
            // 
            // btnDaoAndSqlGen
            // 
            this.btnDaoAndSqlGen.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDaoAndSqlGen.Location = new System.Drawing.Point(20, 551);
            this.btnDaoAndSqlGen.Margin = new System.Windows.Forms.Padding(4);
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.Size = new System.Drawing.Size(721, 31);
            this.btnDaoAndSqlGen.TabIndex = 7;
            this.btnDaoAndSqlGen.Text = "－ Dao・SQL、DTOファイルを生成する －";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = true;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // gbxOutput
            // 
            this.gbxOutput.Controls.Add(this.lblSetOutput);
            this.gbxOutput.Controls.Add(this.btnSetOutput);
            this.gbxOutput.Controls.Add(this.gbxSQLFileEncoding);
            this.gbxOutput.Controls.Add(this.gbxDaoFileEncoding);
            this.gbxOutput.Controls.Add(this.txtSetOutput);
            this.gbxOutput.Location = new System.Drawing.Point(7, 330);
            this.gbxOutput.Margin = new System.Windows.Forms.Padding(4);
            this.gbxOutput.Name = "gbxOutput";
            this.gbxOutput.Padding = new System.Windows.Forms.Padding(4);
            this.gbxOutput.Size = new System.Drawing.Size(700, 122);
            this.gbxOutput.TabIndex = 2;
            this.gbxOutput.TabStop = false;
            this.gbxOutput.Text = "出力設定";
            // 
            // lblSetOutput
            // 
            this.lblSetOutput.AutoSize = true;
            this.lblSetOutput.Location = new System.Drawing.Point(8, 29);
            this.lblSetOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSetOutput.Name = "lblSetOutput";
            this.lblSetOutput.Size = new System.Drawing.Size(79, 15);
            this.lblSetOutput.TabIndex = 0;
            this.lblSetOutput.Text = "出力ファイル";
            // 
            // btnSetOutput
            // 
            this.btnSetOutput.Location = new System.Drawing.Point(489, 22);
            this.btnSetOutput.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetOutput.Name = "btnSetOutput";
            this.btnSetOutput.Size = new System.Drawing.Size(203, 29);
            this.btnSetOutput.TabIndex = 2;
            this.btnSetOutput.Text = "ルート フォルダ パスを指定";
            this.btnSetOutput.UseVisualStyleBackColor = true;
            this.btnSetOutput.Click += new System.EventHandler(this.btnSetOutput_Click);
            // 
            // gbxSQLFileEncoding
            // 
            this.gbxSQLFileEncoding.Controls.Add(this.cmbSFEncoding);
            this.gbxSQLFileEncoding.Location = new System.Drawing.Point(373, 59);
            this.gbxSQLFileEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.gbxSQLFileEncoding.Name = "gbxSQLFileEncoding";
            this.gbxSQLFileEncoding.Padding = new System.Windows.Forms.Padding(4);
            this.gbxSQLFileEncoding.Size = new System.Drawing.Size(319, 56);
            this.gbxSQLFileEncoding.TabIndex = 4;
            this.gbxSQLFileEncoding.TabStop = false;
            this.gbxSQLFileEncoding.Text = "SQLファイル出力のエンコーディングを選択する";
            // 
            // cmbSFEncoding
            // 
            this.cmbSFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSFEncoding.FormattingEnabled = true;
            this.cmbSFEncoding.Location = new System.Drawing.Point(7, 26);
            this.cmbSFEncoding.Name = "cmbSFEncoding";
            this.cmbSFEncoding.Size = new System.Drawing.Size(305, 23);
            this.cmbSFEncoding.TabIndex = 3;
            // 
            // gbxDaoFileEncoding
            // 
            this.gbxDaoFileEncoding.Controls.Add(this.cmbDFEncoding);
            this.gbxDaoFileEncoding.Location = new System.Drawing.Point(27, 59);
            this.gbxDaoFileEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDaoFileEncoding.Name = "gbxDaoFileEncoding";
            this.gbxDaoFileEncoding.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDaoFileEncoding.Size = new System.Drawing.Size(320, 56);
            this.gbxDaoFileEncoding.TabIndex = 3;
            this.gbxDaoFileEncoding.TabStop = false;
            this.gbxDaoFileEncoding.Text = "Daoファイル出力のエンコーディングを選択する";
            // 
            // cmbDFEncoding
            // 
            this.cmbDFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDFEncoding.FormattingEnabled = true;
            this.cmbDFEncoding.Location = new System.Drawing.Point(8, 26);
            this.cmbDFEncoding.Name = "cmbDFEncoding";
            this.cmbDFEncoding.Size = new System.Drawing.Size(305, 23);
            this.cmbDFEncoding.TabIndex = 2;
            // 
            // txtSetOutput
            // 
            this.txtSetOutput.Location = new System.Drawing.Point(100, 25);
            this.txtSetOutput.Margin = new System.Windows.Forms.Padding(4);
            this.txtSetOutput.Name = "txtSetOutput";
            this.txtSetOutput.Size = new System.Drawing.Size(380, 22);
            this.txtSetOutput.TabIndex = 1;
            // 
            // lblFamilyName
            // 
            this.lblFamilyName.AutoSize = true;
            this.lblFamilyName.Location = new System.Drawing.Point(111, 115);
            this.lblFamilyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFamilyName.Name = "lblFamilyName";
            this.lblFamilyName.Size = new System.Drawing.Size(30, 15);
            this.lblFamilyName.TabIndex = 3;
            this.lblFamilyName.Text = "姓：";
            // 
            // txtFamilyName
            // 
            this.txtFamilyName.Location = new System.Drawing.Point(148, 112);
            this.txtFamilyName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFamilyName.Name = "txtFamilyName";
            this.txtFamilyName.Size = new System.Drawing.Size(183, 22);
            this.txtFamilyName.TabIndex = 4;
            // 
            // txtPersonalName
            // 
            this.txtPersonalName.Location = new System.Drawing.Point(460, 112);
            this.txtPersonalName.Margin = new System.Windows.Forms.Padding(4);
            this.txtPersonalName.Name = "txtPersonalName";
            this.txtPersonalName.Size = new System.Drawing.Size(231, 22);
            this.txtPersonalName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(422, 115);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "名：";
            // 
            // txtTimeStampUpdMethod
            // 
            this.txtTimeStampUpdMethod.Location = new System.Drawing.Point(460, 35);
            this.txtTimeStampUpdMethod.Margin = new System.Windows.Forms.Padding(4);
            this.txtTimeStampUpdMethod.Name = "txtTimeStampUpdMethod";
            this.txtTimeStampUpdMethod.Size = new System.Drawing.Size(231, 22);
            this.txtTimeStampUpdMethod.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(377, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "更新方法：";
            // 
            // txtTimeStampColName
            // 
            this.txtTimeStampColName.Location = new System.Drawing.Point(148, 35);
            this.txtTimeStampColName.Margin = new System.Windows.Forms.Padding(4);
            this.txtTimeStampColName.Name = "txtTimeStampColName";
            this.txtTimeStampColName.Size = new System.Drawing.Size(183, 22);
            this.txtTimeStampColName.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 39);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "タイムスタンプ列名：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 69);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 30);
            this.label4.TabIndex = 12;
            this.label4.Text = "Update、Deleteクエリの検索条件に指定するタイムスタンプ\r\nを必須にする場合は、右のチェック ボックスをオンに設定する";
            // 
            // cbxTSIndisp
            // 
            this.cbxTSIndisp.AutoSize = true;
            this.cbxTSIndisp.Location = new System.Drawing.Point(545, 75);
            this.cbxTSIndisp.Margin = new System.Windows.Forms.Padding(4);
            this.cbxTSIndisp.Name = "cbxTSIndisp";
            this.cbxTSIndisp.Size = new System.Drawing.Size(135, 19);
            this.cbxTSIndisp.TabIndex = 13;
            this.cbxTSIndisp.Text = "タイムスタンプ必須";
            this.cbxTSIndisp.UseVisualStyleBackColor = true;
            // 
            // gbxDTO
            // 
            this.gbxDTO.Controls.Add(this.cbxOnlyDTO);
            this.gbxDTO.Controls.Add(this.cbxTypedDataSet);
            this.gbxDTO.Controls.Add(this.cbxEntity);
            this.gbxDTO.Location = new System.Drawing.Point(7, 371);
            this.gbxDTO.Margin = new System.Windows.Forms.Padding(4);
            this.gbxDTO.Name = "gbxDTO";
            this.gbxDTO.Padding = new System.Windows.Forms.Padding(4);
            this.gbxDTO.Size = new System.Drawing.Size(700, 50);
            this.gbxDTO.TabIndex = 14;
            this.gbxDTO.TabStop = false;
            this.gbxDTO.Text = "項目移送用の型を生成する（エンコーディングはDaoと同じになる";
            // 
            // cbxOnlyDTO
            // 
            this.cbxOnlyDTO.AutoSize = true;
            this.cbxOnlyDTO.Location = new System.Drawing.Point(100, 22);
            this.cbxOnlyDTO.Margin = new System.Windows.Forms.Padding(4);
            this.cbxOnlyDTO.Name = "cbxOnlyDTO";
            this.cbxOnlyDTO.Size = new System.Drawing.Size(84, 19);
            this.cbxOnlyDTO.TabIndex = 18;
            this.cbxOnlyDTO.Text = "DTOのみ";
            this.cbxOnlyDTO.UseVisualStyleBackColor = true;
            this.cbxOnlyDTO.CheckedChanged += new System.EventHandler(this.cbxOnlyDTO_CheckedChanged);
            // 
            // cbxTypedDataSet
            // 
            this.cbxTypedDataSet.AutoSize = true;
            this.cbxTypedDataSet.Location = new System.Drawing.Point(504, 22);
            this.cbxTypedDataSet.Margin = new System.Windows.Forms.Padding(4);
            this.cbxTypedDataSet.Name = "cbxTypedDataSet";
            this.cbxTypedDataSet.Size = new System.Drawing.Size(136, 19);
            this.cbxTypedDataSet.TabIndex = 17;
            this.cbxTypedDataSet.Text = "型付きデータセット";
            this.cbxTypedDataSet.UseVisualStyleBackColor = true;
            // 
            // cbxEntity
            // 
            this.cbxEntity.AutoSize = true;
            this.cbxEntity.Location = new System.Drawing.Point(289, 22);
            this.cbxEntity.Margin = new System.Windows.Forms.Padding(4);
            this.cbxEntity.Name = "cbxEntity";
            this.cbxEntity.Size = new System.Drawing.Size(121, 19);
            this.cbxEntity.TabIndex = 16;
            this.cbxEntity.Text = "Entity (POCO)";
            this.cbxEntity.UseVisualStyleBackColor = true;
            // 
            // gbxOracleLikeSetting
            // 
            this.gbxOracleLikeSetting.Controls.Add(this.txtEscapeChar);
            this.gbxOracleLikeSetting.Controls.Add(this.lblEscapeChar);
            this.gbxOracleLikeSetting.Controls.Add(this.groupBox1);
            this.gbxOracleLikeSetting.Enabled = false;
            this.gbxOracleLikeSetting.Location = new System.Drawing.Point(6, 97);
            this.gbxOracleLikeSetting.Margin = new System.Windows.Forms.Padding(4);
            this.gbxOracleLikeSetting.Name = "gbxOracleLikeSetting";
            this.gbxOracleLikeSetting.Padding = new System.Windows.Forms.Padding(4);
            this.gbxOracleLikeSetting.Size = new System.Drawing.Size(700, 81);
            this.gbxOracleLikeSetting.TabIndex = 14;
            this.gbxOracleLikeSetting.TabStop = false;
            this.gbxOracleLikeSetting.Text = "Like検索設定（ODP.NET）";
            // 
            // txtEscapeChar
            // 
            this.txtEscapeChar.Location = new System.Drawing.Point(140, 29);
            this.txtEscapeChar.Margin = new System.Windows.Forms.Padding(4);
            this.txtEscapeChar.MaxLength = 1;
            this.txtEscapeChar.Name = "txtEscapeChar";
            this.txtEscapeChar.Size = new System.Drawing.Size(31, 22);
            this.txtEscapeChar.TabIndex = 9;
            this.txtEscapeChar.Text = "\\";
            this.txtEscapeChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblEscapeChar
            // 
            this.lblEscapeChar.AutoSize = true;
            this.lblEscapeChar.Location = new System.Drawing.Point(24, 32);
            this.lblEscapeChar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEscapeChar.Name = "lblEscapeChar";
            this.lblEscapeChar.Size = new System.Drawing.Size(102, 15);
            this.lblEscapeChar.TabIndex = 5;
            this.lblEscapeChar.Text = "エスケープ文字：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkEscapeToNChar);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbLikeStatement);
            this.groupBox1.Location = new System.Drawing.Point(212, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(472, 55);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NCHAR、NVARCHAR2、NCLOB生成オプション";
            // 
            // chkEscapeToNChar
            // 
            this.chkEscapeToNChar.AutoSize = true;
            this.chkEscapeToNChar.Location = new System.Drawing.Point(245, 25);
            this.chkEscapeToNChar.Margin = new System.Windows.Forms.Padding(4);
            this.chkEscapeToNChar.Name = "chkEscapeToNChar";
            this.chkEscapeToNChar.Size = new System.Drawing.Size(200, 19);
            this.chkEscapeToNChar.TabIndex = 1;
            this.chkEscapeToNChar.Text = "エスケープ文字をTO_NCHAR";
            this.chkEscapeToNChar.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "LIKE記号：";
            // 
            // cmbLikeStatement
            // 
            this.cmbLikeStatement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLikeStatement.FormattingEnabled = true;
            this.cmbLikeStatement.Items.AddRange(new object[] {
            "LIKE",
            "LIKE2",
            "LIKE4",
            "LIKEC"});
            this.cmbLikeStatement.Location = new System.Drawing.Point(103, 22);
            this.cmbLikeStatement.Margin = new System.Windows.Forms.Padding(4);
            this.cmbLikeStatement.Name = "cmbLikeStatement";
            this.cmbLikeStatement.Size = new System.Drawing.Size(116, 23);
            this.cmbLikeStatement.TabIndex = 0;
            // 
            // cbxOnlyTableMaintenance
            // 
            this.cbxOnlyTableMaintenance.AutoSize = true;
            this.cbxOnlyTableMaintenance.Location = new System.Drawing.Point(100, 22);
            this.cbxOnlyTableMaintenance.Margin = new System.Windows.Forms.Padding(4);
            this.cbxOnlyTableMaintenance.Name = "cbxOnlyTableMaintenance";
            this.cbxOnlyTableMaintenance.Size = new System.Drawing.Size(166, 19);
            this.cbxOnlyTableMaintenance.TabIndex = 11;
            this.cbxOnlyTableMaintenance.Text = "テーブルメンテナンスのみ";
            this.cbxOnlyTableMaintenance.UseVisualStyleBackColor = true;
            this.cbxOnlyTableMaintenance.CheckedChanged += new System.EventHandler(this.cbxOnlyTableMaintenance_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPersonalName);
            this.groupBox2.Controls.Add(this.lblFamilyName);
            this.groupBox2.Controls.Add(this.txtFamilyName);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbxTSIndisp);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtTimeStampColName);
            this.groupBox2.Controls.Add(this.txtTimeStampUpdMethod);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(699, 182);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "その他の設定";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(19, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(721, 514);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gbxInput);
            this.tabPage1.Controls.Add(this.gbxOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(713, 485);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ステップ１";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.gbxDataProviders);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.gbxOracleLikeSetting);
            this.tabPage2.Controls.Add(this.gbxDTO);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(713, 485);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ステップ２";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxTableMaintenance);
            this.groupBox3.Controls.Add(this.cbxOnlyTableMaintenance);
            this.groupBox3.Location = new System.Drawing.Point(7, 428);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(700, 50);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "テーブルメンテナンス画面を自動生成する（エンコーディングはDaoと同じになる";
            // 
            // cbxTableMaintenance
            // 
            this.cbxTableMaintenance.AutoSize = true;
            this.cbxTableMaintenance.Location = new System.Drawing.Point(289, 22);
            this.cbxTableMaintenance.Margin = new System.Windows.Forms.Padding(4);
            this.cbxTableMaintenance.Name = "cbxTableMaintenance";
            this.cbxTableMaintenance.Size = new System.Drawing.Size(373, 19);
            this.cbxTableMaintenance.TabIndex = 12;
            this.cbxTableMaintenance.Text = "テーブルメンテナンス（*.aspx、*.aspx.cs、TableAdapter.cs）";
            this.cbxTableMaintenance.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 588);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnDaoAndSqlGen);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form2";
            this.Text = "棟梁：D層自動生成ツール（墨壺） - D層、Dao・SQLファイル生成";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.gbxDataProviders.ResumeLayout(false);
            this.gbxDataProviders.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbxInput.ResumeLayout(false);
            this.gbxInput.PerformLayout();
            this.gbxSqlTemplateEncoding.ResumeLayout(false);
            this.gbxDaoTemplateLanguage.ResumeLayout(false);
            this.gbxDaoTemplateLanguage.PerformLayout();
            this.gbxDaoTemplateEncoding.ResumeLayout(false);
            this.gbxDaoDefinitionEncoding.ResumeLayout(false);
            this.gbxOutput.ResumeLayout(false);
            this.gbxOutput.PerformLayout();
            this.gbxSQLFileEncoding.ResumeLayout(false);
            this.gbxDaoFileEncoding.ResumeLayout(false);
            this.gbxDTO.ResumeLayout(false);
            this.gbxDTO.PerformLayout();
            this.gbxOracleLikeSetting.ResumeLayout(false);
            this.gbxOracleLikeSetting.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbnMySQL;
        private System.Windows.Forms.GroupBox gbxDataProviders;
        private System.Windows.Forms.RadioButton rbnDB2;
        private System.Windows.Forms.RadioButton rbnODP;
        private System.Windows.Forms.RadioButton rbnSQL;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Close_ToolStripMenuItem;
        private System.Windows.Forms.TextBox txtSetDaoDefinition;
        private System.Windows.Forms.Button btnSetDaoDefinition;
        private System.Windows.Forms.Label lblSetDaoDefinition;
        private System.Windows.Forms.Label lblSetSourceTemplate;
        private System.Windows.Forms.Button btnSetSourceTemplate;
        private System.Windows.Forms.TextBox txtSetSourceTemplate;
        private System.Windows.Forms.GroupBox gbxInput;
        private System.Windows.Forms.Button btnDaoAndSqlGen;
        private System.Windows.Forms.GroupBox gbxDaoDefinitionEncoding;
        private System.Windows.Forms.CheckBox cbxDaoDefinitionHeader;
        private System.Windows.Forms.GroupBox gbxDaoTemplateLanguage;
        private System.Windows.Forms.RadioButton rbnDTL_VB;
        private System.Windows.Forms.RadioButton rbnDTL_CS;
        private System.Windows.Forms.GroupBox gbxOutput;
        private System.Windows.Forms.Label lblSetOutput;
        private System.Windows.Forms.Button btnSetOutput;
        private System.Windows.Forms.TextBox txtSetOutput;
        private System.Windows.Forms.GroupBox gbxSQLFileEncoding;
        private System.Windows.Forms.GroupBox gbxDaoFileEncoding;
        private System.Windows.Forms.Label lblFamilyName;
        private System.Windows.Forms.TextBox txtFamilyName;
        private System.Windows.Forms.TextBox txtPersonalName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbxSqlTemplateEncoding;
        private System.Windows.Forms.GroupBox gbxDaoTemplateEncoding;
        private System.Windows.Forms.TextBox txtTimeStampUpdMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTimeStampColName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbxTSIndisp;
        private System.Windows.Forms.RadioButton rbnPstgrs;
        private System.Windows.Forms.RadioButton rbnODB;
        private System.Windows.Forms.RadioButton rbnOLE;
        private System.Windows.Forms.GroupBox gbxDTO;
        private System.Windows.Forms.CheckBox cbxTypedDataSet;
        private System.Windows.Forms.CheckBox cbxEntity;
        private System.Windows.Forms.GroupBox gbxOracleLikeSetting;
        private System.Windows.Forms.TextBox txtEscapeChar;
        private System.Windows.Forms.Label lblEscapeChar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkEscapeToNChar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbLikeStatement;
        private System.Windows.Forms.CheckBox cbxOnlyDTO;
        private System.Windows.Forms.CheckBox cbxOnlyTableMaintenance;
        private System.Windows.Forms.ComboBox cmbSTEncoding;
        private System.Windows.Forms.ComboBox cmbDTEncoding;
        private System.Windows.Forms.ComboBox cmbDDEncoding;
        private System.Windows.Forms.ComboBox cmbSFEncoding;
        private System.Windows.Forms.ComboBox cmbDFEncoding;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbxTableMaintenance;
    }
}