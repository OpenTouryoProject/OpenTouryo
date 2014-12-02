using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Touryo.Infrastructure.Framework.ServiceInterface.AsyncProcessingService
{
    [System.ComponentModel.RunInstaller(true)]
    public class ServiceInstaller : System.Configuration.Install.Installer
    {
        static void main()
        {

        }

        /// <summary>
        ///    Required designer variable.
        /// </summary>
        //private System.ComponentModel.Container components;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller
                serviceProcessInstaller;

        public ServiceInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller =
              new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.Description = "My Windows Service description";
            this.serviceInstaller.DisplayName = "AsyncProcessingService";
            this.serviceInstaller.ServiceName = "AsyncProcessingService";
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account =
              System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});

        }
    }
}
