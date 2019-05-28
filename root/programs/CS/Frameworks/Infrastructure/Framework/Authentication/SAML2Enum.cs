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
//* クラス名        ：SAML2Enum
//* クラス日本語名  ：SAML2で使用する列挙型クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/21  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2で使用する列挙型クラス</summary>
    public class SAML2Enum
    {
        /// <summary>NameIDFormat</summary>
        public enum NameIDFormat : int
        {
            /// <summary>unspecified</summary>
            unspecified,

            /// <summary>persistent</summary>
            persistent,

            /// <summary>transient</summary>
            transient
        }

        /// <summary>ProtocolBinding</summary>
        public enum ProtocolBinding : int
        {
            /// <summary>HttpPost</summary>
            HttpPost,

            /// <summary>HttpRedirect</summary>
            HttpRedirect
        }

        /// <summary>RequestOrResponse</summary>
        public enum RequestOrResponse : int
        {
            /// <summary>Request</summary>
            Request,

            /// <summary>Response</summary>
            Response
        }

        /// <summary>AuthnContextClassRef</summary>
        public enum AuthnContextClassRef : int
        {
            /// <summary>unspecified</summary>
            unspecified,

            /// <summary>Password</summary>
            Password,

            /// <summary>PasswordProtectedTransport</summary>
            PasswordProtectedTransport,

            /// <summary>PreviousSession</summary>
            PreviousSession,

            /// <summary>X509</summary>
            X509
        }

        /// <summary>StatusCode</summary>
        public enum StatusCode : int
        {
            /// <summary>Success</summary>
            Success,
            
            /// <summary>Requester</summary>
            Requester,

            /// <summary>Responder</summary>
            Responder,

            /// <summary>AuthnFailed</summary>
            AuthnFailed,

            /// <summary>UnknownPrincipal</summary>
            UnknownPrincipal,

            /// <summary>VersionMismatch</summary>
            VersionMismatch
        }
    }
}
