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
//* クラス名        ：TestCertificates
//* クラス日本語名  ：署名のテストが使う証明書のパス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（TestPublic の TestCertificates と同じ形）
//**********************************************************************************

using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

namespace TestFramework
{
    /// <summary>署名のテストが使う証明書のパス</summary>
    /// <remarks>
    /// 正本は root/files/resource/X509。csproj がリンクで出力先の X509 フォルダへ複写する。
    /// **鍵の格納先は Exportable だけにする**（MachineKeySet は管理者でないと失敗することがある）。
    /// </remarks>
    internal static class TestCertificates
    {
        /// <summary>*.pfx のパスワード</summary>
        public const string Password = "test";

        /// <summary>鍵の格納先のフラグ</summary>
        public const X509KeyStorageFlags Flags = X509KeyStorageFlags.Exportable;

        /// <summary>出力先の X509 フォルダにあるファイルのパス</summary>
        public static string PathOf(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "X509", fileName);
        }

        /// <summary>RSA（*.cer）</summary>
        public static string RsaCer { get { return PathOf("SHA256RSA.cer"); } }

        /// <summary>RSA（*.pfx）</summary>
        public static string RsaPfx { get { return PathOf("SHA256RSA.pfx"); } }

        /// <summary>ECDSA P-256（*.pfx）</summary>
        public static string EcdsaPfx256 { get { return PathOf("SHA256ECDSA.pfx"); } }

        /// <summary>ECDSA P-256（*.cer）</summary>
        public static string EcdsaCer256 { get { return PathOf("SHA256ECDSA.cer"); } }

        /// <summary>RSA の秘密鍵（*.pfx から）</summary>
        public static RSA RsaPrivateKey()
        {
            return LoadPfx(RsaPfx).GetRSAPrivateKey();
        }

        /// <summary>RSA の公開鍵（*.cer から）</summary>
        public static RSA RsaPublicKey()
        {
#if NETCOREAPP
            return X509CertificateLoader.LoadCertificateFromFile(RsaCer).GetRSAPublicKey();
#else
            return new X509Certificate2(RsaCer).GetRSAPublicKey();
#endif
        }

        /// <summary>*.pfx を読む（net10.0 は X509CertificateLoader。コンストラクタは旧式扱い）</summary>
        public static X509Certificate2 LoadPfx(string path)
        {
#if NETCOREAPP
            return X509CertificateLoader.LoadPkcs12FromFile(path, Password, Flags);
#else
            return new X509Certificate2(path, Password, Flags);
#endif
        }
    }
}
