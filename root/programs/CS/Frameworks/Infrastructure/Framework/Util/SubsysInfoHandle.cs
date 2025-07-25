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
//* クラス名        ：SubsysInfoHandle
//* クラス日本語名  ：サブシステムの情報を扱うクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/09/01  西野 大介         新規作成
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
    /// <summary>サブシステムの情報を扱うクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class SubsysInfoHandle
    {
        /// <summary>サブシステム情報をSessionに保存する。</summary>
        /// <param name="subsysInfo">サブシステム情報</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void SetSubsysInformation(SubsysInfo subsysInfo)
        {
            // Sessionに保存
#if (NETSTD || NETCOREAPP)
            ISession session = MyHttpContext.Current.Session;
            session.SetObjectAsJson(FxHttpSessionIndex.SUB_SYSTEM_INFORMATION, subsysInfo);
#else
            HttpContext.Current.Session[FxHttpSessionIndex.SUB_SYSTEM_INFORMATION] = subsysInfo;
#endif            
        }

        /// <summary>サブシステム情報をSessionから取得する。</summary>
        /// <returns>サブシステム情報</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static SubsysInfo GetSubsysInformation()
        {
            // Sessionから取得
#if (NETSTD || NETCOREAPP)
            ISession session = MyHttpContext.Current.Session;
            return session.GetObjectFromJson<SubsysInfo>(FxHttpSessionIndex.SUB_SYSTEM_INFORMATION);
#else
            return (SubsysInfo)HttpContext.Current.Session[FxHttpSessionIndex.SUB_SYSTEM_INFORMATION];
#endif            
        }

        /// <summary>サブシステム情報をSessionから削除する。</summary>
        /// <remarks>自由に利用できる。</remarks>
        public static void DeleteSubsysInformation()
        {
            // Sessionから削除
#if (NETSTD || NETCOREAPP)
            ISession session = MyHttpContext.Current.Session;
            session.Remove(FxHttpSessionIndex.SUB_SYSTEM_INFORMATION);
#else
            HttpContext.Current.Session.Remove(FxHttpSessionIndex.SUB_SYSTEM_INFORMATION);
#endif
        }
    }
}
