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
//* クラス名        ：FxHttpQueryStringIndex
//* クラス日本語名  ：QueryStringのインデックスに使用する文字列定数を定義する定数クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2009/03/13  西野 大介         ラベルの追加
//*  2010/10/13  西野 大介         ダイアログ表示で消費ウィンドウGUIDを消費しない仕様に変更
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>QueryStringのインデックスに使用する文字列定数を定義する</summary>
    public static class FxHttpQueryStringIndex
    {
        /// <summary>モーダル・ダイアログに「親画面GUID」を渡すためのキー</summary>
        public const string PARENT_SCREEN_GUID = "ParentScreenGUID";

        /// <summary>画面遷移時に次画面に「ウィンドウGUID」を渡すためのキー</summary>
        public const string BROWSER_WINDOW_GUID = "BrowserWindowGUID";
    }
}
