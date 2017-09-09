using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static MachineSetup.Global;

namespace MachineSetup
{
    [Setup("Chocolatey")]
    public class ChocolateySetup : ISetup
    {
        [SetupOption("Install Script URL")]
        public string ChocoInstallScriptUrl = @"https://chocolatey.org/install.ps1";

        public void Run(SetupContext context)
        {
            if(!context.IsChocolateyInstalled())
            {
                string chocoInstallScript = context.DownloadString("Chocolatey install script URL", ChocoInstallScriptUrl);
                context.ExecutePowershell(chocoInstallScript);

                context.ExecuteChocolatey("feature", "enable", "-n=allowGlobalConfirmation");
            }

            //string chocoSavePath = context.CreateSaveDir("chocolatey");
            //string chocoPackagesFilePath = Path.Combine(chocoSavePath, "choco-packages.config");
            //File.WriteAllText(chocoPackagesFilePath, Resources.choco_packages, Encoding.UTF8);
            //ExecuteChocolatey(context, ChocoExePath, "install", chocoPackagesFilePath);
        }
    }
}
