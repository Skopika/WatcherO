﻿using System;
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
        private static DateTime MyRefreshTime= new DateTime();


        static void Main(string[] args)
        {

            MyRefreshTime = DateTime.Now;
            MyRefreshTime = MyRefreshTime.AddMinutes(30);
            //System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
            
            
            


            /* // Call the page and get the generated HTML
             var doc = new HtmlAgilityPack.HtmlDocument();
             HtmlAgilityPack.HtmlNode.ElementsFlags["br"] = HtmlAgilityPack.HtmlElementFlag.Empty;
             doc.OptionWriteEmptyNodes = true;

             try
             {
                 var webRequest = HttpWebRequest.Create(pageUrl);
                 Stream stream = webRequest.GetResponse().GetResponseStream();
                 doc.Load(stream);
                 stream.Close();
             }
             catch (System.UriFormatException uex)
             {
                 Log.Fatal("There was an error in the format of the url: " + itemUrl, uex);
                 throw;
             }
             catch (System.Net.WebException wex)
             {
                 Log.Fatal("There was an error connecting to the url: " + itemUrl, wex);
                 throw;
             }

             //get the div by id and then get the inner text 
             string testDivSelector = "//div[@id='test']";
             var divString = doc.DocumentNode.SelectSingleNode(testDivSelector).InnerHtml.ToString();
             */


        }

        private static void MyTime(Boolean MyEmergency)
        {
            if(DateTime.Now > MyRefreshTime)
            {

            }
            if (MyEmergency) { }

        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);

            aTimer.Interval = 1000;

            using (WebClient client = new WebClient())
            {
                string htmlCode = "";
                string[] names;
                float[,] _MyDatas;
                try
                {   // Open the text file using a stream reader. csak a teszt kedveert hogy ne kelljen a netrol beolvasni a dolgokat egy 10es lekerdezes le van mentve a vinyora.
                    using (StreamReader sr = new StreamReader("c:\\test_3.txt"))
                    {
                        // Read the stream to a string, and write the string to the console.
                        htmlCode = sr.ReadToEnd();
                        //Console.WriteLine(line);
                    }
                }
                catch (Exception excp)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(excp.Message);
                }

                //htmlCode = client.DownloadString("http://www.marketwatch.com/optionscenter/screener?screen=2&displaynum=10");

                // This search returns the substring between two strings, so  
                // the first index is moved to the character just after the first string. 
                //int first = htmlCode.IndexOf("<table id=\"screenerTable\" border=\"0\">") + "<table id=\"screenerTable\" border=\"0\">".Length;
                System.Threading.Thread.Sleep(1000); //ezt azert tettem bele mert  neha varni kellett a weblap adataira
                int _test = htmlCode.IndexOf("problem"); //ha nem jottek le az adatok csak a prboelma van a lappal hibauzenet
                Console.WriteLine(_test);
                if (_test == -1) //ha nem talalta a hibauzenetet akkor johet az adatelemzes
                {

                    int first = htmlCode.IndexOf("<tr class=\"screenerRow\">");
                    Console.WriteLine(first);
                    string _TmpStr = htmlCode.Substring(first);
                    int last = _TmpStr.LastIndexOf("</table>");
                    //int last = _TmpStr.IndexOf("</tbody>" + "</tbody>".Length);
                    Console.WriteLine(last);
                    if (last != -1)
                    {
                        string htmlBody = _TmpStr.Substring(0, last);

                        int TMPfirst = htmlBody.IndexOf("<tr class=\"screenerRow\">");
                        htmlBody = htmlBody.Substring(TMPfirst);
                        string[] words = Regex.Split(htmlBody, "</tr>");
                        System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt");
                        //--int i = 0;
                        for (int i = 0; i < words.Length - 1; i++)
                        {
                            file.WriteLine(i);

                            //file.WriteLine(words[i]);
                            //itt kellene darabokra parsolni a mar beolvasott adatok html tablazatat mindenestul

                            string[] tdLines = Regex.Split(words[i], "</td>");
                            //file.WriteLine(tdLines[0]);
                            //elso darab a papir neve
                            //megkeressuk a </a> -t es addig levagjuk majd visszafele megkeressuk az elso kacsacsort > es a vegeig levagjuk es megkapjuk a nevet
                            int tmpNamelastpos = tdLines[0].LastIndexOf("</a>");
                            string tmpNameStr = tdLines[0].Substring(0, tmpNamelastpos);
                            //file.WriteLine(tmpNameStr);
                            int tmpNamefirstpos = tmpNameStr.LastIndexOf(">");
                            tmpNameStr = tmpNameStr.Substring(tmpNamefirstpos + 1);
                            file.WriteLine(tmpNameStr);

                            string _Tickername = tmpNameStr;

                            //masodik a LAST lesz
                            /*
                            tmpNamefirstpos = tdLines[1].LastIndexOf(">");
                            tmpNameStr = tmpNameStr.Substring(tmpNamefirstpos + 1);
                            file.WriteLine(tmpNameStr);
                            */
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
                                file.WriteLine(fval);
                            }

                            // Name Last Change  Volume Option Volume Average Option Volume   Option Volume Change
                        }
                        
                       
                        file.Close();
                    }
                    else
                    {
                        System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt");
                        file.WriteLine(_TmpStr);

                        file.Close();
                    }
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
                    aTimer.Interval = (1000 * 60) * span.Minutes;
                }
                else
                {

                    //hibauzenetet kaptunk azaz nem jottek le az adatok ujra kell probalkozni 5 perc mulva
                    aTimer.Interval = (1000 * 5 * 60);
                    if (DateTime.Now > MyRefreshTime)
                    {
                        MyRefreshTime.AddMinutes(30);
                    }
                    System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt");
                    file.WriteLine("-1");

                    file.Close();
                    Console.WriteLine("-1");
                }
            }
        }
    }
}