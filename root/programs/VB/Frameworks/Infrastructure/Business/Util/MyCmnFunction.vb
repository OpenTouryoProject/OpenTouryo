'**********************************************************************************
'* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
' 
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：MyCmnFunction
'* クラス日本語名  ：Business層の共通クラス（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
'*  2009/06/02  西野 大介         sln - IR版からの修正
'*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
'*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
'*  2009/08/10  西野 大介         同名のコントロール追加に対応（GridView/ItemTemplate）。
'*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
'*                                nullチェック方法、Contains → ContainsKeyなどに注意
'*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
'*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
'*  2014/05/16  西野 大介         キャスト可否チェックのロジックを見直した。
'*  2017/01/31  西野 大介         System.Webを使用しているCalculateSessionSizeメソッドをPublicから移動
'**********************************************************************************

Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Util
    ''' <summary>Business層の共通クラス</summary>
    Public Class MyCmnFunction

#Region "CalculateSessionSize"

        ''' <summary>Sessionサイズ測定</summary>
        ''' <returns>Sessionサイズ（MB）</returns>
        ''' <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        Public Shared Function CalculateSessionSizeMB() As Long
            'return MyCmnFunction.CalculateSessionSizeKB() / 1000;
            Return CLng(Math.Truncate(Math.Round(MyCmnFunction.CalculateSessionSize() / 1000000.0, 0, MidpointRounding.AwayFromZero)))

        End Function

        ''' <summary>Sessionサイズ測定</summary>
        ''' <returns>Sessionサイズ（KB）</returns>
        ''' <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        Public Shared Function CalculateSessionSizeKB() As Long
            'return MyCmnFunction.CalculateSessionSize() / 1000;
            Return CLng(Math.Truncate(Math.Round(MyCmnFunction.CalculateSessionSize() / 1000.0, 0, MidpointRounding.AwayFromZero)))
        End Function

        ''' <summary>Sessionサイズ測定</summary>
        ''' <returns>Sessionサイズ（バイト）</returns>
        ''' <remarks>シリアル化できないオブジェクトを含む場合は落ちる。</remarks>
        Public Shared Function CalculateSessionSize() As Long
            ' ワーク変数
            Dim size As Long = 0

            ' SessionのオブジェクトをBinarySerializeしてサイズを取得。
            For Each key As String In HttpContext.Current.Session.Keys
                ' 当該キーのオブジェクト・サイズを足しこむ。
                size += BinarySerialize.ObjectToBytes(HttpContext.Current.Session(key)).Length
            Next

            ' Sessionサイズ（バイト）
            Return size
        End Function

#End Region

        ' 2009/07/21-start

