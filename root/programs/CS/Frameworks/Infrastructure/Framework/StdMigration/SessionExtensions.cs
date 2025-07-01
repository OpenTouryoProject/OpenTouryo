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
//* クラス名        ：SessionExtensions
//* クラス日本語名  ：System.Web.SessionState.HttpSessionStateのポーティング用クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成
//*  2018/03/29  西野 大介         .NET Standard対応で、HttpSessionのポーティング
//*  2018/12/05  西野 大介         インデクサを追加...できない（拡張メソッドでは
//*                                BinarySerializeを使用したメソッドを追加
//**********************************************************************************

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Framework.StdMigration
{
    /// <summary>System.Web.SessionState.HttpSessionStateのポーティング用クラス</summary>
    public static class SessionExtensions
    {
        #region Json
        /// <summary>SetObjectAsJson拡張メソッド</summary>
        /// <param name="session">ISession</param>
        /// <param name="key">string</param>
        /// <param name="value">object</param>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>GetObjectFromJson拡張メソッド</summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="session">ISession</param>
        /// <param name="key">string</param>
        /// <returns>typed object</returns>
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        #endregion

#if (NETCOREAPP)
#else
        #region Binary
        /// <summary>SetObjectAsBinary拡張メソッド</summary>
        /// <param name="session">ISession</param>
        /// <param name="key">string</param>
        /// <param name="value">object</param>
        public static void SetObjectAsBinary(this ISession session, string key, object value)
        {
            session.SetString(key, 
                CustomEncode.ToBase64String(
                    BinarySerialize.ObjectToBytes(value)));
        }

        /// <summary>GetObjectFromBinary拡張メソッド</summary>
        /// <param name="session">ISession</param>
        /// <param name="key">string</param>
        /// <returns>object</returns>
        public static object GetObjectFromBinary(this ISession session, string key)
        {
            return BinarySerialize.BytesToObject(
                CustomEncode.FromBase64String(
                    session.GetString(key)));
        }
        #endregion
#endif
    }
}
