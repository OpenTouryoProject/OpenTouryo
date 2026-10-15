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
//* クラス名        ：Program
//* クラス日本語名  ：通信制御の接続オプションのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/14  玄人 幸道         新規作成（#546）
//*  2026/08/17  玄人 幸道         WCF TCP/IPのケースを追加（#561）。全ケースが
//*                                ASP.NET WebAPIで、WCFを一度も通っていなかった。
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

namespace TestTransmission
{
    #region テスト用の型

    /// <summary>サーバが受け取った内容の記録</summary>
    public class Recorded
    {
        /// <summary>要求行</summary>
        public string RequestLine = "";

        /// <summary>ヘッダ</summary>
        public Dictionary<string, string> Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>要求の回数（認証の再送を数えるため）</summary>
        public int Count = 0;

        /// <summary>TLS の握手で受け取ったクライアント証明書のサブジェクト</summary>
        public string ClientCertSubject = null;

        /// <summary>ヘッダの取得（無ければ null）</summary>
        /// <param name="name">ヘッダ名</param>
        /// <returns>値</returns>
        public string Header(string name)
        {
            return this.Headers.ContainsKey(name) ? this.Headers[name] : null;
        }

        /// <summary>記録のクリア</summary>
        public void Clear()
        {
            this.RequestLine = "";
            this.Headers.Clear();
            this.Count = 0;
            this.ClientCertSubject = null;
        }
    }

    /// <summary>テスト用のコンテキスト</summary>
    [Serializable]
    public class TestContext
    {
        /// <summary>誰から</summary>
        public string Who = "client";
    }

    /// <summary>テスト用の引数</summary>
    [Serializable]
    public class TestParam : BaseParameterValue
    {
        /// <summary>コンストラクタ</summary>
        public TestParam() : base("testScreen", "testControl", "testAction") { }

        /// <summary>本文</summary>
        public string Text = "";
    }

    /// <summary>テスト用の戻り値</summary>
    [Serializable]
    public class TestReturn : BaseReturnValue
    {
        /// <summary>本文</summary>
        public string Text = "";
    }

    /// <summary>
    /// WCF TCP/IP のサービス スタブ（#561）
    /// </summary>
    /// <remarks>
    /// **HTTP 側の HandleOrigin / BuildResponse に相当する。**
    /// 応答の作り方も揃えてある（エラー情報は空、戻り値は TestReturn）。
    ///
    /// 受け取った内容を静的フィールドに記録する。
    /// **サーバが何を受け取ったかを見ないと、届いたかどうかを判定できない**ためで、
    /// これも HTTP 側（Recorded）と同じ考え方である。
    /// </remarks>
    public class WcfTcpStub : IWCFTCPSvcForFx
    {
        /// <summary>受け取ったサービス名</summary>
        public static string ServiceName = null;

        /// <summary>受け取った引数の本文</summary>
        public static string ParamText = null;

        /// <summary>受け取った回数</summary>
        public static int Count = 0;

        /// <summary>記録を消す</summary>
        public static void Clear()
        {
            WcfTcpStub.ServiceName = null;
            WcfTcpStub.ParamText = null;
            WcfTcpStub.Count = 0;
        }

        /// <summary>サービス インターフェイス基盤（.NETオンライン）</summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="contextObject">コンテキスト</param>
        /// <param name="parameterValueObject">引数</param>
        /// <param name="returnValueObject">戻り値</param>
        /// <returns>エラー情報のバイト配列</returns>
        public byte[] DotNETOnlineTCP(
            string serviceName, ref byte[] contextObject,
            byte[] parameterValueObject, out byte[] returnValueObject)
        {
            TestParam param = (TestParam)BinarySerialize.BytesToObject(parameterValueObject);

            lock (typeof(WcfTcpStub))
            {
                WcfTcpStub.ServiceName = serviceName;
                WcfTcpStub.ParamText = param.Text;
                WcfTcpStub.Count++;
            }

            TestReturn ret = new TestReturn();
            ret.Text = "サーバからの戻り値";
            returnValueObject = BinarySerialize.ObjectToBytes(ret);

            // 受け取ったコンテキストは、そのまま返す（contextObject には触らない）

            // エラー情報：無し（空文字。Invoke 側がこれを「正常」と判定する）
            return BinarySerialize.ObjectToBytes("");
        }
    }

    #endregion

