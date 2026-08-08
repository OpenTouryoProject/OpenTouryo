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
//* クラス名        ：Program
//* クラス日本語名  ：配布確認用のコンソール アプリケーション
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/08  玄人 幸道         新規作成（#528）
//**********************************************************************************

using System;
using System.IO;

namespace SampleConsoleApp
{
    /// <summary>配布確認用のコンソール アプリケーション</summary>
    /// <remarks>
    /// DeployZipPackWithHTTP で配布したものが、
    /// **配置先で動くこと**を目で確かめるためのサンプル（#528）。
    ///
    /// 自分が置かれたディレクトリとその中身を出して、キー入力で終わる。
    /// 配置先が意図した場所か、同梱ファイルが揃っているかが、これで分かる。
    /// </remarks>
    public class Program
    {
        /// <summary>Main</summary>
        /// <param name="args">string[]</param>
        public static void Main(string[] args)
        {
            // 実行ファイルの場所
            //
            // **カレント ディレクトリを見てはいけない。** 呼び出し側の作業ディレクトリになり、
            // 配布ツールが起動した場合はツール側のカレントを引き継いでしまう。
            // 「配置先で動いているか」を見たいので、自分の居場所を採る。
            string baseDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

            Console.WriteLine("実行ファイルの場所");
            Console.WriteLine("  " + baseDir);
            Console.WriteLine("");

            #region 実行ファイルの場所の一覧

            Console.WriteLine("同じ場所にあるファイルの一覧");

            try
            {
                string[] dirs = Directory.GetDirectories(baseDir);
                Array.Sort(dirs, StringComparer.Ordinal);

                foreach (string dir in dirs)
                {
                    Console.WriteLine("  [D] " + Path.GetFileName(dir));
                }

                string[] files = Directory.GetFiles(baseDir);
                Array.Sort(files, StringComparer.Ordinal);

                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    Console.WriteLine(string.Format("  [F] {0} ({1:N0} バイト)", f.Name, f.Length));
                }

                Console.WriteLine("");
                Console.WriteLine(string.Format(
                    "  フォルダ {0} 件 / ファイル {1} 件", dirs.Length, files.Length));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名も出す。
                Console.WriteLine("  一覧を取得できませんでした : " + ex.GetType().FullName);
                Console.WriteLine("  " + ex.Message);
            }

            #endregion

            Console.WriteLine("");
            Console.WriteLine("何かキーを押すと終了します。");

            // **リダイレクト時は例外になる。** 標準入力が無い状態で ReadKey を呼ぶため。
            // 非対話で実行されることもあるので、握り潰して終わる。
            try
            {
                Console.ReadKey();
            }
            catch { }
        }
    }
}
