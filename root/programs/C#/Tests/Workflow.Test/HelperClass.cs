using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WorkflowTest
{
   public static class HelperClass
    {

    public static  DataTable dt = null;
    public static string WorkflowControlNumber = Guid.NewGuid().ToString();
    }
}
