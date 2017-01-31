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
//* クラス名        ：CstSqlSessionStateProvider
//* クラス日本語名  ：SQL Serverを使用したCustomSessionProvider
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/xx/xx  西野 大介         新規作成
//*  2012/04/05  西野 大介         \n → \r\n 化
//**********************************************************************************

using System;
using System.IO;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Timers;
using System.Diagnostics;

using System.Web;
using System.Web.SessionState;
using System.Web.Configuration;

#region ORGコメント

// 以下のサンプルを修正してSQL Azure用とした
// http://msdn.microsoft.com/ja-jp/library/ms178588.aspx

// 修正ポイント
// ・ODBC → SqlClientへの変更、
// ・それに伴うパラメタライズドクエリの記述修正
// ・１分間隔でデータベース上のExpiredセッションを削除する機能の追加

// 使い方
// ① このファイルをソリューションに含める
// ② SQL Azure上にセッションデータのテーブルを作成する
// ③ 構成設定を記述する

/*
Azure 環境へのテーブル作成

CREATE TABLE ASPStateSessions
(
    SessionId       varchar(80)  NOT NULL,
    ApplicationName varchar(255) NOT NULL,
    Created         DateTime  NOT NULL,
    Expires         DateTime  NOT NULL,
    LockDate        DateTime  NOT NULL,
    LockId          Integer   NOT NULL,
    Timeout         Integer   NOT NULL,
    Locked          Bit     NOT NULL,
    SessionItems    Text,
    Flags           Integer   NOT NULL,
        CONSTRAINT PKSessions PRIMARY KEY (SessionId, ApplicationName)
)

構成設定の記述

<configuration>
  <connectionStrings>
    <add name="SqlSessionStoreConnectionString" connectionString="xxxx" />
  </connectionStrings>

  <system.web>
    <sessionState 
      regenerateExpiredSessionId="true"
      mode="Custom"
      customProvider="CstSqlSessionStateProvider" timeout="1">
      <providers>
        <add name="CstSqlSessionStateProvider"
          type="CustomSessionProvider.CstSqlSessionStateProvider"
          connectionStringName="SqlSessionStoreConnectionString"
          writeExceptionsToEventLog="true" />
      </providers>
    </sessionState>
  </system.web>
</configuration>

*/
#endregion

#region 概要

// ＜ SessionStateModule ＞
// セッションは、SessionStateModuleクラスで管理されます。

// このクラスは、SessionStateStoreProviderを呼び出し、
// 要求の間に異なるタイミングでセッション データを
// 読み取り、セッション データ ストアに書き込みます。

// ①　要求の最初に、
// 　　SessionStateModuleは、
// 　　GetItemExclusiveメソッドを呼び出して
// 　　セッション データ ストアからデータを取得します。

// 　　または、EnableSessionStateページ属性がReadOnlyに
// 　　設定されている場合は、GetItemメソッドを呼び出します。

// ②　要求の最後に、
// 　　SessionStateModuleは（セッションが更新された場合）、
// 　　SetAndReleaseItemExclusiveメソッドを呼び出し、
// 　　更新した値をセッション データ ストアにデータを書き込みます。

// SessionStateModuleは
// SessionStateStoreProviderBaseのメンバを
// 呼び出し、新規セッションを初期化します。

// また、HttpSessionState.Abandonメソッドが呼び出されたときは、
// セッション データ ストアのセッション データを削除します。

#endregion

#region 補足

// ＜カスタムのSessionID＞
// SessionStateModuleは、
// SessionStateStoreProviderに依存せずに、
// 単独でSessionID値を決定します。

// 必要に応じてカスタムのSessionIDManagerを実装するには、
// ISessionIDManagerインターフェイスを継承するクラスを作成します。 
// 詳細については、「ISessionIDManager」の「解説」を参照してください。 
// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.isessionidmanager.aspx

// ＜偽装してSessionストアにアクセス＞
// セキュリティで保護されたセッション データ ストアにアクセスする場合、
// SessionStateModuleはASP.NETのプロセスIDに戻リます。 
// <sessionState>構成要素のuseHostingIdentity属性をfalseに設定すると、
// IISによって提供されるIDをSessionStateModuleが偽装するように指定できます。

// 例えば、Windowsの統合セキュリティを
// 使用するようにIISアプリケーションを構成し、
// IISによって提供されるIDをセッション管理の
// ために偽装するときは、アプリケーションの
// Web.configファイルの<system.web>構成セクションに
// <identity impersonate="true" /> と指定し、
// <sessionState> 構成要素のuseHostingIdentity属性をfalseに設定します。

