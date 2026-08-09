using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Touryo.Infrastructure.Public.Str;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(StringConverter.ToZenkaku("abc"));
        }
    }
}
