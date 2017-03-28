//**********************************************************************************
//* Copyright (C) 2007,2017 Hitachi Solutions,Ltd.
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
//* クラス名        ：JWT
//* クラス日本語名  ：JWT抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Util.JWT
{
    /// <summary>JWT Header</summary>
    public class Header
    {
        /// <summary>
        /// alg=HS256 or RS256
        /// JWTのデジタル署名アルゴリズムを指定する。
        /// HS256 or RS256の署名アルゴリズムのみサポート。
        /// </summary>
        public string alg = "";

        /// <summary>
        /// typ=JWT
        /// アサーションのタイプを指定。JWT固定。
        /// </summary>
        public readonly string typ = "JWT";
    }

    /// <summary>JWT</summary>
    public abstract class JWT
    {
        /// <summary>JWT生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWTの文字列表現</returns>
        public abstract string Create(string payloadJson);

        /// <summary>JWT検証メソッド</summary>
        /// <param name="jwtString">JWTの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public abstract bool Verify(string jwtString);
        
    }
}
