namespace DPQuery_Tool
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cmbDataProvider = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCnnStr = new System.Windows.Forms.TextBox();
            this.btnCnOpen = new System.Windows.Forms.Button();
            this.btnCnClose = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnExecQuery = new System.Windows.Forms.Button();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.cmbSelMethod = new System.Windows.Forms.ComboBox();
            this.groupBoxTx = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnRollbackTx = new System.Windows.Forms.Button();
            this.cmbSelIso = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCommitTx = new System.Windows.Forms.Button();
            this.btnBeginTx = new System.Windows.Forms.Button();
            this.cmbSelTxCtrl = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxR = new System.Windows.Forms.GroupBox();
            this.cmbSaveSlot = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCreateConfig = new System.Windows.Forms.Button();
            this.btnOverwriteQueryFile = new System.Windows.Forms.Button();
            this.btnCloseQueryFile = new System.Windows.Forms.Button();
            this.statBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxType = new System.Windows.Forms.CheckBox();
            this.nudNumOfBind = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SAMPLE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.TEMPLATE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.XML_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ROOT_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ROOT1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ROOT2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.WHERE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.WHERE1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.WHERE2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_ELSE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TXT_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TXT1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TXT2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TAG_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TAG1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IF_TAG2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ELSE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ELSE1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ELSE2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.LIST_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.LIST1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.LIST2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.JOIN_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.JOIN1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.JOIN2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SUB_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SUB1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SUB2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.SELECT_CASE_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DELCMA_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DELCMA1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DELCMA2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.VAL_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.PARAM_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.PARAM1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.PARAM2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DIV_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.COMMENT_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.CDATA_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.CPARAM_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.CPARAM1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.CPARAM2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveQueryFile = new System.Windows.Forms.Button();
            this.btnOpenQueryFile = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label0 = new System.Windows.Forms.Label();
            this.txtSQL = new DPQuery_Tool.RichTextBoxDisableDF();
            this.groupBoxTx.SuspendLayout();
            this.groupBoxR.SuspendLayout();
            this.statBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfBind)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDataProvider
            // 
            resources.ApplyResources(this.cmbDataProvider, "cmbDataProvider");
            this.cmbDataProvider.BackColor = System.Drawing.Color.White;
            this.cmbDataProvider.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmbDataProvider.Name = "cmbDataProvider";
            this.cmbDataProvider.SelectedIndexChanged += new System.EventHandler(this.cmbDataProvider_SelectedIndexChanged);
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
            // txtCnnStr
            // 
            resources.ApplyResources(this.txtCnnStr, "txtCnnStr");
            this.txtCnnStr.Name = "txtCnnStr";
            // 
            // btnCnOpen
            // 
            resources.ApplyResources(this.btnCnOpen, "btnCnOpen");
            this.btnCnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnCnOpen.ForeColor = System.Drawing.Color.White;
            this.btnCnOpen.Name = "btnCnOpen";
            this.btnCnOpen.UseVisualStyleBackColor = false;
            this.btnCnOpen.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnCnOpen.Click += new System.EventHandler(this.btnCnOpen_Click);
            // 
            // btnCnClose
            // 
            resources.ApplyResources(this.btnCnClose, "btnCnClose");
            this.btnCnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnCnClose.ForeColor = System.Drawing.Color.White;
            this.btnCnClose.Name = "btnCnClose";
            this.btnCnClose.UseVisualStyleBackColor = false;
            this.btnCnClose.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnCnClose.Click += new System.EventHandler(this.btnCnClose_Click);
            // 
            // btnSaveConfig
            // 
            resources.ApplyResources(this.btnSaveConfig, "btnSaveConfig");
            this.btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnSaveConfig.ForeColor = System.Drawing.Color.White;
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnExecQuery
            // 
            resources.ApplyResources(this.btnExecQuery, "btnExecQuery");
            this.btnExecQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(166)))), ((int)(((byte)(44)))));
            this.btnExecQuery.ForeColor = System.Drawing.Color.White;
            this.btnExecQuery.Name = "btnExecQuery";
            this.btnExecQuery.UseVisualStyleBackColor = false;
            this.btnExecQuery.EnabledChanged += new System.EventHandler(this.btnColor2_EnabledChanged);
            this.btnExecQuery.Click += new System.EventHandler(this.btnExecQuery_Click);
            // 
            // btnLoadConfig
            // 
            resources.ApplyResources(this.btnLoadConfig, "btnLoadConfig");
            this.btnLoadConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnLoadConfig.ForeColor = System.Drawing.Color.White;
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.UseVisualStyleBackColor = false;
            this.btnLoadConfig.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // cmbSelMethod
            // 
            resources.ApplyResources(this.cmbSelMethod, "cmbSelMethod");
            this.cmbSelMethod.Name = "cmbSelMethod";
            // 
            // groupBoxTx
            // 
            resources.ApplyResources(this.groupBoxTx, "groupBoxTx");
            this.groupBoxTx.Controls.Add(this.label7);
            this.groupBoxTx.Controls.Add(this.btnRollbackTx);
            this.groupBoxTx.Controls.Add(this.cmbSelIso);
            this.groupBoxTx.Controls.Add(this.label6);
            this.groupBoxTx.Controls.Add(this.btnCommitTx);
            this.groupBoxTx.Controls.Add(this.btnBeginTx);
            this.groupBoxTx.Controls.Add(this.cmbSelTxCtrl);
            this.groupBoxTx.Name = "groupBoxTx";
            this.groupBoxTx.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // btnRollbackTx
            // 
            resources.ApplyResources(this.btnRollbackTx, "btnRollbackTx");
            this.btnRollbackTx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnRollbackTx.ForeColor = System.Drawing.Color.White;
            this.btnRollbackTx.Name = "btnRollbackTx";
            this.btnRollbackTx.UseVisualStyleBackColor = false;
            this.btnRollbackTx.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnRollbackTx.Click += new System.EventHandler(this.btnRollbackTx_Click);
            // 
            // cmbSelIso
            // 
            resources.ApplyResources(this.cmbSelIso, "cmbSelIso");
            this.cmbSelIso.Name = "cmbSelIso";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnCommitTx
            // 
            resources.ApplyResources(this.btnCommitTx, "btnCommitTx");
            this.btnCommitTx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnCommitTx.ForeColor = System.Drawing.Color.White;
            this.btnCommitTx.Name = "btnCommitTx";
            this.btnCommitTx.UseVisualStyleBackColor = false;
            this.btnCommitTx.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnCommitTx.Click += new System.EventHandler(this.btnCommitTx_Click);
            // 
            // btnBeginTx
            // 
            resources.ApplyResources(this.btnBeginTx, "btnBeginTx");
            this.btnBeginTx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnBeginTx.ForeColor = System.Drawing.Color.White;
            this.btnBeginTx.Name = "btnBeginTx";
            this.btnBeginTx.UseVisualStyleBackColor = false;
            this.btnBeginTx.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnBeginTx.Click += new System.EventHandler(this.btnBeginTx_Click);
            // 
            // cmbSelTxCtrl
            // 
            resources.ApplyResources(this.cmbSelTxCtrl, "cmbSelTxCtrl");
            this.cmbSelTxCtrl.Name = "cmbSelTxCtrl";
            this.cmbSelTxCtrl.SelectedIndexChanged += new System.EventHandler(this.cmbSelTxCtrl_SelectedIndexChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBoxR
            // 
            resources.ApplyResources(this.groupBoxR, "groupBoxR");
            this.groupBoxR.Controls.Add(this.cmbSaveSlot);
            this.groupBoxR.Controls.Add(this.label4);
            this.groupBoxR.Controls.Add(this.btnCreateConfig);
            this.groupBoxR.Controls.Add(this.btnLoadConfig);
            this.groupBoxR.Controls.Add(this.label2);
            this.groupBoxR.Controls.Add(this.cmbDataProvider);
            this.groupBoxR.Controls.Add(this.btnSaveConfig);
            this.groupBoxR.Controls.Add(this.label3);
            this.groupBoxR.Controls.Add(this.txtCnnStr);
            this.groupBoxR.Name = "groupBoxR";
            this.groupBoxR.TabStop = false;
            // 
            // cmbSaveSlot
            // 
            resources.ApplyResources(this.cmbSaveSlot, "cmbSaveSlot");
            this.cmbSaveSlot.Name = "cmbSaveSlot";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnCreateConfig
            // 
            resources.ApplyResources(this.btnCreateConfig, "btnCreateConfig");
            this.btnCreateConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnCreateConfig.FlatAppearance.BorderColor = System.Drawing.Color.Turquoise;
            this.btnCreateConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnCreateConfig.ForeColor = System.Drawing.Color.White;
            this.btnCreateConfig.Name = "btnCreateConfig";
            this.btnCreateConfig.UseVisualStyleBackColor = false;
            this.btnCreateConfig.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnCreateConfig.Click += new System.EventHandler(this.btnCreateConfig_Click);
            // 
            // btnOverwriteQueryFile
            // 
            resources.ApplyResources(this.btnOverwriteQueryFile, "btnOverwriteQueryFile");
            this.btnOverwriteQueryFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnOverwriteQueryFile.ForeColor = System.Drawing.Color.White;
            this.btnOverwriteQueryFile.Name = "btnOverwriteQueryFile";
            this.btnOverwriteQueryFile.UseVisualStyleBackColor = false;
            this.btnOverwriteQueryFile.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnOverwriteQueryFile.Click += new System.EventHandler(this.btnOverwriteQueryFile_Click);
            // 
            // btnCloseQueryFile
            // 
            resources.ApplyResources(this.btnCloseQueryFile, "btnCloseQueryFile");
            this.btnCloseQueryFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnCloseQueryFile.ForeColor = System.Drawing.Color.White;
            this.btnCloseQueryFile.Name = "btnCloseQueryFile";
            this.btnCloseQueryFile.UseVisualStyleBackColor = false;
            this.btnCloseQueryFile.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnCloseQueryFile.Click += new System.EventHandler(this.btnCloseQueryFile_Click);
            // 
            // statBar
            // 
            resources.ApplyResources(this.statBar, "statBar");
            this.statBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statBar.Name = "statBar";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // lblFilePath
            // 
            resources.ApplyResources(this.lblFilePath, "lblFilePath");
            this.lblFilePath.Name = "lblFilePath";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbxType
            // 
            resources.ApplyResources(this.cbxType, "cbxType");
            this.cbxType.Name = "cbxType";
            this.cbxType.UseVisualStyleBackColor = true;
            // 
            // nudNumOfBind
            // 
            resources.ApplyResources(this.nudNumOfBind, "nudNumOfBind");
            this.nudNumOfBind.Name = "nudNumOfBind";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // contextMenuStrip1
            // 
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SAMPLE_TSMI,
            this.TEMPLATE_TSMI,
            this.XML_TSMI,
            this.ROOT_TSMI,
            this.WHERE_TSMI,
            this.IF_ELSE_TSMI,
            this.LIST_TSMI,
            this.JOIN_TSMI,
            this.SUB_TSMI,
            this.SELECT_CASE_TSMI,
            this.DELCMA_TSMI,
            this.VAL_TSMI,
            this.PARAM_TSMI,
            this.DIV_TSMI,
            this.COMMENT_TSMI,
            this.CDATA_TSMI,
            this.CPARAM_TSMI});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            // 
            // SAMPLE_TSMI
            // 
            resources.ApplyResources(this.SAMPLE_TSMI, "SAMPLE_TSMI");
            this.SAMPLE_TSMI.Name = "SAMPLE_TSMI";
            this.SAMPLE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // TEMPLATE_TSMI
            // 
            resources.ApplyResources(this.TEMPLATE_TSMI, "TEMPLATE_TSMI");
            this.TEMPLATE_TSMI.Name = "TEMPLATE_TSMI";
            this.TEMPLATE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // XML_TSMI
            // 
            resources.ApplyResources(this.XML_TSMI, "XML_TSMI");
            this.XML_TSMI.Name = "XML_TSMI";
            this.XML_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT_TSMI
            // 
            resources.ApplyResources(this.ROOT_TSMI, "ROOT_TSMI");
            this.ROOT_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ROOT1_TSMI,
            this.ROOT2_TSMI});
            this.ROOT_TSMI.Name = "ROOT_TSMI";
            this.ROOT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT1_TSMI
            // 
            resources.ApplyResources(this.ROOT1_TSMI, "ROOT1_TSMI");
            this.ROOT1_TSMI.Name = "ROOT1_TSMI";
            this.ROOT1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT2_TSMI
            // 
            resources.ApplyResources(this.ROOT2_TSMI, "ROOT2_TSMI");
            this.ROOT2_TSMI.Name = "ROOT2_TSMI";
            this.ROOT2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE_TSMI
            // 
            resources.ApplyResources(this.WHERE_TSMI, "WHERE_TSMI");
            this.WHERE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WHERE1_TSMI,
            this.WHERE2_TSMI});
            this.WHERE_TSMI.Name = "WHERE_TSMI";
            this.WHERE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE1_TSMI
            // 
            resources.ApplyResources(this.WHERE1_TSMI, "WHERE1_TSMI");
            this.WHERE1_TSMI.Name = "WHERE1_TSMI";
            this.WHERE1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE2_TSMI
            // 
            resources.ApplyResources(this.WHERE2_TSMI, "WHERE2_TSMI");
            this.WHERE2_TSMI.Name = "WHERE2_TSMI";
            this.WHERE2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_ELSE_TSMI
            // 
            resources.ApplyResources(this.IF_ELSE_TSMI, "IF_ELSE_TSMI");
            this.IF_ELSE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TXT_TSMI,
            this.IF_TAG_TSMI,
            this.ELSE_TSMI});
            this.IF_ELSE_TSMI.Name = "IF_ELSE_TSMI";
            this.IF_ELSE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT_TSMI
            // 
            resources.ApplyResources(this.IF_TXT_TSMI, "IF_TXT_TSMI");
            this.IF_TXT_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TXT1_TSMI,
            this.IF_TXT2_TSMI});
            this.IF_TXT_TSMI.Name = "IF_TXT_TSMI";
            this.IF_TXT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT1_TSMI
            // 
            resources.ApplyResources(this.IF_TXT1_TSMI, "IF_TXT1_TSMI");
            this.IF_TXT1_TSMI.Name = "IF_TXT1_TSMI";
            this.IF_TXT1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT2_TSMI
            // 
            resources.ApplyResources(this.IF_TXT2_TSMI, "IF_TXT2_TSMI");
            this.IF_TXT2_TSMI.Name = "IF_TXT2_TSMI";
            this.IF_TXT2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG_TSMI
            // 
            resources.ApplyResources(this.IF_TAG_TSMI, "IF_TAG_TSMI");
            this.IF_TAG_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TAG1_TSMI,
            this.IF_TAG2_TSMI});
            this.IF_TAG_TSMI.Name = "IF_TAG_TSMI";
            this.IF_TAG_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG1_TSMI
            // 
            resources.ApplyResources(this.IF_TAG1_TSMI, "IF_TAG1_TSMI");
            this.IF_TAG1_TSMI.Name = "IF_TAG1_TSMI";
            this.IF_TAG1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG2_TSMI
            // 
            resources.ApplyResources(this.IF_TAG2_TSMI, "IF_TAG2_TSMI");
            this.IF_TAG2_TSMI.Name = "IF_TAG2_TSMI";
            this.IF_TAG2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE_TSMI
            // 
            resources.ApplyResources(this.ELSE_TSMI, "ELSE_TSMI");
            this.ELSE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ELSE1_TSMI,
            this.ELSE2_TSMI});
            this.ELSE_TSMI.Name = "ELSE_TSMI";
            this.ELSE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE1_TSMI
            // 
            resources.ApplyResources(this.ELSE1_TSMI, "ELSE1_TSMI");
            this.ELSE1_TSMI.Name = "ELSE1_TSMI";
            this.ELSE1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE2_TSMI
            // 
            resources.ApplyResources(this.ELSE2_TSMI, "ELSE2_TSMI");
            this.ELSE2_TSMI.Name = "ELSE2_TSMI";
            this.ELSE2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST_TSMI
            // 
            resources.ApplyResources(this.LIST_TSMI, "LIST_TSMI");
            this.LIST_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LIST1_TSMI,
            this.LIST2_TSMI});
            this.LIST_TSMI.Name = "LIST_TSMI";
            this.LIST_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST1_TSMI
            // 
            resources.ApplyResources(this.LIST1_TSMI, "LIST1_TSMI");
            this.LIST1_TSMI.Name = "LIST1_TSMI";
            this.LIST1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST2_TSMI
            // 
            resources.ApplyResources(this.LIST2_TSMI, "LIST2_TSMI");
            this.LIST2_TSMI.Name = "LIST2_TSMI";
            this.LIST2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN_TSMI
            // 
            resources.ApplyResources(this.JOIN_TSMI, "JOIN_TSMI");
            this.JOIN_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JOIN1_TSMI,
            this.JOIN2_TSMI});
            this.JOIN_TSMI.Name = "JOIN_TSMI";
            this.JOIN_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN1_TSMI
            // 
            resources.ApplyResources(this.JOIN1_TSMI, "JOIN1_TSMI");
            this.JOIN1_TSMI.Name = "JOIN1_TSMI";
            this.JOIN1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN2_TSMI
            // 
            resources.ApplyResources(this.JOIN2_TSMI, "JOIN2_TSMI");
            this.JOIN2_TSMI.Name = "JOIN2_TSMI";
            this.JOIN2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB_TSMI
            // 
            resources.ApplyResources(this.SUB_TSMI, "SUB_TSMI");
            this.SUB_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SUB1_TSMI,
            this.SUB2_TSMI});
            this.SUB_TSMI.Name = "SUB_TSMI";
            this.SUB_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB1_TSMI
            // 
            resources.ApplyResources(this.SUB1_TSMI, "SUB1_TSMI");
            this.SUB1_TSMI.Name = "SUB1_TSMI";
            this.SUB1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB2_TSMI
            // 
            resources.ApplyResources(this.SUB2_TSMI, "SUB2_TSMI");
            this.SUB2_TSMI.Name = "SUB2_TSMI";
            this.SUB2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SELECT_CASE_TSMI
            // 
            resources.ApplyResources(this.SELECT_CASE_TSMI, "SELECT_CASE_TSMI");
            this.SELECT_CASE_TSMI.Name = "SELECT_CASE_TSMI";
            this.SELECT_CASE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DELCMA_TSMI
            // 
            resources.ApplyResources(this.DELCMA_TSMI, "DELCMA_TSMI");
            this.DELCMA_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DELCMA1_TSMI,
            this.DELCMA2_TSMI});
            this.DELCMA_TSMI.Name = "DELCMA_TSMI";
            this.DELCMA_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DELCMA1_TSMI
            // 
            resources.ApplyResources(this.DELCMA1_TSMI, "DELCMA1_TSMI");
            this.DELCMA1_TSMI.Name = "DELCMA1_TSMI";
            this.DELCMA1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DELCMA2_TSMI
            // 
            resources.ApplyResources(this.DELCMA2_TSMI, "DELCMA2_TSMI");
            this.DELCMA2_TSMI.Name = "DELCMA2_TSMI";
            this.DELCMA2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // VAL_TSMI
            // 
            resources.ApplyResources(this.VAL_TSMI, "VAL_TSMI");
            this.VAL_TSMI.Name = "VAL_TSMI";
            this.VAL_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM_TSMI
            // 
            resources.ApplyResources(this.PARAM_TSMI, "PARAM_TSMI");
            this.PARAM_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PARAM1_TSMI,
            this.PARAM2_TSMI});
            this.PARAM_TSMI.Name = "PARAM_TSMI";
            this.PARAM_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM1_TSMI
            // 
            resources.ApplyResources(this.PARAM1_TSMI, "PARAM1_TSMI");
            this.PARAM1_TSMI.Name = "PARAM1_TSMI";
            this.PARAM1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM2_TSMI
            // 
            resources.ApplyResources(this.PARAM2_TSMI, "PARAM2_TSMI");
            this.PARAM2_TSMI.Name = "PARAM2_TSMI";
            this.PARAM2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DIV_TSMI
            // 
            resources.ApplyResources(this.DIV_TSMI, "DIV_TSMI");
            this.DIV_TSMI.Name = "DIV_TSMI";
            this.DIV_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // COMMENT_TSMI
            // 
            resources.ApplyResources(this.COMMENT_TSMI, "COMMENT_TSMI");
            this.COMMENT_TSMI.Name = "COMMENT_TSMI";
            this.COMMENT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CDATA_TSMI
            // 
            resources.ApplyResources(this.CDATA_TSMI, "CDATA_TSMI");
            this.CDATA_TSMI.Name = "CDATA_TSMI";
            this.CDATA_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM_TSMI
            // 
            resources.ApplyResources(this.CPARAM_TSMI, "CPARAM_TSMI");
            this.CPARAM_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CPARAM1_TSMI,
            this.CPARAM2_TSMI});
            this.CPARAM_TSMI.Name = "CPARAM_TSMI";
            this.CPARAM_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM1_TSMI
            // 
            resources.ApplyResources(this.CPARAM1_TSMI, "CPARAM1_TSMI");
            this.CPARAM1_TSMI.Name = "CPARAM1_TSMI";
            this.CPARAM1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM2_TSMI
            // 
            resources.ApplyResources(this.CPARAM2_TSMI, "CPARAM2_TSMI");
            this.CPARAM2_TSMI.Name = "CPARAM2_TSMI";
            this.CPARAM2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // btnSaveQueryFile
            // 
            resources.ApplyResources(this.btnSaveQueryFile, "btnSaveQueryFile");
            this.btnSaveQueryFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnSaveQueryFile.ForeColor = System.Drawing.Color.White;
            this.btnSaveQueryFile.Name = "btnSaveQueryFile";
            this.btnSaveQueryFile.UseVisualStyleBackColor = false;
            this.btnSaveQueryFile.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnSaveQueryFile.Click += new System.EventHandler(this.btnSaveQueryFile_Click);
            // 
            // btnOpenQueryFile
            // 
            resources.ApplyResources(this.btnOpenQueryFile, "btnOpenQueryFile");
            this.btnOpenQueryFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.btnOpenQueryFile.ForeColor = System.Drawing.Color.White;
            this.btnOpenQueryFile.Name = "btnOpenQueryFile";
            this.btnOpenQueryFile.UseVisualStyleBackColor = false;
            this.btnOpenQueryFile.EnabledChanged += new System.EventHandler(this.btnColor1_EnabledChanged);
            this.btnOpenQueryFile.Click += new System.EventHandler(this.btnOpenQueryFile_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
            this.panel1.Controls.Add(this.label9);
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Name = "panel1";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Name = "label9";
            // 
            // label0
            // 
            resources.ApplyResources(this.label0, "label0");
            this.label0.Name = "label0";
            // 
            // txtSQL
            // 
            this.txtSQL.AcceptsTab = true;
            resources.ApplyResources(this.txtSQL, "txtSQL");
            this.txtSQL.DualFont = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSQL_KeyDown);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.Controls.Add(this.groupBoxR);
            this.Controls.Add(this.btnExecQuery);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbSelMethod);
            this.Controls.Add(this.btnCloseQueryFile);
            this.Controls.Add(this.btnOverwriteQueryFile);
            this.Controls.Add(this.groupBoxTx);
            this.Controls.Add(this.btnCnClose);
            this.Controls.Add(this.btnCnOpen);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSaveQueryFile);
            this.Controls.Add(this.btnOpenQueryFile);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudNumOfBind);
            this.Controls.Add(this.cbxType);
            this.Controls.Add(this.label0);
            this.Controls.Add(this.txtSQL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.statBar);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxTx.ResumeLayout(false);
            this.groupBoxTx.PerformLayout();
            this.groupBoxR.ResumeLayout(false);
            this.groupBoxR.PerformLayout();
            this.statBar.ResumeLayout(false);
            this.statBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfBind)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDataProvider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCnnStr;
        private System.Windows.Forms.Button btnCnOpen;
        private System.Windows.Forms.Button btnCnClose;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnExecQuery;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.GroupBox groupBoxR;
        private System.Windows.Forms.ComboBox cmbSelMethod;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSelIso;
        private System.Windows.Forms.StatusStrip statBar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnCreateConfig;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSaveSlot;
        private System.Windows.Forms.Label label4;
        private RichTextBoxDisableDF txtSQL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnRollbackTx;
        private System.Windows.Forms.Button btnCommitTx;
        private System.Windows.Forms.Button btnBeginTx;
        private System.Windows.Forms.ComboBox cmbSelTxCtrl;
        private System.Windows.Forms.GroupBox groupBoxTx;
        private System.Windows.Forms.CheckBox cbxType;
        private System.Windows.Forms.NumericUpDown nudNumOfBind;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SAMPLE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem TEMPLATE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ROOT_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ROOT1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ROOT2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem WHERE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem WHERE1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem WHERE2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_ELSE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TXT_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TXT1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TXT2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TAG_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TAG1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem IF_TAG2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ELSE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ELSE1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem ELSE2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem LIST_TSMI;
        private System.Windows.Forms.ToolStripMenuItem LIST1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem LIST2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem JOIN_TSMI;
        private System.Windows.Forms.ToolStripMenuItem JOIN1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem JOIN2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem SUB_TSMI;
        private System.Windows.Forms.ToolStripMenuItem SUB1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem SUB2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem SELECT_CASE_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DELCMA_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DELCMA1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DELCMA2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem VAL_TSMI;
        private System.Windows.Forms.ToolStripMenuItem PARAM_TSMI;
        private System.Windows.Forms.ToolStripMenuItem PARAM1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem PARAM2_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DIV_TSMI;
        private System.Windows.Forms.ToolStripMenuItem XML_TSMI;
        private System.Windows.Forms.ToolStripMenuItem COMMENT_TSMI;
        private System.Windows.Forms.ToolStripMenuItem CDATA_TSMI;
        private System.Windows.Forms.ToolStripMenuItem CPARAM_TSMI;
        private System.Windows.Forms.ToolStripMenuItem CPARAM1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem CPARAM2_TSMI;
        private System.Windows.Forms.Button btnSaveQueryFile;
        private System.Windows.Forms.Button btnOpenQueryFile;
        private System.Windows.Forms.Button btnCloseQueryFile;
        private System.Windows.Forms.Button btnOverwriteQueryFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label0;
    }
}

