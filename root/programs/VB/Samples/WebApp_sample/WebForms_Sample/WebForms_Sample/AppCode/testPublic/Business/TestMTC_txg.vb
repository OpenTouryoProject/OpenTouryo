'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：TestMTC_txg
'* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御－複数コネクション版）
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

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Public.Db

Imports WebForms_Sample.MyType

''' <summary>
''' TestMTC_mcn の概要の説明です
''' </summary>
Public Class TestMTC_txg
    Inherits MyFcBaseLogic
    ''' <summary>
    ''' 業務処理を実装
    ''' </summary>
    ''' <param name="parameterValue">引数クラス</param>
    ''' <param name="returnValue">戻り値クラス</param>
    Protected Overloads Overrides Sub UOC_DoAction(ByVal parameterValue As BaseParameterValue, ByRef returnValue As BaseReturnValue)
        ' 引数クラスをアップキャスト
        Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

        ' 戻り値クラスを生成
        Dim testReturn As New TestReturnValue()

        ' 戻り値クラスをダウンキャストして戻す
        returnValue = DirectCast(testReturn, BaseReturnValue)

        ' ---

        ' トランザクション パターンIDの領域
        Dim transactionPatternIDs As String() = Nothing

        ' トランザクション グループIDからトランザクション パターンIDを取得
        BaseLogic.GetTransactionPatterns(DirectCast(testParameter.Obj, String), transactionPatternIDs)

        ' トランザクション パターンIDを設定
        testReturn.Obj = transactionPatternIDs

        '#Region "Damを初期化"

        ' トランザクション グループIDから取得した、
        ' トランザクション パターンIDでDam初期化する。
        For Each transactionPatternID As String In transactionPatternIDs
            Dim tempDam As BaseDam = Nothing

            If transactionPatternID.IndexOf("SQL") <> -1 Then
                ' DamSqlSvrを初期化してセット
                tempDam = New DamSqlSvr()
                BaseLogic.InitDam(transactionPatternID, tempDam)
                Me.SetDam(transactionPatternID, tempDam)
            ElseIf transactionPatternID.IndexOf("ODP") <> -1 Then
                ' DamManagedOdpを初期化してセット
                tempDam = New DamManagedOdp()
                BaseLogic.InitDam(transactionPatternID, tempDam)
                Me.SetDam(transactionPatternID, tempDam)
            ElseIf transactionPatternID.IndexOf("MCN") <> -1 Then
                ' DamMySQLを初期化してセット
                tempDam = New DamMySQL()
                BaseLogic.InitDam(transactionPatternID, tempDam)
                Me.SetDam(transactionPatternID, tempDam)
            End If
        Next

        '#End Region

        '#Region "終了時の状態選択"

        '#Region "Damの状態選択"

        ' トランザクションあり
        If (parameterValue.ActionType.Split("%"c))(2) = "UT" Then
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NT" Then
            ' トランザクションなし
            ' → まえもってロールバックしておく

            ' ロールバック
            For Each transactionPatternID As String In transactionPatternIDs
                Me.GetDam(transactionPatternID).RollbackTransaction()
            Next
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NC" Then
            ' コネクションなし
            ' → まえもってロールバック、コネクションクローズしておく
            '
            ' ※ トランザクションを開始して
            '    コミットしないで閉じると、ロールバック扱い。

            ' ロールバック
            For Each transactionPatternID As String In transactionPatternIDs
                Me.GetDam(transactionPatternID).RollbackTransaction()
            Next

            ' コネクションクローズ
            For Each transactionPatternID As String In transactionPatternIDs
                Me.GetDam(transactionPatternID).ConnectionClose()
            Next
        ElseIf (parameterValue.ActionType.Split("%"c))(2) = "NULL" Then
            ' データアクセス制御クラス = Null
            ' → まえもってロールバック、コネクションクローズ、Nullクリアしておく
            '
            ' ※ トランザクションを開始して
            '    コミットしないで閉じると、ロールバック扱い。

            ' ロールバック
            For Each transactionPatternID As String In transactionPatternIDs
                Me.GetDam(transactionPatternID).RollbackTransaction()
            Next

            ' コネクションクローズ
            For Each transactionPatternID As String In transactionPatternIDs
                Me.GetDam(transactionPatternID).ConnectionClose()
            Next

            ' Nullクリア
            For Each transactionPatternID As String In transactionPatternIDs
                Me.SetDam(transactionPatternID, Nothing)
            Next
        End If

        '#End Region

        '#Region "エラーのスロー"

        If (parameterValue.ActionType.Split("%"c))(1) = "Business" Then
            ' 業務例外のスロー
            Throw New BusinessApplicationException("ロールバックのテスト", "ロールバックのテスト", "エラー情報")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "System" Then
            ' システム例外のスロー
            Throw New BusinessSystemException("ロールバックのテスト", "ロールバックのテスト")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other" Then
            ' その他、一般的な例外のスロー
            Throw New Exception("ロールバックのテスト")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other-Business" Then
            ' その他、一般的な例外（業務例外へ振り替え）のスロー
            Throw New Exception("Other-Business")
        ElseIf (parameterValue.ActionType.Split("%"c))(1) = "Other-System" Then
            ' その他、一般的な例外（システム例外へ振り替え）のスロー
            Throw New Exception("Other-System")
        End If

        '#End Region

        '#End Region
    End Sub
End Class
