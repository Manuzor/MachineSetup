using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineSetup;
using System.IO;
using System.Diagnostics;

namespace MachineSetup
{
    public static partial class Global
    {
        public static string DefaultPowershellExePath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

        /// Returns the exit code.
        public static int ExecutePowershell(this SetupContext context, params string[] args)
        {
            Debug.Assert(args.Length > 0);

            List<string> allArgs = new List<string>
            {
                "-ExecutionPolicy", "Unrestricted",
            };
            allArgs.AddRange(args);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(DefaultPowershellExePath)
            {
                Arguments = ToProcessArgumentsString(allArgs),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            int exitCode = 0;
            context.RunProcess(processStartInfo, (proc) => exitCode = proc.ExitCode);

            return exitCode;
        }
    }
}
