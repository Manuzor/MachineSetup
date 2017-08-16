using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Global;

namespace MachineSetup
{
    public class SublimeText3Setup
    {
        public string DownloadPageUrl = @"https://www.sublimetext.com/3";
        public string PackageControlDownloadUrl = @"https://packagecontrol.io/Package%20Control.sublime-package";

        public string SublimeAppData
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string result = Path.Combine(appData, "Sublime Text 3");
                return result;
            }
        }

        public List<string> GitPackagesToInstallAfterwards { get; set; } = new List<string>();

        public void Run(SetupContext context)
        {
            // Get the download URL
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);
            HtmlNode listNode = doc.DocumentNode.Descendants("li").Where(n => n.Id == "dl_win_64").Single();
            string downloadUrl = listNode.Element("a").GetAttributeValue("href", null);

            // Determine the installer path
            string fileName = Path.GetFileName(downloadUrl);
            string installerDir = Path.Combine(context.SavePath, "subl");
            string installerPath = Path.Combine(installerDir, fileName);

            // Download the installer
            context.DownloadFile(friendlyName: fileName, url: downloadUrl, destinationFile: installerPath);

            // Prepare the installer
            InnoSetupInfo setupInfo = InnoSetupInfo.Default;
            setupInfo.LogPath = Path.Combine(installerDir, "installer.log");
            ProcessStartInfo processStartInfo = PrepareInnoSetupProcess(installerPath, setupInfo);

            // Run the installer
            if(context.InstallEnabled)
            {
                bool installedSuccessfully = false;
                context.RunProcess(processStartInfo, (proc) => installedSuccessfully = proc.ExitCode == 0);

                if(installedSuccessfully)
                {
                    // Package control
                    string installedPackagesPath = Path.Combine(SublimeAppData, "Installed Packages");
                    Directory.CreateDirectory(installedPackagesPath);
                    string packageControlPath = Path.Combine(installedPackagesPath, GetFileNameFromUrl(PackageControlDownloadUrl));
                    context.DownloadFile("Sublime Text 3 Package Control", PackageControlDownloadUrl, packageControlPath);

                    // Download git packages.
                    string packagesPath = Path.Combine(SublimeAppData, "Packages");
                    foreach(string url in GitPackagesToInstallAfterwards)
                    {
                        string repoName = GetFileNameFromUrl(url, withExtension: false);
                        string clonePath = Path.Combine(packagesPath, repoName);
                        CloneGitRepository(context, GitExePath, url, clonePath);

                        // HACK
                        if(repoName.Trim().ToLower() == "sublimetext3settings")
                        {
                            ExecutePowershellScript(context, PowershellExePath, Path.Combine(clonePath, "AddPackages.ps1"));
                        }
                    }
                }
            }
        }
    }
}
