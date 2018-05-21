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
//* クラス名        ：ResultWithChallenge
//* クラス日本語名  ：ResultWithChallenge（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/11  西野 大介         新規作成
//**********************************************************************************

using System.Threading;
using System.Threading.Tasks;

using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>IHttpActionResult</summary>
    public class ResultWithChallenge : IHttpActionResult
    {
        /// <summary>IHttpActionResult</summary>
        private readonly IHttpActionResult next;

        /// <summary>ResultWithChallenge</summary>
        /// <param name="next">IHttpActionResult</param>
        public ResultWithChallenge(IHttpActionResult next)
        {
            this.next = next;
        }

        /// <summary>ExecuteAsync</summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await next.ExecuteAsync(cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(
                  new AuthenticationHeaderValue("somescheme", "somechallenge"));
            }
            return response;
        }
    }
}
