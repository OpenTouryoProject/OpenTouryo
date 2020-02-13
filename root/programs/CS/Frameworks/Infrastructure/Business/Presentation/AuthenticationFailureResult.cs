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
//* クラス名        ：AuthenticationFailureResult
//* クラス日本語名  ：AuthenticationFailureResult
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/02/12  西野 大介         新規作成
//**********************************************************************************

using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>AuthenticationFailureResult</summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        #region インスタンス変数

        /// <summary>RequestMessage</summary>
        public HttpRequestMessage RequestMessage { get; private set; }


        /// <summary>ReasonPhrase</summary>
        public string ReasonPhrase { get; private set; }

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="requestMessage">HttpRequestMessage</param>
        /// <param name="reasonPhrase">string</param>
        public AuthenticationFailureResult(HttpRequestMessage requestMessage, string reasonPhrase)
        {
            this.RequestMessage = requestMessage;
            this.ReasonPhrase = reasonPhrase;
        }

        #endregion

        /// <summary>ExecuteAsync</summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Task(HttpResponseMessage)</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        /// <summary>Execute</summary>
        /// <returns>HttpResponseMessage</returns>
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = this.RequestMessage;
            response.ReasonPhrase = this.ReasonPhrase;
            return response;
        }
    }
}
