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

//***********************************************************************************
//* クラス名        ：FxHttpCookieIndex
//* クラス日本語名  ：HttpCookieのインデックスに使用する文字列定数を定義する定数クラス
//* 
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>HttpCookieのインデックスに使用する文字列定数を定義する</summary>
    public class FxHttpCookieIndex
    {
        /// <summary>セッションタイムアウト検出用クッキーのキー</summary>
        public const string SESSION_TIMEOUT = "SessionTimeOut";

        /// <summary>子画面表示機能の戻るボタン対策用クッキーのキー</summary>
        public const string BACK_BUTTON_CONTROL = "BackButtonControl";
    }
}
