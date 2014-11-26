using System;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;

// フレームワーク
using Touryo.Infrastructure.Framework.Exceptions;

// 部品

using AsyncProcessingService.Codes.DAO;
using AsyncProcessingService.Codes.Common;

namespace AsyncProcessingService.Codes.Business
{
    class LayerB : MyFcBaseLogic
    {
        public void UOC_Start(TestParameterValue testParameter)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.Insert(testParameter, testReturn);                   

            // ロールバックのテスト
            this.TestRollback(testParameter);
        }

        private void UOC_Update(TestParameterValue testParameter)
        {
            TestReturnValue testReturn = new TestReturnValue();
            this.ReturnValue = testReturn;
           
            LayerD myDao = new LayerD(this.GetDam());
            myDao.Update(testParameter, testReturn);
           
            this.TestRollback(testParameter);
        }

        private void TestRollback(TestParameterValue testParameter)
        {

            switch ((testParameter.ActionType.Split('%'))[3])
            {

                case "Business":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // 業務例外のスロー
                    throw new BusinessApplicationException(
                        "ロールバックのテスト",
                        "ロールバックのテスト",
                        "エラー情報");                

                case "System":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // システム例外のスロー
                    throw new BusinessSystemException(
                        "ロールバックのテスト",
                        "ロールバックのテスト");

                case "Other":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外のスロー
                    throw new Exception("ロールバックのテスト");

                case "Other-Business":
                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外（業務例外へ振り替え）のスロー
                    throw new Exception("Other-Business");

                case "Other-System":

                    // 戻り値が見えるか確認する。
                    ((TestReturnValue)this.ReturnValue).Obj = "戻り値が戻るか？";

                    // その他、一般的な例外（システム例外へ振り替え）のスロー
                    throw new Exception("Other-System");
            }
        }
    }
}
