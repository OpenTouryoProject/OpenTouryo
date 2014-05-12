﻿//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：SelectPkListReturnValue
//* クラス日本語名  ：テスト用の戻り値クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技セ
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// ベースクラス
using Touryo.Infrastructure.Business.Common;

namespace RerunnableBatch_sample2.Common
{
    /// <summary>
    /// SelectPkListReturnValueの概要の説明です
    /// </summary>
    public class SelectPkListReturnValue : MyReturnValue
    {
        /// <summary>PkList</summary>
        public ArrayList PkList;
    }
}
