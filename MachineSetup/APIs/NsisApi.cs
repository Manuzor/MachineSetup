using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MachineSetup
{
    public struct NsisInfo
    {
        public bool Silent;

        public string DestinationPath;

        public IEnumerable<string> BuildCommandLine()
        {
            if(Silent)
                yield return "/S";

            // Note from the NSIS FAQ:
            // > Must be the last parameter on the command line and must not contain quotes even if the path contains blank spaces.
            if(!string.IsNullOrWhiteSpace(DestinationPath))
                yield return $"/D={DestinationPath}";
        }

        public static readonly NsisInfo Default = new NsisInfo
        {
            Silent = true,
        };
    }

    public static partial class Global
    {
        public static ProcessStartInfo PrepareNsisProcess(string installerPath, NsisInfo info)
        {
            ProcessStartInfo result = new ProcessStartInfo(installerPath)
            {
                Arguments = ToProcessArgumentsString(info.BuildCommandLine()),
            };

            return result;
        }
    }
}
