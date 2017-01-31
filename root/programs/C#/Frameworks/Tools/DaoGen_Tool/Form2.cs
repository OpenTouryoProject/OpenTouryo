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
//* クラス名        ：Form2
//* クラス日本語名  ：D層自動生成ツール（墨壺） - D層、Dao・SQLファイル生成画面
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2010/02/18  西野 大介         データプロバイダ追加（HiRDB、PostgreSQL）
//*  2012/02/06  西野 大介         Select Countメソッドの追加
//*  2012/02/09  西野 大介         HiRDBデータプロバイダのコメントアウト（（ソフト）対応せず）
//*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
//*  2012/08/21  西野 大介         データプロバイダ設定の引き継ぎ処理を追加
//*  2012/08/21  西野 大介         動的SQL（SELECT系）のLIKE検索対応
//*  2012/11/21  西野 大介         Entity、DataSet自動生成の対応
//*  2013/03/13  加藤 幸紀         ODP.NET Like検索用エスケープ文字設定対応
//*  2013/11/19  西野 大介         Entity、DataSet自動生成の対応（DTOのみの生成）
//*  2013/11/19  西野 大介         静・動共用 PKカラム リストを追加。
//*  2013/12/23  西野 大介         テーブルメンテ自動生成コードの追加準備
//*  2014/01/20  西野 大介         I/O時のエンコーディング制御方式を見直し。
//*  2014/02/04  西野 大介         テーブル・メンテ自動生成の実装例を追加
//*  2014/02/28  Sai-san           Timestamp related code
//*  2014/02/28  Santosh-san       Gridview Header column number issue fix in the template
//*  2014/04/30  Santosh san       Internationalization: Added Method to get the strings from the resource files based on the keys values passed.
//*                                and and replaced this method wherever hard coded values.
//*  2014/07/16  Sai-san           Changed ParamSign value to @ in case of PostGreSQL db.
//*  2014/08/25  Santosh Avaji     Added key values to app.config and constants after removing dropdownlist from Template files,
//*                                and did code modification for selecting data provider type.
//*  2015/03/20  Sandeep Nayak     Added TimeStamp placeholder's Key values in app.config file and 
//*                                added constant to get the TimeStamp placeholder from the app.config file and 
//*                                did code modification to replace the required TimeStamp code in the template, if TimeStamp selected in the tool.
//*  2015/06/18  Sai-san           Added <Else></Else> tag in create string method and ReplaceSQL method to fix the bug 'DynInsParameter of dynamic insert'.
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

// [何某]
// ・プロパティ プロシージャ
// ・バインド変数
// ・メンバ変数
// ・XSDのxs:element

namespace DaoGen_Tool
{
    /// <summary>墨壺 - D層、Dao・SQLファイル生成画面</summary>
    public partial class Form2 : Form
    {
        #region インスタンス変数

        #region ファイル、クラス、メソッド名

        /// <summary>SQLファイル（静的）の拡張子</summary>
        private readonly string SqlFileExtension = "sql";

        /// <summary>SQLファイル（動的）の拡張子</summary>
        private readonly string XmlFileExtension = "xml";

        #region 読み

        #region Daoテンプレート ファイル

        /// <summary>Daoテンプレート ファイル名</summary>
        private string DaoTemplateFileName = "";

        /// <summary>Daoテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string DaoTemplateFilePath = "";

        #endregion

        // ↓↓↓【追加】↓↓↓

        #region DTOテンプレート ファイル

        #region Entityテンプレート ファイル

        /// <summary>Entityテンプレート ファイル名</summary>
        private string EntityTemplateFileName = "";
        /// <summary>Entityテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string EntityTemplateFilePath = "";

        #endregion

        #region DataSetテンプレート ファイル

        /// <summary>DataSetテンプレート ファイル名</summary>
        private string DataSetTemplateFileName = "";
        /// <summary>DataSetテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string DataSetTemplateFilePath = "";

        #endregion

        #endregion

        #region メンテナンス画面テンプレート ファイル

        #region TableAdapterテンプレート ファイル

        /// <summary>TableAdapterテンプレート ファイル名</summary>
        private string TableAdapterTemplateFileName = "";
        /// <summary>TableAdapterテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string TableAdapterTemplateFilePath = "";

        #endregion

        #region 画面テンプレート ファイル

        /// <summary>ConditionalSearchテンプレート ファイル名</summary>
        private string ConditionalSearchTemplateFileName = "";
        /// <summary>ConditionalSearchテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string ConditionalSearchTemplateFilePath = "";

        /// <summary>SearchAndUpdateテンプレート ファイル名</summary>
        private string SearchAndUpdateTemplateFileName = "";
        /// <summary>SearchAndUpdateテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string SearchAndUpdateTemplateFilePath = "";

        /// <summary>Detailテンプレート ファイル名</summary>
        private string DetailTemplateFileName = "";
        /// <summary>Detailテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string DetailTemplateFilePath = "";

        #endregion

        #endregion

        // ↑↑↑【追加】↑↑↑

        #region SQLテンプレート ファイル

        #region 静的

        /// <summary>SQLテンプレート ファイル名（静的：Insert）</summary>
        private string InsertTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（静的：Insert）</summary>
        /// <remarks>初期化対象外</remarks>
        private string InsertTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（静的：Select）</summary>
        private string SelectTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（静的：Select）</summary>
        /// <remarks>初期化対象外</remarks>
        private string SelectTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（静的：Update）</summary>
        private string UpdateTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（静的：Update）</summary>
        /// <remarks>初期化対象外</remarks>
        private string UpdateTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（静的：Delete）</summary>
        private string DeleteTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（静的：Delete）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DeleteTemplateFilePath = "";

        #endregion

        #region 動的

        /// <summary>SQLテンプレート ファイル名（動的：Insert）</summary>
        private string DynInsTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（動的：Insert）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DynInsTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（動的：Select）</summary>
        private string DynSelTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（動的：Select）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DynSelTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（動的：Update）</summary>
        private string DynUpdTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（動的：Update）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DynUpdTemplateFilePath = "";

        /// <summary>SQLテンプレート ファイル名（動的：Delete）</summary>
        private string DynDelTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（動的：Delete）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DynDelTemplateFilePath = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>SQLテンプレート ファイル名（動的：SelCnt）</summary>
        private string DynSelCntTemplateFileName = "";
        /// <summary>SQLテンプレート ファイル パス（動的：SelCnt）</summary>
        /// <remarks>初期化対象外</remarks>
        private string DynSelCntTemplateFilePath = "";
        // ↑↑↑【追加】↑↑↑

        #endregion

        #endregion

        #endregion

        #region 書き

        #region Daoクラス ファイル

        /// <summary>Daoクラス名のヘッダ</summary>
        private string DaoClassNameHeader = "";
        /// <summary>Daoクラス名のフッタ</summary>
        private string DaoClassNameFooter = "";

        /// <summary>DaoメソッドのCRUD名（Insert）</summary>
        private string MethodLabel_Ins = "";
        /// <summary>DaoメソッドのCRUD名（Select）</summary>
        private string MethodLabel_Sel = "";
        /// <summary>DaoメソッドのCRUD名（Update）</summary>
        private string MethodLabel_Upd = "";
        /// <summary>DaoメソッドのCRUD名（Delete）</summary>
        private string MethodLabel_Del = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>DaoメソッドのCRUD名（SelCnt）</summary>
        private string MethodLabel_SelCnt = "";
        // ↑↑↑【追加】↑↑↑

        /// <summary>Daoメソッド名のヘッダ（静的）</summary>
        private string MethodNameHeaderS = "";
        /// <summary>Daoメソッド名のフッタ（静的）</summary>
        private string MethodNameFooterS = "";

        /// <summary>Daoメソッド名のヘッダ（動的）</summary>
        private string MethodNameHeaderD = "";
        /// <summary>Daoメソッド名のフッタ（動的）</summary>
        private string MethodNameFooterD = "";

        /// <summary>更新用のパラメタ[何某]のヘッダ</summary>
        private string UpdateParamHeader = "";
        /// <summary>更新用のパラメタ[何某]のヘッダ</summary>
        private string UpdateParamFooter = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>曖昧検索用のパラメタ[何某]のヘッダ</summary>
        private string LikeParamHeader = "";
        /// <summary>曖昧検索用のパラメタ[何某]のヘッダ</summary>
        private string LikeParamFooter = "";
        // ↑↑↑【追加】↑↑↑

        // ※　SQLファイル名は、Daoクラス名＋メソッド名とする。

        #endregion

        // ↓↓↓【追加】↓↓↓

        #region DTO

        #region Entityクラス ファイル

        /// <summary>Entityクラス名のヘッダ</summary>
        private string EntityClassNameHeader = "";
        /// <summary>Entityクラス名のフッタ</summary>
        private string EntityClassNameFooter = "";

        #endregion

        #region DataSetクラス ファイル

        /// <summary>DataSetクラス名のヘッダ</summary>
        private string DataSetClassNameHeader = "";
        /// <summary>DataSetクラス名のフッタ</summary>
        private string DataSetClassNameFooter = "";

        /// <summary>DataTableクラス名のヘッダ</summary>
        private string DataTableClassNameHeader = "";
        /// <summary>DataTableクラス名のフッタ</summary>
        private string DataTableClassNameFooter = "";

        #endregion

        #endregion

        // ↑↑↑【追加】↑↑↑

        #endregion

        #endregion

        #region 置換文字列と置換値

        #region 共通

        /// <summary>ファイル名（置換対象）</summary>
        private string RpFileName = "";
        /// <summary>ファイル名（置換文字列）</summary>
        private string FileName = "";

        /// <summary>タイム スタンプ（置換対象）</summary>
        private string RpTimeStamp = "";
        /// <summary>タイム スタンプ（置換文字列）</summary>
        private string TimeStamp = "";

        /// <summary>ユーザ名（置換対象）</summary>
        private string RpUserName = "";
        /// <summary>ユーザ名（置換文字列）</summary>
        private string UserName = "";

        /// <summary>テーブル名（置換対象）</summary>
        private string RpTableName = "";
        /// <summary>テーブル名（置換文字列）</summary>
        private string TableName = "";

        /// <summary>囲い文字のない カラム リスト（置換対象）</summary>
        private string RpAllColumnList = "";
        /// <summary>囲い文字のない カラム リスト（置換文字列）</summary>
        private string AllColumnList = "";

        /// <summary>囲い文字のない PKカラム リスト（置換対象）</summary>
        private string RpPKColumnList = "";
        /// <summary>囲い文字のない PKカラム リスト（置換文字列）</summary>
        private string PKColumnList = "";

        #endregion

        #region SQL

        /// <summary>XML用のエンコーディング指定（置換対象）</summary>
        private string RpXMLEncoding = "";
        /// <summary>XML用のエンコーディング指定（置換文字列）</summary>
        private string XMLEncoding = "";

        /// <summary>静・動共用 カラム リスト（置換対象）</summary>
        private string RpAllColumnListSQL = "";
        /// <summary>静・動共用 カラム リスト（置換文字列）</summary>
        private string AllColumnListSQL = "";

        /// <summary>静・動共用 PKカラム リスト（置換対象）</summary>
        private string RpPKColumnListSQL = "";
        /// <summary>静・動共用 PKカラム リスト（置換文字列）</summary>
        private string PKColumnListSQL = "";

        // 検索条件

        /// <summary>静的SQL用 検索条件（置換対象）</summary>
        private string RpColumnsCondition = "";
        /// <summary>静的SQL用 検索条件（置換文字列）</summary>
        private string ColumnsCondition = "";

        /// <summary>動的SQL用 検索条件（置換対象）</summary>
        private string RpDynColsCondition = "";
        /// <summary>動的SQL用 検索条件（置換文字列）</summary>
        private string DynColsCondition = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>動的SQL用 曖昧検索条件（置換対象）</summary>
        private string RpDynColsCondition_Like = "";
        /// <summary>動的SQL用 曖昧検索条件（置換文字列）</summary>
        private string DynColsCondition_Like = "";
        // ↑↑↑【追加】↑↑↑

        // 挿入列

        ///// <summary>静的SQL用 挿入カラムリスト（置換対象）</summary>
        //private string RpInsertColumns = "";
        ///// <summary>静的SQL用 挿入カラムリスト（置換文字列）</summary>
        //private string InsertColumns = "";

        /// <summary>動的SQL用 挿入カラムリスト（置換対象）</summary>
        private string RpDynInsColumns = "";
        /// <summary>動的SQL用 挿入カラムリスト（置換文字列）</summary>
        private string DynInsColumns = "";

        // 挿入パラメタ

        /// <summary>静的SQL用 挿入バインド変数リスト（置換対象）</summary>
        private string RpInsertParameters = "";
        /// <summary>静的SQL用 挿入バインド変数リスト（置換文字列）</summary>
        private string InsertParameters = "";

        /// <summary>動的SQL用 挿入バインド変数リスト（置換対象）</summary>
        private string RpDynInsParameters = "";
        /// <summary>動的SQL用 挿入バインド変数リスト（置換文字列）</summary>
        private string DynInsParameters = "";

        // 更新パラメタ

        /// <summary>動的SQL用 更新バインド変数リスト（置換対象）</summary>
        private string RpDynUpdParameters = "";
        /// <summary>動的SQL用 更新バインド変数リスト（置換文字列）</summary>
        private string DynUpdParameters = "";

        #endregion

        #region Dao

        // - Daoクラス・クラス ファイル名

        /// <summary>Daoクラス（ファイル）名（置換対象）</summary>
        private string RpDaoClassName = "";
        /// <summary>Daoクラス（ファイル）名（置換文字列）</summary>
        private string DaoClassName = "";

        // - [何某]生成用

        /// <summary>カラム名（置換対象）</summary>
        private string RpColumnName = "";
        /// <summary>カラム名（置換文字列）</summary>
        private string ColumnName = "";

        /// <summary>カラム番号（置換対象）</summary>
        private string RpColumnNmbr = "";
        /// <summary>カラム番号（置換文字列）</summary>
        private int ColumnNmbr = 0; //sntosh for gridview header text

        #region メソッド名

        /// <summary>Insertのメソッド名（置換対象）</summary>
        private string RpInsertMethodName = "";
        /// <summary>Insertのメソッド名（置換文字列）</summary>
        private string InsertMethodName = "";

        /// <summary>DynInsのメソッド名（置換対象）</summary>
        private string RpDynInsMethodName = "";
        /// <summary>DynInsのメソッド名（置換文字列）</summary>
        private string DynInsMethodName = "";

        /// <summary>Selectのメソッド名（置換対象）</summary>
        private string RpSelectMethodName = "";
        /// <summary>Selectのメソッド名（置換文字列）</summary>
        private string SelectMethodName = "";

        /// <summary>DynSelのメソッド名（置換対象）</summary>
        private string RpDynSelMethodName = "";
        /// <summary>DynSelのメソッド名（置換文字列）</summary>
        private string DynSelMethodName = "";

        /// <summary>Updateのメソッド名（置換対象）</summary>
        private string RpUpdateMethodName = "";
        /// <summary>Updateのメソッド名（置換文字列）</summary>
        private string UpdateMethodName = "";

        /// <summary>DynUpdのメソッド名（置換対象）</summary>
        private string RpDynUpdMethodName = "";
        /// <summary>DynUpdのメソッド名（置換文字列）</summary>
        private string DynUpdMethodName = "";

        /// <summary>Deleteのメソッド名（置換対象）</summary>
        private string RpDeleteMethodName = "";
        /// <summary>Deleteのメソッド名（置換文字列）</summary>
        private string DeleteMethodName = "";

        /// <summary>DynDelのメソッド名（置換対象）</summary>
        private string RpDynDelMethodName = "";
        /// <summary>DynDelのメソッド名（置換文字列）</summary>
        private string DynDelMethodName = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>DynSelCntのメソッド名（置換対象）</summary>
        private string RpDynSelCntMethodName = "";
        /// <summary>DynSelCntのメソッド名（置換文字列）</summary>
        private string DynSelCntMethodName = "";
        // ↑↑↑【追加】↑↑↑

        #endregion

        #region SQLファイル名

        /// <summary>InsertのSQLファイル名（置換対象）</summary>
        private string RpInsertFileName = "";
        /// <summary>InsertのSQLファイル名（置換文字列）</summary>
        private string InsertFileName = "";

        /// <summary>DynInsのSQLファイル名（置換対象）</summary>
        private string RpDynInsFileName = "";
        /// <summary>DynInsのSQLファイル名（置換文字列）</summary>
        private string DynInsFileName = "";

        /// <summary>SelectのSQLファイル名（置換対象）</summary>
        private string RpSelectFileName = "";
        /// <summary>SelectのSQLファイル名（置換文字列）</summary>
        private string SelectFileName = "";

