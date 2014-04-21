//**********************************************************************************
//* クラス名        ：DaoEmployees
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
public class DaoEmployees : MyBaseDao
{
    #region インスタンス変数

    /// <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
    protected Hashtable HtUserParameter = new Hashtable();
    /// <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
    protected Hashtable HtParameter = new Hashtable();
    
    #endregion

    #region コンストラクタ

    /// <summary>コンストラクタ</summary>
    public DaoEmployees(BaseDam dam) : base(dam) { }

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


    /// <summary>EmployeeID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PK_EmployeeID
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

    
    
    /// <summary>LastName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object LastName
    {
        set
        {
            this.HtParameter["LastName"] = value;
        }
        get
        {
            return this.HtParameter["LastName"];
        }
    }
    
    /// <summary>FirstName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object FirstName
    {
        set
        {
            this.HtParameter["FirstName"] = value;
        }
        get
        {
            return this.HtParameter["FirstName"];
        }
    }
    
    /// <summary>Title列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Title
    {
        set
        {
            this.HtParameter["Title"] = value;
        }
        get
        {
            return this.HtParameter["Title"];
        }
    }
    
    /// <summary>TitleOfCourtesy列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object TitleOfCourtesy
    {
        set
        {
            this.HtParameter["TitleOfCourtesy"] = value;
        }
        get
        {
            return this.HtParameter["TitleOfCourtesy"];
        }
    }
    
    /// <summary>BirthDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object BirthDate
    {
        set
        {
            this.HtParameter["BirthDate"] = value;
        }
        get
        {
            return this.HtParameter["BirthDate"];
        }
    }
    
