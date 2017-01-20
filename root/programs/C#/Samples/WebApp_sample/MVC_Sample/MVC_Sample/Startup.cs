using System;

using Owin;
using Microsoft.Owin;

using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Web.Http;

[assembly: OwinStartup(typeof(MVC_Sample.Startup))]

namespace MVC_Sample
{
    public class Startup
    {
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

            // URLルーティングの登録
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // バンドル＆ミニフィケーションの登録
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
