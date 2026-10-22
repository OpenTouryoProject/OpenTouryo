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
//* クラス名        ：CmnServerParams
//* クラス日本語名  ：SAML2, OAuth2/OIDC の各種 Server（IdP / 認可サーバ）側パラメタ
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した IdP 側の処理が読む設定。#634）
//**********************************************************************************

using System;
using System.Globalization;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2, OAuth2/OIDC の各種 Server（IdP / 認可サーバ）側パラメタ</summary>
    /// <remarks>
    /// CmnClientParams（SpRp_*。クライアント役が読む、相手の値と自分の鍵）の対である。
    /// こちらは **IdP / 認可サーバ自身の値**で、SP・RP 側と対になる IdP 側の処理の、設定を読む版が使う。
    ///
    /// **キー名は、汎用認証サイトの設定と同じにしている**（SpRp_ を付けない）。
    /// 汎用認証サイトの設定が、そのまま効く。
    /// 認可サーバとクライアントを兼ねる配備でも、SpRp_* とは別のキーなので、相手の値と自分の値を両方置ける。
    /// </remarks>
    public class CmnServerParams
    {
        #region 自分（IdP / 認可サーバ）の値

        /// <summary>IssuerId（自分の Issuer。SAML2 では entityID）</summary>
        public static string IssuerId
        {
            get
            {
                return GetConfigParameter.GetConfigValue("IssuerId");
            }
        }

        #endregion

        #region 鍵

        /// <summary>RsaPfxFilePath（署名に使う、自分の秘密鍵の *.pfx）</summary>
        public static string RsaPfxFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("RsaPfxFilePath");
            }
        }

        /// <summary>RsaPfxPassword（RsaPfxFilePath のパスワード）</summary>
        public static string RsaPfxPassword
        {
            get
            {
                return GetConfigParameter.GetConfigValue("RsaPfxPassword");
            }
        }

        /// <summary>PreviousSigningCerFilePaths（退役させる途中の署名証明書の *.cer。空白かカンマ区切り）</summary>
        /// <remarks>
        /// 鍵を入れ替える（ロールオーバーする）間、SAML2 のメタデータに新旧の証明書を並べて広告するために使う
        /// （SAML2Bindings.AppendSigningCertificates の設定を読む版）。空なら、広告するのはいまの 1 本だけ。
        /// </remarks>
        public static string PreviousSigningCerFilePaths
        {
            get
            {
                return GetConfigParameter.GetConfigValue("PreviousSigningCerFilePaths");
            }
        }

        #endregion

        #region SAML2

        /// <summary>Saml2AssertionExpireTimeSpanFromMinutes（アサーションの有効期間（分））</summary>
        /// <remarks>無い・数値でなければ FrameworkException（応答を、黙って既定値で作らないため）。</remarks>
        public static double Saml2AssertionExpireTimeSpanFromMinutes
        {
            get
            {
                const string key = "Saml2AssertionExpireTimeSpanFromMinutes";
                string value = GetConfigParameter.GetConfigValue(key);

                double minutes;
                if (string.IsNullOrEmpty(value)
                    || !double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out minutes)
                    || minutes <= 0)
                {
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_CONFIG_VALUE[0],
                        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_CONFIG_VALUE[1], key));
                }

                return minutes;
            }
        }

        #endregion

        #region OAuth2

        /// <summary>RequirePkceS256（PKCE で S256 だけを受けるか。plain を断る）</summary>
        /// <remarks>無ければ false（plain も受ける）。true / false 以外は FrameworkException。</remarks>
        public static bool RequirePkceS256
        {
            get
            {
                const string key = "RequirePkceS256";
                string value = GetConfigParameter.GetConfigValue(key);

                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                bool require;
                if (!bool.TryParse(value, out require))
                {
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH2[0],
                        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH2[1], key));
                }

                return require;
            }
        }

        #endregion

        #region 必須の値

        /// <summary>必須の設定値を読む（無ければ FrameworkException）</summary>
        /// <param name="key">string（設定キー）</param>
        /// <param name="value">string（読んだ値）</param>
        /// <returns>値</returns>
        internal static string Required(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_CONFIG_VALUE[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_CONFIG_VALUE[1], key));
            }

            return value;
        }

        #endregion
    }
}
