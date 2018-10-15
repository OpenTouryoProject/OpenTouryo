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
//* クラス名        ：InstanceCreator
//* クラス日本語名  ：InstanceCreator
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/04  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Linq.Expressions;

namespace Touryo.Infrastructure.Public.FastReflection
{
    /// <summary>InstanceCreator</summary>
    /// <typeparam name="T">Instance type</typeparam>
    public static class InstanceCreator<T>
    {
        // インスタンスの生成速度
        // https://qiita.com/Temarin/items/d6f00428743b0971ec95

        /// <summary>_Factory</summary>
        private static Func<T> _Factory = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        /// <summary>Factory</summary>
        public static Func<T> Factory
        {
            get
            {
                return _Factory;
            }
        }
        
    }
}
