'**********************************************************************************
'* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
' 
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：CstSqlSessionStateProvider
'* クラス日本語名  ：SQL Serverを使用したCustomSessionProvider
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2011/xx/xx  西野 大介         新規作成
'*  2012/04/05  西野 大介         \n → \r\n 化
'**********************************************************************************

Imports System
Imports System.IO
Imports System.Configuration
Imports System.Configuration.Provider
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Specialized
Imports System.Timers
Imports System.Diagnostics

Imports System.Web
Imports System.Web.SessionState
Imports System.Web.Configuration

#Region "ORGコメント"

' 以下のサンプルを修正してSQL Azure用とした
' http://msdn.microsoft.com/ja-jp/library/ms178588.aspx

' 修正ポイント
' ・ODBC → SqlClientへの変更、
' ・それに伴うパラメタライズドクエリの記述修正
' ・１分間隔でデータベース上のExpiredセッションを削除する機能の追加

' 使い方
' ① このファイルをソリューションに含める
' ② SQL Azure上にセッションデータのテーブルを作成する
' ③ 構成設定を記述する

'
'Azure 環境へのテーブル作成
'
'CREATE TABLE ASPStateSessions
'(
'    SessionId       varchar(80)  NOT NULL,
'    ApplicationName varchar(255) NOT NULL,
'    Created         DateTime  NOT NULL,
'    Expires         DateTime  NOT NULL,
'    LockDate        DateTime  NOT NULL,
'    LockId          Integer   NOT NULL,
'    Timeout         Integer   NOT NULL,
'    Locked          Bit     NOT NULL,
'    SessionItems    Text,
'    Flags           Integer   NOT NULL,
'        CONSTRAINT PKSessions PRIMARY KEY (SessionId, ApplicationName)
')
'
'構成設定の記述
'
'<configuration>
'  <connectionStrings>
'    <add name="SqlSessionStoreConnectionString" connectionString="xxxx" />
'  </connectionStrings>
'
'  <system.web>
'    <sessionState 
'      regenerateExpiredSessionId="true"
'      mode="Custom"
'      customProvider="CstSqlSessionStateProvider" timeout="1">
'      <providers>
'        <add name="CstSqlSessionStateProvider"
'          type="CustomSessionProvider.CstSqlSessionStateProvider"
'          connectionStringName="SqlSessionStoreConnectionString"
'          writeExceptionsToEventLog="true" />
'      </providers>
'    </sessionState>
'  </system.web>
'</configuration>
'
'

#End Region

#Region "概要"

' ＜ SessionStateModule ＞
' セッションは、SessionStateModuleクラスで管理されます。

' このクラスは、SessionStateStoreProviderを呼び出し、
' 要求の間に異なるタイミングでセッション データを
' 読み取り、セッション データ ストアに書き込みます。

' ①　要求の最初に、
' 　　SessionStateModuleは、
' 　　GetItemExclusiveメソッドを呼び出して
' 　　セッション データ ストアからデータを取得します。

' 　　または、EnableSessionStateページ属性がReadOnlyに
' 　　設定されている場合は、GetItemメソッドを呼び出します。

' ②　要求の最後に、
' 　　SessionStateModuleは（セッションが更新された場合）、
' 　　SetAndReleaseItemExclusiveメソッドを呼び出し、
' 　　更新した値をセッション データ ストアにデータを書き込みます。

' SessionStateModuleは
' SessionStateStoreProviderBaseのメンバを
' 呼び出し、新規セッションを初期化します。

' また、HttpSessionState.Abandonメソッドが呼び出されたときは、
' セッション データ ストアのセッション データを削除します。

#End Region

#Region "補足"

' ＜カスタムのSessionID＞
' SessionStateModuleは、
' SessionStateStoreProviderに依存せずに、
' 単独でSessionID値を決定します。

' 必要に応じてカスタムのSessionIDManagerを実装するには、
' ISessionIDManagerインターフェイスを継承するクラスを作成します。 
' 詳細については、「ISessionIDManager」の「解説」を参照してください。 
' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.isessionidmanager.aspx

' ＜偽装してSessionストアにアクセス＞
' セキュリティで保護されたセッション データ ストアにアクセスする場合、
' SessionStateModuleはASP.NETのプロセスIDに戻リます。 
' <sessionState>構成要素のuseHostingIdentity属性をfalseに設定すると、
' IISによって提供されるIDをSessionStateModuleが偽装するように指定できます。

' 例えば、Windowsの統合セキュリティを
' 使用するようにIISアプリケーションを構成し、
' IISによって提供されるIDをセッション管理の
' ために偽装するときは、アプリケーションの
' Web.configファイルの<system.web>構成セクションに
' <identity impersonate="true" /> と指定し、
' <sessionState> 構成要素のuseHostingIdentity属性をfalseに設定します。

' useHostingIdentity属性がtrueの場合、
' セッション データ ストアに接続する際に、
' ASP.NETはプロセスID、つまり
' <identity>構成要素に提供される
' ユーザ資格情報 (存在する場合) を偽装します。

' ASP.NETプロセスIDの詳細については、
' 「ASP.NET プロセス ID の構成」および
' 「ASP.NET の偽装」を参照してください。

