using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// ベースクラス
using Touryo.Infrastructure.Business.Common;

namespace MVC_Sample.Logic.Common
{
    public class TestReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>ShipperID</summary>
        public int ShipperID;

        /// <summary>CompanyName</summary>
        public string CompanyName;

        /// <summary>Phone</summary>
        public string Phone;
    }
}