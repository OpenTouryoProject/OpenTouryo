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
//* クラス名        ：DaoDefinitionOptions
//* クラス日本語名  ：Ｄ層定義情報ファイル生成処理のオプション
//*
//* 作成者          ：－
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/07/31  ＸＸ ＸＸ         新規作成（GUIのCUI化）
//**********************************************************************************

namespace DaoGen_Tool
{
    /// <summary>
    /// Ｄ層定義情報ファイル生成処理のオプション
    /// </summary>
    /// <remarks>
    /// 生成処理（Form1.DaoDefinitionGen）を GUI からも CUI からも呼び出せるようにするための引数。
    /// UI コントロールへの参照を持たず、フィールドは全て組み込み型とする。
    /// ・GUI 起動時は Form1 のイベント ハンドラが UI コントロールから値を読んで処理を呼び出す。
    /// ・CUI 起動時は Program がコマンドライン引数から生成する。
    /// </remarks>
    public class DaoDefinitionOptions
    {
        #region 接続情報

        /// <summary>データ プロバイダ（SQL / OLE / ODB / ODP / DB2 / HIR / MCN / NPS）</summary>
        public string Dap = "SQL";

        /// <summary>接続文字列</summary>
        public string ConnectionString = "";

        #endregion

        #region 出力

        /// <summary>出力先ファイル（*.csv）へのパス</summary>
        /// <remarks>
        /// 同じ場所に "*_DBTypeInfo.csv" "*_DotNetTypeInfo.csv" も出力される。
        /// </remarks>
        public string OutputFilePath = "";

        /// <summary>Ｄ層定義情報ファイルの出力エンコーディング（コード ページ）</summary>
        public int CodePage = 65001;

        #endregion

        #region 生成対象

        /// <summary>生成対象とするテーブル・ビュー名の配列</summary>
        /// <remarks>要素が無い場合は、DBMS 上の全テーブル・ビューを対象とする。</remarks>
        public string[] Tables = new string[0];

        /// <summary>生成対象から除外するテーブル・ビュー名の配列</summary>
        public string[] ExcludeTables = new string[0];

        /// <summary>主キー情報（キー：テーブル名、値：主キー列名の配列）</summary>
        /// <remarks>
        /// GUI の「主キー情報の設定」ダイアログに相当する。
        /// 主キー情報を DBMS から取得できないデータ プロバイダ
        /// （ODBC・OLEDB・MySQL・PostgreSQL）で使用する。
        /// </remarks>
        public System.Collections.Generic.Dictionary<string, string[]> PrimaryKeys
            = new System.Collections.Generic.Dictionary<string, string[]>();

        #endregion
    }
}
