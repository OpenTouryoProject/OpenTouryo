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
'* クラス名        ：AsyncProcessingServiceParameterValue
'* クラス日本語名  ：AsyncProcessingServiceParameterValue
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  11/28/2014  Supragyan         Paramter Value class for Asynchronous Processing Service
'*  04/15/2015  Sandeep           Changed datatype of ProgressRate to decimal.
'**********************************************************************************

Imports System.Reflection

Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Util

Namespace Touryo.Infrastructure.Business.AsyncProcessingService
	''' <summary>
	''' Paramter Value class for Asynchronous Processing Service
	''' </summary>
	Public Class AsyncProcessingServiceParameterValue
		Inherits MyParameterValue
		''' <summary>汎用エリア</summary>
		Public Obj As Object

		''' <summary>TaskId</summary>
		Public TaskId As Integer

		''' <summary>UserId</summary>
		Public UserId As String

		''' <summary>ProcessName</summary>
		Public ProcessName As String

		''' <summary>Data</summary>
		Public Data As String

		''' <summary>RegistrationDateTime</summary>
		Public RegistrationDateTime As DateTime

		''' <summary>ExecutionStartDateTime</summary>
		Public ExecutionStartDateTime As DateTime

		''' <summary>NumberOfRetries</summary>
		Public NumberOfRetries As Integer

		''' <summary>ProgressRate</summary>
		Public ProgressRate As Decimal

		''' <summary>Status</summary>
		Public StatusId As Integer

		''' <summary>CompletionDateTime</summary>
		Public CompletionDateTime As DateTime

		''' <summary>CommandId</summary>
		Public CommandId As Integer

		''' <summary>ReservedArea</summary>
		Public ReservedArea As String

		''' <summary>ExceptionInfo</summary>
		Public ExceptionInfo As String

		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo)
				' Baseのコンストラクタに引数を渡すために必要。
			MyBase.New(screenId, controlId, methodName, actionType, user)
		End Sub

		#End Region

		#Region "AsyncStatus"

		''' <summary>
		''' AsyncStatus Enum for storing all status
		''' </summary>
		Public Enum AsyncStatus
			''' <summary>Register</summary>
			<StringValue("Register")> _
			Register = 1

			''' <summary>Processing</summary>
			<StringValue("Processing")> _
			Processing

			''' <summary>End</summary>
			<StringValue("End")> _
			[End]

			''' <summary>AbnormalEnd</summary>
			<StringValue("AbnormalEnd")> _
			AbnormalEnd

			''' <summary>Abort</summary>
			<StringValue("Abort")> _
			Abort
		End Enum

		#End Region

		#Region "AsyncCommand"

		''' <summary>
		''' AsyncCommand Enum for storing command values
		''' </summary>
		Public Enum AsyncCommand
			''' <summary>Stop</summary>
			<StringValue("Stop")> _
			[Stop] = 1

			''' <summary>Abort</summary>
			<StringValue("Abort")> _
			Abort
		End Enum

		#End Region
	End Class

	''' <summary>
	'''  To get the string value
	''' </summary>
	Public Class StringValueAttribute
		Inherits System.Attribute
		Private _value As String

		''' <summary>StringValueAttribute</summary>
		''' <param name="value">value</param>
		Public Sub New(value As String)
			_value = value
		End Sub

		''' <summary>Value</summary>
		Public ReadOnly Property Value() As String
			Get
				Return _value
			End Get
		End Property
	End Class

	''' <summary>
	'''  Class that holds the Enum values string
	''' </summary>
	Public Class StringEnum
		''' <summary>
		'''  To get the string value from Enum value
		''' </summary>
		''' <param name="value">Enum value</param>
		''' <returns>String value of Enum</returns>
		Public Shared Function GetStringValue(value As [Enum]) As String
			Dim output As String = Nothing
			Dim type As Type = value.[GetType]()

			' Gets the 'StringValueAttribute'
			Dim fi As FieldInfo = type.GetField(value.ToString())
            Dim attrs As StringValueAttribute() = TryCast(fi.GetCustomAttributes(GetType(StringValueAttribute), False), StringValueAttribute())
			If attrs.Length > 0 Then
				output = attrs(0).Value
			End If
			Return output
		End Function
	End Class
End Namespace
