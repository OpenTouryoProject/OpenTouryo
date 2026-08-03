//**********************************************************************************
//* クラス名        ：EmployeeTerritoriesEntity
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
public class EmployeeTerritoriesEntity
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

    #endregion
}
