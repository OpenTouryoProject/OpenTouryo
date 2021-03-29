'**********************************************************************************
'* クラス名        ：_DaoClassName_
'* クラス日本語名  ：自動生成Ｄａｏクラス
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2012/06/14  西野  大介        ResourceLoaderに加え、EmbeddedResourceLoaderに対応
'*  2013/09/09  西野  大介        ExecGenerateSQLメソッドを追加した（バッチ更新用）。
'*  2014/11/20  Sandeep           Implemented CommandTimeout property and SetCommandTimeout method.
'*  2015/06/04  Sai               Replaced SqlCommand property with IDbCommand property in SetCommandTimeout method. 
'**********************************************************************************

#Region "using"

' System～
Imports System
Imports System.IO
Imports System.Data
Imports System.Collections

' フレームワーク
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Common

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Dao

#End Region

''' <summary>自動生成Ｄａｏクラス</summary>
Public Class _DaoClassName_
	Inherits MyBaseDao
	#Region "インスタンス変数"

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

	''' <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
	Protected HtUserParameter As New Hashtable()
	''' <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
	Protected HtParameter As New Hashtable()

	#End Region

	#Region "コンストラクタ"

	''' <summary>コンストラクタ</summary>
	Public Sub New(dam As BaseDam)
		MyBase.New(dam)
	End Sub

	#End Region

	#Region "共通関数（パラメタの制御）"
	
	''' <summary>To Set CommandTimeout</summary>
    Private Sub SetCommandTimeout()
        ' If CommandTimeout is >= 0 then set CommandTimeout.
        ' Else skip, automatically it will set default CommandTimeout.
        If Me._commandTimeout >= 0 Then
			Me.GetDam().DamIDbCommand.CommandTimeout = Me._commandTimeout
        End If
    End Sub

	''' <summary>ユーザ パラメタ（文字列置換）をハッシュ テーブルに設定する。</summary>
	''' <param name="userParamName">ユーザ パラメタ名</param>
	''' <param name="userParamValue">ユーザ パラメタ値</param>
	Public Sub SetUserParameteToHt(userParamName As String, userParamValue As String)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtUserParameter(userParamName) = userParamValue
	End Sub

	''' <summary>パラメタ ライズド クエリのパラメタをハッシュ テーブルに設定する。</summary>
	''' <param name="paramName">パラメタ名</param>
	''' <param name="paramValue">パラメタ値</param>
	Public Sub SetParameteToHt(paramName As String, paramValue As Object)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtParameter(paramName) = paramValue
	End Sub

	''' <summary>
	''' ・ユーザ パラメタ（文字列置換）
	''' ・パラメタ ライズド クエリのパラメタ
	''' を格納するハッシュ テーブルをクリアする。
	''' </summary>
	Public Sub ClearParametersFromHt()
		' ユーザ パラメタ（文字列置換）用ハッシュ テーブルを初期化
		Me.HtUserParameter = New Hashtable()
		' パラメタ ライズド クエリのパラメタ用ハッシュ テーブルを初期化
		Me.HtParameter = New Hashtable()
	End Sub

	''' <summary>パラメタの設定（内部用）</summary>
	Protected Sub SetParametersFromHt()
		' ユーザ パラメタ（文字列置換）を設定する。
		For Each userParamName As String In Me.HtUserParameter.Keys
			Me.SetUserParameter(userParamName, Me.HtUserParameter(userParamName).ToString())
		Next

		' パラメタ ライズド クエリのパラメタを設定する。
		For Each paramName As String In Me.HtParameter.Keys
			Me.SetParameter(paramName, Me.HtParameter(paramName))
		Next
	End Sub

	#End Region

	#Region "プロパティ プロシージャ（setter、getter）"

	' ControlComment:LoopStart-PKColumn

	''' <summary>_ColumnName_列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK__ColumnName_() As Object
		Get
			Return Me.HtParameter("_ColumnName_")
		End Get
		Set
			Me.HtParameter("_ColumnName_") = value
		End Set
	End Property

	' ControlComment:LoopEnd-PKColumn

	' ControlComment:LoopStart-ElseColumn

	''' <summary>_ColumnName_列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property _ColumnName_() As Object
		Get
			Return Me.HtParameter("_ColumnName_")
		End Get
		Set
			Me.HtParameter("_ColumnName_") = value
		End Set
	End Property
	' ControlComment:LoopEnd-ElseColumn

	' ControlComment:LoopStart-PPUpdSet

	''' <summary>_ColumnName_列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property _ColumnName_() As Object
		Get
			Return Me.HtParameter("_ColumnName_")
		End Get
		Set
			Me.HtParameter("_ColumnName_") = value
		End Set
	End Property

	' ControlComment:LoopEnd-PPUpdSet

	' ControlComment:LoopStart-PPLike

	''' <summary>_ColumnName_列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property _ColumnName_() As Object
		Get
			Return Me.HtParameter("_ColumnName_")
		End Get
		Set
			Me.HtParameter("_ColumnName_") = value
		End Set
	End Property

	' ControlComment:LoopEnd-PPLike

	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function _InsertMethodName_() As Integer
		' ファイルからSQL（Insert）を設定する。
        Me.SetSqlByFile2("_InsertFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Insert）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	''' <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
	Public Function _DynInsMethodName_() As Integer
		' ファイルからSQL（DynIns）を設定する。
        Me.SetSqlByFile2("_DynInsFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynIns）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Select"

	''' <summary>主キーを指定し、１レコード参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub _SelectMethodName_(dt As DataTable)
		' ファイルからSQL（Select）を設定する。
        Me.SetSqlByFile2("_SelectFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub _DynSelMethodName_(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
        Me.SetSqlByFile2("_DynSelFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynSel）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	#End Region

	#Region "Update"

	''' <summary>主キーを指定し、１レコード更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function _UpdateMethodName_() As Integer
		' ファイルからSQL（Update）を設定する。
        Me.SetSqlByFile2("_UpdateFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Update）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function _DynUpdMethodName_() As Integer
		' ファイルからSQL（DynUpd）を設定する。
        Me.SetSqlByFile2("_DynUpdFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynUpd）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Delete"

	''' <summary>主キーを指定し、１レコード削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function _DeleteMethodName_() As Integer
		' ファイルからSQL（Delete）を設定する。
        Me.SetSqlByFile2("_DeleteFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function _DynDelMethodName_() As Integer
		' ファイルからSQL（DynDel）を設定する。
        Me.SetSqlByFile2("_DynDelFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynDel）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "拡張メソッド"

	''' <summary>テーブルのレコード件数を取得する</summary>
	''' <returns>テーブルのレコード件数</returns>
	Public Function _DynSelCntMethodName_() As Object
		' ファイルからSQL（DynSelCnt）を設定する。
        Me.SetSqlByFile2("_DynSelCntFileName_")

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（SELECT COUNT）を実行し、戻り値を戻す。
		Return Me.ExecSelectScalar()
	End Function

	''' <summary>静的SQLを生成する。</summary>
	''' <param name="fileName">ファイル名</param>
	''' <param name="sqlUtil">SQLユーティリティ</param>
	''' <returns>生成した静的SQL</returns>
	Public Overloads Function ExecGenerateSQL(fileName As String, sqlUtil As SQLUtility) As String
		' ファイルからSQLを設定する。
        Me.SetSqlByFile2(fileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

		' パラメタの設定
		Me.SetParametersFromHt()

		Return MyBase.ExecGenerateSQL(sqlUtil)
	End Function

	#End Region

	#End Region
End Class
