using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// https://www.nuget.org/packages/jose-jwt/
using Jose;
// https://github.com/dvsekhvalnov/jose-jwt/tree/master/jose-jwt/Security/Cryptography
using Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;

namespace EncAndDecUtilCUI
{
    /// <summary>
    /// - jose-jwt - マイクロソフト系技術情報 Wiki
    ///   https://techinfoofmicrosofttech.osscons.jp/index.php?jose-jwt
    /// - Certificates
    ///   https://github.com/OpenTouryoProject/OpenTouryo/tree/develop/root/files/resource/X509
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region Variables

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

            string token = "";
            IDictionary<string, object> headers = null;
            IDictionary<string, object> payload = null;
            payload = new Dictionary<string, object>()
            {
                { "sub", "mr.x@contoso.com" },
                { "exp", 1300819380 }
            };

            #region Keys
            byte[] secretKey = null;
            byte[] x = null;
            byte[] y = null;
            byte[] d = null;

            string privateX509Path = "";
            string publicX509Path = "";
            X509Certificate2 publicX509Key = null;
            X509Certificate2 privateX509Key = null;

            RSA rsa = null;
            DSA dsa = null;

            CngKey publicKeyOfCng = null;
            CngKey privateKeyOfCng = null;

            #endregion

            #endregion

            #region Test of the X.509 Certificates

            #region RSA
            privateX509Path = @"SHA256RSA.pfx";
            publicX509Path = @"SHA256RSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test", x509KSF);
            publicX509Key = new X509Certificate2(publicX509Path, "", x509KSF);
            Program.PrivateX509KeyInspector("RSA", privateX509Key);
            Program.PublicX509KeyInspector("RSA", publicX509Key);
            #endregion

#if NETCORE || NET47
            #region DSA
            // https://github.com/dotnet/corefx/issues/18733#issuecomment-296723615
            privateX509Path = @"SHA256DSA.pfx";
            publicX509Path = @"SHA256DSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test");
            publicX509Key = new X509Certificate2(publicX509Path, "");
            Program.PrivateX509KeyInspector("DSA", privateX509Key);
            Program.PublicX509KeyInspector("DSA", publicX509Key);
            DSA privateDSA = privateX509Key.GetDSAPrivateKey();
            Program.MyWriteLine("privateDSA: " + (privateDSA == null ? "is null" : "is not null"));
            DSA publicDSA = null; // publicX509Key.GetDSAPublicKey(); // Internal.Cryptography.CryptoThrowHelper.WindowsCryptographicException
            #endregion

            #region ECDsa
            // https://github.com/dotnet/corefx/issues/18733#issuecomment-296723615
            privateX509Path = @"SHA256ECDSA.pfx";
            publicX509Path = @"SHA256ECDSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test");
            publicX509Key = new X509Certificate2(publicX509Path, "");
            Program.PrivateX509KeyInspector("ECDsa", privateX509Key);
            Program.PublicX509KeyInspector("ECDsa", publicX509Key);
            ECDsa privateECDsa = privateX509Key.GetECDsaPrivateKey();
            Program.MyWriteLine("privateECDsa: " + (privateECDsa == null ? "is null" : "is not null"));
            ECDsa publicECDsa = publicX509Key.GetECDsaPublicKey();
            Program.MyWriteLine("publicECDsa: " + (publicECDsa == null ? "is null" : "is not null"));
            #endregion
