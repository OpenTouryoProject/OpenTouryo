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
//* クラス名        ：BaseConsolidateDao
//* クラス日本語名  ：Dao集約クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2012/06/14  西野 大介         abstract属性を付与した。
//**********************************************************************************

using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Dao
{
    /// <summary>Dao集約クラスのベースクラスの例</summary>
    public abstract class BaseConsolidateDao
    {
        /// <summary>データアクセス制御クラス</summary>
        private BaseDam _dam;

        /// <summary>データアクセス制御クラス</summary>
        protected BaseDam Dam
        {
            get { return this._dam; }
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="dam">データアクセス制御クラス</param>
        public BaseConsolidateDao(BaseDam dam)
        {
            this._dam = dam;
        }
    }
}
