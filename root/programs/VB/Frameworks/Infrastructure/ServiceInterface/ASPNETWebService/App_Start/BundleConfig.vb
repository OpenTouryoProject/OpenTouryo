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
'* クラス名        ：BundleConfig
'* クラス日本語名  ：バンドル＆ミニフィケーションに関する指定
'*
'* 作成者          ：玄人 幸道
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2026/08/17  玄人 幸道         新規作成（#558）C#版に合わせて追加。
'**********************************************************************************

Imports System.Web.Optimization

Namespace ASPNETWebService
    ''' <summary>バンドル＆ミニフィケーションに関する指定</summary>
    Public Class BundleConfig
        ''' <summary>バンドルの登録</summary>
        ''' <param name="bundles">BundleCollection</param>
        ''' <remarks>
        ''' バンドルの詳細については、https://go.microsoft.com/fwlink/?LinkId=301862 を参照。
        ''' このプロジェクトは WebAPI のみで画面を持たないため、登録するものは無い。
        ''' </remarks>
        Public Shared Sub RegisterBundles(bundles As BundleCollection)
        End Sub
    End Class
End Namespace
