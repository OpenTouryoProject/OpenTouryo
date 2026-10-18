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
//* クラス日本語名  ：署名・暗号のテストが使う証明書のパス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.IO;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

namespace TestPublic.Security
{
    /// <summary>署名・暗号のテストが使う証明書のパス</summary>
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

        /// <summary>*.cer を読む（net10.0 は X509CertificateLoader。コンストラクタは旧式扱い）</summary>
        public static X509Certificate2 LoadCer(string path)
        {
#if NETCOREAPP
            return X509CertificateLoader.LoadCertificateFromFile(path);
#else
            return new X509Certificate2(path);
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

        /// <summary>出力先の X509 フォルダにあるファイルのパス</summary>
        public static string PathOf(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "X509", fileName);
        }

        /// <summary>RSA（*.cer）</summary>
        public static string RsaCer { get { return PathOf("SHA256RSA.cer"); } }

        /// <summary>RSA（*.pfx）</summary>
        public static string RsaPfx { get { return PathOf("SHA256RSA.pfx"); } }

        /// <summary>DSA（*.cer）</summary>
        public static string DsaCer { get { return PathOf("SHA256DSA.cer"); } }

        /// <summary>DSA（*.pfx）</summary>
        public static string DsaPfx { get { return PathOf("SHA256DSA.pfx"); } }

        /// <summary>ECDSA P-256（*.cer）</summary>
        public static string EcdsaCer256 { get { return PathOf("SHA256ECDSA.cer"); } }

        /// <summary>ECDSA P-256（*.pfx）</summary>
        public static string EcdsaPfx256 { get { return PathOf("SHA256ECDSA.pfx"); } }

        /// <summary>ECDSA P-384（*.cer）</summary>
        public static string EcdsaCer384 { get { return PathOf("SHA384ECDSA.cer"); } }

        /// <summary>ECDSA P-384（*.pfx）</summary>
        public static string EcdsaPfx384 { get { return PathOf("SHA384ECDSA.pfx"); } }

        /// <summary>ECDSA P-521（*.cer）</summary>
        public static string EcdsaCer521 { get { return PathOf("SHA521ECDSA.cer"); } }

        /// <summary>ECDSA P-521（*.pfx）</summary>
        public static string EcdsaPfx521 { get { return PathOf("SHA521ECDSA.pfx"); } }
    }
}
