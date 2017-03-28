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
//* クラス名        ：RichTextBoxDisableDF
//* クラス日本語名  ：DualFont制御に対応したRichTextBox
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace DPQuery_Tool
{
    /// <summary>DualFont制御に対応したRichTextBox</summary>
    public class RichTextBoxDisableDF : System.Windows.Forms.RichTextBox
    {
        // 初期設定は継承元のRichTextBoxと同様、DualFontを有効に
        private bool dualFont = true;

        // 各メッセージ変数
        private const uint IMF_DUALFONT = 0x80;
        private const uint WM_USER = 0x400;
        private const uint EM_SETLANGOPTIONS = (WM_USER + 120);
        private const uint EM_GETLANGOPTIONS = (WM_USER + 121);

        // API呼び出し
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern uint SendMessage(IntPtr hWnd, uint wMsg,
            uint wParam, uint lParam);

        /// <summary>
        /// 全角と半角でのフォント設定
        /// ・trueにすると、従来どおり、全角と半角で別のフォントを使用 (勝手に変更される)
        /// ・falseにすると、全角と半角で同じフォントを使用 (勝手に変更されない)
        /// </summary>
        public bool DualFont
        {
            get
            {
                return dualFont;
            }
            set
            {
                // DualFontプロパティ値を取得
                dualFont = value;

                // 現在のリッチテキストボックスのIMEアジア言語サポート設定を取得
                uint dwLangOptions =
                    SendMessage(this.Handle, EM_GETLANGOPTIONS, 0, 0);
                uint options = 0;

                // 「true」指定された場合、DualFontを有効に
                if (value)
                {
                    options = dwLangOptions | IMF_DUALFONT;
                }
                // 「false」指定された場合、DualFontを無効に
                else
                {
                    options = dwLangOptions & ~IMF_DUALFONT;
                }

                // 再度、リッチテキストボックスに設定
                SendMessage(this.Handle, EM_SETLANGOPTIONS, 0, options);
            }
        }

        /// <summary>WordWrap時にフォント設定がクリアされるのを防ぐ</summary>
        public new bool WordWrap
        {
            get
            {
                return base.WordWrap;
            }
            set
            {
                base.WordWrap = value;

                // DualFontプロパティ値を再度設定する
                dualFont = DualFont;
            }
        }
    }
}
