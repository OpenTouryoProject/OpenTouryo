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
//* Class Name :Form3
//* Classjapanese name ：Dynamic Parameterised query execution tool(Automatic Screen Generation for join select statements)
//*
//* Author          ：Santosh Avaji
//* Update History  ：
//* 
//*  Date and time   Updated By        Content
//*  ----------      ----------------  -------------------------------------------------
//*  2014/09/23      Santosh Avaji     Added Code which is required for Automatic Screen generation for Select join statements
//*  2015/10/28      Sandeep           Optimized messages in the resource file and implemented code to format it
//*  2016/04/06      Shashikiran       Modified the code to replace space in Table Names with underscore operator 
//*  2016/05/03      Shashikiran       Modified the code to replace space in column Names with underscore operator  
//**********************************************************************************

#pragma warning disable 0414

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace DPQuery_Tool
{
    public partial class Form3 : Form
    {
        #region 画面テンプレート ファイル

        /// <summary>ConditionalSearchテンプレート ファイル名</summary>
        private string JoinConditionalSearchTemplateFileName = "";
        /// <summary>ConditionalSearchテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string ConditionalSearchTemplateFilePath = "";

        /// <summary>SearchAndUpdateテンプレート ファイル名</summary>
        private string JoinSearchAndUpdateTemplateFileName = "";
        /// <summary>SearchAndUpdateテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string SearchAndUpdateTemplateFilePath = "";

        /// <remarks>初期化対象外</remarks>
        /// <summary>Detailテンプレート ファイル名</summary>
        private string JoinDetailTemplateFileName = "";
        /// <summary>Detailテンプレート ファイル パス</summary>
        /// <remarks>初期化対象外</remarks>
        private string DetailTemplateFilePath = "";

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
        private string RpJoinTableName = "";
        /// <summary>テーブル名（置換文字列）</summary>
        private string TableName = "";
        /// <summary>Join Table name replacement String </summary>
        private string TableName_join = "";

        /// <summary>囲い文字のない カラム リスト（置換対象）</summary>
        private string RpAllColumnList = "";
        /// <summary>囲い文字のない カラム リスト（置換文字列）</summary>
        //private string AllColumnList = "";

        /// <summary>囲い文字のない PKカラム リスト（置換対象）</summary>
        private string RpPKColumnList = "";
        /// <summary>囲い文字のない PKカラム リスト（置換文字列）</summary>
        private string PKColumnList = "";

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

        // Select join query column strings to be replaced
        /// <summary>JoinColumn name (to be replaced) </summary>
        private string RpJoinColumnName = "";
        /// <summary>JoinTextBoxColumn name (to be replaced) </summary>
        private string RpJoinTextboxColumnName = "";

        /// <summary>カラム名（置換文字列）</summary>
        private string ColumnName = "";

        /// <summary> Join Column Name Replacement String</summary>
        private string JoinColumnName = "";
        /// <summary> Join TextboxColumn Name Replacement String</summary>
        private string JoinTextBoxColumnName = "";

        /// <summary>カラム番号（置換対象）</summary>
        private string RpColumnNmbr = "";
        /// <summary>カラム番号（置換文字列）</summary>
        private int ColumnNmbr = 0; // for gridview header text

        #region [何某]生成 制御用文字列

        /// <summary>主キー列の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_PKColumn = "";
        /// <summary>主キー列の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_PKColumn = "";
        /// <summary>主キー以外の列の[何某]生成開始制御用文字列</summary>
        private string CcLoopStart_ElseColumn = "";
        /// <summary>主キー以外の列の[何某]生成終了制御用文字列</summary>
        private string CcLoopEnd_ElseColumn = "";
        //Added for joinTables Loop
        /// <summary>This is to start the loop for the N number Tablenames</summary>
        private string CcLoopStart_JoinTables = "";
        /// <summary>This is to end the loop for the N number Tablenames</summary>
        private string CcLoopEnd_JoinTables = "";

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

        // DB型情報ファイル読み込みフラグ
        private bool ReadDbTypeInfo = false;
        // DB型情報ファイルのパス
        private string DbTypeInfoFilePath = "";

        // ↑↑↑【追加】↑↑↑

        #endregion

        #region  Added variables for Join table Select Query feature

        /// <summary>Lists of Primary Key columns of all Tables from DaoDefinition file</summary>
        public ArrayList PkList = null;
        /// <summary>Lists of Else Key columns of all Tables from DaoDefinition file</summary>
        public ArrayList ElseList = null;
        /// <summary>List of all Column names from all the tables  from DaoDefinition file</summary>
        public ArrayList Allcols = null;
        /// <summary>List of all Table names from all the tables from DaoDefinition file</summary>
        public ArrayList AllTableNames = null;
        /// <summary>List of all table names from DPqueryTool </summary>
        public ArrayList DPQTableNames = null;
        /// <summary>List of all column names from DPqueryTool </summary>
        public ArrayList DPQAllCoumns = null;
        /// <summary>Table Number in the List of Tables</summary>
        public int intTable = 0;

        // DBMS constant to replace
        private string RpDBMS = "";
        //DBMS value  to  be replaced
        private string strDBMS = "";

        //DAP constant to replace
        private string RpDAP = "";
        //DAP value to be replaced
        private string strDAP = "";
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
        public Form3()
        {
            InitializeComponent();
        }

        /// <summary>プロバイダの設定</summary>
        /// <param name="s">プロバイダを表す文字列</param>
        public void Init(string s)
        {
            switch (s)
            {
                case "OLE":
                    this.rbnOLE.Select();
                    break;
                case "ODB":
                    this.rbnODB.Select();
                    break;
                case "ODP":
                    this.rbnODP.Select();
                    break;
                case "DB2":
                    this.rbnDB2.Select();
                    break;
                //case "HIR":
                //    this.rbnHiRDB.Select();
                //    break;
                case "MCN":
                    this.rbnMySQL.Select();
                    break;
                case "NPS":
                    this.rbnPstgrs.Select();
                    break;
                default:
                    this.rbnSQL.Select();
                    break;
            }
        }

        /// <summary>初期設定</summary>
        private void Form3_Load(object sender, EventArgs e)
        {
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

            // ↑↑↑【追加】↑↑↑
            #endregion

            #region テキストボックスの初期化

            // フォルダパスの初期化
            this.txtSetSourceTemplate.Text = GetConfigParameter.GetConfigValue("inputFilesRoot");
            this.txtSetOutput.Text = GetConfigParameter.GetConfigValue("outputFilesRoot");

            #endregion

            #region ファイル名、クラス名、メソッド名（＋空文字チェック）

            #region 読み

            #region クラス テンプレート ファイル


            #region メンテナンス画面テンプレート ファイル

            this.JoinConditionalSearchTemplateFileName = GetConfigParameter.GetConfigValue("JoinConditionalSearchTemplateFileName");
            if (string.IsNullOrEmpty(JoinConditionalSearchTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "JoinConditionalSearchTemplateFileName");
            }

            this.JoinSearchAndUpdateTemplateFileName = GetConfigParameter.GetConfigValue("JoinSearchAndUpdateTemplateFileName");
            if (string.IsNullOrEmpty(JoinSearchAndUpdateTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "JoinSearchAndUpdateTemplateFileName");
            }

            this.JoinDetailTemplateFileName = GetConfigParameter.GetConfigValue("JoinDetailTemplateFileName");
            if (string.IsNullOrEmpty(JoinDetailTemplateFileName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "JoinDetailTemplateFileName");
            }

            #endregion

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


            this.RpTableName = GetConfigParameter.GetConfigValue("RpTableName");
            if (string.IsNullOrEmpty(this.RpTableName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpTableName");
            }
            this.RpJoinTableName = GetConfigParameter.GetConfigValue("RpJoinTableName");
            if (string.IsNullOrEmpty(this.RpJoinTableName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpJoinTableName");
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

            this.RpJoinColumnName = GetConfigParameter.GetConfigValue("RpJoinColumnName");
            if (string.IsNullOrEmpty(this.RpJoinColumnName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpJoinColumnName");
            }
            this.RpJoinTextboxColumnName = GetConfigParameter.GetConfigValue("RpJoinTextboxColumnName");
            if (string.IsNullOrEmpty(this.RpJoinColumnName))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "RpJoinTextboxColumnName");
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

            this.CcLoopStart_JoinTables = GetConfigParameter.GetConfigValue("CcLoopStart_JoinTables");
            if (string.IsNullOrEmpty(this.CcLoopStart_JoinTables))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopStart_JoinTables");
            }

            this.CcLoopEnd_JoinTables = GetConfigParameter.GetConfigValue("CcLoopEnd_JoinTables");
            if (string.IsNullOrEmpty(this.CcLoopEnd_JoinTables))
            {
                throw new CheckException(this.RM_GetString("AppConfigParameterNotSet") + "CcLoopEnd_JoinTables");
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

        // ファイル ダイアログ周り
        #region ファイルの入出力先を指定

        /// <summary>D層定義情報ファイルへのパスを指定する。</summary>
        private void btnSetDaoDefinition_Click(object sender, EventArgs e)
        {
            // ロード先の設定
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル(*.csv)|*.csv";
            //ofd.Filter = this.RM_GetString("OpenFileDialogFilter");
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
            this.TableName_join = "";
            // D層定義情報ファイル
            StreamReader srDaoDef = null;

            // ↓↓↓【追加】↓↓↓
            // .NET型定義情報ファイル
            StreamReader srTypeDef = null;
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



                // ConditionalSearchテンプレート ファイル
                this.ConditionalSearchTemplateFilePath
                    = this.CheckTemplateFile(
                        this.txtSetSourceTemplate.Text,
                        this.JoinConditionalSearchTemplateFileName, "");

                // SearchAndUpdateテンプレート ファイル
                this.SearchAndUpdateTemplateFilePath
                    = this.CheckTemplateFile(
                        this.txtSetSourceTemplate.Text,
                        this.JoinSearchAndUpdateTemplateFileName, "");

                // Detailテンプレート ファイル
                this.DetailTemplateFilePath
                    = this.CheckTemplateFile(
                        this.txtSetSourceTemplate.Text,
                        this.JoinDetailTemplateFileName, "");
                // ↑↑↑【追加】↑↑↑

                #endregion
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
                this.strDAP = "";
                this.strDBMS = "";
                GetProviderInfo();

                #endregion

                #region  Read the DaoDefinitaion Files and get Primary key Information

                intTable = 0;
                PkList = new ArrayList();
                ElseList = new ArrayList();
                AllTableNames = new ArrayList();

                string strTableName = "";
                string strRlDaoDef = "";
                string[] strRlDaoDef2 = null;
                this.OpenSrDefInfo(out srDaoDef, out srTypeDef, out srDbTypeDef);

                if (this.cbxDaoDefinitionHeader.Checked)
                {
                    if (!srDaoDef.EndOfStream)
                    {
                        strRlDaoDef = srDaoDef.ReadLine();
                    }
                }
                while (!srDaoDef.EndOfStream)
                {
                    // for primary keys
                    strRlDaoDef = srDaoDef.ReadLine();
                    strRlDaoDef2 = strRlDaoDef.Split(',');
                    #region Get Tables Names
                    if (strRlDaoDef2[0] != "")
                    {
                        //Only Include the tables which are used in the Select Join statements of DpqueryTool
                        // If table name has space in it. Repalce with '_' operator
                        if (DPQTableNames.Contains(strRlDaoDef2[0].Replace(" ", "_")))
                        {
                            strTableName = strRlDaoDef2[0].Trim().Replace(" ", "_");
                            AllTableNames.Add(strRlDaoDef2[0].Trim().Replace(" ", "_"));

                        }
                        else
                        {
                            //This is to Exclude the Tables which are not used in Select Join statements of DpqueryTool but available in DaoDefinition .csv file.
                            strRlDaoDef = srDaoDef.ReadLine();
                            continue;
                        }
                    }
                    #endregion

                    #region Add only Primary key columns of only required tables
                    ArrayList colpkList = new ArrayList();
                    if (strRlDaoDef2.Length > 2)
                    {
                        for (int i = 1; i < strRlDaoDef2.Length; i++)
                        {
                            if (strRlDaoDef2[i] == "") { break; }
                            //Only Include the columns  which are used in the Select Join statements of DpqueryTool
                            if (DPQAllCoumns.Contains(strTableName + "." + strRlDaoDef2[i].Trim()))
                                colpkList.Add(strTableName + "." + strRlDaoDef2[i].Trim());
                            else if (DPQAllCoumns.Contains(strRlDaoDef2[i].Trim()))
                            {
                                colpkList.Add(strRlDaoDef2[i].Trim());
                            }
                        }
                        PkList.Add(colpkList);
                    }
                    else
                    {
                        MessageBox.Show(this.RM_GetString("PKnotAvailable") + strTableName);
                        return;
                    }

                    #endregion

                    #region Add only Else key columns of only required tables
                    // for Else keys
                    strRlDaoDef = srDaoDef.ReadLine();
                    strRlDaoDef2 = strRlDaoDef.Split(',');
                    ArrayList colelseList = new ArrayList();
                    if (strRlDaoDef2.Length >= 2)
                    {
                        for (int i = 1; i < strRlDaoDef2.Length; i++)
                        {
                            if (strRlDaoDef2[i] == "") { break; }
                            // Verify column name existance and replace space with '_' operator if column name has space in it
                            if (DPQAllCoumns.Contains(strTableName + "." + strRlDaoDef2[i].Trim().Replace(" ","_")))
                                colelseList.Add(strTableName + "." + strRlDaoDef2[i].Trim());
                            else if (DPQAllCoumns.Contains(strRlDaoDef2[i].Trim()))
                            {
                                colelseList.Add(strRlDaoDef2[i].Trim());
                            }
                        }
                    }
                    // To remove Timestamp column display in grid view  and  use TimeStampColName to set when 
                    if (colelseList.Contains(strTableName + "." + this.TimeStampColName))
                        colelseList.Remove(strTableName + "." + this.TimeStampColName);

                    ElseList.Add(colelseList);
                    #endregion
                }
                #endregion

                #region データがなくなるまでループ

                if (PkList.Count > 0)
                {
                    foreach (string tablename in AllTableNames)
                    {

                        this.TableName_join += tablename + "_";
                    }
                    this.TableName_join += "JOIN";

                    #region ASPXファイル、Codebehindファイル

                    // ASPXファイル、Codebehindファイル(_Screen_ConditionalSearch)を生成
                    this.FileName = this.TableName_join + this.JoinConditionalSearchTemplateFileName;
                    this.GenerateClassFile1(
                        this.ConditionalSearchTemplateFilePath,
                        (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                    this.GenerateClassFile1(
                      this.ConditionalSearchTemplateFilePath + "." + this.ClassTemplateFileExtension,
                      (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);

                    //// ASPXファイル、Codebehindファイル(_Screen_SearchAndUpdate)を生成
                    this.FileName = this.TableName_join + this.JoinSearchAndUpdateTemplateFileName;
                    this.GenerateClassFile1(
                        this.SearchAndUpdateTemplateFilePath,
                        (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                    this.GenerateClassFile1(
                        this.SearchAndUpdateTemplateFilePath + "." + this.ClassTemplateFileExtension,
                        (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);

                    // ASPXファイル、Codebehindファイル(Detail)を生成
                    this.FileName = this.TableName_join + this.JoinDetailTemplateFileName;
                    this.GenerateClassFile1(
                        this.DetailTemplateFilePath,
                        (int)this.cmbDTEncoding.SelectedValue, this.FileName);
                    this.GenerateClassFile1(
                        this.DetailTemplateFilePath + "." + this.ClassTemplateFileExtension,
                        (int)this.cmbDTEncoding.SelectedValue, this.FileName + "." + this.ClassTemplateFileExtension);
                    #endregion
                    MessageBox.Show(this.RM_GetString("ScreenGenerationSuccess"));
                }

                else
                {
                    MessageBox.Show(this.RM_GetString("PkSetMessgae"));
                }

                #endregion

                #endregion

                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;

            }
            catch (CheckException cex)
            {
                //MessageBox.Show("チェックエラーです：" + cex.Message);
                MessageBox.Show(string.Format(this.RM_GetString("CheckExceptionError"), cex.Message));

            }
            catch (Exception ex)
            {
                //MessageBox.Show("ランタイムエラーです：" + ex.Message);
                MessageBox.Show(string.Format(this.RM_GetString("RuntimeError"), ex.Message));
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

        #region Dao、Entity、DataSet、画面ASPX & クラス ファイル

        /// <summary>Generate Class file</summary>
        /// <param name="classTemplateFilePath">テンプレート</param>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="classFilePath">クラス ファイル</param>
        private void GenerateClassFile1(
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

            //JoinsTable Loop
            ArrayList joinsTableTemplate = null;
            bool isProcessed;
            bool PkElseProcess;

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
                    PkElseProcess = false;
                    pKColumnTemplate = null;
                    elseColumnTemplate = null;

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

                            // 処理済み
                            rlClassTpl = srClassTpl.ReadLine();
                            PkElseProcess = true;
                        }
                        if (isProcessed)
                        { }
                        else
                        {
                            if (rlClassTpl.IndexOf(this.CcLoopStart_ElseColumn) == -1)
                            { }
                            else
                            {
                                // 主キー以外の列の[何某]テンプレート
                                elseColumnTemplate = new ArrayList();
                                rlClassTpl = srClassTpl.ReadLine();
                                // 読んで（制御コメント内を読む）


                                // 制御コメント終了まで読む
                                while (rlClassTpl.IndexOf(this.CcLoopEnd_ElseColumn) == -1)
                                {
                                    // 追加
                                    elseColumnTemplate.Add(rlClassTpl);

                                    // 読んで・・・
                                    rlClassTpl = srClassTpl.ReadLine();
                                }
                                PkElseProcess = true;
                            }
                        }
                        if (PkElseProcess)
                        {
                            this.WriteElseColumn1(pKColumnTemplate, elseColumnTemplate, swClassFile);
                            isProcessed = true;
                        }

                        if (isProcessed)
                        {
                        }
                        else
                        {
                            if (rlClassTpl.IndexOf(this.CcLoopStart_JoinTables) == -1)
                            { }
                            else
                            {
                                joinsTableTemplate = new ArrayList();
                                rlClassTpl = srClassTpl.ReadLine();

                                while (rlClassTpl.IndexOf(this.CcLoopEnd_JoinTables) == -1)
                                {
                                    joinsTableTemplate.Add(rlClassTpl);

                                    rlClassTpl = srClassTpl.ReadLine();
                                }

                                this.WriteJoinsColumn(joinsTableTemplate, swClassFile);
                                isProcessed = true;
                            }
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

            input = input.Replace(this.RpJoinColumnName, this.JoinColumnName);

            input = input.Replace(this.RpJoinTextboxColumnName, this.JoinTextBoxColumnName);

            // To replace Correct column header Number 
            if (input.IndexOf(this.RpColumnNmbr) != -1)
            {
                input = input.Replace(this.RpColumnNmbr, ColumnNmbr.ToString());
                ColumnNmbr += 1;
            }

            return input;
        }
        // ↑↑↑【追加】↑↑↑

        /// <summary>WriteJoins Column for Table_loops</summary>
        /// <param name="template">Template of Tables in Join Select statements</param>
        /// <param name="sw">Stream Writer</param>
        private void WriteJoinsColumn(ArrayList template, StreamWriter sw)
        {
            ArrayList pKColumnTemplate = null;

            // 主キー以外の列の[何某]テンプレート
            ArrayList elseColumnTemplate = null;

            //JoinsTable Loop

            bool isProcessed;
            for (int Z = 0; Z < PkList.Count; Z++)
            {
                intTable = Z;
                if (template != null)
                {
                    this.TableName = AllTableNames[Z].ToString();

                    // 置換して
                    for (int i = 0; i < template.Count; i++)
                    {
                        isProcessed = false;
                        string rl = template[i].ToString();
                        if (rl.IndexOf(this.CcLoopStart_PKColumn) == -1)
                        { }
                        else
                        {
                            // 主キー列の[何某]テンプレート
                            pKColumnTemplate = new ArrayList();

                            i++;
                            // 読んで（制御コメント内を読む）
                            rl = template[i].ToString();

                            // 制御コメント終了まで読む
                            while (rl.IndexOf(this.CcLoopEnd_PKColumn) == -1)
                            {
                                // 追加
                                pKColumnTemplate.Add(rl);

                                // 読んで・・・
                                i++;
                                rl = template[i].ToString();
                            }

                            // 主キー列の[何某]生成メソッドを呼ぶ。
                            WritePKColumn(pKColumnTemplate, sw);

                            // 処理済み
                            rl = template[i].ToString();


                            isProcessed = true;
                        }
                        if (isProcessed)
                        { }
                        else
                        {
                            if (rl.IndexOf(this.CcLoopStart_ElseColumn) == -1)
                            { }
                            else
                            {
                                // 主キー以外の列の[何某]テンプレート
                                elseColumnTemplate = new ArrayList();
                                i++;

                                rl = template[i].ToString();
                                // 読んで（制御コメント内を読む）


                                // 制御コメント終了まで読む
                                while (rl.IndexOf(this.CcLoopEnd_ElseColumn) == -1)
                                {
                                    // 追加
                                    elseColumnTemplate.Add(rl);

                                    // 読んで・・・
                                    i++;
                                    rl = template[i].ToString();
                                }

                                WriteElseColumn(elseColumnTemplate, sw);
                                isProcessed = true;
                            }
                        }
                        if (isProcessed)
                        { }
                        else
                        {
                            sw.WriteLine(this.ReplaceClass(rl));
                        }

                    }
                }
            }
        }
        /// <summary>主キー以外の列の[何某]を生成する</summary>
        /// <param name="template">主キー以外の列の[何某]のテンプレート</param>
        /// <param name="sr">StreamWriter</param>
        private void WritePKColumn(ArrayList template, StreamWriter sw)
        {
            if (template != null)
            {
                this.TableName = AllTableNames[intTable].ToString();
                ArrayList Pktest = new ArrayList();
                Pktest = (ArrayList)PkList[intTable];
                for (int i = 0; i < Pktest.Count; i++)
                {
                    // 一応、ルールに従って・・・
                    //this.ColumnName = Pktest[i].ToString().Replace('.', '_');
                    if (Pktest[i].ToString().Split('.').Length > 1)
                    {
                        this.ColumnName = Pktest[i].ToString().Split('.')[1].Trim();
                        this.JoinColumnName = Pktest[i].ToString();
                        this.JoinTextBoxColumnName = Pktest[i].ToString().Replace('.', '_');
                    }
                    else
                    {
                        this.ColumnName = Pktest[i].ToString();
                        this.JoinColumnName = Pktest[i].ToString();
                        this.JoinTextBoxColumnName = this.TableName + "_" + Pktest[i].ToString();
                    }
                    foreach (string rl in template)
                    {
                        // カラム名の所は置換して書き込み。
                        sw.WriteLine(this.ReplaceNanigashi(rl));
                    }
                }
            }
        }

        /// <summary>主キー以外の列の[何某]を生成する</summary>
        /// <param name="template">ElseKey Template</param>
        /// <param name="sr">StreamWriter</param>
        private void WriteElseColumn(ArrayList template, StreamWriter sw)
        {
            if (template != null)
            {
                this.TableName = AllTableNames[intTable].ToString();
                ArrayList Elsetest = new ArrayList();
                Elsetest = (ArrayList)ElseList[intTable];
                for (int i = 0; i < Elsetest.Count; i++)
                {
                    // 一応、ルールに従って・・・
                    //this.ColumnName = Elsetest[i].ToString().Replace('.', '_');
                    if (Elsetest[i].ToString().Split('.').Length > 1)
                    {
                        this.ColumnName = Elsetest[i].ToString().Split('.')[1].Trim();
                        this.JoinColumnName = Elsetest[i].ToString().Replace(' ', '_');
                        // Replace space with '_' operator if column name has space in it
                        this.JoinTextBoxColumnName = Elsetest[i].ToString().Replace('.', '_').Replace(' ', '_');
                    }
                    else
                    {
                        this.ColumnName = Elsetest[i].ToString().Trim();
                        this.JoinColumnName = Elsetest[i].ToString();
                        this.JoinTextBoxColumnName = this.TableName + "_" + Elsetest[i].ToString().Trim();
                    }

                    foreach (string rl in template)
                    {
                        // カラム名の所は置換して書き込み。
                        sw.WriteLine(this.ReplaceNanigashi(rl));
                    }
                }
            }
        }

        /// <summary>This method is to loop for both primary keys and else keys of the table</summary>
        /// <param name="template">primaryKey Template</param>
        /// <param name="elseTemplate">ElseKey Template</param>
        /// <param name="sr">StreamWriter</param>
        private void WriteElseColumn1(ArrayList template, ArrayList elseTemplate, StreamWriter sw)
        {
            for (int Z = 0; Z < PkList.Count; Z++)
            {
                if (template != null)
                {
                    this.TableName = AllTableNames[Z].ToString();
                    ArrayList Pktest = new ArrayList();
                    Pktest = (ArrayList)PkList[Z];
                    for (int i = 0; i < Pktest.Count; i++)
                    {
                        // 一応、ルールに従って・・・
                        if (Pktest[i].ToString().Split('.').Length > 1)
                        {
                            this.ColumnName = Pktest[i].ToString().Split('.')[1].Trim();
                            this.JoinColumnName = Pktest[i].ToString();
                            this.JoinTextBoxColumnName = Pktest[i].ToString().Replace('.', '_');
                        }
                        else
                        {
                            this.ColumnName = Pktest[i].ToString();
                            this.JoinColumnName = Pktest[i].ToString();
                            this.JoinTextBoxColumnName = this.TableName + "_" + Pktest[i].ToString();
                        }
                        foreach (string rl in template)
                        {
                            // カラム名の所は置換して書き込み。
                            sw.WriteLine(this.ReplaceNanigashi(rl));
                        }
                    }
                }

                if (elseTemplate != null)
                {
                    this.TableName = AllTableNames[Z].ToString();
                    ArrayList Elsetest = new ArrayList();
                    Elsetest = (ArrayList)ElseList[Z];
                    for (int i = 0; i < Elsetest.Count; i++)
                    {
                        // 一応、ルールに従って・・・
                        if (Elsetest[i].ToString().Split('.').Length > 1)
                        {
                            this.ColumnName = Elsetest[i].ToString().Split('.')[1].Trim();
                            // Replace space with '_' operator if column name has space in it
                            this.JoinColumnName = Elsetest[i].ToString().Replace(' ', '_');
                            // Replace space with '_' operator if column name has space in it
                            this.JoinTextBoxColumnName = Elsetest[i].ToString().Replace('.', '_').Replace(' ','_');
                        }
                        else
                        {
                            this.ColumnName = Elsetest[i].ToString().Trim();
                            // Replace space with '_' operator if column name has space in it
                            this.JoinColumnName = Elsetest[i].ToString().Replace(' ', '_');
                            // Replace space with '_' operator if column name has space in it
                            this.JoinTextBoxColumnName = this.TableName + "_" + Elsetest[i].ToString().Trim().Replace(' ', '_');
                        }

                        foreach (string rl in elseTemplate)
                        {
                            // カラム名の所は置換して書き込み。
                            sw.WriteLine(this.ReplaceNanigashi(rl));
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region 文字列置換処理関数

        #region SQLファイル

        #region 置換処理（調整処理込み）


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
                    throw new CheckException(string.Format(this.RM_GetString("StringIndentation"),replaceString, input));
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
            input = input.Replace(this.RpJoinTableName, this.TableName_join);

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

        /// <summary>
        /// GetProviderInfo method sets the Data provider details for string to be replaced
        /// </summary>
        protected void GetProviderInfo()
        {
            if (this.rbnOLE.Checked)
            {
                this.strDBMS = "OLE";
                this.strDAP = "OLE";
            }
            if (this.rbnODB.Checked)
            {
                this.strDBMS = "ODB";
                this.strDAP = "ODB";
            }

            if (this.rbnODP.Checked)
            {
                this.strDBMS = "Oracle";
                this.strDAP = "ODP";
            }

            if (this.rbnDB2.Checked)
            {
                this.strDBMS = "DB2";
                this.strDAP = "DB2";
            }

            if (this.rbnMySQL.Checked)
            {
                this.strDBMS = "MySQL";
                this.strDAP = "MCN";
            }

            if (this.rbnPstgrs.Checked)
            {
                this.strDBMS = "PstGrS";
                this.strDAP = "NPS";
            }

            if (this.rbnSQL.Checked)
            {
                this.strDBMS = "SQLServer";
                this.strDAP = "SQL";
            }


        }

        #endregion

    }
}