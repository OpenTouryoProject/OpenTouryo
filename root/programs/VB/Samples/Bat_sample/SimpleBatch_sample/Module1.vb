'**********************************************************************************
'* サンプル バッチ
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Module1
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
Imports SimpleBatch_sample.Common
Imports SimpleBatch_sample.Business

' System
Imports System
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
        testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

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
