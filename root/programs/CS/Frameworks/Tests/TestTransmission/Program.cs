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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
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
    ///   ・CertFile / CertPassword … TLS とクライアント証明書の要求が要る
    ///   ・Domain / PDomain        … Windows 統合認証が要る（Basic では無視される）
    ///   ・ConnGroupName           … CallController が読んでいない（未実装）
    /// </remarks>
    class Program
    {
        #region 定数・変数

        /// <summary>オリジンのポート</summary>
        private const int OriginPort = 51090;

        /// <summary>プロキシのポート</summary>
        private const int ProxyPort = 51091;

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

            TcpListener origin = Program.StartOrigin();
            TcpListener proxy = Program.StartProxy();

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
            }
            finally
            {
                origin.Stop();
                proxy.Stop();
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
            bool expectProxy, string expectProxyAuth)
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

        #region オリジン

        /// <summary>オリジンを起動する</summary>
        /// <returns>TcpListener</returns>
        private static TcpListener StartOrigin()
        {
            return Program.StartListener(Program.OriginPort, Program.HandleOrigin);
        }

        /// <summary>オリジンの応答</summary>
        /// <param name="ns">NetworkStream</param>
        private static void HandleOrigin(NetworkStream ns)
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
            return Program.StartListener(Program.ProxyPort, Program.HandleProxy);
        }

        /// <summary>プロキシの中継</summary>
        /// <param name="ns">NetworkStream</param>
        private static void HandleProxy(NetworkStream ns)
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
        /// <returns>TcpListener</returns>
        private static TcpListener StartListener(int port, Action<NetworkStream> handler)
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
                                handler(ns);
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
        private static bool ReadRequest(NetworkStream ns, out string requestLine,
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
        private static void Write(NetworkStream ns, string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            ns.Write(bytes, 0, bytes.Length);
            ns.Flush();
        }

        #endregion
    }
}