// useHostingIdentity属性がtrueの場合、
// セッション データ ストアに接続する際に、
// ASP.NETはプロセスID、つまり
// <identity>構成要素に提供される
// ユーザ資格情報 (存在する場合) を偽装します。

// ASP.NETプロセスIDの詳細については、
// 「ASP.NET プロセス ID の構成」および
// 「ASP.NET の偽装」を参照してください。

#endregion

#region ロック

#region 概要

// ASP.NETアプリケーションは
// マルチスレッドをサポートしているため、
// 同時実行された複数の要求に応答できます。

// 同時実行された複数の要求が、
// 同じセッション情報にアクセスする場合があります。

// フレームセット内の複数のフレームがすべて同じ
// アプリケーションのASP.NET Webページ
// を参照するシナリオを考えてみます。

// フレームセット内の各フレームの個々の要求は、
// Webサーバの異なるスレッドで同時に実行される場合があります。
// 各フレームのASP.NETページがセッションにアクセスすると、
// 複数のスレッドが同時にセッション データ ストアにアクセスする可能性があります。

// セッション データ ストアでのデータの競合や予期不可能な動作を回避
// するため、SessionStateModuleとSessionStateStoreProviderBase
// には、ASP.NET ページの実行時にセッション データ ストアの
// 特定の項目を排他的にロックする機能が含まれます。

#endregion

#region 詳細

// EnableSessionState属性がReadOnlyとして
// マークされているときは、セッション データ ストア
// 項目にロックは設定されません（GetItemメソッド）。

// ただし、同じアプリケーションの他のASP.NETページが
// セッション データ ストアに書き込むことができる可能性があるため、
// この場合、セッション データ ストアからの読み取り専用セッション データ
// に対する要求はロックされたデータが解放されるのを待つことになります。 

// ロックは、GetItemExclusiveメソッドの呼び出し時に、
// セッション データ ストア項目に設定されます。 

// 要求が完了すると、このロックは
// SetAndReleaseItemExclusiveメソッド
// の呼び出し時に解放されます。 

// GetItemExclusive or GetItemメソッドの呼び出し時に、
// ロックされたセッション データ ストア項目を検出した場合、
// SessionStateModuleはセッション データ ストア項目のロックが解放されるか、
// ExecutionTimeoutプロパティの値に指定された時間を超えるまで、
// 0.5 秒間隔でセッション データ ストア項目を再要求します。

// 要求がExecutionTimeoutした場合、
// SessionStateModuleはReleaseItemExclusiveを
// 呼び出してセッション データ ストア項目を解放します。

// ロックされたセッション データ ストア項目は、
// 現在の応答に対するSetAndReleaseItemExclusiveメソッドを呼び出す前に、
// 別のスレッド上でReleaseItemExclusiveメソッドを呼び出して
// 解放されている場合があります（ExecutionTimeout時の動作）。 

// SessionStateModuleは、
// 別のセッションで既に解放されて変更された
// セッション データ ストア項目を
// 設定および解放する可能性があるため、
// SessionStateModuleには、ロックされた
// セッション データ ストア項目を変更する
// ためのロック識別子が要求ごとに含みます。

// セッション データ ストアは、
// セッション データ ストア項目のロック識別子が
// 提供されたロック識別子と一致した場合にのみ更新されます。

#endregion

#endregion

namespace Touryo.Infrastructure.Business.Csp
{
    /// <summary>
    /// CstSqlSessionStateProvider
    /// SQL Serverを使用したSessionStateStoreProvider
    /// </summary>
    /// <remarks>
    /// SessionStateStoreProviderの実装
    /// http://msdn.microsoft.com/ja-jp/library/ms178587.aspx
    /// SessionStateStoreProviderBase
    /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.aspx
    /// </remarks>
    public sealed class CstSqlSessionStateProvider : SessionStateStoreProviderBase
    {
        #region private

        /// <summary>SessionStateSection</summary>
        private SessionStateSection _sessionStateConfig = null;

        /// <summary>ConnectionStringSettings</summary>
        private ConnectionStringSettings _connectionStringSettings = null;

        /// <summary>接続文字列</summary>
        private string ConnectionString = "";

        #endregion

        #region const

        /// <summary>
        /// ログ定数：ソース
        /// </summary>
        private const string EVENT_SOURCE = "CstSqlSessionStateProvider";

        /// <summary>
        /// ログ定数：ログ
        /// </summary>
        private const string EVENT_LOG = "Application";

        /// <summary>
        /// MSG定数：設定周りの例外が発生した場合。
        /// </summary>
        private const string EXCEPTION_MESSAGE = "An exception occurred. Please contact your administrator.";

        #endregion

        #region プロパティ

