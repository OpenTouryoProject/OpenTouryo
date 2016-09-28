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
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_3TierReturnValue
'* クラス日本語名  ：三層データバインド用の戻り値クラス
'*
'* 作成日時        ：－
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2013/01/10  西野　大介        新規作成
'*
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

Namespace Touryo.Infrastructure.Business.Common
	''' <summary>三層データバインド用の戻り値クラス</summary>
	Public Class _3TierReturnValue
		Inherits MyReturnValue
		''' <summary>汎用エリア</summary>
		Public Obj As Object

		''' <summary>データテーブル</summary>
		Public Dt As DataTable
	End Class
End Namespace
