
'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_JoinTableName__Screen_Detail
'* クラス日本語名  ：三層データバインド・詳細表示画面（_JoinTableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）,  _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/12/22 Sai          Modified ReadOnly property of the primary key column textbox to true.  
'*  2016/06/14 Shashikiran  Modified UOC_btnUpdate_Click event to process multiple table update in single transaction
'*  2016/06/24 Shashikiran  Modified UOC_btnDelete_Click event to process multiple table Delete in single transaction
'*  2016/06/24 Shashikiran  Added remarks above UOC_btnDelete_Click event as a guideline for developers to modify the code to set the
'*                          table sequence appropriately for successful delete operation
'**********************************************************************************
' System
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
Partial Public Class _JoinTableName__Screen_Detail
    Inherits MyBaseController

#Region "ページロードのUOCメソッド UOC Method of Page Load"

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
            parameterValue.TableName = "_JoinTableName_"

            ' 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

            ' 編集状態の初期化

            ' 値
            ' ControlComment:LoopStart-PKColumn
            Me.txt_JoinTextboxColumnName_.Text = returnValue.Dt.Rows(0)("_JoinColumnName_").ToString()
            ' ControlComment:LoopEnd-PKColumn
            ' ControlComment:LoopStart-ElseColumn
            Me.txt_JoinTextboxColumnName_.Text = returnValue.Dt.Rows(0)("_JoinColumnName_").ToString()
            ' ControlComment:LoopEnd-ElseColumn
            ' 編集 EDIT
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

#Region "イベントハンドラ EVENT HANDLER"

#Region "編集状態の変更 EDIT CHANGE STATE"

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

#Region "更新系 CRUD SYSTEM"

#Region "Update Record"
    ''' <summary>更新ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnUpdate_Click(ByVal fxEventArgs As FxEventArgs) As String

        '#Region "Create the instance of classes here"

        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecordDM", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        'Initialize the data access procedure
        Dim returnValue As _3TierReturnValue = Nothing
        ' B layer Initialize
        Dim b As New _3TierEngine()
        Dim UpdateWhereConditions As Dictionary(Of String, Object) = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))
        '' Modifications for DM 
        parameterValue.AndEqualSearchConditions = New Dictionary(Of String, Object)()
        ''Declare InsertUpdateValue dictionary and add the values to it
        parameterValue.InsertUpdateValues = New Dictionary(Of String, Object)()
        parameterValue.TargetTableNames = New Dictionary(Of Integer, String)()
        ''Declaring the table counter to add it to TargetTableNames Dictionary
        Dim TableCounter As Integer = 0

        '#End Region

        ' ControlComment:LoopStart-JoinTables
        '#Region "Set the values to be updated to the _TableName_. Then Update to database"
        TableCounter = TableCounter + 1
        ' Remove '_TableName__' from the PrimaryKeyandTimeStamp dictionary Key values so developer need not to change the values manually in Dao_TableName__S3_UPDATE.xml

        For Each k As String In UpdateWhereConditions.Keys
            If k.Contains("_TableName_") Then
                If Not parameterValue.AndEqualSearchConditions.ContainsKey(k.Replace("_TableName_" & "_", "")) Then
                    parameterValue.AndEqualSearchConditions.Add(k.Replace("_TableName_" & "_", ""), UpdateWhereConditions(k))
                End If
            End If
        Next
       
        ' ControlComment:LoopStart-ElseColumn
        parameterValue.InsertUpdateValues.Add("_JoinTextboxColumnName_", Me.txt_JoinTextboxColumnName_.Text)
        ' ControlComment:LoopEnd-ElseColumn  

        'Reset returnvalue with null;
        returnValue = Nothing
        'Name of the table  _TableName_
        parameterValue.TargetTableNames.Add(TableCounter, "_TableName_")
        '#End Region
        ' ControlComment:LoopEnd-JoinTables
        ' Run the Database access process
        returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        Me.lblResult_TableName_.Text = returnValue.Obj.ToString() + " Table Data Updated Sucessfully"



        'Return empty string since there is no need to redirect to any other page.
        Return String.Empty
    End Function

#End Region

#Region "Delete Records"
    ''' <summary>削除ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    ''' <remarks>In case of deleting from multiple tables and when the tables have dependent relation, the sequence of execution of delete statements for these tables become necessary. 
    ''' Developer should decide the sequence of table for the delete operation.
    ''' This can be managed by altering the position of code block present in the region 'Delete the data from the [TableName] table' as required</remarks>
    Protected Function UOC_btnDelete_Click(ByVal fxEventArgs As FxEventArgs) As String
        '#Region "Create the instance of classes here"
        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecordDM", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        'Initialize the data access procedure
        Dim returnValue As _3TierReturnValue = Nothing
        ' B layer Initialize
        Dim b As New _3TierEngine()
        Dim DeleteWhereConditions As Dictionary(Of String, Object) = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))
        ' Modifications for DM 
        parameterValue.AndEqualSearchConditions = New Dictionary(Of String, Object)()
        parameterValue.TargetTableNames = New Dictionary(Of Integer, String)()
        ' Declaring the table counter to add it to TargetTableNames Dictionary
        Dim TableCounter As Integer = 0

        '#End Region
        ' ControlComment:LoopStart-JoinTables
        '#Region "Delete the data from the _TableName_  table"
        ' Remove '_TableName__' from the PrimaryKeyandTimeStamp dictionary Key values so developer need not to change the values manually in Dao_TableName__S4_Delete.xml 
        TableCounter = TableCounter + 1
        For Each k As String In DeleteWhereConditions.Keys
            If k.Split("_"c)(0) = "_TableName_" Then
                If Not parameterValue.AndEqualSearchConditions.ContainsKey(k) Then
                    parameterValue.AndEqualSearchConditions.Add(k, DeleteWhereConditions(k))
                End If
            End If
        Next
        'Reset returnvalue with null;
        returnValue = Nothing
        'Name of the table  _TableName_
        parameterValue.TargetTableNames.Add(TableCounter, "_TableName_")
        '#End Region
        ' ControlComment:LoopEnd-JoinTables
        ' Run the Database access process
        returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        Me.lblResult_TableName_.Text = returnValue.Obj.ToString() + " Data is Deleted from the table: _TableName_"

        'Return empty string since there is no need to redirect to any other page.
        Return String.Empty
    End Function

#End Region

#End Region

#Region "Toggle Control Read only Property"
    ''' <summary>編集可否の制御</summary>
    ''' <param name="readOnly">読取専用プロパティ</param>
    Private Sub SetControlReadOnly(ByVal [readOnly] As Boolean)
        ' 編集可否
        ' ReadOnly

        ' 主キー
        ' ControlComment:LoopStart-PKColumn
        Me.txt_JoinTextboxColumnName_.[ReadOnly] = True
        ' ControlComment:LoopEnd-PKColumn

        ' 主キー以外
        ' ControlComment:LoopStart-ElseColumn
        Me.txt_JoinTextboxColumnName_.[ReadOnly] = [readOnly]
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
        Me.txt_JoinTextboxColumnName_.BackColor = System.Drawing.Color.LightGray
        ' ControlComment:LoopEnd-PKColumn

        ' 主キー以外
        ' ControlComment:LoopStart-ElseColumn
        Me.txt_JoinTextboxColumnName_.BackColor = backColor
        ' ControlComment:LoopEnd-ElseColumn

    End Sub
#End Region

#End Region

End Class