        /// <summary>DynSelのSQLファイル名（置換対象）</summary>
        private string RpDynSelFileName = "";
        /// <summary>DynSelのSQLファイル名（置換文字列）</summary>
        private string DynSelFileName = "";

        /// <summary>UpdateのSQLファイル名（置換対象）</summary>
        private string RpUpdateFileName = "";
        /// <summary>UpdateのSQLファイル名（置換文字列）</summary>
        private string UpdateFileName = "";

        /// <summary>DynUpdのSQLファイル名（置換対象）</summary>
        private string RpDynUpdFileName = "";
        /// <summary>DynUpdのSQLファイル名（置換文字列）</summary>
        private string DynUpdFileName = "";

        /// <summary>DeleteのSQLファイル名（置換対象）</summary>
        private string RpDeleteFileName = "";
        /// <summary>DeleteのSQLファイル名（置換文字列）</summary>
        private string DeleteFileName = "";

        /// <summary>DynDelのSQLファイル名（置換対象）</summary>
        private string RpDynDelFileName = "";
        /// <summary>DynDelのSQLファイル名（置換文字列）</summary>
        private string DynDelFileName = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>DynSelCntのSQLファイル名（置換対象）</summary>
        private string RpDynSelCntFileName = "";
        /// <summary>DynSelCntのSQLファイル名（置換文字列）</summary>
        private string DynSelCntFileName = "";
        // ↑↑↑【追加】↑↑↑

        #endregion

        #region [何某]生成 制御用文字列

        /// <summary>主キー列の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_PKColumn = "";
        /// <summary>主キー列の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_PKColumn = "";
        /// <summary>主キー以外の列の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_ElseColumn = "";
        /// <summary>主キー以外の列の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_ElseColumn = "";
        /// <summary>Update文のSet句用の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_PPUpdSet = "";
        /// <summary>Update文のSet句用の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_PPUpdSet = "";

        // ↓↓↓【追加】↓↓↓
        /// <summary>Select文のLike句用の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_PPLike = "";
        /// <summary>Select文のLike句用の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_PPLike = "";
        // ↑↑↑【追加】↑↑↑        

        /// <summary>CcIsRequired_TimeStamp</summary>
        private string CcIsRequired_TimeStamp = string.Empty;

        #endregion

        #endregion

        // ↓↓↓【追加】↓↓↓
        #region DTO

        #region Entity

        /// <summary>Entityクラス（ファイル）名（置換対象）</summary>
        private string RpEntityClassName = "";
        /// <summary>Entityクラス（ファイル）名（置換文字列）</summary>
        private string EntityClassName = "";

        /// <summary>Entityフィールド型（置換対象）</summary>
        private string RpEntityTypeInfo = "";
        /// <summary>Entityフィールド型（置換文字列）</summary>
        private string EntityTypeInfo = "";

        #endregion

        #region DataSet

        /// <summary>DataSetクラス（ファイル）名（置換対象）</summary>
        private string RpDataSetClassName = "";
        /// <summary>DataSetクラス（ファイル）名（置換文字列）</summary>
        private string DataSetClassName = "";

        /// <summary>DataTableクラス名（置換対象）</summary>
        private string RpDataTableClassName = "";
        /// <summary>DataTableクラス名（置換文字列）</summary>
        private string DataTableClassName = "";

        /// <summary>XSDフィールド型（置換対象）</summary>
        private string RpXSDTypeInfo = "";
        /// <summary>XSDフィールド型（置換文字列）</summary>
        private string XSDTypeInfo = "";

        #endregion

        #endregion

        #region メンテナンス画面

        /// <summary>TimeStampカラム名（置換対象）</summary>
        private string RpTimeStampColName = "";
        /// <summary>TimeStampカラム名（置換文字列）</summary>
        private string TimeStampColName = "";

        /// <summary>TableAdapterで使用するカラム リスト（置換対象）</summary>
        private string RpAllColumnListTableAdapterSQL = "";
        /// <summary>TableAdapterで使用するカラム リスト（置換文字列）</summary>
        private string AllColumnListTableAdapterSQL = "";

        /// <summary>コードビハインドの言語（置換対象）</summary>
        private string RpCodebehindLanguage = "";
        /// <summary>コードビハインドの言語（置換文字列）</summary>
        private string CodebehindLanguage = "";

        /// <summary>クラス・ファイルの拡張子（置換対象）</summary>
        private string RpClassTemplateFileExtension = "";
        /// <summary>クラス・ファイルの拡張子（置換文字列）</summary>
        private string ClassTemplateFileExtension = "";

        /// <summary>コメントアウト（置換対象）</summary>
        private string RpCommentOut = "";
        /// <summary>コメントアウト（置換対象）</summary>
        private string CommentOut = "";

        /// <summary>主キーの先頭列（置換対象）</summary>
        private string RpPKFirstColumn = "";
        /// <summary>主キーの先頭列（置換対象）</summary>
        private string PKFirstColumn = "";

        #endregion
        // ↑↑↑【追加】↑↑↑

        #endregion

        #region タイムスタンプ列情報

        // タイムスタンプ列更新方法
        private string TimeStampUpdMethod = "";
        // タイムスタンプ情報設定ステータス
        private TSS TimeStampStatus = TSS.Non;

        // 上記ステータスの列挙型
        enum TSS
        {
            Non = 1,
            Name,
            NandM
        };

        #endregion

        #region その他、ワーク変数

        // パラメタの先頭記号（DBMSによって可変）
        private char ParamSign;

        // 囲い文字の記号（DBMSによって可変）
        private char EnclosureCharS;
        private char EnclosureCharE;

        // カラム情報
        private ArrayList PK_Columns = null;
        private ArrayList ELSE_Columns = null;
        // ↓↓↓【追加】↓↓↓
        private ArrayList PK_ColumnsType = null;
        private ArrayList ELSE_ColumnsType = null;
        // ↑↑↑【追加】↑↑↑
        // ↓↓↓【追加】↓↓↓
        private ArrayList PK_ColumnsDbType = null;
        private ArrayList ELSE_ColumnsDbType = null;
        // ↑↑↑【追加】↑↑↑

        // ↓↓↓【追加】↓↓↓

        // DTO生成の有無
        private bool CreateDTO = false;
        // MaintenanceScreen生成の有無
        private bool CreateMaintenanceScreen = false;

        // .NET型情報ファイルのパス
        //private bool ReadDotNetTypeInfo = false;
        // .NET型情報ファイルのパス
        private string DotNetTypeInfoFilePath = "";

        // DB型情報ファイル読み込みフラグ
        private bool ReadDbTypeInfo = false;
        // DB型情報ファイルのパス
        private string DbTypeInfoFilePath = "";

        // ↑↑↑【追加】↑↑↑

        // DBMS constant to replace
        private string RpDBMS = "";
        //DBMS value  to  be replaced
        private string strDBMS = "";

        //DAP constant to replace
        private string RpDAP = "";
        //DAP value to be replaced
        private string strDAP = "";

        #endregion

        #endregion

        #region 各種、設定オペ

        // 各種共通関数
        #region 共通関数

        #region ファイル パス情報の作成

        /// <summary>ファイルパス情報の作成</summary>
        /// <param name="directory">ディレクトリ</param>
        /// <param name="fileName">ファイル</param>
        /// <param name="extension">拡張子</param>
        /// <returns>作成したファイルパス情報</returns>
        private string CreateFilePath(string directory, string fileName, string extension)
        {
            string tempPath = "";

            // ファイルパスを作成（x:\対策）
            if (directory[directory.Length - 1] == '\\')
            {
                // x:\の時、\を足さない。

                // 拡張子設定
                if (extension == "")
                {
                    // 拡張子設定・無し
                    tempPath = directory + fileName;
                }
                else
                {
                    // 拡張子設定・有り
                    tempPath = directory + fileName + "." + extension;
                }
            }
            else
            {
                // x:\以外の時、\を足す。

                // 拡張子設定
                if (extension == "")
                {
                    // 拡張子設定・無し
                    tempPath = directory + @"\" + fileName;
                }
                else
                {
                    // 拡張子設定・有り
                    tempPath = directory + @"\" + fileName + "." + extension;
                }
            }

            return tempPath;
        }

        #endregion

        #region ファイルなど存在チェック メソッド

        /// <summary>テンプレート ファイルの存在チェック</summary>
        /// <param name="directory">ディレクトリ</param>
        /// <param name="fileName">ファイル</param>
        /// <param name="extension">拡張子</param>
        /// <returns>存在するファイルのパス情報を戻す</returns>
        private string CheckTemplateFile(string directory, string fileName, string extension)
        {
            // ワーク領域
            string tempPath = this.CreateFilePath(directory, fileName, extension);

            // FileInfoの作成
            FileInfo fileInfo = new FileInfo(tempPath);

            // 存在チェック
            if (fileInfo.Exists)
            {
                return tempPath;
            }
            else
            {
                // throw new CheckException("テンプレート ファイルが存在しません。：" + tempPath) ;
                throw new CheckException(this.RM_GetString("TempFilenotExists") + tempPath);

            }
        }

        #endregion

        #region ストリーム オープン関数

        #region StreamReader

        /// <summary>定義情報ファイルを開く（読込）</summary>
        /// <param name="ddi">D層定義情報</param>
        /// <param name="dti">.NET型情報</param>
        /// <param name="dbti">DB型情報</param>
        private void OpenSrDefInfo(out StreamReader ddi, out StreamReader dti, out StreamReader dbti)
        {
            ddi = null;
            dti = null;
            dbti = null;

            // エンコーディング
            Encoding enc = null;

            // DDE：D層定義情報のEncoding
            enc = Encoding.GetEncoding((int)this.cmbDDEncoding.SelectedValue);

            // 読込ファイル ストリームを生成する。
            ddi = new StreamReader(this.txtSetDaoDefinition.Text, enc);

            // ↓↓↓【追加】↓↓↓
            // ＋エンティテイ生成＋型付きデータセット生成
            if (this.CreateDTO)
            {
                // 読込ファイル ストリームを生成する。
                dti = new StreamReader(this.DotNetTypeInfoFilePath, enc);
            }
            // ↑↑↑【追加】↑↑↑

            // ↓↓↓【追加】↓↓↓
            // DB型情報を読み込む場合
            if (this.ReadDbTypeInfo)
            {
                // 読込ファイル ストリームを生成する。
                dbti = new StreamReader(this.DbTypeInfoFilePath, enc);
            }
            // ↑↑↑【追加】↑↑↑
        }

        /// <summary>テンプレート ファイルを開く（読込）</summary>
        /// <param name="templateFilePath">テンプレート ファイルへのパス</param>
        /// <param name="encoding">エンコーディング</param>
        /// <returns>StreamReader</returns>
        private StreamReader OpenSrTemplate(string templateFilePath, int encoding)
        {
            // 読込ファイル ストリームを生成する。
            return new StreamReader(templateFilePath, Encoding.GetEncoding(encoding));
        }

        #endregion

        #region StreamWriter

        /// <summary>クラス ファイルを開く（書込）</summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>StreamWriter</returns>
        private StreamWriter OpenSwClassFile(string fileName, string extension)
        {
            // ファイルパス情報の作成
            string tempPath = this.CreateFilePath(this.txtSetOutput.Text, fileName, extension);

            // 書込ファイル ストリームを生成する。

            // DFE：DaoClassファイルのEncoding
            return new StreamWriter(tempPath, false, Encoding.GetEncoding((int)this.cmbDFEncoding.SelectedValue));
        }

        /// <summary>SQLファイルを開く（書込）</summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>StreamWriter</returns>
        private StreamWriter OpenSwSQLFile(string fileName, string extension)
        {
            // ファイルパス情報の作成
            string tempPath = this.CreateFilePath(this.txtSetOutput.Text, fileName, extension);

            // 書込ファイル ストリームを生成する。

            // SFE：SQLファイルのEncoding
            return new StreamWriter(tempPath, false, Encoding.GetEncoding((int)this.cmbSFEncoding.SelectedValue));
        }

        #endregion

        #endregion

        #endregion

        // 初期処理
        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form2()
        {
            InitializeComponent();
            this.tabControl1.Controls.Remove(this.tabPage2);
        }

        /// <summary>プロバイダの設定</summary>
        /// <param name="s">プロバイダを表す文字列</param>
        public void Init(string s)
        {
            this.strDAP = s;
            switch (s)
            {
                case "OLE":
                    rbnOLE.Checked = true;
                    this.rbnOLE.Select();
                    this.strDBMS = "OLE";
                    break;
                case "ODB":
                    rbnODB.Checked = true;
                    this.rbnODB.Select();
                    this.strDBMS = "ODB";
                    break;
                case "ODP":
                    rbnODP.Checked = true;
                    this.rbnODP.Select();
                    this.strDBMS = "Oracle";
                    break;
                case "DB2":
                    rbnDB2.Checked = true;
                    this.rbnDB2.Select();
                    this.strDBMS = "DB2";
                    break;
                //case "HIR":
                //    this.rbnHiRDB.Select();
                //    break;
                case "MCN":
                    rbnMySQL.Checked = true;
                    this.rbnMySQL.Select();
                    this.strDBMS = "MCN";
                    break;
                case "NPS":
                    rbnPstgrs.Checked = true;
                    this.rbnPstgrs.Select();
                    this.strDBMS = "PstGrS";
                    break;
                default:
                    rbnSQL.Checked = true;
                    this.rbnSQL.Select();
                    this.strDBMS = "SQLServer";
                    break;
            }
        }

        /// <summary>初期設定</summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            #region label visibility

            //Sets label visibilty valur
            this.lblSelected.Visible = false;

            #endregion

            #region ToolTip

            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.txtFamilyName, this.RM_GetString("ToolTipText"));
            toolTip1.SetToolTip(this.txtPersonalName, this.RM_GetString("ToolTipText"));

            #endregion

            #region ラジオ（など）を初期化する。

            //// データプロバイダ
            //this.rbnSQL.Select();

            // D層定義情報ファイル
            //this.rbnDDE_Shift_JIS.Select();
            this.cmbDDEncoding.ValueMember = "key";
            this.cmbDDEncoding.DisplayMember = "value";
            this.cmbDDEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbDDEncoding.SelectedValue = 65001;

            this.cbxDaoDefinitionHeader.Checked = true;

            // テンプレート ファイル

            //this.rbnDTE_UTF_8.Select();
            this.cmbDTEncoding.ValueMember = "key";
            this.cmbDTEncoding.DisplayMember = "value";
            this.cmbDTEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbDTEncoding.SelectedValue = 65001;

            //this.rbnSTE_Shift_JIS.Select();
            this.cmbSTEncoding.ValueMember = "key";
            this.cmbSTEncoding.DisplayMember = "value";
            this.cmbSTEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbSTEncoding.SelectedValue = 65001;

            this.rbnDTL_CS.Select();

            // Daoファイル
            //this.rbnDFE_UTF_8.Select();
            this.cmbDFEncoding.ValueMember = "key";
            this.cmbDFEncoding.DisplayMember = "value";
            this.cmbDFEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbDFEncoding.SelectedValue = 65001;

            // SQLファイル
            //this.rbnSFE_Shift_JIS.Select();
            this.cmbSFEncoding.ValueMember = "key";
            this.cmbSFEncoding.DisplayMember = "value";
            this.cmbSFEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbSFEncoding.SelectedValue = 65001;

            // ↓↓↓【追加】↓↓↓
            // Like記号
            cmbLikeStatement.SelectedIndex = 0;
            // ↑↑↑【追加】↑↑↑
            #endregion

            #region テキストボックスの初期化

            // フォルダパスの初期化
            this.txtSetSourceTemplate.Text = GetConfigParameter.GetConfigValue("inputFilesRoot");
            this.txtSetOutput.Text = GetConfigParameter.GetConfigValue("outputFilesRoot");

            // 氏名
            this.txtFamilyName.Text = GetConfigParameter.GetConfigValue("familyName");
            this.txtPersonalName.Text = GetConfigParameter.GetConfigValue("personalName");

            #endregion

            #region ファイル名、クラス名、メソッド名（＋空文字チェック）

            #region 読み

            #region クラス テンプレート ファイル

            this.DaoTemplateFileName = GetConfigParameter.GetConfigValue("DaoTemplateFileName");
            if (string.IsNullOrEmpty(this.DaoTemplateFileName))
            {
                //throw new CheckException("app.Configパラメタが設定されていません。：DaoTemplateFileName");
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DaoTemplateFileName");
            }

            // ↓↓↓【追加】↓↓↓
            #region DTOテンプレート ファイル

