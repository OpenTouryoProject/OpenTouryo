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
'* クラス名        ：EditPadding
'* クラス日本語名  ：デザインタイム プロパティ用　パディング指定クラス（テンプレート）
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

' System.Windows
Imports System.Windows
Imports System.Windows.Forms

Namespace Touryo.Infrastructure.CustomControl.RichClient
	''' <summary>PadDirection</summary>
	Public Enum PadDirection
		''' <summary>パッドなし</summary>
		None
		''' <summary>右側にパッド</summary>
		Right
		''' <summary>左側にパッド</summary>
		Left
	End Enum

    ''' <summary>パディング指定クラス</summary>
	''' <remarks>デザインタイム プロパティ用</remarks>
	<TypeConverter(GetType(EditPaddingConverter))> _
	Public Class EditPadding
		''' <summary>コンストラクタ</summary>
		Public Sub New()
		End Sub

        ''' <summary>コンストラクタ</summary>
        ''' <param name="padDir">パッド方向</param>
        ''' <param name="padChar">パッド文字</param>
		Public Sub New(padDir As PadDirection, padChar As System.Nullable(Of Char))
			Me.PadDir = padDir
			Me.PadChar = padChar
		End Sub

		''' <summary>パッド方向</summary>
		Public Property PadDir() As PadDirection
			Get
				Return m_PadDir
			End Get
			Set
				m_PadDir = Value
			End Set
		End Property
		Private m_PadDir As PadDirection
		''' <summary>パッド文字</summary>
		Public Property PadChar() As System.Nullable(Of Char)
			Get
				Return m_PadChar
			End Get
			Set
				m_PadChar = Value
			End Set
		End Property
		Private m_PadChar As System.Nullable(Of Char)

		#Region "比較処理"


		''' <summary>ハッシュを返す</summary>
		''' <returns>ハッシュコード</returns>
		''' <remarks>全メンバのハッシュコードのXOR</remarks>
		Public Overrides Function GetHashCode() As Integer
			Dim hc As Integer = 0

			hc = hc Xor Me.PadDir.GetHashCode()

			If Me.PadChar IsNot Nothing Then
				hc = hc Xor Me.PadChar.GetHashCode()
			End If

			Return hc
		End Function

		''' <summary>Equals</summary>
		''' <param name="ep">EditPadding</param>
		''' <returns>
		''' true：等しい
		''' false：等しくない
		''' </returns>
		''' <remarks>全メンバの==のAND</remarks>
		Public Overloads Function Equals(ep As EditPadding) As Boolean
			' null対応
			If ep Is Nothing Then
				Return False
			End If

            Return (Me.PadDir = ep.PadDir) _
                AndAlso (Me.PadChar.Value = ep.PadChar.Value)
		End Function

		''' <summary>Equals</summary>
		''' <param name="obj">EditPadding</param>
		''' <returns>
		''' true：等しい
		''' false：等しくない
		''' </returns>
		Public Overrides Function Equals(obj As [Object]) As Boolean
			If obj Is Nothing Then
				' nullの場合（ベースへ）
				Return MyBase.Equals(obj)
			Else
				' nullでない場合
				If Not (TypeOf obj Is EditPadding) Then
					' 型が違う
					Return False
				Else
					' 型が一致（オーバロードヘ）
					Return Equals(TryCast(obj, EditPadding))
				End If
			End If
		End Function

		''' <summary>比較演算子（==）</summary>
		''' <param name="l">右辺</param>
		''' <param name="r">左辺</param>
		''' <returns>
		''' true：等しい
		''' false：等しくない
		''' </returns>
		Public Shared Operator =(l As EditPadding, r As EditPadding) As Boolean
			' Check for null on left side.
			If [Object].ReferenceEquals(l, Nothing) Then
				' Check for null on right side.
				If [Object].ReferenceEquals(r, Nothing) Then
					' null == null = true.
					Return True
				Else
					' Only the left side is null.
					Return False
				End If
			Else
				' Equals handles case of null on right side.
				Return l.Equals(r)
			End If
		End Operator

		''' <summary>比較演算子（!=）</summary>
		''' <param name="l">右辺</param>
		''' <param name="r">左辺</param>
		''' <returns>
		''' true：等しい
		''' false：等しくない
		''' </returns>
		Public Shared Operator <>(l As EditPadding, r As EditPadding) As Boolean
			' ==演算子の逆
			Return Not (l = r)
		End Operator

		#End Region
	End Class
End Namespace
