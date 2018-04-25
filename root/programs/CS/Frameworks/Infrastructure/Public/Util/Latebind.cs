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
//* クラス名        ：Latebind
//* クラス日本語名  ：レイトバインド処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/15  西野 大介         新規作成
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#11 ： レイトバインド（assemblyが存在しない場合）
//*  2009/08/10  西野 大介         メソッドが無くても例外をスローしないメソッドを追加
//*  2010/10/21  西野 大介         NonPublic.etc対応（GetMethodとInvokeMemberに適用）
//*  2010/10/21  西野 大介         リファクタリング＋メソッド追加の実施
//*  2010/11/19  西野 大介         オーバーロード メソッド対応
//*  2011/02/08  西野 大介         リファクタリング（CheckTypeOfMethodByName）
//*  2012/08/25  西野 大介         Assembly.LoadFile → .Load（ASP.NETシャドウコピー対応）
//*  2015/12/22  Sai               Replaced substring method with EndsWith string method in first Invoke method.
//*  2016/01/08  Sai               Added ToLower method in first Invoke method.
//**********************************************************************************

using System;
using System.Reflection;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>レイトバインド処理クラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class Latebind
    {
        /// <summary>レイトバインド時に指定するフラグ</summary>
        private static BindingFlags MyFlg
            = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>遅延バインド（メソッド）</summary>
        /// <param name="assemblyName">アセンブリ名（名前かファイルパス指定）</param>
        /// <param name="className">クラス名（名前空間を含む）</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="paramSet">パラメタのセット</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる（メソッドがない場合、例外をスロー）。</remarks>
        public static object InvokeMethod(string assemblyName, string className, string methodName, object[] paramSet)
        {
            Assembly mod = null;

            // #11-start
            try
            {
                // アセンブリ ロード
                string ext = ".dll";
                if (assemblyName.ToLower().EndsWith(ext))
                {
                    //  アセンブリ・ファイルのパス指定の場合、Assembly.LoadFile
                    mod = Assembly.LoadFile(assemblyName);
                }
                else
                {
                    // アセンブリ名前指定の場合、Assembly.Load
                    mod = Assembly.Load(assemblyName);
                }
            }
            catch (Exception)
            {
                // アセンブリ無し
                throw new ArgumentException(
                    String.Format(PublicExceptionMessage.LATEBIND_ERROR0, assemblyName));
                // ここのメッセージを修正すると、InvokeMethod_NoErrにも影響があるので注意。
            }
            // #11-end

            // クラス チェック
            if (mod.GetType(className) == null)
            {
                // クラス無し
                throw new ArgumentException(
                    String.Format(PublicExceptionMessage.LATEBIND_ERROR1, assemblyName, className));
                // ここのメッセージを修正すると、InvokeMethod_NoErrにも影響があるので注意。
            }
            else
            {
                Type t = mod.GetType(className);

                // TypeとInstance
                object o = Activator.CreateInstance(t);

                try
                {
                    // メソッド チェック（BindingFlagsを追加）
                    if (t.GetMethod(methodName, Latebind.MyFlg) == null)
                    {
                        // メソッド無し
                        throw new ArgumentException(
                            String.Format(PublicExceptionMessage.LATEBIND_ERROR2,
                                assemblyName, className, methodName));
                        // ※ ここのException.Messageを修正すると、InvokeMethod_NoErr
                        //    にも影響があるので注意（assemblyNameの存在チェック）。
                    }
                    else
                    {
                        // InvokeMemberメソッドによりメソッドを実行（BindingFlagsを追加）
                        return t.InvokeMember(methodName,
                            BindingFlags.InvokeMethod | Latebind.MyFlg,
                            null, o, paramSet);
                    }
                }
                catch (AmbiguousMatchException)
                {
                    // ここの try - catch = 「あいまいな一致が見つかりました」対応

                    // 複数のMethodInfoを取得
                    MethodInfo[] mis = t.GetMethods(Latebind.MyFlg);

                    // foreachで、メソッド名、引数の数が一致するものを呼び出す。
                    // （パラメタの型一致までは解決できない仕様）
                    foreach (MethodInfo mi in mis)
                    {
                        if (mi.Name == methodName)
                        {
                            if (mi.GetParameters().Length == paramSet.Length)
                            {
                                // Invokeメソッドによりメソッドを実行
                                return mi.Invoke(o, paramSet);
                            }
                        }
                    }

                    // 引数の数が一致するメソッド無し
                    throw new ArgumentException(
                        String.Format(PublicExceptionMessage.LATEBIND_ERROR3,
                            assemblyName, className, methodName));
                    // ※ ここのException.Messageを修正すると、InvokeMethod_NoErr
                    //    にも影響があるので注意（assemblyNameの存在チェック）。
                }
            }
        }

        /// <summary>遅延バインド（メソッド）</summary>
        /// <param name="assemblyName">アセンブリ名（名前かファイルパス指定）</param>
        /// <param name="className">クラス名（名前空間を含む）</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="paramSet">パラメタのセット</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる（メソッドがなくても、例外をスローしない）。。</remarks>
        public static object InvokeMethod_NoErr(
            string assemblyName, string className, string methodName, object[] paramSet)
        {
            try
            {
                return Latebind.InvokeMethod(assemblyName, className, methodName, paramSet);
            }
            catch (ArgumentException agex)
            {
                if (agex.Message.IndexOf(assemblyName) == -1)
                {
                    // 「ｘｘｘｘ無し」でないArgumentExceptionはリスロー
                    // ※ 上記判別方法で妥当と判断した。
                    throw;
                }
                else
                {
                    // ArgumentExceptionを握りつぶす
                    return null;
                }
            }
        }

        /// <summary>遅延バインド（メソッド）</summary>
        /// <param name="objectClass">オブジェクト</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="paramSet">パラメタのセット</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる（メソッドがない場合、例外をスロー）。</remarks>
        public static object InvokeMethod(object objectClass, string methodName, object[] paramSet)
        {
            // TypeとInstance
            Type t = objectClass.GetType();

            try
            {
                // メソッド チェック（BindingFlagsを追加）
                if (t.GetMethod(methodName, Latebind.MyFlg) == null)
                {
                    // メソッド無し
                    throw new ArgumentException(
                            String.Format(PublicExceptionMessage.LATEBIND_ERROR2,
                                t.Assembly.FullName, t.FullName, methodName));
                    // ※ ここのException.Messageを修正すると、InvokeMethod_NoErr
                    //    にも影響があるので注意（methodNameの存在チェック）。
                }
                else
                {
                    // InvokeMemberメソッドによりメソッドを実行（BindingFlagsを追加）
                    return t.InvokeMember(
                        methodName,
                        BindingFlags.InvokeMethod | Latebind.MyFlg,
                        null, objectClass, paramSet);
                }
            }
            catch (AmbiguousMatchException)
            {
                // ここの try - catch = 「あいまいな一致が見つかりました」対応

                // 複数のMethodInfoを取得
                MethodInfo[] mis = t.GetMethods(Latebind.MyFlg);

                // foreachで、メソッド名、引数の数が一致するものを呼び出す。
                // （パラメタの型一致までは解決できない仕様）
                foreach (MethodInfo mi in mis)
                {
                    if (mi.Name == methodName)
                    {
                        if (mi.GetParameters().Length == paramSet.Length)
                        {
                            // Invokeメソッドによりメソッドを実行
                            return mi.Invoke(objectClass, paramSet);
                        }
                    }
                }

                // 引数の数が一致するメソッド無し
                throw new ArgumentException(
                        String.Format(PublicExceptionMessage.LATEBIND_ERROR3,
                            t.Assembly.FullName, t.FullName, methodName));
                // ※ ここのException.Messageを修正すると、InvokeMethod_NoErr
                //    にも影響があるので注意（methodNameの存在チェック）。
            }
        }

        /// <summary>遅延バインド（メソッド）</summary>
        /// <param name="objectClass">オブジェクト</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="paramSet">パラメタのセット</param>
        /// <returns>戻り値</returns>
        /// <remarks>自由に利用できる（メソッドがなくても、例外をスローしない）。</remarks>
        public static object InvokeMethod_NoErr(object objectClass, string methodName, object[] paramSet)
        {
            try
            {
                return Latebind.InvokeMethod(objectClass, methodName, paramSet);
            }
            catch (ArgumentException agex)
            {
                if (agex.Message.IndexOf(methodName) == -1)
                {
                    // 「メソッド無し」でないArgumentExceptionはリスロー
                    // ※ 上記判別方法で妥当と判断した。
                    throw;
                }
                else
                {
                    // ArgumentExceptionを握りつぶす
                    return null;
                }
            }
        }

        /// <summary>型のベース型が一致しているかチェック</summary>
        /// <param name="classType">型</param>
        /// <param name="baseType">ベース型</param>
        /// <returns>
        /// true:一致
        /// false：不一致
        /// </returns>
        public static bool CheckTypeOfBaseClass(Type classType, Type baseType)
        {
            if (classType.BaseType != null)
            {
                if (classType.BaseType == baseType)
                {
                    // 発見
                    return true;
                }
                else
                {
                    // 再帰
                    return CheckTypeOfBaseClass(classType.BaseType, baseType);
                }
            }
            else
            {
                // 終端に達した
                return false;
            }
        }

        /// <summary>
        /// 当該オブジェクトに指定のメソッドが存在するかチェックする。
        /// </summary>
        /// <param name="obj">当該オブジェクト</param>
        /// <param name="methodName">メソッド名</param>
        /// <returns>
        /// ・true：存在する。
        /// ・false：存在しない。
        /// </returns>
        public static bool CheckTypeOfMethodByName(object obj, string methodName)
        {
            bool isFinded = false;

            // 探す
            //BindingFlags MyFlg
            //    = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            // 複数のMethodInfoを取得
            MethodInfo[] mis = obj.GetType().GetMethods(Latebind.MyFlg);

            foreach (MethodInfo mi in mis)
            {
                if (mi.Name == methodName)
                {
                    // 発見
                    isFinded = true;
                }
            }

            return isFinded;
        }
    }
}
