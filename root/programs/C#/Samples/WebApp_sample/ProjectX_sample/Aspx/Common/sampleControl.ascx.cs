//**********************************************************************************
//* クラス名        ：WebUserControl
//* クラス日本語名  ：WebUserControl上のイベントハンドラをハンドルする。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// OpenTouryo
using Touryo.Infrastructure.Framework.Presentation;

namespace ProjectX_sample.Aspx.Common
{
    /// <summary>WebUserControl class</summary>
    public partial class WebUserControl : System.Web.UI.UserControl
    {
        /// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUCButton_Click(FxEventArgs fxEventArgs)
        {
            Response.Write("UOC_btnUCButton_Clickを実行できた。");

            return "";
        }
    } 
}
