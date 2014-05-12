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
//* クラス名        ：AsyncEventEntry
//* クラス日本語名  ：非同期イベント エントリ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/12/21  西野  大介        新規作成
//*  2011/02/21  西野  大介        メッセージＩＤ（一意性）はサポートしないようにした。
//*                                一意性を保証するなら、同期イベントフレームワークを開発。
//*  2011/04/25  西野  大介        コンストラクタのチェック処理を見直し
//*  2011/04/25  西野  大介        IEquatableの実装（List.Remove対策）
//*                                List.Remove メソッド
//*                                http://msdn.microsoft.com/ja-jp/library/cd666k3e.aspx
//*                                EqualityComparer.Default プロパティ
//*                                http://msdn.microsoft.com/ja-jp/library/ms224763.aspx
//*                                IEquatable ジェネリック インターフェイス
//*                                http://msdn.microsoft.com/ja-jp/library/ms131187.aspx
//**********************************************************************************

// System
using System;

using System.Threading;

using System.Windows;
using System.Windows.Forms;

// 業務フレームワーク（循環参照になるため、参照しない）

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.RichClient.Asynchronous
{
    /// <summary>非同期イベント デリゲート エントリ</summary>
    /// <remarks>１つの機能ＩＤに対して複数のエントリを登録可能</remarks>
    public class AsyncEventEntry : IEquatable<AsyncEventEntry>
    {
        #region メンバ

        /// <summary>イベント区分</summary>
        public AsyncEventEnum.EventClass EventClass { private set; get; }

        /// <summary>機能ＩＤ（最大36文字）</summary>
        public string FuncID { private set; get; }

        /// <summary>UIコントロール</summary>
        /// <remarks>メッセージ ループを使用する際に必要</remarks>
        public object Control { private set; get; }

        /// <summary>コールバック</summary>
        public object Callback { private set; get; }

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="eventClass">
        /// イベント区分
        /// ・スレッド関数へ    ：AsyncEventEnum.EventClass.ThreadPool
        /// ・スレッドプールへ  ：AsyncEventEnum.EventClass.WinForm
        /// ・WinFormの結果表示 ：AsyncEventEnum.EventClass.WinForm
        /// ・WPFの結果表示     ：AsyncEventEnum.EventClass.WPF
        /// </param>
        /// <param name="funcID">機能ＩＤ（最大36文字）</param>
        /// <param name="control">
        /// UIコントロール
        /// ・Control（WinForm）
        /// ・DependencyObject（WPF）
        /// </param>
        /// <param name="callback">
        /// ＜コールバック＞
        /// ・スレッド関数へ    ：System.Threading.ParameterizedThreadStart
        /// ・スレッドプールへ  ：System.Threading.WaitCallback
        /// ・WinFormの結果表示 ：AsyncEventFx.SetResultDelegate
        /// ・WPFの結果表示     ：AsyncEventFx.SetResultDelegate
        /// </param>
        public AsyncEventEntry(
            AsyncEventEnum.EventClass eventClass, string funcID, object control, object callback)
        {
            // イベント区分のチェック
            switch (eventClass)
            {
                case AsyncEventEnum.EventClass.Thread:
                    break;
                case AsyncEventEnum.EventClass.ThreadPool:
                    break;
                case AsyncEventEnum.EventClass.WinForm:
                    break;
                case AsyncEventEnum.EventClass.WPF:
                    break;
                default:
                    // イベント区分エラー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CHECK_ERROR[0],
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CHECK_ERROR[1]);
            }

            // イベント区分
            this.EventClass = eventClass;

            // 機能ＩＤのチェック（最大36文字）
            if (funcID.Length <= 36)
            {
                this.FuncID = funcID;
            }
            else
            {
                // 自動切り落とし
                this.FuncID = funcID.Substring(0, 36);
            }

            // イベント区分に対応する
            // UIコントロールであるかのチェック
            if (control != null)
            {
                if (control is Control
                    || this.EventClass == AsyncEventEnum.EventClass.WinForm)
                {
                    // OK
                }
                else if (control is DependencyObject
                    || this.EventClass == AsyncEventEnum.EventClass.WPF)
                {
                    // OK
                }
                else
                {
                    // エラー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CONTROL_CHECK_ERROR[0],
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CONTROL_CHECK_ERROR[1]);
                }

                // UIコントロール
                this.Control = control;
            }

            // イベント区分に対応する
            // コールバックであるかのチェック
            if (callback != null)
            {
                if (callback is System.Threading.ParameterizedThreadStart
                    || this.EventClass == AsyncEventEnum.EventClass.Thread)
                {
                    // OK：スレッド関数
                }
                else if (callback is System.Threading.WaitCallback
                    || this.EventClass == AsyncEventEnum.EventClass.ThreadPool)
                {
                    // OK：スレッドプール
                }
                else if (callback is AsyncEventFx.SetResultDelegate
                    || this.EventClass == AsyncEventEnum.EventClass.WinForm)
                {
                    // OK：WinFormの結果表示
                }
                else if (callback is AsyncEventFx.SetResultDelegate
                    || this.EventClass == AsyncEventEnum.EventClass.WPF)
                {
                    // OK：WPFの結果表示
                }
                else
                {
                    // エラー（型不正）
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR[0],
                        FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR[1]);
                }

                // コールバック
                this.Callback = callback;
            }
            else
            {
                // エラー（null）
                throw new FrameworkException(
                    FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR[0],
                    FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR[1]);
            }
        }
        
        #endregion

        #region IEquatableの実装

        /// <summary>ハッシュを返す</summary>
        /// <returns>ハッシュコード</returns>
        /// <remarks>全メンバのハッシュコードのXOR</remarks>
        public override int GetHashCode()
        {
            int hc = 0;

            hc ^= this.EventClass.GetHashCode();

            if (this.FuncID != null)
            {
                hc ^= this.FuncID.GetHashCode();
            }
            if (this.Control != null)
            {
                hc ^= this.Control.GetHashCode();
            }
            if (this.Callback != null)
            {
                hc ^= this.Callback.GetHashCode();
            }

            return hc;
        }

        /// <summary>Equals</summary>
        /// <param name="aee">AsyncEventEntry</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        /// <remarks>全メンバの==のAND</remarks>
        public bool Equals(AsyncEventEntry aee)
        {
            // null対応
            if (aee == null) { return false; }

            return
                (this.EventClass == aee.EventClass)
                && (this.FuncID == aee.FuncID)
                && (this.Control == aee.Control)
                && (this.Callback == aee.Callback);
        }

        /// <summary>Equals</summary>
        /// <param name="obj">AsyncEventEntry</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                // nullの場合（ベースへ）
                return base.Equals(obj);
            }
            else
            {
                // nullでない場合
                if (!(obj is AsyncEventEntry))
                { 
                    // 型が違う
                    return false;
                }
                else
                {
                    // 型が一致（オーバロードヘ）
                    return Equals(obj as AsyncEventEntry);
                }
            }
        }

        /// <summary>比較演算子（==）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator ==(AsyncEventEntry l, AsyncEventEntry r)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(l, null))
            {
                // Check for null on right side.
                if (Object.ReferenceEquals(r, null))
                {
                    // null == null = true.
                    return true;
                }
                else
                {
                    // Only the left side is null.
                    return false;
                }
            }
            else
            {
                // Equals handles case of null on right side.
                return l.Equals(r);
            }
        }

        /// <summary>比較演算子（!=）</summary>
        /// <param name="l">右辺</param>
        /// <param name="r">左辺</param>
        /// <returns>
        /// true：等しい
        /// false：等しくない
        /// </returns>
        public static bool operator !=(AsyncEventEntry l, AsyncEventEntry r)
        {
            // ==演算子の逆
            return !(l == r);
        }

        #endregion
    }
}
