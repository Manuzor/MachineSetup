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
  public static int ExecutePowershell(SetupContext context, string psExePath, params string[] args)
  {
    List<string> allArgs = new List<string>
    {
        "-ExecutionPolicy", "Unrestricted",
    };
    allArgs.AddRange(args);

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
