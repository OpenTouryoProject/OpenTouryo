//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License

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
//* クラス名            :AsyncProcessingServiceReturnValue.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      Paramter Return Value class for Asynchronous Processing Service
//**********************************************************************************
// System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Common;

namespace AsyncProcessingService
{

    /// <summary>
    /// Paramter Return Value class for Asynchronous Processing Service
    /// </summary>
    public class AsyncProcessingServiceReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>UserId</summary>
        public string UserId;

        /// <summary>ProcessName</summary>
        public string ProcessName;

        /// <summary>Data<summary>
        public string Data;

        /// <summary>RegistrationDateTime</summary>
        public DateTime RegistrationDateTime;

        /// <summary> ExecutionStartDateTime;</summary>
        public DateTime ExecutionStartDateTime;

        /// <summary> NumberOfRetries;</summary>
        public int NumberOfRetries;

        /// <summary>ProgressRate</summary>
        public int ProgressRate;

        /// <summary> StatusId;</summary>
        public int StatusId;

        /// <summary>CompletionDateTime</summary>
        public DateTime CompletionDateTime;

        /// <summary> CommandId;</summary>
        public int CommandId;

        /// <summary>ReservedArea</summary>
        public string ReservedArea;
    }

}
