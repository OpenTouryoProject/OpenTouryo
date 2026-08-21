//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて使用して下さい。

//**********************************************************************************
//* クラス名        ：Startup
//* クラス日本語名  ：Startup
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2026/08/21  玄人 幸道         OpenAPI（IDL）のドキュメント生成に対応（#580）
//*  2026/08/21  玄人 幸道         MVC_Sample と同じ構成に揃えた（#582）。
//*                                環境を移しても動く形（#541）と、転送ヘッダ（#549）。
//*                                **Cookie を使わないため、Cookie 由来の設定は
//*                                コメントアウトで残す**（必要になったときの手本）。
//**********************************************************************************

// ＜MVC_Sample との関係＞（#582）
//
//   **この Startup.cs は Backend/MVC_Sample/MVC_Sample/Startup.cs と
//   diff が取れる形にしてある。** 骨格を揃え、
//   Resource Server で使わないものはコメントアウトで残している。
//
//   Resource Server は **OAuth2 の Bearer ヘッダ認証**であり、
//   Cookie 認証・セッション・静的ファイル・Razor を使わない。
//   そのため次は無効にしてある。**消していないのは、
//   利用者がこのテンプレートから Cookie 認証へ広げるときの手本になるためである。**
//
//     UseCookiePolicy / CookiePolicyOptions / UseSession / AddSession
//     AddDistributedMemoryCache / AddAuthentication + AddCookie
//     AddDataProtection（CookieSecurePolicy / DataProtectionKeyPath）
//     UseStaticFiles / MapRazorPages
//
//   逆に、こちらにしか無いのは CORS と OpenAPI である。

// ＜設定で切り替えるもの＞（#541）
//
//   本番でだけ有効にしたい設定を「コメントアウトして置いておく」と、
//   環境を移すときにソースを書き換えることになる。
//   appsettings.json（および環境変数）で切り替えられるようにしてある。
//
//   | キー（appSettings）              | 既定   | on にすると |
//   |----------------------------------|--------|-------------|
//   | UseHttpsRedirection              | off    | HTTP を HTTPS へリダイレクトする |
//   | CookieSecurePolicy               | (空)   | always で Cookie に Secure 属性を必ず付ける |
//   | DataProtectionKeyPath            | (空)   | データ保護の鍵を、指定フォルダに永続化する |
//   | UseForwardedHeaders              | off    | X-Forwarded-Proto / -For を取り込む（#549） |
//   | ForwardedHeadersKnownProxies     | (空)   | 信用する前段のアドレス（空＝範囲を制限しない） |
//
//   **既定値は、いずれも従来どおりの動作**である。
//
//   **CookieSecurePolicy と DataProtectionKeyPath は、この Resource Server では効かない。**
//   Cookie を使わないため。表に残してあるのは MVC_Sample と揃えるためで、
//   Cookie 認証を足すときに合わせて有効化する（#582）。
//
// ＜環境変数で上書きできる＞
//
//   Host.CreateDefaultBuilder が環境変数を構成に含めるため、
//   appsettings.json の値は環境変数で上書きできる。区切りは「__」（下線 2 つ）。
//
//     appSettings__UseHttpsRedirection=on
//     appSettings__FxXMLSPDefinition=/app/files/resource/Xml/SPDefinition.xml
//     connectionStrings__ConnectionString_SQL=...
//
//   **FxContainerization は要らない。** あちらは「接頭辞なしのキー名」で
//   環境変数を読む別の仕組みで、ON にしたときだけ効く。

using System;
using System.IO;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

//using Microsoft.AspNetCore.Mvc.Cors.Internal;

using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Public.Util;


namespace ASPNETWebService
{
    /// <summary>
    /// Startup
    /// ミドルウェア /サービス / フレームワークを
    /// Startupクラスのメソッドで注入することにより、活用できるようになる。
    /// </summary>
    public class Startup
    {
        #region mem & prop & constructor

        /// <summary>Configuration</summary>
        public IConfiguration Configuration { get; }

