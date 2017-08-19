Imports System.Web.Http
Imports System.Web.Mvc
Imports System.Web.Optimization

Namespace ASPNETWebService

    Public Class Global_asax
        Inherits HttpApplication

        Sub Application_Start(sender As Object, e As EventArgs)
            ' アプリケーションの起動時に呼び出されます
            'AreaRegistration.RegisterAllAreas()
            'GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
            'RouteConfig.RegisterRoutes(RouteTable.Routes)
            'BundleConfig.RegisterBundles(BundleTable.Bundles)
        End Sub
    End Class

End Namespace