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
//* クラス名        ：TestUtil
//* クラス日本語名  ：Public.Utilのテスト
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
using Touryo.Infrastructure.Public.Util;

namespace TestCode
{
    /// <summary>Public.Utilのテスト</summary>
    /// <remarks>
    /// 型変換・配列操作・設定の読み取り（#522）。
    ///
    /// ＜対象を絞っている＞
    ///   このクラス群には**実行環境で値が変わるもの**が混ざる。
    ///   結果ファイルの比較に載せられないため、次は値を出さない。
    ///     ・EnvInfo        … マシン名・OS・ビット数など
    ///     ・GetConfigParameter … 設定値そのもの（net48 と .NET (Core) でパス区切りが違う）
    ///   「取得できること」「無いキーは null」までに留める。
    ///
    ///   RandomValueGenerator / PerformanceRecorder は実行ごとに変わるため対象外。
    /// </remarks>
    public class TestUtil
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestUtil.TestPubCmnFunction();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestUtil.TestArrayOperator();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestUtil.TestConfigAndEnv();
        }

        #endregion

        #region private

        /// <summary>PubCmnFunction</summary>
        /// <remarks>
        /// ChangeType と GetUnderlyingType は **DataToPoco が型変換に使う**。
        /// ここが変わると項目移送の挙動が変わる（TestDto と対で見ること）。
        /// </remarks>
        private static void TestPubCmnFunction()
        {
            MyDebug.OutputDebugAndConsole("PubCmnFunction");

            // --- ChangeType ---
            TestUtil.OutputChangeType("string → int", "123", typeof(int));
            TestUtil.OutputChangeType("int → string", 123, typeof(string));
            TestUtil.OutputChangeType("string → int?", "123", typeof(int?));
            TestUtil.OutputChangeType("null → int?", null, typeof(int?));
            TestUtil.OutputChangeType("string → decimal", "1.5", typeof(decimal));
            TestUtil.OutputChangeType("変換できない", "abc", typeof(int));

            // --- GetUnderlyingType ---
            // Nullable は実体型に、そうでなければ null。
            MyDebug.OutputDebugAndConsole(
                "GetUnderlyingType(int?)  : " + TestUtil.TypeName(PubCmnFunction.GetUnderlyingType(typeof(int?))));
            MyDebug.OutputDebugAndConsole(
                "GetUnderlyingType(int)   : " + TestUtil.TypeName(PubCmnFunction.GetUnderlyingType(typeof(int))));
            MyDebug.OutputDebugAndConsole(
                "GetUnderlyingType(string): " + TestUtil.TypeName(PubCmnFunction.GetUnderlyingType(typeof(string))));

            // --- ToUnixTime ---
            // 固定の日時を与えれば決定的。UTC で与えること。
            DateTimeOffset dto = new DateTimeOffset(2026, 8, 6, 12, 34, 56, TimeSpan.Zero);
            MyDebug.OutputDebugAndConsole("ToUnixTime : " + PubCmnFunction.ToUnixTime(dto));

            // --- GetFileNameNoEx ---
            // **第 2 引数は「パス区切り」であって、拡張子の区切りではない。**
            //   (1) 区切り文字で分けて末尾＝ファイル名を取り出す
            //   (2) それを「.」で分けて、最後の 1 つ（拡張子）を落として繋ぎ直す
            // 「.」を渡すと (1) で拡張子だけが残り、(2) で全部落ちて空になる。
            MyDebug.OutputDebugAndConsole(
                "GetFileNameNoEx(パス付き)    : " + PubCmnFunction.GetFileNameNoEx("C:\\dir\\a.txt", '\\'));
            MyDebug.OutputDebugAndConsole(
                "GetFileNameNoEx(二重拡張子)  : " + PubCmnFunction.GetFileNameNoEx("C:\\dir\\a.b.txt", '\\'));
            MyDebug.OutputDebugAndConsole(
                "GetFileNameNoEx(拡張子なし)  : " + PubCmnFunction.GetFileNameNoEx("C:\\dir\\abc", '\\'));
            MyDebug.OutputDebugAndConsole(
                "GetFileNameNoEx(区切りを「.」): \"" + PubCmnFunction.GetFileNameNoEx("a.txt", '.') + "\"");

            // --- 呼び出し元の情報 ---
            // メソッド名は固定なので決定的。
            // **ファイル パスと行番号は出さない**（環境と編集で変わるため）。
            MyDebug.OutputDebugAndConsole(
                "GetCurrentMethodName : " + PubCmnFunction.GetCurrentMethodName());
        }

        /// <summary>ArrayOperator</summary>
        private static void TestArrayOperator()
        {
            MyDebug.OutputDebugAndConsole("ArrayOperator");

            int[] src = new int[] { 1, 2, 3 };
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            // 縮める
            TestUtil.OutputArray("CopyArray(3 → 2)", delegate { return ArrayOperator.CopyArray<int>(src, 2); });

            // **伸ばせない。** 指定したサイズ分を元配列から読むため、元より大きいと例外になる。
            TestUtil.OutputArray("CopyArray(3 → 5)", delegate { return ArrayOperator.CopyArray<int>(src, 5); });

            // 開始位置を指定
            //
            // **コピーする長さは「コピー先配列の長さ」で固定されている。**
            //   Array.Copy(src, srcStart, dst, dstStart, dstArraySize)
            // そのため書込開始位置を 0 より後ろにすると、必ずコピー先が溢れる。
            // 実質「読取開始位置をずらす」用途にしか使えない。
            TestUtil.OutputArray("CopyArray(読取開始位置をずらす)",
                delegate { return ArrayOperator.CopyArray<int>(src, 2, 1, 0); });
            TestUtil.OutputArray("CopyArray(書込開始位置をずらす)",
                delegate { return ArrayOperator.CopyArray<int>(src, 2, 0, 1); });

            // 連結
            TestUtil.OutputArray("CombineArray",
                delegate { return ArrayOperator.CombineArray<int>(src, new int[] { 4, 5 }); });

            // バイト配列
            TestUtil.OutputArray("ShortenByteArray(8 → 3)",
                delegate { return ArrayOperator.ShortenByteArray(bytes, 3); });

            MyDebug.OutputDebugAndConsole(
                "GetLongFromByte : " + ArrayOperator.GetLongFromByte(bytes));
        }

        /// <summary>設定と環境情報</summary>
        /// <remarks>
        /// **値は出さない。** 実行環境で変わるため。
        /// 「取れること」「無いキーは null」までを確認する。
        /// </remarks>
        private static void TestConfigAndEnv()
        {
            MyDebug.OutputDebugAndConsole("GetConfigParameter / EnvInfo");

            // App.config / appsettings.json の両方に同じ値を書いてあるキー。
            MyDebug.OutputDebugAndConsole(
                "GetConfigValue(FxBusinessMessageCulture) : "
                + GetConfigParameter.GetConfigValue("FxBusinessMessageCulture"));

            // 無いキー
            string notExist = GetConfigParameter.GetConfigValue("OpenTouryo_NotExistKey");
            MyDebug.OutputDebugAndConsole(
                "無いキー : " + (string.IsNullOrEmpty(notExist) ? "null または空" : "値あり"));

            // 環境情報は**取得できることだけ**を見る。
            MyDebug.OutputDebugAndConsole(
                "EnvInfo.MachineName が取れる       : " + !string.IsNullOrEmpty(EnvInfo.MachineName));
            MyDebug.OutputDebugAndConsole(
                "EnvInfo.OsVersionString が取れる   : " + !string.IsNullOrEmpty(EnvInfo.OsVersionString));
            MyDebug.OutputDebugAndConsole(
                "EnvInfo.ProcessBit が 32 または 64 : "
                + (EnvInfo.ProcessBit == 32 || EnvInfo.ProcessBit == 64));
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>ChangeType の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="src">変換元</param>
        /// <param name="dstType">変換先の型</param>
        private static void OutputChangeType(string caseName, object src, Type dstType)
        {
            try
            {
                object ret = PubCmnFunction.ChangeType(src, dstType);

                MyDebug.OutputDebugAndConsole(
                    "ChangeType " + caseName + " : "
                    + (ret == null ? "null" : ret.ToString() + " (" + ret.GetType().Name + ")"));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(
                    "ChangeType " + caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>型名を返す</summary>
        /// <param name="t">型</param>
        /// <returns>型名（null の場合は "(null)"）</returns>
        private static string TypeName(Type t)
        {
            return (t == null) ? "(null)" : t.Name;
        }

        /// <summary>配列を返す処理</summary>
        /// <returns>配列</returns>
        private delegate Array ArrayFunc();

        /// <summary>配列操作の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="func">配列を返す処理</param>
        /// <remarks>
        /// **例外を必ず捕まえる。** 捕まえないと Program.Main まで抜けて実行が中断し、
        /// さらに**パス入りのスタック トレースが結果ファイルに出る**（環境依存の差分になる）。
        /// </remarks>
        private static void OutputArray(string caseName, ArrayFunc func)
        {
            try
            {
                MyDebug.OutputDebugAndConsole(caseName + " : " + TestUtil.Join(func()));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>配列を連結して出力用の文字列にする</summary>
        /// <param name="array">配列</param>
        /// <returns>連結した文字列</returns>
        private static string Join(Array array)
        {
            if (array == null)
            {
                return "(null)";
            }

            string ret = "";

            foreach (object o in array)
            {
                ret += (ret == "" ? "" : ", ") + o.ToString();
            }

            return "[" + ret + "]";
        }

        #endregion
    }
}
