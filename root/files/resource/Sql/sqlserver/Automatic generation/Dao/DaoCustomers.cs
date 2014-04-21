//**********************************************************************************
//* クラス名        ：DaoCustomers
//* クラス日本語名  ：自動生成Ｄａｏクラス
//*
//* 作成日時        ：2014/2/9
//* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2012/06/14  西野  大介        ResourceLoaderに加え、EmbeddedResourceLoaderに対応
//*  2013/09/09  西野  大介        ExecGenerateSQLメソッドを追加した（バッチ更新用）。
//**********************************************************************************

#region using

// System～
using System;
using System.IO;
using System.Data;
using System.Collections;

// フレームワーク
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Common;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Dao;

#endregion

/// <summary>自動生成Ｄａｏクラス</summary>
public class DaoCustomers : MyBaseDao
{
    #region インスタンス変数

    /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
    protected Hashtable HtUserParameter = new Hashtable();
    /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
    protected Hashtable HtParameter = new Hashtable();
    
    #endregion

    #region コンストラクタ

    /// <summary>コンストラクタ</summary>
    public DaoCustomers(BaseDam dam) : base(dam) { }

    #endregion

    #region 共通関数（パラメタの制御）

    /// <summary>ユーザ パラメタ（文字列置換）をハッシュ テーブルに設定する。</summary>
    /// <param name="userParamName">ユーザ パラメタ名</param>
    /// <param name="userParamValue">ユーザ パラメタ値</param>
    public void SetUserParameteToHt(string userParamName, string userParamValue)
    {
        // ユーザ パラメタをハッシュ テーブルに設定
        this.HtUserParameter[userParamName] = userParamValue;
    }

    /// <summary>パラメタ ライズド クエリのパラメタをハッシュ テーブルに設定する。</summary>
    /// <param name="paramName">パラメタ名</param>
    /// <param name="paramValue">パラメタ値</param>
    public void SetParameteToHt(string paramName, object paramValue)
    {
        // ユーザ パラメタをハッシュ テーブルに設定
        this.HtParameter[paramName] = paramValue;
    }

    /// <summary>
    /// ・ユーザ パラメタ（文字列置換）
    /// ・パラメタ ライズド クエリのパラメタ
    /// を格納するハッシュ テーブルをクリアする。
    /// </summary>
    public void ClearParametersFromHt()
    {
        // ユーザ パラメタ（文字列置換）用ハッシュ テーブルを初期化
        this.HtUserParameter = new Hashtable();
        // パラメタ ライズド クエリのパラメタ用ハッシュ テーブルを初期化
        this.HtParameter = new Hashtable();
    }

    /// <summary>パラメタの設定（内部用）</summary>
    protected void SetParametersFromHt()
    {
        // ユーザ パラメタ（文字列置換）を設定する。
        foreach (string userParamName in this.HtUserParameter.Keys)
        {
            this.SetUserParameter(userParamName, this.HtUserParameter[userParamName].ToString());
        }

        // パラメタ ライズド クエリのパラメタを設定する。
        foreach (string paramName in this.HtParameter.Keys)
        {
            this.SetParameter(paramName, this.HtParameter[paramName]);
        }
    }

    #endregion

    #region プロパティ プロシージャ（setter、getter）


    /// <summary>CustomerID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PK_CustomerID
    {
        set
        {
            this.HtParameter["CustomerID"] = value;
        }
        get
        {
            return this.HtParameter["CustomerID"];
        }
    }

    
    
    /// <summary>CompanyName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object CompanyName
    {
        set
        {
            this.HtParameter["CompanyName"] = value;
        }
        get
        {
            return this.HtParameter["CompanyName"];
        }
    }
    
    /// <summary>ContactName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ContactName
    {
        set
        {
            this.HtParameter["ContactName"] = value;
        }
        get
        {
            return this.HtParameter["ContactName"];
        }
    }
    
    /// <summary>ContactTitle列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ContactTitle
    {
        set
        {
            this.HtParameter["ContactTitle"] = value;
        }
        get
        {
            return this.HtParameter["ContactTitle"];
        }
    }
    
    /// <summary>Address列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Address
    {
        set
        {
            this.HtParameter["Address"] = value;
        }
        get
        {
            return this.HtParameter["Address"];
        }
    }
    
    /// <summary>City列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object City
    {
        set
        {
            this.HtParameter["City"] = value;
        }
        get
        {
            return this.HtParameter["City"];
        }
    }
    
