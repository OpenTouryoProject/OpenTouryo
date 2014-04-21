//**********************************************************************************
//* クラス名        ：ShippersEntity
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
public class ShippersEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：ShipperID</summary>
    public bool IsSetPK_ShipperID = false;

    /// <summary>メンバ変数：ShipperID</summary>
    private System.Int32? _PK_ShipperID;

    /// <summary>プロパティ：ShipperID</summary>
    public System.Int32? PK_ShipperID
    {
        get
        {
            return this._PK_ShipperID;
        }
        set
        {
            this.IsSetPK_ShipperID = true;
            this._PK_ShipperID = value;
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

    /// <summary>設定フラグ：Set_ShipperID_forUPD</summary>
    public bool IsSet_Set_ShipperID_forUPD = false;

    /// <summary>メンバ変数：Set_ShipperID_forUPD</summary>
    private System.Int32? _Set_ShipperID_forUPD;

    /// <summary>プロパティ：Set_ShipperID_forUPD</summary>
    public System.Int32? Set_ShipperID_forUPD
    {
        get
        {
            return this._Set_ShipperID_forUPD;
        }
        set
        {
            this.IsSet_Set_ShipperID_forUPD = true;
            this._Set_ShipperID_forUPD = value;
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

    /// <summary>設定フラグ：ShipperID_Like</summary>
    public bool IsSet_ShipperID_Like = false;

    /// <summary>メンバ変数：ShipperID_Like</summary>
    private System.Int32? _ShipperID_Like;

    /// <summary>プロパティ：ShipperID_Like</summary>
    public System.Int32? ShipperID_Like
    {
        get
        {
            return this._ShipperID_Like;
        }
        set
        {
            this.IsSet_ShipperID_Like = true;
            this._ShipperID_Like = value;
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

    #endregion
}
