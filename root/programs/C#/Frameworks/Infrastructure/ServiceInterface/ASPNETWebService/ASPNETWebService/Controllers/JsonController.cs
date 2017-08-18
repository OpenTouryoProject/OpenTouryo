using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASPNETWebService.Controllers
{
    public class JsonController : ApiController
    {
        [HttpGet]
        public string test()
        {
            return "test";
        }
    }
}
