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
        public static string DefaultChocoExePath = @"C:\ProgramData\chocolatey\bin\choco.exe";

        public static bool IsChocolateyInstalled(this SetupContext context)
        {
            return File.Exists(DefaultChocoExePath);
        }

        public static int ExecuteChocolatey(this SetupContext context, params string[] args)
        {
            Debug.Assert(args.Length > 0);

            ProcessStartInfo startInfo = new ProcessStartInfo(DefaultChocoExePath)
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
