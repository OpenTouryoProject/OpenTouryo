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
'* クラス名        ：CmnCheckFunction
'* クラス日本語名  ：リッチクライアント用カスタムコントロールの共通関数クラス（テンプレート）
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
	''' <summary>リッチクライアント用カスタムコントロールの共通関数クラス</summary>
	Public Class CmnCheckFunction
		#Region "メッセージ"

		''' <summary>
		''' 必須チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsIndispensabileCheckErrorMessage As String = "必須チェック エラー"

		''' <summary>
		''' 半角チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsHankakuCheckErrorMessage As String = "半角チェック エラー"
		''' <summary>
		''' 全角チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsZenkakuCheckErrorMessage As String = "全角チェック エラー"

		''' <summary>
		''' 数値チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsNumericCheckErrorMessage As String = "数値チェック エラー"

		''' <summary>
		''' 片仮名チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsKatakanaCheckErrorMessage As String = "片仮名チェック エラー"
		''' <summary>
		''' 半角片仮名チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsHanKatakanaCheckErrorMessage As String = "半角片仮名チェック エラー"

		''' <summary>
		''' 平仮名チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsHiraganaCheckErrorMessage As String = "平仮名チェック エラー"

		''' <summary>
		''' 日付チェック エラー
		''' のエラー メッセージ
		''' </summary>
		Public Const IsDateCheckErrorMessage As String = "日付チェック エラー"

		''' <summary>
		''' 正規表現チェック エラーの
		''' エラー メッセージ
		''' </summary>
		Public Const RegularExpressionCheckErrorMessage As String = "正規表現チェック エラー"
		''' <summary>
		''' 禁則文字チェック エラーの
		''' エラー メッセージ
		''' </summary>
		Public Const ProhibitedCharsCheckErrorMessage As String = "禁則文字チェック エラー"

		#End Region

		#Region "チェック リテラル"

		''' <summary>禁則文字</summary>
		''' <remarks>
		''' ・#（シャープ）
		''' ・'（シングルクォーテーション）
		''' ・\（円マーク）
		''' ・|（パイプ）
		''' ・%（パーセント）
		''' ・_（アンダースコア）
		''' </remarks>
		Public Shared ReadOnly ProhibitedChars As Char() = {"#"C, "'"C, "\"C, "|"C, "%"C, "_"C}

		#End Region

		#Region "一括チェック"

		''' <summary>コントロールのバリデーション</summary>
		''' <param name="parentCtrl">チェックルートのコントロール</param>
		''' <param name="lstCheckResult">チェック結果を保持するリスト</param>
		''' <returns>
		''' ・エラーあり：true
		''' ・エラーなし：false
		''' </returns>
		Public Shared Function HasErrors(parentCtrl As Control, lstCheckResult As List(Of CheckResult)) As Boolean
			' チェック結果を保持するリスト
			If lstCheckResult Is Nothing Then
				lstCheckResult = New List(Of CheckResult)()
			End If

			' チェック対象のコントロールなら、
			If TypeOf parentCtrl Is WebCustomTextBox Then
				' チェックし、
				Dim temp As String() = Nothing
				Dim ic As ICheck = DirectCast(parentCtrl, ICheck)
				If Not ic.Validate(temp) Then
					' エラーならエラー情報を保持する。
					Dim cr As New CheckResult(parentCtrl.ID)
					cr.CheckErrorInfo = temp
					lstCheckResult.Add(cr)
				End If
			End If

			' コントロールを再起検索する。
			For Each childctrl As Control In parentCtrl.Controls
				CmnCheckFunction.HasErrors(childctrl, lstCheckResult)
			Next

			' エラーが有れば、trueを返す。
			Return (0 < lstCheckResult.Count)
		End Function

		#End Region
	End Class
End Namespace
