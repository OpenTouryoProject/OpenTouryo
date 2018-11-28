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
//* クラス名        ：JwkSet
//* クラス日本語名  ：JwkSet
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/16  西野 大介         新規作成
//*  2018/11/28  西野 大介         kid不一致問題の解消
//**********************************************************************************

using System.IO;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JwkSet</summary>
    public class JwkSet
    {
        /// <summary>keys</summary>
        public List<JObject> keys = new List<JObject>();

        /// <summary>LoadJwkSet</summary>
        /// <param name="jwkSetFilePath">string</param>
        /// <returns>JwkSet</returns>
        public static JwkSet LoadJwkSet(string jwkSetFilePath)
        {
            return JsonConvert.DeserializeObject<JwkSet>(
                ResourceLoader.LoadAsString(jwkSetFilePath, Encoding.GetEncoding(CustomEncode.UTF_8)));
        }

        /// <summary>SaveJwkSet</summary>
        /// <param name="jwkSetFilePath">string</param>
        /// <param name="jwkSetObject">JwkSet</param>
        /// <returns>JwkSetString</returns>
        public static void SaveJwkSet(string jwkSetFilePath, JwkSet jwkSetObject)
        {
            // jwkSetObjectのセーブ
            using (StreamWriter sr = File.CreateText(jwkSetFilePath))
            {
                sr.Write(
                    JsonConvert.SerializeObject(
                        jwkSetObject,
                        new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
            }
        }

        /// <summary>GetJwkObject</summary>
        /// <param name="jwkSetObject">JwkSet</param>
        /// <param name="kid">string</param>
        /// <returns>JObject</returns>
        public static JObject GetJwkObject(JwkSet jwkSetObject, string kid)
        {
            foreach (JObject key in jwkSetObject.keys)
            {
                if ((string)key[JwtConst.kid] == kid)
                {
                    return key;
                }
            }

            return null;
        }

        /// <summary>AddJwkToJwkSet</summary>
        /// <param name="jwkSetObject">JwkSet</param>
        /// <param name="jwkObject">JObject</param>
        /// <returns>jwkSetObject</returns>
        public static JwkSet AddJwkToJwkSet(JwkSet jwkSetObject, JObject jwkObject)
        {
            // kidの重複確認
            bool exist = false;
            foreach (JObject key in jwkSetObject.keys)
            {
                // JObject値、.ToString()しないと一致しない。
                if (key[JwtConst.kid].ToString()
                    == jwkObject[JwtConst.kid].ToString())
                {
                    exist = true;
                }
            }

            if (exist)
            {
                // 既存
            }
            else
            {
                // 追加
                jwkSetObject.keys.Add(jwkObject);
            }

            return jwkSetObject;
        }
    }
}
