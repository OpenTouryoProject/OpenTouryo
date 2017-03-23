'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testNestMasterScreen
'* クラス日本語名  ：Master Pageのテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.TestFxLayerP.Nest
    ''' <summary>テスト画面ネスト（Ｐ層）</summary>
    Partial Public Class testNestMasterScreen
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:
        End Sub

#End Region

#Region "共通処理"

        ''' <summary>共通処理１</summary>
        ''' <param name="buttonID">ButtonID</param>
        ''' <param name="x">CPFを表す文字数</param>
        Private Sub commonM(buttonID As String, x As Integer)
            ' Controlの取得
            DirectCast(Me.GetMasterWebControl("lblMSG"), Label).Text = buttonID

            ' ラベルを非表示
            Dim ctrl As Control = Me.GetMasterWebControl("lblTest" & buttonID.Substring(buttonID.Length - x, x))

            ctrl.Visible = Not (ctrl.Visible)
        End Sub

        ''' <summary>共通処理２</summary>
        ''' <param name="buttonID">ButtonID</param>
        ''' <param name="x">CPFを表す文字数</param>
        Private Sub commonC(buttonID As String, x As Integer)
            ' Controlの取得
            DirectCast(Me.GetMasterWebControl("lblMSG"), Label).Text = buttonID

            ' ラベルを非表示
            Dim ctrl As Control = Me.GetContentWebControl("lblTest" & buttonID.Substring(buttonID.Length - x, x))

            ctrl.Visible = Not (ctrl.Visible)
        End Sub

#End Region

#Region "イベント処理"

#Region "Master Page"

#Region "rootMasterPage"

        ''' <summary>btnButtonのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_rootMasterPage_btnButton_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 0)
            Return System.[String].Empty
        End Function

#End Region

#Region "branchMasterPage1"

        ''' <summary>btnButtonAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage1_btnButtonA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 1)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage1_btnButtonB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 1)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage1_btnButtonC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 1)
            Return System.[String].Empty
        End Function

#End Region

#Region "branchMasterPage2"

        ''' <summary>btnButtonAAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonAA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonABのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonAB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonACのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonAC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonBA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonBB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonBC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonCA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonCB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_branchMasterPage2_btnButtonCC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonM(fxEventArgs.ButtonID, 2)
            Return System.[String].Empty
        End Function

#End Region

#End Region

#Region "コンテンツ"

        ''' <summary>btnButtonAAAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonAAA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonAABのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonAAB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonAACのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonAAC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonABAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonABA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonABBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonABB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonABCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonABC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonACAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonACA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonACBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonACB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonACCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonACC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBAAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBAA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBABのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBAB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBACのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBAC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBBAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBBA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBBBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBBB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBBCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBBC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBCAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBCA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBCBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBCB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonBCCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonBCC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCAAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCAA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCABのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCAB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCACのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCAC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCBAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCBA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCBBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCBB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCBCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCBC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCCAのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCCA_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCCBのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCCB_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        ''' <summary>btnButtonCCCのClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButtonCCC_Click(fxEventArgs As FxEventArgs) As String
            Me.commonC(fxEventArgs.ButtonID, 3)
            Return System.[String].Empty
        End Function

        '/// ちなみに存在しないControlを検索した場合どうなるかチェックする。
        '/Control ctrl = null;
        '/ctrl = Me.GetMasterWebControl("xxxx");
        '/ctrl = Me.GetContentWebControl("xxxx");

#End Region

#End Region
    End Class
End Namespace
