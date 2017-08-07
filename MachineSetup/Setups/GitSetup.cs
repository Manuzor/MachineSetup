using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MachineSetup
{
    public class GitSetup
    {
        public string GitHubOwner = "git-for-windows";
        public string GitHubRepo = "git";

        public string LatestPortablePattern { get; set; } = @"PortableGit-.*-64-bit\.7z\.exe$";

        public void Run(SetupContext context)
        {
            // TODO: Download via context in GetLatestRelease?
            GitHubRelease latestRelease = GitHubApi.GetLatestRelease(context, GitHubApi.DefaultApiUrl, GitHubOwner, GitHubRepo);
            GitHubReleaseAsset asset = latestRelease.Assets.Where(r => Regex.IsMatch(r.BrowserDownloadUrl, LatestPortablePattern)).FirstOrDefault();

            string gitInstallerPath = Path.Combine(context.SavePath, Path.GetFileName(asset.BrowserDownloadUrl));
            context.DownloadFile("git installer", asset.BrowserDownloadUrl, gitInstallerPath);

            // TODO: Execute

        }
    }
}
