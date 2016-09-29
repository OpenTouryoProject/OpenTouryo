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
'* クラス名            :Program.cs
'* クラス名クラス名     :
'*
'* 作成者              :Supragyan
'* クラス日本語名       :
'* 更新履歴
'*  Date:        Author:        Comments:
'*  ----------  --------------  -------------------------------------------------
'*  11/28/2014   Supragyan      For Inserts data to database 
'*  17/08/2015   Sandeep        Modified insert method name from 'Start' to 'InsertTask'.
'*                              Modified object of LayerB that is related to Business project,
'*                              instead of AsyncSvc_sample project. 
'**********************************************************************************
'system
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

'部品
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Db

'業務フレームワーク
Imports Touryo.Infrastructure.Business.Util

'AsyncSvc_Sample
Imports AsyncSvc_sample

'AsyncProcessingService
Imports AsyncProcessingService

Namespace TestAsyncSvc_Sample
	''' <summary>
	''' Program class for test user code
	''' </summary>
	Public Class Program
		''' <summary>This is the main entry point for the application.</summary>
        Friend Shared Sub Main(args As String())
            Dim program As New Program()
            program.InsertData()
        End Sub

		''' <summary>
		''' Inserts asynchronous task information to the database
		''' </summary>
		''' <returns></returns>
		Public Function InsertData() As AsyncProcessingServiceParameterValue
			' Create array data to serilize.
			Dim arrayData As Byte() = {1, 2, 3, 4, 5}

			' Sets parameters of AsyncProcessingServiceParameterValue to insert asynchronous task information.
			Dim asyncParameterValue As New AsyncProcessingServiceParameterValue("AsyncProcessingService", "InsertTask", "InsertTask", "SQL", New MyUserInfo("AsyncProcessingService", "AsyncProcessingService"))
			asyncParameterValue.UserId = "A"
			asyncParameterValue.ProcessName = "AAA"
            asyncParameterValue.Data = AsyncSvc_sample.AsyncSvc_sample.LayerB.SerializeToBase64String(arrayData)
			asyncParameterValue.ExecutionStartDateTime = DateTime.Now
			asyncParameterValue.RegistrationDateTime = DateTime.Now
			asyncParameterValue.NumberOfRetries = 0
			asyncParameterValue.ProgressRate = 0
			asyncParameterValue.CompletionDateTime = DateTime.Now
			asyncParameterValue.StatusId = CInt(AsyncProcessingServiceParameterValue.AsyncStatus.Register)
			asyncParameterValue.CommandId = 0
			asyncParameterValue.ReservedArea = "xxxxxx"

			Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction
			Dim asyncReturnValue As AsyncProcessingServiceReturnValue

			' Execute do business logic method.
			Dim layerB As New AsyncProcessingService.LayerB()
			asyncReturnValue = DirectCast(layerB.DoBusinessLogic(DirectCast(asyncParameterValue, AsyncProcessingServiceParameterValue), iso), AsyncProcessingServiceReturnValue)
			Return asyncParameterValue
		End Function
	End Class
End Namespace
