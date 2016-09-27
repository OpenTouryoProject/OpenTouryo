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
//*  2013/02/12  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Text;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Str;

namespace EncAndDecUtil
{
    public partial class Form1 : Form
    {
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
            cbxSPWDPV.DataSource = Enum.GetValues(typeof(EnumHashAlgorithm));
        }

        /// <summary>ハッシュ</summary>
        private void btnGetHash_Click(object sender, EventArgs e)
        {
            if (this.rbnHSString.Checked)
            {
                txtHSCode.Text = GetHash.GetHashString(
                    txtHSString.Text, (EnumHashAlgorithm)cbxHSPV.SelectedValue);
            }
            else
            {
                txtHSCode.Text = CustomEncode.ToHexString(GetHash.GetHashBytes(
                    CustomEncode.StringToByte(txtHSString.Text, CustomEncode.UTF_8), (EnumHashAlgorithm)cbxHSPV.SelectedValue));
            }
        }

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
                                CustomEncode.StringToByte(this.txtKHSPassword.Text, CustomEncode.UTF_8));
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
                                    CustomEncode.StringToByte(this.txtKHSPassword.Text, CustomEncode.UTF_8)));
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
                                CustomEncode.StringToByte(this.txtKHSPassword.Text, CustomEncode.UTF_8),
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
                                    CustomEncode.StringToByte(this.txtKHSPassword.Text, CustomEncode.UTF_8),
                                    (int)nudKHSStretching.Value));
                    }
                }
            }
        }

        /// <summary>塩味パスワード生成</summary>
        private void btnSPWDGen_Click(object sender, EventArgs e)
        {
            this.txtSPWDSaltedPassword.Text = GetHash.GetSaltedPasswd(
                this.txtSPWDRawPassword.Text, (EnumHashAlgorithm)this.cbxSPWDPV.SelectedValue, (int)this.nudSPWDSaltLength.Value);
        }

        /// <summary>生パスワードと塩味パスワードを比較認証</summary>
        private void btnSPWDAuth_Click(object sender, EventArgs e)
        {
            if (GetHash.EqualSaltedPasswd(
                this.txtSPWDRawPassword.Text, this.txtSPWDSaltedPassword.Text,
                (EnumHashAlgorithm)this.cbxSPWDPV.SelectedValue, (int)this.nudSPWDSaltLength.Value))
            {
                MessageBox.Show("認証成功");
            }
            else
            {
                MessageBox.Show("認証失敗");

            }
        }

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

            this.txtASCPrivate.Text = publicKey;
            this.txtASCPublic.Text = privateKey;
        }

        /// <summary>共通鍵・暗号化</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.rbnASCString.Checked)
            {
                // String
                this.txtASCCode.Text = 
                    ASymmetricCryptography.EncryptString(this.txtASCString.Text, this.txtASCPrivate.Text);
            }
            else
            {
                // Bytes
                this.txtASCCode.Text =
                    CustomEncode.ToHexString(
                        ASymmetricCryptography.EncryptBytes(
                            CustomEncode.StringToByte(this.txtASCString.Text, CustomEncode.UTF_8),
                            this.txtASCPrivate.Text));
            }
        }

        /// <summary>共通鍵・復号化</summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.rbnASCString.Checked)
            {
                // String
                this.txtASCString.Text =
                    ASymmetricCryptography.DecryptString(this.txtASCCode.Text, this.txtASCPublic.Text);
            }
            else
            {
                // Bytes
                this.txtASCString.Text =
                    CustomEncode.ByteToString(
                        ASymmetricCryptography.DecryptBytes(
                            CustomEncode.FormHexString(this.txtASCCode.Text),
                            this.txtASCPublic.Text),
                        CustomEncode.UTF_8);
            }
        }

        #endregion
    }
}
