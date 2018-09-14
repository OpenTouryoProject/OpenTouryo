'**********************************************************************************
'* フレームワーク・テストクラス（Ｄ層）
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

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
'**********************************************************************************

Imports MVC_Sample.Logic.Common
Imports MVC_Sample.Models.ViewModels

Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Public.Dto
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

Namespace Logic.Dao
    Public Class LayerD
        Inherits MyBaseDao
        ''' <summary>
        ''' コンストラクタ
        ''' </summary>
        Public Sub New(dam As BaseDam)
            MyBase.New(dam)
        End Sub

#Region "テンプレ"

        ''' <summary>テンプレ</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub テンプレ(testParameter As TestParameterValue, testReturn As TestReturnValue)

            ' ↓DBアクセス-----------------------------------------------------

            ' ● 下記のいづれかの方法でSQLを設定する。

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2("ファイル名")

            '   -- 直接指定する場合。
            Me.SetSqlByCommand("SQL文")

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P1", testParameter.Shipper.ShipperID)

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
        Public Sub SelectCount(testParameter As TestParameterValue, testReturn As TestReturnValue)
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
        Public Sub SelectAll_DT(testParameter As TestParameterValue, testReturn As TestReturnValue)
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

			' DataTableToList
			Dim list As List(Of ShipperViweModel) = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
			testReturn.Obj = list
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する（DS）</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DS(testParameter As TestParameterValue, testReturn As TestReturnValue)
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

			' DataTableToList
			Dim list As List(Of ShipperViweModel) = DataToPoco.DataTableToList(Of ShipperViweModel)(ds.Tables(0))

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
			testReturn.Obj = list
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する（DR）</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DR(testParameter As TestParameterValue, testReturn As TestReturnValue)
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

            '   -- 一覧を返すSELECTクエリを実行する
            Dim idr As IDataReader = DirectCast(Me.ExecSelect_DR(), IDataReader)

			' DataReaderToList
			Dim list As List(Of ShipperViweModel) = DataToPoco.DataReaderToList(Of ShipperViweModel)(idr)

            ' 終了したらクローズ
            idr.Close()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
			testReturn.Obj = list
        End Sub

        ''' <summary>一覧を返すSELECTクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub SelectAll_DSQL(testParameter As TestParameterValue, testReturn As TestReturnValue)
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
			' DataTableToList
			Dim list As List(Of ShipperViweModel) = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
			testReturn.Obj = list
        End Sub

#End Region

#Region "参照"

        ''' <summary>１レコードを返すSELECTクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub [Select](testParameter As TestParameterValue, testReturn As TestReturnValue)
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
            Me.SetParameter("P1", testParameter.Shipper.ShipperID)

            ' 戻り値 dt
            Dim dt As New DataTable()

            '   -- １レコードを返すSELECTクエリを実行する
            Me.ExecSelectFill_DT(dt)

            ' ↑DBアクセス-----------------------------------------------------

			' 一部、DataToPocoのテストコード
			Dim svm As ShipperViweModel = DataToPoco.DataTableToPOCO(Of ShipperViweModel)(dt)
			Debug.WriteLine("svm:" & ObjectInspector.Inspect(svm))

			' mapの書き方は、Key-Valueでdst-srcのproperty field名を書く
			Dim tsvm As TestShipperViweModel = DataToPoco.DataTableToPOCO(Of TestShipperViweModel)(dt, New Dictionary(Of String, String)() From { _
				{"_ShipperID", "ShipperID"}, _
				{"_CompanyName", "CompanyName"}, _
				{"_Phone", "Phone"} _
			})

			Debug.WriteLine("tsvm:" & ObjectInspector.Inspect(tsvm))

			testReturn.Obj = svm
			testReturn.Obj2 = tsvm
        End Sub

#End Region

#End Region

#Region "更新系"

#Region "追加"

        ''' <summary>Insertクエリを実行する</summary>
        ''' <param name="testParameter">引数クラス</param>
        ''' <param name="testReturn">戻り値クラス</param>
        Public Sub Insert(testParameter As TestParameterValue, testReturn As TestReturnValue)
            ' ↓DBアクセス-----------------------------------------------------

            '   -- ファイルから読み込む場合。
            Me.SetSqlByFile2("ShipperInsert.sql")

            ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            Me.SetParameter("P2", testParameter.Shipper.CompanyName)
            Me.SetParameter("P3", testParameter.Shipper.Phone)

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
        Public Sub Update(testParameter As TestParameterValue, testReturn As TestReturnValue)

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
            Me.SetParameter("P1", testParameter.Shipper.ShipperID)
            Me.SetParameter("P2", testParameter.Shipper.CompanyName)
            Me.SetParameter("P3", testParameter.Shipper.Phone)

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
        Public Sub Delete(testParameter As TestParameterValue, testReturn As TestReturnValue)
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
            Me.SetParameter("P1", testParameter.Shipper.ShipperID)

            Dim obj As Object

            '   -- 削除（件数を確認できる）
            obj = Me.ExecInsUpDel_NonQuery()

            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            testReturn.Obj = obj
        End Sub

#End Region

#End Region
    End Class
End Namespace
