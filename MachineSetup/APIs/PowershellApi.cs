using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineSetup;
using System.IO;
using System.Diagnostics;

public static partial class Global
{
    public static string PowershellExePath => @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

    /// Returns the exit code.
    public static int ExecutePowershellScript(SetupContext context, string psExePath, string scriptPath, params string[] scriptArgs)
    {
        if(!File.Exists(scriptPath))
            throw new FileNotFoundException("Unable to find powershell script file.", scriptPath);

        List<string> allArgs = new List<string>
        {
            "-ExecutionPolicy", "Unrestricted",
            "-File", scriptPath,
        };
        allArgs.AddRange(scriptArgs);

        ProcessStartInfo processStartInfo = new ProcessStartInfo(psExePath)
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
