'**********************************************************************************
'* �T���v�� �o�b�`
'**********************************************************************************

'**********************************************************************************
'* �N���X��        �FModule1
'* �N���X���{�ꖼ  �F�T���v�� �o�b�`
'*
'* �쐬����        �F�|
'* �쐬��          �Fsas ���Z
'* �X�V����        �F
'*
'*  ����        �X�V��            ���e
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  �w�w �w�w         �w�w�w�w
'**********************************************************************************

' �^���
Imports SimpleBatch_sample.Common
Imports SimpleBatch_sample.Business

' System
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Collections
Imports System.Collections.Generic

' �Ɩ��t���[�����[�N
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' �t���[�����[�N
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' ���i
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>Module1</summary>
Module Module1

    ''' <summary>Main</summary>
    Sub Main()
        '***********************************************************************
        '* �ȑf�ȃT���v���Ȃ̂ŁA
        '* �E���d���i�^�X�N���A���ʃZ�b�g�𕪊��j
        '* �E�t�F�b�`�E�T�C�Y�i����������ʂ�}����j
        '* �E�R�~�b�g�E�C���^�[�o���A������
        '* ���̍l�����ʓr�K�v�ɂȂ邱�Ƃ�����܂��B
        '***********************************************************************

        ' �R�}���h���C�����o�����֐�������B
        Dim valsLst As List(Of String) = Nothing
        Dim argsDic As Dictionary(Of String, String) = Nothing

        PubCmnFunction.GetCommandArgs("/"c, argsDic, valsLst)

        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue( _
            System.Reflection.Assembly.GetExecutingAssembly().Location, _
            "-", "SelectCount", _
            argsDic("/DAP") & "%" & _
            argsDic("/MODE1") & "%" & _
            argsDic("/MODE2") & "%" & _
            argsDic("/EXROLLBACK"), _
            New MyUserInfo("", ""))

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �a�w�ďo��
        Dim layerB As New LayerB()
        testReturnValue = DirectCast(layerB.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            Dim [error] As String = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            [error] += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            [error] += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf

            Console.WriteLine([error])
            Console.ReadKey()
        Else
            ' ���ʁi����n�j
            Console.WriteLine(testReturnValue.Obj.ToString() & "���̃f�[�^������܂�")
            Console.ReadKey()
        End If
    End Sub

End Module
