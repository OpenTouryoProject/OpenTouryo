//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* �N���X��        �FEventLogWin32
//* �N���X���{�ꖼ  �F�C�x���g���O�֘AWin32 API�錾�N���X
//*
//* �쐬��          �F���Z ����
//* �X�V����        �F
//*
//*  ����        �X�V��            ���e
//*  ----------  ----------------  -------------------------------------------------
//*  2012/09/21  ����  ���        �V�K�쐬
//*  2013/02/18  ����  ���        SetLastError�Ή�
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    class EventLogWin32
    {
        /// <summary>�C�x���g���O�̓o�^�ς݃n���h����Ԃ��܂��B</summary>
        /// <param name="lpUNCServerName">
        /// ��������s����T�[�o�̖��O
        /// �iNULL���w��Ń��[�J���R���s���[�^�j
        /// </param>
        /// <param name="lpSourceName">
        /// �o�^�ς݃n���h�����擾����C�x���g�\�[�X�̖��O
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Application
        /// </param>
        /// <returns>
        /// �C�x���g���O�̓o�^�ς݃n���h��
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr RegisterEventSource(string lpUNCServerName, string lpSourceName);

        /// <summary>�w�肵���C�x���g���O�̃n���h������܂��B</summary>
        /// <param name="hEventLog">
        /// �C�x���g���O�̓o�^�ς݃n���h��
        /// �iRegisterEventSource �֐����Ԃ��n���h���j
        /// </param>
        /// <returns>
        /// true�F�֐�������
        /// false�F�֐������s
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DeregisterEventSource(IntPtr hEventLog);

        /// <summary>�w�肵���C�x���g���O�̍Ō�ɃG���g�����������݂܂��B</summary>
        /// <param name="hEventLog">�C�x���g���O�̃n���h��</param>
        /// <param name="wType">���O�ɏ������ރC�x���g�̎��</param>
        /// <param name="wCategory">�C�x���g�̕���</param>
        /// <param name="dwEventID">�C�x���g���ʎq</param>
        /// <param name="lpUserSid">���[�U�[�Z�L�����e�B���ʎq</param>
        /// <param name="wNumStrings">���b�Z�[�W�Ƀ}�[�W���镶����̐�</param>
        /// <param name="dwDataSize">�o�C�i���f�[�^�̃T�C�Y�i�o�C�g���j</param>
        /// <param name="lpStrings">���b�Z�[�W�Ƀ}�[�W���镶����̔z��</param>
        /// <param name="lpRawData">�o�C�i���f�[�^�̃A�h���X</param>
        /// <returns>
        /// true�F�֐�������
        /// false�F�֐������s
        /// </returns>
        [DllImport("advapi32.dll", EntryPoint = "ReportEventW", CharSet = CharSet.Unicode, SetLastError=true)]
        public static extern bool ReportEvent(
            IntPtr hEventLog,
            ushort wType,
            ushort wCategory,
            int dwEventID,
            IntPtr lpUserSid,
            ushort wNumStrings,
            int dwDataSize,
            string[] lpStrings,
            IntPtr lpRawData);
    }
}
