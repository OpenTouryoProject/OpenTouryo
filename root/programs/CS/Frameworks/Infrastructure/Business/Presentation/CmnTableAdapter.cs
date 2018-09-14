//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：CmnTableAdapter
//* クラス日本語名  ：三層データバインド用のTableAdapter共通（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System.Collections.Generic;

using System.Web;

using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>三層データバインド用のTableAdapter共通（テンプレート）</summary>
    public class CmnTableAdapter
    {
        /// <summary>三層データバインド用の引数クラスを生成</summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="myUserInfo">ユーザ情報</param>
        /// <returns>三層データバインド用の引数クラス</returns>
        protected _3TierParameterValue CreateParameter(string tableName, string methodName, MyUserInfo myUserInfo)
        {
            // 三層データバインド用の引数クラスを生成
            _3TierParameterValue parameterValue = new _3TierParameterValue(
                tableName + "ConditionalSearch", tableName + "TableAdapter", methodName,
                (string)HttpContext.Current.Session["DAP"], myUserInfo);

            // DBMS
            parameterValue.DBMSType = (DbEnum.DBMSType)HttpContext.Current.Session["DBMS"];

            // テーブル
            parameterValue.TableName = tableName;

            // 検索条件（Sessionはnullチェック不要）
            #region AND

            parameterValue.AndEqualSearchConditions =
                (Dictionary<string, object>)HttpContext.Current.Session["AndEqualSearchConditions"];

            parameterValue.AndLikeSearchConditions =
                (Dictionary<string, string>)HttpContext.Current.Session["AndLikeSearchConditions"];

            #endregion

            #region OR

            parameterValue.OrEqualSearchConditions =
                (Dictionary<string, object[]>)HttpContext.Current.Session["OrEqualSearchConditions"];

            parameterValue.OrLikeSearchConditions =
                (Dictionary<string, string[]>)HttpContext.Current.Session["OrLikeSearchConditions"];

            #endregion

            #region Else

            parameterValue.ElseWhereSQL =
                (string)HttpContext.Current.Session["ElseWhereSQL"];

            parameterValue.ElseSearchConditions =
                (Dictionary<string, object>)HttpContext.Current.Session["ElseSearchConditions"];

            #endregion

            return parameterValue;
        }
    }
}
