using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleOptWatcher
{
    class HTMLParser
    {
        public Boolean IsThereProblem(string htmlCode)
        {
            int _test = htmlCode.IndexOf("problem"); //ha nem jottek le az adatok csak a problema van a lappal hibauzenet}

            if (_test == -1)
            {
                return false;
            }
            return true;
        }

        public string[] ExtractDate(string htmlCode)
        {
            int _datepos = htmlCode.IndexOf("screenerDate\">");
            string _TmpStrDate = htmlCode.Substring(_datepos, 60);

            _datepos = _TmpStrDate.IndexOf(">");
            _TmpStrDate = _TmpStrDate.Substring(_datepos + 1);

            _datepos = _TmpStrDate.IndexOf("</div");
            _TmpStrDate = _TmpStrDate.Substring(0, _datepos);
            string[] _dateParts = Regex.Split(_TmpStrDate, "/"); 
            return _dateParts;
        }
        
        public string ExtractTable(string htmlCode)
        {
            int first = htmlCode.IndexOf("<tr class=\"screenerRow\">");
            string _TmpStr = htmlCode.Substring(first);
            int last = _TmpStr.LastIndexOf("</table>");
            string htmlBody = _TmpStr.Substring(0, last);
            return htmlBody;
        }
        
        public string[] TableSplitter (string htmlBody)
        {
            int TMPfirst = htmlBody.IndexOf("<tr class=\"screenerRow\">");
            htmlBody = htmlBody.Substring(TMPfirst);
            string[] words = Regex.Split(htmlBody, "</tr>");
            return words;
        }

    }

}
