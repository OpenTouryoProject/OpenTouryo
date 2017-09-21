'**********************************************************************************
'* テーブル・メンテナンス自動生成・テストクラス（マスタデータ・ロード部品
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：GetMasterData
'* クラス日本語名  ：マスタデータ・ロード部品
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common

''' <summary>マスタデータ・ロード部品</summary>
Public Class GetMasterData
	Inherits MyFcBaseLogic
	''' <summary>業務処理を実装</summary>
    ''' <param name="parameterValue">引数クラス</param>
	Private Sub UOC_Invoke(parameterValue As _3TierParameterValue)
		'メソッド引数にBaseParameterValueの派生の型を定義可能。
		' 戻り値クラスを生成して、事前に戻り値に設定しておく。
		Dim returnValue As New _3TierReturnValue()
		Me.ReturnValue = returnValue

		' ↓業務処理-----------------------------------------------------

		' データアクセス クラスを生成する
		Dim daoSuppliers As New DaoSuppliers(Me.GetDam())

		' 全件参照
		Dim dt1 As New DataTable()
		daoSuppliers.D2_Select(dt1)

		' データアクセス クラスを生成する
		Dim daoCategories As New DaoCategories(Me.GetDam())

		' 実行
		Dim dt2 As New DataTable()
		daoCategories.D2_Select(dt2)

		' 戻り値を戻す
		returnValue.Obj = New DataTable() {dt1, dt2}

		' ↑業務処理-----------------------------------------------------
	End Sub
End Class
