﻿'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
'  
' 
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
'* クラス名        ：MyFcBaseLogic
'* クラス日本語名  ：自動振り分け機能付き業務コード親クラス２（サーバ用）（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2010/03/30  西野 大介         ログ フォーマットにメソッド名を追加
'*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
'*  2010/09/24  西野 大介         Damクラス内にユーザ情報を格納したので
'*  2011/01/24  西野 大介         上記コードが通るカバレージを修正
'*  2011/01/31  西野 大介         Damがnullの場合は処理しないように修正
'*  2011/01/31  西野 大介         Damがnullの場合は処理しないように修正
'*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
'*  2012/04/05  西野 大介         \n → \r\n 化
'**********************************************************************************

' デバッグ用
Imports System.Diagnostics

' メソッドの属性を取得
Imports System.Reflection

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Business
    ''' <summary>自動振り分け機能付き業務コード親クラス２（サーバ用）（テンプレート）</summary>
    ''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
    Public MustInherit Class MyFcBaseLogic
        Inherits BaseLogic
        ''' <summary>性能測定</summary>
        Private perfRec As PerformanceRecorder

#Region "メソッド"

#Region "処理の自動振り分け"

        ''' <summary>自動振り分け処理</summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        Protected Overrides Sub UOC_DoAction(ByVal parameterValue As BaseParameterValue, ByRef returnValue As BaseReturnValue)
            ' メソッド名を生成
            Dim methodName As String = "UOC_" & Convert.ToString(parameterValue.MethodName)

            '#Region "レイトバインドする"

            Dim paramSet As Object() = New Object() {parameterValue}

            Try
                ' Latebind
                Latebind.InvokeMethod(Me, methodName, paramSet)
            Catch rtEx As System.Reflection.TargetInvocationException
                ' InnerExceptionのスタックトレースを保存しておく（以下のリスローで消去されるため）。
                Me.OriginalStackTrace = rtEx.InnerException.StackTrace

                ' InnerExceptionを投げなおす。
                Throw rtEx.InnerException
            Finally
                ' レイトバインドにおいて、
                ' ・ 戻り値（in）の場合、下位で生成した戻り値インスタンスは戻らない。
                ' ・ 戻り値（ref, out）の場合、例外発生時は戻り値インスタンスは戻らない。
                ' という問題がある。

                ' ∴ （特に後者の対応のため、）
                ' メンバ変数を使用して戻り値インスタンスを戻す。
                returnValue = Me.ReturnValue
            End Try

            '#End Region
        End Sub

#End Region

#Region "ＤＢ接続"

        ''' <summary>データアクセス制御クラス（ＤＡＭ）の生成し、コネクションを確立、トランザクションを開始する処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="iso">分離レベル（ＤＢＭＳ毎の分離レベルの違いを理解して設定すること）</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ConnectionOpen(ByVal parameterValue As BaseParameterValue, ByVal iso As DbEnum.IsolationLevelEnum)
            '#Region "トランザクション属性取得例"

            '' クラスの属性、メソッドの属性から調査
            'Dim aryMCA As MyAttribute() = Nothing
            'Dim aryMMA As MyAttribute() = Nothing

            '' クラスの属性を取得
            'MyAttribute.GetAttr(Me, aryMCA)

            'For Each mca As MyAttribute In aryMCA
            '    Debug.WriteLine((Me.[GetType]().ToString() & ".MyAttributeA = ") & mca.MyAttributeA)
            '    Debug.WriteLine((Me.[GetType]().ToString() & ".MyAttributeB = ") & mca.MyAttributeB)
            '    Debug.WriteLine((Me.[GetType]().ToString() & ".MyAttributeC = ") & mca.MyAttributeC)
            '    Debug.WriteLine("+------------------+")
            'Next

            '' メソッドの属性を取得
            'Dim aryMtdInfo As MethodInfo() = Me.[GetType]().GetMethods()

            'For Each mtdInfo As MethodInfo In aryMtdInfo
            '    MyAttribute.GetAttr(mtdInfo, aryMMA)

            '    For Each mma As MyAttribute In aryMMA
            '        Debug.WriteLine((mtdInfo.Name & ".MyAttributeA = ") & mma.MyAttributeA)
            '        Debug.WriteLine((mtdInfo.Name & ".MyAttributeB = ") & mma.MyAttributeB)
            '        Debug.WriteLine((mtdInfo.Name & ".MyAttributeC = ") & mma.MyAttributeC)
            '        Debug.WriteLine("+------------------+")
            '    Next
            'Next

            '#End Region

            ' データアクセス制御クラス（ＤＡＭ）
            Dim dam As BaseDam = Nothing

            '#Region "接続"

            If iso = DbEnum.IsolationLevelEnum.NotConnect Then
                ' 接続しない
            Else
                ' 接続する

                Dim connstring As String = ""

                '#Region "データ プロバイダ選択"

                If parameterValue.ActionType.Split("%"c)(0) = "SQL" Then
                    ' SQL Server / SQL Client用のDamを生成
                    dam = New DamSqlSvr()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_SQL")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "OLE" Then
                    ' OLEDB.NET用のDamを生成
                    dam = New DamOLEDB()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_OLE")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "ODB" Then
                    ' ODBC.NET用のDamを生成
                    dam = New DamODBC()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ODBC")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "ORA" Then
                    ' Oracle / Oracle Client用のDamを生成
                    dam = New DamOraClient()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ORA")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "ODP" Then
                    ' Oracle / ODP.NET用のDamを生成
                    dam = New DamOraOdp()

                    ' 接続文字列をロード（ODP2：Instant Client）
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ODP2")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "DB2" Then
                    ' DB2.NET用のDamを生成
                    dam = New DamDB2()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_DB2")
                    'ElseIf parameterValue.ActionType.Split("%"c)(0) = "HIR" Then
                    '    ' HiRDBデータプロバイダ用のDamを生成
                    '    dam = New DamHiRDB()

                    '    ' 接続文字列をロード
                    '    connstring = GetConfigParameter.GetConnectionString("ConnectionString_HIR")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "MCN" Then
                    ' MySQL Cnn/NET用のDamを生成
                    dam = New DamMySQL()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_MCN")
                ElseIf parameterValue.ActionType.Split("%"c)(0) = "NPS" Then
                    ' PostgreSQL / Npgsql用のDamを生成
                    dam = New DamPstGrS()

                    ' 接続文字列をロード
                    connstring = GetConfigParameter.GetConnectionString("ConnectionString_NPS")
                Else
                    ' ここは通らない
                End If

                '#End Region

                If dam IsNot Nothing Then
                    ' コネクションをオープンする。
                    dam.ConnectionOpen(connstring)

                    '#Region "トランザクションを開始する。"

                    If iso = DbEnum.IsolationLevelEnum.User Then
                        ' 自動トランザクション（規定の分離レベル）
                        dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted)
                    Else
                        ' 自動トランザクション（指定の分離レベル）
                        dam.BeginTransaction(iso)
                    End If

                    '#End Region

                    ' ユーザ情報を格納する（ログ出力で利用）。
                    dam.Obj = DirectCast(parameterValue, MyParameterValue).User

                    ' damを設定する。
                    Me.SetDam(dam)
                End If
            End If

            '#End Region
        End Sub

