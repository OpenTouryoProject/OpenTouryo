//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
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
//* クラス名        ：EnvInfo
//* クラス日本語名  ：EnvInfo
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/08  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Net;

#if (NETSTD || NETCOREAPP)
#else
using Microsoft.Win32;
#endif

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>EnvInfo</summary>
    public class EnvInfo
    {
        #region Properties

        /// <summary>OsVersion</summary>
        public static OperatingSystem OsVersion { get; private set;  }

        /// <summary>OsVersionString</summary>
        public static string OsVersionString { get; private set;  }

        /// <summary>Is64BitOS</summary>
        public static bool Is64BitOS { get; private set;  }

        /// <summary>OsBit</summary>
        public static int OsBit { get; private set;  }

        /// <summary>Is64BitProcess</summary>
        public static bool Is64BitProcess { get; private set;  }

        /// <summary>ProcessBit</summary>
        public static int ProcessBit { get; private set;  }

        /// <summary>FrameworkVersion</summary>
        public static Version FrameworkVersion { get; private set;  }

        /// <summary>FrameworkVersionString</summary>
        public static string FrameworkVersionString { get; private set;  }

        /// <summary>DnsHostName</summary>
        public static string DnsHostName { get; private set;  }

        /// <summary>MachineName</summary>
        public static string MachineName { get; private set;  }

#if (NETSTD || NETCOREAPP)
#else
        /// <summary>OsProductName</summary>
        public static string OsProductName { get; private set;  }

        /// <summary>OsRelease</summary>
        public static string OsRelease { get; private set;  }

        /// <summary>OsBuild</summary>
        public static string OsBuild { get; private set;  }

        /// <summary>RegistryFrameworkVersion</summary>
        public static string RegistryFrameworkVersion { get; private set;  }

        /// <summary>RegistryFrameworkRelease</summary>
        public static string RegistryFrameworkRelease { get; private set;  }

        /// <summary>GetRegistryValue</summary>
        /// <param name="keyname">string</param>
        /// <param name="valuename">string</param>
        /// <returns>Registry値t</returns>
        private static string GetRegistryValue(string keyname, string valuename)
        {
            return Registry.GetValue(keyname, valuename, "").ToString();
        }
#endif
        #endregion

        #region Constructor

        /// <summary>Constructor</summary>
        static EnvInfo()
        {
            EnvInfo.OsVersion = Environment.OSVersion;
            EnvInfo.OsVersionString = Environment.OSVersion.VersionString;
            EnvInfo.Is64BitOS = Environment.Is64BitOperatingSystem;
            EnvInfo.OsBit = Environment.Is64BitOperatingSystem ? 64 : 32;
            EnvInfo.Is64BitProcess = Environment.Is64BitProcess;
            EnvInfo.ProcessBit = Environment.Is64BitProcess ? 64 : 32;
            EnvInfo.FrameworkVersion = Environment.Version;
            EnvInfo.FrameworkVersionString = Environment.Version.ToString();
            EnvInfo.DnsHostName = Dns.GetHostName();
            EnvInfo.MachineName = Environment.MachineName;

#if (NETSTD || NETCOREAPP)
#else
            EnvInfo.OsProductName = EnvInfo.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            EnvInfo.OsRelease = EnvInfo.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId");
            EnvInfo.OsBuild = EnvInfo.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            EnvInfo.RegistryFrameworkVersion = EnvInfo.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Version");
            EnvInfo.RegistryFrameworkRelease = EnvInfo.GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Release");
#endif
        }

        #endregion
    }
}
