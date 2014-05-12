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
//* クラス名        ：Aspx_Common_testScreenCtrl
//* クラス日本語名  ：画面遷移制御機能テスト画面用のマスタ ページ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using Touryo.Infrastructure.Framework.Presentation;

/// <summary>画面遷移制御機能テスト画面用のマスタ ページ</summary>
public partial class Aspx_Common_testScreenCtrl : BaseMasterController
{
    /// <summary>
    /// btnMButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_btnMButton1_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm0";
    }

    //---

    /// <summary>
    /// lbnMLinkButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_lbnMLinkButton1_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm3";
    }

    /// <summary>
    /// lbnMLinkButton2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_lbnMLinkButton2_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm1";
    }

    /// <summary>
    /// lbnMLinkButton3のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_lbnMLinkButton3_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm2";
    }

    /// <summary>
    /// lbnMLinkButton4のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_lbnMLinkButton4_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm4";
    }

    /// <summary>
    /// lbnMLinkButton5のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    public string UOC_lbnMLinkButton5_Click(FxEventArgs fxEventArgs)
    {

        return "WebForm5";
    }
}
