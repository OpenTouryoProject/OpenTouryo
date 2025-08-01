using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// https://www.nuget.org/packages/jose-jwt/
using Jose;
// https://github.com/dvsekhvalnov/jose-jwt/tree/master/jose-jwt/Security/Cryptography
//using Security.Cryptography;
using Jose.keys;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Aead;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Security.KeyExg;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Public.Security.Xml;
using TIPD = Touryo.Infrastructure.Public.Diagnostics;

namespace EncAndDecUtilCUI
{
    /// <summary>
    /// - jose-jwt - マイクロソフト系技術情報 Wiki
    ///   https://techinfoofmicrosofttech.osscons.jp/index.php?jose-jwt
    /// - Certificates
    ///   https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/files/resource/X509
    /// </summary>
    public class Program
    {
        /// <summary>PfxPassword</summary>
        private static string PfxPassword = "test";
        /// <summary>RsaのX509証明書のパス（*.pfx）</summary>
        private static string PrivateRsaX509Path = @"SHA256RSA.pfx";
        /// <summary>RsaのX509証明書のパス（*.cer）</summary>
        private static string PublicRsaX509Path = @"SHA256RSA.cer";
        
        /// <summary>DsaのX509証明書のパス（*.pfx）</summary>
        private static string PrivateDsaX509Path = @"SHA256DSA.pfx";
        /// <summary>DsaのX509証明書のパス（*.cer）</summary>
        private static string PublicDsaX509Path = @"SHA256DSA.cer";

        // どうも証明書をnistP256, P384, P521用に生成しないとダメらしい。
        // prime256v1, secp384r1, secp521r1

        /// <summary>ECDsaのX509証明書のパス（*.pfx）</summary>
        private static string PrivateECDsaX509_256Path = @"SHA256ECDSA.pfx";
        /// <summary>ECDsaのX509証明書のパス（*.cer）</summary>
        private static string PublicECDsaX509_256Path = @"SHA256ECDSA.cer";
        /// <summary>ECDsaのX509証明書のパス（*.pfx）</summary>
        private static string PrivateECDsaX509_384Path = @"SHA384ECDSA.pfx";
        /// <summary>ECDsaのX509証明書のパス（*.cer）</summary>
        private static string PublicECDsaX509_384Path = @"SHA384ECDSA.cer";
        /// <summary>ECDsaのX509証明書のパス（*.pfx）</summary>
        private static string PrivateECDsaX509_512Path = @"SHA521ECDSA.pfx";
        /// <summary>ECDsaのX509証明書のパス（*.cer）</summary>
        private static string PublicECDsaX509_512Path = @"SHA521ECDSA.cer";

        public static void Main(string[] args)
        {
            // configの初期化(無くても動くようにせねば。)
            //GetConfigParameter.InitConfiguration("appsettings.json");

            try
            {
                // Hash
                Program.Hash();
                Program.KeyedHash();
                Program.PasswordHash();
                TIPD.MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                // Cryptography
                Program.PrivateKeyCryptography();
                Program.PublicKeyCryptography();
                TIPD.MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                // Jwt(Jws, Jwe)
                Program.MyJwt();
                Program.JoseJwt();

                // SignedXml 
                Program.SignedXml();

                // Others

                // echoすると例外
                try
                {
                    Console.ReadKey();
                }
                catch { }
            }
            catch (Exception ex)
            {
                TIPD.MyDebug.OutputDebugAndConsole(ex.ToString());
            }
        }

        #region Test (Hash, Cryptography, Jwt, Xml)

        #region Hash

        /// <summary>Hash</summary>
        private static void Hash()
        {
            string data = "ynyKeiR9FXWPkNQHvUPZkAlfUmouExBv";

            #region Managed or CSP
            // Default
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.Default", GetHash.GetHashString(data, EnumHashAlgorithm.Default));
            // MD5
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.MD5", GetHash.GetHashString(data, EnumHashAlgorithm.MD5));
            // RIPEMD160
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.RIPEMD160_M", GetHash.GetHashString(data, EnumHashAlgorithm.RIPEMD160_M));
            // SHA1
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA1", GetHash.GetHashString(data, EnumHashAlgorithm.SHA1));
            // SHA256
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA256", GetHash.GetHashString(data, EnumHashAlgorithm.SHA256));
            // SHA384
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA384", GetHash.GetHashString(data, EnumHashAlgorithm.SHA384));
            // SHA512
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA512", GetHash.GetHashString(data, EnumHashAlgorithm.SHA512));
            #endregion

#if NETCOREAPP
#else
            #region CNG
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.MD5_CNG", GetHash.GetHashString(data, EnumHashAlgorithm.MD5_CNG));
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA1_CNG", GetHash.GetHashString(data, EnumHashAlgorithm.SHA1_CNG));
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA256_CNG", GetHash.GetHashString(data, EnumHashAlgorithm.SHA256_CNG));
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA384_CNG", GetHash.GetHashString(data, EnumHashAlgorithm.SHA384_CNG));
            TIPD.MyDebug.OutputDebugAndConsole("HashAlgorithm.SHA512_CNG", GetHash.GetHashString(data, EnumHashAlgorithm.SHA512_CNG));
            #endregion
#endif
        }

        /// <summary>KeyedHash</summary>
        private static void KeyedHash()
        {
            string data = "ynyKeiR9FXWPkNQHvUPZkAlfUmouExBv";
            string key = "ynyKeiR9FXWPkNQHvUPZkAlfUmouExBv";

            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.Default", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.Default, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACMD5", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACMD5, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACRIPEMD160", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACRIPEMD160, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACSHA1", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACSHA1, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACSHA256", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACSHA256, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACSHA384", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACSHA384, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.HMACSHA512", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.HMACSHA512, key));
            TIPD.MyDebug.OutputDebugAndConsole("KeyedHashAlgorithm.MACTripleDES", GetKeyedHash.GetKeyedHashString(data, EnumKeyedHashAlgorithm.MACTripleDES, key));
        }

