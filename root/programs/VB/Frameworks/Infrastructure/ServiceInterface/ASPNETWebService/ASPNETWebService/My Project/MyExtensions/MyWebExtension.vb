#If _MyType <> "Empty" Then

Namespace My
    ''' <summary>
    ''' Web プロジェクトの My Namespace で使用できるプロパティを定義するためのモジュール。
    ''' </summary>
    ''' <remarks></remarks>
    <Global.Microsoft.VisualBasic.HideModuleName()> _
    Module MyWebExtension
        Private s_Computer As New ThreadSafeObjectProvider(Of Global.Microsoft.VisualBasic.Devices.ServerComputer)
        Private s_User As New ThreadSafeObjectProvider(Of Global.Microsoft.VisualBasic.ApplicationServices.WebUser)
        Private s_Log As New ThreadSafeObjectProvider(Of Global.Microsoft.VisualBasic.Logging.AspLog)
        ''' <summary>
        ''' ホスト コンピューターに関する情報を返します。
        ''' </summary>
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
        Friend ReadOnly Property Computer() As Global.Microsoft.VisualBasic.Devices.ServerComputer
            Get
                Return s_Computer.GetInstance()
            End Get
        End Property
        ''' <summary>
        ''' 現在の Web ユーザーに関する情報を返します。
        ''' </summary>
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
        Friend ReadOnly Property User() As Global.Microsoft.VisualBasic.ApplicationServices.WebUser
            Get
                Return s_User.GetInstance()
            End Get
        End Property
        ''' <summary>
        ''' Request オブジェクトを返します。
        ''' </summary>
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
        <Global.System.ComponentModel.Design.HelpKeyword("My.Request")> _
        Friend ReadOnly Property Request() As Global.System.Web.HttpRequest
            <Global.System.Diagnostics.DebuggerHidden()> _
            Get
                Dim CurrentContext As Global.System.Web.HttpContext = Global.System.Web.HttpContext.Current
                If CurrentContext IsNot Nothing Then
                    Return CurrentContext.Request
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Response オブジェクトを返します。
        ''' </summary>
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
         <Global.System.ComponentModel.Design.HelpKeyword("My.Response")> _
         Friend ReadOnly Property Response() As Global.System.Web.HttpResponse
            <Global.System.Diagnostics.DebuggerHidden()> _
            Get
                Dim CurrentContext As Global.System.Web.HttpContext = Global.System.Web.HttpContext.Current
                If CurrentContext IsNot Nothing Then
                    Return CurrentContext.Response
                End If
                Return Nothing
            End Get
        End Property
        ''' <summary>
        ''' Asp ログ オブジェクトを返します。
        ''' </summary>
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
        Friend ReadOnly Property Log() As Global.Microsoft.VisualBasic.Logging.AspLog
            Get
                Return s_Log.GetInstance()
            End Get
        End Property
     End Module
End Namespace

#End If