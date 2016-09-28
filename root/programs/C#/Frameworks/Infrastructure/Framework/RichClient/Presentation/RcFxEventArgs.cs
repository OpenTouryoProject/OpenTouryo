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
//* クラス名        ：RcFxEventArgs
//* クラス日本語名  ：イベントハンドラの共通引数（Windowアプリケーション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  西野  大介        新規作成
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.RichClient.Presentation
{
    /// <summary>イベントハンドラの共通引数</summary>
    /// <remarks>自由に利用できる。</remarks>
    [Serializable()]
    public class RcFxEventArgs
    {
        #region インスタンス変数

        /// <summary>コントロール名</summary>
        private string _controlName;

        /// <summary>イベントハンドラのメソッド名</summary>
        private string _methodName;

        /// <summary>イベントハンドラのsender</summary>
        private object _sender;

        /// <summary>イベントハンドラのEventArgs</summary>
        private EventArgs _e;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="buttonID">コントロール名</param>
        /// <param name="methodName">レイトバインドする際のメソッド名</param>
        /// <remarks>自由に利用できる。</remarks>
        public RcFxEventArgs(string controlName, string methodName, object sender, EventArgs e)
        {
            // ボタンID
            this._controlName = controlName;

            // メソッド名
            this._methodName = methodName;

            // sender
            this._sender = sender;

            // EventArgs
            this._e = e;
        }

        #endregion

        #region プロパティ

        /// <summary>コントロール名</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ControlName
        {
            get
            {
                return this._controlName;
            }
        }

        /// <summary>イベントハンドラのメソッド名</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MethodName
        {
            // #20-start-end
            get
            {
                return this._methodName;
            }
        }

        /// <summary>イベントハンドラのsender</summary>
        /// <remarks>自由に利用できる。</remarks>
        public object Sender
        {
            get
            {
                return this._sender;
            }
        }

        /// <summary>イベントハンドラのEventArgs</summary>
        /// <remarks>自由に利用できる。</remarks>
        public EventArgs E
        {
            get
            {
                return this._e;
            }
        }

        #endregion
    }
}