#End Region

#Region "開始・終了処理"

        ''' <summary>
        ''' Ｂ層の開始処理を実装
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_PreAction(ByVal parameterValue As BaseParameterValue)
            ' ACCESSログ出力-----------------------------------------------

            Dim myPV As MyParameterValue = DirectCast(parameterValue, MyParameterValue)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス, レイヤ, 
            ' 画面名, コントロール名, メソッド名, 処理名
            ' ------------
            Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "----->>" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType

            ' Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------

            ' 性能測定開始
            Me.perfRec = New PerformanceRecorder()
            Me.perfRec.StartsPerformanceRecord()

        End Sub

        ''' <summary>
        ''' Ｂ層の終了処理を実装
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_AfterAction(ByVal parameterValue As BaseParameterValue, ByVal returnValue As BaseReturnValue)
            ' 性能測定終了
            Me.perfRec.EndsPerformanceRecord()

            ' ACCESSログ出力-----------------------------------------------

            Dim myPV As MyParameterValue = DirectCast(parameterValue, MyParameterValue)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス, レイヤ, 
            ' 画面名, コントロール名, メソッド名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' ------------
            Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime)

            ' Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage)

            ' -------------------------------------------------------------            
        End Sub

        ''' <summary>
        ''' Ｂ層のトランザクションのコミット後の終了処理を実装
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_AfterTransaction(ByVal parameterValue As BaseParameterValue, ByVal returnValue As BaseReturnValue)
            ' TODO:
        End Sub

