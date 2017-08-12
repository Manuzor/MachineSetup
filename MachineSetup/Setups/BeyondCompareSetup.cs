using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using static Global;
using HtmlAgilityPack;

namespace MachineSetup
{
    public class BeyondCompareSetup
    {
        public string DownloadPageUrl = @"https://www.scootersoftware.com/download.php";

        public void Run(SetupContext context)
        {
            // Get the download URL
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);
            HtmlNode anchorNode = doc.DocumentNode.Descendants("a")
                                                  .Where(n => n.InnerText.Trim().ToLower() == "download")
                                                  .Skip(1)
                                                  .First();
            string downloadUrl = anchorNode.GetAttributeValue("href", null);
            // Fix the URL because it's weird on their website...
            downloadUrl = $"https://www.scootersoftware.com{downloadUrl}";

            // Determine the installer path
            string installerDir = Path.Combine(context.SavePath, "beyondcompare4");
            string installerPath = Path.Combine(installerDir, Path.GetFileName(downloadUrl));

            // Download the file
            context.DownloadFile("Beyond Compare 4 installer", downloadUrl, installerPath);

            // Prepare the installer
            InnoSetupInfo setupInfo = InnoSetupInfo.Default;
            setupInfo.LogPath = Path.Combine(installerDir, "installer.log");
            ProcessStartInfo processStartInfo = PrepareInnoSetupProcess(installerPath, setupInfo);

            // Run the installer
            if(context.InstallEnabled)
            {
                context.RunProcess(processStartInfo);
            }
        }
    }
}
