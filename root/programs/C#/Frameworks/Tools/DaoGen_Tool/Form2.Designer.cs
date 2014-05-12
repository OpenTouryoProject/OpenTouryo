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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
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
            resources.ApplyResources(this.rbnMySQL, "rbnMySQL");
            this.rbnMySQL.Name = "rbnMySQL";
            this.rbnMySQL.TabStop = true;
            this.rbnMySQL.UseVisualStyleBackColor = true;
            // 
            // gbxDataProviders
            // 
            resources.ApplyResources(this.gbxDataProviders, "gbxDataProviders");
            this.gbxDataProviders.Controls.Add(this.rbnODB);
            this.gbxDataProviders.Controls.Add(this.rbnOLE);
            this.gbxDataProviders.Controls.Add(this.rbnPstgrs);
            this.gbxDataProviders.Controls.Add(this.rbnMySQL);
            this.gbxDataProviders.Controls.Add(this.rbnDB2);
            this.gbxDataProviders.Controls.Add(this.rbnODP);
            this.gbxDataProviders.Controls.Add(this.rbnSQL);
            this.gbxDataProviders.Name = "gbxDataProviders";
            this.gbxDataProviders.TabStop = false;
            // 
            // rbnODB
            // 
            resources.ApplyResources(this.rbnODB, "rbnODB");
            this.rbnODB.Name = "rbnODB";
            this.rbnODB.UseVisualStyleBackColor = true;
            // 
            // rbnOLE
            // 
            resources.ApplyResources(this.rbnOLE, "rbnOLE");
            this.rbnOLE.Name = "rbnOLE";
            this.rbnOLE.UseVisualStyleBackColor = true;
            // 
            // rbnPstgrs
            // 
            resources.ApplyResources(this.rbnPstgrs, "rbnPstgrs");
            this.rbnPstgrs.Name = "rbnPstgrs";
            this.rbnPstgrs.TabStop = true;
            this.rbnPstgrs.UseVisualStyleBackColor = true;
            // 
            // rbnDB2
            // 
            resources.ApplyResources(this.rbnDB2, "rbnDB2");
            this.rbnDB2.Name = "rbnDB2";
            this.rbnDB2.TabStop = true;
            this.rbnDB2.UseVisualStyleBackColor = true;
            // 
            // rbnODP
            // 
            resources.ApplyResources(this.rbnODP, "rbnODP");
            this.rbnODP.Name = "rbnODP";
            this.rbnODP.TabStop = true;
            this.rbnODP.UseVisualStyleBackColor = true;
            this.rbnODP.CheckedChanged += new System.EventHandler(this.rbnODP_CheckedChanged);
            // 
            // rbnSQL
            // 
            resources.ApplyResources(this.rbnSQL, "rbnSQL");
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.TabStop = true;
            this.rbnSQL.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Close_ToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // Close_ToolStripMenuItem
            // 
            resources.ApplyResources(this.Close_ToolStripMenuItem, "Close_ToolStripMenuItem");
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // txtSetDaoDefinition
            // 
            resources.ApplyResources(this.txtSetDaoDefinition, "txtSetDaoDefinition");
            this.txtSetDaoDefinition.Name = "txtSetDaoDefinition";
            // 
            // btnSetDaoDefinition
            // 
            resources.ApplyResources(this.btnSetDaoDefinition, "btnSetDaoDefinition");
            this.btnSetDaoDefinition.Name = "btnSetDaoDefinition";
            this.btnSetDaoDefinition.UseVisualStyleBackColor = true;
            this.btnSetDaoDefinition.Click += new System.EventHandler(this.btnSetDaoDefinition_Click);
            // 
            // lblSetDaoDefinition
            // 
            resources.ApplyResources(this.lblSetDaoDefinition, "lblSetDaoDefinition");
            this.lblSetDaoDefinition.Name = "lblSetDaoDefinition";
            // 
            // lblSetSourceTemplate
            // 
            resources.ApplyResources(this.lblSetSourceTemplate, "lblSetSourceTemplate");
            this.lblSetSourceTemplate.Name = "lblSetSourceTemplate";
            // 
            // btnSetSourceTemplate
            // 
            resources.ApplyResources(this.btnSetSourceTemplate, "btnSetSourceTemplate");
            this.btnSetSourceTemplate.Name = "btnSetSourceTemplate";
            this.btnSetSourceTemplate.UseVisualStyleBackColor = true;
            this.btnSetSourceTemplate.Click += new System.EventHandler(this.btnSetSourceTemplate_Click);
            // 
            // txtSetSourceTemplate
            // 
            resources.ApplyResources(this.txtSetSourceTemplate, "txtSetSourceTemplate");
            this.txtSetSourceTemplate.Name = "txtSetSourceTemplate";
            // 
            // gbxInput
            // 
            resources.ApplyResources(this.gbxInput, "gbxInput");
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
            this.gbxInput.Name = "gbxInput";
            this.gbxInput.TabStop = false;
            // 
            // gbxSqlTemplateEncoding
            // 
            resources.ApplyResources(this.gbxSqlTemplateEncoding, "gbxSqlTemplateEncoding");
            this.gbxSqlTemplateEncoding.Controls.Add(this.cmbSTEncoding);
            this.gbxSqlTemplateEncoding.Name = "gbxSqlTemplateEncoding";
            this.gbxSqlTemplateEncoding.TabStop = false;
            // 
            // cmbSTEncoding
            // 
            resources.ApplyResources(this.cmbSTEncoding, "cmbSTEncoding");
            this.cmbSTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSTEncoding.FormattingEnabled = true;
            this.cmbSTEncoding.Name = "cmbSTEncoding";
            // 
            // gbxDaoTemplateLanguage
            // 
            resources.ApplyResources(this.gbxDaoTemplateLanguage, "gbxDaoTemplateLanguage");
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_VB);
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_CS);
            this.gbxDaoTemplateLanguage.Name = "gbxDaoTemplateLanguage";
            this.gbxDaoTemplateLanguage.TabStop = false;
            // 
            // rbnDTL_VB
            // 
            resources.ApplyResources(this.rbnDTL_VB, "rbnDTL_VB");
            this.rbnDTL_VB.Name = "rbnDTL_VB";
            this.rbnDTL_VB.TabStop = true;
            this.rbnDTL_VB.UseVisualStyleBackColor = true;
            // 
            // rbnDTL_CS
            // 
            resources.ApplyResources(this.rbnDTL_CS, "rbnDTL_CS");
            this.rbnDTL_CS.Name = "rbnDTL_CS";
            this.rbnDTL_CS.TabStop = true;
            this.rbnDTL_CS.UseVisualStyleBackColor = true;
            // 
            // gbxDaoTemplateEncoding
            // 
            resources.ApplyResources(this.gbxDaoTemplateEncoding, "gbxDaoTemplateEncoding");
            this.gbxDaoTemplateEncoding.Controls.Add(this.cmbDTEncoding);
            this.gbxDaoTemplateEncoding.Name = "gbxDaoTemplateEncoding";
            this.gbxDaoTemplateEncoding.TabStop = false;
            // 
            // cmbDTEncoding
            // 
            resources.ApplyResources(this.cmbDTEncoding, "cmbDTEncoding");
            this.cmbDTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDTEncoding.FormattingEnabled = true;
            this.cmbDTEncoding.Name = "cmbDTEncoding";
            // 
            // cbxDaoDefinitionHeader
            // 
            resources.ApplyResources(this.cbxDaoDefinitionHeader, "cbxDaoDefinitionHeader");
            this.cbxDaoDefinitionHeader.Name = "cbxDaoDefinitionHeader";
            this.cbxDaoDefinitionHeader.UseVisualStyleBackColor = true;
            // 
            // gbxDaoDefinitionEncoding
            // 
            resources.ApplyResources(this.gbxDaoDefinitionEncoding, "gbxDaoDefinitionEncoding");
            this.gbxDaoDefinitionEncoding.Controls.Add(this.cmbDDEncoding);
            this.gbxDaoDefinitionEncoding.Name = "gbxDaoDefinitionEncoding";
            this.gbxDaoDefinitionEncoding.TabStop = false;
            // 
            // cmbDDEncoding
            // 
            resources.ApplyResources(this.cmbDDEncoding, "cmbDDEncoding");
            this.cmbDDEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDDEncoding.FormattingEnabled = true;
            this.cmbDDEncoding.Name = "cmbDDEncoding";
            // 
            // btnDaoAndSqlGen
            // 
            resources.ApplyResources(this.btnDaoAndSqlGen, "btnDaoAndSqlGen");
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = true;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // gbxOutput
            // 
            resources.ApplyResources(this.gbxOutput, "gbxOutput");
            this.gbxOutput.Controls.Add(this.lblSetOutput);
            this.gbxOutput.Controls.Add(this.btnSetOutput);
            this.gbxOutput.Controls.Add(this.gbxSQLFileEncoding);
            this.gbxOutput.Controls.Add(this.gbxDaoFileEncoding);
            this.gbxOutput.Controls.Add(this.txtSetOutput);
            this.gbxOutput.Name = "gbxOutput";
            this.gbxOutput.TabStop = false;
            // 
            // lblSetOutput
            // 
            resources.ApplyResources(this.lblSetOutput, "lblSetOutput");
            this.lblSetOutput.Name = "lblSetOutput";
            // 
            // btnSetOutput
            // 
            resources.ApplyResources(this.btnSetOutput, "btnSetOutput");
            this.btnSetOutput.Name = "btnSetOutput";
            this.btnSetOutput.UseVisualStyleBackColor = true;
            this.btnSetOutput.Click += new System.EventHandler(this.btnSetOutput_Click);
            // 
            // gbxSQLFileEncoding
            // 
            resources.ApplyResources(this.gbxSQLFileEncoding, "gbxSQLFileEncoding");
            this.gbxSQLFileEncoding.Controls.Add(this.cmbSFEncoding);
            this.gbxSQLFileEncoding.Name = "gbxSQLFileEncoding";
            this.gbxSQLFileEncoding.TabStop = false;
            // 
            // cmbSFEncoding
            // 
            resources.ApplyResources(this.cmbSFEncoding, "cmbSFEncoding");
            this.cmbSFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSFEncoding.FormattingEnabled = true;
            this.cmbSFEncoding.Name = "cmbSFEncoding";
            // 
            // gbxDaoFileEncoding
            // 
            resources.ApplyResources(this.gbxDaoFileEncoding, "gbxDaoFileEncoding");
            this.gbxDaoFileEncoding.Controls.Add(this.cmbDFEncoding);
            this.gbxDaoFileEncoding.Name = "gbxDaoFileEncoding";
            this.gbxDaoFileEncoding.TabStop = false;
            // 
            // cmbDFEncoding
            // 
            resources.ApplyResources(this.cmbDFEncoding, "cmbDFEncoding");
            this.cmbDFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDFEncoding.FormattingEnabled = true;
            this.cmbDFEncoding.Name = "cmbDFEncoding";
            // 
            // txtSetOutput
            // 
            resources.ApplyResources(this.txtSetOutput, "txtSetOutput");
            this.txtSetOutput.Name = "txtSetOutput";
            // 
            // lblFamilyName
            // 
            resources.ApplyResources(this.lblFamilyName, "lblFamilyName");
            this.lblFamilyName.Name = "lblFamilyName";
            // 
            // txtFamilyName
            // 
            resources.ApplyResources(this.txtFamilyName, "txtFamilyName");
            this.txtFamilyName.Name = "txtFamilyName";
            // 
            // txtPersonalName
            // 
            resources.ApplyResources(this.txtPersonalName, "txtPersonalName");
            this.txtPersonalName.Name = "txtPersonalName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtTimeStampUpdMethod
            // 
            resources.ApplyResources(this.txtTimeStampUpdMethod, "txtTimeStampUpdMethod");
            this.txtTimeStampUpdMethod.Name = "txtTimeStampUpdMethod";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtTimeStampColName
            // 
            resources.ApplyResources(this.txtTimeStampColName, "txtTimeStampColName");
            this.txtTimeStampColName.Name = "txtTimeStampColName";
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
            // cbxTSIndisp
            // 
            resources.ApplyResources(this.cbxTSIndisp, "cbxTSIndisp");
            this.cbxTSIndisp.Name = "cbxTSIndisp";
            this.cbxTSIndisp.UseVisualStyleBackColor = true;
            // 
            // gbxDTO
            // 
            resources.ApplyResources(this.gbxDTO, "gbxDTO");
            this.gbxDTO.Controls.Add(this.cbxOnlyDTO);
            this.gbxDTO.Controls.Add(this.cbxTypedDataSet);
            this.gbxDTO.Controls.Add(this.cbxEntity);
            this.gbxDTO.Name = "gbxDTO";
            this.gbxDTO.TabStop = false;
            // 
            // cbxOnlyDTO
            // 
            resources.ApplyResources(this.cbxOnlyDTO, "cbxOnlyDTO");
            this.cbxOnlyDTO.Name = "cbxOnlyDTO";
            this.cbxOnlyDTO.UseVisualStyleBackColor = true;
            this.cbxOnlyDTO.CheckedChanged += new System.EventHandler(this.cbxOnlyDTO_CheckedChanged);
            // 
            // cbxTypedDataSet
            // 
            resources.ApplyResources(this.cbxTypedDataSet, "cbxTypedDataSet");
            this.cbxTypedDataSet.Name = "cbxTypedDataSet";
            this.cbxTypedDataSet.UseVisualStyleBackColor = true;
            // 
            // cbxEntity
            // 
            resources.ApplyResources(this.cbxEntity, "cbxEntity");
            this.cbxEntity.Name = "cbxEntity";
            this.cbxEntity.UseVisualStyleBackColor = true;
            // 
            // gbxOracleLikeSetting
            // 
            resources.ApplyResources(this.gbxOracleLikeSetting, "gbxOracleLikeSetting");
            this.gbxOracleLikeSetting.Controls.Add(this.txtEscapeChar);
            this.gbxOracleLikeSetting.Controls.Add(this.lblEscapeChar);
            this.gbxOracleLikeSetting.Controls.Add(this.groupBox1);
            this.gbxOracleLikeSetting.Name = "gbxOracleLikeSetting";
            this.gbxOracleLikeSetting.TabStop = false;
            // 
            // txtEscapeChar
            // 
            resources.ApplyResources(this.txtEscapeChar, "txtEscapeChar");
            this.txtEscapeChar.Name = "txtEscapeChar";
            // 
            // lblEscapeChar
            // 
            resources.ApplyResources(this.lblEscapeChar, "lblEscapeChar");
            this.lblEscapeChar.Name = "lblEscapeChar";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkEscapeToNChar);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbLikeStatement);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chkEscapeToNChar
            // 
            resources.ApplyResources(this.chkEscapeToNChar, "chkEscapeToNChar");
            this.chkEscapeToNChar.Name = "chkEscapeToNChar";
            this.chkEscapeToNChar.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cmbLikeStatement
            // 
            resources.ApplyResources(this.cmbLikeStatement, "cmbLikeStatement");
            this.cmbLikeStatement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLikeStatement.FormattingEnabled = true;
            this.cmbLikeStatement.Items.AddRange(new object[] {
            resources.GetString("cmbLikeStatement.Items"),
            resources.GetString("cmbLikeStatement.Items1"),
            resources.GetString("cmbLikeStatement.Items2"),
            resources.GetString("cmbLikeStatement.Items3")});
            this.cmbLikeStatement.Name = "cmbLikeStatement";
            // 
            // cbxOnlyTableMaintenance
            // 
            resources.ApplyResources(this.cbxOnlyTableMaintenance, "cbxOnlyTableMaintenance");
            this.cbxOnlyTableMaintenance.Name = "cbxOnlyTableMaintenance";
            this.cbxOnlyTableMaintenance.UseVisualStyleBackColor = true;
            this.cbxOnlyTableMaintenance.CheckedChanged += new System.EventHandler(this.cbxOnlyTableMaintenance_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
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
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.gbxInput);
            this.tabPage1.Controls.Add(this.gbxOutput);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.gbxDataProviders);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.gbxOracleLikeSetting);
            this.tabPage2.Controls.Add(this.gbxDTO);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.cbxTableMaintenance);
            this.groupBox3.Controls.Add(this.cbxOnlyTableMaintenance);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cbxTableMaintenance
            // 
            resources.ApplyResources(this.cbxTableMaintenance, "cbxTableMaintenance");
            this.cbxTableMaintenance.Name = "cbxTableMaintenance";
            this.cbxTableMaintenance.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnDaoAndSqlGen);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form2";
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