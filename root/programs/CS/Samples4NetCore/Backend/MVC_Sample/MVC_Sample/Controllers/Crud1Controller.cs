//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Crud1Controller
//* クラス日本語名  ：Html.BeginForm用サンプル アプリ・コントローラ
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
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Diagnostics;

namespace MVC_Sample.Controllers
{
    /// <summary>
    /// Html.BeginForm用サンプル アプリ・コントローラ
    /// </summary>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class Crud1Controller : MyBaseMVControllerCore
    {
        /// <summary>
        /// 画面の初期表示
        /// GET: /Crud2/
        /// </summary>
        /// <returns>初期表示状態の画面 (ViewResult)</returns>
        [HttpGet]
        public IActionResult Index(CrudViweModel model)
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
        public async Task<IActionResult> SelectCount(CrudViweModel model)
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

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataTable として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAll_DT(CrudViweModel model)
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

                    // メッセージを設定。
                    model.Message = message;
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataSet として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAll_DS(CrudViweModel model)
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

                    // メッセージを設定。
                    model.Message = message;
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataReader として取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAll_DR(CrudViweModel model)
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

                    // メッセージを設定。
                    model.Message = message;
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を、動的 SQL を使用して取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectAll_DSQL(CrudViweModel model)
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

                    // メッセージを設定。
                    model.Message = message;
                }
                else
                {
                    // 結果（正常系）
                    model.Shippers = (List<ShipperViweModel>)testReturnValue.Obj;
                }
            }

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// ShipperId をキーにして Shippers テーブルのレコードを取得する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(CrudViweModel model)
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

                    // メッセージを設定。
                    model.Message = message;
                }
                else
                {
                    // 結果（正常系）
                    ModelState.Clear(); // ErrorのClearをしないと何故か設定できない。

                    #region PocoToPocoのテストコード

                    ShipperViweModel svm = null;
                    TestShipperViweModel tsvm = null;

                    // テスト１
                    svm = (ShipperViweModel)BinarySerialize.DeepClone(model.Shipper);

                    if (testReturnValue.Obj2 != null)
                    {
                        PocoToPoco.Map<TestShipperViweModel, ShipperViweModel>(
                            (TestShipperViweModel)testReturnValue.Obj2, svm,
                            // mapの書き方は、Key-Valueでdst-srcのproperty field名を書く
                            new Dictionary<string, string>()
                            {
                                { "ShipperID", "_ShipperID"},
                                { "CompanyName", "_CompanyName"},
                                { "Phone", "_Phone"}
                            });

                        Debug.WriteLine("svm:" + ObjectInspector.Inspect(svm));
                    }

                    // テスト２
                    tsvm = PocoToPoco.Map<ShipperViweModel, TestShipperViweModel>(
                        (ShipperViweModel)testReturnValue.Obj, null,
                            // mapの書き方は、Key-Valueでdst-srcのproperty field名を書く
                            new Dictionary<string, string>()
                            {
                                { "_ShipperID", "ShipperID"},
                                { "_CompanyName", "CompanyName"},
                                { "_Phone", "Phone"}
                            });
                    Debug.WriteLine("tsvm:" + ObjectInspector.Inspect(tsvm));
                    
                    #endregion

                    model.Shipper = (ShipperViweModel)testReturnValue.Obj;
                }
            }

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを追加する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert(CrudViweModel model)
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

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを更新する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CrudViweModel model)
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

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを削除する
        /// </summary>
        /// <param name="model">CrudViweModel</param>
        /// <returns>再描画（ViewResult）</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CrudViweModel model)
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

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
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
            int? temp = HttpContext.Session.GetInt32("cnt");
            if (temp.HasValue)
            {
                HttpContext.Session.SetInt32("cnt", temp.Value + 1); 
            }
            else
            {
                HttpContext.Session.SetInt32("cnt", 1);
            }
            temp = HttpContext.Session.GetInt32("cnt");

            model.Message = "PreventDoubleSubmission:" + temp.Value.ToString();

            // 再表示（Html.BeginFormでは、全体更新。
            return View("Index", model);
        }

        /// <summary>画面遷移する</summary>
        /// <returns>画面遷移のためのRedirectResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transitions()
        {
            return Redirect(Url.Action("Index", "Crud2"));
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