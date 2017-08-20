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
    public struct NextCloudVersion : IComparable<NextCloudVersion>
    {
        public int Major;
        public int Minor;
        public int Revision;
        public int Build;

        public override string ToString() => $"{Major}.{Minor}.{Revision}.{Build}";

        public string ToSetupFileName() => $"Nextcloud-{this}-setup.exe";

        public static NextCloudVersion? TryParse(string str)
        {
            NextCloudVersion? result = null;

            if(!string.IsNullOrWhiteSpace(str))
            {
                Match match = Regex.Match(str, @"(?<major>\d)\.(?<minor>\d)\.(?<revision>\d)\.(?<build>\d)");
                if(match.Success)
                {
                    result = new NextCloudVersion
                    {
                        Major = int.Parse(match.Groups["major"].Value),
                        Minor = int.Parse(match.Groups["minor"].Value),
                        Revision = int.Parse(match.Groups["revision"].Value),
                        Build = int.Parse(match.Groups["build"].Value),
                    };
                }
            }

            return result;
        }

        public int CompareTo(NextCloudVersion other)
        {
            if(Major < other.Major) return -1;
            if(Major > other.Major) return 1;
            if(Minor < other.Minor) return -1;
            if(Minor > other.Minor) return 1;
            if(Revision < other.Revision) return -1;
            if(Revision > other.Revision) return 1;
            if(Build < other.Build) return -1;
            if(Build > other.Build) return 1;
            return 0;
        }
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
