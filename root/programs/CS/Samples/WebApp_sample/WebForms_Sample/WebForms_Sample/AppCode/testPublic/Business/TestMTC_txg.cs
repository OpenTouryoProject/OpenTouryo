//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：TestMTC_txg
//* クラス日本語名  ：Ｂ層のテスト（手動トランザクション制御－複数コネクション版）
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;

using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;

using MyType;

namespace WebForms_Sample
{
    /// <summary>
    /// TestMTC_mcn の概要の説明です
    /// </summary>
    public class TestMTC_txg : MyFcBaseLogic
    {
        /// <summary>
        /// 業務処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        protected override void UOC_DoAction(BaseParameterValue parameterValue, ref BaseReturnValue returnValue)
        {
            // 引数クラスをアップキャスト
            TestParameterValue testParameter = (TestParameterValue)parameterValue;

            // 戻り値クラスを生成
            TestReturnValue testReturn = new TestReturnValue();

            // 戻り値クラスをダウンキャストして戻す
            returnValue = (BaseReturnValue)testReturn;

            // ---

            // トランザクション パターンIDの領域
            string[] transactionPatternIDs;

            // トランザクション グループIDからトランザクション パターンIDを取得
            BaseLogic.GetTransactionPatterns(
                (string)testParameter.Obj, out transactionPatternIDs);

            // トランザクション パターンIDを設定
            testReturn.Obj = transactionPatternIDs;

            #region Damを初期化

            // トランザクション グループIDから取得した、
            // トランザクション パターンIDでDam初期化する。
            foreach (string transactionPatternID in transactionPatternIDs)
            {
                BaseDam tempDam = null;

                if (transactionPatternID.IndexOf("SQL") != -1)
                {
                    // DamSqlSvrを初期化してセット
                    tempDam = new DamSqlSvr();
                    BaseLogic.InitDam(transactionPatternID, tempDam);
                    this.SetDam(transactionPatternID, tempDam);
                }
                else if (transactionPatternID.IndexOf("ODP") != -1)
                {
                    // DamManagedOdpを初期化してセット
                    tempDam = new DamManagedOdp();
                    BaseLogic.InitDam(transactionPatternID, tempDam);
                    this.SetDam(transactionPatternID, tempDam);
                }
                else if (transactionPatternID.IndexOf("MCN") != -1)
                {
                    // DamMySQLを初期化してセット
                    tempDam = new DamMySQL();
                    BaseLogic.InitDam(transactionPatternID, tempDam);
                    this.SetDam(transactionPatternID, tempDam);
                }
            }

            #endregion

            #region 終了時の状態選択

            #region Damの状態選択

            if ((parameterValue.ActionType.Split('%'))[2] == "UT")
            {
                // トランザクションあり
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NT")
            {
                // トランザクションなし
                // → まえもってロールバックしておく

                // ロールバック
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.GetDam(transactionPatternID).RollbackTransaction();
                }
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NC")
            {
                // コネクションなし
                // → まえもってロールバック、コネクションクローズしておく
                //
                // ※ トランザクションを開始して
                //    コミットしないで閉じると、ロールバック扱い。

                // ロールバック
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.GetDam(transactionPatternID).RollbackTransaction();
                }

                // コネクションクローズ
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.GetDam(transactionPatternID).ConnectionClose();
                }
            }
            else if ((parameterValue.ActionType.Split('%'))[2] == "NULL")
            {
                // データアクセス制御クラス = Null
                // → まえもってロールバック、コネクションクローズ、Nullクリアしておく
                //
                // ※ トランザクションを開始して
                //    コミットしないで閉じると、ロールバック扱い。

                // ロールバック
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.GetDam(transactionPatternID).RollbackTransaction();
                }

                // コネクションクローズ
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.GetDam(transactionPatternID).ConnectionClose();
                }

                // Nullクリア
                foreach (string transactionPatternID in transactionPatternIDs)
                {
                    this.SetDam(transactionPatternID, null);
                }
            }

            #endregion

            #region エラーのスロー

            if ((parameterValue.ActionType.Split('%'))[1] == "Business")
            {
                // 業務例外のスロー
                throw new BusinessApplicationException(
                    "ロールバックのテスト",
                    "ロールバックのテスト",
                    "エラー情報");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "System")
            {
                // システム例外のスロー
                throw new BusinessSystemException(
                    "ロールバックのテスト",
                    "ロールバックのテスト");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other")
            {
                // その他、一般的な例外のスロー
                throw new Exception("ロールバックのテスト");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other-Business")
            {
                // その他、一般的な例外（業務例外へ振り替え）のスロー
                throw new Exception("Other-Business");
            }
            else if ((parameterValue.ActionType.Split('%'))[1] == "Other-System")
            {
                // その他、一般的な例外（システム例外へ振り替え）のスロー
                throw new Exception("Other-System");
            }

            #endregion

            #endregion
        }
    }
    
}