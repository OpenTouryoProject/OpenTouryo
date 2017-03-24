//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて流用して下さい。

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

using Owin;
using Microsoft.Owin;

using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(ProjectX_sample.Startup))]

namespace ProjectX_sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // アプリケーションの設定方法の詳細については、http://go.microsoft.com/fwlink/?LinkID=316888 を参照してください

            // アプリケーションのスタートアップで実行するコードです

            // URLルーティングの登録
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // バンドル＆ミニフィケーションの登録
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
