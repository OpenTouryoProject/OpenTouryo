using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace SPA_Sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // IQueryable または IQueryable<T> 戻り値の型を持つアクションのクエリのサポートを有効にするには、次のコード行のコメントを解除してください。
            // 予期しないクエリまたは悪意のあるクエリの処理を避けるには、QueryableAttribute の検証設定を使用して受信するクエリを検証してください。
            // 詳細については、http://go.microsoft.com/fwlink/?LinkId=279712 を参照してください。
            //config.EnableQuerySupport();

            // アプリケーションでのトレースを無効にするには、以下のコード行をコメント アウトするか、削除してください
            // 詳細については、http://www.asp.net/web-api を参照してください
            config.EnableSystemDiagnosticsTracing();

            // JSON データにはキャメル ケースを使用します。
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
