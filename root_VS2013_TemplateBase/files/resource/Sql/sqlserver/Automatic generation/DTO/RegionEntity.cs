//**********************************************************************************
//* クラス名        ：RegionEntity
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
public class RegionEntity
{
    #region メンバ変数

    /// <summary>設定フラグ：RegionID</summary>
    public bool IsSetPK_RegionID = false;

    /// <summary>メンバ変数：RegionID</summary>
    private System.Int32? _PK_RegionID;

    /// <summary>プロパティ：RegionID</summary>
    public System.Int32? PK_RegionID
    {
        get
        {
            return this._PK_RegionID;
        }
        set
        {
            this.IsSetPK_RegionID = true;
            this._PK_RegionID = value;
        }
    }

    /// <summary>設定フラグ：RegionDescription</summary>
    public bool IsSet_RegionDescription = false;

    /// <summary>メンバ変数：RegionDescription</summary>
    private System.String _RegionDescription;

    /// <summary>プロパティ：RegionDescription</summary>
    public System.String RegionDescription
    {
        get
        {
            return this._RegionDescription;
        }
        set
        {
            this.IsSet_RegionDescription = true;
            this._RegionDescription = value;
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
    /// <summary>設定フラグ：Set_RegionDescription_forUPD</summary>
    public bool IsSet_Set_RegionDescription_forUPD = false;

    /// <summary>メンバ変数：Set_RegionDescription_forUPD</summary>
    private System.String _Set_RegionDescription_forUPD;

    /// <summary>プロパティ：Set_RegionDescription_forUPD</summary>
    public System.String Set_RegionDescription_forUPD
    {
        get
        {
            return this._Set_RegionDescription_forUPD;
        }
        set
        {
            this.IsSet_Set_RegionDescription_forUPD = true;
            this._Set_RegionDescription_forUPD = value;
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
    /// <summary>設定フラグ：RegionDescription_Like</summary>
    public bool IsSet_RegionDescription_Like = false;

    /// <summary>メンバ変数：RegionDescription_Like</summary>
    private System.String _RegionDescription_Like;

    /// <summary>プロパティ：RegionDescription_Like</summary>
    public System.String RegionDescription_Like
    {
        get
        {
            return this._RegionDescription_Like;
        }
        set
        {
            this.IsSet_RegionDescription_Like = true;
            this._RegionDescription_Like = value;
        }
    }

    #endregion
}
