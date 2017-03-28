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
'* クラス名        ：CheckType
'* クラス日本語名  ：デザインタイム プロパティ用　チェック タイプ指定クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
'*  2017/01/31  西野 大介         "Indispensable" ---> "Required"
'*  2017/01/31  西野 大介         Added the enum of CheckItems.
'**********************************************************************************

Imports System.ComponentModel

Namespace Touryo.Infrastructure.CustomControl
    ''' <summary>チェック タイプ</summary>
    <TypeConverter(GetType(CheckTypeConverter))>
    Public Class CheckType

        ''' <summary>ビット・フィールド</summary>
        Public Enum CheckItems
            ''' <summary>必須入力</summary>
            Required = 1
            ''' <summary>半角</summary>
            IsHankaku = 2
            ''' <summary>全角</summary>
            IsZenkaku = 4
            ''' <summary>数値</summary>
            IsNumeric = 8
            ''' <summary>片仮名</summary>
            IsKatakana = 16
            ''' <summary>半角片仮名</summary>
            IsHanKatakana = 32
            ''' <summary>平仮名</summary>
            IsHiragana = 64
            ''' <summary>日付</summary>
            IsDate = 128
        End Enum

        ''' <summary>コンストラクタ</summary>
        Public Sub New()
            Me.Required = False
            Me.IsHankaku = False
            Me.IsZenkaku = False
            Me.IsNumeric = False
            Me.IsKatakana = False
            Me.IsHanKatakana = False
            Me.IsHiragana = False
            Me.IsDate = False
        End Sub

#Region "プロパティ"

        ''' <summary>必須</summary>
        Private _isIndispensable As Boolean

        ''' <summary>必須</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property Required() As Boolean
            Get
                Return Me._isIndispensable
            End Get

            Set(ByVal value As Boolean)
                Me._isIndispensable = value
            End Set
        End Property

        ''' <summary>半角</summary>
        Private _isHankaku As Boolean

        ''' <summary>半角</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsHankaku() As Boolean
            Get
                Return Me._isHankaku
            End Get

            Set(ByVal value As Boolean)
                Me._isHankaku = value
            End Set
        End Property

        ''' <summary>全角</summary>
        Private _isZenkaku As Boolean

        ''' <summary>全角</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsZenkaku() As Boolean
            Get
                Return Me._isZenkaku
            End Get

            Set(ByVal value As Boolean)
                Me._isZenkaku = value
            End Set
        End Property

        ''' <summary>数値</summary>
        Private _isNumeric As Boolean

        ''' <summary>数値</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsNumeric() As Boolean
            Get
                Return Me._isNumeric
            End Get

            Set(ByVal value As Boolean)
                Me._isNumeric = value
            End Set
        End Property

        ''' <summary>片仮名</summary>
        Private _isKatakana As Boolean

        ''' <summary>片仮名</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsKatakana() As Boolean
            Get
                Return Me._isKatakana
            End Get

            Set(ByVal value As Boolean)
                Me._isKatakana = value
            End Set
        End Property

        ''' <summary>半角片仮名</summary>
        Private _isHanKatakana As Boolean

        ''' <summary>半角片仮名</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsHanKatakana() As Boolean
            Get
                Return Me._isHanKatakana
            End Get

            Set(ByVal value As Boolean)
                Me._isHanKatakana = value
            End Set
        End Property

        ''' <summary>平仮名</summary>
        Private _isHiragana As Boolean

        ''' <summary>平仮名</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsHiragana() As Boolean
            Get
                Return Me._isHiragana
            End Get

            Set(ByVal value As Boolean)
                Me._isHiragana = value
            End Set
        End Property

        ''' <summary>日付</summary>
        Private _isDate As Boolean

        ''' <summary>日付</summary>
        <DefaultValue(False), NotifyParentProperty(True), RefreshProperties(RefreshProperties.Repaint)>
        Public Property IsDate() As Boolean
            Get
                Return Me._isDate
            End Get

            Set(ByVal value As Boolean)
                Me._isDate = value
            End Set
        End Property

#End Region

#Region "比較処理"

        ''' <summary>ハッシュを返す</summary>
        ''' <returns>ハッシュコード</returns>
        ''' <remarks>
        ''' 全メンバのハッシュコードのXORではなく、
        ''' ビットマスクの演算（boolなので）
        ''' </remarks>
        Public Overrides Function GetHashCode() As Integer
            Dim hc As Integer = 0

            If Me.Required Then
                hc += CInt(CheckItems.Required)
            End If
            If Me.IsHankaku Then
                hc += CInt(CheckItems.IsHankaku)
            End If
            If Me.IsZenkaku Then
                hc += CInt(CheckItems.IsZenkaku)
            End If
            If Me.IsNumeric Then
                hc += CInt(CheckItems.IsNumeric)
            End If
            If Me.IsKatakana Then
                hc += CInt(CheckItems.IsKatakana)
            End If
            If Me.IsHanKatakana Then
                hc += CInt(CheckItems.IsHanKatakana)
            End If
            If Me.IsHiragana Then
                hc += CInt(CheckItems.IsHiragana)
            End If
            If Me.IsDate Then
                hc += CInt(CheckItems.IsDate)
            End If

            Return hc
        End Function

        ''' <summary>Equals</summary>
        ''' <param name="ct">CheckType</param>
        ''' <returns>
        ''' true：等しい
        ''' false：等しくない
        ''' </returns>
        ''' <remarks>全メンバの==のAND</remarks>
        Public Overloads Function Equals(ByVal ct As CheckType) As Boolean
            ' null対応
            If ct Is Nothing Then
                Return False
            End If

            Return _
                (Me.Required = ct.Required) _
                AndAlso (Me.IsHankaku = ct.IsHankaku) _
                AndAlso (Me.IsZenkaku = ct.IsZenkaku) _
                AndAlso (Me.IsNumeric = ct.IsNumeric) _
                AndAlso (Me.IsKatakana = ct.IsKatakana) _
                AndAlso (Me.IsHanKatakana = ct.IsHanKatakana) _
                AndAlso (Me.IsHiragana = ct.IsHiragana) _
                AndAlso (Me.IsDate = ct.IsDate)

        End Function

        ''' <summary>Equals</summary>
        ''' <param name="obj">CheckType</param>
        ''' <returns>
        ''' true：等しい
        ''' false：等しくない
        ''' </returns>
        Public Overrides Function Equals(ByVal obj As [Object]) As Boolean
            If obj Is Nothing Then
                ' nullの場合（ベースへ）
                Return MyBase.Equals(obj)
            Else
                ' nullでない場合
                If Not (TypeOf obj Is CheckType) Then
                    ' 型が違う
                    Return False
                Else
                    ' 型が一致（オーバロードヘ）
                    Return Equals(TryCast(obj, CheckType))
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
        Public Shared Operator =(ByVal l As CheckType, ByVal r As CheckType) As Boolean
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
        Public Shared Operator <>(ByVal l As CheckType, ByVal r As CheckType) As Boolean
            ' ==演算子の逆
            Return Not (l = r)
        End Operator

#End Region
    End Class
End Namespace
