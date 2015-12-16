using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleOptWatcher
{
    enum MWReturnCodes
    {
        MWR_SUCCESS,
        MWR_TECHNICAL_DIFFICULTIES,
        MWR_UNKNOWN_ERROR,
    };

    enum MWColumns : int
    {
        MWC_Symbol = 0,
        MWC_Company = 1,
        MWC_Last = 2,
        MWC_Change = 3,
        MWC_Volume = 4,
        MWC_Option_Volume = 5,
        MWC_Average_Option_Volume = 6,
        MWC_Option_Volume_Change = 7,
        MWC_Action = 8,
    };

    class MWManager
    {
        //Private member variables
        private string HTMLCode;

        // External Managers
        public URLDownloader    Downloader_2;
        public HTMLParser       MyHTMLParser;
        public DateAnalyzer     MyDateAnalyzer;

        //------------------------------

        public MWManager()
        {
            Downloader_2 = new URLDownloader();
            MyHTMLParser = new HTMLParser();
            MyDateAnalyzer = new DateAnalyzer();
        }

        private void ConsoleWrite(ConsoleColor aColor, string aText, object arg0 )
        {
            Console.ForegroundColor = aColor;
            Console.WriteLine(aText, arg0);
        }
        private void ConsoleWrite(ConsoleColor aColor, string aText)
        {
            Console.ForegroundColor = aColor;
            Console.WriteLine(aText);
        }

        private string GetLogFilefullPathName()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "", "Marketwatch.txt");
        }

        private void _AddLogLineToFileAndConsole(ConsoleColor aColor, string aLine)
        {
            string LogFilePathAndFileName = GetLogFilefullPathName();
            StreamWriter file2;
            file2 = File.AppendText(LogFilePathAndFileName);
            file2.WriteLine(aLine);
            file2.Close();

            ConsoleWrite(aColor, aLine);
        }

        private void AddLogLineToFileAndConsole(ConsoleColor aColor, string aLine)
        {
            _AddLogLineToFileAndConsole(aColor, aLine);
        }

        private void AddLogLineToFileAndConsole(ConsoleColor aColor, string aLine, object arg0)
        {
            string Out = @" ";
            Out = String.Format(aLine, arg0);
            _AddLogLineToFileAndConsole(aColor, Out);
        }

        private void AddLogLineToFileAndConsole(ConsoleColor aColor, string aLine, object arg0, object arg1)
        {
            string Out = @" ";
            Out = String.Format(aLine, arg0, arg1);
            _AddLogLineToFileAndConsole(aColor, Out);
        }

        private void AddLogLineToFileAndConsole(ConsoleColor aColor, string aLine, object arg0, object arg1, object arg2)
        {
            string Out = @" ";
            Out = String.Format(aLine, arg0, arg1, arg2);
            _AddLogLineToFileAndConsole(aColor, Out);
        }


        public MWReturnCodes StartProcess()
        {
            AddLogLineToFileAndConsole(ConsoleColor.Gray, "Process starting at {0:HH:mm:ss}", DateTime.Now );

            //Download the HTML page
            HTMLCode = Downloader_2.HTTPPageDownloader("http://www.marketwatch.com/optionscenter/screener?screen=2&displaynum=200");
            System.Threading.Thread.Sleep(1000); //Wait for the download

            //Technical difficulties??
            if (MyHTMLParser.IsThereProblem(HTMLCode) == true) 
            {
                AddLogLineToFileAndConsole(ConsoleColor.Red, "-Technical Difficulties! {0}", DateTime.Now);

                return MWReturnCodes.MWR_TECHNICAL_DIFFICULTIES;
            }

            //download successful, no technicial difficulties
            if (MyDateAnalyzer.IsDateCorrect(MyHTMLParser.ExtractDate(HTMLCode)))
            { //Mai datum kiirasa
                AddLogLineToFileAndConsole(ConsoleColor.Gray, "The webpage date is today: {0}.{1}.{2}.", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            { //Hibas a datum!
                AddLogLineToFileAndConsole(ConsoleColor.Red, "The webpage date is not today.");
            }

            AddLogLineToFileAndConsole(ConsoleColor.Gray, "\nName\tLast\tChange\tVolume\tOptV\tAvrgOV\tOVChange");

            string[] words = MyHTMLParser.TableSplitter(MyHTMLParser.ExtractTable(HTMLCode));

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
                for (MWColumns j = MWColumns.MWC_Last; j < MWColumns.MWC_Action; j++)
                {
                    int _multiplier = 1;

                    int tmpNamefirstposCycle = tdLines[(int)j].LastIndexOf(">");
                    string tmpNameStrCycle = tdLines[(int)j].Substring(tmpNamefirstposCycle + 1);
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

                    if (_Ploc != -1)
                        tmpNameStrCycle = tmpNameStrCycle.Substring(0, _Ploc);

                    tmpNameStrCycle = tmpNameStrCycle.Replace(".", ",");
                    float fval = float.Parse(tmpNameStrCycle) * _multiplier;
                    _MyTempFloat[((int)j) - 2] = fval;

                    if ((j == MWColumns.MWC_Option_Volume_Change) && (fval >= 200))
                    {
                        _percentGood = true;
                    }

                    if ((j == MWColumns.MWC_Option_Volume) && (fval >= 4000))
                    {
                        _volumeGood = true;
                    }

                }

                if ((_percentGood == true) && (_volumeGood == true))
                {
                    AddLogLineToFileAndConsole(ConsoleColor.Gray, "{0} \t {1}", _Tickername, String.Join(" \t ", _MyTempFloat) );
                }
                
            }

            // Name Last Change  Volume Option Volume Average Option Volume   Option Volume Change
            AddLogLineToFileAndConsole(ConsoleColor.Gray, "Name\tLast\tChange\tVolume\tOptV\tAvrgOV\tOVChange");
            AddLogLineToFileAndConsole(ConsoleColor.Gray, "========================^{0}^==========================", DateTime.Now);

            return MWReturnCodes.MWR_SUCCESS;
        }

    }
}