#endif

            #endregion

            Program.MyWriteLine("----------------------------------------------------------------------------------------------------");

            #region Test of the OpenTouryo.Public.Security.

            byte[] sign = null;

            DigitalSignParam dsParam = null;
            DigitalSignXML dsXML = null;
            DigitalSignX509 dsX509 = null;

            if (os.Platform == PlatformID.Win32NT)
            {
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.RsaCSP_SHA256);
                sign = dsParam.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignParam.Verify(RS256): " + dsParam.Verify(new byte[] { }, sign).ToString());

                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.RsaCSP_SHA256);
                sign = dsXML.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignXML.Verify(RS256): " + dsXML.Verify(new byte[] { }, sign).ToString());

                // DSAはFormatterバージョンしか動かない。
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
                sign = dsParam.SignByFormatter(new byte[] { });
                Program.MyWriteLine("DigitalSignParam.Verify(DS1): " + dsParam.VerifyByDeformatter(new byte[] { }, sign).ToString());

                DigitalSignXML dsX2 = new DigitalSignXML(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
                sign = dsX2.SignByFormatter(new byte[] { });
                Program.MyWriteLine("DigitalSignXML.Verify(DS1): " + dsX2.VerifyByDeformatter(new byte[] { }, sign).ToString());


                dsX509 = new DigitalSignX509(@"SHA256RSA.pfx", "test", "SHA256", x509KSF);
                sign = dsX509.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignX509.Verify(RSA): " + dsX509.Verify(new byte[] { }, sign).ToString());

                //dsX509 = new DigitalSignX509(@"SHA256DSA.pfx", "test", "SHA256", x509KSF);
                //sign = dsX509.Sign(new byte[] { });
                //Program.MyWriteLine("DigitalSignX509.Verify(DSA): " + dsX509.Verify(new byte[] { }, sign).ToString());
            }
            else //if (os.Platform == PlatformID.Unix)
            {
#if NETCORE
                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.RsaOpenSsl_SHA256);
                sign = dsParam.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignParam.Verify(RS256): " + dsParam.Verify(new byte[] { }, sign).ToString());

                dsXML = new DigitalSignXML(EnumDigitalSignAlgorithm.RsaOpenSsl_SHA256);
                sign = dsXML.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignXML.Verify(RS256): " + dsXML.Verify(new byte[] { }, sign).ToString());

                dsParam = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1);
                sign = dsParam.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignParam.Verify(DS1): " + dsParam.Verify(new byte[] { }, sign).ToString());

                DigitalSignXML dsX2 = new DigitalSignXML(EnumDigitalSignAlgorithm.DsaOpenSsl_SHA1);
                sign = dsX2.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignXML.Verify(DS1): " + dsX2.Verify(new byte[] { }, sign).ToString());

                dsX509 = new DigitalSignX509(@"SHA256RSA.pfx", "test", "SHA256");
                sign = dsX509.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignX509.Verify(RSA): " + dsX509.Verify(new byte[] { }, sign).ToString());

                dsX509 = new DigitalSignX509(@"SHA256DSA.pfx", "test", "SHA256");
                sign = dsX509.Sign(new byte[] { });
                Program.MyWriteLine("DigitalSignX509.Verify(DSA): " + dsX509.Verify(new byte[] { }, sign).ToString());
#endif
            }
            #endregion

            Program.MyWriteLine("----------------------------------------------------------------------------------------------------");

            #region Test of the jose-jwt

            #region JWT

            #region Unsecured JWT
            // Creating Plaintext (unprotected) Tokens
            // https://github.com/dvsekhvalnov/jose-jwt#creating-plaintext-unprotected-tokens
            token = "";
            token = JWT.Encode(payload, null, JwsAlgorithm.none);
            Program.MyWriteLine("JwsAlgorithm.none: " + token);
            #endregion

            #region JWS (Creating signed Tokens)
            // https://github.com/dvsekhvalnov/jose-jwt#creating-signed-tokens

            #region HS-* family
            // HS256, HS384, HS512
            // https://github.com/dvsekhvalnov/jose-jwt#hs--family
            secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
            token = "";
            token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
            Program.VerifyResult("JwsAlgorithm.HS256: ", token, secretKey);
            #endregion

            #region RS-* and PS-* family
            // RS256, RS384, RS512 and PS256, PS384, PS512
            // https://github.com/dvsekhvalnov/jose-jwt#rs--and-ps--family
            // X509Certificate2 x509Certificate2 = new X509Certificate2();

            privateX509Path = @"SHA256RSA.pfx";
            publicX509Path = @"SHA256RSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test", x509KSF);
            publicX509Key = new X509Certificate2(publicX509Path, "", x509KSF);

            token = "";


