'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testDLScreen
'* クラス日本語名  ：PDFダウンロードのテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.IO

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Public.Util

Namespace Aspx.TestFxLayerP
    ''' <summary>PDFダウンロードのテスト画面（Ｐ層）</summary>
    Partial Public Class testDLScreen
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:

            Response.Clear()

            Response.ContentType = "application/pdf"

            ' HTTPヘッダーの書き方で、
            'こっちは、専用アプリケーションで開く
            'Response.AppendHeader("Content-Disposition", "attachment;filename=test.pdf");

            'こっちは、IEからOLEオブジェクトを開く
            Response.AppendHeader("Content-Disposition", "inline;filename=test.pdf")

            Response.WriteFile(Path.Combine(GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"))

            Response.[End]()
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:
        End Sub

#End Region
    End Class
End Namespace
