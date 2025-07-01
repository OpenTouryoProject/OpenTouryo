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
    ''' Bundling の詳細については、https://go.microsoft.com/fwlink/?LinkID=303951 を参照してください
    ''' </summary>
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)
        RegisterJQueryScriptManager()

        BundleTable.EnableOptimizations = True
        BundleTable.Bundles.UseCdn = True ' same as: bundles.UseCdn = true;

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

        ' 開発に使用し、情報源である開発バージョンの Modernizr を使用します。続いて、
        ' 運用の準備が完了したら、https://modernizr.com のビルド ツールを使用し、必要なテストのみを選びます
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"))

        'ScriptManager.ScriptResourceMapping.AddDefinition(
        '    "respond", New ScriptResourceDefinition() With
        '    {
        '        .Path = "~/Scripts/respond.min.js",
        '        .DebugPath = "~/Scripts/respond.js",
        '        .CdnPath = "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.min.js",
        '        .CdnDebugPath = "//ajax.aspnetcdn.com/ajax/respond/1.4.2/respond.js",
        '        .CdnSupportsSecureConnection = False,
        '        .LoadSuccessExpression = "window.respond"
        '    })
    End Sub

    Public Shared Sub RegisterJQueryScriptManager()
        Dim jQueryScriptResourceDefinition As New ScriptResourceDefinition
        With jQueryScriptResourceDefinition
            .Path = "~/scripts/jquery-3.7.0.min.js"
            .DebugPath = "~/scripts/jquery-3.7.0.js"
            .CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.min.js"
            .CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.0.js"
        End With

        ScriptManager.ScriptResourceMapping.AddDefinition("Jquery", jQueryScriptResourceDefinition)
    End Sub
End Class