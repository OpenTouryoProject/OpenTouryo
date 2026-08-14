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
//* クラス名        ：CallController
//* クラス日本語名  ：クライアント ライブラリ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/02  自動生成          新規作成
//*  2009/04/21  西野 大介         FrameworkExceptionの追加に伴い、実装追加
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#18 ： SoapException対策（例外情報の転送）
//*                                ・#30 ： レイトバインド時のTargetInvocationExceptionを考慮
//*                                ・#32 ： チェック処理のメッセージ変更でデグレ
//*                                         → String.Formatで引数不足のエラー。
//*                                ・#34 ： ユーザエージェント名にユーザ名が指定。
//*                                ・#35 ： 存在チェック対象記述ミス
//*                                ・#36 ： 固定プロパティの見直し
//*                                ・#x  ： リファクタリング
//*                                ・#y  ： 性能対策（I/F：string→byte[]）
//*                                ・#z  ： クライアント証明書対応
//*  2009/08/10  西野 大介         アセンブリ ロード位置の場合分けを実装
//*  2009/09/29  西野 大介         ユーザ情報をコンテキストに（名前の変更）
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2011/05/30  西野 大介         ドメイン指定を省略可能な実装に変更。
//*  2011/12/02  西野 大介         contextObjectにrefを追加した。
//*  2012/06/14  西野 大介         Web参照の（TimeOut、CookieContainer）プロパティ追加
//*  2012/08/25  西野 大介         Assembly.LoadFile → .Load（ASP.NETシャドウコピー対応）
//*  2012/11/28  西野 大介         Web参照を再度自動生成（バグ対応ではなく）
//*  2012/12/14  西野 大介         WCF-HTTP対応
//*  2012/12/17  西野 大介         WCF-TCP/IP対応
//*  2013/01/22  西野 大介         WCF-TCP/IP：Channelの解放（Close、Dispose）を明示
//*  2014/11/14	 Sandeep-san       Implemented WCF communication options
//*  2017/02/14  西野 大介         Invokeの非同期バージョン（InvokeAsync）を追加
//*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れた。
//*  2017/08/18  西野 大介         ASP.NET WebAPI（JSON）対応
//*  2018/03/29  西野 大介         .NET Standard対応：取り敢えずインプロセスのみ残す。
//*  2018/08/04  西野 大介         regionディレクティブのインデントの問題を修正
//*  2018/08/04  西野 大介         HttpClientにpropsの設定値を反映する。
//*  2019/10/01  西野 大介         .NET Standard対応：ASP.NET WebAPI (JSON-RPC)の復元
//*  2021/05/18  西野 大介         ASP.NET WebAPI（JSON）の例外処理の問題を修正
//*  2021/05/18  西野 大介         SOAP、WebAPIのCookieコンテナ対応
//*  2021/05/21  西野 大介         HTTPClientのサーバーサイド実行を可能にするが、
//*                                ・HTTPClientは内部で都度生成せず、
//*                                ・serviceName毎にプールする。
//*                                以下が制約として追加される。
//*                                ・マルチスレッド・クライアントから利用不可。
//*                                  ・Critical Section内で利用する。
//*                                  ・Concurrent Collectionでプールを作成する。
//*  2026/08/13  玄人 幸道         .NET Core版で未対応のprotocolを、nullではなく
//*                                FrameworkExceptionで返すようにした（#543）。
//**********************************************************************************

using System;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Util;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Reflection;

using System.Text;
using System.Linq;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

#if (NETSTD || NETCOREAPP)
#else
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
//using Org.BouncyCastle.Ocsp;

#pragma warning disable 1591

namespace Touryo.Infrastructure.Framework.Transmission
{

