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
'* クラス名        ：IWCFHTTPSvcForFx
'* クラス日本語名  ：WCF HTTP Webサービス インターフェイス（サービス インターフェイス基盤）
'*                   SOAPの.NETオブジェクトのバイナリ転送用メソッドを公開する。
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2012/12/14  西野 大介         新規作成
'**********************************************************************************

Imports System.ServiceModel

Namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
    ' メモ: [リファクター] メニューの [名前の変更] コマンドを使用すると、コードと config ファイルの両方で同時にインターフェイス名 "IWCFSvcForFx" を変更できます。

    ''' <summary>
    ''' WCF HTTP Webサービス（サービス インターフェイス基盤）
    ''' .NET言語用のWebメソッド（.NETオブジェクトI/F）を公開する。
    ''' </summary>
    <ServiceContract()> _
    Public Interface IWCFHTTPSvcForFx
        ''' <summary>
        ''' ASP.NET Webサービスを使用した
        ''' サービス インターフェイス基盤（.NETオンライン）
        ''' </summary>
        ''' <param name="serviceName">サービス名</param>
        ''' <param name="contextObject">コンテキスト</param>
        ''' <param name="parameterValueObject">引数</param>
        ''' <param name="returnValueObject">戻り値</param>
        ''' <returns>返すべきエラーの情報</returns>
        ''' <remarks>値は全て.NETオブジェクトをバイナリシリアライズしたバイト配列データ</remarks>
        <OperationContract()> _
        Function DotNETOnlineWS(serviceName As String, ByRef contextObject As Byte(), parameterValueObject As Byte(), ByRef returnValueObject As Byte()) As Byte()
    End Interface
End Namespace
