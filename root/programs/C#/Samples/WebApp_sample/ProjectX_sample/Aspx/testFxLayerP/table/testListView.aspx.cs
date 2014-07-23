//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_table_testListView
//* クラス日本語名  ：ListViewテスト画面（Ｐ層）
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
using Touryo.Infrastructure.CustomControl;

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

/// <summary>GridViewテスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_table_testListView : MyBaseController
{
    #region 初期化

    /// <summary>ヘッダーに表示する文字列</summary>
    public Dictionary<string, string> HeaderInfo = new Dictionary<string, string>();

    /// <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    protected void Page_Init(object sender, EventArgs e)
    {
    }

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
        // 初回ロード時に、データソースを
        // 生成 ＆ データバインドする。
        this.CreateDataSource();
        this.BindGridData();
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:
    }

    /// <summary>
    /// ★http://bbs.wankuma.com/index.cgi?mode=al2&namber=41245&KLOG=71
    /// </summary>
    protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
    {
        this.CreateDataSource();
        this.BindGridData();
    }

    #endregion

    #region データソースの生成

    /// <summary>DataSourceを生成</summary>
    /// <returns>Datatableを返す</returns>
    private void CreateDataSource()
    {
        // Server.MapPathはアプリケーション ディレクトリを指す。
        DirectoryInfo di = new DirectoryInfo(Server.MapPath("/ProjectX_sample/Aspx/Common"));
        FileInfo[] fi = di.GetFiles();

        // Datatableに
        // アプリケーション ディレクトリの
        // ファイル情報を設定する。
        DataTable dt = new DataTable();
        DataRow dr;

        // 列生成
        dt.Columns.Add(new DataColumn("fileid", typeof(int)));
        dt.Columns.Add(new DataColumn("filename", typeof(String)));
        dt.Columns.Add(new DataColumn("readonly", typeof(Boolean)));
        dt.Columns.Add(new DataColumn("filesize", typeof(long)));
        dt.Columns.Add(new DataColumn("date", typeof(DateTime)));

        // 行生成
        for (int i = 0; i < fi.Length; i++)
        {
            dr = dt.NewRow();
            dr["fileid"] = i;
            dr["filename"] = fi[i].Name;
            dr["readonly"] = fi[i].IsReadOnly;
            dr["filesize"] = fi[i].Length;
            dr["date"] = fi[i].LastWriteTime;
            dt.Rows.Add(dr);
        }

        // 変更のコミット
        dt.AcceptChanges();

        // DataTableをSessionに格納する
        Session["SampleData"] = dt;
    }

    /// <summary>データバインドする</summary>
    private void BindGridData()
    {
        this.ListView1.DataSource = Session["SampleData"];
        this.ListView1.DataBind();
    }

    #endregion

}
