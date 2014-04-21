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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(5, 373);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(450, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "ステータス";
            // 
            // openFilesDialog1
            // 
            this.openFilesDialog1.FileName = "openFilesDialog1";
            this.openFilesDialog1.Filter = "ZIPファイル|*.zip";
            this.openFilesDialog1.Multiselect = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabDeployZipPac);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(462, 362);
            this.panel1.TabIndex = 23;
            // 
            // tabDeployZipPac
            // 
            this.tabDeployZipPac.Controls.Add(this.tabPageDeployWithHTTP);
            this.tabDeployZipPac.Controls.Add(this.tabPageZipPackage);
            this.tabDeployZipPac.Controls.Add(this.tabPageManifesto);
            this.tabDeployZipPac.Location = new System.Drawing.Point(6, 6);
            this.tabDeployZipPac.Name = "tabDeployZipPac";
            this.tabDeployZipPac.SelectedIndex = 0;
            this.tabDeployZipPac.Size = new System.Drawing.Size(450, 350);
            this.tabDeployZipPac.TabIndex = 23;
            // 
            // tabPageDeployWithHTTP
            // 
            this.tabPageDeployWithHTTP.Controls.Add(this.btnDelHistory);
            this.tabPageDeployWithHTTP.Controls.Add(this.lblHistorys);
            this.tabPageDeployWithHTTP.Controls.Add(this.cmbHistorys);
            this.tabPageDeployWithHTTP.Controls.Add(this.btnCheckUpdateAndInstall);
            this.tabPageDeployWithHTTP.Controls.Add(this.gbxWWW);
            this.tabPageDeployWithHTTP.Controls.Add(this.gbxProxy);
            this.tabPageDeployWithHTTP.Location = new System.Drawing.Point(4, 21);
            this.tabPageDeployWithHTTP.Name = "tabPageDeployWithHTTP";
            this.tabPageDeployWithHTTP.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDeployWithHTTP.Size = new System.Drawing.Size(442, 325);
            this.tabPageDeployWithHTTP.TabIndex = 0;
            this.tabPageDeployWithHTTP.Text = "HTTPでデプロイ";
            this.tabPageDeployWithHTTP.UseVisualStyleBackColor = true;
            // 
            // btnDelHistory
            // 
            this.btnDelHistory.Location = new System.Drawing.Point(344, 7);
            this.btnDelHistory.Name = "btnDelHistory";
            this.btnDelHistory.Size = new System.Drawing.Size(86, 23);
            this.btnDelHistory.TabIndex = 19;
            this.btnDelHistory.Text = "削除";
            this.btnDelHistory.UseVisualStyleBackColor = true;
            this.btnDelHistory.Click += new System.EventHandler(this.btnDelHistory_Click);
            // 
            // lblHistorys
            // 
            this.lblHistorys.AutoSize = true;
            this.lblHistorys.Location = new System.Drawing.Point(24, 12);
            this.lblHistorys.Name = "lblHistorys";
            this.lblHistorys.Size = new System.Drawing.Size(57, 12);
            this.lblHistorys.TabIndex = 9;
            this.lblHistorys.Text = "設定ロード";
            // 
            // cmbHistorys
            // 
            this.cmbHistorys.FormattingEnabled = true;
            this.cmbHistorys.Location = new System.Drawing.Point(93, 9);
            this.cmbHistorys.Name = "cmbHistorys";
            this.cmbHistorys.Size = new System.Drawing.Size(245, 20);
            this.cmbHistorys.TabIndex = 18;
            this.cmbHistorys.SelectedIndexChanged += new System.EventHandler(this.cmbHistorys_SelectedIndexChanged);
            // 
            // btnCheckUpdateAndInstall
            // 
            this.btnCheckUpdateAndInstall.Location = new System.Drawing.Point(6, 296);
            this.btnCheckUpdateAndInstall.Name = "btnCheckUpdateAndInstall";
            this.btnCheckUpdateAndInstall.Size = new System.Drawing.Size(424, 23);
            this.btnCheckUpdateAndInstall.TabIndex = 14;
            this.btnCheckUpdateAndInstall.Text = "更新をチェックしてインストール";
            this.btnCheckUpdateAndInstall.UseVisualStyleBackColor = true;
            this.btnCheckUpdateAndInstall.Click += new System.EventHandler(this.btnCheckUpdateAndInstall_Click);
            // 
            // gbxWWW
            // 
            this.gbxWWW.Controls.Add(this.txtURL);
            this.gbxWWW.Controls.Add(this.txtDomain);
            this.gbxWWW.Controls.Add(this.lblDomain);
            this.gbxWWW.Controls.Add(this.lblURL);
            this.gbxWWW.Controls.Add(this.txtPWD);
            this.gbxWWW.Controls.Add(this.lblPWD);
            this.gbxWWW.Controls.Add(this.lblUID);
            this.gbxWWW.Controls.Add(this.txtUID);
            this.gbxWWW.Location = new System.Drawing.Point(6, 32);
            this.gbxWWW.Name = "gbxWWW";
            this.gbxWWW.Size = new System.Drawing.Size(424, 123);
            this.gbxWWW.TabIndex = 7;
            this.gbxWWW.TabStop = false;
            this.gbxWWW.Text = "WWWサーバ";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(87, 16);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(331, 19);
            this.txtURL.TabIndex = 8;
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(87, 91);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.PasswordChar = '*';
            this.txtDomain.Size = new System.Drawing.Size(331, 19);
            this.txtDomain.TabIndex = 6;
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(19, 94);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(40, 12);
            this.lblDomain.TabIndex = 7;
            this.lblDomain.Text = "ドメイン";
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(19, 19);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(27, 12);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "URL";
            // 
            // txtPWD
            // 
            this.txtPWD.Location = new System.Drawing.Point(87, 66);
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.PasswordChar = '*';
            this.txtPWD.Size = new System.Drawing.Size(331, 19);
            this.txtPWD.TabIndex = 4;
            // 
            // lblPWD
            // 
            this.lblPWD.AutoSize = true;
            this.lblPWD.Location = new System.Drawing.Point(19, 69);
            this.lblPWD.Name = "lblPWD";
            this.lblPWD.Size = new System.Drawing.Size(52, 12);
            this.lblPWD.TabIndex = 5;
            this.lblPWD.Text = "パスワード";
            // 
            // lblUID
            // 
            this.lblUID.AutoSize = true;
            this.lblUID.Location = new System.Drawing.Point(19, 44);
            this.lblUID.Name = "lblUID";
            this.lblUID.Size = new System.Drawing.Size(46, 12);
            this.lblUID.TabIndex = 2;
            this.lblUID.Text = "ユーザID";
            // 
            // txtUID
            // 
            this.txtUID.Location = new System.Drawing.Point(87, 41);
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new System.Drawing.Size(331, 19);
            this.txtUID.TabIndex = 3;
            // 
            // gbxProxy
            // 
            this.gbxProxy.Controls.Add(this.txtProxyDomain);
            this.gbxProxy.Controls.Add(this.lblProxyDomain);
            this.gbxProxy.Controls.Add(this.lblProxyPWD);
            this.gbxProxy.Controls.Add(this.txtProxyURL);
            this.gbxProxy.Controls.Add(this.txtProxyPWD);
            this.gbxProxy.Controls.Add(this.lblProxyURL);
            this.gbxProxy.Controls.Add(this.txtProxyUID);
            this.gbxProxy.Controls.Add(this.lblProxyUID);
            this.gbxProxy.Location = new System.Drawing.Point(6, 161);
            this.gbxProxy.Name = "gbxProxy";
            this.gbxProxy.Size = new System.Drawing.Size(424, 123);
            this.gbxProxy.TabIndex = 6;
            this.gbxProxy.TabStop = false;
            this.gbxProxy.Text = "Proxyサーバ";
            // 
            // txtProxyDomain
            // 
            this.txtProxyDomain.Location = new System.Drawing.Point(87, 93);
            this.txtProxyDomain.Name = "txtProxyDomain";
            this.txtProxyDomain.PasswordChar = '*';
            this.txtProxyDomain.Size = new System.Drawing.Size(331, 19);
            this.txtProxyDomain.TabIndex = 8;
            // 
            // lblProxyDomain
            // 
            this.lblProxyDomain.AutoSize = true;
            this.lblProxyDomain.Location = new System.Drawing.Point(19, 96);
            this.lblProxyDomain.Name = "lblProxyDomain";
            this.lblProxyDomain.Size = new System.Drawing.Size(40, 12);
            this.lblProxyDomain.TabIndex = 9;
            this.lblProxyDomain.Text = "ドメイン";
            // 
            // lblProxyPWD
            // 
            this.lblProxyPWD.AutoSize = true;
            this.lblProxyPWD.Location = new System.Drawing.Point(18, 71);
            this.lblProxyPWD.Name = "lblProxyPWD";
            this.lblProxyPWD.Size = new System.Drawing.Size(52, 12);
            this.lblProxyPWD.TabIndex = 12;
            this.lblProxyPWD.Text = "パスワード";
            // 
            // txtProxyURL
            // 
            this.txtProxyURL.Location = new System.Drawing.Point(87, 18);
            this.txtProxyURL.Name = "txtProxyURL";
            this.txtProxyURL.Size = new System.Drawing.Size(331, 19);
            this.txtProxyURL.TabIndex = 8;
            // 
            // txtProxyPWD
            // 
            this.txtProxyPWD.Location = new System.Drawing.Point(87, 68);
            this.txtProxyPWD.Name = "txtProxyPWD";
            this.txtProxyPWD.PasswordChar = '*';
            this.txtProxyPWD.Size = new System.Drawing.Size(331, 19);
            this.txtProxyPWD.TabIndex = 11;
            // 
            // lblProxyURL
            // 
            this.lblProxyURL.AutoSize = true;
            this.lblProxyURL.Location = new System.Drawing.Point(19, 21);
            this.lblProxyURL.Name = "lblProxyURL";
            this.lblProxyURL.Size = new System.Drawing.Size(27, 12);
            this.lblProxyURL.TabIndex = 7;
            this.lblProxyURL.Text = "URL";
            // 
            // txtProxyUID
            // 
            this.txtProxyUID.Location = new System.Drawing.Point(87, 43);
            this.txtProxyUID.Name = "txtProxyUID";
            this.txtProxyUID.Size = new System.Drawing.Size(331, 19);
            this.txtProxyUID.TabIndex = 10;
            // 
            // lblProxyUID
            // 
            this.lblProxyUID.AutoSize = true;
            this.lblProxyUID.Location = new System.Drawing.Point(18, 46);
            this.lblProxyUID.Name = "lblProxyUID";
            this.lblProxyUID.Size = new System.Drawing.Size(46, 12);
            this.lblProxyUID.TabIndex = 9;
            this.lblProxyUID.Text = "ユーザID";
            // 
            // tabPageZipPackage
            // 
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
            this.tabPageZipPackage.Location = new System.Drawing.Point(4, 21);
            this.tabPageZipPackage.Name = "tabPageZipPackage";
            this.tabPageZipPackage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageZipPackage.Size = new System.Drawing.Size(442, 325);
            this.tabPageZipPackage.TabIndex = 1;
            this.tabPageZipPackage.Text = "Zipパッケージを作成";
            this.tabPageZipPackage.UseVisualStyleBackColor = true;
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(19, 12);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(40, 12);
            this.lblFolder.TabIndex = 3;
            this.lblFolder.Text = "フォルダ";
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(94, 34);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(232, 19);
            this.txtFile.TabIndex = 2;
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(19, 37);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(51, 12);
            this.lblFile.TabIndex = 4;
            this.lblFile.Text = "ファイル名";
            // 
            // txtPass
            // 
            this.txtPass.Enabled = false;
            this.txtPass.Location = new System.Drawing.Point(94, 110);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(327, 19);
            this.txtPass.TabIndex = 15;
            // 
            // lblEnc
            // 
            this.lblEnc.AutoSize = true;
            this.lblEnc.Location = new System.Drawing.Point(19, 87);
            this.lblEnc.Name = "lblEnc";
            this.lblEnc.Size = new System.Drawing.Size(42, 12);
            this.lblEnc.TabIndex = 6;
            this.lblEnc.Text = "Encode";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(332, 7);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(89, 23);
            this.btnSelectFolder.TabIndex = 1;
            this.btnSelectFolder.Text = "選択";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Location = new System.Drawing.Point(19, 62);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(65, 12);
            this.lblExt.TabIndex = 18;
            this.lblExt.Text = "除外拡張子";
            // 
            // btnSelectSaveFile
            // 
            this.btnSelectSaveFile.Location = new System.Drawing.Point(332, 32);
            this.btnSelectSaveFile.Name = "btnSelectSaveFile";
            this.btnSelectSaveFile.Size = new System.Drawing.Size(89, 23);
            this.btnSelectSaveFile.TabIndex = 7;
            this.btnSelectSaveFile.Text = "選択";
            this.btnSelectSaveFile.UseVisualStyleBackColor = true;
            this.btnSelectSaveFile.Click += new System.EventHandler(this.btnSelectSaveFile_Click);
            // 
            // cbxExt
            // 
            this.cbxExt.AutoSize = true;
            this.cbxExt.Location = new System.Drawing.Point(406, 62);
            this.cbxExt.Name = "cbxExt";
            this.cbxExt.Size = new System.Drawing.Size(15, 14);
            this.cbxExt.TabIndex = 20;
            this.cbxExt.UseVisualStyleBackColor = true;
            this.cbxExt.CheckedChanged += new System.EventHandler(this.cbxExt_CheckedChanged);
            // 
            // txtExt
            // 
            this.txtExt.Location = new System.Drawing.Point(94, 59);
            this.txtExt.Name = "txtExt";
            this.txtExt.Size = new System.Drawing.Size(306, 19);
            this.txtExt.TabIndex = 19;
            // 
            // tabZipUnZip
            // 
            this.tabZipUnZip.Controls.Add(this.tabPageZip);
            this.tabZipUnZip.Controls.Add(this.tabPageUnZip);
            this.tabZipUnZip.Location = new System.Drawing.Point(6, 137);
            this.tabZipUnZip.Name = "tabZipUnZip";
            this.tabZipUnZip.SelectedIndex = 0;
            this.tabZipUnZip.Size = new System.Drawing.Size(430, 178);
            this.tabZipUnZip.TabIndex = 21;
            // 
            // tabPageZip
            // 
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
            this.tabPageZip.Location = new System.Drawing.Point(4, 21);
            this.tabPageZip.Name = "tabPageZip";
            this.tabPageZip.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageZip.Size = new System.Drawing.Size(422, 153);
            this.tabPageZip.TabIndex = 0;
            this.tabPageZip.Text = "圧縮";
            this.tabPageZip.UseVisualStyleBackColor = true;
            // 
            // lblRootDirCheck
            // 
            this.lblRootDirCheck.AutoSize = true;
            this.lblRootDirCheck.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRootDirCheck.Location = new System.Drawing.Point(25, 105);
            this.lblRootDirCheck.Name = "lblRootDirCheck";
            this.lblRootDirCheck.Size = new System.Drawing.Size(404, 11);
            this.lblRootDirCheck.TabIndex = 19;
            this.lblRootDirCheck.Text = "チェック有り：指定ZIPファイル名をルートZIP内フォルダに含めない（ルート フォルダからの圧縮）。";
            // 
            // cbxRootDir
            // 
            this.cbxRootDir.AutoSize = true;
            this.cbxRootDir.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cbxRootDir.Location = new System.Drawing.Point(8, 84);
            this.cbxRootDir.Name = "cbxRootDir";
            this.cbxRootDir.Size = new System.Drawing.Size(390, 15);
            this.cbxRootDir.TabIndex = 18;
            this.cbxRootDir.Text = "チェック無し：指定ZIPファイル名をルートZIP内フォルダに含める（個別のフォルダ圧縮）。";
            this.cbxRootDir.UseVisualStyleBackColor = true;
            // 
            // cbxFormat
            // 
            this.cbxFormat.AutoSize = true;
            this.cbxFormat.Enabled = false;
            this.cbxFormat.Location = new System.Drawing.Point(396, 61);
            this.cbxFormat.Name = "cbxFormat";
            this.cbxFormat.Size = new System.Drawing.Size(15, 14);
            this.cbxFormat.TabIndex = 17;
            this.cbxFormat.UseVisualStyleBackColor = true;
            this.cbxFormat.CheckedChanged += new System.EventHandler(this.cmbFormat_CheckedChanged);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(9, 123);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(402, 23);
            this.btnCompress.TabIndex = 5;
            this.btnCompress.Text = "圧縮";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // cmbFormat
            // 
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Location = new System.Drawing.Point(53, 58);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(337, 20);
            this.cmbFormat.TabIndex = 14;
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(7, 61);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(29, 12);
            this.lblFormat.TabIndex = 13;
            this.lblFormat.Text = "形式";
            // 
            // cmbCmpLv
            // 
            this.cmbCmpLv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCmpLv.FormattingEnabled = true;
            this.cmbCmpLv.Location = new System.Drawing.Point(53, 32);
            this.cmbCmpLv.Name = "cmbCmpLv";
            this.cmbCmpLv.Size = new System.Drawing.Size(358, 20);
            this.cmbCmpLv.TabIndex = 12;
            // 
            // lblCyp
            // 
            this.lblCyp.AutoSize = true;
            this.lblCyp.Location = new System.Drawing.Point(6, 9);
            this.lblCyp.Name = "lblCyp";
            this.lblCyp.Size = new System.Drawing.Size(41, 12);
            this.lblCyp.TabIndex = 9;
            this.lblCyp.Text = "暗号化";
            // 
            // lblCmpLv
            // 
            this.lblCmpLv.AutoSize = true;
            this.lblCmpLv.Location = new System.Drawing.Point(6, 35);
            this.lblCmpLv.Name = "lblCmpLv";
            this.lblCmpLv.Size = new System.Drawing.Size(41, 12);
            this.lblCmpLv.TabIndex = 11;
            this.lblCmpLv.Text = "圧縮Lv";
            // 
            // cmbCyp
            // 
            this.cmbCyp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCyp.Enabled = false;
            this.cmbCyp.FormattingEnabled = true;
            this.cmbCyp.Location = new System.Drawing.Point(53, 6);
            this.cmbCyp.Name = "cmbCyp";
            this.cmbCyp.Size = new System.Drawing.Size(358, 20);
            this.cmbCyp.TabIndex = 10;
            // 
            // tabPageUnZip
            // 
            this.tabPageUnZip.Controls.Add(this.btnDecomp);
            this.tabPageUnZip.Controls.Add(this.lblEEFA);
            this.tabPageUnZip.Controls.Add(this.cmbEEFA);
            this.tabPageUnZip.Location = new System.Drawing.Point(4, 21);
            this.tabPageUnZip.Name = "tabPageUnZip";
            this.tabPageUnZip.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUnZip.Size = new System.Drawing.Size(422, 153);
            this.tabPageUnZip.TabIndex = 1;
            this.tabPageUnZip.Text = "解凍（テスト）";
            this.tabPageUnZip.UseVisualStyleBackColor = true;
            // 
            // btnDecomp
            // 
            this.btnDecomp.Location = new System.Drawing.Point(9, 123);
            this.btnDecomp.Name = "btnDecomp";
            this.btnDecomp.Size = new System.Drawing.Size(380, 23);
            this.btnDecomp.TabIndex = 6;
            this.btnDecomp.Text = "解凍";
            this.btnDecomp.UseVisualStyleBackColor = true;
            this.btnDecomp.Click += new System.EventHandler(this.btnDecomp_Click);
            // 
            // lblEEFA
            // 
            this.lblEEFA.AutoSize = true;
            this.lblEEFA.Location = new System.Drawing.Point(6, 15);
            this.lblEEFA.Name = "lblEEFA";
            this.lblEEFA.Size = new System.Drawing.Size(62, 12);
            this.lblEEFA.TabIndex = 5;
            this.lblEEFA.Text = "上書き動作";
            // 
            // cmbEEFA
            // 
            this.cmbEEFA.FormattingEnabled = true;
            this.cmbEEFA.Location = new System.Drawing.Point(74, 12);
            this.cmbEEFA.Name = "cmbEEFA";
            this.cmbEEFA.Size = new System.Drawing.Size(312, 20);
            this.cmbEEFA.TabIndex = 0;
            // 
            // cmbEnc
            // 
            this.cmbEnc.Enabled = false;
            this.cmbEnc.FormattingEnabled = true;
            this.cmbEnc.Items.AddRange(new object[] {
            "utf-8",
            "shift_jis",
            "euc-jp"});
            this.cmbEnc.Location = new System.Drawing.Point(94, 84);
            this.cmbEnc.Name = "cmbEnc";
            this.cmbEnc.Size = new System.Drawing.Size(327, 20);
            this.cmbEnc.TabIndex = 8;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(19, 113);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(36, 12);
            this.lblPass.TabIndex = 16;
            this.lblPass.Text = "解パス";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(94, 9);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(232, 19);
            this.txtFolder.TabIndex = 0;
            // 
            // tabPageManifesto
            // 
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
            this.tabPageManifesto.Location = new System.Drawing.Point(4, 21);
            this.tabPageManifesto.Name = "tabPageManifesto";
            this.tabPageManifesto.Size = new System.Drawing.Size(442, 325);
            this.tabPageManifesto.TabIndex = 2;
            this.tabPageManifesto.Text = "配置マニュフェストを作成";
            this.tabPageManifesto.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(372, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "EXE名（パス）はカンマ区切りで指定可能で、先頭のEXEが起動の対象となる。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "起動するEXEのパスは、[インストール ディレクトリ] + [EXE名（パス）]で構成される。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(412, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "インストール ディレクトリには環境変数を指定可能：%USERPROFILE%\\XXX\\YYY\\ZZZ";
            // 
            // lblInsDir
            // 
            this.lblInsDir.AutoSize = true;
            this.lblInsDir.Location = new System.Drawing.Point(19, 191);
            this.lblInsDir.Name = "lblInsDir";
            this.lblInsDir.Size = new System.Drawing.Size(113, 12);
            this.lblInsDir.TabIndex = 8;
            this.lblInsDir.Text = "インストール ディレクトリ";
            // 
            // txtInsDir
            // 
            this.txtInsDir.Location = new System.Drawing.Point(138, 188);
            this.txtInsDir.Name = "txtInsDir";
            this.txtInsDir.Size = new System.Drawing.Size(292, 19);
            this.txtInsDir.TabIndex = 7;
            // 
            // lblExeName
            // 
            this.lblExeName.AutoSize = true;
            this.lblExeName.Location = new System.Drawing.Point(63, 212);
            this.lblExeName.Name = "lblExeName";
            this.lblExeName.Size = new System.Drawing.Size(69, 12);
            this.lblExeName.TabIndex = 6;
            this.lblExeName.Text = "EXE名（パス）";
            // 
            // txtExeName
            // 
            this.txtExeName.Location = new System.Drawing.Point(138, 209);
            this.txtExeName.Name = "txtExeName";
            this.txtExeName.Size = new System.Drawing.Size(292, 19);
            this.txtExeName.TabIndex = 5;
            // 
            // btnCreateManifesto
            // 
            this.btnCreateManifesto.Location = new System.Drawing.Point(15, 294);
            this.btnCreateManifesto.Name = "btnCreateManifesto";
            this.btnCreateManifesto.Size = new System.Drawing.Size(415, 23);
            this.btnCreateManifesto.TabIndex = 4;
            this.btnCreateManifesto.Text = "マニュフェストファイルを作成する";
            this.btnCreateManifesto.UseVisualStyleBackColor = true;
            this.btnCreateManifesto.Click += new System.EventHandler(this.btnCreateManifesto_Click);
            // 
            // btnRemoveZIPFile
            // 
            this.btnRemoveZIPFile.Location = new System.Drawing.Point(15, 158);
            this.btnRemoveZIPFile.Name = "btnRemoveZIPFile";
            this.btnRemoveZIPFile.Size = new System.Drawing.Size(415, 23);
            this.btnRemoveZIPFile.TabIndex = 3;
            this.btnRemoveZIPFile.Text = "削除する";
            this.btnRemoveZIPFile.UseVisualStyleBackColor = true;
            this.btnRemoveZIPFile.Click += new System.EventHandler(this.btnRemoveZIPFile_Click);
            // 
            // btnAddZIPFile
            // 
            this.btnAddZIPFile.Location = new System.Drawing.Point(15, 131);
            this.btnAddZIPFile.Name = "btnAddZIPFile";
            this.btnAddZIPFile.Size = new System.Drawing.Size(415, 23);
            this.btnAddZIPFile.TabIndex = 2;
            this.btnAddZIPFile.Text = "追加する";
            this.btnAddZIPFile.UseVisualStyleBackColor = true;
            this.btnAddZIPFile.Click += new System.EventHandler(this.btnAddZIPFile_Click);
            // 
            // lbxZIPFiles
            // 
            this.lbxZIPFiles.FormattingEnabled = true;
            this.lbxZIPFiles.ItemHeight = 12;
            this.lbxZIPFiles.Location = new System.Drawing.Point(15, 15);
            this.lbxZIPFiles.Name = "lbxZIPFiles";
            this.lbxZIPFiles.ScrollAlwaysVisible = true;
            this.lbxZIPFiles.Size = new System.Drawing.Size(415, 112);
            this.lbxZIPFiles.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 394);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "DeployZipPackWithHTTP";
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

