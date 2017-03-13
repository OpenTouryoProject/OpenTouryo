'**********************************************************************************
'* 単純バッチ処理・サンプル アプリ
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Program
'* クラス日本語名  ：サンプル バッチ
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports SimpleBatch_sample.Business
Imports SimpleBatch_sample.Common

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

''' <summary>Module1</summary>
Module Module1

    ''' <summary>Main</summary>
    Sub Main()
        '***********************************************************************
        '* 簡素なサンプルなので、
        '* ・多重化（タスク毎、結果セットを分割）
        '* ・フェッチ・サイズ（メモリ消費量を抑える）
        '* ・コミット・インターバル、リラン
        '* 等の考慮が別途必要になることがあります。
        '***********************************************************************

        ' コマンドラインをバラす関数がある。
        Dim valsLst As List(Of String) = Nothing
        Dim argsDic As Dictionary(Of String, String) = Nothing

        PubCmnFunction.GetCommandArgs("/"c, argsDic, valsLst)

        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            System.Reflection.Assembly.GetExecutingAssembly().Location, _
            "-", "SelectCount", _
            argsDic("/DAP") & "%" & _
            argsDic("/MODE1") & "%" & _
            argsDic("/MODE2") & "%" & _
            argsDic("/EXROLLBACK"), _
            New MyUserInfo("", ""))

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Ｂ層呼出し
        Dim layerB As New LayerB()
        testReturnValue = layerB.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            Dim [error] As String = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            [error] += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            [error] += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf

            Console.WriteLine([error])
            Console.ReadKey()
        Else
            ' 結果（正常系）
            Console.WriteLine(testReturnValue.Obj.ToString() & "件のデータがあります")
            Console.ReadKey()
        End If
    End Sub

End Module
