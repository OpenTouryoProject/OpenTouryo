//**********************************************************************************
//* クラス名        ：TerritoriesEntity
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
public class TerritoriesEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：TerritoryID</summary>
    public bool IsSetPK_TerritoryID = false;

    /// <summary>メンバ変数：TerritoryID</summary>
    private System.String _PK_TerritoryID;

    /// <summary>プロパティ：TerritoryID</summary>
    public System.String PK_TerritoryID
    {
        get
        {
            return this._PK_TerritoryID;
        }
        set
        {
            this.IsSetPK_TerritoryID = true;
            this._PK_TerritoryID = value;
        }
    }

    /// <summary>設定フラグ：TerritoryDescription</summary>
    public bool IsSet_TerritoryDescription = false;

    /// <summary>メンバ変数：TerritoryDescription</summary>
    private System.String _TerritoryDescription;

    /// <summary>プロパティ：TerritoryDescription</summary>
    public System.String TerritoryDescription
    {
        get
        {
            return this._TerritoryDescription;
        }
        set
        {
            this.IsSet_TerritoryDescription = true;
            this._TerritoryDescription = value;
        }
    }
    /// <summary>設定フラグ：RegionID</summary>
    public bool IsSet_RegionID = false;

    /// <summary>メンバ変数：RegionID</summary>
    private System.Int32? _RegionID;

    /// <summary>プロパティ：RegionID</summary>
    public System.Int32? RegionID
    {
        get
        {
            return this._RegionID;
        }
        set
        {
            this.IsSet_RegionID = true;
            this._RegionID = value;
        }
    }

    /// <summary>設定フラグ：Set_TerritoryID_forUPD</summary>
    public bool IsSet_Set_TerritoryID_forUPD = false;

    /// <summary>メンバ変数：Set_TerritoryID_forUPD</summary>
    private System.String _Set_TerritoryID_forUPD;

    /// <summary>プロパティ：Set_TerritoryID_forUPD</summary>
    public System.String Set_TerritoryID_forUPD
    {
        get
        {
            return this._Set_TerritoryID_forUPD;
        }
        set
        {
            this.IsSet_Set_TerritoryID_forUPD = true;
            this._Set_TerritoryID_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_TerritoryDescription_forUPD</summary>
    public bool IsSet_Set_TerritoryDescription_forUPD = false;

    /// <summary>メンバ変数：Set_TerritoryDescription_forUPD</summary>
    private System.String _Set_TerritoryDescription_forUPD;

    /// <summary>プロパティ：Set_TerritoryDescription_forUPD</summary>
    public System.String Set_TerritoryDescription_forUPD
    {
        get
        {
            return this._Set_TerritoryDescription_forUPD;
        }
        set
        {
            this.IsSet_Set_TerritoryDescription_forUPD = true;
            this._Set_TerritoryDescription_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_RegionID_forUPD</summary>
    public bool IsSet_Set_RegionID_forUPD = false;

    /// <summary>メンバ変数：Set_RegionID_forUPD</summary>
    private System.Int32? _Set_RegionID_forUPD;

    /// <summary>プロパティ：Set_RegionID_forUPD</summary>
    public System.Int32? Set_RegionID_forUPD
    {
        get
        {
            return this._Set_RegionID_forUPD;
        }
        set
        {
            this.IsSet_Set_RegionID_forUPD = true;
            this._Set_RegionID_forUPD = value;
        }
    }

    /// <summary>設定フラグ：TerritoryID_Like</summary>
    public bool IsSet_TerritoryID_Like = false;

    /// <summary>メンバ変数：TerritoryID_Like</summary>
    private System.String _TerritoryID_Like;

    /// <summary>プロパティ：TerritoryID_Like</summary>
    public System.String TerritoryID_Like
    {
        get
        {
            return this._TerritoryID_Like;
        }
        set
        {
            this.IsSet_TerritoryID_Like = true;
            this._TerritoryID_Like = value;
        }
    }
    /// <summary>設定フラグ：TerritoryDescription_Like</summary>
    public bool IsSet_TerritoryDescription_Like = false;

    /// <summary>メンバ変数：TerritoryDescription_Like</summary>
    private System.String _TerritoryDescription_Like;

    /// <summary>プロパティ：TerritoryDescription_Like</summary>
    public System.String TerritoryDescription_Like
    {
        get
        {
            return this._TerritoryDescription_Like;
        }
        set
        {
            this.IsSet_TerritoryDescription_Like = true;
            this._TerritoryDescription_Like = value;
        }
    }
    /// <summary>設定フラグ：RegionID_Like</summary>
    public bool IsSet_RegionID_Like = false;

    /// <summary>メンバ変数：RegionID_Like</summary>
    private System.Int32? _RegionID_Like;

    /// <summary>プロパティ：RegionID_Like</summary>
    public System.Int32? RegionID_Like
    {
        get
        {
            return this._RegionID_Like;
        }
        set
        {
            this.IsSet_RegionID_Like = true;
            this._RegionID_Like = value;
        }
    }

    #endregion
}
