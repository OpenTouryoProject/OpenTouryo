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
//* クラス名        ：ChangeProgressParameter
//* クラス日本語名  ：ChangeProgressデリゲードの引数
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/08/01  西野 大介         新規作成
//**********************************************************************************

namespace DeployZipPackWithHTTP
{
    public class ChangeProgressParameter
    {
        /// <summary>状態</summary>
        public string Status = null;

        /// <summary>進捗値</summary>
        public int ProgressVal = 0;

        /// <summary>コンストラクタ</summary>
        /// <param name="status">状態</param>
        /// <param name="progressVal">進捗値</param>
        public ChangeProgressParameter(string status, int progressVal)
        {
            this.Status = status;
            this.ProgressVal = progressVal;
        }
    }
}
