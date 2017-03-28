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
//* クラス名        ：CColumn
//* クラス日本語名  ：カラムクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2012/11/21  西野 大介         Entity、DataSet自動生成の対応
//**********************************************************************************

namespace DaoGen_Tool
{
    /// <summary>カラムクラス</summary>
    public class CColumn
    {
        /// <summary>カラム名</summary>
        public string Name = "";

        /// <summary>キーかどうか</summary>
        public bool IsKey = false;

        /// <summary>DB型情報</summary>
        public string DBTypeInfo = "";

        /// <summary>.NET型情報</summary>
        public string DotNetTypeInfo = "";

        /// <summary>コンストラクタ</summary>
        /// <param name="name">テーブル名</param>
        /// <param name="dbTypeInfo">DB型情報</param>
        /// <param name="dotNetTypeInfo">.NET型情報</param>
        public CColumn(string name, string dbTypeInfo, string dotNetTypeInfo)
        {
            this.Name = name;
            this.DBTypeInfo = dbTypeInfo;
            this.DotNetTypeInfo = dotNetTypeInfo;
        }
    }
}
