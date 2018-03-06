//**********************************************************************************
//* フレームワーク・テスト UI（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：SampleControl
//* クラス日本語名  ：UserControl上のEvent Handlerをハンドルする。
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
    /// <summary>SampleControl</summary>
    public partial class SampleControl : UserControl
    {
        /// <summary>User ControlにEvent Handlerを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUCButton_Click(FxEventArgs fxEventArgs)
        {
            this.lblUCResult.Text = "UOC_btnUCButton_Clickを実行できた。";
            return "";
        }
    } 
}
