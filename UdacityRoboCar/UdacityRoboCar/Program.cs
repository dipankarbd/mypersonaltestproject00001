using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdacityRoboCar
{
    class Program
    {
        static void Main(string[] args)
        {
            Localization2 localization = new Localization2();
            localization.Execute();

            Console.ReadKey();
        }
    }
}
