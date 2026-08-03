//**********************************************************************************
//* クラス名        ：Order_DetailsEntity
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
public class Order_DetailsEntity
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
    /// <summary>設定フラグ：Quantity</summary>
    public bool IsSet_Quantity = false;

    /// <summary>メンバ変数：Quantity</summary>
    private System.Int16? _Quantity;

    /// <summary>プロパティ：Quantity</summary>
    public System.Int16? Quantity
    {
        get
        {
            return this._Quantity;
        }
        set
        {
            this.IsSet_Quantity = true;
            this._Quantity = value;
        }
    }
    /// <summary>設定フラグ：Discount</summary>
    public bool IsSet_Discount = false;

    /// <summary>メンバ変数：Discount</summary>
    private System.Single? _Discount;

    /// <summary>プロパティ：Discount</summary>
    public System.Single? Discount
    {
        get
        {
            return this._Discount;
        }
        set
        {
            this.IsSet_Discount = true;
            this._Discount = value;
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
    /// <summary>設定フラグ：Set_Quantity_forUPD</summary>
    public bool IsSet_Set_Quantity_forUPD = false;

    /// <summary>メンバ変数：Set_Quantity_forUPD</summary>
    private System.Int16? _Set_Quantity_forUPD;

    /// <summary>プロパティ：Set_Quantity_forUPD</summary>
    public System.Int16? Set_Quantity_forUPD
    {
        get
        {
            return this._Set_Quantity_forUPD;
        }
        set
        {
            this.IsSet_Set_Quantity_forUPD = true;
            this._Set_Quantity_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Discount_forUPD</summary>
    public bool IsSet_Set_Discount_forUPD = false;

    /// <summary>メンバ変数：Set_Discount_forUPD</summary>
    private System.Single? _Set_Discount_forUPD;

    /// <summary>プロパティ：Set_Discount_forUPD</summary>
    public System.Single? Set_Discount_forUPD
    {
        get
        {
            return this._Set_Discount_forUPD;
        }
        set
        {
            this.IsSet_Set_Discount_forUPD = true;
            this._Set_Discount_forUPD = value;
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
    /// <summary>設定フラグ：Quantity_Like</summary>
    public bool IsSet_Quantity_Like = false;

    /// <summary>メンバ変数：Quantity_Like</summary>
    private System.Int16? _Quantity_Like;

    /// <summary>プロパティ：Quantity_Like</summary>
    public System.Int16? Quantity_Like
    {
        get
        {
            return this._Quantity_Like;
        }
        set
        {
            this.IsSet_Quantity_Like = true;
            this._Quantity_Like = value;
        }
    }
    /// <summary>設定フラグ：Discount_Like</summary>
    public bool IsSet_Discount_Like = false;

    /// <summary>メンバ変数：Discount_Like</summary>
    private System.Single? _Discount_Like;

    /// <summary>プロパティ：Discount_Like</summary>
    public System.Single? Discount_Like
    {
        get
        {
            return this._Discount_Like;
        }
        set
        {
            this.IsSet_Discount_Like = true;
            this._Discount_Like = value;
        }
    }

    #endregion
}
