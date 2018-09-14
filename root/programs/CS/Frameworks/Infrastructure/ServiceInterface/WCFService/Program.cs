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
//* クラス名        ：Program
//* クラス日本語名  ：WCFService（サービス インターフェイス基盤）
//*                   TCP/IPの.NETオブジェクトのバイナリ転送用メソッドを公開する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成
//**********************************************************************************

using System;
using System.Net.Http;
using System.ServiceModel;

using Touryo.Infrastructure.Business.Transmission;
using Touryo.Infrastructure.Framework.Authentication;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadKey();
            OAuth2AndOIDCClient.HttpClient = new HttpClient(); // JwkSet取得用
            using (ServiceHost host = new ServiceHost(typeof(WCFTCPSvcForFx)))
            {
                host.Open();
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
