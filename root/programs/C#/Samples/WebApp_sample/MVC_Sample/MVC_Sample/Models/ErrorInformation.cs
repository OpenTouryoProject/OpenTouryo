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
//* クラス名        ：ErrorInformation
//* クラス日本語名  ：ErrorInformation
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

//System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//MVC_Sample
using MVC_Sample.Controllers;

namespace MVC_Sample.Models
{
    /// <summary>
    /// ErrorInformation class
    /// </summary>
    public static class ErrorInformation
    {
        /// <summary>
        /// ErrorMessage
        /// </summary>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// ErrorInformation
        /// </summary>
        public static string ErrorInfo { get; set; }

        /// <summary>
        /// ErrorDatas
        /// </summary>
        public static List<ExceptionData> ErrorDatas { get; set; }
    }
}