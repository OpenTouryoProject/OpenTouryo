using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ExecOnLinux.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            EncAndDecUtilCUI.Program.hogehoge(null);
            return View("Index");
        }
    }
}
