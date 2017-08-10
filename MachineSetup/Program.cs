using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using static Global;

public static partial class Global
{
    public static string EscapeProcessArgument(string argument)
    {
        return argument.Replace("\"", "\"\"\"");
    }

    public static string ToProcessArgumentsString(params string[] args)
    {
        return ToProcessArgumentsString((IEnumerable<string>)args);
    }

    public static string ToProcessArgumentsString(IEnumerable<string> args)
    {
        StringBuilder result = new StringBuilder();
        if(args.Any())
        {
            StringBuilder temp = new StringBuilder();
            foreach(string arg in args)
            {
                temp.Clear();
                temp.Append(arg);
                temp.Replace("\"", "\"\"\"");
                if(arg.Contains(' ') || arg.Contains('\t') || arg.Contains('\n') || arg.Contains('\r'))
                {
                    temp.Insert(0, '"');
                    temp.Append('"');
                }

                result.Append(temp);
                result.Append(' ');
            }

            // Remove trailing space that we inserted.
            result.Length--;
        }

        return result.ToString();
    }
}

namespace MachineSetup
{
    public class SetupContext
    {
        public List<string> UserPath = new List<string>();
        public List<string> MachinePath = new List<string>();

        public bool InstallEnabled { get; set; } = true;

        const int DOWNLOAD_PROGRESS_INDICATOR_SIZE = 40;
        DateTime DownloadStartTime;
        DateTime DownloadLastReport;
        long DownloadLastReceivedBytes;

        public void DownloadFile(string friendlyName, string url, string destinationFile)
        {
            Console.WriteLine($"Downloading {friendlyName}");
            Console.WriteLine(url);

            FileInfo dest = new FileInfo(destinationFile);
            if(dest.Exists)
            {
                Console.WriteLine("File already exists. Skipping download.");
            }
            else
            {
                if(!dest.Directory.Exists)
                {
                    dest.Directory.Create();
                }

                DownloadWrapper(friendlyName, (client) =>
                {
                    client.DownloadFileTaskAsync(new Uri(url), destinationFile).Wait();
                });
            }
        }

        public string DownloadString(string friendlyName, string url)
        {
            Console.WriteLine($"Downloading {friendlyName}");
            Console.WriteLine(url);

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

                DownloadStartTime = DateTime.Now;
                DownloadLastReceivedBytes = 0;

                downloadAction(client);

                DownloadStartTime = DateTime.MinValue;

                Console.WriteLine();
                Console.WriteLine("Download finished!");
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

        public void RunProcess(ProcessStartInfo startInfo, Action<Process> onFinish = null)
        {
            Console.WriteLine("Running process...");
            Console.WriteLine($"{Path.GetFileName(startInfo.FileName)}");
            using(Process proc = Process.Start(startInfo))
            {
                proc.WaitForExit();
                Console.WriteLine($"Process finished with exit code {proc.ExitCode}.");

                onFinish?.Invoke(proc);
            }
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
            Console.WriteLine($"Running {GlobalName} v{GlobalVersion}");

            SetupContext context = new SetupContext()
            {
                UserPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                MachinePath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),

                // For now.
                //InstallEnabled = false,
            };

            SevenZipSetup sevenZip = new SevenZipSetup();
            sevenZip.Run(context);

            GitSetup git = new GitSetup();
            git.Run(context);

            //Environment.SetEnvironmentVariable("PATH", TODO, EnvironmentVariableTarget.User);
        }
    }
}
