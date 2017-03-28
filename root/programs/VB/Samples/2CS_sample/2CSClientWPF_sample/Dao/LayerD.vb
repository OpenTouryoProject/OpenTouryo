'**********************************************************************************
'* フレームワーク・テストクラス（Ｄ層）
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：LayerD
'* クラス日本語名  ：Ｄ層のテスト
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Imports _2CSClientWPF_sample.Common

Imports System.Data

Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Public.Db

Namespace Dao
    ''' <summary>
    ''' LayerD の概要の説明です
    ''' </summary>
    Public Class LayerD
        Inherits MyBaseDao
        ''' <summary>
        ''' コンストラクタ
        ''' </summary>
        Public Sub New(ByVal dam As BaseDam)
            MyBase.New(dam)
        End Sub

#Region "テンプレ"

        ''' <summary>テンプレ</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub テンプレ(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)

            ' ↓DBアクセス-----------------------------------------------------

            ' ● 下記のいづれかの方法でSQLを設定する。

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2("ファイル名")

            '   -- 直接指定する場合。
            Me.SetSqlByCommand("SQL文")

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", testParameter.ShipperID)

            Dim obj As Object

            '   -- 追加、更新、削除の場合（件数を確認できる）
            obj = Me.ExecInsUpDel_NonQuery()

            '   -- 先頭の１セル分の情報を返すSELECTクエリを実行する場合
            obj = Me.ExecSelectScalar()

            '   -- テーブル（or レコード）の情報を返す
            '      SELECTクエリを実行する場合（引数 = データテーブル）
            obj = New DataTable()
            Me.ExecSelectFill_DT(DirectCast(obj, DataTable))

            '   -- テーブル（or レコード）の情報を返す
            '      SELECTクエリを実行する場合（引数 = データセット）
            obj = New DataSet()
            Me.ExecSelectFill_DS(DirectCast(obj, DataSet))

            '   -- データリーダを返す
            Dim idr As IDataReader = DirectCast(Me.ExecSelect_DR(), IDataReader)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#Region "参照系"

#Region "件数取得（SelectCount）"

        ''' <summary>件数情報を返すSELECTクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectCount(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim filename As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                filename = "ShipperCount.sql"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                filename = "ShipperCount.xml"
            End If

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2(filename)

            Dim obj As Object

            '   -- 件数情報を返すSELECTクエリを実行する
            obj = Me.ExecSelectScalar()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#Region "一覧取得（SelectAll）"

        ''' <summary>一覧を返すSELECTクエリを実行する（DT）</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DT(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim commandText As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                commandText = "SELECT * FROM Shippers"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                ' 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
                commandText = "<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
            End If

            '   -- 直接指定する場合。
            Me.SetSqlByCommand(commandText)

            ' 戻り値 dt
            Dim dt As New DataTable()

            '   -- 一覧を返すSELECTクエリを実行する
            Me.ExecSelectFill_DT(dt)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = dt
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する（DS）</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DS(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim commandText As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                commandText = "SELECT * FROM Shippers"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                ' 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
                commandText = "<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
            End If

            '   -- 直接指定する場合。
            Me.SetSqlByCommand(commandText)

            ' 戻り値 ds
            Dim ds As New DataSet()

            '   -- 一覧を返すSELECTクエリを実行する
            Me.ExecSelectFill_DS(ds)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = ds
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する（DR）</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DR(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim commandText As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                commandText = "SELECT * FROM Shippers"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                ' 通常、動的SQLをSetSqlByCommandで直接指定するような使い方はしない。
                commandText = "<?xml version=""1.0"" encoding=""shift_jis"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
            End If

            '   -- 直接指定する場合。
            Me.SetSqlByCommand(commandText)

            ' 戻り値 dt
            Dim dt As New DataTable()

            ' ３列生成
            dt.Columns.Add("c1", GetType(String))
            dt.Columns.Add("c2", GetType(String))
            dt.Columns.Add("c3", GetType(String))

            '   -- 一覧を返すSELECTクエリを実行する
            Dim idr As IDataReader = DirectCast(Me.ExecSelect_DR(), IDataReader)

            While idr.Read()
                ' DRから読む
                Dim objArray As Object() = New Object(2) {}
                idr.GetValues(objArray)

                ' DTに設定する。
                Dim dr As DataRow = dt.NewRow()
                dr.ItemArray = objArray
                dt.Rows.Add(dr)
            End While

            ' 終了したらクローズ
            idr.Close()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = dt
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DSQL(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim filename As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                filename = "ShipperSelectOrder.sql"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                filename = "ShipperSelectOrder.xml"
            End If

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2(filename)

            ' ユーザ定義パラメタに対して、動的に値を設定する。
            Dim orderColumn As String = ""
            Dim orderSequence As String = ""

            If testParameter.OrderColumn = "c1" Then
                orderColumn = "ShipperID"
            ElseIf testParameter.OrderColumn = "c2" Then
                orderColumn = "CompanyName"
            ElseIf testParameter.OrderColumn = "c3" Then
                orderColumn = "Phone"
            Else
            End If

            If testParameter.OrderSequence = "A" Then
                orderSequence = "ASC"
            ElseIf testParameter.OrderSequence = "D" Then
                orderSequence = "DESC"
            Else
            End If

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", "test")

            ' ユーザ入力は指定しない。
            ' ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
            '    必要であれば、前後の空白を明示的に指定する必要がある。
            Me.SetUserParameter("COLUMN", " " & orderColumn & " ")
            Me.SetUserParameter("SEQUENCE", " " & orderSequence & " ")

            ' 戻り値 dt
            Dim dt As New DataTable()

            '   -- 一覧を返すSELECTクエリを実行する
            Me.ExecSelectFill_DT(dt)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = dt
        End Sub

#End Region

#Region "参照"

        ''' <summary>１レコードを返すSELECTクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub [Select](ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim filename As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                filename = "ShipperSelect.sql"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                filename = "ShipperSelect.xml"
            End If

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2(filename)

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", testParameter.ShipperID)

            ' 戻り値 dt
            Dim dt As New DataTable()

            '   -- １レコードを返すSELECTクエリを実行する
            Me.ExecSelectFill_DT(dt)

            ' ↑DBアクセス-----------------------------------------------------

            '/ 戻り値を設定 // 不要
            'testReturn.Obj = dt;

            ' キャストの対策コードを挿入

            ' ・SQLの場合、ShipperIDのintがInt32型にマップされる。
            ' ・ODPの場合、ShipperIDのNUMBERがInt64型にマップされる。
            ' ・DB2の場合、ShipperIDのDECIMALがｘｘｘ型にマップされる。
            If dt.Rows(0).ItemArray.GetValue(0).[GetType]().ToString() = "System.Int32" Then
                ' Int32なのでキャスト
                testReturn.ShipperID = CInt(dt.Rows(0).ItemArray.GetValue(0))
            Else
                ' それ以外の場合、一度、文字列に変換してInt32.Parseする。
                testReturn.ShipperID = Integer.Parse(dt.Rows(0).ItemArray.GetValue(0).ToString())
            End If

            testReturn.CompanyName = DirectCast(dt.Rows(0).ItemArray.GetValue(1), String)
            testReturn.Phone = DirectCast(dt.Rows(0).ItemArray.GetValue(2), String)
        End Sub

#End Region

#End Region

#Region "更新系"

#Region "追加"

        ''' <summary>Insertクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub Insert(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2("ShipperInsert.sql")

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P2", testParameter.CompanyName)
            Me.SetParameter("P3", testParameter.Phone)

            Dim obj As Object

            '   -- 追加（件数を確認できる）
            obj = Me.ExecInsUpDel_NonQuery()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#Region "更新"

        ''' <summary>Updateクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub Update(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)

            ' ↓DBアクセス-----------------------------------------------------

            Dim filename As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                filename = "ShipperUpdate.sql"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                filename = "ShipperUpdate.xml"
            End If

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2(filename)

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", testParameter.ShipperID)
            Me.SetParameter("P2", testParameter.CompanyName)
            Me.SetParameter("P3", testParameter.Phone)

            Dim obj As Object

            '   -- 更新（件数を確認できる）
            obj = Me.ExecInsUpDel_NonQuery()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#Region "削除"

        ''' <summary>Deleteクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub Delete(ByVal testParameter As TestParameterValue, ByVal testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            Dim filename As String = ""

            If (testParameter.ActionType.Split("%"c))(2) = "static" Then
                ' 静的SQL
                filename = "ShipperDelete.sql"
            ElseIf (testParameter.ActionType.Split("%"c))(2) = "dynamic" Then
                ' 動的SQL
                filename = "ShipperDelete.xml"
            End If

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2(filename)

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", testParameter.ShipperID)

            Dim obj As Object

            '   -- 追削除（件数を確認できる）
            obj = Me.ExecInsUpDel_NonQuery()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#End Region
    End Class
End Namespace
