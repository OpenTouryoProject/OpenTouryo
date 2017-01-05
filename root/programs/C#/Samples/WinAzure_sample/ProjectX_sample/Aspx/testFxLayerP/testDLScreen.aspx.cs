﻿//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_testDLScreen
//* クラス日本語名  ：PDFダウンロードのテスト画面（Ｐ層）
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

/// <summary>PDFダウンロードのテスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_testDLScreen : MyBaseController
{
    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:

        Response.Clear();

        Response.ContentType = "application/pdf";

        // HTTPヘッダーの書き方で、
        //こっちは、専用アプリケーションで開く
        //Response.AppendHeader("Content-Disposition", "attachment;filename=test.pdf");

        //こっちは、IEからOLEオブジェクトを開く
        Response.AppendHeader("Content-Disposition", "inline;filename=test.pdf");

        Response.WriteFile(
            Path.Combine(
                GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"));

        Response.End(); 
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
