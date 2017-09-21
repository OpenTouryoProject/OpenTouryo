'**********************************************************************************
'* サンプル アプリ（非同期処理クラス）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：AsyncFunc
'* クラス日本語名  ：サンプル アプリ 非同期処理クラス
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

' 型情報
Imports WSIFType_sample

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

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Public Class AsyncFunc
    Inherits MyBaseAsyncFunc

    ''' <summary>コンストラクタ</summary>
    ''' <param name="_this">WPFやWinFormの要素</param>
    Public Sub New(ByVal _this As Object)
        MyBase.New(_this)
    End Sub

    ''' <summary>サービスの論理名</summary>
    Public LogicalName As String = ""

    ''' <summary>非同期</summary>
    ''' <param name="param">引数</param>
    ''' <returns>結果</returns>
    ''' <remarks>
    ''' ここは副スレッドから実行されるので注意。
    ''' 非同期処理クラスに非同期処理を定義すると、
    ''' メンバ変数を引数として利用できる。
    ''' </remarks>
    Public Function btn6_Exec(ByVal param As Object) As Object
        ' 戻り値（キャスト）
        Dim testParameterValue As TestParameterValue = DirectCast(param, TestParameterValue)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
        Dim callCtrl As New CallController(Program.AccessToken)

        ' Invoke
        testReturnValue = DirectCast(callCtrl.Invoke(Me.LogicalName, testParameterValue), TestReturnValue)

        '' 非同期メッセージボックス表示のテスト
        'Dim dr As DialogResult = Me.ShowAsyncMessageBoxWin( _
        '    "メッセージ", "タイトル", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

        '' 非同期メッセージボックス表示のテスト（エラー）
        'System.Windows.MessageBoxResult mr = af.ShowAsyncMessageBoxWPF("メッセージ", "タイトル",
        ''System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);

        ' 結果表示
        Return testReturnValue
    End Function
End Class
