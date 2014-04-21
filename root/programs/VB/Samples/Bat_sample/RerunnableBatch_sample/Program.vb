'**********************************************************************************
'* サンプル バッチ
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Program
'* クラス日本語名  ：サンプル バッチ
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' 型情報
Imports RerunnableBatch_sample.Common
Imports RerunnableBatch_sample.Business

' System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Collections
Imports System.Collections.Generic

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

''' <summary>Program</summary>
Class Program

    ''' <summary>
    ''' 中間コミットを行う件数 
    ''' </summary>
    ''' <remarks>
    ''' ※ サンプルデータの件数が少ない(830件)ため小さい値としている
    ''' </remarks>
	Public Const INTERMEDIATE_COMMIT_COUNT As Integer = 100

    ''' <summary>Main</summary>
	Friend Shared Sub Main(args As String())

        ' コマンドラインをバラす関数がある。
        Dim valsLst As List(Of String) = Nothing
        Dim argsDic As Dictionary(Of String, String) = Nothing
        PubCmnFunction.GetCommandArgs("/"c, argsDic, valsLst)

        ' 引数クラス値（B層実行用）
		Dim screenId As String = System.Reflection.Assembly.GetExecutingAssembly().Location
		Dim controlId As String = "-"
        Dim actionType As String = "SQL" ' argsDic("/DAP") & "%" & argsDic("/MODE1") & "%" & argsDic("/MODE2") & "%" & argsDic("/EXROLLBACK")
		Dim myUserInfo As New MyUserInfo("userName", "ipAddress")

        ' B層クラス
        Dim layerB As New LayerB()

        ' ↓B層実行：主キー値を全て検索(ORDER BY 主キー)-----------------------------------------------------

        ' 引数クラスを生成
		Dim selectPkListParameterValue As New VoidParameterValue(screenId, controlId, "SelectPkList", actionType, myUserInfo)

        ' Ｂ層呼出し
		Dim selectPkReturnValue As SelectPkListReturnValue = DirectCast(layerB.DoBusinessLogic(selectPkListParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), SelectPkListReturnValue)

        ' 実行結果確認
		If selectPkReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
			Dim [error] As String = "ErrorMessageID:" & selectPkReturnValue.ErrorMessageID & vbCr & vbLf
			[error] += "ErrorMessage:" & selectPkReturnValue.ErrorMessage & vbCr & vbLf
			[error] += "ErrorInfo:" & selectPkReturnValue.ErrorInfo & vbCr & vbLf

			Console.WriteLine([error])
            Console.ReadKey()   'バッチ処理終了
			Return
		End If

        ' 戻り値取得
		Dim pkList As ArrayList = selectPkReturnValue.PkList

        ' ↑B層実行：主キー値を全て検索(ORDER BY 主キー)-----------------------------------------------------

        Dim recordCount As Integer = pkList.Count   ' 全レコード数
        Dim initialIndex As Integer = 0 ' 処理開始インデックス ※ todo:リラン時に途中から再開する場合は初期値を変更する
        Dim transactionCount As Integer = Convert.ToInt32(Math.Ceiling(CDbl(recordCount - initialIndex) / INTERMEDIATE_COMMIT_COUNT))   ' 更新B層実行回数

		For transactionIndex As Integer = 0 To transactionCount - 1
            Dim subPkList As ArrayList      ' 主キー一覧(1トランザクション分)
            Dim subPkStartIndex As Integer  ' 主キー(1トランザクション分)の開始位置
            Dim subPkCount As Integer       ' 主キー数(1トランザクション分)
            ' 取り出す主キーの開始、数を取得
			subPkStartIndex = initialIndex + (transactionIndex * INTERMEDIATE_COMMIT_COUNT)
			If subPkStartIndex + INTERMEDIATE_COMMIT_COUNT - 1 > recordCount - 1 Then
				subPkCount = (recordCount - initialIndex) Mod INTERMEDIATE_COMMIT_COUNT
			Else
				subPkCount = INTERMEDIATE_COMMIT_COUNT
			End If

            ' 主キー一覧(1トランザクション分)を取り出す
			subPkList = New ArrayList()
			subPkList.AddRange(pkList.GetRange(subPkStartIndex, subPkCount))

            ' ↓B層実行：バッチ処理を実行(1トランザクション分)----------------------------------------------------

            ' 引数クラスを生成
			Dim executeBatchProcessParameterValue As New ExecuteBatchProcessParameterValue(screenId, controlId, "ExecuteBatchProcess", actionType, myUserInfo)
			executeBatchProcessParameterValue.SubPkList = subPkList

			' Ｂ層呼出し
			Dim executeBatchProcessReturnValue As VoidReturnValue = DirectCast(layerB.DoBusinessLogic(executeBatchProcessParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), VoidReturnValue)

            ' 実行結果確認
			If selectPkReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
				Dim [error] As String = "ErrorMessageID:" & selectPkReturnValue.ErrorMessageID & vbCr & vbLf
				[error] += "ErrorMessage:" & selectPkReturnValue.ErrorMessage & vbCr & vbLf
				[error] += "ErrorInfo:" & selectPkReturnValue.ErrorInfo & vbCr & vbLf

				Console.WriteLine([error])
                Console.ReadKey()   'バッチ処理終了
				Return
            End If

            '↑B層実行：バッチ処理を実行(1トランザクション分)----------------------------------------------------
		Next
	End Sub
End Class
