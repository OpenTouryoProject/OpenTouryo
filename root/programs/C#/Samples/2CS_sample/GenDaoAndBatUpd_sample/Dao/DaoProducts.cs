//**********************************************************************************
//* フレームワーク・テストクラス（Ｄ層）
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：DaoProducts
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

using System.Data;
using System.Collections;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;

/// <summary>自動生成Ｄａｏクラス</summary>
public class DaoProducts : MyBaseDao
{
    #region インスタンス変数

    /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
    protected Hashtable HtUserParameter = new Hashtable();
    /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
    protected Hashtable HtParameter = new Hashtable();
    
    #endregion

    #region コンストラクタ

    /// <summary>コンストラクタ</summary>
    public DaoProducts(BaseDam dam) : base(dam) { }

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


    /// <summary>ProductID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PK_ProductID
    {
        set
        {
            this.HtParameter["ProductID"] = value;
        }
        get
        {
            return this.HtParameter["ProductID"];
        }
    }

    
    
    /// <summary>ProductName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ProductName
    {
        set
        {
            this.HtParameter["ProductName"] = value;
        }
        get
        {
            return this.HtParameter["ProductName"];
        }
    }
    
    /// <summary>SupplierID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object SupplierID
    {
        set
        {
            this.HtParameter["SupplierID"] = value;
        }
        get
        {
            return this.HtParameter["SupplierID"];
        }
    }
    
    /// <summary>CategoryID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object CategoryID
    {
        set
        {
            this.HtParameter["CategoryID"] = value;
        }
        get
        {
            return this.HtParameter["CategoryID"];
        }
    }
    
    /// <summary>QuantityPerUnit列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object QuantityPerUnit
    {
        set
        {
            this.HtParameter["QuantityPerUnit"] = value;
        }
        get
        {
            return this.HtParameter["QuantityPerUnit"];
        }
    }
    
    /// <summary>UnitPrice列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object UnitPrice
    {
        set
        {
            this.HtParameter["UnitPrice"] = value;
        }
        get
        {
            return this.HtParameter["UnitPrice"];
        }
    }
    
    /// <summary>UnitsInStock列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object UnitsInStock
    {
        set
        {
            this.HtParameter["UnitsInStock"] = value;
        }
        get
        {
            return this.HtParameter["UnitsInStock"];
        }
    }
    
    /// <summary>UnitsOnOrder列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object UnitsOnOrder
    {
        set
        {
            this.HtParameter["UnitsOnOrder"] = value;
        }
        get
        {
            return this.HtParameter["UnitsOnOrder"];
        }
    }
    
    /// <summary>ReorderLevel列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ReorderLevel
    {
        set
        {
            this.HtParameter["ReorderLevel"] = value;
        }
        get
        {
            return this.HtParameter["ReorderLevel"];
        }
    }
    
    /// <summary>Discontinued列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Discontinued
    {
        set
        {
            this.HtParameter["Discontinued"] = value;
        }
        get
        {
            return this.HtParameter["Discontinued"];
        }
    }


    /// <summary>Set_ProductID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ProductID_forUPD
    {
        set
        {
            this.HtParameter["Set_ProductID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ProductID_forUPD"];
        }
    }


    /// <summary>Set_ProductName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ProductName_forUPD
    {
        set
        {
            this.HtParameter["Set_ProductName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ProductName_forUPD"];
        }
    }


    /// <summary>Set_SupplierID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_SupplierID_forUPD
    {
        set
        {
            this.HtParameter["Set_SupplierID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_SupplierID_forUPD"];
        }
    }


    /// <summary>Set_CategoryID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_CategoryID_forUPD
    {
        set
        {
            this.HtParameter["Set_CategoryID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_CategoryID_forUPD"];
        }
    }


    /// <summary>Set_QuantityPerUnit_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_QuantityPerUnit_forUPD
    {
        set
        {
            this.HtParameter["Set_QuantityPerUnit_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_QuantityPerUnit_forUPD"];
        }
    }


    /// <summary>Set_UnitPrice_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_UnitPrice_forUPD
    {
        set
        {
            this.HtParameter["Set_UnitPrice_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_UnitPrice_forUPD"];
        }
    }


