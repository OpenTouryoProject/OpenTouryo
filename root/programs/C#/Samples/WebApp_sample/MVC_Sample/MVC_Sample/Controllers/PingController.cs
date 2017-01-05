//**********************************************************************************
//* Ping Controller
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：PingController
//* クラス日本語名  ：Ping Controller for Html.BeginForm
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/12/28  Sai               Added controller to prevent session timeout.
//*  2016/01/12  Sai               Removed method Ping and modified Index method to return empty string.
//*  2016/01/12  Sai               Modfied XML comments.
//**********************************************************************************

//System
using System.Web.Mvc;

namespace MVC_Sample.Controllers
{
    /// <summary>
    /// Ping Controller
    /// </summary>
    public class PingController : Controller
    {
        //
        // GET: /Ping/

        /// <summary>
        /// 画面の初期表示
        /// </summary>
        /// <returns>空の結果を返す (EmptyResult)</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}
