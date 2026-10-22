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
//* クラス名        ：SAML2Metadata
//* クラス日本語名  ：相手（IdP）の SAML2 メタデータを読む
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の C）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>相手（IdP）の SAML2 メタデータを読む</summary>
    /// <remarks>
    /// SAML2Client は要求を作る側だけで、相手のメタデータを読む側が無かった（#634）。
    /// 自分のメタデータを作るのは SAML2Bindings.CreateMetadata である。
    ///
    /// **期待する entityID を必ず受け取る。** メタデータの entityID と違えば、何も返さない。
    /// どこから引いた値かを、期待する相手に結び付けるためである。
    /// メタデータの署名は検証しない（信頼の根拠は TLS と、この entityID の照合だけ）。
    ///
    /// 引けなければ null を返し、例外にしない（呼び出し側が設定の値に倒せるように）。
    /// 取得は OAuth2AndOIDCClient の HttpClient で行い、URL を鍵にしてキャッシュする。
    /// </remarks>
    public class SAML2Metadata
    {
        #region 結果

        /// <summary>entityID</summary>
        public string EntityId { get; private set; }

        /// <summary>SingleSignOnService（HTTP-Redirect）の Location（無ければ空文字）</summary>
        public string SsoRedirectLocation { get; private set; }

        /// <summary>SingleSignOnService（HTTP-POST）の Location（無ければ空文字）</summary>
        public string SsoPostLocation { get; private set; }

        /// <summary>署名用の証明書（base64 の DER。並んでいる順）</summary>
        /// <remarks>
        /// 鍵のロールオーバー中は 2 本以上並ぶ（新旧）ので、一覧で持つ。
        /// use="signing" と、use の無い KeyDescriptor（どちらにも使える。Metadata 2.4.1.1）を採る。
        /// </remarks>
        public List<string> SigningCertificates { get; private set; }

        /// <summary>取得した時刻（UTC）</summary>
        public DateTime FetchedAt { get; private set; }

        /// <summary>署名用の証明書から、検証鍵（RSA）を全部作る</summary>
        /// <returns>RSA の一覧（並んでいる順。読めないものは飛ばす）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// SAML2 の署名には kid に当たるものが無い（KeyInfo は任意）ので、
        /// どの鍵で署名されたかは応答から分からない。ロールオーバー中は全部試すこと
        /// （SAML2Client.VerifyResponseWithReason は一覧を受ける）。
        /// </remarks>
        public List<RSA> GetSigningKeys()
        {
            List<RSA> keys = new List<RSA>();

            foreach (string base64 in this.SigningCertificates)
            {
                RSA rsa = SAML2Metadata.ToRsa(base64);

                if (rsa != null)
                {
                    keys.Add(rsa);
                }
            }

            return keys;
        }

        /// <summary>署名用の証明書のうち、最初の 1 本から検証鍵（RSA）を作る</summary>
        /// <returns>RSA（作れなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public RSA GetFirstSigningKey()
        {
            List<RSA> keys = this.GetSigningKeys();
            return (keys.Count == 0) ? null : keys[0];
        }

        #endregion

        #region キャッシュ

        /// <summary>取得したメタデータ（鍵はメタデータの URL）</summary>
        private static readonly Dictionary<string, SAML2Metadata> _cache = new Dictionary<string, SAML2Metadata>();

        /// <summary>_cache の排他</summary>
        private static readonly object _lock = new object();

        /// <summary>キャッシュの有効期間</summary>
        private static TimeSpan _cacheLifetime = TimeSpan.FromHours(1);

        /// <summary>キャッシュの有効期間（既定 1 時間）</summary>
        /// <remarks>
        /// 自由に利用できる。これを過ぎたら取り直す。プロセス全体に効く。
        /// 取り直しに失敗したときは、前に取れたものを使い続ける。
        /// </remarks>
        public static TimeSpan CacheLifetime
        {
            get { return SAML2Metadata._cacheLifetime; }
            set { SAML2Metadata._cacheLifetime = value; }
        }

        /// <summary>キャッシュを消す</summary>
        /// <remarks>自由に利用できる（鍵の入れ替えを即座に反映したいとき、試験など）。</remarks>
        public static void Clear()
        {
            lock (SAML2Metadata._lock)
            {
                SAML2Metadata._cache.Clear();
            }
        }

        #endregion

        #region 取得・解析

        /// <summary>メタデータを読む（キャッシュを使う）</summary>
        /// <param name="metadataUrl">string（相手のメタデータの URL）</param>
        /// <param name="expectedEntityId">string（期待する entityID。メタデータの entityID と違えば null を返す）</param>
        /// <returns>SAML2Metadata（引けなければ null）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static async Task<SAML2Metadata> GetAsync(string metadataUrl, string expectedEntityId)
        {
            if (string.IsNullOrEmpty(metadataUrl) || string.IsNullOrEmpty(expectedEntityId))
            {
                return null;
            }

            SAML2Metadata cached = null;

            lock (SAML2Metadata._lock)
            {
                SAML2Metadata._cache.TryGetValue(metadataUrl, out cached);
            }

            if (cached != null
                && cached.EntityId == expectedEntityId
                && (DateTime.UtcNow - cached.FetchedAt) <= SAML2Metadata._cacheLifetime)
            {
                return cached;
            }

            SAML2Metadata fetched = null;

            try
            {
                string xml = await OAuth2AndOIDCClient.GetJwkSetAsync(new Uri(metadataUrl)).ConfigureAwait(false);
                fetched = SAML2Metadata.Parse(xml);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SAML2Metadata: " + ex.GetType().Name);
            }

            if (fetched == null)
            {
                // 取り直しに失敗した。前に取れたものが在れば、それを使い続ける。
                Debug.WriteLine("SAML2Metadata: the metadata could not be read.");
                return (cached != null && cached.EntityId == expectedEntityId) ? cached : null;
            }

            if (fetched.EntityId != expectedEntityId)
            {
                Debug.WriteLine("SAML2Metadata: the entityID did not match the expected entityID.");
                return null;
            }

            lock (SAML2Metadata._lock)
            {
                SAML2Metadata._cache[metadataUrl] = fetched;
            }

            return fetched;
        }

        /// <summary>メタデータの XML を解析する</summary>
        /// <param name="xml">string（EntityDescriptor）</param>
        /// <returns>SAML2Metadata（読めなければ null）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// IDPSSODescriptor の SingleSignOnService と KeyDescriptor を読む。
        /// DTD は処理しない（外から来た XML を読むため）。
        /// </remarks>
        public static SAML2Metadata Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.XmlResolver = null;

                using (XmlReader reader = XmlReader.Create(
                    new System.IO.StringReader(xml),
                    new XmlReaderSettings() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
                {
                    doc.Load(reader);
                }

                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("md", SAML2Const.UrnMetadata);
                ns.AddNamespace("ds", SAML2Const.UrnDSig);

                XmlNode entity = doc.SelectSingleNode("/md:EntityDescriptor", ns);

                if (entity == null)
                {
                    return null;
                }

                SAML2Metadata ret = new SAML2Metadata()
                {
                    EntityId = SAML2Metadata.Attr(entity, "entityID"),
                    SsoRedirectLocation = SAML2Metadata.SsoLocation(doc, ns, SAML2Const.UrnBindingsRedirect),
                    SsoPostLocation = SAML2Metadata.SsoLocation(doc, ns, SAML2Const.UrnBindingsPost),
                    SigningCertificates = new List<string>(),
                    FetchedAt = DateTime.UtcNow
                };

                XmlNodeList keys = doc.SelectNodes(
                    "/md:EntityDescriptor/md:IDPSSODescriptor/md:KeyDescriptor", ns);

                foreach (XmlNode key in keys)
                {
                    string use = SAML2Metadata.Attr(key, "use");

                    // 暗号化用の鍵は使わない。
                    if (use != "" && use != "signing")
                    {
                        continue;
                    }

                    XmlNode cer = key.SelectSingleNode("ds:KeyInfo/ds:X509Data/ds:X509Certificate", ns);

                    if (cer == null || string.IsNullOrEmpty(cer.InnerText))
                    {
                        continue;
                    }

                    // base64 の本体だけを採る（改行・空白は落とす）。
                    ret.SigningCertificates.Add(
                        cer.InnerText.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "").Trim());
                }

                return string.IsNullOrEmpty(ret.EntityId) ? null : ret;
            }
            catch (Exception)
            {
                // 外から来た XML なので、例外にしない。
                return null;
            }
        }

        #endregion

        #region private

        /// <summary>SingleSignOnService の Location を読む</summary>
        /// <param name="doc">XmlDocument</param>
        /// <param name="ns">XmlNamespaceManager</param>
        /// <param name="binding">string</param>
        /// <returns>Location（無ければ空文字）</returns>
        private static string SsoLocation(XmlDocument doc, XmlNamespaceManager ns, string binding)
        {
            foreach (XmlNode node in doc.SelectNodes(
                "/md:EntityDescriptor/md:IDPSSODescriptor/md:SingleSignOnService", ns))
            {
                if (SAML2Metadata.Attr(node, "Binding") == binding)
                {
                    return SAML2Metadata.Attr(node, "Location");
                }
            }

            return "";
        }

        /// <summary>属性値を読む（無ければ空文字）</summary>
        /// <param name="node">XmlNode</param>
        /// <param name="name">string</param>
        /// <returns>値</returns>
        private static string Attr(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null)
            {
                return "";
            }

            XmlNode attr = node.Attributes[name];
            return (attr == null) ? "" : attr.Value;
        }

        /// <summary>証明書（base64 の DER）から RSA を作る</summary>
        /// <param name="base64">string</param>
        /// <returns>RSA（作れなければ null）</returns>
        /// <remarks>
        /// 証明書の読み込みはターゲットで分ける。
        /// new X509Certificate2(byte[]) は .NET 9 以降で旧形式（SYSLIB0057）、
        /// X509CertificateLoader は net48 に無い。
        /// </remarks>
        private static RSA ToRsa(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            try
            {
                byte[] der = Convert.FromBase64String(base64);
#if NETSTD
                X509Certificate2 x509 = X509CertificateLoader.LoadCertificate(der);
#else
                X509Certificate2 x509 = new X509Certificate2(der);
#endif
                return x509.GetRSAPublicKey();
            }
            catch (Exception)
            {
                Debug.WriteLine("SAML2Metadata: a signing certificate could not be read.");
                return null;
            }
        }

        #endregion
    }
}
