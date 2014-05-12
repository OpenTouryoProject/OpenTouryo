Imports System.ServiceModel
Imports Touryo.Infrastructure.Framework.Transmission
Imports Touryo.Infrastructure.Business.Transmission

Module Module1

    Sub Main()
        'Console.ReadKey();

        Using host As New ServiceHost(GetType(WCFTCPSvcForFx))
            host.Open()
            Console.ReadKey()
            host.Close()
        End Using
    End Sub

End Module
