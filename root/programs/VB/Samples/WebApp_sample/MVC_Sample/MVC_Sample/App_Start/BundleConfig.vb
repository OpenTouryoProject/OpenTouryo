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

Imports System.Web
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
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include( _
                    "~/Scripts/jquery-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryui").Include( _
                    "~/Scripts/jquery-ui-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include( _
                    "~/Scripts/jquery.unobtrusive*", _
                    "~/Scripts/jquery.validate*"))

        ' 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備が
        ' できたら、http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include( _
                    "~/Scripts/modernizr-*"))

        bundles.Add(New StyleBundle("~/Content/css").Include("~/Content/site.css"))

        bundles.Add(New StyleBundle("~/Content/themes/base/css").Include( _
                    "~/Content/themes/base/jquery.ui.core.css", _
                    "~/Content/themes/base/jquery.ui.resizable.css", _
                    "~/Content/themes/base/jquery.ui.selectable.css", _
                    "~/Content/themes/base/jquery.ui.accordion.css", _
                    "~/Content/themes/base/jquery.ui.autocomplete.css", _
                    "~/Content/themes/base/jquery.ui.button.css", _
                    "~/Content/themes/base/jquery.ui.dialog.css", _
                    "~/Content/themes/base/jquery.ui.slider.css", _
                    "~/Content/themes/base/jquery.ui.tabs.css", _
                    "~/Content/themes/base/jquery.ui.datepicker.css", _
                    "~/Content/themes/base/jquery.ui.progressbar.css", _
                    "~/Content/themes/base/jquery.ui.theme.css", _
                    "~/Content/themes/base/jquery-ui.css"))
    End Sub
End Class
