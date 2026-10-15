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
//* クラス名        ：TestFastReflection
//* クラス日本語名  ：FastReflectionのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.FastReflection;

namespace TestCode
{
    /// <summary>FastReflectionのテスト</summary>
    /// <remarks>
    /// 反射を式木にコンパイルして高速化する部品（#522）。
    ///
    /// ＜なぜここを見るか＞
    ///   **DataToPoco / PocoToPoco がこれに乗っている。**
    ///   AccessorCacher が返す AccessorName で列と突合するため、
    ///   ここが変わると項目移送の挙動がそのまま変わる（TestDto と対で見ること）。
    ///
    /// ＜キャッシュは静的＞
    ///   AccessorCacher.CncDic は**プロセス全体で共有**される。
    ///   型ごとに一度だけ構築され、二度目以降は再利用される。
    /// </remarks>
    public class TestFastReflection
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestFastReflection.TestInstanceCreator();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestFastReflection.TestAccessorCacher();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestFastReflection.TestCompiledExpression();
        }

        #endregion

        #region テスト用の POCO

        /// <summary>アクセサの確認に使う</summary>
        /// <remarks>プロパティとフィールドの両方を持たせる。</remarks>
        public class AccessorPoco
        {
            /// <summary>読み書きできるプロパティ</summary>
            public int Id { get; set; }

            /// <summary>読み書きできるプロパティ（文字列）</summary>
            public string Name { get; set; }

            /// <summary>Nullable なプロパティ</summary>
            public int? Count { get; set; }

            /// <summary>読み取り専用のプロパティ</summary>
            public string ReadOnly
            {
                get { return "ReadOnly"; }
            }

            /// <summary>フィールド</summary>
            public string Field = "";
        }

        #endregion

        #region private

        /// <summary>InstanceCreator</summary>
        /// <remarks>毎回、別のインスタンスが返ること。</remarks>
        private static void TestInstanceCreator()
        {
            MyDebug.OutputDebugAndConsole("InstanceCreator<T>.Factory");

            AccessorPoco a = InstanceCreator<AccessorPoco>.Factory();
            AccessorPoco b = InstanceCreator<AccessorPoco>.Factory();

            MyDebug.OutputDebugAndConsole("生成できた       : " + (a != null));
            MyDebug.OutputDebugAndConsole("別のインスタンス : " + (!object.ReferenceEquals(a, b)));
            MyDebug.OutputDebugAndConsole("既定値（Id）     : " + a.Id);
            MyDebug.OutputDebugAndConsole("既定値（Name）   : " + (a.Name == null ? "(null)" : a.Name));
        }

        /// <summary>AccessorCacher</summary>
        /// <remarks>
        /// プロパティとフィールドの両方が拾われる。
        /// **列挙の順序は反射に依存する**ため、名前で並べ替えてから出力する。
        /// </remarks>
        private static void TestAccessorCacher()
        {
            MyDebug.OutputDebugAndConsole("AccessorCacher.CacheAccessor");

            AccessorPoco poco = new AccessorPoco();
            AccessorCacher.CacheAccessor(poco);

            List<AccessorInfo> list = AccessorCacher.CncDic[poco.GetType()];

            // 順序が実装に依存しないよう、名前で並べ替える。
            List<AccessorInfo> sorted = new List<AccessorInfo>(list);
            sorted.Sort(delegate(AccessorInfo x, AccessorInfo y)
            {
                return string.CompareOrdinal(x.AccessorName, y.AccessorName);
            });

            MyDebug.OutputDebugAndConsole("件数 : " + sorted.Count);

            foreach (AccessorInfo ai in sorted)
            {
                MyDebug.OutputDebugAndConsole(
                    ai.AccessorName
                    + " : 型=" + ai.AccessorType.Name
                    + ", 実体型=" + (ai.UnderlyingType == null ? "(null)" : ai.UnderlyingType.Name)
                    + ", Get=" + (ai.GetDelegate != null)
                    + ", Set=" + (ai.SetDelegate != null));
            }

            // デリゲート経由での読み書き
            AccessorInfo target = sorted.Find(delegate(AccessorInfo ai) { return ai.AccessorName == "Name"; });
            target.SetDelegate(poco, "設定した値");

            MyDebug.OutputDebugAndConsole("Set → Get : " + target.GetDelegate(poco));

            // 二度目は同じキャッシュが使われる（静的なため）。
            AccessorCacher.CacheAccessor(new AccessorPoco());
            MyDebug.OutputDebugAndConsole(
                "再取得しても同じ : "
                + object.ReferenceEquals(list, AccessorCacher.CncDic[poco.GetType()]));
        }

        /// <summary>CompiledExpressionCreater</summary>
        /// <remarks>プロパティ・フィールドの getter / setter を直接作る。</remarks>
        private static void TestCompiledExpression()
        {
            MyDebug.OutputDebugAndConsole("CompiledExpressionCreater");

            Type t = typeof(AccessorPoco);
            AccessorPoco poco = new AccessorPoco();

            // プロパティ
            Action<object, object> setId =
                CompiledExpressionCreater.CreateSetterOfPropertyOrField(t, "Id");
            Func<object, object> getId =
                CompiledExpressionCreater.CreateGetterOfPropertyOrField(t, "Id");

            setId(poco, 42);
            MyDebug.OutputDebugAndConsole("プロパティ Id : " + getId(poco));

            // フィールド
            Action<object, object> setField =
                CompiledExpressionCreater.CreateSetterOfPropertyOrField(t, "Field");
            Func<object, object> getField =
                CompiledExpressionCreater.CreateGetterOfPropertyOrField(t, "Field");

            setField(poco, "フィールドの値");
            MyDebug.OutputDebugAndConsole("フィールド Field : " + getField(poco));

            // 存在しないメンバ
            try
            {
                CompiledExpressionCreater.CreateGetterOfPropertyOrField(t, "NotExist");
                MyDebug.OutputDebugAndConsole("存在しないメンバ : 例外にならない");
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("存在しないメンバ : 例外 " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}
