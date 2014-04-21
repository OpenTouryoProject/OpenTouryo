using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustCtrl_sample
{
    public class Bean
    {
        public decimal AAA { set; get; }
        public DateTime BBB { set; get; }
        public string CCC { set; get; }

        public Bean(decimal aaa, DateTime bbb, string ccc)
        {
            this.AAA = aaa;
            this.BBB = bbb;
            this.CCC = ccc;
        }
    }
}