    /// <summary>クライアント ライブラリ</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class CallController
    {
        #region グローバル変数

        /// <summary>呼び出しプロトコルの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static ProtocolNameService PRT_NS = new ProtocolNameService();

        /// <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static InProcessNameService IPR_NS = new InProcessNameService();

        #endregion

        #region インスタンス変数

        #region コンテキスト関連

        /// <summary>コンテキスト情報</summary>
        private object _context;

        /// <summary>コンテキスト情報</summary>
        /// <remarks>自由に利用できる。</remarks>
        public object Context
        {
            set { this._context = value; }
            get { return this._context; }
        }

        #endregion

        #region CookieContainer
        /// <summary>CookieContainer</summary>
        public CookieContainer CookieContainer
        {
            set;
            get;
        }
        #endregion

        #region Web参照と関連プロパティ

        #region ASPNETWebAPI → HttpClient

#if (NETSTD || NETCOREAPP)
        /// <summary>HttpClientHandlerのDictionary</summary>
        private Dictionary<string, HttpClientHandler> _handlerCD = new Dictionary<string, HttpClientHandler>();
#else
        /// <summary>WebRequestHandlerのDictionary</summary>
        private Dictionary<string, WebRequestHandler> _handlerCD = new Dictionary<string, WebRequestHandler>();
#endif

        /// <summary>HttpClientのDictionary</summary>
        private Dictionary<string, HttpClient> _httpClientCD = new Dictionary<string, HttpClient>();

        #endregion

        #region プロキシのURL

        /// <summary>プロキシのURL</summary>
        private string _proxyUrl = "";

        /// <summary>プロキシのURL</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ProxyUrl
        {
            set { this._proxyUrl = value; }
        }

        #endregion

        #region セキュリティ資格情報

        /// <summary>クライアント認証情報（WAS）</summary>
        private NetworkCredential _nwcWAS;

        /// <summary>クライアント認証情報（WAS）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public NetworkCredential NetworkCredentialToWAS
        {
            set { this._nwcWAS = value; }
        }

        /// <summary>クライアント認証情報（Proxy）</summary>
        private NetworkCredential _nwcProxy;

        /// <summary>クライアント認証情報（Proxy）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public NetworkCredential NetworkCredentialToProxy
        {
            set { this._nwcProxy = value; }
        }

        #endregion

        #endregion

        #endregion

        #region Constructor

        /// <summary>コンストラクタ</summary>
        /// <param name="context">コンテキスト情報</param>
        /// <remarks>自由に利用できる。</remarks>
        public CallController(object context)
        {
            // コンテキスト情報
            this._context = context;
        }

        #endregion

        #region Invoke

        #region 非同期バージョン

        /// <summary>Invokeメソッドの非同期バージョン</summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="parameterValue">引数</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public object InvokeAsync(string serviceName, object parameterValue)
        {
            return Task<object>.Factory.StartNew(() =>
            {
                return this.Invoke(serviceName, parameterValue);
            });
        }

        #endregion

        /// <summary>サービス インターフェイスに対するクライアント モジュール</summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="parameterValue">引数</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる。</remarks>
        public object Invoke(string serviceName, object parameterValue)
        {
            #region 呼出し制御関係の変数

            // プロトコル
            string protocol = "";

            // モジュール名
            string assemblyName = "";

            // クラス名
            string className = "";

            #endregion

            // 名前解決（プロトコル タイプ）
            CallController.PRT_NS.NameResolutionProtocolType(serviceName, out protocol);

            if (protocol == ((int)FxEnum.TmProtocol.InProcess).ToString())
            {
                #region インプロセス呼び出し

                // 名前解決（インプロセス）
                CallController.IPR_NS.NameResolution(serviceName, out assemblyName, out className);

                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { parameterValue, DbEnum.IsolationLevelEnum.User };

                // #30-start
                try
                {
                    // Ｂ層→Ｄ層呼出し
                    return (BaseReturnValue)Latebind.InvokeMethod(
                            assemblyName, className, FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME, paramSet);
                }
                catch (System.Reflection.TargetInvocationException rtEx)
                {
                    //// InnerExceptionを投げなおす。
                    //throw rtEx.InnerException;

                    // スタックトレースを保って InnerException を throw
                    ExceptionDispatchInfo.Capture(rtEx.InnerException).Throw();

                    return null; // warningを消すためのコード。
                }
                // #30-end                

                #endregion
            }
#if (NETSTD || NETCOREAPP)
            else
            {
                // .NET Core 版が実装するのは InProcess だけで、
                // FxEnum.TmProtocol からも 2 - 5 を落としてある。
                //
                // ＜null を返さない理由＞（#543）
                //   ProtocolNameService は protocol 属性の値を検査せず、
                //   XML に書かれた文字列をそのまま返す。このため、
                //   protocol="4" のような定義があれば**ここに到達する**。
                //   実際、配布している TMProtocolDefinition.xml には
                //   4 と 5 の定義が含まれている。
                //
                //   null を返すと呼び出し側で NullReferenceException になるだけで、
                //   原因が設定の誤りであることに辿り着けない。
                //   .NET Framework 版と同じ例外を投げ、どの protocol が
                //   受け付けられなかったかを示す。

                // エラーをスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prt2, protocol, serviceName)));
            }
#else
            else
            {
                // ＵＲＬ
                string url = "";

                // タイムアウト
                int timeout;

                // プロパティ
                Dictionary<string, string> props;

                // .NETオブジェクトのバイト配列 // #y-↓３行
                byte[] contextObject = null;
                byte[] parameterValueObject = null;
                byte[] returnValueObject = null;

                // エラー情報のバイト配列 // #y-↓１行
                byte[] ret = null;

                // 名前解決（プロトコルＵＲＬ）
                CallController.PRT_NS.NameResolutionProtocolUrl(serviceName, out url, out timeout, out props);

                #region 引数のシリアライズ（#y-このregion）

                // nullチェック
                if (this._context == null)
                {
                    // コンテキスト・nullエラー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[0],
                        String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[1],
                            String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_null, "context")));
                }
                else
                {
                    // 変換
                    contextObject = BinarySerialize.ObjectToBytes(this._context);
                }