        /// <summary>constructor</summary>
        /// <param name="configuration">IConfiguration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // ライブラリにも設定
            GetConfigParameter.InitConfiguration(configuration);
            // Dockerで埋め込まれたリソースを使用する場合、
            // 以下のコメントアウトを解除し、appsettings.jsonのappSettings sectionに、
            // "Azure": "既定の名前空間" を指定し、設定ファイルを埋め込まれたリソースに変更する。
            //Touryo.Infrastructure.Business.Dao.MyBaseDao.UseEmbeddedResource = true;
        }

        #endregion

        #region Configure & ConfigureServices

        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 転送ヘッダの取り込み（#549）
            //
            //   **リバース プロキシで TLS を終端すると、アプリから見た接続は HTTP になる。**
            //   利用者のブラウザは HTTPS で繋いでいるのに Request.IsHttps は false のままで、
            //   Secure 属性が付かない（#536 でフレームワークが立てるようにした分が効かない）。
            //
            //   前段が付ける X-Forwarded-Proto を取り込むと、IsHttps が正しくなる。
            //
            //   **必ずパイプラインの先頭に置く。**
            //   後ろに置くと、それより前のミドルウェア（UseHttpsRedirection など）が
            //   取り込み前のスキームを見てしまう。
            //
            //   既定は off。素の HTTP で動かす開発環境では、転送ヘッダを
            //   誰でも付けられる（＝クライアントが詐称できる）ため。
            //
            //   **Cookie を使わない Resource Server でも要る。**（#582）
            //   HTTPS へのリダイレクト判定と、ログに残る接続元アドレスが変わる。
            if (Startup.IsOn("UseForwardedHeaders"))
            {
                ForwardedHeadersOptions options = new ForwardedHeadersOptions()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
                };

                // 信用する前段を指定する。
                //
                //   **既定ではループバックからの転送しか信用しない。**
                //   コンテナや Kubernetes では前段が別アドレスになるため、
                //   **指定しないとヘッダが黙って捨てられ、何も起きない。**
                //   「on にしたのに直らない」の原因はほぼこれである。
                //
                //   ForwardedHeadersKnownProxies に、前段のアドレスをカンマ区切りで書く。
                string knownProxies = Startup.GetValue("ForwardedHeadersKnownProxies");

                if (string.IsNullOrEmpty(knownProxies))
                {
                    // **前段を特定できない場合（コンテナ等）は、範囲の制限を外す。**
                    //   KnownIPNetworks / KnownProxies を空にすると、
                    //   「どこからの転送でも信用する」という意味になる。
                    //   （KnownNetworks は .NET 10 で非推奨。KnownIPNetworks を使う）
                    //
                    //   **アプリが前段を経由せず直接叩ける状態では使わないこと。**
                    //   クライアントが X-Forwarded-Proto を詐称でき、
                    //   HTTP で来ているのに HTTPS だと判断させられる。
                    //   前段だけが到達できるネットワークに閉じてから使う。
                    options.KnownIPNetworks.Clear();
                    options.KnownProxies.Clear();
                }
                else
                {
                    options.KnownIPNetworks.Clear();
                    options.KnownProxies.Clear();

                    foreach (string ip in knownProxies.Split(','))
                    {
                        string trimmed = ip.Trim();

                        if (!string.IsNullOrEmpty(trimmed))
                        {
                            options.KnownProxies.Add(IPAddress.Parse(trimmed));
                        }
                    }
                }

