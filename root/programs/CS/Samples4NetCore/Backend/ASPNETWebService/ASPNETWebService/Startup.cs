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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Cors.Internal;

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

                // エラー画面
                //app.UseDeveloperExceptionPage(); // 使用しない。
                //app.UseDatabaseErrorPage(); // 使用しない。
               
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
                // ・・・
            }
            #endregion

            #region パイプラインに追加

            // HttpContextのマイグレーション用
            app.UseHttpContextAccessor();

            // エラー画面
            app.UseExceptionHandler("/Home/Error");

            // MVCをパイプラインに追加（routesも設定）
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // UseCorsでAllowAllOriginsを指定。
            app.UseCors("AllowAllOrigins");

            // /wwwroot（既定の）の
            // 静的ファイルをパイプラインに追加
            // app.UseStaticFiles();

            // Identity
            //app.UseAuthentication();

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

            // MVC(WebAPIも含む)をサービス コンテナに追加
            // AddWebApiConventionsは、Web API 2 Controllerの移植を容易にする。
            services.AddMvc().AddWebApiConventions();

            //services.AddMvc(options =>
            //{
            //    // FilterをグローバルにすべてのControllerで有効にする場合
            //    options.Filters.AddService(typeof(MyActionFilter));
            //    // CROSをグローバルにすべてのControllerで有効にする場合
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            //});

            // CORSをサービス コンテナに追加
            // https://docs.microsoft.com/ja-jp/aspnet/core/security/cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                    });

                //options.AddPolicy("AllowSpecificOrigins",
                //builder =>
                //{
                //    // URL は末尾にスラッシュを付けずに指定
                //    builder.WithOrigins(
                //        "http://example.com",
                //        "http://www.contoso.com");
                //});
            });

            #endregion
        }

        #endregion
    }
}
