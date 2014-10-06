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
'* クラス名        ：CmnTableAdapter
'* クラス日本語名  ：三層データバインド用のTableAdapter共通（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

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
