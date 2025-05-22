//**********************************************************************************
//* �e���v���[�g
//**********************************************************************************

// �T���v�����̃e���v���[�g�Ȃ̂ŁA�K�v�ɉ����Ďg�p���ĉ������B

//**********************************************************************************
//* �N���X��        �FGlobal
//* �N���X���{�ꖼ  �FGlobal.asax�̃R�[�h �r�n�C���h
//*
//* �쐬����        �F�|
//* �쐬��          �F�|
//* �X�V����        �F�|
//*
//*  ����        �X�V��            ���e
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  �w�w �w�w         �w�w�w�w
//*  2011/12/07  ���� ���         Application_Error��ACCESS���O��ǉ�
//*  2012/04/05  ���� ���         Application_OnPreRequestHandlerExecute
//*                                OnPostRequestHandlerExecute��ACCESS���O��ǉ�
//**********************************************************************************

// System
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

using System.Web.Routing;

// OpenTouryo
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Framework.Authentication;

namespace MVC_Sample
{
    /// <summary>Global.asax class </summary>
    public class Global : System.Web.HttpApplication
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Global_asax�̃����o�ϐ�(�C���X�^���X�ϐ��j�̓X���b�h�Z�[�t
        /////////////////////////////////////////////////////////////////////////////////

        // �����ɃC���X�^���X�ϐ����`�����ꍇ�A����́A�e�X���b�h�Ɋ��蓖�Ă���B
        // �̂ɁA�}���`�X���b�h�i���[�U�j��ASP.NET�A�v���P�[�V�����ł��������Ȃ��B
        // http:// support.microsoft.com/kb/312607/ja

        // ---

        // �ÓI�ϐ��̏ꍇ�͋�������B

        // ASP.NET1.0�A1.1�ł́AApplication�I�u�W�F�N�g�ł͂Ȃ��A�ÓI�ϐ��̎g�p����������Ă������A
        // ASP.NET2.0�ł́A�ÓI�ϐ����g�p�ł��Ȃ��̂ŁA�ÓI�ϐ��ł͂Ȃ��AApplication�I�u�W�F�N�g��
        // �g�p����i�������AApplication�I�u�W�F�N�g����������̂Œ��ӂ���j�B

        /// <summary>���\����</summary>                                                       
        private PerformanceRecorder perfRec;

        /////////////////////////////////////////////////////////////////////////////////
        // �C�x���g �n���h��
        /////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////
        // �A�v���P�[�V�����̊J�n�A�I���Ɋւ���C�x���g
        ///////////////////////////////////////////////////

        /// <summary>
        /// �A�v���P�[�V�����̊J�n�Ɋւ���C�x���g
        /// </summary>
        void Application_Start(object sender, EventArgs e)
        {
            // �A�v���P�[�V�����̃X�^�[�g�A�b�v�Ŏ��s����R�[�h
            // [!] Startup.Configuration��p�~�A�܂��AMVC�̃e���v���ł́A
            // [!] OnBeginRequest�ɋL�ڂ���Ă������AWebForms�ɍ��킹�R�`���Ɉړ�

            //
            AreaRegistration.RegisterAllAreas();

            // 
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // �O���[�o���t�B���^�̓o�^
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // URL���[�e�B���O�̓o�^
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // �o���h�����~�j�t�B�P�[�V�����̓o�^
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // JwkSet�擾�p
            OAuth2AndOIDCClient.HttpClient = new HttpClient();
        }

        /// <summary>
        /// �A�v���P�[�V�����̏I���Ɋւ���C�x���g
        /// </summary>
        void Application_End(object sender, EventArgs e)
        {
            // �A�v���P�[�V�����̃V���b�g�_�E���Ŏ��s����R�[�h
        }

        ///////////////////////////////////////////////////
        // �A�v���P�[�V�����̃G���[�Ɋւ���C�x���g
        ///////////////////////////////////////////////////

        /// <summary>
        /// �A�v���P�[�V�����̃G���[�Ɋւ���C�x���g
        /// </summary>
        void Application_Error(object sender, EventArgs e)
        {
            // �n���h������Ă��Ȃ��G���[�����������Ƃ��Ɏ��s����R�[�h

            Exception ex = Server.GetLastError().GetBaseException();
            //Server.ClearError(); // Server.GetLastError()���N���A

            // ACCESS���O�o�� ----------------------------------------------

            // ------------
            // ���b�Z�[�W��
            // ------------
            // ���[�U��, IP�A�h���X,���C��, 
            // ��ʖ�, �R���g���[����, ���\�b�h��, ������
            // �������ԁi���s���ԁj, �������ԁiCPU���ԁj
            // �G���[���b�Z�[�WID, �G���[���b�Z�[�W��
            // ------------
            string strLogMessage =
                "," + "�|" +
                "," + Request.UserHostAddress +
                "," + "�|" +
                "," + "Global.asax" +
                "," + "Application_Error" +
                ",,,,," + ex.ToString();

            // Log4Net�փ��O�o��
            LogIF.FatalLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------
        }

        ///////////////////////////////////////////////////
        // �Z�b�V�����̊J�n�A�I���Ɋւ���C�x���g
        ///////////////////////////////////////////////////

        /// <summary>
        /// �Z�b�V�����̊J�n�Ɋւ���C�x���g
        /// </summary>
        void Session_Start(object sender, EventArgs e)
        {
            // �V�K�Z�b�V�������J�n�����Ƃ��Ɏ��s����R�[�h
        }

        /// <summary>
        /// �Z�b�V�����̏I���Ɋւ���C�x���g
        /// </summary>
        void Session_End(object sender, EventArgs e)
        {
            // �Z�b�V�������I�������Ƃ��Ɏ��s����R�[�h

            // Web.config�t�@�C������sessionstate���[�h��[InProc]�ɐݒ肳��Ă���Ƃ��̂݁ASession_End�C�x���g����������B
            // sessionstate���[�h��[StateServer]���A�܂���[SQLServer]�ɐݒ肳��Ă���ꍇ�A�C�x���g�͔������Ȃ��B

        }

        /////////////////////////////////////////////////////////////////////////////////
        // ASP.NET�p�C�v���C�������̃C�x���g �n���h��
        /////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////

        // Global.asax���Ή����Ă���ASP.NET�p�C�v���C�������̃C�x���g �n���h���̈ꗗ
        // -----------------------------------------------------------------------------------
        // �@ Application_OnBeginRequest                :���N�G�X�g�������J�n����O�ɔ��� 
        // �A Application_OnAuthenticateRequest         :�F�؂̒��O�ɔ��� 
        // �B Application_OnAuthorizeRequest            :�F�؂����������^�C�~���O�Ŕ��� 
        // �C Application_OnResolveRequestCache         :���N�G�X�g���L���b�V���O����^�C�~���O�Ŕ��� 
        // �D Application_OnAcquireRequestState         :�Z�b�V������ԂȂǂ��擾����^�C�~���O�Ŕ��� 
        // �E Application_OnPreRequestHandlerExecute    :�y�[�W�̎��s���J�n���钼�O�ɔ��� 
        // �F Application_OnPostRequestHandlerExecute   :�y�[�W�̎��s��������������ɔ��� 
        // �G Application_OnReleaseRequestState         :���ׂĂ̏��������������^�C�~���O�Ŕ��� 
        // �H Application_OnUpdateRequestCache          :�o�̓L���b�V�����X�V�����^�C�~���O�Ŕ��� 
        // �I Application_OnEndRequest                  :���ׂẴ��N�G�X�g���������������^�C�~���O�Ŕ��� 
        // �J Application_OnPreSendRequestHeaders       :�w�b�_���N���C�A���g�ɑ��M���钼�O�ɔ��� 
        // �K Application_OnPreSendRequestContent       :�R���e���c���N���C�A���g�ɑ��M���钼�O�ɔ��� 

        // �C�x���g�E�n���h���͂��̕\�̏��ԂŌĂяo�����B

        // �������AApplication_OnPreSendRequestHeaders���\�b�h��
        // Application_OnPreSendRequestContent���\�b�h��
        // �o�b�t�@�����iHTTP�����o�b�t�@�����O�j���L�����ǂ����ɂ����
        // �Ăяo�����^�C�~���O���قȂ�̂Œ��ӂ��邱�ƁB

        // �o�b�t�@�������L���ł���ꍇ�ɂ́A��L�\�̏��ԂŔ������邪�A
        // �o�b�t�@�����������ł���ꍇ�ɂ͍ŏ��̃y�[�W�o�͂��J�n�����
        // �C�ӂ̃^�C�~���O�ŌĂяo�����B

        // �Ȃ��A���ꂼ��̃C�x���g�E�n���h���̖��O����uApplication_On�v��
        // ��菜����������Global.asax�Ŕ�������C�x���g�̖��O�ł���B
        // Global.asax�ł̓C�x���g���ɁuApplication_On�v���邢�́uApplication_�v��t����
        // �C�x���g�E�n���h�������O�ɒ�`����Ă���A�C�x���g�̔������ɌĂяo�����B     

        /// <summary>
        /// �@ ���N�G�X�g�������J�n����O�ɔ���
        /// </summary>
        void Application_OnBeginRequest(object sender, EventArgs e)
        {
            // Application_Start�Ɉړ�
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// �A �F�؂̒��O�ɔ���
        /// </summary>
        void Application_OnAuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �B �F�؂����������^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnAuthorizeRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �C ���N�G�X�g���L���b�V���O����^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnResolveRequestCache(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �D �Z�b�V������ԂȂǂ��擾����^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnAcquireRequestState(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �E �y�[�W�̎��s���J�n���钼�O�ɔ���
        /// </summary>
        void Application_OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            // ------------
            // ���b�Z�[�W��
            // ------------
            // ���[�U��, IP�A�h���X, ���C��, 
            // ��ʖ�, �R���g���[����, ���\�b�h��, ������
            // ------------
            string strLogMessage =
                "," + "�|" +
                "," + Request.UserHostAddress +
                "," + "-----��" +
                "," + "Global.asax" +
                "," + "Application_OnPreRequest";

            // Log4Net�փ��O�o��
            LogIF.DebugLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // ���\����J�n
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        ///////////////////////////////////////////////////////////////////
        // �y�[�W�̎��s���E�`�F�̊Ԃɓ���B
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// �F �y�[�W�̎��s��������������ɔ���
        /// </summary>
        void Application_OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            // null�`�F�b�N
            if (this.perfRec == null)
            {
                // �Ȃɂ����Ȃ�
            }
            else
            {
                // ���\����I��
                this.perfRec.EndsPerformanceRecord();

                // ACCESS���O�o��-----------------------------------------------

                // ------------
                // ���b�Z�[�W��
                // ------------
                // ���[�U��, IP�A�h���X, ���C��, 
                // ��ʖ�, �R���g���[����, ���\�b�h��, ������
                // �������ԁi���s���ԁj, �������ԁiCPU���ԁj
                // ------------
                string strLogMessage =
                    "," + "�|" +
                    "," + Request.UserHostAddress +
                    "," + "-----��" +
                    "," + "Global.asax" +
                    "," + "Application_OnPostRequest" +
                    "," + "�|" +
                    "," + "�|" +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime;

                // Log4Net�փ��O�o��
                LogIF.DebugLog("ACCESS", strLogMessage);
            }
        }

        /// <summary>
        /// �G ���ׂĂ̏��������������^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnReleaseRequestState(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �H �o�̓L���b�V�����X�V�����^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnUpdateRequestCache(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �I ���ׂẴ��N�G�X�g���������������^�C�~���O�Ŕ���
        /// </summary>
        void Application_OnEndRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �J �w�b�_���N���C�A���g�ɑ��M���钼�O�ɔ���
        /// </summary>
        void Application_OnPreSendRequestHeaders(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// �K �R���e���c���N���C�A���g�ɑ��M���钼�O�ɔ���
        /// </summary>
        void Application_OnPreSendRequestContent(object sender, EventArgs e)
        {
        }

    }
}
