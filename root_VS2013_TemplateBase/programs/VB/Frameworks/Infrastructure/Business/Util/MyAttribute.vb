'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
'  
' 
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
'* クラス名        ：MyAttribute
'* クラス日本語名  ：カスタム属性クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2009/08/06  西野  大介        クラス属性だけでなく、メソッド属性も処理可能に修正
'**********************************************************************************

Imports System.Reflection

' System
Imports System

' フレームワーク
Imports Touryo.Infrastructure.Framework.Util

Namespace Touryo.Infrastructure.Business.Util
    ''' <summary>カスタム属性クラス</summary>
    ''' <remarks>自由に（拡張して）利用できる。</remarks>
    <System.AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Method, AllowMultiple:=True)> _
    Public Class MyAttribute
        Inherits Attribute
        ''' <summary>カスタム属性A</summary>
        ''' <remarks>自由に利用できる。</remarks>
        Public MyAttributeA As String = ""

        ''' <summary>カスタム属性B</summary>
        ''' <remarks>自由に利用できる。</remarks>
        Public MyAttributeB As String = ""

        ''' <summary>カスタム属性C</summary>
        ''' <remarks>自由に利用できる。</remarks>
        Public MyAttributeC As String = ""

        ''' <summary>当該カスタム属性クラスを取得する（クラス属性用）</summary>
        ''' <param name="obj">クラスのオブジェクト</param>
        ''' <param name="myAttribute">カスタム属性クラスの配列</param>
        ''' <remarks>自由に（拡張して）利用できる。</remarks>
        Public Shared Sub GetAttr(ByVal obj As Object, ByRef myAttribute As MyAttribute())
            ' 属性クラスのリストを取得する。
            ' inheritは、AllowMultipleに合わせる。
            ' http://msdn.microsoft.com/ja-jp/library/system.type.getcustomattributes.aspx
            Dim list As Object() = obj.GetType().GetCustomAttributes(GetType(MyAttribute), True)

            ' object[]をMyAttribute[]に変換して返す。
            Dim i As Integer = 0
            myAttribute = New MyAttribute(list.Length - 1) {}
            For Each temp As MyAttribute In list
                myAttribute(i) = temp
            Next
        End Sub

        ''' <summary>カスタム属性クラスを取得する（メソッド属性用）</summary>
        ''' <param name="methodInfo">メソッドのMethodInfo</param>
        ''' <param name="myAttribute">カスタム属性クラスの配列</param>
        ''' <remarks>自由に（拡張して）利用できる。</remarks>
        Public Shared Sub GetAttr(ByVal methodInfo As MethodInfo, ByRef myAttribute As MyAttribute())
            ' 属性クラスのリストを取得する。inheritは、AllowMultipleに合わせる。
            ' http://msdn.microsoft.com/ja-jp/library/system.type.getcustomattributes.aspx
            Dim list As Object() = methodInfo.GetCustomAttributes(GetType(MyAttribute), True)

            ' object[]をMyAttribute[]に変換して返す。
            Dim i As Integer = 0
            myAttribute = New MyAttribute(list.Length - 1) {}
            For Each temp As MyAttribute In list
                myAttribute(i) = temp
            Next
        End Sub
    End Class

End Namespace
