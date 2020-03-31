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
//*  2019/12/25  西野 大介         PPID対応による見直し
//**********************************************************************************

//using Touryo.Infrastructure.Public.FastReflection;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2で使用する列挙型クラス</summary>
    public class SAML2Enum
    {
        #region Enum
        /// <summary>RequestOrResponse</summary>
        public enum RequestOrResponse : int
        {
            /// <summary>Request</summary>
            Request,

            /// <summary>Response</summary>
            Response
        }

        /// <summary>SamlSchema</summary>
        public enum SamlSchema : int
        {
            /// <summary>Request</summary>
            Request,

            ///// <summary>Assertion</summary>
            //Assertion,

            /// <summary>Response</summary>
            Response
        }

        /// <summary>NameIDFormat</summary>
        public enum NameIDFormat : int
        {
            /// <summary>unspecified(UserName or E-Mail or UserID)</summary>
            Unspecified,

            /// <summary>emailAddress(E-Mail)</summary>
            EmailAddress,

            /// <summary>persistent(PPID)</summary>
            Persistent, // UserIDをPPID化

            /// <summary>transient</summary>
            Transient // 現状、サポート無し
        }

        /// <summary>ProtocolBinding</summary>
        public enum ProtocolBinding : int
        {
            /// <summary>HttpPost</summary>
            HttpPost,

            /// <summary>HttpRedirect</summary>
            HttpRedirect
        }

        /// <summary>AuthnContextClassRef</summary>
        public enum AuthnContextClassRef : int
        {
            /// <summary>unspecified</summary>
            Unspecified,

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
        #endregion

        #region Enum ⇔ String

        #region EnumToString
        /// <summary>EnumToString(NameIDFormat)</summary>
        /// <param name="nameIDFormat">NameIDFormat</param>
        /// <returns>string</returns>
        public static string EnumToString(NameIDFormat nameIDFormat)
        {
            string ret = "";
            switch (nameIDFormat)
            {
                case SAML2Enum.NameIDFormat.Unspecified:
                    ret = SAML2Const.UrnNameIDFormatUnspecified;
                    break;
                case SAML2Enum.NameIDFormat.EmailAddress:
                    ret = SAML2Const.UrnNameIDFormatEmailAddress;
                    break;
                case SAML2Enum.NameIDFormat.Persistent:
                    ret = SAML2Const.UrnNameIDFormatPersistent;
                    break;
                case SAML2Enum.NameIDFormat.Transient:
                    ret = SAML2Const.UrnNameIDFormatTransient;
                    break;
            }

            return ret;
        }

        /// <summary>EnumToString(NameIDFormat)</summary>
        /// <param name="protocolBinding">ProtocolBinding</param>
        /// <returns>string</returns>
        public static string EnumToString(ProtocolBinding protocolBinding)
        {
            string ret = "";
            switch (protocolBinding)
            {
                case SAML2Enum.ProtocolBinding.HttpPost:
                    ret = SAML2Const.UrnBindingsPost;
                    break;
                case SAML2Enum.ProtocolBinding.HttpRedirect:
                    ret = SAML2Const.UrnBindingsRedirect;
                    break;
            }

            return ret;
        }

        /// <summary>EnumToString(AuthnContextClassRef)</summary>
        /// <param name="authnContextClassRef">StatusCode</param>
        /// <returns>string</returns>
        public static string EnumToString(AuthnContextClassRef authnContextClassRef)
        {
            string ret = "";
            switch (authnContextClassRef)
            {
                case SAML2Enum.AuthnContextClassRef.Unspecified:
                    ret = SAML2Const.UrnAuthnContextClassRefUnspecified;
                    break;
                case SAML2Enum.AuthnContextClassRef.Password:
                    ret = SAML2Const.UrnAuthnContextClassRefPassword;
                    break;
                case SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport:
                    ret = SAML2Const.UrnAuthnContextClassRefPasswordProtectedTransport;
                    break;
                case SAML2Enum.AuthnContextClassRef.PreviousSession:
                    ret = SAML2Const.UrnAuthnContextClassRefPreviousSession;
                    break;
                case SAML2Enum.AuthnContextClassRef.X509:
                    ret = SAML2Const.UrnAuthnContextClassRefX509;
                    break;
            }

            return ret;
        }

        /// <summary>EnumToString(StatusCode)</summary>
        /// <param name="statusCode">StatusCode</param>
        /// <returns>string</returns>
        public static string EnumToString(StatusCode statusCode)
        {
            string ret = "";
            switch (statusCode)
            {
                case SAML2Enum.StatusCode.Success:
                    ret = SAML2Const.UrnStatusCodeSuccess;
                    break;
                case SAML2Enum.StatusCode.Requester:
                    ret = SAML2Const.UrnStatusCodeRequester;
                    break;
                case SAML2Enum.StatusCode.Responder:
                    ret = SAML2Const.UrnStatusCodeResponder;
                    break;
                case SAML2Enum.StatusCode.AuthnFailed:
                    ret = SAML2Const.UrnStatusCodeAuthnFailed;
                    break;
                case SAML2Enum.StatusCode.UnknownPrincipal:
                    ret = SAML2Const.UrnStatusCodeUnknownPrincipal;
                    break;
                case SAML2Enum.StatusCode.VersionMismatch:
                    ret = SAML2Const.UrnStatusCodeVersionMismatch;
                    break;
            }

            return ret;
        }
        #endregion

        #region StringToEnum
        /// <summary>StringToEnum</summary>
        /// <param name="str">string</param>
        /// <param name="nameIDFormat">NameIDFormat?</param>
        public static void StringToEnum(string str, out NameIDFormat? nameIDFormat)
        {
            if (str == SAML2Const.UrnNameIDFormatUnspecified)
            {
                nameIDFormat = NameIDFormat.Unspecified;
            }
            else if (str == SAML2Const.UrnNameIDFormatEmailAddress)
            {
                nameIDFormat = NameIDFormat.EmailAddress;
            }
            else if (str == SAML2Const.UrnNameIDFormatPersistent)
            {
                nameIDFormat = NameIDFormat.Persistent;
            }
            else if (str == SAML2Const.UrnNameIDFormatTransient)
            {
                nameIDFormat = NameIDFormat.Transient;
            }
            else
            {
                nameIDFormat = null;
            }
        }

        /// <summary>StringToEnum</summary>
        /// <param name="str">string</param>
        /// <param name="protocolBinding">ProtocolBinding?</param>
        public static void StringToEnum(string str, out ProtocolBinding? protocolBinding)
        {
            if (str == SAML2Const.UrnBindingsRedirect)
            {
                protocolBinding = ProtocolBinding.HttpRedirect;
            }
            else if (str == SAML2Const.UrnBindingsPost)
            {
                protocolBinding = ProtocolBinding.HttpPost;
            }
            else
            {
                protocolBinding = null;
            }
        }

        /// <summary>StringToEnum</summary>
        /// <param name="str">string</param>
        /// <param name="authnContextClassRef">AuthnContextClassRef?</param>
        public static void StringToEnum(string str, out AuthnContextClassRef? authnContextClassRef)
        {
            if (str == SAML2Const.UrnAuthnContextClassRefUnspecified)
            {
                authnContextClassRef = AuthnContextClassRef.Unspecified;
            }
            else if (str == SAML2Const.UrnAuthnContextClassRefPassword)
            {
                authnContextClassRef = AuthnContextClassRef.Password;
            }
            else if (str == SAML2Const.UrnAuthnContextClassRefPasswordProtectedTransport)
            {
                authnContextClassRef = AuthnContextClassRef.PasswordProtectedTransport;
            }
            else if (str == SAML2Const.UrnAuthnContextClassRefPreviousSession)
            {
                authnContextClassRef = AuthnContextClassRef.PreviousSession;
            }
            else if (str == SAML2Const.UrnAuthnContextClassRefX509)
            {
                authnContextClassRef = AuthnContextClassRef.X509;
            }
            else
            {
                authnContextClassRef = null;
            }
        }

        /// <summary>StringToEnum</summary>
        /// <param name="str">string</param>
        /// <param name="statusCode">StatusCode?</param>
        public static void StringToEnum(string str, out StatusCode? statusCode)
        {
            if (str == SAML2Const.UrnStatusCodeSuccess)
            {
                statusCode = StatusCode.Success;
            }
            else if (str == SAML2Const.UrnStatusCodeRequester)
            {
                statusCode = StatusCode.Requester;
            }
            else if (str == SAML2Const.UrnStatusCodeResponder)
            {
                statusCode = StatusCode.Responder;
            }
            else if (str == SAML2Const.UrnStatusCodeAuthnFailed)
            {
                statusCode = StatusCode.AuthnFailed;
            }
            else if (str == SAML2Const.UrnStatusCodeUnknownPrincipal)
            {
                statusCode = StatusCode.UnknownPrincipal;
            }
            else if (str == SAML2Const.UrnStatusCodeVersionMismatch)
            {
                statusCode = StatusCode.VersionMismatch;
            }
            else
            {
                statusCode = null;
            }
        }
        #endregion

        #endregion
    }
}
