//**********************************************************************************
//* 単純バッチ処理・サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：サンプル バッチ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using SimpleBatch_sample.Business;
using SimpleBatch_sample.Common;

using System;
using System.IO;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;
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

            // configの初期化
            GetConfigParameter.InitConfiguration("appsettings.json");

            // コマンドラインをバラす関数がある。
            List<string> valsLst = null;
            Dictionary<string, string> argsDic = null;
            
            StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst);

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