                // nullチェック
                if (parameterValue == null)
                {
                    // 引数・nullエラー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[0],
                        String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[1],
                            String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_null, "parameterValue")));
                }
                else
                {
                    // 変換
                    parameterValueObject = BinarySerialize.ObjectToBytes(parameterValue);
                }

                #endregion

                #region サービス呼び出し

                //else
                if (protocol == ((int)FxEnum.TmProtocol.WCF_TCPIP).ToString())
                {
                    #region WCF : netTCPBinding
                                       

                    #endregion
                }
                else if (protocol == ((int)FxEnum.TmProtocol.AspNetWebAPI).ToString())
                {
                    // ASP.NET WebAPI (JSON-RPC)
                    ret = this.ASPNETWebAPI(serviceName, url, timeout, props,
                        contextObject, parameterValueObject, out returnValueObject);
                }
                else
                {
                    // 定義が間違っている（エラー）。

                    // エラーをスロー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                        String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                            String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prt2, protocol, serviceName))); // #14,32-この行
                }

                #endregion
//#endif

                #region 戻り値のデシリアライズ（#y-このregion）

                #region エラー情報

                if (ret == null)
                {
                    // 正常（例外発生：無し）
                }
                else if (Enumerable.SequenceEqual(ret, (BinarySerialize.ObjectToBytes(""))))
                {
                    // 正常（例外発生：無し）
                }
                else
                {
                    // 異常（例外発生：有り）

                    // 例外処理
                    WSErrorInfo wsErrorInfo = (WSErrorInfo)BinarySerialize.BytesToObject(ret);

                    //if (wsErrorInfo.ErrorType == FxEnum.ErrorType.BusinessApplicationException) // #18-このコードブロック
                    //{
                    //    // 業務例外
                    //    throw new BusinessApplicationException(
                    //        wsErrorInfo.ErrorMessageID,
                    //        wsErrorInfo.ErrorMessage,
                    //        wsErrorInfo.ErrorInformation);
                    //}
                    //else
                    if (wsErrorInfo.ErrorType == FxEnum.ErrorType.BusinessSystemException)
                    {
                        // システム例外
                        throw new BusinessSystemException(
                            wsErrorInfo.ErrorMessageID,
                            wsErrorInfo.ErrorMessage);
                    }
                    else if (wsErrorInfo.ErrorType == FxEnum.ErrorType.FrameworkException)
                    {
                        // フレームワーク例外
                        throw new FrameworkException(
                            wsErrorInfo.ErrorMessageID,
                            wsErrorInfo.ErrorMessage);
                    }
                    else if (wsErrorInfo.ErrorType == FxEnum.ErrorType.ElseException)
                    {
                        // その他、一般的な例外
                        throw new Exception(wsErrorInfo.ErrorMessage);
                    }
                }

                #endregion

                #region 処理結果

                // チェック処理
                if (returnValueObject == null)
                {
                    // 戻り値・空エラー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[0],
                        String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR[1],
                            String.Format(FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_null,
                                "returnValueObject")));
                }
                else
                {
                    // 戻り値の復元
                    this._context = BinarySerialize.BytesToObject(contextObject);
                    return (BaseReturnValue)BinarySerialize.BytesToObject(returnValueObject);
                }

                #endregion

                #endregion
            }
