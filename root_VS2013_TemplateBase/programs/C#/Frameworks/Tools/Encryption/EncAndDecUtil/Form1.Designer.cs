namespace EncAndDecUtil
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabHash = new System.Windows.Forms.TabPage();
            this.lblHS3 = new System.Windows.Forms.Label();
            this.txtHSCode = new System.Windows.Forms.TextBox();
            this.lblHS2 = new System.Windows.Forms.Label();
            this.txtHSString = new System.Windows.Forms.TextBox();
            this.gbxHS = new System.Windows.Forms.GroupBox();
            this.rbnHSBytes = new System.Windows.Forms.RadioButton();
            this.rbnHSString = new System.Windows.Forms.RadioButton();
            this.cbxHSPV = new System.Windows.Forms.ComboBox();
            this.lblHS1 = new System.Windows.Forms.Label();
            this.btnGetHash = new System.Windows.Forms.Button();
            this.tabKeyedHash = new System.Windows.Forms.TabPage();
            this.gbxKHS = new System.Windows.Forms.GroupBox();
            this.rbnKHSBytes = new System.Windows.Forms.RadioButton();
            this.rbnKHSString = new System.Windows.Forms.RadioButton();
            this.nudKHSStretching = new System.Windows.Forms.NumericUpDown();
            this.lblKHS6 = new System.Windows.Forms.Label();
            this.lblKHS5 = new System.Windows.Forms.Label();
            this.cbxKHSPV = new System.Windows.Forms.ComboBox();
            this.btnGetKeyedHash = new System.Windows.Forms.Button();
            this.txtKHSCode = new System.Windows.Forms.TextBox();
            this.lblKHS4 = new System.Windows.Forms.Label();
            this.txtKHSSalt = new System.Windows.Forms.TextBox();
            this.lblKHS3 = new System.Windows.Forms.Label();
            this.lblKHS2 = new System.Windows.Forms.Label();
            this.lblKHS1 = new System.Windows.Forms.Label();
            this.txtKHSPassword = new System.Windows.Forms.TextBox();
            this.txtKHSString = new System.Windows.Forms.TextBox();
            this.tabSC = new System.Windows.Forms.TabPage();
            this.gbxSC = new System.Windows.Forms.GroupBox();
            this.rbnSCBytes = new System.Windows.Forms.RadioButton();
            this.rbnSCString = new System.Windows.Forms.RadioButton();
            this.nudSCStretching = new System.Windows.Forms.NumericUpDown();
            this.lblSC6 = new System.Windows.Forms.Label();
            this.lblSC5 = new System.Windows.Forms.Label();
            this.cbxSCPV = new System.Windows.Forms.ComboBox();
            this.btnSCDecrypt = new System.Windows.Forms.Button();
            this.btnSCEncrypt = new System.Windows.Forms.Button();
            this.txtSCCode = new System.Windows.Forms.TextBox();
            this.lblSC4 = new System.Windows.Forms.Label();
            this.txtSCSalt = new System.Windows.Forms.TextBox();
            this.lblSC3 = new System.Windows.Forms.Label();
            this.lblSC2 = new System.Windows.Forms.Label();
            this.lblSC1 = new System.Windows.Forms.Label();
            this.txtSCPassword = new System.Windows.Forms.TextBox();
            this.txtSCString = new System.Windows.Forms.TextBox();
            this.tabASC = new System.Windows.Forms.TabPage();
            this.btnASCGetKey = new System.Windows.Forms.Button();
            this.lblASC4 = new System.Windows.Forms.Label();
            this.txtASCCode = new System.Windows.Forms.TextBox();
            this.txtASCPublic = new System.Windows.Forms.TextBox();
            this.txtASCPrivate = new System.Windows.Forms.TextBox();
            this.lblASC3 = new System.Windows.Forms.Label();
            this.gbxASC = new System.Windows.Forms.GroupBox();
            this.rbnASCBytes = new System.Windows.Forms.RadioButton();
            this.rbnASCString = new System.Windows.Forms.RadioButton();
            this.lblASC5 = new System.Windows.Forms.Label();
            this.btnASCDecrypt = new System.Windows.Forms.Button();
            this.btnASCEncrypt = new System.Windows.Forms.Button();
            this.lblASC2 = new System.Windows.Forms.Label();
            this.lblASC1 = new System.Windows.Forms.Label();
            this.txtASCString = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.nudSPWDSaltLength = new System.Windows.Forms.NumericUpDown();
            this.lblSPWD3 = new System.Windows.Forms.Label();
            this.btnSPWDAuth = new System.Windows.Forms.Button();
            this.lblSPWD4 = new System.Windows.Forms.Label();
            this.txtSPWDSaltedPassword = new System.Windows.Forms.TextBox();
            this.lblSPWD2 = new System.Windows.Forms.Label();
            this.txtSPWDRawPassword = new System.Windows.Forms.TextBox();
            this.cbxSPWDPV = new System.Windows.Forms.ComboBox();
            this.lblSPWD1 = new System.Windows.Forms.Label();
            this.btnSPWDGen = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabHash.SuspendLayout();
            this.gbxHS.SuspendLayout();
            this.tabKeyedHash.SuspendLayout();
            this.gbxKHS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKHSStretching)).BeginInit();
            this.tabSC.SuspendLayout();
            this.gbxSC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSCStretching)).BeginInit();
            this.tabASC.SuspendLayout();
            this.gbxASC.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSPWDSaltLength)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabHash);
            this.tabControl1.Controls.Add(this.tabKeyedHash);
            this.tabControl1.Controls.Add(this.tabSC);
            this.tabControl1.Controls.Add(this.tabASC);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(334, 268);
            this.tabControl1.TabIndex = 10;
            // 
            // tabHash
            // 
            this.tabHash.Controls.Add(this.lblHS3);
            this.tabHash.Controls.Add(this.txtHSCode);
            this.tabHash.Controls.Add(this.lblHS2);
            this.tabHash.Controls.Add(this.txtHSString);
            this.tabHash.Controls.Add(this.gbxHS);
            this.tabHash.Controls.Add(this.cbxHSPV);
            this.tabHash.Controls.Add(this.lblHS1);
            this.tabHash.Controls.Add(this.btnGetHash);
            this.tabHash.Location = new System.Drawing.Point(4, 21);
            this.tabHash.Name = "tabHash";
            this.tabHash.Size = new System.Drawing.Size(326, 243);
            this.tabHash.TabIndex = 2;
            this.tabHash.Text = "ハッシュ";
            this.tabHash.UseVisualStyleBackColor = true;
            // 
            // lblHS3
            // 
            this.lblHS3.AutoSize = true;
            this.lblHS3.Location = new System.Drawing.Point(9, 113);
            this.lblHS3.Name = "lblHS3";
            this.lblHS3.Size = new System.Drawing.Size(76, 12);
            this.lblHS3.TabIndex = 33;
            this.lblHS3.Text = "ハッシュ文字列";
            // 
            // txtHSCode
            // 
            this.txtHSCode.Location = new System.Drawing.Point(103, 110);
            this.txtHSCode.Name = "txtHSCode";
            this.txtHSCode.ReadOnly = true;
            this.txtHSCode.Size = new System.Drawing.Size(212, 19);
            this.txtHSCode.TabIndex = 32;
            // 
            // lblHS2
            // 
            this.lblHS2.AutoSize = true;
            this.lblHS2.Location = new System.Drawing.Point(9, 88);
            this.lblHS2.Name = "lblHS2";
            this.lblHS2.Size = new System.Drawing.Size(41, 12);
            this.lblHS2.TabIndex = 31;
            this.lblHS2.Text = "文字列";
            // 
            // txtHSString
            // 
            this.txtHSString.Location = new System.Drawing.Point(103, 85);
            this.txtHSString.Name = "txtHSString";
            this.txtHSString.Size = new System.Drawing.Size(212, 19);
            this.txtHSString.TabIndex = 30;
            // 
            // gbxHS
            // 
            this.gbxHS.Controls.Add(this.rbnHSBytes);
            this.gbxHS.Controls.Add(this.rbnHSString);
            this.gbxHS.Location = new System.Drawing.Point(9, 7);
            this.gbxHS.Name = "gbxHS";
            this.gbxHS.Size = new System.Drawing.Size(306, 46);
            this.gbxHS.TabIndex = 29;
            this.gbxHS.TabStop = false;
            this.gbxHS.Text = "API選択";
            // 
            // rbnHSBytes
            // 
            this.rbnHSBytes.AutoSize = true;
            this.rbnHSBytes.Location = new System.Drawing.Point(94, 18);
            this.rbnHSBytes.Name = "rbnHSBytes";
            this.rbnHSBytes.Size = new System.Drawing.Size(74, 16);
            this.rbnHSBytes.TabIndex = 1;
            this.rbnHSBytes.TabStop = true;
            this.rbnHSBytes.Text = "バイト配列";
            this.rbnHSBytes.UseVisualStyleBackColor = true;
            // 
            // rbnHSString
            // 
            this.rbnHSString.AutoSize = true;
            this.rbnHSString.Checked = true;
            this.rbnHSString.Location = new System.Drawing.Point(20, 18);
            this.rbnHSString.Name = "rbnHSString";
            this.rbnHSString.Size = new System.Drawing.Size(59, 16);
            this.rbnHSString.TabIndex = 0;
            this.rbnHSString.TabStop = true;
            this.rbnHSString.Text = "文字列";
            this.rbnHSString.UseVisualStyleBackColor = true;
            // 
            // cbxHSPV
            // 
            this.cbxHSPV.FormattingEnabled = true;
            this.cbxHSPV.Location = new System.Drawing.Point(103, 59);
            this.cbxHSPV.Name = "cbxHSPV";
            this.cbxHSPV.Size = new System.Drawing.Size(212, 20);
            this.cbxHSPV.TabIndex = 28;
            // 
            // lblHS1
            // 
            this.lblHS1.AutoSize = true;
            this.lblHS1.Location = new System.Drawing.Point(9, 63);
            this.lblHS1.Name = "lblHS1";
            this.lblHS1.Size = new System.Drawing.Size(75, 12);
            this.lblHS1.TabIndex = 27;
            this.lblHS1.Text = "暗号プロバイダ";
            // 
            // btnGetHash
            // 
            this.btnGetHash.Location = new System.Drawing.Point(102, 135);
            this.btnGetHash.Name = "btnGetHash";
            this.btnGetHash.Size = new System.Drawing.Size(213, 23);
            this.btnGetHash.TabIndex = 0;
            this.btnGetHash.Text = "ハッシュ";
            this.btnGetHash.UseVisualStyleBackColor = true;
            this.btnGetHash.Click += new System.EventHandler(this.btnGetHash_Click);
            // 
            // tabKeyedHash
            // 
            this.tabKeyedHash.Controls.Add(this.gbxKHS);
            this.tabKeyedHash.Controls.Add(this.nudKHSStretching);
            this.tabKeyedHash.Controls.Add(this.lblKHS6);
            this.tabKeyedHash.Controls.Add(this.lblKHS5);
            this.tabKeyedHash.Controls.Add(this.cbxKHSPV);
            this.tabKeyedHash.Controls.Add(this.btnGetKeyedHash);
            this.tabKeyedHash.Controls.Add(this.txtKHSCode);
            this.tabKeyedHash.Controls.Add(this.lblKHS4);
            this.tabKeyedHash.Controls.Add(this.txtKHSSalt);
            this.tabKeyedHash.Controls.Add(this.lblKHS3);
            this.tabKeyedHash.Controls.Add(this.lblKHS2);
            this.tabKeyedHash.Controls.Add(this.lblKHS1);
            this.tabKeyedHash.Controls.Add(this.txtKHSPassword);
            this.tabKeyedHash.Controls.Add(this.txtKHSString);
            this.tabKeyedHash.Location = new System.Drawing.Point(4, 21);
            this.tabKeyedHash.Name = "tabKeyedHash";
            this.tabKeyedHash.Size = new System.Drawing.Size(326, 243);
            this.tabKeyedHash.TabIndex = 3;
            this.tabKeyedHash.Text = "キー付きハッシュ";
            this.tabKeyedHash.UseVisualStyleBackColor = true;
            // 
            // gbxKHS
            // 
            this.gbxKHS.Controls.Add(this.rbnKHSBytes);
            this.gbxKHS.Controls.Add(this.rbnKHSString);
            this.gbxKHS.Location = new System.Drawing.Point(9, 7);
            this.gbxKHS.Name = "gbxKHS";
            this.gbxKHS.Size = new System.Drawing.Size(306, 46);
            this.gbxKHS.TabIndex = 41;
            this.gbxKHS.TabStop = false;
            this.gbxKHS.Text = "API選択";
            // 
            // rbnKHSBytes
            // 
            this.rbnKHSBytes.AutoSize = true;
            this.rbnKHSBytes.Location = new System.Drawing.Point(94, 18);
            this.rbnKHSBytes.Name = "rbnKHSBytes";
            this.rbnKHSBytes.Size = new System.Drawing.Size(74, 16);
            this.rbnKHSBytes.TabIndex = 1;
            this.rbnKHSBytes.TabStop = true;
            this.rbnKHSBytes.Text = "バイト配列";
            this.rbnKHSBytes.UseVisualStyleBackColor = true;
            // 
            // rbnKHSString
            // 
            this.rbnKHSString.AutoSize = true;
            this.rbnKHSString.Checked = true;
            this.rbnKHSString.Location = new System.Drawing.Point(20, 18);
            this.rbnKHSString.Name = "rbnKHSString";
            this.rbnKHSString.Size = new System.Drawing.Size(59, 16);
            this.rbnKHSString.TabIndex = 0;
            this.rbnKHSString.TabStop = true;
            this.rbnKHSString.Text = "文字列";
            this.rbnKHSString.UseVisualStyleBackColor = true;
            // 
            // nudKHSStretching
            // 
            this.nudKHSStretching.Location = new System.Drawing.Point(103, 160);
            this.nudKHSStretching.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudKHSStretching.Name = "nudKHSStretching";
            this.nudKHSStretching.Size = new System.Drawing.Size(212, 19);
            this.nudKHSStretching.TabIndex = 40;
            // 
            // lblKHS6
            // 
            this.lblKHS6.AutoSize = true;
            this.lblKHS6.Location = new System.Drawing.Point(9, 188);
            this.lblKHS6.Name = "lblKHS6";
            this.lblKHS6.Size = new System.Drawing.Size(76, 12);
            this.lblKHS6.TabIndex = 39;
            this.lblKHS6.Text = "ハッシュ文字列";
            // 
            // lblKHS5
            // 
            this.lblKHS5.AutoSize = true;
            this.lblKHS5.Location = new System.Drawing.Point(9, 162);
            this.lblKHS5.Name = "lblKHS5";
            this.lblKHS5.Size = new System.Drawing.Size(65, 12);
            this.lblKHS5.TabIndex = 38;
            this.lblKHS5.Text = "ストレッチング";
            // 
            // cbxKHSPV
            // 
            this.cbxKHSPV.FormattingEnabled = true;
            this.cbxKHSPV.Location = new System.Drawing.Point(103, 59);
            this.cbxKHSPV.Name = "cbxKHSPV";
            this.cbxKHSPV.Size = new System.Drawing.Size(212, 20);
            this.cbxKHSPV.TabIndex = 37;
            // 
            // btnGetKeyedHash
            // 
            this.btnGetKeyedHash.Location = new System.Drawing.Point(103, 210);
            this.btnGetKeyedHash.Name = "btnGetKeyedHash";
            this.btnGetKeyedHash.Size = new System.Drawing.Size(212, 23);
            this.btnGetKeyedHash.TabIndex = 35;
            this.btnGetKeyedHash.Text = "キー付きハッシュ";
            this.btnGetKeyedHash.UseVisualStyleBackColor = true;
            this.btnGetKeyedHash.Click += new System.EventHandler(this.btnGetKeyedHash_Click);
            // 
            // txtKHSCode
            // 
            this.txtKHSCode.Location = new System.Drawing.Point(103, 185);
            this.txtKHSCode.Name = "txtKHSCode";
            this.txtKHSCode.ReadOnly = true;
            this.txtKHSCode.Size = new System.Drawing.Size(212, 19);
            this.txtKHSCode.TabIndex = 34;
            // 
            // lblKHS4
            // 
            this.lblKHS4.AutoSize = true;
            this.lblKHS4.Location = new System.Drawing.Point(11, 138);
            this.lblKHS4.Name = "lblKHS4";
            this.lblKHS4.Size = new System.Drawing.Size(32, 12);
            this.lblKHS4.TabIndex = 33;
            this.lblKHS4.Text = "ソルト";
            // 
            // txtKHSSalt
            // 
            this.txtKHSSalt.Location = new System.Drawing.Point(103, 135);
            this.txtKHSSalt.Name = "txtKHSSalt";
            this.txtKHSSalt.Size = new System.Drawing.Size(212, 19);
            this.txtKHSSalt.TabIndex = 32;
            // 
            // lblKHS3
            // 
            this.lblKHS3.AutoSize = true;
            this.lblKHS3.Location = new System.Drawing.Point(9, 113);
            this.lblKHS3.Name = "lblKHS3";
            this.lblKHS3.Size = new System.Drawing.Size(52, 12);
            this.lblKHS3.TabIndex = 31;
            this.lblKHS3.Text = "パスワード";
            // 
            // lblKHS2
            // 
            this.lblKHS2.AutoSize = true;
            this.lblKHS2.Location = new System.Drawing.Point(9, 88);
            this.lblKHS2.Name = "lblKHS2";
            this.lblKHS2.Size = new System.Drawing.Size(41, 12);
            this.lblKHS2.TabIndex = 30;
            this.lblKHS2.Text = "文字列";
            // 
            // lblKHS1
            // 
            this.lblKHS1.AutoSize = true;
            this.lblKHS1.Location = new System.Drawing.Point(9, 63);
            this.lblKHS1.Name = "lblKHS1";
            this.lblKHS1.Size = new System.Drawing.Size(75, 12);
            this.lblKHS1.TabIndex = 29;
            this.lblKHS1.Text = "暗号プロバイダ";
            // 
            // txtKHSPassword
            // 
            this.txtKHSPassword.Location = new System.Drawing.Point(103, 110);
            this.txtKHSPassword.Name = "txtKHSPassword";
            this.txtKHSPassword.Size = new System.Drawing.Size(212, 19);
            this.txtKHSPassword.TabIndex = 28;
            // 
            // txtKHSString
            // 
            this.txtKHSString.Location = new System.Drawing.Point(103, 85);
            this.txtKHSString.Name = "txtKHSString";
            this.txtKHSString.Size = new System.Drawing.Size(212, 19);
            this.txtKHSString.TabIndex = 27;
            // 
            // tabSC
            // 
            this.tabSC.Controls.Add(this.gbxSC);
            this.tabSC.Controls.Add(this.nudSCStretching);
            this.tabSC.Controls.Add(this.lblSC6);
            this.tabSC.Controls.Add(this.lblSC5);
            this.tabSC.Controls.Add(this.cbxSCPV);
            this.tabSC.Controls.Add(this.btnSCDecrypt);
            this.tabSC.Controls.Add(this.btnSCEncrypt);
            this.tabSC.Controls.Add(this.txtSCCode);
            this.tabSC.Controls.Add(this.lblSC4);
            this.tabSC.Controls.Add(this.txtSCSalt);
            this.tabSC.Controls.Add(this.lblSC3);
            this.tabSC.Controls.Add(this.lblSC2);
            this.tabSC.Controls.Add(this.lblSC1);
            this.tabSC.Controls.Add(this.txtSCPassword);
            this.tabSC.Controls.Add(this.txtSCString);
            this.tabSC.Location = new System.Drawing.Point(4, 21);
            this.tabSC.Name = "tabSC";
            this.tabSC.Padding = new System.Windows.Forms.Padding(3);
            this.tabSC.Size = new System.Drawing.Size(326, 243);
            this.tabSC.TabIndex = 0;
            this.tabSC.Text = "秘密鍵";
            this.tabSC.UseVisualStyleBackColor = true;
            // 
            // gbxSC
            // 
            this.gbxSC.Controls.Add(this.rbnSCBytes);
            this.gbxSC.Controls.Add(this.rbnSCString);
            this.gbxSC.Location = new System.Drawing.Point(9, 7);
            this.gbxSC.Name = "gbxSC";
            this.gbxSC.Size = new System.Drawing.Size(306, 46);
            this.gbxSC.TabIndex = 26;
            this.gbxSC.TabStop = false;
            this.gbxSC.Text = "API選択";
            // 
            // rbnSCBytes
            // 
            this.rbnSCBytes.AutoSize = true;
            this.rbnSCBytes.Location = new System.Drawing.Point(94, 18);
            this.rbnSCBytes.Name = "rbnSCBytes";
            this.rbnSCBytes.Size = new System.Drawing.Size(74, 16);
            this.rbnSCBytes.TabIndex = 1;
            this.rbnSCBytes.TabStop = true;
            this.rbnSCBytes.Text = "バイト配列";
            this.rbnSCBytes.UseVisualStyleBackColor = true;
            // 
            // rbnSCString
            // 
            this.rbnSCString.AutoSize = true;
            this.rbnSCString.Checked = true;
            this.rbnSCString.Location = new System.Drawing.Point(20, 18);
            this.rbnSCString.Name = "rbnSCString";
            this.rbnSCString.Size = new System.Drawing.Size(59, 16);
            this.rbnSCString.TabIndex = 0;
            this.rbnSCString.TabStop = true;
            this.rbnSCString.Text = "文字列";
            this.rbnSCString.UseVisualStyleBackColor = true;
            // 
            // nudSCStretching
            // 
            this.nudSCStretching.Location = new System.Drawing.Point(103, 160);
            this.nudSCStretching.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudSCStretching.Name = "nudSCStretching";
            this.nudSCStretching.Size = new System.Drawing.Size(212, 19);
            this.nudSCStretching.TabIndex = 24;
            // 
            // lblSC6
            // 
            this.lblSC6.AutoSize = true;
            this.lblSC6.Location = new System.Drawing.Point(9, 188);
            this.lblSC6.Name = "lblSC6";
            this.lblSC6.Size = new System.Drawing.Size(65, 12);
            this.lblSC6.TabIndex = 23;
            this.lblSC6.Text = "暗号文字列";
            // 
            // lblSC5
            // 
            this.lblSC5.AutoSize = true;
            this.lblSC5.Location = new System.Drawing.Point(9, 162);
            this.lblSC5.Name = "lblSC5";
            this.lblSC5.Size = new System.Drawing.Size(65, 12);
            this.lblSC5.TabIndex = 21;
            this.lblSC5.Text = "ストレッチング";
            // 
            // cbxSCPV
            // 
            this.cbxSCPV.FormattingEnabled = true;
            this.cbxSCPV.Location = new System.Drawing.Point(103, 59);
            this.cbxSCPV.Name = "cbxSCPV";
            this.cbxSCPV.Size = new System.Drawing.Size(212, 20);
            this.cbxSCPV.TabIndex = 20;
            // 
            // btnSCDecrypt
            // 
            this.btnSCDecrypt.Location = new System.Drawing.Point(217, 210);
            this.btnSCDecrypt.Name = "btnSCDecrypt";
            this.btnSCDecrypt.Size = new System.Drawing.Size(98, 23);
            this.btnSCDecrypt.TabIndex = 19;
            this.btnSCDecrypt.Text = "復号化";
            this.btnSCDecrypt.UseVisualStyleBackColor = true;
            this.btnSCDecrypt.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnSCEncrypt
            // 
            this.btnSCEncrypt.Location = new System.Drawing.Point(103, 210);
            this.btnSCEncrypt.Name = "btnSCEncrypt";
            this.btnSCEncrypt.Size = new System.Drawing.Size(98, 23);
            this.btnSCEncrypt.TabIndex = 18;
            this.btnSCEncrypt.Text = "暗号化";
            this.btnSCEncrypt.UseVisualStyleBackColor = true;
            this.btnSCEncrypt.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSCCode
            // 
            this.txtSCCode.Location = new System.Drawing.Point(103, 185);
            this.txtSCCode.Name = "txtSCCode";
            this.txtSCCode.ReadOnly = true;
            this.txtSCCode.Size = new System.Drawing.Size(212, 19);
            this.txtSCCode.TabIndex = 17;
            // 
            // lblSC4
            // 
            this.lblSC4.AutoSize = true;
            this.lblSC4.Location = new System.Drawing.Point(11, 138);
            this.lblSC4.Name = "lblSC4";
            this.lblSC4.Size = new System.Drawing.Size(32, 12);
            this.lblSC4.TabIndex = 16;
            this.lblSC4.Text = "ソルト";
            // 
            // txtSCSalt
            // 
            this.txtSCSalt.Location = new System.Drawing.Point(103, 135);
            this.txtSCSalt.Name = "txtSCSalt";
            this.txtSCSalt.Size = new System.Drawing.Size(212, 19);
            this.txtSCSalt.TabIndex = 15;
            // 
            // lblSC3
            // 
            this.lblSC3.AutoSize = true;
            this.lblSC3.Location = new System.Drawing.Point(9, 113);
            this.lblSC3.Name = "lblSC3";
            this.lblSC3.Size = new System.Drawing.Size(52, 12);
            this.lblSC3.TabIndex = 14;
            this.lblSC3.Text = "パスワード";
            // 
            // lblSC2
            // 
            this.lblSC2.AutoSize = true;
            this.lblSC2.Location = new System.Drawing.Point(9, 88);
            this.lblSC2.Name = "lblSC2";
            this.lblSC2.Size = new System.Drawing.Size(41, 12);
            this.lblSC2.TabIndex = 13;
            this.lblSC2.Text = "文字列";
            // 
            // lblSC1
            // 
            this.lblSC1.AutoSize = true;
            this.lblSC1.Location = new System.Drawing.Point(9, 63);
            this.lblSC1.Name = "lblSC1";
            this.lblSC1.Size = new System.Drawing.Size(75, 12);
            this.lblSC1.TabIndex = 12;
            this.lblSC1.Text = "暗号プロバイダ";
            // 
            // txtSCPassword
            // 
            this.txtSCPassword.Location = new System.Drawing.Point(103, 110);
            this.txtSCPassword.Name = "txtSCPassword";
            this.txtSCPassword.Size = new System.Drawing.Size(212, 19);
            this.txtSCPassword.TabIndex = 11;
            // 
            // txtSCString
            // 
            this.txtSCString.Location = new System.Drawing.Point(103, 85);
            this.txtSCString.Name = "txtSCString";
            this.txtSCString.Size = new System.Drawing.Size(212, 19);
            this.txtSCString.TabIndex = 10;
            // 
            // tabASC
            // 
            this.tabASC.Controls.Add(this.btnASCGetKey);
            this.tabASC.Controls.Add(this.lblASC4);
            this.tabASC.Controls.Add(this.txtASCCode);
            this.tabASC.Controls.Add(this.txtASCPublic);
            this.tabASC.Controls.Add(this.txtASCPrivate);
            this.tabASC.Controls.Add(this.lblASC3);
            this.tabASC.Controls.Add(this.gbxASC);
            this.tabASC.Controls.Add(this.lblASC5);
            this.tabASC.Controls.Add(this.btnASCDecrypt);
            this.tabASC.Controls.Add(this.btnASCEncrypt);
            this.tabASC.Controls.Add(this.lblASC2);
            this.tabASC.Controls.Add(this.lblASC1);
            this.tabASC.Controls.Add(this.txtASCString);
            this.tabASC.Location = new System.Drawing.Point(4, 21);
            this.tabASC.Name = "tabASC";
            this.tabASC.Padding = new System.Windows.Forms.Padding(3);
            this.tabASC.Size = new System.Drawing.Size(326, 243);
            this.tabASC.TabIndex = 1;
            this.tabASC.Text = "共通鍵";
            this.tabASC.UseVisualStyleBackColor = true;
            // 
            // btnASCGetKey
            // 
            this.btnASCGetKey.Location = new System.Drawing.Point(103, 185);
            this.btnASCGetKey.Name = "btnASCGetKey";
            this.btnASCGetKey.Size = new System.Drawing.Size(212, 23);
            this.btnASCGetKey.TabIndex = 39;
            this.btnASCGetKey.Text = "鍵取得";
            this.btnASCGetKey.UseVisualStyleBackColor = true;
            this.btnASCGetKey.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblASC4
            // 
            this.lblASC4.AutoSize = true;
            this.lblASC4.Location = new System.Drawing.Point(9, 138);
            this.lblASC4.Name = "lblASC4";
            this.lblASC4.Size = new System.Drawing.Size(41, 12);
            this.lblASC4.TabIndex = 38;
            this.lblASC4.Text = "公開鍵";
            // 
            // txtASCCode
            // 
            this.txtASCCode.Location = new System.Drawing.Point(103, 160);
            this.txtASCCode.Name = "txtASCCode";
            this.txtASCCode.ReadOnly = true;
            this.txtASCCode.Size = new System.Drawing.Size(212, 19);
            this.txtASCCode.TabIndex = 37;
            // 
            // txtASCPublic
            // 
            this.txtASCPublic.Location = new System.Drawing.Point(103, 135);
            this.txtASCPublic.Name = "txtASCPublic";
            this.txtASCPublic.ReadOnly = true;
            this.txtASCPublic.Size = new System.Drawing.Size(212, 19);
            this.txtASCPublic.TabIndex = 36;
            // 
            // txtASCPrivate
            // 
            this.txtASCPrivate.Location = new System.Drawing.Point(103, 110);
            this.txtASCPrivate.Name = "txtASCPrivate";
            this.txtASCPrivate.ReadOnly = true;
            this.txtASCPrivate.Size = new System.Drawing.Size(212, 19);
            this.txtASCPrivate.TabIndex = 35;
            // 
            // lblASC3
            // 
            this.lblASC3.AutoSize = true;
            this.lblASC3.Location = new System.Drawing.Point(9, 113);
            this.lblASC3.Name = "lblASC3";
            this.lblASC3.Size = new System.Drawing.Size(41, 12);
            this.lblASC3.TabIndex = 34;
            this.lblASC3.Text = "秘密鍵";
            // 
            // gbxASC
            // 
            this.gbxASC.Controls.Add(this.rbnASCBytes);
            this.gbxASC.Controls.Add(this.rbnASCString);
            this.gbxASC.Location = new System.Drawing.Point(9, 7);
            this.gbxASC.Name = "gbxASC";
            this.gbxASC.Size = new System.Drawing.Size(306, 46);
            this.gbxASC.TabIndex = 33;
            this.gbxASC.TabStop = false;
            this.gbxASC.Text = "API選択";
            // 
            // rbnASCBytes
            // 
            this.rbnASCBytes.AutoSize = true;
            this.rbnASCBytes.Location = new System.Drawing.Point(94, 18);
            this.rbnASCBytes.Name = "rbnASCBytes";
            this.rbnASCBytes.Size = new System.Drawing.Size(74, 16);
            this.rbnASCBytes.TabIndex = 1;
            this.rbnASCBytes.TabStop = true;
            this.rbnASCBytes.Text = "バイト配列";
            this.rbnASCBytes.UseVisualStyleBackColor = true;
            // 
            // rbnASCString
            // 
            this.rbnASCString.AutoSize = true;
            this.rbnASCString.Checked = true;
            this.rbnASCString.Location = new System.Drawing.Point(20, 18);
            this.rbnASCString.Name = "rbnASCString";
            this.rbnASCString.Size = new System.Drawing.Size(59, 16);
            this.rbnASCString.TabIndex = 0;
            this.rbnASCString.TabStop = true;
            this.rbnASCString.Text = "文字列";
            this.rbnASCString.UseVisualStyleBackColor = true;
            // 
            // lblASC5
            // 
            this.lblASC5.AutoSize = true;
            this.lblASC5.Location = new System.Drawing.Point(9, 163);
            this.lblASC5.Name = "lblASC5";
            this.lblASC5.Size = new System.Drawing.Size(65, 12);
            this.lblASC5.TabIndex = 32;
            this.lblASC5.Text = "暗号文字列";
            // 
            // btnASCDecrypt
            // 
            this.btnASCDecrypt.Location = new System.Drawing.Point(217, 214);
            this.btnASCDecrypt.Name = "btnASCDecrypt";
            this.btnASCDecrypt.Size = new System.Drawing.Size(98, 23);
            this.btnASCDecrypt.TabIndex = 31;
            this.btnASCDecrypt.Text = "復号化";
            this.btnASCDecrypt.UseVisualStyleBackColor = true;
            this.btnASCDecrypt.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnASCEncrypt
            // 
            this.btnASCEncrypt.Location = new System.Drawing.Point(103, 214);
            this.btnASCEncrypt.Name = "btnASCEncrypt";
            this.btnASCEncrypt.Size = new System.Drawing.Size(98, 23);
            this.btnASCEncrypt.TabIndex = 30;
            this.btnASCEncrypt.Text = "暗号化";
            this.btnASCEncrypt.UseVisualStyleBackColor = true;
            this.btnASCEncrypt.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblASC2
            // 
            this.lblASC2.AutoSize = true;
            this.lblASC2.Location = new System.Drawing.Point(9, 88);
            this.lblASC2.Name = "lblASC2";
            this.lblASC2.Size = new System.Drawing.Size(41, 12);
            this.lblASC2.TabIndex = 29;
            this.lblASC2.Text = "文字列";
            // 
            // lblASC1
            // 
            this.lblASC1.AutoSize = true;
            this.lblASC1.Location = new System.Drawing.Point(9, 63);
            this.lblASC1.Name = "lblASC1";
            this.lblASC1.Size = new System.Drawing.Size(218, 12);
            this.lblASC1.TabIndex = 28;
            this.lblASC1.Text = "暗号プロバイダ：RSACryptoServiceProvider";
            // 
            // txtASCString
            // 
            this.txtASCString.Location = new System.Drawing.Point(103, 85);
            this.txtASCString.Name = "txtASCString";
            this.txtASCString.Size = new System.Drawing.Size(212, 19);
            this.txtASCString.TabIndex = 27;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.nudSPWDSaltLength);
            this.tabPage1.Controls.Add(this.lblSPWD3);
            this.tabPage1.Controls.Add(this.btnSPWDAuth);
            this.tabPage1.Controls.Add(this.lblSPWD4);
            this.tabPage1.Controls.Add(this.txtSPWDSaltedPassword);
            this.tabPage1.Controls.Add(this.lblSPWD2);
            this.tabPage1.Controls.Add(this.txtSPWDRawPassword);
            this.tabPage1.Controls.Add(this.cbxSPWDPV);
            this.tabPage1.Controls.Add(this.lblSPWD1);
            this.tabPage1.Controls.Add(this.btnSPWDGen);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(326, 243);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "パスワードDB保存";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // nudSPWDSaltLength
            // 
            this.nudSPWDSaltLength.Location = new System.Drawing.Point(104, 60);
            this.nudSPWDSaltLength.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudSPWDSaltLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSPWDSaltLength.Name = "nudSPWDSaltLength";
            this.nudSPWDSaltLength.Size = new System.Drawing.Size(212, 19);
            this.nudSPWDSaltLength.TabIndex = 43;
            this.nudSPWDSaltLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblSPWD3
            // 
            this.lblSPWD3.AutoSize = true;
            this.lblSPWD3.Location = new System.Drawing.Point(9, 62);
            this.lblSPWD3.Name = "lblSPWD3";
            this.lblSPWD3.Size = new System.Drawing.Size(62, 12);
            this.lblSPWD3.TabIndex = 42;
            this.lblSPWD3.Text = "ソルトの長さ";
            // 
            // btnSPWDAuth
            // 
            this.btnSPWDAuth.Location = new System.Drawing.Point(103, 139);
            this.btnSPWDAuth.Name = "btnSPWDAuth";
            this.btnSPWDAuth.Size = new System.Drawing.Size(213, 23);
            this.btnSPWDAuth.TabIndex = 41;
            this.btnSPWDAuth.Text = "認証";
            this.btnSPWDAuth.UseVisualStyleBackColor = true;
            this.btnSPWDAuth.Click += new System.EventHandler(this.btnSPWDAuth_Click);
            // 
            // lblSPWD4
            // 
            this.lblSPWD4.AutoSize = true;
            this.lblSPWD4.Location = new System.Drawing.Point(9, 88);
            this.lblSPWD4.Name = "lblSPWD4";
            this.lblSPWD4.Size = new System.Drawing.Size(76, 12);
            this.lblSPWD4.TabIndex = 40;
            this.lblSPWD4.Text = "塩味パスワード";
            // 
            // txtSPWDSaltedPassword
            // 
            this.txtSPWDSaltedPassword.Location = new System.Drawing.Point(103, 85);
            this.txtSPWDSaltedPassword.Name = "txtSPWDSaltedPassword";
            this.txtSPWDSaltedPassword.ReadOnly = true;
            this.txtSPWDSaltedPassword.Size = new System.Drawing.Size(212, 19);
            this.txtSPWDSaltedPassword.TabIndex = 39;
            // 
            // lblSPWD2
            // 
            this.lblSPWD2.AutoSize = true;
            this.lblSPWD2.Location = new System.Drawing.Point(9, 38);
            this.lblSPWD2.Name = "lblSPWD2";
            this.lblSPWD2.Size = new System.Drawing.Size(64, 12);
            this.lblSPWD2.TabIndex = 38;
            this.lblSPWD2.Text = "生パスワード";
            // 
            // txtSPWDRawPassword
            // 
            this.txtSPWDRawPassword.Location = new System.Drawing.Point(103, 35);
            this.txtSPWDRawPassword.Name = "txtSPWDRawPassword";
            this.txtSPWDRawPassword.Size = new System.Drawing.Size(212, 19);
            this.txtSPWDRawPassword.TabIndex = 37;
            // 
            // cbxSPWDPV
            // 
            this.cbxSPWDPV.FormattingEnabled = true;
            this.cbxSPWDPV.Location = new System.Drawing.Point(103, 9);
            this.cbxSPWDPV.Name = "cbxSPWDPV";
            this.cbxSPWDPV.Size = new System.Drawing.Size(212, 20);
            this.cbxSPWDPV.TabIndex = 36;
            // 
            // lblSPWD1
            // 
            this.lblSPWD1.AutoSize = true;
            this.lblSPWD1.Location = new System.Drawing.Point(9, 13);
            this.lblSPWD1.Name = "lblSPWD1";
            this.lblSPWD1.Size = new System.Drawing.Size(75, 12);
            this.lblSPWD1.TabIndex = 35;
            this.lblSPWD1.Text = "暗号プロバイダ";
            // 
            // btnSPWDGen
            // 
            this.btnSPWDGen.Location = new System.Drawing.Point(103, 110);
            this.btnSPWDGen.Name = "btnSPWDGen";
            this.btnSPWDGen.Size = new System.Drawing.Size(213, 23);
            this.btnSPWDGen.TabIndex = 34;
            this.btnSPWDGen.Text = "塩味パスワードの生成";
            this.btnSPWDGen.UseVisualStyleBackColor = true;
            this.btnSPWDGen.Click += new System.EventHandler(this.btnSPWDGen_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 278);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "暗号化・復号化ユーティリティ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabHash.ResumeLayout(false);
            this.tabHash.PerformLayout();
            this.gbxHS.ResumeLayout(false);
            this.gbxHS.PerformLayout();
            this.tabKeyedHash.ResumeLayout(false);
            this.tabKeyedHash.PerformLayout();
            this.gbxKHS.ResumeLayout(false);
            this.gbxKHS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKHSStretching)).EndInit();
            this.tabSC.ResumeLayout(false);
            this.tabSC.PerformLayout();
            this.gbxSC.ResumeLayout(false);
            this.gbxSC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSCStretching)).EndInit();
            this.tabASC.ResumeLayout(false);
            this.tabASC.PerformLayout();
            this.gbxASC.ResumeLayout(false);
            this.gbxASC.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSPWDSaltLength)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSC;
        private System.Windows.Forms.ComboBox cbxSCPV;
        private System.Windows.Forms.Button btnSCDecrypt;
        private System.Windows.Forms.Button btnSCEncrypt;
        private System.Windows.Forms.TextBox txtSCCode;
        private System.Windows.Forms.TextBox txtSCSalt;
        private System.Windows.Forms.Label lblSC3;
        private System.Windows.Forms.Label lblSC2;
        private System.Windows.Forms.Label lblSC1;
        private System.Windows.Forms.TextBox txtSCPassword;
        private System.Windows.Forms.TextBox txtSCString;
        private System.Windows.Forms.TabPage tabASC;
        private System.Windows.Forms.Label lblSC5;
        private System.Windows.Forms.Label lblSC4;
        private System.Windows.Forms.NumericUpDown nudSCStretching;
        private System.Windows.Forms.Label lblSC6;
        private System.Windows.Forms.GroupBox gbxSC;
        private System.Windows.Forms.RadioButton rbnSCBytes;
        private System.Windows.Forms.RadioButton rbnSCString;
        private System.Windows.Forms.Label lblASC4;
        private System.Windows.Forms.TextBox txtASCCode;
        private System.Windows.Forms.TextBox txtASCPublic;
        private System.Windows.Forms.TextBox txtASCPrivate;
        private System.Windows.Forms.Label lblASC3;
        private System.Windows.Forms.GroupBox gbxASC;
        private System.Windows.Forms.RadioButton rbnASCBytes;
        private System.Windows.Forms.RadioButton rbnASCString;
        private System.Windows.Forms.Label lblASC5;
        private System.Windows.Forms.Button btnASCDecrypt;
        private System.Windows.Forms.Button btnASCEncrypt;
        private System.Windows.Forms.Label lblASC2;
        private System.Windows.Forms.Label lblASC1;
        private System.Windows.Forms.TextBox txtASCString;
        private System.Windows.Forms.Button btnASCGetKey;
        private System.Windows.Forms.TabPage tabHash;
        private System.Windows.Forms.Button btnGetHash;
        private System.Windows.Forms.GroupBox gbxHS;
        private System.Windows.Forms.RadioButton rbnHSBytes;
        private System.Windows.Forms.RadioButton rbnHSString;
        private System.Windows.Forms.ComboBox cbxHSPV;
        private System.Windows.Forms.Label lblHS1;
        private System.Windows.Forms.Label lblHS3;
        private System.Windows.Forms.TextBox txtHSCode;
        private System.Windows.Forms.Label lblHS2;
        private System.Windows.Forms.TextBox txtHSString;
        private System.Windows.Forms.TabPage tabKeyedHash;
        private System.Windows.Forms.GroupBox gbxKHS;
        private System.Windows.Forms.RadioButton rbnKHSBytes;
        private System.Windows.Forms.RadioButton rbnKHSString;
        private System.Windows.Forms.NumericUpDown nudKHSStretching;
        private System.Windows.Forms.Label lblKHS6;
        private System.Windows.Forms.Label lblKHS5;
        private System.Windows.Forms.ComboBox cbxKHSPV;
        private System.Windows.Forms.Button btnGetKeyedHash;
        private System.Windows.Forms.TextBox txtKHSCode;
        private System.Windows.Forms.Label lblKHS4;
        private System.Windows.Forms.TextBox txtKHSSalt;
        private System.Windows.Forms.Label lblKHS3;
        private System.Windows.Forms.Label lblKHS2;
        private System.Windows.Forms.Label lblKHS1;
        private System.Windows.Forms.TextBox txtKHSPassword;
        private System.Windows.Forms.TextBox txtKHSString;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblSPWD4;
        private System.Windows.Forms.TextBox txtSPWDSaltedPassword;
        private System.Windows.Forms.Label lblSPWD2;
        private System.Windows.Forms.TextBox txtSPWDRawPassword;
        private System.Windows.Forms.ComboBox cbxSPWDPV;
        private System.Windows.Forms.Label lblSPWD1;
        private System.Windows.Forms.Button btnSPWDGen;
        private System.Windows.Forms.Button btnSPWDAuth;
        private System.Windows.Forms.NumericUpDown nudSPWDSaltLength;
        private System.Windows.Forms.Label lblSPWD3;

    }
}

