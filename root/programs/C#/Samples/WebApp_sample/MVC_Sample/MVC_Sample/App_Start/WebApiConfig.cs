﻿//**********************************************************************************
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MVC_Sample
{
    /// <summary>
    /// ルート定義に関する指定（WebApi用）
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Routing in ASP.NET Web API  The Official Microsoft ASP.NET Site
        /// http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
        ///  Web API RoutingはMVC Routingに非常に似ています。
        ///  主な違いは Web API URI パスではなく HTTP メソッドを使用してアクションを選択することです。
        ///  さらに、Web APIの中でMVC Routingを使用しても良い。
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //// トレース機能を有効化します。
            //TraceConfig.Register(config);
        }
    }
}
