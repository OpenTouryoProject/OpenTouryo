//**********************************************************************************
//* クラス名        ：SHIPPERSEntity
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
public class SHIPPERSEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：SHIPPERID</summary>
    public bool IsSetPK_SHIPPERID = false;

    /// <summary>メンバ変数：SHIPPERID</summary>
    private System.Decimal? _PK_SHIPPERID;

    /// <summary>プロパティ：SHIPPERID</summary>
    public System.Decimal? PK_SHIPPERID
    {
        get
        {
            return this._PK_SHIPPERID;
        }
        set
        {
            this.IsSetPK_SHIPPERID = true;
            this._PK_SHIPPERID = value;
        }
    }

    /// <summary>設定フラグ：COMPANYNAME</summary>
    public bool IsSet_COMPANYNAME = false;

    /// <summary>メンバ変数：COMPANYNAME</summary>
    private System.String _COMPANYNAME;

    /// <summary>プロパティ：COMPANYNAME</summary>
    public System.String COMPANYNAME
    {
        get
        {
            return this._COMPANYNAME;
        }
        set
        {
            this.IsSet_COMPANYNAME = true;
            this._COMPANYNAME = value;
        }
    }
    /// <summary>設定フラグ：PHONE</summary>
    public bool IsSet_PHONE = false;

    /// <summary>メンバ変数：PHONE</summary>
    private System.String _PHONE;

    /// <summary>プロパティ：PHONE</summary>
    public System.String PHONE
    {
        get
        {
            return this._PHONE;
        }
        set
        {
            this.IsSet_PHONE = true;
            this._PHONE = value;
        }
    }

    /// <summary>設定フラグ：Set_SHIPPERID_forUPD</summary>
    public bool IsSet_Set_SHIPPERID_forUPD = false;

    /// <summary>メンバ変数：Set_SHIPPERID_forUPD</summary>
    private System.Decimal? _Set_SHIPPERID_forUPD;

    /// <summary>プロパティ：Set_SHIPPERID_forUPD</summary>
    public System.Decimal? Set_SHIPPERID_forUPD
    {
        get
        {
            return this._Set_SHIPPERID_forUPD;
        }
        set
        {
            this.IsSet_Set_SHIPPERID_forUPD = true;
            this._Set_SHIPPERID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_COMPANYNAME_forUPD</summary>
    public bool IsSet_Set_COMPANYNAME_forUPD = false;

    /// <summary>メンバ変数：Set_COMPANYNAME_forUPD</summary>
    private System.String _Set_COMPANYNAME_forUPD;

    /// <summary>プロパティ：Set_COMPANYNAME_forUPD</summary>
    public System.String Set_COMPANYNAME_forUPD
    {
        get
        {
            return this._Set_COMPANYNAME_forUPD;
        }
        set
        {
            this.IsSet_Set_COMPANYNAME_forUPD = true;
            this._Set_COMPANYNAME_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_PHONE_forUPD</summary>
    public bool IsSet_Set_PHONE_forUPD = false;

    /// <summary>メンバ変数：Set_PHONE_forUPD</summary>
    private System.String _Set_PHONE_forUPD;

    /// <summary>プロパティ：Set_PHONE_forUPD</summary>
    public System.String Set_PHONE_forUPD
    {
        get
        {
            return this._Set_PHONE_forUPD;
        }
        set
        {
            this.IsSet_Set_PHONE_forUPD = true;
            this._Set_PHONE_forUPD = value;
        }
    }

    /// <summary>設定フラグ：SHIPPERID_Like</summary>
    public bool IsSet_SHIPPERID_Like = false;

    /// <summary>メンバ変数：SHIPPERID_Like</summary>
    private System.Decimal? _SHIPPERID_Like;

    /// <summary>プロパティ：SHIPPERID_Like</summary>
    public System.Decimal? SHIPPERID_Like
    {
        get
        {
            return this._SHIPPERID_Like;
        }
        set
        {
            this.IsSet_SHIPPERID_Like = true;
            this._SHIPPERID_Like = value;
        }
    }
    /// <summary>設定フラグ：COMPANYNAME_Like</summary>
    public bool IsSet_COMPANYNAME_Like = false;

    /// <summary>メンバ変数：COMPANYNAME_Like</summary>
    private System.String _COMPANYNAME_Like;

    /// <summary>プロパティ：COMPANYNAME_Like</summary>
    public System.String COMPANYNAME_Like
    {
        get
        {
            return this._COMPANYNAME_Like;
        }
        set
        {
            this.IsSet_COMPANYNAME_Like = true;
            this._COMPANYNAME_Like = value;
        }
    }
    /// <summary>設定フラグ：PHONE_Like</summary>
    public bool IsSet_PHONE_Like = false;

    /// <summary>メンバ変数：PHONE_Like</summary>
    private System.String _PHONE_Like;

    /// <summary>プロパティ：PHONE_Like</summary>
    public System.String PHONE_Like
    {
        get
        {
            return this._PHONE_Like;
        }
        set
        {
            this.IsSet_PHONE_Like = true;
            this._PHONE_Like = value;
        }
    }

    #endregion
}