    /// <summary>Region列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Region
    {
        set
        {
            this.HtParameter["Region"] = value;
        }
        get
        {
            return this.HtParameter["Region"];
        }
    }
    
    /// <summary>PostalCode列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PostalCode
    {
        set
        {
            this.HtParameter["PostalCode"] = value;
        }
        get
        {
            return this.HtParameter["PostalCode"];
        }
    }
    
    /// <summary>Country列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Country
    {
        set
        {
            this.HtParameter["Country"] = value;
        }
        get
        {
            return this.HtParameter["Country"];
        }
    }
    
    /// <summary>Phone列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Phone
    {
        set
        {
            this.HtParameter["Phone"] = value;
        }
        get
        {
            return this.HtParameter["Phone"];
        }
    }
    
    /// <summary>Fax列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Fax
    {
        set
        {
            this.HtParameter["Fax"] = value;
        }
        get
        {
            return this.HtParameter["Fax"];
        }
    }


    /// <summary>Set_CustomerID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_CustomerID_forUPD
    {
        set
        {
            this.HtParameter["Set_CustomerID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_CustomerID_forUPD"];
        }
    }


    /// <summary>Set_CompanyName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_CompanyName_forUPD
    {
        set
        {
            this.HtParameter["Set_CompanyName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_CompanyName_forUPD"];
        }
    }


    /// <summary>Set_ContactName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ContactName_forUPD
    {
        set
        {
            this.HtParameter["Set_ContactName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ContactName_forUPD"];
        }
    }


    /// <summary>Set_ContactTitle_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ContactTitle_forUPD
    {
        set
        {
            this.HtParameter["Set_ContactTitle_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ContactTitle_forUPD"];
        }
    }


    /// <summary>Set_Address_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Address_forUPD
    {
        set
        {
            this.HtParameter["Set_Address_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Address_forUPD"];
        }
    }


    /// <summary>Set_City_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_City_forUPD
    {
        set
        {
            this.HtParameter["Set_City_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_City_forUPD"];
        }
    }


    /// <summary>Set_Region_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Region_forUPD
    {
        set
        {
            this.HtParameter["Set_Region_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Region_forUPD"];
        }
    }


    /// <summary>Set_PostalCode_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_PostalCode_forUPD
    {
        set
        {
            this.HtParameter["Set_PostalCode_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_PostalCode_forUPD"];
        }
    }


    /// <summary>Set_Country_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Country_forUPD
    {
        set
        {
            this.HtParameter["Set_Country_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Country_forUPD"];
        }
    }


    /// <summary>Set_Phone_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Phone_forUPD
    {
        set
        {
            this.HtParameter["Set_Phone_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Phone_forUPD"];
        }
    }


    /// <summary>Set_Fax_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Fax_forUPD
    {
        set
        {
            this.HtParameter["Set_Fax_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Fax_forUPD"];
        }
    }



    /// <summary>CustomerID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object CustomerID_Like
    {
        set
        {
            this.HtParameter["CustomerID_Like"] = value;
        }
        get
        {
            return this.HtParameter["CustomerID_Like"];
        }
    }


    /// <summary>CompanyName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object CompanyName_Like
    {
        set
        {
            this.HtParameter["CompanyName_Like"] = value;
        }
        get
        {
            return this.HtParameter["CompanyName_Like"];
        }
    }


    /// <summary>ContactName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ContactName_Like
    {
        set
        {
            this.HtParameter["ContactName_Like"] = value;
        }
        get
        {
            return this.HtParameter["ContactName_Like"];
        }
    }


    /// <summary>ContactTitle_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ContactTitle_Like
    {
        set
        {
            this.HtParameter["ContactTitle_Like"] = value;
        }
        get
        {
            return this.HtParameter["ContactTitle_Like"];
        }
    }


    /// <summary>Address_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Address_Like
    {
        set
        {
            this.HtParameter["Address_Like"] = value;
        }
        get
        {
            return this.HtParameter["Address_Like"];
        }
    }


    /// <summary>City_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object City_Like
    {
        set
        {
            this.HtParameter["City_Like"] = value;
        }
        get
        {
            return this.HtParameter["City_Like"];
        }
    }


    /// <summary>Region_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Region_Like
    {
        set
        {
            this.HtParameter["Region_Like"] = value;
        }
        get
        {
            return this.HtParameter["Region_Like"];
        }
    }


    /// <summary>PostalCode_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object PostalCode_Like
    {
        set
        {
            this.HtParameter["PostalCode_Like"] = value;
        }
        get
        {
            return this.HtParameter["PostalCode_Like"];
        }
    }


