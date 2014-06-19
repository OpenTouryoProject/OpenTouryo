//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：BaseDao
//* クラス日本語名  ：データアクセス親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/03/19  西野  大介        DRのインターフェイスをobject→IDataReaderへ変更。
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#18 ： 例外の振り替え処理は不要。
//*                                ・#x  ： Warning落とし。
//*  2009/11/26  西野  大介        SetParameterメソッドの型指定有りオーバーロードの追加
//*  2010/03/03  西野  大介        SetParameterメソッドのその他オーバーロードの追加
//*  2010/11/02  西野  大介        GetParameterメソッドを追加（ｽﾄｱﾄﾞ ﾕｰｻﾞﾋﾞﾘﾃｨ向上）
//*  2012/08/23  西野  大介        ログ出力情報の追加依頼へ対応（情報取得処理追加）。
//*  2013/05/28  西野  大介        SetParameterオーバライドでPJ毎のカスタム型推論を実装可能に。
//*  2013/07/07  西野  大介        ExecGenerateSQL（SQL生成）メソッド（実行しない）を追加
//**********************************************************************************

// System
using System;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Dao
{
    /// <summary>データアクセス親クラス１</summary>
    /// <remarks>データアクセス親クラス２、データアクセス クラス、業務コード クラスから利用する。</remarks>
    public class BaseDao
    {
        #region インスタンス変数

        /// <summary>データアクセス制御クラス</summary>
        private BaseDam _dam;

        /// <summary>ログ出力情報</summary>
        protected object LogInfo = null;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="dam">データアクセスオブジェクト</param>
        /// <remarks>業務コード クラスから利用する。</remarks>
        public BaseDao(BaseDam dam)
        {
            this._dam = dam;
        }

        #endregion

        #region メソッド（連結）

        #region その他

        /// <summary>
        /// データアクセスオブジェクトを返す。
        /// </summary>
        /// <returns>データアクセスオブジェクト</returns>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected BaseDam GetDam()
        {
            return this._dam;
        }

        #endregion

        #region SQLの作成

        #region SetSqlBy

        /// <summary>SQL文を記述したファイルへのパスを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetSqlByFile(string sqlFilePath)
        {
            this._dam.SetSqlByFile(sqlFilePath);
        }

        /// <summary>SQL文を記述したファイルへのパスとCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetSqlByFile(string sqlFilePath, CommandType commandType)
        {
            this._dam.SetSqlByFile(sqlFilePath, commandType);
        }

        /// <summary>SQL文を設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetSqlByCommand(string commandText)
        {
            this._dam.SetSqlByCommand(commandText);
        }

        /// <summary>SQL文とCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetSqlByCommand(string commandText, CommandType commandType)
        {
            this._dam.SetSqlByCommand(commandText, commandType);
        }

        #endregion

        #region SetParameter

        /// <summary>パラメタライズドクエリのパラメタを取得する（Out,RetValパラメタ用）。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <returns>Out,RetValパラメタのバリュー</returns>
        /// <remarks>
        /// 派生のデータアクセス クラスから利用する。
        /// 動的SQLの場合はSQL実行後に利用可能
        /// </remarks>
        protected object GetParameter(string parameterName)
        {
            return this._dam.GetParameter(parameterName);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected virtual void SetParameter(string parameterName, object obj)
        {
            this._dam.SetParameter(parameterName, obj);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（データプロバイダ固有）</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetParameter(string parameterName, object obj, object dbTypeInfo)
        {
            this._dam.SetParameter(parameterName, obj, dbTypeInfo);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（データプロバイダ固有）</param>
        /// <param name="size">パラメタのサイズ（設定しない場合は、-1を指定）</param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetParameter(string parameterName, object obj, object dbTypeInfo, int size)
        {
            this._dam.SetParameter(parameterName, obj, dbTypeInfo, size);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（データプロバイダ固有）</param>
        /// <param name="size">パラメタのサイズ（設定しない場合は、-1を指定）</param>
        /// <param name="paramDirection"></param>
        /// <remarks>派生のデータアクセス クラスから利用する。</remarks>
        protected void SetParameter(string parameterName, object obj,
            object dbTypeInfo, int size, ParameterDirection paramDirection)
        {
            this._dam.SetParameter(parameterName, obj, dbTypeInfo, size, paramDirection);
        }

        #endregion

        /// <summary>ユーザパラメタを指定の文字列で置換する。</summary>
        /// <param name="userParameterName">置換対象のユーザパラメタ名</param>
        /// <param name="userParameterValue">置換の文字列</param>
        /// <remarks>
        /// SQLインジェクションされる可能性があるユーザ入力は「userParameterValue」に指定しないこと。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>        
        protected void SetUserParameter(string userParameterName, string userParameterValue)
        {
            this._dam.SetUserParameter(userParameterName, userParameterValue);
        }

        #endregion

        #region SQLの実行

        /// <summary>Selectクエリを実行し、データテーブルを返す。</summary>
        /// <param name="dt">データテーブル</param>
        /// <remarks>
        /// DataAdapterのFillを実行する。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>
        protected void ExecSelectFill_DT(DataTable dt)
        {
            // SQL実行開始処理を実装する共通UOCメソッド
            this.UOC_PreQuery();

            try
            {
                // SQL実行
                this._dam.ExecSelectFill_DT(dt);

                // ログ情報の設定
                this.LogInfo = dt;

                // SQL実行終了処理を実装する共通UOCメソッド（正常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog());
            }
            catch(Exception ex)
            {
                // SQL実行終了処理を実装する共通UOCメソッド（異常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog(), ex);

                // #18,29-リスローに戻す
                throw;
            }
            finally
            {
                // ログ情報のクリア
                this.LogInfo = null;
            }
        }

        /// <summary>Selectクエリを実行し、データセットを返す。</summary>
        /// <param name="ds">データセット</param>
        /// <remarks>
        /// DataAdapterのFillを実行する。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>
        protected void ExecSelectFill_DS(DataSet ds)
        {
            // SQL実行開始処理を実装する共通UOCメソッド
            this.UOC_PreQuery();

            try
            {
                // SQL実行
                this._dam.ExecSelectFill_DS(ds);

                // ログ情報の設定
                this.LogInfo = ds;

                // SQL実行終了処理を実装する共通UOCメソッド（正常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog());
            }
            catch (Exception ex)
            {
                // SQL実行終了処理を実装する共通UOCメソッド（異常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog(), ex);

                // #18,29-リスローに戻す
                throw;
            }
            finally
            {
                // ログ情報のクリア
                this.LogInfo = null;
            }
        }

        /// <summary>Selectクエリを実行し、データリーダを返す。</summary>
        /// <returns>
        /// CommandのExecuteScalarを実行する。
        /// 派生のデータアクセス クラスから利用する。
        /// </returns>
        protected IDataReader ExecSelect_DR()
        {
            // SQL実行開始処理を実装する共通UOCメソッド
            this.UOC_PreQuery();

            object dr = null; // #x-この行

            try
            {
                // SQL実行
                dr = this._dam.ExecSelect_DR();

                // ログ情報の設定
                this.LogInfo = dr;

                // SQL実行終了処理を実装する共通UOCメソッド（正常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog());
            }
            catch (Exception ex)
            {
                // SQL実行終了処理を実装する共通UOCメソッド（異常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog(), ex);

                // #18,29-リスローに戻す
                throw;
            }
            finally
            {
                // ログ情報のクリア
                this.LogInfo = null;
            }

            // データリーダを返す。
            return (IDataReader)dr;
        }

        /// <summary>Selectクエリを実行し、結果セットの最初の行の最初の列を返す。</summary>
        /// <returns>結果セットの最初の行の最初の列（オブジェクト型） </returns>
        /// <remarks>
        /// SqlCommandのExecuteScalarを実行する。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>
        protected object ExecSelectScalar()
        {
            // SQL実行開始処理を実装する共通UOCメソッド
            this.UOC_PreQuery();

            object obj = null; // #x-この行

            try
            {
                // SQL実行
                obj = this._dam.ExecSelectScalar();

                // ログ情報の設定
                this.LogInfo = obj;

                // SQL実行終了処理を実装する共通UOCメソッド（正常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog());
            }
            catch (Exception ex)
            {
                // SQL実行終了処理を実装する共通UOCメソッド（異常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog(), ex);

                // #18,29-リスローに戻す
                throw;
            }
            finally
            {
                // ログ情報のクリア
                this.LogInfo = null;
            }

            // オブジェクトを返す。
            return obj;
        }

        /// <summary>Insert、Update、Deleteクエリを実行し、影響を受けた行数を返す。</summary>
        /// <returns>影響を受けた行数</returns>
        /// <remarks>
        /// SqlCommandのExecuteNonQueryを実行する。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>
        protected int ExecInsUpDel_NonQuery()
        {
            // SQL実行開始処理を実装する共通UOCメソッド
            this.UOC_PreQuery();

            int i = 0; // #x-この行

            try
            {
                // SQL実行
                i = this._dam.ExecInsUpDel_NonQuery();

                // ログ情報の設定
                this.LogInfo = i;

                // SQL実行終了処理を実装する共通UOCメソッド（正常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog());
            }
            catch (Exception ex)
            {
                // SQL実行終了処理を実装する共通UOCメソッド（異常終了）
                this.UOC_AfterQuery(this._dam.GetCurrentQueryForLog(), ex);

                // #18,29-リスローに戻す
                throw;
            }
            finally
            {
                // ログ情報のクリア
                this.LogInfo = null;
            }

            // 影響を受けた行数を返す。
            return i;
        }

        /// <summary>静的SQLを生成する</summary>
        /// <param name="sqlUtil">SQLUtility</param>
        /// <returns>SQL文</returns>
        /// <remarks>
        /// Commandでの実行はしない。
        /// 派生のデータアクセス クラスから利用する。
        /// </remarks>
        protected string ExecGenerateSQL(SQLUtility sqlUtil)
        {
            // 委譲（UOC_PreQuery、UOC_AfterQueryは実行しない）
            return this._dam.ExecGenerateSQL(sqlUtil);
        }

        #endregion

        #endregion

        #region ＵＯＣメソッド
        
        /// <summary>SQL実行開始処理を実装する共通UOCメソッド</summary>
        /// <remarks>派生のデータアクセス親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_PreQuery() { }

        /// <summary>SQL実行終了処理を実装する共通UOCメソッド（正常時）</summary>
        /// <param name="sql">実行したSQLの情報</param>
        /// <remarks>派生のデータアクセス親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterQuery(string sql) { }

        /// <summary>SQL実行終了処理を実装する共通UOCメソッド（異常時）</summary>
        /// <param name="sql">実行したSQLの情報</param>
        /// <param name="ex">エラー情報</param>
        /// <remarks>派生のデータアクセス親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterQuery(string sql, Exception ex) { }

        #endregion
    }
}
