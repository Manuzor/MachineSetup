using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MachineSetup.MsiApi;


namespace MachineSetup
{
    // /q[n | b | r | f]
    // Sets user interface level
    public enum MsiUILevel
    {
        UNKNOWN,

        // n - No UI
        NoUI,
        // b - Basic UI
        BasicUI,
        // r - Reduced UI
        ReducedUI,
        // f - Full UI(default)
        FullUI,
    }

    public struct MsiArguments
    {
        // /quiet
        // Quiet mode, no user interaction
        public bool Quiet;

        // /passive
        // Unattended mode - progress bar only
        public bool Passive;

        public MsiUILevel UILevel;

        public IEnumerable<string> BuildCommandLine()
        {
            if(this.Quiet) yield return "/quiet";
            if(this.Passive) yield return "/passive";
            if(this.UILevel != MsiUILevel.UNKNOWN)
            {
                switch(this.UILevel)
                {
                    case MsiUILevel.NoUI: yield return "/qn"; break;
                    case MsiUILevel.BasicUI: yield return "/qb"; break;
                    case MsiUILevel.ReducedUI: yield return "/qr"; break;
                    case MsiUILevel.FullUI: yield return "/qf"; break;

                    default: throw new ArgumentException(nameof(this.UILevel));
                }
            }
        }

        public static MsiArguments FromCommandLine(IEnumerable<string> args)
        {
            MsiArguments result = new MsiArguments();
            foreach(string arg in args.Select(a => a.Substring(1).ToLower()))
            {
                if(arg == "quiet") result.Quiet = true;
                else if(arg == "passive") result.Passive = true;
                else if(arg.StartsWith("q") && arg.Length == 2)
                {
                    switch(arg[1])
                    {
                        case 'n': result.UILevel = MsiUILevel.NoUI; break;
                        case 'b': result.UILevel = MsiUILevel.BasicUI; break;
                        case 'r': result.UILevel = MsiUILevel.ReducedUI; break;
                        case 'f': result.UILevel = MsiUILevel.FullUI; break;
                    }
                }
                else
                {
                    throw new ArgumentException("Unknown argument: " + nameof(arg));
                }
            }
            return result;
        }
    }

    public enum MsiExitCode
    {
        UNKNOWN = -1, // TODO Find out which exit codes are not used by msiexec

        Ok = 0,
        Help = 1639,
    }

    public class MsiInstaller
    {
        public string InstallerPath;
        public MsiArguments MsiArguments;
        public List<string> InstallerArguments = new List<string>();

        /// <param name="installerPath">The path of the installer package (*.msi). (not msiexec.exe)</param>
        public MsiInstaller(string installerPath) { InstallerPath = installerPath; }

        public ProcessStartInfo CreateProcessStartInfo()
        {
            if(!File.Exists(InstallerPath))
            {
                throw new System.IO.FileNotFoundException("Installer can't be found", InstallerPath);
            }

            List<string> args = new List<string>
            {
                "/i", InstallerPath,
            };
            args.AddRange(MsiArguments.BuildCommandLine());
            args.AddRange(InstallerArguments);

            // TODO Better argument escaping.

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = MsiPath,
                Arguments = ProcessHelper.ToProcessArgumentsString(args),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            return startInfo;
        }

        public MsiExitCode? InterpretExitCode(Process proc)
        {
            return InterpretExitCode(proc.ExitCode);
        }

        public MsiExitCode? InterpretExitCode(int exitCodeValue)
        {
            foreach(MsiExitCode exitCode in Enum.GetValues(typeof(MsiExitCode)))
            {
                if((int)exitCode == exitCodeValue)
                {
                    return exitCode;
                }
            }

            return null;
        }
    }

    public static class MsiApi
    {
        public static string MsiPath => @"C:\Windows\system32\msiexec.exe";
    }
}
