using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup
{
    public enum GitHubContentType
    {
        Unknown,
        Executable,
        Zip,
        BZip2,
    }

    public static class GitHubContentTypeUtil
    {
        public static string ToApiString(this GitHubContentType type)
        {
            switch(type)
            {
                case GitHubContentType.Unknown: return string.Empty;
                case GitHubContentType.Executable: return "application/executable";
                case GitHubContentType.Zip: return "application/zip";
                case GitHubContentType.BZip2: return "application/x-bzip2";

                default: throw new ArgumentException(nameof(type));
            }
        }

        public static GitHubContentType FromApiString(string str)
        {
            switch(str.ToLower())
            {
                case "application/executable": return GitHubContentType.Executable;
                case "application/zip": return GitHubContentType.Zip;
                case "application/x-bzip2": return GitHubContentType.BZip2;

                default: return GitHubContentType.Unknown;
            }
        }
    }

    public enum GitHubReleaseState
    {
        Unknown,
        Uploaded,
    }

    public static class GitHubReleaseStateUtil
    {
        public static string ToApiString(this GitHubReleaseState state)
        {
            switch(state)
            {
                case GitHubReleaseState.Unknown: return string.Empty;
                case GitHubReleaseState.Uploaded: return "uploaded";

                default: throw new ArgumentException(nameof(state));
            }
        }

        public static GitHubReleaseState FromApiString(string str)
        {
            switch(str.ToLower())
            {
                case "uploaded": return GitHubReleaseState.Uploaded;

                default: return GitHubReleaseState.Unknown;
            }
        }
    }

    public class GitHubReleaseAsset
    {
        public long ID;

        public string RawContentType;
        public GitHubContentType ContentType;

        public string RawState;
        public GitHubReleaseState State;

        public long SizeInBytes;

        public string BrowserDownloadUrl;
    }

    public class GitHubRelease
    {
        public bool IsDraft;
        public bool IsPrerelease;
        public List<GitHubReleaseAsset> Assets = new List<GitHubReleaseAsset>();
    }

    public static class GitHubApi
    {
        public const string DefaultApiUrl = "https://api.github.com";

        public static GitHubRelease GetLatestRelease(SetupContext context, string apiUrl, string owner, string repo)
        {
            string url = $"{apiUrl}/repos/{owner}/{repo}/releases/latest";
            string jsonSource = context.DownloadString("list of latest git releases", url);

            JObject json = JObject.Parse(jsonSource);

            GitHubRelease result = new GitHubRelease
            {
                IsPrerelease = json.Value<bool>("prerelease"),
                IsDraft = json.Value<bool>("draft"),
            };

            foreach(JToken assetToken in json["assets"])
            {
                GitHubReleaseAsset asset = new GitHubReleaseAsset
                {
                    ID = assetToken.Value<long>("id"),
                    RawContentType = assetToken.Value<string>("content_type"),
                    RawState = assetToken.Value<string>("state"),
                    SizeInBytes = assetToken.Value<long>("size"),
                    BrowserDownloadUrl = assetToken.Value<string>("browser_download_url"),
                };

                asset.ContentType = GitHubContentTypeUtil.FromApiString(asset.RawContentType);
                asset.State = GitHubReleaseStateUtil.FromApiString(asset.RawState);

                result.Assets.Add(asset);
            }

            return result;
        }
    }
}
