'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
'* クラス名        ：EditDigitsAfterDPConverter
'* クラス日本語名  ：デザインタイム プロパティ用
'*                   小数点数以下ｘ桁編集方法の指定クラスのコンバータ（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

' System
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Globalization

' System.Windows
Imports System.Windows
Imports System.Windows.Forms

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>デザインタイム プロパティ用　小数点数以下ｘ桁編集方法の指定クラスのコンバータ（テンプレート）</summary>
	Public Class EditDigitsAfterDPConverter
		Inherits ExpandableObjectConverter
		''' <summary>
		''' ConvertTo（プロパティグリッド表示値への変換）を実行可能か。
		''' </summary>
		''' <param name="context">コンテキスト</param>
		''' <param name="destinationType">変換後の型</param>
		''' <returns>
		''' ConvertTo実行可：true。
		''' ConvertTo実行不可：false。
		''' </returns>
		Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
			' 型をチェック
			If destinationType Is GetType(EditDigitsAfterDP) Then
				' EditDigitsAfterDP型ならtrueを返す。
				Return True
			End If

			' 上記以外の場合、ベースへ。
			Return MyBase.CanConvertTo(context, destinationType)
		End Function

		''' <summary>
		''' ConvertTo（プロパティグリッド表示値への変換）を実行する。
		''' </summary>
		''' <param name="context">コンテキスト</param>
		''' <param name="culture">カルチャと</param>
		''' <param name="value">EditDigitsAfterDPオブジェクト</param>
		''' <param name="destinationType">変換後の型</param>
		''' <returns>文字列</returns>
		Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As CultureInfo, value As Object, destinationType As Type) As Object
			' 型をチェック
			If destinationType Is GetType(String) Then
				' 文字列へ変換
				If TypeOf value Is EditDigitsAfterDP Then
					Dim edadp As EditDigitsAfterDP = DirectCast(value, EditDigitsAfterDP)
					Return edadp.HowToCut.ToString() & " : " & edadp.DigitsAfterDP.ToString()
				End If
			End If

			' 上記以外の場合、ベースへ。
			Return MyBase.ConvertTo(context, culture, value, destinationType)
		End Function

		''' <summary>
		''' ConvertFrom（プロパティグリッドからの変換）を実行可能か。
		''' </summary>
		''' <param name="context">コンテキスト</param>
		''' <param name="sourceType">文字列</param>
		''' <returns>
		''' ConvertFrom実行可：true。
		''' ConvertFrom実行不可：false。
		''' </returns>
		Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
			' 型をチェック
			If sourceType Is GetType(String) Then
				' 文字列型ならtrueを返す。
				Return True
			End If

			' 上記以外の場合、ベースへ。
			Return MyBase.CanConvertFrom(context, sourceType)
		End Function

		''' <summary>
		''' ConvertFrom（プロパティグリッドからの変換）を実行する。
		''' </summary>
		''' <param name="context">コンテキスト</param>
		''' <param name="culture">カルチャ</param>
		''' <param name="value">文字列</param>
		''' <returns>EditDigitsAfterDPオブジェクト</returns>
		Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As CultureInfo, value As Object) As Object
			' 文字列型の場合
			If TypeOf value Is String Then
				Dim s As String() = value.ToString().Split(":"C)

				Dim edadp As New EditDigitsAfterDP()

				Dim howToCut As String = s(0).Trim()
				If howToCut = CutMethod.Banker.ToString() Then
					edadp.HowToCut = CutMethod.Banker
				ElseIf howToCut = CutMethod._4sya5nyu.ToString() Then
					edadp.HowToCut = CutMethod._4sya5nyu
				ElseIf howToCut = CutMethod.Ceiling.ToString() Then
					edadp.HowToCut = CutMethod.Floor
				Else
					edadp.HowToCut = CutMethod.Ceiling
				End If

				If s.Length <= 1 OrElse s(1).Trim() = "" Then
					edadp.DigitsAfterDP = 0
				Else
					edadp.DigitsAfterDP = UInteger.Parse(s(1).Trim())
				End If

				Return edadp
			End If

			' 上記以外の場合、ベースへ。
			Return MyBase.ConvertFrom(context, culture, value)
		End Function
	End Class
End Namespace
