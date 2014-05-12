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
    /// <summary>WebRole��EntryPoint</summary>
    public class WebRole : RoleEntryPoint
    {
        /// <summary>OnStart�C�x���g</summary>
        public override bool OnStart()
        {
            // Windows Azure�f�f�iWindows Azure Diagnostics�j�֘A�̐ݒ�
            this.ConfigureDiagnostics();

            //// RoleEnvironment.Changed�C�x���g�E�n���h�����d�|����
            //RoleEnvironment.Changed += (sender, args) =>
            //{
            //    // ���[�����̍\���ݒ�̕ύX�����������ꍇ
            //    if (args.Changes.Any(chg => chg is RoleEnvironmentConfigurationSettingChange))
            //    {
            //        // �ύX���ARoleEnvironmentConfigurationSetting�ɂ������ꍇ�A
            //        // �i�g�����\�b�h�iAny�j�{�����_�����g�p���Ă���j
            //        // Windows Azure�f�f�iWindows Azure Diagnostics�j�֘A�́i�āj�ݒ�
            //        this.ConfigureDiagnostics();
            //    }
            //};

            // base.OnStart���Ă�
            return base.OnStart();
        }

        /// <summary>Windows Azure�f�f�iWindows Azure Diagnostics�j�֘A�̐ݒ�</summary>
        private void ConfigureDiagnostics()
        {
            // Windows Azure�f�f�iWindows Azure Diagnostics�j���o�͗p�̃X�g���[�W �A�J�E���g�擾
            string wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

            // �X�g���[�W �A�J�E���g�̏�����
            CloudStorageAccount cloudStorageAccount =
              CloudStorageAccount.Parse(
                RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

            // RoleInstance�f�f�Ǘ��̏������i���[���ɑ΂���f�f�̗L�����j
            RoleInstanceDiagnosticManager roleInstanceDiagnosticManager =
              cloudStorageAccount.CreateRoleInstanceDiagnosticManager(
                RoleEnvironment.DeploymentId,
                RoleEnvironment.CurrentRoleInstance.Role.Name,
                RoleEnvironment.CurrentRoleInstance.Id);

            // RoleInstance�f�f�Ǘ�����R���t�B�O���擾����B
            // �E��{�I�ɂ̓f�t�H���g�ݒ�𗘗p����B
            // �ERoleEnvironment.Changed�C�x���g �n���h�����d�|����ꍇ�̓J�����g�ݒ�𗘗p����B
            DiagnosticMonitorConfiguration config =
                DiagnosticMonitor.GetDefaultInitialConfiguration();
            // roleInstanceDiagnosticManager.GetCurrentConfiguration();

            #region Windows Azure�f�f�iWindows Azure Diagnostics�jAPI���g�p�����ݒ�J�n

            #region �C���t���X�g���N�`�� ���O�i�f�f���j�^���̂̃��O�j
            // �]�����x������ѓ]���Ԋu��ݒ�
            config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // �v����
            config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromSeconds(15); // �v����
            #endregion
            // �o�͐��Table�X�g���[�W���FWADDiagnosticInfrastructureLogsTable

            #region �C�x���g ���O�̐ݒ�
            // �擾����C�x���g �\�[�X��ݒ�
            config.WindowsEventLog.DataSources.Add("Application!*");
            config.WindowsEventLog.DataSources.Add("System!*");
            // �]�����x������ѓ]���Ԋu��ݒ�
            config.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // �v����
            config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(15); // �v����
            #endregion
            // �o�͐��Table�X�g���[�W���FWADWindowsEventLogsTable

            #region �p�t�H�[�}���X �J�E���^�̓]���ݒ�
            // �J�E���^�A�T���v�����O ���[�g�̎w��
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
            // �]���Ԋu��ݒ�
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(15); // �v����
            #endregion
            // �o�͐��Table�X�g���[�W���FWADPerformanceCountersTable

            #region �N���b�V�� �_���v�]���̗L����
            CrashDumps.EnableCollection(true);
            #endregion
            // �o�͐��Blob�X�g���[�W �R���e�i���Fwad-crash-dumps

            #region IIS���O�AFREB���O�̓]���ݒ�(�� web.config �ւ̐ݒ���K�v)
            // IIS���O�̓f�t�H���g�Ŏ擾���L���ƂȂ��Ă��邽�߁ABlob�ւ̓]�����w�肷��݂̂Ŏ��W���\�ƂȂ�B
            // FREB�iFailed Request Trace log�j���O�ɂ��Ă�web.config �ւ̐ݒ���K�v
            #endregion
            // �o�͐��Blob�X�g���[�W �R���e�i���Fwad-iis-logfiles�Awad-iis-failedreqlogfiles

            #region �g���[�X ���O�̐ݒ�(�� *.config �ւ̐ݒ���K�v)
            // �]�����x������ѓ]���Ԋu��ݒ�
            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Undefined; // �v����
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1); // �v����
            #endregion
            // �o�͐��Table�X�g���[�W���FWADLogsTable

            #region �J�X�^�� ���O�i���[�J�� �X�g���[�W�ւ̏o�͂̏ꍇ�j

            // �o�͐�f�B���N�g���ݒ�
            DirectoryConfiguration dirConfig1 = new DirectoryConfiguration()
            {
                // �o�͐�Blob�R���e�i�̎w��
                Container = "my-custom-logfiles1",
                // �N�H�[�^�[�̐ݒ�i���ۂɎg���ʁj
                DirectoryQuotaInMB = 100,
            };

            // ���[�J�� �X�g���[�W�̃p�X���w��
            //�i���[�J�� �X�g���[�W�̐ݒ莩�̂́AVisual Studio���g�p��*.csdef�ɐݒ�\�j
            LocalResource ls = RoleEnvironment.GetLocalResource("LogStorage");
            dirConfig1.Path = ls.RootPath;

            // log4net�Ɋ��ϐ��o�R�Ńp�X���iRootPath�j��
            // �n�����Ƃ������G�~�����[�^��ł��܂����삹���f�O�B

            // ���[�J�� �X�g���[�W��]�����R���N�V�����ɒǉ�
            config.Directories.DataSources.Add(dirConfig1);
            // �Ȃ��A���[�J�� �X�g���[�W�̃p�X�́ALocalResource.RootPath�ɂĎ擾���\�ł���B

            #endregion
            // �o�͐��Blob�X�g���[�W �R���e�i���Fmy-custom-logfiles1

            #region �J�X�^�����O�i�C�ӂ̏o�͐�̏ꍇ�j

            // �o�͐�f�B���N�g���ݒ�
            DirectoryConfiguration dirConfig2 = new DirectoryConfiguration()
            {
                // �o�͐�Blob�R���e�i�̎w��
                Container = "my-custom-logfiles2",
                // �N�H�[�^�[�̐ݒ�i���ۂɎg���ʁj
                DirectoryQuotaInMB = 100,
            };

            // �C�ӂ̃f�B���N�g�����w��
            string path = "c:\\logs";
            dirConfig2.Path = path;
            // �f�B���N�g����]�����R���N�V�����ɒǉ�
            config.Directories.DataSources.Add(dirConfig2);

            // �f�B���N�g�� �Z�L�����e�B���擾
            DirectorySecurity ds = Directory.GetAccessControl(path);

            // Everyone FullControl�̃A�N�Z�X ���[���̐���
            FileSystemAccessRule AccessRule = new FileSystemAccessRule(
                "Everyone",
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            // �f�B���N�g�� �L�����e�B�ɃA�N�Z�X ���[����ǉ�
            ds.AddAccessRule(AccessRule);

            // �f�B���N�g���Ƀf�B���N�g�� �Z�L�����e�B�𔽉f
            // �� <Runtime executionContext="elevated"/>���u*.csdef�v�ɋL�q�B
            Directory.SetAccessControl(path, ds);

            #endregion
            // �o�͐��Blob�X�g���[�W �R���e�i���Fmy-custom-logfiles2

            // IIS���O�A�J�X�^�� ���O�A�N���b�V�� �_���v�Ȃǂ�
            // �g�p����f�B���N�g�� �o�b�t�@����Blob�X�g���[�W�ւ̓]���Ԋu�̎w��
            config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(15);

            #endregion

            // RoleInstance�f�f�Ǘ��ɃR���t�B�O��ݒ�
            roleInstanceDiagnosticManager.SetCurrentConfiguration(config);

            // �f�f�̊J�n�i�G�~�����[�^�ł͕s�v�����A���@�ł͕K�v
            DiagnosticMonitor.Start(wadConnectionString, config);

            // ���[�J�� �X�g���[�W�ւ̃��O�o�̓e�X�g
            path = Path.Combine(ls.RootPath,
                string.Format("test_{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

            // StreamWriter���J���A���O���o��
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("{0} : {1}", DateTime.UtcNow, "���[�J�� �X�g���[�W�ւ̃��O�o�̓e�X�g");
                sw.Close();
            }
        }
    }
}
