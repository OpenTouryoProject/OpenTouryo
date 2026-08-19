//**********************************************************************************
//* テスト・コントローラー
//**********************************************************************************

// テスト・コントローラーなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：ValuesController
//* クラス日本語名  ：疎通確認用
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/09/07  西野 大介         新規作成
//**********************************************************************************

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace ASPNETWebServiceCore.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// GET api/values/get
        /// </summary>
        /// <returns>
        /// IEnumerable(string)
        /// </returns>
        [HttpGet]
        public IEnumerable<string> get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
