using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MachineSetup
{
    using static Global;

    public class VisualStudioSetup
    {
        public string DownloadUrl_CommunityEdition = @"https://aka.ms/vs/15/release/vs_community.exe";

        public void Run(SetupContext context)
        {
            string installerDir = Path.Combine(context.SavePath, "vs");
            string downloadUrl = DownloadUrl_CommunityEdition;
            string name = Path.GetFileName(downloadUrl);
            string installerPath = Path.Combine(installerDir, name);
            context.DownloadFile("Visual Studio 2017 Community installer", downloadUrl, installerPath);

            ProcessStartInfo startInfo = new ProcessStartInfo(installerPath)
            {
                Arguments = ToProcessArgumentsString(
                    "--passive",
                    "--norestart",
                    "--addProductLang", "en-US",
                    "--wait"),
            };

            if(context.InstallEnabled)
            {
                context.RunProcess(startInfo);
            }
        }
    }
}
