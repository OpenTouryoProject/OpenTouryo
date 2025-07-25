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
//* クラス名        ：UserInfoHandle
//* クラス日本語名  ：ユーザの情報を扱うクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2009/03/13  西野 大介         ラベル名のみ変更
//*  2009/04/13  西野 大介         削除メソッドを追加
//*  2018/03/29  西野 大介         .NET Standard対応で、HttpSessionのポーティング
//**********************************************************************************

#if (NETSTD || NETCOREAPP)
using Touryo.Infrastructure.Framework.StdMigration;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>ユーザの情報を扱うクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class UserInfoHandle
    {
        #region UserInformation

        /// <summary>ユーザ情報をSessionに保存する。</summary>
        /// <param name="userInfo">ユーザ情報</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void SetUserInformation(UserInfo userInfo)
        {
            // Sessionに保存
#if (NETSTD || NETCOREAPP)
            ISession session = MyHttpContext.Current.Session;
            session.SetObjectAsJson(FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION, userInfo);
#else
            HttpContext.Current.Session[FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION] = userInfo;
#endif
        }

#if (NETSTD || NETCOREAPP)

        /// <summary>ユーザ情報をSessionから取得する。</summary>
        /// <returns>ユーザ情報</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static T GetUserInformation<T>() where T : UserInfo
        {
            // Sessionから取得
            ISession session = MyHttpContext.Current.Session;
            return session.GetObjectFromJson<T>(FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION);
        }

#else
        /// <summary>ユーザ情報をSessionから取得する。</summary>
        /// <returns>ユーザ情報</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static UserInfo GetUserInformation()
        {
            // Sessionから取得
            return (UserInfo)HttpContext.Current.Session[FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION];
        }
#endif

        /// <summary>ユーザ情報をSessionから削除する。</summary>
        /// <remarks>自由に利用できる。</remarks>
        public static void DeleteUserInformation()
        {
            // Sessionから削除
#if (NETSTD || NETCOREAPP)
            ISession session = MyHttpContext.Current.Session;
            session.Remove(FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION);
#else
            HttpContext.Current.Session.Remove(FxHttpSessionIndex.AUTHENTICATION_USER_INFORMATION);
#endif
        }

        #endregion
    }
}