#if NETCORE
            rsa = (RSA)privateX509Key.PrivateKey;
#else
            // .net frameworkでは、何故かコレが必要。
            rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(privateX509Key.PrivateKey);
#endif
            token = JWT.Encode(payload, rsa, JwsAlgorithm.RS256);
            Program.VerifyResult("JwsAlgorithm.RS256: ", token, rsa);

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
                Program.VerifyResult("JwsAlgorithm.ES256: ", token, publicKeyOfCng);
            }
            else //if (os.Platform == PlatformID.Unix)
            {
                // (x, y, d)を使用して、ECCurveからECDsaOpenSslを生成できれば...。

                //ECCurve eCCurve = new ECCurve();
                ////eCCurve.A = x;
                ////eCCurve.B = y;
                ////ECDsaOpenSsl ecd = new ECDsaOpenSsl(eCCurve);
                ////eCCurve = ecd.ExportExplicitParameters(true).Curve;

                //token = "";
                //token = JWT.Encode(payload, new ECDsaOpenSsl(eCCurve), JwsAlgorithm.ES256);
                //Program.VerifyResult("JwsAlgorithm.ES256: ", token, new ECDsaOpenSsl(eCCurve));
            }

#if NETCORE || NET47
            privateX509Path = @"SHA256ECDSA.pfx";
            publicX509Path = @"SHA256ECDSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test");
            publicX509Key = new X509Certificate2(publicX509Path, "");

            try
            {
#if NETCORE
                if (os.Platform == PlatformID.Win32NT)
                {
                }
                else //if (os.Platform == PlatformID.Unix)
                {
                    // ECCurveを分析してみる。
                    ECCurve eCCurve = ((ECDsaOpenSsl)privateX509Key.GetECDsaPrivateKey()).ExportExplicitParameters(true).Curve;
                    Program.MyWriteLine("Inspect ECCurve: " + ObjectInspector.Inspect(eCCurve));
                }
#endif
                token = "";
                token = JWT.Encode(payload, privateX509Key.GetECDsaPrivateKey(), JwsAlgorithm.ES256);
                Program.VerifyResult("JwsAlgorithm.ES256: ", token, publicX509Key.GetECDsaPublicKey());
            }
            catch (Exception ex)
            {
                Program.MyWriteLine("JwsAlgorithm.ES256: " + ex.GetType().ToString() + ", " + ex.Message);
            }