    /// <summary>
    /// 通信制御（CallController）の接続オプションを、外部環境なしで確認する。
    /// </summary>
    /// <remarks>
    /// ＜何を見るか＞（#546）
    ///   TMProtocolDefinition.xml の Prop に書いた接続オプションが、
    ///   実際の HTTP 要求に反映されているかを見る。
    ///   オプションはクライアント側の設定なので、**サーバが受け取った内容**を
    ///   記録して突き合わせないと、効いたかどうかを判定できない。
    ///
    /// ＜1 プロセスに閉じている理由＞
    ///   オリジンとプロキシを別プロセスにすると、起動順と後始末が要る。
    ///   また BinaryFormatter の型解決も、同一プロセスなら確実である。
    ///
    /// ＜HttpListener を使わない理由＞
    ///   URL の予約（netsh http add urlacl）が要る場合がある。
    ///   TcpListener なら、その手当てなしに動く。
    ///
    /// ＜対象外＞
    ///   ・Domain / PDomain … Windows 統合認証が要る（Basic 認証では無視される）
    ///   ・ConnGroupName    … HttpClient へ移って設定する口が無くなった。
    ///                        目的（接続の仕切り）は、CallController が
    ///                        サービス名ごとにハンドラをプールすることで満たされている
    /// </remarks>
    class Program
    {
        #region 定数・変数

        /// <summary>オリジンのポート</summary>
        private const int OriginPort = 51090;

        /// <summary>プロキシのポート</summary>
        private const int ProxyPort = 51091;

        /// <summary>オリジン（TLS）のポート</summary>
        private const int TlsPort = 51092;

        /// <summary>WCF TCP/IP のアドレス（定義 XML の url と合わせる）（#561）</summary>
        /// <remarks>
        /// **net.tcp の自己ホストは URL ACL を要求しない**ので、管理者権限は要らない。
        /// （HttpListener を避けた理由は上の「HttpListener を使わない理由」を参照）
        /// </remarks>
        private const string WcfTcpUrl = "net.tcp://127.0.0.1:51093/TestTransmission/WCFTCPSvcForFx/";

        /// <summary>クライアント証明書のファイル名（定義 XML の CertFile と合わせる）</summary>
        private const string ClientCertFile = "TestClient.pfx";

        /// <summary>クライアント証明書のパスワード（定義 XML の CertPassword と合わせる）</summary>
        private const string ClientCertPassword = "pfxpass";

        /// <summary>クライアント証明書のサブジェクト</summary>
        private const string ClientCertSubject = "CN=OpenTouryoTestClient";

        /// <summary>オリジンが受け取った内容</summary>
        private static readonly Recorded OriginRec = new Recorded();

        /// <summary>プロキシが受け取った内容</summary>
        private static readonly Recorded ProxyRec = new Recorded();

        /// <summary>オリジンが認証を要求するか</summary>
        private static bool RequireAuth = false;

        /// <summary>プロキシが認証を要求するか</summary>
        private static bool RequireProxyAuth = false;

        /// <summary>NG の件数</summary>
        private static int NG = 0;

        #endregion

        /// <summary>エントリ ポイント</summary>
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            #region TLS の準備（#546 段 4）

            // **自己署名の証明書を、実行のたびに作る。**
            //   リポジトリに証明書を置くと、期限切れで或る日突然テストが落ちる。
            //   作るのは一瞬なので、毎回作る方が確実である。
            X509Certificate2 serverCert = Program.CreateSelfSigned("CN=OpenTouryoTestServer");
            X509Certificate2 clientCert = Program.CreateSelfSigned(Program.ClientCertSubject);

            // クライアント証明書は、定義 XML の CertFile が指すパスに書き出す。
            // 実行時に作るので、パスは実行ファイルの隣（相対パス）で固定する。
            File.WriteAllBytes(Program.ClientCertFile,
                clientCert.Export(X509ContentType.Pfx, Program.ClientCertPassword));

            // **自己署名なので、クライアント側の検証を通す。**
            //   CallController は ServerCertificateValidationCallback を公開していないため、
            //   ServicePointManager 側（プロセス全体）で受け入れる。テスト専用の措置である。
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate { return true; };

            #endregion

            TcpListener origin = Program.StartOrigin();
            TcpListener proxy = Program.StartProxy();
            TcpListener tls = Program.StartListener(Program.TlsPort, Program.HandleOrigin, serverCert);
            ServiceHost wcf = Program.StartWcfTcp();

