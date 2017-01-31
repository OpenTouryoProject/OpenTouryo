//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testDLFrame
//* クラス日本語名  ：PDFダウンロードのテスト・フレーム（Ｐ層）
//*                   window.openから直接DLできない現象ことがあったので回避策。
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using Touryo.Infrastructure.Business.Presentation;

namespace ProjectX_sample.Aspx.TestFxLayerP
{
    /// <summary>PDFダウンロードのテスト・フレーム（Ｐ層）</summary>
    public partial class testDLFrame : MyBaseController
    {
        #region ページロードのUOCメソッド

        /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:
            this.iframe1.Attributes.Add("src", "testDLScreen.aspx");
        }

        /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            // TODO:
        }

        #endregion
    } 
}
