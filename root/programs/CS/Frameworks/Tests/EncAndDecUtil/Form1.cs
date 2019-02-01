//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：Form1
//* クラス日本語名  ：暗号化/復号化ユーティリティ・ツール（メイン画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/12  西野 大介         新規作成
//*  2017/01/10  西野 大介         引数指定の誤りと、HashのStretchCountを指定可能に修正
//*  2017/01/10  西野 大介         秘密鍵と公開鍵の画面表示が誤っていたため、これを修正
//*  2017/01/13  西野 大介         上記修正への対応と、GetSaltedPasswordのI/F変更に対する修正対応
//*  2017/01/13  西野 大介         追加のGetSaltedPasswordメソッド、CodeSigning、JWSクラスの検証画面
//*  2017/12/**  西野 大介         メンテナンス、暗号化ライブラリ追加に伴うテストコード追加
//*  2018/10/30  西野 大介         メンテナンス、テスト・コードの追加（MAC, AEAD, KeyExchange, etc.
//*  2018/11/07  西野 大介         メンテナンス、テスト・コードの追加（DSA、ECDSA証明書, etc.
//*  2018/11/09  西野 大介         インスタンス・メソッド化対応
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//**********************************************************************************

using System;
using System.Windows.Forms;
using System.Diagnostics;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace EncAndDecUtil
{
    public partial class Form1 : Form
    {
        #region 証明書

        // ココの署名・検証処理を実行するには、ClickOnce署名機能などを使用し予め、
        // *.pfx 証明書を、password = "test" などとして、生成しておく必要があります。
        // 検証プロセスでは、*.pfx 証明書を使用することも出来ますが、通常は、秘密鍵無しで、
        // エクスポートした *.cer 証明証を使用します。この場合、パスワードは不要になります。
        // *.cer 証明証に証明書チェーンが無ければ、WindowsOSの証明書ストア（リポジトリ）である
        // 「信頼されたルート証明機関」にインストールする必要があります（実行アカウントにも注意が必要） 。

        /// <summary>SHA256RSA_pfx</summary>
        private string SHA256RSA_pfx = GetConfigParameter.GetConfigValue("SHA256RSA_pfx");
        /// <summary>SHA256RSA_cer</summary>
        private string SHA256RSA_cer = GetConfigParameter.GetConfigValue("SHA256RSA_cer");

        /// <summary>SHA256DSA_pfx</summary>
        private string SHA256DSA_pfx = GetConfigParameter.GetConfigValue("SHA256DSA_pfx");
        /// <summary>SHA256DSA_cer</summary>
        private string SHA256DSA_cer = GetConfigParameter.GetConfigValue("SHA256DSA_cer");

        /// <summary>SHA256ECDSA_pfx</summary>
        private string SHA256ECDSA_pfx = GetConfigParameter.GetConfigValue("SHA256ECDSA_pfx");
        /// <summary>SHA256ECDSA_cer</summary>
        private string SHA256ECDSA_cer = GetConfigParameter.GetConfigValue("SHA256ECDSA_cer");

        /// <summary>CertificateFilePassword</summary>
        private string CertificateFilePassword = "test";

        #endregion

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>Form_Load</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // HS    : Hash
            // KHS   : KeyedHash
            // SC    : SymmetricCryptography
            // ASC   : ASymmetricCryptography
            // MAC   : MessageAuthenticationCode
            // AEAD  : AuthenticatedEncryptionWithAssociatedData
            // DS    : DigitalSign(RSA, DSA)
            // DSE   : DigitalSign(ECDSA)
            // RKEX  : RsaKeyExchange
            // EKEX  : EcdhKeyExchange

            #region Cbx

            this.cbxHSPV.DataSource = Enum.GetValues(typeof(EnumHashAlgorithm));
            this.cbxKHSPV.DataSource = Enum.GetValues(typeof(EnumKeyedHashAlgorithm));

            this.cbxSCPV1.DataSource = Enum.GetValues(typeof(EnumSymmetricAlgorithm));
            this.cbxSCPV2.DataSource = Enum.GetValues(typeof(EnumSymmetricAlgorithm));
            this.cbxSCPV3.DataSource = Enum.GetValues(typeof(EnumSymmetricAlgorithm));

            this.cbxASCPV.DataSource = Enum.GetValues(typeof(EnumASymmetricAlgorithm));

            this.cbxSPWDPV1.DataSource = Enum.GetValues(typeof(EnumHashAlgorithm));
            this.cbxSPWDPV2.DataSource = Enum.GetValues(typeof(EnumKeyedHashAlgorithm));

            this.cbxMACPV.DataSource = Enum.GetValues(typeof(EnumKeyedHashAlgorithm));

            this.cbxDSPV.DataSource = Enum.GetValues(typeof(EnumDigitalSignAlgorithm));
            this.cbxDSEPV.DataSource = Enum.GetValues(typeof(EnumDigitalSignAlgorithm));

            this.cbxRKEXPV.DataSource = Enum.GetValues(typeof(EnumKeyExchange));

            #endregion

            #region Version

            // n.n の表記ならコレでイケる。
            if (float.Parse(EnvInfo.RegistryFrameworkVersion.Substring(0, 3)) < 4.7F)
            {
                this.rbnDSX509D.Enabled = false;            // DSA
                this.rbnDSEX509.Enabled = false;            // ECDsa
                this.rbnJWSES256_Param.Enabled = false;     // ECDsa
                this.rbnJWSES256_X509.Enabled = false;      // ECDsa
            }

            #endregion
        }

        #endregion

        #region 暗号処理

        #region ハッシュ

        #region ハッシュ

        /// <summary>ハッシュ</summary>
        private void btnGetHash_Click(object sender, EventArgs e)
        {
            if (this.rbnHSString.Checked)
            {
                txtHSCode.Text = GetHash.GetHashString(
                    txtHSString.Text, (EnumHashAlgorithm)cbxHSPV.SelectedValue, (int)this.nudHSStretching.Value);
            }
            else
            {
                txtHSCode.Text = CustomEncode.ToHexString(GetHash.GetHashBytes(
                    CustomEncode.StringToByte(txtHSString.Text, CustomEncode.UTF_8),
                    (EnumHashAlgorithm)cbxHSPV.SelectedValue, (int)this.nudHSStretching.Value));
            }
        }

        #endregion

        #region キー付きハッシュ

        /// <summary>キー付きハッシュ</summary>
        private void btnGetKeyedHash_Click(object sender, EventArgs e)
        {
            if (this.rbnKHSString.Checked)
            {
                // String
                this.txtKHSCode.Text =
                    GetKeyedHash.GetKeyedHashString(
                        this.txtKHSString.Text,
                        (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                        this.txtKHSPassword.Text);
            }
            else
            {
                // Bytes
                this.txtKHSCode.Text =
                    CustomEncode.ToHexString(
                        GetKeyedHash.GetKeyedHashBytes(
                            CustomEncode.StringToByte(txtKHSString.Text, CustomEncode.UTF_8),
                            (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                            CustomEncode.StringToByte(this.txtKHSPassword.Text, CustomEncode.UTF_8)));
            }
        }

        #endregion

        #region 塩味パスワード生成（ハッシュ）

        /// <summary>塩味パスワード生成（ハッシュ）</summary>
        private void btnSPWDGen1_Click(object sender, EventArgs e)
        {
            this.txtSPWDSaltedPassword1.Text = GetPasswordHashV2.GetSaltedPassword(
                this.txtSPWDRawPassword1.Text, (EnumHashAlgorithm)this.cbxSPWDPV1.SelectedValue,
                (int)this.nudSPWDSaltLength1.Value, (int)this.nudSPWDStretchCount1.Value);
        }

        /// <summary>生パスワード（ハッシュ）と塩味パスワードを比較認証</summary>
        private void btnSPWDAuth1_Click(object sender, EventArgs e)
        {
            // パラメタ系は渡さないで検証可能
            if (GetPasswordHashV2.EqualSaltedPassword(
                this.txtSPWDRawPassword1.Text,
                this.txtSPWDSaltedPassword1.Text,
                (EnumHashAlgorithm)this.cbxSPWDPV1.SelectedValue))
            {
                MessageBox.Show("認証成功");
            }
            else
            {
                MessageBox.Show("認証失敗");

            }
        }

        #endregion

        #region 塩味パスワード生成（キー付きハッシュ）

        /// <summary>塩味パスワード（キー付きハッシュ）生成</summary>
        private void btnSPWDGen2_Click(object sender, EventArgs e)
        {
            this.txtSPWDSaltedPassword2.Text = GetPasswordHashV2.GetSaltedPassword(
                this.txtSPWDRawPassword2.Text,
                (EnumKeyedHashAlgorithm)this.cbxSPWDPV2.SelectedValue,
                this.txtSPWDKey2.Text,
                (int)this.nudSPWDSaltLength2.Value,
                (int)this.nudSPWDStretchCount2.Value);
        }

        /// <summary>生パスワード（キー付きハッシュ）と塩味パスワードを比較認証</summary>
        private void btnSPWDAuth2_Click(object sender, EventArgs e)
        {
            // パラメタ系は渡さないで検証可能
            if (GetPasswordHashV2.EqualSaltedPassword(
                this.txtSPWDRawPassword2.Text,
                this.txtSPWDSaltedPassword2.Text,
                (EnumKeyedHashAlgorithm)this.cbxSPWDPV2.SelectedValue))
            {
                MessageBox.Show("認証成功");
            }
            else
            {
                MessageBox.Show("認証失敗");

            }
        }

        #endregion

        #endregion

        #region 秘密鍵

        /// <summary>秘密鍵・暗号化</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            SymmetricCryptography sc = null;

            EnumSymmetricAlgorithm esa =
                (EnumSymmetricAlgorithm)cbxSCPV1.SelectedValue
                | (EnumSymmetricAlgorithm)cbxSCPV2.SelectedValue
                | (EnumSymmetricAlgorithm)cbxSCPV3.SelectedValue;

            if (string.IsNullOrEmpty(txtSCSalt.Text))
            {
                // ソルト無し
                sc = new SymmetricCryptography(esa);

                if (this.rbnSCString.Checked)
                {
                    // String
                    this.txtSCCode.Text = sc.EncryptString(
                        this.txtSCString.Text,
                        this.txtSCPassword.Text);
                }
                else
                {
                    // Bytes
                    this.txtSCCode.Text = CustomEncode.ToHexString(sc.EncryptBytes(
                        CustomEncode.StringToByte(txtSCString.Text, CustomEncode.UTF_8),
                        this.txtSCPassword.Text));
                }
            }
            else
            {
                // ソルト有り
                if (this.nudSCStretching.Value == 0)
                {
                    // ストレッチング無し
                    sc = new SymmetricCryptography(esa, CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8));

                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCCode.Text = sc.EncryptString(this.txtSCString.Text, this.txtSCPassword.Text);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCCode.Text =
                            CustomEncode.ToHexString(sc.EncryptBytes(CustomEncode.StringToByte(
                                txtSCString.Text, CustomEncode.UTF_8), this.txtSCPassword.Text));
                    }
                }
                else
                {
                    // ストレッチング有り
                    sc = new SymmetricCryptography(esa,
                        CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                        (int)this.nudSCStretching.Value);

                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCCode.Text = sc.EncryptString(this.txtSCString.Text, this.txtSCPassword.Text);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCCode.Text =
                            CustomEncode.ToHexString(sc.EncryptBytes(CustomEncode.StringToByte(
                                txtSCString.Text, CustomEncode.UTF_8), this.txtSCPassword.Text));
                    }
                }
            }
        }

        /// <summary>秘密鍵・復号化</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            SymmetricCryptography sc = null;

            EnumSymmetricAlgorithm esa =
                (EnumSymmetricAlgorithm)cbxSCPV1.SelectedValue
                | (EnumSymmetricAlgorithm)cbxSCPV2.SelectedValue
                | (EnumSymmetricAlgorithm)cbxSCPV3.SelectedValue;

            if (string.IsNullOrEmpty(txtSCSalt.Text))
            {
                // ソルト無し
                sc = new SymmetricCryptography(esa);

                if (this.rbnSCString.Checked)
                {
                    // String
                    this.txtSCString.Text = sc.DecryptString(this.txtSCCode.Text, this.txtSCPassword.Text);
                }
                else
                {
                    // Bytes
                    this.txtSCString.Text =
                        CustomEncode.ByteToString(sc.DecryptBytes(CustomEncode.FormHexString(
                            this.txtSCCode.Text), this.txtSCPassword.Text), CustomEncode.UTF_8);
                }
            }
            else
            {
                // ソルト有り
                if (this.nudSCStretching.Value == 0)
                {
                    // ストレッチング無し
                    sc = new SymmetricCryptography(esa, CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8));

                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCString.Text = sc.DecryptString(this.txtSCCode.Text, this.txtSCPassword.Text);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCString.Text =
                            CustomEncode.ByteToString(sc.DecryptBytes(CustomEncode.FormHexString(
                                this.txtSCCode.Text), this.txtSCPassword.Text), CustomEncode.UTF_8);
                    }
                }
                else
                {
                    // ストレッチング有り
                    sc = new SymmetricCryptography(esa,
                        CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                        (int)this.nudSCStretching.Value);

                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCString.Text = sc.DecryptString(this.txtSCCode.Text, this.txtSCPassword.Text);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCString.Text =
                         CustomEncode.ByteToString(sc.DecryptBytes(CustomEncode.FormHexString(
                             this.txtSCCode.Text), this.txtSCPassword.Text), CustomEncode.UTF_8);
                    }
                }
            }
        }

        #endregion

        #region 公開鍵

        /// <summary>キーペア取得</summary>
        private void button3_Click(object sender, EventArgs e)
        {
            ASymmetricCryptography asc = null;
            if (((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue) == EnumASymmetricAlgorithm.X509)
            {
                asc = new ASymmetricCryptography(
                    EnumASymmetricAlgorithm.X509, this.SHA256RSA_pfx, this.CertificateFilePassword,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                this.txtASCPublic.Text = "- x509 -";
                this.txtASCPrivate.Text = "- x509 -";
            }
            else
            {
                asc = new ASymmetricCryptography((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue);
                this.txtASCPublic.Text = asc.PublicXmlKey;
                this.txtASCPrivate.Text = asc.PrivateXmlKey;
            }

            string date = "hogehoge";
            if (!(asc.DecryptString(asc.EncryptString(date)) == date))
            {
                throw new Exception("問題あり");
            }
        }

        /// <summary>公開鍵で暗号化</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtASCPrivate.Text)) { return; }

            ASymmetricCryptography asc = null;

            if (((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue) == EnumASymmetricAlgorithm.X509)
            {
                asc = new ASymmetricCryptography(
                    EnumASymmetricAlgorithm.X509, this.SHA256RSA_cer, "",
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                if (this.rbnASCString.Checked)
                {
                    // String
                    this.txtASCCode.Text = asc.EncryptString(this.txtASCString.Text);
                }
                else
                {
                    // Bytes
                    this.txtASCCode.Text =
                        CustomEncode.ToHexString(
                            asc.EncryptBytes(CustomEncode.StringToByte(
                                this.txtASCString.Text, CustomEncode.UTF_8)));
                }
            }
            else
            {
                asc = new ASymmetricCryptography((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue);

                if (this.rbnASCString.Checked)
                {
                    // String
                    this.txtASCCode.Text = asc.EncryptString(this.txtASCString.Text, this.txtASCPublic.Text);
                }
                else
                {
                    // Bytes
                    this.txtASCCode.Text =
                        CustomEncode.ToHexString(
                            asc.EncryptBytes(CustomEncode.StringToByte(
                                this.txtASCString.Text, CustomEncode.UTF_8), this.txtASCPublic.Text));
                }
            }
        }

        /// <summary>秘密鍵で復号化</summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtASCPublic.Text)) { return; }

            ASymmetricCryptography asc = null;

            if (((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue) == EnumASymmetricAlgorithm.X509)
            {
                asc = new ASymmetricCryptography(
                   EnumASymmetricAlgorithm.X509, this.SHA256RSA_pfx, this.CertificateFilePassword,
                   X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                if (this.rbnASCString.Checked)
                {
                    // String
                    this.txtASCString.Text = asc.DecryptString(this.txtASCCode.Text);
                }
                else
                {
                    // Bytes
                    this.txtASCString.Text =
                        CustomEncode.ByteToString(
                            asc.DecryptBytes(CustomEncode.FormHexString(
                                this.txtASCCode.Text)), CustomEncode.UTF_8);
                }
            }
            else
            {
                asc = new ASymmetricCryptography((EnumASymmetricAlgorithm)cbxASCPV.SelectedValue);

                if (this.rbnASCString.Checked)
                {
                    // String
                    this.txtASCString.Text = asc.DecryptString(this.txtASCCode.Text, this.txtASCPrivate.Text);
                }
                else
                {
                    // Bytes
                    this.txtASCString.Text =
                        CustomEncode.ByteToString(
                            asc.DecryptBytes(CustomEncode.FormHexString(
                                this.txtASCCode.Text), this.txtASCPrivate.Text), CustomEncode.UTF_8);
                }
            }
        }

        #endregion

        #region 署名

        #region RSA, DSA

        /// <summary>rbnDS_CheckedChanged</summary>
        private void rbnDS_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbnDSXML.Checked || this.rbnDSParam.Checked || this.rbnDSX509D.Checked)
            {
                this.cbxDSPV.Enabled = true;
                this.txtDSHash.Text = "";
                this.txtDSHash.ReadOnly = true;
            }
            else if (this.rbnDSX509R.Checked)
            {
                this.cbxDSPV.Enabled = false;
                this.txtDSHash.Text = "SHA256";
                this.txtDSHash.ReadOnly = false;
            }
        }

        /// <summary>署名</summary>
        private void btnDSSign_Click(object sender, EventArgs e)
        {
            DigitalSignXML dsXML = null;
            DigitalSignParam dsParam = null;
            DigitalSignX509 dsX509 = null;

            byte[] data = CustomEncode.StringToByte(this.txtDSData.Text, CustomEncode.UTF_8);
            byte[] sign = null;
            bool ret = false;

            EnumDigitalSignAlgorithm edsa = (EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue;

            if (edsa == EnumDigitalSignAlgorithm.RsaCSP_MD5
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA1
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA256
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA384
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA512
                || edsa == EnumDigitalSignAlgorithm.DsaCSP_SHA1)
            {
                if (rbnDSXML.Checked)
                {
                    // XMLKey
                    dsXML = new DigitalSignXML(edsa);

                    if (this.chkDSUF.Checked)
                    {
                        sign = dsXML.SignByFormatter(data);
                        ret = dsXML.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        sign = dsXML.Sign(data);
                        ret = dsXML.Verify(data, sign);
                    }

                    this.txtDSPrivateKey.Text = dsXML.PrivateKey;
                    this.txtDSPublicKey.Text = dsXML.PublicKey;
                }
                else if (rbnDSParam.Checked)
                {
                    // ParamKey
                    dsParam = new DigitalSignParam(edsa);

                    if (this.chkDSUF.Checked)
                    {
                        sign = dsParam.SignByFormatter(data);
                        ret = dsParam.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        sign = dsParam.Sign(data);
                        ret = dsParam.Verify(data, sign);
                    }

                    this.txtDSPrivateKey.Text = CustomEncode.ToBase64String(BinarySerialize.ObjectToBytes(dsParam.PrivateKey));
                    this.txtDSPublicKey.Text = CustomEncode.ToBase64String(BinarySerialize.ObjectToBytes(dsParam.PublicKey));
                }
                else
                {
                    // X509
                    if (this.rbnDSX509R.Checked)
                    {
                        dsX509 = new DigitalSignX509(this.SHA256RSA_pfx, this.CertificateFilePassword, this.txtDSHash.Text,
                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                    }
                    else if (this.rbnDSX509D.Checked)
                    {
                        // NET46.1以降に、I/Fは存在する。Windows上で動作しない可能性あり。
                        dsX509 = new DigitalSignX509(this.SHA256DSA_pfx, this.CertificateFilePassword, this.txtDSHash.Text,
                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                    }

                    if (this.chkDSUF.Checked)
                    {
                        sign = dsX509.SignByFormatter(data);
                        ret = dsX509.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        sign = dsX509.Sign(data);
                        ret = dsX509.Verify(data, sign);
                    }

                    this.txtDSPrivateKey.Text = dsX509.PrivateKey.GetType().ToString();
                    this.txtDSPublicKey.Text = dsX509.PublicKey.GetType().ToString();
                }
            }
            else
            {
                this.TabControl1.SelectedIndex = 7;
                return;
            }

            txtDSSign.Text = CustomEncode.ToBase64String(sign);
        }

        /// <summary>検証</summary>
        private void btnDSVerify_Click(object sender, EventArgs e)
        {
            DigitalSignXML dsXML = null;
            DigitalSignParam dsParam = null;
            DigitalSignX509 dsX509 = null;

            byte[] data = CustomEncode.StringToByte(this.txtDSData.Text, CustomEncode.UTF_8);
            byte[] sign = CustomEncode.FromBase64String(this.txtDSSign.Text);
            bool ret = false;

            EnumDigitalSignAlgorithm edsa = (EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue;

            if (edsa == EnumDigitalSignAlgorithm.RsaCSP_MD5
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA1
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA256
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA384
                || edsa == EnumDigitalSignAlgorithm.RsaCSP_SHA512
                || edsa == EnumDigitalSignAlgorithm.DsaCSP_SHA1)
            {
                if (rbnDSXML.Checked)
                {
                    // XMLKey
                    dsXML = new DigitalSignXML(this.txtDSPublicKey.Text,
                        (EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue);

                    if (this.chkDSUF.Checked)
                    {
                        ret = dsXML.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        ret = dsXML.Verify(data, sign);
                    }
                }
                else if (rbnDSParam.Checked)
                {
                    // ParamKey
                    if (((EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue) ==
                            EnumDigitalSignAlgorithm.DsaCSP_SHA1)
                    {
                        dsParam = new DigitalSignParam(
                            (DSAParameters)BinarySerialize.BytesToObject(CustomEncode.FromBase64String(this.txtDSPublicKey.Text)),
                            (EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue);
                    }
                    else
                    {
                        dsParam = new DigitalSignParam(
                            (RSAParameters)BinarySerialize.BytesToObject(CustomEncode.FromBase64String(this.txtDSPublicKey.Text)),
                            (EnumDigitalSignAlgorithm)this.cbxDSPV.SelectedValue);
                    }

                    if (this.chkDSUF.Checked)
                    {
                        ret = dsParam.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        ret = dsParam.Verify(data, sign);
                    }
                }
                else
                {
                    // X509
                    if (this.rbnDSX509R.Checked)
                    {
                        dsX509 = new DigitalSignX509(SHA256RSA_cer, "", this.txtDSHash.Text);
                    }
                    else if (this.rbnDSX509D.Checked)
                    {
                        dsX509 = new DigitalSignX509(SHA256DSA_cer, "", this.txtDSHash.Text);
                    }

                    if (this.chkDSUF.Checked)
                    {
                        ret = dsX509.VerifyByDeformatter(data, sign);
                    }
                    else
                    {
                        ret = dsX509.Verify(data, sign);
                    }
                }
            }
            else
            {
                this.TabControl1.SelectedIndex = 7;
                return;
            }

            if (ret)
            {
                MessageBox.Show("検証成功");
            }
            else
            {
                MessageBox.Show("検証失敗");
            }
        }

        #endregion

        #region ECDsa

        /// <summary>rbnDSE_CheckedChanged</summary>
        private void rbnDSE_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbnDSECng.Checked)
            {
                this.cbxDSEPV.Enabled = true;
                this.txtDSEHash.Text = "";
                this.txtDSEHash.ReadOnly = true;
            }
            else if (this.rbnDSEX509.Checked)
            {
                this.cbxDSEPV.Enabled = false;
                this.txtDSEHash.Text = "SHA256";
                this.txtDSEHash.ReadOnly = false;
            }
        }

        /// <summary>署名</summary>
        private void btnDSESign_Click(object sender, EventArgs e)
        {

#if NET45 || NET46
            MessageBox.Show("NET47以上でサポート");
#else

            byte[] data = CustomEncode.StringToByte(this.txtDSEData.Text, CustomEncode.UTF_8);
            byte[] sign = null;

            EnumDigitalSignAlgorithm edsa = (EnumDigitalSignAlgorithm)this.cbxDSEPV.SelectedValue;

            if (rbnDSECng.Checked)
            {
                DigitalSignECDsaCng dsECDsa = null;

                if (edsa == EnumDigitalSignAlgorithm.ECDsaCng_P256
                    || edsa == EnumDigitalSignAlgorithm.ECDsaCng_P384
                    || edsa == EnumDigitalSignAlgorithm.ECDsaCng_P521)
                {
                    dsECDsa = new DigitalSignECDsaCng(edsa);
                    sign = dsECDsa.Sign(data);
                    bool ret = dsECDsa.Verify(data, sign);

                    this.txtDSEPrivateKey.Text = dsECDsa.PrivateKey.GetType().ToString();
                    this.txtDSEPublicKey.Text = CustomEncode.ToBase64String(dsECDsa.PublicKey);
                }
                else
                {
                    this.TabControl1.SelectedIndex = 6;
                    return;
                }
            }
            else
            {
                DigitalSignECDsaX509 dsECDsa = null;

                // NET47以降に、I/Fは存在する。
                dsECDsa = new DigitalSignECDsaX509(this.SHA256ECDSA_pfx, this.CertificateFilePassword, new HashAlgorithmName(this.txtDSEHash.Text));
                sign = dsECDsa.Sign(data);
                bool ret = dsECDsa.Verify(data, sign);

                this.txtDSEPrivateKey.Text = ((DigitalSignECDsaX509)dsECDsa).PrivateKey.GetType().ToString();
                this.txtDSEPublicKey.Text = ((DigitalSignECDsaX509)dsECDsa).PublicKey.GetType().ToString();
            }

            txtDSESign.Text = CustomEncode.ToBase64String(sign);
#endif
        }

        /// <summary>検証<</summary>
        private void btnDSEVerify_Click(object sender, EventArgs e)
        {

#if NET45 || NET46
            MessageBox.Show("NET47以上でサポート");
#else

            byte[] data = CustomEncode.StringToByte(this.txtDSEData.Text, CustomEncode.UTF_8);
            byte[] sign = CustomEncode.FromBase64String(this.txtDSESign.Text);
            bool ret = false;

            DigitalSign dsECDsa = null;

            EnumDigitalSignAlgorithm edsa = (EnumDigitalSignAlgorithm)this.cbxDSEPV.SelectedValue;

            if (rbnDSECng.Checked)
            {
                if (edsa == EnumDigitalSignAlgorithm.ECDsaCng_P256
                    || edsa == EnumDigitalSignAlgorithm.ECDsaCng_P384
                    || edsa == EnumDigitalSignAlgorithm.ECDsaCng_P521)
                {
                    dsECDsa = new DigitalSignECDsaCng(CustomEncode.FromBase64String(this.txtDSEPublicKey.Text));
                }
                else
                {
                    this.TabControl1.SelectedIndex = 6;
                    return;
                }
            }
            else
            {
                dsECDsa = new DigitalSignECDsaX509(this.SHA256ECDSA_cer, "", new HashAlgorithmName(this.txtDSEHash.Text));
            }

            ret = dsECDsa.Verify(data, sign);

            if (ret)
            {
                MessageBox.Show("検証成功");
            }
            else
            {
                MessageBox.Show("検証失敗");
            }
#endif
        }

        #endregion

        #endregion

        #region 鍵交換

        #region RSA鍵交換

        /// <summary>RSAのアリス</summary>
        RsaAlice _rsaAlice = null;

        /// <summary>RSAのボブ</summary>
        RsaBob _rsaBob = null;

        /// <summary>鍵交換</summary>
        private void btnRKEXEC_Click(object sender, EventArgs e)
        {
            EnumKeyExchange ekex = (EnumKeyExchange)this.cbxRKEXPV.SelectedValue;

            // キー交換、秘密鍵生成 & 暗号化プロバイダ生成
            if (ekex == EnumKeyExchange.RSAPKCS1KeyExchange)
            {
                if (((Button)sender).Name == "btnRKEXEC1")
                {
                    this._rsaBob = new RsaPkcs1Bob();
                }
                else
                {
                    this._rsaBob = new RsaPkcs1Bob(
                        this.SHA256RSA_pfx, this.CertificateFilePassword,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                }

                this._rsaAlice = new RsaPkcs1Alice(this._rsaBob.ExchangeKey);
                ((RsaPkcs1Bob)this._rsaBob).GeneratePrivateKey(this._rsaAlice.ExchangeKey, this._rsaAlice.IV);
            }
            else if (ekex == EnumKeyExchange.RSAOAEPKeyExchange)
            {
                if (((Button)sender).Name == "btnRKEXEC1")
                {
                    this._rsaBob = new RsaOaepBob();
                }
                else
                {
                    this._rsaBob = new RsaOaepBob(
                        this.SHA256RSA_pfx, this.CertificateFilePassword,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                }

                this._rsaAlice = new RsaOaepAlice(this._rsaBob.ExchangeKey);
                ((RsaOaepBob)this._rsaBob).GeneratePrivateKey(this._rsaAlice.ExchangeKey, this._rsaAlice.IV);
            }
            else
            {
                this.TabControl1.SelectedIndex = 9;
                return;
            }

            this.txtRKEXKeyInfo.Text = "";
            this.txtRKEXKeyInfo.Text += "bob.ExchangeKey:\n";
            this.txtRKEXKeyInfo.Text += CustomEncode.ToBase64String(this._rsaBob.ExchangeKey);
            this.txtRKEXKeyInfo.Text += "\n";
            this.txtRKEXKeyInfo.Text += "alice.ExchangeKey:\n";
            this.txtRKEXKeyInfo.Text += CustomEncode.ToBase64String(this._rsaAlice.ExchangeKey);
            this.txtRKEXKeyInfo.Text += "\n";
            this.txtRKEXKeyInfo.Text += "IV of alice and bob:\n";
            this.txtRKEXKeyInfo.Text += CustomEncode.ToBase64String(this._rsaAlice.IV);
        }

        /// <summary>送受信</summary>
        private void btnRKEXSR_Click(object sender, EventArgs e)
        {
            EnumKeyExchange ekex = (EnumKeyExchange)this.cbxRKEXPV.SelectedValue;

            if (ekex == EnumKeyExchange.RSAPKCS1KeyExchange
                || ekex == EnumKeyExchange.RSAOAEPKeyExchange)
            {
                this.txtRKEXBobString.Text =
                    CustomEncode.ByteToString(
                        this._rsaBob.Decrypt(
                            this._rsaAlice.Encrypt(
                                CustomEncode.StringToByte(
                                    this.txtRKEXAliceString.Text, CustomEncode.UTF_8))), CustomEncode.UTF_8);

            }
            else
            {
                this.TabControl1.SelectedIndex = 9;
                return;
            }
        }

        #endregion

        #region ECDH鍵交換

        /// <summary>ECDHCngのアリス</summary>
        EcdhCngAlice _ecdhCngAlice = null;

        /// <summary>ECDHCngのボブ</summary>
        EcdhCngBob _ecdhCngBob = null;

        /// <summary>鍵交換</summary>
        private void btnEKEXEC_Click(object sender, EventArgs e)
        {
            this._ecdhCngBob = new EcdhCngBob();
            ECDiffieHellmanCng ecdhCng = (ECDiffieHellmanCng)this._ecdhCngBob.ECDiffieHellman;
            ECDiffieHellmanKeyDerivationFunction fnc = ecdhCng.KeyDerivationFunction;
            CngAlgorithm cngAlg = ecdhCng.HashAlgorithm;
            byte[] hmacKey = ecdhCng.HmacKey;
            byte[] label = ecdhCng.Label;
            byte[] seed = ecdhCng.Seed;
            byte[] secretPrepend = ecdhCng.SecretPrepend;
            byte[] secretAppend = ecdhCng.SecretAppend;

            // 以下を一致させる
            if (fnc == ECDiffieHellmanKeyDerivationFunction.Hash)
            {
                this._ecdhCngAlice = new EcdhCngAlice(
                    fnc, cngAlg, hmacKey, secretPrepend, secretAppend);
                //ecdh.KeyDerivationFunction = func;
                //ecdh.HashAlgorithm = hash;
                //ecdh.SecretPrepend = secretPrependOrLabel;
                //ecdh.SecretAppend = secretAppendOrSeed;
            }
            else if (fnc == ECDiffieHellmanKeyDerivationFunction.Hmac)
            {
                this._ecdhCngAlice = new EcdhCngAlice(
                    fnc, cngAlg, hmacKey, secretPrepend, secretAppend);
                //ecdh.KeyDerivationFunction = func;
                //ecdh.HashAlgorithm = hash;
                //ecdh.HmacKey = hmacKey;
                //ecdh.SecretPrepend = secretPrependOrLabel;
                //ecdh.SecretAppend = secretAppendOrSeed;
            }
            else if (fnc == ECDiffieHellmanKeyDerivationFunction.Tls)
            {
                this._ecdhCngAlice = new EcdhCngAlice(
                    fnc, cngAlg, hmacKey, label, seed);
                //ecdh.KeyDerivationFunction = func;
                //ecdh.Label = secretPrependOrLabel;
                //ecdh.Seed = secretAppendOrSeed;
            }

            // キー交換、秘密鍵生成
            this._ecdhCngAlice.DeriveKeyMaterial(this._ecdhCngBob.ExchangeKey);
            this._ecdhCngBob.DeriveKeyMaterial(this._ecdhCngAlice.ExchangeKey);

            // 暗号化プロバイダ生成
            this._ecdhCngAlice.CreateAesSP();
            this._ecdhCngBob.CreateAesSP(this._ecdhCngAlice.IV);

            this.txtEKEXKeyInfo.Text = "";
            this.txtEKEXKeyInfo.Text += "bob.ExchangeKey:\n";
            this.txtEKEXKeyInfo.Text += CustomEncode.ToBase64String(this._ecdhCngBob.ExchangeKey);
            this.txtEKEXKeyInfo.Text += "\n";
            this.txtEKEXKeyInfo.Text += "alice.ExchangeKey:\n";
            this.txtEKEXKeyInfo.Text += CustomEncode.ToBase64String(this._ecdhCngAlice.ExchangeKey);
            this.txtEKEXKeyInfo.Text += "\n";
            this.txtEKEXKeyInfo.Text += "IV of alice and bob:\n";
            this.txtEKEXKeyInfo.Text += CustomEncode.ToBase64String(this._ecdhCngAlice.IV);
        }

        /// <summary>送受信</summary>
        private void btnEKEXSR_Click(object sender, EventArgs e)
        {
            this.txtEKEXBobString.Text =
                    CustomEncode.ByteToString(
                        this._ecdhCngBob.Decrypt(
                            this._ecdhCngAlice.Encrypt(
                                CustomEncode.StringToByte(
                                    this.txtEKEXAliceString.Text, CustomEncode.UTF_8))), CustomEncode.UTF_8);
        }

        #endregion

        #endregion

        #region MAC

        /// <summary>btnGetMAC_Click</summary>
        private void btnGetMAC_Click(object sender, EventArgs e)
        {
            this.txtMACValue.Text = MsgAuthCode.GetMAC(
                this.txtMACString.Text,
                (EnumKeyedHashAlgorithm)this.cbxMACPV.SelectedValue,
                this.txtMACPassword.Text);
        }

        /// <summary>btnVerifyMAC_Click</summary>
        private void btnVerifyMAC_Click(object sender, EventArgs e)
        {
            bool ret = MsgAuthCode.VerifyMAC(
                this.txtMACString.Text,
                (EnumKeyedHashAlgorithm)this.cbxMACPV.SelectedValue,
                this.txtMACPassword.Text,
                this.txtMACValue.Text);

            if (ret)
            {
                MessageBox.Show("検証成功");
            }
            else
            {
                MessageBox.Show("検証失敗");
            }
        }

        #endregion

        #region AEAD

        /// <summary>AEAD暗号化</summary>
        private void btnAEADEncrypt_Click(object sender, EventArgs e)
        {
            byte[] aeadCek = CustomEncode.StringToByte(this.txtAEADCek.Text, CustomEncode.UTF_8);
            byte[] aeadIv = CustomEncode.StringToByte(this.txtAEADIv.Text, CustomEncode.UTF_8);
            byte[] aeadAad = CustomEncode.StringToByte(this.txtAEADAad.Text, CustomEncode.UTF_8);

            AuthEncrypt aead = null;

            if (this.rbnAeadA256GCM.Checked)
            {
                aead = new AeadA256Gcm(aeadCek, aeadIv, aeadAad);
            }
            else
            {
                aead = new AeadA128CbcHS256(aeadCek, aeadIv, aeadAad);
            }            

            aead.Encrypt(CustomEncode.StringToByte(this.txtAEADPlaint.Text, CustomEncode.UTF_8));
            this.txtAEADCiphert.Text = CustomEncode.ToBase64String(aead.Result.Ciphert);
            this.txtAEADTag.Text = CustomEncode.ToBase64String(aead.Result.Tag);
        }

        /// <summary>AEAD復号化</summary>
        private void btnAEADDecrypt_Click(object sender, EventArgs e)
        {
            byte[] aeadCek = CustomEncode.StringToByte(this.txtAEADCek.Text, CustomEncode.UTF_8);
            byte[] aeadIv = CustomEncode.StringToByte(this.txtAEADIv.Text, CustomEncode.UTF_8);
            byte[] aeadAad = CustomEncode.StringToByte(this.txtAEADAad.Text, CustomEncode.UTF_8);

            AuthEncrypt aead = null;

            if (this.rbnAeadA256GCM.Checked)
            {
                aead = new AeadA256Gcm(aeadCek, aeadIv, aeadAad);
            }
            else
            {
                aead = new AeadA128CbcHS256(aeadCek, aeadIv, aeadAad);
            }

            this.txtAEADPlaint.Text = CustomEncode.ByteToString(
                aead.Decrypt(new AeadResult()
                {
                    Ciphert = CustomEncode.FromBase64String(this.txtAEADCiphert.Text),
                    Tag = CustomEncode.FromBase64String(this.txtAEADTag.Text)
                }),
                CustomEncode.UTF_8);
        }

        /// <summary>
        /// RFC7516 > Appendix A.  JWE Examples > A.1.  Example JWE using RSAES OAEP and AES GCM
        /// https://tools.ietf.org/html/draft-ietf-jose-json-web-encryption-23#appendix-A.1
        /// </summary>
        private void TestA256GCMByRFC7516()
        {
            #region データ
            // A.1
            var plaint = new byte[]
            {
                84, 104, 101, 32, 116, 114, 117, 101, 32, 115, 105, 103, 110, 32,
                111, 102, 32, 105, 110, 116, 101, 108, 108, 105, 103, 101, 110, 99,
                101, 32, 105, 115, 32, 110, 111, 116, 32, 107, 110, 111, 119, 108,
                101, 100, 103, 101, 32, 98, 117, 116, 32, 105, 109, 97, 103, 105,
                110, 97, 116, 105, 111, 110, 46
            };

            // A.1.2  CEK
            var cek = new byte[]
            {
                177, 161, 244, 128, 84, 143, 225, 115, 63, 180, 3, 255, 107, 154,
                212, 246, 138, 7, 110, 91, 112, 46, 34, 105, 47, 130, 203, 46, 122,
                234, 64, 252
            };

            // A.1.4 IV
            var iv = new byte[]
            {
                227, 197, 117, 252, 2, 219, 233, 68, 180, 225, 77, 219
            };

            // A.1.5 AAD
            var aad = new byte[]
            {
                101, 121, 74, 104, 98, 71, 99, 105,
                79, 105, 74, 83, 85, 48, 69,
                116, 84, 48, 70, 70, 85, 67, 73,
                115, 73, 109, 86, 117, 89, 121, 73,
                54, 73, 107, 69, 121, 78, 84, 90,
                72, 81, 48, 48, 105, 102, 81
            };

            // A.1.6 CipherText

            var ciphert = new byte[]
            {
                229, 236, 166, 241, 53, 191, 115,
                196, 174, 43, 73, 109, 39, 122,
                233, 96, 140, 206, 120, 52, 51, 237,
                48, 11, 190, 219, 186, 80, 111,
                104, 50, 142, 47, 167, 59, 61, 181,
                127, 196, 21, 40, 82, 242, 32,
                123, 143, 168, 226, 73, 216, 176,
                144, 138, 247, 106, 60, 16, 205,
                160, 109, 64, 63, 192
            };

            // Tag
            var tag = new byte[]
            {
                92, 80, 104, 49, 133, 25, 161,
                215, 173, 101, 219, 211, 136, 91,
                210, 145
            };

            #endregion

            #region コード

            AeadA256Gcm aesGcm = null;
            AeadResult aesRet = null;

            aesGcm = new AeadA256Gcm(cek, iv, aad);
            aesGcm.Encrypt(plaint);
            aesRet = aesGcm.Result;

            if (CustomEncode.ToBase64String(tag) == CustomEncode.ToBase64String(aesRet.Tag))
            {
                Debug.WriteLine("tag is ok!");
            }

            if (CustomEncode.ToBase64String(ciphert) == CustomEncode.ToBase64String(aesRet.Ciphert))
            {
                Debug.WriteLine("ciphert is ok!");
            }

            //// Aeadをnullクリアすると、
            //// ciphert + tag からAeadを計算する（ただの結合）。
            //aesRet.Aead = null;

            // 再初期化する・しない（しなくてもイイ、使いまわし可能）。
            aesGcm = new AeadA256Gcm(cek, iv, aad);

            if (CustomEncode.ToBase64String(plaint) == CustomEncode.ToBase64String(aesGcm.Decrypt(aesRet)))
            {
                Debug.WriteLine("plaint is ok!");
            }

            #endregion
        }

        #endregion

        #region JWS

        /// <summary>JWS生成</summary>
        private void btnJWSSign_Click(object sender, EventArgs e)
        {
            if (rbnJWSHS256.Checked)
            {
                // HS256
                string password = GetPassword.Generate(20, 10);
                JWS_HS256 jwsHS256 = new JWS_HS256(CustomEncode.StringToByte(password, CustomEncode.UTF_8));

                // 生成
                string jws = jwsHS256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = password;
                this.txtJWSJWK.Text = jwsHS256.JWK;

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
            else if (rbnJWSRS256_XML.Checked)
            {
                // RS256 (XML)
                JWS_RS256_XML jwsRS256 = new JWS_RS256_XML();

                // 生成
                string jws = jwsRS256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = jwsRS256.XMLPublicKey;
                this.txtJWSJWK.Text = RsaPublicKeyConverter.ParamToJwk(
                        RsaPublicKeyConverter.XmlToProvider(jwsRS256.XMLPublicKey).ExportParameters(false));

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
            else if (rbnJWSRS256_Param.Checked)
            {
                // RS256 (Param)
                JWS_RS256_Param jwsRS256 = new JWS_RS256_Param();

                // 生成
                string jws = jwsRS256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = RsaPublicKeyConverter.ParamToXml(jwsRS256.RsaPublicParameters);
                this.txtJWSJWK.Text = RsaPublicKeyConverter.ParamToJwk(jwsRS256.RsaPublicParameters);

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
            else if (rbnJWSRS256_X509.Checked)
            {
                // RS256 (X509)
                JWS_RS256_X509 jwsRS256 = new JWS_RS256_X509(this.SHA256RSA_pfx, this.CertificateFilePassword,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                // 生成
                string jws = jwsRS256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = jwsRS256.DigitalSignX509.PublicKey.ToXmlString(false);
                this.txtJWSJWK.Text = RsaPublicKeyConverter.ParamToJwk(
                        RsaPublicKeyConverter.X509CerToProvider(this.SHA256RSA_cer).ExportParameters(false));

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
#if NET45 || NET46
#else
            else if (rbnJWSES256_Param.Checked)
            {
                // ES256 (Param)
                JWS_ES256_Param jwsES256 = new JWS_ES256_Param();

                // 生成
                string jws = jwsES256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = jwsES256.ECDsaPublicParameters.GetType().ToString();
                this.txtJWSJWK.Text = EccPublicKeyConverter.ParamToJwk(jwsES256.ECDsaPublicParameters);

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
            else if (rbnJWSES256_X509.Checked)
            {
                // ES256 (X509)
                JWS_ES256_X509 jwsES256 = new JWS_ES256_X509(this.SHA256ECDSA_pfx, this.CertificateFilePassword);

                // 生成
                string jws = jwsES256.Create(this.txtJWSPayload.Text);

                // 出力
                this.txtJWSKey.Text = jwsES256.DigitalSignECDsaX509.PublicKey.GetType().ToString();
                this.txtJWSJWK.Text = EccPublicKeyConverter.ParamToJwk(jwsES256.DigitalSignECDsaX509.PublicKey.ExportParameters(false));

                this.txtJWSSigned.Text = jws;

                // 改竄可能なフィールドに出力
                string[] temp = jws.Split('.');
                this.txtJWSHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWSPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
#endif
        }

        /// <summary>JWS検証</summary>
        private void btnJWSVerify_Click(object sender, EventArgs e)
        {
            bool ret = false;

            if (rbnJWSHS256.Checked)
            {
                // HS256

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                //JWS_HS256 jwsHS256 = new JWS_HS256(CustomEncode.StringToByte(this.txtJWSKey.Text, CustomEncode.UTF_8));
                JWS_HS256 jwsHS256 = new JWS_HS256(this.txtJWSJWK.Text);
                ret = jwsHS256.Verify(newJWS);
            }
            else if (rbnJWSRS256_XML.Checked)
            {
                // RS256 (XML)

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWS_RS256_XML jwsRS256 = new JWS_RS256_XML(this.txtJWSKey.Text);
                ret = jwsRS256.Verify(newJWS);
            }
            else if (rbnJWSRS256_Param.Checked)
            {
                // RS256 (Param)

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                //JWS_RS256_Param jwsRS256 = new JWS_RS256_Param(
                //    RsaPublicKeyConverter.XmlToProvider(this.txtJWSKey.Text).ExportParameters(false));
                JWS_RS256_Param jwsRS256 = new JWS_RS256_Param(
                    RsaPublicKeyConverter.JwkToProvider(this.txtJWSJWK.Text).ExportParameters(false));
                ret = jwsRS256.Verify(newJWS);
            }
            else if (rbnJWSRS256_X509.Checked)
            {
                // RS256 (X509)

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWS_RS256_X509 jwsRS256 = new JWS_RS256_X509(this.SHA256RSA_cer, "");
                ret = jwsRS256.Verify(newJWS);
            }
#if NET45 || NET46
#else
            else if (rbnJWSES256_Param.Checked)
            {
                // ES256 (Param)

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWS_ES256_Param jwsES256 = new JWS_ES256_Param(EccPublicKeyConverter.JwkToParam(this.txtJWSJWK.Text), false);
                ret = jwsES256.Verify(newJWS);
            }
            else if (rbnJWSES256_X509.Checked)
            {
                // ES256 (X509)

                // 入力
                string[] temp = this.txtJWSSigned.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWS =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWSPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWS_ES256_X509 jwsES256 = new JWS_ES256_X509(this.SHA256ECDSA_cer, "");
                ret = jwsES256.Verify(newJWS);
            }
#endif

            if (ret)
            {
                MessageBox.Show("検証成功");
            }
            else
            {
                MessageBox.Show("検証失敗");
            }
        }

        #endregion

        #region JWE

        /// <summary>JWE生成</summary>
        private void btnJWEEncrypt_Click(object sender, EventArgs e)
        {
            string jweString = "";
            JWE jwe = null;

            if (rbnJWEOAEP_Param.Checked)
            {
                // RSAES-OAEP and AES GCM (Param)
                jwe = new JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm.RsaCsp);

                // 生成
                jweString = jwe.Create(this.txtJWEPayload.Text);
                this.txtJWEEncrypted.Text = jweString;

                // JWK
                this.txtJWEXMLKey.Text = jwe.ASymmetricCryptography.PrivateXmlKey;
            }
            else if (rbnJWEOAEP_X509.Checked)
            {
                // RSAES-OAEP and AES GCM (X509)
                jwe = new JWE_RsaOaepAesGcm_X509(this.SHA256RSA_cer, "");

                // 生成
                jweString = jwe.Create(this.txtJWEPayload.Text);
                this.txtJWEEncrypted.Text = jweString;

                // JWK
                this.txtJWEXMLKey.Text = jwe.ASymmetricCryptography.PublicXmlKey;
            }
            else if (rbnJWERSA15_Param.Checked)
            {
                // RSA1_5 and A128CBC-HS256 (Param)
                jwe = new JWE_Rsa15A128CbcHS256_Param(EnumASymmetricAlgorithm.RsaCsp);

                // 生成
                jweString = jwe.Create(this.txtJWEPayload.Text);
                this.txtJWEEncrypted.Text = jweString;

                // JWK
                this.txtJWEXMLKey.Text = jwe.ASymmetricCryptography.PrivateXmlKey;
            }
            else if (rbnJWERSA15_X509.Checked)
            {
                // RSA1_5 and A128CBC-HS256 (X509)
                jwe = new JWE_Rsa15A128CbcHS256_X509(this.SHA256RSA_cer, "");

                // 生成
                jweString = jwe.Create(this.txtJWEPayload.Text);
                this.txtJWEEncrypted.Text = jweString;

                // JWK
                this.txtJWEXMLKey.Text = jwe.ASymmetricCryptography.PublicXmlKey;
            }

            // フィールドに出力
            string[] temp = jweString.Split('.');
            // ヘッダー
            this.txtJWEHeader.Text = CustomEncode.ByteToString(
                CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
            // コンテンツ暗号化キー（CEK）
            this.txtJWECek.Text = temp[1]; // 元々ByteなのでBase64Urlのまま出す。
            // 初期化ベクトル
            this.txtJWEIV.Text = temp[2];  // 元々ByteなのでBase64Urlのまま出す。
            // 追加認証データ（AAD）
            this.txtJWEAAD.Text = temp[0]; // ≒ヘッダー（をASCIIバイト化しBase64Url）
            // ペイロード
            this.txtJWEAAD.Text = temp[3]; // 認証付き暗号（AEAD）による暗号化
            // 認証タグ（MAC）
            this.txtJWEMAC.Text = temp[4];  // 元々ByteなのでBase64Urlのまま出す。
        }

        /// <summary>JWE復号</summary>
        private void btnJWEDecrypt_Click(object sender, EventArgs e)
        {
            bool ret = false;
            string payload = "";
            JWE jwe = null;

            if (rbnJWEOAEP_Param.Checked)
            {
                // RSAES-OAEP and AES GCM (Param)
                RSA rsa = RSA.Create();
                rsa.FromXmlString(this.txtJWEXMLKey.Text);
                jwe = new JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm.RsaCsp, rsa.ExportParameters(true));

                // 復号
                ret = jwe.Decrypt(this.txtJWEEncrypted.Text, out payload);
            }
            else if (rbnJWEOAEP_X509.Checked)
            {
                // RSAES-OAEP and AES GCM (X509)
                jwe = new JWE_RsaOaepAesGcm_X509(this.SHA256RSA_pfx, this.CertificateFilePassword);

                // 復号
                ret = jwe.Decrypt(this.txtJWEEncrypted.Text, out payload);
            }
            else if (rbnJWERSA15_Param.Checked)
            {
                // RSA1_5 and A128CBC-HS256 (Param)
                RSA rsa = RSA.Create();
                rsa.FromXmlString(this.txtJWEXMLKey.Text);
                jwe = new JWE_Rsa15A128CbcHS256_Param(EnumASymmetricAlgorithm.RsaCsp, rsa.ExportParameters(true));

                // 復号
                ret = jwe.Decrypt(this.txtJWEEncrypted.Text, out payload);
            }
            else if (rbnJWERSA15_X509.Checked)
            {
                // RSA1_5 and A128CBC-HS256 (X509)
                jwe = new JWE_Rsa15A128CbcHS256_X509(this.SHA256RSA_pfx, this.CertificateFilePassword);

                // 復号
                ret = jwe.Decrypt(this.txtJWEEncrypted.Text, out payload);
            }

            this.txtJWEPayload.Text = payload;

            if (ret)
            {
                MessageBox.Show("復号成功");
            }
            else
            {
                MessageBox.Show("復号失敗");
            }
        }

        #endregion

        #endregion
    }
}
