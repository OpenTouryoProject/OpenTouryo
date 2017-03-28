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
//* クラス名        ：HiddenButton
//* クラス日本語名  ：Clickイベントを発生させることが出来る隠しボタン（Windowアプリケーション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Windows.Forms;

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
