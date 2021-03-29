'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_TableName_Detail
'* クラス日本語名  ：三層データバインド・詳細表示画面（_TableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/12/22  Sai               Modified ReadOnly property of the primary key column textbox to true.
'**********************************************************************************

Imports MyType

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

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

''' <summary>三層データバインド・サンプル アプリ画面（詳細表示）</summary>
Partial Public Class _TableName_Detail
    Inherits MyBaseController
#Region "ページロードのUOCメソッド"

    ''' <summary>
    ''' ページロードのUOCメソッド（個別：初回ロード）
    ''' </summary>
    ''' <remarks>
    ''' 実装必須
    ''' </remarks>
    Protected Overrides Sub UOC_FormInit()
        ' フォーム初期化（初回ロード）時に実行する処理を実装する

        ' TODO:
        Dim parameterValue As _3TierParameterValue = Nothing
        Dim returnValue As _3TierReturnValue = Nothing

        If Session("PrimaryKeyAndTimeStamp") Is Nothing Then
            ' 追加処理のみ。
            Me.btnEdit.Enabled = False
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False

            ' 編集
            Me.SetControlReadOnly(False)
        Else
            ' 詳細表示処理

            ' 引数クラスを生成
            parameterValue = New _3TierParameterValue(Me.ContentPageFileNoEx, "FormInit", "SelectRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

            ' テーブル
            parameterValue.TableName = "_TableName_"

            ' 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

            ' 編集状態の初期化

            ' 値
            ' ControlComment:LoopStart-PKColumn
            Me.txt_ColumnName_.Text = returnValue.Dt.Rows(0)("_ColumnName_").ToString()
            ' ControlComment:LoopEnd-PKColumn
            ' ControlComment:LoopStart-ElseColumn
            Me.txt_ColumnName_.Text = returnValue.Dt.Rows(0)("_ColumnName_").ToString()
            ' ControlComment:LoopEnd-ElseColumn
            ' 編集
            Me.SetControlReadOnly(True)
        End If
    End Sub

    ''' <summary>
    ''' ページロードのUOCメソッド（個別：ポストバック）
    ''' </summary>
    ''' <remarks>
    ''' 実装必須
    ''' </remarks>
    Protected Overrides Sub UOC_FormInit_PostBack()
        ' フォーム初期化（ポストバック）時に実行する処理を実装する

        ' TODO:
        Session("DAP") = "_DAP_"
    End Sub

#End Region

#Region "イベントハンドラ"

#Region "編集状態の変更"

    ''' <summary>編集ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnEdit_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 編集状態の変更

        ' 編集
        Me.SetControlReadOnly(False)

        ' 画面遷移しない。
        Return String.Empty
    End Function

#End Region

#Region "更新系"

    ''' <summary>追加ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnInsert_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        ' テーブル
        parameterValue.TableName = "_TableName_"

        ' 追加値（TimeStamp列は外す。主キーは採番方法次第。
        parameterValue.InsertUpdateValues = New Dictionary(Of String, Object)()
        ' ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", Me.txt_ColumnName_.Text)
        ' ControlComment:LoopEnd-ElseColumn

        ' B層を生成
        Dim b As New _3TierEngine()

        ' データ取得処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        ' 結果表示
        Me.lblResult.Text = returnValue.Obj.ToString() + "件追加しました。"

        ' 画面遷移しない。
        Return String.Empty
    End Function

    ''' <summary>更新ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnUpdate_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        ' テーブル
        parameterValue.TableName = "_TableName_"

        ' 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

        ' 更新値（TimeStamp列は外す。主キーは採番方法次第。
        parameterValue.InsertUpdateValues = New Dictionary(Of String, Object)()
        ' ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_ColumnName_", Me.txt_ColumnName_.Text)
        ' ControlComment:LoopEnd-ElseColumn      

        ' B層を生成
        Dim b As New _3TierEngine()

        ' データ取得処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        ' 結果表示
        Me.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。"

        ' 画面遷移しない。
        Return String.Empty
    End Function

    ''' <summary>削除ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnDelete_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        ' テーブル
        parameterValue.TableName = "_TableName_"

        ' 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

        ' B層を生成
        Dim b As New _3TierEngine()

        ' データ取得処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        ' 結果表示
        Me.lblResult.Text = returnValue.Obj.ToString() + "件削除しました。"

        ' 画面遷移しない。
        Return String.Empty
    End Function

#End Region

    ''' <summary>編集可否の制御</summary>
    ''' <param name="readOnly">読取専用プロパティ</param>
    Private Sub SetControlReadOnly(ByVal [readOnly] As Boolean)
        ' 編集可否
        ' ReadOnly

        ' 主キー
        ' ControlComment:LoopStart-PKColumn
        Me.txt_ColumnName_.[ReadOnly] = True
        ' ControlComment:LoopEnd-PKColumn

        ' 主キー以外
        ' ControlComment:LoopStart-ElseColumn
        Me.txt_ColumnName_.[ReadOnly] = [readOnly]
        ' ControlComment:LoopEnd-ElseColumn


        ' 背景色
        ' BackColor
        Dim backColor As System.Drawing.Color

        If [readOnly] Then
            backColor = System.Drawing.Color.LightGray
        Else
            backColor = System.Drawing.Color.Empty
        End If

        ' 主キー
        ' ControlComment:LoopStart-PKColumn
        Me.txt_ColumnName_.BackColor = backColor ' System.Drawing.Color.LightGray
        ' ControlComment:LoopEnd-PKColumn

        ' 主キー以外
        ' ControlComment:LoopStart-ElseColumn
        Me.txt_ColumnName_.BackColor = backColor
        ' ControlComment:LoopEnd-ElseColumn


    End Sub

#End Region
End Class

