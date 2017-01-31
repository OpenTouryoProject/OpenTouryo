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
//*  2017/01/13  西野 大介         追加のGetSaltedPasswordメソッド、CodeSigning、JWTクラスの検証画面
//**********************************************************************************

using System;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Util.JWT;

namespace EncAndDecUtil
{
    public partial class Form1 : Form
    {
        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>Form_Load</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            cbxHSPV.DataSource = Enum.GetValues(typeof(EnumHashAlgorithm));
            cbxKHSPV.DataSource = Enum.GetValues(typeof(EnumKeyedHashAlgorithm));
            cbxSCPV.DataSource = Enum.GetValues(typeof(EnumSymmetricAlgorithm));
            cbxSPWDPV1.DataSource = Enum.GetValues(typeof(EnumHashAlgorithm));
            cbxSPWDPV2.DataSource = Enum.GetValues(typeof(EnumKeyedHashAlgorithm));
            cbxCCXMLPV.DataSource = Enum.GetValues(typeof(EnumDigitalSignAlgorithm));
        }
        
        #endregion

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
            if (string.IsNullOrEmpty(txtKHSSalt.Text))
            {
                // ソルト無し
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
                                this.txtKHSPassword.Text));
                }
            }
            else
            {
                // ソルト有り
                if (this.nudKHSStretching.Value == 0)
                {
                    // ストレッチング無し
                    if (this.rbnKHSString.Checked)
                    {
                        // String
                        this.txtKHSCode.Text =
                            GetKeyedHash.GetKeyedHashString(
                                this.txtKHSString.Text,
                                (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                                this.txtKHSPassword.Text,
                                CustomEncode.StringToByte(this.txtKHSSalt.Text, CustomEncode.UTF_8));
                    }
                    else
                    {
                        // Bytes
                        this.txtKHSCode.Text =
                            CustomEncode.ToHexString(
                                GetKeyedHash.GetKeyedHashBytes(
                                    CustomEncode.StringToByte(txtKHSString.Text, CustomEncode.UTF_8),
                                    (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                                    this.txtKHSPassword.Text,
                                    CustomEncode.StringToByte(this.txtKHSSalt.Text, CustomEncode.UTF_8)));
                    }
                }
                else
                {
                    // ストレッチング有り
                    if (this.rbnKHSString.Checked)
                    {
                        // String
                        this.txtKHSCode.Text =
                            GetKeyedHash.GetKeyedHashString(
                                this.txtKHSString.Text,
                                (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                                this.txtKHSPassword.Text,
                                CustomEncode.StringToByte(this.txtKHSSalt.Text, CustomEncode.UTF_8),
                                (int)nudKHSStretching.Value);
                    }
                    else
                    {
                        // Bytes
                        this.txtKHSCode.Text =
                            CustomEncode.ToHexString(
                                GetKeyedHash.GetKeyedHashBytes(
                                    CustomEncode.StringToByte(txtKHSString.Text, CustomEncode.UTF_8),
                                    (EnumKeyedHashAlgorithm)cbxKHSPV.SelectedValue,
                                    this.txtKHSPassword.Text,
                                    CustomEncode.StringToByte(this.txtKHSSalt.Text, CustomEncode.UTF_8),
                                    (int)nudKHSStretching.Value));
                    }
                }
            }
        }

        #endregion

        #region 塩味パスワード生成（ハッシュ）

        /// <summary>塩味パスワード生成</summary>
        private void btnSPWDGen1_Click(object sender, EventArgs e)
        {
            this.txtSPWDSaltedPassword1.Text = GetHash.GetSaltedPassword(
                this.txtSPWDRawPassword1.Text, (EnumHashAlgorithm)this.cbxSPWDPV1.SelectedValue,
                (int)this.nudSPWDSaltLength1.Value, (int)this.nudSPWDStretchCount1.Value);
        }

        /// <summary>生パスワードと塩味パスワードを比較認証</summary>
        private void btnSPWDAuth1_Click(object sender, EventArgs e)
        {
            // パラメタ系は渡さないで検証可能
            if (GetHash.EqualSaltedPassword(
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

        private void btnSPWDGen2_Click(object sender, EventArgs e)
        {
            this.txtSPWDSaltedPassword2.Text = GetKeyedHash.GetSaltedPassword(
                this.txtSPWDRawPassword2.Text, (EnumKeyedHashAlgorithm)this.cbxSPWDPV2.SelectedValue,
                this.txtSPWDKey2.Text, (int)this.nudSPWDSaltLength2.Value, (int)this.nudSPWDStretchCount2.Value);
        }

        private void btnSPWDAuth2_Click(object sender, EventArgs e)
        {
            // パラメタ系は渡さないで検証可能
            if (GetKeyedHash.EqualSaltedPassword(
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

        #region 秘密鍵

        /// <summary>秘密鍵・暗号化</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSCSalt.Text))
            {
                // ソルト無し
                if (this.rbnSCString.Checked)
                {
                    // String
                    this.txtSCCode.Text =
                        SymmetricCryptography.EncryptString(
                            this.txtSCString.Text,
                            this.txtSCPassword.Text,
                            (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue);
                }
                else
                {
                    // Bytes
                    this.txtSCCode.Text =
                        CustomEncode.ToHexString(
                            SymmetricCryptography.EncryptBytes(
                                CustomEncode.StringToByte(txtSCString.Text, CustomEncode.UTF_8),
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue));
                }
            }
            else
            {
                // ソルト有り
                if (this.nudSCStretching.Value == 0)
                {
                    // ストレッチング無し
                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCCode.Text =
                            SymmetricCryptography.EncryptString(
                                this.txtSCString.Text,
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8));
                    }
                    else
                    {
                        // Bytes
                        this.txtSCCode.Text =
                            CustomEncode.ToHexString(
                                SymmetricCryptography.EncryptBytes(
                                    CustomEncode.StringToByte(txtSCString.Text, CustomEncode.UTF_8),
                                    this.txtSCPassword.Text,
                                    (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                    CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8)));
                    }
                }
                else
                {
                    // ストレッチング有り
                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCCode.Text
                            = SymmetricCryptography.EncryptString(
                                this.txtSCString.Text,
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                                (int)this.nudSCStretching.Value);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCCode.Text =
                            CustomEncode.ToHexString(
                                SymmetricCryptography.EncryptBytes(
                                    CustomEncode.StringToByte(txtSCString.Text, CustomEncode.UTF_8),
                                    this.txtSCPassword.Text,
                                    (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                    CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                                    (int)this.nudSCStretching.Value));
                    }
                }
            }
        }

        /// <summary>秘密鍵・復号化</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSCSalt.Text))
            {
                // ソルト無し
                if (this.rbnSCString.Checked)
                {
                    // String
                    this.txtSCString.Text =
                        SymmetricCryptography.DecryptString(
                            this.txtSCCode.Text,
                            this.txtSCPassword.Text,
                            (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue);
                }
                else
                {
                    // Bytes
                    this.txtSCString.Text =
                         CustomEncode.ByteToString(
                            SymmetricCryptography.DecryptBytes(
                                CustomEncode.FormHexString(this.txtSCCode.Text),
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue),
                            CustomEncode.UTF_8);
                }
            }
            else
            {
                // ソルト有り
                if (this.nudSCStretching.Value == 0)
                {
                    // ストレッチング無し
                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCString.Text
                            = SymmetricCryptography.DecryptString(
                                this.txtSCCode.Text,
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8));
                    }
                    else
                    {
                        // Bytes
                        this.txtSCString.Text =
                         CustomEncode.ByteToString(
                            SymmetricCryptography.DecryptBytes(
                                CustomEncode.FormHexString(this.txtSCCode.Text),
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8)),
                            CustomEncode.UTF_8);
                    }
                }
                else
                {
                    // ストレッチング有り
                    if (this.rbnSCString.Checked)
                    {
                        // String
                        this.txtSCString.Text
                            = SymmetricCryptography.DecryptString(
                                this.txtSCCode.Text,
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                                (int)this.nudSCStretching.Value);
                    }
                    else
                    {
                        // Bytes
                        this.txtSCString.Text =
                         CustomEncode.ByteToString(
                            SymmetricCryptography.DecryptBytes(
                                CustomEncode.FormHexString(this.txtSCCode.Text),
                                this.txtSCPassword.Text,
                                (EnumSymmetricAlgorithm)cbxSCPV.SelectedValue,
                                CustomEncode.StringToByte(txtSCSalt.Text, CustomEncode.UTF_8),
                                (int)this.nudSCStretching.Value),
                            CustomEncode.UTF_8);
                    }
                }
            }
        }

        #endregion

        #region 共通鍵

        /// <summary>鍵取得</summary>
        private void button3_Click(object sender, EventArgs e)
        {
            string publicKey = "";
            string privateKey = "";
            ASymmetricCryptography.GetKeys(out publicKey, out privateKey);

            this.txtASCPublic.Text = publicKey;
            this.txtASCPrivate.Text = privateKey;
        }

        /// <summary>共通鍵・暗号化</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.rbnASCString.Checked)
            {
                // String
                this.txtASCCode.Text =
                    ASymmetricCryptography.EncryptString(this.txtASCString.Text, this.txtASCPublic.Text);
            }
            else
            {
                // Bytes
                this.txtASCCode.Text =
                    CustomEncode.ToHexString(
                        ASymmetricCryptography.EncryptBytes(
                            CustomEncode.StringToByte(this.txtASCString.Text, CustomEncode.UTF_8), this.txtASCPublic.Text));
            }
        }

        /// <summary>共通鍵・復号化</summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.rbnASCString.Checked)
            {
                // String
                this.txtASCString.Text =
                    ASymmetricCryptography.DecryptString(this.txtASCCode.Text, this.txtASCPrivate.Text);
            }
            else
            {
                // Bytes
                this.txtASCString.Text =
                    CustomEncode.ByteToString(
                        ASymmetricCryptography.DecryptBytes(
                            CustomEncode.FormHexString(this.txtASCCode.Text), this.txtASCPrivate.Text),
                        CustomEncode.UTF_8);
            }
        }

        #endregion

        #region 証明書

        // ココの署名・検証処理を実行するには、ClickOnce署名機能などを使用し予め、
        // *.pfx 証明書を、password = "test" などとして、生成しておく必要があります。
        // 検証プロセスでは、*.pfx 証明書を使用することも出来ますが、通常は、秘密鍵無しで、
        // エクスポートした *.cer 証明証を使用します。この場合、パスワードは不要になります。
        // *.cer 証明証に証明書チェーンが無ければ、WindowsOSの証明書ストア（リポジトリ）である
        // 「信頼されたルート証明機関」にインストールする必要があります（実行アカウントにも注意が必要） 。

        /// <summary>CertificateFilePath</summary>
        private string CertificateFilePath_pfx = @"..\..\EncAndDecUtil_RS256.pfx";
        private string CertificateFilePath_cer = @"..\..\EncAndDecUtil_RS256.cer";
        private string CertificateFilePassword = "test";

        #endregion

        #region 署名

        /// <summary>rbnCCXML_CheckedChanged</summary>
        private void rbnCCXML_CheckedChanged(object sender, EventArgs e)
        {
            this.cbxCCXMLPV.Enabled = this.rbnCCXML.Checked;
        }

        /// <summary>rbnCCX509_CheckedChanged</summary>
        private void rbnCCX509_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCCHash.ReadOnly = !this.rbnCCX509.Checked;
        }

        /// <summary>署名</summary>
        private void btnCCSign_Click(object sender, EventArgs e)
        {
            DigitalSignXML csXML = null;
            DigitalSignX509 csX509 = null;

            byte[] data = CustomEncode.StringToByte(this.txtCCData.Text, CustomEncode.UTF_8);
            byte[] sign = null;
            //bool ret = false;

            if (rbnCCXML.Checked)
            {
                // XMLKey
                csXML = new DigitalSignXML((EnumDigitalSignAlgorithm)this.cbxCCXMLPV.SelectedValue);
                sign = csXML.Sign(data);
                //ret = csXML.Verify(data, sign);

                txtCCPrivateKey.Text = csXML.XMLPrivateKey;
                txtCCPublicKey.Text = csXML.XMLPublicKey;
            }
            else
            {
                // X509Cer
                csX509 = new DigitalSignX509(this.CertificateFilePath_pfx, this.CertificateFilePassword, this.txtCCHash.Text);

                sign = csX509.Sign(data);
                //ret = csX509.Verify(data, sign);

                txtCCPrivateKey.Text = csX509.X509PrivateKey;
                txtCCPublicKey.Text = csX509.X509PublicKey;
            }

            txtCCSign.Text = CustomEncode.ToBase64String(sign);
        }

        /// <summary>検証</summary>
        private void btnCCVerify_Click(object sender, EventArgs e)
        {
            DigitalSignXML csXML = null;
            DigitalSignX509 csX509 = null;

            byte[] data = CustomEncode.StringToByte(this.txtCCData.Text, CustomEncode.UTF_8);
            byte[] sign = CustomEncode.FromBase64String(this.txtCCSign.Text);
            bool ret = false;

            if (rbnCCXML.Checked)
            {
                // XMLKey
                csXML = new DigitalSignXML((EnumDigitalSignAlgorithm)this.cbxCCXMLPV.SelectedValue);
                csXML.XMLPublicKey = txtCCPublicKey.Text;
                ret = csXML.Verify(data, sign);
            }
            else
            {
                // X509Cer
                // *.pfxを使用して、検証することもできるが、
                //csX509 = new CodeSigningX509(this.CertificateFilePath_pfx, this.CertificateFilePassword, this.txtCCHash.Text);
                // 通常は、*.cerを使用して検証する。
                csX509 = new DigitalSignX509(CertificateFilePath_cer, "", this.txtCCHash.Text);

                ret = csX509.Verify(data, sign);
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

        #region JWT

        /// <summary>JWT生成</summary>
        private void btnJWTSign_Click(object sender, EventArgs e)
        {
            if (rbnJWTHS256.Checked)
            {
                // HS256
                string password = GetPassword.Generate(20, 10);
                JWT_HS256 jwtHS256 = new JWT_HS256(CustomEncode.StringToByte(password, CustomEncode.UTF_8));

                // 生成
                string jwt = jwtHS256.Create(this.txtJWTPayload.Text);

                // 出力
                this.txtJWTKey.Text = password;
                this.txtJWTJWK.Text = jwtHS256.JWK;
                this.txtJWTSign.Text = jwt;

                // 改竄可能なフィールドに出力
                string[] temp = jwt.Split('.');
                this.txtJWTHeader.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8);
                this.txtJWTPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
            else
            {
                // RS256 (X509Cer)
                JWT_RS256 jwtRS256 = new JWT_RS256(this.CertificateFilePath_pfx, this.CertificateFilePassword);

                // 生成
                string jwt = jwtRS256.Create(this.txtJWTPayload.Text);

                // 出力
                this.txtJWTSign.Text = jwt;

                // 改竄可能なフィールドに出力
                string[] temp = jwt.Split('.');
                this.txtJWTHeader.Text =  CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[0]),CustomEncode.UTF_8);
                this.txtJWTPayload.Text = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
            }
        }

        /// <summary>JWT検証</summary>
        private void btnJWTVerify_Click(object sender, EventArgs e)
        {

            bool ret = false;

            if (rbnJWTHS256.Checked)
            {
                // HS256
                
                // 入力
                string[] temp = this.txtJWTSign.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWT =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWTHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWTPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWT_HS256 jwtHS256 = new JWT_HS256(CustomEncode.StringToByte(this.txtJWTKey.Text, CustomEncode.UTF_8));
                ret = jwtHS256.Verify(newJWT);
            }
            else
            {
                // RS256 (X509Cer)
                
                // 入力
                string[] temp = this.txtJWTSign.Text.Split('.');

                // 改変可能なフィールドから入力
                string newJWT =
                    CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWTHeader.Text, CustomEncode.UTF_8))
                    + "." + CustomEncode.ToBase64UrlString(CustomEncode.StringToByte(this.txtJWTPayload.Text, CustomEncode.UTF_8))
                    + "." + temp[2];

                // 検証
                JWT_RS256 jwtRS256 = new JWT_RS256(this.CertificateFilePath_cer, "");
                ret = jwtRS256.Verify(newJWT);
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
    }
}
