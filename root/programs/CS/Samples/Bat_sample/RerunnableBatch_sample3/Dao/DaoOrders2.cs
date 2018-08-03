//**********************************************************************************
//* フレームワーク・テストクラス（Ｄ層）
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：DaoOrders2
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
public class DaoOrders2 : MyBaseDao
{
    #region インスタンス変数

    /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
    protected Hashtable HtUserParameter = new Hashtable();
    /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
    protected Hashtable HtParameter = new Hashtable();
    
    #endregion

    #region コンストラクタ

    /// <summary>コンストラクタ</summary>
    public DaoOrders2(BaseDam dam) : base(dam) { }

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


    /// <summary>OrderID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PK_OrderID
    {
        set
        {
            this.HtParameter["OrderID"] = value;
        }
        get
        {
            return this.HtParameter["OrderID"];
        }
    }

    
    
    /// <summary>CustomerID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object CustomerID
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
    
    /// <summary>EmployeeID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object EmployeeID
    {
        set
        {
            this.HtParameter["EmployeeID"] = value;
        }
        get
        {
            return this.HtParameter["EmployeeID"];
        }
    }
    
    /// <summary>OrderDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object OrderDate
    {
        set
        {
            this.HtParameter["OrderDate"] = value;
        }
        get
        {
            return this.HtParameter["OrderDate"];
        }
    }
    
    /// <summary>RequiredDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object RequiredDate
    {
        set
        {
            this.HtParameter["RequiredDate"] = value;
        }
        get
        {
            return this.HtParameter["RequiredDate"];
        }
    }
    
    /// <summary>ShippedDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShippedDate
    {
        set
        {
            this.HtParameter["ShippedDate"] = value;
        }
        get
        {
            return this.HtParameter["ShippedDate"];
        }
    }
    
    /// <summary>ShipVia列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipVia
    {
        set
        {
            this.HtParameter["ShipVia"] = value;
        }
        get
        {
            return this.HtParameter["ShipVia"];
        }
    }
    
    /// <summary>Freight列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Freight
    {
        set
        {
            this.HtParameter["Freight"] = value;
        }
        get
        {
            return this.HtParameter["Freight"];
        }
    }
    
    /// <summary>ShipName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipName
    {
        set
        {
            this.HtParameter["ShipName"] = value;
        }
        get
        {
            return this.HtParameter["ShipName"];
        }
    }
    
    /// <summary>ShipAddress列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipAddress
    {
        set
        {
            this.HtParameter["ShipAddress"] = value;
        }
        get
        {
            return this.HtParameter["ShipAddress"];
        }
    }
    
    /// <summary>ShipCity列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipCity
    {
        set
        {
            this.HtParameter["ShipCity"] = value;
        }
        get
        {
            return this.HtParameter["ShipCity"];
        }
    }
    
    /// <summary>ShipRegion列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipRegion
    {
        set
        {
            this.HtParameter["ShipRegion"] = value;
        }
        get
        {
            return this.HtParameter["ShipRegion"];
        }
    }
    
    /// <summary>ShipPostalCode列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipPostalCode
    {
        set
        {
            this.HtParameter["ShipPostalCode"] = value;
        }
        get
        {
            return this.HtParameter["ShipPostalCode"];
        }
    }
    
    /// <summary>ShipCountry列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ShipCountry
    {
        set
        {
            this.HtParameter["ShipCountry"] = value;
        }
        get
        {
            return this.HtParameter["ShipCountry"];
        }
    }


    /// <summary>Set_OrderID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_OrderID_forUPD
    {
        set
        {
            this.HtParameter["Set_OrderID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_OrderID_forUPD"];
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


    /// <summary>Set_EmployeeID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_EmployeeID_forUPD
    {
        set
        {
            this.HtParameter["Set_EmployeeID_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_EmployeeID_forUPD"];
        }
    }


    /// <summary>Set_OrderDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_OrderDate_forUPD
    {
        set
        {
            this.HtParameter["Set_OrderDate_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_OrderDate_forUPD"];
        }
    }


    /// <summary>Set_RequiredDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_RequiredDate_forUPD
    {
        set
        {
            this.HtParameter["Set_RequiredDate_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_RequiredDate_forUPD"];
        }
    }


    /// <summary>Set_ShippedDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShippedDate_forUPD
    {
        set
        {
            this.HtParameter["Set_ShippedDate_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShippedDate_forUPD"];
        }
    }


    /// <summary>Set_ShipVia_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipVia_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipVia_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipVia_forUPD"];
        }
    }


