using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        public string InstallerPath;

        // /quiet
        // Quiet mode, no user interaction
        public bool Quiet;

        // /passive
        // Unattended mode - progress bar only
        public bool Passive;

        public MsiUILevel UILevel;

        public IEnumerable<string> BuildCommandLine()
        {
            yield return "/i";
            yield return this.InstallerPath;

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
    }

    public static ProcessStartInfo PrepareMsiProcess(MsiInfo info) => PrepareProcess(MsiExePath, info);

    public static ProcessStartInfo PrepareProcess(string msiPath, MsiInfo info)
    {
        if(!File.Exists(info.InstallerPath))
        {
            throw new System.IO.FileNotFoundException("Installer can't be found", info.InstallerPath);
        }

        // TODO Better argument escaping.

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = msiPath,
            Arguments = ToProcessArgumentsString(info.BuildCommandLine()),
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
