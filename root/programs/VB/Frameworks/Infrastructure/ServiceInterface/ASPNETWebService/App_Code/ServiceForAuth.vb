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
'* クラス名        ：ServiceForAuth
'* クラス日本語名  ：認証サービスを公開する。
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2011/xx/xx  西野 大介         新規作成
'*  2012/08/25  西野 大介         Assembly.LoadFile → .Load（ASP.NETシャドウコピー対応）
'**********************************************************************************

Imports System.IO
Imports System.Text
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.EnterpriseServices

' System
Imports System
Imports System.Xml
Imports System.Data
Imports System.Collections

' System.Web
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Common
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

Namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService

	' 名前空間は、必要に応じて書き換え下さい。

	''' <summary>
	''' 認証サービスを公開する。
	''' </summary>
	<WebService([Namespace] := FxLiteral.WS_NAME_SPACE)> _
	<WebServiceBinding(ConformsTo := WsiProfiles.BasicProfile1_1)> _
	Public Class ServiceForAuth
		Inherits System.Web.Services.WebService
		#Region "コンストラクタ"

				'デザインされたコンポーネントを使用する場合、次の行をコメントを解除してください 
				'InitializeComponent(); 
		Public Sub New()
		End Sub

		#End Region

        ' IIS - ASP.NET（ワーカ プロセス）間のバッファリング
        ' キャッシュ無効化
        ' Webサービスの説明
        ' Sessionのサポート
        ' オーバーロード時の識別用エイリアス
        ' トランザクション サポート
		''' <summary>チャレンジを返す</summary>
		''' <returns>チャレンジ</returns>
		<WebMethod(BufferResponse := True, CacheDuration := 0, Description := "Authentication Service(GetChallenge)", EnableSession := True, MessageName := "", TransactionOption := TransactionOption.NotSupported)> _
		Public Function GetChallenge() As String
			' チャレンジ
			Session("challenge") = Guid.NewGuid().ToString()
			Return DirectCast(Session("challenge"), String)
		End Function

        ' IIS - ASP.NET（ワーカ プロセス）間のバッファリング
        ' キャッシュ無効化
        ' Webサービスの説明
        ' Sessionのサポート
        ' オーバーロード時の識別用エイリアス
        ' トランザクション サポート
		''' <summary>認証チケットを返す</summary>
		''' <param name="encUid">チャレンジで暗号化されたユーザID</param>
		''' <param name="encPwd">チャレンジで暗号化されたパスワード</param>
		''' <returns>認証チケット</returns>
		<WebMethod(BufferResponse := True, CacheDuration := 0, Description := "Authentication Service(GetAuthTicket)", EnableSession := True, MessageName := "", TransactionOption := TransactionOption.NotSupported)> _
		Public Function GetAuthTicket(encUid As String, encPwd As String) As String
			Try
				' ユーザIDの復号化
				Dim uid As String = SymmetricCryptography.DecryptString(encUid, DirectCast(Session("challenge"), String), EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)
				' パスワードの復号化
				Dim pwd As String = SymmetricCryptography.DecryptString(encPwd, DirectCast(Session("challenge"), String), EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)

				' 認証する。
				Dim isAuthenticated As Boolean = False

				'#Region "認証処理のＵＯＣ"

				' ★★　コンテキストの情報を使用するなどして
				'       認証処理をＵＯＣする（必要に応じて）。

                '/ Ｂ層・Ｄ層呼出し
                '/   認証チェックとタイムスタンプの更新
				'MyUserInfo userInfo =new MyUserInfo(
				'    "未認証：" + uid, HttpContext.Current.Request.UserHostAddress);

				'BaseReturnValue returnValue = (BaseReturnValue)Latebind.InvokeMethod(
                '    "xxxx", "yyyy",
				'    FxLiteral.TRANSMISSION_INPROCESS_METHOD_NAME,
				'    new object[] {
				'        new AuthParameterValue("－", "－", "zzzz", "",userInfo, pwd),
				'        DbEnum.IsolationLevelEnum.User });

                '/ 認証されたか・されなかったか
				'isAuthenticated = !returnValue.ErrorFlag;

				isAuthenticated = True

				'#End Region

				If isAuthenticated Then
					' 認証チケットを作成して暗号化する（DateTime.Nowにより可変に）。
					Dim authTicket As String() = {uid, pwd, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}

					Return SymmetricCryptography.EncryptString(CustomEncode.ToBase64String(BinarySerialize.ObjectToBytes(authTicket)), GetConfigParameter.GetConfigValue("private-key"), EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)
				Else
					' 認証失敗
					Return String.Empty
				End If
			Catch
				' 認証失敗
				Return String.Empty
			Finally
				' セッションの解放
				Session.Abandon()
			End Try
		End Function

        ' IIS - ASP.NET（ワーカ プロセス）間のバッファリング
        ' キャッシュ無効化
        ' Webサービスの説明
        ' Sessionのサポート
        ' オーバーロード時の識別用エイリアス
        ' トランザクション サポート
		''' <summary>認証チケットを検証する。</summary>
        ''' <param name="authTicket">認証チケット（暗号化）</param>
		''' <returns>認証チケット（復号化）</returns>
		''' <remarks>注：テスト用です。</remarks>
		<WebMethod(BufferResponse := True, CacheDuration := 0, Description := "Authentication Service(ValidateAuthTicket)", EnableSession := False, MessageName := "", TransactionOption := TransactionOption.NotSupported)> _
		Public Function ValidateAuthTicket(authTicket As String) As String()
			Try
				' 認証チケットの復号化
				Return DirectCast(BinarySerialize.BytesToObject(CustomEncode.FromBase64String(SymmetricCryptography.DecryptString(authTicket, GetConfigParameter.GetConfigValue("private-key"), EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider))), String())
			Catch
				' 認証失敗
				Return Nothing
			End Try
		End Function
	End Class
End Namespace
