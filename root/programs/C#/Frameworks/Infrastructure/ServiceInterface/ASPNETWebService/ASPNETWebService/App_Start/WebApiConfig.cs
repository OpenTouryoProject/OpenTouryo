//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて使用して下さい。

//**********************************************************************************
//* クラス名        ：WebApiConfig
//* クラス日本語名  ：ルート定義に関する指定（WebApi用）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.Http;
//using Microsoft.Owin.Security.OAuth;

using Newtonsoft.Json.Serialization;

namespace ASPNETWebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //// Web API configuration and services
            //// 「Bearer Token」認証のみを使用するように、Web API を設定。
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // JSON データにはDefaultを使用 (JSON.NET)
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new DefaultContractResolver();

            // CORS (Cross-Origin Resource Sharing)の有効化
            // 別ドメイン上で動作する Web アプリからアクセス可能に設定。
            config.EnableCors();

            // Web API routes を設定する。

            // Attribute Routing
            config.MapHttpAttributeRoutes();

            // MapHttpRoute
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //// トレース機能を有効化します。
            //TraceConfig.Register(config);
        }
    }
}
