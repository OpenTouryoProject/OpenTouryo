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
'* クラス名        ：CmnDao
'* クラス日本語名  ：共通Daoクラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
'*                                nullチェック方法、Contains → ContainsKeyなどに注意
'*  2010/11/02  西野 大介         GetParameterメソッドを追加（ｽﾄｱﾄﾞ ﾕｰｻﾞﾋﾞﾘﾃｨ向上）
'*  2010/11/02  西野 大介         その他、リファクタリングなど（メソッド名、修飾子の変更）
'*                                特にprotected → public化の「new & base」に注意！
'*                                （ミスると再帰呼び出しの無限ループになる...疎通で確認可）
'*  2011/10/09  西野 大介         国際化対応
'*  2012/06/14  西野 大介         ResourceLoaderに加え、EmbeddedResourceLoaderに対応
'*  2013/07/07  西野 大介         ExecGenerateSQL（SQL生成）メソッド（実行しない）を追加
'*  2014/11/20  Sandeep           Implemented CommandTimeout property and SetCommandTimeout method.
'*  2014/11/20  Sai               removed IDbCommand property in SetCommandTimeout method.
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Public.Db

Namespace Touryo.Infrastructure.Business.Dao
	''' <summary>共通Daoクラス</summary>
	''' <remarks>自由に（拡張して）利用できる。</remarks>
	Public Class CmnDao
		Inherits MyBaseDao
		#Region "インスタンス変数"

		#Region "パラメタ"

		''' <summary>ユーザ パラメタ（文字列置換）用ディクショナリ</summary>
		Private DicUserParameter As New Dictionary(Of String, String)()

		''' <summary>パラメタ ライズド クエリのパラメタ用ディクショナリ</summary>
		Private DicParameter As New Dictionary(Of String, Object)()

		''' <summary>パラメタ ライズド クエリの指定されたパラメータ（の型）を保持するディクショナリ</summary>
		Private DicParameterType As New Dictionary(Of String, Object)()
		''' <summary>パラメタ ライズド クエリの指定されたパラメータ（のサイズ）を保持するディクショナリ</summary>
		Private DicParameterSize As New Dictionary(Of String, Integer)()
		''' <summary>パラメタ ライズド クエリの指定されたパラメータ（の方向）を保持するディクショナリ</summary>
		Private DicParameterDirection As New Dictionary(Of String, ParameterDirection)()

		#End Region

		#Region "パラメタの制御"

		''' <summary>パラメタライズドクエリのパラメタを取得する（Out,RetValパラメタ用）。</summary>
		''' <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
		''' <returns>Out,RetValパラメタのバリュー</returns>
		''' <remarks>
		''' 動的SQLの場合はSQL実行後に利用可能
		''' </remarks>
		Public Shadows Function GetParameter(parameterName As String) As Object
			' ★ ベースのメソッドを呼ぶ
			Return MyBase.GetParameter(parameterName)

		End Function

		''' <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="obj">パラメタ値</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub SetParameter(parameterName As String, obj As Object)
			' ユーザ パラメタをディクショナリに設定
			Me.DicParameter(parameterName) = obj
		End Sub

		''' <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="obj">パラメタ値</param>
		''' <param name="dbTypeInfo">パラメタの型</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub SetParameter(parameterName As String, obj As Object, dbTypeInfo As Object)
			' ユーザ パラメタをディクショナリに設定
			Me.DicParameter(parameterName) = obj

			' 機能改善
			Me.DicParameterType(parameterName) = dbTypeInfo
		End Sub

		''' <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="obj">パラメタ値</param>
		''' <param name="dbTypeInfo">パラメタの型</param>
		''' <param name="size">パラメタのサイズ</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub SetParameter(parameterName As String, obj As Object, dbTypeInfo As Object, size As Integer)
			' ユーザ パラメタをディクショナリに設定
			Me.DicParameter(parameterName) = obj

			' 機能改善
			Me.DicParameterType(parameterName) = dbTypeInfo
			Me.DicParameterSize(parameterName) = size
		End Sub

		''' <summary>パラメタ ライズド クエリのパラメタをディクショナリに設定する。</summary>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="obj">パラメタ値</param>
		''' <param name="dbTypeInfo">パラメタの型</param>
		''' <param name="size">パラメタのサイズ</param>
		''' <param name="paramDirection">パラメタの方向</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub SetParameter(parameterName As String, obj As Object, dbTypeInfo As Object, size As Integer, paramDirection As ParameterDirection)
			' ユーザ パラメタをディクショナリに設定
			Me.DicParameter(parameterName) = obj

			' 機能改善
			Me.DicParameterType(parameterName) = dbTypeInfo
			Me.DicParameterSize(parameterName) = size
			Me.DicParameterDirection(parameterName) = paramDirection
		End Sub

		''' <summary>ユーザ パラメタ（文字列置換）をディクショナリに設定する。</summary>
		''' <param name="userParamName">ユーザ パラメタ名</param>
		''' <param name="userParamValue">ユーザ パラメタ値</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub SetUserParameter(userParamName As String, userParamValue As String)
			' ユーザ パラメタをディクショナリに設定
			Me.DicUserParameter(userParamName) = userParamValue
		End Sub

		''' <summary>
		''' ・ユーザ パラメタ（文字列置換）
		''' ・パラメタ ライズド クエリのパラメタ
		''' を格納するディクショナリをクリアする。
		''' </summary>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Sub ClearParameters()
			' ユーザ パラメタ（文字列置換）用ディクショナリを初期化
			Me.DicUserParameter = New Dictionary(Of String, String)()
			' パラメタ ライズド クエリのパラメタ用ディクショナリを初期化
			Me.DicParameter = New Dictionary(Of String, Object)()

			' 同上
			Me.DicParameterType = New Dictionary(Of String, Object)()
			Me.DicParameterSize = New Dictionary(Of String, Integer)()
			Me.DicParameterDirection = New Dictionary(Of String, ParameterDirection)()
		End Sub

		#End Region

		#Region "SQLファイル名"

		''' <summary>SQLファイル名</summary>
		Private _sQLFileName As String = ""

		#Region "プロパティ プロシージャ"

		''' <summary>SQLファイル名</summary>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public WriteOnly Property SQLFileName() As String
			Set
				Me._sQLFileName = value
				Me._sQLText = ""
			End Set
		End Property

		#End Region

		#End Region

		#Region "SQLテキスト"

		''' <summary>SQLテキスト</summary>
		Private _sQLText As String = ""

		#Region "プロパティ プロシージャ"

		''' <summary>SQLテキスト</summary>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public WriteOnly Property SQLText() As String
			Set
				Me._sQLText = value
				Me._sQLFileName = ""
			End Set
		End Property

		#End Region

		#End Region

		#Region "CommandTimeout"

		''' <summary>CommandTimeout</summary>
		Private _commandTimeout As Integer = -1

		#Region "プロパティ プロシージャ"

		''' <summary>CommandTimeout</summary>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public WriteOnly Property CommandTimeout() As Integer
			Set(value As Integer)
				Me._commandTimeout = value
			End Set
		End Property

		#End Region

		#End Region

		#End Region

		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		''' <remarks>自由に利用できる。</remarks>
		Public Sub New(dam As BaseDam)
			MyBase.New(dam)
		End Sub

		#End Region

		#Region "クエリ メソッド"

		#Region "Exec(new & base)"

		''' <summary>Command.ExecuteScalarメソッドでデータを取得する。</summary>
		''' <returns>データ</returns>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Function ExecSelectScalar() As Object
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' SQLを実行し、データを戻す。
			' （★ ベースのメソッドを呼ぶ）
			Return MyBase.ExecSelectScalar()
		End Function

		''' <summary>DataAdapter.Fill(DataTable)メソッドでデータを取得する。</summary>
		''' <param name="dt">結果セット（データ テーブル）</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub ExecSelectFill_DT(dt As DataTable)
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' SQLを実行し、結果セット（データ テーブル）を戻す。
			' （★ ベースのメソッドを呼ぶ）
			MyBase.ExecSelectFill_DT(dt)
		End Sub

		''' <summary>DataAdapter.Fill(DataSet)メソッドでデータを取得する。</summary>
		''' <param name="ds">結果セット（データ セット）</param>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Sub ExecSelectFill_DS(ds As DataSet)
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' SQLを実行し、結果セット（データ セット）を戻す。
			' （★ ベースのメソッドを呼ぶ）
			MyBase.ExecSelectFill_DS(ds)
		End Sub

		''' <summary>Command.ExecuteReaderメソッドでデータを取得する。</summary>
		''' <returns>結果セット（データ リーダ）</returns>
		''' <remarks>自由に（拡張して）利用できる。</remarks>        
		Public Shadows Function ExecSelect_DR() As IDataReader
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' SQLを実行し、結果セット（データ リーダ）を戻す。
			' （★ ベースのメソッドを呼ぶ）
			Return MyBase.ExecSelect_DR()
		End Function

		''' <summary>Command.ExecuteNonQueryメソッドでSQLを実行する。</summary>
		''' <returns>影響を受けた行の数</returns>
		''' <remarks>自由に（拡張して）利用できる。</remarks>        
		Public Shadows Function ExecInsUpDel_NonQuery() As Integer
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' SQLを実行し、戻り値を戻す。
			' （★ ベースのメソッドを呼ぶ）
			Return MyBase.ExecInsUpDel_NonQuery()
		End Function

		''' <summary>ExecGenerateSQLで静的SQLを生成する</summary>
		''' <param name="sqlUtil">SQLUtility</param>
		''' <returns>SQL文</returns>
		''' <remarks>自由に（拡張して）利用できる。</remarks>
		Public Shadows Function ExecGenerateSQL(sqlUtil As SQLUtility) As String
			' SQLの設定
            Me.SetSQL()

            ' To Set CommandTimeout
            Me.SetCommandTimeout()

			' パラメタの一括設定
			Me.SetParameters()

			' 静的SQLを生成する。
			' （★ ベースのメソッドを呼ぶ）
			Return MyBase.ExecGenerateSQL(sqlUtil)
		End Function

		#End Region

		#Region "共通関数"

		''' <summary>SQLの指定</summary>
		Private Sub SetSQL()
			' SQL指定
			If Me._sQLFileName <> "" Then
				' ファイルから
				Me.SetSqlByFile2(Me._sQLFileName)
			ElseIf Me._sQLText <> "" Then
				' テキストから
				Me.SetSqlByCommand(Me._sQLText)
			Else
				' SQLエラー
				Throw New BusinessSystemException(MyBusinessSystemExceptionMessage.CMN_DAO_ERROR(0), [String].Format(MyBusinessSystemExceptionMessage.CMN_DAO_ERROR(1), MyBusinessSystemExceptionMessage.CMN_DAO_ERROR_SQL))
			End If
        End Sub

        ''' <summary>To Set CommandTimeout</summary>
        Private Sub SetCommandTimeout()
            ' If CommandTimeout is >= 0 then set CommandTimeout.
            ' Else skip, automatically it will set default CommandTimeout.
            If Me._commandTimeout >= 0 Then
                Me.GetDam().DamIDbCommand.CommandTimeout = Me._commandTimeout
            End If
        End Sub

		''' <summary>パラメタの一括設定（内部用）</summary>
		Private Sub SetParameters()
			' ユーザ パラメタ（文字列置換）を設定する。
			For Each userParamName As String In Me.DicUserParameter.Keys
				' ★ ベースのメソッドを呼ぶ
				MyBase.SetUserParameter(userParamName, Me.DicUserParameter(userParamName).ToString())
			Next

			' パラメタ ライズド クエリのパラメタを設定する。
			For Each paramName As String In Me.DicParameter.Keys
				' 機能改善

				' デフォルト値
				Dim type As Object = Nothing
				Dim size As Integer = -1
				Dim direction As ParameterDirection = ParameterDirection.Input

				' あったら設定（DicParameterType）
				If Me.DicParameterType.ContainsKey(paramName) Then
					type = Me.DicParameterType(paramName)
				End If

				' あったら設定（DicParameterSize）
				If Me.DicParameterSize.ContainsKey(paramName) Then
					size = Me.DicParameterSize(paramName)
				End If

				' あったら設定（DicParameterDirection）
				If Me.DicParameterDirection.ContainsKey(paramName) Then
					direction = Me.DicParameterDirection(paramName)
				End If

				' ★ ベースのメソッドを呼ぶ
				MyBase.SetParameter(paramName, Me.DicParameter(paramName), type, size, direction)
			Next
		End Sub

		#End Region

		#End Region
	End Class
End Namespace
