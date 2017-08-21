using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using static Global;

namespace MachineSetup
{
    public partial struct GimpVersion
    {
        public string ToSetupFileName() => $"gimp-{this}-setup.exe";
    }

    public class GimpSetup
    {
        public string DownloadPageUrl { get; set; } = @"https://download.gimp.org/mirror/pub/gimp/stable/windows/";

        public void Run(SetupContext context)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);
            List<GimpVersion> versions =
                doc.DocumentNode.Descendants("a")
                                .Select(a => GimpVersion.TryParse(a.GetAttributeValue("href", null)))
                                .Where(v => v != null)
                                .Select(v => v.Value)
                                .Distinct()
                                .ToList();

            versions.Sort();

            GimpVersion latestVersion = versions.Last();
            string name = latestVersion.ToSetupFileName();
            string installerDir = Path.Combine(context.SavePath, "gimp");
            string installerPath = Path.Combine(installerDir, name);

            string url = $"{DownloadPageUrl}{name}";
            context.DownloadFile("Gimp installer", url, installerPath);

            InnoSetupInfo info = InnoSetupInfo.Default;
            info.LogPath = Path.Combine(installerDir, "installer.log");
            ProcessStartInfo processStartInfo = PrepareInnoSetupProcess(installerPath, info);

            if(context.InstallEnabled)
            {
                context.RunProcess(processStartInfo);
            }
        }
    }
}
