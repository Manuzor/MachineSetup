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
                context.RunProcess(processStartInfo);
            }
        }
    }
}
