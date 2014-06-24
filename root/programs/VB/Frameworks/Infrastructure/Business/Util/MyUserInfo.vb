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
'* クラス名        ：MyUserInfo
'* クラス日本語名  ：ユーザ情報クラス（必要なコンテキスト情報を追加）（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2010/09/24  西野  大介        フィールドの追加
'*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
'**********************************************************************************

' System
Imports System

' フレームワーク
Imports Touryo.Infrastructure.Framework.Util

Namespace Touryo.Infrastructure.Business.Util
    ''' <summary>ユーザ情報クラス（必要なコンテキスト情報を追加）</summary>
    ''' <remarks>自由に（拡張して）利用できる。</remarks>
    <Serializable()> _
    Public Class MyUserInfo
        Inherits UserInfo
        ''' <summary>ユーザ名</summary>
        Private _userName As String = ""

        ''' <summary>IPアドレス</summary>
        Private _ipAddress As String

        ''' <summary>コンストラクタ</summary>
        ''' <param name="userName">ユーザ名</param>
        ''' <param name="ipAddress">IPアドレス</param>
        ''' <remarks>自由に利用できる。</remarks>
        Public Sub New(ByVal userName As String, ByVal ipAddress As String)
            Me._userName = userName
            Me._ipAddress = ipAddress
        End Sub

        ''' <summary>ユーザ名</summary>
        ''' <remarks>自由に利用できる。</remarks>
        Public Property UserName() As String
            Get
                Return Me._userName
            End Get
            Set(ByVal value As String)
                Me._userName = value
            End Set
        End Property

        ''' <summary>IPアドレス</summary>
        ''' <remarks>自由に利用できる。</remarks>
        Public Property IPAddress() As String
            Get
                Return Me._ipAddress
            End Get
            Set(ByVal value As String)
                Me._ipAddress = value
            End Set
        End Property
    End Class
End Namespace
