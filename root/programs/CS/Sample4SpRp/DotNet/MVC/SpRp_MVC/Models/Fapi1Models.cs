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
//* クラス名        ：Fapi1Session・Fapi1Result
//* クラス日本語名  ：FAPI1 のフローの、セッションに置く値と、結果の画面のモデル
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668）
//*  2026/11/09  玄人 幸道         FAPI2 でも使う。結果に Profile（FAPI 1.0 / 2.0）と、預け先（PAR・/ros）を足した（#668 の追加）
//**********************************************************************************

namespace SpRp_MVC.Models
{
    /// <summary>認可要求から戻りまでの間だけ、セッションに置く値</summary>
    public class Fapi1Session
    {
        /// <summary>flow（FAPI1 : oauth2 / oidc / pkce、FAPI2 : ros / par）</summary>
        public string Flow { get; set; }

        /// <summary>client_id（flow で変わる）</summary>
        public string ClientId { get; set; }

        /// <summary>response_mode（空なら query）</summary>
        public string ResponseMode { get; set; }

        /// <summary>state</summary>
        public string State { get; set; }

        /// <summary>nonce（oauth2 では null）</summary>
        public string Nonce { get; set; }

        /// <summary>code_verifier（pkce だけ）</summary>
        public string CodeVerifier { get; set; }
    }

    /// <summary>結果の画面</summary>
    /// <remarks>**トークンそのものは出さない**（クレームと UserInfo だけ）。</remarks>
    public class Fapi1Result
    {
        /// <summary>成功したか</summary>
        public bool Ok { get; set; }

        /// <summary>失敗の理由</summary>
        public string Error { get; set; }

        /// <summary>プロファイル（FAPI 1.0 / FAPI 2.0）</summary>
        public string Profile { get; set; } = "FAPI 1.0";

        /// <summary>認可要求の預け先（FAPI2 : PAR / /ros。FAPI1 は空）</summary>
        public string PushedTo { get; set; }

        /// <summary>id_token の形（FAPI2 : JWS / JWE の中の JWS。FAPI1 は空）</summary>
        public string IdTokenForm { get; set; }

        /// <summary>flow</summary>
        public string Flow { get; set; }

        /// <summary>response_mode</summary>
        public string ResponseMode { get; set; }

        /// <summary>トークン要求のクライアント認証</summary>
        public string AuthMethod { get; set; }

        /// <summary>token_type</summary>
        public string TokenType { get; set; }

        /// <summary>id_token のクレーム（JSON）</summary>
        public string IdTokenClaims { get; set; }

        /// <summary>UserInfo（JSON）</summary>
        public string UserInfo { get; set; }
    }
}
