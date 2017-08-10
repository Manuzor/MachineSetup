using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static Global;

namespace MachineSetup
{
    public class GitSetup
    {
        public string GitHubOwner = "git-for-windows";
        public string GitHubRepo = "git";

        public string InstallerPattern = @"^Git-.*-64-bit\.exe$";

        public void Run(SetupContext context)
        {
            GitHubRelease latestRelease = GitHubApi.GetLatestReleaseOnGitHub(context, GitHubApi.DefaultApiUrl, GitHubOwner, GitHubRepo);
            IEnumerable<GitHubReleaseAsset> matches =
                latestRelease.Assets.Where(a => Regex.IsMatch(Path.GetFileName(a.BrowserDownloadUrl),
                                                              InstallerPattern));

            GitHubReleaseAsset asset = matches.FirstOrDefault();

            if(asset != null)
            {
                string gitWorkingDir = Path.Combine(context.SavePath, "git");
                string gitInstallerPath = Path.Combine(gitWorkingDir, Path.GetFileName(asset.BrowserDownloadUrl));
                context.DownloadFile("git installer", asset.BrowserDownloadUrl, gitInstallerPath);

                if(context.InstallEnabled)
                {
                    string installerLogPath = Path.Combine(gitWorkingDir, "installer.log");
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(gitInstallerPath)
                    {
                        // TODO: Extract common API?
                        // NOTE: Install destination is default C:\Program Files
                        Arguments = ToProcessArgumentsString(
                            "/SP-", // Disables the initial prompt
                            "/SILENT", // Only show progress
                            "/SUPPRESSMSGBOXES", // Suppress most message boxes
                            "/NORESTART", // Prevent system restart
                            $"/LOG={installerLogPath}",
                            "/CLOSEAPPLICATIONS", // Close all applications that lock required files
                            "/TASKS=!desktopicon" // Prevent desktop icon to be created
                        ),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    context.RunProcess(processStartInfo);
                }
            }
        }
    }
}