    /// <summary>Country_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Country_Like
    {
        set
        {
            this.HtParameter["Country_Like"] = value;
        }
        get
        {
            return this.HtParameter["Country_Like"];
        }
    }


    /// <summary>Phone_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Phone_Like
    {
        set
        {
            this.HtParameter["Phone_Like"] = value;
        }
        get
        {
            return this.HtParameter["Phone_Like"];
        }
    }


    /// <summary>Fax_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Fax_Like
    {
        set
        {
            this.HtParameter["Fax_Like"] = value;
        }
        get
        {
            return this.HtParameter["Fax_Like"];
        }
    }


    #endregion

    #region クエリ メソッド

    #region Insert
    
    /// <summary>１レコード挿入する。</summary>
    /// <returns>挿入された行の数</returns>
    public int S1_Insert()
    {
        // ファイルからSQL（Insert）を設定する。
        this.SetSqlByFile2("DaoCustomers_S1_Insert.sql");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（Insert）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }

    /// <summary>１レコード挿入する。</summary>
    /// <returns>挿入された行の数</returns>
    /// <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
    public int D1_Insert()
    {
        // ファイルからSQL（DynIns）を設定する。
        this.SetSqlByFile2("DaoCustomers_D1_Insert.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（DynIns）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }

    #endregion

    #region Select

    /// <summary>主キーを指定し、１レコード参照する。</summary>
    /// <param name="dt">結果を格納するDataTable</param>
    public void S2_Select(DataTable dt)
    {
        // ファイルからSQL（Select）を設定する。
        this.SetSqlByFile2("DaoCustomers_S2_Select.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（Select）を実行し、戻り値を戻す。
        this.ExecSelectFill_DT(dt);
    }

    /// <summary>検索条件を指定し、結果セットを参照する。</summary>
    /// <param name="dt">結果を格納するDataTable</param>
    public void D2_Select(DataTable dt)
    {
        // ファイルからSQL（DynSel）を設定する。
        this.SetSqlByFile2("DaoCustomers_D2_Select.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（DynSel）を実行し、戻り値を戻す。
        this.ExecSelectFill_DT(dt);
    }

    #endregion

    #region Update

    /// <summary>主キーを指定し、１レコード更新する。</summary>
    /// <returns>更新された行の数</returns>
    /// <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
    public int S3_Update()
    {
        // ファイルからSQL（Update）を設定する。
        this.SetSqlByFile2("DaoCustomers_S3_Update.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（Update）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }

    /// <summary>任意の検索条件でデータを更新する。</summary>
    /// <returns>更新された行の数</returns>
    /// <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
    public int D3_Update()
    {
        // ファイルからSQL（DynUpd）を設定する。
        this.SetSqlByFile2("DaoCustomers_D3_Update.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（DynUpd）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }
    
    #endregion

    #region Delete

    /// <summary>主キーを指定し、１レコード削除する。</summary>
    /// <returns>削除された行の数</returns>
    public int S4_Delete()
    {
        // ファイルからSQL（Delete）を設定する。
        this.SetSqlByFile2("DaoCustomers_S4_Delete.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（Delete）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }

    /// <summary>任意の検索条件でデータを削除する。</summary>
    /// <returns>削除された行の数</returns>
    public int D4_Delete()
    {
        // ファイルからSQL（DynDel）を設定する。
        this.SetSqlByFile2("DaoCustomers_D4_Delete.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（DynDel）を実行し、戻り値を戻す。
        return this.ExecInsUpDel_NonQuery();
    }

    #endregion

    #region 拡張メソッド

    /// <summary>テーブルのレコード件数を取得する</summary>
    /// <returns>テーブルのレコード件数</returns>
    public object D5_SelCnt()
    {
        // ファイルからSQL（DynSelCnt）を設定する。
        this.SetSqlByFile2("DaoCustomers_D5_SelCnt.xml");

        // パラメタの設定
        this.SetParametersFromHt();

        // SQL（SELECT COUNT）を実行し、戻り値を戻す。
        return this.ExecSelectScalar();
    }
    
    /// <summary>静的SQLを生成する。</summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="sqlUtil">SQLユーティリティ</param>
    /// <returns>生成した静的SQL</returns>
    public string ExecGenerateSQL(string fileName, SQLUtility sqlUtil)
    {
        // ファイルからSQLを設定する。
        this.SetSqlByFile2(fileName);

        // パラメタの設定
        this.SetParametersFromHt();

        return base.ExecGenerateSQL(sqlUtil);
    }
    
    #endregion
    
    #endregion
}