        /// <summary>PasswordHash</summary>
        private static void PasswordHash()
        {
            bool ret = false;
            string providedPassword = "";
            string hashedPassword = "";
            string key = "";

            #region 互換性確認
            // 旧版の情報
            providedPassword = "Bx@A]p7u";
            hashedPassword = "$1$.JU5JJFZXQVIqWQ==.Rkg3YW5WVWt4YQ==.MTAwMA==.ODU0MXJPRWJmVzA9";
            key = "%NI$VWAR*Y";

            ret = GetPasswordHashV1.EqualSaltedPassword(
                providedPassword,
                hashedPassword.Substring(4),
                EnumKeyedHashAlgorithm.MACTripleDES, HashAlgorithmName.SHA1);

            TIPD.MyDebug.OutputDebugAndConsole("GetPasswordHashV1.EqualSaltedPassword (old)", ret.ToString());

            ret = GetPasswordHashV2.EqualSaltedPassword(
                providedPassword,
                hashedPassword.Substring(4),
                EnumKeyedHashAlgorithm.MACTripleDES, HashAlgorithmName.SHA1);

            TIPD.MyDebug.OutputDebugAndConsole("GetPasswordHashV2.EqualSaltedPassword (old)", ret.ToString());
            #endregion

            #region 可逆性確認
            // MACTripleDES
            hashedPassword = GetPasswordHashV2.GetSaltedPassword(
                    providedPassword,                    // password
                    EnumKeyedHashAlgorithm.MACTripleDES, // algorithm
                    key,                                 // key(pwd)
                    10,                                  // salt length
                    1000                                 // stretch count
                );

            ret = GetPasswordHashV2.EqualSaltedPassword(
                providedPassword,
                hashedPassword,
                EnumKeyedHashAlgorithm.MACTripleDES);

            TIPD.MyDebug.OutputDebugAndConsole("GetPasswordHashV2.EqualSaltedPassword (new)", ret.ToString());

            // HMACSHA512
            hashedPassword = GetPasswordHashV2.GetSaltedPassword(
                    providedPassword,                    // password
                    EnumKeyedHashAlgorithm.HMACSHA512,   // algorithm
                    key,                                 // key(pwd)
                    10,                                  // salt length
                    1000                                 // stretch count
                );

            ret = GetPasswordHashV2.EqualSaltedPassword(
                providedPassword,
                hashedPassword,
                EnumKeyedHashAlgorithm.HMACSHA512);

            TIPD.MyDebug.OutputDebugAndConsole("GetPasswordHashV2.EqualSaltedPassword (new)", ret.ToString());
            #endregion
        }

        #endregion

        #region Cryptography

        #region PrivateKey

        /// <summary>PrivateKeyCryptography</summary>
        private static void PrivateKeyCryptography()
        {
        }

        #endregion

        #region PublicKey

        /// <summary>PublicKeyCryptography</summary>
        private static void PublicKeyCryptography()
        {
            #region Variables

            #region Env
            OperatingSystem os = Environment.OSVersion;

            // https://github.com/dotnet/corefx/issues/29404#issuecomment-385287947
            //   *.pfxから証明書を開く場合、X509KeyStorageFlags.Exportableの指定が必要な場合がある。
            //   Linuxのキーは常にエクスポート可能だが、WindowsやMacOSでは必ずしもそうではない。
            X509KeyStorageFlags x509KSF = 0;
            if (os.Platform == PlatformID.Win32NT)
            {
                x509KSF = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
            }
            else //if (os.Platform == PlatformID.Unix)
            {
                x509KSF = X509KeyStorageFlags.DefaultKeySet;
            }
            #endregion

            #region Keys
            X509Certificate2 publicX509Key = null;
            X509Certificate2 privateX509Key = null;

            #endregion

            #region DigitalSign
            string moji = "hogehoge";
            byte[] data = CustomEncode.StringToByte(moji, CustomEncode.UTF_8);
            byte[] sign = null;
            #endregion

            #endregion

            #region Test of the X.509 Certificates

            #region RSA
            privateX509Key = new X509Certificate2(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            publicX509Key = new X509Certificate2(Program.PublicRsaX509Path, "", x509KSF);

            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPrivateX509Key("RSA", privateX509Key);
            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPublicX509Key("RSA", publicX509Key);
            #endregion

#if NETCOREAPP || NET47
            #region DSA
            // https://github.com/dotnet/corefx/issues/18733#issuecomment-296723615

            privateX509Key = new X509Certificate2(Program.PrivateDsaX509Path, Program.PfxPassword);
            publicX509Key = new X509Certificate2(Program.PublicDsaX509Path, "");

            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPrivateX509Key("DSA", privateX509Key);
            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPublicX509Key("DSA", publicX509Key);

            DSA privateDSA = privateX509Key.GetDSAPrivateKey();
            TIPD.MyDebug.OutputDebugAndConsole("privateDSA", (privateDSA == null ? "is null" : "is not null"));
            //DSA publicDSA = null; // publicX509Key.GetDSAPublicKey(); // Internal.Cryptography.CryptoThrowHelper.WindowsCryptographicException
            #endregion

            #region ECDsa
            // https://github.com/dotnet/corefx/issues/18733#issuecomment-296723615
            privateX509Key = new X509Certificate2(Program.PrivateECDsaX509_256Path, Program.PfxPassword);
            publicX509Key = new X509Certificate2(Program.PublicECDsaX509_256Path, "");

            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPrivateX509Key("ECDsa", privateX509Key);
            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectPublicX509Key("ECDsa", publicX509Key);

            ECDsa privateECDsa = privateX509Key.GetECDsaPrivateKey();
            TIPD.MyDebug.OutputDebugAndConsole("privateECDsa", (privateECDsa == null ? "is null" : "is not null"));
            ECDsa publicECDsa = publicX509Key.GetECDsaPublicKey();
            TIPD.MyDebug.OutputDebugAndConsole("publicECDsa", (publicECDsa == null ? "is null" : "is not null"));

            #endregion
#endif

            #endregion

            #region Test of the OpenTouryo.Public.Security.ASymCrypt

            ASymmetricCryptography ascPublic = new ASymmetricCryptography(
                EnumASymmetricAlgorithm.X509, Program.PublicRsaX509Path, "", x509KSF);

            string temp = ascPublic.EncryptString(moji);

            ASymmetricCryptography ascPrivate = new ASymmetricCryptography(
                EnumASymmetricAlgorithm.X509, Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);

            temp = ascPrivate.DecryptString(temp);

            TIPD.MyDebug.OutputDebugAndConsole("ASymCrypt(X509).Enc&Dec", (temp == moji).ToString());

            #endregion

            #region Test of the OpenTouryo.Public.Security.DigitalSign

            // RSA, DSA
            DigitalSignX509 dsX509 = null;
            DigitalSignParam dsParam = null;
            DigitalSignXML dsXML = null;

            // ECDsa
#if NETCOREAPP || NET47
            DigitalSignECDsaX509 dsECDsaX509 = null;
            DigitalSignECDsaCng dsECDsaCng = null;
#endif
#if NETCOREAPP
            DigitalSignECDsaOpenSsl dsECDsaOpenSsl = null;
#endif

            if (os.Platform == PlatformID.Win32NT)
            {
                #region RSA
                // X509
                dsX509 = new DigitalSignX509(Program.PrivateRsaX509Path, Program.PfxPassword, "SHA256", x509KSF);
                sign = dsX509.Sign(data);

                dsX509 = new DigitalSignX509(Program.PublicRsaX509Path, "", "SHA256", x509KSF);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignX509.Verify(RS256)", dsX509.Verify(data, sign).ToString());

                // Param
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.Rsa_SHA256);
                sign = dsParam.Sign(data);

                dsParam = new DigitalSignParam((RSAParameters)dsParam.PublicKey, EnumDigitalSignAlgorithm.Rsa_SHA256);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(RS256)", dsParam.Verify(data, sign).ToString());

                // XML
                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.Rsa_SHA256);
                sign = dsXML.Sign(data);

#if !NETCOREAPP
                // NETCOREでは、XML鍵のExportが動かない。
                dsXML = new DigitalSignXML(dsXML.PublicKey, EnumDigitalSignAlgorithm.Rsa_SHA256);
#endif
                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignXML.Verify(RS256)", dsXML.Verify(data, sign).ToString());
                #endregion

                #region DSA
                // DSAはFormatterバージョンしか動かない。
                // また、WinではDSAのX509が処理できない（Linux上では動作することを確認済み）。
                //dsX509 = new DigitalSignX509(Program.PrivateDsaX509Path, Program.PfxPassword , "SHA256", x509KSF);
                //sign = dsX509.Sign(data);
                //MyDebug.OutputDebugAndConsole("DigitalSignX509.Verify(DSA-SHA256)", dsX509.Verify(data, sign).ToString());

                // Param
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
                sign = dsParam.SignByFormatter(data);

                dsParam = new DigitalSignParam((DSAParameters)dsParam.PublicKey, EnumDigitalSignAlgorithm.DsaCSP_SHA1);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(DSA-SHA1)", dsParam.VerifyByDeformatter(data, sign).ToString());

                // XML
                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
                sign = dsXML.SignByFormatter(data);

#if !NETCOREAPP
                // NETCOREでは、XML鍵のExportが動かない。
                dsXML = new DigitalSignXML(dsXML.PublicKey, EnumDigitalSignAlgorithm.DsaCSP_SHA1);
#endif
                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignXML.Verify(DSA-SHA1)", dsXML.VerifyByDeformatter(data, sign).ToString());
                #endregion

                #region ECDsa
                #region 256
#if NETCOREAPP || NET47
                // X509
                dsECDsaX509 = new DigitalSignECDsaX509(Program.PrivateECDsaX509_256Path, Program.PfxPassword, HashAlgorithmName.SHA256);
                sign = dsECDsaX509.Sign(data);

                dsECDsaX509 = new DigitalSignECDsaX509(Program.PublicECDsaX509_256Path, "", HashAlgorithmName.SHA256);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignECDsaX509.Verify(ECDSA-SHA256)", dsECDsaX509.Verify(data, sign).ToString());

#if NET47 || NETCOREAPP3_0
                // Param
                dsECDsaCng = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P256);
                sign = dsECDsaCng.Sign(data);

