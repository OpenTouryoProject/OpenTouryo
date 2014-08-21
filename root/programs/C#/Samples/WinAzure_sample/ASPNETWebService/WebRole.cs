using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics.Management;

using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;

namespace ASPNETWebService
{
    /// <summary>WebRoleのEntryPoint</summary>
    public class WebRole : RoleEntryPoint
    {
        /// <summary>OnStartイベント</summary>
        public override bool OnStart()
        {
            // Windows Azure診断（Windows Azure Diagnostics）関連の設定
            this.ConfigureDiagnostics();

            //// RoleEnvironment.Changedイベント・ハンドラを仕掛ける
            //RoleEnvironment.Changed += (sender, args) =>
            //{
            //    // ロール環境の構成設定の変更が発生した場合
            //    if (args.Changes.Any(chg => chg is RoleEnvironmentConfigurationSettingChange))
            //    {
            //        // 変更が、RoleEnvironmentConfigurationSettingにあった場合、
            //        // （拡張メソッド（Any）＋ラムダ式を使用している）
            //        // Windows Azure診断（Windows Azure Diagnostics）関連の（再）設定
            //        this.ConfigureDiagnostics();
            //    }
            //};

            // base.OnStartを呼ぶ
            return base.OnStart();
        }

        /// <summary>Windows Azure診断（Windows Azure Diagnostics）関連の設定</summary>
        private void ConfigureDiagnostics()
        {
            // Windows Azure診断（Windows Azure Diagnostics）情報出力用のストレージ アカウント取得
            string wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

            //// ストレージ アカウントの初期化
            //CloudStorageAccount cloudStorageAccount =
            //  CloudStorageAccount.Parse(
            //    RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

            //// RoleInstance診断管理の初期化（ロールに対する診断の有効化）
            //RoleInstanceDiagnosticManager roleInstanceDiagnosticManager =
            //  cloudStorageAccount.CreateRoleInstanceDiagnosticManager(
            //    RoleEnvironment.DeploymentId,
            //    RoleEnvironment.CurrentRoleInstance.Role.Name,
            //    RoleEnvironment.CurrentRoleInstance.Id);

            var storageConnectionString = RoleEnvironment.GetConfigurationSettingValue(
                   wadConnectionString);
            var deploymentDiagnosticManager = new DeploymentDiagnosticManager(
                                                  storageConnectionString,
                                                  RoleEnvironment.DeploymentId);
            RoleInstanceDiagnosticManager roleInstanceDiagnosticManager = deploymentDiagnosticManager.GetRoleInstanceDiagnosticManager(
                                    RoleEnvironment.CurrentRoleInstance.Role.Name,
                                    RoleEnvironment.CurrentRoleInstance.Id);

            // RoleInstance診断管理からコンフィグを取得する。
            // ・基本的にはデフォルト設定を利用する。
            // ・RoleEnvironment.Changedイベント ハンドラを仕掛ける場合はカレント設定を利用する。
            DiagnosticMonitorConfiguration config =
                DiagnosticMonitor.GetDefaultInitialConfiguration();
            // roleInstanceDiagnosticManager.GetCurrentConfiguration();

            #region Windows Azure診断（Windows Azure Diagnostics）APIを使用した設定開始

            #region インフラストラクチャ ログ（診断モニタ自体のログ）
            // 転送レベルおよび転送間隔を設定
            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // 要検討
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromSeconds(15); // 要検討
            #endregion
            // 出力先のTableストレージ名：WADDiagnosticInfrastructureLogsTable

            #region イベント ログの設定
            // 取得するイベント ソースを設定
            config.WindowsEventLog.DataSources.Add("Application!*");
            config.WindowsEventLog.DataSources.Add("System!*");
            // 転送レベルおよび転送間隔を設定
            config.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // 要検討
            config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(15); // 要検討
            #endregion
            // 出力先のTableストレージ名：WADWindowsEventLogsTable

            #region パフォーマンス カウンタの転送設定
            // カウンタ、サンプリング レートの指定
            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = TimeSpan.FromSeconds(10)
            });
            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Memory\Available Bytes",
                SampleRate = TimeSpan.FromSeconds(10)
            });
            // 転送間隔を設定
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(15); // 要検討
            #endregion
            // 出力先のTableストレージ名：WADPerformanceCountersTable

            #region クラッシュ ダンプ転送の有効化
            CrashDumps.EnableCollection(true);
            #endregion
            // 出力先のBlobストレージ コンテナ名：wad-crash-dumps

            #region IISログ、FREBログの転送設定(※ web.config への設定も必要)
            // IISログはデフォルトで取得が有効となっているため、Blobへの転送を指定するのみで収集が可能となる。
            // FREB（Failed Request Trace log）ログについてはweb.config への設定も必要
            #endregion
            // 出力先のBlobストレージ コンテナ名：wad-iis-logfiles、wad-iis-failedreqlogfiles

            #region トレース ログの設定(※ *.config への設定も必要)
            // 転送レベルおよび転送間隔を設定
            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // 要検討
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1); // 要検討
            #endregion
            // 出力先のTableストレージ名：WADLogsTable

            #region カスタム ログ（ローカル ストレージへの出力の場合）

            // 出力先ディレクトリ設定
            DirectoryConfiguration dirConfig1 = new DirectoryConfiguration()
            {
                // 出力先Blobコンテナの指定
                Container = "my-custom-logfiles1",
                // クォーターの設定（実際に使う量）
                DirectoryQuotaInMB = 100,
            };

            // ローカル ストレージのパスを指定
            //（ローカル ストレージの設定自体は、Visual Studioを使用し*.csdefに設定可能）
            LocalResource ls = RoleEnvironment.GetLocalResource("LogStorage");
            dirConfig1.Path = ls.RootPath;

            // log4netに環境変数経由でパス情報（RootPath）を
            // 渡そうとしたがエミュレータ上でうまく動作せず断念。

            // ローカル ストレージを転送元コレクションに追加
            config.Directories.DataSources.Add(dirConfig1);
            // なお、ローカル ストレージのパスは、LocalResource.RootPathにて取得が可能である。

            #endregion
            // 出力先のBlobストレージ コンテナ名：my-custom-logfiles1

            #region カスタムログ（任意の出力先の場合）

            // 出力先ディレクトリ設定
            DirectoryConfiguration dirConfig2 = new DirectoryConfiguration()
            {
                // 出力先Blobコンテナの指定
                Container = "my-custom-logfiles2",
                // クォーターの設定（実際に使う量）
                DirectoryQuotaInMB = 100,
            };

            // 任意のディレクトリを指定
            string path = "c:\\logs";
            dirConfig2.Path = path;
            // ディレクトリを転送元コレクションに追加
            config.Directories.DataSources.Add(dirConfig2);

            // ディレクトリ セキュリティを取得
            DirectorySecurity ds = Directory.GetAccessControl(path);

            // Everyone FullControlのアクセス ルールの生成
            FileSystemAccessRule AccessRule = new FileSystemAccessRule(
                "Everyone",
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            // ディレクトリ キュリティにアクセス ルールを追加
            ds.AddAccessRule(AccessRule);

            // ディレクトリにディレクトリ セキュリティを反映
            // ★ <Runtime executionContext="elevated"/>を「*.csdef」に記述。
            Directory.SetAccessControl(path, ds);

            #endregion
            // 出力先のBlobストレージ コンテナ名：my-custom-logfiles2

            // IISログ、カスタム ログ、クラッシュ ダンプなどで
            // 使用するディレクトリ バッファからBlobストレージへの転送間隔の指定
            config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(15);

            #endregion

            // RoleInstance診断管理にコンフィグを設定
            roleInstanceDiagnosticManager.SetCurrentConfiguration(config);

            // 診断の開始（エミュレータでは不要だが、実機では必要
            DiagnosticMonitor.Start(wadConnectionString, config);

            // ローカル ストレージへのログ出力テスト
            path = Path.Combine(ls.RootPath,
                string.Format("test_{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

            // StreamWriterを開き、ログを出力
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("{0} : {1}", DateTime.UtcNow, "ローカル ストレージへのログ出力テスト");
                sw.Close();
            }
        }
    }
}
