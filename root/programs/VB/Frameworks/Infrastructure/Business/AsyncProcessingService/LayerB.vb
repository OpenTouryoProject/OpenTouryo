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
'* クラス名        ：LayerB
'* クラス日本語名  ：LayerB
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  11/28/2014  Supragyan         Created LayerB class for AsyncProcessing Service
'*  11/28/2014  Supragyan         Created Insert,Update,Select method for AsyncProcessing Service
'*  04/15/2015  Sandeep           Did code modification of insert, update and select for AsyncProcessing Service
'*  06/09/2015  Sandeep           Implemented code to update stop command to all the running asynchronous task
'*                                Modified code to reset Exception information, before starting asynchronous task 
'*  06/26/2015  Sandeep           Implemented code to get commandID in the SelectTask method,
'*                                to resolve unstable "Register" state, when you invoke [Abort] to AsyncTask, at this "Register" state
'*  06/01/2016  Sandeep           Implemented method to test the connection of specified database
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Business

Namespace Touryo.Infrastructure.Business.AsyncProcessingService
	#Region "LayerB"

	''' <summary>
	''' LayerB class for AsyncProcessing Service
	''' </summary>
	Public Class LayerB
		Inherits MyFcBaseLogic
		#Region "Insert"

		''' <summary>
		''' Inserts Async Parameter values to Database through LayerD 
		''' </summary>
		''' <param name="asyncParameterValue"></param>
		Public Sub UOC_InsertTask(asyncParameterValue As AsyncProcessingServiceParameterValue)
			' 戻り値クラスを生成して、事前に戻り値に設定しておく。
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.InsertTask(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "Update"

		#Region "UpdateTaskStart"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is started
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskStart(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskStart(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "UpdateTaskRetry"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is failed and can be retried later
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskRetry(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskRetry(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "UpdateTaskFail"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is failed and abort this task [status=Abort] 
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskFail(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskFail(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "UpdateTaskSuccess"

		''' <summary>
		'''  Updates information in the database that the asynchronous task is completed
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskSuccess(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskSuccess(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "UpdateTaskProgress"

		''' <summary>
		'''  Updates progress rate of the asynchronous task in the database.
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskProgress(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskProgress(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "UpdateTaskCommand"

		''' <summary>
		'''  Updates command value information of a selected asynchronous task
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_UpdateTaskCommand(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.UpdateTaskCommand(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "StopAllTask"

		''' <summary>
		'''  Set stop command for all running asynchronous task
		''' </summary>
		''' <param name="asyncParameterValue">Asynchronous Parameter Values</param>
		Private Sub UOC_StopAllTask(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.StopAllTask(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#End Region

		#Region "Select"

		#Region "SelectCommand"

		''' <summary>
		''' Selects user command from Database through LayerD 
		''' </summary>
		''' <param name="asyncParameterValue"></param>
		Private Sub UOC_SelectCommand(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.SelectCommand(asyncParameterValue, asyncReturnValue)
		End Sub

		#End Region

		#Region "SelectTask"

		''' <summary>
		''' Selects Asynchronous task from LayerD 
		''' </summary>
		''' <param name="asyncParameterValue">Async Parameter Value</param>
		Private Sub UOC_SelectTask(asyncParameterValue As AsyncProcessingServiceParameterValue)
			Dim asyncReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = asyncReturnValue

			Dim myDao As New LayerD(Me.GetDam())
			myDao.SelectTask(asyncParameterValue, asyncReturnValue)

			Dim dt As DataTable = DirectCast(asyncReturnValue.Obj, DataTable)
			asyncReturnValue.Obj = Nothing

			If dt IsNot Nothing Then
				If dt.Rows.Count <> 0 Then
					asyncReturnValue.TaskId = Convert.ToInt32(dt.Rows(0)("Id"))
					asyncReturnValue.UserId = dt.Rows(0)("UserId").ToString()
					asyncReturnValue.ProcessName = dt.Rows(0)("ProcessName").ToString()
					asyncReturnValue.Data = dt.Rows(0)("Data").ToString()
					asyncReturnValue.NumberOfRetries = Convert.ToInt32(dt.Rows(0)("NumberOfRetries"))
					asyncReturnValue.ReservedArea = dt.Rows(0)("ReservedArea").ToString()
					asyncReturnValue.CommandId = Convert.ToInt32(dt.Rows(0)("CommandId"))
				End If
			End If
		End Sub

		#End Region

		#End Region
	End Class

	#End Region
End Namespace

