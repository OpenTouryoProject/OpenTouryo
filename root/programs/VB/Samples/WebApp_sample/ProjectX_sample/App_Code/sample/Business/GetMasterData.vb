'**********************************************************************************
'* マスタデータ・ロード部品
'**********************************************************************************

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

''' <summary>マスタデータ・ロード部品</summary>
Public Class GetMasterData
	Inherits MyFcBaseLogic
	''' <summary>業務処理を実装</summary>
    ''' <param name="parameterValue">引数クラス</param>
	Private Sub UOC_Invoke(parameterValue As _3TierParameterValue)
		'メソッド引数にBaseParameterValueの派生の型を定義可能。
		' 戻り値クラスを生成して、事前に戻り地に設定しておく。
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
