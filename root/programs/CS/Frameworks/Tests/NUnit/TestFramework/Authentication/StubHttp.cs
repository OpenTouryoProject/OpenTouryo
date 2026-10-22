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
//* クラス名        ：StubHttp
//* クラス日本語名  ：OAuth2AndOIDCClient の HttpClient を差し替える、テスト用の応答
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>OAuth2AndOIDCClient の HttpClient を差し替える、テスト用の応答</summary>
    /// <remarks>
    /// ネットワークに出ずに、取得する部品（jwks_uri・メタデータ・DCR・DPoP の nonce）を確かめる。
    /// using で囲めば、元の HttpClient に戻す（HttpClient は static でプロセス全体に効くため）。
    /// </remarks>
    internal sealed class StubHttp : HttpMessageHandler, IDisposable
    {
        /// <summary>差し替える前の HttpClient</summary>
        private readonly HttpClient _original;

        /// <summary>要求から応答を作る処理</summary>
        private readonly Func<HttpRequestMessage, string, HttpResponseMessage> _respond;

        /// <summary>受けた要求（メソッド・URL・ヘッダ・本文）</summary>
        public List<Tuple<HttpRequestMessage, string>> Requests = new List<Tuple<HttpRequestMessage, string>>();

        /// <summary>差し替える</summary>
        /// <param name="respond">要求と本文から、応答を作る</param>
        public StubHttp(Func<HttpRequestMessage, string, HttpResponseMessage> respond)
        {
            this._respond = respond;
            this._original = OAuth2AndOIDCClient.HttpClient;
            OAuth2AndOIDCClient.HttpClient = new HttpClient(this, false);
        }

        /// <summary>JSON の応答を作る</summary>
        public static HttpResponseMessage Json(HttpStatusCode status, string json)
        {
            return new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }

        /// <summary>SendAsync</summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string body = (request.Content == null) ? null : await request.Content.ReadAsStringAsync();
            this.Requests.Add(Tuple.Create(request, body));
            return this._respond(request, body);
        }

        /// <summary>元の HttpClient に戻す</summary>
        void IDisposable.Dispose()
        {
            OAuth2AndOIDCClient.HttpClient = this._original;
        }
    }
}
