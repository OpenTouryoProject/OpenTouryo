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
//* クラス名        ：TestLatebind
//* クラス日本語名  ：Latebindのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Reflection;

namespace TestCode
{
    /// <summary>Latebindのテスト</summary>
    /// <remarks>
    /// フレームワークの**動的呼び出しの心臓部**（#522）。
    /// Ｂ層・Ｄ層の自動振り分け（UOC_ メソッドの解決）がこれに依存するため、
    /// 壊れるとフレームワーク全体が動かなくなる。
    ///
    /// ＜押さえる観点＞
    ///   ・**private / static も呼べる**（BindingFlags に NonPublic を含む）
    ///   ・メソッド が無いとき、例外を投げる版と null を返す版がある
    ///   ・**オーバーロードは引数の「数」でしか解決しない**（型では解決しない）
    ///   ・呼び先が投げた例外は、そのままではなく包まれて伝わる
    /// </remarks>
    public class TestLatebind
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestLatebind.TestInvoke();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestLatebind.TestNotFound();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestLatebind.TestOverload();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestLatebind.TestCheck();
        }

        #endregion

        #region テスト用の型

        /// <summary>呼び出される側の基底</summary>
        public class TargetBase
        {
            /// <summary>基底のメソッド</summary>
            /// <returns>固定値</returns>
            public string BaseMethod()
            {
                return "BaseMethod";
            }
        }

        /// <summary>呼び出される側</summary>
        public class Target : TargetBase
        {
            /// <summary>引数なし</summary>
            /// <returns>固定値</returns>
            public string PublicMethod()
            {
                return "PublicMethod";
            }

            /// <summary>引数あり</summary>
            /// <param name="a">数値</param>
            /// <param name="b">文字列</param>
            /// <returns>連結した文字列</returns>
            public string WithArgs(int a, string b)
            {
                return "WithArgs:" + a + ":" + b;
            }

            /// <summary>static</summary>
            /// <returns>固定値</returns>
            public static string StaticMethod()
            {
                return "StaticMethod";
            }

            /// <summary>private</summary>
            /// <returns>固定値</returns>
            /// <remarks>
            /// **BindingFlags に NonPublic が含まれるため、外から呼べる。**
            /// フレームワークが protected な UOC_ メソッドを解決できるのはこのため。
            /// </remarks>
            private string PrivateMethod()
            {
                return "PrivateMethod";
            }

            /// <summary>オーバーロード（引数 1 個）</summary>
            /// <param name="a">数値</param>
            /// <returns>固定値</returns>
            public string Overloaded(int a)
            {
                return "Overloaded(int):" + a;
            }

            /// <summary>オーバーロード（引数 2 個）</summary>
            /// <param name="a">数値</param>
            /// <param name="b">数値</param>
            /// <returns>固定値</returns>
            public string Overloaded(int a, int b)
            {
                return "Overloaded(int,int):" + a + ":" + b;
            }

            /// <summary>例外を投げる</summary>
            public void ThrowsInside()
            {
                throw new InvalidOperationException("呼び先で投げた例外");
            }
        }

        #endregion

        #region private

        /// <summary>呼び出せること</summary>
        private static void TestInvoke()
        {
            MyDebug.OutputDebugAndConsole("Latebind.InvokeMethod");

            Target target = new Target();

            TestLatebind.Output("引数なし", target, "PublicMethod", new object[] { });
            TestLatebind.Output("引数あり", target, "WithArgs", new object[] { 1, "x" });
            TestLatebind.Output("static", target, "StaticMethod", new object[] { });

            // **private も呼べる。**
            TestLatebind.Output("private", target, "PrivateMethod", new object[] { });

            // 基底クラスのメソッド
            TestLatebind.Output("基底のメソッド", target, "BaseMethod", new object[] { });

            // 呼び先が投げた例外は、そのままではなく包まれて伝わる。
            TestLatebind.Output("呼び先が例外", target, "ThrowsInside", new object[] { });
        }

        /// <summary>メソッドが無いとき</summary>
        /// <remarks>
        /// 例外を投げる版と、null を返す版がある。
        /// **null を返す版は「メッセージにメソッド名が含まれるか」で判別している**
        /// （Latebind.cs の InvokeMethod_NoErr）。実装のコメントにも
        /// 「Exception.Message を修正すると影響がある」と注意書きがある。
        /// </remarks>
        private static void TestNotFound()
        {
            MyDebug.OutputDebugAndConsole("Latebind : メソッドが無いとき");

            Target target = new Target();

            // 例外を投げる版
            TestLatebind.Output("InvokeMethod", target, "NotExist", new object[] { });

            // null を返す版
            try
            {
                object ret = Latebind.InvokeMethod_NoErr(target, "NotExist", new object[] { });
                MyDebug.OutputDebugAndConsole(
                    "[InvokeMethod_NoErr] " + (ret == null ? "null" : ret.ToString()));
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("[InvokeMethod_NoErr] 例外 : " + ex.GetType().FullName);
            }
        }

        /// <summary>オーバーロードの解決</summary>
        /// <remarks>
        /// オーバーロードがあると GetMethod が AmbiguousMatchException を投げるため、
        /// Latebind は**引数の「数」が一致するもの**を探して呼ぶ。
        ///
        /// **型までは解決しない。** 引数の数が同じオーバーロード
        /// （例：Foo(int) と Foo(string)）は、どちらが呼ばれるか保証されない。
        /// このため、ここでは引数の数が違うものだけを確認している。
        /// </remarks>
        private static void TestOverload()
        {
            MyDebug.OutputDebugAndConsole("Latebind : オーバーロード");

            Target target = new Target();

            TestLatebind.Output("引数 1 個", target, "Overloaded", new object[] { 1 });
            TestLatebind.Output("引数 2 個", target, "Overloaded", new object[] { 1, 2 });

            // 引数の数が一致しない場合
            TestLatebind.Output("引数 3 個（該当なし）", target, "Overloaded", new object[] { 1, 2, 3 });
        }

        /// <summary>型・メソッドの確認</summary>
        private static void TestCheck()
        {
            MyDebug.OutputDebugAndConsole("Latebind : 型とメソッドの確認");

            Target target = new Target();

            MyDebug.OutputDebugAndConsole(
                "CheckTypeOfBaseClass(Target, TargetBase) : "
                + Latebind.CheckTypeOfBaseClass(typeof(Target), typeof(TargetBase)));

            MyDebug.OutputDebugAndConsole(
                "CheckTypeOfBaseClass(Target, string)     : "
                + Latebind.CheckTypeOfBaseClass(typeof(Target), typeof(string)));

            MyDebug.OutputDebugAndConsole(
                "CheckTypeOfMethodByName(PublicMethod)    : "
                + Latebind.CheckTypeOfMethodByName(target, "PublicMethod"));

            // **private も「存在する」と判定される。**
            MyDebug.OutputDebugAndConsole(
                "CheckTypeOfMethodByName(PrivateMethod)   : "
                + Latebind.CheckTypeOfMethodByName(target, "PrivateMethod"));

            MyDebug.OutputDebugAndConsole(
                "CheckTypeOfMethodByName(NotExist)        : "
                + Latebind.CheckTypeOfMethodByName(target, "NotExist"));
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>呼び出して結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="target">対象</param>
        /// <param name="methodName">メソッド名</param>
        /// <param name="paramSet">引数</param>
        /// <remarks>
        /// 例外はメッセージではなく**型名だけ**を出す。
        /// メッセージは実行環境の言語で変わり、期待結果と一致しなくなるため。
        /// </remarks>
        private static void Output(string caseName, object target, string methodName, object[] paramSet)
        {
            try
            {
                object ret = Latebind.InvokeMethod(target, methodName, paramSet);

                MyDebug.OutputDebugAndConsole(
                    "[" + caseName + "] " + (ret == null ? "null" : ret.ToString()));
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("[" + caseName + "] 例外 : " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}
