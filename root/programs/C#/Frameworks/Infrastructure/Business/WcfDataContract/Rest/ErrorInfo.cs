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
//* クラス名        ：ErrorInfo
//* クラス日本語名  ：WCF Webサービス（サービス インターフェイス基盤）
//*                   REST（XML、JSON）汎用Webメソッド用の例外データ・コントラクト
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/08/13  西野 大介         新規作成
//**********************************************************************************

using System.Runtime.Serialization;

namespace Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest
{
    /// <summary>エラーのデータ・コントラクト</summary>
    [DataContract]
    public class ErrorInfo
    {
        /// <summary>
        /// エラーの種類
        /// </summary>
        [DataMember]
        public string ErrorType;

        /// <summary>
        /// エラーメッセージ ID
        /// </summary>
        [DataMember]
        public string MessageID;

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [DataMember]
        public string Message;

        /// <summary>
        /// エラーに関する情報
        /// </summary>
        [DataMember]
        public string Information;
    }
}
