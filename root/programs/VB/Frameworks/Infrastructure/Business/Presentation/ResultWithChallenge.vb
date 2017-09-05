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
'* クラス名        ：ResultWithChallenge
'* クラス日本語名  ：ResultWithChallenge（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2017/08/11  西野 大介         新規作成
'**********************************************************************************

Imports System.Threading
Imports System.Threading.Tasks

Imports System.Web.Http
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers

Namespace Touryo.Infrastructure.Business.Presentation

    ''' <summary>IHttpActionResult</summary>
    Public Class ResultWithChallenge
        Implements IHttpActionResult
        ''' <summary>IHttpActionResult</summary>
        Private ReadOnly [next] As IHttpActionResult

        ''' <summary>ResultWithChallenge</summary>
        ''' <param name="next">IHttpActionResult</param>
        Public Sub New([next] As IHttpActionResult)
            Me.[next] = [next]
        End Sub

        ''' <summary>ExecuteAsync</summary>
        ''' <param name="cancellationToken">CancellationToken</param>
        ''' <returns>HttpResponseMessage</returns>
        Public Async Function ExecuteAsync(cancellationToken As CancellationToken) As Task(Of HttpResponseMessage) Implements IHttpActionResult.ExecuteAsync
            Dim response As HttpResponseMessage = Await [next].ExecuteAsync(cancellationToken)
            If response.StatusCode = HttpStatusCode.Unauthorized Then
                response.Headers.WwwAuthenticate.Add(New AuthenticationHeaderValue("somescheme", "somechallenge"))
            End If
            Return response
        End Function
    End Class
End Namespace
