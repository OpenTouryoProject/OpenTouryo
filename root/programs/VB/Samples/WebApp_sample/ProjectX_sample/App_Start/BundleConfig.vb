'**********************************************************************************
'* テンプレート
'**********************************************************************************

' サンプル中のテンプレートなので、必要に応じて流用して下さい。

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
        BundleTable.EnableOptimizations = True
        BundleTable.Bundles.UseCdn = True
        ' same as: bundles.UseCdn = true;
        ' ( new ScriptBundle("~/XXXX") のパスは実在するpathと被るとRender時にバグる。
        ' なので、bundlesと実在しないpathを指定している（CSSも同じbundlesを使用する）。

        bundles.Add(New ScriptBundle("~/bundles/app").Include("~/Scripts/app/Site.js"))

        bundles.Add(New ScriptBundle("~/bundles/touryo").Include(
                        "~/Scripts/touryo/common.js",
                        "~/Scripts/touryo/ie_key_event.js",
                        "~/Scripts/touryo/else.js"))

        ' こちらのCDNフォールバック設定はScriptManager内で行われているため不要
        bundles.Add(New ScriptBundle("~/bundles/WebFormsJs").Include(
                    "~/Scripts/WebForms/WebForms.js",
                    "~/Scripts/WebForms/WebUIValidation.js",
                    "~/Scripts/WebForms/MenuStandards.js",
                    "~/Scripts/WebForms/Focus.js",
                    "~/Scripts/WebForms/GridView.js",
                    "~/Scripts/WebForms/DetailsView.js",
                    "~/Scripts/WebForms/TreeView.js",
                    "~/Scripts/WebForms/WebParts.js"))

        ' こちらのCDNフォールバック設定はScriptManager内で行われているため不要
        ' これらのファイルには明示的な依存関係があり、ファイルが動作するためには順序が重要です
        bundles.Add(New ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"))

        ' 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備ができたら、
        ' http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
        ' min 無し
        bundles.Add(New ScriptBundle(
                    "~/bundles/modernizr",
                    "//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.8.3.js") With
                    {
                        .CdnFallbackExpression = "window.Modernizr"
                    }.Include("~/Scripts/modernizr-*"))

        ScriptManager.ScriptResourceMapping.AddDefinition(
            "respond", New ScriptResourceDefinition() With
            {
                .Path = "~/Scripts/respond.min.js",
                .DebugPath = "~/Scripts/respond.js",
                .CdnPath = "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.min.js",
                .CdnDebugPath = "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.js",
                .CdnSupportsSecureConnection = False,
                .LoadSuccessExpression = "window.respond"
            })
    End Sub
End Class