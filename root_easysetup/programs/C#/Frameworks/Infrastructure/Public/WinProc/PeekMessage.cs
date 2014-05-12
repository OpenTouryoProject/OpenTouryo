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
//* クラス名        ：PeekMessage
//* クラス日本語名  ：PeekMessageを使用した処理を実装する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/18  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using Touryo.Infrastructure.Public.Win32;

namespace Touryo.Infrastructure.Public.WinProc
{
    /// <summary>
    /// PeekMessageを使用した処理を実装する。
    /// </summary>
    public class PeekMessage
    {
        /// <summary>
        /// 保留されているウィンドウメッセージを削除する。
        /// </summary>
        public static void RemoveMessage()
        {
            PeekMessage.RemoveMessage(null);
        }

        /// <summary>
        /// 保留されているウィンドウメッセージを削除する。
        /// </summary>
        /// <param name="msgs">
        /// 追加のDispatch対象のメッセージ
        /// 既定で
        /// ・WM_LBUTTONUP
        /// ・WM_MOUSELEAVE
        /// ・WM_PAINT
        /// はDispatchMessageする。
        /// </param>
        public static void RemoveMessage(int[] msgs)
        {
            int[] i, j;
            PeekMessage.RemoveMessage(msgs, out i, out j);
        }

        /// <summary>
        /// 保留されているウィンドウメッセージを削除する。
        /// </summary>
        /// <param name="msgs">
        /// 追加のDispatch対象のメッセージ
        /// 既定で
        /// ・WM_LBUTTONUP
        /// ・WM_MOUSELEAVE
        /// ・WM_PAINT
        /// はDispatchMessageする。
        /// </param>
        /// <param name="filteredMessages">Filterされたメッセージ</param>
        /// <param name="dispatchedMessages">Dispatchされたメッセージ</param>
        public static void RemoveMessage(int[] msgs, out int[] filteredMessages, out int[] dispatchedMessages)
        {
            // メッセージキューから特定のメッセージを消す
            // http://blogs.wankuma.com/youchi/archive/2010/05/27/189449.aspx

            List<int> _filteredMessages = new List<int>();
            List<int> _dispatchedMessages = new List<int>();

            WinProcWin32.MSG wm;

            // 保留されているウィンドウメッセージを削除
            while (WinProcWin32.PeekMessage(
                out wm, 0, 0, 0, WinProcWin32.EPeekMessageOption.PM_REMOVE))
            {
                // メッセージを取得した場合、true が返る。
                switch (wm.msg)
                {
                    case WinProcWin32.WM_LBUTTONUP:
                    case WinProcWin32.WM_MOUSELEAVE:
                    case WinProcWin32.WM_PAINT:
                        
                        // 上記3つのメッセージだけDispatchする。
                        _dispatchedMessages.Add(wm.msg);
                        WinProcWin32.DispatchMessage(out wm);

                        break;

                    default:

                        // フラグ
                        bool isDispatched = false;

                        //// デバッグ出力
                        //Debug.WriteLine("msg:0x" + wm.msg.ToString("X2"));

                        // 追加のDispatch対象のメッセージ
                        if (msgs == null)
                        {
                            // なし。
                        }
                        else
                        {
                            // あり。
                            foreach (int msg in msgs)
                            {
                                if (wm.msg == msg)
                                {
                                    // 指定のメッセージだけDispatchする。
                                    isDispatched = true;
                                    _dispatchedMessages.Add(wm.msg);
                                    WinProcWin32.DispatchMessage(out wm);
                                    break;
                                }
                                else
                                {
                                }
                            }
                        }

                        // フラグをチェックし、
                        if (!isDispatched)
                        {
                            // Dispatchされていないメッセージを格納。
                            _filteredMessages.Add(wm.msg);
                        }

                        break;
                }
            }

            // 戻り値を返す。
            filteredMessages = _filteredMessages.ToArray();
            dispatchedMessages = _dispatchedMessages.ToArray();
        }
    }
}