#endif
        }

        #endregion

#if (NETSTD || NETCOREAPP)
#else
        #region WCF_TCPIP
        /// <summary>WCF TCPIP</summary>
        /// <param name="serviceName">string</param>
        /// <param name="url">string</param>
        /// <param name="timeout">int</param>
        /// <param name="props">Dictionary(string, string)</param>
        /// <param name="contextObject">byte[]</param>
        /// <param name="parameterValueObject">byte[]</param>
        /// <param name="returnValueObject">byte[]</param>
        /// <returns>エラー情報のバイト配列</returns>
        private byte[] WCF_TCPIP(
            string serviceName, string url, int timeout, Dictionary<string, string> props,
            byte[] contextObject, byte[] parameterValueObject, out byte[] returnValueObject)
        {
            // WCF TCP/IPサービス
            IWCFTCPSvcForFx WCF_TCPIP = null;

            // エラー情報のバイト配列
            byte[] ret = null;

            // 都度newしても接続はプールされているので、オーバーヘッドは少ない（と思われる）。
            ChannelFactory<IWCFTCPSvcForFx> factory = new ChannelFactory<IWCFTCPSvcForFx>(
                FxLiteral.WCF_TCPIP_ENDPOINT_CONFIGNAME, new EndpointAddress(url));

            try
            {
                #region Specifying WCF options: netTCPBinding

                #region WASのクライアント認証のセキュリティ資格情報 for WCF

                NetworkCredential nwcWAS = this.CreateCredentials(props);
                if (nwcWAS != null)
                {
                    factory.Credentials.Windows.ClientCredential = nwcWAS;
                }

                #endregion

                #region クライアント証明書 CERTIFICATE

                X509Certificate2 x509 = this.CreateX509Certificate(props);
                if (x509 != null)
                {
                    factory.Credentials.ClientCertificate.Certificate = x509;
                }

                #endregion

                WCF_TCPIP = factory.CreateChannel();

                #endregion

                // 同期呼び出しで実行
                ret = WCF_TCPIP.DotNETOnlineTCP(
                    serviceName, ref contextObject,
                    parameterValueObject, out returnValueObject);
            }
            finally
            {
                // Close、Disposeを実行する。
                // http://geekswithblogs.net/SoftwareDoneRight/archive/2008/05/23/clean-up-wcf-clients--the-right-way.aspx

                if (WCF_TCPIP != null)
                {
                    ((IClientChannel)WCF_TCPIP).Close();
                    ((IDisposable)WCF_TCPIP).Dispose();
                    WCF_TCPIP = null;
                }
            }

            return ret;
        }

        #endregion
