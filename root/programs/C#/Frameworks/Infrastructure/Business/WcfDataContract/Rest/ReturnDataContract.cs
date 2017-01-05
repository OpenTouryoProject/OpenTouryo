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
//* クラス名        ：ReturnDataContract
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッド用の戻り値データ・コントラクト
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
    /// <summary>クライアントに返すデータ</summary>
    /// <remarks>自動的にREST（XML、JSON）形式にマーシャリングされる</remarks>
    [DataContract]
    public class ReturnDataContract
    {
        /// <summary>エラー情報 (エラーが発生しなければ null)</summary>
        [DataMember]
        public ErrorInfo Error = null;

        /// <summary>WCFサービスの戻り値</summary>
        [DataMember]
        public Informations Info = null;
    }
}