            this.EntityTemplateFileName = GetConfigParameter.GetConfigValue("EntityTemplateFileName");
            if (string.IsNullOrEmpty(this.EntityTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "EntityTemplateFileName");
            }

            this.DataSetTemplateFileName = GetConfigParameter.GetConfigValue("DataSetTemplateFileName");
            if (string.IsNullOrEmpty(this.DataSetTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DataSetTemplateFileName");
            }

            #endregion

            #region メンテナンス画面テンプレート ファイル

            this.TableAdapterTemplateFileName = GetConfigParameter.GetConfigValue("TableAdapterTemplateFileName");
            if (string.IsNullOrEmpty(TableAdapterTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "TableAdapterTemplateFileName");
            }

            this.ConditionalSearchTemplateFileName = GetConfigParameter.GetConfigValue("ConditionalSearchTemplateFileName");
            if (string.IsNullOrEmpty(ConditionalSearchTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "ConditionalSearchTemplateFileName");
            }

            this.SearchAndUpdateTemplateFileName = GetConfigParameter.GetConfigValue("SearchAndUpdateTemplateFileName");
            if (string.IsNullOrEmpty(SearchAndUpdateTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "SearchAndUpdateTemplateFileName");
            }

            this.DetailTemplateFileName = GetConfigParameter.GetConfigValue("DetailTemplateFileName");
            if (string.IsNullOrEmpty(DetailTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DetailTemplateFileName");
            }

            #endregion

            // ↑↑↑【追加】↑↑↑

            #endregion

            #region SQLテンプレート ファイル

            #region 静的

            this.InsertTemplateFileName = GetConfigParameter.GetConfigValue("InsertTemplateFileName");
            if (string.IsNullOrEmpty(this.InsertTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "InsertTemplateFileName");
            }

            this.SelectTemplateFileName = GetConfigParameter.GetConfigValue("SelectTemplateFileName");
            if (string.IsNullOrEmpty(this.SelectTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "SelectTemplateFileName");
            }

            this.UpdateTemplateFileName = GetConfigParameter.GetConfigValue("UpdateTemplateFileName");
            if (string.IsNullOrEmpty(this.UpdateTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "UpdateTemplateFileName");
            }

            this.DeleteTemplateFileName = GetConfigParameter.GetConfigValue("DeleteTemplateFileName");
            if (string.IsNullOrEmpty(this.DeleteTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DeleteTemplateFileName");
            }

            #endregion

            #region 動的

            this.DynInsTemplateFileName = GetConfigParameter.GetConfigValue("DynInsTemplateFileName");
            if (string.IsNullOrEmpty(this.DynInsTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DynInsTemplateFileName");
            }

            this.DynSelTemplateFileName = GetConfigParameter.GetConfigValue("DynSelTemplateFileName");
            if (string.IsNullOrEmpty(this.DynSelTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DynSelTemplateFileName");
            }

            this.DynUpdTemplateFileName = GetConfigParameter.GetConfigValue("DynUpdTemplateFileName");
            if (string.IsNullOrEmpty(this.DynUpdTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DynUpdTemplateFileName");
            }

            this.DynDelTemplateFileName = GetConfigParameter.GetConfigValue("DynDelTemplateFileName");
            if (string.IsNullOrEmpty(this.DynDelTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DynDelTemplateFileName");
            }

            // ↓↓↓【追加】↓↓↓
            this.DynSelCntTemplateFileName = GetConfigParameter.GetConfigValue("DynSelCntTemplateFileName");
            if (string.IsNullOrEmpty(this.DynSelCntTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DynSelCntTemplateFileName");
            }
            // ↑↑↑【追加】↑↑↑

            #endregion

            #endregion

            #endregion

            #region 書き

            #region クラスのヘッダ・フッタ（空文字チェックはしない）

            #region Dao

            this.DaoClassNameHeader = GetConfigParameter.GetConfigValue("DaoClassNameHeader");
            if (this.DaoClassNameHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DaoClassNameHeader");
            }

            this.DaoClassNameFooter = GetConfigParameter.GetConfigValue("DaoClassNameFooter");
            if (this.DaoClassNameFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DaoClassNameFooter");
            }

            #endregion

            // ↓↓↓【追加】↓↓↓
            #region Entity

            this.EntityClassNameHeader = GetConfigParameter.GetConfigValue("EntityClassNameHeader");
            if (this.EntityClassNameHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "EntityClassNameHeader");
            }

            this.EntityClassNameFooter = GetConfigParameter.GetConfigValue("EntityClassNameFooter");
            if (this.EntityClassNameFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "EntityClassNameFooter");
            }

            #endregion

            #region DataSet

            this.DataSetClassNameHeader = GetConfigParameter.GetConfigValue("DataSetClassNameHeader");
            if (this.DataSetClassNameHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DataSetClassNameHeader");
            }

            this.DataSetClassNameFooter = GetConfigParameter.GetConfigValue("DataSetClassNameFooter");
            if (this.DataSetClassNameFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DataSetClassNameFooter");
            }

            this.DataTableClassNameHeader = GetConfigParameter.GetConfigValue("DataTableClassNameHeader");
            if (this.DataTableClassNameHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DataTableClassNameHeader");
            }

            this.DataTableClassNameFooter = GetConfigParameter.GetConfigValue("DataTableClassNameFooter");
            if (this.DataTableClassNameFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "DataTableClassNameFooter");
            }

            #endregion
            // ↑↑↑【追加】↑↑↑

            #endregion

            #region メソッドラベル

            this.MethodLabel_Ins = GetConfigParameter.GetConfigValue("MethodLabel_Ins");
            if (string.IsNullOrEmpty(this.MethodLabel_Ins))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodLabel_Ins");
            }

            this.MethodLabel_Sel = GetConfigParameter.GetConfigValue("MethodLabel_Sel");
            if (string.IsNullOrEmpty(this.MethodLabel_Sel))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodLabel_Sel");
            }

            this.MethodLabel_Upd = GetConfigParameter.GetConfigValue("MethodLabel_Upd");
            if (string.IsNullOrEmpty(this.MethodLabel_Upd))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodLabel_Upd");
            }

            this.MethodLabel_Del = GetConfigParameter.GetConfigValue("MethodLabel_Del");
            if (string.IsNullOrEmpty(this.MethodLabel_Del))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodLabel_Del");
            }

            // ↓↓↓【追加】↓↓↓
            this.MethodLabel_SelCnt = GetConfigParameter.GetConfigValue("MethodLabel_SelCnt");
            if (string.IsNullOrEmpty(this.MethodLabel_SelCnt))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodLabel_SelCnt");
            }
            // ↑↑↑【追加】↑↑↑

            #endregion

            #region メソッドのヘッダ・フッタ（空文字チェックはしない）

            this.MethodNameHeaderS = GetConfigParameter.GetConfigValue("MethodNameHeaderS");
            if (this.MethodNameHeaderS == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodNameHeaderS");
            }

            this.MethodNameFooterS = GetConfigParameter.GetConfigValue("MethodNameFooterS");
            if (this.MethodNameFooterS == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodNameFooterS");
            }

            this.MethodNameHeaderD = GetConfigParameter.GetConfigValue("MethodNameHeaderD");
            if (this.MethodNameHeaderD == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodNameHeaderD");
            }

