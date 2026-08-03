//**********************************************************************************
//* クラス名        ：ts_test_table2Entity
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
public class ts_test_table2Entity
{
    #region メンバ変数

    /// <summary>設定フラグ：id</summary>
    public bool IsSetPK_id = false;

    /// <summary>メンバ変数：id</summary>
    private System.Int32? _PK_id;

    /// <summary>プロパティ：id</summary>
    public System.Int32? PK_id
    {
        get
        {
            return this._PK_id;
        }
        set
        {
            this.IsSetPK_id = true;
            this._PK_id = value;
        }
    }

    /// <summary>設定フラグ：ts</summary>
    public bool IsSet_ts = false;

    /// <summary>メンバ変数：ts</summary>
    private System.Double? _ts;

    /// <summary>プロパティ：ts</summary>
    public System.Double? ts
    {
        get
        {
            return this._ts;
        }
        set
        {
            this.IsSet_ts = true;
            this._ts = value;
        }
    }
    /// <summary>設定フラグ：val</summary>
    public bool IsSet_val = false;

    /// <summary>メンバ変数：val</summary>
    private System.String _val;

    /// <summary>プロパティ：val</summary>
    public System.String val
    {
        get
        {
            return this._val;
        }
        set
        {
            this.IsSet_val = true;
            this._val = value;
        }
    }

    /// <summary>設定フラグ：Set_id_forUPD</summary>
    public bool IsSet_Set_id_forUPD = false;

    /// <summary>メンバ変数：Set_id_forUPD</summary>
    private System.Int32? _Set_id_forUPD;

    /// <summary>プロパティ：Set_id_forUPD</summary>
    public System.Int32? Set_id_forUPD
    {
        get
        {
            return this._Set_id_forUPD;
        }
        set
        {
            this.IsSet_Set_id_forUPD = true;
            this._Set_id_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_ts_forUPD</summary>
    public bool IsSet_Set_ts_forUPD = false;

    /// <summary>メンバ変数：Set_ts_forUPD</summary>
    private System.Double? _Set_ts_forUPD;

    /// <summary>プロパティ：Set_ts_forUPD</summary>
    public System.Double? Set_ts_forUPD
    {
        get
        {
            return this._Set_ts_forUPD;
        }
        set
        {
            this.IsSet_Set_ts_forUPD = true;
            this._Set_ts_forUPD = value;
        }
    }
    /// <summary>設定フラグ：Set_val_forUPD</summary>
    public bool IsSet_Set_val_forUPD = false;

    /// <summary>メンバ変数：Set_val_forUPD</summary>
    private System.String _Set_val_forUPD;

    /// <summary>プロパティ：Set_val_forUPD</summary>
    public System.String Set_val_forUPD
    {
        get
        {
            return this._Set_val_forUPD;
        }
        set
        {
            this.IsSet_Set_val_forUPD = true;
            this._Set_val_forUPD = value;
        }
    }

    /// <summary>設定フラグ：id_Like</summary>
    public bool IsSet_id_Like = false;

    /// <summary>メンバ変数：id_Like</summary>
    private System.Int32? _id_Like;

    /// <summary>プロパティ：id_Like</summary>
    public System.Int32? id_Like
    {
        get
        {
            return this._id_Like;
        }
        set
        {
            this.IsSet_id_Like = true;
            this._id_Like = value;
        }
    }
    /// <summary>設定フラグ：ts_Like</summary>
    public bool IsSet_ts_Like = false;

    /// <summary>メンバ変数：ts_Like</summary>
    private System.Double? _ts_Like;

    /// <summary>プロパティ：ts_Like</summary>
    public System.Double? ts_Like
    {
        get
        {
            return this._ts_Like;
        }
        set
        {
            this.IsSet_ts_Like = true;
            this._ts_Like = value;
        }
    }
    /// <summary>設定フラグ：val_Like</summary>
    public bool IsSet_val_Like = false;

    /// <summary>メンバ変数：val_Like</summary>
    private System.String _val_Like;

    /// <summary>プロパティ：val_Like</summary>
    public System.String val_Like
    {
        get
        {
            return this._val_Like;
        }
        set
        {
            this.IsSet_val_Like = true;
            this._val_Like = value;
        }
    }

    #endregion
}
