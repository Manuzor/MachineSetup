using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    using static Global;

    public class GitSetup
    {
        public string GitHubOwner = "git-for-windows";
        public string GitHubRepo = "git";

        public string DefaultGitConfig = @"


";

        public string InstallerPattern = @"^Git-.*-64-bit\.exe$";

        public void Run(SetupContext context)
        {
            GitHubRelease latestRelease = GitHubApi.GetLatestReleaseOnGitHub(context, GitHubApi.DefaultApiUrl, GitHubOwner, GitHubRepo);
            IEnumerable<GitHubReleaseAsset> matches =
                latestRelease.Assets.Where(a => Regex.IsMatch(GetFileNameFromUrl(a.BrowserDownloadUrl),
                                                              InstallerPattern));

            GitHubReleaseAsset asset = matches.FirstOrDefault();

            if(asset != null)
            {
                string gitWorkingDir = Path.Combine(context.SavePath, "git");
                string gitInstallerPath = Path.Combine(gitWorkingDir, GetFileNameFromUrl(asset.BrowserDownloadUrl));
                context.DownloadFile("git installer", asset.BrowserDownloadUrl, gitInstallerPath);

                if(context.InstallEnabled)
                {
                    string installerLogPath = Path.Combine(gitWorkingDir, "installer.log");

                    InnoSetupInfo setupInfo = InnoSetupInfo.Default;
                    setupInfo.LogPath = installerLogPath;
                    ProcessStartInfo processStartInfo = PrepareInnoSetupProcess(gitInstallerPath, setupInfo);
                    context.RunProcess(processStartInfo);
                }

                // Write .gitconfig
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string gitconfigPath = Path.Combine(home, ".gitconfig");
                File.WriteAllText(gitconfigPath, Resources.gitconfig, Encoding.UTF8);
            }
        }
    }
}
