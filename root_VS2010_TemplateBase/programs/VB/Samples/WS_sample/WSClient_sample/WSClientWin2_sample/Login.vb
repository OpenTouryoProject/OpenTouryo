'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：login
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Collections

' Windowアプリケーション
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Business.RichClient.Presentation

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.RichClient.Presentation

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>login</summary>
Partial Public Class Login
    Inherits MyBaseControllerWin

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        Program.FlagEnd = True ' フラグ初期化
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()

    End Sub

    ''' <summary>ログイン</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        MyBaseControllerWin.UserInfo.UserName = Me.textBox1.Text
        MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName

        Program.FlagEnd = False ' フラグ完了
        Me.Close()
    End Sub
End Class
