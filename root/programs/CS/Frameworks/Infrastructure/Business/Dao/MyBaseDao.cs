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
//* クラス名        ：MyBaseDao
//* クラス日本語名  ：データアクセス親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/04/21  西野 大介         FrameworkExceptionの追加に伴い、実装変更
//*  2010/09/24  西野 大介         Damクラス内にユーザ情報を格納したので
//*  2012/06/14  西野 大介         SetSqlByFile2を追加（SetSqlByFile強化版）
//*                                ・sqlTextFilePathを自動連結
//*                                ・EmbeddedResourceLoaderに対応
//*  2018/08/07  西野 大介         CommandType.StoredProcedureを設定可能に。
//**********************************************************************************

using System;
using System.IO;
using System.Data;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Dao
{
    /// <summary>データアクセス親クラス２（テンプレート）</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    public abstract class MyBaseDao : BaseDao
    {
        /// <summary>埋め込まれたリソースを使用する</summary>
        public static bool UseEmbeddedResource = false;

        /// <summary>SetSqlByFileの強化版メソッド</summary>
        /// <param name="sQLFileName">ファイル名</param>
        public void SetSqlByFile2(string sQLFileName)
        {
            this.SetSqlByFile2(sQLFileName, null);
        }

        /// <summary>SetSqlByFileの強化版メソッド</summary>
        /// <param name="sQLFileName">ファイル名</param>
        /// <param name="cmdType">CommandType</param>
        public void SetSqlByFile2(string sQLFileName, CommandType? cmdType)
        {
            // SQLを設定する。
            if (MyBaseDao.UseEmbeddedResource)
            {
                // 埋め込まれたリソースファイル
                if (cmdType.HasValue)
                {
                    this.SetSqlByFile(
                        GetConfigParameter.GetConfigValue("sqlTextFilePath") + "." + sQLFileName, cmdType.Value);
                }
                else
                {
                    this.SetSqlByFile(
                        GetConfigParameter.GetConfigValue("sqlTextFilePath") + "." + sQLFileName);
                }
            }
            else
            {
                // 通常のファイル
                if (cmdType.HasValue)
                {
                    this.SetSqlByFile(
                        Path.Combine(
                            GetConfigParameter.GetConfigValue("sqlTextFilePath"), sQLFileName), cmdType.Value);
                }
                else
                {
                    this.SetSqlByFile(
                        Path.Combine(
                            GetConfigParameter.GetConfigValue("sqlTextFilePath"), sQLFileName));
                }   
            }
        }

        /// <summary>
        /// 性能測定
        /// </summary>
        private PerformanceRecorder perfRec;

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>
        /// コンストラクタは継承されないので、派生先で呼び出す必要がある。
        /// コンストラクタの実行順は、基本クラス→派生クラスの順
        /// ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
        /// 自由に利用できる。
        /// </remarks>
        public MyBaseDao(BaseDam dam):base(dam) { }

        #endregion

        #region 開始・終了処理

        /// <summary>SQL実行開始処理を実装する共通UOCメソッド</summary>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_PreQuery()
        {
            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        /// <summary>SQL実行終了処理を実装する共通UOCメソッド（正常時）</summary>
        /// <param name="sql">実行したSQLの情報</param>
        /// <remarks>データ アクセス親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterQuery(string sql)
        {
            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // SQLトレースログ出力

            // ------------
            // メッセージ部
            // ------------
            // 処理時間（実行時間）, 処理時間（CPU時間）, 実行したSQLの情報
            // ------------
            string strLogMessage =
                this.perfRec.ExecTime + "," + this.perfRec.CpuTime + "," + sql;

            // Log4Netへログ出力
            if (string.IsNullOrEmpty(GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG)))
            {
                // SQLトレースログ（OFF）
            }
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.ON)
            {
                LogIF.InfoLog("SQLTRACE", strLogMessage);
            }
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.OFF)
            {
                // SQLトレースログ（OFF）
            }
            else
            {
                // パラメータ・エラー（書式不正）
                throw new ArgumentException(
                    String.Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_TRACELOG));
            }

            // ---

            // 以下も、ログ出力で使用可能
            object obj = null;
            
            // UOC_Connection等で情報を設定しておく。
            // UserInfoなどの情報を想定している。
            obj = this.GetDam().Obj;

            // SQL実行時に情報が自動設定される。
            // ・ExecSelectFill_DT
            //   DataTable
            // ・ExecSelectFill_DS
            //   DataSet
            // ・ExecSelect_DR
            //   IDataReader
            // ・ExecSelectScalar
            //   object
            // ・ExecInsUpDel_NonQuery
            //   int
            obj = this.LogInfo;
        }

        /// <summary>SQL実行終了処理を実装する共通UOCメソッド（異常時）</summary>
        /// <param name="sql">実行したSQLの情報</param>
        /// <param name="ex">エラー情報</param>
        /// <remarks>データ アクセス親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterQuery(string sql, Exception ex)
        {
            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // SQLトレースログ出力

            // ------------
            // メッセージ部
            // ------------
            // 処理時間（実行時間）, 処理時間（CPU時間）, ユーザ名, 実行したSQLの情報
            // ------------
            string strLogMessage
                = this.perfRec.ExecTime + ","
                + this.perfRec.CpuTime + ","
                + ((MyUserInfo)(this.GetDam().Obj)).UserName + ","
                + sql;

            // Log4Netへログ出力
            if (string.IsNullOrEmpty(GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG)))
            {
                // SQLトレースログ（OFF）
            }
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.ON)
            {
                LogIF.ErrorLog("SQLTRACE", strLogMessage);
            }
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.OFF)
            {
                // SQLトレースログ（OFF）
            }
            else
            {
                // パラメータ・エラー（書式不正）
                throw new ArgumentException(
                    String.Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_TRACELOG));
            }

            // ---

            // 以下も、ログ出力で使用可能
            object obj = null;

            // UOC_Connection等で情報を設定しておく。
            // UserInfoなどの情報を想定している。
            obj = this.GetDam().Obj;

            // SQL実行時に情報が自動設定される。
            // ・ExecSelectFill_DT
            //   DataTable
            // ・ExecSelectFill_DS
            //   DataSet
            // ・ExecSelect_DR
            //   IDataReader
            // ・ExecSelectScalar
            //   object
            // ・ExecInsUpDel_NonQuery
            //   int
            obj = this.LogInfo;
        }

        #endregion
    }
}