#endif

        #region ASPNETWebAPI
        /// <summary>
        /// ASP.NET WebAPI (JSON-RPC)
        /// .NETCoreでも利用するため共通化
        /// </summary>
        /// <param name="serviceName">string</param>
        /// <param name="url">string</param>
        /// <param name="timeout">int</param>
        /// <param name="props">Dictionary(string, string)</param>
        /// <param name="contextObject">byte[]</param>
        /// <param name="parameterValueObject">byte[]</param>
        /// <param name="returnValueObject">byte[]</param>
        /// <returns>エラー情報のバイト配列</returns>
        private byte[] ASPNETWebAPI(
            string serviceName, string url, int timeout, Dictionary<string, string> props,
            byte[] contextObject, byte[] parameterValueObject, out byte[] returnValueObject)
        {
            #region Handler

            // Equivalent to WebRequestHandler in .net Core · Issue #26223 · dotnet/corefx
            // https://github.com/dotnet/corefx/issues/26223

            #region Handlerの変数宣言
#if (NETSTD || NETCOREAPP)
            #region HttpClientHandler
            HttpClientHandler handler = null;
            #endregion
#else
            #region WebRequestHandler
            WebRequestHandler handler = null;
            #endregion
#endif
            #endregion

            #region DictionaryをCheckし...

            if (!this._handlerCD.ContainsKey(serviceName))
            {
                #region new ...Handler();
#if (NETSTD || NETCOREAPP)
                #region HttpClientHandler
                handler = new HttpClientHandler();
                #endregion
#else
                #region WebRequestHandler
                handler = new WebRequestHandler();
                #endregion
#endif
                #endregion

                #region CookieContainerの設定
                if (this.CookieContainer != null)
                    handler.CookieContainer = this.CookieContainer;
                #endregion

                #region WASのクライアント認証のセキュリティ資格情報 for WCF

                NetworkCredential nwcWAS = this.CreateCredentials(props);
                if (nwcWAS != null)
                {
                    handler.Credentials = nwcWAS;
                }

                #endregion

                #region プロキシ経由の要求を行うためのプロキシ情報

                WebProxy proxy = this.CreateProxy(props);
                if (proxy != null)
                {
                    handler.Proxy = proxy;
                }

                #endregion

                #region HTTP圧縮の有効・無効（Default：false）

                if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION))// Dic化でnullチェック変更
                {
                    // XML定義：キーが無い
                }
                else
                {
                    if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION]))
                    {
                        // XML定義：null or 空文字列
                    }
                    else
                    {
                        // XML定義：あり

                        bool compress;

                        if (Boolean.TryParse(props[FxLiteral.TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION], out compress))
                        {
                            // 書式正常
                            if (compress)
                            {
                                handler.AutomaticDecompression =
                                    DecompressionMethods.GZip | DecompressionMethods.Deflate;
                            }
                        }
                        else
                        {
                            // パラメータ・エラー（書式不正）
                            throw new FrameworkException(
                                FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH2[0],
                                String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH2[1],
                                    FxLiteral.TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION
                                    + "=" + props[FxLiteral.TRANSMISSION_HTTP_PROP_ENABLEDE_COMPRESSION]));
                        }
                    }
                }

                #endregion

                this._handlerCD[serviceName] = handler;
            }
            else
            {
                handler = this._handlerCD[serviceName];
            }
            
            #endregion

            #endregion

            #region HttpClient

            HttpClient client = null;

            #region DictionaryをCheckし...
            if (!this._httpClientCD.ContainsKey(serviceName))
            {
                client = new HttpClient(handler); //, disposeHandler: xxx);
                // HttpClientFactoryはdisposeHandlerをfalseにして、
                // handlerをプールするらしいが、handler構築するには、
                // HttpClientBuilderなどを使う必要があるとか。
            }
            else
            {
                client = this._httpClientCD[serviceName];
            }
            #endregion

            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonConvert.SerializeObject(
                    new
                    {
                        ServiceName = serviceName,
                        ContextObject = CustomEncode.ToBase64String(contextObject),
                        ParameterValueObject = CustomEncode.ToBase64String(parameterValueObject)
                    }),
                    Encoding.UTF8, "application/json"),
            };

            #region タイムアウト
            if (0 <= timeout)
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
            }
            #endregion

            #region クライアント証明書、エージェント ヘッダ、接続グループ.etc

            #region クライアント証明書 CERTIFICATE

            X509Certificate2 x509 = this.CreateX509Certificate(props);
            if (x509 != null)
            {
                handler.ClientCertificates.Add(x509);
            }

            #endregion

            #region ユーザ エージェント ヘッダ値

            // （Default：MS Web Services Client Protocol number、numberは、CLRのver）
            if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_USER_AGENT))// Dic化でnullチェック変更
            {
                // XML定義：キーが無い
            }
            else
            {
                if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_USER_AGENT]))
                {
                    // XML定義：null or 空文字列
                }
                else
                {
                    // XML定義：あり
                    client.DefaultRequestHeaders.Add(
                        "User-Agent",
                        props[FxLiteral.TRANSMISSION_HTTP_PROP_USER_AGENT]);
                }
            }

            #endregion

            #region 接続グループ（Default：Empty）

            // －

            #endregion

            #endregion

            #endregion

            // 同期呼び出しで実行
            httpResponseMessage = client.SendAsync(httpRequestMessage).Result;
            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            JObject jObject = (JObject)JsonConvert.DeserializeObject(result);

            byte[] ret = CustomEncode.FromBase64String((string)jObject["Return"]);
            contextObject = CustomEncode.FromBase64String((string)jObject["ContextObject"]);
            returnValueObject = CustomEncode.FromBase64String((string)jObject["ReturnValueObject"]);

            return ret;
        }
        #endregion

        #region ヘルパー

        /// <summary>CreateProxy</summary>
        /// <param name="props">Dictionary(string, string)</param>
        /// <returns>WebProxy</returns>
        private WebProxy CreateProxy(Dictionary<string, string> props)
        {
            WebProxy proxy = null;

            #region プロキシ生成

            if (string.IsNullOrEmpty(this._proxyUrl))
            {
                // ユーザ指定：なし

                if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_URL))// Dic化でnullチェック変更
                {
                    // XML定義：キーが無い
                }
                else
                {
                    if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_URL]))
                    {
                        // XML定義：null or 空文字列
                    }
                    else
                    {
                        // XML定義：あり

                        // プロキシを生成（XML定義）
                        proxy = new WebProxy(new Uri(props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_URL]));
                    }
                }
            }
            else
            {
                // ユーザ指定：あり

                // プロキシを生成（ユーザ指定）
                proxy = new WebProxy(new Uri(this._proxyUrl));
            }

            #endregion

            #region プロキシのオプション設定

            if (proxy == null)
            {
                // プロキシがない
            }
            else
            {
                // プロキシがある

                // ローカル プロキシをバイパスの有効・無効（Default：false）。 
                proxy.BypassProxyOnLocal = false; // 無効

                // 実行アカウントでのWindows認証の有効・無効（Default：false）。                    
                proxy.UseDefaultCredentials = true; // 有効

                #region Proxyのクライアント認証のセキュリティ資格情報

                // Proxyのセキュリティ資格情報
                if (this._nwcProxy == null)
                {
                    // ユーザ指定：なし

                    if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_USER_NAME))// Dic化でnullチェック変更
                    {
                        // XML定義：キーが無い
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_USER_NAME]))
                        {
                            // XML定義：null or 空文字列
                        }
                        else
                        {
                            // XML定義：あり

                            // Proxyのセキュリティ資格情報を生成する。
                            NetworkCredential nwcProxy = new NetworkCredential();

                            nwcProxy.UserName = props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_USER_NAME];
                            nwcProxy.Password = props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_PASSWORD];

                            // 省略可能に変更した。
                            if (props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_DOMAIN))
                            {
                                nwcProxy.Domain = props[FxLiteral.TRANSMISSION_HTTP_PROP_PROXY_DOMAIN];
                            }

                            // Proxyのセキュリティ資格情報を設定する（XML定義）。
                            proxy.Credentials = nwcProxy;
                        }
                    }
                }
                else
                {
                    // ユーザ指定：あり

                    // Proxyのセキュリティ資格情報を設定する（ユーザ指定）。
                    proxy.Credentials = this._nwcProxy;
                }

                #endregion
            }

            #endregion

            return proxy;
        }

        /// <summary>CreateCredentials</summary>
        /// <param name="props">Dictionary(string, string)</param>
        /// <returns>NetworkCredential</returns>
        private NetworkCredential CreateCredentials(Dictionary<string, string> props)
        {
            NetworkCredential nwcWAS = null;

            // WASのセキュリティ資格情報
            if (this._nwcWAS == null)
            {
                // ユーザ指定：なし

                if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_USER_NAME))// Dic化でnullチェック変更
                {
                    // XML定義：キーが無い
                }
                else
                {
                    if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_USER_NAME]))
                    {
                        // XML定義：null or 空文字列
                    }
                    else
                    {
                        // XML定義：あり

                        // WASのセキュリティ資格情報を生成する。
                        nwcWAS = new NetworkCredential();
                        nwcWAS.UserName = props[FxLiteral.TRANSMISSION_HTTP_PROP_USER_NAME];
                        nwcWAS.Password = props[FxLiteral.TRANSMISSION_HTTP_PROP_PASSWORD];

                        // 省略可能に変更した。
                        if (props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_DOMAIN))
                        {
                            nwcWAS.Domain = props[FxLiteral.TRANSMISSION_HTTP_PROP_DOMAIN];
                        }
                    }
                }
            }
            else
            {
                // ユーザ指定：あり

                // WASのセキュリティ資格情報を設定する（ユーザ指定）。
                nwcWAS = this._nwcWAS;
            }

            return nwcWAS;
        }

        /// <summary>CreateCredentials</summary>
        /// <param name="props">Dictionary(string, string)</param>
        /// <returns>X509Certificate</returns>
        private X509Certificate2 CreateX509Certificate(Dictionary<string, string> props)
        {
            X509Certificate2 x509 = null;

            // http://support.microsoft.com/kb/895971/ja
            // http://msdn.microsoft.com/ja-jp/library/system.security.cryptography.x509certificates.x509certificate.x509certificate.aspx

            if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE))// Dic化でnullチェック変更
            {
                // XML定義：キーが無い
            }
            else
            {
                if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE]))
                {
                    // XML定義：null or 空文字列
                }
                else
                {
                    if (!props.ContainsKey(FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_PASSWORD))// Dic化でnullチェック変更
                    {
                        // XML定義：キーが無い

                        // クライアント証明書のファイルパス
#if NETSTD
                        x509 = X509CertificateLoader.LoadCertificateFromFile(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE]);
#else
                        x509 = new X509Certificate2(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE]); 
#endif
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_PASSWORD]))
                        {
                            // XML定義：null or 空文字列

                            // クライアント証明書のファイルパス
#if NETSTD
                            x509 = X509CertificateLoader.LoadCertificateFromFile(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE]);
#else
                            x509 = new X509Certificate2(props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE]);
#endif
                        }
                        else
                        {
                            // XML定義：あり

                            // クライアント証明書のファイルパス ＋ クライアント証明書ＤＢのパスワード
#if NETSTD
                            x509 = X509CertificateLoader.LoadPkcs12FromFile(
                                props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE],
                                props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_PASSWORD]);
#else
                            x509 = new X509Certificate2(
                                props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_FILE],
                                props[FxLiteral.TRANSMISSION_HTTP_PROP_X509CERTIFICATE_PASSWORD]);
#endif
                        }
                    }
                }
            }

            return x509;
        }

        #endregion
    }
}

#pragma warning restore 1591
