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
//* クラス名        ：DamOLEDB
//* クラス日本語名  ：データアクセス・プロバイダ＝OLEDBのデータアクセス制御クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/02/09  西野 大介         新規作成
//*  2012/03/16  西野 大介         ClearTextの所の仕様変更（文字列中は、空白・タブを詰めない）。
//*  2012/03/21  西野 大介         SQLの型指定（.net型）対応
//*  2013/02/15  加藤 幸紀         ChangesToOrderBind()修正。（順番バインドのパラメタ置換処理方式の見直し）
//*  2013/07/07  西野 大介         ExecGenerateSQL（SQL生成）メソッド（実行しない）を追加
//*  2013/07/09  西野 大介         静的SQLでもユーザパラメタを保存（操作ログで使用する用途）
//*  2015/07/05  Sai               Implemented virtual property of IDbCommand in DamOLEDB class
//*  2017/09/06  西野 大介         IN句展開、ArrayListに加えて、List<T>のサポートを追加
//**********************************************************************************

using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>データアクセス・プロバイダ＝OleDbのデータアクセス制御クラス</summary>
    /// <remarks>必要なメソッド・プロパティを利用する</remarks>
    public class DamOLEDB : BaseDam
    {
        #region クラス変数

        /// <summary>パラメタの先頭記号（OLEDBは順番バインド（？）のみなので本部品で対応（@P1））</summary>
        private readonly static char _paramSign = '@';

        #endregion

        #region インスタンス変数

        /// <summary>コネクション</summary>
        private OleDbConnection _cnn;

        /// <summary>トランザクション</summary>
        private OleDbTransaction _tx;

        /// <summary>コマンド</summary>
        private OleDbCommand _cmd;

        /// <summary>アダプタ</summary>
        private OleDbDataAdapter _adpt;

        /// <summary>分離レベル</summary>
        private DbEnum.IsolationLevelEnum _iso;

        #endregion

        #region プロパティ

        /// <summary>OleDbConnection（読み取り専用）</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public OleDbConnection DamOleDbConnection
        {
            get
            {
                // コネクションを戻す
                return _cnn;
            }
        }

        /// <summary>OleDbDataAdapter（読み取り専用）</summary>
        /// <remarks>
        /// 利用時、コネクション、トランザクションが有効な状態になっている事
        /// 必要に応じて利用する。
        /// </remarks>
        public OleDbDataAdapter DamOleDbDataAdapter
        {
            get
            {
                // コネクション、トランザクションを設定
                this._cmd.Connection = this._cnn;
                this._cmd.Transaction = this._tx;

                // SelectCommandからデータアダプタを生成
                this._adpt = new OleDbDataAdapter(this._cmd);

                // アダプタを戻す
                return _adpt;
            }
        }

        /// <summary>OleDbCommand（読み取り専用）</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public OleDbCommand DamOleDbCommand
        {
            get
            {
                // コマンドを戻す
                return _cmd;
            }
        }

        #region IDbCommand

        /// <summary>
        /// Property for IDbCommand to support multiple db
        /// </summary>
        public override IDbCommand DamIDbCommand
        {
            get
            {
                return (IDbCommand)this._cmd;
            }
        }

        #endregion

        /// <summary>OleDbTransaction（読み取り専用）</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public OleDbTransaction DamOleDbTransaction
        {
            get
            {
                // トランザクションを戻す
                return _tx;
            }
        }

        #endregion

        #region メソッド

        #region コネクション

        /// <summary>コネクションの確立</summary>
        /// <param name="connstring">接続文字列</param>
        /// <remarks>必要に応じて利用する。</remarks>
        public override void ConnectionOpen(string connstring)
        {
            // コネクションをオープン
            this._cnn = new OleDbConnection(connstring);
            this._cnn.Open();
        }

        /// <summary>コネクションの切断</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public override void ConnectionClose()
        {
            if (this._cnn == null)
            {
                // 参照が無い
            }
            else
            {
                // 参照がある
                this._cnn.Close();
            }
        }

        #endregion

        #region トランザクション

        #region 開始

        /// <summary>トランザクション開始</summary>
        /// <param name="iso">分離レベル（内部プロバイダによるので全てサポート）</param>
        public override void BeginTransaction(DbEnum.IsolationLevelEnum iso)
        {
            // 分離レベル設定のチェック
            if (iso == DbEnum.IsolationLevelEnum.NoTransaction)
            {
                // トランザクションを開始しない（nullのまま）。
            }
            else if (iso == DbEnum.IsolationLevelEnum.DefaultTransaction)
            {
                // 規定の分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction();
            }
            else if (iso == DbEnum.IsolationLevelEnum.ReadUncommitted)
            {
                // 非コミット読み取りの分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction(IsolationLevel.ReadUncommitted);
            }
            else if (iso == DbEnum.IsolationLevelEnum.ReadCommitted)
            {
                // コミット済み読み取りの分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            else if (iso == DbEnum.IsolationLevelEnum.RepeatableRead)
            {
                // 反復可能読み取りの分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction(IsolationLevel.RepeatableRead);
            }
            else if (iso == DbEnum.IsolationLevelEnum.Serializable)
            {
                // 直列化可能の分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction(IsolationLevel.Serializable);
            }
            else if (iso == DbEnum.IsolationLevelEnum.Snapshot)
            {
                // スナップショット分離レベルでトランザクションを開始する。
                this._tx = this._cnn.BeginTransaction(IsolationLevel.Snapshot);
            }
            else if (iso == DbEnum.IsolationLevelEnum.User)
            {
                // 無効な分離レベル（ユーザ指定）。
                throw new ArgumentException(
                    PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_USR);
            }
            else if (iso == DbEnum.IsolationLevelEnum.NotConnect)
            {
                // 2009/03/29 -- 追加したNotConnectの対応（このコードブロック）。

                // 無効な分離レベル（NotConnect指定）。
                throw new ArgumentException(
                    PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_NC);
            }
            else
            {
                // 通らない予定
            }

            // 分離レベル（iso）をメンバ変数に保存
            _iso = iso;
        }

        #endregion

        #region 終了

        /// <summary>トランザクションのコミット</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public override void CommitTransaction()
        {
            // Txオブジェクトの存在チェック
            if (this._tx == null)
            {
                // nullのためなにもしない。
            }
            else
            {
                // トランザクションのコミット
                this._tx.Commit();

                // nullクリア
                this._tx = null;
            }
        }


        /// <summary>トランザクションのロールバック</summary>
        /// <remarks>必要に応じて利用する。</remarks>
        public override void RollbackTransaction()
        {
            // Txオブジェクトの存在チェック
            if (this._tx == null)
            {
                // nullのためなにもしない。
            }
            else
            {
                // トランザクションのロールバック
                this._tx.Rollback();

                // nullクリア
                this._tx = null;
            }
        }

        #endregion

        #endregion

        #region SQLの作成

        # region SetSql系

        /// <summary>SQL文を記述したファイルへのパスを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetSqlByFile(string sqlFilePath)
        {
            // CommandTypeをTextとしてCommandオブジェクトを生成する
            this.SetSqlByFile(sqlFilePath, CommandType.Text);
        }

        /// <summary>SQL文を記述したファイルへのパスとCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetSqlByFile(string sqlFilePath, CommandType commandType)
        {
            // ファイルから、実行するSQL文を読み込む
            string commandText = this.Load(sqlFilePath);

            // SQL文を指定してCommandオブジェクトを生成する
            this.SetSqlByCommand(commandText, commandType);
        }

        /// <summary>SQL文を設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetSqlByCommand(string commandText)
        {
            // CommandTypeをTextとしてCommandオブジェクトを生成する
            this.SetSqlByCommand(commandText, CommandType.Text);
        }

        /// <summary>SQL文とCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetSqlByCommand(string commandText, CommandType commandType)
        {
            // コマンド テキストが、動的パラメタライズド クエリであるか確認する。
            // 結果は、プロパティに保存される。
            this.CheckCommandText(commandText);

            // Commandオブジェクトを生成する
            this._cmd = new OleDbCommand();
            this._cmd.CommandText = commandText;
            this._cmd.CommandType = commandType;

            // システム共通のCommandTimeout値を設定する。
            this.SetCommandTimeout(this._cmd); // #x-この行
        }

        # endregion

        #region SetParameter系

        /// <summary>パラメタライズドクエリのパラメタを取得する（Out,RetValパラメタ用）。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <returns>Out,RetValパラメタのバリュー</returns>
        /// <remarks>
        /// 通常、Dao経由で利用する。
        /// 動的SQLの場合はSQL実行後に利用可能
        /// </remarks>
        public override object GetParameter(string parameterName)
        {
            // nullチェック
            if (this._cmd != null)
            {
                // 存在しない場合はnullが返る。
                return this._cmd.Parameters[parameterName].Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetParameter(string parameterName, object obj)
        {
            this.SetParameter(parameterName, obj, null, -1, ParameterDirection.Input);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（OleDbType）（設定しない場合は、nullを指定）</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetParameter(string parameterName, object obj, object dbTypeInfo)
        {
            this.SetParameter(parameterName, obj, dbTypeInfo, -1, ParameterDirection.Input);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（OleDbType）（設定しない場合は、nullを指定）</param>
        /// <param name="size">パラメタのサイズ（設定しない場合は、-1を指定）</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetParameter(string parameterName, object obj, object dbTypeInfo, int size)
        {
            this.SetParameter(parameterName, obj, dbTypeInfo, size, ParameterDirection.Input);
        }

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（OleDbType）（設定しない場合は、nullを指定）</param>
        /// <param name="size">パラメタのサイズ（設定しない場合は、-1を指定）</param>
        /// <param name="paramDirection">パラメタの方向</param>
        /// <remarks>通常、Dao経由で利用する。</remarks>
        public override void SetParameter(string parameterName, object obj,
            object dbTypeInfo, int size, ParameterDirection paramDirection)
        {
            if (this.IsDPQ)
            {
                // 動的パラメタライズド クエリ

                // パラメタを保存（実行時に解析してまとめて設定する）
                this._parameter.Add(parameterName, obj);
                this._parameterType.Add(parameterName, dbTypeInfo);
                this._parameterSize.Add(parameterName, size);
                this._parameterDirection.Add(parameterName, paramDirection);
            }
            else
            {
                // 通常のパラメタライズド クエリ

                // パラメタ・ライズド・クエリにパラメタを設定する。
                OleDbParameter param = new OleDbParameter();

                #region dbTypeInfo

                // nullの場合は、設定しない。
                if (dbTypeInfo != null)
                {
                    // フラグ
                    bool isDbType = false;

                    // 設定方法の選択
                    string sqlDotnetTypeInfo =
                        GetConfigParameter.GetConfigValue(PubLiteral.SQL_DOTNET_TYPEINFO);

                    // デフォルト値対策：設定なし（null）の場合の扱いを決定
                    if (sqlDotnetTypeInfo != null)
                    {
                        // ON / OFF
                        if (sqlDotnetTypeInfo.ToUpper() == PubLiteral.ON)
                        {
                            // ON扱い = DbType
                            isDbType = true;
                        }
                        else if (sqlDotnetTypeInfo.ToUpper() == PubLiteral.OFF)
                        {
                            // OFF扱い = OleDbType
                            //isDbType = false;
                        }
                        else
                        {
                            // パラメータ・エラー（書式不正）
                            throw new ArgumentException(
                                String.Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_DOTNET_TYPEINFO));
                        }
                    }

                    if (isDbType)
                    {
                        // ON扱い = DbType
                        param.DbType = (DbType)dbTypeInfo;
                    }
                    else
                    {
                        // OFF扱い = OleDbType
                        param.OleDbType = (OleDbType)dbTypeInfo;
                    }
                }

                #endregion

                // -1の場合は、設定しない。
                if (size != -1)
                {
                    param.Size = size;
                }

                // パラメタの方向
                param.Direction = paramDirection;

                // 最後に名前と値を設定（Oracleの件に準拠）
                param.ParameterName = parameterName;
                param.Value = obj;

                this._cmd.Parameters.Add(param);
            }
        }

        #endregion

        /// <summary>ユーザパラメタを指定の文字列で置換する。</summary>
        /// <param name="userParameterName">置換対象のユーザパラメタ名</param>
        /// <param name="userParameterValue">置換の文字列</param>
        /// <remarks>
        /// SQLインジェクションされる可能性があるユーザ入力は「userParameterValue」に指定しないこと。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override void SetUserParameter(string userParameterName, string userParameterValue)
        {
            if (this.IsDPQ)
            {
                // 動的パラメタライズド クエリ

                // パラメタを保存（実行時に解析してまとめて設定する）
                this._userParameter.Add(userParameterName, userParameterValue);
            }
            else
            {
                // 通常のパラメタライズド クエリ

                // パラメタを保存（操作ログで使用する用途）
                this._userParameter.Add(userParameterName, userParameterValue);

                // 現在のコマンドテキストを取得
                string commandText = this._cmd.CommandText;

                // ユーザパラメタを指定の文字列で置換する
                commandText = commandText.Replace("%" + userParameterName + "%", userParameterValue);

                // 置換後のコマンドテキストを設定
                this._cmd.CommandText = commandText;
            }
        }

        #endregion

        #region SQLの実行

        #region SQL実行前の制御用メソッド

        /// <summary>
        /// SQL実行前の、
        /// ・通常のパラメタライズド クエリ
        /// ・動的パラメタライズド クエリ
        /// 制御用メソッド
        /// </summary>
        private void PreExecQuery()
        {
            if (this.IsDPQ)
            {
                // 動的パラメタライズド クエリ

                // パラメタライズド クエリに変換する（各タグの処理、IN句の処理）。
                this._cmd.CommandText = this.Convert(DamOLEDB._paramSign);

                // パラメタを設定する前に、this._QueryStatusをSQL
                // （通常のパラメタライズド・クエリ）に設定する。
                // ※ SetParameter内での動作が変わるため。
                this._QueryStatus = DbEnum.QueryStatusEnum.SPQ;

                #region Dictionaryのパラメタを設定

                foreach (string paramName in this._parameter.Keys)
                {
                    // 2008/10/24---null・DBNull対応（下記nullチェックを追加）
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._parameter[paramName] == null)
                    {
                        // パラメタがnullの場合

                        // 既定値対策されているのでこのままで良い。
                        this.SetParameter(
                            paramName,
                            this._parameter[paramName],
                            this._parameterType[paramName],
                            (int)this._parameterSize[paramName],
                            (ParameterDirection)this._parameterDirection[paramName]);
                    }
                    else if (this._parameter[paramName] is IList
                        && !(this._parameter[paramName] is System.Array))
                    {
                        // パラメタがnullでなく、ArrayListかList<T>の場合(IList)
                        IList al = (IList)this._parameter[paramName];

                        // パラメタを展開して設定。 

                        int counter = 1;

                        foreach (object obj in al)
                        {
                            // 既定値対策されているのでこのままで良い。
                            this.SetParameter(
                                paramName + "_" + counter.ToString(), obj,
                                this._parameterType[paramName],
                                (int)this._parameterSize[paramName],
                                (ParameterDirection)this._parameterDirection[paramName]);

                            // カウンタをインクリメント
                            counter++;
                        }
                    }
                    else
                    {
                        // それ以外のパラメタは、そのまま設定。

                        // 既定値対策されているのでこのままで良い。
                        this.SetParameter(
                            paramName,
                            this._parameter[paramName],
                            this._parameterType[paramName],
                            (int)this._parameterSize[paramName],
                            (ParameterDirection)this._parameterDirection[paramName]);
                    }
                }

                #endregion

                // 2009/04/20---実行後、初期化、
                // 静的パラメタライズドクエリとして連続実行可能にする。
                this.init();
            }
            else
            {
                // 通常のパラメタライズド クエリそのまま実行
            }

            // で、

            // 最後に通常のパラメタライズド クエリの
            // 「名前バインド」を「順番バインド」に置換する。
            this.ChangesToOrderBind();
        }

        /// <summary>
        /// 通常のパラメタライズド クエリの
        /// 「名前バインド」を「順番バインド」に置換する。
        /// </summary>
        private void ChangesToOrderBind()
        {
            // Command.Parametersの退避
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (OleDbParameter parameter in this._cmd.Parameters)
            {
                parameters.Add(parameter.ParameterName, parameter);
            }

            // Command.Parametersをクリア
            this._cmd.Parameters.Clear();

            // パラメタ名
            string paramName = "";

            // CommandTextを退避
            string tmpCommandText = this._cmd.CommandText;

            // 名前バインド パラメタ → 順番バインド パラメタ
            while (true)
            {
                // パラメタ記号の位置（インデックス）
                int paramSignIndex;

                // Command.CommandTextから、パラメタ名を取得する。
                paramName = this.GetParamByText(this._cmd.CommandText, DamOLEDB._paramSign, out paramSignIndex);

                if (paramSignIndex == -1)
                {
                    // パラメタが検出されなかった場合
                    // 置換処理を終了する
                    break;
                }

                // 名前バインド パラメタ → 順番バインド パラメタの"?"に置換。
                //this._cmd.CommandText = this._cmd.CommandText.Replace(DamOLEDB._paramSign + paramName, "?");
                this._cmd.CommandText = this._cmd.CommandText.Substring(0, paramSignIndex) + "?" + this._cmd.CommandText.Substring(paramSignIndex + paramName.Length + 1);   //+1はパラメタの先頭記号分

                // 取得したパラメタ名がパラメタにあることを確認する。
                if (parameters.ContainsKey(paramName)) // Dic化でメソッド名が微変化
                {
                    //退避したパラメタに存在する

                    //パラメタを設定する
                    //　既出のパラメタの場合も設定する。
                    this._cmd.Parameters.Add(parameters[paramName]);
                }
                else
                {
                    //退避したパラメタに存在しない

                    //例外：「スカラー変数 "@xxx" を宣言してください。」
                    throw new ArgumentException(string.Format(PublicExceptionMessage.ORDER_BIND_ERROR_PARAMETER_NOT_FOUND, DamOLEDB._paramSign + paramName));
                }
            }
        }

        #endregion

        #region 外部公開API

        /// <summary>Selectクエリを実行し、データテーブルを返す。</summary>
        /// <param name="dt">データテーブル</param>
        /// <remarks>
        /// OleDbDataAdapterのFillを実行する。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override void ExecSelectFill_DT(DataTable dt)
        {
            // SQL実行前の、
            // ・通常のパラメタライズド クエリ
            // ・動的パラメタライズド クエリ
            // 制御用メソッド
            this.PreExecQuery();

            // コネクション、トランザクションを設定
            this._cmd.Connection = this._cnn;
            this._cmd.Transaction = this._tx;

            // SelectCommandからデータアダプタを生成
            this._adpt = new OleDbDataAdapter(this._cmd);

            // データをFill
            this._adpt.Fill(dt);
        }

        /// <summary>Selectクエリを実行し、データセットを返す。</summary>
        /// <param name="ds">データセット</param>
        /// <remarks>
        /// OleDbDataAdapterのFillを実行する。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override void ExecSelectFill_DS(DataSet ds)
        {
            // SQL実行前の、
            // ・通常のパラメタライズド クエリ
            // ・動的パラメタライズド クエリ
            // 制御用メソッド
            this.PreExecQuery();

            // コネクション、トランザクションを設定
            this._cmd.Connection = this._cnn;
            this._cmd.Transaction = this._tx;

            // SelectCommandからデータアダプタを生成
            this._adpt = new OleDbDataAdapter(this._cmd);

            // データをFill
            this._adpt.Fill(ds);
        }

        /// <summary>Selectクエリを実行し、データリーダを返す。</summary>
        /// <returns>OleDbDataReader</returns>
        /// <remarks>
        /// OleDbCommandのExecuteReaderを実行する。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override IDataReader ExecSelect_DR()
        {
            // SQL実行前の、
            // ・通常のパラメタライズド クエリ
            // ・動的パラメタライズド クエリ
            // 制御用メソッド
            this.PreExecQuery();

            // コネクション、トランザクションを設定
            this._cmd.Connection = this._cnn;
            this._cmd.Transaction = this._tx;

            // データリーダを返す。
            return this._cmd.ExecuteReader();
        }

        /// <summary>Selectクエリを実行し、結果セットの最初の行の最初の列を返す。</summary>
        /// <returns>結果セットの最初の行の最初の列（オブジェクト型） </returns>
        /// <remarks>
        /// OleDbCommandのExecuteScalarを実行する。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override object ExecSelectScalar()
        {
            // SQL実行前の、
            // ・通常のパラメタライズド クエリ
            // ・動的パラメタライズド クエリ
            // 制御用メソッド
            this.PreExecQuery();

            // コネクション、トランザクションを設定
            this._cmd.Connection = this._cnn;
            this._cmd.Transaction = this._tx;

            // 結果セットの最初の行の最初の列を返す。
            return this._cmd.ExecuteScalar();
        }

        /// <summary>Insert、Update、Deleteクエリを実行し、影響を受けた行数を返す。</summary>
        /// <returns>影響を受けた行数</returns>
        /// <remarks>
        /// OleDbCommandのExecuteNonQueryを実行する。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override int ExecInsUpDel_NonQuery()
        {
            // SQL実行前の、
            // ・通常のパラメタライズド クエリ
            // ・動的パラメタライズド クエリ
            // 制御用メソッド
            this.PreExecQuery();

            // コネクション、トランザクションを設定
            this._cmd.Connection = this._cnn;
            this._cmd.Transaction = this._tx;

            // SQLを実行して、影響を受けた行数を返す。
            return this._cmd.ExecuteNonQuery();
        }

        /// <summary>静的SQLを生成する</summary>
        /// <param name="sqlUtil">SQLUtility</param>
        /// <returns>SQL文</returns>
        /// <remarks>
        /// Commandでの実行はしない。
        /// 通常、Dao経由で利用する。
        /// </remarks>
        public override string ExecGenerateSQL(SQLUtility sqlUtil)
        {
            throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
        }

        #endregion

        #endregion

        #region その他

        /// <summary>現在コマンドオブジェクトに設定されているSQLを取得する。</summary>
        /// <returns>現在コマンドオブジェクトに設定されているSQL</returns>
        /// <remarks>必要に応じて利用する。</remarks>
        public override string GetCurrentQuery()
        {
            // 現在のコマンドテキストを取得
            return this._cmd.CommandText;
        }

        /// <summary>現在コマンドオブジェクトに設定されているSQLを取得する（ログ出力用に編集したもの）。</summary>
        /// <returns>現在コマンドオブジェクトに設定されているSQL（ログ出力用に編集したもの）</returns>
        /// <remarks>必要に応じて利用する。</remarks>
        public override string GetCurrentQueryForLog()
        {
            // #16-start
            if (string.IsNullOrEmpty(GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG)))
            {
                // デフォルト値対策：設定なし（null）の場合の扱いを決定
                // Log4Netへログ出力（OFF）
                return "";
            }
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.OFF)
            {
                // Log4Netへログ出力（OFF）
                return "";
            }
            // #16-end
            else if (GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() == PubLiteral.ON)
            {
                // Log4Netへログ出力（ON）

                // コマンドテキスト
                string commandText = PubLiteral.SQLTRACELOG_COMMAND_TEXT_HEADER;

                // コマンドテキストを取得
                commandText += this._cmd.CommandText;

                // パラメタ
                string commandParameter = PubLiteral.SQLTRACELOG_COMMAND_PARAM_HEADER;

                // パラメタを取得
                foreach (OleDbParameter p in this._cmd.Parameters)
                {
                    // 2008/10/24---null・DBNull対応（ここから）

                    if (p.Value == null)
                    {
                        commandParameter += p.ParameterName + "=null" + ",";
                    }
                    else if (p.Value == DBNull.Value)
                    {
                        commandParameter += p.ParameterName + "=DBNull" + ",";
                    }
                    else
                    {
                        commandParameter += p.ParameterName + "=" + p.Value.ToString() + ",";
                    }

                    // 2008/10/24---null・DBNull対応（ここまで）
                }

                // 最後に、改行を取り除き、無駄な空白を削除して結果を返す。
                //return this.ClearText(commandText + " " + commandParameter);
                return this.ClearText(commandText) + " " + commandParameter;
            }
            else
            {
                // パラメータ・エラー
                throw new ArgumentException(
                    string.Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_TRACELOG));
            }
        }

        #endregion

        #endregion
    }
}
