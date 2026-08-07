namespace DeployZipPackWithHTTP
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblStatus = new System.Windows.Forms.Label();
            this.openFilesDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabDeployZipPac = new System.Windows.Forms.TabControl();
            this.tabPageDeployWithHTTP = new System.Windows.Forms.TabPage();
            this.btnDelHistory = new System.Windows.Forms.Button();
            this.lblHistorys = new System.Windows.Forms.Label();
            this.cmbHistorys = new System.Windows.Forms.ComboBox();
            this.btnCheckUpdateAndInstall = new System.Windows.Forms.Button();
            this.gbxWWW = new System.Windows.Forms.GroupBox();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lblURL = new System.Windows.Forms.Label();
            this.txtPWD = new System.Windows.Forms.TextBox();
            this.lblPWD = new System.Windows.Forms.Label();
            this.lblUID = new System.Windows.Forms.Label();
            this.txtUID = new System.Windows.Forms.TextBox();
            this.gbxProxy = new System.Windows.Forms.GroupBox();
            this.txtProxyDomain = new System.Windows.Forms.TextBox();
            this.lblProxyDomain = new System.Windows.Forms.Label();
            this.lblProxyPWD = new System.Windows.Forms.Label();
            this.txtProxyURL = new System.Windows.Forms.TextBox();
            this.txtProxyPWD = new System.Windows.Forms.TextBox();
            this.lblProxyURL = new System.Windows.Forms.Label();
            this.txtProxyUID = new System.Windows.Forms.TextBox();
            this.lblProxyUID = new System.Windows.Forms.Label();
            this.tabPageZipPackage = new System.Windows.Forms.TabPage();
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.lblFile = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblEnc = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.lblExt = new System.Windows.Forms.Label();
            this.btnSelectSaveFile = new System.Windows.Forms.Button();
            this.cbxExt = new System.Windows.Forms.CheckBox();
            this.txtExt = new System.Windows.Forms.TextBox();
            this.tabZipUnZip = new System.Windows.Forms.TabControl();
            this.tabPageZip = new System.Windows.Forms.TabPage();
            this.lblRootDirCheck = new System.Windows.Forms.Label();
            this.cbxRootDir = new System.Windows.Forms.CheckBox();
            this.cbxFormat = new System.Windows.Forms.CheckBox();
            this.btnCompress = new System.Windows.Forms.Button();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cmbCmpLv = new System.Windows.Forms.ComboBox();
            this.lblCyp = new System.Windows.Forms.Label();
            this.lblCmpLv = new System.Windows.Forms.Label();
            this.cmbCyp = new System.Windows.Forms.ComboBox();
            this.tabPageUnZip = new System.Windows.Forms.TabPage();
            this.btnDecomp = new System.Windows.Forms.Button();
            this.lblEEFA = new System.Windows.Forms.Label();
            this.cmbEEFA = new System.Windows.Forms.ComboBox();
            this.cmbEnc = new System.Windows.Forms.ComboBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.tabPageManifesto = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInsDir = new System.Windows.Forms.Label();
            this.txtInsDir = new System.Windows.Forms.TextBox();
            this.lblExeName = new System.Windows.Forms.Label();
            this.txtExeName = new System.Windows.Forms.TextBox();
            this.btnCreateManifesto = new System.Windows.Forms.Button();
            this.btnRemoveZIPFile = new System.Windows.Forms.Button();
            this.btnAddZIPFile = new System.Windows.Forms.Button();
            this.lbxZIPFiles = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.tabDeployZipPac.SuspendLayout();
            this.tabPageDeployWithHTTP.SuspendLayout();
            this.gbxWWW.SuspendLayout();
            this.gbxProxy.SuspendLayout();
            this.tabPageZipPackage.SuspendLayout();
            this.tabZipUnZip.SuspendLayout();
            this.tabPageZip.SuspendLayout();
            this.tabPageUnZip.SuspendLayout();
            this.tabPageManifesto.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            // 
            // saveFileDialog1
            // 
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // openFilesDialog1
            // 
            this.openFilesDialog1.FileName = "openFilesDialog1";
            resources.ApplyResources(this.openFilesDialog1, "openFilesDialog1");
            this.openFilesDialog1.Multiselect = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.tabDeployZipPac);
            this.panel1.Name = "panel1";
            // 
            // tabDeployZipPac
            // 
            resources.ApplyResources(this.tabDeployZipPac, "tabDeployZipPac");
            this.tabDeployZipPac.Controls.Add(this.tabPageDeployWithHTTP);
            this.tabDeployZipPac.Controls.Add(this.tabPageZipPackage);
            this.tabDeployZipPac.Controls.Add(this.tabPageManifesto);
            this.tabDeployZipPac.Name = "tabDeployZipPac";
            this.tabDeployZipPac.SelectedIndex = 0;
            // 
            // tabPageDeployWithHTTP
            // 
            resources.ApplyResources(this.tabPageDeployWithHTTP, "tabPageDeployWithHTTP");
            this.tabPageDeployWithHTTP.Controls.Add(this.btnDelHistory);
            this.tabPageDeployWithHTTP.Controls.Add(this.lblHistorys);
            this.tabPageDeployWithHTTP.Controls.Add(this.cmbHistorys);
            this.tabPageDeployWithHTTP.Controls.Add(this.btnCheckUpdateAndInstall);
            this.tabPageDeployWithHTTP.Controls.Add(this.gbxWWW);
            this.tabPageDeployWithHTTP.Controls.Add(this.gbxProxy);
            this.tabPageDeployWithHTTP.Name = "tabPageDeployWithHTTP";
            this.tabPageDeployWithHTTP.UseVisualStyleBackColor = true;
            // 
            // btnDelHistory
            // 
            resources.ApplyResources(this.btnDelHistory, "btnDelHistory");
            this.btnDelHistory.Name = "btnDelHistory";
            this.btnDelHistory.UseVisualStyleBackColor = true;
            this.btnDelHistory.Click += new System.EventHandler(this.btnDelHistory_Click);
            // 
            // lblHistorys
            // 
            resources.ApplyResources(this.lblHistorys, "lblHistorys");
            this.lblHistorys.Name = "lblHistorys";
            // 
            // cmbHistorys
            // 
            resources.ApplyResources(this.cmbHistorys, "cmbHistorys");
            this.cmbHistorys.FormattingEnabled = true;
            this.cmbHistorys.Name = "cmbHistorys";
            this.cmbHistorys.SelectedIndexChanged += new System.EventHandler(this.cmbHistorys_SelectedIndexChanged);
            // 
            // btnCheckUpdateAndInstall
            // 
            resources.ApplyResources(this.btnCheckUpdateAndInstall, "btnCheckUpdateAndInstall");
            this.btnCheckUpdateAndInstall.Name = "btnCheckUpdateAndInstall";
            this.btnCheckUpdateAndInstall.UseVisualStyleBackColor = true;
            this.btnCheckUpdateAndInstall.Click += new System.EventHandler(this.btnCheckUpdateAndInstall_Click);
            // 
            // gbxWWW
            // 
            resources.ApplyResources(this.gbxWWW, "gbxWWW");
            this.gbxWWW.Controls.Add(this.txtURL);
            this.gbxWWW.Controls.Add(this.txtDomain);
            this.gbxWWW.Controls.Add(this.lblDomain);
            this.gbxWWW.Controls.Add(this.lblURL);
            this.gbxWWW.Controls.Add(this.txtPWD);
            this.gbxWWW.Controls.Add(this.lblPWD);
            this.gbxWWW.Controls.Add(this.lblUID);
            this.gbxWWW.Controls.Add(this.txtUID);
            this.gbxWWW.Name = "gbxWWW";
            this.gbxWWW.TabStop = false;
            // 
            // txtURL
            // 
            resources.ApplyResources(this.txtURL, "txtURL");
            this.txtURL.Name = "txtURL";
            // 
            // txtDomain
            // 
            resources.ApplyResources(this.txtDomain, "txtDomain");
            this.txtDomain.Name = "txtDomain";
            // 
            // lblDomain
            // 
            resources.ApplyResources(this.lblDomain, "lblDomain");
            this.lblDomain.Name = "lblDomain";
            // 
            // lblURL
            // 
            resources.ApplyResources(this.lblURL, "lblURL");
            this.lblURL.Name = "lblURL";
            // 
            // txtPWD
            // 
            resources.ApplyResources(this.txtPWD, "txtPWD");
            this.txtPWD.Name = "txtPWD";
            // 
            // lblPWD
            // 
            resources.ApplyResources(this.lblPWD, "lblPWD");
            this.lblPWD.Name = "lblPWD";
            // 
            // lblUID
            // 
            resources.ApplyResources(this.lblUID, "lblUID");
            this.lblUID.Name = "lblUID";
            // 
            // txtUID
            // 
            resources.ApplyResources(this.txtUID, "txtUID");
            this.txtUID.Name = "txtUID";
            // 
            // gbxProxy
            // 
            resources.ApplyResources(this.gbxProxy, "gbxProxy");
            this.gbxProxy.Controls.Add(this.txtProxyDomain);
            this.gbxProxy.Controls.Add(this.lblProxyDomain);
            this.gbxProxy.Controls.Add(this.lblProxyPWD);
            this.gbxProxy.Controls.Add(this.txtProxyURL);
            this.gbxProxy.Controls.Add(this.txtProxyPWD);
            this.gbxProxy.Controls.Add(this.lblProxyURL);
            this.gbxProxy.Controls.Add(this.txtProxyUID);
            this.gbxProxy.Controls.Add(this.lblProxyUID);
            this.gbxProxy.Name = "gbxProxy";
            this.gbxProxy.TabStop = false;
            // 
            // txtProxyDomain
            // 
            resources.ApplyResources(this.txtProxyDomain, "txtProxyDomain");
            this.txtProxyDomain.Name = "txtProxyDomain";
            // 
            // lblProxyDomain
            // 
            resources.ApplyResources(this.lblProxyDomain, "lblProxyDomain");
            this.lblProxyDomain.Name = "lblProxyDomain";
            // 
            // lblProxyPWD
            // 
            resources.ApplyResources(this.lblProxyPWD, "lblProxyPWD");
            this.lblProxyPWD.Name = "lblProxyPWD";
            // 
            // txtProxyURL
            // 
            resources.ApplyResources(this.txtProxyURL, "txtProxyURL");
            this.txtProxyURL.Name = "txtProxyURL";
            // 
            // txtProxyPWD
            // 
            resources.ApplyResources(this.txtProxyPWD, "txtProxyPWD");
            this.txtProxyPWD.Name = "txtProxyPWD";
            // 
            // lblProxyURL
            // 
            resources.ApplyResources(this.lblProxyURL, "lblProxyURL");
            this.lblProxyURL.Name = "lblProxyURL";
            // 
            // txtProxyUID
            // 
            resources.ApplyResources(this.txtProxyUID, "txtProxyUID");
            this.txtProxyUID.Name = "txtProxyUID";
            // 
            // lblProxyUID
            // 
            resources.ApplyResources(this.lblProxyUID, "lblProxyUID");
            this.lblProxyUID.Name = "lblProxyUID";
            // 
            // tabPageZipPackage
            // 
            resources.ApplyResources(this.tabPageZipPackage, "tabPageZipPackage");
            this.tabPageZipPackage.Controls.Add(this.lblFolder);
            this.tabPageZipPackage.Controls.Add(this.txtFile);
            this.tabPageZipPackage.Controls.Add(this.lblFile);
            this.tabPageZipPackage.Controls.Add(this.txtPass);
            this.tabPageZipPackage.Controls.Add(this.lblEnc);
            this.tabPageZipPackage.Controls.Add(this.btnSelectFolder);
            this.tabPageZipPackage.Controls.Add(this.lblExt);
            this.tabPageZipPackage.Controls.Add(this.btnSelectSaveFile);
            this.tabPageZipPackage.Controls.Add(this.cbxExt);
            this.tabPageZipPackage.Controls.Add(this.txtExt);
            this.tabPageZipPackage.Controls.Add(this.tabZipUnZip);
            this.tabPageZipPackage.Controls.Add(this.cmbEnc);
            this.tabPageZipPackage.Controls.Add(this.lblPass);
            this.tabPageZipPackage.Controls.Add(this.txtFolder);
            this.tabPageZipPackage.Name = "tabPageZipPackage";
            this.tabPageZipPackage.UseVisualStyleBackColor = true;
            // 
            // lblFolder
            // 
            resources.ApplyResources(this.lblFolder, "lblFolder");
            this.lblFolder.Name = "lblFolder";
            // 
            // txtFile
            // 
            resources.ApplyResources(this.txtFile, "txtFile");
            this.txtFile.Name = "txtFile";
            // 
            // lblFile
            // 
            resources.ApplyResources(this.lblFile, "lblFile");
            this.lblFile.Name = "lblFile";
            // 
            // txtPass
            // 
            resources.ApplyResources(this.txtPass, "txtPass");
            this.txtPass.Name = "txtPass";
            // 
            // lblEnc
            // 
            resources.ApplyResources(this.lblEnc, "lblEnc");
            this.lblEnc.Name = "lblEnc";
            // 
            // btnSelectFolder
            // 
            resources.ApplyResources(this.btnSelectFolder, "btnSelectFolder");
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // lblExt
            // 
            resources.ApplyResources(this.lblExt, "lblExt");
            this.lblExt.Name = "lblExt";
            // 
            // btnSelectSaveFile
            // 
            resources.ApplyResources(this.btnSelectSaveFile, "btnSelectSaveFile");
            this.btnSelectSaveFile.Name = "btnSelectSaveFile";
            this.btnSelectSaveFile.UseVisualStyleBackColor = true;
            this.btnSelectSaveFile.Click += new System.EventHandler(this.btnSelectSaveFile_Click);
            // 
            // cbxExt
            // 
            resources.ApplyResources(this.cbxExt, "cbxExt");
            this.cbxExt.Name = "cbxExt";
            this.cbxExt.UseVisualStyleBackColor = true;
            this.cbxExt.CheckedChanged += new System.EventHandler(this.cbxExt_CheckedChanged);
            // 
            // txtExt
            // 
            resources.ApplyResources(this.txtExt, "txtExt");
            this.txtExt.Name = "txtExt";
            // 
            // tabZipUnZip
            // 
            resources.ApplyResources(this.tabZipUnZip, "tabZipUnZip");
            this.tabZipUnZip.Controls.Add(this.tabPageZip);
            this.tabZipUnZip.Controls.Add(this.tabPageUnZip);
            this.tabZipUnZip.Name = "tabZipUnZip";
            this.tabZipUnZip.SelectedIndex = 0;
            // 
            // tabPageZip
            // 
            resources.ApplyResources(this.tabPageZip, "tabPageZip");
            this.tabPageZip.Controls.Add(this.lblRootDirCheck);
            this.tabPageZip.Controls.Add(this.cbxRootDir);
            this.tabPageZip.Controls.Add(this.cbxFormat);
            this.tabPageZip.Controls.Add(this.btnCompress);
            this.tabPageZip.Controls.Add(this.cmbFormat);
            this.tabPageZip.Controls.Add(this.lblFormat);
            this.tabPageZip.Controls.Add(this.cmbCmpLv);
            this.tabPageZip.Controls.Add(this.lblCyp);
            this.tabPageZip.Controls.Add(this.lblCmpLv);
            this.tabPageZip.Controls.Add(this.cmbCyp);
            this.tabPageZip.Name = "tabPageZip";
            this.tabPageZip.UseVisualStyleBackColor = true;
            // 
            // lblRootDirCheck
            // 
            resources.ApplyResources(this.lblRootDirCheck, "lblRootDirCheck");
            this.lblRootDirCheck.Name = "lblRootDirCheck";
            // 
            // cbxRootDir
            // 
            resources.ApplyResources(this.cbxRootDir, "cbxRootDir");
            this.cbxRootDir.Name = "cbxRootDir";
            this.cbxRootDir.UseVisualStyleBackColor = true;
            // 
            // cbxFormat
            // 
            resources.ApplyResources(this.cbxFormat, "cbxFormat");
            this.cbxFormat.Name = "cbxFormat";
            this.cbxFormat.UseVisualStyleBackColor = true;
            this.cbxFormat.CheckedChanged += new System.EventHandler(this.cmbFormat_CheckedChanged);
            // 
            // btnCompress
            // 
            resources.ApplyResources(this.btnCompress, "btnCompress");
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // cmbFormat
            // 
            resources.ApplyResources(this.cmbFormat, "cmbFormat");
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Name = "cmbFormat";
            // 
            // lblFormat
            // 
            resources.ApplyResources(this.lblFormat, "lblFormat");
            this.lblFormat.Name = "lblFormat";
            // 
            // cmbCmpLv
            // 
            resources.ApplyResources(this.cmbCmpLv, "cmbCmpLv");
            this.cmbCmpLv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCmpLv.FormattingEnabled = true;
            this.cmbCmpLv.Name = "cmbCmpLv";
            // 
            // lblCyp
            // 
            resources.ApplyResources(this.lblCyp, "lblCyp");
            this.lblCyp.Name = "lblCyp";
            // 
            // lblCmpLv
            // 
            resources.ApplyResources(this.lblCmpLv, "lblCmpLv");
            this.lblCmpLv.Name = "lblCmpLv";
            // 
            // cmbCyp
            // 
            resources.ApplyResources(this.cmbCyp, "cmbCyp");
            this.cmbCyp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCyp.FormattingEnabled = true;
            this.cmbCyp.Name = "cmbCyp";
            // 
            // tabPageUnZip
            // 
            resources.ApplyResources(this.tabPageUnZip, "tabPageUnZip");
            this.tabPageUnZip.Controls.Add(this.btnDecomp);
            this.tabPageUnZip.Controls.Add(this.lblEEFA);
            this.tabPageUnZip.Controls.Add(this.cmbEEFA);
            this.tabPageUnZip.Name = "tabPageUnZip";
            this.tabPageUnZip.UseVisualStyleBackColor = true;
            // 
            // btnDecomp
            // 
            resources.ApplyResources(this.btnDecomp, "btnDecomp");
            this.btnDecomp.Name = "btnDecomp";
            this.btnDecomp.UseVisualStyleBackColor = true;
            this.btnDecomp.Click += new System.EventHandler(this.btnDecomp_Click);
            // 
            // lblEEFA
            // 
            resources.ApplyResources(this.lblEEFA, "lblEEFA");
            this.lblEEFA.Name = "lblEEFA";
            // 
            // cmbEEFA
            // 
            resources.ApplyResources(this.cmbEEFA, "cmbEEFA");
            this.cmbEEFA.FormattingEnabled = true;
            this.cmbEEFA.Name = "cmbEEFA";
            // 
            // cmbEnc
            // 
            resources.ApplyResources(this.cmbEnc, "cmbEnc");
            this.cmbEnc.FormattingEnabled = true;
            this.cmbEnc.Items.AddRange(new object[] {
            resources.GetString("cmbEnc.Items"),
            resources.GetString("cmbEnc.Items1"),
            resources.GetString("cmbEnc.Items2")});
            this.cmbEnc.Name = "cmbEnc";
            // 
            // lblPass
            // 
            resources.ApplyResources(this.lblPass, "lblPass");
            this.lblPass.Name = "lblPass";
            // 
            // txtFolder
            // 
            resources.ApplyResources(this.txtFolder, "txtFolder");
            this.txtFolder.Name = "txtFolder";
            // 
            // tabPageManifesto
            // 
            resources.ApplyResources(this.tabPageManifesto, "tabPageManifesto");
            this.tabPageManifesto.Controls.Add(this.label3);
            this.tabPageManifesto.Controls.Add(this.label2);
            this.tabPageManifesto.Controls.Add(this.label1);
            this.tabPageManifesto.Controls.Add(this.lblInsDir);
            this.tabPageManifesto.Controls.Add(this.txtInsDir);
            this.tabPageManifesto.Controls.Add(this.lblExeName);
            this.tabPageManifesto.Controls.Add(this.txtExeName);
            this.tabPageManifesto.Controls.Add(this.btnCreateManifesto);
            this.tabPageManifesto.Controls.Add(this.btnRemoveZIPFile);
            this.tabPageManifesto.Controls.Add(this.btnAddZIPFile);
            this.tabPageManifesto.Controls.Add(this.lbxZIPFiles);
            this.tabPageManifesto.Name = "tabPageManifesto";
            this.tabPageManifesto.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblInsDir
            // 
            resources.ApplyResources(this.lblInsDir, "lblInsDir");
            this.lblInsDir.Name = "lblInsDir";
            // 
            // txtInsDir
            // 
            resources.ApplyResources(this.txtInsDir, "txtInsDir");
            this.txtInsDir.Name = "txtInsDir";
            // 
            // lblExeName
            // 
            resources.ApplyResources(this.lblExeName, "lblExeName");
            this.lblExeName.Name = "lblExeName";
            // 
            // txtExeName
            // 
            resources.ApplyResources(this.txtExeName, "txtExeName");
            this.txtExeName.Name = "txtExeName";
            // 
            // btnCreateManifesto
            // 
            resources.ApplyResources(this.btnCreateManifesto, "btnCreateManifesto");
            this.btnCreateManifesto.Name = "btnCreateManifesto";
            this.btnCreateManifesto.UseVisualStyleBackColor = true;
            this.btnCreateManifesto.Click += new System.EventHandler(this.btnCreateManifesto_Click);
            // 
            // btnRemoveZIPFile
            // 
            resources.ApplyResources(this.btnRemoveZIPFile, "btnRemoveZIPFile");
            this.btnRemoveZIPFile.Name = "btnRemoveZIPFile";
            this.btnRemoveZIPFile.UseVisualStyleBackColor = true;
            this.btnRemoveZIPFile.Click += new System.EventHandler(this.btnRemoveZIPFile_Click);
            // 
            // btnAddZIPFile
            // 
            resources.ApplyResources(this.btnAddZIPFile, "btnAddZIPFile");
            this.btnAddZIPFile.Name = "btnAddZIPFile";
            this.btnAddZIPFile.UseVisualStyleBackColor = true;
            this.btnAddZIPFile.Click += new System.EventHandler(this.btnAddZIPFile_Click);
            // 
            // lbxZIPFiles
            // 
            resources.ApplyResources(this.lbxZIPFiles, "lbxZIPFiles");
            this.lbxZIPFiles.FormattingEnabled = true;
            this.lbxZIPFiles.Name = "lbxZIPFiles";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.tabDeployZipPac.ResumeLayout(false);
            this.tabPageDeployWithHTTP.ResumeLayout(false);
            this.tabPageDeployWithHTTP.PerformLayout();
            this.gbxWWW.ResumeLayout(false);
            this.gbxWWW.PerformLayout();
            this.gbxProxy.ResumeLayout(false);
            this.gbxProxy.PerformLayout();
            this.tabPageZipPackage.ResumeLayout(false);
            this.tabPageZipPackage.PerformLayout();
            this.tabZipUnZip.ResumeLayout(false);
            this.tabPageZip.ResumeLayout(false);
            this.tabPageZip.PerformLayout();
            this.tabPageUnZip.ResumeLayout(false);
            this.tabPageUnZip.PerformLayout();
            this.tabPageManifesto.ResumeLayout(false);
            this.tabPageManifesto.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.OpenFileDialog openFilesDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabDeployZipPac;
        private System.Windows.Forms.TabPage tabPageDeployWithHTTP;
        private System.Windows.Forms.Button btnDelHistory;
        private System.Windows.Forms.Label lblHistorys;
        private System.Windows.Forms.ComboBox cmbHistorys;
        private System.Windows.Forms.Button btnCheckUpdateAndInstall;
        private System.Windows.Forms.GroupBox gbxWWW;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.TextBox txtPWD;
        private System.Windows.Forms.Label lblPWD;
        private System.Windows.Forms.Label lblUID;
        private System.Windows.Forms.TextBox txtUID;
        private System.Windows.Forms.GroupBox gbxProxy;
        private System.Windows.Forms.TextBox txtProxyDomain;
        private System.Windows.Forms.Label lblProxyDomain;
        private System.Windows.Forms.Label lblProxyPWD;
        private System.Windows.Forms.TextBox txtProxyURL;
        private System.Windows.Forms.TextBox txtProxyPWD;
        private System.Windows.Forms.Label lblProxyURL;
        private System.Windows.Forms.TextBox txtProxyUID;
        private System.Windows.Forms.Label lblProxyUID;
        private System.Windows.Forms.TabPage tabPageZipPackage;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblEnc;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Button btnSelectSaveFile;
        private System.Windows.Forms.CheckBox cbxExt;
        private System.Windows.Forms.TextBox txtExt;
        private System.Windows.Forms.TabControl tabZipUnZip;
        private System.Windows.Forms.TabPage tabPageZip;
        private System.Windows.Forms.CheckBox cbxRootDir;
        private System.Windows.Forms.CheckBox cbxFormat;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbCmpLv;
        private System.Windows.Forms.Label lblCyp;
        private System.Windows.Forms.Label lblCmpLv;
        private System.Windows.Forms.ComboBox cmbCyp;
        private System.Windows.Forms.TabPage tabPageUnZip;
        private System.Windows.Forms.Button btnDecomp;
        private System.Windows.Forms.Label lblEEFA;
        private System.Windows.Forms.ComboBox cmbEEFA;
        private System.Windows.Forms.ComboBox cmbEnc;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.TabPage tabPageManifesto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInsDir;
        private System.Windows.Forms.TextBox txtInsDir;
        private System.Windows.Forms.Label lblExeName;
        private System.Windows.Forms.TextBox txtExeName;
        private System.Windows.Forms.Button btnCreateManifesto;
        private System.Windows.Forms.Button btnRemoveZIPFile;
        private System.Windows.Forms.Button btnAddZIPFile;
        private System.Windows.Forms.ListBox lbxZIPFiles;
        private System.Windows.Forms.Label lblRootDirCheck;
        private System.Windows.Forms.Label label3;
    }
}

