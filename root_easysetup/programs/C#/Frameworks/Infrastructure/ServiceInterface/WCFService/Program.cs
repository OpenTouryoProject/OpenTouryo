using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Business.Transmission;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadKey();

            using (ServiceHost host = new ServiceHost(typeof(WCFTCPSvcForFx)))
            {
                host.Open();
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
