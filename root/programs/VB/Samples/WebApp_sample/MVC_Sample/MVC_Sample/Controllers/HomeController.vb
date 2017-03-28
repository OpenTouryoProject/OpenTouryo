'**********************************************************************************
'* サンプル アプリ・コントローラ
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：HomeController
'* クラス日本語名  ：認証用サンプル アプリ・コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports MVC_Sample.Models.ViewModels

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util

Namespace Controllers
    ''' <summary>HomeController</summary>
    <Authorize>
    Public Class HomeController
        Inherits MyBaseMVController
        ''' <summary>
        ''' GET: Home
        ''' </summary>
        ''' <returns>ActionResult</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Function Index() As ActionResult
            Return View()
        End Function

        ''' <summary>
        ''' GET: /Home/Login
        ''' </summary>
        ''' <returns>ActionResult</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Function Login() As ActionResult
            ' Session消去
            Me.FxSessionAbandon()

            Return Me.View()
        End Function

        ''' <summary>
        ''' POST: /Home/Login
        ''' </summary>
        ''' <param name="model">LoginViewModel</param>
        ''' <returns>ActionResult</returns>
        <HttpPost>
        <AllowAnonymous>
        <ValidateAntiForgeryToken>
        Public Function Login(model As LoginViewModel) As ActionResult
            If Not String.IsNullOrEmpty(model.UserName) Then
                ' 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                ' 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                ' を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                FormsAuthentication.RedirectFromLoginPage(model.UserName, False)

                ' 認証情報を保存する。
                Dim ui As New MyUserInfo(model.UserName, Request.UserHostAddress)
                UserInfoHandle.SetUserInformation(ui)

                '基盤に任せるのでリダイレクトしない。
                'return this.Redirect(ReturnUrl);
                Return New EmptyResult()
            Else
                ' ユーザー認証 失敗
                Me.ModelState.AddModelError(String.Empty, "指定されたユーザー名またはパスワードが正しくありません。")

                ' Session消去
                Me.FxSessionAbandon()

                ' ポストバック的な
                Return Me.View(model)
            End If
        End Function

        ''' <summary>
        ''' Get: /Home/Scroll
        ''' </summary>
        ''' <returns></returns>
        <HttpGet>
        Public Function Scroll() As ActionResult
            Return Me.View()
        End Function

        ''' <summary>
        ''' Get: /Home/Logout
        ''' </summary>
        ''' <returns></returns>
        <HttpGet>
        Public Function Logout() As ActionResult
            FormsAuthentication.SignOut()
            Return Me.Redirect(Url.Action("Index", "Home"))
        End Function
    End Class
End Namespace
