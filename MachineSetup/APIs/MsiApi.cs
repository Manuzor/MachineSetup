using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MachineSetup
{
    public static partial class Global
    {
        public static string MsiExePath => @"C:\Windows\system32\msiexec.exe";

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

        public struct MsiInfo
        {
            // /quiet
            // Quiet mode, no user interaction
            public bool Quiet;

            // /passive
            // Unattended mode - progress bar only
            public bool Passive;

            public MsiUILevel UILevel;

            public string LogPath;

            public List<string> AdditionalArguments;

            public static readonly MsiInfo Default = new MsiInfo
            {
                Passive = true,
                AdditionalArguments = new List<string>(),
            };

            public IEnumerable<string> BuildCommandLine(string installerPath)
            {
                yield return "/i";
                yield return installerPath;

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

                if(this.LogPath != null)
                {
                    yield return $"/L*";
                    yield return this.LogPath;
                }

                if(this.AdditionalArguments != null)
                {
                    foreach(string arg in this.AdditionalArguments)
                        yield return arg;
                }
            }
        }

        public static ProcessStartInfo PrepareMsiProcess(string msiPath, string installerPath, MsiInfo info)
        {
            if(!File.Exists(installerPath))
            {
                throw new System.IO.FileNotFoundException("Installer can't be found", installerPath);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = msiPath,
                Arguments = ToProcessArgumentsString(info.BuildCommandLine(installerPath)),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            return startInfo;
        }

        public enum MsiExitCode
        {
            UNKNOWN = -1, // TODO Find out which exit codes are not used by msiexec

            Ok = 0,
            Help = 1639,
        }

        public static MsiExitCode? InterpretMsiExitCode(Process proc)
        {
            return InterpretMsiExitCode(proc.ExitCode);
        }

        public static MsiExitCode? InterpretMsiExitCode(int exitCodeValue)
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
}
