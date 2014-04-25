# Open ���� Visual Studio2013 �e���v���[�g�E�x�[�X�̗��p���@(How to Use OpenTouryo Visual Studio 2013 template base)


Open���� Visual Studio2013 �e���v���[�g�E�x�[�X��
���������T���v���̎��s�菇�͉��L�̂Ƃ���ł��B

(Execution procedure of the samples that are shipped with the
OpenTouryo Visual Studio 2013 template base is as follows.)

* �u/root_VS2013_TemplateBase/�v�ȉ��̃t�H���_���uC:\root�v�t�H���_�ȉ��ɔz�u���܂��B
   (Deploy to under �uC:\root�vfolder from under �u/root_VS2013_TemplateBase/�vfolder.)
   
* Visual Studio 2013 �� SQL Server �̃C���X�g�[��
   (Installing Visual Studio 2013 and SQL Server.)
   
* �T���v��DB�̏���(Prepare Sample DB)

   - ���L����_�E�����[�h���C���X�g�[�����܂��B
      (Download and install from the following.)
      
      Download: NorthWind and pubs Sample Databases for SQL Server 2000 - Microsoft Download Center - Download Details
      http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=23654
      
   - ���L�R�}���h�����s���܂�(Run the following command)�B
      "C:\Program Files\Microsoft SQL Server\100\Tools\Binn\SQLCMD.EXE" -S localhost\SQLExpress -E -i "C:\SQL Server 2000 Sample Databases\instnwnd.sql"

* �Z�b�V������ԃT�[�r�X�̏���(Preparing the session state service)
   - �Ǘ��҂Ƃ��ăR�}���h�v�����v�g���N�����A���L�R�}���h�����s���܂��B
      (Start a command prompt as an administrator, and then run the following command.)
      
      sc config aspnet_state start= auto
      net start aspnet_state

* �v���O�����̃r���h(Building the program)
   C:\root_VS2010_TemplateBase\programs\C#
   C:\root_VS2010_TemplateBase\programs\VB

   �t�H���_�ȉ��̃r���h�o�b�`��ԍ����Ɏ��s���ăv���O�������r���h���܂��B
   �K�v�ł���΁A���ɍ��킹�āAz_Common.bat����BUILDFILEPATH�����������܂��B
   
   (Build the program by running in numerical order the build batch of Above folder. 
   And then, if necessary, for your environment, you can rewrite the BUILDFILEPATH of z_Common.bat within.)

* VS2013��WebSite�ŉ��z�p�X�̃��[�g�Ƀv���W�F�N�g����
   ����Ȃ��Ȃ������ƂɋN�����Ĉȉ��̑Ή����K�v�ɂȂ�܂����B
   
   (The project name lost from root of the virtual path in the WebSite of VS2013.
   I now need to be addressed in the following.)
   
   e.g.: �v���W�F�N�g�� (project name) = ProjectX_sample
         - before version Visual Studio 2012 -> http://localhost:9999/ProjectX_sample/Aspx/start/menu.aspx
         - Visual Studio 2013 -> http://localhost:9999/Aspx/start/menu.aspx
         
   asp.net - Upgrade VS2012 project to VS2013 web site and it won't let me specify IIS port - Stack Overflow
   http://stackoverflow.com/questions/19635467/upgrade-vs2012-project-to-vs2013-web-site-and-it-wont-let-me-specify-iis-port
   
   - �O�̂��߁Aaspnet_regiis�����s���܂��B
      CMD���Ǘ��҃��[�h�ŋN�����āuaspnet_regiis.exe - i�v�R�}���h�����s���܂��B
      
      (Just in case, Run the aspnet_regiis. 
       You can run the "aspnet_regiis.exe - i" command to launch in administrator mode the CMD.)
       
      C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe - i
  
   - �v���W�F�N�g�̃��[�g�E�t�H���_��IIS�̉��z�f�B���N�g���ɐݒ肵�܂��i�G�C���A�X�FProjectX_sample�j�B
      (Set the virtual directory in IIS to "root folder of the project" (alias: ProjectX_sample).)
      
      \root\programs\VB\Samples\WebApp_sample\ProjectX_sample
  
   - VS2013���u�Ǘ��҂Ƃ��Ď��s�v�ŋN�����āA
      �v���W�F�N�g�E�t�H���_��������WebSite�Ƃ���HTTP-IIS����J���܂��B
      �v���W�F�N�g���f�o�b�O���s���A���z�p�X�̃��[�g�Ƀv���W�F�N�g�������鎖���m�F���܂��B
      
      (Start with "Run as Administrator" on VS2013, 
      I open it from the HTTP-IIS WebSite as an existing project folder. 
      The debug run the project to see that the project name to enter the root of the virtual path.)
       
   - SQL Server�ւ̐ڑ��̎b��΍�Ƃ��ċU�������܂��B
      (Impersonate as an interim measure for a connection to SQL Server.)
      
      <!-- �U������ꍇ�͈ȉ���L���ɂ���(Enable the following: If you want to impersonate) -->
      <identity impersonate="true" userName="xxxx" password="yyyy" />
      
      ���̑Ή����@�Ƃ��āASQL Server�F�؂�L���ɂ��Ă��ǂ��B
      (In response other methods, it is also possible to enable the SQL Server authentication.)
      
* �T���v���̎��s(Running the Sample)

   ���L�t�@�C�����J�����s����iVB�ł͈ꕔ�̒񋟂ɂȂ��Ă��܂��j�B
   ���O�C����ʂ��o���ꍇ�́A�p�X���[�h�̊m�F�͍s���Ă��Ȃ����߁A�C�ӂ̐�������͂��Ă��������B
   
   (Open and run the following file (VB version provide some). 
   If the login screen appears, because not check the password, please enter the number of any.)
   
   - Web �̏ꍇ(In the case of Web)�F
      - ASP.NET
         C:\root\programs\C#\Samples\WebApp_sample\ProjectX_sample\ProjectX_sample.sln
      - ASP.NET MVC
         C:\\root\programs\C#\Samples\WebApp_sample\MVC_Sample\MVC_Sample.sln
    
   - C/S 2�K�w�̏ꍇ(In the case of two-tier C/S)�F
      - Windows Forms
         C:\root\programs\C#\Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln
      - WPF
         C:\root\programs\C#\Samples\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln
    
   - C/S 3�K�w�̏ꍇ(In the case of three-tier C/S)�F
      - Windows Forms
         C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln
         C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWinCone_sample\WSClientWinCone_sample.sln
      - WPF
         C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWPF_sample\WSClientWPF_sample.sln
         C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWPFbap_sample\WSClientWPFbap_sample.sln
    
   - Silverlight �̏�(In the case of Silverlight)�F
      C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientSL_samples\WSClientSL_samples.sln
   - Windows�X�g�A�A�v�� �̏ꍇ(In the case of Windows Store App )�F
      C:\root\programs\C#\Samples\WS_sample\WSClient_sample\WSClientWinStore_samples\WSClientWinStore_samples.sln
   - Windows Azure �̏ꍇ(In the case of Windows Azure)�F
      C:\root_org\programs\C#\Samples\WinAzure_sample\WinAzure_sample.sln

* �e�`���[�g���A���̓��e�ɏ]��Open�����̕]�����\�ł��B
   (Evaluation of OpenTouryo is possible in accordance with the contents of each tutorial.)
   
   C:\documents\5_Tutorial\
   
* �܂��A�e���v���[�g�E�x�[�X���`���[�g���A���̓��e�ɏ]���J�X�^�}�C�Y���邱�ƂŁA
   ���YVisual Studio�o�[�W�����̈Č������v���W�F�N�g�E�e���v���[�g���쐬�ł��܂��B 
   
   (Further, You  will customize template base according to the contents of the tutorial, 
   You can create project template for the Visual Studio version for the project.)
    
   C:\documents\5_Tutorial\Tutorial_Template_development.doc
   
# ���쌠�A���C�Z���X(Copyright, license)

[License]�f�B���N�g�����m�F�������B
(Please check [License] directory.)

# �o�O�Ή�(Bug fix)

�o�O�̔�����ʒm���������ꍇ�A�ʒm�̑Ó����̊m�F��A
�o�b�N���O�ɉ������C�ӂ̃^�C�~���O�Ńt�B�b�N�X����܂��B

�o�O�C���p�b�`�̎捞�݂́A�ŐV�Ŏ捞�݂ɂ���������܂��B
�Ⴕ���́A���Y�o�O���g���b�L���O�E�c�[���ォ��m�F����
���|�W�g������o�O�t�B�b�N�X����DIFF���擾���e���}�[�W���Ă��������B

If there is a notification or discovery of the bug,
after confirmation of the validity of the notification, 
It will be fixed at any time be added to the backlog. 

Incorporation of bug fixes are implemented by the latest version of incorporation. 
Or, check from the tracking tool on the bug 
Please have your own merge by get the DIFF of bug fixes from the repository at the time.

# �f�[�^�v���o�C�_�̓���A�A�o�葱���A�g�p�����ւ̓Y�t�ɂ���(obtain the data provider. export procedures. attach to license.)


Open�����ł́A��X�̃f�[�^�E�v���o�C�_���T�|�[�g���Ă��܂����A
�e�f�[�^�E�v���o�C�_�̓���E�A�o�葱���Ɋւ��ẮA�e���Ή��������B

The Open Touryo is support the data provider of various, 
For information on obtaining and export procedures for each data provider, please support their own.