    /// <summary>Set_Freight_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Freight_forUPD
    {
        set
        {
            this.HtParameter["Set_Freight_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Freight_forUPD"];
        }
    }


    /// <summary>Set_ShipName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipName_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipName_forUPD"];
        }
    }


    /// <summary>Set_ShipAddress_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipAddress_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipAddress_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipAddress_forUPD"];
        }
    }


    /// <summary>Set_ShipCity_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipCity_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipCity_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipCity_forUPD"];
        }
    }


    /// <summary>Set_ShipRegion_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipRegion_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipRegion_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipRegion_forUPD"];
        }
    }


    /// <summary>Set_ShipPostalCode_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipPostalCode_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipPostalCode_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipPostalCode_forUPD"];
        }
    }


    /// <summary>Set_ShipCountry_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ShipCountry_forUPD
    {
        set
        {
            this.HtParameter["Set_ShipCountry_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ShipCountry_forUPD"];
        }
    }



    /// <summary>OrderID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object OrderID_Like
    {
        set
        {
            this.HtParameter["OrderID_Like"] = value;
        }
        get
        {
            return this.HtParameter["OrderID_Like"];
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


    /// <summary>EmployeeID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object EmployeeID_Like
    {
        set
        {
            this.HtParameter["EmployeeID_Like"] = value;
        }
        get
        {
            return this.HtParameter["EmployeeID_Like"];
        }
    }


    /// <summary>OrderDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object OrderDate_Like
    {
        set
        {
            this.HtParameter["OrderDate_Like"] = value;
        }
        get
        {
            return this.HtParameter["OrderDate_Like"];
        }
    }


    /// <summary>RequiredDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object RequiredDate_Like
    {
        set
        {
            this.HtParameter["RequiredDate_Like"] = value;
        }
        get
        {
            return this.HtParameter["RequiredDate_Like"];
        }
    }


    /// <summary>ShippedDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShippedDate_Like
    {
        set
        {
            this.HtParameter["ShippedDate_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShippedDate_Like"];
        }
    }


    /// <summary>ShipVia_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipVia_Like
    {
        set
        {
            this.HtParameter["ShipVia_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipVia_Like"];
        }
    }


    /// <summary>Freight_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Freight_Like
    {
        set
        {
            this.HtParameter["Freight_Like"] = value;
        }
        get
        {
            return this.HtParameter["Freight_Like"];
        }
    }


    /// <summary>ShipName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipName_Like
    {
        set
        {
            this.HtParameter["ShipName_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipName_Like"];
        }
    }


    /// <summary>ShipAddress_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipAddress_Like
    {
        set
        {
            this.HtParameter["ShipAddress_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipAddress_Like"];
        }
    }


    /// <summary>ShipCity_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipCity_Like
    {
        set
        {
            this.HtParameter["ShipCity_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipCity_Like"];
        }
    }


    /// <summary>ShipRegion_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipRegion_Like
    {
        set
        {
            this.HtParameter["ShipRegion_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipRegion_Like"];
        }
    }


    /// <summary>ShipPostalCode_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipPostalCode_Like
    {
        set
        {
            this.HtParameter["ShipPostalCode_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipPostalCode_Like"];
        }
    }


    /// <summary>ShipCountry_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ShipCountry_Like
    {
        set
        {
            this.HtParameter["ShipCountry_Like"] = value;
        }
        get
        {
            return this.HtParameter["ShipCountry_Like"];
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
        this.SetSqlByFile2("DaoOrders2_S1_Insert.sql");

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
        this.SetSqlByFile2("DaoOrders2_D1_Insert.xml");

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
        this.SetSqlByFile2("DaoOrders2_S2_Select.xml");

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
        this.SetSqlByFile2("DaoOrders2_D2_Select.xml");

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
        this.SetSqlByFile2("DaoOrders2_S3_Update.xml");

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
        this.SetSqlByFile2("DaoOrders2_D3_Update.xml");

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
        this.SetSqlByFile2("DaoOrders2_S4_Delete.xml");

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
        this.SetSqlByFile2("DaoOrders2_D4_Delete.xml");

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
        this.SetSqlByFile2("DaoOrders2_D5_SelCnt.xml");

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
