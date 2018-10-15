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
//* クラス名        ：CompiledExpressionCreater
//* クラス日本語名  ：CompiledExpressionCreater
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
    /// <summary>CompiledExpressionCreater</summary>
    public class CompiledExpressionCreater
    {
        // neue cc - Expression Treeのこね方・入門編 - 動的にデリゲートを生成してリフレクションを高速化
        // http://neue.cc/2011/04/20_317.html
        
        /// <summary>CompiledExpressionでGetterを生成して返す。</summary>
        /// <param name="type">Type</param>
        /// <param name="memberName">string</param>
        /// <returns>CompiledExpression (GetterOfPropertyOrField)</returns>
        /// <remarks>PropertyInfo・FieldInfoのGetValue高速化用</remarks>
        public static Func<object, object> CreateGetterOfPropertyOrField(Type type, string memberName)
        {
            var target = Expression.Parameter(typeof(object), "target");

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(target , type), memberName)
                    , typeof(object))
                , target);

            return lambda.Compile();
        }

        /// <summary>CompiledExpressionでSetterを生成して返す。</summary>
        /// <param name="type">Type</param>
        /// <param name="memberName">string</param>
        /// <returns>CompiledExpression (SetterOfPropertyOrField)</returns>
        /// <remarks>PropertyInfo・FieldInfoのSetValue高速化用</remarks>
        public static Action<object, object> CreateSetterOfPropertyOrField(Type type, string memberName)
        {
            var target = Expression.Parameter(typeof(object), "target");
            var value = Expression.Parameter(typeof(object), "value");

            var left = Expression.PropertyOrField(
                Expression.Convert(target, type), memberName);

            var right = Expression.Convert(value, left.Type);

            var lambda = Expression.Lambda<Action<object, object>>(
                Expression.Assign(left, right), target, value);

            return lambda.Compile();
        }
    }
}
