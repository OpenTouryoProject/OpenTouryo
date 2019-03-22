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
//* クラス名        ：HttpContextExtensions
//* クラス日本語名  ：System.Web.HttpContextのポーティング用クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成
//*  2018/04/10  西野 大介         .NET Standard対応で、HttpContextのポーティング
//*  2019/03/22  西野 大介         2.1で追加されたAddHttpContextAccessorと衝突したのでリネーム。
//**********************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Touryo.Infrastructure.Framework.StdMigration
{
    /// <summary>
    /// System.Web.HttpContextのポーティング用クラス
    /// ASP.NET Core 2.1以降では不要になっている模様。
    /// 
    /// - .NET Standardへの移行 - マイクロソフト系技術情報 Wiki
    ///   https://techinfoofmicrosofttech.osscons.jp/index.php?.NET%20Standard%E3%81%B8%E3%81%AE%E7%A7%BB%E8%A1%8C#yfe521c8
    ///
    /// - Microsoft Docs
    ///   - ASP.NET Core で HttpContext にアクセスする
    ///     https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/http-context
    ///   - HttpServiceCollectionExtensions.AddHttpContextAccessor(IServiceCollection) Method (Microsoft.Extensions.DependencyInjection)
    ///     https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.httpservicecollectionextensions.addhttpcontextaccessor
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>Startup.ConfigureServicesメソッドから呼び出す。</summary>
        /// <param name="services">IServiceCollection</param>
        public static IServiceCollection _AddHttpContextAccessor(this IServiceCollection services)
        {
            return services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>Startup.Configureメソッドから呼び出す。</summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder _UseHttpContextAccessor(this IApplicationBuilder app)
        {
            // MyHttpContextの初期化
            MyHttpContext.Configure(
                app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            return app;
        }
    }
}
