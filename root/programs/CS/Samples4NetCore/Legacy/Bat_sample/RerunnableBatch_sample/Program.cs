//**********************************************************************************
//* リラン可能バッチ処理・サンプル アプリ
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

using RerunnableBatch_sample.Business;
using RerunnableBatch_sample.Common;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace RerunnableBatch_sample
{
    /// <summary>Program</summary>
    class Program
    {
        /// <summary>
        /// 中間コミットを行う件数 
        /// </summary>
        /// <remarks>
        /// ※ サンプルデータの件数が少ない(830件)ため小さい値としている
        /// </remarks>
        public const int INTERMEDIATE_COMMIT_COUNT = 100;

        /// <summary>Main</summary>
        static void Main(string[] args)
        {
            // configの初期化
            string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
                .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            GetConfigParameter.InitConfiguration(dir + "/appsettings.json");

            // コマンドラインをバラす関数がある。
            List<string> valsLst = null;
            Dictionary<string, string> argsDic = null;
            StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst);

            // 引数クラス値（B層実行用）
            string screenId = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string controlId = "-";
            string actionType = "SQL"; // argsDic["/DAP"] + "%" + argsDic["/MODE1"] + "%" + argsDic["/MODE2"] + "%" + argsDic["/EXROLLBACK"];
            MyUserInfo myUserInfo = new MyUserInfo("userName", "ipAddress");

            // B層クラス
            LayerB layerB = new LayerB();

            // ↓B層実行：主キー値を全て検索(ORDER BY 主キー)-----------------------------------------------------

            // 引数クラスを生成
            VoidParameterValue selectPkListParameterValue = new VoidParameterValue(screenId, controlId, "SelectPkList", actionType, myUserInfo);

            // Ｂ層呼出し
            SelectPkListReturnValue selectPkReturnValue = (SelectPkListReturnValue)layerB.DoBusinessLogic(selectPkListParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

            // 実行結果確認
            if (selectPkReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                string error = "ErrorMessageID:" + selectPkReturnValue.ErrorMessageID + "\r\n";
                error += "ErrorMessage:" + selectPkReturnValue.ErrorMessage + "\r\n";
                error += "ErrorInfo:" + selectPkReturnValue.ErrorInfo + "\r\n";

                Console.WriteLine(error);
                Console.ReadKey();
                return; //バッチ処理終了
            }

            // 戻り値取得
            ArrayList pkList = selectPkReturnValue.PkList;

            // ↑B層実行：主キー値を全て検索(ORDER BY 主キー)-----------------------------------------------------

            int recordCount = pkList.Count; // 全レコード数
            int initialIndex = 0; // 処理開始インデックス ※ todo:リラン時に途中から再開する場合は初期値を変更する
            int transactionCount = Convert.ToInt32(Math.Ceiling(((double)(recordCount - initialIndex)) / INTERMEDIATE_COMMIT_COUNT));   // 更新B層実行回数

            // 性能測定
            // 性能測定 - 開始
            PerformanceRecorder pr = new PerformanceRecorder();
            pr.StartsPerformanceRecord();

            for (int transactionIndex = 0; transactionIndex < transactionCount; transactionIndex++)
            {
                ArrayList subPkList;    // 主キー一覧(1トランザクション分)
                int subPkStartIndex;    // 主キー(1トランザクション分)の開始位置
                int subPkCount;         // 主キー数(1トランザクション分)

                // 取り出す主キーの開始、数を取得
                subPkStartIndex = initialIndex + (transactionIndex * INTERMEDIATE_COMMIT_COUNT);
                if (subPkStartIndex + INTERMEDIATE_COMMIT_COUNT - 1 > recordCount - 1)
                {
                    subPkCount = (recordCount - initialIndex) % INTERMEDIATE_COMMIT_COUNT;
                }
                else
                {
                    subPkCount = INTERMEDIATE_COMMIT_COUNT;
                }

                // 主キー一覧(1トランザクション分)を取り出す
                subPkList = new ArrayList();
                subPkList.AddRange(pkList.GetRange(subPkStartIndex, subPkCount));

                // ↓B層実行：バッチ処理を実行(1トランザクション分)----------------------------------------------------

                // 引数クラスを生成
                ExecuteBatchProcessParameterValue executeBatchProcessParameterValue = new ExecuteBatchProcessParameterValue(screenId, controlId, "ExecuteBatchProcess", actionType, myUserInfo);
                executeBatchProcessParameterValue.SubPkList = subPkList;

                // Ｂ層呼出し
                VoidReturnValue executeBatchProcessReturnValue = (VoidReturnValue)layerB.DoBusinessLogic(executeBatchProcessParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

                // 実行結果確認
                if (selectPkReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    string error = "ErrorMessageID:" + selectPkReturnValue.ErrorMessageID + "\r\n";
                    error += "ErrorMessage:" + selectPkReturnValue.ErrorMessage + "\r\n";
                    error += "ErrorInfo:" + selectPkReturnValue.ErrorInfo + "\r\n";

                    Console.WriteLine(error);
                    Console.ReadKey();
                    return; // バッチ処理終了
                }

                // ↑B層実行：バッチ処理を実行(1トランザクション分)----------------------------------------------------
            }

            // 性能測定 - 終了
            Console.WriteLine(pr.EndsPerformanceRecord());
            Console.ReadKey();
        }
    }
}
