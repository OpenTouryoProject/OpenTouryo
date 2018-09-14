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
//**********************************************************************************

using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

using Microsoft.AspNetCore.Mvc.Cors.Internal;

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

        /// <summary>HostingEnvironment </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>Configuration</summary>
        public IConfiguration Configuration { get; }

        /// <summary>constructor</summary>
        /// <param name="env">IConfiguration</param>
        /// <param name="config">IConfiguration</param>
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            // 自前
            //IConfigurationBuilder builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();
            //config = builder.Build();

            // メンバに設定
            this.HostingEnvironment = env;
            this.Configuration = config;

            // ライブラリにも設定
            GetConfigParameter.InitConfiguration(config);
            // Dockerで埋め込まれたリソースを使用する場合、
            // 以下のコメントアウトを解除し、appsettings.jsonのappSettings sectionに、
            // "Azure": "既定の名前空間" を指定し、設定ファイルを埋め込まれたリソースに変更する。
            //Touryo.Infrastructure.Business.Dao.MyBaseDao.UseEmbeddedResource = true;
    }

        #endregion

        #region Configure & ConfigureServices

        /// <summary>
        /// Configure
        /// ・必須
        /// ・ConfigureServices メソッドの後に、WebHostに呼び出される。
        /// ・アプリケーションの要求処理パイプラインを構成する。
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="loggerFactory">ILoggerFactory</param>
        /// <remarks>
        /// this.HostingEnvironmentやthis.Configurationを見て、パイプライン構成を切り替える。
        /// </remarks>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Development、Staging、Productionの
            // 環境変数（ASPNETCORE_ENVIRONMENT）値を使用可能。
            //bool flg = this.HostingEnvironment.IsDevelopment();
            //flg = this.HostingEnvironment.IsStaging();
            //flg = this.HostingEnvironment.IsProduction();

            #region Development or それ以外のモード

            if (this.HostingEnvironment.IsDevelopment())
            {
                // Developmentモードの場合

                // 開発用エラー画面
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                
                // 簡易ログ出力
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                // ブラウザー リンク
                // 開発環境と 1-n ブラウザの間の通信チャネルを作成
                // https://blogs.msdn.microsoft.com/chack/2013/12/16/visual-studio-2013-1/
                app.UseBrowserLink();
            }
            else
            {
                // Developmentモードでない場合

                // カスタム例外処理ページ
                // MyMVCCoreFilterAttribute.OnExceptionで処理。
            }

            #endregion

            #region パイプラインに追加

            // Cookieを使用する。
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                //Secure= CookieSecurePolicy.Always
            });

            // Sessionを使用する。
            app.UseSession(new SessionOptions()
            {
                IdleTimeout = TimeSpan.FromMinutes(30), // ここで調整
                IOTimeout = TimeSpan.FromSeconds(30),
                Cookie = new CookieBuilder()
                {
                    Expiration = TimeSpan.FromDays(1), // 効かない
                    HttpOnly = true,
                    Name = "mvc_session",
                    Path = "/",
                    SameSite= SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                }
            });

            // HttpContextのマイグレーション用
            app.UseHttpContextAccessor();

            // MVCをパイプラインに追加（routesも設定）
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // UseCorsでAllowAllOriginsを指定。
            //app.UseCors("AllowAllOrigins");

            // /wwwroot（既定の）の
            // 静的ファイルをパイプラインに追加
            app.UseStaticFiles();

            // Identity
            //app.UseAuthentication();

            // Identityではなく、CookieAuthentication
            app.UseAuthentication();

            #endregion
        }

        /// <summary>
        /// ConfigureServices
        /// 必要に応じて、ミドルウェア /サービス / フレームワークを注入する。
        /// ・実行は任意
        /// ・Configure メソッドの前に、WebHostにより呼び出される。
        /// ・規約によって構成オプションを設定する。
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <remarks>
        /// IServiceCollectionコンテナにサービスを追加すると、
        /// Configure メソッドと、アプリケーション内でサービスを利用できるようになる。
        /// サービスは、DI or IApplicationBuilder.ApplicationServices から解決される。
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // 構成情報から、AppConfiguration SectionをAppConfiguration Classへバインドするようなケース。
            //services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            #region Development or それ以外のモード

            if (this.HostingEnvironment.IsDevelopment())
            {
                // Developmentモードの場合

                // Sessionのモード
                services.AddDistributedMemoryCache(); // 開発用
            }
            else
            {
                // Developmentモードでない場合

                // Sessionのモード
                //services.AddDistributedSqlServerCache();
                //services.AddDistributedRedisCache();
            }

            #endregion

            // Sessionを使用する。
            services.AddSession();

            // HttpContextのマイグレーション用
            services.AddHttpContextAccessor();

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
                //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"C:\artifacts"));
            });

            #endregion
        }

        #endregion
    }
}
