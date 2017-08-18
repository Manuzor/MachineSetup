using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static partial class Global
{
    public struct NextCloudVersion
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
                Match match = Regex.Match(str, @"(?<major>\d)\.(?<minor>\d)\.(?<revision>\d)\.(?<build>\d)\.");
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
    }
}

namespace MachineSetup
{
    public class NextCloudSetup
    {
        public string DownloadPageUrl { get; set; } = @"https://download.nextcloud.com/desktop/releases/Windows/";

        public void Run(SetupContext context)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(DownloadPageUrl);

            // TODO: Continue here!
        }
    }
}
