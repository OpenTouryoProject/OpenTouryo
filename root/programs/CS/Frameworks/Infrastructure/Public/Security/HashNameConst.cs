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
//* クラス名        ：HashNameConst
//* クラス日本語名  ：HashNameConst
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/30  西野 大介         新規作成
//*  2018/11/13  西野 大介         名称変更
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ハッシュ名定数</summary>
    public class HashNameConst
    {
        #region ハッシュ

        /// <summary>MD5</summary>
        public const string MD5 = "MD5";

        /// <summary>SHA1</summary>
        public const string SHA1 = "SHA1";

        /// <summary>SHA256</summary>
        public const string SHA256 = "SHA256";

        /// <summary>SHA384</summary>
        public const string SHA384 = "SHA384";

        /// <summary>SHA512</summary>
        public const string SHA512 = "SHA512";
        
        #endregion
    }
}