            try
            {
                //          表題                  サービス名        認証   Px認証 UserAgent                  gzip   ユーザ           プロキシ
                Program.Case("素の呼び出し", "testPlain", false, false, null, false, null, false, null);
                Program.Case("UserAgent", "testUA", false, false, "OpenTouryoTestAgent/1.0", false, null, false, null);
                Program.Case("Compression", "testGzip", false, false, null, true, null, false, null);
                Program.Case("認証", "testAuth", true, false, null, false, "fxuser:fxpass", false, null);
                Program.Case("プロキシ経由", "testProxy", false, false, null, false, null, true, null);
                Program.Case("プロキシ認証", "testProxyAuth", false, true, null, false, null, true, "pxuser:pxpass");
                Program.Case("全オプション", "testAll", true, true, "OpenTouryoTestAgent/1.0", true, "fxuser:fxpass", true, "pxuser:pxpass");

                // クライアント証明書（TLS。プロキシは経由しない）
                Program.Case("クライアント証明書", "testCert", false, false, null, false, null, false, null,
                    Program.ClientCertSubject);

                // WCF TCP/IP（#561）
                Program.CaseWcfTcp("WCF TCP/IP", "testWcfTcp");
            }
            finally
            {
                origin.Stop();
                proxy.Stop();
                tls.Stop();

                if (wcf != null)
                {
                    try { wcf.Close(); }
                    catch { wcf.Abort(); }
                }
            }

            Console.WriteLine();
            Console.WriteLine("NG : {0} 件", Program.NG);
        }

        #region 検証