        /// <summary>
        /// イベントログの有効・無効プロパティ
        /// </summary>
        /// <remarks>
        /// false：例外は来訪者に投げられます。
        /// true ：例外はイベント・ログに書かれています。
        /// </remarks>
        private bool _writeExceptionsToEventLog = false;

        /// <summary>
        /// イベントログの有効・無効プロパティ・プロシージャ
        /// </summary>
        public bool WriteExceptionsToEventLog
        {
            get { return this._writeExceptionsToEventLog; }
            set { this._writeExceptionsToEventLog = value; }
        }

        /// <summary>
        /// アプリケーション名
        /// （ApplicationVirtualPath）
        /// </summary>
        /// <remarks>
        /// キーにする。
        /// 異なるアプリケーションでは
        /// SessionIDが重複するため。
        /// </remarks>
        private string _applicationName;

        /// <summary>
        /// アプリケーション名
        /// （ApplicationVirtualPath）
        /// プロパティ・プロシージャ
        /// </summary>
        public string ApplicationName
        {
            get { return this._applicationName; }
        }

        #endregion

        #region SessionStateStoreProviderBaseメンバ

        #region 初期化・終了

        /// <summary>
        /// ProviderBase.Initialize メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.configuration.provider.providerbase.initialize.aspx
        /// SessionStateStoreProviderを初期化する。
        /// web.configのsessionStateのprovidersのadd要素から初期化
        /// </summary>
        /// <param name="name">名称属性</param>
        /// <param name="config">追加属性の取得</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // web.configのsessionStateの
            // providersのadd要素から初期化する。

            // config引数がnullの時、
            if (config == null)
            {
                // 例外
                throw new ArgumentNullException("config");
            }

            // 名称属性がNullOrEmptyの時、
            if (string.IsNullOrEmpty(name))
            {
                // デフォルトの設定
                name = "CstSqlSessionStateProvider";
            }

            // 追加属性（description）がNullOrEmptyの時、
            if (String.IsNullOrEmpty(config["description"]))
            {
                // デフォルトの設定
                config.Remove("description");
                config.Add("description", "Custom Sql Session State Provider");
            }

            // ベースの初期化
            base.Initialize(name, config);

            // アプリケーション名
            this._applicationName =
              System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            // <sessionState> configuration element.
            Configuration configuration =
              WebConfigurationManager.OpenWebConfiguration(ApplicationName);

            this._sessionStateConfig =
              (SessionStateSection)configuration.GetSection("system.web/sessionState");

            // 接続文字列を初期化
            this._connectionStringSettings =
              ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (this._connectionStringSettings == null ||
              this._connectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            this.ConnectionString = this._connectionStringSettings.ConnectionString;

            // ログの出力
            this._writeExceptionsToEventLog = false;

            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                    this._writeExceptionsToEventLog = true;
            }

            // 定期的にデータを削除するスレッドを起動
            DeleteExpireSessionPeriodicallyStart();
        }

