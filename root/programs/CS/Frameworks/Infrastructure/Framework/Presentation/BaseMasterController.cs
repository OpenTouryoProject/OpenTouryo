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
//* クラス名        ：BaseMasterController
//* クラス日本語名  ：マスタ ページ親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/07/21  西野 大介        新規作成
//*  2009/07/21  西野 大介        コントロール取得処理の仕様変更
//*  2009/07/21  西野 大介        マスタ ページのネストに対応
//**********************************************************************************

using System.Collections;

namespace Touryo.Infrastructure.Framework.Presentation
{
    /// <summary>マスターページのProtectメンバの情報を取得するために作成</summary>
    public class BaseMasterController : System.Web.UI.MasterPage
    {
        /// <summary>ContentPlaceHolderコントロールのリストを取得</summary>
        public IList ContentPlaceHolders2
        {   
            get
            {
                return this.ContentPlaceHolders;
            }
        }
    }
}
