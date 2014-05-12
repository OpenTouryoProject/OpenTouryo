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
//* クラス名        ：Aspx_Common_testAspNetAjaxExtension_Single
//* クラス日本語名  ：Ajaxテスト用のマスタ ページ（updateパネルを親から纏めて使用）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using Touryo.Infrastructure.Framework.Presentation;

/// <summary>Ajaxテスト用のマスタ ページ（updateパネルを親から纏めて使用）</summary>
public partial class Aspx_Common_testAspNetAjaxExtension_Single : BaseMasterController
{
    /// <summary>btnMButton1のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
    {
        // テキストボックスの値を変更
        this.TextBox1.Text = "ajaxのポストバック（ボタンクリック）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>btnMButton2のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_btnMButton2_Click(FxEventArgs fxEventArgs)
    {
        // テキストボックスの値を変更
        this.TextBox2.Text = "通常のポストバック（ボタンクリック）";

        return "";
    }

    /// <summary>
    /// ddlMDropDownList1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_ddlMDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // テキストボックスの値を変更
        this.TextBox3.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>
    /// ddlMDropDownList2のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_ddlMDropDownList2_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // テキストボックスの値を変更
        this.TextBox4.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        return "";
    }

    /// <summary>btnMButton3のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_btnMButton3_Click(FxEventArgs fxEventArgs)
    {
        throw new Exception("Ajaxでエラー");

        //return "";
    }
}
