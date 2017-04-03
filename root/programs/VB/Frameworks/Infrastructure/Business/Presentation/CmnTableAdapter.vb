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
'* クラス名        ：CmnTableAdapter
'* クラス日本語名  ：三層データバインド用のTableAdapter共通（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'**********************************************************************************

Imports System.Web

Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Public.Db

Namespace Touryo.Infrastructure.Business.Presentation
	''' <summary>三層データバインド用のTableAdapter共通（テンプレート）</summary>
	Public Class CmnTableAdapter
		''' <summary>三層データバインド用の引数クラスを生成</summary>
		''' <param name="tableName">テーブル名</param>
		''' <param name="methodName">メソッド名</param>
		''' <param name="myUserInfo">ユーザ情報</param>
		''' <returns>三層データバインド用の引数クラス</returns>
		Protected Function CreateParameter(tableName As String, methodName As String, myUserInfo As MyUserInfo) As _3TierParameterValue
			' 三層データバインド用の引数クラスを生成
			Dim parameterValue As New _3TierParameterValue(tableName & "ConditionalSearch", tableName & "TableAdapter", methodName, DirectCast(HttpContext.Current.Session("DAP"), String), myUserInfo)

			' DBMS
			parameterValue.DBMSType = DirectCast(HttpContext.Current.Session("DBMS"), DbEnum.DBMSType)

			' テーブル
			parameterValue.TableName = tableName

			' 検索条件（Sessionはnullチェック不要）
			'#Region "AND"

			parameterValue.AndEqualSearchConditions = DirectCast(HttpContext.Current.Session("AndEqualSearchConditions"), Dictionary(Of String, Object))

			parameterValue.AndLikeSearchConditions = DirectCast(HttpContext.Current.Session("AndLikeSearchConditions"), Dictionary(Of String, String))

			'#End Region

			'#Region "OR"

			parameterValue.OrEqualSearchConditions = DirectCast(HttpContext.Current.Session("OrEqualSearchConditions"), Dictionary(Of String, Object()))

			parameterValue.OrLikeSearchConditions = DirectCast(HttpContext.Current.Session("OrLikeSearchConditions"), Dictionary(Of String, String()))

			'#End Region

			'#Region "Else"

			parameterValue.ElseWhereSQL = DirectCast(HttpContext.Current.Session("ElseWhereSQL"), String)

			parameterValue.ElseSearchConditions = DirectCast(HttpContext.Current.Session("ElseSearchConditions"), Dictionary(Of String, Object))

			'#End Region

			Return parameterValue
		End Function
	End Class
End Namespace
