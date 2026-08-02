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
//* クラス名        ：DaoGenOptions
//* クラス日本語名  ：Dao・DTO・SQL 生成処理のオプション
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/07/31  玄人 幸道         新規作成（GUIのCUI化）
//**********************************************************************************

namespace DaoGen_Tool
{
    /// <summary>
    /// Dao・DTO・SQL 生成処理のオプション
    /// </summary>
    /// <remarks>
    /// 生成処理（Form2.DaoAndSqlGen）を GUI からも CUI からも呼び出せるようにするための引数。
    /// UI コントロールへの参照を持たず、フィールドは全て組み込み型とする。
    /// ・GUI 起動時は Form2 のイベント ハンドラが UI コントロールから値を読んで生成する。
    /// ・CUI 起動時は Program がコマンドライン引数から生成する。
    /// </remarks>
    public class DaoGenOptions
    {
        #region 入出力パス

        /// <summary>Ｄ層定義情報ファイル（*.csv）へのパス</summary>
        /// <remarks>
        /// 同じ場所の "*_DotNetTypeInfo.csv" "*_DBTypeInfo.csv" も必要に応じて読み込まれる。
        /// </remarks>
        public string DaoDefinitionFilePath = "";

        /// <summary>テンプレート ファイルのルート フォルダ</summary>
        public string TemplateRootPath = "";

        /// <summary>出力先フォルダ</summary>
        public string OutputPath = "";

        #endregion

        #region データ プロバイダ・言語

        /// <summary>データ プロバイダ（SQL / OLE / ODB / ODP / DB2 / MCN / NPS）</summary>
        public string Dap = "SQL";

        /// <summary>true:Visual C# / false:Visual Basic</summary>
        public bool IsCSharp = true;

        #endregion

        #region 生成対象

        /// <summary>エンティティ（DTO）を生成する</summary>
        public bool CreateEntity = false;

        /// <summary>型付きデータセットを生成する</summary>
        public bool CreateTypedDataSet = false;

        /// <summary>テーブル メンテナンス画面を生成する</summary>
        public bool CreateTableMaintenance = false;

        /// <summary>DTO のみ生成する（Dao を生成しない）</summary>
        public bool OnlyDTO = false;

        /// <summary>テーブル メンテナンス画面のみ生成する（Dao を生成しない）</summary>
        public bool OnlyTableMaintenance = false;

        #endregion

        #region 生成オプション

        /// <summary>Ｄ層定義情報ファイルの1行目をヘッダとして読み飛ばす</summary>
        public bool DaoDefinitionHeader = true;

        /// <summary>エスケープ文字（ODP 使用時に1文字必須）</summary>
        public string EscapeChar = "";

        /// <summary>タイム スタンプ列名（空文字なら未使用）</summary>
        public string TimeStampColName = "";

        /// <summary>タイム スタンプの更新方法</summary>
        public string TimeStampUpdMethod = "";

        /// <summary>作成者（姓）</summary>
        public string FamilyName = "";

        /// <summary>作成者（名）</summary>
        public string PersonalName = "";

        #endregion

        #region エンコーディング

        /// <summary>SQL ファイル（XML）のエンコーディング名</summary>
        public string XmlEncoding = "utf-8";

        /// <summary>クラス ファイルのエンコーディング（コード ページ）</summary>
        public int ClassFileCodePage = 65001;

        /// <summary>Ｄ層定義情報ファイルの読み込みエンコーディング（コード ページ）</summary>
        public int DaoDefinitionCodePage = 65001;

        /// <summary>クラス ファイルの出力エンコーディング（コード ページ）</summary>
        public int ClassFileOutputCodePage = 65001;

        /// <summary>SQL ファイルの出力エンコーディング（コード ページ）</summary>
        public int SqlFileCodePage = 65001;

        /// <summary>SQL テンプレートの読み込みエンコーディング（コード ページ）</summary>
        public int SqlTemplateCodePage = 65001;

        /// <summary>LIKE 文（Oracle 用の LIKE 句の記述）</summary>
        public string LikeStatement = "LIKE";

        #endregion
    }
}
