'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：LayerB_Dynamic
'* クラス日本語名  ：Ｂ層（動的SQLのCRUD：Categoryテーブル）
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports GenDaoAndBatUpd_sample.Common

Imports Touryo.Infrastructure.Business.RichClient.Business
Imports Touryo.Infrastructure.Framework.Common

Namespace Business
    ''' <summary>Ｂ層（動的SQLのCRUD：Categoryテーブル）</summary>
    Class LayerB_Dynamic
        Inherits MyFcBaseLogic2CS
        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_Insert(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoCategories As New DaoCategories(Me.GetDam())

            ' １件挿入
            'daoCategories.PK_CategoryID = testParameter.field1;
            daoCategories.CategoryName = testParameter.field2
            daoCategories.Description = testParameter.field3
            'daoCategories.Picture = testParameter.field4;

            ' インサート
            testReturn.obj = daoCategories.D1_Insert()

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub

        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_Select(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoCategories As New DaoCategories(Me.GetDam())

            ' ｎ件参照
            If testParameter.field1_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.PK_CategoryID = testParameter.field1_ForSearch
            End If

            If testParameter.field2_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.CategoryName = testParameter.field2_ForSearch
            End If

            Dim dt As New DataTable()
            daoCategories.D2_Select(dt)

            testReturn.dt = dt

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub

        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_Update(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoCategories As New DaoCategories(Me.GetDam())

            ' ｎ件更新

            ' 更新値設定
            If testParameter.field2_ForUpd.ToString().Trim() = "" Then
            Else
                daoCategories.Set_CategoryName_forUPD = testParameter.field2_ForUpd
            End If

            If testParameter.field3_ForUpd.ToString().Trim() = "" Then
            Else
                daoCategories.Set_Description_forUPD = testParameter.field3_ForUpd
            End If

            ' 検索条件設定
            If testParameter.field1_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.PK_CategoryID = testParameter.field1_ForSearch
            End If

            If testParameter.field2_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.CategoryName = testParameter.field2_ForSearch
            End If

            ' アップデート
            testReturn.obj = daoCategories.D3_Update()

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub

        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_Delete(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoCategories As New DaoCategories(Me.GetDam())

            ' ｎ件削除

            ' 検索条件設定
            If testParameter.field1_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.PK_CategoryID = testParameter.field1_ForSearch
            End If

            If testParameter.field2_ForSearch.ToString().Trim() = "" Then
            Else
                daoCategories.CategoryName = testParameter.field2_ForSearch
            End If

            ' デリート
            testReturn.obj = daoCategories.D4_Delete()

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub

        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_SelectAll(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoCategories As New DaoCategories(Me.GetDam())

            ' 全件取得

            ' 実行
            Dim dt As New DataTable()
            daoCategories.D2_Select(dt)

            ' 戻り値を戻す
            testReturn.dt = dt

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub
    End Class
End Namespace
