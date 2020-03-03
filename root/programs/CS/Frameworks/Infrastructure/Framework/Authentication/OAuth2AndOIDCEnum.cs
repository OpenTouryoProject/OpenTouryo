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
//* クラス名        ：OAuth2AndOIDCEnum
//* クラス日本語名  ：OAuth2 / OIDCで使用する列挙型クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/06  西野 大介         新規作成
//*  2019/12/25  西野 大介         PPID対応による見直し
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2 / OIDCで使用する列挙型クラス</summary>
    public class OAuth2AndOIDCEnum
    {
        #region ResponseMode

        /// <summary>ResponseMode</summary>
        public enum ResponseMode : int
        {
            /// <summary>query</summary>
            query,

            /// <summary>fragment</summary>
            fragment,

            /// <summary>form_post</summary>
            form_post,

            /// <summary>jwt</summary>
            jwt,

            /// <summary>query.jwt</summary>
            query_jwt,

            /// <summary>fragment.jwt</summary>
            fragment_jwt,

            /// <summary>form_post.jwt</summary>
            form_post_jwt
        }

        #endregion

        #region AuthMethods

        /// <summary>AuthMethods</summary>
        public enum AuthMethods : int
        {
            /// <summary>client_secret_basic</summary>
            client_secret_basic,

            /// <summary>client_secret_post</summary>
            client_secret_post,

            /// <summary>client_secret_jwt</summary>
            client_secret_jwt,

            /// <summary>private_key_jwt</summary>
            private_key_jwt,

            /// <summary>tls_client_auth</summary>
            tls_client_auth
        }

        #endregion

        #region SubjectTypes

        /// <summary>SubjectTypes</summary>
        public enum SubjectTypes : int
        {
            /// <summary>uname</summary>
            uname,

            /// <summary>public</summary>
            @public,

            /// <summary>pairwise</summary>
            pairwise
        }

        #endregion

        #region CIBA

        #region CIBA mode

        /// <summary>CibaのMode</summary>
        public enum CibaMode : int
        {
            /// <summary>poll</summary>
            poll,

            /// <summary>ping</summary>
            ping,

            /// <summary>push</summary>
            push
        }

        #endregion

        #region CIBA state

        /// <summary>CibaのState</summary>
        public enum CibaState : int
        {
            /// <summary>
            /// 保留中
            /// </summary>
            authorization_pending,
            /// <summary>
            /// 許可された（仕様外）
            /// </summary>
            access_permitted,
            /// <summary>
            /// 拒否された
            /// </summary>
            access_denied,
            /// <summary>
            /// 期限切れ
            /// </summary>
            expired_token,
            /// <summary>
            /// Polling間隔を5秒遅らせる。
            /// </summary>
            slow_down,
            /// <summary>
            /// 見つからない（仕様外）
            /// </summary>
            not_found,
            /// <summary>
            /// データ不正（仕様外）
            /// </summary>
            irregularity_data
        }

        #endregion

        #endregion

        #region ClientInfo(仕様外)

        /// <summary>ClientType</summary>
        public enum ClientType : int
        {
            /// <summary>Confidential</summary>
            confidential,

            /// <summary>Public(SPA)</summary>
            public_spa,

            /// <summary>Public(Native)</summary>
            public_native
        }

        /// <summary>ClientMode</summary>
        public enum ClientMode : int
        {
            /// <summary>normal</summary>
            normal,

            /// <summary>fapi1</summary>
            fapi1,

            /// <summary>fapi2</summary>
            fapi2
        }

        #endregion
    }
}