        /// <summary>1 ケースを実行して判定する</summary>
        /// <param name="title">表題</param>
        /// <param name="serviceName">サービス名（TMProtocolDefinition.xml の Transmission）</param>
        /// <param name="requireAuth">オリジンが認証を要求するか</param>
        /// <param name="requireProxyAuth">プロキシが認証を要求するか</param>
        /// <param name="expectUserAgent">期待する User-Agent（不要なら null）</param>
        /// <param name="expectGzip">Accept-Encoding に gzip を期待するか</param>
        /// <param name="expectAuth">期待する Basic の中身（不要なら null）</param>
        /// <param name="expectProxy">プロキシを経由することを期待するか</param>
        /// <param name="expectProxyAuth">期待するプロキシ Basic の中身（不要なら null）</param>
        private static void Case(
            string title, string serviceName, bool requireAuth, bool requireProxyAuth,
            string expectUserAgent, bool expectGzip, string expectAuth,
            bool expectProxy, string expectProxyAuth, string expectClientCert = null)
        {
            Program.RequireAuth = requireAuth;
            Program.RequireProxyAuth = requireProxyAuth;

            Console.WriteLine();
            Console.WriteLine("=== {0}（{1}）===", title, serviceName);

            lock (Program.OriginRec) { Program.OriginRec.Clear(); }
            lock (Program.ProxyRec) { Program.ProxyRec.Clear(); }

            #region 呼び出し

            string returned = null;

            try
            {
                TestParam param = new TestParam();
                param.Text = "こんにちは";

                CallController cc = new CallController(new TestContext());
                TestReturn ret = (TestReturn)cc.Invoke(serviceName, param);

                returned = (ret == null) ? null : ret.Text;
            }
            catch (Exception ex)
            {
                Exception e = ex;
                while (e.InnerException != null) { e = e.InnerException; }
                Console.WriteLine("  [NG] 例外 : {0} : {1}", e.GetType().Name, e.Message);
                Program.NG++;
                return;
            }

            #endregion

            #region 判定

            Program.Check("戻り値", "サーバからの戻り値", returned);

            lock (Program.OriginRec)
            {
                Program.Check("オリジンへ到達", true, Program.OriginRec.Count > 0);

                if (expectUserAgent != null)
                {
                    Program.Check("User-Agent", expectUserAgent, Program.OriginRec.Header("User-Agent"));
                }

                if (expectGzip)
                {
                    string accept = Program.OriginRec.Header("Accept-Encoding");
                    Program.Check("Accept-Encoding に gzip", true,
                        accept != null && accept.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (expectAuth != null)
                {
                    // 認証は 401 を挟むため、要求は 2 回になる
                    Program.Check("認証の再送", 2, Program.OriginRec.Count);
                    Program.Check("Authorization", expectAuth, Program.Basic(Program.OriginRec.Header("Authorization")));
                }

                if (expectClientCert != null)
                {
                    // TLS の握手でサーバが受け取ったクライアント証明書
                    Program.Check("クライアント証明書", expectClientCert, Program.OriginRec.ClientCertSubject);
                }
            }

            lock (Program.ProxyRec)
            {
                Program.Check("プロキシ経由", expectProxy, Program.ProxyRec.Count > 0);

                if (expectProxy)
                {
                    // プロキシ宛は絶対 URI になる
                    Program.Check("要求行が絶対 URI", true,
                        Program.ProxyRec.RequestLine.IndexOf(" http://", StringComparison.Ordinal) > 0);
                }

                if (expectProxyAuth != null)
                {
                    Program.Check("Proxy-Authorization", expectProxyAuth,
                        Program.Basic(Program.ProxyRec.Header("Proxy-Authorization")));
                }
            }

            #endregion
        }

        /// <summary>WCF TCP/IP の 1 ケースを実行して判定する（#561）</summary>
        /// <param name="title">表題</param>
        /// <param name="serviceName">サービス名（TMProtocolDefinition.xml の Transmission）</param>
        /// <remarks>
        /// **HTTP 側の Case とは見る対象が違う**ので、別の関数にしてある。
        /// 接続オプション（UserAgent・gzip・プロキシ）は HTTP のもので、WCF には無い。
        ///
        /// ここで見たいのは「**呼び出しがサービスまで届き、戻り値が返るか**」である。
        /// これが通らなくなったのが #561 で、呼び出しが空だったため
        /// returnValueObject が null のまま例外になっていた。
        /// </remarks>
        private static void CaseWcfTcp(string title, string serviceName)
        {
            Console.WriteLine();
            Console.WriteLine("=== {0}（{1}）===", title, serviceName);

            WcfTcpStub.Clear();

            string returned = null;

            try
            {
                TestParam param = new TestParam();
                param.Text = "こんにちは";

                CallController cc = new CallController(new TestContext());
                TestReturn ret = (TestReturn)cc.Invoke(serviceName, param);

                returned = (ret == null) ? null : ret.Text;
            }
            catch (Exception ex)
            {
                Exception e = ex;
                while (e.InnerException != null) { e = e.InnerException; }
                Console.WriteLine("  [NG] 例外 : {0} : {1}", e.GetType().Name, e.Message);
                Program.NG++;
                return;
            }

            Program.Check("戻り値", "サーバからの戻り値", returned);

            lock (typeof(WcfTcpStub))
            {
                // **サービスに届いたか。** 届かずに戻り値だけ合う、ということは起きないが、
                // 「呼び出しが空」の状態を確実に捕まえるために、受信側でも見る。
                Program.Check("サービスへ到達", 1, WcfTcpStub.Count);
                Program.Check("サービス名", serviceName, WcfTcpStub.ServiceName);
                Program.Check("引数", "こんにちは", WcfTcpStub.ParamText);
            }
        }

        /// <summary>期待値と突き合わせて出力する</summary>
        /// <param name="name">項目名</param>
        /// <param name="expected">期待値</param>
        /// <param name="actual">実測値</param>
        private static void Check(string name, object expected, object actual)
        {
            bool ok = object.Equals(expected, actual);
            if (!ok) { Program.NG++; }

            Console.WriteLine("  [{0}] {1,-22} : {2}",
                ok ? "OK" : "NG", name,
                ok ? Program.Show(actual) : Program.Show(actual) + "（期待 : " + Program.Show(expected) + "）");
        }

        /// <summary>値を出力用の文字列にする</summary>
        /// <param name="o">値</param>
        /// <returns>文字列</returns>
        private static string Show(object o)
        {
            return (o == null) ? "(なし)" : o.ToString();
        }

        /// <summary>Basic 認証の値を復号する</summary>
        /// <param name="value">ヘッダの値</param>
        /// <returns>「ユーザ:パスワード」（Basic でなければ元の値）</returns>
        private static string Basic(string value)
        {
            if (value == null || !value.StartsWith("Basic ")) { return value; }

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(value.Substring(6)));
            }
            catch
            {
                return value;
            }
        }

        #endregion

        #region 証明書

