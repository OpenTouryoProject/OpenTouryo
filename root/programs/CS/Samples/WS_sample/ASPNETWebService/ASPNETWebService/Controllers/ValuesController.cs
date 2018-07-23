using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASPNETWebService.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values/get
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}