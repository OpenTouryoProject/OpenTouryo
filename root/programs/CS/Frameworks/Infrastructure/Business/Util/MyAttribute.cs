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
//* クラス名        ：MyAttribute
//* クラス日本語名  ：カスタム属性クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/08/06  西野 大介         クラス属性だけでなく、メソッド属性も処理可能に修正
//**********************************************************************************

using System;
using System.Reflection;

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>カスタム属性クラス</summary>
    /// <remarks>自由に（拡張して）利用できる。</remarks>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MyAttribute : Attribute
    {
        /// <summary>カスタム属性A</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MyAttributeA = "";

        /// <summary>カスタム属性B</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MyAttributeB = "";

        /// <summary>カスタム属性C</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string MyAttributeC = "";

        /// <summary>当該カスタム属性クラスを取得する（クラス属性用）</summary>
        /// <param name="obj">クラスのオブジェクト</param>
        /// <param name="myAttribute">カスタム属性クラスの配列</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public static void GetAttr(object obj, out MyAttribute[] myAttribute)
        {
            // 属性クラスのリストを取得する。
            // inheritは、AllowMultipleに合わせる。
            // http://msdn.microsoft.com/ja-jp/library/system.type.getcustomattributes.aspx
            object[] list = obj.GetType().GetCustomAttributes(typeof(MyAttribute), true);

            // object[]をMyAttribute[]に変換して返す。
            int i = 0;
            myAttribute = new MyAttribute[list.Length];
            foreach (MyAttribute temp in list)
            {
                myAttribute[i] = temp;
            }
        }

        /// <summary>カスタム属性クラスを取得する（メソッド属性用）</summary>
        /// <param name="methodInfo">メソッドのMethodInfo</param>
        /// <param name="myAttribute">カスタム属性クラスの配列</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public static void GetAttr(MethodInfo methodInfo, out MyAttribute[] myAttribute)
        {
            // 属性クラスのリストを取得する。inheritは、AllowMultipleに合わせる。
            // http://msdn.microsoft.com/ja-jp/library/system.type.getcustomattributes.aspx
            object[] list = methodInfo.GetCustomAttributes(typeof(MyAttribute), true);

            // object[]をMyAttribute[]に変換して返す。
            int i = 0;
            myAttribute = new MyAttribute[list.Length];
            foreach (MyAttribute temp in list)
            {
                myAttribute[i] = temp;
            }
        }
    }

}
