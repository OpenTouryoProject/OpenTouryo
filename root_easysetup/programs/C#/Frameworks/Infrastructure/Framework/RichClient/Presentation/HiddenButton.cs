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
//* クラス名        ：HiddenButton
//* クラス日本語名  ：Clickイベントを発生させることが出来る隠しボタン（Windowアプリケーション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  西野  大介        新規作成
//**********************************************************************************

using System.Reflection;

// System
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

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

using Touryo.Infrastructure.Framework.RichClient.Util;

namespace Touryo.Infrastructure.Framework.RichClient.Presentation
{
    /// <summary>Clickイベントを発生させることが出来る隠しボタン</summary>
    public class HiddenButton : Button
    {
        /// <summary>コンストラクタ</summary>
        public HiddenButton()
        {
            // 見えないボタン
            this.Visible = false;
        }

        /// <summary>Clickイベントを発生させる</summary>
        public void DoClick()
        {
            this.OnClick(new EventArgs());
        }
    }

}
