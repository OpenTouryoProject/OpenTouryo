﻿//**********************************************************************************
//* クラス名        ：_EntityClassName_
//* クラス日本語名  ：自動生成Entityクラス
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：棟梁 D層自動生成ツール（墨壺）, _UserName_
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
public class _EntityClassName_
{
    #region メンバ変数

    // ControlComment:LoopStart-PKColumn
    /// <summary>設定フラグ：_ColumnName_</summary>
    public bool IsSetPK__ColumnName_ = false;

    /// <summary>メンバ変数：_ColumnName_</summary>
    private _EntityTypeInfo_ _PK__ColumnName_;

    /// <summary>プロパティ：_ColumnName_</summary>
    public _EntityTypeInfo_ PK__ColumnName_
    {
        get
        {
            return this._PK__ColumnName_;
        }
        set
        {
            this.IsSetPK__ColumnName_ = true;
            this._PK__ColumnName_ = value;
        }
    }
    // ControlComment:LoopEnd-PKColumn

    // ControlComment:LoopStart-ElseColumn
    /// <summary>設定フラグ：_ColumnName_</summary>
    public bool IsSet__ColumnName_ = false;

    /// <summary>メンバ変数：_ColumnName_</summary>
    private _EntityTypeInfo_ __ColumnName_;

    /// <summary>プロパティ：_ColumnName_</summary>
    public _EntityTypeInfo_ _ColumnName_
    {
        get
        {
            return this.__ColumnName_;
        }
        set
        {
            this.IsSet__ColumnName_ = true;
            this.__ColumnName_ = value;
        }
    }
    // ControlComment:LoopEnd-ElseColumn

    // ControlComment:LoopStart-PPUpdSet
    /// <summary>設定フラグ：_ColumnName_</summary>
    public bool IsSet__ColumnName_ = false;

    /// <summary>メンバ変数：_ColumnName_</summary>
    private _EntityTypeInfo_ __ColumnName_;

    /// <summary>プロパティ：_ColumnName_</summary>
    public _EntityTypeInfo_ _ColumnName_
    {
        get
        {
            return this.__ColumnName_;
        }
        set
        {
            this.IsSet__ColumnName_ = true;
            this.__ColumnName_ = value;
        }
    }
    // ControlComment:LoopEnd-PPUpdSet

    // ControlComment:LoopStart-PPLike
    /// <summary>設定フラグ：_ColumnName_</summary>
    public bool IsSet__ColumnName_ = false;

    /// <summary>メンバ変数：_ColumnName_</summary>
    private _EntityTypeInfo_ __ColumnName_;

    /// <summary>プロパティ：_ColumnName_</summary>
    public _EntityTypeInfo_ _ColumnName_
    {
        get
        {
            return this.__ColumnName_;
        }
        set
        {
            this.IsSet__ColumnName_ = true;
            this.__ColumnName_ = value;
        }
    }
    // ControlComment:LoopEnd-PPLike

    #endregion
}
