using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MachineSetup
{
    using static Global;

    public class FirefoxSetup
    {
        public string DownloadPageUrl = @"https://www.mozilla.org/firefox/all/";
        public string InstallerUrl = @"https://download.mozilla.org/?product=firefox-55.0.1-SSL&os=win64&lang=en-US";

        public void Run(SetupContext context)
        {
            // Find the download URL.
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);
            HtmlNode anchorNode = doc.DocumentNode.SelectSingleNode(@"//*[@id='en-US']/td[contains(@class, 'win64')]/a");
            string downloadUrl = anchorNode.GetAttributeValue("href", def: null);

            // Determine the installer path.
            string installerDir = Path.Combine(context.SavePath, "firefox");
            Match nameMatch = Regex.Match(downloadUrl, @"\bproduct=(.*?)(?:-SSL)?&");
            string name = nameMatch.Groups[1].Value;
            name = Path.ChangeExtension(name, ".exe");
            string installerPath = Path.Combine(installerDir, name);

            // Download the installer.
            context.DownloadFile("Firefox installer", downloadUrl, installerPath);

            ProcessStartInfo startInfo = new ProcessStartInfo(installerPath)
            {
                Arguments = "-ms", // Silent install.
            };

            if(context.InstallEnabled)
            {
                context.RunProcess(startInfo);
            }
        }
    }
}
