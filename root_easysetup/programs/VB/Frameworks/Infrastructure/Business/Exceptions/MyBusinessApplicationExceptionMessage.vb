﻿'**********************************************************************************
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
'* クラス名        ：MyBusinessApplicationExceptionMessage
'* クラス日本語名  ：業務例外のメッセージＩＤ、メッセージに使用する
'*                   文字列定数を定義する定数クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2011/10/09  西野  大介        国際化対応
'*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
'*  2013/01/23  Rituparna Biswas  Review for globalization.
'*  2014/02/03  西野  大介        取り込み：リソースファイル名とスイッチ名の変更、#pragma warning disableの追加。
'**********************************************************************************

' System
Imports System
Imports System.Threading
Imports System.Resources
Imports System.Globalization
Imports Touryo.Infrastructure.Public.Util
Imports My.Resources
Imports System.Configuration



Namespace Touryo.Infrastructure.Business.Exceptions
    ''' <summary>
    ''' Business層の
    ''' 業務例外のメッセージＩＤ、メッセージに
    ''' 使用する文字列定数を定義する定数クラス
    ''' </summary>
    Public Class MyBusinessApplicationExceptionMessage
#Region "SAMPLE_ERROR"
        ''' <summary>業務例外のメッセージＩＤ、メッセージに使用する文字列定数（例）</summary>
        Public Shared ReadOnly Property SAMPLE_ERROR() As String()
            Get
                Dim temp As String = ""
                'Get current property name.
                'string key = PubCmnFunction.GetCurrentMethodName();
                Dim key As String = PubCmnFunction.GetCurrentPropertyName()

                ' Stores the specified string resource for the specified culture or current UI culture.
                temp = MyBusinessApplicationExceptionMessage.CmnFunc(key)

                ' Returns the specified string resource  for the specified culture or current UI culture.
                Return New String() {"MessageID_SampleError", temp}
            End Get
        End Property
#End Region

#Region "CmnFunc"
        '''<summary>Returns the specified string resource for the specified culture or current UI culture. </summary> 
        '''<param name="key">resource key</param> 
        '''<returns>resource string</returns>
        Private Shared Function CmnFunc(ByVal key As String) As String
            ' We acquire ResourceManager.
            Dim rm As ResourceManager = MyBusinessApplicationExceptionMessageResource.ResourceManager

            'We acquire a value from App.Config.
            Dim FxUICulture As String = GetConfigParameter.GetConfigValue(PubLiteral.EXCEPTIONMESSAGECULTUER)

            If String.IsNullOrEmpty(FxUICulture) Then

                'When the key is not set to App.Config, we use a default culture. 
                Return rm.GetString(key)
            Else
                Try
                    ' When the key is set to App.Config, we use the specified culture.

                    Dim culture As CultureInfo = New CultureInfo(FxUICulture)
                    Return rm.GetString(key, culture)

                Catch ex As Exception
                    'When the specified culture is not an effective name, we use a default culture.
                    Return rm.GetString(key)
                End Try
            End If
        End Function
#End Region
    End Class


End Namespace
