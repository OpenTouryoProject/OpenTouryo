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
//* クラス名        ：CmnDao
//* クラス日本語名  ：共通Daoクラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/11/02  西野 大介         GetParameterメソッドを追加（ｽﾄｱﾄﾞ ﾕｰｻﾞﾋﾞﾘﾃｨ向上）
//*  2010/11/02  西野 大介         その他、リファクタリングなど（メソッド名、修飾子の変更）
//*                                特にprotected → public化の「new & base」に注意！
//*                                （ミスると再帰呼び出しの無限ループになる...疎通で確認可）
//*  2011/10/09  西野 大介         国際化対応
//*  2012/06/14  西野 大介         ResourceLoaderに加え、EmbeddedResourceLoaderに対応
//*  2013/07/07  西野 大介         ExecGenerateSQL（SQL生成）メソッド（実行しない）を追加
//*  2014/11/20  Sandeep           Implemented CommandTimeout property and SetCommandTimeout method.
//*  2014/11/20  Sai               removed IDbCommand property in SetCommandTimeout method.
//*  2018/08/07  西野 大介         ストアド実行のためCommandType.StoredProcedureを設定可能に。
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Dao
{
    /// <summary>共通Daoクラス</summary>
    /// <remarks>自由に（拡張して）利用できる。</remarks>
    public class CmnDao : MyBaseDao
    {
        #region インスタンス変数

        /// <summary>CommandType</summary>
        private CommandType? _cmdType = null;

        #region パラメタ

        /// <summary>ユーザ パラメタ（文字列置換）用ディクショナリ</summary>
        private Dictionary<string, string> DicUserParameter = new Dictionary<string, string>();

        /// <summary>パラメタ ライズド クエリのパラメタ用ディクショナリ</summary>
        private Dictionary<string, object> DicParameter = new Dictionary<string, object>();

        #region 追加のDictionary
        /// <summary>パラメタ ライズド クエリの指定されたパラメータ（の型）を保持するディクショナリ</summary>
        private Dictionary<string, object> DicParameterType = new Dictionary<string, object>();
        /// <summary>パラメタ ライズド クエリの指定されたパラメータ（のサイズ）を保持するディクショナリ</summary>
        private Dictionary<string, int> DicParameterSize = new Dictionary<string, int>();
        /// <summary>パラメタ ライズド クエリの指定されたパラメータ（の方向）を保持するディクショナリ</summary>
        private Dictionary<string, ParameterDirection> DicParameterDirection = new Dictionary<string, ParameterDirection>();
        #endregion

        #endregion

        #region パラメタの制御

        /// <summary>パラメタライズドクエリのパラメタを取得する（Out,RetValパラメタ用）。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <returns>Out,RetValパラメタのバリュー</returns>
        /// <remarks>
        /// 動的SQLの場合はSQL実行後に利用可能
        /// </remarks>
        public new object GetParameter(string parameterName)
        {
            // ★ ベースのメソッドを呼ぶ
            return base.GetParameter(parameterName);

        }

        /// <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="obj">パラメタ値</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void SetParameter(string parameterName, object obj)
        {
            // ユーザ パラメタをディクショナリに設定
            this.DicParameter[parameterName] = obj;
        }

        /// <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="obj">パラメタ値</param>
        /// <param name="dbTypeInfo">パラメタの型</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void SetParameter(string parameterName, object obj, object dbTypeInfo)
        {
            // ユーザ パラメタをディクショナリに設定
            this.DicParameter[parameterName] = obj;

            // 機能改善
            this.DicParameterType[parameterName] = dbTypeInfo;
        }

        /// <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="obj">パラメタ値</param>
        /// <param name="dbTypeInfo">パラメタの型</param>
        /// <param name="size">パラメタのサイズ</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void SetParameter(string parameterName, object obj, object dbTypeInfo, int size)
        {
            // ユーザ パラメタをディクショナリに設定
            this.DicParameter[parameterName] = obj;

            // 機能改善
            this.DicParameterType[parameterName] = dbTypeInfo;
            this.DicParameterSize[parameterName] = size;
        }

        /// <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
        /// <param name="parameterName">パラメタ名</param>
        /// <param name="obj">パラメタ値</param>
        /// <param name="dbTypeInfo">パラメタの型</param>
        /// <param name="size">パラメタのサイズ</param>
        /// <param name="paramDirection">パラメタの方向</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void SetParameter(string parameterName, object obj,
            object dbTypeInfo, int size, ParameterDirection paramDirection)
        {
            // ユーザ パラメタをディクショナリに設定
            this.DicParameter[parameterName] = obj;

            // 機能改善
            this.DicParameterType[parameterName] = dbTypeInfo;
            this.DicParameterSize[parameterName] = size;
            this.DicParameterDirection[parameterName] = paramDirection;
        }

        /// <summary>ユーザ パラメタ（文字列置換）をディクショナリに設定する。</summary>
        /// <param name="userParamName">ユーザ パラメタ名</param>
        /// <param name="userParamValue">ユーザ パラメタ値</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void SetUserParameter(string userParamName, string userParamValue)
        {
            // ユーザ パラメタをディクショナリに設定
            this.DicUserParameter[userParamName] = userParamValue;
        }

        /// <summary>
        /// ・ユーザ パラメタ（文字列置換）
        /// ・パラメタ ライズド クエリのパラメタ
        /// を格納するディクショナリをクリアする。
        /// </summary>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public void ClearParameters()
        {
            // ユーザ パラメタ（文字列置換）用ディクショナリを初期化
            this.DicUserParameter = new Dictionary<string, string>();
            // パラメタ ライズド クエリのパラメタ用ディクショナリを初期化
            this.DicParameter = new Dictionary<string, object>();

            // 同上
            this.DicParameterType = new Dictionary<string, object>();
            this.DicParameterSize = new Dictionary<string, int>();
            this.DicParameterDirection = new Dictionary<string, ParameterDirection>();
        }

        #endregion

        #region SQLファイル名

        /// <summary>SQLファイル名</summary>
        private string _sQLFileName = "";

        #region プロパティ プロシージャ

        /// <summary>SQLファイル名</summary>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public string SQLFileName
        {
            set
            {
                this._sQLFileName = value;
                this._sQLText = "";
            }
        }

        #endregion

        #endregion

        #region SQLテキスト

        /// <summary>SQLテキスト</summary>
        private string _sQLText = "";

        #region プロパティ プロシージャ

        /// <summary>SQLテキスト</summary>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public string SQLText
        {
            set
            {
                this._sQLText = value;
                this._sQLFileName = "";
            }
        }

        #endregion

        #endregion

        #region CommandTimeout

        /// <summary>CommandTimeout</summary>
        private int _commandTimeout = -1;

        #region プロパティ プロシージャ

        /// <summary>CommandTimeout</summary>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public int CommandTimeout
        {
            set
            {
                this._commandTimeout = value;
            }
        }

        #endregion

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public CmnDao(BaseDam dam) : base(dam) { }

        /// <summary>コンストラクタ</summary>
        /// <remarks>自由に利用できる。</remarks>
        public CmnDao(BaseDam dam, CommandType cmdType) : base(dam)
        {
            this._cmdType = cmdType;
        }

        #endregion

        #region クエリ メソッド

        #region Exec(new & base)

        /// <summary>Command.ExecuteScalarメソッドでデータを取得する。</summary>
        /// <returns>データ</returns>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new object ExecSelectScalar()
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // SQLを実行し、データを戻す。
            // （★ ベースのメソッドを呼ぶ）
            return base.ExecSelectScalar();
        }

        /// <summary>DataAdapter.Fill(DataTable)メソッドでデータを取得する。</summary>
        /// <param name="dt">結果セット（データ テーブル）</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void ExecSelectFill_DT(DataTable dt)
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // SQLを実行し、結果セット（データ テーブル）を戻す。
            // （★ ベースのメソッドを呼ぶ）
            base.ExecSelectFill_DT(dt);
        }

        /// <summary>DataAdapter.Fill(DataSet)メソッドでデータを取得する。</summary>
        /// <param name="ds">結果セット（データ セット）</param>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new void ExecSelectFill_DS(DataSet ds)
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // SQLを実行し、結果セット（データ セット）を戻す。
            // （★ ベースのメソッドを呼ぶ）
            base.ExecSelectFill_DS(ds);
        }

        /// <summary>Command.ExecuteReaderメソッドでデータを取得する。</summary>
        /// <returns>結果セット（データ リーダ）</returns>
        /// <remarks>自由に（拡張して）利用できる。</remarks>        
        public new IDataReader ExecSelect_DR()
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // SQLを実行し、結果セット（データ リーダ）を戻す。
            // （★ ベースのメソッドを呼ぶ）
            return base.ExecSelect_DR();
        }

        /// <summary>Command.ExecuteNonQueryメソッドでSQLを実行する。</summary>
        /// <returns>影響を受けた行の数</returns>
        /// <remarks>自由に（拡張して）利用できる。</remarks>        
        public new int ExecInsUpDel_NonQuery()
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // SQLを実行し、戻り値を戻す。
            // （★ ベースのメソッドを呼ぶ）
            return base.ExecInsUpDel_NonQuery();
        }

        /// <summary>ExecGenerateSQLで静的SQLを生成する</summary>
        /// <param name="sqlUtil">SQLUtility</param>
        /// <returns>SQL文</returns>
        /// <remarks>自由に（拡張して）利用できる。</remarks>
        public new string ExecGenerateSQL(SQLUtility sqlUtil)
        {
            // SQLの設定
            this.SetSQL();

            // Set CommandTimeout
            this.SetCommandTimeout();

            // パラメタの一括設定
            this.SetParameters();

            // 静的SQLを生成する。
            // （★ ベースのメソッドを呼ぶ）
            return base.ExecGenerateSQL(sqlUtil);
        }

        #endregion

        #region 共通関数

        /// <summary>SQLの指定</summary>
        private void SetSQL()
        {
            // SQL指定
            if (this._sQLFileName != "")
            {
                // ファイルから
                if (this._cmdType.HasValue)
                {
                    this.SetSqlByFile2(this._sQLFileName, this._cmdType.Value); 
                }
                else
                {
                    this.SetSqlByFile2(this._sQLFileName);
                }
            }
            else if (this._sQLText != "")
            {
                // テキストから
                if (this._cmdType.HasValue)
                {
                    this.SetSqlByCommand(this._sQLText, this._cmdType.Value);
                }
                else
                {
                    this.SetSqlByCommand(this._sQLText);
                }
                
            }
            else
            {
                // SQLエラー
                throw new BusinessSystemException(
                    MyBusinessSystemExceptionMessage.CMN_DAO_ERROR[0],
                    String.Format(MyBusinessSystemExceptionMessage.CMN_DAO_ERROR[1],
                        MyBusinessSystemExceptionMessage.CMN_DAO_ERROR_SQL));
            }
        }

        /// <summary>To Set CommandTimeout</summary>
        private void SetCommandTimeout()
        {
            // If CommandTimeout is >= 0 then set CommandTimeout.
            // Else skip, automatically it will set default CommandTimeout.
            if (this._commandTimeout >= 0)
            {
                this.GetDam().DamIDbCommand.CommandTimeout = this._commandTimeout;
            }
        }

        /// <summary>パラメタの一括設定（内部用）</summary>
        private void SetParameters()
        {
            // ユーザ パラメタ（文字列置換）を設定する。
            foreach (string userParamName in this.DicUserParameter.Keys)
            {
                // ★ ベースのメソッドを呼ぶ
                base.SetUserParameter(userParamName, this.DicUserParameter[userParamName].ToString());
            }

            // パラメタ ライズド クエリのパラメタを設定する。
            foreach (string paramName in this.DicParameter.Keys)
            {
                // 機能改善

                // デフォルト値
                object type = null;
                int size = -1;
                ParameterDirection direction = ParameterDirection.Input;

                // あったら設定（DicParameterType）
                if (this.DicParameterType.ContainsKey(paramName))
                {
                    type = this.DicParameterType[paramName];
                }

                // あったら設定（DicParameterSize）
                if (this.DicParameterSize.ContainsKey(paramName))
                {
                    size = this.DicParameterSize[paramName];
                }

                // あったら設定（DicParameterDirection）
                if (this.DicParameterDirection.ContainsKey(paramName))
                {
                    direction = this.DicParameterDirection[paramName];
                }

                // ★ ベースのメソッドを呼ぶ
                base.SetParameter(paramName, this.DicParameter[paramName], type, size, direction);
            }
        }

        #endregion

        #endregion
    }
}
