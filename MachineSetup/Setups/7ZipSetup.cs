using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Global;

namespace MachineSetup
{
    public partial struct SevenZipVersion
    {
        public bool IsBeta => Flags.HasFlag(VersionFlags.Beta);

        public override string ToString()
        {
            string result = $"{Major}.{Minor.ToString("00")}";
            if(IsBeta)
                result += " beta";
            return result;
        }

        public string ToFileName(string extension, bool x64)
        {
            StringBuilder result = new StringBuilder("7z")
                                             .Append(this.Major)
                                             .Append(this.Minor.ToString("00"));

            if(x64)
                result.Append("-x64");

            if(!extension.StartsWith("."))
                result.Append('.');
            result.Append(extension);

            return result.ToString();
        }

        public static SevenZipVersion? TryParse(string str)
        {
            SevenZipVersion? result = null;
            if(!string.IsNullOrWhiteSpace(str))
            {
                Match match = Regex.Match(str, @"(?<major>\d{1,4})\.(?<minor>\d{1,4})\s*(?<beta>beta)?");
                if(match.Success)
                {
                    result = new SevenZipVersion
                    {
                        Major = int.Parse(match.Groups["major"].Value),
                        Minor = int.Parse(match.Groups["minor"].Value),
                        Flags = match.Groups["beta"].Success ? VersionFlags.Beta : 0,
                    };
                }
            }

            return result;
        }
    }
}

namespace MachineSetup
{
    public class SevenZipSetup
    {
        public string VersionListUrl { get; set; } = @"https://sourceforge.net/projects/sevenzip/files/7-Zip/";
        public bool AllowBeta { get; set; } = true;

        public void Run(SetupContext context)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(VersionListUrl);
            HtmlNode fileListNode = doc.DocumentNode.Descendants("table").Where(e => e.Id == "files_list").Single();
            IEnumerable<string> versionStrings = fileListNode.Element("tbody")
                                                             .Elements("tr")
                                                             .Select(n => n.GetAttributeValue("title", null));
            List<SevenZipVersion> versions = versionStrings.Where(s => !string.IsNullOrWhiteSpace(s))
                                                           .Select(s => SevenZipVersion.TryParse(s))
                                                           .Where(v => v != null)
                                                           .Select(v => v.Value)
                                                           .ToList();
            versions.Sort();
            SevenZipVersion latestVersion;
            if(AllowBeta)
                latestVersion = versions.Last();
            else
                latestVersion = versions.Last(v => !v.IsBeta);

            string fileName = latestVersion.ToFileName("msi", x64: true);
            string url = $@"http://7-zip.org/a/{fileName}";
            string installerPath = Path.Combine(context.SavePath, "7z");
            string savePath = Path.Combine(installerPath, fileName);
            context.DownloadFile("7-Zip installer", url, savePath);

            MsiInfo msi = MsiInfo.Default;
            msi.LogPath = Path.Combine(installerPath, "installer.log");
            ProcessStartInfo startInfo = PrepareMsiProcess(MsiExePath, savePath, msi);

            if(context.InstallEnabled)
            {
                context.RunProcess(startInfo, (proc) =>
                {
                    if(InterpretMsiExitCode(proc) != MsiExitCode.Ok)
                    {
                        throw new Exception("Unable to run MSI installer");
                    }
                });
            }
        }
    }
}
