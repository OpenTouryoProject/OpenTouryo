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
//* クラス名        ：BaseViewModel
//* クラス日本語名  ：BaseViewModel
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Util;

namespace MVC_Sample.Models.ViewModels
{
    /// <summary>BaseViewModel</summary>
    [Serializable]
    public class BaseViewModel
    {

        /// <summary>UserName</summary>
        public static string UserName
        {
            get
            {
                //MyUserInfo myUserInfo = (MyUserInfo)UserInfoHandle.GetUserInformation();

                // ClaimsIdentityを使用できるようになったのでMyUserInfoは卒業。

                AuthenticateResult authenticateInfo =
                    AuthenticationHttpContextExtensions.AuthenticateAsync(
                        MyHttpContext.Current, CookieAuthenticationDefaults.AuthenticationScheme).Result;
                
                string userName = authenticateInfo.Principal?.Identity?.Name; // null 条件演算子

                if (string.IsNullOrEmpty(userName))
                {
                    return "anonymous";
                }
                else
                {
                    return userName;
                }
                
            }
        }
    }
}