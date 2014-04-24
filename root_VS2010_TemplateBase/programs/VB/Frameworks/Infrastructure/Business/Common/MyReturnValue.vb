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
'* クラス名        ：MyReturnValue
'* クラス日本語名  ：戻り値親クラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 更新履歴        ：－
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2009/04/02  西野  大介        シリアライズ可能にする（WS対応）
'**********************************************************************************

' System
Imports System

' フレームワーク
Imports Touryo.Infrastructure.Framework.Common

Namespace Touryo.Infrastructure.Business.Common
    ''' <summary>戻り値親クラス２</summary>
    ''' <remarks>
    ''' シリアライズ可能にする（WS対応）自由に（拡張して）利用できる。
    ''' </remarks>
    <Serializable()> _
    Public Class MyReturnValue
        Inherits BaseReturnValue
    End Class
End Namespace