                app.UseForwardedHeaders(options);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // **画面を持たないため、エラー画面へは飛ばさない。**（#582）
                //   MVC_Sample は app.UseExceptionHandler("/Home/Error") を使う。
                //   WebAPI では、例外はフレームワーク（MyBaseAsyncApiController）が
                //   捕捉して JSON で返す。
                //app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // HTTPS へのリダイレクト（#541）
            //
            //   **既定は off。** 平文 HTTP で動かす環境（疎通確認や、
            //   TLS を前段のリバース プロキシで終端する構成）でリダイレクトすると、
            //   到達できなくなるため。
            //   TLS を自分で終端するなら on にする。
            //
            //   **on にするだけでは足りない。リダイレクト先のポートも要る。**
            //   決められないと、ミドルウェアは警告を出すだけで素通りする
            //   （見落としやすい。有効にしたのに HTTP のまま通ってしまう）。
            //
            //     warn: ...HttpsRedirectionMiddleware[3]
            //           Failed to determine the https port for redirect.
            //
            //   ポートは、次のいずれかで決まる。
            //     ・https の URL を Kestrel にバインドする（--urls に https://... を含める）
            //     ・環境変数 ASPNETCORE_HTTPS_PORT=443（単数）
            //     ・環境変数 HTTPS_PORT=443
            //
            //   **ASPNETCORE_HTTPS_PORTS（複数）では決まらない。**
            //   あちらは Kestrel が「どのポートで待ち受けるか」を決めるもので、
            //   このミドルウェアが読むのは単数形の方である。紛らわしいので注意。
            if (Startup.IsOn("UseHttpsRedirection"))
            {
                app.UseHttpsRedirection();
            }

            // HttpContextのマイグレーション用
            app._UseHttpContextAccessor();

            // **静的ファイルは持たない。**（#582）
            //   WebAPI なので /wwwroot が無い。
            //app.UseStaticFiles();

            // **Cookie を使わない。**（#582）
            //   Cookie ポリシーは ConfigureServices 側ごとコメントアウトしてある。
            //app.UseCookiePolicy();

            // **セッションを使わない。**（#582）
            //   Bearer トークンで都度認証するため、サーバ側に状態を持たない。
            //app.UseSession(new SessionOptions()
            //{
            //    IdleTimeout = TimeSpan.FromMinutes(30), // ここで調整
            //    IOTimeout = TimeSpan.FromSeconds(30),
            //    Cookie = new CookieBuilder()
            //    {
            //        HttpOnly = true,
            //        Name = "ws_session",
            //        Path = "/",
            //        SameSite = SameSiteMode.Strict,
            //        SecurePolicy = CookieSecurePolicy.SameAsRequest
            //    }
            //});

            // Routing
            app.UseRouting();

            // Identity
            // Identityではなく、CookieAuthentication
            //
            // **Resource Server は Bearer ヘッダ認証である。**（#582）
            //   認証は MyBaseAsyncApiController の属性
            //   （EnumHttpAuthHeader.Bearer）が行うため、
            //   ここでミドルウェアを挟まない。
            //app.UseAuthentication();
            //app.UseAuthorization();

