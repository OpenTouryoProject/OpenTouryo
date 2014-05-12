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
//* クラス名        ：Test code for Internationalization
//* クラス日本語名  ：国際化対応のテスト・コード
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/01/20  西野 大介         新規作成
//*  2014/xx/xx  Rituparna.Biswas  
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using Touryo.Infrastructure.Framework.Util;

using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Util;

namespace ConsoleApplication1
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        static void Main(string[] args)
        {
            #region Culture information(Culture and UICulture) is set up here.

            // Specifications changed by a command line argument are good.

            // The resolution of the command line argument is possible in following API.
            //PubCmnFunction.GetCommandArgs(・・・);

            Dictionary<string, string> dic = null;
            List<string> list = null;

            PubCmnFunction.GetCommandArgs('/', out dic, out list);

            if (dic.ContainsKey("/CULTURE"))
            {
                // Use the UICulture. 
                Thread.CurrentThread.CurrentUICulture
                    = new System.Globalization.CultureInfo(dic["/CULTURE"]);
            }

            #endregion

            #region A result is written in standard output.

            #region (1) PublicExceptionMessage

            Console.WriteLine("<PublicExceptionMessage>"); 
            Console.WriteLine("NO_CONFIG:" + PublicExceptionMessage.NO_CONFIG);
            Console.WriteLine("SWITCH_ERROR:" + PublicExceptionMessage.SWITCH_ERROR);
            Console.WriteLine("COMMANDTIMEOUT_ERROR:" + PublicExceptionMessage.COMMANDTIMEOUT_ERROR);
            Console.WriteLine("RESOURCE_FILE_NOT_FOUND:" + PublicExceptionMessage.RESOURCE_FILE_NOT_FOUND);
            Console.WriteLine("XML_DECLARATION_ERROR:" + PublicExceptionMessage.XML_DECLARATION_ERROR);
            Console.WriteLine("XML_ELEMENT_ERROR:" + PublicExceptionMessage.XML_ELEMENT_ERROR);
            Console.WriteLine("XML_ELEMENT_ERROR_LOG4NET:" + PublicExceptionMessage.XML_ELEMENT_ERROR_LOG4NET);
            Console.WriteLine("ZIP_PASSWORD:" + PublicExceptionMessage.ZIP_PASSWORD);
            Console.WriteLine("LATEBIND_ERROR0:" + PublicExceptionMessage.LATEBIND_ERROR0);
            Console.WriteLine("LATEBIND_ERROR1:" + PublicExceptionMessage.LATEBIND_ERROR1);
            Console.WriteLine("LATEBIND_ERROR2:" + PublicExceptionMessage.LATEBIND_ERROR2);
            Console.WriteLine("LATEBIND_ERROR3:" + PublicExceptionMessage.LATEBIND_ERROR3);
            Console.WriteLine("DB_ISO_LEVEL_PARAM_ERROR_UC:" + PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_UC);
            Console.WriteLine("DB_ISO_LEVEL_PARAM_ERROR_RR:" + PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_RR);
            Console.WriteLine("DB_ISO_LEVEL_PARAM_ERROR_SS:" + PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_SS);
            Console.WriteLine("DB_ISO_LEVEL_PARAM_ERROR_USR:" + PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_USR);
            Console.WriteLine("DB_ISO_LEVEL_PARAM_ERROR_NC:" + PublicExceptionMessage.DB_ISO_LEVEL_PARAM_ERROR_NC);
            Console.WriteLine("THIS_DPQ_TAG_IS_UNKNOWN:" + PublicExceptionMessage.THIS_DPQ_TAG_IS_UNKNOWN);
            Console.WriteLine("DPQ_TAG_FORMAT_ERROR:" + PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR);
            Console.WriteLine("DPQ_TAG_NAME_ATTR_NOT_EXIST:" + PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST);
            Console.WriteLine("DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY:" + PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY);
            Console.WriteLine("DPQ_TAG_VALUE_ATTR_NOT_EXIST:" + PublicExceptionMessage.DPQ_TAG_VALUE_ATTR_NOT_EXIST);
            Console.WriteLine("DPQ_TAG_INNER_PARAM_NOT_EXIST:" + PublicExceptionMessage.DPQ_TAG_INNER_PARAM_NOT_EXIST);
            Console.WriteLine("PARAM_TAG_ERROR:" + PublicExceptionMessage.PARAM_TAG_ERROR);
            Console.WriteLine("PARAM_TAG_TYPE_ERROR:" + PublicExceptionMessage.PARAM_TAG_TYPE_ERROR);
            Console.WriteLine("PARAM_TAG_VALUE_ERROR:" + PublicExceptionMessage.PARAM_TAG_VALUE_ERROR);
            Console.WriteLine("DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE:" + PublicExceptionMessage.DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE);
            Console.WriteLine("DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_TEXT_PARAM_OF_IF_TAG_IS_NULL:" + PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_TEXT_PARAM_OF_IF_TAG_IS_NULL);
            Console.WriteLine("DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_NULL:" + PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_NULL);
            Console.WriteLine("DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_FALSE:" + PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_FALSE);
            Console.WriteLine("ORDER_BIND_ERROR_PARAMETER_NOT_FOUND:" + PublicExceptionMessage.ORDER_BIND_ERROR_PARAMETER_NOT_FOUND);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_START_CHARACTER:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_START_CHARACTER);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_ESCAPE_CHARACTER:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_ESCAPE_CHARACTER);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_CURLY_BRACE:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_CURLY_BRACE);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_PROPERTY_NAME_IS_EMPTY:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_PROPERTY_NAME_IS_EMPTY);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_PROPERTY_VALUE_IS_EMPTY:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_PROPERTY_VALUE_IS_EMPTY);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_NAME:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_NAME);
            Console.WriteLine("PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_VALUE:" + PublicExceptionMessage.PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_VALUE);
            Console.WriteLine("ARGUMENT_INJUSTICE:" + PublicExceptionMessage.ARGUMENT_INJUSTICE);
            Console.WriteLine("PARAM_IS_NULL:" + PublicExceptionMessage.PARAM_IS_NULL);
            Console.WriteLine("NOT_IMPLEMENTED:" + PublicExceptionMessage.NOT_IMPLEMENTED);

            Console.WriteLine("\r\n");

            #endregion

            #region (2) FrameworkExceptionMessage

            Console.WriteLine("<FrameworkExceptionMessage>"); 
            Console.WriteLine("ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR:");

            /*1*/
            foreach (var item in FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR)
            {
                Console.WriteLine("MESSAGE_XML_FORMAT_ERROR: " + item);
            }

            /*2*/
            Console.WriteLine("MESSAGE_XML_FORMAT_ERROR_ATTR: " + FrameworkExceptionMessage.MESSAGE_XML_FORMAT_ERROR_ATTR);
            
            /*3*/
            foreach (var item in FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR)
            {
                Console.WriteLine("SHAREDPROPERTY_XML_FORMAT_ERROR: " + item);
            }

            /*4*/
            Console.WriteLine("SHAREDPROPERTY_XML_FORMAT_ERROR_ATTR: " + FrameworkExceptionMessage.SHAREDPROPERTY_XML_FORMAT_ERROR_ATTR);
            
            /*5*/
            foreach (var item in FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR)
            {
                Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR: " + item);
            }
            
            /*6*/
            Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR_value: " + FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value);
            
            /*7*/
            Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR_label: " + FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_label);
            
            /*8*/
            Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR_dl1: " + FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_dl1);
            
            /*9*/
            Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR_dl2: " + FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_dl2);
            
            /*10*/
            Console.WriteLine("SCREEN_CONTROL_XML_FORMAT_ERROR_mode: " + FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_mode);
            
            /*11*/
            foreach (var item in FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR)
            {
                Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR: " + item);
            }
            
            /*12*/
            Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR_tp: " + FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tp);
            
            /*13*/
            Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso1: " + FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso1);

            /*14*/
            Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso2: " + FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso2);
            
            /*15*/
            Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR_tg: " + FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tg);
            
            /*16*/
            Console.WriteLine("TRANSACTION_CONTROL_XML_FORMAT_ERROR_tgval: " + FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tgval);
            
            /*17*/
            foreach (var item in FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR)
            {
                Console.WriteLine("IPR_NAMESERVICE_XML_FORMAT_ERROR: " + item);
            }
            
            /*18*/
            Console.WriteLine("IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName1: " + FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName1);
            
            /*19*/
            Console.WriteLine("IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName2: " + FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName2);
            
            /*20*/
            Console.WriteLine("IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName1: " + FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName1);
            
            /*21*/
            Console.WriteLine("IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName2: " + FrameworkExceptionMessage.IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName2);
            
            /*22*/
            foreach (var item in FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR)
            {
                Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR: " + item);
            }
            
            /*23*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_prt1: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prt1);
            
            /*24*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_prt2: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prt2);
            
            /*25*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_url1: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url1);
            
            /*26*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_url2: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url2);
            
            /*27*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_url3: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url3);
            
            /*28*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_url4: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url4);
            
            /*29*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_to: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_to);
            
            /*30*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_prop1: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prop1);
            
            /*31*/
            Console.WriteLine("PRT_NAMESERVICE_XML_FORMAT_ERROR_prop2: " + FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prop2);
            
            /*32*/
            Console.WriteLine("NAMESERVICE_XML_FORMAT_ERROR_tm: " + FrameworkExceptionMessage.NAMESERVICE_XML_FORMAT_ERROR_tm);
            
            /*33*/
            foreach (var item in FrameworkExceptionMessage.NO_FX_HIDDEN)
            {
                Console.WriteLine("NO_FX_HIDDEN: " + item);
            }
            
            /*34*/
            foreach (var item in FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_NUMVAL)
            {
                Console.WriteLine("ERROR_IN_WRITING_OF_FX_NUMVAL: " + item);
            }
            
            /*35*/
            foreach (var item in FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1)
            {
                Console.WriteLine("ERROR_IN_WRITING_OF_FX_PATH1: " + item);
            }
            
            /*36*/
            foreach (var item in FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2)
            {
                Console.WriteLine("ERROR_IN_WRITING_OF_FX_PATH2: " + item);
            }
            
            /*37*/
            foreach (var item in FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1)
            {
                Console.WriteLine("ERROR_IN_WRITING_OF_FX_SWITCH1: " + item);
            }
            
            /*38*/
            foreach (var item in FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH2)
            {
                Console.WriteLine("ERROR_IN_WRITING_OF_FX_SWITCH2: " + item);
            }

            /*39*/
            foreach (var item in FrameworkExceptionMessage.PARAMETER_CHECK_ERROR)
            {
                Console.WriteLine("PARAMETER_CHECK_ERROR: " + item);
            }
            
            /*40*/
            Console.WriteLine("PARAMETER_CHECK_ERROR_null: " + FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_null);

            /*41*/
            Console.WriteLine("PARAMETER_CHECK_ERROR_empty: " + FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_empty);

            /*42*/
            Console.WriteLine("PARAMETER_CHECK_ERROR_length: " + FrameworkExceptionMessage.PARAMETER_CHECK_ERROR_length);
            
            /*43*/
            foreach (var item in FrameworkExceptionMessage.ASYNC_FUNC_CHECK_ERROR)
            {
                Console.WriteLine("ASYNC_FUNC_CHECK_ERROR: " + item);
            }
            
            /*44*/
            foreach (var item in FrameworkExceptionMessage.ASYNC_MSGBOX_ERROR)
            {
                Console.WriteLine("ASYNC_MSGBOX_ERROR: " + item);
            }
            
            /*45*/
            foreach (var item in FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CHECK_ERROR)
            {
                Console.WriteLine("ASYNC_EVENT_ENTRY_CHECK_ERROR: " + item);
            }
            
            /*46*/
            foreach (var item in FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CONTROL_CHECK_ERROR)
            {
                Console.WriteLine("ASYNC_EVENT_ENTRY_CONTROL_CHECK_ERROR: " + item);
            }

            /*47*/
            foreach (var item in FrameworkExceptionMessage.ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR)
            {
                Console.WriteLine("ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR: " + item);
            }

            /*48*/
            foreach (var item in FrameworkExceptionMessage.CONTROL_TYPE_ERROR)
            {
                Console.WriteLine("48. CONTROL_TYPE_ERROR: " + item);
            }
            
            /*49*/
            foreach (var item in FrameworkExceptionMessage.CONTROL_REPETITION_ERROR1)
            {
                Console.WriteLine("CONTROL_REPETITION_ERROR1: " + item);
            }

            /*50*/
            foreach (var item in FrameworkExceptionMessage.CONTROL_REPETITION_ERROR2)
            {
                Console.WriteLine("CONTROL_REPETITION_ERROR2: " + item);
            }

            /*51*/
            foreach (var item in FrameworkExceptionMessage.NO_MASTER_PAGE)
            {
                Console.WriteLine("NO_MASTER_PAGE: " + item);
            }
            
            /*52*/
            foreach (var item in FrameworkExceptionMessage.MASTER_PAGE_TYPE_ERROR)
            {
                Console.WriteLine("MASTER_PAGE_TYPE_ERROR: " + item);
            }
            
            /*53*/
            foreach (var item in FrameworkExceptionMessage.SESSION_TIMEOUT)
            {
                Console.WriteLine("SESSION_TIMEOUT: " + item);
            }
            
            /*54*/
            foreach (var item in FrameworkExceptionMessage.ILLEGAL_OPERATION_CHECK_ERROR)
            {
                Console.WriteLine("ILLEGAL_OPERATION_CHECK_ERROR: " + item);
            }
            
            /*55*/
            foreach (var item in FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR)
            {
                Console.WriteLine("SCREEN_CONTROL_CHECK_ERROR: " + item);
            }
            
            /*56*/
            Console.WriteLine("SCREEN_CONTROL_CHECK_ERROR_get: " + FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_get);
            
            /*57*/
            Console.WriteLine("SCREEN_CONTROL_CHECK_ERROR_naked: " + FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_naked);

            /*58*/
            Console.WriteLine("SCREEN_CONTROL_CHECK_ERROR_nolbl: " + FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_nolbl);
            
            /*59*/
            foreach (var item in FrameworkExceptionMessage.FX_PROCESSING_STATUS_ERROR)
            {
                Console.WriteLine("FX_PROCESSING_STATUS_ERROR: " + item);
            }
            
            /*60*/
            Console.WriteLine("FX_PROCESSING_STATUS_ERROR_NO_BH_QUEUE: " + FrameworkExceptionMessage.FX_PROCESSING_STATUS_ERROR_NO_BH_QUEUE);
            
            /*61*/
            foreach (var item in FrameworkExceptionMessage.DIALOG_AFTER_PROCESSING_STATUS_ERROR)
            {
                Console.WriteLine("DIALOG_AFTER_PROCESSING_STATUS_ERROR: " + item);
            }
            
            /*62*/
            foreach (var item in FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR)
            {
                Console.WriteLine("DIALOG_CLOSING_STATUS_ERROR: " + item);
            }
            
            /*63*/
            Console.WriteLine("DIALOG_CLOSING_STATUS_ERROR1: " + FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR1);

            /*64*/
            Console.WriteLine("DIALOG_CLOSING_STATUS_ERROR2: " + FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR2);

            /*65*/
            foreach (var item in FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR)
            {
                Console.WriteLine("LB_ILLEGAL_CALL_CHECK_ERROR: " + item);
            }

            Console.WriteLine("\r\n");

            #endregion

            #region (3) MyBusinessSystemExceptionMessage

            Console.WriteLine("<MyBusinessSystemExceptionMessage>");

            Console.WriteLine("SAMPLE_ERROR:");
            foreach (string s in MyBusinessSystemExceptionMessage.SAMPLE_ERROR)
            {
                Console.WriteLine(" - " + s);
            }

            Console.WriteLine("CMN_DAO_ERROR:");
            foreach (string s in MyBusinessSystemExceptionMessage.CMN_DAO_ERROR)
            {
                Console.WriteLine(" - " + s);
            }

            Console.WriteLine("CMN_DAO_ERROR_SQL:" + MyBusinessSystemExceptionMessage.CMN_DAO_ERROR_SQL);

            Console.WriteLine("\r\n");

            #endregion

            #region (4) MyBusinessApplicationExceptionMessage

            Console.WriteLine("<MyBusinessApplicationExceptionMessage>");

            Console.WriteLine("SAMPLE_ERROR:");
            foreach (string s in MyBusinessApplicationExceptionMessage.SAMPLE_ERROR)
            {
                Console.WriteLine(" - " + s);
            }

            Console.WriteLine("\r\n");

            #endregion

            #region (5) GetMessage
            
            Console.WriteLine("<GetMessage>");
            
            Console.WriteLine(GetMessage.GetMessageDescription("I0001"));
            Console.WriteLine(GetMessage.GetMessageDescription("I0002"));
            Console.WriteLine(GetMessage.GetMessageDescription("I0003"));
            Console.WriteLine(GetMessage.GetMessageDescription("I0004"));
            Console.WriteLine(GetMessage.GetMessageDescription("I0005"));
            
            Console.WriteLine(GetMessage.GetMessageDescription("E0001"));
            Console.WriteLine(GetMessage.GetMessageDescription("E0002"));
            Console.WriteLine(GetMessage.GetMessageDescription("E0003"));
            Console.WriteLine(GetMessage.GetMessageDescription("E0004"));
            Console.WriteLine(GetMessage.GetMessageDescription("E0005"));

            #endregion

            #endregion

            // As notes, when you translate into English from Japanese,
            // the turn of {0} and {1} used by String.Format be reversed.

            Console.ReadKey();
        }
    }
}
