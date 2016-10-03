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
//* クラス名        ：FxEventArgs
//* クラス日本語名  ：イベントハンドラの共通引数
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#20： 不要なコードブロックの削除
//*  2010/10/21  西野  大介        RepeaterコントロールのItemCommandイベント追加
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Presentation
{
    /// <summary>イベントハンドラの共通引数</summary>
    /// <remarks>自由に利用できる。</remarks>
    [Serializable()]
    public class FxEventArgs
    {
        #region インスタンス変数

        /// <summary>ボタンID</summary>
        private string _buttonID;

        /// <summary>内部ボタンID</summary>
        private string _innerButtonID;

        /// <summary>イメージボタンのX座標</summary>
        private int _x;

        /// <summary>イメージボタンのY座標</summary>
        private int _y;

        /// <summary>その他（PostBackValue、CommandNameなど）</summary>
        private string _postBackValue;

        /// <summary>イベントハンドラのメソッド名</summary>
        private string _methodName;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="buttonID">ボタンID</param>
        /// <param name="x">イメージボタンのX座標</param>
        /// <param name="y">イメージボタンのY座標</param>
        /// <param name="postBackValue">その他（PostBackValue、CommandNameなど）</param>
        /// <param name="methodName">レイトバインドする際のメソッド名</param>
        /// <remarks>自由に利用できる。</remarks>
        public FxEventArgs(string buttonID, int x, int y, string postBackValue, string methodName)
        {
            // ボタンID
            this._buttonID = buttonID;

            // 内部ボタンID
            this._innerButtonID = "";

            // イメージボタンのX座標
            this._x = x;

            // イメージボタンのY座標
            this._y = y;

            // その他（PostBackValue、CommandNameなど）
            this._postBackValue = postBackValue;

            // メソッド名
            this._methodName = methodName;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="buttonID">ボタンID</param>
        /// <param name="innerButtonID">内部ボタンID</param>
        /// <param name="x">イメージボタンのX座標</param>
        /// <param name="y">イメージボタンのY座標</param>
        /// <param name="postBackValue">その他（PostBackValue、CommandNameなど）</param>
        /// <param name="methodName">レイトバインドする際のメソッド名</param>
        /// <remarks>自由に利用できる。</remarks>
        public FxEventArgs(string buttonID, string innerButtonID, int x, int y, string postBackValue, string methodName)
        {
            // ボタンID
            this._buttonID = buttonID;

            // 内部ボタンID
            this._innerButtonID = innerButtonID;

            // イメージボタンのX座標
            this._x = x;

            // イメージボタンのY座標
            this._y = y;

            // その他（PostBackValue、CommandNameなど）
            this._postBackValue = postBackValue;

            // メソッド名
            this._methodName = methodName;
        }

        #endregion

        #region プロパティ

        /// <summary>ボタンID</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ButtonID
        {
            get
            {
                return this._buttonID;
            }
        }

        /// <summary>内部ボタンID</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string InnerButtonID
        {
            get
            {
                return this._innerButtonID;
            }
        }

        /// <summary>イメージボタンのX座標</summary>
        /// <remarks>自由に利用できる。</remarks>
        public int X
        {
            get
            {
                return this._x;
            }
        }

        /// <summary>イメージボタンのY座標</summary>
        /// <remarks>自由に利用できる。</remarks>
        public int Y
        {
            get
            {
                return this._y;
            }
        }

        /// <summary>イメージマップのホットスポットのポストバック値</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string PostBackValue
        {
            get
            {
                return this._postBackValue;
            }
        }

        /// <summary>イベントハンドラのメソッド名</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MethodName
        {
            get
            {
                return this._methodName;
            }
        }

        #endregion
    }
}
