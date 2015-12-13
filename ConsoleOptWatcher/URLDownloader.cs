using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleOptWatcher
{
    class URLDownloader
    {
        private WebClient client = new WebClient();

        public string HTTPPageDownloader(string URLtoDownload)
        {
            string htmlCode = client.DownloadString(URLtoDownload);
            return htmlCode;
        }

    }
}
