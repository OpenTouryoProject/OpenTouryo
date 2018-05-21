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
//* クラス名        ：IEdit
//* クラス日本語名  ：編集処理に関するプロパティ グリッドのインターフェイス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System.ComponentModel;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>編集処理に関するプロパティ グリッドのインターフェイス（テンプレート）</summary>
    public interface IEdit
    {
        /// <summary>初期値編集</summary>
        [Category("Edit"),
        Description("初期値編集")]
        EditInitialValue EditInitialValue { get; set; }

        /// <summary>初期値編集のチェックのデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        bool ShouldSerializeEditInitialValue();
        
        /// <summary>桁区切り編集</summary>
        [Category("Edit"),
        Description("桁区切り編集")]
        EditAddFigure EditAddFigure { get; set; }

        /// <summary>桁区切り編集のチェックのデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        bool ShouldSerializeEditAddFigure();

        /// <summary>文字埋め編集</summary>
        [Category("Edit"),
        Description("文字埋め編集")]
        EditPadding EditPadding { get; set; }

        /// <summary>文字埋め編集のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        bool ShouldSerializeEditPadding();

        /// <summary>小数点以下ｘ桁編集</summary>
        [Category("Edit"),
        Description("小数点以下ｘ桁編集")]
        EditDigitsAfterDP EditDigitsAfterDP { get; set; }

        /// <summary>文字埋め編集のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        bool ShouldSerializeEditDigitsAfterDP();
    }
}
