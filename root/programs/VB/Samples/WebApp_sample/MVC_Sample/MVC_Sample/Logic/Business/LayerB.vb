'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：LayerB
'* クラス日本語名  ：Ｂ層のテスト
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports MVC_Sample.Logic.Dao
Imports MVC_Sample.Logic.Common
Imports MVC_Sample.Models.ViewModels

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Public.Dto

Namespace Logic.Business
    Public Class LayerB
        Inherits MyFcBaseLogic
#Region "テンプレ"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_メソッド名(testParameter As TestParameterValue)
            'メソッド引数にBaseParameterValueの派生の型を定義可能。
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------

            ' 個別Dao
            Dim myDao As New LayerD(Me.GetDam())
            'myDao.xxxx(testParameter, ref testReturn);

            ' 共通Dao
            Dim cmnDao As New CmnDao(Me.GetDam())
            cmnDao.ExecSelectScalar()

            ' ↑業務処理-----------------------------------------------------
        End Sub

#End Region

#Region "UOCメソッド"

#Region "SelectCount"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_SelectCount(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperCount.sql"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperCount.xml"
                            Exit Select
                    End Select

                    ' 共通Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = cmnDao.ExecSelectScalar()

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' 共通Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = genDao.D5_SelCnt()

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.SelectCount(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "SelectAll_DT"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_SelectAll_DT(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------
            Dim dt As DataTable = Nothing
            Dim list As List(Of ShipperViweModel) = Nothing

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
                            Exit Select
                    End Select

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt)

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 自動生成Daoを実行
                    genDao.D2_Select(dt)

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.SelectAll_DT(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "SelectAll_DS"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_SelectAll_DS(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------
            Dim ds As DataSet = Nothing
            Dim list As List(Of ShipperViweModel) = Nothing

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
                            Exit Select
                    End Select

                    ' 戻り値 ds
                    ds = New DataSet()

                    ' 共通Daoを実行
                    cmnDao.ExecSelectFill_DS(ds)

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(ds.Tables(0))

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' 戻り値 ds
                    ds = New DataSet()
                    ds.Tables.Add(New DataTable())

                    ' 自動生成Daoを実行
                    genDao.D2_Select(ds.Tables(0))

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(ds.Tables(0))

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.SelectAll_DS(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "SelectAll_DR"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_SelectAll_DR(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------
            Dim dt As DataTable = Nothing
            Dim list As List(Of ShipperViweModel) = Nothing

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLText = "SELECT * FROM Shippers"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLText = "<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT>SELECT * FROM Shippers</ROOT>"
                            Exit Select
                    End Select

                    ' 共通Daoを実行
                    Dim idr As IDataReader = cmnDao.ExecSelect_DR()

                    ' DataReaderToList
                    list = DataToPoco.DataReaderToList(Of ShipperViweModel)(idr)

                    ' 終了したらクローズ
                    idr.Close()

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' DRのI/Fなし

                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 自動生成Daoを実行
                    genDao.D2_Select(dt)

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

                    ' 戻り値を設定
                    testReturn.Obj = list

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.SelectAll_DR(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "SelectAll_DSQL"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_SelectAll_DSQL(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------
            Dim dt As DataTable = Nothing
            Dim list As List(Of ShipperViweModel) = Nothing

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelectOrder.sql"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelectOrder.xml"
                            Exit Select
                    End Select

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
                    cmnDao.SetParameter("P1", "test")

                    ' ユーザ入力は指定しない。
                    ' ※ 動的SQLのVALタグは、前後の空白をつめることが有るので、
                    '    必要であれば、前後の空白を明示的に指定する必要がある。
                    cmnDao.SetUserParameter("COLUMN", " " & orderColumn & " ")
                    cmnDao.SetUserParameter("SEQUENCE", " " & orderSequence & " ")

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt)

                    ' DataTableToList
                    list = DataToPoco.DataTableToList(Of ShipperViweModel)(dt)

                    ' 自動生成Daoを実行
                    testReturn.Obj = list

                    Exit Select
                Case Else

                    'case "generate": // 自動生成Daoを使用する。
                    '    // 当該SQLなし
                    '    break;

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.SelectAll_DSQL(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "Select"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_Select(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------
            Dim dt As DataTable = Nothing

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelect.sql"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperSelect.xml"
                            Exit Select
                    End Select

                    ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID)

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 共通Daoを実行
                    cmnDao.ExecSelectFill_DT(dt)

                    ' DataTableToPOCO
                    testReturn.Obj = DataToPoco.DataTableToPOCO(Of ShipperViweModel)(dt)

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID

                    ' 戻り値 dt
                    dt = New DataTable()

                    ' 自動生成Daoを実行
                    genDao.S2_Select(dt)

                    ' DataTableToPOCO
                    testReturn.Obj = DataToPoco.DataTableToPOCO(Of ShipperViweModel)(dt)

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.[Select](testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "Insert"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_Insert(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    cmnDao.SQLFileName = "ShipperInsert.sql"

                    ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P2", testParameter.Shipper.CompanyName)
                    cmnDao.SetParameter("P3", testParameter.Shipper.Phone)

                    ' 共通Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery()

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' パラメタに対して、動的に値を設定する。
                    genDao.CompanyName = testParameter.Shipper.CompanyName
                    genDao.Phone = testParameter.Shipper.Phone

                    ' 自動生成Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = genDao.D1_Insert()

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.Insert(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "Update"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_Update(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperUpdate.sql"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperUpdate.xml"
                            Exit Select
                    End Select

                    ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID)
                    cmnDao.SetParameter("P2", testParameter.Shipper.CompanyName)
                    cmnDao.SetParameter("P3", testParameter.Shipper.Phone)

                    ' 共通Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery()

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID
                    genDao.Set_CompanyName_forUPD = testParameter.Shipper.CompanyName
                    genDao.Set_Phone_forUPD = testParameter.Shipper.Phone

                    ' 自動生成Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = genDao.S3_Update()

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.Update(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#Region "Delete"

        ''' <summary>業務処理を実装</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub UOC_Delete(testParameter As TestParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim testReturn As New TestReturnValue()
            Me.ReturnValue = testReturn

            ' ↓業務処理-----------------------------------------------------

            Select Case (testParameter.ActionType.Split("%"c))(1)
                Case "common"
                    ' 共通Daoを使用する。
                    ' 共通Daoを生成
                    Dim cmnDao As New CmnDao(Me.GetDam())

                    Select Case (testParameter.ActionType.Split("%"c))(2)
                        Case "static"
                            ' 静的SQLを指定
                            cmnDao.SQLFileName = "ShipperDelete.sql"
                            Exit Select

                        Case "dynamic"
                            ' 動的SQLを指定
                            cmnDao.SQLFileName = "ShipperDelete.xml"
                            Exit Select
                    End Select

                    ' パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
                    cmnDao.SetParameter("P1", testParameter.Shipper.ShipperID)

                    ' 共通Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = cmnDao.ExecInsUpDel_NonQuery()

                    Exit Select

                Case "generate"
                    ' 自動生成Daoを使用する。
                    ' 自動生成Daoを生成
                    Dim genDao As New DaoShippers(Me.GetDam())

                    ' パラメタに対して、動的に値を設定する。
                    genDao.PK_ShipperID = testParameter.Shipper.ShipperID

                    ' 自動生成Daoを実行
                    ' 戻り値を設定
                    testReturn.Obj = genDao.S4_Delete()

                    Exit Select
                Case Else

                    ' 個別Daoを使用する。
                    Dim myDao As New LayerD(Me.GetDam())
                    myDao.Delete(testParameter, testReturn)
                    Exit Select
            End Select

            ' ↑業務処理-----------------------------------------------------

            ' ロールバックのテスト
            Me.TestRollback(testParameter)
        End Sub

#End Region

#End Region

#Region "ロールバックのテスト"

        ''' <summary>ロールバックのテスト</summary>
        ''' <param name="testParameter">引数クラス</param>
        Private Sub TestRollback(testParameter As TestParameterValue)
            Select Case (testParameter.ActionType.Split("%"c))(3)

                Case "Business"

                    ' 戻り値が見えるか確認する。
                    DirectCast(Me.ReturnValue, TestReturnValue).Obj = "戻り値が戻るか？"

                    ' 業務例外のスロー
                    Throw New BusinessApplicationException("ロールバックのテスト", "ロールバックのテスト", "エラー情報")
                'break; // 到達できないためコメントアウト

                Case "System"

                    ' 戻り値が見えるか確認する。
                    DirectCast(Me.ReturnValue, TestReturnValue).Obj = "戻り値が戻るか？"

                    ' システム例外のスロー
                    Throw New BusinessSystemException("ロールバックのテスト", "ロールバックのテスト")
                'break; // 到達できないためコメントアウト

                Case "Other"

                    ' 戻り値が見えるか確認する。
                    DirectCast(Me.ReturnValue, TestReturnValue).Obj = "戻り値が戻るか？"

                    ' その他、一般的な例外のスロー
                    Throw New Exception("ロールバックのテスト")
                'break; // 到達できないためコメントアウト

                Case "Other-Business"
                    ' 戻り値が見えるか確認する。
                    DirectCast(Me.ReturnValue, TestReturnValue).Obj = "戻り値が戻るか？"

                    ' その他、一般的な例外（業務例外へ振り替え）のスロー
                    Throw New Exception("Other-Business")
                'break; // 到達できないためコメントアウト

                Case "Other-System"

                    ' 戻り値が見えるか確認する。
                    DirectCast(Me.ReturnValue, TestReturnValue).Obj = "戻り値が戻るか？"

                    ' その他、一般的な例外（システム例外へ振り替え）のスロー
                    Throw New Exception("Other-System")
                    'break; // 到達できないためコメントアウト
            End Select
        End Sub

#End Region
    End Class
End Namespace
