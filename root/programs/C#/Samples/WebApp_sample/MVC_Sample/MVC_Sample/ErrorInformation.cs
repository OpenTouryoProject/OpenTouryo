//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：ErrorInformation class
//* クラス日本語名  ：Html.BeginForm用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/08/28  Supragyan         Created ErrorInformation class to define error properties
//**********************************************************************************
//System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//MVC_Sample
using MVC_Sample.Controllers;

namespace MVC_Sample
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