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
    public partial struct NextCloudVersion
    {
        public string ToSetupFileName() => $"Nextcloud-{this}-setup.exe";
    }

    public class NextCloudSetup
    {
        public string DownloadPageUrl { get; set; } = @"https://download.nextcloud.com/desktop/releases/Windows/";

        public void Run(SetupContext context)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);
            List<NextCloudVersion> versions
                = doc.DocumentNode.Descendants("a")
                                  .Select(a => NextCloudVersion.TryParse(a.GetAttributeValue("href", null)))
                                  .Where(v => v != null)
                                  .Select(v => v.Value)
                                  .ToList();

            versions.Sort();

            NextCloudVersion latestVersion = versions.Last();
            string name = latestVersion.ToSetupFileName();
            string installerDir = Path.Combine(context.SavePath, "nextcloud");
            string installerPath = Path.Combine(installerDir, name);

            string url = $"{DownloadPageUrl}{name}";
            context.DownloadFile("NextCloud installer", url, installerPath);

            if(context.InstallEnabled)
            {
                ProcessStartInfo processStartInfo = PrepareNsisProcess(installerPath, NsisInfo.Default);
                context.RunProcess(processStartInfo);
            }
        }
    }
}
