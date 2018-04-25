//**********************************************************************************
//* フレームワーク・テスト UI（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：SampleChildControl
//* クラス日本語名  ：UserControl上のEvent Handlerをハンドルする（ネスト）。
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Web.UI;
using Touryo.Infrastructure.Framework.Presentation;

namespace WebForms_Sample.Aspx.Common.Wuc
{
    /// <summary>SampleChildControl</summary>
    public partial class SampleChildControl : UserControl
    {
        /// <summary>User ControlにEvent Handlerを実装可能にしたのでそのテスト（ネスト）。</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUCChildButton_Click(FxEventArgs fxEventArgs)
        {
            this.lblUCChildResult.Text = "UOC_btnUCChildButton_Clickを実行できた。";
            return "";
        }
    } 
}
