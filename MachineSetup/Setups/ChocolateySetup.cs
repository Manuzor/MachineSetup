using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MachineSetup.Global;

namespace MachineSetup
{
    [Setup("Choci setup")]
    public class ChocolateySetup : ISetup
    {
        [SetupOption]
        public string ChocoInstallScriptUrl = @"https://chocolatey.org/install.ps1";

        public void Run(SetupContext context)
        {
            if(!IsChocolateyInstalled(context, ChocoExePath))
            {
                string chocoInstallScript = context.DownloadString("Chocolatey install script URL", ChocoInstallScriptUrl);
                ExecutePowershell(context, PowershellExePath, chocoInstallScript);
            }

            string chocoSavePath = context.CreateSaveDir("chocolatey");
            string chocoPackagesFilePath = Path.Combine(chocoSavePath, "choco-packages.config");
            File.WriteAllText(chocoPackagesFilePath, Resources.choco_packages, Encoding.UTF8);
            ExecuteChocolatey(context, ChocoExePath, "install", chocoPackagesFilePath);
        }
    }
}
