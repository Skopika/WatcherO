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
        private static DateTime MyRefreshTime = new DateTime();


        static void Main(string[] args)
        {

            //MyRefreshTime = DateTime.Now;
            //MyRefreshTime = MyRefreshTime.AddMinutes(30);
            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 3000;
            //aTimer.Enabled = true;

            TimerCallback();

            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        private static void SetForegroundColor(ConsoleColor MyColor)
        {
            Console.ForegroundColor = MyColor;
        }

        private static void TimerCallback()
        {
            SetForegroundColor(ConsoleColor.Red);
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", DateTime.Now);

            string htmlCode = ""; //nullazzuk a kimeneti valtozot ha esetleg tesztbol felolvassuk a filet amit elozetesen elmentettem


            URLDownloader Downloader_2 = new URLDownloader();
            HTMLParser MyHTMLParser = new HTMLParser();
            DateAnalyzer MyDateAnalyzer = new DateAnalyzer();

            htmlCode = Downloader_2.HTTPPageDownloader("http://www.marketwatch.com/optionscenter/screener?screen=2&displaynum=200");

            System.Threading.Thread.Sleep(1000); //ezt azert tettem bele mert neha varni kellett a weblap adataira

            string LogFilePathAndFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "", "Marketwatch.txt");



            if (MyHTMLParser.IsThereProblem(htmlCode) == true) //Hibauzenetet talaltunk, szopo, nem jött le a tablazat
            {
                //hibauzenetet kaptunk azaz nem jottek le az adatok ujra kell probalkozni 5 perc mulva
                aTimer.Interval = (1000 * 5 * 60);
                if (DateTime.Now > MyRefreshTime)
                {
                    MyRefreshTime.AddMinutes(30);
                }
                //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt");
                StreamWriter file2;// = new StreamWriter(LogFilePathAndFileNAme);
                file2 = File.AppendText(LogFilePathAndFileName);
                file2.WriteLine("Nem jottek le az adatok! {0}", DateTime.Now);

                file2.Close();
                Console.WriteLine("Nem jottek le az adatok! {0}", DateTime.Now);

                return;
            }

            //Minden rendben, nincs hibauzenet
            if (MyDateAnalyzer.IsDateCorrect(MyHTMLParser.ExtractDate(htmlCode)))
            { //Mai datum kiirasa
                SetForegroundColor(ConsoleColor.Yellow);
                Console.WriteLine("A datum mai {0}.{1}.{2}.", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            { //Hibas a datum!
                SetForegroundColor(ConsoleColor.Yellow);
                Console.WriteLine("A datum Hibas!!!!");
            }

            SetForegroundColor(ConsoleColor.Red);

            StreamWriter file;
            if (!File.Exists(LogFilePathAndFileName))
            {
                // Create a file to write to.
                using (file = File.CreateText(LogFilePathAndFileName))
                {
                    file.WriteLine("Name\tLast\tChange\tVolume\tOptV\tAvrgOV\tOVChange");
                    file.Close();
                }
            }
            file = File.AppendText(LogFilePathAndFileName);

            string[] words = MyHTMLParser.TableSplitter(MyHTMLParser.ExtractTable(htmlCode));

            for (int i = 0; i < words.Length - 1; i++)
            {

                //itt kellene darabokra parsolni a mar beolvasott adatok html tablazatat mindenestul

                string[] tdLines = Regex.Split(words[i], "</td>");

                //elso darab a papir neve
                //megkeressuk a </a> -t es addig levagjuk majd visszafele megkeressuk az elso kacsacsort > es a vegeig levagjuk es megkapjuk a nevet
                int tmpNamelastpos = tdLines[0].LastIndexOf("</a>");
                string tmpNameStr = tdLines[0].Substring(0, tmpNamelastpos);
                int tmpNamefirstpos = tmpNameStr.LastIndexOf(">");
                tmpNameStr = tmpNameStr.Substring(tmpNamefirstpos + 1);

                string _Tickername = tmpNameStr;

                //masodik a LAST lesz
                Boolean _percentGood = false;
                Boolean _volumeGood = false;
                float[] _MyTempFloat = new float[6];
                for (int j = 2; j < 8; j++)
                {
                    int _multiplier = 1;

                    int tmpNamefirstposCycle = tdLines[j].LastIndexOf(">");
                    string tmpNameStrCycle = tdLines[j].Substring(tmpNamefirstposCycle + 1);
                    //meg kell nezni hogy az adott szam tartalmaz-e K betut mert akkor az ezres szorzo ill tartalmaz-e % jelet
                    //a % jel tartalmazasa csak annyit jelent hogy azt is le kell nyesni a vegerol hogy szamma tudjuk castolni
                    int _Mloc = tmpNameStrCycle.IndexOf("M");
                    if (_Mloc != -1)
                    {
                        tmpNameStrCycle = tmpNameStrCycle.Substring(0, _Mloc);
                        _multiplier = 1000000;
                    }
                    int _Kloc = tmpNameStrCycle.IndexOf("K");
                    if (_Kloc != -1)
                    {
                        tmpNameStrCycle = tmpNameStrCycle.Substring(0, _Kloc);
                        _multiplier = 1000;
                    }
                    int _Ploc = tmpNameStrCycle.IndexOf("%");
                    if (_Ploc != -1) tmpNameStrCycle = tmpNameStrCycle.Substring(0, _Ploc);
                    tmpNameStrCycle = tmpNameStrCycle.Replace(".", ",");
                    float fval = float.Parse(tmpNameStrCycle) * _multiplier;
                    _MyTempFloat[j - 2] = fval;

                    if (j == 7) { if (fval >= 300) { _percentGood = true; } }
                    if (j == 5) { if (fval >= 4000) { _volumeGood = true; } }

                }

                if (_percentGood == true && _volumeGood == true)
                {
                    Console.WriteLine("{0} \t {1}", _Tickername, String.Join(" \t ", _MyTempFloat));
                    file.WriteLine("{0} \t {1}", _Tickername, String.Join(" \t ", _MyTempFloat));
                }
                // Name Last Change  Volume Option Volume Average Option Volume   Option Volume Change

                Console.WriteLine("Name\tLast\tChange\tVolume\tOptV\tAvrgOV\tOVChange");
                Console.WriteLine("========================^{0}^==========================", DateTime.Now);

                file.WriteLine("Name\tLast\tChange\tVolume\tOptV\tAvrgOV\tOVChange");
                file.WriteLine("========================^{0}^==========================", DateTime.Now);

                file.Close();

                {
                    //Console.WriteLine(htmlBody);
                    // Write the string to a file.
                    //megtortent a levalogatas igy be kell allitanunk a szamlalot a megfelelo ertekre 
                    //hogy a 30 perces frissitesi idopontban ujra frissitsuk az adatokat
                    if (DateTime.Now > MyRefreshTime) //ha ez egy olyan sikeres update volt ami a timer lejartakor keletkezett akkor beallitjuk a kovetkezo timer helyes idopontjat
                    {
                        MyRefreshTime.AddMinutes(30);
                    }
                    //a helyes idopont most mar biztosan a "most" nal kesobbi idopont igy ki tudjuk szamitani a kulonbseget:
                    TimeSpan span = MyRefreshTime.Subtract(DateTime.Now);
                    aTimer.Interval = ((1000 * 60) * span.Minutes) + (span.Seconds * 1000);
                }
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TimerCallback();
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
