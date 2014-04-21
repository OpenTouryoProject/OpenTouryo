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
            this.cmbDataProvider = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCnnStr = new System.Windows.Forms.TextBox();
            this.btnCnOpen = new System.Windows.Forms.Button();
            this.btnCnClose = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnExecQuery = new System.Windows.Forms.Button();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.btnOpenQueryFile = new System.Windows.Forms.Button();
            this.groupBoxEXE = new System.Windows.Forms.GroupBox();
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
            this.groupBoxCN = new System.Windows.Forms.GroupBox();
            this.groupBoxQF = new System.Windows.Forms.GroupBox();
            this.btnSaveQueryFile = new System.Windows.Forms.Button();
            this.btnCloseQueryFile = new System.Windows.Forms.Button();
            this.btnOverwriteQueryFile = new System.Windows.Forms.Button();
            this.groupBoxR = new System.Windows.Forms.GroupBox();
            this.cmbSaveSlot = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCreateConfig = new System.Windows.Forms.Button();
            this.statBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label0 = new System.Windows.Forms.Label();
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
            this.DECMA_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DECMA1_TSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DECMA2_TSMI = new System.Windows.Forms.ToolStripMenuItem();
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
            this.txtSQL = new DPQuery_Tool.RichTextBoxDisableDF();
            this.groupBoxEXE.SuspendLayout();
            this.groupBoxTx.SuspendLayout();
            this.groupBoxCN.SuspendLayout();
            this.groupBoxQF.SuspendLayout();
            this.groupBoxR.SuspendLayout();
            this.statBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfBind)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDataProvider
            // 
            this.cmbDataProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataProvider.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbDataProvider.Location = new System.Drawing.Point(208, 565);
            this.cmbDataProvider.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbDataProvider.Name = "cmbDataProvider";
            this.cmbDataProvider.Size = new System.Drawing.Size(212, 23);
            this.cmbDataProvider.TabIndex = 0;
            this.cmbDataProvider.SelectedIndexChanged += new System.EventHandler(this.cmbDataProvider_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 569);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "データプロバイダを選択します";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(429, 569);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "接続文字列を設定します";
            // 
            // txtCnnStr
            // 
            this.txtCnnStr.Location = new System.Drawing.Point(605, 565);
            this.txtCnnStr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCnnStr.Name = "txtCnnStr";
            this.txtCnnStr.Size = new System.Drawing.Size(705, 22);
            this.txtCnnStr.TabIndex = 1;
            // 
            // btnCnOpen
            // 
            this.btnCnOpen.Location = new System.Drawing.Point(12, 39);
            this.btnCnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCnOpen.Name = "btnCnOpen";
            this.btnCnOpen.Size = new System.Drawing.Size(69, 29);
            this.btnCnOpen.TabIndex = 0;
            this.btnCnOpen.Text = "接続";
            this.btnCnOpen.UseVisualStyleBackColor = true;
            this.btnCnOpen.Click += new System.EventHandler(this.btnCnOpen_Click);
            // 
            // btnCnClose
            // 
            this.btnCnClose.Location = new System.Drawing.Point(12, 88);
            this.btnCnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCnClose.Name = "btnCnClose";
            this.btnCnClose.Size = new System.Drawing.Size(69, 29);
            this.btnCnClose.TabIndex = 1;
            this.btnCnClose.Text = "切断";
            this.btnCnClose.UseVisualStyleBackColor = true;
            this.btnCnClose.Click += new System.EventHandler(this.btnCnClose_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(181, 71);
            this.btnSaveConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(120, 29);
            this.btnSaveConfig.TabIndex = 2;
            this.btnSaveConfig.Text = "設定のセーブ";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnExecQuery
            // 
            this.btnExecQuery.Location = new System.Drawing.Point(651, 55);
            this.btnExecQuery.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExecQuery.Name = "btnExecQuery";
            this.btnExecQuery.Size = new System.Drawing.Size(320, 101);
            this.btnExecQuery.TabIndex = 4;
            this.btnExecQuery.Text = "クエリ実行";
            this.btnExecQuery.UseVisualStyleBackColor = true;
            this.btnExecQuery.Click += new System.EventHandler(this.btnExecQuery_Click);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(181, 119);
            this.btnLoadConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(120, 29);
            this.btnLoadConfig.TabIndex = 3;
            this.btnLoadConfig.Text = "設定のロード";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // btnOpenQueryFile
            // 
            this.btnOpenQueryFile.Location = new System.Drawing.Point(11, 39);
            this.btnOpenQueryFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpenQueryFile.Name = "btnOpenQueryFile";
            this.btnOpenQueryFile.Size = new System.Drawing.Size(133, 29);
            this.btnOpenQueryFile.TabIndex = 0;
            this.btnOpenQueryFile.Text = "を開く";
            this.btnOpenQueryFile.UseVisualStyleBackColor = true;
            this.btnOpenQueryFile.Click += new System.EventHandler(this.btnOpenQueryFile_Click);
            // 
            // groupBoxEXE
            // 
            this.groupBoxEXE.Controls.Add(this.cmbSelMethod);
            this.groupBoxEXE.Controls.Add(this.groupBoxTx);
            this.groupBoxEXE.Controls.Add(this.label5);
            this.groupBoxEXE.Controls.Add(this.groupBoxCN);
            this.groupBoxEXE.Controls.Add(this.groupBoxQF);
            this.groupBoxEXE.Controls.Add(this.btnExecQuery);
            this.groupBoxEXE.Location = new System.Drawing.Point(332, 609);
            this.groupBoxEXE.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxEXE.Name = "groupBoxEXE";
            this.groupBoxEXE.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxEXE.Size = new System.Drawing.Size(980, 164);
            this.groupBoxEXE.TabIndex = 3;
            this.groupBoxEXE.TabStop = false;
            this.groupBoxEXE.Text = "実行制御";
            // 
            // cmbSelMethod
            // 
            this.cmbSelMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelMethod.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbSelMethod.Location = new System.Drawing.Point(839, 22);
            this.cmbSelMethod.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSelMethod.Name = "cmbSelMethod";
            this.cmbSelMethod.Size = new System.Drawing.Size(131, 23);
            this.cmbSelMethod.TabIndex = 3;
            // 
            // groupBoxTx
            // 
            this.groupBoxTx.Controls.Add(this.label7);
            this.groupBoxTx.Controls.Add(this.btnRollbackTx);
            this.groupBoxTx.Controls.Add(this.cmbSelIso);
            this.groupBoxTx.Controls.Add(this.label6);
            this.groupBoxTx.Controls.Add(this.btnCommitTx);
            this.groupBoxTx.Controls.Add(this.btnBeginTx);
            this.groupBoxTx.Controls.Add(this.cmbSelTxCtrl);
            this.groupBoxTx.Location = new System.Drawing.Point(432, 14);
            this.groupBoxTx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxTx.Name = "groupBoxTx";
            this.groupBoxTx.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxTx.Size = new System.Drawing.Size(205, 142);
            this.groupBoxTx.TabIndex = 2;
            this.groupBoxTx.TabStop = false;
            this.groupBoxTx.Text = "トランザクション";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 52);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "制御方式";
            // 
            // btnRollbackTx
            // 
            this.btnRollbackTx.Location = new System.Drawing.Point(105, 109);
            this.btnRollbackTx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRollbackTx.Name = "btnRollbackTx";
            this.btnRollbackTx.Size = new System.Drawing.Size(93, 29);
            this.btnRollbackTx.TabIndex = 4;
            this.btnRollbackTx.Text = "ロールバック";
            this.btnRollbackTx.UseVisualStyleBackColor = true;
            this.btnRollbackTx.Click += new System.EventHandler(this.btnRollbackTx_Click);
            // 
            // cmbSelIso
            // 
            this.cmbSelIso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelIso.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbSelIso.Location = new System.Drawing.Point(87, 20);
            this.cmbSelIso.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSelIso.Name = "cmbSelIso";
            this.cmbSelIso.Size = new System.Drawing.Size(109, 23);
            this.cmbSelIso.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 24);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "分離レベル";
            // 
            // btnCommitTx
            // 
            this.btnCommitTx.Location = new System.Drawing.Point(8, 109);
            this.btnCommitTx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCommitTx.Name = "btnCommitTx";
            this.btnCommitTx.Size = new System.Drawing.Size(93, 29);
            this.btnCommitTx.TabIndex = 3;
            this.btnCommitTx.Text = "コミット";
            this.btnCommitTx.UseVisualStyleBackColor = true;
            this.btnCommitTx.Click += new System.EventHandler(this.btnCommitTx_Click);
            // 
            // btnBeginTx
            // 
            this.btnBeginTx.Location = new System.Drawing.Point(8, 78);
            this.btnBeginTx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBeginTx.Name = "btnBeginTx";
            this.btnBeginTx.Size = new System.Drawing.Size(191, 29);
            this.btnBeginTx.TabIndex = 2;
            this.btnBeginTx.Text = "開始";
            this.btnBeginTx.UseVisualStyleBackColor = true;
            this.btnBeginTx.Click += new System.EventHandler(this.btnBeginTx_Click);
            // 
            // cmbSelTxCtrl
            // 
            this.cmbSelTxCtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelTxCtrl.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbSelTxCtrl.Location = new System.Drawing.Point(87, 49);
            this.cmbSelTxCtrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSelTxCtrl.Name = "cmbSelTxCtrl";
            this.cmbSelTxCtrl.Size = new System.Drawing.Size(109, 23);
            this.cmbSelTxCtrl.TabIndex = 1;
            this.cmbSelTxCtrl.SelectedIndexChanged += new System.EventHandler(this.cmbSelTxCtrl_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(651, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "インターフェイスを選択します";
            // 
            // groupBoxCN
            // 
            this.groupBoxCN.Controls.Add(this.btnCnOpen);
            this.groupBoxCN.Controls.Add(this.btnCnClose);
            this.groupBoxCN.Location = new System.Drawing.Point(15, 22);
            this.groupBoxCN.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxCN.Name = "groupBoxCN";
            this.groupBoxCN.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxCN.Size = new System.Drawing.Size(92, 134);
            this.groupBoxCN.TabIndex = 0;
            this.groupBoxCN.TabStop = false;
            this.groupBoxCN.Text = "DB接続";
            // 
            // groupBoxQF
            // 
            this.groupBoxQF.Controls.Add(this.btnSaveQueryFile);
            this.groupBoxQF.Controls.Add(this.btnOpenQueryFile);
            this.groupBoxQF.Controls.Add(this.btnCloseQueryFile);
            this.groupBoxQF.Controls.Add(this.btnOverwriteQueryFile);
            this.groupBoxQF.Location = new System.Drawing.Point(120, 22);
            this.groupBoxQF.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxQF.Name = "groupBoxQF";
            this.groupBoxQF.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxQF.Size = new System.Drawing.Size(297, 134);
            this.groupBoxQF.TabIndex = 1;
            this.groupBoxQF.TabStop = false;
            this.groupBoxQF.Text = "クエリ ファイル";
            // 
            // btnSaveQueryFile
            // 
            this.btnSaveQueryFile.Location = new System.Drawing.Point(153, 86);
            this.btnSaveQueryFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveQueryFile.Name = "btnSaveQueryFile";
            this.btnSaveQueryFile.Size = new System.Drawing.Size(133, 30);
            this.btnSaveQueryFile.TabIndex = 3;
            this.btnSaveQueryFile.Text = "に保存";
            this.btnSaveQueryFile.UseVisualStyleBackColor = true;
            this.btnSaveQueryFile.Click += new System.EventHandler(this.btnSaveQueryFile_Click);
            // 
            // btnCloseQueryFile
            // 
            this.btnCloseQueryFile.Location = new System.Drawing.Point(153, 39);
            this.btnCloseQueryFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCloseQueryFile.Name = "btnCloseQueryFile";
            this.btnCloseQueryFile.Size = new System.Drawing.Size(133, 29);
            this.btnCloseQueryFile.TabIndex = 1;
            this.btnCloseQueryFile.Text = "を閉じる";
            this.btnCloseQueryFile.UseVisualStyleBackColor = true;
            this.btnCloseQueryFile.Click += new System.EventHandler(this.btnCloseQueryFile_Click);
            // 
            // btnOverwriteQueryFile
            // 
            this.btnOverwriteQueryFile.Location = new System.Drawing.Point(11, 88);
            this.btnOverwriteQueryFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOverwriteQueryFile.Name = "btnOverwriteQueryFile";
            this.btnOverwriteQueryFile.Size = new System.Drawing.Size(133, 29);
            this.btnOverwriteQueryFile.TabIndex = 2;
            this.btnOverwriteQueryFile.Text = "を上書き保存";
            this.btnOverwriteQueryFile.UseVisualStyleBackColor = true;
            this.btnOverwriteQueryFile.Click += new System.EventHandler(this.btnOverwriteQueryFile_Click);
            // 
            // groupBoxR
            // 
            this.groupBoxR.Controls.Add(this.cmbSaveSlot);
            this.groupBoxR.Controls.Add(this.label4);
            this.groupBoxR.Controls.Add(this.btnCreateConfig);
            this.groupBoxR.Controls.Add(this.btnLoadConfig);
            this.groupBoxR.Controls.Add(this.btnSaveConfig);
            this.groupBoxR.Location = new System.Drawing.Point(0, 609);
            this.groupBoxR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxR.Name = "groupBoxR";
            this.groupBoxR.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxR.Size = new System.Drawing.Size(321, 164);
            this.groupBoxR.TabIndex = 2;
            this.groupBoxR.TabStop = false;
            this.groupBoxR.Text = "設定";
            // 
            // cmbSaveSlot
            // 
            this.cmbSaveSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSaveSlot.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbSaveSlot.Location = new System.Drawing.Point(31, 105);
            this.cmbSaveSlot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSaveSlot.Name = "cmbSaveSlot";
            this.cmbSaveSlot.Size = new System.Drawing.Size(123, 23);
            this.cmbSaveSlot.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 86);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "スロットを選択します。";
            // 
            // btnCreateConfig
            // 
            this.btnCreateConfig.Location = new System.Drawing.Point(21, 24);
            this.btnCreateConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreateConfig.Name = "btnCreateConfig";
            this.btnCreateConfig.Size = new System.Drawing.Size(280, 29);
            this.btnCreateConfig.TabIndex = 0;
            this.btnCreateConfig.Text = "設定の作成";
            this.btnCreateConfig.UseVisualStyleBackColor = true;
            this.btnCreateConfig.Click += new System.EventHandler(this.btnCreateConfig_Click);
            // 
            // statBar
            // 
            this.statBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statBar.Location = new System.Drawing.Point(0, 782);
            this.statBar.Name = "statBar";
            this.statBar.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statBar.Size = new System.Drawing.Size(1312, 22);
            this.statBar.TabIndex = 0;
            this.statBar.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(109, 532);
            this.lblFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(22, 15);
            this.lblFilePath.TabIndex = 0;
            this.lblFilePath.Text = "－";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 532);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ファイル名：";
            // 
            // label0
            // 
            this.label0.AutoSize = true;
            this.label0.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label0.Location = new System.Drawing.Point(264, 500);
            this.label0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label0.Name = "label0";
            this.label0.Size = new System.Drawing.Size(959, 15);
            this.label0.TabIndex = 0;
            this.label0.Text = "テキスト ボックス内のショートカット ： Alt+O（ファイルを開く），Alt+S（ファイルに保存），Alt+C（ファイルを閉じる），Alt+E（クエリを実行），" +
    "Alt+F4（終了）";
            // 
            // cbxType
            // 
            this.cbxType.AutoSize = true;
            this.cbxType.Location = new System.Drawing.Point(833, 531);
            this.cbxType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(169, 19);
            this.cbxType.TabIndex = 4;
            this.cbxType.Text = "型推測モード（型指定）";
            this.cbxType.UseVisualStyleBackColor = true;
            // 
            // nudNumOfBind
            // 
            this.nudNumOfBind.Location = new System.Drawing.Point(1060, 528);
            this.nudNumOfBind.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudNumOfBind.Name = "nudNumOfBind";
            this.nudNumOfBind.Size = new System.Drawing.Size(77, 22);
            this.nudNumOfBind.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1145, 532);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 15);
            this.label8.TabIndex = 7;
            this.label8.Text = "配列バインド数指定";
            // 
            // contextMenuStrip1
            // 
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
            this.DECMA_TSMI,
            this.VAL_TSMI,
            this.PARAM_TSMI,
            this.DIV_TSMI,
            this.COMMENT_TSMI,
            this.CDATA_TSMI,
            this.CPARAM_TSMI});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 502);
            // 
            // SAMPLE_TSMI
            // 
            this.SAMPLE_TSMI.Name = "SAMPLE_TSMI";
            this.SAMPLE_TSMI.Size = new System.Drawing.Size(181, 28);
            this.SAMPLE_TSMI.Text = "SAMPLE";
            this.SAMPLE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // TEMPLATE_TSMI
            // 
            this.TEMPLATE_TSMI.Name = "TEMPLATE_TSMI";
            this.TEMPLATE_TSMI.Size = new System.Drawing.Size(181, 28);
            this.TEMPLATE_TSMI.Text = "TEMPLATE";
            this.TEMPLATE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // XML_TSMI
            // 
            this.XML_TSMI.Name = "XML_TSMI";
            this.XML_TSMI.Size = new System.Drawing.Size(181, 28);
            this.XML_TSMI.Text = "XML";
            this.XML_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT_TSMI
            // 
            this.ROOT_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ROOT1_TSMI,
            this.ROOT2_TSMI});
            this.ROOT_TSMI.Name = "ROOT_TSMI";
            this.ROOT_TSMI.Size = new System.Drawing.Size(181, 28);
            this.ROOT_TSMI.Text = "ROOT";
            this.ROOT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT1_TSMI
            // 
            this.ROOT1_TSMI.Name = "ROOT1_TSMI";
            this.ROOT1_TSMI.Size = new System.Drawing.Size(131, 28);
            this.ROOT1_TSMI.Text = "ROOT1";
            this.ROOT1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ROOT2_TSMI
            // 
            this.ROOT2_TSMI.Name = "ROOT2_TSMI";
            this.ROOT2_TSMI.Size = new System.Drawing.Size(131, 28);
            this.ROOT2_TSMI.Text = "ROOT2";
            this.ROOT2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE_TSMI
            // 
            this.WHERE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WHERE1_TSMI,
            this.WHERE2_TSMI});
            this.WHERE_TSMI.Name = "WHERE_TSMI";
            this.WHERE_TSMI.Size = new System.Drawing.Size(181, 28);
            this.WHERE_TSMI.Text = "WHERE";
            this.WHERE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE1_TSMI
            // 
            this.WHERE1_TSMI.Name = "WHERE1_TSMI";
            this.WHERE1_TSMI.Size = new System.Drawing.Size(143, 28);
            this.WHERE1_TSMI.Text = "WHERE1";
            this.WHERE1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // WHERE2_TSMI
            // 
            this.WHERE2_TSMI.Name = "WHERE2_TSMI";
            this.WHERE2_TSMI.Size = new System.Drawing.Size(143, 28);
            this.WHERE2_TSMI.Text = "WHERE2";
            this.WHERE2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_ELSE_TSMI
            // 
            this.IF_ELSE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TXT_TSMI,
            this.IF_TAG_TSMI,
            this.ELSE_TSMI});
            this.IF_ELSE_TSMI.Name = "IF_ELSE_TSMI";
            this.IF_ELSE_TSMI.Size = new System.Drawing.Size(181, 28);
            this.IF_ELSE_TSMI.Text = "IF-ELSE";
            this.IF_ELSE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT_TSMI
            // 
            this.IF_TXT_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TXT1_TSMI,
            this.IF_TXT2_TSMI});
            this.IF_TXT_TSMI.Name = "IF_TXT_TSMI";
            this.IF_TXT_TSMI.Size = new System.Drawing.Size(134, 28);
            this.IF_TXT_TSMI.Text = "IF_TXT";
            this.IF_TXT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT1_TSMI
            // 
            this.IF_TXT1_TSMI.Name = "IF_TXT1_TSMI";
            this.IF_TXT1_TSMI.Size = new System.Drawing.Size(152, 28);
            this.IF_TXT1_TSMI.Text = "IF_TXT11";
            this.IF_TXT1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TXT2_TSMI
            // 
            this.IF_TXT2_TSMI.Name = "IF_TXT2_TSMI";
            this.IF_TXT2_TSMI.Size = new System.Drawing.Size(152, 28);
            this.IF_TXT2_TSMI.Text = "IF_TXT12";
            this.IF_TXT2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG_TSMI
            // 
            this.IF_TAG_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IF_TAG1_TSMI,
            this.IF_TAG2_TSMI});
            this.IF_TAG_TSMI.Name = "IF_TAG_TSMI";
            this.IF_TAG_TSMI.Size = new System.Drawing.Size(134, 28);
            this.IF_TAG_TSMI.Text = "IF_TAG";
            this.IF_TAG_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG1_TSMI
            // 
            this.IF_TAG1_TSMI.Name = "IF_TAG1_TSMI";
            this.IF_TAG1_TSMI.Size = new System.Drawing.Size(143, 28);
            this.IF_TAG1_TSMI.Text = "IF_TAG1";
            this.IF_TAG1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // IF_TAG2_TSMI
            // 
            this.IF_TAG2_TSMI.Name = "IF_TAG2_TSMI";
            this.IF_TAG2_TSMI.Size = new System.Drawing.Size(143, 28);
            this.IF_TAG2_TSMI.Text = "IF_TAG2";
            this.IF_TAG2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE_TSMI
            // 
            this.ELSE_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ELSE1_TSMI,
            this.ELSE2_TSMI});
            this.ELSE_TSMI.Name = "ELSE_TSMI";
            this.ELSE_TSMI.Size = new System.Drawing.Size(134, 28);
            this.ELSE_TSMI.Text = "ELSE";
            this.ELSE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE1_TSMI
            // 
            this.ELSE1_TSMI.Name = "ELSE1_TSMI";
            this.ELSE1_TSMI.Size = new System.Drawing.Size(125, 28);
            this.ELSE1_TSMI.Text = "ELSE1";
            this.ELSE1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // ELSE2_TSMI
            // 
            this.ELSE2_TSMI.Name = "ELSE2_TSMI";
            this.ELSE2_TSMI.Size = new System.Drawing.Size(125, 28);
            this.ELSE2_TSMI.Text = "ELSE2";
            this.ELSE2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST_TSMI
            // 
            this.LIST_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LIST1_TSMI,
            this.LIST2_TSMI});
            this.LIST_TSMI.Name = "LIST_TSMI";
            this.LIST_TSMI.Size = new System.Drawing.Size(181, 28);
            this.LIST_TSMI.Text = "LIST";
            this.LIST_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST1_TSMI
            // 
            this.LIST1_TSMI.Name = "LIST1_TSMI";
            this.LIST1_TSMI.Size = new System.Drawing.Size(123, 28);
            this.LIST1_TSMI.Text = "LIST1";
            this.LIST1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // LIST2_TSMI
            // 
            this.LIST2_TSMI.Name = "LIST2_TSMI";
            this.LIST2_TSMI.Size = new System.Drawing.Size(123, 28);
            this.LIST2_TSMI.Text = "LIST2";
            this.LIST2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN_TSMI
            // 
            this.JOIN_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JOIN1_TSMI,
            this.JOIN2_TSMI});
            this.JOIN_TSMI.Name = "JOIN_TSMI";
            this.JOIN_TSMI.Size = new System.Drawing.Size(181, 28);
            this.JOIN_TSMI.Text = "JOIN";
            this.JOIN_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN1_TSMI
            // 
            this.JOIN1_TSMI.Name = "JOIN1_TSMI";
            this.JOIN1_TSMI.Size = new System.Drawing.Size(124, 28);
            this.JOIN1_TSMI.Text = "JOIN1";
            this.JOIN1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // JOIN2_TSMI
            // 
            this.JOIN2_TSMI.Name = "JOIN2_TSMI";
            this.JOIN2_TSMI.Size = new System.Drawing.Size(124, 28);
            this.JOIN2_TSMI.Text = "JOIN2";
            this.JOIN2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB_TSMI
            // 
            this.SUB_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SUB1_TSMI,
            this.SUB2_TSMI});
            this.SUB_TSMI.Name = "SUB_TSMI";
            this.SUB_TSMI.Size = new System.Drawing.Size(181, 28);
            this.SUB_TSMI.Text = "SUB";
            this.SUB_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB1_TSMI
            // 
            this.SUB1_TSMI.Name = "SUB1_TSMI";
            this.SUB1_TSMI.Size = new System.Drawing.Size(120, 28);
            this.SUB1_TSMI.Text = "SUB1";
            this.SUB1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SUB2_TSMI
            // 
            this.SUB2_TSMI.Name = "SUB2_TSMI";
            this.SUB2_TSMI.Size = new System.Drawing.Size(120, 28);
            this.SUB2_TSMI.Text = "SUB2";
            this.SUB2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // SELECT_CASE_TSMI
            // 
            this.SELECT_CASE_TSMI.Name = "SELECT_CASE_TSMI";
            this.SELECT_CASE_TSMI.Size = new System.Drawing.Size(181, 28);
            this.SELECT_CASE_TSMI.Text = "SELECT-CASE";
            this.SELECT_CASE_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DECMA_TSMI
            // 
            this.DECMA_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DECMA1_TSMI,
            this.DECMA2_TSMI});
            this.DECMA_TSMI.Name = "DECMA_TSMI";
            this.DECMA_TSMI.Size = new System.Drawing.Size(181, 28);
            this.DECMA_TSMI.Text = "DECMA";
            this.DECMA_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DECMA1_TSMI
            // 
            this.DECMA1_TSMI.Name = "DECMA1_TSMI";
            this.DECMA1_TSMI.Size = new System.Drawing.Size(141, 28);
            this.DECMA1_TSMI.Text = "DECMA1";
            this.DECMA1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DECMA2_TSMI
            // 
            this.DECMA2_TSMI.Name = "DECMA2_TSMI";
            this.DECMA2_TSMI.Size = new System.Drawing.Size(141, 28);
            this.DECMA2_TSMI.Text = "DECMA2";
            this.DECMA2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // VAL_TSMI
            // 
            this.VAL_TSMI.Name = "VAL_TSMI";
            this.VAL_TSMI.Size = new System.Drawing.Size(181, 28);
            this.VAL_TSMI.Text = "VAL";
            this.VAL_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM_TSMI
            // 
            this.PARAM_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PARAM1_TSMI,
            this.PARAM2_TSMI});
            this.PARAM_TSMI.Name = "PARAM_TSMI";
            this.PARAM_TSMI.Size = new System.Drawing.Size(181, 28);
            this.PARAM_TSMI.Text = "PARAM";
            this.PARAM_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM1_TSMI
            // 
            this.PARAM1_TSMI.Name = "PARAM1_TSMI";
            this.PARAM1_TSMI.Size = new System.Drawing.Size(140, 28);
            this.PARAM1_TSMI.Text = "PARAM1";
            this.PARAM1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // PARAM2_TSMI
            // 
            this.PARAM2_TSMI.Name = "PARAM2_TSMI";
            this.PARAM2_TSMI.Size = new System.Drawing.Size(140, 28);
            this.PARAM2_TSMI.Text = "PARAM2";
            this.PARAM2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // DIV_TSMI
            // 
            this.DIV_TSMI.Name = "DIV_TSMI";
            this.DIV_TSMI.Size = new System.Drawing.Size(181, 28);
            this.DIV_TSMI.Text = "DIV";
            this.DIV_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // COMMENT_TSMI
            // 
            this.COMMENT_TSMI.Name = "COMMENT_TSMI";
            this.COMMENT_TSMI.Size = new System.Drawing.Size(181, 28);
            this.COMMENT_TSMI.Text = "COMMENT";
            this.COMMENT_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CDATA_TSMI
            // 
            this.CDATA_TSMI.Name = "CDATA_TSMI";
            this.CDATA_TSMI.Size = new System.Drawing.Size(181, 28);
            this.CDATA_TSMI.Text = "CDATA";
            this.CDATA_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM_TSMI
            // 
            this.CPARAM_TSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CPARAM1_TSMI,
            this.CPARAM2_TSMI});
            this.CPARAM_TSMI.Name = "CPARAM_TSMI";
            this.CPARAM_TSMI.Size = new System.Drawing.Size(181, 28);
            this.CPARAM_TSMI.Text = "CPARAM";
            this.CPARAM_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM1_TSMI
            // 
            this.CPARAM1_TSMI.Name = "CPARAM1_TSMI";
            this.CPARAM1_TSMI.Size = new System.Drawing.Size(152, 28);
            this.CPARAM1_TSMI.Text = "CPARAM1";
            this.CPARAM1_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // CPARAM2_TSMI
            // 
            this.CPARAM2_TSMI.Name = "CPARAM2_TSMI";
            this.CPARAM2_TSMI.Size = new System.Drawing.Size(152, 28);
            this.CPARAM2_TSMI.Text = "CPARAM2";
            this.CPARAM2_TSMI.Click += new System.EventHandler(this.TSMI_Click);
            // 
            // txtSQL
            // 
            this.txtSQL.AcceptsTab = true;
            this.txtSQL.DualFont = true;
            this.txtSQL.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSQL.Location = new System.Drawing.Point(0, 1);
            this.txtSQL.Margin = new System.Windows.Forms.Padding(4);
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.Size = new System.Drawing.Size(1311, 513);
            this.txtSQL.TabIndex = 5;
            this.txtSQL.Text = "";
            this.txtSQL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSQL_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1312, 804);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudNumOfBind);
            this.Controls.Add(this.cbxType);
            this.Controls.Add(this.label0);
            this.Controls.Add(this.txtSQL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.statBar);
            this.Controls.Add(this.cmbDataProvider);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCnnStr);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBoxEXE);
            this.Controls.Add(this.groupBoxR);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "棟梁：動的パラメタライズド・クエリ 分析ツール";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxEXE.ResumeLayout(false);
            this.groupBoxEXE.PerformLayout();
            this.groupBoxTx.ResumeLayout(false);
            this.groupBoxTx.PerformLayout();
            this.groupBoxCN.ResumeLayout(false);
            this.groupBoxQF.ResumeLayout(false);
            this.groupBoxR.ResumeLayout(false);
            this.groupBoxR.PerformLayout();
            this.statBar.ResumeLayout(false);
            this.statBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumOfBind)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnOpenQueryFile;
        private System.Windows.Forms.GroupBox groupBoxEXE;
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
        private System.Windows.Forms.Button btnCloseQueryFile;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOverwriteQueryFile;
        private System.Windows.Forms.ComboBox cmbSaveSlot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxQF;
        private System.Windows.Forms.Button btnSaveQueryFile;
        private RichTextBoxDisableDF txtSQL;
        private System.Windows.Forms.Label label0;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnRollbackTx;
        private System.Windows.Forms.Button btnCommitTx;
        private System.Windows.Forms.Button btnBeginTx;
        private System.Windows.Forms.ComboBox cmbSelTxCtrl;
        private System.Windows.Forms.GroupBox groupBoxCN;
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
        private System.Windows.Forms.ToolStripMenuItem DECMA_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DECMA1_TSMI;
        private System.Windows.Forms.ToolStripMenuItem DECMA2_TSMI;
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
    }
}