#End Region

#Region "例外処理"

        ''' <summary>
        ''' Ｂ層の業務例外による異常終了の後処理を実装するUOCメソッド。
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        ''' <param name="baEx">BusinessApplicationException</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(ByVal parameterValue As BaseParameterValue, ByVal returnValue As BaseReturnValue, ByVal baEx As BusinessApplicationException)
            ' 業務例外発生時の処理を実装
            ' TODO:

            ' nullチェック
            ' なにもしない
            If Me.perfRec Is Nothing Then
            Else
                ' 性能測定終了
                Me.perfRec.EndsPerformanceRecord()

                ' ACCESSログ出力-----------------------------------------------

                Dim myPV As MyParameterValue = DirectCast(parameterValue, MyParameterValue)

                ' ------------
                ' メッセージ部
                ' ------------
                ' ユーザ名, IPアドレス, レイヤ, 
                ' 画面名, コントロール名, メソッド名, 処理名
                ' 処理時間（実行時間）, 処理時間（CPU時間）
                ' エラーメッセージID, エラーメッセージ等
                ' ------------
                Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & Convert.ToString(baEx.messageID) & "," & Convert.ToString(baEx.Message)

                ' Log4Netへログ出力
                LogIF.WarnLog("ACCESS", strLogMessage)
            End If

            ' -------------------------------------------------------------   
        End Sub

        ''' <summary>
        ''' Ｂ層のシステム例外による異常終了の後処理を実装するUOCメソッド。
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        ''' <param name="bsEx">BusinessSystemException</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(ByVal parameterValue As BaseParameterValue, ByVal returnValue As BaseReturnValue, ByVal bsEx As BusinessSystemException)
            ' システム例外発生時の処理を実装
            ' TODO:

            ' nullチェック
            ' なにもしない
            If Me.perfRec Is Nothing Then
            Else
                ' 性能測定終了
                Me.perfRec.EndsPerformanceRecord()

                ' ACCESSログ出力-----------------------------------------------

                Dim myPV As MyParameterValue = DirectCast(parameterValue, MyParameterValue)

                ' ------------
                ' メッセージ部
                ' ------------
                ' ユーザ名, IPアドレス, レイヤ, 
                ' 画面名, コントロール名, メソッド名, 処理名
                ' 処理時間（実行時間）, 処理時間（CPU時間）
                ' エラーメッセージID, エラーメッセージ等
                ' ------------
                Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & Convert.ToString(bsEx.messageID) & "," & Convert.ToString(bsEx.Message) & vbCr & vbLf
				' OriginalStackTrace（ログ出力）の品質向上
				If Me.OriginalStackTrace = "" Then
                    strLogMessage &= bsEx.StackTrace
				Else
                    strLogMessage &= Me.OriginalStackTrace
				End If

                ' Log4Netへログ出力
                LogIF.ErrorLog("ACCESS", strLogMessage)
            End If

            ' -------------------------------------------------------------   
        End Sub

        ''' <summary>
        ''' Ｂ層の一般的な例外による異常終了の後処理を実装するUOCメソッド。
        ''' </summary>
        ''' <param name="parameterValue">引数クラス</param>
        ''' <param name="returnValue">戻り値クラス</param>
        ''' <param name="ex">Exception</param>
        ''' <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        Protected Overrides Sub UOC_ABEND(ByVal parameterValue As BaseParameterValue, ByRef returnValue As BaseReturnValue, ByVal ex As Exception)
            ' 一般的な例外発生時の処理を実装
            ' TODO:

            ' nullチェック
            If Me.perfRec Is Nothing Then
                ' なにもしない

                ' リスロー
                Throw ex
            Else
                ' 性能測定終了
                Me.perfRec.EndsPerformanceRecord()

                ' キャスト
                Dim myPV As MyParameterValue = DirectCast(parameterValue, MyParameterValue)

                ' システム例外に振り替える用のワーク
                Dim sysErrorFlag As Boolean = False
                Dim sysErrorMessageID As String = ""
                Dim sysErrorMessage As String = ""

                '#Region "例外の振替処理のIF文"

                If ex.Message = "Other-Business" Then
                    ' 業務例外へ変換
                    returnValue.ErrorFlag = True
                    returnValue.ErrorMessageID = "振替後"
                    returnValue.ErrorMessage = "振替後"
                    returnValue.ErrorInfo = "振り替える場合は、基本的にここを利用。"
                ElseIf ex.Message = "Other-System" Then
                    ' システム例外へ振替
                    sysErrorFlag = True
                    sysErrorMessageID = "振替後"
                    sysErrorMessage = "振替後"
                End If

                '#End Region

                '#Region "ACCESSログ出力、リスローする・しない"

                If returnValue.ErrorFlag Then
                    ' 業務例外へ変換

                    ' ------------
                    ' メッセージ部
                    ' ------------
                    ' ユーザ名, IPアドレス, レイヤ, 
                    ' 画面名, コントロール名, メソッド名, 処理名
                    ' 処理時間（実行時間）, 処理時間（CPU時間）
                    ' エラーメッセージID, エラーメッセージ等
                    ' ------------
                    Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & Convert.ToString(returnValue.ErrorMessageID) & "," & Convert.ToString(returnValue.ErrorMessage)

                    ' Log4Netへログ出力
                    LogIF.WarnLog("ACCESS", strLogMessage)
                ElseIf sysErrorFlag Then
                    ' システム例外へ振替

                    ' ------------
                    ' メッセージ部
                    ' ------------
                    ' ユーザ名, IPアドレス, レイヤ, 
                    ' 画面名, コントロール名, メソッド名, 処理名
                    ' 処理時間（実行時間）, 処理時間（CPU時間）
                    ' エラーメッセージID, エラーメッセージ等
                    ' ------------
                    Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & sysErrorMessageID & "," & sysErrorMessage & vbCr & vbLf
					' OriginalStackTrace（ログ出力）の品質向上
					If Me.OriginalStackTrace = "" Then
                        strLogMessage &= ex.StackTrace
					Else
                        strLogMessage &= Me.OriginalStackTrace
					End If

                    ' Log4Netへログ出力
                    LogIF.ErrorLog("ACCESS", strLogMessage)

                    ' 振替てスロー
                    Throw New BusinessSystemException(sysErrorMessageID, sysErrorMessage)
                Else
                    ' そのまま

                    ' ------------
                    ' メッセージ部
                    ' ------------
                    ' ユーザ名, IPアドレス, レイヤ, 
                    ' 画面名, コントロール名, メソッド名, 処理名
                    ' 処理時間（実行時間）, 処理時間（CPU時間）
                    ' エラーメッセージID, エラーメッセージ等
                    ' ------------
                    Dim strLogMessage As String = ((((("," & myPV.User.UserName & ",") & myPV.User.IPAddress & "," & "<<-----" & ",") & myPV.ScreenId & ",") & myPV.ControlId & ",") & myPV.MethodName & ",") & myPV.ActionType & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime) & "," & "other Exception" & "," & ex.Message & vbCr & vbLf
					' OriginalStackTrace（ログ出力）の品質向上
					If Me.OriginalStackTrace = "" Then
                        strLogMessage &= ex.StackTrace
					Else
                        strLogMessage &= Me.OriginalStackTrace
					End If

                    ' Log4Netへログ出力
                    LogIF.ErrorLog("ACCESS", strLogMessage)

                    ' リスロー
                    Throw ex

                    '#End Region
                End If
            End If
        End Sub

#End Region

#End Region
    End Class
End Namespace
