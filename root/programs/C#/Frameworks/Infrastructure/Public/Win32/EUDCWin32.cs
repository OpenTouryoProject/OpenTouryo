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
//* クラス名        ：EUDCWin32
//* クラス日本語名  ：EUDC（End User Defined Character）関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/07/17  西野  大介        新規作成
//*  2013/02/18  西野  大介        SetLastError対応
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    /// <summary>
    /// EUDC（End User Defined Character）関連Win32 API宣言クラス
    /// </summary>
    /// <remarks>
    /// EUDC ＝ 外字
    /// </remarks>
    public class EUDCWin32
    {
        /// <summary>
        /// sdk32:外字ファイルを更新する方法
        /// http://support.microsoft.com/kb/413278/ja
        /// 外字使用の中止→外字ファイルをコピー→外字使用の開始
        /// 
        /// Session0に外字がロードされていない場合
        /// http://www.nec.co.jp/middle/WebSAM/products/Rakuform/qabody2.html
        /// 外字使用の中止→外字使用の開始
        /// </summary>
        /// <param name="fEnableEUDC">
        /// true:外字使用の開始
        /// false:外字使用の中止
        /// </param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool EnableEUDC(bool fEnableEUDC);
    }
}
