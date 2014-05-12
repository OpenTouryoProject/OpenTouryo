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
'* クラス名        ：RcMyCmnFunction
'* クラス日本語名  ：Business.RichClient層の共通クラス
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2010/11/21  西野  大介        新規作成
'*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
'**********************************************************************************

' System
Imports System
Imports System.Xml
Imports System.Data
Imports System.Collections
Imports System.Collections.Generic

' Windowアプリケーション
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' WPFアプリケーション
Imports System.Windows

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.RichClient.Util
	''' <summary>Business.RichClient層の共通クラス</summary>
	Public Class RcMyCmnFunction
		''' <summary>コントロール取得＆イベントハンドラ設定</summary>
		''' <param name="ctrl">コントロール</param>
		''' <param name="prefix">プレフィックス</param>
		''' <param name="eventHandler">イベント ハンドラ</param>
		''' <param name="ControlHt">ディクショナリ</param>
		Friend Shared Sub GetCtrlAndSetClickEventHandler(ctrl As Control, prefix As String, eventHandler As Object, ControlHt As Dictionary(Of String, Control))
			'#Region "チェック処理"

			' コントロール指定が無い場合
			If ctrl Is Nothing Then
				' 何もしないで戻る。
				Return
			End If

			' プレフィックス指定が無い場合
			If prefix Is Nothing OrElse prefix = "" Then
				' 何もしないで戻る。
				Return
			End If

			'#End Region

			'#Region "コントロール取得＆イベントハンドラ設定"

			' コントロールのNameチェック
					' コントロールName無し
			If ctrl.Name Is Nothing Then
			Else
				' コントロールName有り

				' コントロールのName長確認
				If prefix.Length <= ctrl.Name.Length Then
					' 指定のプレフィックス
					If prefix = ctrl.Name.Substring(0, prefix.Length) Then
						' イベントハンドラを設定する。
						If prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX) Then
							' CHECK BOX
							Dim checkBox As CheckBox = Nothing

							Try
								' キャストできる
								checkBox = DirectCast(ctrl, CheckBox)
							Catch ex As Exception
								' キャストできない
								Throw New FrameworkException(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(0), [String].Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(1), prefix, ctrl.[GetType]().ToString()), ex)
							End Try

                            AddHandler checkBox.CheckedChanged, DirectCast(eventHandler, EventHandler)

							' ディクショナリに格納
							' ControlHt.Add(ctrl.Name, ctrl);
								' 2009/08/10-この行
							ControlHt(ctrl.Name) = ctrl
						End If
					End If
				End If
			End If

			'#End Region

			'#Region "再起"

			' 子コントロールがある場合、
			If ctrl.Controls.Count <> 0 Then
				' 子コントロール毎に
				For Each childCtrl As Control In ctrl.Controls
					' 再起する。
					RcMyCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, ControlHt)
				Next
			End If

			'#End Region
		End Sub

		''' <summary>コントロール取得＆イベントハンドラ設定</summary>
		''' <param name="ctrl">コントロール</param>
		''' <param name="prefixAndEvtHndHt">プレフィックスとイベント ハンドラのディクショナリ</param>
		''' <param name="controlHt">コントロールのディクショナリ</param>
		Friend Shared Sub GetCtrlAndSetClickEventHandler2(ctrl As Control, prefixAndEvtHndHt As Dictionary(Of String, Object), controlHt As Dictionary(Of String, Control))
			' ループ
			For Each prefix As String In prefixAndEvtHndHt.Keys
				Dim eventHandler As Object = prefixAndEvtHndHt(prefix)

				'#Region "チェック処理"

				' コントロール指定が無い場合
				If ctrl Is Nothing Then
					' 何もしないで戻る。
					Return
				End If

				' プレフィックス指定が無い場合
				If prefix Is Nothing OrElse prefix = "" Then
					' 何もしないで戻る。
					Return
				End If

				'#End Region

				'#Region "コントロール取得＆イベントハンドラ設定"

				' コントロールのNameチェック
						' コントロールName無し
				If ctrl.Name Is Nothing Then
				Else
					' コントロールName有り

					' コントロールのName長確認
					If prefix.Length <= ctrl.Name.Length Then
						' 指定のプレフィックス
						If prefix = ctrl.Name.Substring(0, prefix.Length) Then
							' イベントハンドラを設定する。
							If prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX) Then
								' CHECK BOX
								Dim checkBox As CheckBox = Nothing

								If TypeOf ctrl Is CheckBox Then
									' キャストできる
									checkBox = DirectCast(ctrl, CheckBox)
								Else
									' キャストできない
									Throw New FrameworkException(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(0), [String].Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(1), prefix, ctrl.[GetType]().ToString()))
								End If

                                AddHandler checkBox.CheckedChanged, DirectCast(eventHandler, EventHandler)

								' ディクショナリに格納
								controlHt(ctrl.Name) = ctrl
								Exit For
							End If
						End If
					End If

					'#End Region
				End If
			Next

			'#Region "再起"

			' 子コントロールがある場合、
			If ctrl.Controls.Count <> 0 Then
				' 子コントロール毎に
				For Each childCtrl As Control In ctrl.Controls
					' 再起する。
					RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt)
				Next
			End If

			'#End Region
		End Sub
	End Class
End Namespace
