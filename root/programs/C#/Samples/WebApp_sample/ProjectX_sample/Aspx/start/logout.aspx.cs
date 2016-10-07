//**********************************************************************************
//* サンプル
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_Start_logout
//* クラス日本語名  ：ログアウト画面（Forms認証対応）
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
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace ProjectX_sample.Aspx.Start
{
    /// <summary>ログアウト画面（Forms認証対応）</summary>
    public partial class logout : MyBaseController
    {
        #region ページロードのUOCメソッド

        /// <summary>
        /// ページロードのUOCメソッド（個別：初回ロード）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
        }

        /// <summary>
        /// ページロードのUOCメソッド（個別：ポストバック）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない
        }

        #endregion

        #region イベントハンドラ

        /// <summary>ログアウト</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // ログオフ（認証チケットを削除する）
            FormsAuthentication.SignOut();

            // ログインページ（login.aspx）に遷移する。
            Response.Redirect("login.aspx");

            // 任意のページに飛ばした場合、認証チケットが無いので、
            // ログインページ（login.aspx）強制的に遷移する。
            // ※ ポストバックのままだと、そのまま画面が表示されてしまう。

            return string.Empty;
        }

        #endregion
    } 
}
