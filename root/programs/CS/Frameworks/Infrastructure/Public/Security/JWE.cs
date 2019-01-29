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
//* クラス名        ：JWE
//* クラス日本語名  ：JWE抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/29  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWE</summary>
    public abstract class JWE
    {
        /// <summary>JWE生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWEの文字列表現</returns>
        public abstract string Create(string payloadJson);

        /// <summary>JWE検証メソッド</summary>
        /// <param name="jwtString">JWEの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public abstract bool Verify(string jwtString);        
    }
}
