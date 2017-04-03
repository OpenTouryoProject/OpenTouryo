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
'* クラス名        ：LoginViewModel
'* クラス日本語名  ：LoginViewModel
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.ComponentModel.DataAnnotations

Namespace Models.ViewModels
    ''' <summary>LoginViewModel</summary>
    Public Class LoginViewModel
        ''' <summary>
        ''' UserName
        ''' </summary>
        <Required(AllowEmptyStrings:=False)>
        <Display(Name:="User name")>
        Public Property UserName() As String
            Get
                Return m_UserName
            End Get
            Set
                m_UserName = Value
            End Set
        End Property
        Private m_UserName As String

        ''' <summary>
        ''' PWDS
        ''' </summary>
        <Required(AllowEmptyStrings:=False)>
        <Display(Name:="passwowd")>
        Public Property Passwowd() As String
            Get
                Return m_Passwowd
            End Get
            Set
                m_Passwowd = Value
            End Set
        End Property
        Private m_Passwowd As String
    End Class
End Namespace
