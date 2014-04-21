//**********************************************************************************
//* クラス名        ：EmployeesEntity
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
public class EmployeesEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：EmployeeID</summary>
    public bool IsSetPK_EmployeeID = false;

    /// <summary>メンバ変数：EmployeeID</summary>
    private System.Int32? _PK_EmployeeID;

    /// <summary>プロパティ：EmployeeID</summary>
    public System.Int32? PK_EmployeeID
    {
        get
        {
            return this._PK_EmployeeID;
        }
        set
        {
            this.IsSetPK_EmployeeID = true;
            this._PK_EmployeeID = value;
        }
    }

    /// <summary>設定フラグ：LastName</summary>
    public bool IsSet_LastName = false;

    /// <summary>メンバ変数：LastName</summary>
    private System.String _LastName;

    /// <summary>プロパティ：LastName</summary>
    public System.String LastName
    {
        get
        {
            return this._LastName;
        }
        set
        {
            this.IsSet_LastName = true;
            this._LastName = value;
        }
    }
    /// <summary>設定フラグ：FirstName</summary>
    public bool IsSet_FirstName = false;

    /// <summary>メンバ変数：FirstName</summary>
    private System.String _FirstName;

    /// <summary>プロパティ：FirstName</summary>
    public System.String FirstName
    {
        get
        {
            return this._FirstName;
        }
        set
        {
            this.IsSet_FirstName = true;
            this._FirstName = value;
        }
    }
    /// <summary>設定フラグ：Title</summary>
    public bool IsSet_Title = false;

    /// <summary>メンバ変数：Title</summary>
    private System.String _Title;

    /// <summary>プロパティ：Title</summary>
    public System.String Title
    {
        get
        {
            return this._Title;
        }
        set
        {
            this.IsSet_Title = true;
            this._Title = value;
        }
    }
    /// <summary>設定フラグ：TitleOfCourtesy</summary>
    public bool IsSet_TitleOfCourtesy = false;

    /// <summary>メンバ変数：TitleOfCourtesy</summary>
    private System.String _TitleOfCourtesy;

    /// <summary>プロパティ：TitleOfCourtesy</summary>
    public System.String TitleOfCourtesy
    {
        get
        {
            return this._TitleOfCourtesy;
        }
        set
        {
            this.IsSet_TitleOfCourtesy = true;
            this._TitleOfCourtesy = value;
        }
    }
    /// <summary>設定フラグ：BirthDate</summary>
    public bool IsSet_BirthDate = false;

    /// <summary>メンバ変数：BirthDate</summary>
    private System.DateTime? _BirthDate;

    /// <summary>プロパティ：BirthDate</summary>
    public System.DateTime? BirthDate
    {
        get
        {
            return this._BirthDate;
        }
        set
        {
            this.IsSet_BirthDate = true;
            this._BirthDate = value;
        }
    }
    /// <summary>設定フラグ：HireDate</summary>
    public bool IsSet_HireDate = false;

    /// <summary>メンバ変数：HireDate</summary>
    private System.DateTime? _HireDate;

    /// <summary>プロパティ：HireDate</summary>
    public System.DateTime? HireDate
    {
        get
        {
            return this._HireDate;
        }
        set
        {
            this.IsSet_HireDate = true;
            this._HireDate = value;
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
    /// <summary>設定フラグ：HomePhone</summary>
    public bool IsSet_HomePhone = false;

    /// <summary>メンバ変数：HomePhone</summary>
    private System.String _HomePhone;

    /// <summary>プロパティ：HomePhone</summary>
    public System.String HomePhone
    {
        get
        {
            return this._HomePhone;
        }
        set
        {
            this.IsSet_HomePhone = true;
            this._HomePhone = value;
        }
    }
    /// <summary>設定フラグ：Extension</summary>
    public bool IsSet_Extension = false;

    /// <summary>メンバ変数：Extension</summary>
    private System.String _Extension;

    /// <summary>プロパティ：Extension</summary>
    public System.String Extension
    {
        get
        {
            return this._Extension;
        }
        set
        {
            this.IsSet_Extension = true;
            this._Extension = value;
        }
    }
    /// <summary>設定フラグ：Photo</summary>
    public bool IsSet_Photo = false;

    /// <summary>メンバ変数：Photo</summary>
    private System.Byte[] _Photo;

    /// <summary>プロパティ：Photo</summary>
    public System.Byte[] Photo
    {
        get
        {
            return this._Photo;
        }
        set
        {
            this.IsSet_Photo = true;
            this._Photo = value;
        }
    }
    /// <summary>設定フラグ：Notes</summary>
    public bool IsSet_Notes = false;

    /// <summary>メンバ変数：Notes</summary>
    private System.String _Notes;

    /// <summary>プロパティ：Notes</summary>
    public System.String Notes
    {
        get
        {
            return this._Notes;
        }
        set
        {
            this.IsSet_Notes = true;
            this._Notes = value;
        }
    }
    /// <summary>設定フラグ：ReportsTo</summary>
    public bool IsSet_ReportsTo = false;

    /// <summary>メンバ変数：ReportsTo</summary>
    private System.Int32? _ReportsTo;

    /// <summary>プロパティ：ReportsTo</summary>
    public System.Int32? ReportsTo
    {
        get
        {
            return this._ReportsTo;
        }
        set
        {
            this.IsSet_ReportsTo = true;
            this._ReportsTo = value;
        }
    }
    /// <summary>設定フラグ：PhotoPath</summary>
    public bool IsSet_PhotoPath = false;

    /// <summary>メンバ変数：PhotoPath</summary>
    private System.String _PhotoPath;

    /// <summary>プロパティ：PhotoPath</summary>
    public System.String PhotoPath
    {
        get
        {
            return this._PhotoPath;
        }
        set
        {
            this.IsSet_PhotoPath = true;
            this._PhotoPath = value;
        }
    }

    /// <summary>設定フラグ：Set_EmployeeID_forUPD</summary>
    public bool IsSet_Set_EmployeeID_forUPD = false;

    /// <summary>メンバ変数：Set_EmployeeID_forUPD</summary>
    private System.Int32? _Set_EmployeeID_forUPD;

    /// <summary>プロパティ：Set_EmployeeID_forUPD</summary>
    public System.Int32? Set_EmployeeID_forUPD
    {
        get
        {
            return this._Set_EmployeeID_forUPD;
        }
        set
        {
            this.IsSet_Set_EmployeeID_forUPD = true;
            this._Set_EmployeeID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_LastName_forUPD</summary>
    public bool IsSet_Set_LastName_forUPD = false;

    /// <summary>メンバ変数：Set_LastName_forUPD</summary>
    private System.String _Set_LastName_forUPD;

    /// <summary>プロパティ：Set_LastName_forUPD</summary>
    public System.String Set_LastName_forUPD
    {
        get
        {
            return this._Set_LastName_forUPD;
        }
        set
        {
            this.IsSet_Set_LastName_forUPD = true;
            this._Set_LastName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_FirstName_forUPD</summary>
    public bool IsSet_Set_FirstName_forUPD = false;

    /// <summary>メンバ変数：Set_FirstName_forUPD</summary>
    private System.String _Set_FirstName_forUPD;

    /// <summary>プロパティ：Set_FirstName_forUPD</summary>
    public System.String Set_FirstName_forUPD
    {
        get
        {
            return this._Set_FirstName_forUPD;
        }
        set
        {
            this.IsSet_Set_FirstName_forUPD = true;
            this._Set_FirstName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Title_forUPD</summary>
    public bool IsSet_Set_Title_forUPD = false;

    /// <summary>メンバ変数：Set_Title_forUPD</summary>
    private System.String _Set_Title_forUPD;

    /// <summary>プロパティ：Set_Title_forUPD</summary>
    public System.String Set_Title_forUPD
    {
        get
        {
            return this._Set_Title_forUPD;
        }
        set
        {
            this.IsSet_Set_Title_forUPD = true;
            this._Set_Title_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_TitleOfCourtesy_forUPD</summary>
    public bool IsSet_Set_TitleOfCourtesy_forUPD = false;

    /// <summary>メンバ変数：Set_TitleOfCourtesy_forUPD</summary>
    private System.String _Set_TitleOfCourtesy_forUPD;

    /// <summary>プロパティ：Set_TitleOfCourtesy_forUPD</summary>
    public System.String Set_TitleOfCourtesy_forUPD
    {
        get
        {
            return this._Set_TitleOfCourtesy_forUPD;
        }
        set
        {
            this.IsSet_Set_TitleOfCourtesy_forUPD = true;
            this._Set_TitleOfCourtesy_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_BirthDate_forUPD</summary>
    public bool IsSet_Set_BirthDate_forUPD = false;

    /// <summary>メンバ変数：Set_BirthDate_forUPD</summary>
    private System.DateTime? _Set_BirthDate_forUPD;

    /// <summary>プロパティ：Set_BirthDate_forUPD</summary>
    public System.DateTime? Set_BirthDate_forUPD
    {
        get
        {
            return this._Set_BirthDate_forUPD;
        }
        set
        {
            this.IsSet_Set_BirthDate_forUPD = true;
            this._Set_BirthDate_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_HireDate_forUPD</summary>
    public bool IsSet_Set_HireDate_forUPD = false;

    /// <summary>メンバ変数：Set_HireDate_forUPD</summary>
    private System.DateTime? _Set_HireDate_forUPD;

    /// <summary>プロパティ：Set_HireDate_forUPD</summary>
    public System.DateTime? Set_HireDate_forUPD
    {
        get
        {
            return this._Set_HireDate_forUPD;
        }
        set
        {
            this.IsSet_Set_HireDate_forUPD = true;
            this._Set_HireDate_forUPD = value;
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
    /// <summary>設定フラグ：Set_HomePhone_forUPD</summary>
    public bool IsSet_Set_HomePhone_forUPD = false;

    /// <summary>メンバ変数：Set_HomePhone_forUPD</summary>
    private System.String _Set_HomePhone_forUPD;

    /// <summary>プロパティ：Set_HomePhone_forUPD</summary>
    public System.String Set_HomePhone_forUPD
    {
        get
        {
            return this._Set_HomePhone_forUPD;
        }
        set
        {
            this.IsSet_Set_HomePhone_forUPD = true;
            this._Set_HomePhone_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Extension_forUPD</summary>
    public bool IsSet_Set_Extension_forUPD = false;

    /// <summary>メンバ変数：Set_Extension_forUPD</summary>
    private System.String _Set_Extension_forUPD;

    /// <summary>プロパティ：Set_Extension_forUPD</summary>
    public System.String Set_Extension_forUPD
    {
        get
        {
            return this._Set_Extension_forUPD;
        }
        set
        {
            this.IsSet_Set_Extension_forUPD = true;
            this._Set_Extension_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Photo_forUPD</summary>
    public bool IsSet_Set_Photo_forUPD = false;

    /// <summary>メンバ変数：Set_Photo_forUPD</summary>
    private System.Byte[] _Set_Photo_forUPD;

    /// <summary>プロパティ：Set_Photo_forUPD</summary>
    public System.Byte[] Set_Photo_forUPD
    {
        get
        {
            return this._Set_Photo_forUPD;
        }
        set
        {
            this.IsSet_Set_Photo_forUPD = true;
            this._Set_Photo_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Notes_forUPD</summary>
    public bool IsSet_Set_Notes_forUPD = false;

    /// <summary>メンバ変数：Set_Notes_forUPD</summary>
    private System.String _Set_Notes_forUPD;

    /// <summary>プロパティ：Set_Notes_forUPD</summary>
    public System.String Set_Notes_forUPD
    {
        get
        {
            return this._Set_Notes_forUPD;
        }
        set
        {
            this.IsSet_Set_Notes_forUPD = true;
            this._Set_Notes_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ReportsTo_forUPD</summary>
    public bool IsSet_Set_ReportsTo_forUPD = false;

    /// <summary>メンバ変数：Set_ReportsTo_forUPD</summary>
    private System.Int32? _Set_ReportsTo_forUPD;

    /// <summary>プロパティ：Set_ReportsTo_forUPD</summary>
    public System.Int32? Set_ReportsTo_forUPD
    {
        get
        {
            return this._Set_ReportsTo_forUPD;
        }
        set
        {
            this.IsSet_Set_ReportsTo_forUPD = true;
            this._Set_ReportsTo_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_PhotoPath_forUPD</summary>
    public bool IsSet_Set_PhotoPath_forUPD = false;

    /// <summary>メンバ変数：Set_PhotoPath_forUPD</summary>
    private System.String _Set_PhotoPath_forUPD;

    /// <summary>プロパティ：Set_PhotoPath_forUPD</summary>
    public System.String Set_PhotoPath_forUPD
    {
        get
        {
            return this._Set_PhotoPath_forUPD;
        }
        set
        {
            this.IsSet_Set_PhotoPath_forUPD = true;
            this._Set_PhotoPath_forUPD = value;
        }
    }

    /// <summary>設定フラグ：EmployeeID_Like</summary>
    public bool IsSet_EmployeeID_Like = false;

    /// <summary>メンバ変数：EmployeeID_Like</summary>
    private System.Int32? _EmployeeID_Like;

    /// <summary>プロパティ：EmployeeID_Like</summary>
    public System.Int32? EmployeeID_Like
    {
        get
        {
            return this._EmployeeID_Like;
        }
        set
        {
            this.IsSet_EmployeeID_Like = true;
            this._EmployeeID_Like = value;
        }
    }
    /// <summary>設定フラグ：LastName_Like</summary>
    public bool IsSet_LastName_Like = false;

    /// <summary>メンバ変数：LastName_Like</summary>
    private System.String _LastName_Like;

    /// <summary>プロパティ：LastName_Like</summary>
    public System.String LastName_Like
    {
        get
        {
            return this._LastName_Like;
        }
        set
        {
            this.IsSet_LastName_Like = true;
            this._LastName_Like = value;
        }
    }
    /// <summary>設定フラグ：FirstName_Like</summary>
    public bool IsSet_FirstName_Like = false;

    /// <summary>メンバ変数：FirstName_Like</summary>
    private System.String _FirstName_Like;

    /// <summary>プロパティ：FirstName_Like</summary>
    public System.String FirstName_Like
    {
        get
        {
            return this._FirstName_Like;
        }
        set
        {
            this.IsSet_FirstName_Like = true;
            this._FirstName_Like = value;
        }
    }
    /// <summary>設定フラグ：Title_Like</summary>
    public bool IsSet_Title_Like = false;

    /// <summary>メンバ変数：Title_Like</summary>
    private System.String _Title_Like;

    /// <summary>プロパティ：Title_Like</summary>
    public System.String Title_Like
    {
        get
        {
            return this._Title_Like;
        }
        set
        {
            this.IsSet_Title_Like = true;
            this._Title_Like = value;
        }
    }
    /// <summary>設定フラグ：TitleOfCourtesy_Like</summary>
    public bool IsSet_TitleOfCourtesy_Like = false;

    /// <summary>メンバ変数：TitleOfCourtesy_Like</summary>
    private System.String _TitleOfCourtesy_Like;

    /// <summary>プロパティ：TitleOfCourtesy_Like</summary>
    public System.String TitleOfCourtesy_Like
    {
        get
        {
            return this._TitleOfCourtesy_Like;
        }
        set
        {
            this.IsSet_TitleOfCourtesy_Like = true;
            this._TitleOfCourtesy_Like = value;
        }
    }
    /// <summary>設定フラグ：BirthDate_Like</summary>
    public bool IsSet_BirthDate_Like = false;

    /// <summary>メンバ変数：BirthDate_Like</summary>
    private System.DateTime? _BirthDate_Like;

    /// <summary>プロパティ：BirthDate_Like</summary>
    public System.DateTime? BirthDate_Like
    {
        get
        {
            return this._BirthDate_Like;
        }
        set
        {
            this.IsSet_BirthDate_Like = true;
            this._BirthDate_Like = value;
        }
    }
    /// <summary>設定フラグ：HireDate_Like</summary>
    public bool IsSet_HireDate_Like = false;

    /// <summary>メンバ変数：HireDate_Like</summary>
    private System.DateTime? _HireDate_Like;

    /// <summary>プロパティ：HireDate_Like</summary>
    public System.DateTime? HireDate_Like
    {
        get
        {
            return this._HireDate_Like;
        }
        set
        {
            this.IsSet_HireDate_Like = true;
            this._HireDate_Like = value;
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
    /// <summary>設定フラグ：HomePhone_Like</summary>
    public bool IsSet_HomePhone_Like = false;

    /// <summary>メンバ変数：HomePhone_Like</summary>
    private System.String _HomePhone_Like;

    /// <summary>プロパティ：HomePhone_Like</summary>
    public System.String HomePhone_Like
    {
        get
        {
            return this._HomePhone_Like;
        }
        set
        {
            this.IsSet_HomePhone_Like = true;
            this._HomePhone_Like = value;
        }
    }
    /// <summary>設定フラグ：Extension_Like</summary>
    public bool IsSet_Extension_Like = false;

    /// <summary>メンバ変数：Extension_Like</summary>
    private System.String _Extension_Like;

    /// <summary>プロパティ：Extension_Like</summary>
    public System.String Extension_Like
    {
        get
        {
            return this._Extension_Like;
        }
        set
        {
            this.IsSet_Extension_Like = true;
            this._Extension_Like = value;
        }
    }
    /// <summary>設定フラグ：Photo_Like</summary>
    public bool IsSet_Photo_Like = false;

    /// <summary>メンバ変数：Photo_Like</summary>
    private System.Byte[] _Photo_Like;

    /// <summary>プロパティ：Photo_Like</summary>
    public System.Byte[] Photo_Like
    {
        get
        {
            return this._Photo_Like;
        }
        set
        {
            this.IsSet_Photo_Like = true;
            this._Photo_Like = value;
        }
    }
    /// <summary>設定フラグ：Notes_Like</summary>
    public bool IsSet_Notes_Like = false;

    /// <summary>メンバ変数：Notes_Like</summary>
    private System.String _Notes_Like;

    /// <summary>プロパティ：Notes_Like</summary>
    public System.String Notes_Like
    {
        get
        {
            return this._Notes_Like;
        }
        set
        {
            this.IsSet_Notes_Like = true;
            this._Notes_Like = value;
        }
    }
    /// <summary>設定フラグ：ReportsTo_Like</summary>
    public bool IsSet_ReportsTo_Like = false;

    /// <summary>メンバ変数：ReportsTo_Like</summary>
    private System.Int32? _ReportsTo_Like;

    /// <summary>プロパティ：ReportsTo_Like</summary>
    public System.Int32? ReportsTo_Like
    {
        get
        {
            return this._ReportsTo_Like;
        }
        set
        {
            this.IsSet_ReportsTo_Like = true;
            this._ReportsTo_Like = value;
        }
    }
    /// <summary>設定フラグ：PhotoPath_Like</summary>
    public bool IsSet_PhotoPath_Like = false;

    /// <summary>メンバ変数：PhotoPath_Like</summary>
    private System.String _PhotoPath_Like;

    /// <summary>プロパティ：PhotoPath_Like</summary>
    public System.String PhotoPath_Like
    {
        get
        {
            return this._PhotoPath_Like;
        }
        set
        {
            this.IsSet_PhotoPath_Like = true;
            this._PhotoPath_Like = value;
        }
    }

    #endregion
}
