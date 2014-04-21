//**********************************************************************************
//* 3層データバインド・カスタムTableAdapter
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：my_tableTableAdapter
//* クラス日本語名  ：三層データバインド・カスタムTableAdapter（my_table）
//*
//* 作成日時        ：2014/3/30
//* 作成者          ：自動生成ツール（墨壺２）, 日立 太郎
//* 更新履歴        ：
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

/// <summary>三層データバインド・カスタムTableAdapter（my_table）</summary>
public class my_tableTableAdapter : CmnTableAdapter
{
    /// <summary>データ件数取得処理を実装</summary>
    /// <returns>データ件数</returns>
    public int SelectCountMethod()
    {
        _3TierReturnValue returnValue = null;

        try
        {
            // ここのデータアクセス処理は棟梁のB層を呼び出して実装する。

            // ユーザ情報を取得
            MyUserInfo myUserInfo = MyBaseController.GetUserInfo2();

            // 引数クラスを生成
            _3TierParameterValue parameterValue
                = this.CreateParameter("my_table", "SelectCountMethod", myUserInfo);

            // B層を生成
            _3TierEngine b = new _3TierEngine();

            // データ件数取得処理を実行
            returnValue = (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);
        }
        catch (Exception ex)
        {
            MyBaseController.TransferErrorScreen2(ex);
            return 0;
        }

        // データ件数を返却
        //（OracleでdecimalになるケースがあるのでParseしている。）
        return int.Parse(returnValue.Obj.ToString());
    }

    /// <summary>データ取得処理を実装</summary>
    /// <param name="startRowIndex">開始位置</param>
    /// <param name="maximumRows">取得行数</param>
    /// <returns>DataTable</returns>
    public DataTable SelectMethod(int startRowIndex, int maximumRows)
    {
        _3TierReturnValue returnValue = null;

        try
        {
            // ここのデータアクセス処理は棟梁のB層を呼び出して実装する。

            // ユーザ情報を取得
            MyUserInfo myUserInfo = MyBaseController.GetUserInfo2();

            // 引数クラスを生成
            _3TierParameterValue parameterValue
                = this.CreateParameter("my_table", "SelectMethod", myUserInfo);

            // カラムリスト（射影
            parameterValue.ColumnList = "_s_columna_e_, _s_columnb_e_, _s_columnc_e_, _s_columnd_e_";

            // ソート条件
            parameterValue.SortExpression =
                (string)HttpContext.Current.Session["SortExpression"];
            parameterValue.SortDirection =
                (string)HttpContext.Current.Session["SortDirection"];

            // ページング条件
            parameterValue.MaximumRows = maximumRows;
            parameterValue.StartRowIndex = startRowIndex;

            // B層を生成
            _3TierEngine b = new _3TierEngine();

            // データ取得処理を実行
            returnValue = (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // GridView1.DataSourceから取得できなかったのでSessionに格納。
            HttpContext.Current.Session["SearchResult"] = returnValue.Dt;
        }
        catch (Exception ex)
        {
            MyBaseController.TransferErrorScreen2(ex);
            return null;
        }

        // 取得データを返却
        return returnValue.Dt;
    }
}
