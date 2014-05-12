//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：WSErrorInfo
//* クラス日本語名  ：Webサービスのエラー情報クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/02  西野  大介        新規作成
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#18 ： SoapException対策（例外情報の転送）
//**********************************************************************************

// System
using System;

// フレームワーク
using Touryo.Infrastructure.Framework.Util;

namespace Touryo.Infrastructure.Framework.Transmission
{
    /// <summary>Webサービスのエラー情報クラス</summary>
    /// <remarks>シリアライズ可能にする（WS対応）</remarks>
    [Serializable()]
    public class WSErrorInfo
    {
        /// <summary>エラーの型情報</summary>
        private FxEnum.ErrorType _errorType;

        /// <summary>エラーの型情報</summary>
        public FxEnum.ErrorType ErrorType
        {
            set { this._errorType = value; }
            get { return this._errorType; }
        }

        /// <summary>エラーのメッセージID</summary>
        private string _errorMessageID = "";

        /// <summary>エラーのメッセージID</summary>
        public string ErrorMessageID
        {
            set { this._errorMessageID = value; }
            get { return this._errorMessageID; }
        }

        /// <summary>エラーのメッセージ</summary>
        private string _errorMessage = "";

        /// <summary>エラーのメッセージ</summary>
        public string ErrorMessage
        {
            set { this._errorMessage = value; }
            get { return this._errorMessage; }
        }

        // #18-start
        /// <summary>エラーの情報</summary>
        private string _errorInformation = "";

        /// <summary>エラーの情報</summary>
        public string ErrorInformation
        {
            set { this._errorInformation = value; }
            get { return this._errorInformation; }
        }
        // #18-end        
    }
}
