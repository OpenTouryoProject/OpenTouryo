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
//* クラス名        ：WinProcWin32
//* クラス日本語名  ：メッセージ・ループ関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/02/18  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    /// <summary>
    /// メッセージ・ループ関連Win32 API宣言クラス
    /// </summary>
    public class WinProcWin32
    {
        #region 定義

        #region 定数

        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632637.aspx
        /// </summary>
        public const int WM_NULL = 0x0000;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632619.aspx
        /// </summary>
        public const int WM_CREATE = 0x0001;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632620.aspx
        /// </summary>
        public const int WM_DESTROY = 0x0002;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632631.aspx
        /// </summary>
        public const int WM_MOVE = 0x0003;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632646.aspx
        /// </summary>
        public const int WM_SIZE = 0x0005;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646274.aspx
        /// </summary>
        public const int WM_ACTIVATE = 0x0006;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646283.aspx
        /// </summary>
        public const int WM_SETFOCUS = 0x0007;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646282.aspx
        /// </summary>
        public const int WM_KILLFOCUS = 0x0008;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632621.aspx
        /// </summary>
        public const int WM_ENABLE = 0x000A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145219.aspx
        /// </summary>
        public const int WM_SETREDRAW = 0x000B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632644.aspx
        /// </summary>
        public const int WM_SETTEXT = 0x000C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632627.aspx
        /// </summary>
        public const int WM_GETTEXT = 0x000D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632628.aspx
        /// </summary>
        public const int WM_GETTEXTLENGTH = 0x000E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145213.aspx
        /// </summary>
        public const int WM_PAINT = 0x000F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632617.aspx
        /// </summary>
        public const int WM_CLOSE = 0x0010;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa376890.aspx
        /// </summary>
        public const int WM_QUERYENDSESSION = 0x0011;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632640.aspx
        /// </summary>
        public const int WM_QUERYOPEN = 0x0013;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa376889.aspx
        /// </summary>
        public const int WM_ENDSESSION = 0x0016;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632641.aspx
        /// </summary>
        public const int WM_QUIT = 0x0012;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms648055.aspx
        /// </summary>
        public const int WM_ERASEBKGND = 0x0014;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145223.aspx
        /// </summary>
        public const int WM_SYSCOLORCHANGE = 0x0015;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632645.aspx
        /// </summary>
        public const int WM_SHOWWINDOW = 0x0018;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms725499.aspx
        /// </summary>
        public const int WM_WININICHANGE = 0x001A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms725497.aspx
        /// </summary>
        public const int WM_SETTINGCHANGE = WM_WININICHANGE;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145209.aspx
        /// </summary>
        public const int WM_DEVMODECHANGE = 0x001B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632614.aspx
        /// </summary>
        public const int WM_ACTIVATEAPP = 0x001C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145211.aspx
        /// </summary>
        public const int WM_FONTCHANGE = 0x001D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms725498.aspx
        /// </summary>
        public const int WM_TIMECHANGE = 0x001E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632615.aspx
        /// </summary>
        public const int WM_CANCELMODE = 0x001F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms648382.aspx
        /// </summary>
        public const int WM_SETCURSOR = 0x0020;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645612.aspx
        /// </summary>
        public const int WM_MOUSEACTIVATE = 0x0021;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632616.aspx
        /// </summary>
        public const int WM_CHILDACTIVATE = 0x0022;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644972.aspx
        /// </summary>
        public const int WM_QUEUESYNC = 0x0023;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632626.aspx
        /// </summary>
        public const int WM_GETMINMAXINFO = 0x0024;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_PAINTICON = 0x0026;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_ICONERASEBKGND = 0x0027;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645432.aspx
        /// </summary>
        public const int WM_NEXTDLGCTL = 0x0028;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145220.aspx
        /// </summary>
        public const int WM_SPOOLERSTATUS = 0x002A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb775923.aspx
        /// </summary>
        public const int WM_DRAWITEM = 0x002B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb775925.aspx
        /// </summary>
        public const int WM_MEASUREITEM = 0x002C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761362.aspx
        /// </summary>
        public const int WM_DELETEITEM = 0x002D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761364.aspx
        /// </summary>
        public const int WM_VKEYTOITEM = 0x002E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761358.aspx
        /// </summary>
        public const int WM_CHARTOITEM = 0x002F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632642.aspx
        /// </summary>
        public const int WM_SETFONT = 0x0030;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632624.aspx
        /// </summary>
        public const int WM_GETFONT = 0x0031;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646284.aspx
        /// </summary>
        public const int WM_SETHOTKEY = 0x0032;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646278.aspx
        /// </summary>
        public const int WM_GETHOTKEY = 0x0033;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632639.aspx
        /// </summary>
        public const int WM_QUERYDRAGICON = 0x0037;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb775921.aspx
        /// </summary>
        public const int WM_COMPAREITEM = 0x0039;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd373892.aspx
        /// </summary>
        public const int WM_GETOBJECT = 0x003D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632618.aspx
        /// </summary>
        public const int WM_COMPACTING = 0x0041;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_COMMNOTIFY = 0x0044;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632653.aspx
        /// </summary>
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632652.aspx
        /// </summary>
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa373245.aspx
        /// </summary>
        public const int WM_POWER = 0x0048;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649011.aspx
        /// </summary>
        public const int WM_COPYDATA = 0x004A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644971.aspx
        /// </summary>
        public const int WM_CANCELJOURNAL = 0x004B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb775583.aspx
        /// </summary>
        public const int WM_NOTIFY = 0x004E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632630.aspx
        /// </summary>
        public const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632629.aspx
        /// </summary>
        public const int WM_INPUTLANGCHANGE = 0x0051;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb774307.aspx
        /// </summary>
        public const int WM_TCARD = 0x0052;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb774305.aspx
        /// </summary>
        public const int WM_HELP = 0x0053;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632651.aspx
        /// </summary>
        public const int WM_USERCHANGED = 0x0054;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb775584.aspx
        /// </summary>
        public const int WM_NOTIFYFORMAT = 0x0055;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647592.aspx
        /// </summary>
        public const int WM_CONTEXTMENU = 0x007B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632649.aspx
        /// </summary>
        public const int WM_STYLECHANGING = 0x007C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632648.aspx
        /// </summary>
        public const int WM_STYLECHANGED = 0x007D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145210.aspx
        /// </summary>
        public const int WM_DISPLAYCHANGE = 0x007E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632625.aspx
        /// </summary>
        public const int WM_GETICON = 0x007F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632643.aspx
        /// </summary>
        public const int WM_SETICON = 0x0080;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632635.aspx
        /// </summary>
        public const int WM_NCCREATE = 0x0081;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632636.aspx
        /// </summary>
        public const int WM_NCDESTROY = 0x0082;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632634.aspx
        /// </summary>
        public const int WM_NCCALCSIZE = 0x0083;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645618.aspx
        /// </summary>
        public const int WM_NCHITTEST = 0x0084;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145212.aspx
        /// </summary>
        public const int WM_NCPAINT = 0x0085;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632633.aspx
        /// </summary>
        public const int WM_NCACTIVATE = 0x0086;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645425.aspx
        /// </summary>
        public const int WM_GETDLGCODE = 0x0087;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145221.aspx
        /// </summary>
        public const int WM_SYNCPAINT = 0x0088;
        /// <summary></summary>
        public const int WM_NCMOUSEMOVE = 0x00A0;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645627.aspx
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645621.aspx
        /// </summary>
        public const int WM_NCLBUTTONUP = 0x00A2;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645619.aspx
        /// </summary>
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645620.aspx
        /// </summary>
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645630.aspx
        /// </summary>
        public const int WM_NCRBUTTONUP = 0x00A5;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645628.aspx
        /// </summary>
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645620.aspx
        /// </summary>
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645621.aspx
        /// </summary>
        public const int WM_NCMBUTTONUP = 0x00A8;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645622.aspx
        /// </summary>
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645620.aspx
        /// </summary>
        public const int WM_NCXBUTTONDOWN = 0x00AB;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645621.aspx
        /// </summary>
        public const int WM_NCXBUTTONUP = 0x00AC;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645619.aspx
        /// </summary>
        public const int WM_NCXBUTTONDBLCLK = 0x00AD;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645591.aspx
        /// </summary>
        public const int WM_INPUT_DEVICE_CHANGE = 0x00FE;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645590.aspx
        /// </summary>
        public const int WM_INPUT = 0x00FF;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms912655.aspx
        /// </summary>
        public const int WM_KEYFIRST = 0x0100;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/gg153546.aspx
        /// </summary>
        public const int WM_KEYDOWN = 0x0100;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646281.aspx
        /// </summary>
        public const int WM_KEYUP = 0x0101;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646276.aspx
        /// </summary>
        public const int WM_CHAR = 0x0102;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646277.aspx
        /// </summary>
        public const int WM_DEADCHAR = 0x0103;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646286.aspx
        /// </summary>
        public const int WM_SYSKEYDOWN = 0x0104;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646287.aspx
        /// </summary>
        public const int WM_SYSKEYUP = 0x0105;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646357.aspx
        /// </summary>
        public const int WM_SYSCHAR = 0x0106;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646285.aspx
        /// </summary>
        public const int WM_SYSDEADCHAR = 0x0107;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646288.aspx
        /// </summary>
        public const int WM_UNICHAR = 0x0109;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa453875.aspx
        /// _WIN32_WINNT >= 0x0501（Windows XP）
        /// </summary>
        public const int WM_KEYLAST = 0x0109;
        ///// <summary></summary>
        //public  const int WM_KEYLAST = 0x0108;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374136.aspx
        /// </summary>
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374133.aspx
        /// </summary>
        public const int WM_IME_COMPOSITION = 0x010F;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_IME_KEYLAST = 0x010F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645428.aspx
        /// </summary>
        public const int WM_INITDIALOG = 0x0110;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647591.aspx
        /// </summary>
        public const int WM_COMMAND = 0x0111;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646360.aspx
        /// </summary>
        public const int WM_SYSCOMMAND = 0x0112;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644902.aspx
        /// </summary>
        public const int WM_TIMER = 0x0113;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb787575.aspx
        /// </summary>
        public const int WM_HSCROLL = 0x0114;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb787577.aspx
        /// </summary>
        public const int WM_VSCROLL = 0x0115;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646344.aspx
        /// </summary>
        public const int WM_INITMENU = 0x0116;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646347.aspx
        /// </summary>
        public const int WM_INITMENUPOPUP = 0x0117;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd353242.aspx
        /// </summary>
        public const int WM_GESTURE = 0x0119;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd353243.aspx
        /// </summary>
        public const int WM_GESTURENOTIFY = 0x011A;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_MENUSELECT = 0x011F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646352.aspx
        /// </summary>
        public const int WM_MENUCHAR = 0x0120;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645422.aspx
        /// </summary>
        public const int WM_ENTERIDLE = 0x0121;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647610.aspx
        /// </summary>
        public const int WM_MENURBUTTONUP = 0x0122;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647606.aspx
        /// </summary>
        public const int WM_MENUDRAG = 0x0123;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647607.aspx
        /// </summary>
        public const int WM_MENUGETOBJECT = 0x0124;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647614.aspx
        /// </summary>
        public const int WM_UNINITMENUPOPUP = 0x0125;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647603.aspx
        /// </summary>
        public const int WM_MENUCOMMAND = 0x0126;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646342.aspx
        /// </summary>
        public const int WM_CHANGEUISTATE = 0x0127;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646361.aspx
        /// </summary>
        public const int WM_UPDATEUISTATE = 0x0128;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646355.aspx
        /// </summary>
        public const int WM_QUERYUISTATE = 0x0129;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_CTLCOLORMSGBOX = 0x0132;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761691.aspx
        /// </summary>
        public const int WM_CTLCOLOREDIT = 0x0133;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761360.aspx
        /// </summary>
        public const int WM_CTLCOLORLISTBOX = 0x0134;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761849.aspx
        /// </summary>
        public const int WM_CTLCOLORBTN = 0x0135;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645417.aspx
        /// </summary>
        public const int WM_CTLCOLORDLG = 0x0136;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb787573.aspx
        /// </summary>
        public const int WM_CTLCOLORSCROLLBAR = 0x0137;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb787524.aspx
        /// </summary>
        public const int WM_CTLCOLORSTATIC = 0x0138;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632613.aspx
        /// </summary>
        public const int MN_GETHMENU = 0x01E1;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_MOUSEFIRST = 0x0200;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645616.aspx
        /// </summary>
        public const int WM_MOUSEMOVE = 0x0200;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645607.aspx
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x0201;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645608.aspx
        /// </summary>
        public const int WM_LBUTTONUP = 0x0202;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645606.aspx
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x0203;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646242.aspx
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x0204;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646243.aspx
        /// </summary>
        public const int WM_RBUTTONUP = 0x0205;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646241.aspx
        /// </summary>
        public const int WM_RBUTTONDBLCLK = 0x0206;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645610.aspx
        /// </summary>
        public const int WM_MBUTTONDOWN = 0x0207;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645611.aspx
        /// </summary>
        public const int WM_MBUTTONUP = 0x0208;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645606.aspx
        /// </summary>
        public const int WM_MBUTTONDBLCLK = 0x0209;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645617.aspx
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646245.aspx
        /// </summary>
        public const int WM_XBUTTONDOWN = 0x020B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645608.aspx
        /// </summary>
        public const int WM_XBUTTONUP = 0x020C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646244.aspx
        /// </summary>
        public const int WM_XBUTTONDBLCLK = 0x020D;
        /// <summary>
        /// 
        /// _WIN32_WINNT >= 0x0500（Windows 2000）
        /// </summary>
        public const int WM_MOUSELAST = 0x020D;
        ///// <summary></summary>
        //public  const int WM_MOUSELAST = 0x020A;
        ///// <summary></summary>
        //public  const int WM_MOUSELAST = 0x0209;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/hh454920.aspx
        /// </summary>
        public const int WM_PARENTNOTIFY = 0x0210;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647595.aspx
        /// </summary>
        public const int WM_ENTERMENULOOP = 0x0211;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647599.aspx
        /// </summary>
        public const int WM_EXITMENULOOP = 0x0212;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms647592.aspx
        /// </summary>
        public const int WM_NEXTMENU = 0x0213;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632647.aspx
        /// </summary>
        public const int WM_SIZING = 0x0214;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645605.aspx
        /// </summary>
        public const int WM_CAPTURECHANGED = 0x0215;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632632.aspx
        /// </summary>
        public const int WM_MOVING = 0x0216;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa373247.aspx
        /// </summary>
        public const int WM_POWERBROADCAST = 0x0218;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa363480.aspx
        /// </summary>
        public const int WM_DEVICECHANGE = 0x0219;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644913.aspx
        /// </summary>
        public const int WM_MDICREATE = 0x0220;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644914.aspx
        /// </summary>
        public const int WM_MDIDESTROY = 0x0221;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644911.aspx
        /// </summary>
        public const int WM_MDIACTIVATE = 0x0222;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644920.aspx
        /// </summary>
        public const int WM_MDIRESTORE = 0x0223;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644918.aspx
        /// </summary>
        public const int WM_MDINEXT = 0x0224;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644917.aspx
        /// </summary>
        public const int WM_MDIMAXIMIZE = 0x0225;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644922.aspx
        /// </summary>
        public const int WM_MDITILE = 0x0226;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644912.aspx
        /// </summary>
        public const int WM_MDICASCADE = 0x0227;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644916.aspx
        /// </summary>
        public const int WM_MDIICONARRANGE = 0x0228;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644915.aspx
        /// </summary>
        public const int WM_MDIGETACTIVE = 0x0229;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644921.aspx
        /// </summary>
        public const int WM_MDISETMENU = 0x0230;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632622.aspx
        /// </summary>
        public const int WM_ENTERSIZEMOVE = 0x0231;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632623.aspx
        /// </summary>
        public const int WM_EXITSIZEMOVE = 0x0232;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb774303.aspx
        /// </summary>
        public const int WM_DROPFILES = 0x0233;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644919.aspx
        /// </summary>
        public const int WM_MDIREFRESHMENU = 0x0234;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374142.aspx
        /// </summary>
        public const int WM_IME_SETCONTEXT = 0x0281;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374139.aspx
        /// </summary>
        public const int WM_IME_NOTIFY = 0x0282;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374135.aspx
        /// </summary>
        public const int WM_IME_CONTROL = 0x0283;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374134.aspx
        /// </summary>
        public const int WM_IME_COMPOSITIONFULL = 0x0284;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374141.aspx
        /// </summary>
        public const int WM_IME_SELECT = 0x0285;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374132.aspx
        /// </summary>
        public const int WM_IME_CHAR = 0x0286;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374140.aspx
        /// </summary>
        public const int WM_IME_REQUEST = 0x0288;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374137.aspx
        /// </summary>
        public const int WM_IME_KEYDOWN = 0x0290;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd374138.aspx
        /// </summary>
        public const int WM_IME_KEYUP = 0x0291;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645613.aspx
        /// </summary>
        public const int WM_MOUSEHOVER = 0x02A1;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645615.aspx
        /// </summary>
        public const int WM_MOUSELEAVE = 0x02A3;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645625.aspx
        /// </summary>
        public const int WM_NCMOUSEHOVER = 0x02A0;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms645626.aspx
        /// </summary>
        public const int WM_NCMOUSELEAVE = 0x02A2;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa383828.aspx
        /// </summary>
        public const int WM_WTSSESSION_CHANGE = 0x02B1;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_TABLET_FIRST = 0x02c0;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_TABLET_LAST = 0x02df;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649023.aspx
        /// </summary>
        public const int WM_CUT = 0x0300;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649022.aspx
        /// </summary>
        public const int WM_COPY = 0x0301;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649028.aspx
        /// </summary>
        public const int WM_PASTE = 0x0302;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649020.aspx
        /// </summary>
        public const int WM_CLEAR = 0x0303;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/bb761693.aspx
        /// </summary>
        public const int WM_UNDO = 0x0304;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649030.aspx
        /// </summary>
        public const int WM_RENDERFORMAT = 0x0305;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649029.aspx
        /// </summary>
        public const int WM_RENDERALLFORMATS = 0x0306;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649024.aspx
        /// </summary>
        public const int WM_DESTROYCLIPBOARD = 0x0307;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649025.aspx
        /// </summary>
        public const int WM_DRAWCLIPBOARD = 0x0308;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649027.aspx
        /// </summary>
        public const int WM_PAINTCLIPBOARD = 0x0309;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649032.aspx
        /// </summary>
        public const int WM_VSCROLLCLIPBOARD = 0x030A;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649031.aspx
        /// </summary>
        public const int WM_SIZECLIPBOARD = 0x030B;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649018.aspx
        /// </summary>
        public const int WM_ASKCBFORMATNAME = 0x030C;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649019.aspx
        /// </summary>
        public const int WM_CHANGECBCHAIN = 0x030D;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms649026.aspx
        /// </summary>
        public const int WM_HSCROLLCLIPBOARD = 0x030E;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145218.aspx
        /// </summary>
        public const int WM_QUERYNEWPALETTE = 0x030F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145215.aspx
        /// </summary>
        public const int WM_PALETTEISCHANGING = 0x0310;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145214.aspx
        /// </summary>
        public const int WM_PALETTECHANGED = 0x0311;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646279.aspx
        /// </summary>
        public const int WM_HOTKEY = 0x0312;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145216.aspx
        /// </summary>
        public const int WM_PRINT = 0x0317;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd145217.aspx
        /// </summary>
        public const int WM_PRINTCLIENT = 0x0318;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646275.aspx
        /// </summary>
        public const int WM_APPCOMMAND = 0x0319;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms632650.aspx
        /// </summary>
        public const int WM_THEMECHANGED = 0x031A;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_HANDHELDFIRST = 0x0358;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_HANDHELDLAST = 0x035F;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_AFXFIRST = 0x0360;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_AFXLAST = 0x037F;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_PENWINFIRST = 0x0380;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_PENWINLAST = 0x038F;
        /// <summary>
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms644930.aspx
        /// </summary>
        public const int WM_APP = 0x8000;

        #endregion

        #region 構造体・列挙型

        /// <summary>
        /// MSG 構造体型
        /// http://msdn.microsoft.com/ja-jp/library/vstudio/900ks98t.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            /// <summary>
            /// ウィンドウ プロシージャがメッセージを受け取るウィンドウを指定します。
            /// </summary>
            public IntPtr hWnd;

            /// <summary>
            /// メッセージ番号を指定します。
            /// </summary>
            public int msg;

            /// <summary>
            /// メッセージに関する追加情報を指定します。 厳密な意味は message のメンバーの値によって異なります。
            /// </summary>
            public IntPtr wParam;

            /// <summary>
            /// メッセージに関する追加情報を指定します。 厳密な意味は message のメンバーの値によって異なります。
            /// </summary>
            public IntPtr lParam;

            /// <summary>
            /// メッセージがポストされた時間を指定します。
            /// </summary>
            public int time;

            /// <summary>
            /// メッセージがポストされたときに、カーソル位置を画面座標で指定します。
            /// </summary>
            public POINT pt;
        }

        /// <summary>
        /// wRemoveMsgで利用可能なオプション
        /// </summary>
        public enum EPeekMessageOption
        {
            /// <summary>
            /// 処理後メッセージをキューから削除しない
            /// </summary>
            PM_NOREMOVE = 0,
            /// <summary>
            /// 処理後メッセージをキューから削除する
            /// </summary>
            PM_REMOVE
        }

        #endregion

        #endregion

        #region API

        /// <summary>
        /// 着信した送信済みメッセージをディスパッチ（送出）し、
        /// スレッドのメッセージキューにポスト済みメッセージが存在するかどうかをチェックし、
        /// 存在する場合は、指定された構造体にそのメッセージを格納します。
        /// http://msdn.microsoft.com/ja-jp/library/cc410948.aspx
        /// </summary>
        /// <param name="lpMsg">
        /// 1 個の 構造体へのポインタを指定します。
        /// 関数から制御が返ると、この構造体に、メッセージ情報が格納されます。
        /// </param>
        /// <param name="hWnd">メッセージの取得に使う（取得元）ウィンドウのハンドルを指定します。</param>
        /// <param name="wMsgFilterMin">取得対象のメッセージの最小値を整数で指定します。</param>
        /// <param name="wMsgFilterMax">取得対象のメッセージの最大値を整数で指定します。</param>
        /// <param name="wRemoveMsg">
        /// メッセージの処理方法を指定します。次の値のいずれかを指定します。
        /// PM_NOREMOVE：PeekMessage 関数がメッセージを処理した後、そのメッセージをキューから削除しません。 
        /// PM_REMOVE  ：PeekMessage 関数がメッセージを処理した後、そのメッセージをキューから削除します。 
        /// </param>
        /// <returns>
        /// ・メッセージを取得した場合、0 以外の値が返ります。
        /// ・メッセージを取得しなかった場合、0 が返ります。
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PeekMessage(
            out MSG lpMsg,
            int hWnd,
            int wMsgFilterMin,
            int wMsgFilterMax,
            EPeekMessageOption wRemoveMsg);

        /// <summary>
        /// 呼び出し側スレッドのメッセージキューからメッセージを取得し、指定された構造体にそのメッセージを格納します。
        /// ポストされたメッセージが取得可能になるまで、この関数は、着信した送信済みメッセージをディスパッチ（送出）します。
        /// GetMessage 関数とは異なり、PeekMessage 関数は、何かメッセージがポストされるのを待たずに制御を返します。
        /// http://msdn.microsoft.com/ja-jp/library/cc364699.aspx
        /// </summary>
        /// <param name="lpMsg">
        /// 1 個の 構造体へのポインタを指定します。関数から制御が返ると、この構造体に、
        /// 呼び出し側スレッドのメッセージキューから取得したメッセージ情報が格納されます。
        /// </param>
        /// <param name="hWnd">
        /// メッセージの取得に使う（取得元）ウィンドウのハンドルを指定します。
        /// このウィンドウは、呼び出し側スレッドに所属していなければなりません。NULL値には、特別な意味があります。
        /// NULL：呼び出し側スレッドに所属する任意のウィンドウへ送信されたメッセージと、
        /// PostThreadMessage 関数を使って呼び出し側スレッドへポストされたスレッドメッセージを取得します。
        /// </param>
        /// <param name="wMsgFilterMin">取得対象のメッセージの最小値を整数で指定します。</param>
        /// <param name="wMsgFilterMax">取得対象のメッセージの最大値を整数で指定します。 </param>
        /// <returns>
        /// ・WM_QUIT 以外のメッセージを取得した場合、0 以外の値が返ります。
        /// ・WM_QUIT メッセージを取得した場合、0 が返ります。
        /// エラーが発生した場合、-1 が返ります。
        /// たとえば、hWnd パラメータで無効なウィンドウハンドルを指定した場合や、
        /// lpMsg で無効なポインタを指定した場合は、エラーが発生します。
        /// 拡張エラー情報を取得するには、 関数を使います。
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMessage(
            out MSG lpMsg,
            int hWnd,
            int wMsgFilterMin,
            int wMsgFilterMax);

        /// <summary>
        /// 1 つのウィンドウプロシージャへメッセージをディスパッチ（送出）します。
        /// 一般的に、GetMessage 関数が取得したメッセージをディスパッチするために、この関数を使います。
        /// http://msdn.microsoft.com/ja-jp/library/cc410766.aspx
        /// </summary>
        /// <param name="lpMsg">メッセージを保持している、1 個の 構造体へのポインタを指定します。</param>
        /// <returns>
        /// ウィンドウプロシージャの戻り値が返ります。
        /// 戻り値の意味は、ディスパッチされるメッセージにより異なります。
        /// 一般的にはこの戻り値を無視します。
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr DispatchMessage(out MSG lpMsg);

        #endregion
    }
}
