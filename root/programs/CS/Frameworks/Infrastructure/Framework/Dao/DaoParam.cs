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
//* クラス名        ：DaoParam
//* クラス日本語名  ：DaoParam
//*
//* 作成者          ：西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/06/20  西野 大介         新規作成
//**********************************************************************************

using System.Data;

namespace Touryo.Infrastructure.Framework.Dao
{
    /// <summary>DaoParam</summary>
    public class DaoParam
    {
        /// <summary>Value</summary>
        public object Value;

        /// <summary>DbType</summary>
        public object DbType;

        /// <summary>Size</summary>
        public int Size;

        /// <summary>ParameterDirection</summary>
        public ParameterDirection Direction;

        /// <summary>constructor</summary>
        /// <param name="value">object</param>
        /// <param name="dbType">object</param>
        /// <param name="size">int</param>
        /// <param name="direction">ParameterDirection</param>
        public DaoParam(object value, object dbType,
            int size, ParameterDirection direction)
        {
            this.Value = value;
            this.DbType = dbType;
            this.Size = size;
            this.Direction = direction;
        }
    }
}
