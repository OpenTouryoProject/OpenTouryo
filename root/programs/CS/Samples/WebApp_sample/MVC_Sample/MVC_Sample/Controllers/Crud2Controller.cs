//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Crud2Controller
//* クラス日本語名  ：Ajax.BeginForm用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MVC_Sample.Logic.Business;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models.ViewModels;

using System.Collections.Generic;
using System.Threading.Tasks;

using System.Web.Mvc;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Public.Db;

namespace MVC_Sample.Controllers
{
    /// <summary>
    /// Ajax.BeginForm用サンプル アプリ・コントローラ
    /// </summary>
    [Authorize]
    public class Crud2Controller : MyBaseMVController
    {
        /// <summary>
        /// 画面の初期表示
        /// GET: /Crud2/
        /// </summary>
        /// <returns>初期表示状態の画面 (ViewResult)</returns>
        [HttpGet]
        public ActionResult Index(CrudViweModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Shippers テーブルのレコード数をカウントする
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectCount(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;
                }
                else
                {
                    // 結果（正常系）
                    message = testReturnValue.Obj.ToString() + "件のデータがあります";
                }

                // メッセージを設定。
                model.Message = message;
            }

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataTable として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAll_DT(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                    // Ajax.BeginFormでは、UpdateTargetIdで指定した
                    // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                    // ＃ここではjQueryを使用している。
                    string scriptText = "$('#lblMessage').text('" + message + "');";
                    return JavaScript(scriptText);
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_ChartView", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataSet として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAll_DS(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                    // Ajax.BeginFormでは、UpdateTargetIdで指定した
                    // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                    // ＃ここではjQueryを使用している。
                    string scriptText = "$('#lblMessage').text('" + message + "');";
                    return JavaScript(scriptText);
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_ChartView", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataReader として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAll_DR(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                    // Ajax.BeginFormでは、UpdateTargetIdで指定した
                    // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                    // ＃ここではjQueryを使用している。
                    string scriptText = "$('#lblMessage').text('" + message + "');";
                    return JavaScript(scriptText);
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }
            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_ChartView", model);

        }

        /// <summary>
        /// Shippers テーブルのレコード全件を、動的 SQL を使用して取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAll_DSQL(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // 動的SQLの要素を設定
                testParameterValue.OrderColumn = model.DdlOrderColumn;
                testParameterValue.OrderSequence = model.DdlOrderSequence;

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                    // Ajax.BeginFormでは、UpdateTargetIdで指定した
                    // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                    // ＃ここではjQueryを使用している。
                    string scriptText = "$('#lblMessage').text('" + message + "');";
                    return JavaScript(scriptText);
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }
            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_ChartView", model);

        }

        /// <summary>
        /// ShipperId をキーにして Shippers テーブルのレコードを取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Select(CrudViweModel model)
        {
            string scriptText = "";

            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper;

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                    // メッセージを設定。
                    model.Message = message;

                    // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                    return PartialView("_MessageView", model);
                }
                else
                {
                    // 結果（正常系）
                }

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                ShipperViweModel svm = (ShipperViweModel)testReturnValue.Obj;
                scriptText = string.Format(
                    "$('#Shipper_ShipperID').val('{0}');$('#Shipper_CompanyName').val('{1}');$('#Shipper_Phone').val('{2}');",
                    svm.ShipperID, svm.CompanyName, svm.Phone);
            }

            return JavaScript(scriptText);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを追加する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                    model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper;

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;
                }
                else
                {
                    // 結果（正常系）
                    message = testReturnValue.Obj.ToString() + "件追加";
                }

                // メッセージを設定。
                model.Message = message;
            }
            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを更新する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper;

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;
                }
                else
                {
                    // 結果（正常系）
                    message = testReturnValue.Obj.ToString() + "件更新";
                }

                // メッセージを設定。
                model.Message = message;
            }

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを削除する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(CrudViweModel model)
        {
            if (ModelState.IsValid)
            {
                // 引数クラスを生成。下位（Ｂ・Ｄ層）は、テスト クラスを流用する
                TestParameterValue testParameterValue
                    = new TestParameterValue(this.ControllerName, "-", this.ActionName,
                        model.DdlDap + "%" + model.DdlMode1 + "%" + model.DdlMode2 + "%" + model.DdlExRollback, this.UserInfo);

                // 動的SQLの要素を設定
                testParameterValue.Shipper = model.Shipper;

                // Ｂ層呼出し＋都度コミット
                LayerB layerB = new LayerB();
                TestReturnValue testReturnValue = (TestReturnValue)await layerB.DoBusinessLogicAsync(
                    testParameterValue, this.SelectIsolationLevel(model.DdlIso));

                // 結果表示するメッセージ
                string message = "";

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                    message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                    message += "ErrorInfo:" + testReturnValue.ErrorInfo;
                }
                else
                {
                    // 結果（正常系）
                    message = testReturnValue.Obj.ToString() + "件削除";
                }

                // メッセージを設定。
                model.Message = message;
            }

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Sleepを実行し二重送信防止機能をテストする
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreventDoubleSubmission(CrudViweModel model)
        {
            System.Threading.Thread.Sleep(5 * 1000);

            // メッセージを設定。

            // 確認用のカウンタ
            if (Session["cnt"] == null)
            {
                Session["cnt"] = 1;
            }
            else
            {
                Session["cnt"] = ((int)Session["cnt"]) + 1;
            }

            model.Message = "PreventDoubleSubmission:" + Session["cnt"].ToString();

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>画面遷移する</summary>
        /// <returns>画面遷移のためのJavaScriptResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transitions()
        {
            return JavaScript("location.href='" + Url.Action("Index", "Crud1") + "';");
        }

        /// <summary>分離レベルの設定</summary>
        private DbEnum.IsolationLevelEnum SelectIsolationLevel(string iso)
        {
            if (iso == "NC")
            {
                return DbEnum.IsolationLevelEnum.NotConnect;
            }
            else if (iso == "NT")
            {
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
            else if (iso == "RU")
            {
                return DbEnum.IsolationLevelEnum.ReadUncommitted;
            }
            else if (iso == "RC")
            {
                return DbEnum.IsolationLevelEnum.ReadCommitted;
            }
            else if (iso == "RR")
            {
                return DbEnum.IsolationLevelEnum.RepeatableRead;
            }
            else if (iso == "SZ")
            {
                return DbEnum.IsolationLevelEnum.Serializable;
            }
            else if (iso == "SS")
            {
                return DbEnum.IsolationLevelEnum.Snapshot;
            }
            else if (iso == "DF")
            {
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
            else
            {
                //throw new Exception("分離レベルの設定がおかしい");
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
        }
    }
}
