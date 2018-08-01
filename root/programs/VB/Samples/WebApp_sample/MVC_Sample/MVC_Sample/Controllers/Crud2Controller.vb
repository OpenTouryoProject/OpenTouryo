'**********************************************************************************
'* サンプル アプリ・コントローラ
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Crud2Controller
'* クラス日本語名  ：Ajax.BeginForm用サンプル アプリ・コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports MVC_Sample.Logic.Business
Imports MVC_Sample.Logic.Common
Imports MVC_Sample.Models.ViewModels

Imports System.Threading.Tasks

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Public.Db

Namespace Controllers
    ''' <summary>
    ''' Ajax.BeginForm用サンプル アプリ・コントローラ
    ''' </summary>
    <Authorize>
    Public Class Crud2Controller
        Inherits MyBaseMVController
        ''' <summary>
        ''' 画面の初期表示
        ''' GET: /Crud2/
        ''' </summary>
        ''' <returns>初期表示状態の画面 (ViewResult)</returns>
        <HttpGet>
        Public Function Index(model As CrudViweModel) As ActionResult
            Return View(model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード数をカウントする
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function SelectCount(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

                ' 結果表示するメッセージ
                Dim message As String = ""

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
            End If

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataTable として取得する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function SelectAll_DT(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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
                    model.Shippers = testReturnValue.Obj
                End If
            End If

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_ChartView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataSet として取得する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function SelectAll_DS(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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
                    model.Shippers = testReturnValue.Obj
                End If
            End If

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_ChartView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を DataReader として取得する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function SelectAll_DR(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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
                    model.Shippers = testReturnValue.Obj
                End If
            End If
            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_ChartView", model)

        End Function

        ''' <summary>
        ''' Shippers テーブルのレコード全件を、動的 SQL を使用して取得する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function SelectAll_DSQL(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' 動的SQLの要素を設定
                testParameterValue.OrderColumn = model.DdlOrderColumn
                testParameterValue.OrderSequence = model.DdlOrderSequence

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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
                    model.Shippers = testReturnValue.Obj
                End If
            End If
            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_ChartView", model)

        End Function

        ''' <summary>
        ''' ShipperId をキーにして Shippers テーブルのレコードを取得する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function [Select](model As CrudViweModel) As Task(Of ActionResult)
            Dim scriptText As String = ""

            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

                ' 結果表示するメッセージ
                Dim message As String = ""

                If testReturnValue.ErrorFlag = True Then
                    ' 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID & ";"
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage & ";"
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo

                    ' メッセージを設定。
                    model.Message = message

                    ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                    Return PartialView("_MessageView", model)

                Else
                    ' 結果（正常系）
                End If

                ' Ajax.BeginFormでは、UpdateTargetIdで指定した
                ' ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                ' ＃ここではjQueryを使用している。
                Dim svm As ShipperViweModel = DirectCast(testReturnValue.Obj, ShipperViweModel)
                scriptText = String.Format(
                    "$('#Shipper_ShipperID').val('{0}');$('#Shipper_CompanyName').val('{1}');$('#Shipper_Phone').val('{2}');",
                    svm.ShipperID, svm.CompanyName, svm.Phone)
            End If

            Return JavaScript(scriptText)
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを追加する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function Insert(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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

                ' メッセージを設定。
                model.Message = message
            End If
            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを更新する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function Update(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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

                ' メッセージを設定。
                model.Message = message
            End If

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Shippers テーブルに新規レコードを削除する
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Async Function Delete(model As CrudViweModel) As Task(Of ActionResult)
            If ModelState.IsValid Then
                ' 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                Dim testParameterValue As New TestParameterValue(
                    Me.ControllerName, "-", Me.ActionName,
                    Convert.ToString(model.DdlDap) & "%" &
                    Convert.ToString(model.DdlMode1) & "%" &
                    Convert.ToString(model.DdlMode2) & "%" &
                    Convert.ToString(model.DdlExRollback), Me.UserInfo)

                ' 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper

                ' Ｂ層呼出し＋都度コミット
                Dim layerB As New LayerB()
                Dim testReturnValue As TestReturnValue = Await layerB.DoBusinessLogicAsync(testParameterValue, Me.SelectIsolationLevel(model.DdlIso))

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

                ' メッセージを設定。
                model.Message = message
            End If

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>
        ''' Sleepを実行し二重送信防止機能をテストする
        ''' </summary>
        ''' <param name="model">CrudViweModel</param>
        ''' <returns>再描画（ViewResult）</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Function PreventDoubleSubmission(model As CrudViweModel) As ActionResult
            System.Threading.Thread.Sleep(5 * 1000)

            ' メッセージを設定。

            ' 確認用のカウンタ
            If Session("cnt") Is Nothing Then
                Session("cnt") = 1
            Else
                Session("cnt") = CInt(Session("cnt")) + 1
            End If

            model.Message = "PreventDoubleSubmission:" & Session("cnt").ToString()

            ' Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            Return PartialView("_MessageView", model)
        End Function

        ''' <summary>画面遷移する</summary>
        ''' <returns>画面遷移のためのJavaScriptResult</returns>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Public Function Transitions() As ActionResult
            Return JavaScript("location.href='" & Url.Action("Index", "Crud1") & "';")
        End Function

        ''' <summary>分離レベルの設定</summary>
        Private Function SelectIsolationLevel(iso As String) As DbEnum.IsolationLevelEnum
            If iso = "NC" Then
                Return DbEnum.IsolationLevelEnum.NotConnect
            ElseIf iso = "NT" Then
                Return DbEnum.IsolationLevelEnum.NoTransaction
            ElseIf iso = "RU" Then
                Return DbEnum.IsolationLevelEnum.ReadUncommitted
            ElseIf iso = "RC" Then
                Return DbEnum.IsolationLevelEnum.ReadCommitted
            ElseIf iso = "RR" Then
                Return DbEnum.IsolationLevelEnum.RepeatableRead
            ElseIf iso = "SZ" Then
                Return DbEnum.IsolationLevelEnum.Serializable
            ElseIf iso = "SS" Then
                Return DbEnum.IsolationLevelEnum.Snapshot
            ElseIf iso = "DF" Then
                Return DbEnum.IsolationLevelEnum.DefaultTransaction
            Else
                'throw new Exception("分離レベルの設定がおかしい");
                Return DbEnum.IsolationLevelEnum.DefaultTransaction
            End If
        End Function
    End Class
End Namespace
