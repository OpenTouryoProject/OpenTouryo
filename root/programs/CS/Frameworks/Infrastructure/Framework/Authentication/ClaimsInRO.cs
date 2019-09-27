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
//* クラス名        ：ClaimsInRO
//* クラス日本語名  ：RequestObject中のClaimsパラメタ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/19  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>RequestObject中のClaimsパラメタ</summary>
    public class ClaimsInRO
    {
        // https://openid.net/specs/openid-connect-core-1_0.html#RequestObject
        //   "claims":
        //    {
        //     "userinfo":
        //      {
        //       "given_name": {"essential": true},
        //       "nickname": null,
        //       "email": {"essential": true},
        //       "email_verified": {"essential": true},
        //       "picture": null
        //      },
        //     "id_token":
        //      {
        //       "gender": null,
        //       "birthdate": {"essential": true},
        //       "acr": {"values": ["urn:mace:incommon:iap:silver"]}
        //      }
        //    }

        /// <summary>Claimsパラメタの実体</summary>
        private Dictionary<string, object> _claims = new Dictionary<string, object>();

        /// <summary>Claimsパラメタ</summary>
        public Dictionary<string, object> Claims
        {
            get
            {
                return this._claims;
            }
        }

        /// <summary>constructor</summary>
        /// <param name="userinfoClaims">
        /// userinfoのClaims
        /// objectは、以下をメンバに持つ匿名型
        /// essential(bool), value(string), values(string[])
        /// </param>
        /// <param name="id_tokenClaims">
        /// id_tokenのClaims
        /// objectは、以下をメンバに持つ匿名型
        /// essential(bool), value(string), values(string[])
        /// </param>
        /// <param name="id_tokenAcr">
        /// id_tokenのarc
        /// objectは、以下をメンバに持つ匿名型
        /// essential(bool), value(string), values(string[])
        /// </param>
        public ClaimsInRO(
            Dictionary<string, object> userinfoClaims,
            Dictionary<string, object> id_tokenClaims, object id_tokenAcr)
        {
            // OpenID Connect - ユーザー属性クレーム関連 - マイクロソフト系技術情報 Wiki
            //  > 格納要求 > claimsパラメタによる詳細な格納要求 > 個別のクレーム値
            // https://techinfoofmicrosofttech.osscons.jp/index.php?OpenID%20Connect%20-%20%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E5%B1%9E%E6%80%A7%E3%82%AF%E3%83%AC%E3%83%BC%E3%83%A0%E9%96%A2%E9%80%A3#q9d112d2

            #region userinfo
            if (userinfoClaims != null)
            {
                Dictionary<string, object> claims_userinfo = new Dictionary<string, object>();
                foreach (string key in userinfoClaims.Keys)
                {
                    claims_userinfo.Add(key, userinfoClaims[key]);
                }

                this._claims.Add(OAuth2AndOIDCConst.claims_userinfo, claims_userinfo);
            }
            #endregion

            #region id_token
            if (id_tokenClaims != null || id_tokenAcr != null)
            {
                Dictionary<string, object> claims_id_token = new Dictionary<string, object>();
                if (id_tokenClaims != null)
                {
                    foreach (string key in id_tokenClaims.Keys)
                    {
                        claims_id_token.Add(key, id_tokenClaims[key]);
                    }
                }

                // - acr
                if (id_tokenAcr != null)
                {
                    claims_id_token.Add(OAuth2AndOIDCConst.acr, id_tokenAcr);
                }

                this._claims.Add(OAuth2AndOIDCConst.claims_id_token, claims_id_token);
            }
            #endregion
        }
    }
}
