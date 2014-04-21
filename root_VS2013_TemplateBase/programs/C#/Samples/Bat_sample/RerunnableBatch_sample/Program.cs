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
using RerunnableBatch_sample.Common;
using RerunnableBatch_sample.Business;

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
            // コマンドラインをバラす関数がある。
            List<string> valsLst = null;
            Dictionary<string, string> argsDic = null;
            PubCmnFunction.GetCommandArgs('/', out argsDic, out valsLst);

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
        }
    }
}
