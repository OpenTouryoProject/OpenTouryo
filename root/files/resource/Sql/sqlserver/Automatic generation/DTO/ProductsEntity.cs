//**********************************************************************************
//* クラス名        ：ProductsEntity
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
public class ProductsEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：ProductID</summary>
    public bool IsSetPK_ProductID = false;

    /// <summary>メンバ変数：ProductID</summary>
    private System.Int32? _PK_ProductID;

    /// <summary>プロパティ：ProductID</summary>
    public System.Int32? PK_ProductID
    {
        get
        {
            return this._PK_ProductID;
        }
        set
        {
            this.IsSetPK_ProductID = true;
            this._PK_ProductID = value;
        }
    }

    /// <summary>設定フラグ：ProductName</summary>
    public bool IsSet_ProductName = false;

    /// <summary>メンバ変数：ProductName</summary>
    private System.String _ProductName;

    /// <summary>プロパティ：ProductName</summary>
    public System.String ProductName
    {
        get
        {
            return this._ProductName;
        }
        set
        {
            this.IsSet_ProductName = true;
            this._ProductName = value;
        }
    }
    /// <summary>設定フラグ：SupplierID</summary>
    public bool IsSet_SupplierID = false;

    /// <summary>メンバ変数：SupplierID</summary>
    private System.Int32? _SupplierID;

    /// <summary>プロパティ：SupplierID</summary>
    public System.Int32? SupplierID
    {
        get
        {
            return this._SupplierID;
        }
        set
        {
            this.IsSet_SupplierID = true;
            this._SupplierID = value;
        }
    }
    /// <summary>設定フラグ：CategoryID</summary>
    public bool IsSet_CategoryID = false;

    /// <summary>メンバ変数：CategoryID</summary>
    private System.Int32? _CategoryID;

    /// <summary>プロパティ：CategoryID</summary>
    public System.Int32? CategoryID
    {
        get
        {
            return this._CategoryID;
        }
        set
        {
            this.IsSet_CategoryID = true;
            this._CategoryID = value;
        }
    }
    /// <summary>設定フラグ：QuantityPerUnit</summary>
    public bool IsSet_QuantityPerUnit = false;

    /// <summary>メンバ変数：QuantityPerUnit</summary>
    private System.String _QuantityPerUnit;

    /// <summary>プロパティ：QuantityPerUnit</summary>
    public System.String QuantityPerUnit
    {
        get
        {
            return this._QuantityPerUnit;
        }
        set
        {
            this.IsSet_QuantityPerUnit = true;
            this._QuantityPerUnit = value;
        }
    }
    /// <summary>設定フラグ：UnitPrice</summary>
    public bool IsSet_UnitPrice = false;

    /// <summary>メンバ変数：UnitPrice</summary>
    private System.Decimal? _UnitPrice;

    /// <summary>プロパティ：UnitPrice</summary>
    public System.Decimal? UnitPrice
    {
        get
        {
            return this._UnitPrice;
        }
        set
        {
            this.IsSet_UnitPrice = true;
            this._UnitPrice = value;
        }
    }
    /// <summary>設定フラグ：UnitsInStock</summary>
    public bool IsSet_UnitsInStock = false;

    /// <summary>メンバ変数：UnitsInStock</summary>
    private System.Int16? _UnitsInStock;

    /// <summary>プロパティ：UnitsInStock</summary>
    public System.Int16? UnitsInStock
    {
        get
        {
            return this._UnitsInStock;
        }
        set
        {
            this.IsSet_UnitsInStock = true;
            this._UnitsInStock = value;
        }
    }
    /// <summary>設定フラグ：UnitsOnOrder</summary>
    public bool IsSet_UnitsOnOrder = false;

    /// <summary>メンバ変数：UnitsOnOrder</summary>
    private System.Int16? _UnitsOnOrder;

    /// <summary>プロパティ：UnitsOnOrder</summary>
    public System.Int16? UnitsOnOrder
    {
        get
        {
            return this._UnitsOnOrder;
        }
        set
        {
            this.IsSet_UnitsOnOrder = true;
            this._UnitsOnOrder = value;
        }
    }
    /// <summary>設定フラグ：ReorderLevel</summary>
    public bool IsSet_ReorderLevel = false;

    /// <summary>メンバ変数：ReorderLevel</summary>
    private System.Int16? _ReorderLevel;

    /// <summary>プロパティ：ReorderLevel</summary>
    public System.Int16? ReorderLevel
    {
        get
        {
            return this._ReorderLevel;
        }
        set
        {
            this.IsSet_ReorderLevel = true;
            this._ReorderLevel = value;
        }
    }
    /// <summary>設定フラグ：Discontinued</summary>
    public bool IsSet_Discontinued = false;

    /// <summary>メンバ変数：Discontinued</summary>
    private System.Boolean? _Discontinued;

    /// <summary>プロパティ：Discontinued</summary>
    public System.Boolean? Discontinued
    {
        get
        {
            return this._Discontinued;
        }
        set
        {
            this.IsSet_Discontinued = true;
            this._Discontinued = value;
        }
    }

    /// <summary>設定フラグ：Set_ProductID_forUPD</summary>
    public bool IsSet_Set_ProductID_forUPD = false;

    /// <summary>メンバ変数：Set_ProductID_forUPD</summary>
    private System.Int32? _Set_ProductID_forUPD;

    /// <summary>プロパティ：Set_ProductID_forUPD</summary>
    public System.Int32? Set_ProductID_forUPD
    {
        get
        {
            return this._Set_ProductID_forUPD;
        }
        set
        {
            this.IsSet_Set_ProductID_forUPD = true;
            this._Set_ProductID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ProductName_forUPD</summary>
    public bool IsSet_Set_ProductName_forUPD = false;

    /// <summary>メンバ変数：Set_ProductName_forUPD</summary>
    private System.String _Set_ProductName_forUPD;

    /// <summary>プロパティ：Set_ProductName_forUPD</summary>
    public System.String Set_ProductName_forUPD
    {
        get
        {
            return this._Set_ProductName_forUPD;
        }
        set
        {
            this.IsSet_Set_ProductName_forUPD = true;
            this._Set_ProductName_forUPD = value;
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
    /// <summary>設定フラグ：Set_CategoryID_forUPD</summary>
    public bool IsSet_Set_CategoryID_forUPD = false;

    /// <summary>メンバ変数：Set_CategoryID_forUPD</summary>
    private System.Int32? _Set_CategoryID_forUPD;

    /// <summary>プロパティ：Set_CategoryID_forUPD</summary>
    public System.Int32? Set_CategoryID_forUPD
    {
        get
        {
            return this._Set_CategoryID_forUPD;
        }
        set
        {
            this.IsSet_Set_CategoryID_forUPD = true;
            this._Set_CategoryID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_QuantityPerUnit_forUPD</summary>
    public bool IsSet_Set_QuantityPerUnit_forUPD = false;

    /// <summary>メンバ変数：Set_QuantityPerUnit_forUPD</summary>
    private System.String _Set_QuantityPerUnit_forUPD;

    /// <summary>プロパティ：Set_QuantityPerUnit_forUPD</summary>
    public System.String Set_QuantityPerUnit_forUPD
    {
        get
        {
            return this._Set_QuantityPerUnit_forUPD;
        }
        set
        {
            this.IsSet_Set_QuantityPerUnit_forUPD = true;
            this._Set_QuantityPerUnit_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_UnitPrice_forUPD</summary>
    public bool IsSet_Set_UnitPrice_forUPD = false;

    /// <summary>メンバ変数：Set_UnitPrice_forUPD</summary>
    private System.Decimal? _Set_UnitPrice_forUPD;

    /// <summary>プロパティ：Set_UnitPrice_forUPD</summary>
    public System.Decimal? Set_UnitPrice_forUPD
    {
        get
        {
            return this._Set_UnitPrice_forUPD;
        }
        set
        {
            this.IsSet_Set_UnitPrice_forUPD = true;
            this._Set_UnitPrice_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_UnitsInStock_forUPD</summary>
    public bool IsSet_Set_UnitsInStock_forUPD = false;

    /// <summary>メンバ変数：Set_UnitsInStock_forUPD</summary>
    private System.Int16? _Set_UnitsInStock_forUPD;

    /// <summary>プロパティ：Set_UnitsInStock_forUPD</summary>
    public System.Int16? Set_UnitsInStock_forUPD
    {
        get
        {
            return this._Set_UnitsInStock_forUPD;
        }
        set
        {
            this.IsSet_Set_UnitsInStock_forUPD = true;
            this._Set_UnitsInStock_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_UnitsOnOrder_forUPD</summary>
    public bool IsSet_Set_UnitsOnOrder_forUPD = false;

    /// <summary>メンバ変数：Set_UnitsOnOrder_forUPD</summary>
    private System.Int16? _Set_UnitsOnOrder_forUPD;

    /// <summary>プロパティ：Set_UnitsOnOrder_forUPD</summary>
    public System.Int16? Set_UnitsOnOrder_forUPD
    {
        get
        {
            return this._Set_UnitsOnOrder_forUPD;
        }
        set
        {
            this.IsSet_Set_UnitsOnOrder_forUPD = true;
            this._Set_UnitsOnOrder_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ReorderLevel_forUPD</summary>
    public bool IsSet_Set_ReorderLevel_forUPD = false;

    /// <summary>メンバ変数：Set_ReorderLevel_forUPD</summary>
    private System.Int16? _Set_ReorderLevel_forUPD;

    /// <summary>プロパティ：Set_ReorderLevel_forUPD</summary>
    public System.Int16? Set_ReorderLevel_forUPD
    {
        get
        {
            return this._Set_ReorderLevel_forUPD;
        }
        set
        {
            this.IsSet_Set_ReorderLevel_forUPD = true;
            this._Set_ReorderLevel_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Discontinued_forUPD</summary>
    public bool IsSet_Set_Discontinued_forUPD = false;

    /// <summary>メンバ変数：Set_Discontinued_forUPD</summary>
    private System.Boolean? _Set_Discontinued_forUPD;

    /// <summary>プロパティ：Set_Discontinued_forUPD</summary>
    public System.Boolean? Set_Discontinued_forUPD
    {
        get
        {
            return this._Set_Discontinued_forUPD;
        }
        set
        {
            this.IsSet_Set_Discontinued_forUPD = true;
            this._Set_Discontinued_forUPD = value;
        }
    }

    /// <summary>設定フラグ：ProductID_Like</summary>
    public bool IsSet_ProductID_Like = false;

    /// <summary>メンバ変数：ProductID_Like</summary>
    private System.Int32? _ProductID_Like;

    /// <summary>プロパティ：ProductID_Like</summary>
    public System.Int32? ProductID_Like
    {
        get
        {
            return this._ProductID_Like;
        }
        set
        {
            this.IsSet_ProductID_Like = true;
            this._ProductID_Like = value;
        }
    }
    /// <summary>設定フラグ：ProductName_Like</summary>
    public bool IsSet_ProductName_Like = false;

    /// <summary>メンバ変数：ProductName_Like</summary>
    private System.String _ProductName_Like;

    /// <summary>プロパティ：ProductName_Like</summary>
    public System.String ProductName_Like
    {
        get
        {
            return this._ProductName_Like;
        }
        set
        {
            this.IsSet_ProductName_Like = true;
            this._ProductName_Like = value;
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
    /// <summary>設定フラグ：CategoryID_Like</summary>
    public bool IsSet_CategoryID_Like = false;

    /// <summary>メンバ変数：CategoryID_Like</summary>
    private System.Int32? _CategoryID_Like;

    /// <summary>プロパティ：CategoryID_Like</summary>
    public System.Int32? CategoryID_Like
    {
        get
        {
            return this._CategoryID_Like;
        }
        set
        {
            this.IsSet_CategoryID_Like = true;
            this._CategoryID_Like = value;
        }
    }
    /// <summary>設定フラグ：QuantityPerUnit_Like</summary>
    public bool IsSet_QuantityPerUnit_Like = false;

    /// <summary>メンバ変数：QuantityPerUnit_Like</summary>
    private System.String _QuantityPerUnit_Like;

    /// <summary>プロパティ：QuantityPerUnit_Like</summary>
    public System.String QuantityPerUnit_Like
    {
        get
        {
            return this._QuantityPerUnit_Like;
        }
        set
        {
            this.IsSet_QuantityPerUnit_Like = true;
            this._QuantityPerUnit_Like = value;
        }
    }
    /// <summary>設定フラグ：UnitPrice_Like</summary>
    public bool IsSet_UnitPrice_Like = false;

    /// <summary>メンバ変数：UnitPrice_Like</summary>
    private System.Decimal? _UnitPrice_Like;

    /// <summary>プロパティ：UnitPrice_Like</summary>
    public System.Decimal? UnitPrice_Like
    {
        get
        {
            return this._UnitPrice_Like;
        }
        set
        {
            this.IsSet_UnitPrice_Like = true;
            this._UnitPrice_Like = value;
        }
    }
    /// <summary>設定フラグ：UnitsInStock_Like</summary>
    public bool IsSet_UnitsInStock_Like = false;

    /// <summary>メンバ変数：UnitsInStock_Like</summary>
    private System.Int16? _UnitsInStock_Like;

    /// <summary>プロパティ：UnitsInStock_Like</summary>
    public System.Int16? UnitsInStock_Like
    {
        get
        {
            return this._UnitsInStock_Like;
        }
        set
        {
            this.IsSet_UnitsInStock_Like = true;
            this._UnitsInStock_Like = value;
        }
    }
    /// <summary>設定フラグ：UnitsOnOrder_Like</summary>
    public bool IsSet_UnitsOnOrder_Like = false;

    /// <summary>メンバ変数：UnitsOnOrder_Like</summary>
    private System.Int16? _UnitsOnOrder_Like;

    /// <summary>プロパティ：UnitsOnOrder_Like</summary>
    public System.Int16? UnitsOnOrder_Like
    {
        get
        {
            return this._UnitsOnOrder_Like;
        }
        set
        {
            this.IsSet_UnitsOnOrder_Like = true;
            this._UnitsOnOrder_Like = value;
        }
    }
    /// <summary>設定フラグ：ReorderLevel_Like</summary>
    public bool IsSet_ReorderLevel_Like = false;

    /// <summary>メンバ変数：ReorderLevel_Like</summary>
    private System.Int16? _ReorderLevel_Like;

    /// <summary>プロパティ：ReorderLevel_Like</summary>
    public System.Int16? ReorderLevel_Like
    {
        get
        {
            return this._ReorderLevel_Like;
        }
        set
        {
            this.IsSet_ReorderLevel_Like = true;
            this._ReorderLevel_Like = value;
        }
    }
    /// <summary>設定フラグ：Discontinued_Like</summary>
    public bool IsSet_Discontinued_Like = false;

    /// <summary>メンバ変数：Discontinued_Like</summary>
    private System.Boolean? _Discontinued_Like;

    /// <summary>プロパティ：Discontinued_Like</summary>
    public System.Boolean? Discontinued_Like
    {
        get
        {
            return this._Discontinued_Like;
        }
        set
        {
            this.IsSet_Discontinued_Like = true;
            this._Discontinued_Like = value;
        }
    }

    #endregion
}
