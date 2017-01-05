'**********************************************************************************
'* サンプル アプリ・コントローラ
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：CrudMuController
'* クラス日本語名  ：Ajax.BeginForm用サンプル アプリ・コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/10/29  Sai               Modified the code of navigating to CrudMu2 controller
'*                                in the Transitions method.         
'**********************************************************************************

'System
Imports System
Imports System.Web
Imports System.Web.Mvc

Imports System.Data

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

Imports MVC_Sample.Logic.Business
Imports MVC_Sample.Logic.Common
Imports MVC_Sample.Models

Namespace Controllers
    ''' <summary>
    ''' Ajax.BeginForm用サンプル アプリ・コントローラ
    ''' </summary>
    Public Class CrudMuController
        Inherits MyBaseMVController
        '
        ' GET: /CrudMu/

        ''' <summary>
        ''' 画面の初期表示
        ''' </summary>
        ''' <returns>初期表示状態の画面 (ViewResult)</returns>
        <HttpGet> _
        Public Function Index() As ActionResult
            Dim model As New CrudModel()
            Return View(model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード数をカウントする
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <returns>Shippers テーブルのレコード数を表示するためのPartialViewResult</returns>
        Public Function GetCount(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button1", "SelectCount", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""
            Dim model As New CrudModel()

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo
            Else
                ' 結果（正常系）
                message = testReturnValue.Obj.ToString() & "件のデータがあります"
            End If

            ' メッセージを設定。
            model.Message = message

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataTable として取得する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function SelectAll_DT(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button2", "SelectAll_DT", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim scriptText As String = "$('#lblMessage').text('" & message & "');"
                Return JavaScript(scriptText)
            Else
                ' 結果（正常系）
                Dim model As New CrudModel()
                model.shippers = New DataSets.DsNorthwind.ShippersDataTable()
                Dim dt As DataTable = DirectCast(testReturnValue.Obj, DataTable)

                For Each row As DataRow In dt.Rows
                    Dim srow As DataSets.DsNorthwind.ShippersRow = model.shippers.NewShippersRow()
                    srow.ShipperID = Integer.Parse(row(0).ToString())
                    srow.CompanyName = row(1).ToString()
                    srow.Phone = row(2).ToString()

                    model.shippers.Rows.Add(srow)
                Next

                ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                Return PartialView("_ChartView", model)
            End If
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataSet として取得する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function SelectAll_DS(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button3", "SelectAll_DS", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim scriptText As String = "$('#lblMessage').text('" & message & "');"
                Return JavaScript(scriptText)
            Else
                ' 結果（正常系）
                Dim model As New CrudModel()
                model.shippers = New DataSets.DsNorthwind.ShippersDataTable()
                Dim ds As DataSet = DirectCast(testReturnValue.Obj, DataSet)

                For Each row As DataRow In ds.Tables(0).Rows
                    Dim srow As DataSets.DsNorthwind.ShippersRow = model.shippers.NewShippersRow()
                    srow.ShipperID = Integer.Parse(row(0).ToString())
                    srow.CompanyName = row(1).ToString()
                    srow.Phone = row(2).ToString()

                    model.shippers.Rows.Add(srow)
                Next

                ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                Return PartialView("_ChartView", model)
            End If
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataReader として取得する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function SelectAll_DR(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button4", "SelectAll_DR", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim scriptText As String = "$('#lblMessage').text('" & message & "');"
                Return JavaScript(scriptText)
            Else
                ' 結果（正常系）
                Dim model As New CrudModel()
                model.shippers = New DataSets.DsNorthwind.ShippersDataTable()
                Dim dt As DataTable = DirectCast(testReturnValue.Obj, DataTable)

                For Each row As DataRow In dt.Rows
                    Dim srow As DataSets.DsNorthwind.ShippersRow = model.shippers.NewShippersRow()
                    srow.ShipperID = Integer.Parse(row(0).ToString())
                    srow.CompanyName = row(1).ToString()
                    srow.Phone = row(2).ToString()

                    model.shippers.Rows.Add(srow)
                Next

                ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                Return PartialView("_ChartView", model)
            End If
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を、動的 SQL を使用して取得する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <param name="ddlOrderColumn">並び替え対象列</param>
        ''' <param name="ddlOrderSequence">昇順・降順</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function SelectAll_DSQL(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String, ddlOrderColumn As String, ddlOrderSequence As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button5", "SelectAll_DSQL", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 動的SQLの要素を設定
            testParameterValue.OrderColumn = ddlOrderColumn
            testParameterValue.OrderSequence = ddlOrderSequence

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim scriptText As String = "$('#lblMessage').text('" & message & "');"
                Return JavaScript(scriptText)
            Else
                ' 結果（正常系）
                Dim model As New CrudModel()
                model.shippers = New DataSets.DsNorthwind.ShippersDataTable()
                Dim dt As DataTable = DirectCast(testReturnValue.Obj, DataTable)

                For Each row As DataRow In dt.Rows
                    Dim srow As DataSets.DsNorthwind.ShippersRow = model.shippers.NewShippersRow()
                    srow.ShipperID = Integer.Parse(row(0).ToString())
                    srow.CompanyName = row(1).ToString()
                    srow.Phone = row(2).ToString()

                    model.shippers.Rows.Add(srow)
                Next

                ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                Return PartialView("_ChartView", model)
            End If
        End Function

        ''' <summary>
        ''' ShipperId をキーにして Shippers テーブルのレコードを取得する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <param name="textBox1">ShipperId</param>
        ''' <returns>Shippers テーブルのレコードを表示するJavaScriptResult</returns>
        Public Function [Select](ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String, textBox1 As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button6", "Select", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 動的SQLの要素を設定
            testParameterValue.ShipperID = Integer.Parse(textBox1)

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo

                Dim model As New CrudModel() With {.Message = message}

                ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                Return PartialView("_MessageView", model)
            Else
                ' 結果（正常系）

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim scriptText As String = _
                    String.Format("$('#textBox1').val('{0}');$('#textBox2').val('{1}');$('#textBox3').val('{2}');", _
                                  testReturnValue.ShipperID, _
                                  testReturnValue.CompanyName, _
                                  testReturnValue.Phone)
                Return JavaScript(scriptText)
            End If
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを追加する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <param name="textBox2">CompanyName</param>
        ''' <param name="textBox3">Phone</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function Insert(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String, textBox2 As String, textBox3 As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button7", "Insert", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 動的SQLの要素を設定
            testParameterValue.CompanyName = textBox2
            testParameterValue.Phone = textBox3

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo
            Else
                ' 結果（正常系）
                message = testReturnValue.Obj.ToString() & "件追加"
            End If

            Dim model As New CrudModel() With {.Message = message}

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを更新する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <param name="textBox1">ShipperId</param>
        ''' <param name="textBox2">CompanyName</param>
        ''' <param name="textBox3">Phone</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function Update(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String, textBox1 As String, textBox2 As String, _
            textBox3 As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button8", "Update", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 動的SQLの要素を設定
            testParameterValue.ShipperID = Integer.Parse(textBox1)
            testParameterValue.CompanyName = textBox2
            testParameterValue.Phone = textBox3

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo
            Else
                ' 結果（正常系）
                message = testReturnValue.Obj.ToString() & "件更新"
            End If

            Dim model As New CrudModel() With {.Message = message}

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを削除する
        ''' </summary>
        ''' <param name="ddlDap">データプロバイダ</param>
        ''' <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        ''' <param name="ddlMode2">静的、動的のクエリ モード</param>
        ''' <param name="ddlExRollback">コミット、ロールバック</param>
        ''' <param name="textBox1">ShipperId</param>
        ''' <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        Public Function Delete(ddlDap As String, ddlMode1 As String, ddlMode2 As String, ddlExRollback As String, textBox1 As String) As ActionResult
            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim testParameterValue As New TestParameterValue( _
                "CrudMu", "button9", "Delete", _
                ddlDap & "%" & ddlMode1 & "%" & ddlMode2 & "%" & ddlExRollback, _
                New MyUserInfo("aaa", "192.168.1.1"))

            ' 動的SQLの要素を設定
            testParameterValue.ShipperID = Integer.Parse(textBox1)

            ' 戻り値
            Dim testReturnValue As TestReturnValue

            ' 分離レベルの設定
            Dim iso As DbEnum.IsolationLevelEnum = DbEnum.IsolationLevelEnum.DefaultTransaction

            ' Ｂ層呼出し＋都度コミット
            Dim layerB As New LayerB()
            testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, iso), TestReturnValue)

            ' 結果表示するメッセージ
            Dim message As String = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                message += "ErrorInfo:" + testReturnValue.ErrorInfo
            Else
                ' 結果（正常系）
                message = testReturnValue.Obj.ToString() & "件削除"
            End If

            Dim model As New CrudModel() With {.Message = message}

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>画面遷移する</summary>
        ''' <returns>画面遷移のためのJavaScriptResult</returns>
        Public Function Transitions() As ActionResult
            Return JavaScript("location.href='" & Url.Action("Index", "CrudMu2") & "';")
        End Function

        ''' <summary>
        ''' Sleepを実行し二重送信防止機能をテストする
        ''' </summary>
        ''' <returns>EmptyResult</returns>
        Public Function PreventDoubleSubmission() As ActionResult
            System.Threading.Thread.Sleep(5 * 1000)

            ' 結果表示するメッセージ

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            Dim model As New CrudModel() With {.Message = "PreventDoubleSubmission:" & Session("cnt").ToString()}

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function
    End Class
End Namespace
