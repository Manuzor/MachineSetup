using System;
using System.Diagnostics;
using System.IO;

using static Global;


namespace MachineSetup
{
    public class SevenZipSetup
    {
        public void Run(SetupContext context)
        {
            const string url = @"http://7-zip.org/a/7z1604-x64.msi";
            string name = GetFileNameFromUrl(url);
            string savePath = Path.Combine(context.SavePath, "7z", name);
            context.DownloadFile("7-Zip installer", url, savePath);

            ProcessStartInfo startInfo = PrepareMsiProcess(MsiExePath, savePath, MsiInfo.Default);

            if(context.InstallEnabled)
            {
                context.RunProcess(startInfo, (proc) =>
                {
                    if(InterpretMsiExitCode(proc) != MsiExitCode.Ok)
                    {
                        throw new Exception("Unable to run MSI installer");
                    }
                });
            }
        }
    }
}
