using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MachineSetup
{
    public class SetupContext
    {
        const int DOWNLOAD_PROGRESS_INDICATOR_SIZE = 40;
        DateTime DownloadStartTime;
        DateTime DownloadLastReport;
        long DownloadLastReceivedBytes;

        public void DownloadFile(string friendlyName, string url, string destinationFile)
        {
            FileInfo dest = new FileInfo(destinationFile);
            if(!dest.Directory.Exists)
            {
                dest.Directory.Create();
            }

            DownloadWrapper(friendlyName, (client) =>
            {
                client.DownloadFileTaskAsync(new Uri(url), destinationFile).Wait();
            });
        }

        public string DownloadString(string friendlyName, string url)
        {
            string result = null;
            DownloadWrapper(friendlyName, (client) =>
            {
                Task<string> task = client.DownloadStringTaskAsync(new Uri(url));
                task.Wait();
                result = task.Result;
            });

            return result;
        }

        private void DownloadWrapper(string friendlyName, Action<WebClient> downloadAction)
        {
            using(WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)");
                client.DownloadProgressChanged += (o, e) => HandeDownloadProgress(friendlyName, e.BytesReceived, e.TotalBytesToReceive);

                Console.WriteLine($"Downloading {friendlyName}");
                DownloadStartTime = DateTime.Now;
                DownloadLastReceivedBytes = 0;

                downloadAction(client);

                DownloadStartTime = DateTime.MinValue;

                Console.WriteLine();
            }
        }

        private void HandeDownloadProgress(string id, long bytesReceived, long totalBytesToReceive)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceStart = DownloadLastReport - DownloadStartTime;
            TimeSpan deltaTime = DownloadLastReport - DownloadStartTime;

            // TODO
            double bytesPerSecond = 0;

            double percentage = (double)bytesReceived / totalBytesToReceive;

            const int gap = 2;
            int charsDone = (int)(percentage * (DOWNLOAD_PROGRESS_INDICATOR_SIZE - gap));
            if(charsDone < 0)
                charsDone = 0;
            string done = new string('=', charsDone);
            string remaining = new string('-', DOWNLOAD_PROGRESS_INDICATOR_SIZE - charsDone - gap);
            Debug.Assert(done.Length + remaining.Length + gap == DOWNLOAD_PROGRESS_INDICATOR_SIZE);

            char chaser = bytesReceived < totalBytesToReceive ? '>' : '=';
            Console.Write($"\r[{done}{chaser}{remaining}] {percentage.ToString("P2")} {(bytesPerSecond / 1024).ToString("N2")} KiB/s");

            DownloadLastReceivedBytes = bytesReceived;
            DownloadLastReport = now;
        }

        private string _savePath;
        public string SavePath
        {
            get
            {
                if(_savePath == null)
                {
                    string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    _savePath = Path.Combine(commonAppData, "MachineSetup");
                }

                return _savePath;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
#if false
            using(WebClient web = new WebClient())
            {
                web.DownloadProgressChanged += Web_DownloadProgressChanged;
                const string url = @"https://www.python.org/ftp/python/3.6.2/python-3.6.2-amd64-webinstall.exe";
                Task.WaitAll(web.DownloadFileTaskAsync(new Uri(url), "python-3.6.2-amd64-webinstall.exe"));
                Console.WriteLine();
                Console.WriteLine("Done");
            }
#endif

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.WriteLine($"Running {Metadata.Name} v{Metadata.Version}");
            SetupContext context = new SetupContext();

            GitSetup git = new GitSetup();
            git.Run(context);
        }
    }
}
