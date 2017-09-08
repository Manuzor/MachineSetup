using MachineSetup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup
{
    public static partial class Global
    {
        public static string ChocoExePath = @"C:\ProgramData\chocolatey\bin\choco.exe";

        public static bool IsChocolateyInstalled(SetupContext context, string chocoExePath)
        {
            return File.Exists(chocoExePath);
        }

        public static int ExecuteChocolatey(SetupContext context, string chocoExePath, params string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(chocoExePath)
            {
                Arguments = ToProcessArgumentsString(args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            int exitCode = 0;
            context.RunProcess(startInfo, (proc) => exitCode = proc.ExitCode);
            return exitCode;
        }
    }
}
