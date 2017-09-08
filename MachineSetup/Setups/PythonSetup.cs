using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    using static Global;

    public struct PythonVersion : IComparable<PythonVersion>
    {
        public int Major;
        public int Minor;
        public int Revision;

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(Major);
            if(Major >= 0 && Minor >= 0)
            {
                result.Append('.').Append(Minor);
                if(Revision >= 0)
                {
                    result.Append('.').Append(Revision);
                }
            }
            return result.ToString();
        }

        public int CompareTo(PythonVersion other)
        {
            if(this.Major < other.Major) return -1;
            if(this.Major > other.Major) return 1;
            if(this.Minor < other.Minor) return -1;
            if(this.Minor > other.Minor) return 1;
            if(this.Revision < other.Revision) return -1;
            if(this.Revision > other.Revision) return 1;
            return 0;
        }

        public static readonly PythonVersion Default = new PythonVersion
        {
            Major = -1,
            Minor = -1,
            Revision = -1,
        };

        public static PythonVersion Parse(string str)
        {
            PythonVersion result = PythonVersion.Default;

            string[] parts = str.Split('.');
            if(parts.Length > 0) result.Major = int.Parse(parts[0]);
            if(parts.Length > 1) result.Minor = int.Parse(parts[1]);
            if(parts.Length > 2) result.Revision = int.Parse(parts[2]);

            return result;
        }
    }

    public class PythonSetup
    {
        public string VersionFtpPageUrl = @"https://www.python.org/ftp/python/";

        public void Run(SetupContext context)
        {
            // Fetch list of available python versions
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(VersionFtpPageUrl);
            List<PythonVersion> versions = new List<PythonVersion>();
            foreach(HtmlNode anchor in doc.DocumentNode.Descendants("a"))
            {
                string href = anchor.GetAttributeValue("href", null);
                Match match = Regex.Match(href, @"^(\d\.\d+(?:\.\d+)?)/?$");
                if(match.Success)
                {
                    string version = match.Groups[1].Value;
                    PythonVersion pyVersion = PythonVersion.Parse(version);
                    versions.Add(pyVersion);
                }
            }

            versions.Sort();

            string installerDir = Path.Combine(context.SavePath, "python");

            // Python 2
            PythonVersion latest2 = versions.Where(v => v.Major == 2).Last();
            DownloadAndInstall2(context, installerDir, latest2);

            // Python 3
            PythonVersion latest3 = versions.Where(v => v.Major == 3).Last();
            DownloadAndInstall3(context, installerDir, latest3);
        }

        private void DownloadAndInstall2(SetupContext context, string installerDir, PythonVersion version)
        {
            // Construct download URL of the latest version
            string downloadUrl = $"{VersionFtpPageUrl}{version}/python-{version}.amd64.msi";

            // Construct the installer path
            string installerPath = Path.Combine(installerDir, Path.GetFileName(downloadUrl));

            // Download the installer
            context.DownloadFile("Python webinstaller", downloadUrl, installerPath);

            // Run the installer
            string logPath = Path.Combine(installerDir, Path.ChangeExtension(installerPath, ".log"));
            MsiInfo info = MsiInfo.Default;
            info.AdditionalArguments.Add("ALLUSERS=1");
            info.AdditionalArguments.Add("ADDLOCAL=DefaultFeature,Tools");
            ProcessStartInfo processStartInfo = PrepareMsiProcess(MsiExePath, installerPath, info);
            context.RunProcess(processStartInfo);
        }

        private void DownloadAndInstall3(SetupContext context, string installerDir, PythonVersion version)
        {
            // Construct download URL of the latest version
            string downloadUrl = $"{VersionFtpPageUrl}{version}/python-{version}-amd64-webinstall.exe";

            // Construct the installer path
            string installerPath = Path.Combine(installerDir, GetFileNameFromUrl(downloadUrl));

            // Download the installer
            context.DownloadFile("Python webinstaller", downloadUrl, installerPath);

            // Run the installer
            string logPath = Path.Combine(installerDir, Path.ChangeExtension(installerPath, ".log"));
            ProcessStartInfo processStartInfo = new ProcessStartInfo(installerPath)
            {
                Arguments = ToProcessArgumentsString(
                    // See: https://docs.python.org/3.6/using/windows.html

                    // > To skip past the user interaction but still display
                    // > progress and errors, pass the /passive option.
                    "/passive",

                    "/log", logPath,

                    // > Perform a system-wide installation.
                    "InstallAllUsers=1",

                    // > Compile all .py files to .pyc.
                    "CompileAll=1",

                    // > Add install and Scripts directories tho PATH and .PY to PATHEXT
                    // Note: No need since py.exe will be installed in C:\Windows
                    //       But maybe we want this for the PATHEXT part?
                    //"PrependPath=1",

                    // > Create shortcuts for the interpreter, documentation
                    // > and IDLE if installed.
                    "Shortcuts=0"
                    ),
            };

            context.RunProcess(processStartInfo);
        }
    }
}
