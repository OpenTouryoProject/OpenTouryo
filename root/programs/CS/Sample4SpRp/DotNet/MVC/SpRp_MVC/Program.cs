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
//* クラス日本語名  ：SpRp サンプル（MVC）の起動
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668）
//*  2026/11/10  玄人 幸道         SpRp_ClientCertPfxFilePath が在れば、部品の HttpClient にクライアント証明書を載せる（FAPI2 の MTLS。#668 の追加）
//**********************************************************************************

using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Util;

namespace SpRp_MVC
{
    /// <summary>起動</summary>
    /// <remarks>
    /// **待ち受けは appsettings.json の Kestrel の節**（https://localhost:44336）。
    /// 戻り先（`SpRp_RedirectUri`）は IdP に登録した値と 1 文字も違ってはならないので、
    /// launchSettings.json に頼らず、どの起動のしかたでも同じ URL にする。
    /// </remarks>
    public class Program
    {
        /// <summary>Main</summary>
        /// <param name="args">引数</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // **Open棟梁 の部品は、GetConfigParameter で設定を読む**（appsettings.json の appSettings の節）
            GetConfigParameter.InitConfiguration((IConfiguration)builder.Configuration);

            // **HttpClient は 1 つを使い回す**（OAuth2AndOIDCClient の静的プロパティ。設定しないと、どの要求も送られない）
            OAuth2AndOIDCClient.HttpClient = Program.CreateHttpClient();

            builder.Services.AddControllersWithViews();

            // **state・nonce・code_verifier は、認可要求から戻りまでの間だけ、セッションに置く**
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // **IdP から戻るのはトップ レベルの GET**（query・query.jwt）なので、Lax で Cookie が付く
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            WebApplication app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
        }

        /// <summary>部品（OAuth2AndOIDCClient）が使う HttpClient を作る</summary>
        /// <returns>HttpClient</returns>
        /// <remarks>
        /// **SpRp_ClientCertPfxFilePath が在れば、そのクライアント証明書を載せる**（FAPI2 の par_mtls。tls_client_auth・RFC 8705。#668 の追加）。
        /// - **証明書は、サーバが求めたときだけ送られる**（TLS のハンドシェイク）。求めない IdP には、今までどおり証明書なしで届く
        /// - **HttpClient は静的プロパティで、プロセス全体に効く**（OAuth2AndOIDCClient.HttpClient の remarks）。
        ///   そのため、**証明書を求める IdP（MTLS を有効にしたスタブ）では、ほかの flow の要求にも証明書が付く**
        ///   （トークンが証明書に縛られても、UserInfo も同じ HttpClient で呼ぶので、そのまま通る）
        /// </remarks>
        private static HttpClient CreateHttpClient()
        {
            string pfx = CmnClientParams.ClientCertPfxFilePath;
            if (string.IsNullOrEmpty(pfx))
            {
                return new HttpClient();
            }

            HttpClientHandler handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
            };
            handler.ClientCertificates.Add(
                X509CertificateLoader.LoadPkcs12FromFile(pfx, CmnClientParams.ClientCertPfxPassword));

            return new HttpClient(handler);
        }
    }
}
