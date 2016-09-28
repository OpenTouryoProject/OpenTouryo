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
'* クラス名           :LayerD.cs
'* クラス名クラス名     :
'*
'* 作成者            :Supragyan
'* クラス日本語名      :
'* 更新履歴
'*  Date:        Author:        Comments:
'*  ----------  --------------  -------------------------------------------------
'*  11/28/2014   Supragyan      Created LayerD class for AsyncProcessing Service
'*  11/28/2014   Supragyan      Created Insert,Update,Select method for AsyncProcessing Service
'*  04/14/2015   Sandeep        Did code modification of update and select asynchronous task 
'*  04/14/2015   Sandeep        Did code implementation of SetSqlByFile3 to access the SQL from embedded resource
'*  05/28/2015   Sandeep        Did code implementation to update Exception information to the database
'*  06/09/2015   Sandeep        Implemented code to update stop command to all the running asynchronous task
'*                              Modified code to reset Exception information, before starting asynchronous task 
'*  06/26/2015   Sandeep        Removed the where condition command <> 'Abort' from the SelectTask asynchronous task,
'*                              to resolve unstable "Register" state, when you invoke [Abort] to AsyncTask, at this "Register" state
'*  06/26/2015   Sandeep        Since RootNamespace does not exists, removed code of "assemblyNameSpace" to access embedded resource  
'**********************************************************************************

' System
Imports System
Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

'業務フレームワーク
Imports Touryo.Infrastructure.Business.Dao

'部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Util

Namespace AsyncProcessingService
	''' <summary>
	''' LayerD class for AsyncProcessing Service
	''' </summary>
	Public Class LayerD
		Inherits MyBaseDao
		''' <summary>AsyncProcessingService用B層</summary>
		''' <param name="dam">dam</param>
		Public Sub New(dam As BaseDam)
			MyBase.New(dam)
		End Sub

		#Region "SetSqlByFile3"

		''' <summary>
		'''  Get SQL query from the embedded resource assembly
		''' </summary>
		Public Sub SetSqlByFile3(filename As String)
			' SQLファイルのEncoding情報の取得
			Dim sqlEncoding As String = GetConfigParameter.GetConfigValue(PubLiteral.SQL_ENCODING)

			If String.IsNullOrEmpty(sqlEncoding) Then
				' デフォルト：UTF-8
				sqlEncoding = "utf-8"
			End If

			Dim assemblyString As String = "Business"

			' Get SQL query from embedded resource file. 
            Dim commandText As String = EmbeddedResourceLoader.LoadAsString(assemblyString, filename, Encoding.GetEncoding(sqlEncoding))

			' Set sql command as text
			Me.SetSqlByCommand(commandText)
		End Sub

		#End Region

		#Region "Insert"

		''' <summary>
		''' Inserts async parameter values to database
		''' </summary>
		''' <param name="asyncParameterValue"></param>
		''' <param name="asyncReturnValue"></param>
		Public Sub InsertTask(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "AsyncProcessingServiceInsert.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P2", asyncParameterValue.UserId)
			Me.SetParameter("P3", asyncParameterValue.ProcessName)
			Me.SetParameter("P4", asyncParameterValue.Data)
			Me.SetParameter("P5", asyncParameterValue.RegistrationDateTime)
			Me.SetParameter("P6", DBNull.Value)
			Me.SetParameter("P7", asyncParameterValue.NumberOfRetries)
			Me.SetParameter("P8", DBNull.Value)
			Me.SetParameter("P9", asyncParameterValue.StatusId)
			Me.SetParameter("P10", asyncParameterValue.ProgressRate)
			Me.SetParameter("P11", asyncParameterValue.CommandId)
			Me.SetParameter("P12", asyncParameterValue.ReservedArea)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "Update"

		#Region "UpdateTaskStart"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is started
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskStart(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskStart.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.ExecutionStartDateTime)
			Me.SetParameter("P3", asyncParameterValue.StatusId)
			Me.SetParameter("P4", DBNull.Value)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "UpdateTaskRetry"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is failed and can be retried later
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskRetry(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskRetry.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.NumberOfRetries)
			Me.SetParameter("P3", asyncParameterValue.CompletionDateTime)
			Me.SetParameter("P4", asyncParameterValue.StatusId)
			Me.SetParameter("P5", asyncParameterValue.ExceptionInfo)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "UpdateTaskFail"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is failed and abort this task [status=Abort]
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskFail(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskFail.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.CompletionDateTime)
			Me.SetParameter("P3", asyncParameterValue.StatusId)
			Me.SetParameter("P4", asyncParameterValue.ExceptionInfo)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "UpdateTaskSuccess"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is completed
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskSuccess(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskSuccess.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.CompletionDateTime)
			Me.SetParameter("P3", asyncParameterValue.ProgressRate)
			Me.SetParameter("P4", asyncParameterValue.StatusId)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "UpdateTaskProgress"

		''' <summary>
		'''  Updates progress rate of the asynchronous task in the database.
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskProgress(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskProgress.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.ProgressRate)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "UpdateTaskCommand"

		''' <summary>
		'''  Updates command value information of a selected asynchronous task
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub UpdateTaskCommand(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "UpdateTaskCommand.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)
			Me.SetParameter("P2", asyncParameterValue.CommandId)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#Region "StopAllTask"

		''' <summary>
		'''  Set stop command for all running asynchronous task.
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub StopAllTask(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "StopAllTask.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.StatusId)
			Me.SetParameter("P2", asyncParameterValue.CommandId)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecInsUpDel_NonQuery()
		End Sub

		#End Region

		#End Region

		#Region "Select"

		#Region "SelectCommand"

		''' <summary>
		'''  Selects user command from database
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		''' <param name="asyncReturnValue">Asynchronous Return Values</param>
		Public Sub SelectCommand(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "SelectCommand.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.TaskId)

			' Execute SQL query
			asyncReturnValue.Obj = Me.ExecSelectScalar()
		End Sub

		#End Region

		#Region "SelectTask"

		''' <summary>
		'''  To get Asynchronous Task from the database
		''' </summary>
		''' <param name="asyncParameterValue"></param>
		''' <param name="asyncReturnValue"></param>
		Public Sub SelectTask(asyncParameterValue As AsyncProcessingServiceParameterValue, asyncReturnValue As AsyncProcessingServiceReturnValue)
			Dim filename As String = String.Empty
			filename = "SelectTask.sql"

			' Get SQL query from file.
			Me.SetSqlByFile3(filename)

			' Set SQL parameter values
			Me.SetParameter("P1", asyncParameterValue.RegistrationDateTime)
			Me.SetParameter("P2", asyncParameterValue.NumberOfRetries)
			Me.SetParameter("P3", CInt(AsyncProcessingServiceParameterValue.AsyncStatus.Register))
			Me.SetParameter("P4", CInt(AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd))
			Me.SetParameter("P7", asyncParameterValue.CompletionDateTime)

			Dim dt As New DataTable()

			' Get Asynchronous Task from the database
			Me.ExecSelectFill_DT(dt)
			asyncReturnValue.Obj = dt
		End Sub

		#End Region

		#End Region
	End Class
End Namespace
