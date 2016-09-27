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
//* クラス名        ：BaseReturnValue
//* クラス日本語名  ：戻り値親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/04/02  西野  大介        シリアライズ可能にする（WS対応）
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Common
{
    /// <summary>戻り値親クラス１</summary>
    /// <remarks>
    /// シリアライズ可能にする（WS対応）
    /// 自由に利用できる。
    /// </remarks>
    [Serializable()]
    public class BaseReturnValue
    {
        #region インスタンス変数

        /// <summary>エラーフラグ</summary>
        /// <remarks>自由に利用できる。</remarks>
        private bool _errorFlag = false;

        /// <summary>エラーメッセージID</summary>
        /// <remarks>自由に利用できる。</remarks>
        private string _errorMessageID = "";

        /// <summary>エラーメッセージ</summary>
        /// <remarks>自由に利用できる。</remarks>
        private string _errorMessage = "";

        /// <summary>エラー情報</summary>
        /// <remarks>自由に利用できる。</remarks>
        private string _errorInfo = "";

        #endregion

        #region プロパティ

        /// <summary>エラーフラグのプロパティ</summary>
        /// <remarks>
        /// true・・・エラーあり
        /// false・・・エラーなし
        /// 自由に利用できる。
        /// </remarks>
        public bool ErrorFlag
        {
            set
            {
                this._errorFlag = value;
            }
            get
            {
                return this._errorFlag;
            }
        }

        /// <summary>エラーメッセージIDのプロパティ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ErrorMessageID
        {
            set
            {
                this._errorMessageID = value;
            }
            get
            {
                return this._errorMessageID;
            }
        }

        /// <summary>エラーメッセージのプロパティ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ErrorMessage
        {
            set
            {
                this._errorMessage = value;
            }
            get
            {
                return this._errorMessage;
            }
        }

        /// <summary>エラー情報のプロパティ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ErrorInfo
        {
            set
            {
                this._errorInfo = value;
            }
            get
            {
                return this._errorInfo;
            }
        }

        #endregion
    }
}
