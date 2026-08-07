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
//* クラス名        ：MyException
//* クラス日本語名  ：独自エクセプション
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2011/09/12  西野 大介         画面表示せず、ログ出力のみする例外処理方式を追加
//**********************************************************************************

using System;

namespace DeployZipPackWithHTTP
{
    /// <summary>独自エクセプション</summary>
    public class MyException : Exception
    {
        /// <summary>
        /// ログ出力用
        /// </summary>
        public string ToLog
        {
            private set;
            get;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="message">メッセージ</param>
        public MyException(string message)
            : base(message) { }

        /// <summary>コンストラクタ</summary>
        /// <param name="message">メッセージ</param>
        /// <param name="toLog">ログ出力用</param>
        public MyException(string message, string toLog)
            : base(message)
        {
            // ログ出力用
            this.ToLog = toLog;
        }
    }
}
