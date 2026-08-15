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
//*  2026/08/15  玄人 幸道         環境を移しても動くように整理（#541）。
//*                                本番向けの設定を、コメントアウトではなく
//*                                appsettings.json の値で切り替える形にした。
//**********************************************************************************

// ＜設定で切り替えるもの＞（#541）
//
//   本番でだけ有効にしたい設定を「コメントアウトして置いておく」と、
//   環境を移すときにソースを書き換えることになる。
//   appsettings.json（および環境変数）で切り替えられるようにしてある。
//
//   | キー（appSettings）      | 既定   | on にすると |
//   |--------------------------|--------|-------------|
//   | UseHttpsRedirection      | off    | HTTP を HTTPS へリダイレクトする |
//   | CookieSecurePolicy       | (空)   | always で Cookie に Secure 属性を必ず付ける |
//   | DataProtectionKeyPath    | (空)   | データ保護の鍵を、指定フォルダに永続化する |
//
//   **既定値は、いずれも従来どおりの動作**である。
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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


namespace MVC_Sample
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
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

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

            // /wwwroot（既定の）の
            // 静的ファイルをパイプラインに追加
            app.UseStaticFiles();

            // Cookieを使用する。
            //
            // **引数を渡さない。**（#541）
            //   引数を渡すと、ConfigureServices の services.Configure(CookiePolicyOptions)
            //   が使われなくなる。以前は両方に書いてあり、**DI 側は効いていなかった。**
            //   設定は ConfigureServices 側に一本化してある。
            app.UseCookiePolicy();

            // Sessionを使用する。
            app.UseSession(new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromMinutes(30), // ここで調整
                IOTimeout = TimeSpan.FromSeconds(30),
                Cookie = new CookieBuilder()
                {
                    // **Expiration は書かない。**（#541）
                    //   セッション Cookie は有効期限を持たない（ブラウザを閉じると消える）ため、
                    //   指定しても無視される。持続時間を変えたいなら上の IdleTimeout を使う。
                    HttpOnly = true,
                    Name = "mvc_session",
                    Path = "/",
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                }
            });

            // Routing
            app.UseRouting();

            // Identity
            // Identityではなく、CookieAuthentication
            app.UseAuthentication();
            app.UseAuthorization();

            // Routingの設定
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
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
            //   Configure 側の app.UseCookiePolicy() が、ここの設定を使う。
            //   **以前は Configure 側で引数を渡しており、ここは効いていなかった。**
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;

                // Cookie の Secure 属性（既定は空＝各 Cookie の設定に従う）
                //
                //   TLS で公開するなら always にする。
                //   平文 HTTP の環境で always にすると、**Cookie が送られず
                //   ログインできなくなる**ので、既定では変えない。
                //   （Antiforgery の Cookie も返らなくなるため、
                //   　症状は「ログイン画面は出るが POST が 400」になる）
                if (Startup.GetValue("CookieSecurePolicy").ToLower() == "always")
                {
                    options.Secure = CookieSecurePolicy.Always;
                }

                // ＜同意の確認は既定で無効＞
                //   options.CheckConsentNeeded = context => true; と書いてあったが、
                //   上記のとおり効いていなかった。**有効にすると、同意前は
                //   必須でない Cookie（セッションを含む）が送られなくなる**ため、
                //   挙動が変わる。必要な画面を用意したうえで有効にすること。
            });

            // Sessionのモード
            //
            // **AddDistributedMemoryCache はプロセス内に持つ。**（#541）
            //   名前に反して分散しない。次のいずれかに当てはまるなら、
            //   外部のストアに替えないとセッションが失われる。
            //     ・インスタンスを複数立てる（ロード バランサの配下など）
            //     ・コンテナを作り直す・再起動する
            //   net48 側の WebForms_Sample / MVC_Sample が StateServer を使うのと同じ論点である。
            services.AddDistributedMemoryCache(); // 開発用
            //services.AddDistributedSqlServerCache();   // Microsoft.Extensions.Caching.SqlServer
            //services.AddStackExchangeRedisCache();     // Microsoft.Extensions.Caching.StackExchangeRedis

            // Sessionを使用する。
            services.AddSession();

            // Core 3.0のテンプレートではUseMvcの
            // 代わりにこれらを使用するようになった。
            services
                .AddControllersWithViews();// MVC & WebAPI
                //.AddNewtonsoftJson();// JSON シリアライザの変更

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

            services.AddMvc();

            // Forms認証
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = new PathString("/Home/Login");
                //options.LogoutPath = new PathString("/Home/Logout");
                options.AccessDeniedPath = new PathString(GetConfigParameter.GetConfigValue("FxErrorScreenPath"));
                options.ReturnUrlParameter = "ReturnUrl";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;

                // ここに options.DataProtectionProvider を書くと、
                // **認証 Cookie にしか効かない。**（#541）
                // セッションや Antiforgery も同じ鍵を使うので、
                // 下の AddDataProtection でアプリ全体に設定している。
            });

            // データ保護の鍵の置き場所（#541）
            //
            //   認証 Cookie・セッション・Antiforgery トークンは、この鍵で保護されている。
            //   **鍵が変わると、既存の Cookie が読めなくなり、ログインし直しになる。**
            //   既定の置き場所はプロセスの環境に依存する（下は Windows の例）ため、
            //     ・コンテナを作り直す
            //     ・インスタンスを複数立てる
            //   と鍵が揃わない。永続化・共有する場所を指定できるようにしてある。
            //
            //     info: ...XmlKeyManager[63]
            //           User profile is available. Using
            //           'C:\Users\＜user＞\AppData\Local\ASP.NET\DataProtection-Keys'
            //           as key repository ...
            //
            //   既定は空＝従来どおり（指定しない）。
            string keyPath = Startup.GetValue("DataProtectionKeyPath");

            if (!string.IsNullOrEmpty(keyPath))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                    // **アプリケーション名も固定する。**
                    //   既定ではコンテンツ ルートのパスから決まるため、
                    //   鍵を共有していても**配置先のパスが違うと復号できない**。
                    //   （Windows の C:\... とコンテナの /app など）
                    .SetApplicationName("MVC_Sample");
            }

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
