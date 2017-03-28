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
//* クラス日本語名  ：暗号化/復号化テスト・ツール（メイン画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/12  西野 大介         新規作成
//*  2017/01/10  西野 大介         秘密鍵、暗号化のプロバイダを削除（AesCryptoServiceProvider）
//*  2017/01/10  西野 大介         ハッシュ（キー付き）のHMACプロバイダを複数追加した。
//*  2017/01/10  西野 大介         公開鍵、署名・検証のECDsaCngプロバイダの検証処理を追加した。
//*  2017/01/10  西野 大介         公開鍵、署名・検証の各プロバイダのHashアルゴリズムを追加した。
//*  2017/01/10  西野 大介         内部文書化（変数名の見直し）を行った。
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
            HashAlgorithm hashAlgorithm = null;

            if (this.comboBox1.SelectedItem.ToString() == "既定のプロバイダ")
            {
                // 既定の暗号化サービスプロバイダ
                hashAlgorithm = HashAlgorithm.Create();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "MD5CryptoServiceProvider")
            {
                // MD5CryptoServiceProviderサービスプロバイダ
                hashAlgorithm = new MD5CryptoServiceProvider();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA1CryptoServiceProvider")
            {
                // SHA1CryptoServiceProviderサービスプロバイダ
                hashAlgorithm = new SHA1CryptoServiceProvider();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA1Managed")
            {
                // SHA1Managedサービスプロバイダ
                hashAlgorithm = new SHA1Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA256Managed")
            {
                // SHA256Managedサービスプロバイダ
                hashAlgorithm = new SHA256Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA384Managed")
            {
                // SHA384Managedサービスプロバイダ
                hashAlgorithm = new SHA384Managed();
            }
            else if (this.comboBox1.SelectedItem.ToString() == "SHA512Managed")
            {
                // SHA512Managedサービスプロバイダ
                hashAlgorithm = new SHA512Managed();
            }

            return hashAlgorithm;
        }

        #endregion

        #region プロバイダ説明

        /// <summary>ハッシュ（キー無し）サービスプロバイダの説明</summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm hashAlgorithm = this.CreateHashAlgorithmServiceProvider();

            if (hashAlgorithm is MD5CryptoServiceProvider)
            {
                this.textBox15.Text = "MD5 ハッシュ";
                this.textBox16.Text = "ハッシュ サイズは 16 バイトです。";   
            }
            else if (hashAlgorithm is SHA1CryptoServiceProvider)
            {
                this.textBox15.Text = "SHA1 ハッシュ";
                this.textBox16.Text = "ハッシュ サイズは 20 バイトです。";
            }
            else if (hashAlgorithm is SHA1Managed)
            {
                this.textBox15.Text = "SHA1 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 20 バイトです。";
            }
            else if (hashAlgorithm is SHA256Managed)
            {
                this.textBox15.Text = "SHA256 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 32 バイトです。";
            }
            else if (hashAlgorithm is SHA384Managed)
            {
                this.textBox15.Text = "SHA384 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 48 バイトです。";
            }
            else if (hashAlgorithm is SHA512Managed)
            {
                this.textBox15.Text = "SHA512 ハッシュ（マネージ ライブラリ）";
                this.textBox16.Text = "ハッシュ サイズは 64 バイトです。";
            }

            // ハッシュ（キー無し）サービスプロバイダの各プロパティを出力
            if (hashAlgorithm != null)
            {
                this.textBox1.Text = "";
                
                this.textBox1.Text += "・HashSize:" + hashAlgorithm.HashSize.ToString() + "\r\n";
                this.textBox1.Text += "・InputBlockSize:" + hashAlgorithm.InputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・OutputBlockSize:" + hashAlgorithm.OutputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・CanReuseTransform:" + hashAlgorithm.CanReuseTransform.ToString() + "\r\n";
                this.textBox1.Text += "・CanTransformMultipleBlocks:" + hashAlgorithm.CanTransformMultipleBlocks.ToString() + "\r\n";
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
            HashAlgorithm hashAlgorithm = this.CreateHashAlgorithmServiceProvider();

            // 元文字列
            string sourceString = this.textBox11.Text;

            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] sourceStringByte = Encoding.UTF8.GetBytes(sourceString);

            // ハッシュ値を計算する
            byte[] hashByte = hashAlgorithm.ComputeHash(sourceStringByte);

            //結果を表示

            // 生バイト
            this.textBox12.Text = CustomEncode.ToHexString(hashByte);
            // Base64
            this.textBox13.Text = Convert.ToBase64String(hashByte);
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
            KeyedHashAlgorithm keyedHashAlgorithm = null;

            if (this.comboBox2.SelectedItem.ToString() == "既定のプロバイダ")
            {
                // 既定の暗号化サービスプロバイダ
                keyedHashAlgorithm = KeyedHashAlgorithm.Create();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACMD5")
            {
                // HMACMD5サービスプロバイダ
                keyedHashAlgorithm = new HMACMD5();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACRIPEMD160")
            {
                // HMACRIPEMD160サービスプロバイダ
                keyedHashAlgorithm = new HMACRIPEMD160();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACSHA1")
            {
                // HMACSHA1サービスプロバイダ
                keyedHashAlgorithm = new HMACSHA1();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACSHA256")
            {
                // HMACSHA256サービスプロバイダ
                keyedHashAlgorithm = new HMACSHA256();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACSHA384")
            {
                // HMACSHA384サービスプロバイダ
                keyedHashAlgorithm = new HMACSHA384();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "HMACSHA512")
            {
                // HMACSHA512サービスプロバイダ
                keyedHashAlgorithm = new HMACSHA512();
            }
            else if (this.comboBox2.SelectedItem.ToString() == "MACTripleDES")
            {
                // MACTripleDESサービスプロバイダ
                keyedHashAlgorithm = new MACTripleDES();
            }

           return keyedHashAlgorithm;
        }

        #endregion

        #region プロバイダの説明

        /// <summary>ハッシュ（キー付き）サービスプロバイダの説明</summary>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ハッシュ（キー付き）サービスプロバイダ
            KeyedHashAlgorithm keyedHashAlgorithm = this.CreateKeyedHashAlgorithmServiceProvider();

            if (keyedHashAlgorithm is HMACMD5)
            {
                this.textBox25.Text = "MD5 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 128 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is HMACRIPEMD160)
            {
                this.textBox25.Text = "MD160 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 160 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is HMACSHA1)
            {
                this.textBox25.Text = "SHA1 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 160 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is HMACSHA256)
            {
                this.textBox25.Text = "SHA256 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 256 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is HMACSHA384)
            {
                this.textBox25.Text = "SHA1 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 384 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is HMACSHA512)
            {
                this.textBox25.Text = "SHA1 を使用して、ハッシュ メッセージ認証コード（HMAC）を計算します。";
                this.textBox26.Text = "どのサイズのキーでも受け入れ、長さが 512 ビットのハッシュ シーケンスを生成します。";
            }
            else if (keyedHashAlgorithm is MACTripleDES)
            {
                this.textBox25.Text = "TripleDES を使用して、メッセージ認証コード（MAC）を計算します。";
                this.textBox26.Text = "長さが 16 または 24 バイトのキーを使用し、長さが 64 ビットのハッシュ シーケンスを生成します。";
            }

            // ハッシュ（キー付き）サービスプロバイダの各プロパティを出力
            if (keyedHashAlgorithm != null)
            {
                this.textBox1.Text = "";

                this.textBox1.Text += "・HashSize:" + keyedHashAlgorithm.HashSize.ToString() + "\r\n";
                this.textBox1.Text += "・InputBlockSize:" + keyedHashAlgorithm.InputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・OutputBlockSize:" + keyedHashAlgorithm.OutputBlockSize.ToString() + "\r\n";
                this.textBox1.Text += "・CanReuseTransform:" + keyedHashAlgorithm.CanReuseTransform.ToString() + "\r\n";
                this.textBox1.Text += "・CanTransformMultipleBlocks:" + keyedHashAlgorithm.CanTransformMultipleBlocks.ToString() + "\r\n";
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
            KeyedHashAlgorithm keyedHashAlgorithm = this.CreateKeyedHashAlgorithmServiceProvider();

            // 元文字列
            string sourceString = this.textBox21a.Text;

            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] sourceStringByte = Encoding.UTF8.GetBytes(sourceString);

            // キー文字列
            string keyString = this.textBox21b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] akb = Encoding.UTF8.GetBytes(keyString);

            // ハッシュ値を計算する
            if (keyedHashAlgorithm is HMACSHA1)
            {
                // どのサイズのキーでも受け入れる
                keyedHashAlgorithm.Key = akb;
            }
            else if (keyedHashAlgorithm is MACTripleDES)
            {
                // 長さが 16 または 24 バイトのキーを受け入れる
                if (akb.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (akb.Length < 24)
                {
                    keyedHashAlgorithm.Key = PubCmnFunction.ShortenByteArray(akb, 16);
                }
                else
                {
                    // 24バイトに切り詰め
                    keyedHashAlgorithm.Key = PubCmnFunction.ShortenByteArray(akb, 24);
                }
            }

            byte[] hashByte = keyedHashAlgorithm.ComputeHash(sourceStringByte);

            //結果を表示

            // 生バイト
            this.textBox22.Text = CustomEncode.ToHexString(hashByte);
            // Base64
            this.textBox23.Text = Convert.ToBase64String(hashByte);
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
            SymmetricAlgorithm symmetricAlgorithm = null;

            if (this.comboBox3.SelectedItem.ToString() == "AesManaged")
            {
                // AesManagedサービスプロバイダ
                symmetricAlgorithm = new AesManaged();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "DESCryptoServiceProvider")
            {
                // DESCryptoServiceProviderサービスプロバイダ
                symmetricAlgorithm = new DESCryptoServiceProvider();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "RC2CryptoServiceProvider")
            {
                // RC2CryptoServiceProviderサービスプロバイダ
                symmetricAlgorithm = new RC2CryptoServiceProvider();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "RijndaelManaged")
            {
                // RijndaelManagedサービスプロバイダ
                symmetricAlgorithm = new RijndaelManaged();
            }
            else if (this.comboBox3.SelectedItem.ToString() == "TripleDESCryptoServiceProvider")
            {
                // TripleDESCryptoServiceProviderサービスプロバイダ
                symmetricAlgorithm = new TripleDESCryptoServiceProvider();
            }

            return symmetricAlgorithm;
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
            SymmetricAlgorithm symmetricAlgorithm = this.CreateSymmetricAlgorithmServiceProvider();
            
            if (symmetricAlgorithm is AesManaged)
            {
                this.textBox35.Text = "AESアルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイト、32 バイトのキー長";
            }
            else if (symmetricAlgorithm is DESCryptoServiceProvider)
            {
                this.textBox35.Text = "DESアルゴリズム";
                this.textBox36.Text = "8 バイトのキー長";
            }
            else if (symmetricAlgorithm is RC2CryptoServiceProvider)
            {
                this.textBox35.Text = "RC2 アルゴリズム";
                this.textBox36.Text = "5 バイトから 16 バイトのキー長を 1 バイト単位";
            }
            else if (symmetricAlgorithm is RijndaelManaged)
            {
                this.textBox35.Text = "Rijndael アルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイト、32 バイトのキー長";
            }
            else if (symmetricAlgorithm is TripleDESCryptoServiceProvider)
            {
                this.textBox35.Text = "TripleDES アルゴリズム";
                this.textBox36.Text = "16 バイト、24 バイトのキー長";
            }

            // 秘密鍵・暗号化サービスプロバイダの各プロパティを出力
            if (symmetricAlgorithm != null)
            {
                this.textBox1.Text = "";
                KeySizes[] kszs = null;
                
                this.textBox1.Text += "・Mode:" + symmetricAlgorithm.Mode.ToString() + "\r\n";
                this.textBox1.Text += "・Padding:" + symmetricAlgorithm.Padding.ToString() + "\r\n";
                this.textBox1.Text += "・FeedbackSize:" + symmetricAlgorithm.FeedbackSize.ToString() + "\r\n";

                this.textBox1.Text += "\r\n";

                this.textBox1.Text += "・KeySize:" + symmetricAlgorithm.KeySize.ToString() + "\r\n";
                kszs = symmetricAlgorithm.LegalKeySizes;
                this.textBox1.Text += "・LegalKeySizes:\r\n";
                foreach (KeySizes ksz in kszs)
                {
                    this.textBox1.Text += "　・ksz.MaxSize:" + ksz.MaxSize + "\r\n";
                    this.textBox1.Text += "　・ksz.MinSize:" + ksz.MinSize + "\r\n";
                    this.textBox1.Text += "　・ksz.SkipSize:" + ksz.SkipSize + "\r\n";
                    this.textBox1.Text += "\r\n";
                }

                this.textBox1.Text += "・BlockSize:" + symmetricAlgorithm.BlockSize.ToString() + "\r\n";
                kszs = symmetricAlgorithm.LegalBlockSizes;
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
        /// <param name="symmetricAlgorithm">秘密鍵・暗号化サービスプロバイダ</param>
        /// <param name="keyStringByte">byte型配列に変換したキー文字列（UTF-8 Enc）</param>
        private void SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(SymmetricAlgorithm symmetricAlgorithm, byte[] keyStringByte)
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
            if (symmetricAlgorithm is AesManaged)
            {
                // AesManaged クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aesmanaged.aspx
                // 高度暗号化標準 (AES: Advanced Encryption Standard) 対称アルゴリズムのマネージ実装を提供します。 

                // AesManaged.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.aesmanaged.keysize.aspx
                // キーの最大サイズは 256 ビット（32バイト）です。

                // 秘密鍵
                // 16 バイト、24 バイト、32 バイトのキー長
                if (keyStringByte.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (keyStringByte.Length < 24)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 16);
                }
                else if (keyStringByte.Length < 32)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 24);
                }
                else
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 32);
                }
            }
            else if (symmetricAlgorithm is DESCryptoServiceProvider)
            {
                // DESCryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.descryptoserviceprovider.aspx
                // DES (Data Encryption Standard) アルゴリズムの暗号サービス プロバイダー (CSP: Cryptographic Service Provider)

                // DES.Key プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.des.key.aspx
                // このアルゴリズムは、64 ビット（8バイト）のキー長をサポートします。

                // 秘密鍵
                // 8 バイトのキー長
                if (keyStringByte.Length < 8)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 8);
                }
            }
            else if (symmetricAlgorithm is RC2CryptoServiceProvider)
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
                if (keyStringByte.Length < 5)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (keyStringByte.Length < 16)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, keyStringByte.Length);
                }
                else
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 16);
                }

            }
            else if (symmetricAlgorithm is RijndaelManaged)
            {
                // RijndaelManaged クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.rijndaelmanaged.aspx
                // Rijndael アルゴリズムのマネージ バージョンにアクセスします。  

                // SymmetricAlgorithm.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.symmetricalgorithm.keysize.aspx
                // 有効なキー サイズは、対称アルゴリズムの特定の実装によって指定され、LegalKeySizes プロパティに一覧表示されます。

                // 秘密鍵
                // 16 バイト、24 バイト、32 バイトのキー長
                if (keyStringByte.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (keyStringByte.Length < 24)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 16);
                }
                else if (keyStringByte.Length < 32)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 24);
                }
                else
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 32);
                }
            }
            else if (symmetricAlgorithm is TripleDESCryptoServiceProvider)
            {
                // TripleDESCryptoServiceProvider クラス (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.tripledescryptoserviceprovider.aspx
                // TripleDES アルゴリズムの暗号サービス プロバイダー (CSP: Cryptographic Service Provider) バージョンにアクセスする、ラッパー オブジェクトを定義します。  

                // SymmetricAlgorithm.KeySize プロパティ (System.Security.Cryptography)
                // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.symmetricalgorithm.keysize.aspx
                // 有効なキー サイズは、対称アルゴリズムの特定の実装によって指定され、LegalKeySizes プロパティに一覧表示されます。

                // 秘密鍵
                // 16 バイト、24 バイトのキー長
                if (keyStringByte.Length < 16)
                {
                    MessageBox.Show("キーの長さが不足しています。");
                    return;
                }
                else if (keyStringByte.Length < 24)
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 16);
                }
                else
                {
                    symmetricAlgorithm.Key = PubCmnFunction.ShortenByteArray(keyStringByte, 24);
                }
            }

            // 初期化ベクタ
            symmetricAlgorithm.IV = PubCmnFunction.ShortenByteArray(keyStringByte, symmetricAlgorithm.BlockSize / 8);
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
            string sourceString = this.textBox31a.Text;

            // 元文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] sourceStringByte = Encoding.UTF8.GetBytes(sourceString);

            // キー文字列
            string keyString = this.textBox31b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] keyStringByte = Encoding.UTF8.GetBytes(keyString);

            // 秘密鍵・暗号化サービスプロバイダを生成、初期化
            SymmetricAlgorithm symmetricAlgorithm = this.CreateSymmetricAlgorithmServiceProvider();
            this.SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(symmetricAlgorithm, keyStringByte);

            // データ出力先メモリストリーム
            MemoryStream ms = new MemoryStream();
            
            // 暗号化オブジェクトの作成
            ICryptoTransform ict = symmetricAlgorithm.CreateEncryptor();

            // メモリストリームを暗号化ストリームで装飾
            CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write);

            // 暗号化ストリーム⇒メモリストリームに書き込む
            cs.Write(sourceStringByte, 0, sourceStringByte.Length);
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
            byte[] encryptedStringByte = Convert.FromBase64String(this.textBox33.Text);

            // キー文字列
            string keyString = this.textBox31b.Text;

            // キー文字列をbyte型配列に変換する（UTF-8 Enc）
            byte[] keyStringByte = Encoding.UTF8.GetBytes(keyString);

            // 秘密鍵・暗号化サービスプロバイダを生成、初期化
            SymmetricAlgorithm sa = this.CreateSymmetricAlgorithmServiceProvider();
            this.SetKeyAndInitializationVectorToSymmetricAlgorithmServiceProvider(sa, keyStringByte);

            try
            {
                // データ入力元メモリストリーム
                MemoryStream ms = new MemoryStream(encryptedStringByte);

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
            AsymmetricAlgorithm asymmetricAlgorithm = null;
            if (this.comboBox4.SelectedItem.ToString() == "RSACryptoServiceProvider")
            {
                // RSACryptoServiceProviderサービスプロバイダ
                asymmetricAlgorithm = new RSACryptoServiceProvider();
            }
            else if(this.comboBox4.SelectedItem.ToString() == "DSACryptoServiceProvider")
            {
                // DSACryptoServiceProviderサービスプロバイダ
                asymmetricAlgorithm = new DSACryptoServiceProvider();
            }
            else if (this.comboBox4.SelectedItem.ToString() == "ECDsaCng")
            {
                // ECDsaCngサービスプロバイダ
                asymmetricAlgorithm = new ECDsaCng();
            }
            else if (this.comboBox4.SelectedItem.ToString() == "ECDiffieHellmanCng")
            {
                // ECDiffieHellmanCngサービスプロバイダ
                asymmetricAlgorithm = new ECDiffieHellmanCng();
            }

            return asymmetricAlgorithm;
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
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider();

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    this.textBox45.Text = "RSAアルゴリズム";
                    this.textBox46.Text = "48 バイトから 2048 バイトのキー長を 1 バイト単位";
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    this.textBox45.Text = "DSAアルゴリズム";
                    this.textBox46.Text = "64 バイトから 128 バイトのキー長を 8 バイト単位";
                }
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    this.textBox45.Text = "ECDSAのCNG実装";
                    this.textBox46.Text = "";
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    this.textBox45.Text = "ECDHアルゴリズムのCNG実装";
                    this.textBox46.Text = "";
                }

                // 公開鍵・暗号化サービスプロバイダの各プロパティを出力
                if (asymmetricAlgorithm != null)
                {
                    this.textBox1.Text = "";
                    KeySizes[] kszs = null;

                    this.textBox1.Text += "・SignatureAlgorithm:" + asymmetricAlgorithm.SignatureAlgorithm.ToString() + "\r\n";

                    if (asymmetricAlgorithm.KeyExchangeAlgorithm != null)
                    {
                        this.textBox1.Text += "・KeyExchangeAlgorithm:" + asymmetricAlgorithm.KeyExchangeAlgorithm.ToString() + "\r\n";
                    }

                    this.textBox1.Text += "\r\n";

                    this.textBox1.Text += "・KeySize:" + asymmetricAlgorithm.KeySize.ToString() + "\r\n";
                    kszs = asymmetricAlgorithm.LegalKeySizes;
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
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider();

                // 公開鍵をXML形式で取得
                this.textBox41b.Text = asymmetricAlgorithm.ToXmlString(false);
                // 秘密鍵をXML形式で取得
                this.textBox41c.Text = asymmetricAlgorithm.ToXmlString(true);
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
        /// <param name="asymmetricAlgorithm">公開鍵・暗号化サービスプロバイダ</param>
        private void SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(AsymmetricAlgorithm asymmetricAlgorithm)
        {
            if (asymmetricAlgorithm is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)asymmetricAlgorithm;

                // 設定する
            }
            else if(asymmetricAlgorithm is DSACryptoServiceProvider)
            {
                DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)asymmetricAlgorithm;

                // 設定する
            }
            else if (asymmetricAlgorithm is ECDsaCng)
            {
                ECDsaCng ecdsa = (ECDsaCng)asymmetricAlgorithm;

                // 設定する
            }
            else if (asymmetricAlgorithm is ECDiffieHellmanCng)
            {
                ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)asymmetricAlgorithm;

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
                byte[] encryptedStringByte = null;
                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] sourceStringByte = Encoding.UTF8.GetBytes(this.textBox41a.Text);

                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider();
                this.SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(asymmetricAlgorithm);

                // 公開鍵
                asymmetricAlgorithm.FromXmlString(this.textBox41b.Text);

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)asymmetricAlgorithm;

                    // 暗号化する（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                    encryptedStringByte = rsa.Encrypt(sourceStringByte, false);
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)asymmetricAlgorithm;

                    // 暗号化する
                    throw new NotImplementedException("DSACryptoServiceProviderの共通鍵暗号化はサポートされていません。");
                }
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    ECDsaCng ecdsa = (ECDsaCng)asymmetricAlgorithm;

                    // 暗号化する
                    throw new NotImplementedException("ECDsaCngの共通鍵暗号化はサポートされていません。");
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)asymmetricAlgorithm;

                    // 暗号化する
                    throw new NotImplementedException("ECDiffieHellmanCngの共通鍵暗号化はサポートされていません。");
                }

                // 結果を表示

                // 生バイト
                this.textBox42.Text = CustomEncode.ToHexString(encryptedStringByte);
                // Base64
                this.textBox43.Text = Convert.ToBase64String(encryptedStringByte);
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
                byte[] encryptedStringByte = Convert.FromBase64String(this.textBox43.Text);
                // 元文字列（バイト配列に）
                byte[] sourceStringByte = null;

                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider();
                this.SetKeyAndInitializationVectorToAsymmetricAlgorithmServiceProvider(asymmetricAlgorithm);

                // 秘密鍵
                asymmetricAlgorithm.FromXmlString(this.textBox41c.Text);

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)asymmetricAlgorithm;

                    // 復号化（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
                    sourceStringByte = rsa.Decrypt(encryptedStringByte, false);
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    DSACryptoServiceProvider dsacsp = (DSACryptoServiceProvider)asymmetricAlgorithm;

                    // 復号化する
                    throw new NotImplementedException("DSACryptoServiceProviderの共通鍵暗号化はサポートされていません。");
                }
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    ECDsaCng ecdsa = (ECDsaCng)asymmetricAlgorithm;

                    // 復号化する
                    throw new NotImplementedException("ECDsaCngの共通鍵暗号化はサポートされていません。");
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)asymmetricAlgorithm;

                    // 復号化する
                    throw new NotImplementedException("ECDiffieHellmanCngの共通鍵暗号化はサポートされていません。");
                }

                // 結果を表示
                this.textBox44.Text = Encoding.UTF8.GetString(sourceStringByte);
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

        /// <summary>署名で使用するAsymmetricAlgorithm</summary>
        private string _signinAsymmetricAlgorithm = "";
        /// <summary>>署名で使用するAsymmetricAlgorithmのHashAlgorithm</summary>
        private string _signinHashAlgorithmOfAsymmetricAlgorithm = "";

        /// <summary>公開鍵・暗号化サービスプロバイダの生成</summary>
        /// <returns>公開鍵・暗号化サービスプロバイダ</returns>
        private AsymmetricAlgorithm CreateAsymmetricAlgorithmServiceProvider2()
        {
            // 公開鍵・暗号化サービスプロバイダ
            AsymmetricAlgorithm asymmetricAlgorithm = null;
            string[] temp = this.comboBox5.SelectedItem.ToString().Split(':');
            this._signinAsymmetricAlgorithm = temp[0];
            this._signinHashAlgorithmOfAsymmetricAlgorithm = temp[1];

            if (this._signinAsymmetricAlgorithm == "RSACryptoServiceProvider")
            {
                // RSACryptoServiceProviderサービスプロバイダ
                asymmetricAlgorithm = new RSACryptoServiceProvider();
            }
            else if (this._signinAsymmetricAlgorithm == "DSACryptoServiceProvider")
            {
                // DSACryptoServiceProviderサービスプロバイダ
                asymmetricAlgorithm = new DSACryptoServiceProvider();
            }
            else if (this._signinAsymmetricAlgorithm == "ECDsaCng")
            {
                // ECDsaCngサービスプロバイダ
                asymmetricAlgorithm = new ECDsaCng();
            }
            else if (this._signinAsymmetricAlgorithm == "ECDiffieHellmanCng")
            {
                // ECDiffieHellmanCngサービスプロバイダ
                asymmetricAlgorithm = new ECDiffieHellmanCng();
            }
            
            return asymmetricAlgorithm;
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
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider2();

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    this.textBox55.Text = "RSAアルゴリズム";
                    this.textBox56.Text = "48 バイトから 2048 バイトのキー長を 1 バイト単位";
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    this.textBox55.Text = "DSAアルゴリズム";
                    this.textBox56.Text = "64 バイトから 128 バイトのキー長を 8 バイト単位";
                }
                
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    this.textBox55.Text = "ECDSAのCNG実装";
                    this.textBox56.Text = "";
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    this.textBox55.Text = "ECDHアルゴリズムのCNG実装";
                    this.textBox56.Text = "";
                }

                // 公開鍵・暗号化サービスプロバイダの各プロパティを出力
                if (asymmetricAlgorithm != null)
                {
                    this.textBox1.Text = "";
                    KeySizes[] kszs = null;

                    this.textBox1.Text += "・SignatureAlgorithm:" + asymmetricAlgorithm.SignatureAlgorithm.ToString() + "\r\n";

                    if (asymmetricAlgorithm.KeyExchangeAlgorithm != null)
                    {
                        this.textBox1.Text += "・KeyExchangeAlgorithm:" + asymmetricAlgorithm.KeyExchangeAlgorithm.ToString() + "\r\n";
                    }

                    this.textBox1.Text += "\r\n";

                    this.textBox1.Text += "・KeySize:" + asymmetricAlgorithm.KeySize.ToString() + "\r\n";
                    kszs = asymmetricAlgorithm.LegalKeySizes;
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

        /// <summary>CngKey</summary>
        private CngKey _cngKey = null;

        /// <summary>鍵の取得</summary>
        private void button50_Click(object sender, EventArgs e)
        {
            try
            {
                // 公開鍵・暗号化サービスプロバイダ
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider2();

                if (asymmetricAlgorithm is RSACryptoServiceProvider
                    || asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    // 公開鍵をXML形式で取得
                    this.textBox51b.Text = asymmetricAlgorithm.ToXmlString(false);
                    // 秘密鍵をXML形式で取得
                    this.textBox51c.Text = asymmetricAlgorithm.ToXmlString(true);
                }
                else
                {
                    byte[] publicKey = null;
                    //byte[] privateKey = null;

                    if (asymmetricAlgorithm is ECDsaCng)
                    {
                        // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                        if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P256")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDsaP256, out this._cngKey, out publicKey);//, out privateKey);
                        }
                        else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P384")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDsaP256, out this._cngKey, out publicKey);//, out privateKey);
                        }
                        else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P521")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDsaP256, out this._cngKey, out publicKey);//, out privateKey);
                        }
                    }
                    else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                    {
                        // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                        if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P256")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDiffieHellmanP256, out this._cngKey, out publicKey);//, out privateKey);
                        }
                        else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P384")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDiffieHellmanP384, out this._cngKey, out publicKey);//, out privateKey);
                        }
                        else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "P521")
                        {
                            this.CreateCngKey(CngAlgorithm.ECDiffieHellmanP521, out this._cngKey, out publicKey);//, out privateKey);
                        }
                    }

                    // 公開鍵
                    this.textBox51b.Text = CustomEncode.ToBase64String(publicKey);
                    // 秘密鍵
                    this.textBox51c.Text = " - cngKey - ";
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
                AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider2();

                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] sourceStringByte  = Encoding.UTF8.GetBytes(this.textBox51a.Text);
                // ハッシュ値
                byte[] hashedSourceStringByte = null;
                // 署名
                byte[] ab_sign = null;

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    // 秘密鍵
                    asymmetricAlgorithm.FromXmlString(this.textBox51c.Text);

                    // キャスト
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)asymmetricAlgorithm;

                    // RSAPKCS1SignatureFormatterオブジェクトを作成
                    RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);

                    // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                    if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "MD5")
                    {
                        rsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = MD5.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA1")
                    {
                        rsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA1.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA256")
                    {
                        rsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA256.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA384")
                    {
                        rsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA384.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA512")
                    {
                        rsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA512.Create().ComputeHash(sourceStringByte);
                    }

                    // 署名を作成
                    ab_sign = rsaFormatter.CreateSignature(hashedSourceStringByte);
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    // 秘密鍵
                    asymmetricAlgorithm.FromXmlString(this.textBox51c.Text);

                    // キャスト
                    DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)asymmetricAlgorithm;

                    // DSASignatureFormatterオブジェクトを作成
                    DSASignatureFormatter dsaFormatter = new DSASignatureFormatter(dsa);

                    // 署名の作成に使用するハッシュアルゴリズムを指定し、ハッシュ値を計算
                    if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "MD5")
                    {
                        dsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = MD5.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA1")
                    {
                        dsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA1.Create().ComputeHash(sourceStringByte);
                    }
                    else if (this._signinHashAlgorithmOfAsymmetricAlgorithm == "SHA256")
                    {
                        dsaFormatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);
                        hashedSourceStringByte = SHA256.Create().ComputeHash(sourceStringByte);
                    }

                    // 署名を作成
                    ab_sign = dsaFormatter.CreateSignature(hashedSourceStringByte);
                }
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    // キャスト
                    ECDsaCng ecdsa = (ECDsaCng)asymmetricAlgorithm;

                    // こんなんで、すいません。
                    using (ecdsa = new ECDsaCng(this._cngKey))
                    {
                        // 署名を作成
                        ab_sign = ecdsa.SignData(sourceStringByte);
                        ecdsa.Clear();
                    }
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    // キャスト
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)asymmetricAlgorithm;

                    throw new NotImplementedException("ECDsaCng:未実装");

                    // ・・・SignDataが無かった。

                    //// こんなんで、すいません。
                    //using (ecdhcng = new ECDiffieHellmanCng(this._cngKey))
                    //{
                    //    // 署名を作成
                    //    ab_sign = ecdhcng.SignData(asb);
                    //    ecdhcng.Clear();
                    //}
                }

                // 結果を表示

                // ハッシュ

                if (hashedSourceStringByte == null)
                {
                    // 生バイト
                    this.textBox52.Text = " - cngKey - ";
                    // Base64
                    this.textBox53.Text = " - cngKey - ";
                }
                else
                {
                    // 生バイト
                    this.textBox52.Text = CustomEncode.ToHexString(hashedSourceStringByte);
                    // Base64
                    this.textBox53.Text = Convert.ToBase64String(hashedSourceStringByte);
                }

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

        /// <summary>CreateCngKey</summary>
        /// <param name="ca">CngAlgorithm</param>
        /// <param name="cngKey">CngKey</param>
        /// <param name="publicKey">publicKey</param>
        private void CreateCngKey(CngAlgorithm cngAlgorithm, out CngKey cngKey, out byte[] publicKey)//, out byte[] privateKey)
        {
            cngKey = CngKey.Create(cngAlgorithm);
            publicKey = cngKey.Export(CngKeyBlobFormat.GenericPublicBlob);

            // ↓サポートされない操作であるらしい。
            //privateKey = cngKey.Export(CngKeyBlobFormat.GenericPrivateBlob);
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
            AsymmetricAlgorithm asymmetricAlgorithm = this.CreateAsymmetricAlgorithmServiceProvider2();

            try
            {
                // 結果フラグ
                bool flg = false;

                // 元文字列をbyte型配列に変換する（UTF-8 Enc）
                byte[] sourceStringByte = Encoding.UTF8.GetBytes(this.textBox51a.Text);

                // ハッシュ値
                byte[] hashStringByte = null;

                if (asymmetricAlgorithm is RSACryptoServiceProvider)
                {
                    // 公開鍵
                    asymmetricAlgorithm.FromXmlString(this.textBox51b.Text);
                    // ハッシュ値を取得
                    hashStringByte = Convert.FromBase64String(this.textBox53.Text);

                    // キャスト
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)asymmetricAlgorithm;

                    // RSAPKCS1SignatureDeformatterオブジェクトを作成
                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);

                    //　検証に使用するハッシュアルゴリズムを指定し
                    // 上記で、ハッシュ値を計算した際と同じアルゴリズムを使用すること。
                    rsaDeformatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);

                    // 検証する
                    flg = rsaDeformatter.VerifySignature(hashStringByte, Convert.FromBase64String(this.textBox55.Text));
                }
                else if (asymmetricAlgorithm is DSACryptoServiceProvider)
                {
                    // 公開鍵
                    asymmetricAlgorithm.FromXmlString(this.textBox51b.Text);
                    // ハッシュ値を取得
                    hashStringByte = Convert.FromBase64String(this.textBox53.Text);

                    // キャスト
                    DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)asymmetricAlgorithm;
                    
                    // DSASignatureFormatterオブジェクトを作成
                    DSASignatureDeformatter dsaSignatureDeformatter = new DSASignatureDeformatter(dsa);

                    //　検証に使用するハッシュアルゴリズムを指定し
                    // 上記で、ハッシュ値を計算した際と同じアルゴリズムを使用すること。
                    dsaSignatureDeformatter.SetHashAlgorithm(this._signinHashAlgorithmOfAsymmetricAlgorithm);

                    // 検証する
                    flg = dsaSignatureDeformatter.VerifySignature(hashStringByte, Convert.FromBase64String(this.textBox55.Text));
                }
                else if (asymmetricAlgorithm is ECDsaCng)
                {
                    // キャスト
                    ECDsaCng ecdsa = (ECDsaCng)asymmetricAlgorithm;

                    // こんなんで、すいません。
                    //using (ecdsa = new ECDsaCng(this._cngKey))

                    // 公開鍵
                    using (ecdsa = new ECDsaCng(CngKey.Import(
                        CustomEncode.FromBase64String(this.textBox51b.Text),
                        CngKeyBlobFormat.GenericPublicBlob)))
                    {
                        // 検証する
                        flg = ecdsa.VerifyData(sourceStringByte, Convert.FromBase64String(this.textBox55.Text));
                        ecdsa.Clear();
                    }
                }
                else if (asymmetricAlgorithm is ECDiffieHellmanCng)
                {
                    // キャスト
                    ECDiffieHellmanCng ecdhcng = (ECDiffieHellmanCng)asymmetricAlgorithm;

                    // 検証する
                    throw new NotImplementedException("ECDiffieHellmanCng:未実装");
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
