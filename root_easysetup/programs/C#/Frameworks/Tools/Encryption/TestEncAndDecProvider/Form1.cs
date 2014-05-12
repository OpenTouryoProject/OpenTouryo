//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス日本語名  ：暗号化/復号化テスト・ツール（メイン画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/12  西野  大介        新規作成
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace TestEncAndDecProvider
{
    /// <summary>暗号化/復号化テスト・ツール（メイン画面）</summary>
    public partial class Form1 : Form
    {
        #region 初期化

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>Form_Load</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;
        }

        /// <summary>コンボ初期化</summary>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;
        }
        
        #endregion

        #region ハッシュ（キー無し）

        #region プロバイダ生成

        /// <summary>ハッシュ（キー無し）サービスプロバイダの生成</summary>
        /// <returns>ハッシュ（キー無し）サービスプロバイダ</returns>
        private HashAlgorithm CreateHashAlgorithmServiceProvider()
        {
            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm ha = null;

            if (this.comboBox1.SelectedItem.ToString() == "既定のプロバイダ")
            {
                // 既定の暗号化サービスプロバイダ
                ha = HashAlgorithm.Create();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "MD5CryptoServiceProvider")
            {
                // MD5CryptoServiceProviderサービスプロバイダ
                ha = new MD5CryptoServiceProvider();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA1CryptoServiceProvider")
            {
                // SHA1CryptoServiceProviderサービスプロバイダ
                ha = new SHA1CryptoServiceProvider();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA1Managed")
            {
                // SHA1Managedサービスプロバイダ
                ha = new SHA1Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA256Managed")
            {
                // SHA256Managedサービスプロバイダ
                ha = new SHA256Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA384Managed")
            {
                // SHA384Managedサービスプロバイダ
                ha = new SHA384Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA512Managed")
            {
                // SHA512Managedサービスプロバイダ
                ha = new SHA512Managed();
            }

            return ha;
        }

        #endregion

        #region プロバイダ説明

        /// <summary>ハッシュ（キー無し）サービスプロバイダの説明</summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm ha = this.CreateHashAlgorithmServiceProvider();

            if (ha is MD5CryptoServiceProvider)
            {
                this.textBox15.Text = "MD5 ハッシュ";
                this.textBox16.Text = "ハッシュ サイズは 16 バイトです。";   
            }
            else if (ha is SHA1CryptoServiceProvider)
            {
                this.textBox15.Text = "SHA1 ハッシュ";
                this.textBox16.Text = "ハッシュ サイズは 20 バイトです。";
            }
            else if (ha is SHA1Managed)
            {
                this.textBox15.Text = "SHA1 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 20 バイトです。";
            }
            else if (ha is SHA256Managed)
            {
                this.textBox15.Text = "SHA256 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 32 バイトです。";
            }
            else if (ha is SHA384Managed)
            {
                this.textBox15.Text = "SHA384 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 48 バイトです。";
            }
            else if (ha is SHA512Managed)
            {
                this.textBox15.Text = "SHA512 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 64 バイトです。";
            }

            // ハッシュ（キー無し）サービスプロバイダの各プロパティを出力
            if (ha != null)
            {
                this.textBox1.Text = "";
                
                this.textBox1.Text += "・HashSize:" + ha.HashSize.ToString() + "\r\n";
                this.textBox1.Text += "・InputBlockSize:" + ha.InputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・OutputBlockSize:" + ha.OutputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・CanReuseTransform:" + ha.CanReuseTransform.ToString() + "\r\n";
                this.textBox1.Text += "・CanTransformMultipleBlocks:" + ha.CanTransformMultipleBlocks.ToString() + "\r\n";
            }
        }

        #endregion

        #region ハッシュ化

        /// <summary>ハッシュ化</summary>
        private void button11_Click(object sender, EventArgs e)
        {
            this.textBox12.Text = "";
            this.textBox13.Text = "";

            if (this.textBox11.Text == "")
            {
                return;
            }

            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm ha = this.CreateHashAlgorithmServiceProvider();

            // 元文字列
            string ss = this.textBox11.Text;

            //元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] asb = Encoding.UTF8.GetBytes(ss);

            // ハッシュ値を計算する
            byte[] hb = ha.ComputeHash(asb);

            //結果を表示

            // 生バイト
            this.textBox12.Text = CustomEncode.ToHexString(hb);
            // Base64
            this.textBox13.Text = Convert.ToBase64String(hb);
        }

        #endregion

        #endregion

        #region ハッシュ（キー付き）

        #region プロバイダ生成

        /// <summary>ハッシュ（キー付き）サービスプロバイダの生成</summary>
        /// <returns>ハッシュ（キー付き）サービスプロバイダ</returns>
        private KeyedHashAlgorithm CreateKeyedHashAlgorithmServiceProvider()
        {
            // ハッシュ（キー付き）サービスプロバイダ
            KeyedHashAlgorithm kha = null;

            if (this.comboBox2.SelectedItem.ToString() == "既定のプロバイダ")
            {
                // 既定の暗号化サービスプロバイダ
                kha = KeyedHashAlgorithm.Create();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACSHA1")
            {
                // HMACSHA1サービスプロバイダ
                kha = new HMACSHA1();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "MACTripleDES")
            {
                // MACTripleDESサービスプロバイダ
                kha = new MACTripleDES();
            }

           return kha;
        }

        #endregion

        #region プロバイダの説明

        /// <summary>ハッシュ（キー付き）サービスプロバイダの説明</summary>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ハッシュ（キー付き）サービスプロバイダ
            KeyedHashAlgorithm kha = this.CreateKeyedHashAlgorithmServiceProvider();

            if (kha is HMACSHA1)
            {
                this.textBox25.Text = "SHA1 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 20 バイトのハッシュ シーケンスを生成します。";
            }
            else if (kha is MACTripleDES)
            {
                this.textBox25.Text = "TripleDES を使用して、メッセージ認証コード（MAC）を計算します。";
                this.textBox26.Text = "長さが 16 または 24 バイトのキーを使用し、長さが 8 バイトのハッシュ シーケンスを生成します。";
            }

            // ハッシュ（キー付き）サービスプロバイダの各プロパティを出力
            if (kha != null)
            {
                this.textBox1.Text = "";

                this.textBox1.Text += "・HashSize:" + kha.HashSize.ToString() + "\r\n";
                this.textBox1.Text += "・InputBlockSize:" + kha.InputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・OutputBlockSize:" + kha.OutputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・CanReuseTransform:" + kha.CanReuseTransform.ToString() + "\r\n";
                this.textBox1.Text += "・CanTransformMultipleBlocks:" + kha.CanTransformMultipleBlocks.ToString() + "\r\n";
            }
        }

        #endregion

        #region ハッシュ化

        /// <summary>ハッシュ化</summary>
        private void button21_Click(object sender, EventArgs e)
        {
            this.textBox22.Text = "";
            this.textBox23.Text = "";

            if (this.textBox21a.Text == "" || this.textBox21b.Text == "")
            {
                return;
            }

            // ハッシュ（キー付き）サービスプロバイダ
            KeyedHashAlgorithm kha = this.CreateKeyedHashAlgorithmServiceProvider();

            // 元文字列
            string ss = this.textBox21a.Text;

            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] asb = Encoding.UTF8.GetBytes(ss);

            // キー文字列
            string ks = this.textBox21b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] akb = Encoding.UTF8.GetBytes(ks);

            // ハッシュ値を計算する
            if (kha is HMACSHA1)
            {
                // どのサイズのキーでも受け入れる
                kha.Key = akb;
            }
            else if (kha is MACTripleDES)
            {
                // 長さが 16 または 24 バイトのキーを受け入れる
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    kha.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else
                {
                    // 24バイトに切り詰め
                    kha.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
            }

            byte[] hb = kha.ComputeHash(asb);

            //結果を表示

            // 生バイト
            this.textBox22.Text = CustomEncode.ToHexString(hb);
            // Base64
            this.textBox23.Text = Convert.ToBase64String(hb);
        }

        #endregion

        #endregion

        #region 秘密鍵・暗号化

        #region プロバイダ生成

        /// <summary>秘密鍵・暗号化サービスプロバイダの生成</summary>
        /// <returns>秘密鍵・暗号化サービスプロバイダ</returns>
        private SymmetricAlgorithm CreateSymmetricAlgorithmServiceProvider()
        {
            // 秘密鍵・暗号化サービスプロバイダ
            SymmetricAlgorithm sa = null;

            if (this.comboBox3.SelectedItem.ToString() == "AesCryptoServiceProvider")
            {
                // AesCryptoServiceProviderサービスプロバイダ
                sa = new AesCryptoServiceProvider();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "AesManaged")
            {
                // AesManagedサービスプロバイダ
                sa = new AesManaged();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "DESCryptoServiceProvider")
            {
                // DESCryptoServiceProviderサービスプロバイダ
                sa = new DESCryptoServiceProvider();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "RC2CryptoServiceProvider")
            {
                // RC2CryptoServiceProviderサービスプロバイダ
                sa = new RC2CryptoServiceProvider();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "RijndaelManaged")
            {
                // RijndaelManagedサービスプロバイダ
                sa = new RijndaelManaged();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "TripleDESCryptoServiceProvider")
            {
                // TripleDESCryptoServiceProviderサービスプロバイダ
                sa = new TripleDESCryptoServiceProvider();
            }

            return sa;
        }

        #endregion

        #region プロバイダ説明

        /// <summary>秘密鍵・暗号化サービスプロバイダの説明</summary>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.SelectedTab.Name != "tabPage3")
            {
                return;
            }

            // 秘密鍵・暗号化サービスプロバイダ
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithmServiceProvider();
            
            if (sa is AesCryptoServiceProvider)
            {
                this.textBox35.Text = "AESアルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイト、32 バイトのキー長";
            }
            else if (sa is AesManaged)
            {
                this.textBox35.Text = "AESアルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイト、32 バイトのキー長";
            }
            else if (sa is DESCryptoServiceProvider)
            {
                this.textBox35.Text = "DESアルゴリズム";
                this.textBox36.Text = "8 バイトのキー長";
            }
            else if (sa is RC2CryptoServiceProvider)
            {
                this.textBox35.Text = "RC2 アルゴリズム";
                this.textBox36.Text = "5 バイトから 16 バイトのキー長を 1 バイト単位";
            }
            else if (sa is RijndaelManaged)
            {
                this.textBox35.Text = "Rijndael アルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイト、32 バイトのキー長";
            }
            else if (sa is TripleDESCryptoServiceProvider)
            {
                this.textBox35.Text = "TripleDES アルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイトのキー長";
            }

            // 秘密鍵・暗号化サービスプロバイダの各プロパティを出力
            if (sa != null)
            {
                this.textBox1.Text = "";
                KeySizes[] kszs = null;
                
                this.textBox1.Text += "・Mode:" + sa.Mode.ToString() + "\r\n";
                this.textBox1.Text += "・Padding:" + sa.Padding.ToString() + "\r\n";
                this.textBox1.Text += "・FeedbackSize:" + sa.FeedbackSize.ToString() + "\r\n";

                this.textBox1.Text += "\r\n";

                this.textBox1.Text += "・KeySize:" + sa.KeySize.ToString() + "\r\n";
                kszs = sa.LegalKeySizes;
                this.textBox1.Text += "・LegalKeySizes:\r\n";
                foreach (KeySizes ksz in kszs)
                {
                    this.textBox1.Text += "　・ksz.MaxSize:" + ksz.MaxSize + "\r\n";
                    this.textBox1.Text += "　・ksz.MinSize:" + ksz.MinSize + "\r\n";
                    this.textBox1.Text += "　・ksz.SkipSize:" + ksz.SkipSize + "\r\n";
                    this.textBox1.Text += "\r\n";
                }

                this.textBox1.Text += "・BlockSize:" + sa.BlockSize.ToString() + "\r\n";
                kszs = sa.LegalBlockSizes;
                this.textBox1.Text += "・LegalBlockSizes:\r\n";
                foreach (KeySizes ksz in kszs)
                {
                    this.textBox1.Text += "　・ksz.MaxSize:" + ksz.MaxSize + "\r\n";
                    this.textBox1.Text += "　・ksz.MinSize:" + ksz.MinSize + "\r\n";
                    this.textBox1.Text += "　・ksz.SkipSize:" + ksz.SkipSize + "\r\n";
                    this.textBox1.Text += "\r\n";
                }
            }
        }

        #endregion

        #region プロバイダ設定

        /// <summary>秘密鍵・暗号化サービスプロバイダの設定</summary>
        /// <param name="sa">秘密鍵・暗号化サービスプロバイダ</param>
        /// <param name="akb">byte型配列に変換したキー文字列（UTF-8 Enc）</param>
        private void SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(SymmetricAlgorithm sa, byte[] akb)
        {
            // SymmetricAlgorithm.IV プロパティ (System.Security.Cryptography)
            // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.symmetricalgorithm.iv.aspx
            // ・SymmetricAlgorithm クラスの新しいインスタンスを作成したときや、
            // 　GenerateIV メソッドを手動で呼び出したときには、
            // 　自動的に IV プロパティが新しい乱数値に設定されます。
            //
            // ・IV プロパティのサイズは8で割られたBlockSize プロパティと同じである必要があります。
            //
            // ・SymmetricAlgorithm クラスのいずれかを使用して暗号化されたデータを復号化するには、
            // 　Key プロパティと IV プロパティを、暗号化に使用された値と同じ値に設定する必要があります。

            //sa.GenerateIV();
            //sa.GenerateKey();

            // 共有キーと初期化ベクタを設定
            if (sa is AesCryptoServiceProvider)
            {
                // AesCryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aescryptoserviceprovider.aspx
                // 高度暗号化標準 (AES: Advanced Encryption Standard) アルゴリズムの CAPI実装を使用して、対称の暗号化と復号化を実行します。

                // AesCryptoServiceProvider.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aescryptoserviceprovider.keysize.aspx
                // キーの最小サイズは 128 ビット（16バイト）で、最大サイズは 256 ビット（32バイト）です。

                // 秘密鍵
                // 16 バイト、24 バイト、32 バイトのキー長
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else if (akb.Length < 32)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 32);
                }
            }
            else if (sa is AesManaged)
            {
                // AesManaged クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aesmanaged.aspx
                // 高度暗号化標準 (AES: Advanced Encryption Standard) 対称アルゴリズムのマネージ実装を提供します。 

                // AesManaged.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aesmanaged.keysize.aspx
                // キーの最大サイズは 256 ビット（32バイト）です。

                // 秘密鍵
                // 16 バイト、24 バイト、32 バイトのキー長
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else if (akb.Length < 32)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 32);
                }
            }
            else if (sa is DESCryptoServiceProvider)
            {
                // DESCryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.descryptoserviceprovider.aspx
                // DES (Data Encryption Standard) アルゴリズムの暗号サービス プロバイダー (CSP: Cryptographic Service Provider)

                // DES.Key プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.des.key.aspx
                // このアルゴリズムは、64 ビット（8バイト）のキー長をサポートします。

                // 秘密鍵
                // 8 バイトのキー長
                if (akb.Length < 8)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 8);
                }
            }
            else if (sa is RC2CryptoServiceProvider)
            {
                // RC2CryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.rc2cryptoserviceprovider.aspx
                // RC2 アルゴリズムの暗号サービス プロバイダー (CSP: Cryptographic Service Provider) 実装にアクセスするためのラッパー オブジェクトを定義します。 

                // RC2.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.rc2.keysize.aspx
                // このアルゴリズムは 40 ～ 1024 ビットのキー長を 8 ビット単位でサポートしますが、
                // RC2CryptoServiceProvider 実装は 40 ～ 128 ビット（5 バイトから 16 バイト）のキー長のみを 8 ビット（1 バイト）単位でサポートします。

                // 秘密鍵
                // 5 バイトから 16 バイトのキー長を 1 バイト単位
                if (akb.Length < 5)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 16)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, akb.Length);
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }

            }
            else if (sa is RijndaelManaged)
            {
                // RijndaelManaged クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.rijndaelmanaged.aspx
                // Rijndael アルゴリズムのマネージ バージョンにアクセスします。  

                // SymmetricAlgorithm.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.symmetricalgorithm.keysize.aspx
                // 有効なキー サイズは、対称アルゴリズムの特定の実装によって指定され、LegalKeySizes プロパティに一覧表示されます。

                // 秘密鍵
                // 16 バイト、24 バイト、32 バイトのキー長
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else if (akb.Length < 32)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 32);
                }
            }
            else if (sa is TripleDESCryptoServiceProvider)
            {
                // TripleDESCryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.tripledescryptoserviceprovider.aspx
                // TripleDES アルゴリズムの暗号サービス プロバイダー (CSP: Cryptographic Service Provider) バージョンにアクセスする、ラッパー オブジェクトを定義します。  

                // SymmetricAlgorithm.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.symmetricalgorithm.keysize.aspx
                // 有効なキー サイズは、対称アルゴリズムの特定の実装によって指定され、LegalKeySizes プロパティに一覧表示されます。

                // 秘密鍵
                // 16 バイト、24 バイトのキー長
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else
                {
                    sa.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
            }

            // 初期化ベクタ
            sa.IV = PubCmnFunction.ShortenByteArray(akb, sa.BlockSize / 8);
        }

        #endregion

        #region 暗号化

        /// <summary>秘密鍵・暗号化</summary>
        private void button31_Click(object sender, EventArgs e)
        {
            this.textBox32.Text = "";
            this.textBox33.Text = "";
            this.textBox34.Text = "";

            if (this.textBox31a.Text == "" || this.textBox31b.Text == "")
            {
                return;
            }

            // 元文字列
            string ss = this.textBox31a.Text;

            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] asb = Encoding.UTF8.GetBytes(ss);

            // キー文字列
            string ks = this.textBox31b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] akb = Encoding.UTF8.GetBytes(ks);

            // 秘密鍵・暗号化サービスプロバイダを生成、初期化
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithmServiceProvider();
            this.SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(sa, akb);

            // データ出力先メモリストリーム
            MemoryStream ms = new MemoryStream();
            
            // 暗号化オブジェクトの作成
            ICryptoTransform ict = sa.CreateEncryptor();

            // メモリストリームを暗号化ストリームで装飾
            CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write);

            // 暗号化ストリーム⇒メモリストリームに書き込む
            cs.Write(asb, 0, asb.Length);
            cs.FlushFinalBlock();

            // 暗号をメモリストリームから取得
            byte[] acb = ms.ToArray();

            // ストリームを閉じる
            cs.Close();
            ms.Close();

            // 結果を表示

            // 生バイト
            this.textBox32.Text = CustomEncode.ToHexString(acb);
            // Base64
            this.textBox33.Text = Convert.ToBase64String(acb);
        }

        #endregion

        #region 復号化

        /// <summary>秘密鍵・復号化</summary>
        private void button32_Click(object sender, EventArgs e)
        {
            this.textBox34.Text = "";

            if (this.textBox31a.Text == "" || this.textBox31b.Text == "")
            {
                return;
            }

            // 暗号
            byte[] acb = Convert.FromBase64String(this.textBox33.Text);

            // キー文字列
            string ks = this.textBox31b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] akb = Encoding.UTF8.GetBytes(ks);

            // 秘密鍵・暗号化サービスプロバイダを生成、初期化
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithmServiceProvider();
            this.SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(sa, akb);

            try
            {
                // データ入力元メモリストリーム
                MemoryStream ms = new MemoryStream(acb);

                // 復号化オブジェクトの作成
                ICryptoTransform ict = sa.CreateDecryptor();

                // メモリストリームを復号化ストリームで装飾
                CryptoStream ds = new CryptoStream(ms, ict, CryptoStreamMode.Read);
                // 復号化ストリームをStreamReaderで装飾
                StreamReader sr = new StreamReader(ds, Encoding.UTF8);

                // StreamReaderを使用して文字列を取得する
                string ss = sr.ReadToEnd();

                // ストリームを閉じる
                sr.Close();
                ds.Close();
                ms.Close();

                // 結果を表示
                this.textBox34.Text = ss;
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox34.Text = "エラーです。キーを変更した可能性があります。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #endregion

        #region 公開鍵

        #region 暗号化

        #region プロバイダ生成

        /// <summary>公開鍵・暗号化サービスプロバイダの生成</summary>
        /// <returns>公開鍵・暗号化サービスプロバイダ</returns>
        private AsymmetricAlgorithm CreateAsymmetricAlgorithmServiceProvider()
        {
            // 公開鍵・暗号化サービスプロバイダ
            AsymmetricAlgorithm aa = null;
            if (this.comboBox4.SelectedItem.ToString() == "DSACryptoServiceProvider")
            {
                // DSACryptoServiceProviderサービスプロバイダ
                aa = new DSACryptoServiceProvider();
            }
            else if (this.comboBox4.SelectedItem.ToString() == "ECDiffieHellmanCng")
            {
                // ECDiffieHellmanCngサービスプロバイダ
                aa = new ECDiffieHellmanCng();
            }
            else if (this.comboBox4.SelectedItem.ToString() == "ECDsaCng")
            {
                // ECDsaCngサービスプロバイダ
                aa = new ECDsaCng();
            }
            else if (this.comboBox4.SelectedItem.ToString() == "RSACryptoServiceProvider")
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
            }

            return aa;
        }

        #endregion

        #region プロバイダ説明

        /// <summary>公開鍵・暗号化サービスプロバイダの説明</summary>
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.SelectedTab.Name != "tabPage4")
            {
                return;
            }

            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider();

                if (aa is DSACryptoServiceProvider)
                {
                    this.textBox45.Text = "DSAアルゴリズム";
                    this.textBox46.Text = "64 バイトから 128 バイトのキー長を 8 バイト単位";
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    this.textBox45.Text = "ECDHアルゴリズムのCNG実装";
                    this.textBox46.Text = "";
                }
                else if (aa is ECDsaCng)
                {
                    this.textBox45.Text = "ECDSAのCNG実装";
                    this.textBox46.Text = "";
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    this.textBox45.Text = "RSAアルゴリズム";
                    this.textBox46.Text = "48 バイトから 2048 バイトのキー長を 1 バイト単位";
                }

                // 公開鍵・暗号化サービスプロバイダの各プロパティを出力
                if (aa != null)
                {
                    this.textBox1.Text = "";
                    KeySizes[] kszs = null;

                    this.textBox1.Text += "・SignatureAlgorithm:" + aa.SignatureAlgorithm.ToString() + "\r\n";

                    if (aa.KeyExchangeAlgorithm != null)
                    {
                        this.textBox1.Text += "・KeyExchangeAlgorithm:" + aa.KeyExchangeAlgorithm.ToString() + "\r\n";
                    }

                    this.textBox1.Text += "\r\n";

                    this.textBox1.Text += "・KeySize:" + aa.KeySize.ToString() + "\r\n";
                    kszs = aa.LegalKeySizes;
                    this.textBox1.Text += "・LegalKeySizes:\r\n";
                    foreach (KeySizes ksz in kszs)
                    {
                        this.textBox1.Text += "　・ksz.MaxSize:" + ksz.MaxSize + "\r\n";
                        this.textBox1.Text += "　・ksz.MinSize:" + ksz.MinSize + "\r\n";
                        this.textBox1.Text += "　・ksz.SkipSize:" + ksz.SkipSize + "\r\n";
                        this.textBox1.Text += "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox1.Text = "エラーです。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #region 鍵の取得

        /// <summary>鍵の取得</summary>
        private void button40_Click(object sender, EventArgs e)
        {
            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider();

                // 公開鍵をXML形式で取得
                this.textBox41b.Text = aa.ToXmlString(false);
                // 秘密鍵をXML形式で取得
                this.textBox41c.Text = aa.ToXmlString(true);
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox1.Text = "エラーです。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #region プロバイダ設定

        /// <summary>公開鍵・暗号化サービスプロバイダの設定</summary>
        /// <param name="sa">公開鍵・暗号化サービスプロバイダ</param>
        private void SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(AsymmetricAlgorithm aa)
        {
            if (aa is DSACryptoServiceProvider)
            {
                DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)aa;

                // 設定する
            }
            else if (aa is ECDiffieHellmanCng)
            {
                ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)aa;

                // 設定する
            }
            else if (aa is ECDsaCng)
            {
                ECDsaCng ecdsa = (ECDsaCng)aa;

                // 設定する
            }
            else if (aa is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)aa;

                // 設定する
            }
        }

        #endregion

        #region 暗号化

        /// <summary>公開鍵・暗号化</summary>
        private void button41_Click(object sender, EventArgs e)
        {
            this.textBox42.Text = "";
            this.textBox43.Text = "";
            this.textBox44.Text = "";

            if (this.textBox41a.Text == "" 
                || this.textBox41b.Text == ""
                || this.textBox41c.Text == "")
            {
                return;
            }

            try
            {
                // 暗号のbyte型配列
                byte[] acb = null;
                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] asb = Encoding.UTF8.GetBytes(this.textBox41a.Text);

                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider();
                this.SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(aa);

                // 公開鍵
                aa.FromXmlString(this.textBox41b.Text);

                if (aa is DSACryptoServiceProvider)
                {
                    DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)aa;

                    // 暗号化する
                    throw new NotImplementedException("DSACryptoServiceProviderの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)aa;

                    // 暗号化する
                    throw new NotImplementedException("ECDiffieHellmanCngの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is ECDsaCng)
                {
                    ECDsaCng ecdsa = (ECDsaCng)aa;

                    // 暗号化する
                    throw new NotImplementedException("ECDsaCngの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)aa;

                    // 暗号化する（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                    acb = rsa.Encrypt(asb, false);
                }

                // 結果を表示

                // 生バイト
                this.textBox42.Text = CustomEncode.ToHexString(acb);
                // Base64
                this.textBox43.Text = Convert.ToBase64String(acb);
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox44.Text = "エラーです。キーを変更した可能性があります。\r\n"
                    + ex.ToString();
            }
        }
        
        #endregion

        #region 復号化

        /// <summary>公開鍵・復号化</summary>
        private void button42_Click(object sender, EventArgs e)
        {
            this.textBox44.Text = "";

            if (this.textBox41a.Text == ""
                || this.textBox41b.Text == ""
                || this.textBox41c.Text == "")
            {
                return;
            }

            try
            {
                
                // 暗号のbyte型配列
                byte[] acb = Convert.FromBase64String(this.textBox43.Text);
                // 元文字列（バイト配列に）
                byte[] asb = null;

                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider();
                this.SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(aa);

                // 秘密鍵
                aa.FromXmlString(this.textBox41c.Text);

                if (aa is DSACryptoServiceProvider)
                {
                    DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)aa;

                    // 復号化する
                    throw new NotImplementedException("DSACryptoServiceProviderの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)aa;

                    // 復号化する
                    throw new NotImplementedException("ECDiffieHellmanCngの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is ECDsaCng)
                {
                    ECDsaCng ecdsa = (ECDsaCng)aa;

                    // 復号化する
                    throw new NotImplementedException("ECDsaCngの共通鍵暗号化はサポートされていません。");
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)aa;

                    // 復号化（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                    asb = rsa.Decrypt(acb, false);
                }

                // 結果を表示
                this.textBox44.Text = Encoding.UTF8.GetString(asb);
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox44.Text = "エラーです。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #endregion

        #region 署名・検証

        #region プロバイダ生成

        /// <summary>公開鍵・暗号化サービスプロバイダの生成</summary>
        /// <returns>公開鍵・暗号化サービスプロバイダ</returns>
        private AsymmetricAlgorithm CreateAsymmetricAlgorithmServiceProvider2()
        {
            // 公開鍵・暗号化サービスプロバイダ
            AsymmetricAlgorithm aa = null;
            if (this.comboBox5.SelectedItem.ToString().IndexOf("DSACryptoServiceProvider") != -1)
            {
                // DSACryptoServiceProviderサービスプロバイダ
                aa = new DSACryptoServiceProvider();
            }
            else if (this.comboBox5.SelectedItem.ToString().IndexOf("ECDiffieHellmanCng") != -1)
            {
                // ECDiffieHellmanCngサービスプロバイダ
                aa = new ECDiffieHellmanCng();
            }
            else if (this.comboBox5.SelectedItem.ToString().IndexOf("ECDsaCng") != -1)
            {
                // ECDsaCngサービスプロバイダ
                aa = new ECDsaCng();
            }
            else if (this.comboBox5.SelectedItem.ToString().IndexOf("RSACryptoServiceProvider") != -1)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
            }

            return aa;
        }

        #endregion

        #region プロバイダ説明

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.SelectedTab.Name != "tabPage5")
            {
                return;
            }

            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider2();

                if (aa is DSACryptoServiceProvider)
                {
                    this.textBox55.Text = "DSAアルゴリズム";
                    this.textBox56.Text = "64 バイトから 128 バイトのキー長を 8 バイト単位";
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    this.textBox55.Text = "ECDHアルゴリズムのCNG実装";
                    this.textBox56.Text = "";
                }
                else if (aa is ECDsaCng)
                {
                    this.textBox55.Text = "ECDSAのCNG実装";
                    this.textBox56.Text = "";
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    this.textBox55.Text = "RSAアルゴリズム";
                    this.textBox56.Text = "48 バイトから 2048 バイトのキー長を 1 バイト単位";
                }

                // 公開鍵・暗号化サービスプロバイダの各プロパティを出力
                if (aa != null)
                {
                    this.textBox1.Text = "";
                    KeySizes[] kszs = null;

                    this.textBox1.Text += "・SignatureAlgorithm:" + aa.SignatureAlgorithm.ToString() + "\r\n";

                    if (aa.KeyExchangeAlgorithm != null)
                    {
                        this.textBox1.Text += "・KeyExchangeAlgorithm:" + aa.KeyExchangeAlgorithm.ToString() + "\r\n";
                    }

                    this.textBox1.Text += "\r\n";

                    this.textBox1.Text += "・KeySize:" + aa.KeySize.ToString() + "\r\n";
                    kszs = aa.LegalKeySizes;
                    this.textBox1.Text += "・LegalKeySizes:\r\n";
                    foreach (KeySizes ksz in kszs)
                    {
                        this.textBox1.Text += "　・ksz.MaxSize:" + ksz.MaxSize + "\r\n";
                        this.textBox1.Text += "　・ksz.MinSize:" + ksz.MinSize + "\r\n";
                        this.textBox1.Text += "　・ksz.SkipSize:" + ksz.SkipSize + "\r\n";
                        this.textBox1.Text += "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox1.Text = "エラーです。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #region 鍵の取得

        /// <summary>鍵の取得</summary>
        private void button50_Click(object sender, EventArgs e)
        {
            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider2();

                // 公開鍵をXML形式で取得
                this.textBox51b.Text = aa.ToXmlString(false);
                // 秘密鍵をXML形式で取得
                this.textBox51c.Text = aa.ToXmlString(true);
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox1.Text = "エラーです。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #region 署名

        /// <summary>署名</summary>
        private void button51_Click(object sender, EventArgs e)
        {
            this.textBox52.Text = "";
            this.textBox53.Text = "";
            this.textBox54.Text = "";
            this.textBox55.Text = "";
            this.textBox56.Text = "";

            if (this.textBox51a.Text == ""
                || this.textBox51b.Text == ""
                || this.textBox51c.Text == "")
            {
                return;
            }

            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider2();

                // 秘密鍵
                aa.FromXmlString(this.textBox51c.Text);

                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] asb  = Encoding.UTF8.GetBytes(this.textBox51a.Text);
                // ハッシュ値
                byte[] ahb = null;
                // 署名
                byte[] ab_sign = null;

                if (aa is DSACryptoServiceProvider)
                {
                    // キャスト
                    DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)aa;

                    // DSASignatureFormatterオブジェクトを作成
                    DSASignatureFormatter dsaFormatter = new DSASignatureFormatter(dsa);

                    // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                    if (this.comboBox5.SelectedItem.ToString().IndexOf("SHA1") != -1)
                    {
                        dsaFormatter.SetHashAlgorithm("SHA1");
                        ahb = SHA1.Create().ComputeHash(asb);
                    }

                    // 署名を作成
                    ab_sign = dsaFormatter.CreateSignature(ahb);
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    // キャスト
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)aa;

                    // 署名を作成
                    throw new NotImplementedException("ECDiffieHellmanCng:未実装");
                }
                else if (aa is ECDsaCng)
                {
                    // キャスト
                    ECDsaCng ecdsa = (ECDsaCng)aa;

                    // 署名を作成
                    throw new NotImplementedException("ECDsaCng:未実装");
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    // キャスト
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)aa;

                    // RSAPKCS1SignatureFormatterオブジェクトを作成
                    RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);

                    // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                    if (this.comboBox5.SelectedItem.ToString().IndexOf("SHA1") != -1)
                    {
                        rsaFormatter.SetHashAlgorithm("SHA1");
                        ahb = SHA1.Create().ComputeHash(asb);
                    }
                    else if (this.comboBox5.SelectedItem.ToString().IndexOf("MD5") != -1)
                    {
                        rsaFormatter.SetHashAlgorithm("MD5");
                        ahb = MD5.Create().ComputeHash(asb);
                    }

                    // 署名を作成
                    ab_sign = rsaFormatter.CreateSignature(ahb);
                }

                // 結果を表示

                // ハッシュ

                // 生バイト
                this.textBox52.Text = CustomEncode.ToHexString(ahb);
                // Base64
                this.textBox53.Text = Convert.ToBase64String(ahb);

                // 署名

                // 生バイト
                this.textBox54.Text = CustomEncode.ToHexString(ab_sign);
                // Base64
                this.textBox55.Text = Convert.ToBase64String(ab_sign);
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox56.Text = "エラーです。キーを変更した可能性があります。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #region 検証

        private void button52_Click(object sender, EventArgs e)
        {
            this.textBox56.Text = "";

            if (this.textBox51a.Text == ""
                || this.textBox51b.Text == ""
                || this.textBox51c.Text == "")
            {
                return;
            }

            // 公開鍵・暗号化サービスプロバイダ
            AsymmetricAlgorithm aa = this.CreateAsymmetricAlgorithmServiceProvider2();

            // 公開鍵
            aa.FromXmlString(this.textBox51b.Text);

            try
            {
                // 結果フラグ
                bool flg = false;

                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] asb = Encoding.UTF8.GetBytes(this.textBox51a.Text);

                // ハッシュ値を取得
                byte[] ahb = Convert.FromBase64String(this.textBox53.Text);

                if (aa is DSACryptoServiceProvider)
                {
                    // キャスト
                    DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)aa;
                    
                    // DSASignatureFormatterオブジェクトを作成
                    DSASignatureDeformatter dsaSignatureDeformatter = new DSASignatureDeformatter(dsa);

                    //　検証に使用するハッシュアルゴリズムを指定し
                    // 上記で、ハッシュ値を計算した際と同じアルゴリズムを使用すること。
                    if (this.comboBox5.SelectedItem.ToString().IndexOf("SHA1") != -1)
                    {
                        dsaSignatureDeformatter.SetHashAlgorithm("SHA1");
                    }

                    // 検証する
                    flg = dsaSignatureDeformatter.VerifySignature(ahb, Convert.FromBase64String(this.textBox55.Text));
                }
                else if (aa is ECDiffieHellmanCng)
                {
                    // キャスト
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)aa;

                    // 検証する
                    throw new NotImplementedException("ECDiffieHellmanCng:未実装");
                }
                else if (aa is ECDsaCng)
                {
                    // キャスト
                    ECDsaCng ecdsa = (ECDsaCng)aa;

                    // 検証する
                    throw new NotImplementedException("ECDsaCng:未実装");
                }
                else if (aa is RSACryptoServiceProvider)
                {
                    // キャスト
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)aa;

                    // RSAPKCS1SignatureDeformatterオブジェクトを作成
                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);

                    //　検証に使用するハッシュアルゴリズムを指定し
                    // 上記で、ハッシュ値を計算した際と同じアルゴリズムを使用すること。
                    if (this.comboBox5.SelectedItem.ToString().IndexOf("SHA1") != -1)
                    {
                        rsaDeformatter.SetHashAlgorithm("SHA1");
                    }
                    else if (this.comboBox5.SelectedItem.ToString().IndexOf("MD5") != -1)
                    {
                        rsaDeformatter.SetHashAlgorithm("MD5");
                    }

                    // 検証する
                    flg = rsaDeformatter.VerifySignature(ahb, Convert.FromBase64String(this.textBox55.Text));
                }

                // 検証結果を表示
                if (flg)
                {
                    this.textBox56.Text = "デジタル署名は署名前のメッセージであることが検証されました。";
                }
                else
                {
                    this.textBox56.Text = "デジタル署名は署名前のメッセージであることが検証されませんでした。";
                }
            }
            catch (Exception ex)
            {
                // 結果を表示
                this.textBox56.Text = "エラーです。キーを変更した可能性があります。\r\n"
                    + ex.ToString();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
