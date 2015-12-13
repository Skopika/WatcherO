using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleOptWatcher
{
    class DateAnalyzer
    {
        public Boolean IsDateCorrect(string[] _dateParts)
        {
            if (DateTime.Now.Year == Int32.Parse(_dateParts[2]) && DateTime.Now.Day == Int32.Parse(_dateParts[1]) && DateTime.Now.Month == Int32.Parse(_dateParts[0]))
            {
                /* MyWriteForegroundColor(ConsoleColor.Yellow);
                 Console.WriteLine("A datum mai {0}.{1}.{2}.", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                 MyWriteForegroundColor(ConsoleColor.Red);*/
                return true;
            }
           /* else
            {
                MyWriteForegroundColor(ConsoleColor.Yellow);
                Console.WriteLine("A datum Hibas!!!! {0}, {1}/{2},{3}/{4},{5}/{6}", _TmpStrDate, DateTime.Now.Year, Int32.Parse(_dateParts[2]), DateTime.Now.Month, Int32.Parse(_dateParts[0]), DateTime.Now.Day, Int32.Parse(_dateParts[1]));
                MyWriteForegroundColor(ConsoleColor.Red);
            }*/


            return false;
        }
    }
}
