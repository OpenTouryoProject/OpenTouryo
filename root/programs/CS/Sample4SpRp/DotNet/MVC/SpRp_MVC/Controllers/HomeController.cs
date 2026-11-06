#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：HomeController
//* クラス日本語名  ：フローを選ぶ画面
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668）
//**********************************************************************************

using Microsoft.AspNetCore.Mvc;

namespace SpRp_MVC.Controllers
{
    /// <summary>フローを選ぶ画面</summary>
    public class HomeController : Controller
    {
        /// <summary>フローの一覧</summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
