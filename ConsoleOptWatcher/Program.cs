using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

//using HtmlAgilityPack;
//using Fizzler.Systems.HtmlAgilityPack;

namespace ConsoleOptWatcher
{
    class Program
    {
        //private static System.Timers.Timer aTimer;
        private static System.Timers.Timer aTimer = new System.Timers.Timer();

        static void Main(string[] args)
        {
            if (true)
            {
                MWManager MarketwatchManager = new MWManager();

                while(true)
                {
                    if (MarketwatchManager.StartProcess() == MWReturnCodes.MWR_SUCCESS)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000); //Wait for the download
                }
            }
            else
            {
                /*DateTime MyRefreshTime = DateTime.Now.AddMinutes(30);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = 3000;
                aTimer.Enabled = true;*/
            }

            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        private static void TimerCallback()
        {
            MWManager MarketwatchManager = new MWManager();
            switch(MarketwatchManager.StartProcess())
            {
                case MWReturnCodes.MWR_SUCCESS:
                {
                    DateTime MyRefreshTime = DateTime.Now;
                    //ha ez egy olyan sikeres update volt ami a timer lejartakor keletkezett akkor beallitjuk a kovetkezo timer helyes idopontjat
                    MyRefreshTime.AddMinutes(30);
                    //a helyes idopont most mar biztosan a "most" nal kesobbi idopont igy ki tudjuk szamitani a kulonbseget:
                    TimeSpan span = MyRefreshTime.Subtract(DateTime.Now);
                    aTimer.Interval = ((1000 * 60) * span.Minutes) + (span.Seconds * 1000);
                    break;
                }
                case MWReturnCodes.MWR_TECHNICAL_DIFFICULTIES:
                {
                    break;
                }
            }

        }
    }
}

/*
            //Filebol adatolvasas teszteleshez. nem resze a programnak ifdefelheto eppenseggel ha kell.
            try
            {   // Open the text file using a stream reader. csak a teszt kedveert hogy ne kelljen a netrol beolvasni a dolgokat egy 10es lekerdezes le van mentve a vinyora.
                using (StreamReader sr = new StreamReader("c:\\test_3.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    htmlCode = sr.ReadToEnd();
                }
            }
            catch (Exception excp)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(excp.Message);
            } */


//le kell vizsgalnunk a datumot es ki kell majd irnunk a kovetkezo sorban!!! es kiirni melle hogy warning ha nem mai a datum.
/*int _datepos = htmlCode.IndexOf("screenerDate\">");
string _TmpStrDate = htmlCode.Substring(_datepos, 60);

_datepos = _TmpStrDate.IndexOf(">");
_TmpStrDate = _TmpStrDate.Substring(_datepos+1);

_datepos = _TmpStrDate.IndexOf("</div");
_TmpStrDate = _TmpStrDate.Substring(0,_datepos);
string[] _dateParts = Regex.Split(_TmpStrDate, "/");
if (DateTime.Now.Year== Int32.Parse(_dateParts[2])&& DateTime.Now.Day == Int32.Parse(_dateParts[1]) && DateTime.Now.Month == Int32.Parse(_dateParts[0]))
{
    MyWriteForegroundColor( ConsoleColor.Yellow);
    Console.WriteLine("A datum mai {0}.{1}.{2}.", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    MyWriteForegroundColor (ConsoleColor.Red);
}
else
{
    MyWriteForegroundColor(ConsoleColor.Yellow);
    Console.WriteLine("A datum Hibas!!!! {0}, {1}/{2},{3}/{4},{5}/{6}", _TmpStrDate, DateTime.Now.Year, Int32.Parse(_dateParts[2]), DateTime.Now.Month, Int32.Parse(_dateParts[0]), DateTime.Now.Day, Int32.Parse(_dateParts[1]));
    MyWriteForegroundColor( ConsoleColor.Red);
} */


/*Console.WriteLine("Nincs hibauzenet.Johet az elemzes. ");
               int first = htmlCode.IndexOf("<tr class=\"screenerRow\">");
               string _TmpStr = htmlCode.Substring(first);
               int last = _TmpStr.LastIndexOf("</table>");*/
