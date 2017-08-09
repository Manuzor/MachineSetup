using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MachineSetup
{
    public class SevenZipSetup
    {
        public void Run(SetupContext context)
        {
            const string url = @"http://7-zip.org/a/7z1604-x64.msi";
            string name = Path.GetFileName(url);
            string savePath = Path.Combine(context.SavePath, name);
            context.DownloadFile("7-Zip installer", url, savePath);

            MsiInstaller installer = new MsiInstaller(savePath)
            {
                MsiArguments = new MsiArguments
                {
                    Passive = true,
                },
            };

            ProcessStartInfo startInfo = installer.CreateProcessStartInfo();
            context.RunProcess(startInfo, (proc) =>
            {
                if(installer.InterpretExitCode(proc) != MsiExitCode.Ok)
                {
                    throw new Exception("Unable to run MSI installer");
                }
            });
        }
    }
}
