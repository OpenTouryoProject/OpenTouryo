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
'* クラス名        ：DaoT_Workflow
'* クラス日本語名  ：自動生成Ｄａｏクラス
'*
'* 作成日時        ：2014/7/18
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2012/06/14  西野 大介         ResourceLoaderに加え、EmbeddedResourceLoaderに対応
'*  2013/09/09  西野 大介         ExecGenerateSQLメソッドを追加した（バッチ更新用）。
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Public.Db

Namespace Touryo.Infrastructure.Business.Workflow

    ''' <summary>自動生成Ｄａｏクラス</summary>
    Public Class DaoT_Workflow
        Inherits MyBaseDao
#Region "インスタンス変数"

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


        ''' <summary>WorkflowControlNo列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property PK_WorkflowControlNo() As Object
            Get
                Return Me.HtParameter("WorkflowControlNo")
            End Get
            Set
                Me.HtParameter("WorkflowControlNo") = value
            End Set
        End Property



        ''' <summary>SubSystemId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property SubSystemId() As Object
            Get
                Return Me.HtParameter("SubSystemId")
            End Get
            Set
                Me.HtParameter("SubSystemId") = value
            End Set
        End Property

        ''' <summary>WorkflowName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property WorkflowName() As Object
            Get
                Return Me.HtParameter("WorkflowName")
            End Get
            Set
                Me.HtParameter("WorkflowName") = value
            End Set
        End Property

        ''' <summary>UserId列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property UserId() As Object
            Get
                Return Me.HtParameter("UserId")
            End Get
            Set
                Me.HtParameter("UserId") = value
            End Set
        End Property

        ''' <summary>UserInfo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property UserInfo() As Object
            Get
                Return Me.HtParameter("UserInfo")
            End Get
            Set
                Me.HtParameter("UserInfo") = value
            End Set
        End Property

        ''' <summary>ReserveArea列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property ReserveArea() As Object
            Get
                Return Me.HtParameter("ReserveArea")
            End Get
            Set
                Me.HtParameter("ReserveArea") = value
            End Set
        End Property

        ''' <summary>StartDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property StartDate() As Object
            Get
                Return Me.HtParameter("StartDate")
            End Get
            Set
                Me.HtParameter("StartDate") = value
            End Set
        End Property

        ''' <summary>EndDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
        Public Property EndDate() As Object
            Get
                Return Me.HtParameter("EndDate")
            End Get
            Set
                Me.HtParameter("EndDate") = value
            End Set
        End Property


        ''' <summary>Set_WorkflowControlNo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_WorkflowControlNo_forUPD() As Object
            Get
                Return Me.HtParameter("Set_WorkflowControlNo_forUPD")
            End Get
            Set
                Me.HtParameter("Set_WorkflowControlNo_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_SubSystemId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_SubSystemId_forUPD() As Object
            Get
                Return Me.HtParameter("Set_SubSystemId_forUPD")
            End Get
            Set
                Me.HtParameter("Set_SubSystemId_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_WorkflowName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_WorkflowName_forUPD() As Object
            Get
                Return Me.HtParameter("Set_WorkflowName_forUPD")
            End Get
            Set
                Me.HtParameter("Set_WorkflowName_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_UserId_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_UserId_forUPD() As Object
            Get
                Return Me.HtParameter("Set_UserId_forUPD")
            End Get
            Set
                Me.HtParameter("Set_UserId_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_UserInfo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_UserInfo_forUPD() As Object
            Get
                Return Me.HtParameter("Set_UserInfo_forUPD")
            End Get
            Set
                Me.HtParameter("Set_UserInfo_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_ReserveArea_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_ReserveArea_forUPD() As Object
            Get
                Return Me.HtParameter("Set_ReserveArea_forUPD")
            End Get
            Set
                Me.HtParameter("Set_ReserveArea_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_StartDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_StartDate_forUPD() As Object
            Get
                Return Me.HtParameter("Set_StartDate_forUPD")
            End Get
            Set
                Me.HtParameter("Set_StartDate_forUPD") = value
            End Set
        End Property


        ''' <summary>Set_EndDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
        Public Property Set_EndDate_forUPD() As Object
            Get
                Return Me.HtParameter("Set_EndDate_forUPD")
            End Get
            Set
                Me.HtParameter("Set_EndDate_forUPD") = value
            End Set
        End Property



        ''' <summary>WorkflowControlNo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property WorkflowControlNo_Like() As Object
            Get
                Return Me.HtParameter("WorkflowControlNo_Like")
            End Get
            Set
                Me.HtParameter("WorkflowControlNo_Like") = value
            End Set
        End Property


        ''' <summary>SubSystemId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property SubSystemId_Like() As Object
            Get
                Return Me.HtParameter("SubSystemId_Like")
            End Get
            Set
                Me.HtParameter("SubSystemId_Like") = value
            End Set
        End Property


        ''' <summary>WorkflowName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property WorkflowName_Like() As Object
            Get
                Return Me.HtParameter("WorkflowName_Like")
            End Get
            Set
                Me.HtParameter("WorkflowName_Like") = value
            End Set
        End Property


        ''' <summary>UserId_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property UserId_Like() As Object
            Get
                Return Me.HtParameter("UserId_Like")
            End Get
            Set
                Me.HtParameter("UserId_Like") = value
            End Set
        End Property


        ''' <summary>UserInfo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property UserInfo_Like() As Object
            Get
                Return Me.HtParameter("UserInfo_Like")
            End Get
            Set
                Me.HtParameter("UserInfo_Like") = value
            End Set
        End Property


        ''' <summary>ReserveArea_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property ReserveArea_Like() As Object
            Get
                Return Me.HtParameter("ReserveArea_Like")
            End Get
            Set
                Me.HtParameter("ReserveArea_Like") = value
            End Set
        End Property


        ''' <summary>StartDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property StartDate_Like() As Object
            Get
                Return Me.HtParameter("StartDate_Like")
            End Get
            Set
                Me.HtParameter("StartDate_Like") = value
            End Set
        End Property


        ''' <summary>EndDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
        ''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
        Public Property EndDate_Like() As Object
            Get
                Return Me.HtParameter("EndDate_Like")
            End Get
            Set
                Me.HtParameter("EndDate_Like") = value
            End Set
        End Property


#End Region

#Region "クエリ メソッド"

#Region "Insert"

        ''' <summary>１レコード挿入する。</summary>
        ''' <returns>挿入された行の数</returns>
        Public Function S1_Insert() As Integer
            ' ファイルからSQL（Insert）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_S1_Insert.sql")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（Insert）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

        ''' <summary>１レコード挿入する。</summary>
        ''' <returns>挿入された行の数</returns>
        ''' <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
        Public Function D1_Insert() As Integer
            ' ファイルからSQL（DynIns）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_D1_Insert.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（DynIns）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

#End Region

#Region "Select"

        ''' <summary>主キーを指定し、１レコード参照する。</summary>
        ''' <param name="dt">結果を格納するDataTable</param>
        Public Sub S2_Select(dt As DataTable)
            ' ファイルからSQL（Select）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_S2_Select.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（Select）を実行し、戻り値を戻す。
            Me.ExecSelectFill_DT(dt)
        End Sub

        ''' <summary>検索条件を指定し、結果セットを参照する。</summary>
        ''' <param name="dt">結果を格納するDataTable</param>
        Public Sub D2_Select(dt As DataTable)
            ' ファイルからSQL（DynSel）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_D2_Select.xml")

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
        Public Function S3_Update() As Integer
            ' ファイルからSQL（Update）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_S3_Update.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（Update）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

        ''' <summary>任意の検索条件でデータを更新する。</summary>
        ''' <returns>更新された行の数</returns>
        ''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
        Public Function D3_Update() As Integer
            ' ファイルからSQL（DynUpd）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_D3_Update.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（DynUpd）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

#End Region

#Region "Delete"

        ''' <summary>主キーを指定し、１レコード削除する。</summary>
        ''' <returns>削除された行の数</returns>
        Public Function S4_Delete() As Integer
            ' ファイルからSQL（Delete）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_S4_Delete.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（Delete）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

        ''' <summary>任意の検索条件でデータを削除する。</summary>
        ''' <returns>削除された行の数</returns>
        Public Function D4_Delete() As Integer
            ' ファイルからSQL（DynDel）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_D4_Delete.xml")

            ' パラメタの設定
            Me.SetParametersFromHt()

            ' SQL（DynDel）を実行し、戻り値を戻す。
            Return Me.ExecInsUpDel_NonQuery()
        End Function

#End Region

#Region "拡張メソッド"

        ''' <summary>テーブルのレコード件数を取得する</summary>
        ''' <returns>テーブルのレコード件数</returns>
        Public Function D5_SelCnt() As Object
            ' ファイルからSQL（DynSelCnt）を設定する。
            Me.SetSqlByFile2("DaoT_Workflow_D5_SelCnt.xml")

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

            ' パラメタの設定
            Me.SetParametersFromHt()

            Return MyBase.ExecGenerateSQL(sqlUtil)
        End Function

#End Region

#End Region
    End Class
End Namespace