#Region "コントロール取得＆イベントハンドラ設定"

        ''' <summary>コントロール取得＆イベントハンドラ設定（下位互換）</summary>
        ''' <param name="ctrl">コントロール</param>
        ''' <param name="prefix">プレフィックス</param>
        ''' <param name="eventHandler">イベント ハンドラ</param>
        ''' <param name="controlHt">ディクショナリ</param>
        Friend Shared Sub GetCtrlAndSetClickEventHandler(ctrl As Control, prefix As String, eventHandler As Object, controlHt As Dictionary(Of String, Control))
            '#Region "チェック処理"

            ' コントロール指定が無い場合
            If ctrl Is Nothing Then
                ' 何もしないで戻る。
                Return
            End If

            ' プレフィックス指定が無い場合
            If prefix Is Nothing OrElse prefix = "" Then
                ' 何もしないで戻る。
                Return
            End If

            '#End Region

            '#Region "コントロール取得＆イベントハンドラ設定"

            ' コントロールのIDチェック
            ' コントロールID無し
            If ctrl.ID Is Nothing Then
            Else
                ' コントロールID有り

                ' コントロールのID長確認
                If prefix.Length <= ctrl.ID.Length Then
                    ' 指定のプレフィックス
                    If prefix = ctrl.ID.Substring(0, prefix.Length) Then
                        ' イベントハンドラを設定する。
                        If prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX) Then
                            ' CHECK BOX
                            Dim checkBox As CheckBox = Nothing

                            Try
                                ' キャストできる
                                checkBox = DirectCast(ctrl, CheckBox)
                            Catch ex As Exception
                                ' キャストできない
                                Throw New FrameworkException(
                                    FrameworkExceptionMessage.CONTROL_TYPE_ERROR(0),
                                    [String].Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(1), prefix, ctrl.[GetType]().ToString()), ex)
                            End Try

                            AddHandler checkBox.CheckedChanged, DirectCast(eventHandler, EventHandler)

                            ' ディクショナリに格納
                            ' ControlHt.Add(ctrl.ID, ctrl);
                            ' ControlHt[ctrl.ID] = ctrl;
                            ' 2011/02/12
                            FxCmnFunction.AddControlToDic(ctrl, controlHt)
                        End If
                    End If
                End If
            End If

            '#End Region

            '#Region "再帰"

            ' 子コントロールがある場合、
            If ctrl.HasControls() Then
                ' 子コントロール毎に
                For Each childCtrl As Control In ctrl.Controls
                    ' 再帰する。
                    MyCmnFunction.GetCtrlAndSetClickEventHandler(childCtrl, prefix, eventHandler, controlHt)
                Next
            End If

            '#End Region
        End Sub

        ''' <summary>コントロール取得＆イベントハンドラ設定</summary>
        ''' <param name="ctrl">コントロール</param>
        ''' <param name="prefixAndEvtHndHt">プレフィックスとイベント ハンドラのディクショナリ</param>
        ''' <param name="controlHt">コントロールのディクショナリ</param>
        Friend Shared Sub GetCtrlAndSetClickEventHandler2(ctrl As Control, prefixAndEvtHndHt As Dictionary(Of String, Object), controlHt As Dictionary(Of String, Control))
            ' ループ
            For Each prefix As String In prefixAndEvtHndHt.Keys
                Dim eventHandler As Object = prefixAndEvtHndHt(prefix)

                '#Region "チェック処理"

                ' コントロール指定が無い場合
                If ctrl Is Nothing Then
                    ' 何もしないで戻る。
                    Return
                End If

                ' プレフィックス指定が無い場合
                If prefix Is Nothing OrElse prefix = "" Then
                    ' 何もしないで戻る。
                    Return
                End If

                '#End Region

                '#Region "コントロール取得＆イベントハンドラ設定"

                ' コントロールのIDチェック
                ' コントロールID無し
                If ctrl.ID Is Nothing Then
                Else
                    ' コントロールID有り

                    ' コントロールのID長確認
                    If prefix.Length <= ctrl.ID.Length Then
                        ' 指定のプレフィックス
                        If prefix = ctrl.ID.Substring(0, prefix.Length) Then
                            ' イベントハンドラを設定する。
                            If prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX) Then
                                ' CHECK BOX
                                Dim checkBox As CheckBox = Nothing

                                If TypeOf ctrl Is CheckBox Then
                                    ' キャストできる
                                    checkBox = DirectCast(ctrl, CheckBox)
                                Else
                                    ' キャストできない
                                    Throw New FrameworkException(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(0), [String].Format(FrameworkExceptionMessage.CONTROL_TYPE_ERROR(1), prefix, ctrl.[GetType]().ToString()))
                                End If

                                AddHandler checkBox.CheckedChanged, DirectCast(eventHandler, EventHandler)

                                ' ディクショナリに格納
                                controlHt(ctrl.ID) = ctrl
                                Exit For
                            End If
                        End If
                    End If

                    '#End Region
                End If
            Next

            '#Region "再帰"

            ' 子コントロールがある場合、
            If ctrl.HasControls() Then
                ' 子コントロール毎に
                For Each childCtrl As Control In ctrl.Controls
                    ' 再帰する。
                    MyCmnFunction.GetCtrlAndSetClickEventHandler2(childCtrl, prefixAndEvtHndHt, controlHt)
                Next
            End If

            '#End Region
        End Sub

#End Region

        ' 2009/07/21-end
    End Class
End Namespace
