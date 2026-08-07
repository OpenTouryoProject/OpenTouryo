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
//* クラス名        ：TestObjectInspector
//* クラス日本語名  ：ObjectInspectorのテスト
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

namespace TestCode
{
    /// <summary>ObjectInspectorのテスト</summary>
    /// <remarks>
    /// オブジェクトの中身を文字列に落とす部品（#522）。
    ///
    /// ＜なぜここを優先するか＞
    ///   **調査やログで使う道具そのもの**なので、これが壊れると
    ///   他の不具合を追うときの足場が崩れる。
    ///   出力が決定的で、結果ファイルの比較にそのまま載る。
    ///
    /// ＜静的な設定を持つことに注意＞
    ///   DateTimeFormat / TimeSpanFormat / ExclusionFullyQualifiedNameParts は
    ///   **static**。設定したまま戻さないと、後続のテストの出力が変わる。
    ///   このテストは必ず元に戻す。
    /// </remarks>
    public class TestObjectInspector
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            // 静的な設定を退避する。
            string orgDateTimeFormat = ObjectInspector.DateTimeFormat;
            string orgTimeSpanFormat = ObjectInspector.TimeSpanFormat;
            string[] orgExclusion = ObjectInspector.ExclusionFullyQualifiedNameParts;

            try
            {
                TestObjectInspector.TestBasic();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                TestObjectInspector.TestPoco();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                TestObjectInspector.TestDepthLimit();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                TestObjectInspector.TestDateTime();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                TestObjectInspector.TestExclusion();
            }
            finally
            {
                // **必ず戻す。** 後続のテストの出力に影響するため。
                ObjectInspector.DateTimeFormat = orgDateTimeFormat;
                ObjectInspector.TimeSpanFormat = orgTimeSpanFormat;
                ObjectInspector.ExclusionFullyQualifiedNameParts = orgExclusion;
            }
        }

        #endregion

        #region テスト用の POCO

        /// <summary>入れ子にする側</summary>
        public class InnerPoco
        {
            /// <summary>Code</summary>
            public string Code { get; set; }

            /// <summary>Value</summary>
            public int Value { get; set; }
        }

        /// <summary>入れ子を持つ側</summary>
        public class OuterPoco
        {
            /// <summary>Name</summary>
            public string Name { get; set; }

            /// <summary>Inner（入れ子）</summary>
            public InnerPoco Inner { get; set; }

            /// <summary>Numbers（配列）</summary>
            public int[] Numbers { get; set; }
        }

        /// <summary>自分自身を入れ子にできる型</summary>
        /// <remarks>深さの上限と、循環参照の確認に使う。</remarks>
        public class NestPoco
        {
            /// <summary>Level</summary>
            public int Level { get; set; }

            /// <summary>Child（自分自身）</summary>
            public NestPoco Child { get; set; }
        }

        #endregion

        #region private

        /// <summary>基本の型</summary>
        private static void TestBasic()
        {
            MyDebug.OutputDebugAndConsole("ObjectInspector.Inspect : 基本の型");

            TestObjectInspector.Output("null", null);
            TestObjectInspector.Output("int", 123);
            TestObjectInspector.Output("string", "あいう");
            TestObjectInspector.Output("bool", true);
            TestObjectInspector.Output("空文字", "");

            // 配列・コレクション
            TestObjectInspector.Output("int[]", new int[] { 1, 2, 3 });

            // 要素が多い場合。**入れ子は浅いので、全要素が出るのが期待。**
            TestObjectInspector.Output("int[] （8 要素）", new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            TestObjectInspector.Output("List<string>", list);

            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("x", 1);
            dic.Add("y", 2);
            TestObjectInspector.Output("Dictionary", dic);
        }

        /// <summary>POCO（入れ子と配列を含む）</summary>
        private static void TestPoco()
        {
            MyDebug.OutputDebugAndConsole("ObjectInspector.Inspect : POCO");

            OuterPoco outer = new OuterPoco();
            outer.Name = "外側";
            outer.Numbers = new int[] { 10, 20 };

            // 入れ子が null の場合
            TestObjectInspector.Output("入れ子が null", outer);

            // 入れ子に値がある場合
            InnerPoco inner = new InnerPoco();
            inner.Code = "C1";
            inner.Value = 99;
            outer.Inner = inner;

            TestObjectInspector.Output("入れ子に値", outer);
        }

        /// <summary>深さの上限と循環参照</summary>
        /// <remarks>
        /// **再帰には深さの上限がある**（ObjectInspector の内部で 5 段）。
        /// 超えた先は "-over-" になり、それ以上は辿らない。
        ///
        /// ＜幅は深さを消費しない＞
        ///   入口で加算し、出口で減算する深さカウンタなので、
        ///   要素数がいくら多くても打ち切られない（上の「int[] （8 要素）」）。
        ///
        /// ＜循環参照＞
        ///   自分自身を指していても、深さの上限で止まる。
        ///   **この上限が壊れると停止しなくなる**ため、ここで固定しておく。
        /// </remarks>
        private static void TestDepthLimit()
        {
            MyDebug.OutputDebugAndConsole("ObjectInspector.Inspect : 深さの上限");

            // 深い入れ子（8 段）
            NestPoco deep = new NestPoco();
            deep.Level = 1;

            NestPoco current = deep;
            for (int i = 2; i <= 8; i++)
            {
                current.Child = new NestPoco();
                current.Child.Level = i;
                current = current.Child;
            }

            TestObjectInspector.Output("入れ子 8 段", deep);

            // 循環参照（自分自身を指す）
            NestPoco cyclic = new NestPoco();
            cyclic.Level = 1;
            cyclic.Child = cyclic;

            TestObjectInspector.Output("循環参照", cyclic);
        }

        /// <summary>日時（書式の指定）</summary>
        /// <remarks>
        /// **DateTimeFormat を指定しないと ToString() になり、実行環境のロケールで変わる。**
        /// 期待結果の比較に載せるものは、必ず書式を指定すること。
        /// ここでは「指定した場合」だけを記録する（未指定は環境で変わるため記録できない）。
        /// </remarks>
        private static void TestDateTime()
        {
            MyDebug.OutputDebugAndConsole("ObjectInspector.Inspect : 日時");

            DateTime dt = new DateTime(2026, 8, 6, 12, 34, 56);
            TimeSpan ts = new TimeSpan(1, 2, 3, 4);

            ObjectInspector.DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
            ObjectInspector.TimeSpanFormat = "d\\.hh\\:mm\\:ss";

            TestObjectInspector.Output("DateTime（書式あり）", dt);
            TestObjectInspector.Output("TimeSpan（書式あり）", ts);

            ObjectInspector.DateTimeFormat = "";
            ObjectInspector.TimeSpanFormat = "";
        }

        /// <summary>除外の指定</summary>
        /// <remarks>
        /// ExclusionFullyQualifiedNameParts に含まれる型は、中身を展開せず
        /// 「excluded.」を前置して ToString() の結果を出す。
        /// 大きなオブジェクトや、展開すると副作用のある型を避けるための仕組み。
        /// </remarks>
        private static void TestExclusion()
        {
            MyDebug.OutputDebugAndConsole("ObjectInspector.Inspect : 除外");

            InnerPoco inner = new InnerPoco();
            inner.Code = "C2";
            inner.Value = 1;

            OuterPoco outer = new OuterPoco();
            outer.Name = "外側";
            outer.Inner = inner;
            outer.Numbers = new int[] { 1 };

            // 除外なし
            ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { };
            TestObjectInspector.Output("除外なし", outer);

            // InnerPoco を除外する
            ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { "InnerPoco" };
            TestObjectInspector.Output("InnerPoco を除外", outer);

            ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { };
        }

        /// <summary>1 件分を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="obj">対象</param>
        private static void Output(string caseName, object obj)
        {
            MyDebug.OutputDebugAndConsole("[" + caseName + "]");
            MyDebug.OutputDebugAndConsole(ObjectInspector.Inspect(obj));
        }

        #endregion
    }
}
