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
    using System.Collections;
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

        public static string GetDisplayName(this ISetup setup)
        {
            Type type = setup.GetType();
            string result;

            SetupAttribute attr = type.GetCustomAttribute<SetupAttribute>();
            if(attr != null && attr.DisplayName != null)
            {
                result = attr.DisplayName;
            }
            else
            {
                result = type.Name;
            }

            return result;
        }

        public static string GetSetupOptionDisplayName(this MemberInfo member)
        {
            string result;

            SetupOptionAttribute option = member.GetCustomAttribute<SetupOptionAttribute>();
            if(option != null && option.DisplayName != null)
            {
                result = option.DisplayName;
            }
            else
            {
                result = member.Name;
            }

            return result;
        }

        public static string PrintSetupOptionValue(object value)
        {
            StringBuilder buffer = new StringBuilder();
            PrintSetupOptionValue(buffer);
            return buffer.ToString();
        }

        public static void PrintSetupOptionValue(StringBuilder buffer, object value)
        {
            bool isString = value is string;
            if(!isString && value is IEnumerable enumerable)
            {
                buffer.Append('[');
                string prefix = string.Empty;
                foreach(object item in enumerable)
                {
                    buffer.Append(prefix);
                    prefix = ", ";
                    PrintSetupOptionValue(buffer, item);
                }
                buffer.Append(']');
            }
            else
            {
                if(isString) buffer.Append('"');
                buffer.Append(value);
                if(isString) buffer.Append('"');
            }
        }
    }

    public enum SetupState
    {
        None,
        Running,
        Success,

        Error,
    }

    public interface ISetup
    {
        void Run(SetupContext context);
    }

    public class SetupAttribute : Attribute
    {
        public string DisplayName;
        public string Description;
        public string[] Links;

        public SetupAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    public class SetupDependencyAttribute : Attribute
    {
        public Type RequiredType;

        public SetupDependencyAttribute(Type requiredType)
        {
            RequiredType = requiredType;
        }
    }

    public class SetupOptionAttribute : Attribute
    {
        public string DisplayName;
        public string Description;

        public SetupOptionAttribute() { }
        public SetupOptionAttribute(string displayName)
        {
            DisplayName = displayName;
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

    public class SetupCandidate
    {
        public enum State
        {
            None,
            Pending,
            Processed,
        }

        public Type SetupType;
        public State CurrentState;

        public SetupCandidate(Type type) { SetupType = type; }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SetupOptionData
    {
        public ISetup SetupInstance;
        public MemberInfo Member;
        public string DisplayName;

        public object Value
        {
            get => Member.GetValue(SetupInstance);
            set => Member.SetValue(SetupInstance, value);
        }

        private string DebuggerDisplay => $"{DisplayName} = {PrintSetupOptionValue(Value)}";
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SetupData
    {
        public ISetup SetupInstance;
        public SetupState State;
        public string DisplayName;
        public SetupOptionData[] Options;
        public SetupData[] DirectDependencies;

        public IEnumerable<SetupData> TransitiveDependencies
        {
            get
            {
                foreach(SetupData dependency in DirectDependencies)
                {
                    foreach(SetupData innerDependency in dependency.TransitiveDependencies)
                        yield return innerDependency;

                    yield return dependency;
                }
            }
        }

        private string DebuggerDisplay => $"{DisplayName} ({SetupInstance.GetType()})";
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

            Stopwatch perfTotal = Stopwatch.StartNew();

            Console.WriteLine("Gathering setups...");
            Stopwatch perfGathering = Stopwatch.StartNew();
            List<SetupCandidate> setupCandidates =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 from type in assembly.GetTypes()
                 where type.IsDefined(typeof(SetupAttribute))
                 select new SetupCandidate(type)
                 ).ToList();
            perfGathering.Stop();
            Console.WriteLine($"Gathered all setup types in {perfGathering.Elapsed.TotalSeconds:.000}s.");

            List<SetupData> dataList = new List<SetupData>();
            Console.WriteLine("Resolving setup dependencies...");
            Stopwatch perfDependencies = Stopwatch.StartNew();
            foreach(SetupCandidate candidate in setupCandidates)
            {
                ProcessSetupCandidate(dataList, setupCandidates, candidate);
            }
            perfDependencies.Stop();
            Console.WriteLine($"Resolved setup dependencies in {perfDependencies.Elapsed.TotalSeconds:.000}s");

            foreach(SetupData setup in dataList)
            {
                if(setup.TransitiveDependencies.All(s => s.State != SetupState.Error))
                {
                    Console.WriteLine($"---[{setup.DisplayName}]---------");
                    Stopwatch perfExec = Stopwatch.StartNew();

                    foreach(SetupOptionData option in setup.Options)
                    {
                        StringBuilder buffer = new StringBuilder();
                        PrintSetupOptionValue(buffer, option.Value);
                        Console.WriteLine($"{option.DisplayName} = {buffer.ToString()}");
                    }

                    setup.State = SetupState.Running;
                    try
                    {
                        //setup.SetupInstance.Run(context);
                        //System.Threading.Thread.Sleep(100);

                        setup.State = SetupState.Success;
                    }
                    catch
                    {
                        setup.State = SetupState.Error;
                    }

                    perfExec.Stop();
                    Console.WriteLine($"Finished [{setup.DisplayName}] in {perfExec.Elapsed:hh\\:mm\\:ss\\.fff}");
                }
            }

            //Environment.SetEnvironmentVariable("PATH", TODO, EnvironmentVariableTarget.User);

            perfTotal.Stop();
            Console.WriteLine($"Total execution time: {perfTotal.Elapsed:hh\\:mm\\:ss\\.fff}");
        }

        static void ProcessSetupCandidate(List<SetupData> result, List<SetupCandidate> all, SetupCandidate candidate)
        {
            if(candidate.CurrentState == SetupCandidate.State.Pending)
            {
                throw new InvalidOperationException("Cyclic dependency detected.");
            }

            if(candidate.CurrentState != SetupCandidate.State.Processed)
            {
                candidate.CurrentState = SetupCandidate.State.Pending;

                Debug.Assert(typeof(ISetup).IsAssignableFrom(candidate.SetupType), $"Type '{candidate.SetupType.FullName}' is decorated with {nameof(SetupAttribute)} but does not implement the {nameof(ISetup)} interface.");
                Debug.Assert(candidate.SetupType.IsClass && !candidate.SetupType.IsAbstract, $"Type '{candidate.SetupType.FullName}' must be an instantiable class-type.");

                List<SetupData> dependencies = new List<SetupData>();

                foreach(SetupDependencyAttribute attr in candidate.SetupType.GetCustomAttributes<SetupDependencyAttribute>())
                {
                    SetupData depData = result.FirstOrDefault(c => c.SetupInstance.GetType() == attr.RequiredType);
                    if(depData == null)
                    {
                        SetupCandidate depCandidate = all.Single(c => c.SetupType == attr.RequiredType);
                        ProcessSetupCandidate(result, all, depCandidate);
                        depData = result.FirstOrDefault(c => c.SetupInstance.GetType() == attr.RequiredType);

                        if(depData == null)
                        {
                            throw new NotImplementedException("Unresolved dependency detected.");
                        }
                    }

                    dependencies.Add(depData);
                }

                ISetup instance = (ISetup)Activator.CreateInstance(candidate.SetupType);

                SetupData data = new SetupData
                {
                    SetupInstance = instance,
                    DisplayName = instance.GetDisplayName(),
                    Options = (
                        from member in candidate.SetupType.GetMembers(MemberTypes.Field | MemberTypes.Property)
                        where member.IsDefined(typeof(SetupOptionAttribute))
                        select new SetupOptionData { SetupInstance = instance, Member = member, DisplayName = member.GetSetupOptionDisplayName() }
                    ).ToArray(),
                    DirectDependencies = dependencies.ToArray(),
                };

                result.Add(data);
                candidate.CurrentState = SetupCandidate.State.Processed;
            }
        }
    }
}
