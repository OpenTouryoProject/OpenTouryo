'**********************************************************************************
'* �N���X��        �FGlobal
'* �N���X���{�ꖼ  �FGlobal.asax�̃R�[�h �r�n�C���h
'*
'* �쐬����        �F�|
'* �쐬��          �F�|
'* �X�V����        �F�|
'*
'*  ����        �X�V��            ���e
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  �w�w �w�w         �w�w�w�w
'*  2011/12/07  ���� ���         Application_Error��ACCESS���O��ǉ�
'*  2012/04/05  ���� ���         Application_OnPreRequestHandlerExecute
'*                                OnPostRequestHandlerExecute��ACCESS���O��ǉ�
'**********************************************************************************

' System
Imports System.Net.Http
Imports System.Web.Http
Imports System.Web.Optimization

Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Framework.Authentication

''' <summary>Global.asax class </summary>
Public Class [Global]
    Inherits HttpApplication
    '//////////////////////////////////////////////////////////////////////////////
    ' Global_asax�̃����o�ϐ�(�C���X�^���X�ϐ��j�̓X���b�h�Z�[�t
    '//////////////////////////////////////////////////////////////////////////////

    ' �����ɃC���X�^���X�ϐ����`�����ꍇ�A����́A�e�X���b�h�Ɋ��蓖�Ă���B
    ' �̂ɁA�}���`�X���b�h�i���[�U�j��ASP.NET�A�v���P�[�V�����ł��������Ȃ��B
    ' http:// support.microsoft.com/kb/312607/ja

    ' ---

    ' �ÓI�ϐ��̏ꍇ�͋�������B

    ' ASP.NET1.0�A1.1�ł́AApplication�I�u�W�F�N�g�ł͂Ȃ��A�ÓI�ϐ��̎g�p����������Ă������A
    ' ASP.NET2.0�ł́A�ÓI�ϐ����g�p�ł��Ȃ��̂ŁA�ÓI�ϐ��ł͂Ȃ��AApplication�I�u�W�F�N�g��
    ' �g�p����i�������AApplication�I�u�W�F�N�g����������̂Œ��ӂ���j�B

    ''' <summary>���\����</summary>                                                       
    Private perfRec As PerformanceRecorder

    '//////////////////////////////////////////////////////////////////////////////
    ' �C�x���g �n���h��
    '//////////////////////////////////////////////////////////////////////////////

    '////////////////////////////////////////////////
    ' �A�v���P�[�V�����̊J�n�A�I���Ɋւ���C�x���g
    '////////////////////////////////////////////////

    ''' <summary>
    ''' �A�v���P�[�V�����̊J�n�Ɋւ���C�x���g
    ''' </summary>
    Private Sub Application_Start(sender As Object, e As EventArgs)
        ' �A�v���P�[�V�����̃X�^�[�g�A�b�v�Ŏ��s����R�[�h
        ' [!] Startup.Configuration��p�~�A�܂��AMVC�̃e���v���ł́A
        ' [!] OnBeginRequest�ɋL�ڂ���Ă������AWebForms�ɍ��킹�R�`���Ɉړ�
        ' �A�v���P�[�V�����̃X�^�[�g�A�b�v�Ŏ��s����R�[�h�ł�

        AreaRegistration.RegisterAllAreas()

        WebApiConfig.Register(GlobalConfiguration.Configuration)

        ' �O���[�o���t�B���^�̓o�^
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)

        ' URL���[�e�B���O�̓o�^
        RouteConfig.RegisterRoutes(RouteTable.Routes)

        ' �o���h�����~�j�t�B�P�[�V�����̓o�^
        BundleConfig.RegisterBundles(BundleTable.Bundles)

        ' JwkSet�擾�p
        OAuth2AndOIDCClient.HttpClient = New HttpClient()
    End Sub

    ''' <summary>
    ''' �A�v���P�[�V�����̏I���Ɋւ���C�x���g
    ''' </summary>
    Private Sub Application_End(sender As Object, e As EventArgs)
        ' �A�v���P�[�V�����̃V���b�g�_�E���Ŏ��s����R�[�h
    End Sub

    '////////////////////////////////////////////////
    ' �A�v���P�[�V�����̃G���[�Ɋւ���C�x���g
    '////////////////////////////////////////////////

    ''' <summary>
    ''' �A�v���P�[�V�����̃G���[�Ɋւ���C�x���g
    ''' </summary>
    Private Sub Application_Error(sender As Object, e As EventArgs)
        ' �n���h������Ă��Ȃ��G���[�����������Ƃ��Ɏ��s����R�[�h

        Dim ex As Exception = Server.GetLastError().GetBaseException()
        'Server.ClearError(); // Server.GetLastError()���N���A

        ' ACCESS���O�o�� ----------------------------------------------

        ' ------------
        ' ���b�Z�[�W��
        ' ------------
        ' ���[�U��, IP�A�h���X,���C��, 
        ' ��ʖ�, �R���g���[����, ���\�b�h��, ������
        ' �������ԁi���s���ԁj, �������ԁiCPU���ԁj
        ' �G���[���b�Z�[�WID, �G���[���b�Z�[�W��
        ' ------------
        Dim strLogMessage As String = ("," & "�|" & ",") + Request.UserHostAddress & "," & "�|" & "," & "Global.asax" & "," & "Application_Error" & ",,,,," & ex.ToString()

        ' Log4Net�փ��O�o��
        LogIF.FatalLog("ACCESS", strLogMessage)

        ' -------------------------------------------------------------
    End Sub

    '////////////////////////////////////////////////
    ' �Z�b�V�����̊J�n�A�I���Ɋւ���C�x���g
    '////////////////////////////////////////////////

    ''' <summary>
    ''' �Z�b�V�����̊J�n�Ɋւ���C�x���g
    ''' </summary>
    Private Sub Session_Start(sender As Object, e As EventArgs)
        ' �V�K�Z�b�V�������J�n�����Ƃ��Ɏ��s����R�[�h
    End Sub

    ''' <summary>
    ''' �Z�b�V�����̏I���Ɋւ���C�x���g
    ''' </summary>
    Private Sub Session_End(sender As Object, e As EventArgs)
        ' �Z�b�V�������I�������Ƃ��Ɏ��s����R�[�h

        ' Web.config�t�@�C������sessionstate���[�h��[InProc]�ɐݒ肳��Ă���Ƃ��̂݁ASession_End�C�x���g����������B
        ' sessionstate���[�h��[StateServer]���A�܂���[SQLServer]�ɐݒ肳��Ă���ꍇ�A�C�x���g�͔������Ȃ��B

    End Sub

    '//////////////////////////////////////////////////////////////////////////////
    ' ASP.NET�p�C�v���C�������̃C�x���g �n���h��
    '//////////////////////////////////////////////////////////////////////////////

    '////////////////////////////////////////////////

    ' Global.asax���Ή����Ă���ASP.NET�p�C�v���C�������̃C�x���g �n���h���̈ꗗ
    ' -----------------------------------------------------------------------------------
    ' �@ Application_OnBeginRequest                :���N�G�X�g�������J�n����O�ɔ��� 
    ' �A Application_OnAuthenticateRequest         :�F�؂̒��O�ɔ��� 
    ' �B Application_OnAuthorizeRequest            :�F�؂����������^�C�~���O�Ŕ��� 
    ' �C Application_OnResolveRequestCache         :���N�G�X�g���L���b�V���O����^�C�~���O�Ŕ��� 
    ' �D Application_OnAcquireRequestState         :�Z�b�V������ԂȂǂ��擾����^�C�~���O�Ŕ��� 
    ' �E Application_OnPreRequestHandlerExecute    :�y�[�W�̎��s���J�n���钼�O�ɔ��� 
    ' �F Application_OnPostRequestHandlerExecute   :�y�[�W�̎��s��������������ɔ��� 
    ' �G Application_OnReleaseRequestState         :���ׂĂ̏��������������^�C�~���O�Ŕ��� 
    ' �H Application_OnUpdateRequestCache          :�o�̓L���b�V�����X�V�����^�C�~���O�Ŕ��� 
    ' �I Application_OnEndRequest                  :���ׂẴ��N�G�X�g���������������^�C�~���O�Ŕ��� 
    ' �J Application_OnPreSendRequestHeaders       :�w�b�_���N���C�A���g�ɑ��M���钼�O�ɔ��� 
    ' �K Application_OnPreSendRequestContent       :�R���e���c���N���C�A���g�ɑ��M���钼�O�ɔ��� 

    ' �C�x���g�E�n���h���͂��̕\�̏��ԂŌĂяo�����B

    ' �������AApplication_OnPreSendRequestHeaders���\�b�h��
    ' Application_OnPreSendRequestContent���\�b�h��
    ' �o�b�t�@�����iHTTP�����o�b�t�@�����O�j���L�����ǂ����ɂ����
    ' �Ăяo�����^�C�~���O���قȂ�̂Œ��ӂ��邱�ƁB

    ' �o�b�t�@�������L���ł���ꍇ�ɂ́A��L�\�̏��ԂŔ������邪�A
    ' �o�b�t�@�����������ł���ꍇ�ɂ͍ŏ��̃y�[�W�o�͂��J�n�����
    ' �C�ӂ̃^�C�~���O�ŌĂяo�����B

    ' �Ȃ��A���ꂼ��̃C�x���g�E�n���h���̖��O����uApplication_On�v��
    ' ��菜����������Global.asax�Ŕ�������C�x���g�̖��O�ł���B
    ' Global.asax�ł̓C�x���g���ɁuApplication_On�v���邢�́uApplication_�v��t����
    ' �C�x���g�E�n���h�������O�ɒ�`����Ă���A�C�x���g�̔������ɌĂяo�����B     

    ''' <summary>
    ''' �@ ���N�G�X�g�������J�n����O�ɔ���
    ''' </summary>
    Private Sub Application_OnBeginRequest(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �A �F�؂̒��O�ɔ���
    ''' </summary>
    Private Sub Application_OnAuthenticateRequest(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �B �F�؂����������^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnAuthorizeRequest(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �C ���N�G�X�g���L���b�V���O����^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnResolveRequestCache(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �D �Z�b�V������ԂȂǂ��擾����^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnAcquireRequestState(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �E �y�[�W�̎��s���J�n���钼�O�ɔ���
    ''' </summary>
    Private Sub Application_OnPreRequestHandlerExecute(sender As Object, e As EventArgs)
        ' ------------
        ' ���b�Z�[�W��
        ' ------------
        ' ���[�U��, IP�A�h���X, ���C��, 
        ' ��ʖ�, �R���g���[����, ���\�b�h��, ������
        ' ------------
        Dim strLogMessage As String = ("," & "�|" & ",") + Request.UserHostAddress & "," & "-----��" & "," & "Global.asax" & "," & "Application_OnPreRequest"

        ' Log4Net�փ��O�o��
        LogIF.DebugLog("ACCESS", strLogMessage)

        ' -------------------------------------------------------------

        ' ���\����J�n
        Me.perfRec = New PerformanceRecorder()
        Me.perfRec.StartsPerformanceRecord()
    End Sub

    '////////////////////////////////////////////////////////////////
    ' �y�[�W�̎��s���E�`�F�̊Ԃɓ���B
    '////////////////////////////////////////////////////////////////

    ''' <summary>
    ''' �F �y�[�W�̎��s��������������ɔ���
    ''' </summary>
    Private Sub Application_OnPostRequestHandlerExecute(sender As Object, e As EventArgs)
        ' null�`�F�b�N
        ' �Ȃɂ����Ȃ�
        If Me.perfRec Is Nothing Then
        Else
            ' ���\����I��
            Me.perfRec.EndsPerformanceRecord()

            ' ACCESS���O�o��-----------------------------------------------

            ' ------------
            ' ���b�Z�[�W��
            ' ------------
            ' ���[�U��, IP�A�h���X, ���C��, 
            ' ��ʖ�, �R���g���[����, ���\�b�h��, ������
            ' �������ԁi���s���ԁj, �������ԁiCPU���ԁj
            ' ------------
            Dim strLogMessage As String = ("," & "�|" & ",") + Request.UserHostAddress & "," & "-----��" & "," & "Global.asax" & "," & "Application_OnPostRequest" & "," & "�|" & "," & "�|" & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime)

            ' Log4Net�փ��O�o��
            LogIF.DebugLog("ACCESS", strLogMessage)
        End If
    End Sub

    ''' <summary>
    ''' �G ���ׂĂ̏��������������^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnReleaseRequestState(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �H �o�̓L���b�V�����X�V�����^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnUpdateRequestCache(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �I ���ׂẴ��N�G�X�g���������������^�C�~���O�Ŕ���
    ''' </summary>
    Private Sub Application_OnEndRequest(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �J �w�b�_���N���C�A���g�ɑ��M���钼�O�ɔ���
    ''' </summary>
    Private Sub Application_OnPreSendRequestHeaders(sender As Object, e As EventArgs)
    End Sub

    ''' <summary>
    ''' �K �R���e���c���N���C�A���g�ɑ��M���钼�O�ɔ���
    ''' </summary>
    Private Sub Application_OnPreSendRequestContent(sender As Object, e As EventArgs)
    End Sub

End Class