                dsECDsaCng = new DigitalSignECDsaCng(dsECDsaCng.PublicKey);
                MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(ECDSA-P256)", dsECDsaCng.Verify(data, sign).ToString());
#endif
#endif
                #endregion

                #region 512
#if NETCOREAPP || NET47
                // X509
                dsECDsaX509 = new DigitalSignECDsaX509(Program.PrivateECDsaX509_256Path, Program.PfxPassword, HashAlgorithmName.SHA512);
                sign = dsECDsaX509.Sign(data);

                dsECDsaX509 = new DigitalSignECDsaX509(Program.PublicECDsaX509_256Path, "", HashAlgorithmName.SHA512);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignECDsaX509.Verify(ECDSA-SHA512)", dsECDsaX509.Verify(data, sign).ToString());

#if NET47 || NETCOREAPP3_0
                // Param
                dsECDsaCng = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P521);
                sign = dsECDsaCng.Sign(data);

                dsECDsaCng = new DigitalSignECDsaCng(dsECDsaCng.PublicKey);
                MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(ECDSA-P521)", dsECDsaCng.Verify(data, sign).ToString());
#endif
#endif
                #endregion
                #endregion
            }
            else //if (os.Platform == PlatformID.Unix)
            {
#if NETCOREAPP
                #region RSA
                // X509
                dsX509 = new DigitalSignX509(Program.PrivateRsaX509Path, Program.PfxPassword, "SHA256");
                sign = dsX509.Sign(data);

                dsX509 = new DigitalSignX509(Program.PublicRsaX509Path, "", "SHA256");

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignX509.Verify(RS256)", dsX509.Verify(data, sign).ToString());

                // Param
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.Rsa_SHA256);
                sign = dsParam.Sign(data);

                dsParam = new DigitalSignParam((RSAParameters)dsParam.PublicKey, EnumDigitalSignAlgorithm.Rsa_SHA256);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(RS256)", dsParam.Verify(data, sign).ToString());

                // XML
                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.Rsa_SHA256);
                sign = dsXML.Sign(data);

                //dsXML = new DigitalSignXML(dsXML.PublicKey, EnumDigitalSignAlgorithm.RsaOpenSsl_SHA256); // 動かない
                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignXML.Verify(RS256)", dsXML.Verify(data, sign).ToString());
                #endregion

                #region DSA
                // X509
                dsX509 = new DigitalSignX509(Program.PrivateDsaX509Path, Program.PfxPassword, "SHA256");
                sign = dsX509.Sign(data);

                dsX509 = new DigitalSignX509(Program.PublicDsaX509Path, "", "SHA256");

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignX509.Verify(DSA-SHA256)", dsX509.Verify(data, sign).ToString());

                // Param
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1);
                sign = dsParam.Sign(data);

                dsParam = new DigitalSignParam((DSAParameters)dsParam.PublicKey, EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(DSA-SHA1)", dsParam.Verify(data, sign).ToString());

                // XML
                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1);
                sign = dsXML.Sign(data);

                //dsXML = new DigitalSignXML(dsXML.PublicKey, EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1); // 動かない

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignXML.Verify(DSA-SHA1)", dsXML.Verify(data, sign).ToString());
                #endregion

                #region ECDsa (.NET Core on Linux)
                // X509
                dsECDsaX509 = new DigitalSignECDsaX509(Program.PrivateECDsaX509_256Path, Program.PfxPassword, HashAlgorithmName.SHA256);
                sign = dsECDsaX509.Sign(data);

                dsECDsaX509 = new DigitalSignECDsaX509(Program.PublicECDsaX509_256Path, "", HashAlgorithmName.SHA256);

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignECDsaX509.Verify(ECDSA)", dsECDsaX509.Verify(data, sign).ToString());

                // Param
                dsECDsaOpenSsl = new DigitalSignECDsaOpenSsl(
                    EnumDigitalSignAlgorithm.ECDsaOpenSsl_P256, SHA256.Create());
                sign = dsECDsaOpenSsl.Sign(data);

                dsECDsaOpenSsl = new DigitalSignECDsaOpenSsl(
                    dsECDsaOpenSsl.PublicKey.Value, SHA256.Create());

                TIPD.MyDebug.OutputDebugAndConsole("DigitalSignParam.Verify(ECDSA-P256)", dsParam.Verify(data, sign).ToString());
                #endregion
