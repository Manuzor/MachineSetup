using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    [Flags]
    public enum VersionFlags
    {
        Beta,
        RC1,
        RC2,
        RC3,
        Pre,
    }
}

namespace MachineSetup
{
    public partial struct NextCloudVersion : IComparable<NextCloudVersion>
    {
        public const string VersionRegex = @"(?<Major>\d+)\.(?<Minor>\d+)\.(?<Revision>\d+)\.(?<Build>\d+)";

        public int Major;
        public int Minor;
        public int Revision;
        public int Build;

        public override string ToString()
        {
            StringBuilder result = new StringBuilder($"{Major}.{Minor}.{Revision}.{Build}");
            return result.ToString();
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

        public static NextCloudVersion? TryParse(string str)
        {
            NextCloudVersion? result = null;
            if(!string.IsNullOrWhiteSpace(str))
            {
                Match match = Regex.Match(str, VersionRegex);
                if(match.Success)
                {
                    result = new NextCloudVersion
                    {
                        Major = int.Parse(match.Groups["Major"].Value),
                        Minor = int.Parse(match.Groups["Minor"].Value),
                        Revision = int.Parse(match.Groups["Revision"].Value),
                        Build = int.Parse(match.Groups["Build"].Value),
                    };
                }
            }

            return result;
        }
    }
}

namespace MachineSetup
{
    public partial struct GimpVersion : IComparable<GimpVersion>
    {
        public const string VersionRegex = @"(?<Major>\d+)\.(?<Minor>\d+)\.(?<Revision>\d+)";

        public int Major;
        public int Minor;
        public int Revision;

        public override string ToString()
        {
            StringBuilder result = new StringBuilder($"{Major}.{Minor}.{Revision}");
            return result.ToString();
        }

        public int CompareTo(GimpVersion other)
        {
            if(Major < other.Major) return -1;
            if(Major > other.Major) return 1;
            if(Minor < other.Minor) return -1;
            if(Minor > other.Minor) return 1;
            if(Revision < other.Revision) return -1;
            if(Revision > other.Revision) return 1;
            return 0;
        }

        public static GimpVersion? TryParse(string str)
        {
            GimpVersion? result = null;
            if(!string.IsNullOrWhiteSpace(str))
            {
                Match match = Regex.Match(str, VersionRegex);
                if(match.Success)
                {
                    result = new GimpVersion
                    {
                        Major = int.Parse(match.Groups["Major"].Value),
                        Minor = int.Parse(match.Groups["Minor"].Value),
                        Revision = int.Parse(match.Groups["Revision"].Value),
                    };
                }
            }

            return result;
        }
    }
}

namespace MachineSetup
{
    public partial struct SevenZipVersion : IComparable<SevenZipVersion>
    {
        public const string VersionRegex = @"(?<Major>\d+)\.(?<Minor>\d+)";

        public int Major;
        public int Minor;
        public VersionFlags Flags;

        public int CompareTo(SevenZipVersion other)
        {
            if(Major < other.Major) return -1;
            if(Major > other.Major) return 1;
            if(Minor < other.Minor) return -1;
            if(Minor > other.Minor) return 1;
            if(Flags != 0 && other.Flags == 0) return -1;
            if(Flags == 0 && other.Flags != 0) return 1;
            return 0;
        }
    }
}

