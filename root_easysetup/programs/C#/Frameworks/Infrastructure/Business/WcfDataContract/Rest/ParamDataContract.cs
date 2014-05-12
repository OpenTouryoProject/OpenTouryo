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
//* クラス名        ：ParamDataContract
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッド用の引数データ・コントラクト
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest
{
    /// <summary>サービスに送信するデータ</summary>
    /// <remarks>自動的にREST（XML、JSON）形式にマーシャリングされる</remarks>
    [DataContract]
    public class ParamDataContract
    {
        /// <summary>サービスの論理名称</summary>
        [DataMember]
        public string ServiceName = string.Empty;

        /// <summary>画面ID</summary>
        [DataMember]
        public string ScreenId = string.Empty;

        /// <summary>コントロールID</summary>
        [DataMember]
        public string ControlId = string.Empty;

        /// <summary>メソッド名</summary>
        [DataMember]
        public string MethodName = string.Empty;

        /// <summary>アクションタイプ</summary>
        [DataMember]
        public string ActionType = string.Empty;

        /// <summary>ユーザ名</summary>
        [DataMember]
        public string UserName = string.Empty;

        /// <summary>WCFサービスの引数</summary>
        [DataMember]
        public Informations Info = null;
    }

   
}
