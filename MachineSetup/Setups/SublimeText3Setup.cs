using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MachineSetup
{
    using static Global;

    [Setup("Sublime Text 3")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class SublimeText3Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "sublimetext3");

            // TODO: Investigate whether the following choco pacakge is worth
            // it or not. Sublime offers to install packagecontrol in Ctrl+P
            // afterall.
            //context.ExecuteChocolatey("install", "sublimetext3.packagecontrol");
        }
    }

    [Setup("Sublime Text 3 - Packages")]
    [SetupDependency(typeof(SublimeText3Setup))]
    public class SublimeText3PackagesSetup : ISetup
    {
        [SetupOption("AppData Path")]
        public string SublimeAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sublime Text 3");

        [SetupOption("Packages Path")]
        public string SublimePackagesPath => Path.Combine(SublimeAppDataPath, "Packages");

        [SetupOption("Installed Packages Path")]
        public string SublimeInstalledPackagesPath => Path.Combine(SublimeAppDataPath, "Installed Packages");

        [SetupOption("Git packages")]
        public List<string> GitPackagesToInstallAfterwards { get; set; } = new List<string>
        {
            @"https://github.com/Manuzor/SublimeText3Settings.git",
            @"https://github.com/Manuzor/Emvee.git",
        };

        public void Run(SetupContext context)
        {
            string packagesPath = SublimePackagesPath;
            if(!Directory.Exists(packagesPath))
                Directory.CreateDirectory(packagesPath);

            // Download git packages.
            foreach(string url in GitPackagesToInstallAfterwards)
            {
                string repoName = GetFileNameFromUrl(url, withExtension: false);
                string clonePath = Path.Combine(packagesPath, repoName);
                CloneGitRepository(context, GitExePath, url, clonePath);

                // HACK
                if(repoName.Trim().ToLower() == "sublimetext3settings")
                {
                    context.ExecutePowershell("-File", Path.Combine(clonePath, "AddPackages.ps1"));
                }
            }
        }
    }
}
