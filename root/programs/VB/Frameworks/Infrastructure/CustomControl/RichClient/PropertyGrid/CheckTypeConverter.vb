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
'* クラス名        ：CheckTypeConverter
'* クラス日本語名  ：デザインタイム プロパティ用　CheckTypeクラスのコンバータ（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
'*  2017/01/31  西野 大介         "Indispensable" ---> "Required"
'**********************************************************************************

Imports System.Globalization
Imports System.ComponentModel

Namespace Touryo.Infrastructure.CustomControl.RichClient
    ''' <summary>デザインタイム プロパティ用　CheckTypeクラスのコンバータ（テンプレート）</summary>
    Public Class CheckTypeConverter
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
        Public Overrides Function CanConvertTo(ByVal context As ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            ' 型をチェック
            If destinationType Is GetType(CheckType) Then
                ' CheckType型ならtrueを返す。
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
        ''' <param name="value">CheckTypeオブジェクト</param>
        ''' <param name="destinationType">変換後の型</param>
        ''' <returns>文字列</returns>
        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object, ByVal destinationType As Type) As Object
            ' 型をチェック
            If destinationType Is GetType(String) Then
                ' 文字列へ変換
                If TypeOf value Is CheckType Then
                    Dim ct As CheckType = DirectCast(value, CheckType)

                    Dim s As String = ""

                    If ct.Required Then
                        s += "Required, "
                    End If
                    If ct.IsHankaku Then
                        s += "IsHankaku, "
                    End If
                    If ct.IsZenkaku Then
                        s += "IsZenkaku, "
                    End If
                    If ct.IsNumeric Then
                        s += "IsNumeric, "
                    End If
                    If ct.IsKatakana Then
                        s += "IsKatakana, "
                    End If
                    If ct.IsHanKatakana Then
                        s += "IsHanKatakana, "
                    End If
                    If ct.IsHiragana Then
                        s += "IsHiragana, "
                    End If
                    If ct.IsDate Then
                        s += "IsDate, "
                    End If

                    Return s.Substring(0, s.Length - 2)
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
        Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
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
        ''' <returns>CheckTypeオブジェクト</returns>
        Public Overrides Function ConvertFrom(ByVal context As ITypeDescriptorContext, ByVal culture As CultureInfo, ByVal value As Object) As Object
            ' 文字列型の場合
            If TypeOf value Is String Then
                Dim arys As String() = value.ToString().Split(","c)
                Dim ct As New CheckType()

                For Each s As String In arys
                    Dim t As String = s.Trim()

                    If t = "Required" Then
                        ct.Required = True
                    End If
                    If t = "IsZenkaku" Then
                        ct.IsZenkaku = True
                    End If
                    If t = "IsHankaku" Then
                        ct.IsHankaku = True
                    End If
                    If t = "IsNumeric" Then
                        ct.IsNumeric = True
                    End If
                    If t = "IsKatakana" Then
                        ct.IsKatakana = True
                    End If
                    If t = "IsHanKatakana" Then
                        ct.IsHanKatakana = True
                    End If
                    If t = "IsHiragana" Then
                        ct.IsHiragana = True
                    End If
                    If t = "IsDate" Then
                        ct.IsDate = True
                    End If
                Next

                Return ct
            End If

            ' 上記以外の場合、ベースへ。
            Return MyBase.ConvertFrom(context, culture, value)
        End Function
    End Class
End Namespace