            // CORS（**こちらにしか無い**）
            //
            //   ブラウザから直接叩ける Resource Server として公開するため。
            //   **AllowCredentials は付けない。**
            //   AllowAnyOrigin とは同時に指定できず、
            //   資格情報を送らない構成として一貫させている。
            app.UseCors( //認証・認可の後ろ
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            //.AllowCredentials());

            // Routingの設定
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // OpenAPI（IDL）を返す（#580）
                //   **開発時だけに絞っていない。**
                //   Resource Server のテンプレートとして、
                //   利用側が IDL を引けることに意味があるため。
                endpoints.MapOpenApi();

                //endpoints.MapRazorPages();
            });
        }

        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // 構成情報から、AppConfiguration SectionをAppConfiguration Classへバインドするようなケース。
            //services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            // HttpContextのマイグレーション用
            services._AddHttpContextAccessor();

            // Cookie ポリシー（#541）
            //
            //   **Cookie を使わないため無効にしてある。**（#582）
            //   Cookie 認証を足すときは、ここと Configure 側の
            //   app.UseCookiePolicy() を合わせて有効にする。
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.HttpOnly = HttpOnlyPolicy.Always;
            //    options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //
            //    // Cookie の Secure 属性（既定は空＝各 Cookie の設定に従う）
            //    //
            //    //   TLS で公開するなら always にする。
            //    //   平文 HTTP の環境で always にすると、**Cookie が送られず
            //    //   ログインできなくなる**ので、既定では変えない。
            //    if (Startup.GetValue("CookieSecurePolicy").ToLower() == "always")
            //    {
            //        options.Secure = CookieSecurePolicy.Always;
            //    }
            //});

            // Sessionのモード
            //
            // **セッションを使わないため無効にしてある。**（#582）
            //   Bearer トークンで都度認証する。
            //   セッションを足すなら、AddDistributedMemoryCache が
            //   プロセス内に持つ点（#541）に注意すること。
            //services.AddDistributedMemoryCache(); // 開発用
            //services.AddDistributedSqlServerCache();   // Microsoft.Extensions.Caching.SqlServer
            //services.AddStackExchangeRedisCache();     // Microsoft.Extensions.Caching.StackExchangeRedis

            // Sessionを使用する。
            //services.AddSession();

            // Core 3.0のテンプレートではUseMvcの
            // 代わりにこれらを使用するようになった。
            services
                .AddControllers()// WebAPI
                .AddNewtonsoftJson();// JSON シリアライザの変更

            //services.AddControllersWithViews(); // MVC & WebAPI
            // services.AddRazorPages(); // Razor Page

            // OpenAPI（IDL）のドキュメント生成（#580）
            //
            // **.NET 9 以降は標準で入っている。** Swashbuckle は要らない。
            // 既定では /openapi/v1.json で返る。
            services.AddOpenApi();

            #region Add Frameworks

            // 一般的な Webアプリでは、
            // EF, Identity, MVC などのミドルウェア サービスを登録する。
            // ミドルウェアの実行順序は、IStartupFilter の登録順に設定される。

            // EF
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            // Add application services.
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();

            // **AddMvc は要らない。**（#582）
            //   ビューを持たないため、上の AddControllers で足りる。
            //services.AddMvc();

            // Forms認証
            //
            // **Resource Server は Bearer ヘッダ認証である。**（#582）
            //   Cookie 認証を足すときに有効にする。
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = new PathString("/Home/Login");
            //    options.AccessDeniedPath = new PathString(GetConfigParameter.GetConfigValue("FxErrorScreenPath"));
            //    options.ReturnUrlParameter = "ReturnUrl";
            //    options.ExpireTimeSpan = TimeSpan.FromHours(1);
            //    options.SlidingExpiration = true;
            //    options.Cookie.HttpOnly = true;
            //});

            // データ保護の鍵の置き場所（#541）
            //
            //   認証 Cookie・セッション・Antiforgery トークンは、この鍵で保護されている。
            //   **鍵が変わると、既存の Cookie が読めなくなり、ログインし直しになる。**
            //
            //   **この Resource Server では、いずれも使っていない。**（#582）
            //   Cookie 認証やセッションを足すときに、合わせて有効にする。
            //   コンテナでは**必ず要る**（作り直すたびに鍵が変わるため）。
            //string keyPath = Startup.GetValue("DataProtectionKeyPath");
            //
            //if (!string.IsNullOrEmpty(keyPath))
            //{
            //    services.AddDataProtection()
            //        .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
            //        // **アプリケーション名も固定する。**
            //        //   既定ではコンテンツ ルートのパスから決まるため、
            //        //   鍵を共有していても**配置先のパスが違うと復号できない**。
            //        .SetApplicationName("ASPNETWebService");
            //}

            #endregion
        }

        #endregion

        #region 設定の読み取り

        /// <summary>appSettings の値を取得する（無ければ空文字）</summary>
        /// <param name="key">キー</param>
        /// <returns>値</returns>
        /// <remarks>
        /// 環境変数 appSettings__＜キー＞ でも上書きできる（#541）。
        /// Host.CreateDefaultBuilder が環境変数を構成に含めるため。
        /// </remarks>
        private static string GetValue(string key)
        {
            return GetConfigParameter.GetConfigValue(key) ?? "";
        }

        /// <summary>appSettings の値が on かを判定する</summary>
        /// <param name="key">キー</param>
        /// <returns>on なら true</returns>
        private static bool IsOn(string key)
        {
            return Startup.GetValue(key).ToLower() == "on";
        }

        #endregion
    }
}
