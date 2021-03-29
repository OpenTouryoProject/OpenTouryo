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
'*  2019/05/14  西野  大介        クエリ再利用の性能向上対策コードの追加
'*  2020/06/20  西野  大介        型指定等が可能になるように修正を行った。
'**********************************************************************************

#Region "using"

' System～
Imports System
Imports System.IO
Imports System.Data
Imports System.Collections
Imports System.Collections.Concurrent

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
    ''' <summary>クエリのキャッシュ</summary>
    Protected Shared CDicQueryCache As New ConcurrentDictionary(Of String, String)()

    #Region "インスタンス変数"

    ''' <summary>キャッシュID</summary>
    Protected CacheId As String = ""

    #Region "パラメタ"
    ''' <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
    Protected HtUserParameter As New Hashtable()
    ''' <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
    Protected HtParameter As New Hashtable()
    #End Region

    #Region "CommandTimeout"

    ''' <summary>CommandTimeout</summary>
    Private _commandTimeout As Integer = -1

    #Region "プロパティ プロシージャ"

    ''' <summary>CommandTimeout</summary>
    ''' <remarks>自由に（拡張して）利用できる。</remarks>
    Public WriteOnly Property CommandTimeout() As Integer
        Set
            Me._commandTimeout = value
        End Set
    End Property

    #End Region

    #End Region

    #End Region

    #Region "コンストラクタ"

    ''' <summary>コンストラクタ</summary>
    ''' <param name="dam">BaseDam</param>
    Public Sub New(dam As BaseDam)
        MyBase.New(dam)
    End Sub

    ''' <summary>コンストラクタ</summary>
    ''' <param name="dam">BaseDam</param>
    ''' <param name="cacheId">キャッシュ用ID</param>
    Public Sub New(dam As BaseDam, cacheId As String)
        MyBase.New(dam)
        Me.CacheId = cacheId
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
			Dim obj As Object = Me.HtParameter(paramName)

			If TypeOf obj Is DaoParam Then
				Dim dp As DaoParam = DirectCast(obj, DaoParam)
				Me.SetParameter(paramName, dp.Value, dp.DbType, dp.Size, dp.Direction)
			Else
				Me.SetParameter(paramName, obj)
			End If
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

    #Region "クエリのキャッシュ処理"

    ''' <summary>ファイル or キャッシュからSetSqlByFile2する。</summary>
    ''' <param name="sqlFileName">string</param>
    Protected Sub SetSqlFromFileOrCache(sqlFileName As String)
        If String.IsNullOrEmpty(Me.CacheId) Then
            ' キャッシュ設定なし
            ' ファイルからSQL（Insert）を設定する。
            Me.SetSqlByFile2(sqlFileName)
        Else
            ' キャッシュ設定あり
            Dim temp As String = Me.CacheId & sqlFileName
            If _DaoClassName_.CDicQueryCache.ContainsKey(temp) Then
                ' キャッシュからSQL（Insert）を設定する。
                Me.SetSqlByCommand(_DaoClassName_.CDicQueryCache(temp))
            Else
                ' ファイルからSQL（Insert）を設定する。
                Me.SetSqlByFile2(sqlFileName)
            End If
        End If
    End Sub

    ''' <summary>クエリをキャッシュする。</summary>
    ''' <param name="sqlFileName">string</param>
    Protected Sub SetSqlToCache(sqlFileName As String)
        Dim temp As String = Me.CacheId & sqlFileName

        If Not String.IsNullOrEmpty(Me.CacheId) AndAlso Not _DaoClassName_.CDicQueryCache.ContainsKey(temp) Then
            ' クエリをキャッシュ
            _DaoClassName_.CDicQueryCache(temp) = Me.GetDam().DamIDbCommand.CommandText
        End If
    End Sub

    #End Region

    #Region "Insert"

    ''' <summary>１レコード挿入する。</summary>
    ''' <returns>挿入された行の数</returns>
    Public Function _InsertMethodName_() As Integer
        Dim sqlFileName As String = "_InsertFileName_"

        ' SQL（Insert）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（Insert）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    ''' <summary>１レコード挿入する。</summary>
    ''' <returns>挿入された行の数</returns>
    ''' <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
    Public Function _DynInsMethodName_() As Integer
        Dim sqlFileName As String = "_DynInsFileName_"

        ' ファイルからSQL（DynIns）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（DynIns）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    #End Region

    #Region "Select"

    ''' <summary>主キーを指定し、１レコード参照する。</summary>
    ''' <param name="dt">結果を格納するDataTable</param>
    Public Sub _SelectMethodName_(dt As DataTable)
        Dim sqlFileName As String = "_SelectFileName_"

        ' ファイルからSQL（Select）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（Select）を実行し、戻り値を戻す。
        Me.ExecSelectFill_DT(dt)
        Me.SetSqlToCache(sqlFileName)
    End Sub

    ''' <summary>検索条件を指定し、結果セットを参照する。</summary>
    ''' <param name="dt">結果を格納するDataTable</param>
    Public Sub _DynSelMethodName_(dt As DataTable)
        Dim sqlFileName As String = "_DynSelFileName_"

        ' ファイルからSQL（DynSel）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（DynSel）を実行し、戻り値を戻す。
        Me.ExecSelectFill_DT(dt)
        Me.SetSqlToCache(sqlFileName)
    End Sub

    #End Region

    #Region "Update"

    ''' <summary>主キーを指定し、１レコード更新する。</summary>
    ''' <returns>更新された行の数</returns>
    ''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
    Public Function _UpdateMethodName_() As Integer
        Dim sqlFileName As String = "_UpdateFileName_"

        ' ファイルからSQL（Update）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（Update）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    ''' <summary>任意の検索条件でデータを更新する。</summary>
    ''' <returns>更新された行の数</returns>
    ''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
    Public Function _DynUpdMethodName_() As Integer
        Dim sqlFileName As String = "_DynUpdFileName_"

        ' ファイルからSQL（DynUpd）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（DynUpd）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    #End Region

    #Region "Delete"

    ''' <summary>主キーを指定し、１レコード削除する。</summary>
    ''' <returns>削除された行の数</returns>
    Public Function _DeleteMethodName_() As Integer
        Dim sqlFileName As String = "_DeleteFileName_"

        ' ファイルからSQL（Delete）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（Delete）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    ''' <summary>任意の検索条件でデータを削除する。</summary>
    ''' <returns>削除された行の数</returns>
    Public Function _DynDelMethodName_() As Integer
        Dim sqlFileName As String = "_DynDelFileName_"

        ' ファイルからSQL（DynDel）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（DynDel）を実行し、戻り値を戻す。
        Dim rowCount As Integer = Me.ExecInsUpDel_NonQuery()
        Me.SetSqlToCache(sqlFileName)
        Return rowCount
    End Function

    #End Region

    #Region "拡張メソッド"

    ''' <summary>テーブルのレコード件数を取得する</summary>
    ''' <returns>テーブルのレコード件数</returns>
    Public Function _DynSelCntMethodName_() As Object
        Dim sqlFileName As String = "_DynSelCntFileName_"

        ' ファイルからSQL（DynSelCnt）を設定する。
        Me.SetSqlFromFileOrCache(sqlFileName)

        ' Set CommandTimeout
        Me.SetCommandTimeout()

        ' パラメタの設定
        Me.SetParametersFromHt()

        ' SQL（SELECT COUNT）を実行し、戻り値を戻す。
        Dim scalar As Object = Me.ExecSelectScalar()
        Me.SetSqlToCache(sqlFileName)
        Return scalar
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
