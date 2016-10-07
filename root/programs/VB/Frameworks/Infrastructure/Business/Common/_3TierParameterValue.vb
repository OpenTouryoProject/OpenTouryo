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
'* クラス名        ：_3TierParameterValue
'* クラス日本語名  ：三層データバインド用の引数クラス
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

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
'・・・

' 部品
Imports Touryo.Infrastructure.Public.Db

Namespace Touryo.Infrastructure.Business.Common
	''' <summary>三層データバインド用の引数クラス</summary>
	Public Class _3TierParameterValue
		Inherits MyParameterValue
		''' <summary>テーブル名</summary>
		Public DBMSType As DbEnum.DBMSType

		''' <summary>汎用エリア</summary>
		Public Obj As Object

		''' <summary>テーブル名</summary>
		Public TableName As String

		''' <summary>カラムリスト（射影）</summary>
		Public ColumnList As String

		''' <summary>検索条件　AND, ＝</summary>
		Public AndEqualSearchConditions As Dictionary(Of String, Object)

		''' <summary>検索条件　AND, Like</summary>
		Public AndLikeSearchConditions As Dictionary(Of String, String)

		''' <summary>検索条件　OR, ＝</summary>
		Public OrEqualSearchConditions As Dictionary(Of String, Object())

		''' <summary>検索条件　OR, Like</summary>
		Public OrLikeSearchConditions As Dictionary(Of String, String())

		''' <summary>検索条件　その他</summary>
		Public ElseSearchConditions As Dictionary(Of String, Object)

		''' <summary>検索条件　その他に対応するWhere句</summary>
		Public ElseWhereSQL As String

		''' <summary>ソート列</summary>
		Public SortExpression As String

		''' <summary>ソート方向</summary>
		Public SortDirection As String

		''' <summary>開始行番号（ページング時）</summary>
		Public StartRowIndex As Integer

		''' <summary>最大行番号（ページング時）</summary>
		Public MaximumRows As Integer

		''' <summary>追加・更新値</summary>
		Public InsertUpdateValues As Dictionary(Of String, Object)

        ''' <summary>Target Table Name</summary>
        public TargetTableNames As Dictionary(Of Integer, String) 

		''' <summary>データテーブルの型情報</summary>
		''' <remarks>型付きデータテーブルのを指定可能にする</remarks>
		''' <example>typeof(xxxx.xxxDataTable)</example>
		Public DataTableType As Type

		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo)
				' Baseのコンストラクタに引数を渡すために必要。
			MyBase.New(screenId, controlId, methodName, actionType, user)
		End Sub

		#End Region
	End Class
End Namespace
