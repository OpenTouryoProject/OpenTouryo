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
//* クラス名        ：JWS_PS384
//* クラス日本語名  ：JWS PS384生成クラス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/03  玄人 幸道         新規作成（#596）
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWS PS384生成クラス</summary>
    /// <remarks>
    /// RSASSA-PSS using SHA-384 and MGF1 with SHA-384（RFC 7518 3.5）
    /// 鍵は JWS_RS384 と同じ RSA 鍵を使う（パディングだけが違う）。
    /// </remarks>
    public abstract class JWS_PS384 : JWS_RSA
    {
        /// <summary>constructor</summary>
        public JWS_PS384()
        {
            this.Init(JwtConst.PS384);
        }
    }
}
