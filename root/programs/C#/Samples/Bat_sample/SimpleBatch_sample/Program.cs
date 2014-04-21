//**********************************************************************************
//* サンプル バッチ
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：サンプル バッチ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// 型情報
using SimpleBatch_sample.Common;
using SimpleBatch_sample.Business;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
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

namespace SimpleBatch_sample
{
    /// <summary>Program</summary>
    class Program
    {
        /// <summary>Main</summary>
        static void Main(string[] args)
        {
            ////////////////////////////////////////////////////////////////////////
            // 簡素なサンプルなので、
            // ・多重化（タスク毎、結果セットを分割）
            // ・フェッチ・サイズ（メモリ消費量を抑える）
            // ・コミット・インターバル、リラン
            // 等の考慮が別途必要になることがあります。
            ////////////////////////////////////////////////////////////////////////

            // コマンドラインをバラす関数がある。
            List<string> valsLst = null;
            Dictionary<string, string> argsDic = null;
            
            PubCmnFunction.GetCommandArgs('/', out argsDic, out valsLst);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    System.Reflection.Assembly.GetExecutingAssembly().Location, "-", "SelectCount",
                    argsDic["/DAP"] + "%"
                    + argsDic["/MODE1"] + "%"
                    + argsDic["/MODE2"] + "%"
                    + argsDic["/EXROLLBACK"],
                    new MyUserInfo("", ""));

            // 戻り値
            TestReturnValue testReturnValue;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                string error = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                error += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                error += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";

                Console.WriteLine(error);
                Console.ReadKey();
            }
            else
            {
                // 結果（正常系）
                Console.WriteLine(testReturnValue.Obj.ToString() + "件のデータがあります");
                Console.ReadKey();
            }
        }
    }
}
