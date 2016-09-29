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
//* クラス名        ：WebUserControl
//* クラス日本語名  ：WebUserControl上のイベントハンドラをハンドルする。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************
// System
using System;
using System.Data;
using System.Configuration;
using System.Collections;

// System.Web
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// Touryo
using Touryo.Infrastructure.Framework.Presentation;

namespace ProjectX_sample.Aspx.Common
{
    /// <summary>WebUserControl class</summary>
    public partial class WebUserControl : System.Web.UI.UserControl
    {
        /// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnUCButton_Click(FxEventArgs fxEventArgs)
        {
            Response.Write("UOC_btnUCButton_Clickを実行できた。");

            return "";
        }
    } 
}
