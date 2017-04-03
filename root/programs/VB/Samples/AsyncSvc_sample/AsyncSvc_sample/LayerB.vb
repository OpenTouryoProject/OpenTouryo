'**********************************************************************************
'* 非同期処理サービス・サンプル アプリ
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：LayerB
'* クラス日本語名  ：LayerB
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  11/28/2014   Supragyan        LayerB class for AsyncProcessing Service.
'*  17/08/2015   Sandeep          Implemented serialization and deserialization methods.
'*                                Modified the code to start and update asynchronous task.
'*                                Implemented code to get command value and update progress rate.
'*                                Implemented code to declare and initialize the member variable.
'*                                Implemented code to handle abnormal termination, while updating the asynchronous process.
'*                                Implemented code to resume asynchronous process in the middle of the processing.
'*  21/08/2015   Sandeep          Modified code to call layerD of AsynProcessingService instead of do business logic.
'*  28/08/2015   Sandeep          Resolved transaction timeout issue by using DamKeyForABT and DamKeyForAMT properties.
'*  07/06/2016   Sandeep          Implemented code that respond to various test cases, other than success state.
'*  08/06/2016   Sandeep          Implemented method to update the command of selected task.
'**********************************************************************************

Imports System

Imports Touryo.Infrastructure.Business.AsyncProcessingService
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace AsyncSvc_sample
	''' <summary>
	''' LayerB class for AsyncProcessing service sample
	''' </summary>
	Public Class LayerB
		Inherits MyApsBaseLogic
		#Region "Member declartion"

		' Number of seconds
		Private NumberOfSeconds As Integer

		#End Region

		#Region "Member initialization"

		''' <summary>Constructor</summary>
		Public Sub New()
			' Number of seconds to sleep the thread.
			Dim numberOfSeconds As String = GetConfigParameter.GetConfigValue("FxSleepUserProcess")
			If Not String.IsNullOrEmpty(numberOfSeconds) Then
				Me.NumberOfSeconds = Integer.Parse(numberOfSeconds)
			Else
				Me.NumberOfSeconds = 5
			End If
		End Sub

		#End Region

		#Region "Member methods"

		''' <summary>
		'''  Converts base64 string to deserialized byte array.
		''' </summary>
		''' <param name="base64String">Base64 String</param>
		''' <returns>byte array</returns>
		Private Function DeserializeFromBase64String(base64String As String) As Byte()
			Dim deserializeData As Byte() = Nothing
			If String.IsNullOrEmpty(base64String) Then
				deserializeData = CustomEncode.FromBase64String(base64String)
			End If
			Return deserializeData
		End Function

		''' <summary>
		'''  Converts byte array to serialized base64 string
		''' </summary>
		''' <param name="arrayData">byte array</param>
		''' <returns>base64 string</returns>
		Public Shared Function SerializeToBase64String(arrayData As Byte()) As String
			Dim base64String As String = String.Empty
			If arrayData IsNot Nothing Then
				CustomEncode.ToBase64String(arrayData)
			End If
			Return base64String
		End Function

		''' <summary>
		'''  Get command information from database. 
		''' </summary>
		''' <param name="taskID">asynchronous task id</param>
        ''' <param name="userReturnValue">asynchronous return value</param>
		Private Sub GetCommandValue(taskID As Integer, userReturnValue As AsyncProcessingServiceReturnValue)
			' Sets parameters of AsyncProcessingServiceParameterValue to get command value.
			Dim asyncParameterValue As New AsyncProcessingServiceParameterValue("AsyncProcessingService", "SelectCommand", "SelectCommand", "SQL", New MyUserInfo("AsyncProcessingService", "AsyncProcessingService"))
			asyncParameterValue.TaskId = taskID

			' Calls data access part of asynchronous processing service.
			Dim myDao As New LayerD(Me.GetDam(Me.DamKeyforAMT))
			myDao.SelectCommand(asyncParameterValue, userReturnValue)
			userReturnValue.CommandId = CInt(userReturnValue.Obj)
		End Sub

		''' <summary>
		'''  Resumes asynchronous process in the middle of the processing.
		''' </summary>
		''' <param name="taskID">Task ID</param>
		''' <param name="userReturnValue">asynchronous return value</param>
		Private Sub ResumeProcessing(taskID As Integer, userReturnValue As AsyncProcessingServiceReturnValue)
			' Sets parameters of AsyncProcessingServiceParameterValue to resume asynchronous process in the middle of the processing.
			Dim asyncParameterValue As New AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateTaskCommand", "UpdateTaskCommand", "SQL", New MyUserInfo("AsyncProcessingService", "AsyncProcessingService"))
			asyncParameterValue.TaskId = taskID
			asyncParameterValue.CommandId = 0

			' Calls data access part of asynchronous processing service.
			Dim myDao As New LayerD(Me.GetDam(Me.DamKeyforAMT))
			myDao.UpdateTaskCommand(asyncParameterValue, userReturnValue)
		End Sub

		''' <summary>
		'''  Updates the progress rate in the database. 
		''' </summary>
		''' <param name="taskID">asynchronous task id</param>
		''' <param name="progressRate">progress rate</param>
		Private Sub UpdateProgressRate(taskID As Integer, userReturnValue As AsyncProcessingServiceReturnValue, progressRate As Decimal)
			' Sets parameters of AsyncProcessingServiceParameterValue to Update progress rate
			Dim asyncParameterValue As New AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateTaskProgress", "UpdateTaskProgress", "SQL", New MyUserInfo("AsyncProcessingService", "AsyncProcessingService"))
			asyncParameterValue.TaskId = taskID
			asyncParameterValue.ProgressRate = progressRate

			' Calls data access part of asynchronous processing service.
			Dim myDao As New LayerD(Me.GetDam(Me.DamKeyforAMT))
			myDao.UpdateTaskProgress(asyncParameterValue, userReturnValue)
		End Sub

		''' <summary>
		''' Initiate the processing of asynchronous task.
		''' </summary>
		''' <param name="userParameterValue">asynchronous parameter values</param>
		Public Sub UOC_Start(userParameterValue As AsyncProcessingServiceParameterValue)
			' Generates a return value class.
			Dim userReturnValue As New AsyncProcessingServiceReturnValue()
			Me.ReturnValue = userReturnValue

			' Get array data from serialized base64 string.
			Dim arrayData As Byte() = Me.DeserializeFromBase64String(userParameterValue.Data)

			' Get command information from database to check for retry.
			Me.GetCommandValue(userParameterValue.TaskId, userReturnValue)

			If userReturnValue.CommandId = CInt(AsyncProcessingServiceParameterValue.AsyncCommand.[Stop]) Then
				' Retry task: to resume asynchronous process in the middle of the processing.
				Me.ResumeProcessing(userParameterValue.TaskId, userReturnValue)
					' Otherwise, implement code to initiating a new task. 
					'...
			Else
			End If

			' Updates the progress rate and handles abnormal termination of the process.
			Me.Update(userParameterValue.TaskId, userReturnValue)
		End Sub

		''' <summary>
		'''  Updates the progress rate and handles abnormal termination of the process.
		''' </summary>
		''' <param name="taskID">Task ID</param>
		''' <param name="userReturnValue">user parameter value</param>
		Private Sub Update(taskID As Integer, userReturnValue As AsyncProcessingServiceReturnValue)
			' Place the following statements in the loop, till the completion of task.
			' AsyncProcess: Loop-Start

			' Get command information from database to check for retry.
			Me.GetCommandValue(taskID, userReturnValue)

			Select Case userReturnValue.CommandId
				Case CInt(AsyncProcessingServiceParameterValue.AsyncCommand.[Stop])
					' If you want to retry, then throw the following exception.
					Throw New BusinessApplicationException("APSStopCommand", GetMessage.GetMessageDescription("CTE0003"), "")
				Case CInt(AsyncProcessingServiceParameterValue.AsyncCommand.Abort)
					' Implement code to forcefully Abort the task.
					'...

					' If the task is abnormal terminated, then throw the exception .
					Throw New BusinessSystemException("APSAbortCommand", GetMessage.GetMessageDescription("CTE0004"))
				Case Else
					' Update the progress rate in database.
					Me.UpdateProgressRate(taskID, userReturnValue, 50)

					' Sleeps the thread, to minimize the CPU utilization.
					System.Threading.Thread.Sleep(Me.NumberOfSeconds * 1000)
					Exit Select
			End Select
			'AsyncProcess: Loop-End

			' If loop ends with no error, that indicates the task is completed sucessfully.
			Return
		End Sub

		#End Region
	End Class
End Namespace
