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
//* クラス名        ：AsyncEventHeader
//* クラス日本語名  ：非同期イベント ヘッダ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/12/21  西野  大介        新規作成
//*  2011/02/21  西野  大介        メッセージＩＤ（一意性）はサポートしないようにした。
//*                                一意性を保証するなら、機能IDを一意にするか、
//*                                データ部で判別するか、同期イベントを使用すべき。
//**********************************************************************************

// System
using System;
using System.Runtime.InteropServices;

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
    /// <summary>
    /// 非同期イベント ヘッダ（C言語構造体）の授受用
    /// </summary>
    /// <remarks>
    /// C言語構造体と同様に、メンバが宣言された順にメモリに配置するには、
    /// StructLayoutにLayoutKind.Sequentialという値を指定する。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct AsyncEventHeader
    {
        // UnmanagedType 列挙体で
        // アンマネージ コードにマーシャリングする方法を指定する。
        // http://msdn.microsoft.com/ja-jp/library/system.runtime.interopservices.unmanagedtype.aspx

        #region フィールド

        /// <summary>
        /// ＜送信先イベント区分＞
        /// 送信先イベント区分を識別する。
        /// （コールバック関数を登録＆呼出す場合に利用する）
        /// </summary>
        /// <remarks>
        /// ・Thread        （0）
        /// ・ThreadPool    （1）
        /// ・Windows Forms （2）
        /// ・WPF           （3）
        /// </remarks>
        [MarshalAs(UnmanagedType.I4)]
        public int DstEventClass;

        /// <summary>
        /// ＜送信先機能ＩＤ＞
        /// 送信先機能を識別する。
        /// （コールバック関数を登録＆呼出す場合に利用する）
        /// </summary>
        /// <remarks>
        /// 最大36文字
        /// サイズを超える場合は、切り詰められる。
        /// </remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public char[] DstFuncID;

        /// <summary>
        /// ＜送信元 名前付きパイプ名＞
        /// 送信元プロセスを識別する（戻りメッセージの受信時）。
        /// </summary>
        /// <remarks>
        /// 最大36文字
        /// サイズを超える場合は、切り詰められる。
        /// </remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public char[] SrcPipeName;

        /// <summary>
        /// ＜送信元イベント区分＞
        /// 送信先イベント区分を識別する。
        /// （コールバック関数を登録＆呼出す場合に利用する）
        /// </summary>
        /// <remarks>
        /// ・Thread        （0）
        /// ・ThreadPool    （1）
        /// ・Windows Forms （2）
        /// ・WPF           （3）
        /// </remarks>
        [MarshalAs(UnmanagedType.I4)]
        public int SrcEventClass;

        /// <summary>
        /// ＜送信元機能ＩＤ＞
        /// 送信元機能を識別する（戻りメッセージの受信時）。
        /// （コールバック関数を登録＆呼出す場合に利用する）
        /// </summary>
        /// <remarks>
        /// 最大36文字
        /// サイズを超える場合は、切り詰められる。
        /// </remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public char[] SrcFuncID;

        /// <summary>
        /// ＜データ部のバイト長＞
        /// データ部の長さを示す。
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint DataLength;

        #endregion

        /// <summary>コンストラクタ</summary>
        /// <param name="dstEventClass">送信先イベント区分</param>
        /// <param name="dstFuncID">送信先機能ＩＤ（最大36文字）</param>
        /// <param name="srcEventClass">送信元イベント区分</param>
        /// <param name="srcFuncID">送信元機能ＩＤ（最大36文字）</param>
        /// <param name="srcPipeName">送信元 名前付きパイプ名（最大36文字）</param>
        /// <param name="dataLength">データ部のバイト長</param>
        public AsyncEventHeader(
            AsyncEventEnum.EventClass dstEventClass, string dstFuncID,
            AsyncEventEnum.EventClass srcEventClass, string srcFuncID,
            string srcPipeName, uint dataLength)
        {
            // 送信先イベント区分
            this.DstEventClass = (int)dstEventClass;

            // 送信先機能ＩＤ
            this.DstFuncID = new char[36];
            this.DstFuncID = dstFuncID.PadRight(36,' ').ToCharArray(0, 36);

            // 送信元イベント区分
            this.SrcEventClass = (int)srcEventClass;

            // 送信元機能ＩＤ
            this.SrcFuncID = new char[36];
            this.SrcFuncID = srcFuncID.PadRight(36, ' ').ToCharArray(0, 36);

            // 送信元 名前付きパイプ名
            this.SrcPipeName = new char[36];
            this.SrcPipeName = srcPipeName.PadRight(36, ' ').ToCharArray(0, 36);

            // データ部のバイト長
            this.DataLength = dataLength;
        }
    }
}