    /// <summary>Set_UnitsInStock_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_UnitsInStock_forUPD
    {
        set
        {
            this.HtParameter["Set_UnitsInStock_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_UnitsInStock_forUPD"];
        }
    }


    /// <summary>Set_UnitsOnOrder_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_UnitsOnOrder_forUPD
    {
        set
        {
            this.HtParameter["Set_UnitsOnOrder_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_UnitsOnOrder_forUPD"];
        }
    }


    /// <summary>Set_ReorderLevel_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ReorderLevel_forUPD
    {
        set
        {
            this.HtParameter["Set_ReorderLevel_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ReorderLevel_forUPD"];
        }
    }


    /// <summary>Set_Discontinued_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Discontinued_forUPD
    {
        set
        {
            this.HtParameter["Set_Discontinued_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Discontinued_forUPD"];
        }
    }



    /// <summary>ProductID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ProductID_Like
    {
        set
        {
            this.HtParameter["ProductID_Like"] = value;
        }
        get
        {
            return this.HtParameter["ProductID_Like"];
        }
    }


    /// <summary>ProductName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ProductName_Like
    {
        set
        {
            this.HtParameter["ProductName_Like"] = value;
        }
        get
        {
            return this.HtParameter["ProductName_Like"];
        }
    }


    /// <summary>SupplierID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object SupplierID_Like
    {
        set
        {
            this.HtParameter["SupplierID_Like"] = value;
        }
        get
        {
            return this.HtParameter["SupplierID_Like"];
        }
    }


    /// <summary>CategoryID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object CategoryID_Like
    {
        set
        {
            this.HtParameter["CategoryID_Like"] = value;
        }
        get
        {
            return this.HtParameter["CategoryID_Like"];
        }
    }


    /// <summary>QuantityPerUnit_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object QuantityPerUnit_Like
    {
        set
        {
            this.HtParameter["QuantityPerUnit_Like"] = value;
        }
        get
        {
            return this.HtParameter["QuantityPerUnit_Like"];
        }
    }


    /// <summary>UnitPrice_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object UnitPrice_Like
    {
        set
        {
            this.HtParameter["UnitPrice_Like"] = value;
        }
        get
        {
            return this.HtParameter["UnitPrice_Like"];
        }
    }


    /// <summary>UnitsInStock_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object UnitsInStock_Like
    {
        set
        {
            this.HtParameter["UnitsInStock_Like"] = value;
        }
        get
        {
            return this.HtParameter["UnitsInStock_Like"];
        }
    }


    /// <summary>UnitsOnOrder_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object UnitsOnOrder_Like
    {
        set
        {
            this.HtParameter["UnitsOnOrder_Like"] = value;
        }
        get
        {
            return this.HtParameter["UnitsOnOrder_Like"];
        }
    }


    /// <summary>ReorderLevel_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ReorderLevel_Like
    {
        set
        {
            this.HtParameter["ReorderLevel_Like"] = value;
        }
        get
        {
            return this.HtParameter["ReorderLevel_Like"];
        }
    }


    /// <summary>Discontinued_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Discontinued_Like
    {
        set
        {
            this.HtParameter["Discontinued_Like"] = value;
        }
        get
        {
            return this.HtParameter["Discontinued_Like"];
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
        this.SetSqlByFile2("DaoProducts_S1_Insert.sql");

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
        this.SetSqlByFile2("DaoProducts_D1_Insert.xml");

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
        this.SetSqlByFile2("DaoProducts_S2_Select.xml");

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
        this.SetSqlByFile2("DaoProducts_D2_Select.xml");

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
        this.SetSqlByFile2("DaoProducts_S3_Update.xml");

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
        this.SetSqlByFile2("DaoProducts_D3_Update.xml");

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
        this.SetSqlByFile2("DaoProducts_S4_Delete.xml");

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
        this.SetSqlByFile2("DaoProducts_D4_Delete.xml");

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
        this.SetSqlByFile2("DaoProducts_D5_SelCnt.xml");

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
