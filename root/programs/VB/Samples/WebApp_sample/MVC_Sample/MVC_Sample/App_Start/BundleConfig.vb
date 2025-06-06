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
Public Module BundleConfig
    ''' <summary>
    ''' 特集：ASP.NET 4.5新機能概説（1）：
    ''' Visual Studio 2012の新機能とASP.NET 4.5のコア機能 (3-4) - ＠IT
    ''' http://www.atmarkit.co.jp/ait/articles/1303/08/news072_3.html
    ''' ASP.NET 4.5では、リクエスト時のファイル読み込み時間を
    ''' 削減するためにバンドル＆ミニフィケーションの仕組みが導入された。
    ''' Bundling の詳細については、https://go.microsoft.com/fwlink/?LinkId=301862 を参照してください
    ''' </summary>
    Public Sub RegisterBundles(ByVal bundles As BundleCollection)
        ' see : https://www.asp.net/ajax/cdn
        BundleTable.EnableOptimizations = True
        BundleTable.Bundles.UseCdn = True ' same as: bundles.UseCdn = true;

        ' ( new ScriptBundle("~/XXXX") のパスは実在するpathと被るとRender時にバグる。
        ' なので、bundlesと実在しないpathを指定している（CSSも同じbundlesを使用する）。
        
        bundles.Add(New ScriptBundle("~/bundles/app").Include(
                    "~/Scripts/app/Site.js"))

        bundles.Add(New ScriptBundle("~/bundles/touryo").Include(
                    "~/Scripts/touryo/common.js",
                    "~/Scripts/touryo/else.js"))
        
        ' jquery、jqueryvalを新規作成テンプレ準拠に
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"))

        ' jqueryvaluno、jqueryunoajax (削除)
        
        'modernizr、bootstrapを新規作成テンプレ準拠に
        ' 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備が
        ' 運用の準備が完了したら、https://modernizr.com のビルド ツールを使用し、必要なテストのみを選択します。
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"))

        bundles.Add(New Bundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/site.css"))
                  
        ' respond (削除)
        
        bundles.Add(New StyleBundle("~/bundles/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/font-awesome.min.css",
                    "~/Content/touryo/Style.css",
                    "~/Content/app/Site.css"))
    End Sub
End Module

