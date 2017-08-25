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
//* クラス名        ：LoginViewModel
//* クラス日本語名  ：LoginViewModel
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.ComponentModel.DataAnnotations;

namespace MVC_Sample.Models.ViewModels
{
    /// <summary>LoginViewModel</summary>
    public class LoginViewModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// PWDS
        /// </summary>
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}