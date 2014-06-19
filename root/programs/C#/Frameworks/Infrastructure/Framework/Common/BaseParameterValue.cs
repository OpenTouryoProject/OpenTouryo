//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：BaseParameterValue
//* クラス日本語名  ：引数親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/01/30  西野  大介        ２層C/S対応
//*  2009/03/13  西野  大介        ラベル名のみ変更
//*  2009/04/02  西野  大介        Binaryシリアライズ可能にする（WS対応）
//*  2009/04/08  西野  大介        セッションを使用しない（部分信頼で処理できないため）
//*  2009/04/08  西野  大介        上記の変更に合わせて、Is2CSを削除
//*  2009/05/12  西野  大介        デフォルト コンストラクタを追加
//*  2010/03/03  西野  大介        自動振り分け処理に使用するメソッド名を追加
//*  2010/03/11  西野  大介        引数の順番を変更した。
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Common
{
    /// <summary>引数親クラス１</summary>
    /// <remarks>
    /// シリアライズ可能にする（WS対応）
    /// 自由に利用できる。
    /// </remarks>
    [Serializable()]
    public class BaseParameterValue
    {
        #region インスタンス変数

        /// <summary>スクリーンID（画面名）</summary>
        private string _screenId = "";

        /// <summary>コントロールID（コントロール名）</summary>
        private string _controlId = "";

        /// <summary>メソッド名（自動振り分け処理で利用）</summary>
        private string _methodName = "";

        /// <summary>アクション タイプ（処理区分）</summary>
        private string _actionType = "";

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">スクリーンID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <remarks>
        /// 自由に利用できる（互換性の維持）。
        /// </remarks>
        public BaseParameterValue(string screenId, string controlId, string actionType)
        {
            // スクリーンID
            this._screenId = screenId;

            // コントロールID
            this._controlId = controlId;

            // メソッド名
            this._methodName = "";

            // アクション タイプ
            this._actionType = actionType;            
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="screenId">スクリーンID</param>
        /// <param name="controlId">コントロールID</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="actionType">アクションタイプ</param>
        /// <remarks>
        /// 自由に利用できる。
        /// </remarks>
        public BaseParameterValue(string screenId, string controlId, string methodName, string actionType)
        {
            // スクリーンID
            this._screenId = screenId;

            // コントロールID
            this._controlId = controlId;

            // メソッド名
            this._methodName = methodName;

            // アクション タイプ
            this._actionType = actionType;
        }

        #endregion

        #region プロパティ

        /// <summary>スクリーンIDのプロパティ（読み取り専用）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ScreenId
        {
            get
            {
                return this._screenId;
            }
        }

        /// <summary>コントロールIDのプロパティ（読み取り専用）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ControlId
        {
            get
            {
                return this._controlId;
            }
        }

        /// <summary>メソッド名のプロパティ（読み取り専用）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MethodName
        {
            get
            {
                return this._methodName;
            }
        }

        /// <summary>アクション タイプのプロパティ（読み取り専用）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ActionType
        {
            get
            {
                return this._actionType;
            }
        }

        #endregion
    }
}
