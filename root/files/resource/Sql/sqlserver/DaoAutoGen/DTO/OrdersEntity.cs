//**********************************************************************************
//* クラス名        ：OrdersEntity
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
public class OrdersEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：OrderID</summary>
    public bool IsSetPK_OrderID = false;

    /// <summary>メンバ変数：OrderID</summary>
    private System.Int32? _PK_OrderID;

    /// <summary>プロパティ：OrderID</summary>
    public System.Int32? PK_OrderID
    {
        get
        {
            return this._PK_OrderID;
        }
        set
        {
            this.IsSetPK_OrderID = true;
            this._PK_OrderID = value;
        }
    }

    /// <summary>設定フラグ：CustomerID</summary>
    public bool IsSet_CustomerID = false;

    /// <summary>メンバ変数：CustomerID</summary>
    private System.String _CustomerID;

    /// <summary>プロパティ：CustomerID</summary>
    public System.String CustomerID
    {
        get
        {
            return this._CustomerID;
        }
        set
        {
            this.IsSet_CustomerID = true;
            this._CustomerID = value;
        }
    }
    /// <summary>設定フラグ：EmployeeID</summary>
    public bool IsSet_EmployeeID = false;

    /// <summary>メンバ変数：EmployeeID</summary>
    private System.Int32? _EmployeeID;

    /// <summary>プロパティ：EmployeeID</summary>
    public System.Int32? EmployeeID
    {
        get
        {
            return this._EmployeeID;
        }
        set
        {
            this.IsSet_EmployeeID = true;
            this._EmployeeID = value;
        }
    }
    /// <summary>設定フラグ：OrderDate</summary>
    public bool IsSet_OrderDate = false;

    /// <summary>メンバ変数：OrderDate</summary>
    private System.DateTime? _OrderDate;

    /// <summary>プロパティ：OrderDate</summary>
    public System.DateTime? OrderDate
    {
        get
        {
            return this._OrderDate;
        }
        set
        {
            this.IsSet_OrderDate = true;
            this._OrderDate = value;
        }
    }
    /// <summary>設定フラグ：RequiredDate</summary>
    public bool IsSet_RequiredDate = false;

    /// <summary>メンバ変数：RequiredDate</summary>
    private System.DateTime? _RequiredDate;

    /// <summary>プロパティ：RequiredDate</summary>
    public System.DateTime? RequiredDate
    {
        get
        {
            return this._RequiredDate;
        }
        set
        {
            this.IsSet_RequiredDate = true;
            this._RequiredDate = value;
        }
    }
    /// <summary>設定フラグ：ShippedDate</summary>
    public bool IsSet_ShippedDate = false;

    /// <summary>メンバ変数：ShippedDate</summary>
    private System.DateTime? _ShippedDate;

    /// <summary>プロパティ：ShippedDate</summary>
    public System.DateTime? ShippedDate
    {
        get
        {
            return this._ShippedDate;
        }
        set
        {
            this.IsSet_ShippedDate = true;
            this._ShippedDate = value;
        }
    }
    /// <summary>設定フラグ：ShipVia</summary>
    public bool IsSet_ShipVia = false;

    /// <summary>メンバ変数：ShipVia</summary>
    private System.Int32? _ShipVia;

    /// <summary>プロパティ：ShipVia</summary>
    public System.Int32? ShipVia
    {
        get
        {
            return this._ShipVia;
        }
        set
        {
            this.IsSet_ShipVia = true;
            this._ShipVia = value;
        }
    }
    /// <summary>設定フラグ：Freight</summary>
    public bool IsSet_Freight = false;

    /// <summary>メンバ変数：Freight</summary>
    private System.Decimal? _Freight;

    /// <summary>プロパティ：Freight</summary>
    public System.Decimal? Freight
    {
        get
        {
            return this._Freight;
        }
        set
        {
            this.IsSet_Freight = true;
            this._Freight = value;
        }
    }
    /// <summary>設定フラグ：ShipName</summary>
    public bool IsSet_ShipName = false;

    /// <summary>メンバ変数：ShipName</summary>
    private System.String _ShipName;

    /// <summary>プロパティ：ShipName</summary>
    public System.String ShipName
    {
        get
        {
            return this._ShipName;
        }
        set
        {
            this.IsSet_ShipName = true;
            this._ShipName = value;
        }
    }
    /// <summary>設定フラグ：ShipAddress</summary>
    public bool IsSet_ShipAddress = false;

    /// <summary>メンバ変数：ShipAddress</summary>
    private System.String _ShipAddress;

    /// <summary>プロパティ：ShipAddress</summary>
    public System.String ShipAddress
    {
        get
        {
            return this._ShipAddress;
        }
        set
        {
            this.IsSet_ShipAddress = true;
            this._ShipAddress = value;
        }
    }
    /// <summary>設定フラグ：ShipCity</summary>
    public bool IsSet_ShipCity = false;

    /// <summary>メンバ変数：ShipCity</summary>
    private System.String _ShipCity;

    /// <summary>プロパティ：ShipCity</summary>
    public System.String ShipCity
    {
        get
        {
            return this._ShipCity;
        }
        set
        {
            this.IsSet_ShipCity = true;
            this._ShipCity = value;
        }
    }
    /// <summary>設定フラグ：ShipRegion</summary>
    public bool IsSet_ShipRegion = false;

    /// <summary>メンバ変数：ShipRegion</summary>
    private System.String _ShipRegion;

    /// <summary>プロパティ：ShipRegion</summary>
    public System.String ShipRegion
    {
        get
        {
            return this._ShipRegion;
        }
        set
        {
            this.IsSet_ShipRegion = true;
            this._ShipRegion = value;
        }
    }
    /// <summary>設定フラグ：ShipPostalCode</summary>
    public bool IsSet_ShipPostalCode = false;

    /// <summary>メンバ変数：ShipPostalCode</summary>
    private System.String _ShipPostalCode;

    /// <summary>プロパティ：ShipPostalCode</summary>
    public System.String ShipPostalCode
    {
        get
        {
            return this._ShipPostalCode;
        }
        set
        {
            this.IsSet_ShipPostalCode = true;
            this._ShipPostalCode = value;
        }
    }
    /// <summary>設定フラグ：ShipCountry</summary>
    public bool IsSet_ShipCountry = false;

    /// <summary>メンバ変数：ShipCountry</summary>
    private System.String _ShipCountry;

    /// <summary>プロパティ：ShipCountry</summary>
    public System.String ShipCountry
    {
        get
        {
            return this._ShipCountry;
        }
        set
        {
            this.IsSet_ShipCountry = true;
            this._ShipCountry = value;
        }
    }

    /// <summary>設定フラグ：Set_OrderID_forUPD</summary>
    public bool IsSet_Set_OrderID_forUPD = false;

    /// <summary>メンバ変数：Set_OrderID_forUPD</summary>
    private System.Int32? _Set_OrderID_forUPD;

    /// <summary>プロパティ：Set_OrderID_forUPD</summary>
    public System.Int32? Set_OrderID_forUPD
    {
        get
        {
            return this._Set_OrderID_forUPD;
        }
        set
        {
            this.IsSet_Set_OrderID_forUPD = true;
            this._Set_OrderID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_CustomerID_forUPD</summary>
    public bool IsSet_Set_CustomerID_forUPD = false;

    /// <summary>メンバ変数：Set_CustomerID_forUPD</summary>
    private System.String _Set_CustomerID_forUPD;

    /// <summary>プロパティ：Set_CustomerID_forUPD</summary>
    public System.String Set_CustomerID_forUPD
    {
        get
        {
            return this._Set_CustomerID_forUPD;
        }
        set
        {
            this.IsSet_Set_CustomerID_forUPD = true;
            this._Set_CustomerID_forUPD = value;
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
    /// <summary>設定フラグ：Set_OrderDate_forUPD</summary>
    public bool IsSet_Set_OrderDate_forUPD = false;

    /// <summary>メンバ変数：Set_OrderDate_forUPD</summary>
    private System.DateTime? _Set_OrderDate_forUPD;

    /// <summary>プロパティ：Set_OrderDate_forUPD</summary>
    public System.DateTime? Set_OrderDate_forUPD
    {
        get
        {
            return this._Set_OrderDate_forUPD;
        }
        set
        {
            this.IsSet_Set_OrderDate_forUPD = true;
            this._Set_OrderDate_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_RequiredDate_forUPD</summary>
    public bool IsSet_Set_RequiredDate_forUPD = false;

    /// <summary>メンバ変数：Set_RequiredDate_forUPD</summary>
    private System.DateTime? _Set_RequiredDate_forUPD;

    /// <summary>プロパティ：Set_RequiredDate_forUPD</summary>
    public System.DateTime? Set_RequiredDate_forUPD
    {
        get
        {
            return this._Set_RequiredDate_forUPD;
        }
        set
        {
            this.IsSet_Set_RequiredDate_forUPD = true;
            this._Set_RequiredDate_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShippedDate_forUPD</summary>
    public bool IsSet_Set_ShippedDate_forUPD = false;

    /// <summary>メンバ変数：Set_ShippedDate_forUPD</summary>
    private System.DateTime? _Set_ShippedDate_forUPD;

    /// <summary>プロパティ：Set_ShippedDate_forUPD</summary>
    public System.DateTime? Set_ShippedDate_forUPD
    {
        get
        {
            return this._Set_ShippedDate_forUPD;
        }
        set
        {
            this.IsSet_Set_ShippedDate_forUPD = true;
            this._Set_ShippedDate_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipVia_forUPD</summary>
    public bool IsSet_Set_ShipVia_forUPD = false;

    /// <summary>メンバ変数：Set_ShipVia_forUPD</summary>
    private System.Int32? _Set_ShipVia_forUPD;

    /// <summary>プロパティ：Set_ShipVia_forUPD</summary>
    public System.Int32? Set_ShipVia_forUPD
    {
        get
        {
            return this._Set_ShipVia_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipVia_forUPD = true;
            this._Set_ShipVia_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Freight_forUPD</summary>
    public bool IsSet_Set_Freight_forUPD = false;

    /// <summary>メンバ変数：Set_Freight_forUPD</summary>
    private System.Decimal? _Set_Freight_forUPD;

    /// <summary>プロパティ：Set_Freight_forUPD</summary>
    public System.Decimal? Set_Freight_forUPD
    {
        get
        {
            return this._Set_Freight_forUPD;
        }
        set
        {
            this.IsSet_Set_Freight_forUPD = true;
            this._Set_Freight_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipName_forUPD</summary>
    public bool IsSet_Set_ShipName_forUPD = false;

    /// <summary>メンバ変数：Set_ShipName_forUPD</summary>
    private System.String _Set_ShipName_forUPD;

    /// <summary>プロパティ：Set_ShipName_forUPD</summary>
    public System.String Set_ShipName_forUPD
    {
        get
        {
            return this._Set_ShipName_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipName_forUPD = true;
            this._Set_ShipName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipAddress_forUPD</summary>
    public bool IsSet_Set_ShipAddress_forUPD = false;

    /// <summary>メンバ変数：Set_ShipAddress_forUPD</summary>
    private System.String _Set_ShipAddress_forUPD;

    /// <summary>プロパティ：Set_ShipAddress_forUPD</summary>
    public System.String Set_ShipAddress_forUPD
    {
        get
        {
            return this._Set_ShipAddress_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipAddress_forUPD = true;
            this._Set_ShipAddress_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipCity_forUPD</summary>
    public bool IsSet_Set_ShipCity_forUPD = false;

    /// <summary>メンバ変数：Set_ShipCity_forUPD</summary>
    private System.String _Set_ShipCity_forUPD;

    /// <summary>プロパティ：Set_ShipCity_forUPD</summary>
    public System.String Set_ShipCity_forUPD
    {
        get
        {
            return this._Set_ShipCity_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipCity_forUPD = true;
            this._Set_ShipCity_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipRegion_forUPD</summary>
    public bool IsSet_Set_ShipRegion_forUPD = false;

    /// <summary>メンバ変数：Set_ShipRegion_forUPD</summary>
    private System.String _Set_ShipRegion_forUPD;

    /// <summary>プロパティ：Set_ShipRegion_forUPD</summary>
    public System.String Set_ShipRegion_forUPD
    {
        get
        {
            return this._Set_ShipRegion_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipRegion_forUPD = true;
            this._Set_ShipRegion_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipPostalCode_forUPD</summary>
    public bool IsSet_Set_ShipPostalCode_forUPD = false;

    /// <summary>メンバ変数：Set_ShipPostalCode_forUPD</summary>
    private System.String _Set_ShipPostalCode_forUPD;

    /// <summary>プロパティ：Set_ShipPostalCode_forUPD</summary>
    public System.String Set_ShipPostalCode_forUPD
    {
        get
        {
            return this._Set_ShipPostalCode_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipPostalCode_forUPD = true;
            this._Set_ShipPostalCode_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ShipCountry_forUPD</summary>
    public bool IsSet_Set_ShipCountry_forUPD = false;

    /// <summary>メンバ変数：Set_ShipCountry_forUPD</summary>
    private System.String _Set_ShipCountry_forUPD;

    /// <summary>プロパティ：Set_ShipCountry_forUPD</summary>
    public System.String Set_ShipCountry_forUPD
    {
        get
        {
            return this._Set_ShipCountry_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipCountry_forUPD = true;
            this._Set_ShipCountry_forUPD = value;
        }
    }

    /// <summary>設定フラグ：OrderID_Like</summary>
    public bool IsSet_OrderID_Like = false;

    /// <summary>メンバ変数：OrderID_Like</summary>
    private System.Int32? _OrderID_Like;

    /// <summary>プロパティ：OrderID_Like</summary>
    public System.Int32? OrderID_Like
    {
        get
        {
            return this._OrderID_Like;
        }
        set
        {
            this.IsSet_OrderID_Like = true;
            this._OrderID_Like = value;
        }
    }
    /// <summary>設定フラグ：CustomerID_Like</summary>
    public bool IsSet_CustomerID_Like = false;

    /// <summary>メンバ変数：CustomerID_Like</summary>
    private System.String _CustomerID_Like;

    /// <summary>プロパティ：CustomerID_Like</summary>
    public System.String CustomerID_Like
    {
        get
        {
            return this._CustomerID_Like;
        }
        set
        {
            this.IsSet_CustomerID_Like = true;
            this._CustomerID_Like = value;
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
    /// <summary>設定フラグ：OrderDate_Like</summary>
    public bool IsSet_OrderDate_Like = false;

    /// <summary>メンバ変数：OrderDate_Like</summary>
    private System.DateTime? _OrderDate_Like;

    /// <summary>プロパティ：OrderDate_Like</summary>
    public System.DateTime? OrderDate_Like
    {
        get
        {
            return this._OrderDate_Like;
        }
        set
        {
            this.IsSet_OrderDate_Like = true;
            this._OrderDate_Like = value;
        }
    }
    /// <summary>設定フラグ：RequiredDate_Like</summary>
    public bool IsSet_RequiredDate_Like = false;

    /// <summary>メンバ変数：RequiredDate_Like</summary>
    private System.DateTime? _RequiredDate_Like;

    /// <summary>プロパティ：RequiredDate_Like</summary>
    public System.DateTime? RequiredDate_Like
    {
        get
        {
            return this._RequiredDate_Like;
        }
        set
        {
            this.IsSet_RequiredDate_Like = true;
            this._RequiredDate_Like = value;
        }
    }
    /// <summary>設定フラグ：ShippedDate_Like</summary>
    public bool IsSet_ShippedDate_Like = false;

    /// <summary>メンバ変数：ShippedDate_Like</summary>
    private System.DateTime? _ShippedDate_Like;

    /// <summary>プロパティ：ShippedDate_Like</summary>
    public System.DateTime? ShippedDate_Like
    {
        get
        {
            return this._ShippedDate_Like;
        }
        set
        {
            this.IsSet_ShippedDate_Like = true;
            this._ShippedDate_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipVia_Like</summary>
    public bool IsSet_ShipVia_Like = false;

    /// <summary>メンバ変数：ShipVia_Like</summary>
    private System.Int32? _ShipVia_Like;

    /// <summary>プロパティ：ShipVia_Like</summary>
    public System.Int32? ShipVia_Like
    {
        get
        {
            return this._ShipVia_Like;
        }
        set
        {
            this.IsSet_ShipVia_Like = true;
            this._ShipVia_Like = value;
        }
    }
    /// <summary>設定フラグ：Freight_Like</summary>
    public bool IsSet_Freight_Like = false;

    /// <summary>メンバ変数：Freight_Like</summary>
    private System.Decimal? _Freight_Like;

    /// <summary>プロパティ：Freight_Like</summary>
    public System.Decimal? Freight_Like
    {
        get
        {
            return this._Freight_Like;
        }
        set
        {
            this.IsSet_Freight_Like = true;
            this._Freight_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipName_Like</summary>
    public bool IsSet_ShipName_Like = false;

    /// <summary>メンバ変数：ShipName_Like</summary>
    private System.String _ShipName_Like;

    /// <summary>プロパティ：ShipName_Like</summary>
    public System.String ShipName_Like
    {
        get
        {
            return this._ShipName_Like;
        }
        set
        {
            this.IsSet_ShipName_Like = true;
            this._ShipName_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipAddress_Like</summary>
    public bool IsSet_ShipAddress_Like = false;

    /// <summary>メンバ変数：ShipAddress_Like</summary>
    private System.String _ShipAddress_Like;

    /// <summary>プロパティ：ShipAddress_Like</summary>
    public System.String ShipAddress_Like
    {
        get
        {
            return this._ShipAddress_Like;
        }
        set
        {
            this.IsSet_ShipAddress_Like = true;
            this._ShipAddress_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipCity_Like</summary>
    public bool IsSet_ShipCity_Like = false;

    /// <summary>メンバ変数：ShipCity_Like</summary>
    private System.String _ShipCity_Like;

    /// <summary>プロパティ：ShipCity_Like</summary>
    public System.String ShipCity_Like
    {
        get
        {
            return this._ShipCity_Like;
        }
        set
        {
            this.IsSet_ShipCity_Like = true;
            this._ShipCity_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipRegion_Like</summary>
    public bool IsSet_ShipRegion_Like = false;

    /// <summary>メンバ変数：ShipRegion_Like</summary>
    private System.String _ShipRegion_Like;

    /// <summary>プロパティ：ShipRegion_Like</summary>
    public System.String ShipRegion_Like
    {
        get
        {
            return this._ShipRegion_Like;
        }
        set
        {
            this.IsSet_ShipRegion_Like = true;
            this._ShipRegion_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipPostalCode_Like</summary>
    public bool IsSet_ShipPostalCode_Like = false;

    /// <summary>メンバ変数：ShipPostalCode_Like</summary>
    private System.String _ShipPostalCode_Like;

    /// <summary>プロパティ：ShipPostalCode_Like</summary>
    public System.String ShipPostalCode_Like
    {
        get
        {
            return this._ShipPostalCode_Like;
        }
        set
        {
            this.IsSet_ShipPostalCode_Like = true;
            this._ShipPostalCode_Like = value;
        }
    }
    /// <summary>設定フラグ：ShipCountry_Like</summary>
    public bool IsSet_ShipCountry_Like = false;

    /// <summary>メンバ変数：ShipCountry_Like</summary>
    private System.String _ShipCountry_Like;

    /// <summary>プロパティ：ShipCountry_Like</summary>
    public System.String ShipCountry_Like
    {
        get
        {
            return this._ShipCountry_Like;
        }
        set
        {
            this.IsSet_ShipCountry_Like = true;
            this._ShipCountry_Like = value;
        }
    }

    #endregion
}
