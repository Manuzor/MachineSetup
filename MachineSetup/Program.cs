using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MachineSetup
{
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

        public static bool IsElevatedProcess
        {
            get
            {
                using(WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
        }

        public static string GetFileNameFromUrl(string url, bool withExtension = true)
        {
            string encodedFileName;
            if(withExtension) encodedFileName = Path.GetFileName(url);
            else encodedFileName = Path.GetFileNameWithoutExtension(url);

            string result = WebUtility.UrlDecode(encodedFileName);
            return result;
        }
    }

    public class SetupContext
    {
        public List<string> UserPath = new List<string>();
        public List<string> MachinePath = new List<string>();

        public bool InstallEnabled = true;

        internal List<string> PendingTempFilePaths = new List<string>();

        const int DOWNLOAD_PROGRESS_INDICATOR_SIZE = 40;
        DateTime DownloadStartTime;
        DateTime DownloadLastReport;
        long DownloadLastReceivedBytes;

        internal WebClient CurrentClient;

        public void DownloadFile(string friendlyName, string url, string destinationFile)
        {
            Console.WriteLine($"Downloading {friendlyName}");
            Console.WriteLine(url);

            if(File.Exists(destinationFile))
            {
                Console.WriteLine("File already exists. Skipping download.");
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));

                string tempFile = $"{destinationFile}.part";
                PendingTempFilePaths.Add(tempFile);
                DownloadWrapper(friendlyName, (client) =>
                {
                    client.DownloadFileTaskAsync(new Uri(url), tempFile).Wait();
                });

                File.Move(tempFile, destinationFile);
                PendingTempFilePaths.Remove(tempFile);
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
            try
            {
                using(WebClient client = new WebClient())
                {
                    CurrentClient = client;
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
            finally
            {
                CurrentClient = null;
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
            Console.WriteLine($"\"{Path.GetFileName(startInfo.FileName)}\"");

            // Run with administrative privileges.
            startInfo.Verb = "runas";

            using(Process proc = new Process { StartInfo = startInfo })
            {
                proc.OutputDataReceived += (o, e) => Console.Out.WriteLine(e.Data);
                proc.ErrorDataReceived += (o, e) => Console.Error.WriteLine(e.Data);

                proc.Start();
                if(startInfo.RedirectStandardOutput) proc.BeginOutputReadLine();
                if(startInfo.RedirectStandardError) proc.BeginErrorReadLine();

                proc.WaitForExit();

                if(proc.ExitCode != 0)
                    Console.Error.WriteLine($"Process finished with exit code {proc.ExitCode}.");

                onFinish?.Invoke(proc);
            }
        }

        public string CreateSaveDir(string dir)
        {
            string fullPath = Path.Combine(SavePath, dir);
            if(!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
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
            if(!Debugger.IsAttached && !IsElevatedProcess)
            {
                string message = "You did not start this process with administrative privileges. " +
                    "All setups will ask for administrative privileges individually and you have to click Yes/No every time." +
                    "Do you really want this?";

                Form form = new Form();
                DialogResult dialog =
                    MessageBox.Show(message,
                                    "No Administrative Privileges",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation);
                if(dialog == DialogResult.No)
                {
                    Environment.Exit(1);
                }
            }

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Console.WriteLine($"Running {GlobalName} v{GlobalVersion}");

            SetupContext context = new SetupContext()
            {
                UserPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                MachinePath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
            };

            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;

                if(context.CurrentClient != null)
                {
                    context.CurrentClient.CancelAsync();
                }

                foreach(string tempFilePath in context.PendingTempFilePaths)
                {
                    if(File.Exists(tempFilePath))
                        File.Delete(tempFilePath);
                }
            };

#if DEBUG
            if(Debugger.IsAttached)
                context.InstallEnabled = false;
#endif

            Console.WriteLine($"Save path: {context.SavePath}");

#if false
            VisualStudioSetup vs = new VisualStudioSetup();
            vs.Run(context);
#endif

#if false
            SevenZipSetup sevenZip = new SevenZipSetup();
            sevenZip.Run(context);
#endif

#if false
            GitSetup git = new GitSetup();
            git.Run(context);
#endif

#if false
            SublimeText3Setup subl = new SublimeText3Setup
            {
                GitPackagesToInstallAfterwards = new List<string>
                {
                    @"https://github.com/Manuzor/SublimeText3Settings.git",
                    @"https://github.com/Manuzor/Emvee.git",
                },
            };

            subl.Run(context);
#endif

#if false
            FirefoxSetup firefox = new FirefoxSetup();
            firefox.Run(context);
#endif

#if false
            BeyondCompareSetup bcomp = new BeyondCompareSetup();
            bcomp.Run(context);
#endif

#if false
            PythonSetup py = new PythonSetup();
            py.Run(context);
#endif

#if false
            NextCloudSetup nextcloud = new NextCloudSetup();
            nextcloud.Run(context);
#endif

#if false
      GimpSetup gimp = new GimpSetup();
      gimp.Run(context);
#endif

#if false
      PaintNetSetup paintnet = new PaintNetSetup();
      paintnet.Run(context);
#endif

            if(!IsChocolateyInstalled(context, ChocoExePath))
            {
                const string chocoInstallScriptUrl = @"https://chocolatey.org/install.ps1";
                string chocoInstallScript = context.DownloadString("Chocolatey install script URL", chocoInstallScriptUrl);
                ExecutePowershell(context, PowershellExePath, chocoInstallScript);
            }

            string chocoSavePath = context.CreateSaveDir("chocolatey");
            string chocoPackagesFilePath = Path.Combine(chocoSavePath, "choco-packages.config");
            File.WriteAllText(chocoPackagesFilePath, Resources.choco_packages, Encoding.UTF8);
            ExecuteChocolatey(context, ChocoExePath, "install", chocoPackagesFilePath);

            //Environment.SetEnvironmentVariable("PATH", TODO, EnvironmentVariableTarget.User);
        }
    }
}
