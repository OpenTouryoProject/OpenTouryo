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
'* クラス名        ：EditDigitsAfterDP
'* クラス日本語名  ：デザインタイム プロパティ用
'*                   小数点数以下ｘ桁編集方法の指定クラス（テンプレート）
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
	''' <summary>CutMethod</summary>
	Public Enum CutMethod
		''' <summary>なし</summary>
		None
		''' <summary>最近接偶数編集</summary>
		Banker
		''' <summary>四捨五入</summary>
		_4sya5nyu
		''' <summary>切り捨て（絶対値：0への丸め）</summary>
		Floor
		''' <summary>切り上げ（絶対値：無限大への丸め）</summary>
		Ceiling
		''' <summary>切り捨て（大小：負の無限大への丸め）</summary>
		FloorRM
		''' <summary>切り上げ（大小：正の無限大への丸め）</summary>
		CeilingRP
	End Enum

	''' <summary>小数点数以下ｘ桁編集方法の指定クラス</summary>
	''' <remarks>デザインタイム プロパティ用</remarks>
	<TypeConverter(GetType(EditDigitsAfterDPConverter))> _
	Public Class EditDigitsAfterDP
		''' <summary>コンストラクタ</summary>
		Public Sub New()
		End Sub

		''' <summary>コンストラクタ</summary>
		''' <param name="howToCut">切り方</param>
		''' <param name="digitsAfterDP">小数点数以下ｘ桁</param>
		Public Sub New(howToCut As CutMethod, digitsAfterDP As UInteger)
			Me.HowToCut = howToCut
			Me.DigitsAfterDP = digitsAfterDP
		End Sub

		''' <summary>切り方</summary>
		Public Property HowToCut() As System.Nullable(Of CutMethod)
			Get
				Return m_HowToCut
			End Get
			Set
				m_HowToCut = Value
			End Set
		End Property
		Private m_HowToCut As System.Nullable(Of CutMethod)
		''' <summary>小数点数以下ｘ桁</summary>
		Public Property DigitsAfterDP() As UInteger
			Get
				Return m_DigitsAfterDP
			End Get
			Set
				m_DigitsAfterDP = Value
			End Set
		End Property
		Private m_DigitsAfterDP As UInteger

		#Region "比較処理"

		''' <summary>ハッシュを返す</summary>
		''' <returns>ハッシュコード</returns>
		''' <remarks>全メンバのハッシュコードのXOR</remarks>
		Public Overrides Function GetHashCode() As Integer
			Dim hc As Integer = 0

			hc = hc Xor Me.HowToCut.GetHashCode()
			hc = hc Xor Me.DigitsAfterDP.GetHashCode()

			Return hc
		End Function

		''' <summary>Equals</summary>
		''' <param name="edadp">EditDigitsAfterDP</param>
		''' <returns>
		''' true：等しい
		''' false：等しくない
		''' </returns>
		''' <remarks>全メンバの==のAND</remarks>
		Public Overloads Function Equals(edadp As EditDigitsAfterDP) As Boolean
			' null対応
			If edadp Is Nothing Then
				Return False
			End If

            Return (Me.HowToCut.Value = edadp.HowToCut.Value) AndAlso (Me.DigitsAfterDP = edadp.DigitsAfterDP)
		End Function

		''' <summary>Equals</summary>
		''' <param name="obj">EditDigitsAfterDP</param>
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
				If Not (TypeOf obj Is EditDigitsAfterDP) Then
					' 型が違う
					Return False
				Else
					' 型が一致（オーバロードヘ）
					Return Equals(TryCast(obj, EditDigitsAfterDP))
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
		Public Shared Operator =(l As EditDigitsAfterDP, r As EditDigitsAfterDP) As Boolean
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
		Public Shared Operator <>(l As EditDigitsAfterDP, r As EditDigitsAfterDP) As Boolean
			' ==演算子の逆
			Return Not (l = r)
		End Operator

		#End Region
	End Class
End Namespace