#endif
            }
            #endregion
        }

        #endregion

        #endregion

        #region Jwt

        #region MyJwt

        /// <summary>MyJwt</summary>
        private static void MyJwt()
        {
            #region Variables

            string temp = "";
            bool ret = false;

            #region Env
            OperatingSystem os = Environment.OSVersion;

            // https://github.com/dotnet/corefx/issues/29404#issuecomment-385287947
            //   *.pfxから証明書を開く場合、X509KeyStorageFlags.Exportableの指定が必要な場合がある。
            //   Linuxのキーは常にエクスポート可能だが、WindowsやMacOSでは必ずしもそうではない。
            X509KeyStorageFlags x509KSF = 0;
            if (os.Platform == PlatformID.Win32NT)
            {
                x509KSF = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
            }
            else //if (os.Platform == PlatformID.Unix)
            {
                x509KSF = X509KeyStorageFlags.DefaultKeySet;
            }
            #endregion

            #region Token
            string token = "";

            IDictionary<string, object> payload = null;
            payload = new Dictionary<string, object>()
                {
                    { "sub", "mr.x@contoso.com" },
                    { "exp", 1300819380 }
                };

            string payloadString = JsonConvert.SerializeObject(payload);
            #endregion

            #region Keys
            byte[] key = null;
            string jwk = "";
            RsaPublicKeyConverter rpkc = null;
#if NETCOREAPP || NET47
            EccPublicKeyConverter epkc = null;
#endif
            #endregion

            #region Jws
            // RS256
            JWS_RS256_X509 jWS_RS256_X509 = null;
            JWS_RS256_Param jWS_RS256_Param = null;

            // RS384
            JWS_RS384_XML jWS_RS384_XML = null;
            JWS_RS384_Param jWS_RS384_Param = null;

            // RS512
            JWS_RS512_X509 jWS_RS512_X509 = null;
            JWS_RS512_Param jWS_RS512_Param = null;

#if NETCOREAPP || NET47
            // ES256
            JWS_ES256_X509 jWS_ES256_X509 = null;
            JWS_ES256_Param jWS_ES256_Param = null;

            // ES384
            JWS_ES384_X509 jWS_ES384_X509 = null;
            JWS_ES384_Param jWS_ES384_Param = null;

            // ES512
            JWS_ES512_X509 jWS_ES512_X509 = null;
            JWS_ES512_Param jWS_ES512_Param = null;
#endif
            #endregion

            #region Jwe
            Touryo.Infrastructure.Public.Security.Jwt.JWE jwe = null;
            #endregion

            #endregion

            #region Jws
            if (os.Platform == PlatformID.Win32NT)
            {
                #region HMACSHA(HS)
                key = CustomEncode.StringToByte("てすとてすとてすとてすと", CustomEncode.UTF_8);

                // HS256 署名・検証
                JWS_HS256 jWS_HS256 = new JWS_HS256(key);
                token = jWS_HS256.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_HS256.Create", token);
                TIPD.MyDebug.OutputDebugAndConsole("JWS_HS256.Verify", jWS_HS256.Verify(token).ToString());

                // HS384 署名・検証
                JWS_HS384 jWS_HS384 = new JWS_HS384(key);
                token = jWS_HS384.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_HS384.Create", token);
                TIPD.MyDebug.OutputDebugAndConsole("JWS_HS384.Verify", jWS_HS384.Verify(token).ToString());

                // HS512 署名・検証
                JWS_HS512 jWS_HS512 = new JWS_HS512(key);
                token = jWS_HS512.Create(payloadString);

                Touryo.Infrastructure.Public.Security.MyDebug.InspectJwt("JWS_HS512.Create", token);
                TIPD.MyDebug.OutputDebugAndConsole("JWS_HS512.Verify", jWS_HS512.Verify(token).ToString());

                // JWKを使用
                jWS_HS512 = new JWS_HS512(jWS_HS512.JWK);
                token = jWS_HS512.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_HS512.Create with JWK", token);
                TIPD.MyDebug.OutputDebugAndConsole("JWS_HS512.Verify with JWK", jWS_HS512.Verify(token).ToString());
                #endregion

                #region RSA(RS)

                #region 256
                // 署名（X509）
                jWS_RS256_X509 = new JWS_RS256_X509(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
                token = jWS_RS256_X509.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_RS256_X509.Create", token);

                // 鍵の相互変換
                rpkc = new RsaPublicKeyConverter(JWS_RSA.RS._256);
                jwk = rpkc.ParamToJwk(((RSA)jWS_RS256_X509.DigitalSignX509.AsymmetricAlgorithm).ExportParameters(false));

                TIPD.MyDebug.OutputDebugAndConsole("RSA JWK", jwk);

                // 検証（X509）
                jWS_RS256_X509 = new JWS_RS256_X509(Program.PublicRsaX509Path, "", x509KSF);

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS256_X509.Verify", jWS_RS256_X509.Verify(token).ToString());

                // 検証（Param）
                jWS_RS256_Param = new JWS_RS256_Param(rpkc.JwkToParam(jwk));

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS256_Param.Verify", jWS_RS256_Param.Verify(token).ToString());
                #endregion
#if NETCOREAPP
#else
                #region 384
                // 署名（XML）
                jWS_RS384_XML = new JWS_RS384_XML();
                token = jWS_RS384_XML.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("jWS_RS384_XML.Create", token);

                // 鍵の相互変換
                rpkc = new RsaPublicKeyConverter(JWS_RSA.RS._384);
                jwk = rpkc.XmlToJwk(jWS_RS384_XML.XMLPrivateKey);

                TIPD.MyDebug.OutputDebugAndConsole("RSA JWK", jwk);

                // 検証（XML）
                //jWS_RS384_XML = new JWS_RS384_XML(jWS_RS384_XML.XMLPrivateKey);
                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS384_XML.Verify", jWS_RS384_XML.Verify(token).ToString());

                // 検証（Param）
                jWS_RS384_Param = new JWS_RS384_Param(rpkc.JwkToParam(jwk));
                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS384_Param.Verify", jWS_RS384_Param.Verify(token).ToString());
                #endregion

                #region 512
                // 署名（X509）
                jWS_RS512_X509 = new JWS_RS512_X509(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
                token = jWS_RS512_X509.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_RS512_X509.Create", token);

                // 鍵の相互変換
                rpkc = new RsaPublicKeyConverter(JWS_RSA.RS._512);
                jwk = rpkc.ParamToJwk(((RSA)jWS_RS512_X509.DigitalSignX509.AsymmetricAlgorithm).ExportParameters(false));

                TIPD.MyDebug.OutputDebugAndConsole("RSA JWK", jwk);

                // 検証（X509）
                jWS_RS512_X509 = new JWS_RS512_X509(Program.PublicRsaX509Path, "", x509KSF);

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS512_X509.Verify", jWS_RS512_X509.Verify(token).ToString());

                // 検証（Param）
                jWS_RS512_Param = new JWS_RS512_Param(rpkc.JwkToParam(jwk));

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS512_Param.Verify", jWS_RS512_Param.Verify(token).ToString());
                #endregion
#endif
                #endregion

                // DSA

#if NETCOREAPP || NET47
                #region ECDsa(ES)

                #region 256
                // 署名（X509）
                jWS_ES256_X509 = new JWS_ES256_X509(Program.PrivateECDsaX509_256Path, Program.PfxPassword);
                token = jWS_ES256_X509.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_ES256_X509.Create", token);

                // 鍵の相互変換
                epkc = new EccPublicKeyConverter(JWS_ECDSA.ES._256);
                jwk = epkc.ParamToJwk(
                    ((ECDsa)jWS_ES256_X509.DigitalSignECDsaX509.AsymmetricAlgorithm).ExportParameters(false));

                TIPD.MyDebug.OutputDebugAndConsole("ECDSA JWK", jwk);

                // 検証（X509）
                jWS_ES256_X509 = new JWS_ES256_X509(Program.PublicECDsaX509_256Path, "");

                TIPD.MyDebug.OutputDebugAndConsole("JWS_ES256_X509.Verify", jWS_ES256_X509.Verify(token).ToString());

#if NET47 || NETCOREAPP3_0
                // 検証（Param）
                //// Core2.0-2.2 on WinでVerifyがエラーになる。
                //// DigitalSignECDsaOpenSslを試してみるが生成できない、
                //// Core on Win に OpenSSLベースのプロバイダは無いため）
                jWS_ES256_Param = new JWS_ES256_Param(epkc.JwkToParam(jwk), false);
                MyDebug.OutputDebugAndConsole("JWS_ES256_Param.Verify", jWS_ES256_Param.Verify(token).ToString());
#elif NETCOREAPP2_0
                // Core2.0-2.2 on Winで ECDsaCngは動作しない。
#endif
                // ★ xLibTest
                Program.VerifyResultJwt("JwsAlgorithm.xLibTest", token, jWS_ES256_X509.DigitalSignECDsaX509.AsymmetricAlgorithm, JwsAlgorithm.ES256);
                #endregion
#if NETCOREAPP
#else
                #region 384
                // 署名（X509）
                jWS_ES384_X509 = new JWS_ES384_X509(Program.PrivateECDsaX509_384Path, Program.PfxPassword);
                token = jWS_ES384_X509.Create(payloadString);
                MyDebug.InspectJwt("JWS_ES384_X509.Create", token);

                // 検証（X509）
                jWS_ES384_X509 = new JWS_ES384_X509(Program.PublicECDsaX509_384Path, "");
                MyDebug.OutputDebugAndConsole("JWS_ES384_X509.Verify", jWS_ES384_X509.Verify(token).ToString());

                // 鍵の相互変換
                epkc = new EccPublicKeyConverter(JWS_ECDSA.ES._384);
                jwk = epkc.ParamToJwk(
                    ((ECDsa)jWS_ES384_X509.DigitalSignECDsaX509.AsymmetricAlgorithm).ExportParameters(false));

                MyDebug.OutputDebugAndConsole("ECDSA JWK", jwk);

                // 検証（X509）
                jWS_ES384_X509 = new JWS_ES384_X509(Program.PublicECDsaX509_384Path, "");
                MyDebug.OutputDebugAndConsole("JWS_ES384_X509.Verify", jWS_ES384_X509.Verify(token).ToString());

#if NET47 || NETCOREAPP3_0
                // 検証（Param）
                //// Core2.0-2.2 on WinでVerifyがエラーになる。
                //// DigitalSignECDsaOpenSslを試してみるが生成できない、
                //// Core on Win に OpenSSLベースのプロバイダは無いため）
                jWS_ES384_Param = new JWS_ES384_Param(epkc.JwkToParam(jwk), false);
                MyDebug.OutputDebugAndConsole("JWS_ES384_Param.Verify", jWS_ES384_Param.Verify(token).ToString());
#elif NETCOREAPP2_0
                // Core2.0-2.2 on Winで ECDsaCngは動作しない。
#endif
                // ★ xLibTest
                Program.VerifyResultJwt("JwsAlgorithm.xLibTest", token, jWS_ES384_X509.DigitalSignECDsaX509.AsymmetricAlgorithm, JwsAlgorithm.ES384);
                #endregion

                #region 512
                // 署名（X509）
                jWS_ES512_X509 = new JWS_ES512_X509(Program.PrivateECDsaX509_512Path, Program.PfxPassword);
                token = jWS_ES512_X509.Create(payloadString);
                MyDebug.InspectJwt("JWS_ES512_X509.Create", token);

                // 検証（X509）
                jWS_ES512_X509 = new JWS_ES512_X509(Program.PublicECDsaX509_512Path, "");
                MyDebug.OutputDebugAndConsole("JWS_ES512_X509.Verify", jWS_ES512_X509.Verify(token).ToString());

                // 鍵の相互変換
                epkc = new EccPublicKeyConverter(JWS_ECDSA.ES._512);
                jwk = epkc.ParamToJwk(
                    ((ECDsa)jWS_ES512_X509.DigitalSignECDsaX509.AsymmetricAlgorithm).ExportParameters(false));

                MyDebug.OutputDebugAndConsole("ECDSA JWK", jwk);

                // 検証（X509）
                jWS_ES512_X509 = new JWS_ES512_X509(Program.PublicECDsaX509_512Path, "");
                MyDebug.OutputDebugAndConsole("JWS_ES512_X509.Verify", jWS_ES512_X509.Verify(token).ToString());

#if NET47 || NETCOREAPP3_0
                // 検証（Param）
                //// Core2.0-2.2 on WinでVerifyがエラーになる。
                //// DigitalSignECDsaOpenSslを試してみるが生成できない、
                //// Core on Win に OpenSSLベースのプロバイダは無いため）
                jWS_ES512_Param = new JWS_ES512_Param(epkc.JwkToParam(jwk), false);
                MyDebug.OutputDebugAndConsole("JWS_ES512_Param.Verify", jWS_ES512_Param.Verify(token).ToString());
#elif NETCOREAPP2_0
                // Core2.0-2.2 on Winで ECDsaCngは動作しない。
#endif
                // ★ xLibTest
                Program.VerifyResultJwt("JwsAlgorithm.xLibTest", token, jWS_ES512_X509.DigitalSignECDsaX509.AsymmetricAlgorithm, JwsAlgorithm.ES512);
                #endregion
#endif
                #endregion
#endif
            }
            else //if (os.Platform == PlatformID.Unix)
            {
#if NETCOREAPP
                #region RSA(RS256)
                // 署名（X509）
                jWS_RS256_X509 = new JWS_RS256_X509(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
                token = jWS_RS256_X509.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_RS256_X509.Create", token);

                // 鍵の相互変換
                rpkc = new RsaPublicKeyConverter(JWS_RSA.RS._256);
                jwk = rpkc.ParamToJwk(((RSA)jWS_RS256_X509.DigitalSignX509.AsymmetricAlgorithm).ExportParameters(false));

                TIPD.MyDebug.OutputDebugAndConsole("RSA JWK", jwk);

                // 検証（X509）
                jWS_RS256_X509 = new JWS_RS256_X509(Program.PublicRsaX509Path, "", x509KSF);

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS256_X509.Verify", jWS_RS256_X509.Verify(token).ToString());

                // 検証（Param）
                jWS_RS256_Param = new JWS_RS256_Param(rpkc.JwkToParam(jwk));

                TIPD.MyDebug.OutputDebugAndConsole("JWS_RS256_Param.Verify", jWS_RS256_Param.Verify(token).ToString());
                #endregion

                // DSA

                #region ECDsa(ES256)
                // 署名（X509）
                jWS_ES256_X509 = new JWS_ES256_X509(Program.PrivateECDsaX509_256Path, Program.PfxPassword);
                token = jWS_ES256_X509.Create(payloadString);

                Touryo.Infrastructure.Public.Security.
                MyDebug.InspectJwt("JWS_ES256_X509.Create", token);

                // 鍵の相互変換
                epkc = new EccPublicKeyConverter(JWS_ECDSA.ES._256);
                jwk = epkc.ParamToJwk(((ECDsa)jWS_ES256_X509.DigitalSignECDsaX509.AsymmetricAlgorithm).ExportParameters(false));

                TIPD.MyDebug.OutputDebugAndConsole("ECDSA JWK", jwk);

                // 検証（X509）
                jWS_ES256_X509 = new JWS_ES256_X509(Program.PublicECDsaX509_256Path, "");

                TIPD.MyDebug.OutputDebugAndConsole("JWS_ES256_X509.Verify", jWS_ES256_X509.Verify(token).ToString());

                // 検証（Param）
                jWS_ES256_Param = new JWS_ES256_Param(epkc.JwkToParam(jwk), false);

                TIPD.MyDebug.OutputDebugAndConsole("JWS_ES256_Param.Verify", jWS_ES256_X509.Verify(token).ToString());

                // ★ xLibTest
                Program.VerifyResultJwt("JwsAlgorithm.xLibTest", token, jWS_ES256_X509.DigitalSignECDsaX509.AsymmetricAlgorithm, JwsAlgorithm.ES256);
                #endregion
#endif
            }
            #endregion

            #region Jwe

            #region RsaOaepAesGcm
            // 暗号化
            jwe = new JWE_RsaOaepAesGcm_X509(Program.PublicRsaX509Path, "", x509KSF);
            token = jwe.Create(payloadString);

            // 復号化
            jwe = new JWE_RsaOaepAesGcm_X509(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            ret = jwe.Decrypt(token, out temp);

            TIPD.MyDebug.OutputDebugAndConsole("JWE_RsaOaepAesGcm_X509.Decrypt", ret.ToString() + " : " + temp);

            // ★ xLibTest
            Program.VerifyResultJwt("JweAlgorithm.xLibTest", token,
                jwe.ASymmetricCryptography.AsymmetricAlgorithm, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);
            #endregion

            #region Rsa15A128CbcHS256
            // 暗号化
            jwe = new JWE_Rsa15A128CbcHS256_X509(Program.PublicRsaX509Path, "", x509KSF);
            token = jwe.Create(payloadString);

            // 復号化
            jwe = new JWE_Rsa15A128CbcHS256_X509(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            ret = jwe.Decrypt(token, out temp);

            TIPD.MyDebug.OutputDebugAndConsole("JWE_Rsa15A128CbcHS256_X509.Decrypt", ret.ToString() + " : " + temp);

            // ★ xLibTest
            Program.VerifyResultJwt("JweAlgorithm.xLibTest", token,
                jwe.ASymmetricCryptography.AsymmetricAlgorithm, JweAlgorithm.RSA1_5, JweEncryption.A128CBC_HS256);

            #endregion

            #endregion

        }

        #endregion

        #region JoseJwt

        /// <summary>JoseJwt</summary>
        private static void JoseJwt()
        {
            #region Variables

            #region Env
            OperatingSystem os = Environment.OSVersion;

            // https://github.com/dotnet/corefx/issues/29404#issuecomment-385287947
            //   *.pfxから証明書を開く場合、X509KeyStorageFlags.Exportableの指定が必要な場合がある。
            //   Linuxのキーは常にエクスポート可能だが、WindowsやMacOSでは必ずしもそうではない。
            X509KeyStorageFlags x509KSF = 0;
            if (os.Platform == PlatformID.Win32NT)
            {
                x509KSF = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
            }
            else //if (os.Platform == PlatformID.Unix)
            {
                x509KSF = X509KeyStorageFlags.DefaultKeySet;
            }
            #endregion

            #region Token
            string token = "";
            IDictionary<string, object> headers = null;
            IDictionary<string, object> payload = null;
            payload = new Dictionary<string, object>()
                {
                    { "sub", "mr.x@contoso.com" },
                    { "exp", 1300819380 }
                };
            #endregion

            #region Keys

            byte[] secretKey = null;
            byte[] x = null;
            byte[] y = null;
            byte[] d = null;

            X509Certificate2 publicX509Key = null;
            X509Certificate2 privateX509Key = null;

            RSA rsa = null;
            //DSA dsa = null;

            CngKey publicKeyOfCng = null;
            CngKey privateKeyOfCng = null;

            RsaPublicKeyConverter rpkc = null;
#if NETCOREAPP || NET47
            EccPublicKeyConverter epkc = null;
#endif
            #endregion

            #endregion

            #region JWT

            #region Unsecured JWT
            // Creating Plaintext (unprotected) Tokens
            // https://github.com/dvsekhvalnov/jose-jwt#creating-plaintext-unprotected-tokens
            token = "";
            token = JWT.Encode(payload, null, JwsAlgorithm.none);

            TIPD.MyDebug.OutputDebugAndConsole("JwsAlgorithm.none", token);
            #endregion

            #region JWS (Creating signed Tokens)
            // https://github.com/dvsekhvalnov/jose-jwt#creating-signed-tokens

            #region HS-* family
            // HS256, HS384, HS512
            // https://github.com/dvsekhvalnov/jose-jwt#hs--family
            secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
            token = "";
            token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
            Program.VerifyResultJwt("JwsAlgorithm.HS256", token, secretKey);
            #endregion

            #region RS-* and PS-* family
            // RS256, RS384, RS512 and PS256, PS384, PS512
            // https://github.com/dvsekhvalnov/jose-jwt#rs--and-ps--family
            // X509Certificate2 x509Certificate2 = new X509Certificate2();
            privateX509Key = new X509Certificate2(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            publicX509Key = new X509Certificate2(Program.PublicRsaX509Path, "", x509KSF);

            token = "";


#if NETCOREAPP
            rsa = (RSA)privateX509Key.GetRSAPrivateKey();//.PrivateKey;
#else
            // .net frameworkでは、何故かコレが必要。
            rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(privateX509Key.PrivateKey);
#endif
            token = JWT.Encode(payload, rsa, JwsAlgorithm.RS256);
            Program.VerifyResultJwt("JwsAlgorithm.RS256", token, rsa);

            #endregion

            #region ES- * family
            // ES256, ES384, ES512 ECDSA signatures
            // https://github.com/dvsekhvalnov/jose-jwt#es---family

            x = new byte[] { 4, 114, 29, 223, 58, 3, 191, 170, 67, 128, 229, 33, 242, 178, 157, 150, 133, 25, 209, 139, 166, 69, 55, 26, 84, 48, 169, 165, 67, 232, 98, 9 };
            y = new byte[] { 131, 116, 8, 14, 22, 150, 18, 75, 24, 181, 159, 78, 90, 51, 71, 159, 214, 186, 250, 47, 207, 246, 142, 127, 54, 183, 72, 72, 253, 21, 88, 53 };
            d = new byte[] { 42, 148, 231, 48, 225, 196, 166, 201, 23, 190, 229, 199, 20, 39, 226, 70, 209, 148, 29, 70, 125, 14, 174, 66, 9, 198, 80, 251, 95, 107, 98, 206 };

            if (os.Platform == PlatformID.Win32NT)
            {
                // https://github.com/dvsekhvalnov/jose-jwt/blob/master/jose-jwt/Security/Cryptography/EccKey.cs
                privateKeyOfCng = EccKey.New(x, y, d);
                publicKeyOfCng = EccKey.New(x, y);

                token = "";
                token = JWT.Encode(payload, privateKeyOfCng, JwsAlgorithm.ES256);
                Program.VerifyResultJwt("JwsAlgorithm.ES256", token, publicKeyOfCng);
            }
            else //if (os.Platform == PlatformID.Unix)
            {
#if NETCOREAPP
                ECParameters eCParameters = new ECParameters();

                epkc = new EccPublicKeyConverter(JWS_ECDSA.ES._256);

                // Curve
                eCParameters.Curve =
                    epkc.GetECCurveFromCrvString(
                        epkc.GetCrvStringFromXCoordinate(x));

                // x, y, d
                eCParameters.Q.X = x;
                eCParameters.Q.Y = y;
                eCParameters.D = d;
                ECDsaOpenSsl eCDsaOpenSsl = new ECDsaOpenSsl(eCParameters.Curve);
                eCDsaOpenSsl.ImportParameters(eCParameters);

                token = "";
                token = JWT.Encode(payload, eCDsaOpenSsl, JwsAlgorithm.ES256);
                Program.VerifyResultJwt("JwsAlgorithm.ES256", token, eCDsaOpenSsl);
#endif
            }

#if NETCOREAPP || NET47
            privateX509Key = new X509Certificate2(Program.PrivateECDsaX509_256Path, Program.PfxPassword);
            publicX509Key = new X509Certificate2(Program.PublicECDsaX509_256Path, "");

            try
            {
#if NETCOREAPP
                if (os.Platform == PlatformID.Win32NT)
                {
                }
                else //if (os.Platform == PlatformID.Unix)
                {
                    // ECCurveを分析してみる。
                    ECCurve eCCurve = ((ECDsaOpenSsl)privateX509Key.GetECDsaPrivateKey()).ExportExplicitParameters(true).Curve;
                    TIPD.MyDebug.OutputDebugAndConsole("Inspect ECCurve", TIPD.ObjectInspector.Inspect(eCCurve));
                }
#endif
                token = "";
                token = JWT.Encode(payload, privateX509Key.GetECDsaPrivateKey(), JwsAlgorithm.ES256);
                Program.VerifyResultJwt("JwsAlgorithm.ES256", token, publicX509Key.GetECDsaPublicKey());
            }
            catch (Exception ex)
            {
                TIPD.MyDebug.OutputDebugAndConsole("JwsAlgorithm.ES256", ex.GetType().ToString() + ", " + ex.Message);
            }
#endif

            #endregion

            #endregion

            #region JWE (Creating encrypted Tokens)
            // https://github.com/dvsekhvalnov/jose-jwt#creating-encrypted-tokens

            #region RSA-* key management family of algorithms
            // RSA-OAEP-256, RSA-OAEP and RSA1_5 key
            // https://github.com/dvsekhvalnov/jose-jwt#rsa--key-management-family-of-algorithms
            privateX509Key = new X509Certificate2(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            publicX509Key = new X509Certificate2(Program.PublicRsaX509Path, "", x509KSF);

            // RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256
            token = "";
            token = JWT.Encode(payload, publicX509Key.GetRSAPublicKey(), JweAlgorithm.RSA1_5, JweEncryption.A128CBC_HS256);
            Program.VerifyResultJwt("JweAlgorithm.RSA1_5, JweEncryption.A128CBC_HS256", token, privateX509Key.GetRSAPrivateKey());

            // RSAES-OAEP and AES GCM
            try
            {
                token = "";
                token = JWT.Encode(payload, publicX509Key.GetRSAPublicKey(), JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);
                Program.VerifyResultJwt("JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM", token, privateX509Key.GetRSAPrivateKey());
            }
            catch (Exception ex)
            {
                // Unhandled Exception: System.DllNotFoundException: Unable to load DLL 'bcrypt.dll' at ubunntu
                TIPD.MyDebug.OutputDebugAndConsole("JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM", ex.GetType().ToString() + ", " + ex.Message);
            }
            #endregion

            #region Other key management family of algorithms

            secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

            #region DIR direct pre-shared symmetric key family of algorithms
            // https://github.com/dvsekhvalnov/jose-jwt#dir-direct-pre-shared-symmetric-key-family-of-algorithms
            token = "";
            token = JWT.Encode(payload, secretKey, JweAlgorithm.DIR, JweEncryption.A128CBC_HS256);
            Program.VerifyResultJwt("JweAlgorithm.DIR, JweEncryption.A128CBC_HS256", token, secretKey);
            #endregion

            #region AES Key Wrap key management family of algorithms
            // AES128KW, AES192KW and AES256KW key management
            // https://github.com/dvsekhvalnov/jose-jwt#aes-key-wrap-key-management-family-of-algorithms
            token = "";
            token = JWT.Encode(payload, secretKey, JweAlgorithm.A256KW, JweEncryption.A256CBC_HS512);
            Program.VerifyResultJwt("JweAlgorithm.A256KW, JweEncryption.A256CBC_HS512", token, secretKey);
            #endregion

            #region AES GCM Key Wrap key management family of algorithms
            // AES128GCMKW, AES192GCMKW and AES256GCMKW key management
            // https://github.com/dvsekhvalnov/jose-jwt#aes-gcm-key-wrap-key-management-family-of-algorithms
            try
            {
                token = "";
                token = JWT.Encode(payload, secretKey, JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512);
                Program.VerifyResultJwt("JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512", token, secretKey);
            }
            catch (Exception ex)
            {
                // Unhandled Exception: System.DllNotFoundException: Unable to load DLL 'bcrypt.dll' at ubunntu
                TIPD.MyDebug.OutputDebugAndConsole("JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512", ex.GetType().ToString() + ", " + ex.Message);
            }
            #endregion

            #region ECDH-ES and ECDH-ES with AES Key Wrap key management family of algorithms
            // ECDH-ES and ECDH-ES+A128KW, ECDH-ES+A192KW, ECDH-ES+A256KW key management
            // https://github.com/dvsekhvalnov/jose-jwt#ecdh-es-and-ecdh-es-with-aes-key-wrap-key-management-family-of-algorithms
            try
            {
                x = new byte[] { 4, 114, 29, 223, 58, 3, 191, 170, 67, 128, 229, 33, 242, 178, 157, 150, 133, 25, 209, 139, 166, 69, 55, 26, 84, 48, 169, 165, 67, 232, 98, 9 };
                y = new byte[] { 131, 116, 8, 14, 22, 150, 18, 75, 24, 181, 159, 78, 90, 51, 71, 159, 214, 186, 250, 47, 207, 246, 142, 127, 54, 183, 72, 72, 253, 21, 88, 53 };
                publicKeyOfCng = EccKey.New(x, y, usage: CngKeyUsages.KeyAgreement);
                token = "";
                token = JWT.Encode(payload, publicKeyOfCng, JweAlgorithm.ECDH_ES, JweEncryption.A256GCM);
                Program.VerifyResultJwt("JweAlgorithm.ECDH_ES, JweEncryption.A256GCM", token, publicKeyOfCng);
            }
            catch (Exception ex)
            {
                // System.NotImplementedException: 'not yet'
                TIPD.MyDebug.OutputDebugAndConsole("JweAlgorithm.ECDH_ES, JweEncryption.A256GCM", ex.GetType().ToString() + ", " + ex.Message);
            }
            #endregion

            #region PBES2 using HMAC SHA with AES Key Wrap key management family of algorithms
            token = "";
            token = JWT.Encode(payload, "top secret", JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
            Program.VerifyResultJwt("JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512", token, "top secret");
            #endregion

            #endregion

            #endregion

            #endregion

            #region ELSE

            #region Additional utilities
            // https://github.com/dvsekhvalnov/jose-jwt#additional-utilities

            #region Adding extra headers
            // https://github.com/dvsekhvalnov/jose-jwt#adding-extra-headers

            headers = new Dictionary<string, object>()
                {
                    { "typ", "JWT" },
                    { "cty", "JWT" },
                    { "keyid", "111-222-333"}
                };

            privateX509Key = new X509Certificate2(Program.PrivateRsaX509Path, Program.PfxPassword, x509KSF);
            publicX509Key = new X509Certificate2(Program.PublicRsaX509Path, "", x509KSF);

#if NETCOREAPP
            rsa = (RSA)privateX509Key.GetRSAPrivateKey();
#else
            // .net frameworkでは、何故かコレが必要。
            rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(privateX509Key.PrivateKey);
#endif

            token = "";
            token = JWT.Encode(payload, rsa, JwsAlgorithm.RS256, extraHeaders: headers);
            Program.VerifyResultJwt("Adding extra headers to RS256", token, rsa);
            #endregion

            #region Strict validation
            // https://github.com/dvsekhvalnov/jose-jwt#strict-validation
            // 厳密な検証では、Algorithmを指定可能
            TIPD.MyDebug.OutputDebugAndConsole("Strict validation(RS256)", JWT.Decode(token, rsa, JwsAlgorithm.RS256));
            #endregion

            #region Two-phase validation
            // https://github.com/dvsekhvalnov/jose-jwt#two-phase-validation
            // ヘッダのkeyidクレームからキーを取り出して復号化する方法。
            //headers = JWT.Headers(token);
            // ・・・
            //string hoge = JWT.Decode(token, "key");
            #endregion

            #region Working with binary payload
            // https://github.com/dvsekhvalnov/jose-jwt#working-with-binary-payload
            #endregion

            #endregion

            #region Settings
            // https://github.com/dvsekhvalnov/jose-jwt#settings
            // グローバル設定

            #region Example of JWTSettings
            // https://github.com/dvsekhvalnov/jose-jwt#example-of-jwtsettings

            #endregion

            #region Customizing json <-> object parsing & mapping
            // https://github.com/dvsekhvalnov/jose-jwt#customizing-json---object-parsing--mapping
            // マッピング
            // https://github.com/dvsekhvalnov/jose-jwt#example-of-newtonsoftjson-mapper
            // https://github.com/dvsekhvalnov/jose-jwt#example-of-servicestack-mapper

            #endregion

            #region Customizing algorithm implementations
            // https://github.com/dvsekhvalnov/jose-jwt#customizing-algorithm-implementations
            // https://github.com/dvsekhvalnov/jose-jwt#example-of-custom-algorithm-implementation
            #endregion

            #region Providing aliases
            // https://github.com/dvsekhvalnov/jose-jwt#providing-aliases
            #endregion

            #endregion

            #region Dealing with keys
            // https://github.com/dvsekhvalnov/jose-jwt#dealing-with-keys
            // https://github.com/dvsekhvalnov/jose-jwt#rsacryptoserviceprovider
            // - http://stackoverflow.com/questions/7444586/how-can-i-sign-a-file-using-rsa-and-sha256-with-net
            // - http://hintdesk.com/c-how-to-fix-invalid-algorithm-specified-when-signing-with-sha256/
            // https://github.com/dvsekhvalnov/jose-jwt#if-you-have-only-rsa-private-key
            // - http://www.donaldsbaconbytes.com/2016/08/create-jwt-with-a-private-rsa-key/
            #endregion

            #region Strong-Named assembly
            // https://github.com/dvsekhvalnov/jose-jwt#strong-named-assembly
            // - https://github.com/dvsekhvalnov/jose-jwt/issues/5
            // - https://github.com/brutaldev/StrongNameSigner
            #endregion

            #region More examples
            // https://github.com/dvsekhvalnov/jose-jwt#more-examples
            // https://github.com/dvsekhvalnov/jose-jwt/blob/master/UnitTests/TestSuite.cs
            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region SignedXml

        /// <summary>SignedXml</summary>
        private static void SignedXml()
        {
            string xml = ""
                + "<xml>"
                + "  <a ID=\"a\">"
                + "    <b ID=\"b\">"
                + "      <c/>"
                + "    </b>"
                + "  </a>"
                + "</xml>";

            // SignedXml2
            SignedXml2 signedXml = new SignedXml2(new RSACryptoServiceProvider());

            // XmlWriterSettings
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = false
            };

            // ネスト
            XmlDocument xmlDoc = null;
            xmlDoc = signedXml.Create(xml, "b");
            xml = xmlDoc.XmlToString(xmlWriterSettings);
            xmlDoc = signedXml.Create(xml, "a");
            xml = xmlDoc.XmlToString(xmlWriterSettings);

            // 内側 (b
            bool ret = signedXml.Verify(xml, "b");
            // 外側 (a
            ret = signedXml.Verify(xml, "a");

            TIPD.MyDebug.OutputDebugAndConsole("Verify nested signedXml", ret.ToString() + " : " + xml);
        }

        #endregion

        #endregion

        #region Inspector and Verifier

        /// <summary>VerifyResultJwt</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwt">string</param>
        /// <param name="key">object</param>
        /// <param name="alg">JwsAlgorithm?</param>
        private static void VerifyResultJwt(string testLabel, string jwt, object key, JwsAlgorithm? alg = null)
        {
            TIPD.MyDebug.OutputDebugAndConsole(testLabel, "Original:" + jwt);

            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectJwt(testLabel, jwt);

            if (alg.HasValue)
            {
                TIPD.MyDebug.OutputDebugAndConsole(testLabel, "Decoded:" + JWT.Decode(jwt, key, alg.Value));
            }
            else
            {
                TIPD.MyDebug.OutputDebugAndConsole(testLabel, "Decoded:" + JWT.Decode(jwt, key));
            }
        }

        /// <summary>VerifyResultJwt</summary>
        /// <param name="testLabel">string</param>
        /// <param name="jwt">string</param>
        /// <param name="key">object</param>
        /// <param name="alg">JweAlgorithm</param>
        /// <param name="enc">JweEncryption</param>
        private static void VerifyResultJwt(string testLabel, string jwt, object key, JweAlgorithm alg, JweEncryption enc)
        {
            TIPD.MyDebug.OutputDebugAndConsole(testLabel, "Original:" + jwt);

            Touryo.Infrastructure.Public.Security.
            MyDebug.InspectJwt(testLabel, jwt);

            TIPD.MyDebug.OutputDebugAndConsole(testLabel, "Decoded:" + JWT.Decode(jwt, key, alg, enc));
        }

        #endregion
    }
}
