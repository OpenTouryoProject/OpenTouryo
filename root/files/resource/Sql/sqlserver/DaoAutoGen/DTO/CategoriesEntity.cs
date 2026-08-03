//**********************************************************************************
//* クラス名        ：CategoriesEntity
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
public class CategoriesEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：CategoryID</summary>
    public bool IsSetPK_CategoryID = false;

    /// <summary>メンバ変数：CategoryID</summary>
    private System.Int32? _PK_CategoryID;

    /// <summary>プロパティ：CategoryID</summary>
    public System.Int32? PK_CategoryID
    {
        get
        {
            return this._PK_CategoryID;
        }
        set
        {
            this.IsSetPK_CategoryID = true;
            this._PK_CategoryID = value;
        }
    }

    /// <summary>設定フラグ：CategoryName</summary>
    public bool IsSet_CategoryName = false;

    /// <summary>メンバ変数：CategoryName</summary>
    private System.String _CategoryName;

    /// <summary>プロパティ：CategoryName</summary>
    public System.String CategoryName
    {
        get
        {
            return this._CategoryName;
        }
        set
        {
            this.IsSet_CategoryName = true;
            this._CategoryName = value;
        }
    }
    /// <summary>設定フラグ：Description</summary>
    public bool IsSet_Description = false;

    /// <summary>メンバ変数：Description</summary>
    private System.String _Description;

    /// <summary>プロパティ：Description</summary>
    public System.String Description
    {
        get
        {
            return this._Description;
        }
        set
        {
            this.IsSet_Description = true;
            this._Description = value;
        }
    }
    /// <summary>設定フラグ：Picture</summary>
    public bool IsSet_Picture = false;

    /// <summary>メンバ変数：Picture</summary>
    private System.Byte[] _Picture;

    /// <summary>プロパティ：Picture</summary>
    public System.Byte[] Picture
    {
        get
        {
            return this._Picture;
        }
        set
        {
            this.IsSet_Picture = true;
            this._Picture = value;
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
    /// <summary>設定フラグ：Set_CategoryName_forUPD</summary>
    public bool IsSet_Set_CategoryName_forUPD = false;

    /// <summary>メンバ変数：Set_CategoryName_forUPD</summary>
    private System.String _Set_CategoryName_forUPD;

    /// <summary>プロパティ：Set_CategoryName_forUPD</summary>
    public System.String Set_CategoryName_forUPD
    {
        get
        {
            return this._Set_CategoryName_forUPD;
        }
        set
        {
            this.IsSet_Set_CategoryName_forUPD = true;
            this._Set_CategoryName_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Description_forUPD</summary>
    public bool IsSet_Set_Description_forUPD = false;

    /// <summary>メンバ変数：Set_Description_forUPD</summary>
    private System.String _Set_Description_forUPD;

    /// <summary>プロパティ：Set_Description_forUPD</summary>
    public System.String Set_Description_forUPD
    {
        get
        {
            return this._Set_Description_forUPD;
        }
        set
        {
            this.IsSet_Set_Description_forUPD = true;
            this._Set_Description_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_Picture_forUPD</summary>
    public bool IsSet_Set_Picture_forUPD = false;

    /// <summary>メンバ変数：Set_Picture_forUPD</summary>
    private System.Byte[] _Set_Picture_forUPD;

    /// <summary>プロパティ：Set_Picture_forUPD</summary>
    public System.Byte[] Set_Picture_forUPD
    {
        get
        {
            return this._Set_Picture_forUPD;
        }
        set
        {
            this.IsSet_Set_Picture_forUPD = true;
            this._Set_Picture_forUPD = value;
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
    /// <summary>設定フラグ：CategoryName_Like</summary>
    public bool IsSet_CategoryName_Like = false;

    /// <summary>メンバ変数：CategoryName_Like</summary>
    private System.String _CategoryName_Like;

    /// <summary>プロパティ：CategoryName_Like</summary>
    public System.String CategoryName_Like
    {
        get
        {
            return this._CategoryName_Like;
        }
        set
        {
            this.IsSet_CategoryName_Like = true;
            this._CategoryName_Like = value;
        }
    }
    /// <summary>設定フラグ：Description_Like</summary>
    public bool IsSet_Description_Like = false;

    /// <summary>メンバ変数：Description_Like</summary>
    private System.String _Description_Like;

    /// <summary>プロパティ：Description_Like</summary>
    public System.String Description_Like
    {
        get
        {
            return this._Description_Like;
        }
        set
        {
            this.IsSet_Description_Like = true;
            this._Description_Like = value;
        }
    }
    /// <summary>設定フラグ：Picture_Like</summary>
    public bool IsSet_Picture_Like = false;

    /// <summary>メンバ変数：Picture_Like</summary>
    private System.Byte[] _Picture_Like;

    /// <summary>プロパティ：Picture_Like</summary>
    public System.Byte[] Picture_Like
    {
        get
        {
            return this._Picture_Like;
        }
        set
        {
            this.IsSet_Picture_Like = true;
            this._Picture_Like = value;
        }
    }

    #endregion
}
