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
//* クラス名        ：Informations
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッド用の情報データ・コントラクト
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest
{
    /// <summary>WCFサービスの引数・戻り値</summary>
    [DataContract]
    public class Informations
    {
        /// <summary>文字列情報</summary>
        [DataMember]
        public string Str = null;

        /// <summary>Dictionary情報</summary>
        [DataMember]
        public Dictionary<string, string> Dictionary = null;

        /// <summary>Dictionary List、もしくは二次元表情報</summary>
        [DataMember]
        public List<Dictionary<string, string>> DicList = null;

        /// <summary>Dictionary List配列、もしくは二次元表配列情報</summary>
        [DataMember]
        public List<Dictionary<string, string>>[] DicLists = null;

        #region コンストラクタ
        
        /// <summary>コンストラクタ</summary>
        public Informations()
        {
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="str">文字列情報</param>
        public Informations(string str)
        {
            this.Str = str;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="dictionary">Dictionary情報</param>
        public Informations(Dictionary<string, string> dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="dicList">
        /// Dictionary List、もしくは二次元表情報
        /// </param>
        public Informations(List<Dictionary<string, string>> dicList)
        {
            this.DicList = dicList;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="dicLists">
        /// Dictionary List配列、もしくは二次元表配列情報
        /// </param>
        public Informations(List<Dictionary<string, string>>[] dicLists)
        {
            this.DicLists = dicLists;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="str">文字列情報</param>
        /// <param name="dictionary">Dictionary情報</param>
        public Informations(
            string str,
            Dictionary<string, string> dictionary)
        {
            this.Str = str;
            this.Dictionary = dictionary;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="str">文字列情報</param>
        /// <param name="dictionary">Dictionary情報</param>
        /// <param name="dicList">Dictionary List、もしくは二次元表情報</param>
        public Informations(
            string str,
            Dictionary<string, string> dictionary,
            List<Dictionary<string, string>> dicList)
        {
            this.Str = str;
            this.Dictionary = dictionary;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="str">文字列情報</param>
        /// <param name="dictionary">Dictionary情報</param>
        /// <param name="dicList">Dictionary List、もしくは二次元表情報</param>
        /// <param name="dicLists">Dictionary List配列、もしくは二次元表配列情報</param>
        public Informations(
            string str,
            Dictionary<string, string> dictionary,
            List<Dictionary<string, string>> dicList,
            List<Dictionary<string, string>>[] dicLists)
        {
            this.Str = str;
            this.Dictionary = dictionary;
            this.DicList = dicList;
            this.DicLists = dicLists;
        }

        #endregion
    }
}
