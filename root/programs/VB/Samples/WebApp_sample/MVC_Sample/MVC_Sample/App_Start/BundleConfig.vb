'**********************************************************************************
'* テンプレート
'**********************************************************************************

' サンプル中のテンプレートなので、必要に応じて使用して下さい。

'**********************************************************************************
'* クラス名        ：BundleConfig
'* クラス日本語名  ：バンドル＆ミニフィケーションに関する指定
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.Web.Optimization

''' <summary>
''' バンドル＆ミニフィケーションに関する指定
''' </summary>
Public Class BundleConfig
    ''' <summary>
    ''' 特集：ASP.NET 4.5新機能概説（1）：
    ''' Visual Studio 2012の新機能とASP.NET 4.5のコア機能 (3-4) - ＠IT
    ''' http://www.atmarkit.co.jp/ait/articles/1303/08/news072_3.html
    ''' ASP.NET 4.5では、リクエスト時のファイル読み込み時間を
    ''' 削減するためにバンドル＆ミニフィケーションの仕組みが導入された。
    ''' Bundling の詳細については、http://go.microsoft.com/fwlink/?LinkId=254725 を参照してください
    ''' </summary>
    Public Shared Sub RegisterBundles(bundles As BundleCollection)
        ' see : https://www.asp.net/ajax/cdn

        Dim jqueryVersion As String = "3.1.1"

        BundleTable.EnableOptimizations = True
        BundleTable.Bundles.UseCdn = True
        ' same as: bundles.UseCdn = true;
        ' ( new ScriptBundle("~/XXXX") のパスは実在するpathと被るとRender時にバグる。
        ' なので、bundlesと実在しないpathを指定している（CSSも同じbundlesを使用する）。

        bundles.Add(New ScriptBundle("~/bundles/app").Include(
                    "~/Scripts/app/Site.js"))

        bundles.Add(New ScriptBundle("~/bundles/touryo").Include(
                    "~/Scripts/touryo/common.js",
                    "~/Scripts/touryo/else.js"))

        bundles.Add(New ScriptBundle(
                    "~/bundles/jquery",
                    String.Format("//ajax.aspnetcdn.com/ajax/jquery/jquery-{0}.min.js", jqueryVersion)) With
                    {
                        .CdnFallbackExpression = "window.jQuery"
                    }.Include(String.Format("~/Scripts/jquery-{0}.js", jqueryVersion)))

        bundles.Add(New ScriptBundle(
                    "~/bundles/jqueryval",
                    "//ajax.aspnetcdn.com/ajax/jquery.validate/1.16.0/jquery.validate.min.js") With
                    {
                        .CdnFallbackExpression = "window.jQuery.validator"
                    }.Include("~/Scripts/jquery.validate.js"))

        bundles.Add(New ScriptBundle(
                    "~/bundles/jqueryvaluno",
                    "//ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.min.js") With
                    {
                        .CdnFallbackExpression = "window.jQuery.validator.unobtrusive"
                    }.Include("~/Scripts/jquery.validate.unobtrusive.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryunoajax").Include(
                    "~/Scripts/jquery.unobtrusive-ajax.js")) ' CDNで提供されていない。

        ' 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備ができたら、
        ' http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
        bundles.Add(New ScriptBundle(
                    "~/bundles/modernizr",
                    "//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.8.3.js") With
                    {
                        .CdnFallbackExpression = "window.Modernizr"
                    }.Include("~/Scripts/modernizr-*"))

        bundles.Add(New ScriptBundle(
                    "~/bundles/bootstrap",
                    "//ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js") With
                    {
                        .CdnFallbackExpression = "window.jQuery.fn.modal"
                    }.Include("~/Scripts/bootstrap.js"))

        bundles.Add(New ScriptBundle(
                    "~/bundles/respond",
                    "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.min.js") With
                    {
                        .CdnFallbackExpression = "window.respond"
                    }.Include("~/Scripts/respond.js"))

        bundles.Add(New StyleBundle("~/bundles/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/font-awesome.min.css",
                    "~/Content/touryo/Style.css",
                    "~/Content/app/Site.css"))
    End Sub
End Class
