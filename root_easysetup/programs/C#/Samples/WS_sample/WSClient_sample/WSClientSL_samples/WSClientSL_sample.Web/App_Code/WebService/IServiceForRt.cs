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
//* クラス名        ：IServiceForRt
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッドを公開する。
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Reflection;

// System.Web
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

using Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest;

namespace Touryo.Infrastructure.Framework.ServiceInterface.WCFWebService
{
    /// <summary>
    /// WCF Webサービス（サービス インターフェイス基盤）
    /// REST（XML、JSON）汎用Webメソッドを公開する。
    /// </summary>
    [ServiceContract]
    public interface IServiceForRt
    {
        /// <summary>XML汎用Webメソッド</summary>
        /// <param name="param">XML 形式で送信された引数（ParamDataContract）</param>
        /// <returns>XML 形式で送信される戻り値（ReturnDataContract）</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "XML")]
        ReturnDataContract XML(ParamDataContract param);

        /// <summary>JSON汎用Webメソッド</summary>
        /// <param name="param">JSON 形式で送信された引数（ParamDataContract）</param>
        /// <returns>JSON 形式で送信される戻り値（ReturnDataContract）</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "JSON")]
        ReturnDataContract JSON(ParamDataContract param);
    }
}