        /// <summary>自己署名の証明書を作る</summary>
        /// <param name="subject">サブジェクト（例 : CN=Xxx）</param>
        /// <returns>秘密鍵付きの証明書</returns>
        /// <remarks>
        /// **証明書はリポジトリに置かず、実行のたびに作る。**（#546 段 4）
        ///   置くと期限切れで或る日突然テストが落ちる。
        ///
        /// **Exportable で作り直している理由。**
        ///   CertificateRequest が返す証明書の秘密鍵は、そのままでは
        ///   SslStream のサーバ認証や PFX への書き出しに使えないことがある。
        ///   いったん PFX に通して読み直すと、確実に扱える形になる。
        /// </remarks>
        private static X509Certificate2 CreateSelfSigned(string subject)
        {
            using (RSA rsa = RSA.Create(2048))
            {
                CertificateRequest request = new CertificateRequest(
                    new X500DistinguishedName(subject), rsa,
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // サーバ認証とクライアント認証の両方に使えるようにする
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection
                        {
                            new Oid("1.3.6.1.5.5.7.3.1"),   // サーバ認証
                            new Oid("1.3.6.1.5.5.7.3.2")    // クライアント認証
                        }, false));

                X509Certificate2 cert = request.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

                return new X509Certificate2(
                    cert.Export(X509ContentType.Pfx, "temp"), "temp",
                    X509KeyStorageFlags.Exportable);
            }
        }

        #endregion

        #region WCF TCP/IP のホスト（#561）

        /// <summary>WCF TCP/IP のサービスを自己ホストで起動する</summary>
        /// <returns>ServiceHost</returns>
        /// <remarks>
        /// **セキュリティは None にする。** 既定の netTcpBinding は Transport
        /// （Windows 資格情報）で、クライアント側（App.config）と揃える必要がある。
        /// ここで見たいのは通信そのものなので、両側とも None に揃えてある。
        /// </remarks>
        private static ServiceHost StartWcfTcp()
        {
            ServiceHost host = new ServiceHost(typeof(WcfTcpStub), new Uri(Program.WcfTcpUrl));

            host.AddServiceEndpoint(
                typeof(IWCFTCPSvcForFx), new NetTcpBinding(SecurityMode.None), "");

            host.Open();

            return host;
        }

        #endregion

        #region オリジン

        /// <summary>オリジンを起動する</summary>
        /// <returns>TcpListener</returns>
        private static TcpListener StartOrigin()
        {
            return Program.StartListener(Program.OriginPort, Program.HandleOrigin, null);
        }

        /// <summary>オリジンの応答</summary>
        /// <param name="ns">NetworkStream</param>
        private static void HandleOrigin(Stream ns)
        {
            string requestLine;
            Dictionary<string, string> headers;
            byte[] body;

            if (!Program.ReadRequest(ns, out requestLine, out headers, out body)) { return; }

            lock (Program.OriginRec)
            {
                Program.OriginRec.Count++;
                Program.OriginRec.RequestLine = requestLine;
                foreach (KeyValuePair<string, string> h in headers) { Program.OriginRec.Headers[h.Key] = h.Value; }
            }

            // 認証が要る場合、最初の要求には 401 を返して Basic を要求する。
            // HttpClient は資格情報が設定されていれば、Authorization を付けて再送する。
            if (Program.RequireAuth && !headers.ContainsKey("Authorization"))
            {
                Program.Write(ns,
                    "HTTP/1.1 401 Unauthorized\r\n"
                    + "WWW-Authenticate: Basic realm=\"fx\"\r\n"
                    + "Content-Length: 0\r\n"
                    + "Connection: close\r\n\r\n");
                return;
            }

            byte[] payload = Encoding.UTF8.GetBytes(Program.BuildResponse(body));
            string contentEncoding = "";

            // 要求が gzip を受け付けるなら、圧縮して返す。
            // クライアント側で復号できているかは、戻り値が取れるかで分かる。
            string accept = headers.ContainsKey("Accept-Encoding") ? headers["Accept-Encoding"] : "";

            if (accept.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                payload = Program.Gzip(payload);
                contentEncoding = "Content-Encoding: gzip\r\n";
            }

            Program.Write(ns,
                "HTTP/1.1 200 OK\r\n"
                + "Content-Type: application/json; charset=utf-8\r\n"
                + contentEncoding
                + "Content-Length: " + payload.Length + "\r\n"
                + "Connection: close\r\n\r\n");

            ns.Write(payload, 0, payload.Length);
            ns.Flush();
        }

        /// <summary>CallController が期待する形の応答を作る</summary>
        /// <param name="requestBody">要求の本文</param>
        /// <returns>JSON</returns>
        /// <remarks>
        /// 要求 : {"ServiceName":…,"ContextObject":base64,"ParameterValueObject":base64}
        /// 応答 : {"Return":base64,"ContextObject":base64,"ReturnValueObject":base64}
        /// Return が「空文字のバイト列」なら、例外なしと解釈される。
        /// </remarks>
        private static string BuildResponse(byte[] requestBody)
        {
            JObject request = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(requestBody));

            TestReturn ret = new TestReturn();
            ret.Text = "サーバからの戻り値";

            return JsonConvert.SerializeObject(new
            {
                Return = CustomEncode.ToBase64String(BinarySerialize.ObjectToBytes("")),

                // 受け取ったコンテキストは、そのまま返す
                ContextObject = (string)request["ContextObject"],

                ReturnValueObject = CustomEncode.ToBase64String(BinarySerialize.ObjectToBytes(ret))
            });
        }

        /// <summary>gzip で圧縮する</summary>
        /// <param name="raw">元のバイト列</param>
        /// <returns>圧縮後のバイト列</returns>
        private static byte[] Gzip(byte[] raw)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gz.Write(raw, 0, raw.Length);
                }

                return ms.ToArray();
            }
        }

        #endregion

        #region プロキシ

        /// <summary>プロキシを起動する</summary>
        /// <returns>TcpListener</returns>
        /// <remarks>平文 HTTP の順方向プロキシ。HTTPS を通すには CONNECT の実装が要る。</remarks>
        private static TcpListener StartProxy()
        {
            return Program.StartListener(Program.ProxyPort, Program.HandleProxy, null);
        }

        /// <summary>プロキシの中継</summary>
        /// <param name="ns">NetworkStream</param>
        private static void HandleProxy(Stream ns)
        {
            string requestLine;
            Dictionary<string, string> headers;
            byte[] body;

            if (!Program.ReadRequest(ns, out requestLine, out headers, out body)) { return; }

            lock (Program.ProxyRec)
            {
                Program.ProxyRec.Count++;
                Program.ProxyRec.RequestLine = requestLine;
                foreach (KeyValuePair<string, string> h in headers) { Program.ProxyRec.Headers[h.Key] = h.Value; }
            }

            if (Program.RequireProxyAuth && !headers.ContainsKey("Proxy-Authorization"))
            {
                Program.Write(ns,
                    "HTTP/1.1 407 Proxy Authentication Required\r\n"
                    + "Proxy-Authenticate: Basic realm=\"px\"\r\n"
                    + "Content-Length: 0\r\n"
                    + "Connection: close\r\n\r\n");
                return;
            }

            // 要求行「POST http://host:port/path HTTP/1.1」から、パスを取り出す
            string[] parts = requestLine.Split(' ');
            Uri target = new Uri(parts[1]);

            // **ホスト名は解決せず、必ずオリジンへ中継する。**
            //
            //   宛先に実在しない名前（fx-origin.test）を使っているのは、
            //   .NET Framework の WebProxy.IsBypassed が
            //   **ループバック宛を常に迂回する**ためである（.NET (Core) は迂回しない）。
            //   127.0.0.1 や localhost を宛先にすると、プロキシを設定していても
            //   直接オリジンへ行ってしまい、プロキシの検証にならない。
            //
            //   名前解決させると hosts ファイルの編集（管理者権限）が要るので、
            //   このテスト用プロキシが名前を無視して繋ぎ替える。
            using (TcpClient upstream = new TcpClient("127.0.0.1", Program.OriginPort))
            using (NetworkStream us = upstream.GetStream())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(parts[0] + " " + target.PathAndQuery + " " + parts[2] + "\r\n");

                foreach (KeyValuePair<string, string> h in headers)
                {
                    // プロキシ向けのヘッダは、そのまま送らない
                    if (h.Key.StartsWith("Proxy-", StringComparison.OrdinalIgnoreCase)) { continue; }
                    sb.Append(h.Key + ": " + h.Value + "\r\n");
                }

                sb.Append("Connection: close\r\n\r\n");

                Program.Write(us, sb.ToString());
                if (body.Length > 0) { us.Write(body, 0, body.Length); }
                us.Flush();

                // 応答は、そのまま返す
                byte[] buffer = new byte[8192];
                int length;

                while ((length = us.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ns.Write(buffer, 0, length);
                }

                ns.Flush();
            }
        }

        #endregion

        #region HTTP の読み書き

        /// <summary>接続を受け付けて処理する</summary>
        /// <param name="port">ポート</param>
        /// <param name="handler">処理</param>
        /// <param name="serverCert">サーバ証明書（null なら平文）</param>
        /// <returns>TcpListener</returns>
        /// <remarks>
        /// **サーバ証明書を渡すと TLS になる。**（#546 段 4）
        ///   SslStream を挟むだけなので、`netsh http add sslcert` によるポートへの
        ///   証明書の紐付け（管理者権限）が要らない。Kestrel も要らない。
        /// </remarks>
        private static TcpListener StartListener(int port, Action<Stream> handler, X509Certificate2 serverCert)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            ThreadPool.QueueUserWorkItem(delegate
            {
                while (true)
                {
                    TcpClient client;

                    // Stop() すると例外で抜ける
                    try { client = listener.AcceptTcpClient(); }
                    catch { return; }

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            using (client)
                            using (NetworkStream ns = client.GetStream())
                            {
                                if (serverCert == null)
                                {
                                    handler(ns);
                                }
                                else
                                {
                                    // **クライアント証明書の検証を通す。**
                                    //   コールバックを渡さないと既定の検証になり、
                                    //   自己署名のクライアント証明書は
                                    //   「信頼されていない機関によって発行された」として
                                    //   握手ごと失敗する。ここで見たいのは
                                    //   「証明書が送られてきたか」なので、検証はしない。
                                    using (SslStream ssl = new SslStream(ns, false,
                                        delegate { return true; }))
                                    {
                                        // **クライアント証明書を要求する。**
                                        // 要求しないと、クライアントが設定していても送られてこない。
                                        ssl.AuthenticateAsServer(
                                            serverCert, true, SslProtocols.Tls12, false);

                                        lock (Program.OriginRec)
                                        {
                                            X509Certificate remote = ssl.RemoteCertificate;
                                            Program.OriginRec.ClientCertSubject =
                                                (remote == null) ? null : remote.Subject;
                                        }

                                        handler(ssl);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("  [NG] サーバ側で例外 : {0} : {1}", ex.GetType().Name, ex.Message);
                            Program.NG++;
                        }
                    });
                }
            });

            return listener;
        }

        /// <summary>要求行・ヘッダ・本文を読む</summary>
        /// <param name="ns">NetworkStream</param>
        /// <param name="requestLine">要求行</param>
        /// <param name="headers">ヘッダ</param>
        /// <param name="body">本文</param>
        /// <returns>読めたら true</returns>
        private static bool ReadRequest(Stream ns, out string requestLine,
            out Dictionary<string, string> headers, out byte[] body)
        {
            requestLine = "";
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            body = new byte[0];

            #region ヘッダ

            // **本文を読み過ぎないよう、CRLF CRLF まで 1 バイトずつ読む。**
            MemoryStream head = new MemoryStream();
            int b;
            int match = 0;

            while ((b = ns.ReadByte()) >= 0)
            {
                head.WriteByte((byte)b);

                if ((match == 0 || match == 2) && b == '\r') { match++; }
                else if ((match == 1 || match == 3) && b == '\n') { match++; }
                else { match = (b == '\r') ? 1 : 0; }

                if (match == 4) { break; }
            }

            if (head.Length == 0) { return false; }

            string[] lines = Encoding.ASCII.GetString(head.ToArray())
                .Split(new string[] { "\r\n" }, StringSplitOptions.None);

            requestLine = lines[0];

            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i] == "") { break; }

                int colon = lines[i].IndexOf(':');
                if (colon <= 0) { continue; }

                headers[lines[i].Substring(0, colon).Trim()] = lines[i].Substring(colon + 1).Trim();
            }

            #endregion

            #region 本文

            if (headers.ContainsKey("Content-Length"))
            {
                int length = int.Parse(headers["Content-Length"]);
                body = new byte[length];

                int read = 0;

                while (read < length)
                {
                    int n = ns.Read(body, read, length - read);
                    if (n <= 0) { break; }
                    read += n;
                }
            }

            #endregion

            return true;
        }

        /// <summary>文字列を送る</summary>
        /// <param name="ns">NetworkStream</param>
        /// <param name="text">文字列</param>
        private static void Write(Stream ns, string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            ns.Write(bytes, 0, bytes.Length);
            ns.Flush();
        }

        #endregion
    }
}
