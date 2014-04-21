//**********************************************************************************
//* クラス名        ：SuppliersEntity
//* クラス日本語名  ：自動生成Entityクラス
//*
//* 作成日時        ：2014/2/9
//* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System～
using System;

/// <summary>自動生成Entityクラス</summary>
[Serializable()]
public class SuppliersEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：SupplierID</summary>
    public bool IsSetPK_SupplierID = false;

    /// <summary>メンバ変数：SupplierID</summary>
    private System.Int32? _PK_SupplierID;

    /// <summary>プロパティ：SupplierID</summary>
    public System.Int32? PK_SupplierID
    {
        get
        {
            return this._PK_SupplierID;
        }
        set
        {
            this.IsSetPK_SupplierID = true;
            this._PK_SupplierID = value;
        }
    }

    /// <summary>設定フラグ：CompanyName</summary>
    public bool IsSet_CompanyName = false;

    /// <summary>メンバ変数：CompanyName</summary>
    private System.String _CompanyName;

    /// <summary>プロパティ：CompanyName</summary>
    public System.String CompanyName
    {
        get
        {
            return this._CompanyName;
        }
        set
        {
            this.IsSet_CompanyName = true;
            this._CompanyName = value;
        }
    }
    /// <summary>設定フラグ：ContactName</summary>
    public bool IsSet_ContactName = false;

    /// <summary>メンバ変数：ContactName</summary>
    private System.String _ContactName;

    /// <summary>プロパティ：ContactName</summary>
    public System.String ContactName
    {
        get
        {
            return this._ContactName;
        }
        set
        {
            this.IsSet_ContactName = true;
            this._ContactName = value;
        }
    }
    /// <summary>設定フラグ：ContactTitle</summary>
    public bool IsSet_ContactTitle = false;

    /// <summary>メンバ変数：ContactTitle</summary>
    private System.String _ContactTitle;

    /// <summary>プロパティ：ContactTitle</summary>
    public System.String ContactTitle
    {
        get
        {
            return this._ContactTitle;
        }
        set
        {
            this.IsSet_ContactTitle = true;
            this._ContactTitle = value;
        }
    }
    /// <summary>設定フラグ：Address</summary>
    public bool IsSet_Address = false;

    /// <summary>メンバ変数：Address</summary>
    private System.String _Address;

    /// <summary>プロパティ：Address</summary>
    public System.String Address
    {
        get
        {
            return this._Address;
        }
        set
        {
            this.IsSet_Address = true;
            this._Address = value;
        }
    }
    /// <summary>設定フラグ：City</summary>
    public bool IsSet_City = false;

    /// <summary>メンバ変数：City</summary>
    private System.String _City;

    /// <summary>プロパティ：City</summary>
    public System.String City
    {
        get
        {
            return this._City;
        }
        set
        {
            this.IsSet_City = true;
            this._City = value;
        }
    }
    /// <summary>設定フラグ：Region</summary>
    public bool IsSet_Region = false;

    /// <summary>メンバ変数：Region</summary>
    private System.String _Region;

    /// <summary>プロパティ：Region</summary>
    public System.String Region
    {
        get
        {
            return this._Region;
        }
        set
        {
            this.IsSet_Region = true;
            this._Region = value;
        }
    }
    /// <summary>設定フラグ：PostalCode</summary>
    public bool IsSet_PostalCode = false;

    /// <summary>メンバ変数：PostalCode</summary>
    private System.String _PostalCode;

    /// <summary>プロパティ：PostalCode</summary>
    public System.String PostalCode
    {
        get
        {
            return this._PostalCode;
        }
        set
        {
            this.IsSet_PostalCode = true;
            this._PostalCode = value;
        }
    }
    /// <summary>設定フラグ：Country</summary>
    public bool IsSet_Country = false;

    /// <summary>メンバ変数：Country</summary>
    private System.String _Country;

    /// <summary>プロパティ：Country</summary>
    public System.String Country
    {
        get
        {
            return this._Country;
        }
        set
        {
            this.IsSet_Country = true;
            this._Country = value;
        }
    }
    /// <summary>設定フラグ：Phone</summary>
    public bool IsSet_Phone = false;

    /// <summary>メンバ変数：Phone</summary>
    private System.String _Phone;

    /// <summary>プロパティ：Phone</summary>
    public System.String Phone
    {
        get
        {
            return this._Phone;
        }
        set
        {
            this.IsSet_Phone = true;
            this._Phone = value;
        }
    }
    /// <summary>設定フラグ：Fax</summary>
    public bool IsSet_Fax = false;

    /// <summary>メンバ変数：Fax</summary>
    private System.String _Fax;

    /// <summary>プロパティ：Fax</summary>
    public System.String Fax
    {
        get
        {
            return this._Fax;
        }
        set
        {
            this.IsSet_Fax = true;
            this._Fax = value;
        }
    }
    /// <summary>設定フラグ：HomePage</summary>
    public bool IsSet_HomePage = false;

    /// <summary>メンバ変数：HomePage</summary>
    private System.String _HomePage;

    /// <summary>プロパティ：HomePage</summary>
    public System.String HomePage
    {
        get
        {
            return this._HomePage;
        }
        set
        {
            this.IsSet_HomePage = true;
            this._HomePage = value;
        }
    }

    /// <summary>設定フラグ：Set_SupplierID_forUPD</summary>
    public bool IsSet_Set_SupplierID_forUPD = false;

    /// <summary>メンバ変数：Set_SupplierID_forUPD</summary>
    private System.Int32? _Set_SupplierID_forUPD;

    /// <summary>プロパティ：Set_SupplierID_forUPD</summary>
    public System.Int32? Set_SupplierID_forUPD
    {
        get
        {
            return this._Set_SupplierID_forUPD;
        }
        set
        {
            this.IsSet_Set_SupplierID_forUPD = true;
            this._Set_SupplierID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_CompanyName_forUPD</summary>
    public bool IsSet_Set_CompanyName_forUPD = false;

    /// <summary>メンバ変数：Set_CompanyName_forUPD</summary>
    private System.String _Set_CompanyName_forUPD;

    /// <summary>プロパティ：Set_CompanyName_forUPD</summary>
    public System.String Set_CompanyName_forUPD
    {
        get
        {
            return this._Set_CompanyName_forUPD;
        }
        set
        {
            this.IsSet_Set_CompanyName_forUPD = true;
            this._Set_CompanyName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ContactName_forUPD</summary>
    public bool IsSet_Set_ContactName_forUPD = false;

    /// <summary>メンバ変数：Set_ContactName_forUPD</summary>
    private System.String _Set_ContactName_forUPD;

    /// <summary>プロパティ：Set_ContactName_forUPD</summary>
    public System.String Set_ContactName_forUPD
    {
        get
        {
            return this._Set_ContactName_forUPD;
        }
        set
        {
            this.IsSet_Set_ContactName_forUPD = true;
            this._Set_ContactName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ContactTitle_forUPD</summary>
    public bool IsSet_Set_ContactTitle_forUPD = false;

    /// <summary>メンバ変数：Set_ContactTitle_forUPD</summary>
    private System.String _Set_ContactTitle_forUPD;

    /// <summary>プロパティ：Set_ContactTitle_forUPD</summary>
    public System.String Set_ContactTitle_forUPD
    {
        get
        {
            return this._Set_ContactTitle_forUPD;
        }
        set
        {
            this.IsSet_Set_ContactTitle_forUPD = true;
            this._Set_ContactTitle_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Address_forUPD</summary>
    public bool IsSet_Set_Address_forUPD = false;

    /// <summary>メンバ変数：Set_Address_forUPD</summary>
    private System.String _Set_Address_forUPD;

    /// <summary>プロパティ：Set_Address_forUPD</summary>
    public System.String Set_Address_forUPD
    {
        get
        {
            return this._Set_Address_forUPD;
        }
        set
        {
            this.IsSet_Set_Address_forUPD = true;
            this._Set_Address_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_City_forUPD</summary>
    public bool IsSet_Set_City_forUPD = false;

    /// <summary>メンバ変数：Set_City_forUPD</summary>
    private System.String _Set_City_forUPD;

    /// <summary>プロパティ：Set_City_forUPD</summary>
    public System.String Set_City_forUPD
    {
        get
        {
            return this._Set_City_forUPD;
        }
        set
        {
            this.IsSet_Set_City_forUPD = true;
            this._Set_City_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Region_forUPD</summary>
    public bool IsSet_Set_Region_forUPD = false;

    /// <summary>メンバ変数：Set_Region_forUPD</summary>
    private System.String _Set_Region_forUPD;

    /// <summary>プロパティ：Set_Region_forUPD</summary>
    public System.String Set_Region_forUPD
    {
        get
        {
            return this._Set_Region_forUPD;
        }
        set
        {
            this.IsSet_Set_Region_forUPD = true;
            this._Set_Region_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_PostalCode_forUPD</summary>
    public bool IsSet_Set_PostalCode_forUPD = false;

    /// <summary>メンバ変数：Set_PostalCode_forUPD</summary>
    private System.String _Set_PostalCode_forUPD;

    /// <summary>プロパティ：Set_PostalCode_forUPD</summary>
    public System.String Set_PostalCode_forUPD
    {
        get
        {
            return this._Set_PostalCode_forUPD;
        }
        set
        {
            this.IsSet_Set_PostalCode_forUPD = true;
            this._Set_PostalCode_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Country_forUPD</summary>
    public bool IsSet_Set_Country_forUPD = false;

    /// <summary>メンバ変数：Set_Country_forUPD</summary>
    private System.String _Set_Country_forUPD;

    /// <summary>プロパティ：Set_Country_forUPD</summary>
    public System.String Set_Country_forUPD
    {
        get
        {
            return this._Set_Country_forUPD;
        }
        set
        {
            this.IsSet_Set_Country_forUPD = true;
            this._Set_Country_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Phone_forUPD</summary>
    public bool IsSet_Set_Phone_forUPD = false;

    /// <summary>メンバ変数：Set_Phone_forUPD</summary>
    private System.String _Set_Phone_forUPD;

    /// <summary>プロパティ：Set_Phone_forUPD</summary>
    public System.String Set_Phone_forUPD
    {
        get
        {
            return this._Set_Phone_forUPD;
        }
        set
        {
            this.IsSet_Set_Phone_forUPD = true;
            this._Set_Phone_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Fax_forUPD</summary>
    public bool IsSet_Set_Fax_forUPD = false;

    /// <summary>メンバ変数：Set_Fax_forUPD</summary>
    private System.String _Set_Fax_forUPD;

    /// <summary>プロパティ：Set_Fax_forUPD</summary>
    public System.String Set_Fax_forUPD
    {
        get
        {
            return this._Set_Fax_forUPD;
        }
        set
        {
            this.IsSet_Set_Fax_forUPD = true;
            this._Set_Fax_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_HomePage_forUPD</summary>
    public bool IsSet_Set_HomePage_forUPD = false;

    /// <summary>メンバ変数：Set_HomePage_forUPD</summary>
    private System.String _Set_HomePage_forUPD;

    /// <summary>プロパティ：Set_HomePage_forUPD</summary>
    public System.String Set_HomePage_forUPD
    {
        get
        {
            return this._Set_HomePage_forUPD;
        }
        set
        {
            this.IsSet_Set_HomePage_forUPD = true;
            this._Set_HomePage_forUPD = value;
        }
    }

    /// <summary>設定フラグ：SupplierID_Like</summary>
    public bool IsSet_SupplierID_Like = false;

    /// <summary>メンバ変数：SupplierID_Like</summary>
    private System.Int32? _SupplierID_Like;

    /// <summary>プロパティ：SupplierID_Like</summary>
    public System.Int32? SupplierID_Like
    {
        get
        {
            return this._SupplierID_Like;
        }
        set
        {
            this.IsSet_SupplierID_Like = true;
            this._SupplierID_Like = value;
        }
    }
    /// <summary>設定フラグ：CompanyName_Like</summary>
    public bool IsSet_CompanyName_Like = false;

    /// <summary>メンバ変数：CompanyName_Like</summary>
    private System.String _CompanyName_Like;

    /// <summary>プロパティ：CompanyName_Like</summary>
    public System.String CompanyName_Like
    {
        get
        {
            return this._CompanyName_Like;
        }
        set
        {
            this.IsSet_CompanyName_Like = true;
            this._CompanyName_Like = value;
        }
    }
    /// <summary>設定フラグ：ContactName_Like</summary>
    public bool IsSet_ContactName_Like = false;

    /// <summary>メンバ変数：ContactName_Like</summary>
    private System.String _ContactName_Like;

    /// <summary>プロパティ：ContactName_Like</summary>
    public System.String ContactName_Like
    {
        get
        {
            return this._ContactName_Like;
        }
        set
        {
            this.IsSet_ContactName_Like = true;
            this._ContactName_Like = value;
        }
    }
    /// <summary>設定フラグ：ContactTitle_Like</summary>
    public bool IsSet_ContactTitle_Like = false;

    /// <summary>メンバ変数：ContactTitle_Like</summary>
    private System.String _ContactTitle_Like;

    /// <summary>プロパティ：ContactTitle_Like</summary>
    public System.String ContactTitle_Like
    {
        get
        {
            return this._ContactTitle_Like;
        }
        set
        {
            this.IsSet_ContactTitle_Like = true;
            this._ContactTitle_Like = value;
        }
    }
    /// <summary>設定フラグ：Address_Like</summary>
    public bool IsSet_Address_Like = false;

    /// <summary>メンバ変数：Address_Like</summary>
    private System.String _Address_Like;

    /// <summary>プロパティ：Address_Like</summary>
    public System.String Address_Like
    {
        get
        {
            return this._Address_Like;
        }
        set
        {
            this.IsSet_Address_Like = true;
            this._Address_Like = value;
        }
    }
    /// <summary>設定フラグ：City_Like</summary>
    public bool IsSet_City_Like = false;

    /// <summary>メンバ変数：City_Like</summary>
    private System.String _City_Like;

    /// <summary>プロパティ：City_Like</summary>
    public System.String City_Like
    {
        get
        {
            return this._City_Like;
        }
        set
        {
            this.IsSet_City_Like = true;
            this._City_Like = value;
        }
    }
    /// <summary>設定フラグ：Region_Like</summary>
    public bool IsSet_Region_Like = false;

    /// <summary>メンバ変数：Region_Like</summary>
    private System.String _Region_Like;

    /// <summary>プロパティ：Region_Like</summary>
    public System.String Region_Like
    {
        get
        {
            return this._Region_Like;
        }
        set
        {
            this.IsSet_Region_Like = true;
            this._Region_Like = value;
        }
    }
    /// <summary>設定フラグ：PostalCode_Like</summary>
    public bool IsSet_PostalCode_Like = false;

    /// <summary>メンバ変数：PostalCode_Like</summary>
    private System.String _PostalCode_Like;

    /// <summary>プロパティ：PostalCode_Like</summary>
    public System.String PostalCode_Like
    {
        get
        {
            return this._PostalCode_Like;
        }
        set
        {
            this.IsSet_PostalCode_Like = true;
            this._PostalCode_Like = value;
        }
    }
    /// <summary>設定フラグ：Country_Like</summary>
    public bool IsSet_Country_Like = false;

    /// <summary>メンバ変数：Country_Like</summary>
    private System.String _Country_Like;

    /// <summary>プロパティ：Country_Like</summary>
    public System.String Country_Like
    {
        get
        {
            return this._Country_Like;
        }
        set
        {
            this.IsSet_Country_Like = true;
            this._Country_Like = value;
        }
    }
    /// <summary>設定フラグ：Phone_Like</summary>
    public bool IsSet_Phone_Like = false;

    /// <summary>メンバ変数：Phone_Like</summary>
    private System.String _Phone_Like;

    /// <summary>プロパティ：Phone_Like</summary>
    public System.String Phone_Like
    {
        get
        {
            return this._Phone_Like;
        }
        set
        {
            this.IsSet_Phone_Like = true;
            this._Phone_Like = value;
        }
    }
    /// <summary>設定フラグ：Fax_Like</summary>
    public bool IsSet_Fax_Like = false;

    /// <summary>メンバ変数：Fax_Like</summary>
    private System.String _Fax_Like;

    /// <summary>プロパティ：Fax_Like</summary>
    public System.String Fax_Like
    {
        get
        {
            return this._Fax_Like;
        }
        set
        {
            this.IsSet_Fax_Like = true;
            this._Fax_Like = value;
        }
    }
    /// <summary>設定フラグ：HomePage_Like</summary>
    public bool IsSet_HomePage_Like = false;

    /// <summary>メンバ変数：HomePage_Like</summary>
    private System.String _HomePage_Like;

    /// <summary>プロパティ：HomePage_Like</summary>
    public System.String HomePage_Like
    {
        get
        {
            return this._HomePage_Like;
        }
        set
        {
            this.IsSet_HomePage_Like = true;
            this._HomePage_Like = value;
        }
    }

    #endregion
}
