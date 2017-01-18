//**********************************************************************************
//* BundleConfig
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：BundleConfig
//* クラス日本語名  ：BundleConfig
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System
using System.Web.UI;
using System.Web.Optimization;

namespace ProjectX_sample
{
    public class BundleConfig
    {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkID=303951 を参照してください。
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.UseCdn = true;    // same as: bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                                    "~/Scripts/app/Site.js"));

            bundles.Add(new ScriptBundle("~/bundles/otr").Include(
                                    "~/Scripts/otr/common.js",
                                    "~/Scripts/otr/ie_key_event.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // これらのファイルには明示的な依存関係があり、ファイルが動作するためには順序が重要です
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // 開発に使用し、情報源である開発バージョンの Modernizr を使用します。続いて、
            // 運用の準備が完了したら、http://modernizr.com のビルド ツールを使用し、必要なテストのみを選択します
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });
        }
    }
}