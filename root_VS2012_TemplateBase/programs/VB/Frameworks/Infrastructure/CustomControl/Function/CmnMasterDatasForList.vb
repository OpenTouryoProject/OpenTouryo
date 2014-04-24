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
'* クラス名        ：CmnMasterDatasForList
'* クラス日本語名  ：リスト用マスタデータ関連処理クラス（テンプレート）
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
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text

' System.Web
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Touryo.Infrastructure.CustomControl
	''' <summary>リスト用マスタデータ関連処理クラス</summary>
	Public Class CmnMasterDatasForList
		#Region "マスタデータ名の収集"

		''' <summary>マスタデータ名の収集</summary>
		''' <param name="parentCtrl"></param>
		''' <param name="masterDataNames"></param>
		Public Shared Sub GetMasterDataNames(parentCtrl As Control, masterDataNames As List(Of String))
			If masterDataNames Is Nothing Then
				masterDataNames = New List(Of String)()
			End If

			' 対象のコントロールなら、
			If TypeOf parentCtrl Is WebCustomDropDownList Then
				' || WinCustomXXXX.etc)
				' 新規か？
				Dim isNew As Boolean = True

				' マスタデータ名を取得
				Dim im As IMasterData = DirectCast(parentCtrl, IMasterData)

				For Each mdn As String In masterDataNames
					If mdn = im.MasterDataName Then
						' 一致 → 新規でない。
						isNew = False
					End If
				Next

				' 新規か？
				If isNew Then
					' 新規の場合は追加する。
					masterDataNames.Add(im.MasterDataName)
				End If
			End If

			' コントロールを再起検索する。
			For Each childctrl As Control In parentCtrl.Controls
				CmnMasterDatasForList.GetMasterDataNames(childctrl, masterDataNames)
			Next
		End Sub

		#End Region

		#Region "マスタデータの設定"

		''' <summary>マスタデータの保管場所（Sessionプロパティ化）</summary>
		Private Shared Property MasterDatas() As Dictionary(Of String, IEnumerable)
			Get
				' 初期化
				If HttpContext.Current.Session("wcc_masterDatas") Is Nothing Then
					HttpContext.Current.Session("wcc_masterDatas") = New Dictionary(Of String, IEnumerable)()
				End If

				Return DirectCast(HttpContext.Current.Session("wcc_masterDatas"), Dictionary(Of String, IEnumerable))
			End Get
			Set
				HttpContext.Current.Session("wcc_masterDatas") = value
			End Set
		End Property

		''' <summary>マスタデータを設定</summary>
		Public Shared Sub ClearMasterData()
			CmnMasterDatasForList.MasterDatas = New Dictionary(Of String, IEnumerable)()
		End Sub

		''' <summary>マスタデータを設定</summary>
		''' <param name="name">マスタデータ名</param>
		''' <param name="obj">マスタデータ</param>
		Public Shared Sub SetMasterData(name As String, obj As IEnumerable)
			If name Is Nothing Then
				name = ""
			End If
			name = name.Replace("　", "").Replace(" ", "").ToUpper()

			CmnMasterDatasForList.MasterDatas(name) = obj
		End Sub

		''' <summary>マスタデータを取得</summary>
		''' <param name="name">マスタデータ名</param>
		''' <returns>マスタデータ</returns>
		''' <remarks>データソースに指定する用</remarks>
		Public Shared Function GetMasterData(name As String) As IEnumerable
			If name Is Nothing Then
				name = ""
			End If
			name = name.Replace("　", "").Replace(" ", "").ToUpper()

			If CmnMasterDatasForList.MasterDatas.ContainsKey(name) Then
				Return CmnMasterDatasForList.MasterDatas(name)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>マスタデータを取得</summary>
		''' <param name="name">マスタデータ名</param>
		''' <param name="items">itemsプロパティ</param>
		''' <remarks>itemsプロパティに設定する用</remarks>
		Public Shared Sub GetMasterData(name As String, items As IList)
			If name Is Nothing Then
				name = ""
			End If
			name = name.Replace("　", "").Replace(" ", "").ToUpper()

			If CmnMasterDatasForList.MasterDatas.ContainsKey(name) Then
				Dim ie As IEnumerable = CmnMasterDatasForList.MasterDatas(name)

				If ie Is Nothing Then
					Return
				End If

				For Each obj As Object In ie
					items.Add(obj)
				Next
			End If
		End Sub

		''' <summary>マスタデータを削除</summary>
		''' <param name="name">マスタデータ名</param>
		Public Shared Sub DeleteMasterData(name As String)
			If name Is Nothing Then
				name = ""
			End If
			name = name.Replace("　", "").Replace(" ", "").ToUpper()

			If CmnMasterDatasForList.MasterDatas.ContainsKey(name) Then
				CmnMasterDatasForList.MasterDatas.Remove(name)
			End If
		End Sub

		#End Region
	End Class
End Namespace