#endif

            #endregion

            #endregion

            #region JWE (Creating encrypted Tokens)
            // https://github.com/dvsekhvalnov/jose-jwt#creating-encrypted-tokens

            #region RSA-* key management family of algorithms
            // RSA-OAEP-256, RSA-OAEP and RSA1_5 key
            // https://github.com/dvsekhvalnov/jose-jwt#rsa--key-management-family-of-algorithms

            privateX509Path = @"SHA256RSA.pfx";
            publicX509Path = @"SHA256RSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test", x509KSF);
            publicX509Key = new X509Certificate2(publicX509Path, "", x509KSF);

            // RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256
            token = "";
            token = JWT.Encode(payload, publicX509Key.PublicKey.Key, JweAlgorithm.RSA1_5, JweEncryption.A128CBC_HS256);
            Program.VerifyResult("JweAlgorithm.RSA1_5, JweEncryption.A128CBC_HS256: ", token, privateX509Key.PrivateKey);

            // RSAES-OAEP and AES GCM
            try
            {
                token = "";
                token = JWT.Encode(payload, publicX509Key.PublicKey.Key, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);
                Program.VerifyResult("JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM: ", token, privateX509Key.PrivateKey);
            }
            catch (Exception ex)
            {
                // Unhandled Exception: System.DllNotFoundException: Unable to load DLL 'bcrypt.dll' at ubunntu
                Program.MyWriteLine("JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM: " + ex.GetType().ToString() + ", " + ex.Message);
            }
            #endregion

            #region Other key management family of algorithms

            secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

            #region DIR direct pre-shared symmetric key family of algorithms
            // https://github.com/dvsekhvalnov/jose-jwt#dir-direct-pre-shared-symmetric-key-family-of-algorithms
            token = "";
            token = JWT.Encode(payload, secretKey, JweAlgorithm.DIR, JweEncryption.A128CBC_HS256);
            Program.VerifyResult("JweAlgorithm.DIR, JweEncryption.A128CBC_HS256: ", token, secretKey);
            #endregion

            #region AES Key Wrap key management family of algorithms
            // AES128KW, AES192KW and AES256KW key management
            // https://github.com/dvsekhvalnov/jose-jwt#aes-key-wrap-key-management-family-of-algorithms
            token = "";
            token = JWT.Encode(payload, secretKey, JweAlgorithm.A256KW, JweEncryption.A256CBC_HS512);
            Program.VerifyResult("JweAlgorithm.A256KW, JweEncryption.A256CBC_HS512: ", token, secretKey);
            #endregion

            #region AES GCM Key Wrap key management family of algorithms
            // AES128GCMKW, AES192GCMKW and AES256GCMKW key management
            // https://github.com/dvsekhvalnov/jose-jwt#aes-gcm-key-wrap-key-management-family-of-algorithms
            try
            {
                token = "";
                token = JWT.Encode(payload, secretKey, JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512);
                Program.VerifyResult("JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512: ", token, secretKey);
            }
            catch (Exception ex)
            {
                // Unhandled Exception: System.DllNotFoundException: Unable to load DLL 'bcrypt.dll' at ubunntu
                Program.MyWriteLine("JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512: " + ex.GetType().ToString() + ", " + ex.Message);
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
                Program.VerifyResult("JweAlgorithm.ECDH_ES, JweEncryption.A256GCM: ", token, publicKeyOfCng);
            }
            catch (Exception ex)
            {
                // System.NotImplementedException: 'not yet'
                Program.MyWriteLine("JweAlgorithm.ECDH_ES, JweEncryption.A256GCM: " + ex.GetType().ToString() + ", " + ex.Message);
            }
            #endregion

            #region PBES2 using HMAC SHA with AES Key Wrap key management family of algorithms
            token = "";
            token = JWT.Encode(payload, "top secret", JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
            Program.VerifyResult("JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512: ", token, "top secret");
            #endregion

            #endregion

            #endregion

            #endregion

            Program.MyWriteLine("----------------------------------------------------------------------------------------------------");

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

            privateX509Path = @"SHA256RSA.pfx";
            publicX509Path = @"SHA256RSA.cer";
            privateX509Key = new X509Certificate2(privateX509Path, "test", x509KSF);
            publicX509Key = new X509Certificate2(publicX509Path, "", x509KSF);

#if NETCORE
            rsa = (RSA)privateX509Key.PrivateKey;
#else
            // .net frameworkでは、何故かコレが必要。
            rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(privateX509Key.PrivateKey);
#endif

            token = "";
            token = JWT.Encode(payload, rsa, JwsAlgorithm.RS256, extraHeaders: headers);
            Program.VerifyResult("Adding extra headers to RS256: ", token, rsa);
            #endregion

            #region Strict validation
            // https://github.com/dvsekhvalnov/jose-jwt#strict-validation
            // 厳密な検証では、Algorithmを指定可能
            Program.MyWriteLine("Strict validation(RS256): " + JWT.Decode(token, rsa, JwsAlgorithm.RS256));
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

            #endregion

            Console.ReadKey();
        }

        #region Inspector and Verifier

        /// <summary>PrivateX509KeyInspector</summary>
        /// <param name="lbl">string</param>
        /// <param name="privateX509Key">X509Certificate2</param>
        private static void PrivateX509KeyInspector(string lbl, X509Certificate2 privateX509Key)
        {
            Program.MyWriteLine(lbl + " privateX509Key: " + (privateX509Key == null ? "is null" : "is not null"));
            if (privateX509Key == null)
            {
                return;
            }

            try
            {
                Program.MyWriteLine(lbl + " privateSignatureAlgorithm: " + privateX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                Program.MyWriteLine(lbl + " privateSignatureAlgorithm: " + ex.GetType().ToString() + ", " + ex.Message);
            }

            try
            {
                if (privateX509Key.HasPrivateKey)
                {
                    Program.MyWriteLine(lbl + " privateX509Key.PrivateKey: " + (
                        privateX509Key.PrivateKey == null ? "is null" : "is " + privateX509Key.PrivateKey.ToString()));
                }
            }
            catch (Exception ex)
            {
                Program.MyWriteLine(lbl + " privateX509Key.PrivateKey: " + ex.GetType().ToString() + ", " + ex.Message);
            }
        }

        /// <summary>PublicX509KeyInspector</summary>
        /// <param name="lbl">string</param>
        /// <param name="publicX509Key">X509Certificate2</param>
        private static void PublicX509KeyInspector(string lbl, X509Certificate2 publicX509Key)
        {
            Program.MyWriteLine(lbl + " publicX509Key: " + (publicX509Key == null ? "is null" : "is not null"));
            if (publicX509Key == null)
            {
                return;
            }

            try
            {
                Program.MyWriteLine(lbl + " publicSignatureAlgorithm: " + publicX509Key.SignatureAlgorithm.FriendlyName);
            }
            catch (Exception ex)
            {
                Program.MyWriteLine(lbl + " publicSignatureAlgorithm: " + ex.GetType().ToString() + ", " + ex.Message);
            }

            if (publicX509Key.PublicKey != null)
            {
                Program.MyWriteLine(lbl + " publicX509Key: " + (
                    publicX509Key.PublicKey == null ? "is null" : "is " + publicX509Key.PublicKey.ToString()));

                try
                {
                    if (publicX509Key.PublicKey.Key != null)
                    {
                        Program.MyWriteLine(lbl + " publicX509Key.Key: " + (
                            publicX509Key.PublicKey.Key == null ? "is null" : "is " + publicX509Key.PublicKey.Key.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    Program.MyWriteLine(lbl + " publicX509Key.Key: " + ex.GetType().ToString() + ", " + ex.Message);
                }
            }
        }
        /// <summary>VerifyResult</summary>
        /// <param name="testId">string</param>
        /// <param name="token">string</param>
        /// <param name="key">object</param>
        private static void VerifyResult(string testId, string token, object key)
        {
            Program.MyWriteLine(testId + token);

            string[] aryStr = token.Split('.');

            Program.MyWriteLine("JWT Header: " + CustomEncode.ByteToString(
                CustomEncode.FromBase64UrlString(aryStr[0]), CustomEncode.UTF_8));

            if (3 < aryStr.Length)
            {
                // JWE
                Program.MyWriteLine("- JWE Encrypted Key: " + aryStr[1]);
                Program.MyWriteLine("- JWE Initialization Vector: " + aryStr[2]);
                Program.MyWriteLine("- JWE Ciphertext: " + aryStr[3]);
                Program.MyWriteLine("- JWE Authentication Tag: " + aryStr[4]);
            }

            Program.MyWriteLine("Decoded: " + JWT.Decode(token, key));
        }

        /// <summary>MyWriteLine</summary>
        /// <param name="s">string</param>
        private static void MyWriteLine(string s)
        {
            Console.WriteLine(s);
            Debug.WriteLine(s);
        }
        #endregion
    }
}