        /// <summary>
        /// SessionStateProviderBase.SetItemExpireCallback メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.setitemexpirecallback.aspx
        /// Global.asax ファイルに定義されている Session_OnEnd イベント
        /// の SessionStateItemExpireCallback デリゲートへの参照を設定。
        /// </summary>
        /// <param name="expireCallback">
        /// SessionStateItemExpireCallback デリゲート
        /// </param>
        /// <returns>
        /// SessionStateStoreProviderが Session_OnEnd イベントの
        /// 呼び出しをサポートする場合は true。それ以外の場合は false。
        /// </returns>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            // Session_OnEnd イベントの呼び出しをサポートしない。
            return false;
        }

        /// <summary>
        /// IDisposable.Dispose メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.idisposable.dispose.aspx
        /// IDisposableの実装
        /// </summary>
        public override void Dispose()
        {
            // 特になし。
        }

        #endregion

        #region 新規セッション

        /// <summary>
        /// SessionStateStoreProviderBase.CreateNewStoreData メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.createnewstoredata.aspx
        /// 現在の要求で使用するSessionStateStoreDataオブジェクトを新規作成 
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="timeout">セッション状態のTimeout値</param>
        /// <returns>
        /// 現在の要求に関する新しいSessionStateStoreData
        /// </returns>
        /// <remarks>
        /// SessionStateModuleはASP.NETページの
        /// 要求の開始時のAcquireRequestStateイベントで
        /// 
        /// ・受信した要求にセッションID がない場合
        /// 
        /// ・受信した要求にセッションID はあるが、
        /// 　データ ストアにセッションが見つからない場合
        /// 　
        /// CreateNewStoreDataメソッドを呼び出す。
        /// </remarks>
        public override SessionStateStoreData CreateNewStoreData(
          HttpContext context,
          int timeout)
        {
            // お決まりのコード？
            return new SessionStateStoreData(
                new SessionStateItemCollection(),
              SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        #endregion

        #region セッション データ ストアの読み・書き

        #region セッション データ ストアの書き込み

        /// <summary>
        /// SessionStateProviderBase.SetAndReleaseItemExclusive メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.setandreleaseitemexclusive.aspx
        /// 現在の要求の値を使用してセッション データ ストア項目
        /// の情報を更新し、データのロックを解除する。
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="id">セッション識別子</param>
        /// <param name="item">SessionStateStoreData</param>
        /// <param name="lockId">ロック識別子</param>
        /// <param name="newItem">
        /// セッション項目を
        /// ・新しい項目に指定する場合は：true
        /// ・既存の項目に指定する場合は：false
        /// </param>
        public override void SetAndReleaseItemExclusive(
            HttpContext context,
            string id,
            SessionStateStoreData item,
            object lockId,
            bool newItem)
        {
            // セッション・コレクションをシリアライズ
            string sessItems = Serialize((SessionStateItemCollection)item.Items);

            // データプロバイダ
            SqlConnection conn = new SqlConnection(this.ConnectionString);
            SqlCommand deleteCmd = null;
            SqlCommand insertCmd = null;

            if (newItem)
            {
                // 新しい項目（del→ins）の保存処理

                // 既存のセッションを消す
                //（有効期限切れであること）
                deleteCmd = new SqlCommand(
                    "DELETE FROM ASPStateSessions" +
                    " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND Expires < @Expires", conn);

                // パラメタ指定

                // WHERE
                deleteCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                deleteCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
                deleteCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now;

                // 新しい項目を追加
                insertCmd = new SqlCommand(
                    "INSERT INTO ASPStateSessions" +
                    " (SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags)" +
                    " Values" +
                    " (@SessionId, @ApplicationName, @Created, @Expires, @LockDate, @LockId , @Timeout, @Locked, @SessionItems, @Flags)", conn);

                // パラメタ指定

                // Values
                insertCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                insertCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
                insertCmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now;
                insertCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)item.Timeout);
                insertCmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value = DateTime.Now;
                insertCmd.Parameters.Add("@LockId", SqlDbType.Int).Value = 0;
                insertCmd.Parameters.Add("@Timeout", SqlDbType.Int).Value = item.Timeout;
                insertCmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = false;
                insertCmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, sessItems.Length).Value = sessItems;
                insertCmd.Parameters.Add("@Flags", SqlDbType.Int).Value = 0;
            }
            else
            {
                // 既存の項目（upd）の保存処理

                // ロック識別子を確認して更新（合わせてロックを解放）
                insertCmd = new SqlCommand(
                  "UPDATE ASPStateSessions" +
                  " SET Expires = @Expires, SessionItems = @SessionItems, Locked = @Locked" +
                  " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn);

                // パラメタ指定

                // SET
                insertCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)item.Timeout);
                insertCmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, sessItems.Length).Value = sessItems;
                insertCmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = false;

                // WHERE
                insertCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                insertCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
                insertCmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId;
            }

            try
            {
                // 保存処理の実行
                conn.Open();

                // あるときだけ
                if (deleteCmd != null)
                    deleteCmd.ExecuteNonQuery();

                insertCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                // ★★有効期限切れでない新規項目のインサート失敗は考えられるが・・・。

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "SetAndReleaseItemExclusive");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                    throw;
            }
            finally
            {
                // 保存処理の終了処理
                conn.Close();
            }
        }

        #endregion

        #region セッション データ ストアの読み込み

        /// <summary>
        /// SessionStateProviderBase.GetItemExclusive メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.getitemexclusive.aspx
        /// セッション データ ストアから読み取り専用の
        /// SessionStateStoreDataを返す。
        /// （通常は、こちらのメソッドが使用される）
        /// </summary>
        /// <param name="context">
        /// HttpContext
        /// </param>
        /// <param name="id">
        /// セッション識別子
        /// </param>
        /// <param name="locked">
        /// 要求したセッション項目が
        /// セッション データ ストアで
        /// ・ロックされているときはブール値 true
        /// ・それ以外の場合は false
        /// </param>
        /// <param name="lockAge">
        /// セッション データ ストアの項目がロック
        /// された時間に設定された TimeSpan オブジェクト
        /// </param>
        /// <param name="lockId">
        /// ロック識別子
        /// </param>
        /// <param name="actionFlags">
        /// 現在のセッションが初期化されていない
        /// cookieless セッションかどうかを示す
        /// SessionStateActions 値
        /// </param>
        /// <returns>
        /// SessionStateStoreData
        /// </returns>
        public override SessionStateStoreData GetItemExclusive(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            // GetSessionStoreItemへ
            return GetSessionStoreItem(true, // ture
                context, id, out locked,
                out lockAge, out lockId, out actionFlags);
        }

        /// <summary>
        /// SessionStateProviderBase.GetItem メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.getitem.aspx
        /// セッション データ ストアから
        /// 読み取り専用のSessionStateStoreDataを返す。
        /// （EnableSessionState属性がReadOnly
        /// の場合こちらのメソッドが使用される）
        /// </summary>
        /// <param name="context">
        /// HttpContext
        /// </param>
        /// <param name="id">
        /// セッション識別子
        /// </param>
        /// <param name="locked">
        /// 要求したセッション項目が
        /// セッション データ ストアで
        /// ・ロックされているときはブール値 true
        /// ・それ以外の場合は false
        /// </param>
        /// <param name="lockAge">
        /// セッション データ ストアの項目がロック
        /// された時間に設定された TimeSpan オブジェクト
        /// </param>
        /// <param name="lockId">
        /// ロック識別子
        /// </param>
        /// <param name="actionFlags">
        /// 現在のセッションが初期化されていない
        /// cookieless セッションかどうかを示す
        /// SessionStateActions 値
        /// </param>
        /// <returns>
        /// SessionStateStoreData
        /// </returns>
        public override SessionStateStoreData GetItem(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            // GetSessionStoreItemへ
            return GetSessionStoreItem(false, // false
                context, id, out locked,
                out lockAge, out lockId, out actionFlags);
        }

        /// <summary>
        /// GetItemおよびGetItemExclusiveの両方によって呼ばれ、
        /// セッション データ ストアを検索する。
        /// lockRecordパラメーターがtrueの場合
        /// (GetItemExclusiveから呼ばれた場合)、
        /// セッション データ ストア項目をロックし、
        /// 新しいLockIdおよびLockDateをセットする。
        /// </summary>
        /// <param name="lockRecord">
        /// GetItemExclusiveから呼ばれた場合、true
        /// GetItemから呼ばれた場合、false
        /// </param>
        /// <param name="context">
        /// HttpContext
        /// </param>
        /// <param name="id">
        /// セッション識別子
        /// </param>
        /// <param name="locked">
        /// 要求したセッション項目が
        /// セッション データ ストアで
        /// ・ロックされているときはブール値 true
        /// ・それ以外の場合は false
        /// </param>
        /// <param name="lockAge">
        /// セッション データ ストアの項目がロック
        /// された時間に設定された TimeSpan オブジェクト
        /// </param>
        /// <param name="lockId">
        /// ロック識別子
        /// </param>
        /// <param name="actionFlags">
        /// 現在のセッションが初期化されていない
        /// cookieless セッションかどうかを示す
        /// SessionStateActions 値
        /// </param>
        /// <returns>
        /// SessionStateStoreData
        /// </returns>
        private SessionStateStoreData GetSessionStoreItem(
            bool lockRecord,
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actionFlags)
        {
            // Initial values for return value and out parameters.
            SessionStateStoreData item = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;

            // Sql database connection.
            SqlConnection conn = new SqlConnection(this.ConnectionString);
            // SqlCommand for database commands.
            SqlCommand cmd = null;
            // DataReader to read database record.
            SqlDataReader reader = null;
            // DateTime to check if current session item is expired.
            DateTime expires;
            // String to hold serialized SessionStateItemCollection.
            string serializedItems = "";
            // True if a record is found in the database.
            bool foundRecord = false;
            // True if the returned session item is expired and needs to be deleted.
            bool deleteData = false;
            // Timeout value from the data store.
            int timeout = 0;

            try
            {
                conn.Open();


                if (lockRecord)
                {
                    cmd = new SqlCommand(
                      "UPDATE ASPStateSessions SET" +
                      " Locked = @Locked, LockDate = @LockDate " +
                      " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND Locked = @Locked2 AND Expires > @Expires", conn);
                    cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value
                      = DateTime.Now;
                    cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                    cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar,
                      255).Value = ApplicationName;
                    cmd.Parameters.Add("@Locked2", SqlDbType.Int).Value = false;
                    cmd.Parameters.Add
                      ("@Expires", SqlDbType.DateTime).Value = DateTime.Now;

                    if (cmd.ExecuteNonQuery() == 0)
                        // No record was updated because the record was locked or not found.
                        locked = true;
                    else
                        // The record was updated.

                        locked = false;
                }

                // Retrieve the current session item information.
                cmd = new SqlCommand(
                  "SELECT Expires, SessionItems, LockId, LockDate, Flags, Timeout " +
                  "  FROM ASPStateSessions " +
                  "  WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn);
                cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar,
                  255).Value = ApplicationName;

                // Retrieve session item data from the data source.
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                while (reader.Read())
                {
                    expires = reader.GetDateTime(0);

                    if (expires < DateTime.Now)
                    {
                        // The record was expired. Mark it as not locked.
                        locked = false;
                        // The session was expired. Mark the data for deletion.
                        deleteData = true;
                    }
                    else
                        foundRecord = true;

                    serializedItems = reader.GetString(1);
                    lockId = reader.GetInt32(2);
                    lockAge = DateTime.Now.Subtract(reader.GetDateTime(3));
                    actionFlags = (SessionStateActions)reader.GetInt32(4);
                    timeout = reader.GetInt32(5);
                }
                reader.Close();


                // If the returned session item is expired, 
                // delete the record from the data source.
                if (deleteData)
                {
                    cmd = new SqlCommand("DELETE FROM ASPStateSessions " +
                      "WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn);
                    cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                    cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar,
                      255).Value = ApplicationName;

                    cmd.ExecuteNonQuery();
                }

                // The record was not found. Ensure that locked is false.
                if (!foundRecord)
                    locked = false;

                // If the record was found and you obtained a lock, then set 
                // the lockId, clear the actionFlags,
                // and create the SessionStateStoreItem to return.
                if (foundRecord && !locked)
                {
                    lockId = (int)lockId + 1;

                    cmd = new SqlCommand("UPDATE ASPStateSessions SET" +
                      " LockId = @LockId, Flags = 0 " +
                      " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn);
                    cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId;
                    cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
                    cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;

                    cmd.ExecuteNonQuery();

                    // If the actionFlags parameter is not InitializeItem, 
                    // deserialize the stored SessionStateItemCollection.
                    if (actionFlags == SessionStateActions.InitializeItem)
                        item = CreateNewStoreData(context, Convert.ToInt32(this._sessionStateConfig.Timeout.TotalMinutes));
                    else
                        item = Deserialize(context, serializedItems, timeout);
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetSessionStoreItem");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                    throw;
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            return item;
        }

        #endregion

        #endregion

        #region その他、フレームワークから呼び出されるメソッド

        #region Abandonによる削除

        /// <summary>
        /// SessionStateProviderBase.RemoveItem メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.removeitem.aspx
        /// セッション データ ストアから項目データを削除する
        /// （Abandonメソッドが呼び出されている場合）。
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="id">セッション識別子</param>
        /// <param name="lockId">ロック識別子</param>
        /// <param name="item">SessionStateStoreData</param>
        public override void RemoveItem(
            HttpContext context,
            string id,
            object lockId,
            SessionStateStoreData item)
        {
            // Abandon処理のクエリ準備
            SqlConnection conn = new SqlConnection(this.ConnectionString);

            // ロック識別子まで要る。
            SqlCommand cmd = new SqlCommand(
                "DELETE FROM ASPStateSessions" +
                " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn);

            // パラメタ指定

            // WHERE
            cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId;

            try
            {
                // Abandon処理のクエリ実行
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            //catch (SqlException e)
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RemoveItem");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // Abandon処理の終了処理
                conn.Close();
            }
        }

        #endregion

        #region 例外的ケース

        /// <summary>
        /// SessionStateProviderBase.ReleaseItemExclusive メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.releaseitemexclusive.aspx
        /// セッション データ ストアの項目のロックを解除する
        /// （ロックが ExecutionTimeout 値を超えている場合）。
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="id">セッション識別子</param>
        /// <param name="lockId">ロック識別子</param>
        public override void ReleaseItemExclusive(
            HttpContext context,
            string id,
            object lockId)
        {
            // リリース処理のクエリ準備
            // ロック解放とスライディング
            SqlConnection conn = new SqlConnection(this.ConnectionString);

            SqlCommand cmd = new SqlCommand(
                  "UPDATE ASPStateSessions" +
                  " SET Locked = 0, Expires = @Expires" +
                  " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn);

            // パラメタ指定

            // SET
            cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value
                = DateTime.Now.AddMinutes(this._sessionStateConfig.Timeout.TotalMinutes);

            // WHERE
            cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId;

            try
            {
                // リリース処理のクエリ実行
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ReleaseItemExclusive");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // リリース処理の終了処理
                conn.Close();
            }
        }

        /// <summary>
        /// SessionStateProviderBase.ResetItemTimeout メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.resetitemtimeout.aspx
        /// セッション データ ストアの項目の有効期限の日時を更新
        /// （エラー時や、EnableSessionState = false時のスライディング処理用）。
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="id">セッション識別子</param>
        /// <remarks>
        /// EnableSessionState属性がtrueの場合で、
        /// 
        /// エラーが発生したために要求が
        /// ・AcquireRequestState
        /// ・ReleaseRequestState
        /// イベントを生成しなかった場合
        /// 
        /// または、EnableSessionState属性がfalseに設定されている場合にも、
        /// ASP.NETページが要求されるとResetItemTimeoutメソッドが呼び出され、
        /// セッション データ ストアのデータの有効期限の日時が更新されます。
        /// </remarks>
        public override void ResetItemTimeout(
            HttpContext context,
            string id)
        {
            // 有効期限の更新処理のクエリ準備
            SqlConnection conn = new SqlConnection(this.ConnectionString);

            SqlCommand cmd = new SqlCommand(
              "UPDATE ASPStateSessions SET Expires = @Expires " +
              "WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn);

            // SET
            cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value
                = DateTime.Now.AddMinutes(this._sessionStateConfig.Timeout.TotalMinutes);

            // WHERE
            cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;

            try
            {
                // 有効期限の更新処理のクエリ実行
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            //catch (SqlException e)
            catch (Exception e)
            {
                // 例外処理

                if (WriteExceptionsToEventLog)
                {
                    // イベント・ログの出力
                    WriteToEventLog(e, "ResetItemTimeout");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // 有効期限の更新処理の終了処理
                conn.Close();
            }
        }

        /// <summary>
        /// SessionStateProviderBase.CreateUninitializedItem メソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.createuninitializeditem.aspx
        /// 新しいSessionStateStoreProviderをデータ ストアに追加（cookieless→URL用）
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="id">セッション識別子</param>
        /// <param name="timeout">Timeout値</param>
        /// <remarks>
        /// ・cookieless
        /// 　http://msdn.microsoft.com/ja-jp/library/system.web.configuration.sessionstatesection.cookieless.aspx
        /// 　クライアント セッションを識別するのに Cookie を使用するかどうかを示す値。
        /// 　
        /// ・regenerateExpiredSessionId
        /// 　http://msdn.microsoft.com/ja-jp/library/system.web.configuration.sessionstatesection.regenerateexpiredsessionid.aspx
        /// 　有効期限が切れたセッションID がクライアントによって
        /// 　指定されている場合に、セッションID を再発行するかどうかを示す値
        /// 　セッション ID を再生成する必要がある場合は true。それ以外の場合は false。 既定値は false
        /// 　既定では、RegenerateExpiredSessionId が有効な場合、Cookieless URL のみ再発行される。
        /// 
        /// 属性が共にtrueの場合、
        /// 有効期限切れのSessionID値を含む要求を最初に行う際に呼び出される。
        /// 
        /// 有効期限切れのSessionIDに対してSessionID値を新規に作成し 
        /// 新規作成された SessionID値を含むURLにブラウザをリダイレクトする。
        /// </remarks>
        public override void CreateUninitializedItem(
            HttpContext context,
            string id,
            int timeout)
        {
            // 追加処理のクエリ準備
            SqlConnection conn = new SqlConnection(this.ConnectionString);

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO ASPStateSessions" +
                " (SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags)" +
                " Values(@SessionId, @ApplicationName, @Created, @Expires, @LockDate, @LockId , @Timeout, @Locked, @SessionItems, @Flags)", conn);

            // パラメタ指定

            // Values
            cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)timeout);
            cmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@Timeout", SqlDbType.Int).Value = timeout;
            cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = false;
            cmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, 0).Value = "";
            cmd.Parameters.Add("@Flags", SqlDbType.Int).Value = 1;

            // 削除処理はバック・グラウンドの処理で・・・

            try
            {
                // 追加処理のクエリ実行
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            //catch (SqlException e)
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateUninitializedItem");
                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // 追加処理の終了処理
                conn.Close();
            }
        }

        #endregion

        #region HTTPパイプライン

        /// <summary>
        /// SessionStateProviderBase.InitializeRequestイベント（HTTPパイプライン）
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.initializerequest.aspx
        /// SessionStateProviderが必要とする要求ごとの初期化を実行
        /// </summary>
        /// <param name="context">HttpContext</param>
        public override void InitializeRequest(HttpContext context)
        {
        }

        /// <summary>
        /// SessionStateProviderBase.EndRequestイベント（HTTPパイプライン）
        /// http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.endrequest.aspx
        /// SessionStateProviderが必要とする要求ごとのクリーンアップを実行
        /// </summary>
        /// <param name="context">HttpContext</param>
        public override void EndRequest(HttpContext context)
        {
        }

        #endregion

        #endregion

        #endregion

        #region ユーティリティ

        #region イベント・ログ

        /// <summary>
        /// イベント・ログに例外詳細を書く補助機能
        /// 
        /// 個人のデータベース詳細がブラウザに返されないことを保証するために、
        /// 例外はセキュリティ対策としてイベント・ログに書かれている。
        /// </summary>
        /// <param name="e">例外</param>
        /// <param name="action">アクション</param>
        private void WriteToEventLog(Exception e, string action)
        {
            // ログ・インスタンス
            EventLog log = new EventLog();

            log.Source = EVENT_SOURCE;
            log.Log = EVENT_LOG;

            // メッセージ作成
            string message =
              "An exception occurred communicating with the data source.\r\n\r\n";

            message += "Action: " + action + "\r\n\r\n";
            message += "Exception: " + e.ToString();

            // 書き込み
            log.WriteEntry(message);
        }

        #endregion

        #region データ削除処理

        /// <summary>
        /// セッション・データの削除処理の起動メソッド
        /// （Initializeから一回だけ呼ばれる）
        /// </summary>
        private void DeleteExpireSessionPeriodicallyStart()
        {
            //// トレース・ログの出力
            //System.Diagnostics.Trace.WriteLine(
            //    "DeleteExpireSessionUtil.DeleteExpireSessionPeriodicallyStart() : " + this.ConnectionString);

            // バックグラウンド・スレッド起動

            // タイマー（60秒間隔）でゴミ掃除
            Timer t = new System.Timers.Timer(60000);

            // 削除処理のタイマー・スレッド関数を設定
            t.Elapsed += new ElapsedEventHandler(this.DeleteExpireSession);

            // タイマーの有効化
            t.Enabled = true;
        }

        // DeleteExpireSession重複起動とは、
        // ユーザから呼ばれた場合？

        // System.Timers.Timerで呼ばれた場合は、
        // 重複起動にはならないと考える。

        /// <summary>
        /// 重複起動制御用のフラグ
        /// </summary>
        private bool isCurrentExecuting = false;

        /// <summary>
        /// 重複起動制御用の
        /// クリティカル・セクション・オブジェクト
        /// </summary>
        private object syncObject = new object();

        /// <summary>
        /// セッション・データの削除処理
        /// </summary>
        private void DeleteExpireSession(object sender, ElapsedEventArgs e)
        {
            //// トレース・ログの出力
            //System.Diagnostics.Trace.WriteLine(
            //    "DeleteExpireSessionUtil.DeleteExpireSession()");

            // クリティカル・セクション
            lock (syncObject)
            {
                if (isCurrentExecuting == true)
                {
                    // 入っている → 戻す
                    return;
                }
                else
                {
                    // 入っていない → 入った
                    isCurrentExecuting = true;
                }
            }

            // 削除処理のクエリ準備
            string commandString = "DELETE FROM ASPStateSessions WHERE Expires < @Expires";

            SqlConnection conn = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand(commandString, conn);

            // WHERE
            cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now;

            try
            {
                // 削除処理のクエリ実行
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // 例外処理

                // トレース・ログの出力
                Trace.WriteLine(
                    "DeleteExpireSessionUtil.DeleteExpireSession() : Exception Raised." + ex.ToString());

                // イベント・ログの出力
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(ex, "DeleteExpireSession");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // 削除処理の終了処理
                conn.Close();

                // クリティカル・セクション
                lock (syncObject)
                {
                    // 入った → 出た。
                    isCurrentExecuting = false;
                }
            }
        }

        #endregion

        #region Serialize、Deserialize

        /// <summary>
        /// Serialize
        /// （SessionStateItemCollection → Base64）
        /// </summary>
        /// <param name="items">アイテム</param>
        /// <returns>Base64</returns>
        private string Serialize(SessionStateItemCollection items)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    if (items != null)
                        items.Serialize(writer);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserialize
        /// （Base64 → SessionStateItemCollection → SessionStateStoreData）
        /// </summary>
        /// <param name="context">コンテキスト</param>
        /// <param name="serializedItems">Base64</param>
        /// <param name="timeout">Timeout値</param>
        /// <returns>オブジェクト</returns>
        private SessionStateStoreData Deserialize(
            HttpContext context,
            string serializedItems,
            int timeout)
        {
            SessionStateItemCollection sessionItems = null;

            using (MemoryStream ms =
              new MemoryStream(Convert.FromBase64String(serializedItems)))
            {
                sessionItems = new SessionStateItemCollection();

                if (ms.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        sessionItems = SessionStateItemCollection.Deserialize(reader);
                    }
                }
            }

            return new SessionStateStoreData(sessionItems,
              SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        #endregion

        #endregion
    }
}
