namespace DPQuery_Tool
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.btnSetOutput = new System.Windows.Forms.Button();
            this.cmbSFEncoding = new System.Windows.Forms.ComboBox();
            this.cmbDFEncoding = new System.Windows.Forms.ComboBox();
            this.lblSetOutput = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
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
            this.btnSetDaoDefinition = new System.Windows.Forms.Button();
            this.lblSetSourceTemplate = new System.Windows.Forms.Label();
            this.txtSetDaoDefinition = new System.Windows.Forms.TextBox();
            this.btnSetSourceTemplate = new System.Windows.Forms.Button();
            this.lblSetDaoDefinition = new System.Windows.Forms.Label();
            this.txtSetSourceTemplate = new System.Windows.Forms.TextBox();
            this.gbxOutput = new System.Windows.Forms.GroupBox();
            this.gbxSQLFileEncoding = new System.Windows.Forms.GroupBox();
            this.gbxDaoFileEncoding = new System.Windows.Forms.GroupBox();
            this.txtSetOutput = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxTSIndisp = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTimeStampColName = new System.Windows.Forms.TextBox();
            this.txtTimeStampUpdMethod = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.gbxDataProviders = new System.Windows.Forms.GroupBox();
            this.rbnODB = new System.Windows.Forms.RadioButton();
            this.rbnOLE = new System.Windows.Forms.RadioButton();
            this.rbnPstgrs = new System.Windows.Forms.RadioButton();
            this.rbnMySQL = new System.Windows.Forms.RadioButton();
            this.rbnDB2 = new System.Windows.Forms.RadioButton();
            this.rbnODP = new System.Windows.Forms.RadioButton();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPersonalName = new System.Windows.Forms.TextBox();
            this.lblFamilyName = new System.Windows.Forms.Label();
            this.txtFamilyName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Close_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.btnDaoAndSqlGen = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbxInput.SuspendLayout();
            this.gbxSqlTemplateEncoding.SuspendLayout();
            this.gbxDaoTemplateLanguage.SuspendLayout();
            this.gbxDaoTemplateEncoding.SuspendLayout();
            this.gbxDaoDefinitionEncoding.SuspendLayout();
            this.gbxOutput.SuspendLayout();
            this.gbxSQLFileEncoding.SuspendLayout();
            this.gbxDaoFileEncoding.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbxDataProviders.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSetOutput
            // 
            resources.ApplyResources(this.btnSetOutput, "btnSetOutput");
            this.btnSetOutput.Name = "btnSetOutput";
            this.btnSetOutput.UseVisualStyleBackColor = true;
            this.btnSetOutput.Click += new System.EventHandler(this.btnSetOutput_Click);
            // 
            // cmbSFEncoding
            // 
            this.cmbSFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSFEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSFEncoding, "cmbSFEncoding");
            this.cmbSFEncoding.Name = "cmbSFEncoding";
            // 
            // cmbDFEncoding
            // 
            this.cmbDFEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDFEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbDFEncoding, "cmbDFEncoding");
            this.cmbDFEncoding.Name = "cmbDFEncoding";
            // 
            // lblSetOutput
            // 
            resources.ApplyResources(this.lblSetOutput, "lblSetOutput");
            this.lblSetOutput.Name = "lblSetOutput";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gbxInput);
            this.tabPage1.Controls.Add(this.gbxOutput);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            resources.ApplyResources(this.gbxInput, "gbxInput");
            this.gbxInput.Name = "gbxInput";
            this.gbxInput.TabStop = false;
            // 
            // gbxSqlTemplateEncoding
            // 
            this.gbxSqlTemplateEncoding.Controls.Add(this.cmbSTEncoding);
            resources.ApplyResources(this.gbxSqlTemplateEncoding, "gbxSqlTemplateEncoding");
            this.gbxSqlTemplateEncoding.Name = "gbxSqlTemplateEncoding";
            this.gbxSqlTemplateEncoding.TabStop = false;
            // 
            // cmbSTEncoding
            // 
            this.cmbSTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSTEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSTEncoding, "cmbSTEncoding");
            this.cmbSTEncoding.Name = "cmbSTEncoding";
            // 
            // gbxDaoTemplateLanguage
            // 
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_VB);
            this.gbxDaoTemplateLanguage.Controls.Add(this.rbnDTL_CS);
            resources.ApplyResources(this.gbxDaoTemplateLanguage, "gbxDaoTemplateLanguage");
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
            this.gbxDaoTemplateEncoding.Controls.Add(this.cmbDTEncoding);
            resources.ApplyResources(this.gbxDaoTemplateEncoding, "gbxDaoTemplateEncoding");
            this.gbxDaoTemplateEncoding.Name = "gbxDaoTemplateEncoding";
            this.gbxDaoTemplateEncoding.TabStop = false;
            // 
            // cmbDTEncoding
            // 
            this.cmbDTEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDTEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbDTEncoding, "cmbDTEncoding");
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
            this.gbxDaoDefinitionEncoding.Controls.Add(this.cmbDDEncoding);
            resources.ApplyResources(this.gbxDaoDefinitionEncoding, "gbxDaoDefinitionEncoding");
            this.gbxDaoDefinitionEncoding.Name = "gbxDaoDefinitionEncoding";
            this.gbxDaoDefinitionEncoding.TabStop = false;
            // 
            // cmbDDEncoding
            // 
            this.cmbDDEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDDEncoding.FormattingEnabled = true;
            resources.ApplyResources(this.cmbDDEncoding, "cmbDDEncoding");
            this.cmbDDEncoding.Name = "cmbDDEncoding";
            // 
            // btnSetDaoDefinition
            // 
            resources.ApplyResources(this.btnSetDaoDefinition, "btnSetDaoDefinition");
            this.btnSetDaoDefinition.Name = "btnSetDaoDefinition";
            this.btnSetDaoDefinition.UseVisualStyleBackColor = true;
            this.btnSetDaoDefinition.Click += new System.EventHandler(this.btnSetDaoDefinition_Click);
            // 
            // lblSetSourceTemplate
            // 
            resources.ApplyResources(this.lblSetSourceTemplate, "lblSetSourceTemplate");
            this.lblSetSourceTemplate.Name = "lblSetSourceTemplate";
            // 
            // txtSetDaoDefinition
            // 
            resources.ApplyResources(this.txtSetDaoDefinition, "txtSetDaoDefinition");
            this.txtSetDaoDefinition.Name = "txtSetDaoDefinition";
            // 
            // btnSetSourceTemplate
            // 
            resources.ApplyResources(this.btnSetSourceTemplate, "btnSetSourceTemplate");
            this.btnSetSourceTemplate.Name = "btnSetSourceTemplate";
            this.btnSetSourceTemplate.UseVisualStyleBackColor = true;
            this.btnSetSourceTemplate.Click += new System.EventHandler(this.btnSetSourceTemplate_Click);
            // 
            // lblSetDaoDefinition
            // 
            resources.ApplyResources(this.lblSetDaoDefinition, "lblSetDaoDefinition");
            this.lblSetDaoDefinition.Name = "lblSetDaoDefinition";
            // 
            // txtSetSourceTemplate
            // 
            resources.ApplyResources(this.txtSetSourceTemplate, "txtSetSourceTemplate");
            this.txtSetSourceTemplate.Name = "txtSetSourceTemplate";
            // 
            // gbxOutput
            // 
            this.gbxOutput.Controls.Add(this.lblSetOutput);
            this.gbxOutput.Controls.Add(this.btnSetOutput);
            this.gbxOutput.Controls.Add(this.gbxSQLFileEncoding);
            this.gbxOutput.Controls.Add(this.gbxDaoFileEncoding);
            this.gbxOutput.Controls.Add(this.txtSetOutput);
            resources.ApplyResources(this.gbxOutput, "gbxOutput");
            this.gbxOutput.Name = "gbxOutput";
            this.gbxOutput.TabStop = false;
            // 
            // gbxSQLFileEncoding
            // 
            this.gbxSQLFileEncoding.Controls.Add(this.cmbSFEncoding);
            resources.ApplyResources(this.gbxSQLFileEncoding, "gbxSQLFileEncoding");
            this.gbxSQLFileEncoding.Name = "gbxSQLFileEncoding";
            this.gbxSQLFileEncoding.TabStop = false;
            // 
            // gbxDaoFileEncoding
            // 
            this.gbxDaoFileEncoding.Controls.Add(this.cmbDFEncoding);
            resources.ApplyResources(this.gbxDaoFileEncoding, "gbxDaoFileEncoding");
            this.gbxDaoFileEncoding.Name = "gbxDaoFileEncoding";
            this.gbxDaoFileEncoding.TabStop = false;
            // 
            // txtSetOutput
            // 
            resources.ApplyResources(this.txtSetOutput, "txtSetOutput");
            this.txtSetOutput.Name = "txtSetOutput";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.gbxDataProviders);
            this.tabPage2.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxTSIndisp);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtTimeStampColName);
            this.groupBox1.Controls.Add(this.txtTimeStampUpdMethod);
            this.groupBox1.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbxTSIndisp
            // 
            resources.ApplyResources(this.cbxTSIndisp, "cbxTSIndisp");
            this.cbxTSIndisp.Name = "cbxTSIndisp";
            this.cbxTSIndisp.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtTimeStampColName
            // 
            resources.ApplyResources(this.txtTimeStampColName, "txtTimeStampColName");
            this.txtTimeStampColName.Name = "txtTimeStampColName";
            // 
            // txtTimeStampUpdMethod
            // 
            resources.ApplyResources(this.txtTimeStampUpdMethod, "txtTimeStampUpdMethod");
            this.txtTimeStampUpdMethod.Name = "txtTimeStampUpdMethod";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            resources.ApplyResources(this.gbxDataProviders, "gbxDataProviders");
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
            // rbnMySQL
            // 
            resources.ApplyResources(this.rbnMySQL, "rbnMySQL");
            this.rbnMySQL.Name = "rbnMySQL";
            this.rbnMySQL.TabStop = true;
            this.rbnMySQL.UseVisualStyleBackColor = true;
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
            // 
            // rbnSQL
            // 
            resources.ApplyResources(this.rbnSQL, "rbnSQL");
            this.rbnSQL.Checked = true;
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.TabStop = true;
            this.rbnSQL.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPersonalName);
            this.groupBox2.Controls.Add(this.lblFamilyName);
            this.groupBox2.Controls.Add(this.txtFamilyName);
            this.groupBox2.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // txtPersonalName
            // 
            resources.ApplyResources(this.txtPersonalName, "txtPersonalName");
            this.txtPersonalName.Name = "txtPersonalName";
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Close_ToolStripMenuItem
            // 
            this.Close_ToolStripMenuItem.Name = "Close_ToolStripMenuItem";
            resources.ApplyResources(this.Close_ToolStripMenuItem, "Close_ToolStripMenuItem");
            this.Close_ToolStripMenuItem.Click += new System.EventHandler(this.Close_ToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Close_ToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // btnDaoAndSqlGen
            // 
            resources.ApplyResources(this.btnDaoAndSqlGen, "btnDaoAndSqlGen");
            this.btnDaoAndSqlGen.Name = "btnDaoAndSqlGen";
            this.btnDaoAndSqlGen.UseVisualStyleBackColor = true;
            this.btnDaoAndSqlGen.Click += new System.EventHandler(this.btnDaoAndSqlGen_Click);
            // 
            // Form3
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnDaoAndSqlGen);
            this.Name = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
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
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbxDataProviders.ResumeLayout(false);
            this.gbxDataProviders.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetOutput;
        private System.Windows.Forms.ComboBox cmbSFEncoding;
        private System.Windows.Forms.ComboBox cmbDFEncoding;
        private System.Windows.Forms.Label lblSetOutput;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox gbxInput;
        private System.Windows.Forms.GroupBox gbxSqlTemplateEncoding;
        private System.Windows.Forms.ComboBox cmbSTEncoding;
        private System.Windows.Forms.GroupBox gbxDaoTemplateLanguage;
        private System.Windows.Forms.RadioButton rbnDTL_VB;
        private System.Windows.Forms.RadioButton rbnDTL_CS;
        private System.Windows.Forms.GroupBox gbxDaoTemplateEncoding;
        private System.Windows.Forms.ComboBox cmbDTEncoding;
        private System.Windows.Forms.CheckBox cbxDaoDefinitionHeader;
        private System.Windows.Forms.GroupBox gbxDaoDefinitionEncoding;
        private System.Windows.Forms.ComboBox cmbDDEncoding;
        private System.Windows.Forms.Button btnSetDaoDefinition;
        private System.Windows.Forms.Label lblSetSourceTemplate;
        private System.Windows.Forms.TextBox txtSetDaoDefinition;
        private System.Windows.Forms.Button btnSetSourceTemplate;
        private System.Windows.Forms.Label lblSetDaoDefinition;
        private System.Windows.Forms.TextBox txtSetSourceTemplate;
        private System.Windows.Forms.GroupBox gbxOutput;
        private System.Windows.Forms.GroupBox gbxSQLFileEncoding;
        private System.Windows.Forms.GroupBox gbxDaoFileEncoding;
        private System.Windows.Forms.TextBox txtSetOutput;
        private System.Windows.Forms.ToolStripMenuItem Close_ToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button btnDaoAndSqlGen;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox gbxDataProviders;
        private System.Windows.Forms.RadioButton rbnODB;
        private System.Windows.Forms.RadioButton rbnOLE;
        private System.Windows.Forms.RadioButton rbnPstgrs;
        private System.Windows.Forms.RadioButton rbnMySQL;
        private System.Windows.Forms.RadioButton rbnDB2;
        private System.Windows.Forms.RadioButton rbnODP;
        private System.Windows.Forms.RadioButton rbnSQL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPersonalName;
        private System.Windows.Forms.Label lblFamilyName;
        private System.Windows.Forms.TextBox txtFamilyName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbxTSIndisp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTimeStampColName;
        private System.Windows.Forms.TextBox txtTimeStampUpdMethod;
        private System.Windows.Forms.Label label6;
    }
}