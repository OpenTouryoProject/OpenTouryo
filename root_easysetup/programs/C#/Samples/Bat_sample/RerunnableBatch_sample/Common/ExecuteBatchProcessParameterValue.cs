﻿//**********************************************************************************
//* フレームワーク・テストクラス
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：ExecuteBatchProcessParameterValue
//* クラス日本語名  ：テスト用の引数クラス
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
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Common;

namespace RerunnableBatch_sample.Common
{
    /// <summary>
    /// ExecuteBatchProcessParameterValue の概要の説明です
    /// </summary>
    public class ExecuteBatchProcessParameterValue : MyParameterValue
    {
        /// <summary>1トランザクションで処理を行う主キー一覧</summary>
        public ArrayList SubPkList;

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public ExecuteBatchProcessParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
