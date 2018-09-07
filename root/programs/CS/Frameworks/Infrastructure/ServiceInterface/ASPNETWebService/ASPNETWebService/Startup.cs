//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて使用して下さい。

//**********************************************************************************
//* クラス名        ：OwinStartup
//* クラス日本語名  ：OwinStartup
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;

using Owin;
using Microsoft.Owin;

using Touryo.Infrastructure.Framework.Authentication;

[assembly: OwinStartup(typeof(ASPNETWebService.Startup))]

namespace ASPNETWebService
{
    public class Startup
    {
        /// <summary>Configuration</summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            // アプリケーションのスタートアップで実行するコードです

            //
            AreaRegistration.RegisterAllAreas();

            // 
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // グローバルフィルタの登録
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //// URLルーティングの登録
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            //// バンドル＆ミニフィケーションの登録
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //// 認証に関するOWINミドルウェアの設定を行う。
            //StartupAuth.Configure(app);
            OAuth2AndOIDCClient.HttpClient = new HttpClient(); // JwkSet取得用

            GlobalConfiguration.Configuration.Initializer(GlobalConfiguration.Configuration);
        }
    }
}