    /// <summary>HireDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object HireDate
    {
        set
        {
            this.HtParameter["HireDate"] = value;
        }
        get
        {
            return this.HtParameter["HireDate"];
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
    
    /// <summary>HomePhone列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object HomePhone
    {
        set
        {
            this.HtParameter["HomePhone"] = value;
        }
        get
        {
            return this.HtParameter["HomePhone"];
        }
    }
    
    /// <summary>Extension列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Extension
    {
        set
        {
            this.HtParameter["Extension"] = value;
        }
        get
        {
            return this.HtParameter["Extension"];
        }
    }
    
    /// <summary>Photo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Photo
    {
        set
        {
            this.HtParameter["Photo"] = value;
        }
        get
        {
            return this.HtParameter["Photo"];
        }
    }
    
    /// <summary>Notes列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object Notes
    {
        set
        {
            this.HtParameter["Notes"] = value;
        }
        get
        {
            return this.HtParameter["Notes"];
        }
    }
    
    /// <summary>ReportsTo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object ReportsTo
    {
        set
        {
            this.HtParameter["ReportsTo"] = value;
        }
        get
        {
            return this.HtParameter["ReportsTo"];
        }
    }
    
    /// <summary>PhotoPath列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
    public object PhotoPath
    {
        set
        {
            this.HtParameter["PhotoPath"] = value;
        }
        get
        {
            return this.HtParameter["PhotoPath"];
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


    /// <summary>Set_LastName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_LastName_forUPD
    {
        set
        {
            this.HtParameter["Set_LastName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_LastName_forUPD"];
        }
    }


    /// <summary>Set_FirstName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_FirstName_forUPD
    {
        set
        {
            this.HtParameter["Set_FirstName_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_FirstName_forUPD"];
        }
    }


    /// <summary>Set_Title_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Title_forUPD
    {
        set
        {
            this.HtParameter["Set_Title_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Title_forUPD"];
        }
    }


    /// <summary>Set_TitleOfCourtesy_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_TitleOfCourtesy_forUPD
    {
        set
        {
            this.HtParameter["Set_TitleOfCourtesy_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_TitleOfCourtesy_forUPD"];
        }
    }


    /// <summary>Set_BirthDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_BirthDate_forUPD
    {
        set
        {
            this.HtParameter["Set_BirthDate_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_BirthDate_forUPD"];
        }
    }


    /// <summary>Set_HireDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_HireDate_forUPD
    {
        set
        {
            this.HtParameter["Set_HireDate_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_HireDate_forUPD"];
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


    /// <summary>Set_HomePhone_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_HomePhone_forUPD
    {
        set
        {
            this.HtParameter["Set_HomePhone_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_HomePhone_forUPD"];
        }
    }


    /// <summary>Set_Extension_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Extension_forUPD
    {
        set
        {
            this.HtParameter["Set_Extension_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Extension_forUPD"];
        }
    }


    /// <summary>Set_Photo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Photo_forUPD
    {
        set
        {
            this.HtParameter["Set_Photo_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Photo_forUPD"];
        }
    }


    /// <summary>Set_Notes_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_Notes_forUPD
    {
        set
        {
            this.HtParameter["Set_Notes_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_Notes_forUPD"];
        }
    }


    /// <summary>Set_ReportsTo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_ReportsTo_forUPD
    {
        set
        {
            this.HtParameter["Set_ReportsTo_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_ReportsTo_forUPD"];
        }
    }


    /// <summary>Set_PhotoPath_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
    public object Set_PhotoPath_forUPD
    {
        set
        {
            this.HtParameter["Set_PhotoPath_forUPD"] = value;
        }
        get
        {
            return this.HtParameter["Set_PhotoPath_forUPD"];
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


    /// <summary>LastName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object LastName_Like
    {
        set
        {
            this.HtParameter["LastName_Like"] = value;
        }
        get
        {
            return this.HtParameter["LastName_Like"];
        }
    }


    /// <summary>FirstName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object FirstName_Like
    {
        set
        {
            this.HtParameter["FirstName_Like"] = value;
        }
        get
        {
            return this.HtParameter["FirstName_Like"];
        }
    }


    /// <summary>Title_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Title_Like
    {
        set
        {
            this.HtParameter["Title_Like"] = value;
        }
        get
        {
            return this.HtParameter["Title_Like"];
        }
    }


    /// <summary>TitleOfCourtesy_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object TitleOfCourtesy_Like
    {
        set
        {
            this.HtParameter["TitleOfCourtesy_Like"] = value;
        }
        get
        {
            return this.HtParameter["TitleOfCourtesy_Like"];
        }
    }


    /// <summary>BirthDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object BirthDate_Like
    {
        set
        {
            this.HtParameter["BirthDate_Like"] = value;
        }
        get
        {
            return this.HtParameter["BirthDate_Like"];
        }
    }


    /// <summary>HireDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object HireDate_Like
    {
        set
        {
            this.HtParameter["HireDate_Like"] = value;
        }
        get
        {
            return this.HtParameter["HireDate_Like"];
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


    /// <summary>HomePhone_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object HomePhone_Like
    {
        set
        {
            this.HtParameter["HomePhone_Like"] = value;
        }
        get
        {
            return this.HtParameter["HomePhone_Like"];
        }
    }


    /// <summary>Extension_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Extension_Like
    {
        set
        {
            this.HtParameter["Extension_Like"] = value;
        }
        get
        {
            return this.HtParameter["Extension_Like"];
        }
    }


    /// <summary>Photo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Photo_Like
    {
        set
        {
            this.HtParameter["Photo_Like"] = value;
        }
        get
        {
            return this.HtParameter["Photo_Like"];
        }
    }


    /// <summary>Notes_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object Notes_Like
    {
        set
        {
            this.HtParameter["Notes_Like"] = value;
        }
        get
        {
            return this.HtParameter["Notes_Like"];
        }
    }


    /// <summary>ReportsTo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object ReportsTo_Like
    {
        set
        {
            this.HtParameter["ReportsTo_Like"] = value;
        }
        get
        {
            return this.HtParameter["ReportsTo_Like"];
        }
    }


    /// <summary>PhotoPath_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
    /// <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
    public object PhotoPath_Like
    {
        set
        {
            this.HtParameter["PhotoPath_Like"] = value;
        }
        get
        {
            return this.HtParameter["PhotoPath_Like"];
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
        this.SetSqlByFile2("DaoEmployees_S1_Insert.sql");

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
        this.SetSqlByFile2("DaoEmployees_D1_Insert.xml");

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
        this.SetSqlByFile2("DaoEmployees_S2_Select.xml");

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
        this.SetSqlByFile2("DaoEmployees_D2_Select.xml");

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
        this.SetSqlByFile2("DaoEmployees_S3_Update.xml");

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
        this.SetSqlByFile2("DaoEmployees_D3_Update.xml");

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
        this.SetSqlByFile2("DaoEmployees_S4_Delete.xml");

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
        this.SetSqlByFile2("DaoEmployees_D4_Delete.xml");

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
        this.SetSqlByFile2("DaoEmployees_D5_SelCnt.xml");

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