#End Region

#Region "ロック"

#Region "概要"

' ASP.NETアプリケーションは
' マルチスレッドをサポートしているため、
' 同時実行された複数の要求に応答できます。

' 同時実行された複数の要求が、
' 同じセッション情報にアクセスする場合があります。

' フレームセット内の複数のフレームがすべて同じ
' アプリケーションのASP.NET Webページ
' を参照するシナリオを考えてみます。

' フレームセット内の各フレームの個々の要求は、
' Webサーバの異なるスレッドで同時に実行される場合があります。
' 各フレームのASP.NETページがセッションにアクセスすると、
' 複数のスレッドが同時にセッション データ ストアにアクセスする可能性があります。

' セッション データ ストアでのデータの競合や予期不可能な動作を回避
' するため、SessionStateModuleとSessionStateStoreProviderBase
' には、ASP.NET ページの実行時にセッション データ ストアの
' 特定の項目を排他的にロックする機能が含まれます。

#End Region

#Region "詳細"

' EnableSessionState属性がReadOnlyとして
' マークされているときは、セッション データ ストア
' 項目にロックは設定されません（GetItemメソッド）。

' ただし、同じアプリケーションの他のASP.NETページが
' セッション データ ストアに書き込むことができる可能性があるため、
' この場合、セッション データ ストアからの読み取り専用セッション データ
' に対する要求はロックされたデータが解放されるのを待つことになります。 

' ロックは、GetItemExclusiveメソッドの呼び出し時に、
' セッション データ ストア項目に設定されます。 

' 要求が完了すると、このロックは
' SetAndReleaseItemExclusiveメソッド
' の呼び出し時に解放されます。 

' GetItemExclusive or GetItemメソッドの呼び出し時に、
' ロックされたセッション データ ストア項目を検出した場合、
' SessionStateModuleはセッション データ ストア項目のロックが解放されるか、
' ExecutionTimeoutプロパティの値に指定された時間を超えるまで、
' 0.5 秒間隔でセッション データ ストア項目を再要求します。

' 要求がExecutionTimeoutした場合、
' SessionStateModuleはReleaseItemExclusiveを
' 呼び出してセッション データ ストア項目を解放します。

' ロックされたセッション データ ストア項目は、
' 現在の応答に対するSetAndReleaseItemExclusiveメソッドを呼び出す前に、
' 別のスレッド上でReleaseItemExclusiveメソッドを呼び出して
' 解放されている場合があります（ExecutionTimeout時の動作）。 

' SessionStateModuleは、
' 別のセッションで既に解放されて変更された
' セッション データ ストア項目を
' 設定および解放する可能性があるため、
' SessionStateModuleには、ロックされた
' セッション データ ストア項目を変更する
' ためのロック識別子が要求ごとに含みます。

' セッション データ ストアは、
' セッション データ ストア項目のロック識別子が
' 提供されたロック識別子と一致した場合にのみ更新されます。

#End Region

#End Region

