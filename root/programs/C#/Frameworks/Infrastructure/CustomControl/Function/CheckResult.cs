﻿//**********************************************************************************
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
//* クラス名        ：CheckResult
//* クラス日本語名  ：チェック結果格納クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

// System.Web
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>チェック結果格納クラス</summary>
    public class CheckResult
    {
        /// <summary>コントロール名</summary>
        private string _ctrlName = "";
        
        /// <summary>コントロール名</summary>
        public string CtrlName
        {
            get
            {
                return this._ctrlName;
            }

            private set
            {
                this._ctrlName = value;
            }
        }

        /// <summary>チェックエラー情報名</summary>
        private string[] _checkErrorInfo = null;

        /// <summary>チェックエラー情報名</summary>
        public string[] CheckErrorInfo
        {
            get
            {
                return this._checkErrorInfo;
            }

            set
            {
                this._checkErrorInfo = value;
            }
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="ctrlName">コントロール名</param>
        public CheckResult(string ctrlName)
        {
            this.CtrlName = ctrlName;
        }
    }
}
