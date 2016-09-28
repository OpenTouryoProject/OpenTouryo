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
'* クラス名        ：IMasterData
'* クラス日本語名  ：マスタデータ名取得のインターフェイス（テンプレート）
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

' System.Web
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports System.Diagnostics
Imports System.Globalization

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>
	''' マスタデータ名取得のインターフェイス（テンプレート）
	''' </summary>
	Public Interface IMasterData
		''' <summary>マスタデータ名</summary>
		<DefaultValue(""), Description("マスタデータ名")> _
		Property MasterDataName() As String

	End Interface
End Namespace