Namespace Touryo.Infrastructure.Business.Csp
	''' <summary>
	''' CstSqlSessionStateProvider
	''' SQL Serverを使用したSessionStateStoreProvider
	''' </summary>
	''' <remarks>
	''' SessionStateStoreProviderの実装
	''' http://msdn.microsoft.com/ja-jp/library/ms178587.aspx
	''' SessionStateStoreProviderBase
	''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.aspx
	''' </remarks>
	Public NotInheritable Class CstSqlSessionStateProvider
		Inherits SessionStateStoreProviderBase
		#Region "private"

		''' <summary>SessionStateSection</summary>
		Private _sessionStateConfig As SessionStateSection = Nothing

		''' <summary>ConnectionStringSettings</summary>
		Private _connectionStringSettings As ConnectionStringSettings = Nothing

		''' <summary>接続文字列</summary>
		Private ConnectionString As String = ""

		#End Region

		#Region "const"

		''' <summary>
		''' ログ定数：ソース
		''' </summary>
		Private Const EVENT_SOURCE As String = "CstSqlSessionStateProvider"

		''' <summary>
		''' ログ定数：ログ
		''' </summary>
		Private Const EVENT_LOG As String = "Application"

		''' <summary>
		''' MSG定数：設定周りの例外が発生した場合。
		''' </summary>
		Private Const EXCEPTION_MESSAGE As String = "An exception occurred. Please contact your administrator."

		#End Region

		#Region "プロパティ"

		''' <summary>
		''' イベントログの有効・無効プロパティ
		''' </summary>
		''' <remarks>
		''' false：例外は来訪者に投げられます。
		''' true ：例外はイベント・ログに書かれています。
		''' </remarks>
		Private _writeExceptionsToEventLog As Boolean = False

		''' <summary>
		''' イベントログの有効・無効プロパティ・プロシージャ
		''' </summary>
		Public Property WriteExceptionsToEventLog() As Boolean
			Get
				Return Me._writeExceptionsToEventLog
			End Get
			Set
				Me._writeExceptionsToEventLog = value
			End Set
		End Property

		''' <summary>
		''' アプリケーション名
		''' （ApplicationVirtualPath）
		''' </summary>
		''' <remarks>
		''' キーにする。
		''' 異なるアプリケーションでは
		''' SessionIDが重複するため。
		''' </remarks>
		Private _applicationName As String

		''' <summary>
		''' アプリケーション名
		''' （ApplicationVirtualPath）
		''' プロパティ・プロシージャ
		''' </summary>
		Public ReadOnly Property ApplicationName() As String
			Get
				Return Me._applicationName
			End Get
		End Property

		#End Region

		#Region "SessionStateStoreProviderBaseメンバ"

		#Region "初期化・終了"

		''' <summary>
		''' ProviderBase.Initialize メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.configuration.provider.providerbase.initialize.aspx
		''' SessionStateStoreProviderを初期化する。
		''' web.configのsessionStateのprovidersのadd要素から初期化
		''' </summary>
		''' <param name="name">名称属性</param>
		''' <param name="config">追加属性の取得</param>
		Public Overrides Sub Initialize(name As String, config As NameValueCollection)
			' web.configのsessionStateの
			' providersのadd要素から初期化する。

			' config引数がnullの時、
			If config Is Nothing Then
				' 例外
				Throw New ArgumentNullException("config")
			End If

			' 名称属性がNullOrEmptyの時、
			If String.IsNullOrEmpty(name) Then
				' デフォルトの設定
				name = "CstSqlSessionStateProvider"
			End If

			' 追加属性（description）がNullOrEmptyの時、
			If [String].IsNullOrEmpty(config("description")) Then
				' デフォルトの設定
				config.Remove("description")
				config.Add("description", "Custom Sql Session State Provider")
			End If

			' ベースの初期化
			MyBase.Initialize(name, config)

			' アプリケーション名
			Me._applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath

			' <sessionState> configuration element.
			Dim configuration As Configuration = WebConfigurationManager.OpenWebConfiguration(ApplicationName)

			Me._sessionStateConfig = DirectCast(configuration.GetSection("system.web/sessionState"), SessionStateSection)

			' 接続文字列を初期化
			Me._connectionStringSettings = ConfigurationManager.ConnectionStrings(config("connectionStringName"))

			If Me._connectionStringSettings Is Nothing OrElse Me._connectionStringSettings.ConnectionString.Trim() = "" Then
				Throw New ProviderException("Connection string cannot be blank.")
			End If

			Me.ConnectionString = Me._connectionStringSettings.ConnectionString

			' ログの出力
			Me._writeExceptionsToEventLog = False

			If config("writeExceptionsToEventLog") IsNot Nothing Then
				If config("writeExceptionsToEventLog").ToUpper() = "TRUE" Then
					Me._writeExceptionsToEventLog = True
				End If
			End If

			' 定期的にデータを削除するスレッドを起動
			DeleteExpireSessionPeriodicallyStart()
		End Sub

		''' <summary>
		''' SessionStateProviderBase.SetItemExpireCallback メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.setitemexpirecallback.aspx
		''' Global.asax ファイルに定義されている Session_OnEnd イベント
		''' の SessionStateItemExpireCallback デリゲートへの参照を設定。
		''' </summary>
		''' <param name="expireCallback">
		''' SessionStateItemExpireCallback デリゲート
		''' </param>
		''' <returns>
		''' SessionStateStoreProviderが Session_OnEnd イベントの
		''' 呼び出しをサポートする場合は true。それ以外の場合は false。
		''' </returns>
		Public Overrides Function SetItemExpireCallback(expireCallback As SessionStateItemExpireCallback) As Boolean
			' Session_OnEnd イベントの呼び出しをサポートしない。
			Return False
		End Function

		''' <summary>
		''' IDisposable.Dispose メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.idisposable.dispose.aspx
		''' IDisposableの実装
		''' </summary>
		Public Overrides Sub Dispose()
			' 特になし。
		End Sub

		#End Region

		#Region "新規セッション"

		''' <summary>
		''' SessionStateStoreProviderBase.CreateNewStoreData メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.createnewstoredata.aspx
		''' 現在の要求で使用するSessionStateStoreDataオブジェクトを新規作成 
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="timeout">セッション状態のTimeout値</param>
		''' <returns>
		''' 現在の要求に関する新しいSessionStateStoreData
		''' </returns>
		''' <remarks>
		''' SessionStateModuleはASP.NETページの
		''' 要求の開始時のAcquireRequestStateイベントで
		''' 
		''' ・受信した要求にセッションID がない場合
		''' 
		''' ・受信した要求にセッションID はあるが、
		''' 　データ ストアにセッションが見つからない場合
		''' 　
		''' CreateNewStoreDataメソッドを呼び出す。
		''' </remarks>
		Public Overrides Function CreateNewStoreData(context As HttpContext, timeout As Integer) As SessionStateStoreData
			' お決まりのコード？
			Return New SessionStateStoreData(New SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout)
		End Function

		#End Region

		#Region "セッション データ ストアの読み・書き"

		#Region "セッション データ ストアの書き込み"

		''' <summary>
		''' SessionStateProviderBase.SetAndReleaseItemExclusive メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.setandreleaseitemexclusive.aspx
		''' 現在の要求の値を使用してセッション データ ストア項目
		''' の情報を更新し、データのロックを解除する。
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="id">セッション識別子</param>
		''' <param name="item">SessionStateStoreData</param>
		''' <param name="lockId">ロック識別子</param>
		''' <param name="newItem">
		''' セッション項目を
		''' ・新しい項目に指定する場合は：true
		''' ・既存の項目に指定する場合は：false
		''' </param>
		Public Overrides Sub SetAndReleaseItemExclusive(context As HttpContext, id As String, item As SessionStateStoreData, lockId As Object, newItem As Boolean)
			' セッション・コレクションをシリアライズ
			Dim sessItems As String = Serialize(DirectCast(item.Items, SessionStateItemCollection))

			' データプロバイダ
			Dim conn As New SqlConnection(Me.ConnectionString)
			Dim deleteCmd As SqlCommand = Nothing
			Dim insertCmd As SqlCommand = Nothing

			If newItem Then
				' 新しい項目（del→ins）の保存処理

				' 既存のセッションを消す
				'（有効期限切れであること）
				deleteCmd = New SqlCommand("DELETE FROM ASPStateSessions" & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND Expires < @Expires", conn)

				' パラメタ指定

				' WHERE
				deleteCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
				deleteCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
				deleteCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now

				' 新しい項目を追加
				insertCmd = New SqlCommand("INSERT INTO ASPStateSessions" & " (SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags)" & " Values" & " (@SessionId, @ApplicationName, @Created, @Expires, @LockDate, @LockId , @Timeout, @Locked, @SessionItems, @Flags)", conn)

				' パラメタ指定

				' Values
				insertCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
				insertCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
				insertCmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now
				insertCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(CType(item.Timeout, [Double]))
				insertCmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value = DateTime.Now
				insertCmd.Parameters.Add("@LockId", SqlDbType.Int).Value = 0
				insertCmd.Parameters.Add("@Timeout", SqlDbType.Int).Value = item.Timeout
				insertCmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = False
				insertCmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, sessItems.Length).Value = sessItems
				insertCmd.Parameters.Add("@Flags", SqlDbType.Int).Value = 0
			Else
				' 既存の項目（upd）の保存処理

				' ロック識別子を確認して更新（合わせてロックを解放）
				insertCmd = New SqlCommand("UPDATE ASPStateSessions" & " SET Expires = @Expires, SessionItems = @SessionItems, Locked = @Locked" & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn)

				' パラメタ指定

				' SET
				insertCmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(CType(item.Timeout, [Double]))
				insertCmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, sessItems.Length).Value = sessItems
				insertCmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = False

				' WHERE
				insertCmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
				insertCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
				insertCmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId
			End If

			Try
				' 保存処理の実行
				conn.Open()

				' あるときだけ
				If deleteCmd IsNot Nothing Then
					deleteCmd.ExecuteNonQuery()
				End If

				insertCmd.ExecuteNonQuery()
			Catch e As SqlException
				' ★★有効期限切れでない新規項目のインサート失敗は考えられるが・・・。

				If WriteExceptionsToEventLog Then
					WriteToEventLog(e, "SetAndReleaseItemExclusive")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				' 保存処理の終了処理
				conn.Close()
			End Try
		End Sub

		#End Region

		#Region "セッション データ ストアの読み込み"

		''' <summary>
		''' SessionStateProviderBase.GetItemExclusive メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.getitemexclusive.aspx
		''' セッション データ ストアから読み取り専用の
		''' SessionStateStoreDataを返す。
		''' （通常は、こちらのメソッドが使用される）
		''' </summary>
		''' <param name="context">
		''' HttpContext
		''' </param>
		''' <param name="id">
		''' セッション識別子
		''' </param>
		''' <param name="locked">
		''' 要求したセッション項目が
		''' セッション データ ストアで
		''' ・ロックされているときはブール値 true
		''' ・それ以外の場合は false
		''' </param>
		''' <param name="lockAge">
		''' セッション データ ストアの項目がロック
		''' された時間に設定された TimeSpan オブジェクト
		''' </param>
		''' <param name="lockId">
		''' ロック識別子
		''' </param>
		''' <param name="actionFlags">
		''' 現在のセッションが初期化されていない
		''' cookieless セッションかどうかを示す
		''' SessionStateActions 値
		''' </param>
		''' <returns>
		''' SessionStateStoreData
		''' </returns>
		Public Overrides Function GetItemExclusive(context As HttpContext, id As String, ByRef locked As Boolean, ByRef lockAge As TimeSpan, ByRef lockId As Object, ByRef actionFlags As SessionStateActions) As SessionStateStoreData
			' GetSessionStoreItemへ
			' ture
			Return GetSessionStoreItem(True, context, id, locked, lockAge, lockId, _
				actionFlags)
		End Function

		''' <summary>
		''' SessionStateProviderBase.GetItem メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.getitem.aspx
		''' セッション データ ストアから
		''' 読み取り専用のSessionStateStoreDataを返す。
		''' （EnableSessionState属性がReadOnly
		''' の場合こちらのメソッドが使用される）
		''' </summary>
		''' <param name="context">
		''' HttpContext
		''' </param>
		''' <param name="id">
		''' セッション識別子
		''' </param>
		''' <param name="locked">
		''' 要求したセッション項目が
		''' セッション データ ストアで
		''' ・ロックされているときはブール値 true
		''' ・それ以外の場合は false
		''' </param>
		''' <param name="lockAge">
		''' セッション データ ストアの項目がロック
		''' された時間に設定された TimeSpan オブジェクト
		''' </param>
		''' <param name="lockId">
		''' ロック識別子
		''' </param>
		''' <param name="actionFlags">
		''' 現在のセッションが初期化されていない
		''' cookieless セッションかどうかを示す
		''' SessionStateActions 値
		''' </param>
		''' <returns>
		''' SessionStateStoreData
		''' </returns>
		Public Overrides Function GetItem(context As HttpContext, id As String, ByRef locked As Boolean, ByRef lockAge As TimeSpan, ByRef lockId As Object, ByRef actionFlags As SessionStateActions) As SessionStateStoreData
			' GetSessionStoreItemへ
			' false
			Return GetSessionStoreItem(False, context, id, locked, lockAge, lockId, _
				actionFlags)
		End Function

		''' <summary>
		''' GetItemおよびGetItemExclusiveの両方によって呼ばれ、
		''' セッション データ ストアを検索する。
		''' lockRecordパラメーターがtrueの場合
		''' (GetItemExclusiveから呼ばれた場合)、
		''' セッション データ ストア項目をロックし、
		''' 新しいLockIdおよびLockDateをセットする。
		''' </summary>
		''' <param name="lockRecord">
		''' GetItemExclusiveから呼ばれた場合、true
		''' GetItemから呼ばれた場合、false
		''' </param>
		''' <param name="context">
		''' HttpContext
		''' </param>
		''' <param name="id">
		''' セッション識別子
		''' </param>
		''' <param name="locked">
		''' 要求したセッション項目が
		''' セッション データ ストアで
		''' ・ロックされているときはブール値 true
		''' ・それ以外の場合は false
		''' </param>
		''' <param name="lockAge">
		''' セッション データ ストアの項目がロック
		''' された時間に設定された TimeSpan オブジェクト
		''' </param>
		''' <param name="lockId">
		''' ロック識別子
		''' </param>
		''' <param name="actionFlags">
		''' 現在のセッションが初期化されていない
		''' cookieless セッションかどうかを示す
		''' SessionStateActions 値
		''' </param>
		''' <returns>
		''' SessionStateStoreData
		''' </returns>
		Private Function GetSessionStoreItem(lockRecord As Boolean, context As HttpContext, id As String, ByRef locked As Boolean, ByRef lockAge As TimeSpan, ByRef lockId As Object, _
			ByRef actionFlags As SessionStateActions) As SessionStateStoreData
			' Initial values for return value and out parameters.
			Dim item As SessionStateStoreData = Nothing
			lockAge = TimeSpan.Zero
			lockId = Nothing
			locked = False
			actionFlags = 0

			' Sql database connection.
			Dim conn As New SqlConnection(Me.ConnectionString)
			' SqlCommand for database commands.
			Dim cmd As SqlCommand = Nothing
			' DataReader to read database record.
			Dim reader As SqlDataReader = Nothing
			' DateTime to check if current session item is expired.
			Dim expires As DateTime
			' String to hold serialized SessionStateItemCollection.
			Dim serializedItems As String = ""
			' True if a record is found in the database.
			Dim foundRecord As Boolean = False
			' True if the returned session item is expired and needs to be deleted.
			Dim deleteData As Boolean = False
			' Timeout value from the data store.
			Dim timeout As Integer = 0

			Try
				conn.Open()


				If lockRecord Then
					cmd = New SqlCommand("UPDATE ASPStateSessions SET" & " Locked = @Locked, LockDate = @LockDate " & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND Locked = @Locked2 AND Expires > @Expires", conn)
					cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = True
					cmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value = DateTime.Now
					cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
					cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
					cmd.Parameters.Add("@Locked2", SqlDbType.Int).Value = False
					cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now

					If cmd.ExecuteNonQuery() = 0 Then
						' No record was updated because the record was locked or not found.
						locked = True
					Else
						' The record was updated.

						locked = False
					End If
				End If

				' Retrieve the current session item information.
				cmd = New SqlCommand("SELECT Expires, SessionItems, LockId, LockDate, Flags, Timeout " & "  FROM ASPStateSessions " & "  WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn)
				cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
				cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName

				' Retrieve session item data from the data source.
				reader = cmd.ExecuteReader(CommandBehavior.SingleRow)
				While reader.Read()
					expires = reader.GetDateTime(0)

					If expires < DateTime.Now Then
						' The record was expired. Mark it as not locked.
						locked = False
						' The session was expired. Mark the data for deletion.
						deleteData = True
					Else
						foundRecord = True
					End If

					serializedItems = reader.GetString(1)
					lockId = reader.GetInt32(2)
					lockAge = DateTime.Now.Subtract(reader.GetDateTime(3))
					actionFlags = DirectCast(reader.GetInt32(4), SessionStateActions)
					timeout = reader.GetInt32(5)
				End While
				reader.Close()


				' If the returned session item is expired, 
				' delete the record from the data source.
				If deleteData Then
					cmd = New SqlCommand("DELETE FROM ASPStateSessions " & "WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn)
					cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
					cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName

					cmd.ExecuteNonQuery()
				End If

				' The record was not found. Ensure that locked is false.
				If Not foundRecord Then
					locked = False
				End If

				' If the record was found and you obtained a lock, then set 
				' the lockId, clear the actionFlags,
				' and create the SessionStateStoreItem to return.
				If foundRecord AndAlso Not locked Then
					lockId = CInt(lockId) + 1

					cmd = New SqlCommand("UPDATE ASPStateSessions SET" & " LockId = @LockId, Flags = 0 " & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn)
					cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId
					cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
					cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName

					cmd.ExecuteNonQuery()

					' If the actionFlags parameter is not InitializeItem, 
					' deserialize the stored SessionStateItemCollection.
					If actionFlags = SessionStateActions.InitializeItem Then
						item = CreateNewStoreData(context, Convert.ToInt32(Me._sessionStateConfig.Timeout.TotalMinutes))
					Else
						item = Deserialize(context, serializedItems, timeout)
					End If
				End If
			Catch e As SqlException
				If WriteExceptionsToEventLog Then
					WriteToEventLog(e, "GetSessionStoreItem")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				If reader IsNot Nothing Then
					reader.Close()
				End If
				conn.Close()
			End Try

			Return item
		End Function

		#End Region

		#End Region

		#Region "その他、フレームワークから呼び出されるメソッド"

		#Region "Abandonによる削除"

		''' <summary>
		''' SessionStateProviderBase.RemoveItem メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.removeitem.aspx
		''' セッション データ ストアから項目データを削除する
		''' （Abandonメソッドが呼び出されている場合）。
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="id">セッション識別子</param>
		''' <param name="lockId">ロック識別子</param>
		''' <param name="item">SessionStateStoreData</param>
		Public Overrides Sub RemoveItem(context As HttpContext, id As String, lockId As Object, item As SessionStateStoreData)
			' Abandon処理のクエリ準備
			Dim conn As New SqlConnection(Me.ConnectionString)

			' ロック識別子まで要る。
			Dim cmd As New SqlCommand("DELETE FROM ASPStateSessions" & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn)

			' パラメタ指定

			' WHERE
			cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
			cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
			cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId

			Try
				' Abandon処理のクエリ実行
				conn.Open()
				cmd.ExecuteNonQuery()
			'catch (SqlException e)
			Catch e As Exception
				If WriteExceptionsToEventLog Then
					WriteToEventLog(e, "RemoveItem")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				' Abandon処理の終了処理
				conn.Close()
			End Try
		End Sub

		#End Region

		#Region "例外的ケース"

		''' <summary>
		''' SessionStateProviderBase.ReleaseItemExclusive メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.releaseitemexclusive.aspx
		''' セッション データ ストアの項目のロックを解除する
		''' （ロックが ExecutionTimeout 値を超えている場合）。
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="id">セッション識別子</param>
		''' <param name="lockId">ロック識別子</param>
		Public Overrides Sub ReleaseItemExclusive(context As HttpContext, id As String, lockId As Object)
			' リリース処理のクエリ準備
			' ロック解放とスライディング
			Dim conn As New SqlConnection(Me.ConnectionString)

			Dim cmd As New SqlCommand("UPDATE ASPStateSessions" & " SET Locked = 0, Expires = @Expires" & " WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName AND LockId = @LockId", conn)

			' パラメタ指定

			' SET
			cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(Me._sessionStateConfig.Timeout.TotalMinutes)

			' WHERE
			cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
			cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
			cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = lockId

			Try
				' リリース処理のクエリ実行
				conn.Open()
				cmd.ExecuteNonQuery()
			Catch e As SqlException
				If WriteExceptionsToEventLog Then
					WriteToEventLog(e, "ReleaseItemExclusive")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				' リリース処理の終了処理
				conn.Close()
			End Try
		End Sub

		''' <summary>
		''' SessionStateProviderBase.ResetItemTimeout メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.resetitemtimeout.aspx
		''' セッション データ ストアの項目の有効期限の日時を更新
		''' （エラー時や、EnableSessionState = false時のスライディング処理用）。
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="id">セッション識別子</param>
		''' <remarks>
		''' EnableSessionState属性がtrueの場合で、
		''' 
		''' エラーが発生したために要求が
		''' ・AcquireRequestState
		''' ・ReleaseRequestState
		''' イベントを生成しなかった場合
		''' 
		''' または、EnableSessionState属性がfalseに設定されている場合にも、
		''' ASP.NETページが要求されるとResetItemTimeoutメソッドが呼び出され、
		''' セッション データ ストアのデータの有効期限の日時が更新されます。
		''' </remarks>
		Public Overrides Sub ResetItemTimeout(context As HttpContext, id As String)
			' 有効期限の更新処理のクエリ準備
			Dim conn As New SqlConnection(Me.ConnectionString)

			Dim cmd As New SqlCommand("UPDATE ASPStateSessions SET Expires = @Expires " & "WHERE SessionId = @SessionId AND ApplicationName = @ApplicationName", conn)

			' SET
			cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(Me._sessionStateConfig.Timeout.TotalMinutes)

			' WHERE
			cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
			cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName

			Try
				' 有効期限の更新処理のクエリ実行
				conn.Open()
				cmd.ExecuteNonQuery()
			'catch (SqlException e)
			Catch e As Exception
				' 例外処理

				If WriteExceptionsToEventLog Then
					' イベント・ログの出力
					WriteToEventLog(e, "ResetItemTimeout")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				' 有効期限の更新処理の終了処理
				conn.Close()
			End Try
		End Sub

		''' <summary>
		''' SessionStateProviderBase.CreateUninitializedItem メソッド
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.createuninitializeditem.aspx
		''' 新しいSessionStateStoreProviderをデータ ストアに追加（cookieless→URL用）
		''' </summary>
		''' <param name="context">HttpContext</param>
		''' <param name="id">セッション識別子</param>
		''' <param name="timeout">Timeout値</param>
		''' <remarks>
		''' ・cookieless
		''' 　http://msdn.microsoft.com/ja-jp/library/system.web.configuration.sessionstatesection.cookieless.aspx
		''' 　クライアント セッションを識別するのに Cookie を使用するかどうかを示す値。
		''' 　
		''' ・regenerateExpiredSessionId
		''' 　http://msdn.microsoft.com/ja-jp/library/system.web.configuration.sessionstatesection.regenerateexpiredsessionid.aspx
		''' 　有効期限が切れたセッションID がクライアントによって
		''' 　指定されている場合に、セッションID を再発行するかどうかを示す値
		''' 　セッション ID を再生成する必要がある場合は true。それ以外の場合は false。 既定値は false
		''' 　既定では、RegenerateExpiredSessionId が有効な場合、Cookieless URL のみ再発行される。
		''' 
		''' 属性が共にtrueの場合、
		''' 有効期限切れのSessionID値を含む要求を最初に行う際に呼び出される。
		''' 
		''' 有効期限切れのSessionIDに対してSessionID値を新規に作成し 
		''' 新規作成された SessionID値を含むURLにブラウザをリダイレクトする。
		''' </remarks>
		Public Overrides Sub CreateUninitializedItem(context As HttpContext, id As String, timeout As Integer)
			' 追加処理のクエリ準備
			Dim conn As New SqlConnection(Me.ConnectionString)

			Dim cmd As New SqlCommand("INSERT INTO ASPStateSessions" & " (SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags)" & " Values(@SessionId, @ApplicationName, @Created, @Expires, @LockDate, @LockId , @Timeout, @Locked, @SessionItems, @Flags)", conn)

			' パラメタ指定

			' Values
			cmd.Parameters.Add("@SessionId", SqlDbType.VarChar, 80).Value = id
			cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = ApplicationName
			cmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now
			cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(CType(timeout, [Double]))
			cmd.Parameters.Add("@LockDate", SqlDbType.DateTime).Value = DateTime.Now
			cmd.Parameters.Add("@LockId", SqlDbType.Int).Value = 0
			cmd.Parameters.Add("@Timeout", SqlDbType.Int).Value = timeout
			cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = False
			cmd.Parameters.Add("@SessionItems", SqlDbType.VarChar, 0).Value = ""
			cmd.Parameters.Add("@Flags", SqlDbType.Int).Value = 1

			' 削除処理はバック・グラウンドの処理で・・・

			Try
				' 追加処理のクエリ実行
				conn.Open()
				cmd.ExecuteNonQuery()
			'catch (SqlException e)
			Catch e As Exception
				If WriteExceptionsToEventLog Then
					WriteToEventLog(e, "CreateUninitializedItem")
					Throw New ProviderException(EXCEPTION_MESSAGE)
				Else
					Throw
				End If
			Finally
				' 追加処理の終了処理
				conn.Close()
			End Try
		End Sub

		#End Region

		#Region "HTTPパイプライン"

		''' <summary>
		''' SessionStateProviderBase.InitializeRequestイベント（HTTPパイプライン）
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.initializerequest.aspx
		''' SessionStateProviderが必要とする要求ごとの初期化を実行
		''' </summary>
		''' <param name="context">HttpContext</param>
		Public Overrides Sub InitializeRequest(context As HttpContext)
		End Sub

		''' <summary>
		''' SessionStateProviderBase.EndRequestイベント（HTTPパイプライン）
		''' http://msdn.microsoft.com/ja-jp/library/system.web.sessionstate.sessionstatestoreproviderbase.endrequest.aspx
		''' SessionStateProviderが必要とする要求ごとのクリーンアップを実行
		''' </summary>
		''' <param name="context">HttpContext</param>
		Public Overrides Sub EndRequest(context As HttpContext)
		End Sub

		#End Region

		#End Region

		#End Region

		#Region "ユーティリティ"

		#Region "イベント・ログ"

		''' <summary>
		''' イベント・ログに例外詳細を書く補助機能
		''' 
		''' 個人のデータベース詳細がブラウザに返されないことを保証するために、
		''' 例外はセキュリティ対策としてイベント・ログに書かれている。
		''' </summary>
		''' <param name="e">例外</param>
		''' <param name="action">アクション</param>
		Private Sub WriteToEventLog(e As Exception, action As String)
			' ログ・インスタンス
			Dim log As New EventLog()

			log.Source = EVENT_SOURCE
			log.Log = EVENT_LOG

			' メッセージ作成
			Dim message As String = "An exception occurred communicating with the data source." & vbCr & vbLf & vbCr & vbLf

			message += "Action: " & action & vbCr & vbLf & vbCr & vbLf
			message += "Exception: " & e.ToString()

			' 書き込み
			log.WriteEntry(message)
		End Sub

		#End Region

		#Region "データ削除処理"

		''' <summary>
		''' セッション・データの削除処理の起動メソッド
		''' （Initializeから一回だけ呼ばれる）
		''' </summary>
		Private Sub DeleteExpireSessionPeriodicallyStart()
            '/ トレース・ログの出力
            'Trace.WriteLine("DeleteExpireSessionUtil.DeleteExpireSessionPeriodicallyStart() : " & Me.ConnectionString)

            ' バックグラウンド・スレッド起動

            ' タイマー（60秒間隔）でゴミ掃除
            Dim t As Timer = New System.Timers.Timer(60000)

            ' 削除処理のタイマー・スレッド関数を設定
            AddHandler t.Elapsed, AddressOf Me.DeleteExpireSession

            ' タイマーの有効化
            t.Enabled = True
		End Sub

		' DeleteExpireSession重複起動とは、
		' ユーザから呼ばれた場合？

		' System.Timers.Timerで呼ばれた場合は、
		' 重複起動にはならないと考える。

		''' <summary>
		''' 重複起動制御用のフラグ
		''' </summary>
		Private isCurrentExecuting As Boolean = False

		''' <summary>
		''' 重複起動制御用の
		''' クリティカル・セクション・オブジェクト
		''' </summary>
		Private syncObject As New Object()

		''' <summary>
		''' セッション・データの削除処理
		''' </summary>
		Private Sub DeleteExpireSession(sender As Object, e As ElapsedEventArgs)
            '/ トレース・ログの出力
            'Trace.WriteLine("DeleteExpireSessionUtil.DeleteExpireSession()")

            ' クリティカル・セクション
            SyncLock syncObject
				If isCurrentExecuting = True Then
					' 入っている → 戻す
					Return
				Else
					' 入っていない → 入った
					isCurrentExecuting = True
				End If
			End SyncLock

			' 削除処理のクエリ準備
			Dim commandString As String = "DELETE FROM ASPStateSessions WHERE Expires < @Expires"

			Dim conn As New SqlConnection(Me.ConnectionString)
			Dim cmd As New SqlCommand(commandString, conn)

			' WHERE
			cmd.Parameters.Add("@Expires", SqlDbType.DateTime).Value = DateTime.Now

			Try
				' 削除処理のクエリ実行
				conn.Open()
				cmd.ExecuteNonQuery()
			Catch ex As Exception
				' 例外処理

				' トレース・ログの出力
				Trace.WriteLine("DeleteExpireSessionUtil.DeleteExpireSession() : Exception Raised." & ex.ToString())

				' イベント・ログの出力
				If WriteExceptionsToEventLog Then
					WriteToEventLog(ex, "DeleteExpireSession")
				Else
					Throw
				End If
			Finally
				' 削除処理の終了処理
				conn.Close()

				' クリティカル・セクション
				SyncLock syncObject
					' 入った → 出た。
					isCurrentExecuting = False
				End SyncLock
			End Try
		End Sub

		#End Region

		#Region "Serialize、Deserialize"

		''' <summary>
		''' Serialize
		''' （SessionStateItemCollection → Base64）
		''' </summary>
		''' <param name="items">アイテム</param>
		''' <returns>Base64</returns>
		Private Function Serialize(items As SessionStateItemCollection) As String
			Using ms As New MemoryStream()
				Using writer As New BinaryWriter(ms)
					If items IsNot Nothing Then
						items.Serialize(writer)
					End If
				End Using

				Return Convert.ToBase64String(ms.ToArray())
			End Using
		End Function

		''' <summary>
		''' Deserialize
		''' （Base64 → SessionStateItemCollection → SessionStateStoreData）
		''' </summary>
		''' <param name="context">コンテキスト</param>
		''' <param name="serializedItems">Base64</param>
		''' <param name="timeout">Timeout値</param>
		''' <returns>オブジェクト</returns>
		Private Function Deserialize(context As HttpContext, serializedItems As String, timeout As Integer) As SessionStateStoreData
			Dim sessionItems As SessionStateItemCollection = Nothing

			Using ms As New MemoryStream(Convert.FromBase64String(serializedItems))
				sessionItems = New SessionStateItemCollection()

				If ms.Length > 0 Then
					Using reader As New BinaryReader(ms)
						sessionItems = SessionStateItemCollection.Deserialize(reader)
					End Using
				End If
			End Using

			Return New SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout)
		End Function

		#End Region

		#End Region
	End Class
End Namespace
