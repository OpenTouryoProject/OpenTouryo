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
'* クラス名        ：WCFHTTPSvcForFx
'* クラス日本語名  ：WCFの.NETオブジェクトのバイナリ転送用
'*                   Soap Webメソッドを公開するサービス インターフェイス基盤。
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2012/12/14  西野 大介         新規作成
'*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れた。
'**********************************************************************************

Imports System.ServiceModel
Imports System.Runtime.ExceptionServices

Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Transmission
Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Util

Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
    ' メモ: [リファクター] メニューの [名前の変更] コマンドを使用すると、
    ' コード、svc、および config ファイルで同時にクラス名 "WCFSvcForFx" を変更できます。

    '[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]

    ''' <summary>
    ''' WCFの.NETオブジェクトのバイナリ転送用Soap Webメソッドを公開するサービス インターフェイス基盤。
    ''' </summary>
    <ServiceBehavior>
    Public Class WCFHTTPSvcForFx
        Implements IWCFHTTPSvcForFx
#Region "グローバル変数"

        ''' <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        ''' <remarks>
        ''' 初期化は起動時の１回のみであり、
        ''' 読み取り専用のデータを保持する場合
        ''' のみに適用するデザインパターンとする。
        ''' </remarks>
        Private Shared IPR_NS As New InProcessNameService()

#End Region

#Region "WCFの.NETオブジェクトのバイナリ転送用Soap Webメソッド"

        ''' <summary>
        ''' WCFの.NETオブジェクトのバイナリ転送用Soap Webメソッド
        ''' </summary>
        ''' <param name="serviceName">サービス名</param>
        ''' <param name="contextObject">コンテキスト</param>
        ''' <param name="parameterValueObject">引数</param>
        ''' <param name="returnValueObject">戻り値</param>
        ''' <returns>返すべきエラーの情報</returns>
        ''' <remarks>値は全て.NETオブジェクトをバイナリシリアライズしたバイト配列データ</remarks>
        Public Function DotNETOnlineWS(serviceName As String, ByRef contextObject As Byte(), parameterValueObject As Byte(), ByRef returnValueObject As Byte()) As Byte() Implements IWCFHTTPSvcForFx.DotNETOnlineWS
            ' ステータス
            Dim status As String = "－"

            ' 初期化のため
            returnValueObject = Nothing

            '#Region "呼出し制御関係の変数"

            ' アセンブリ名
            Dim assemblyName As String = ""

            ' クラス名
            Dim className As String = ""

            '#End Region

            '#Region "引数・戻り値関係の変数"

            ' コンテキスト情報
            Dim context As Object
            ' 2009/09/29-この行
            ' 引数・戻り値の.NETオブジェクト
            Dim parameterValue As BaseParameterValue = Nothing
            Dim returnValue As BaseReturnValue = Nothing

            ' エラー情報（クライアント側で復元するため）
            Dim wsErrorInfo As New WSErrorInfo()

            ' エラー情報（ログ出力用）
            Dim errorType As String = ""
            ' 2009/09/15-この行
            Dim errorMessageID As String = ""
            Dim errorMessage As String = ""
            Dim errorToString As String = ""

            '#End Region

            Try
                ' 開始ログの出力
                LogIF.InfoLog("SERVICE-IF", FxLiteral.SIF_STATUS_START)

                '#Region "名前解決"

                ' ★
                status = FxLiteral.SIF_STATUS_NAME_SERVICE

                ' 名前解決（インプロセス）
                WCFHTTPSvcForFx.IPR_NS.NameResolution(serviceName, assemblyName, className)

                '#End Region

                '#Region "引数のデシリアライズ"

                ' ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE

                ' コンテキストクラスの.NETオブジェクト化
                context = BinarySerialize.BytesToObject(contextObject)
                ' 2009/09/29-この行
                ' ※ コンテキストの利用方法は任意だが、サービスインターフェイス上での利用に止める。
                ' 引数クラスの.NETオブジェクト化
                parameterValue = DirectCast(BinarySerialize.BytesToObject(parameterValueObject), BaseParameterValue)

                ' 引数クラスをパラメタ セットに格納
                Dim paramSet As Object() = New Object() {parameterValue, DbEnum.IsolationLevelEnum.User}

                '#End Region

                '#Region "認証処理のＵＯＣ"

                ' ★
                status = FxLiteral.SIF_STATUS_AUTHENTICATION

                If TypeOf context Is String Then
                    ' System.Stringの場合
                    Dim access_token As String = DirectCast(context, String)
                    If Not String.IsNullOrEmpty(access_token) Then
                        Dim [sub] As String = ""
                        Dim roles As List(Of String) = Nothing
                        Dim scopes As List(Of String) = Nothing
                        Dim jobj As JObject = Nothing

                        If JwtToken.Verify(access_token, [sub], roles, scopes, jobj) Then
                            ' 認証成功
                            Debug.WriteLine("認証成功")
                            ' 認証失敗（認証必須ならエラーにする。
                        Else
                        End If
                        ' 認証失敗（認証必須ならエラーにする。
                    Else
                    End If
                Else
                    ' MyUserInfoの場合
                End If

                'contextObject = BinarySerialize.ObjectToBytes(hogehoge); // 更新可能だが...。

                '#End Region

                '#Region "Ｂ層・Ｄ層呼出し"

                ' ★
                status = FxLiteral.SIF_STATUS_INVOKE

                ' #17-start
                Try
                    ' Ｂ層・Ｄ層呼出し
                    'returnValue = (BaseReturnValue)Latebind.InvokeMethod(
                    '    AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + assemblyName + ".dll",
                    '    className, FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME, paramSet);
                    returnValue = DirectCast(Latebind.InvokeMethod(assemblyName, className, FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME, paramSet), BaseReturnValue)
                Catch rtEx As System.Reflection.TargetInvocationException
                    '/ InnerExceptionを投げなおす。
                    'throw rtEx.InnerException;

                    ' スタックトレースを保って InnerException を throw
                    ExceptionDispatchInfo.Capture(rtEx.InnerException).[Throw]()
                End Try
                ' #17-end

                '#End Region

                '#Region "戻り値のシリアライズ"

                ' ★
                status = FxLiteral.SIF_STATUS_SERIALIZE

                returnValueObject = BinarySerialize.ObjectToBytes(returnValue)

                '#End Region

                ' ★
                status = ""

                ' 戻り値を返す。
                Return Nothing
            Catch bsEx As BusinessSystemException
                ' システム例外

                ' エラー情報を設定する。
                wsErrorInfo.ErrorType = FxEnum.ErrorType.BusinessSystemException
                wsErrorInfo.ErrorMessageID = bsEx.messageID
                wsErrorInfo.ErrorMessage = bsEx.Message

                ' ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.BusinessSystemException.ToString()
                ' 2009/09/15-この行
                errorMessageID = bsEx.messageID
                errorMessage = bsEx.Message

                errorToString = bsEx.ToString()

                ' エラー情報を戻す。
                Return BinarySerialize.ObjectToBytes(wsErrorInfo)
            Catch fxEx As FrameworkException
                ' フレームワーク例外
                ' ★ インナーエクセプション情報は消失

                ' エラー情報を設定する。
                wsErrorInfo.ErrorType = FxEnum.ErrorType.FrameworkException
                wsErrorInfo.ErrorMessageID = fxEx.messageID
                wsErrorInfo.ErrorMessage = fxEx.Message

                ' ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.FrameworkException.ToString()
                ' 2009/09/15-この行
                errorMessageID = fxEx.messageID
                errorMessage = fxEx.Message

                errorToString = fxEx.ToString()

                ' エラー情報を戻す。
                Return BinarySerialize.ObjectToBytes(wsErrorInfo)
            Catch ex As Exception
                ' ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.ElseException.ToString()
                ' 2009/09/15-この行
                errorMessageID = "－"
                errorMessage = ex.Message

                errorToString = ex.ToString()

                ' SoapExceptionになって伝播
                Throw
            Finally
                ' Sessionステートレス
                'HttpContext.Current.Session.Clear();
                'HttpContext.Current.Session.Abandon();

                ' 終了ロクの出力
                If status = "" Then
                    ' 終了ログ出力
                    LogIF.InfoLog("SERVICE-IF", "正常終了")
                Else
                    ' 終了ログ出力
                    ' 2009/09/15-この行
                    LogIF.ErrorLog("SERVICE-IF", "異常終了" & "：" & status & vbCr & vbLf &
                                   "エラー タイプ：" & errorType & vbCr & vbLf &
                                   "エラー メッセージID：" & errorMessageID & vbCr & vbLf &
                                   "エラー メッセージ：" & errorMessage & vbCr & vbLf & errorToString)
                End If
            End Try
        End Function

#End Region

    End Class
End Namespace