            this.MethodNameFooterD = GetConfigParameter.GetConfigValue("MethodNameFooterD");
            if (this.MethodNameFooterD == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "MethodNameFooterD");
            }

            this.UpdateParamHeader = GetConfigParameter.GetConfigValue("UpdateParamHeader");
            if (this.UpdateParamHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "UpdateParamHeader");
            }

            this.UpdateParamFooter = GetConfigParameter.GetConfigValue("UpdateParamFooter");
            if (this.UpdateParamFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "UpdateParamFooter");
            }

            // ↓↓↓【追加】↓↓↓
            this.LikeParamHeader = GetConfigParameter.GetConfigValue("LikeParamHeader");
            if (this.LikeParamHeader == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "LikeParamHeader");
            }

            this.LikeParamFooter = GetConfigParameter.GetConfigValue("LikeParamFooter");
            if (this.LikeParamFooter == null)
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "LikeParamFooter");
            }
            // ↑↑↑【追加】↑↑↑

            #endregion

            #endregion

            #endregion

            #region 置換文字列（＋空文字チェック）

            #region 共通

            this.RpFileName = GetConfigParameter.GetConfigValue("RpFileName");
            if (string.IsNullOrEmpty(this.RpFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpFileName");
            }

            this.RpTimeStamp = GetConfigParameter.GetConfigValue("RpTimeStamp");
            if (string.IsNullOrEmpty(this.RpTimeStamp))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpTimeStamp");
            }

            this.RpTimeStampColName = GetConfigParameter.GetConfigValue("RpTimeStampColName");
            if (string.IsNullOrEmpty(this.RpTimeStampColName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpColumnName");
            }

            this.RpUserName = GetConfigParameter.GetConfigValue("RpUserName");
            if (string.IsNullOrEmpty(this.RpUserName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpUserName");
            }

            #endregion

            #region SQL

            this.RpXMLEncoding = GetConfigParameter.GetConfigValue("RpXMLEncoding");
            if (string.IsNullOrEmpty(this.RpXMLEncoding))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpXMLEncoding");
            }

            this.RpTableName = GetConfigParameter.GetConfigValue("RpTableName");
            if (string.IsNullOrEmpty(this.RpTableName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpTableName");
            }

            this.RpAllColumnListSQL = GetConfigParameter.GetConfigValue("RpAllColumnListSQL");
            if (string.IsNullOrEmpty(this.RpAllColumnListSQL))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpAllColumnListSQL");
            }

            this.RpPKColumnListSQL = GetConfigParameter.GetConfigValue("RpPKColumnListSQL");
            if (string.IsNullOrEmpty(this.RpPKColumnListSQL))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpPKColumnListSQL");
            }

            // 検索条件

            this.RpColumnsCondition = GetConfigParameter.GetConfigValue("RpColumnsCondition");
            if (string.IsNullOrEmpty(this.RpColumnsCondition))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpColumnsCondition");
            }
            this.RpDynColsCondition = GetConfigParameter.GetConfigValue("RpDynColsCondition");
            if (string.IsNullOrEmpty(this.RpDynColsCondition))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynColsCondition");
            }

            this.RpDynColsCondition_Like = GetConfigParameter.GetConfigValue("RpDynColsCondition_Like");
            if (string.IsNullOrEmpty(this.RpDynColsCondition_Like))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynColsCondition_Like");
            }

            // 挿入列

            //this.RpInsertColumns = GetConfigParameter.GetConfigValue("RpInsertColumns");
            //if (string.IsNullOrEmpty(this.RpInsertColumns))
            //{
            //    throw new CheckException("app.Configパラメタが設定されていません。：RpInsertColumns");
            //}
            this.RpDynInsColumns = GetConfigParameter.GetConfigValue("RpDynInsColumns");
            if (string.IsNullOrEmpty(this.RpDynInsColumns))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynInsColumns");
            }

            // 挿入パラメタ

            this.RpInsertParameters = GetConfigParameter.GetConfigValue("RpInsertParameters");
            if (string.IsNullOrEmpty(this.RpInsertParameters))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpInsertParameters");
            }
            this.RpDynInsParameters = GetConfigParameter.GetConfigValue("RpDynInsParameters");
            if (string.IsNullOrEmpty(this.RpDynInsParameters))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynInsParameters");
            }

            // 更新パラメタ

            this.RpDynUpdParameters = GetConfigParameter.GetConfigValue("RpDynUpdParameters");
            if (string.IsNullOrEmpty(this.RpDynUpdParameters))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynUpdParameters");
            }

            #endregion

            #region Dao

            // クラス（ファイル）名
            this.RpDaoClassName = GetConfigParameter.GetConfigValue("RpDaoClassName");
            if (string.IsNullOrEmpty(this.RpDaoClassName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDaoClassName");
            }

            // カラム情報
            this.RpColumnName = GetConfigParameter.GetConfigValue("RpColumnName");
            if (string.IsNullOrEmpty(this.RpColumnName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpColumnName");
            }

            #region 制御文字列

            this.CcLoopStart_PKColumn = GetConfigParameter.GetConfigValue("CcLoopStart_PKColumn");
            if (string.IsNullOrEmpty(this.CcLoopStart_PKColumn))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopStart_PKColumn");
            }

            this.CcLoopEnd_PKColumn = GetConfigParameter.GetConfigValue("CcLoopEnd_PKColumn");
            if (string.IsNullOrEmpty(this.CcLoopEnd_PKColumn))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopEnd_PKColumn");
            }

            this.CcLoopStart_ElseColumn = GetConfigParameter.GetConfigValue("CcLoopStart_ElseColumn");
            if (string.IsNullOrEmpty(this.CcLoopStart_ElseColumn))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopStart_ElseColumn");
            }

            this.CcLoopEnd_ElseColumn = GetConfigParameter.GetConfigValue("CcLoopEnd_ElseColumn");
            if (string.IsNullOrEmpty(this.CcLoopEnd_ElseColumn))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopEnd_ElseColumn");
            }

            this.CcLoopStart_PPUpdSet = GetConfigParameter.GetConfigValue("CcLoopStart_PPUpdSet");
            if (string.IsNullOrEmpty(this.CcLoopStart_PPUpdSet))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopStart_PPUpdSet");
            }

            this.CcLoopEnd_PPUpdSet = GetConfigParameter.GetConfigValue("CcLoopEnd_PPUpdSet");
            if (string.IsNullOrEmpty(this.CcLoopEnd_PPUpdSet))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopEnd_PPUpdSet");
            }

            // ↓↓↓【追加】↓↓↓
            this.CcLoopStart_PPLike = GetConfigParameter.GetConfigValue("CcLoopStart_PPLike");
            if (string.IsNullOrEmpty(this.CcLoopStart_PPLike))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopStart_PPLike");
            }

            this.CcLoopEnd_PPLike = GetConfigParameter.GetConfigValue("CcLoopEnd_PPLike");
            if (string.IsNullOrEmpty(this.CcLoopEnd_PPLike))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopEnd_PPLike");
            }
            // ↑↑↑【追加】↑↑↑

            // To Get TimeStamp placeholder from config file 
            this.CcIsRequired_TimeStamp = GetConfigParameter.GetConfigValue("CcIsRequired_TimeStamp");
            if (string.IsNullOrEmpty(this.CcIsRequired_TimeStamp))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcIsRequired_TimeStamp");
            }

            #endregion

            #region メソッド名

            this.RpInsertMethodName = GetConfigParameter.GetConfigValue("RpInsertMethodName");
            if (string.IsNullOrEmpty(this.RpInsertMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpInsertMethodName");
            }
            this.RpDynInsMethodName = GetConfigParameter.GetConfigValue("RpDynInsMethodName");
            if (string.IsNullOrEmpty(this.RpDynInsMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynInsMethodName");
            }

            this.RpSelectMethodName = GetConfigParameter.GetConfigValue("RpSelectMethodName");
            if (string.IsNullOrEmpty(this.RpSelectMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpSelectMethodName");
            }
            this.RpDynSelMethodName = GetConfigParameter.GetConfigValue("RpDynSelMethodName");
            if (string.IsNullOrEmpty(this.RpDynSelMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynSelMethodName");
            }

            this.RpUpdateMethodName = GetConfigParameter.GetConfigValue("RpUpdateMethodName");
            if (string.IsNullOrEmpty(this.RpUpdateMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpUpdateMethodName");
            }
            this.RpDynUpdMethodName = GetConfigParameter.GetConfigValue("RpDynUpdMethodName");
            if (string.IsNullOrEmpty(this.RpDynUpdMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynUpdMethodName");
            }

            this.RpDeleteMethodName = GetConfigParameter.GetConfigValue("RpDeleteMethodName");
            if (string.IsNullOrEmpty(this.RpDeleteMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDeleteMethodName");
            }
            this.RpDynDelMethodName = GetConfigParameter.GetConfigValue("RpDynDelMethodName");
            if (string.IsNullOrEmpty(this.RpDynDelMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynDelMethodName");
            }
            // ↓↓↓【追加】↓↓↓
            this.RpDynSelCntMethodName = GetConfigParameter.GetConfigValue("RpDynSelCntMethodName");
            if (string.IsNullOrEmpty(this.RpDynSelCntMethodName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynSelCntMethodName");
            }
            // ↑↑↑【追加】↑↑↑
            #endregion

            #region ファイル名

            this.RpInsertFileName = GetConfigParameter.GetConfigValue("RpInsertFileName");
            if (string.IsNullOrEmpty(this.RpInsertFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpInsertFileName");
            }
            this.RpDynInsFileName = GetConfigParameter.GetConfigValue("RpDynInsFileName");
            if (string.IsNullOrEmpty(this.RpDynInsFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynInsFileName");
            }

            this.RpSelectFileName = GetConfigParameter.GetConfigValue("RpSelectFileName");
            if (string.IsNullOrEmpty(this.RpSelectFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpSelectFileName");
            }
            this.RpDynSelFileName = GetConfigParameter.GetConfigValue("RpDynSelFileName");
            if (string.IsNullOrEmpty(this.RpDynSelFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynSelFileName");
            }

            this.RpUpdateFileName = GetConfigParameter.GetConfigValue("RpUpdateFileName");
            if (string.IsNullOrEmpty(this.RpUpdateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpUpdateFileName");
            }
            this.RpDynUpdFileName = GetConfigParameter.GetConfigValue("RpDynUpdFileName");
            if (string.IsNullOrEmpty(this.RpDynUpdFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynUpdFileName");
            }

            this.RpDeleteFileName = GetConfigParameter.GetConfigValue("RpDeleteFileName");
            if (string.IsNullOrEmpty(this.RpDeleteFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDeleteFileName");
            }
            this.RpDynDelFileName = GetConfigParameter.GetConfigValue("RpDynDelFileName");
            if (string.IsNullOrEmpty(this.RpDynDelFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynDelFileName");
            }
            // ↓↓↓【追加】↓↓↓
            this.RpDynSelCntFileName = GetConfigParameter.GetConfigValue("RpDynSelCntFileName");
            if (string.IsNullOrEmpty(this.RpDynSelCntFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDynSelCntFileName");
            }
            // ↑↑↑【追加】↑↑↑

            #endregion

            #endregion

            // ↓↓↓【追加】↓↓↓
            #region DTO

            #region Entity

            // Entityクラス（ファイル）名
            this.RpEntityClassName = GetConfigParameter.GetConfigValue("RpEntityClassName");
            if (string.IsNullOrEmpty(RpEntityClassName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpEntityClassName");
            }

            // Entityのフィールドの型情報
            this.RpEntityTypeInfo = GetConfigParameter.GetConfigValue("RpEntityTypeInfo");
            if (string.IsNullOrEmpty(RpEntityTypeInfo))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpEntityTypeInfo");
            }

            #endregion

            #region DataSet

            // DataSetクラス（ファイル）名
            this.RpDataSetClassName = GetConfigParameter.GetConfigValue("RpDataSetClassName");
            if (string.IsNullOrEmpty(RpDataSetClassName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDataSetClassName");
            }

            // DataTableクラス名
            this.RpDataTableClassName = GetConfigParameter.GetConfigValue("RpDataTableClassName");
            if (string.IsNullOrEmpty(RpDataTableClassName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDataTableClassName");
            }

            // XSDのエレメントの型情報
            this.RpXSDTypeInfo = GetConfigParameter.GetConfigValue("RpXSDTypeInfo");
            if (string.IsNullOrEmpty(RpXSDTypeInfo))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpXSDTypeInfo");
            }

            #endregion

            #endregion

            #region メンテナンス画面

            this.RpAllColumnList = GetConfigParameter.GetConfigValue("RpAllColumnList");
            if (string.IsNullOrEmpty(this.RpAllColumnList))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpAllColumnList");
            }

            this.RpPKColumnList = GetConfigParameter.GetConfigValue("RpPKColumnList");
            if (string.IsNullOrEmpty(this.RpPKColumnList))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpPKColumnList");
            }

            this.RpAllColumnListTableAdapterSQL = GetConfigParameter.GetConfigValue("RpAllColumnListTableAdapterSQL");
            if (string.IsNullOrEmpty(this.RpAllColumnListTableAdapterSQL))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpAllColumnListTableAdapterSQL");
            }

            this.RpCodebehindLanguage = GetConfigParameter.GetConfigValue("RpCodebehindLanguage");
            if (string.IsNullOrEmpty(RpCodebehindLanguage))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpCodebehindLanguage");
            }

            this.RpClassTemplateFileExtension = GetConfigParameter.GetConfigValue("RpClassTemplateFileExtension");
            if (string.IsNullOrEmpty(RpClassTemplateFileExtension))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpClassTemplateFileExtension");
            }

            this.RpCommentOut = GetConfigParameter.GetConfigValue("RpCommentOut");
            if (string.IsNullOrEmpty(RpCommentOut))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpCommentOut");
            }

            this.RpPKFirstColumn = GetConfigParameter.GetConfigValue("RpPKFirstColumn");
            if (string.IsNullOrEmpty(RpPKFirstColumn))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpPKFirstColumn");
            }

            this.RpColumnNmbr = GetConfigParameter.GetConfigValue("RpColumnNmbr");
            if (string.IsNullOrEmpty(RpColumnNmbr))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpColumnNmbr");
            }

            this.RpDBMS = GetConfigParameter.GetConfigValue("RpDBMS");
            if (string.IsNullOrEmpty(RpDBMS))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDBMS");
            }

            this.RpDAP = GetConfigParameter.GetConfigValue("RpDAP");
            if (string.IsNullOrEmpty(RpDBMS))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpDAP");
            }

            #endregion
            // ↑↑↑【追加】↑↑↑

            #endregion
        }

        # endregion

        // メニュー周り
        #region メニューの処理

        /// <summary>閉じる</summary>
        private void Close_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 活性・非活性制御

        /// <summary>「エスケープ文字」活性・非活性制御</summary>
        bool GbxOracleLikeSettingEnabled;

        /// <summary>「エスケープ文字」活性・非活性制御</summary>
        private void rbnODP_CheckedChanged(object sender, EventArgs e)
        {
            this.gbxOracleLikeSetting.Enabled = this.rbnODP.Checked;
        }

        /// <summary>DTOのみ設定のオン・オフの切替</summary>
        private void cbxOnlyDTO_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbxOnlyDTO.Checked)
            {
                // DTOのみの生成の場合
                this.gbxDataProviders.Enabled = false;

                // バックアップしてfalse
                this.GbxOracleLikeSettingEnabled = this.gbxOracleLikeSetting.Enabled;
                this.gbxOracleLikeSetting.Enabled = false;

                this.cbxTableMaintenance.Enabled = false;
                this.cbxOnlyTableMaintenance.Enabled = false;

                this.txtTimeStampColName.Enabled = false;
                this.txtTimeStampUpdMethod.Enabled = false;
            }
            else
            {
                // Daoを含めた生成の場合
                this.gbxDataProviders.Enabled = true;

                // バックアップから復元
                this.gbxOracleLikeSetting.Enabled = this.GbxOracleLikeSettingEnabled;

                this.cbxTableMaintenance.Enabled = true;
                this.cbxOnlyTableMaintenance.Enabled = true;

                this.txtTimeStampColName.Enabled = true;
                this.txtTimeStampUpdMethod.Enabled = true;
            }
        }

        /// <summary>メンテナンス画面のみ設定のオン・オフの切替</summary>
        private void cbxOnlyTableMaintenance_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbxOnlyTableMaintenance.Checked)
            {
                // DTOのみの生成の場合
                this.gbxDataProviders.Enabled = false;

                // バックアップしてfalse
                this.GbxOracleLikeSettingEnabled = this.gbxOracleLikeSetting.Enabled;
                this.gbxOracleLikeSetting.Enabled = false;

                this.gbxDTO.Enabled = false;

                this.txtTimeStampColName.Enabled = false;
                this.txtTimeStampUpdMethod.Enabled = false;
            }
            else
            {
                // Daoを含めた生成の場合
                this.gbxDataProviders.Enabled = true;

                // バックアップから復元
                this.gbxOracleLikeSetting.Enabled = this.GbxOracleLikeSettingEnabled;

                this.gbxDTO.Enabled = true;

                this.txtTimeStampColName.Enabled = true;
                this.txtTimeStampUpdMethod.Enabled = true;
            }
        }

        #endregion

        // ファイル ダイアログ周り
        #region ファイルの入出力先を指定

        /// <summary>D層定義情報ファイルへのパスを指定する。</summary>
        private void btnSetDaoDefinition_Click(object sender, EventArgs e)
        {
            // ロード先の設定
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "CSVファイル(*.csv)|*.csv";
            ofd.Filter = this.RM_GetString("OpenFileDialogFilter");
            //ofd.Title = "D層定義情報ファイル";
            ofd.Title = this.RM_GetString("OpenFileDialogTitle");

            DialogResult dRet = ofd.ShowDialog();

            if (dRet == DialogResult.OK)
            {
                // OKで
                if (ofd.FileName == "")
                {
                    // ファイルが指定されていない。
                }
                else
                {
                    // ファイルが指定されている。
                    this.txtSetDaoDefinition.Text = ofd.FileName;
                }
            }
        }

        /// <summary>入力ファイル（テンプレート ファイル）のルート フォルダ パスを指定</summary>
        private void btnSetSourceTemplate_Click(object sender, EventArgs e)
        {
            // ロード先の設定
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "入力ファイル（テンプレート ファイル）のルート フォルダ パスを指定";
            fbd.Description = this.RM_GetString("TemplateFolderPath");
            fbd.SelectedPath = this.txtSetSourceTemplate.Text;
            DialogResult dRet = fbd.ShowDialog();

            if (dRet == DialogResult.OK)
            {
                // OKで
                if (fbd.SelectedPath == "")
                {
                    // ファイルが指定されていない。
                }
                else
                {
                    // ファイルが指定されている。
                    this.txtSetSourceTemplate.Text = fbd.SelectedPath;
                }
            }
        }

        /// <summary>出力ファイル（Daoファイル、SQLファイル）のルート フォルダ パスを指定</summary>
        private void btnSetOutput_Click(object sender, EventArgs e)
        {
            // セーブ先の設定
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "出力ファイル（Daoファイル、SQLファイル）のルート フォルダ パスを指定";
            fbd.Description = this.RM_GetString("OutputFolderPath");
            fbd.SelectedPath = this.txtSetOutput.Text;
            DialogResult dRet = fbd.ShowDialog();

            if (dRet == DialogResult.OK)
            {
                // OKで
                if (fbd.SelectedPath == "")
                {
                    // ファイルが指定されていない。
                }
                else
                {
                    // ファイルが指定されている。
                    this.txtSetOutput.Text = fbd.SelectedPath;
                }
            }
        }

        #endregion

        #endregion

        // メイン ロジック
        #region 生成処理

        /// <summary>D層、Daoクラスファイル、SQLファイルを生成する。</summary>
        private void btnDaoAndSqlGen_Click(object sender, EventArgs e)
        {
            // D層定義情報ファイル
            StreamReader srDaoDef = null;

            // ↓↓↓【追加】↓↓↓
            // .NET型定義情報ファイル
            StreamReader srTypeDef = null;

            // DTO生成フラグの初期化
            this.CreateDTO = (this.cbxEntity.Checked || this.cbxTypedDataSet.Checked);

            // メンテナンス画面生成フラグの初期化
            this.CreateMaintenanceScreen = (this.cbxOnlyTableMaintenance.Checked || this.cbxTableMaintenance.Checked);

            // ↑↑↑【追加】↑↑↑

            // ↓↓↓【追加】↓↓↓
            // DB型定義情報ファイル
            StreamReader srDbTypeDef = null;

            // DB型定義情報ファイル読み込みフラグ設定
            this.ReadDbTypeInfo = (this.ReadDbTypeInfo || rbnODP.Checked);
            // ↑↑↑【追加】↑↑↑

            try
            {
                // カーソルを待機状態にする
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                #region 拡張子の設定

                if (this.rbnDTL_CS.Checked)
                {
                    // Visual C#
                    this.ClassTemplateFileExtension = "cs";
                    this.CodebehindLanguage = "C#";
                    this.CommentOut = "//";
                }
                else
                {
                    // Visual Basic
                    this.ClassTemplateFileExtension = "vb";
                    this.CodebehindLanguage = "VB";
                    this.CommentOut = "'";
                }

                #endregion

                #region チェック処理

                FileInfo fileInfo = null;
                DirectoryInfo dirInfo = null;

                #region D層定義情報ファイル

                // 標準生成
                fileInfo = new FileInfo(this.txtSetDaoDefinition.Text);
                if (fileInfo.Exists) { }
                else
                {
                    //MessageBox.Show(string.Format("チェックエラーです：D層定義情報ファイル[{0}]が存在しません。", fileInfo.Name));
                    MessageBox.Show(string.Format(this.RM_GetString("FilenotExistDlayerInfo"), fileInfo.Name));
                    return;
                }

                // ↓↓↓【追加】↓↓↓
                // ＋エンティテイ生成＋型付きデータセット生成
                if (this.CreateDTO)
                {
                    // *.CSVである前提の仕様だがOKか？（一応ファイル名を出力する）
                    int temp = this.txtSetDaoDefinition.Text.Length - 4;

                    this.DotNetTypeInfoFilePath =
                        this.txtSetDaoDefinition.Text.Substring(0, temp)
                        + "_DotNetTypeInfo" + this.txtSetDaoDefinition.Text.Substring(temp);

                    fileInfo = new FileInfo(this.DotNetTypeInfoFilePath);
                    if (fileInfo.Exists) { }
                    else
                    {
                        //MessageBox.Show(string.Format("チェックエラーです：.NET型情報ファイル[{0}]が存在しません。", fileInfo.Name));
                        MessageBox.Show(string.Format(this.RM_GetString("FilenotExistNETtypeInfo"), fileInfo.Name));
                        return;
                    }
                }
                // ↑↑↑【追加】↑↑↑

                // ↓↓↓【追加】↓↓↓
                // DB型情報ファイル読み込み
                if (this.ReadDbTypeInfo)
                {
                    // *.CSVである前提の仕様だがOKか？（一応ファイル名を出力する）
                    int temp = this.txtSetDaoDefinition.Text.Length - 4;

                    this.DbTypeInfoFilePath = this.txtSetDaoDefinition.Text.Substring(0, temp)
                        + "_DBTypeInfo" + this.txtSetDaoDefinition.Text.Substring(temp);

                    fileInfo = new FileInfo(this.DbTypeInfoFilePath);
                    if (fileInfo.Exists) { }
                    else
                    {
                        //MessageBox.Show(string.Format("チェックエラーです：DB型情報ファイル[{0}]が存在しません。", fileInfo.Name));
                        MessageBox.Show(string.Format(this.RM_GetString("FilenotExistDBtypeInfo"), fileInfo.Name));
                        return;
                    }
                }
                // ↑↑↑【追加】↑↑↑

                #endregion

                #region 入力ファイル

                dirInfo = new DirectoryInfo(this.txtSetSourceTemplate.Text);
                if (dirInfo.Exists)
                {
                }
                else
                {
                    //MessageBox.Show("入力ファイル（テンプレート ファイル）のルート フォルダが存在しません。");
                    MessageBox.Show(this.RM_GetString("InputFileRootFolderNotExist"));
                    return;
                }

                #region Dao、Entity、DataSetテンプレート ファイル

                // Daoテンプレート ファイル
                if (!this.cbxOnlyDTO.Checked
                    && !this.cbxOnlyTableMaintenance.Checked)
                {
                    this.DaoTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DaoTemplateFileName,
                            this.ClassTemplateFileExtension);
                }

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    // Entityテンプレート ファイル
                    this.EntityTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.EntityTemplateFileName,
                            this.ClassTemplateFileExtension);

                    // DataSetテンプレート ファイル
                    this.DataSetTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DataSetTemplateFileName, "");
                }

                if (this.CreateMaintenanceScreen)
                {
                    // TableAdapterテンプレート ファイル
                    this.TableAdapterTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.TableAdapterTemplateFileName,
                            this.ClassTemplateFileExtension);

                    // ConditionalSearchテンプレート ファイル
                    this.ConditionalSearchTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.ConditionalSearchTemplateFileName, "");

                    // SearchAndUpdateテンプレート ファイル
                    this.SearchAndUpdateTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.SearchAndUpdateTemplateFileName, "");

                    // Detailテンプレート ファイル
                    this.DetailTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DetailTemplateFileName, "");
                }
                // ↑↑↑【追加】↑↑↑

                #endregion

                if (!this.cbxOnlyDTO.Checked
                    && !this.cbxOnlyTableMaintenance.Checked)
                {
                    #region SQLテンプレート ファイル（静的）

                    this.InsertTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.InsertTemplateFileName, "");

                    this.SelectTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.SelectTemplateFileName, "");

                    this.UpdateTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.UpdateTemplateFileName, "");

                    this.DeleteTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DeleteTemplateFileName, "");

                    #endregion

                    #region SQLテンプレート ファイル（動的）

                    this.DynInsTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DynInsTemplateFileName, "");

                    this.DynSelTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DynSelTemplateFileName, "");

                    this.DynUpdTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DynUpdTemplateFileName, "");

                    this.DynDelTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DynDelTemplateFileName, "");

                    // ↓↓↓【追加】↓↓↓
                    this.DynSelCntTemplateFilePath
                        = this.CheckTemplateFile(
                            this.txtSetSourceTemplate.Text,
                            this.DynSelCntTemplateFileName, "");
                    // ↑↑↑【追加】↑↑↑

                    #endregion
                }

                #endregion

                #region 出力ファイル

                dirInfo = new DirectoryInfo(this.txtSetOutput.Text);
                if (dirInfo.Exists) { }
                else
                {
                    //MessageBox.Show("出力ファイル（Daoクラス ファイル、SQLファイル.etc）のルート フォルダが存在しません。");
                    MessageBox.Show(this.RM_GetString("OutputFileRootFolderNotExist"));
                    return;
                }

                #endregion

                #region Like検索用エスケープ文字

                // ↓↓↓【追加】↓↓↓

                //エスケープ文字のチェック
                if (rbnODP.Checked == true && txtEscapeChar.Text.Length != 1)
                {
                    //Oracle ODP.NET かつ エスケープ文字が入力されていない(1文字でない)場合
                    //MessageBox.Show("エスケープ文字が設定されていません。");
                    MessageBox.Show(this.RM_GetString("EscCharacterNotSet"));
                    return;
                }

                // ↑↑↑【追加】↑↑↑

                #endregion

                #region タイムスタンプ列

                // タイムスタンプ列名を取得
                this.TimeStampColName = this.txtTimeStampColName.Text.Trim();

                // タイムスタンプ更新方法を取得
                this.TimeStampUpdMethod = this.txtTimeStampUpdMethod.Text.Trim();

                // タイムスタンプの設定ステータスのチェック＆設定
                if (this.TimeStampColName == "" & this.TimeStampUpdMethod == "")
                {
                    // なにもない。
                    this.TimeStampStatus = TSS.Non;
                }
                else if (this.TimeStampColName != "" & this.TimeStampUpdMethod == "")
                {
                    // 名前だけある。
                    this.TimeStampStatus = TSS.Name;
                }
                else if (this.TimeStampColName != "" & this.TimeStampUpdMethod != "")
                {
                    // 名前と更新方法の両方がある。
                    this.TimeStampStatus = TSS.NandM;
                }
                else
                {
                    // 更新方法だけある。
                    //MessageBox.Show("タイムスタンプ列名がありません。");
                    MessageBox.Show(this.RM_GetString("NoTimestampColName"));

                    return;
                }

                #endregion

                #endregion

                #region 生成処理（将来的に関数化しても良い）

                #region 固定情報の設定

                // 日付
                this.TimeStamp = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;

                // ユーザ名
                this.UserName = this.txtFamilyName.Text + " " + this.txtPersonalName.Text;

                //// エンコーディング（XML用）
                this.XMLEncoding = this.cmbSFEncoding.Text;

                // SQLの制御文字
                if (rbnSQL.Checked || rbnOLE.Checked || rbnODB.Checked)
                {
                    this.ParamSign = '@';

                    this.EnclosureCharS = '[';
                    this.EnclosureCharE = ']';
                }
                else if (rbnODP.Checked)
                {
                    this.ParamSign = ':';

                    this.EnclosureCharS = '"';
                    this.EnclosureCharE = '"';
                }
                else if (rbnDB2.Checked)
                {
                    this.ParamSign = '@';

                    this.EnclosureCharS = '"';
                    this.EnclosureCharE = '"';
                }
                else if (rbnMySQL.Checked)
                {
                    this.ParamSign = '@';

                    this.EnclosureCharS = '`';
                    this.EnclosureCharE = '`';
                }
                else if (rbnPstgrs.Checked)
                {
                    // Npgsqlでは、「@」・「:」の双方をサポートするが、
                    // 本Fxでは、「:」を選択（動作がOracleに近いため）。
                    this.ParamSign = '@';

                    // PostgreSQLでは、表名を囲えないので注意。
                    this.EnclosureCharS = '"';
                    this.EnclosureCharE = '"';
                }
                else
                {
                    // 他（ありえない）
                }

                #endregion

                #region D層定義情報ファイル読取変数

                // D層定義情報ファイル・.NET型定義情報ファイルを開く
                this.OpenSrDefInfo(out srDaoDef, out srTypeDef, out srDbTypeDef);

                // D層定義情報ファイルから読んだ１行
                string rlDaoDef = "";
                string[] rlDaoDef2 = null;

                // ↓↓↓【追加】↓↓↓
                // .NET型定義情報ファイルから読んだ１行
                string rlTypeDef = "";
                string[] rlTypeDef2 = null;
                // ↑↑↑【追加】↑↑↑

                // ↓↓↓【追加】↓↓↓
                // DB型定義情報ファイルから読んだ１行
                string rlDbTypeDef = "";
                string[] rlDbTypeDef2 = null;
                // ↑↑↑【追加】↑↑↑

                #endregion

                #region D層定義情報ファイル ヘッダー制御

                if (this.cbxDaoDefinitionHeader.Checked)
                {
                    // ヘッダーは、無視
                    if (!srDaoDef.EndOfStream)
                    {
                        rlDaoDef = srDaoDef.ReadLine();
                    }
                    // ↓↓↓【追加】↓↓↓
                    if (this.CreateDTO)
                    {
                        if (!srTypeDef.EndOfStream)
                        {
                            rlTypeDef = srTypeDef.ReadLine();
                        }
                    }
                    // ↑↑↑【追加】↑↑↑

                    // ↓↓↓【追加】↓↓↓
                    if (this.ReadDbTypeInfo)
                    {
                        if (!srDbTypeDef.EndOfStream)
                        {
                            rlDbTypeDef = srDbTypeDef.ReadLine();
                        }
                    }
                    // ↑↑↑【追加】↑↑↑
                }
                else
                {
                    // ヘッダーは、無し
                }

                #endregion

                #region データがなくなるまでループ

                while (!srDaoDef.EndOfStream)
                {
                    // 消しました（２行目でやってないので）。
                    //// null・空文字チェック
                    ////（.NET型定義情報はD層定義情報に合わせる
                    //if (string.IsNullOrEmpty(rlDaoDef))
                    //{
                    //    // スキップ
                    //    continue;
                    //}

                    // １行読込
                    rlDaoDef = srDaoDef.ReadLine();
                    rlDaoDef2 = rlDaoDef.Split(',');
                    // ↓↓↓【追加】↓↓↓
                    if (this.CreateDTO)
                    {
                        rlTypeDef = srTypeDef.ReadLine();
                        rlTypeDef2 = rlTypeDef.Split(',');
                    }
                    // ↑↑↑【追加】↑↑↑
                    // ↓↓↓【追加】↓↓↓
                    if (this.ReadDbTypeInfo)
                    {
                        rlDbTypeDef = srDbTypeDef.ReadLine();
                        rlDbTypeDef2 = rlDbTypeDef.Split(',');
                    }
                    // ↑↑↑【追加】↑↑↑

                    if (rlDaoDef2[0] != "")
                    {
                        // テーブル名の退避
                        this.TableName = rlDaoDef2[0].Trim();

                        #region 生成準備

                        #region 主キー列の情報の収集

                        //  初期化
                        this.PK_Columns = new ArrayList();
                        this.PK_ColumnsType = new ArrayList();
                        this.PK_ColumnsDbType = new ArrayList();

                        // 長さが２以上の場合、主キー列を取りに行く。
                        if (rlDaoDef2.Length >= 2)
                        {
                            for (int i = 1; i < rlDaoDef2.Length; i++)
                            {
                                if (rlDaoDef2[i] == "") { break; }
                                this.PK_Columns.Add(rlDaoDef2[i].Trim());
                            }
                        }
                        // ↓↓↓【追加】↓↓↓
                        if (this.CreateDTO)
                        {
                            if (rlTypeDef2.Length >= 2)
                            {
                                for (int i = 1; i < rlTypeDef2.Length; i++)
                                {
                                    if (rlTypeDef2[i] == "") { break; }
                                    this.PK_ColumnsType.Add(rlTypeDef2[i].Trim());
                                }
                            }
                        }
                        // ↑↑↑【追加】↑↑↑

                        // ↓↓↓【追加】↓↓↓
                        if (this.ReadDbTypeInfo)
                        {
                            if (rlDbTypeDef2.Length >= 2)
                            {
                                for (int i = 1; i < rlDbTypeDef2.Length; i++)
                                {
                                    if (rlDbTypeDef2[i] == "") { break; }
                                    this.PK_ColumnsDbType.Add(rlDbTypeDef2[i].Trim());
                                }
                            }
                        }
                        // ↑↑↑【追加】↑↑↑

                        // チェック処理（主キー列が無い）
                        if (PK_Columns.Count == 0)
                        {
                            throw new CheckException(
                                //this.TableName + "テーブルに主キーが定義されていません。");
                                this.TableName + this.RM_GetString("PrimaryKeyNotDefined"));
                        }

                        #endregion

                        // ２行目（その他の列情報）を読む。
                        rlDaoDef = srDaoDef.ReadLine();
                        rlDaoDef2 = rlDaoDef.Split(',');

                        // ↓↓↓【追加】↓↓↓
                        if (this.CreateDTO)
                        {
                            rlTypeDef = srTypeDef.ReadLine();
                            rlTypeDef2 = rlTypeDef.Split(',');
                        }
                        // ↑↑↑【追加】↑↑↑

                        // ↓↓↓【追加】↓↓↓
                        if (this.ReadDbTypeInfo)
                        {
                            rlDbTypeDef = srDbTypeDef.ReadLine();
                            rlDbTypeDef2 = rlDbTypeDef.Split(',');
                        }
                        // ↑↑↑【追加】↑↑↑

                        #region 主キー以外の列情報の収集

                        //  初期化
                        this.ELSE_Columns = new ArrayList();
                        this.ELSE_ColumnsType = new ArrayList();
                        this.ELSE_ColumnsDbType = new ArrayList();

                        // チェック処理（テーブル名に値が入っている）
                        if (rlDaoDef2[0].Trim() != "")
                        {
                            throw new CheckException(
                                //"不正なフォーマットです。"
                                this.RM_GetString("InvalidFormat")
                                + this.TableName + "：" + rlDaoDef);
                        }

                        // 長さが２以上の場合、主キー以外の列情報を取りに行く。
                        if (rlDaoDef2.Length >= 2)
                        {
                            for (int i = 1; i < rlDaoDef2.Length; i++)
                            {
                                if (rlDaoDef2[i] == "") { break; }
                                this.ELSE_Columns.Add(rlDaoDef2[i].Trim());
                            }
                        }
                        // ↓↓↓【追加】↓↓↓
                        if (this.CreateDTO)
                        {
                            if (rlTypeDef2.Length >= 2)
                            {
                                for (int i = 1; i < rlTypeDef2.Length; i++)
                                {
                                    if (rlTypeDef2[i] == "") { break; }
                                    this.ELSE_ColumnsType.Add(rlTypeDef2[i].Trim());
                                }
                            }
                        }
                        // ↑↑↑【追加】↑↑↑

                        // ↓↓↓【追加】↓↓↓
                        if (this.ReadDbTypeInfo)
                        {
                            if (rlDbTypeDef2.Length >= 2)
                            {
                                for (int i = 1; i < rlDbTypeDef2.Length; i++)
                                {
                                    if (rlDbTypeDef2[i] == "") { break; }
                                    this.ELSE_ColumnsDbType.Add(rlDbTypeDef2[i].Trim());
                                }
                            }
                        }
                        // ↑↑↑【追加】↑↑↑

                        #endregion

                        #endregion

                        #region コメント

                        #region 共通

                        #region（１）共通 静的カラム リスト

                        // ● タイムスタンプ状態（Non、Name、NandM）
                        //    例：[aaa],
                        //        [bbb],
                        //        [ccc],
                        //        [ddd],
                        //        [eee] ～

                        // ※ カンマ区切りにして後で、
                        //    [,] → [,] + [\r\n] + [インデント]と置換する。

                        #endregion

                        #region（２）共通 静的 検索条件 ★

                        // ● タイムスタンプ状態（Non）
                        //    例：[aaa] = @aaa,      [aaa] = @aaa
                        //        [bbb] = @bbb,      AND [bbb] = @bbb
                        //        [ccc] = @ccc,  →  AND [ccc] = @ccc
                        //        [ddd] = @ddd,      AND [ddd] = @ddd
                        //        [eee] = @eee ～    AND [eee] = @eee ～

                        // ※ 主キーの列のみ対象とするので、IS NULLは不要

                        // ※ カンマ区切りにして後で、
                        //    [,] → [\r\n] + [インデント] + [AND ]と置換する。

                        // ↓ //

                        // ● タイムスタンプ状態（Name、NandM）
                        //    例：[aaa] = @aaa
                        //        AND [bbb] = @bbb
                        //        AND [ccc] = @ccc
                        //        AND [ddd] = @ddd
                        //    ★  <IF>AND [eee] = @eee</IF> ～

                        // ※ タイムスタンプ列が有る場合は、それも対象にする。

                        // ※ タイムスタンプ状態によっては、
                        //    「AND [eee] = @eee」 → 「<IF>AND [eee] = @eee</IF>」と置換する。

                        #endregion

                        #region（３）共通 動的 検索条件

                        // 例：<IF>AND [aaa] = @aaa<ELSE>[aaa] IS NILL</ELSE></IF>,
                        //     <IF>AND [bbb] = @bbb<ELSE>[bbb] IS NILL</ELSE></IF>,
                        //     <IF>AND [ccc] = @ccc<ELSE>[ccc] IS NILL</ELSE></IF>,
                        //     <IF>AND [ddd] = @ddd<ELSE>[ddd] IS NILL</ELSE></IF>,
                        //     <IF>AND [eee] = @eee<ELSE>[eee] IS NILL</ELSE></IF> ～

                        // ※ カンマ区切りにして後で、
                        //    [>,] → [>] + [\r\n] + [インデント]と置換する。

                        #endregion

                        #region（４）共通 動的SET句リスト ★

                        // ● タイムスタンプ状態（Non、Name）
                        //    例：<IF>[aaa] = @Set_aaa_Upd,</IF>,
                        //        <IF>[bbb] = @Set_bbb_Upd,</IF>,
                        //        <IF>[ccc] = @Set_ccc_Upd,</IF>,
                        //        <IF>[ddd] = @Set_ddd_Upd,</IF>,
                        //        <IF>[eee] = @Set_eee_Upd,</IF> ～

                        // ※ カンマ区切りにして後で、
                        //    [>,] → [>] + [\r\n] + [インデント]と置換する。

                        // ↓

                        // ● タイムスタンプ状態（NandM）
                        //    例：<IF>[aaa] = @Set_aaa_Upd,</IF>,
                        //        <IF>[bbb] = @Set_bbb_Upd,</IF>,
                        //        <IF>[ccc] = @Set_ccc_Upd,</IF>,
                        //        <IF>[ddd] = @Set_ddd_Upd,</IF>,
                        //        [eee] = SYSTIMESTAMP(), ～

                        // ※ タイムスタンプ状態によっては、
                        //    「<IF>[eee] = @Set_eee_Upd,</IF>」 → 「[eee] = SYSTIMESTAMP(),」と置換する。

                        #endregion

                        #endregion

                        #region 個別

                        #region（５）Insert用パラメタ リスト ★

                        // ● タイムスタンプ状態（Non、Name）
                        //    例：@aaa,
                        //        @bbb,
                        //        @ccc,
                        //        @ddd,
                        //        @eee ～

                        // ※ カンマ区切りにして後で、
                        //    [,] → [,] + [\r\n] + [インデント]と置換する。

                        // ↓

                        // ● タイムスタンプ状態（NandM）
                        //    例：@aaa,
                        //        @bbb,
                        //        @ccc,
                        //        @ddd,
                        //        SYSTIMESTAMP() ～

                        // ※ タイムスタンプ状態によっては、
                        //    「@eee」 → 「SYSTIMESTAMP()」と置換する。

                        #endregion

                        #region（６）DynIns用カラム リスト ★

                        // ● タイムスタンプ状態（Non、Name）
                        //    例：<INSCOL name="aaa">[aaa],</INSCOL>,
                        //        <INSCOL name="bbb">[bbb],</INSCOL>,
                        //        <INSCOL name="ccc">[ccc],</INSCOL>,
                        //        <INSCOL name="ddd">[ddd],</INSCOL>,
                        //        <INSCOL name="eee">[eee],</INSCOL> ～

                        // ※ カンマ区切りにして後で、
                        //    [>,] → [>] + [\r\n] + [インデント]と置換する。

                        // ↓

                        // ● タイムスタンプ状態（NandM）
                        //    例：<INSCOL name="aaa">[aaa],</INSCOL>,
                        //        <INSCOL name="bbb">[bbb],</INSCOL>,
                        //        <INSCOL name="ccc">[ccc],</INSCOL>,
                        //        <INSCOL name="ddd">[ddd],</INSCOL>,
                        //        [eee], ～

                        // ※ タイムスタンプ状態によっては、
                        //    「<INSCOL name="eee">[eee],</INSCOL>」 → 「[eee],」と置換する。

                        #endregion

                        #region（７）DynIns用パラメタ リスト ★


                        // ● タイムスタンプ状態（Non、Name）
                        //    例：<IF>@aaa,</IF>,
                        //        <IF>@bbb,</IF>,
                        //        <IF>@ccc,</IF>,
                        //        <IF>@ddd,</IF>,
                        //        <IF>@eee,</IF> ～

                        // ※ カンマ区切りにして後で、
                        //    [>,] → [>] + [\r\n] + [インデント]と置換する。

                        // ↓

                        // ● タイムスタンプ状態（NandM）
                        //    例：<IF>@aaa,</IF>,
                        //        <IF>@bbb,</IF>,
                        //        <IF>@ccc,</IF>,
                        //        <IF>@ddd,</IF>,
                        //        SYSTIMESTAMP(), ～

                        // ※ タイムスタンプ状態によっては、
                        //    「<IF>@eee,</IF>」 → 「SYSTIMESTAMP(),」と置換する。

                        #endregion

                        #endregion

                        #endregion

                        #region  初期化

                        //（１）共通 静的カラム リスト
                        this.AllColumnListSQL = "";
                        this.PKColumnListSQL = "";

                        //（２）共通 静的 検索条件 ★
                        this.ColumnsCondition = "";

                        //（３）共通 動的 検索条件
                        this.DynColsCondition = "";

                        // ↓↓↓【追加】↓↓↓
                        //（３．５）共通 動的 曖昧検索条件
                        this.DynColsCondition_Like = "";
                        // ↑↑↑【追加】↑↑↑

                        //（４）共通 動的更新SET句リスト ★
                        this.DynUpdParameters = "";

                        //（５）Insert用パラメタ リスト ★
                        this.InsertParameters = "";

                        //（６）DynIns用カラム リスト ★
                        this.DynInsColumns = "";

                        //（７）DynIns用パラメタ リスト ★
                        this.DynInsParameters = "";

                        // その他、ColumnList
                        this.AllColumnList = "";
                        this.PKColumnList = "";
                        this.PKFirstColumn = "";
                        this.AllColumnListTableAdapterSQL = "";

                        #endregion

                        #region 生成処理

                        // 主キー列
                        for (int index = 0; index < this.PK_Columns.Count; index++)
                        {
                            string col = (string)PK_Columns[index];
                            string dbType = "";
                            if (this.ReadDbTypeInfo == true)
                            {
                                dbType = (string)PK_ColumnsDbType[index];
                            }
                            // 連結
                            this.CreateString(col, dbType, true);
                        }

                        // 主キー以外の列
                        for (int index = 0; index < this.ELSE_Columns.Count; index++)
                        {
                            string col = (string)ELSE_Columns[index];
                            string dbType = "";
                            if (this.ReadDbTypeInfo == true)
                            {
                                dbType = (string)ELSE_ColumnsDbType[index];
                            }
                            // 連結
                            this.CreateString(col, dbType, false);
                        }
                        // ↑↑↑【修正】↑↑↑

                        // 最後のカンマを削除する。
                        this.DeleteLastComma();

                        #endregion

                        // Daoクラス名
                        this.DaoClassName = this.DaoClassNameHeader + this.TableName.Replace(' ', '_') + this.DaoClassNameFooter;

                        // DTO、メンテナンス画面のみの場合はDao、SQLは生成しない。
                        if (!(this.cbxOnlyDTO.Checked || this.cbxOnlyTableMaintenance.Checked))
                        {
                            #region SQLファイルを生成

                            // メソッド名、SQLファイル名はDaoファイル用であるが、
                            // SQLファイル用に流用するため、このブロックで生成。

                            #region Insertの処理

                            // メソッド名
                            this.InsertMethodName
                                = this.MethodNameHeaderS + this.MethodLabel_Ins + this.MethodNameFooterS;

                            // SQLファイル名（保持用ワーク）
                            this.InsertFileName
                                = this.DaoClassName + "_" + this.InsertMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.InsertFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.InsertTemplateFilePath, this.SqlFileExtension);

                            #endregion

                            #region DynInsの処理

                            // メソッド名
                            this.DynInsMethodName
                                = this.MethodNameHeaderD + this.MethodLabel_Ins + this.MethodNameFooterD;

                            // SQLファイル名（保持用ワーク）
                            this.DynInsFileName
                                = this.DaoClassName + "_" + this.DynInsMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DynInsFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DynInsTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region Selectの処理

                            // メソッド名
                            this.SelectMethodName
                                = this.MethodNameHeaderS + this.MethodLabel_Sel + this.MethodNameFooterS;

                            // SQLファイル名（保持用ワーク）
                            this.SelectFileName
                                = this.DaoClassName + "_" + this.SelectMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.SelectFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.SelectTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region DynSelの処理

                            // メソッド名
                            this.DynSelMethodName
                                = this.MethodNameHeaderD + this.MethodLabel_Sel + this.MethodNameFooterD;

                            // SQLファイル名（保持用ワーク）
                            this.DynSelFileName
                                = this.DaoClassName + "_" + this.DynSelMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DynSelFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DynSelTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region Updateの処理

                            // メソッド名
                            this.UpdateMethodName
                                = this.MethodNameHeaderS + this.MethodLabel_Upd + this.MethodNameFooterS;

                            // SQLファイル名（保持用ワーク）
                            this.UpdateFileName
                                = this.DaoClassName + "_" + this.UpdateMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.UpdateFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.UpdateTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region DynUpdの処理

                            // メソッド名
                            this.DynUpdMethodName
                                = this.MethodNameHeaderD + this.MethodLabel_Upd + this.MethodNameFooterD;

                            // SQLファイル名（保持用ワーク）
                            this.DynUpdFileName
                                = this.DaoClassName + "_" + this.DynUpdMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DynUpdFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DynUpdTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region Deleteの処理

                            // メソッド名
                            this.DeleteMethodName
                                = this.MethodNameHeaderS + this.MethodLabel_Del + this.MethodNameFooterS;

                            // SQLファイル名（保持用ワーク）
                            this.DeleteFileName
                                = this.DaoClassName + "_" + this.DeleteMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DeleteFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DeleteTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            #region DynDelの処理

                            // メソッド名
                            this.DynDelMethodName
                                = this.MethodNameHeaderD + this.MethodLabel_Del + this.MethodNameFooterD;

                            // SQLファイル名（保持用ワーク）
                            this.DynDelFileName
                                = this.DaoClassName + "_" + this.DynDelMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DynDelFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DynDelTemplateFilePath, this.XmlFileExtension);

                            #endregion

                            // ↓↓↓【追加】↓↓↓
                            #region DynSelCntの処理

                            // メソッド名
                            this.DynSelCntMethodName
                                = this.MethodNameHeaderD + this.MethodLabel_SelCnt + this.MethodNameFooterD;

                            // SQLファイル名（保持用ワーク）
                            this.DynSelCntFileName
                                = this.DaoClassName + "_" + this.DynSelCntMethodName;

                            // ファイル名（テンポラリ・カレント）
                            this.FileName = this.DynSelCntFileName;

                            // ファイル生成
                            this.GenerateSQLFile(this.DynSelCntTemplateFilePath, this.XmlFileExtension);

                            #endregion
                            // ↑↑↑【追加】↑↑↑

                            #endregion

                            #region Daoファイルを生成

                            // カレント・ファイル出力用とReplace用途
                            this.FileName = this.DaoClassName;

                            // Daoファイルを生成
                            // DTE：DaoClassテンプレートのEncoding
                            this.GenerateClassFile(
                                this.DaoTemplateFilePath,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName);

                            #endregion
                        }

                        // ↓↓↓【追加】↓↓↓
                        if (this.CreateDTO
                            && !this.cbxOnlyTableMaintenance.Checked)
                        {
                            #region Entityファイルを生成

                            if (this.cbxEntity.Checked)
                            {
                                // カレント・ファイル出力用とReplace用途
                                // Entityクラス名
                                this.EntityClassName
                                    = this.EntityClassNameHeader + this.TableName.Replace(' ', '_') + this.EntityClassNameFooter;

                                // Entityファイル名
                                this.FileName = this.EntityClassName;

                                // Entityファイルを生成
                                // DTE：DaoClassテンプレートのEncoding
                                this.GenerateClassFile(
                                    this.EntityTemplateFilePath,
                                    (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                            }

                            #endregion

                            #region DataSetファイルを生成

                            if (this.cbxTypedDataSet.Checked)
                            {
                                // カレント・ファイル出力用とReplace用途
                                // DataSetクラス名
                                this.DataSetClassName
                                    = this.DataSetClassNameHeader + this.TableName.Replace(' ', '_') + this.DataSetClassNameFooter;

                                // DataSetファイル名
                                this.FileName = this.DataSetClassName + ".xsd";

                                // DataTableクラス名
                                this.DataTableClassName
                                    = this.DataTableClassNameHeader + this.TableName.Replace(' ', '_') + this.DataTableClassNameFooter;

                                // DataSetファイルを生成
                                // DTE：DaoClassテンプレートのEncoding
                                this.GenerateClassFile(
                                    this.DataSetTemplateFilePath,
                                    (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                            }

                            #endregion
                        }

                        if (this.CreateMaintenanceScreen
                            && !this.cbxOnlyDTO.Checked)
                        {
                            // カレント・ファイル出力用とReplace用途

                            // TableAdapterを生成
                            this.FileName = this.TableName.Replace(' ', '_') + this.TableAdapterTemplateFileName;
                            this.GenerateClassFile(
                                this.TableAdapterTemplateFilePath,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName);

                            #region ASPXファイル、Codebehindファイル

                            // ASPXファイル、Codebehindファイル(ConditionalSearch)を生成
                            this.FileName = this.TableName + this.ConditionalSearchTemplateFileName;
                            this.GenerateClassFile(
                                this.ConditionalSearchTemplateFilePath,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                            this.GenerateClassFile(
                                this.ConditionalSearchTemplateFilePath + "." + this.ClassTemplateFileExtension,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);

                            // ASPXファイル、Codebehindファイル(SearchAndUpdate)を生成
                            this.FileName = this.TableName + this.SearchAndUpdateTemplateFileName;
                            this.GenerateClassFile(
                                this.SearchAndUpdateTemplateFilePath,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                            this.GenerateClassFile(
                                this.SearchAndUpdateTemplateFilePath + "." + this.ClassTemplateFileExtension,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);


                            // ASPXファイル、Codebehindファイル(Detail)を生成
                            this.FileName = this.TableName + this.DetailTemplateFileName;
                            this.GenerateClassFile(
                                this.DetailTemplateFilePath,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                            this.GenerateClassFile(
                                this.DetailTemplateFilePath + "." + this.ClassTemplateFileExtension,
                                (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);

                            #endregion
                        }
                        // ↑↑↑【追加】↑↑↑

                        // テーブル名の初期化
                        this.TableName = "";
                    }
                    else
                    {
                        //　不正な状態
                        throw new CheckException(
                            //"D層定義情報ファイル・フォーマットエラー。テーブル名がありません。：" + rlDaoDef);
                            this.RM_GetString("FileFormatError") + rlDaoDef);
                    }
                }

                #endregion

                #endregion

                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;

                // メッセージ
                //MessageBox.Show("自動生成完了！");
                MessageBox.Show(this.RM_GetString("FileGenComplete"));
            }
            catch (CheckException cex)
            {
                //MessageBox.Show("チェックエラーです：" + cex.Message);
                MessageBox.Show(this.RM_GetString("CheckExceptionError") + cex.Message);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("ランタイムエラーです：" + ex.Message);
                MessageBox.Show(this.RM_GetString("RuntimeError") + ex.Message);
            }
            finally
            {
                // D層定義情報ファイル
                if (srDaoDef != null)
                {
                    srDaoDef.Close();
                }

                // ↓↓↓【追加】↓↓↓
                // .NET型定義情報ファイル
                if (srTypeDef != null)
                {
                    srTypeDef.Close();
                }
                // ↑↑↑【追加】↑↑↑

                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        #endregion

        #region ファイル生成関数

        #region SQLファイル

        /// <summary>SQLファイルを生成する共通関数</summary>
        /// <param name="sqlTemplate">SQLテンプレートを指定</param>
        /// <param name="extension">出力するSQLの拡張子を指定</param>
        private void GenerateSQLFile(string sqlTemplate, string extension)
        {
            #region 変数

            // SQLテンプレート ファイル
            StreamReader srSQLTpl = null;

            // 読んだ行
            string rlSQLTpl = null;

            // SQLファイル
            StreamWriter swSQLFile = null;

            #endregion

            try
            {
                #region 処理

                // 入力テンプレート
                // STE：SQLテンプレートのEncoding
                srSQLTpl = this.OpenSrTemplate(sqlTemplate, (int)cmbSTEncoding.SelectedValue);

                // 出力ファイル
                swSQLFile = this.OpenSwSQLFile(this.FileName, extension);

                // ループ
                while (!srSQLTpl.EndOfStream)
                {
                    // 読んで
                    rlSQLTpl = srSQLTpl.ReadLine();

                    // 置換して
                    rlSQLTpl = this.ReplaceSQL(rlSQLTpl, sqlTemplate);

                    // 書いて
                    swSQLFile.WriteLine(rlSQLTpl);
                }

                #endregion
            }
            finally
            {
                #region ストリームの破棄

                // SQLテンプレート ファイル
                if (srSQLTpl != null)
                {
                    srSQLTpl.Close();
                }
                // SQLファイル
                if (srSQLTpl != null)
                {
                    swSQLFile.Close();
                }

                #endregion
            }
        }

        #endregion

        #region Dao、Entity、DataSet、画面ASPX & クラス ファイル

        /// <summary>Dao、Entity、DataSet、画面クラス ファイルを生成する共通関数</summary>
        /// <param name="classTemplateFilePath">テンプレート</param>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="classFilePath">クラス ファイル</param>
        private void GenerateClassFile(
            string classTemplateFilePath, int encoding, string classFilePath)
        {
            #region 変数

            // 初期化
            ColumnNmbr = 0;

            // クラス テンプレート ファイル
            StreamReader srClassTpl = null;

            // 読んだ行
            string rlClassTpl = "";

            // クラス ファイル
            StreamWriter swClassFile = null;

            // 主キー列の[何某]テンプレート
            ArrayList pKColumnTemplate = null;

            // 主キー以外の列の[何某]テンプレート
            ArrayList elseColumnTemplate = null;

            // Update文のSet句用の[何某]テンプレート
            ArrayList pPUpdSetTemplate = null;

            // ↓↓↓【追加】↓↓↓
            // Select文のLike句用の[何某]テンプレート
            ArrayList pPLikeTemplate = null;
            // ↑↑↑【追加】↑↑↑

            // 処理済フラグ
            bool isProcessed;

            #endregion

            try
            {
                #region 処理

                // 入力テンプレート
                srClassTpl = this.OpenSrTemplate(classTemplateFilePath, encoding);

                // 出力ファイル（拡張子の調整）
                string temp = "";
                if (classTemplateFilePath.Substring(classTemplateFilePath.Length - 4).ToUpper() != ".XSD"
                    && classTemplateFilePath.Substring(classTemplateFilePath.Length - 5).ToUpper() != ".ASPX")
                {
                    // XSD、ASPXでは無い場合
                    temp = this.ClassTemplateFileExtension;
                }

                swClassFile = this.OpenSwClassFile(this.FileName, temp);

                // ループ
                while (!srClassTpl.EndOfStream)
                {
                    // 読んで
                    rlClassTpl = srClassTpl.ReadLine();

                    // 未処理
                    isProcessed = false;

                    #region loop

                    #region 主キー列の[何某]

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // 主キー列の[何某]開始
                        // 制御コメント開始があった場合の処理
                        if (rlClassTpl.IndexOf(this.CcLoopStart_PKColumn) == -1)
                        { }
                        else
                        {
                            // 主キー列の[何某]テンプレート
                            pKColumnTemplate = new ArrayList();

                            // 読んで（制御コメント内を読む）
                            rlClassTpl = srClassTpl.ReadLine();

                            // 制御コメント終了まで読む
                            while (rlClassTpl.IndexOf(this.CcLoopEnd_PKColumn) == -1)
                            {
                                // 追加
                                pKColumnTemplate.Add(rlClassTpl);

                                // 読んで・・・
                                rlClassTpl = srClassTpl.ReadLine();
                            }

                            // 主キー列の[何某]生成メソッドを呼ぶ。
                            this.WritePKColumn(pKColumnTemplate, swClassFile);

                            // 処理済み
                            isProcessed = true;
                        }
                    }

                    #endregion

                    #region 主キー以外の列の[何某]

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // 主キー以外の列の[何某]開始
                        // 制御コメント開始があった場合の処理
                        if (rlClassTpl.IndexOf(this.CcLoopStart_ElseColumn) == -1)
                        { }
                        else
                        {
                            // 主キー以外の列の[何某]テンプレート
                            elseColumnTemplate = new ArrayList();

                            // 読んで（制御コメント内を読む）
                            rlClassTpl = srClassTpl.ReadLine();

                            // 制御コメント終了まで読む
                            while (rlClassTpl.IndexOf(this.CcLoopEnd_ElseColumn) == -1)
                            {
                                // 追加
                                elseColumnTemplate.Add(rlClassTpl);

                                // 読んで・・・
                                rlClassTpl = srClassTpl.ReadLine();
                            }

                            // 主キー以外の列の[何某]生成メソッドを呼ぶ。
                            this.WriteElseColumn(elseColumnTemplate, swClassFile);

                            // 処理済み
                            isProcessed = true;
                        }
                    }

                    #endregion

                    #region Update文のSet句用の[何某]

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // Update文のSet句用の[何某]開始
                        // 制御コメント開始があった場合の処理
                        if (rlClassTpl.IndexOf(this.CcLoopStart_PPUpdSet) == -1)
                        { }
                        else
                        {
                            // Update文のSet句用の[何某]テンプレート
                            pPUpdSetTemplate = new ArrayList();

                            // 読んで（制御コメント内を読む）
                            rlClassTpl = srClassTpl.ReadLine();

                            // 制御コメント終了まで読む
                            while (rlClassTpl.IndexOf(this.CcLoopEnd_PPUpdSet) == -1)
                            {
                                // 追加
                                pPUpdSetTemplate.Add(rlClassTpl);

                                // 読んで・・・
                                rlClassTpl = srClassTpl.ReadLine();
                            }

                            // Update文のSet句用の[何某]生成メソッドを呼ぶ。
                            this.WritePPUpdSet(pPUpdSetTemplate, swClassFile);

                            // 処理済み
                            isProcessed = true;
                        }
                    }

                    #endregion

                    #region Select文のLike句用の[何某]

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // Select文のLike句用の[何某]開始
                        // 制御コメント開始があった場合の処理
                        if (rlClassTpl.IndexOf(this.CcLoopStart_PPLike) == -1)
                        { }
                        else
                        {
                            // Select文のLike句用の[何某]テンプレート
                            pPLikeTemplate = new ArrayList();

                            // 読んで（制御コメント内を読む）
                            rlClassTpl = srClassTpl.ReadLine();

                            // 制御コメント終了まで読む
                            while (rlClassTpl.IndexOf(this.CcLoopEnd_PPLike) == -1)
                            {
                                // 追加
                                pPLikeTemplate.Add(rlClassTpl);

                                // 読んで・・・
                                rlClassTpl = srClassTpl.ReadLine();
                            }

                            // Select文のLike句用の[何某]生成メソッドを呼ぶ。
                            this.WritePPLike(pPLikeTemplate, swClassFile);

                            // 処理済み
                            isProcessed = true;
                        }
                    }
                    // ↑↑↑【追加】↑↑↑

                    #endregion

                    #region TimeStamp column code

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // To replace the TimeStamp column code in the template 
                        if (rlClassTpl.IndexOf(this.CcIsRequired_TimeStamp) == -1)
                        { }
                        else
                        {
                            // 読んで（制御コメント内を読む）
                            rlClassTpl = srClassTpl.ReadLine();

                            if (!string.IsNullOrEmpty(this.TimeStampColName))
                            {
                                // To replace TimeStamp code, if TimeStamp colum is selected
                                rlClassTpl = this.ReplaceClass(srClassTpl.ReadLine());
                            }
                            else
                            {
                                // To remove TimeStamp code, if TimeStamp colum is not selected
                                srClassTpl.ReadLine();
                            }

                            // 書いて
                            swClassFile.WriteLine(rlClassTpl);

                            // 処理済み
                            isProcessed = true;
                        }
                    }

                    #endregion

                    #endregion

                    #region 通常の置換処理

                    if (isProcessed)
                    {
                        // 処理済み
                    }
                    else
                    {
                        // 未処理

                        // 置換して
                        rlClassTpl = this.ReplaceClass(rlClassTpl);

                        // 書いて
                        swClassFile.WriteLine(rlClassTpl);

                        // 処理済み
                        isProcessed = true;
                    }

                    #endregion
                }

                #endregion
            }
            finally
            {
                #region ストリームの破棄

                // クラス テンプレート ファイル
                if (srClassTpl != null)
                {
                    srClassTpl.Close();
                }
                // クラス ファイル
                if (swClassFile != null)
                {
                    swClassFile.Close();
                }

                #endregion
            }
        }

        #region [何某]を生成する

        // ↓↓↓【追加】↓↓↓
        /// <summary>[何某]周りの置換処理</summary>
        /// <param name="input">テンプレートの文字列</param>
        /// <returns>置換後のファイル出力する文字列</returns>
        private string ReplaceNanigashi(string input)
        {
            input = input.Replace(this.RpColumnName, this.ColumnName);

            // To replace Correct column header Number 
            if (input.IndexOf(this.RpColumnNmbr) != -1)
            {
                input = input.Replace(this.RpColumnNmbr, ColumnNmbr.ToString());
                ColumnNmbr += 1;
            }

            if (this.CreateDTO)
            {
                input = input.Replace(this.RpEntityTypeInfo, CmnMethods.AdjustTypeName(this.EntityTypeInfo, this.rbnDTL_VB.Checked));
                input = input.Replace(this.RpXSDTypeInfo, CmnMethods.ToXsTypeName(this.XSDTypeInfo));
            }

            return input;
        }
        // ↑↑↑【追加】↑↑↑

        /// <summary>主キー列の[何某]を生成する</summary>
        /// <param name="template">主キー列の[何某]のテンプレート</param>
        /// <param name="sr">StreamWriter</param>
        private void WritePKColumn(ArrayList template, StreamWriter sw)
        {
            // 主キーでループ
            //foreach (string col in this.PK_Columns)
            for (int i = 0; i < this.PK_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName = this.PK_Columns[i].ToString().Replace(' ', '_');

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.PK_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.PK_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }
        }

        /// <summary>主キー以外の列の[何某]を生成する</summary>
        /// <param name="template">主キー以外の列の[何某]のテンプレート</param>
        /// <param name="sr">StreamWriter</param>
        private void WriteElseColumn(ArrayList template, StreamWriter sw)
        {
            // 主キー以外でループ
            //foreach (string col in this.ELSE_Columns)
            for (int i = 0; i < this.ELSE_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName = this.ELSE_Columns[i].ToString().Replace(' ', '_');

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.ELSE_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.ELSE_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }
        }

        /// <summary>Update文のSet句用の[何某]を生成する</summary>
        /// <param name="template">Update文のSet句用の[何某]のテンプレート</param>
        /// <param name="sr">StreamWriter</param>
        private void WritePPUpdSet(ArrayList template, StreamWriter sw)
        {
            // 主キーの列でループ
            //foreach (string col in this.PK_Columns)
            for (int i = 0; i < this.PK_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName
                    = this.UpdateParamHeader + this.PK_Columns[i].ToString().Replace(' ', '_') + this.UpdateParamFooter;

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.PK_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.PK_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }

            // 主キー以外の列でループ
            //foreach (string col in this.ELSE_Columns)
            for (int i = 0; i < this.ELSE_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName
                    = this.UpdateParamHeader + this.ELSE_Columns[i].ToString().Replace(' ', '_') + this.UpdateParamFooter;

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.ELSE_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.ELSE_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }
        }

        // ↓↓↓【追加】↓↓↓
        /// <summary>Select文のLike句用の[何某]を生成する</summary>
        /// <param name="template">Select文のLike句用の[何某]のテンプレート</param>
        /// <param name="sr">StreamWriter</param>
        private void WritePPLike(ArrayList template, StreamWriter sw)
        {
            // 主キーの列でループ
            //foreach (string col in this.PK_Columns)
            for (int i = 0; i < this.PK_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName
                    = this.LikeParamHeader + this.PK_Columns[i].ToString().Replace(' ', '_') + this.LikeParamFooter;

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.PK_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.PK_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }

            // 主キー以外の列でループ
            //foreach (string col in this.ELSE_Columns)
            for (int i = 0; i < this.ELSE_Columns.Count; i++)
            {
                // 一応、ルールに従って・・・
                this.ColumnName
                    = this.LikeParamHeader + this.ELSE_Columns[i].ToString().Replace(' ', '_') + this.LikeParamFooter;

                // ↓↓↓【追加】↓↓↓
                if (this.CreateDTO)
                {
                    this.EntityTypeInfo = this.ELSE_ColumnsType[i].ToString();
                    this.XSDTypeInfo = this.ELSE_ColumnsType[i].ToString();
                }
                // ↑↑↑【追加】↑↑↑

                // [何某]を生成
                foreach (string rl in template)
                {
                    // カラム名の所は置換して書き込み。
                    sw.WriteLine(this.ReplaceNanigashi(rl));
                }
            }
        }
        // ↑↑↑【追加】↑↑↑

        #endregion

        #endregion

        #endregion

        #region 文字列置換処理関数

        #region SQLファイル

        #region 基本クエリ文字列生成

        /// <summary>クエリ文字列生成</summary>
        /// <param name="col">列名</param>
        /// <param name="dbType">DB型</param>
        /// <param name="isPK">主キー列のカラムかどうか</param>
        private void CreateString(string col, string dbType, bool isPK)
        {
            #region 共通

            //（１）共通 静的カラム リスト
            this.AllColumnListSQL += this.EnclosureCharS + col + this.EnclosureCharE + ",";

            if (isPK)
            {
                this.PKColumnListSQL += this.EnclosureCharS + col + this.EnclosureCharE + ",";
            }

            //（２）共通 静的 検索条件 ★
            if (isPK)
            {
                // 主キー列である
                this.ColumnsCondition +=
                    this.EnclosureCharS + col + this.EnclosureCharE + " = "
                    + this.ParamSign.ToString() + col.Replace(" ", "_") + ",";
            }
            else
            {
                // 主キー列でない

                if (this.TimeStampStatus == TSS.Non)
                {
                    // タイムスタンプ指定無し
                }
                else
                {
                    // タイムスタンプ指定有り
                    if (col == this.TimeStampColName)
                    {
                        //↓↓↓【削除】　WHERE句にタイムスタンプ列が入らないように

                        // 誰がコメントアウトした？
                        // タイムスタンプ列である。
                        this.ColumnsCondition +=
                            this.EnclosureCharS + col + this.EnclosureCharE + " = "
                            + this.ParamSign.ToString() + col.Replace(" ", "_") + ",";

                        //↑↑↑【削除】
                    }
                }
            }

            //（３）共通 動的 検索条件
            this.DynColsCondition +=
                "<IF>AND "
                + this.EnclosureCharS + col + this.EnclosureCharE
                + " = " + this.ParamSign.ToString() + col.Replace(' ', '_') +
                "<ELSE>AND "
                + this.EnclosureCharS + col + this.EnclosureCharE
                + " IS NULL</ELSE>" + "</IF>" + ",";

            //（３．５）共通 動的 曖昧検索条件
            if (this.rbnODP.Checked == true)
            {
                //Oracle ODP.NETの場合
                if (dbType.ToLower() == "nchar" || dbType.ToLower() == "nvarchar2" || dbType.ToLower() == "nclob")
                {
                    //NCHAR、NVARCHAR2、NCLOBの場合
                    this.DynColsCondition_Like +=
                        "<IF>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                        + " = " + this.ParamSign.ToString() + col.Replace(' ', '_') +
                        "<ELSE>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                        + " IS NULL</ELSE>" + "</IF>" + "," +

                        "<IF>AND "
                        + this.EnclosureCharS + col + this.EnclosureCharE + " " + cmbLikeStatement.Text + " "
                        + this.ParamSign.ToString() + this.LikeParamHeader + col.Replace(' ', '_') + this.LikeParamFooter;
                    if (chkEscapeToNChar.Checked == true)
                    {
                        //TO_NCHARする場合
                        this.DynColsCondition_Like += " ESCAPE TO_NCHAR('" + txtEscapeChar.Text + "')";
                    }
                    else
                    {
                        //TO_NCHARしない場合
                        this.DynColsCondition_Like += " ESCAPE '" + txtEscapeChar.Text + "'";
                    }

                    this.DynColsCondition_Like += "</IF>" + ",";
                }
                else
                {
                    //NCHAR、NVARCHAR2、NCLOB以外の場合
                    this.DynColsCondition_Like +=
                        "<IF>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                          + " = " + this.ParamSign.ToString() + col.Replace(' ', '_') +
                          "<ELSE>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                          + " IS NULL</ELSE>" + "</IF>" + "," +

                        "<IF>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                          + " LIKE " + this.ParamSign.ToString() + this.LikeParamHeader + col.Replace(' ', '_') + this.LikeParamFooter
                          + " ESCAPE '" + txtEscapeChar.Text + "' " + "</IF>" + ",";
                }
            }
            else
            {
                //Oracle ODP.NET以外の場合
                this.DynColsCondition_Like +=
                    "<IF>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                      + " = " + this.ParamSign.ToString() + col.Replace(' ', '_') +
                      "<ELSE>AND " + this.EnclosureCharS + col + this.EnclosureCharE
                      + " IS NULL</ELSE>" + "</IF>" + "," +

                    "<IF>AND "
                      + this.EnclosureCharS + col + this.EnclosureCharE + " LIKE "
                      + this.ParamSign.ToString() + this.LikeParamHeader + col.Replace(' ', '_') + this.LikeParamFooter
                      + "</IF>" + ",";
            }

            //（４）共通 動的更新SET句リスト ★
            this.DynUpdParameters +=
                "<IF>"
                + this.EnclosureCharS + col + this.EnclosureCharE + " = "
                + this.ParamSign.ToString() + this.UpdateParamHeader + col.Replace(' ', '_') + this.UpdateParamFooter
                + ",</IF>" + ",";

            #endregion

            #region 個別

            //（５）Insert用パラメタ リスト ★
            this.InsertParameters += this.ParamSign + col.Replace(' ', '_') + ",";

            //（６）DynIns用カラム リスト ★
            this.DynInsColumns +=
                "<INSCOL name=\"" + col.Replace(' ', '_') + "\">"
                + this.EnclosureCharS + col + this.EnclosureCharE + ",</INSCOL>" + ",";

            //（７）DynIns用パラメタ リスト ★
            this.DynInsParameters +=
                "<IF>" + this.ParamSign + col.Replace(' ', '_') + ",<ELSE></ELSE></IF>" + ",";

            #endregion

            #region その他

            this.AllColumnList += col.Replace(' ', '_') + ", ";
            this.AllColumnListTableAdapterSQL += "_s_" + col + "_e_, ";

            if (isPK)
            {
                this.PKColumnList += col.Replace(' ', '_') + ", ";
                if (string.IsNullOrEmpty(this.PKFirstColumn)) { this.PKFirstColumn = col.Replace(' ', '_'); }
            }

            #endregion
        }

        /// <summary>置換文字列と置換するSQLの最後のカンマを削除する</summary>
        private void DeleteLastComma()
        {
            //（１）共通 静的カラム リスト
            this.DeleteLastComma2(ref this.AllColumnListSQL);
            this.DeleteLastComma2(ref this.PKColumnListSQL);

            //（２）共通 静的 検索条件 ★
            this.DeleteLastComma2(ref this.ColumnsCondition);
            //（３）共通 動的 検索条件
            this.DeleteLastComma2(ref this.DynColsCondition);
            //（３．５）共通 動的 曖昧検索条件
            this.DeleteLastComma2(ref this.DynColsCondition_Like);
            //（４）共通 動的更新SET句リスト ★
            this.DeleteLastComma2(ref this.DynUpdParameters);

            //（５）Insert用パラメタ リスト ★
            this.DeleteLastComma2(ref this.InsertParameters);
            //（６）DynIns用カラム リスト ★
            this.DeleteLastComma2(ref this.DynInsColumns);
            //（７）DynIns用パラメタ リスト ★
            this.DeleteLastComma2(ref this.DynInsParameters);

            this.DeleteLastComma2(ref this.AllColumnList);
            this.DeleteLastComma2(ref this.PKColumnList);
            this.DeleteLastComma2(ref this.AllColumnListTableAdapterSQL);
        }

        /// <summary>置換文字列と置換するSQLの最後のカンマを削除する</summary>
        /// <param name="str">入出力</param>
        private void DeleteLastComma2(ref string str)
        {
            if (str.Length >= 1)
            {
                if (str[str.Length - 1] == ',')
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }

            if (str.Length >= 2)
            {
                if (str.Substring(str.Length - 2, 2) == ", ")
                {
                    str = str.Substring(0, str.Length - 2);
                }
            }
        }

        #endregion

        #region 置換処理（調整処理込み）

        /// <summary>SQLテンプレートから入力した文字列の置換処理</summary>
        /// <param name="input">SQLテンプレートから入力した文字列</param>
        /// <param name="sqlTemplate">SQLテンプレートを指定</param>
        /// <returns>SQLファイルに出力する置換後の文字列</returns>
        private string ReplaceSQL(string input, string sqlTemplate)
        {
            // ワーク
            string indent = "";

            string oldStirng = "";
            string newStirng = "";

            string work = "";

            #region ヘッダ・フッタ、テーブル

            // ・XMLのエンコーディング
            if (input.IndexOf(this.RpXMLEncoding) != -1)
            {
                input = input.Replace(this.RpXMLEncoding, this.XMLEncoding);
            }

            // ・ファイル名
            if (input.IndexOf(this.RpFileName) != -1)
            {
                input = input.Replace(this.RpFileName, this.FileName);
            }

            // ・日付
            if (input.IndexOf(this.RpTimeStamp) != -1)
            {
                input = input.Replace(this.RpTimeStamp, this.TimeStamp);
            }

            // ・ユーザ名
            if (input.IndexOf(this.RpUserName) != -1)
            {
                input = input.Replace(this.RpUserName, this.UserName);
            }

            // テーブル名
            if (input.IndexOf(this.RpTableName) != -1)
            {
                //if (this.rbnHiRDB.Checked)
                //{
                //    // HiRDBで、表名に囲い文字を適用できない。
                //    input = input.Replace(this.RpTableName, this.TableName);
                //}
                //else
                //{
                // その他のDBでは囲い文字を使用可能である。
                input = input.Replace(this.RpTableName, this.EnclosureCharS + this.TableName + this.EnclosureCharE);
                //}
            }

            #endregion

            #region クエリ文字列

            #region（１）共通 静的カラム リスト

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpAllColumnListSQL, out indent))
            {
                // [,] → [,] + [\r\n] + [インデント]と置換する。
                oldStirng = ",";
                newStirng = "," + "\r\n" + indent;

                // ワークへコピー
                work = this.AllColumnListSQL;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // 置換
                input = input.Replace(this.RpAllColumnListSQL, work);
            }

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpPKColumnListSQL, out indent))
            {
                // [,] → [,] + [\r\n] + [インデント]と置換する。
                oldStirng = ",";
                newStirng = "," + "\r\n" + indent;

                // ワークへコピー
                work = this.PKColumnListSQL;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // 置換
                input = input.Replace(this.RpPKColumnListSQL, work);
            }

            #endregion

            #region（２）共通 静的 検索条件 ★

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpColumnsCondition, out indent))
            {
                // [,] → [\r\n] + [インデント] + [AND ]と置換する。  
                oldStirng = ",";
                newStirng = "\r\n" + indent + "AND ";

                // ワークへコピー
                work = this.ColumnsCondition;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // タイムスタンプ状態によっては、
                if (this.TimeStampStatus != TSS.Non)
                {
                    //「AND [eee] = @eee」 → 「<IF>AND [eee] = @eee</IF>」と置換する。

                    if (this.cbxTSIndisp.Checked &&
                        (sqlTemplate == this.UpdateTemplateFilePath || sqlTemplate == this.DeleteTemplateFilePath))
                    {
                        // Update、Deleteクエリのタイムスタンプ指定必須の指定がなされた場合、
                        // Update、Deleteクエリの共通 静的 検索条件に、調整２を施さない。
                    }
                    else
                    {
                        // 上記以外の場合調整２を施す。

                        oldStirng =
                            "AND " + this.EnclosureCharS + this.TimeStampColName + this.EnclosureCharE
                            + " = " + this.ParamSign + this.TimeStampColName.Replace(' ', '_');

                        newStirng = "<IF>" + oldStirng + "</IF>";

                        // 調整２
                        work = work.Replace(oldStirng, newStirng);
                    }
                }

                // 置換
                input = input.Replace(this.RpColumnsCondition, work);
            }

            #endregion

            #region（３）共通 動的 検索条件

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpDynColsCondition, out indent))
            {
                // [>,] → [>] + [\r\n] + [インデント]と置換する。
                oldStirng = ">,";
                newStirng = ">" + "\r\n" + indent;

                // ワークへコピー
                work = this.DynColsCondition;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // 置換
                input = input.Replace(this.RpDynColsCondition, work);
            }

            #endregion

            #region（３．５）共通 動的 曖昧検索条件

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpDynColsCondition_Like, out indent))
            {
                // [>,] → [>] + [\r\n] + [インデント]と置換する。
                oldStirng = ">,";
                newStirng = ">" + "\r\n" + indent;

                // ワークへコピー
                work = this.DynColsCondition_Like;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // 置換
                input = input.Replace(this.RpDynColsCondition_Like, work);
            }

            #endregion

            #region（４）共通 動的更新SET句リスト ★

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpDynUpdParameters, out indent))
            {
                // [>,] → [>] + [\r\n] + [インデント]と置換する。
                oldStirng = ">,";
                newStirng = ">" + "\r\n" + indent;

                // ワークへコピー
                work = this.DynUpdParameters;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // タイムスタンプ状態によっては、
                if (this.TimeStampStatus == TSS.NandM)
                {
                    //「<IF>[eee] = @Set_eee_Upd,</IF>」 → 「[eee] = SYSTIMESTAMP(),」と置換する。
                    oldStirng =
                        "<IF>" +
                        this.EnclosureCharS + this.TimeStampColName + this.EnclosureCharE + " = " +
                        this.ParamSign + this.UpdateParamHeader + this.TimeStampColName.Replace(' ', '_') + this.UpdateParamFooter
                        + ",</IF>";

                    newStirng =
                        this.EnclosureCharS + this.TimeStampColName + this.EnclosureCharE
                        + " = " + this.TimeStampUpdMethod + ",";

                    // 調整２
                    work = work.Replace(oldStirng, newStirng);
                }

                // 置換
                input = input.Replace(this.RpDynUpdParameters, work);
            }

            #endregion

            #region（５）Insert用パラメタ リスト ★

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpInsertParameters, out indent))
            {
                // [,] → [,] + [\r\n] + [インデント]と置換する。
                oldStirng = ",";
                newStirng = "," + "\r\n" + indent;

                // ワークへコピー
                work = this.InsertParameters;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // タイムスタンプ状態によっては、
                if (this.TimeStampStatus == TSS.NandM)
                {
                    //「@eee」 → 「SYSTIMESTAMP()」と置換する。
                    oldStirng = this.ParamSign + this.TimeStampColName.Replace(' ', '_');

                    newStirng = this.TimeStampUpdMethod;

                    // 誤変換が生じ得るの、最後に「,」を追加する。
                    work += ",";

                    // 調整２
                    work = work.Replace(oldStirng + ",", newStirng + ",");

                    // 先ほど追加した「,」を削除する。
                    work = work.Substring(0, work.Length - 1);
                }

                // 置換
                input = input.Replace(this.RpInsertParameters, work);
            }

            #endregion

            #region（６）DynIns用カラム リスト ★

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpDynInsColumns, out indent))
            {
                // [>,] → [>] + [\r\n] + [インデント]と置換する。
                oldStirng = ">,";
                newStirng = ">" + "\r\n" + indent;

                // ワークへコピー
                work = this.DynInsColumns;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // タイムスタンプ状態によっては、
                if (this.TimeStampStatus == TSS.NandM)
                {
                    //「<INSCOL name="eee">[eee],</INSCOL>」 → 「[eee],」と置換する。
                    oldStirng = "<INSCOL name=\"" + this.TimeStampColName.Replace(' ', '_') + "\">"
                        + this.EnclosureCharS + this.TimeStampColName + this.EnclosureCharE + ",</INSCOL>";

                    newStirng = this.EnclosureCharS + this.TimeStampColName + this.EnclosureCharE + ",";

                    // 調整２
                    work = work.Replace(oldStirng, newStirng);
                }

                // 置換
                input = input.Replace(this.RpDynInsColumns, work);
            }

            #endregion

            #region（７）DynIns用パラメタ リスト ★

            // 状態チェック
            if (this.ExistenceCheckOfSubstitutionString(input, this.RpDynInsParameters, out indent))
            {
                // [>,] → [>] + [\r\n] + [インデント]と置換する。
                oldStirng = ">,";
                newStirng = ">" + "\r\n" + indent;

                // ワークへコピー
                work = this.DynInsParameters;

                // 調整１
                work = work.Replace(oldStirng, newStirng);

                // タイムスタンプ状態によっては、
                if (this.TimeStampStatus == TSS.NandM)
                {
                    //「<IF>@eee,</IF>」 → 「SYSTIMESTAMP(),」と置換する。
                    oldStirng = "<IF>" + this.ParamSign + this.TimeStampColName.Replace(' ', '_') + ",<ELSE></ELSE></IF>";

                    newStirng = this.TimeStampUpdMethod + ",";

                    // 調整２
                    work = work.Replace(oldStirng, newStirng);
                }

                // 置換
                input = input.Replace(this.RpDynInsParameters, work);
            }

            #endregion

            #endregion

            // そのまま返す。
            return input;
        }

        /// <summary>置換文字列の存在チェックとインデントの取得</summary>
        /// <param name="input">入力</param>
        /// <param name="replaceString">置換文字列</param>
        /// <param name="queryString">インデント文字列</param>
        /// <returns>置換文字列の存在チェック結果</returns>
        private bool ExistenceCheckOfSubstitutionString(
            string input, string replaceString, out string indent)
        {
            int index = 0;

            // 置換対象の存在確認
            index = input.IndexOf(replaceString);

            if (index == -1)
            {
                // 存在しない
                indent = "";
                return false;
            }
            else
            {
                // 存在する

                // インデントを取得
                indent = input.Substring(0, index);

                // インデント チェック
                if (indent.Trim() == "")
                {
                    // インデントのみの場合
                }
                else
                {
                    // インデント以外の文字列が混じる場合
                    //throw new CheckException(replaceString + "の前には、インデントのみ指定できます。：" + input);
                    throw new CheckException(replaceString + this.RM_GetString("StringIndentation") + input);
                }

                return true;
            }
        }

        #endregion

        #endregion

        #region Classファイル

        /// <summary>クラス テンプレートから入力した文字列の置換処理</summary>
        /// <param name="input">クラス テンプレートから入力した文字列</param>
        /// <returns>クラス テンプレートに出力する置換後の文字列</returns>
        private string ReplaceClass(string input)
        {
            #region ヘッダ共通部
            // To replace Correct column header Number 
            if (input.IndexOf(this.RpColumnNmbr) != -1)
            {
                input = input.Replace(this.RpColumnNmbr, ColumnNmbr.ToString());
                ColumnNmbr += 1;
            }

            // ファイル名
            input = input.Replace(this.RpFileName, this.FileName);

            // Daoクラス名
            input = input.Replace(this.RpDaoClassName, this.DaoClassName);

            // Entityクラス名
            input = input.Replace(this.RpEntityClassName, this.EntityClassName);
            // DataSetクラス名
            input = input.Replace(this.RpDataSetClassName, this.DataSetClassName);
            // DataTableクラス名
            input = input.Replace(this.RpDataTableClassName, this.DataTableClassName);

            // ・タイムスタンプ
            input = input.Replace(this.RpTimeStamp, this.TimeStamp);
            // ・ユーザ名
            input = input.Replace(this.RpUserName, this.UserName);

            // コードビハインドの言語
            input = input.Replace(this.RpCodebehindLanguage, this.CodebehindLanguage);
            // コードビハインドの拡張子
            input = input.Replace(this.RpClassTemplateFileExtension, this.ClassTemplateFileExtension);

            #endregion

            #region メイン

            // テーブル名
            input = input.Replace(this.RpTableName, this.TableName);

            // メソッド名（静的）
            input = input.Replace(this.RpInsertMethodName, this.InsertMethodName);
            input = input.Replace(this.RpSelectMethodName, this.SelectMethodName);
            input = input.Replace(this.RpUpdateMethodName, this.UpdateMethodName);
            input = input.Replace(this.RpDeleteMethodName, this.DeleteMethodName);

            // メソッド名（動的）
            input = input.Replace(this.RpDynInsMethodName, this.DynInsMethodName);
            input = input.Replace(this.RpDynSelMethodName, this.DynSelMethodName);
            input = input.Replace(this.RpDynUpdMethodName, this.DynUpdMethodName);
            input = input.Replace(this.RpDynDelMethodName, this.DynDelMethodName);
            input = input.Replace(this.RpDynSelCntMethodName, this.DynSelCntMethodName);

            // ファイル名（静的）
            input = input.Replace(this.RpInsertFileName, this.InsertFileName + "." + this.SqlFileExtension);
            input = input.Replace(this.RpSelectFileName, this.SelectFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpUpdateFileName, this.UpdateFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpDeleteFileName, this.DeleteFileName + "." + this.XmlFileExtension);

            // ファイル名（動的）
            input = input.Replace(this.RpDynInsFileName, this.DynInsFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpDynSelFileName, this.DynSelFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpDynUpdFileName, this.DynUpdFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpDynDelFileName, this.DynDelFileName + "." + this.XmlFileExtension);
            input = input.Replace(this.RpDynSelCntFileName, this.DynSelCntFileName + "." + this.XmlFileExtension);

            // 列情報
            // コメントアウト(TimeStamp)
            if (!string.IsNullOrEmpty(this.TimeStampColName))
            {
                input = input.Replace("TS" + this.RpCommentOut, "");
                input = input.Replace(this.RpTimeStampColName, this.TimeStampColName);
            }
            else
            {
                input = input.Replace("TS" + this.RpCommentOut, this.CommentOut);
            }

            // 主キー 
            // ・先頭行
            input = input.Replace(this.RpPKFirstColumn, this.PKFirstColumn);
            // ・リスト
            input = input.Replace(this.RpPKColumnList, this.PKColumnList);

            // TableAdapterで使用するカラム リスト
            input = input.Replace(this.RpAllColumnListTableAdapterSQL, this.AllColumnListTableAdapterSQL);

            #endregion

            // DBMS and DAP in the pages for Data Provide rselection
            input = input.Replace(this.RpDBMS, this.strDBMS);
            input = input.Replace(this.RpDAP, this.strDAP);

            return input;
        }

        #endregion

        #endregion

        /// <summary>This Method gets the string values from resource file based on the key passed</summary>
        private string RM_GetString(string key)
        {
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }

        /// <summary>This Method add the Tab 2 and remove Tab 1</summary>
        private void btnOptions_Click(object sender, EventArgs e)
        {
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Remove(this.tabPage1);
        }

        /// <summary>This Method cancels the Options tab</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            switch (this.strDAP)
            {
                case "OLE":
                    this.rbnOLE.Checked = true;
                    break;
                case "ODB":
                    this.rbnODB.Checked = true;
                    break;
                case "Oracle":
                    this.rbnODP.Checked = true;
                    break;
                case "DB2":
                    this.rbnDB2.Checked = true;
                    break;
                case "MCN":
                    this.rbnMySQL.Checked = true;
                    break;
                case "PstGrS":
                    this.rbnPstgrs.Checked = true;
                    break;
                default:
                    this.rbnSQL.Checked = true;
                    break;
            }

            if (this.cbxOnlyTableMaintenance.Checked || this.cbxTableMaintenance.Checked)
            {
                this.cbxOnlyTableMaintenance.Checked = false;
                this.cbxTableMaintenance.Checked = false;
            }
            if (cbxOnlyDTO.Checked || this.cbxEntity.Checked || this.cbxTypedDataSet.Checked)
            {
                this.cbxOnlyDTO.Checked = false;
                this.cbxEntity.Checked = false;
                this.cbxTypedDataSet.Checked = false;
            }
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Remove(this.tabPage2);
        }

        /// <summary>This Method saves options setting and redirects to step2</summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Remove(this.tabPage2);
            this.lblSelected.Visible = true;
        }
    }
}
