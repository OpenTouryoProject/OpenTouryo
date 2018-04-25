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
//* クラス名        ：GetClientIpAddress
//* クラス日本語名  ：GetClientIpAddress
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/04/10  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.Primitives;

using Touryo.Infrastructure.Framework.StdMigration;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>GetClientIpAddress</summary>
    public class GetClientIpAddress
    {
        /// <summary>GetClientIpAddress</summary>
        /// <param name="tryUseXForwardHeader">bool</param>
        /// <returns>IPAddress(文字列)</returns>
        public string GetAddress(bool tryUseXForwardHeader = true)
        {
            // X-Forwarded-For, MS_HttpContext, MS_OwinContext

            // c# - How do I get client IP address in ASP.NET CORE? - Stack Overflow
            // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
            // - X-Forwarded-For - Wikipedia
            //   https://en.wikipedia.org/wiki/X-Forwarded-For
            // - Remote IP Address with Go
            //   https://husobee.github.io/golang/ip-address/2015/12/17/remote-ip-go.html

            string ip = "";

            if (tryUseXForwardHeader)
            {
                ip = this.SplitCsv(GetHeaderValue("X-Forwarded-For")).FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(ip)
                && MyHttpContext.Current?.Connection?.RemoteIpAddress != null)
            {
                ip = MyHttpContext.Current.Connection.RemoteIpAddress.ToString();

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = GetHeaderValue("REMOTE_ADDR");

                    if (string.IsNullOrWhiteSpace(ip))
                    {
                        throw new Exception("Unable to determine caller's IP.");
                    }
                }
            }

            if (ip == "::1"/*localhost*/)
            {
                return "localhost";
            }
            else
            {
                return ip;
            }
        }

        /// <summary>GetHeaderValue</summary>
        /// <param name="headerName">string</param>
        /// <returns>headerValue</returns>
        private string GetHeaderValue(string headerName)
        {
            StringValues values;

            if (MyHttpContext.Current?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                // writes out as Csv when there are multiple.
                return values.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>SplitCsv</summary>
        /// <param name="csvList">string</param>
        /// <returns>List(string)</returns>
        private List<string> SplitCsv(string csvList)
        {
            if (string.IsNullOrWhiteSpace(csvList))
            {
                return new List<string>();
            }
            else
            {
                return csvList
                    .TrimEnd(',')
                    .Split(',')
                    .AsEnumerable<string>()
                    .Select(s => s.Trim())
                    .ToList();
            }
        }
    }
}
