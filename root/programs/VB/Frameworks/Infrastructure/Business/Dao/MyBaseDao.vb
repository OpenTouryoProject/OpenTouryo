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
'* クラス名        ：MyBaseDao
'* クラス日本語名  ：データアクセス親クラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2009/04/21  西野 大介         FrameworkExceptionの追加に伴い、実装変更
'*  2010/09/24  西野 大介         Damクラス内にユーザ情報を格納したので
'*  2012/06/14  西野 大介         SetSqlByFile2を追加（SetSqlByFile強化版）
'*                                ・sqlTextFilePathを自動連結
'*                                ・EmbeddedResourceLoaderに対応
'**********************************************************************************

Imports System.IO

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Dao
	''' <summary>データアクセス親クラス２（テンプレート）</summary>
	''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
	Public MustInherit Class MyBaseDao
		Inherits BaseDao
		''' <summary>埋め込まれたリソースを使用する</summary>
		Public Shared UseEmbeddedResource As Boolean = False

		''' <summary>SetSqlByFileの強化版メソッド</summary>
		''' <param name="sQLFileName">ファイル名</param>
		Public Sub SetSqlByFile2(sQLFileName As String)
			' SQLを設定する。
			If MyBaseDao.UseEmbeddedResource Then
				' 埋め込まれたリソースファイル
				Me.SetSqlByFile(GetConfigParameter.GetConfigValue("sqlTextFilePath") & "." & sQLFileName)
			Else
				' 通常のファイル
				Me.SetSqlByFile(Path.Combine(GetConfigParameter.GetConfigValue("sqlTextFilePath"), sQLFileName))
			End If
		End Sub

		''' <summary>
		''' 性能測定
		''' </summary>
		Private perfRec As PerformanceRecorder

		#Region "コンストラクタ"

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <remarks>
		''' コンストラクタは継承されないので、派生先で呼び出す必要がある。
		''' コンストラクタの実行順は、基本クラス→派生クラスの順
		''' ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
		''' 自由に利用できる。
		''' </remarks>
		Public Sub New(dam As BaseDam)
			MyBase.New(dam)
		End Sub

		#End Region

		#Region "開始・終了処理"

		''' <summary>SQL実行開始処理を実装する共通UOCメソッド</summary>
		''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_PreQuery()
			' 性能測定開始
			Me.perfRec = New PerformanceRecorder()
			Me.perfRec.StartsPerformanceRecord()
		End Sub

		''' <summary>SQL実行終了処理を実装する共通UOCメソッド（正常時）</summary>
		''' <param name="sql">実行したSQLの情報</param>
		''' <remarks>データ アクセス親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_AfterQuery(sql As String)
			' 性能測定終了
			Me.perfRec.EndsPerformanceRecord()

            ' SQLトレースログ出力

            ' ------------
            ' メッセージ部
            ' ------------
            ' 処理時間（実行時間）, 処理時間（CPU時間）, 実行したSQLの情報
            ' ------------
            Dim strLogMessage As String =
                Me.perfRec.ExecTime &
                "," & Me.perfRec.CpuTime & "," & sql

            ' Log4Netへログ出力
            If String.IsNullOrEmpty(GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG)) Then
                ' SQLトレースログ（OFF）
            ElseIf GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() = PubLiteral.[ON] Then
                LogIF.InfoLog("SQLTRACE", strLogMessage)
            ElseIf GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() = PubLiteral.OFF Then
                ' SQLトレースログ（OFF）
            Else
                ' パラメータ・エラー（書式不正）
                Throw New ArgumentException([String].Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_TRACELOG))
            End If

            ' ---

            ' 以下も、ログ出力で使用可能
            Dim obj As Object = Nothing

            ' UOC_Connection等で情報を設定しておく。
            ' UserInfoなどの情報を想定している。
            obj = Me.GetDam().Obj

            ' SQL実行時に情報が自動設定される。
            ' ・ExecSelectFill_DT
            '   DataTable
            ' ・ExecSelectFill_DS
            '   DataSet
            ' ・ExecSelect_DR
            '   IDataReader
            ' ・ExecSelectScalar
            '   object
            ' ・ExecInsUpDel_NonQuery
            '   int
            obj = Me.LogInfo
		End Sub

		''' <summary>SQL実行終了処理を実装する共通UOCメソッド（異常時）</summary>
		''' <param name="sql">実行したSQLの情報</param>
		''' <param name="ex">エラー情報</param>
		''' <remarks>データ アクセス親クラス１から利用される派生の末端</remarks>
		Protected Overrides Sub UOC_AfterQuery(sql As String, ex As Exception)
			' 性能測定終了
			Me.perfRec.EndsPerformanceRecord()

            ' SQLトレースログ出力

            ' ------------
            ' メッセージ部
            ' ------------
            ' 処理時間（実行時間）, 処理時間（CPU時間）, ユーザ名, 実行したSQLの情報
            ' ------------
            Dim strLogMessage As String =
                Me.perfRec.ExecTime &
                "," & Me.perfRec.CpuTime &
                "," & DirectCast(Me.GetDam().Obj, MyUserInfo).UserName & "," & sql

            ' Log4Netへログ出力
            If String.IsNullOrEmpty(GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG)) Then
                ' SQLトレースログ（OFF）
            ElseIf GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() = PubLiteral.[ON] Then
                LogIF.ErrorLog("SQLTRACE", strLogMessage)
            ElseIf GetConfigParameter.GetConfigValue(PubLiteral.SQL_TRACELOG).ToUpper() = PubLiteral.OFF Then
                ' SQLトレースログ（OFF）
            Else
                ' パラメータ・エラー（書式不正）
                Throw New ArgumentException([String].Format(PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_TRACELOG))
            End If

            ' ---

            ' 以下も、ログ出力で使用可能
            Dim obj As Object = Nothing

            ' UOC_Connection等で情報を設定しておく。
            ' UserInfoなどの情報を想定している。
            obj = Me.GetDam().Obj

            ' SQL実行時に情報が自動設定される。
            ' ・ExecSelectFill_DT
            '   DataTable
            ' ・ExecSelectFill_DS
            '   DataSet
            ' ・ExecSelect_DR
            '   IDataReader
            ' ・ExecSelectScalar
            '   object
            ' ・ExecInsUpDel_NonQuery
            '   int
            obj = Me.LogInfo
		End Sub

		#End Region
	End Class
End Namespace
