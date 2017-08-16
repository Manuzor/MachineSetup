using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MachineSetup;

public static partial class Global
{
    private static string _gitExePath;
    public static string GitExePath
    {
        get
        {
            if(_gitExePath == null)
            {
                // TODO: Search for it.
                _gitExePath = @"C:\Program Files\Git\mingw64\bin\git.exe";
            }

            return _gitExePath;
        }
    }

    public static void CloneGitRepository(SetupContext context, string gitExePath, string url, string destination, int depth = 0, bool recursive = false)
    {
        if(!Path.IsPathRooted(destination))
        {
            throw new ArgumentException(
                "The given destination path must be an absolute path!",
                nameof(destination));
        }

        Console.WriteLine($"Cloning git repository: \"{url}\"");
        Console.WriteLine($"Repository destination: \"{destination}\"");
        if(Directory.Exists(Path.Combine(destination, ".git")))
        {
            Console.Error.WriteLine("Repo already exists. Not cloning it again.");
        }
        else
        {
            List<string> args = new List<string>
            {
                "clone", url, destination
            };

            if(depth > 0)
                args.Add($"--depth={depth}");

            if(recursive)
                args.Add("--recursive");

            ProcessStartInfo processStartInfo = new ProcessStartInfo(gitExePath)
            {
                Arguments = ToProcessArgumentsString(args),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            context.RunProcess(processStartInfo);
        }
    }
}
