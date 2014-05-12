﻿//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_3TierReturnValue
//* クラス日本語名  ：三層データバインド用の戻り値クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/01/10  西野　大介        新規作成
//*
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Business.Common
{
    /// <summary>三層データバインド用の戻り値クラス</summary>
    public class _3TierReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>データテーブル</summary>
        public DataTable Dt;
    }
}
