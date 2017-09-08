using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup
{
    // Commandline reference:
    // http://www.jrsoftware.org/ishelp/index.php?topic=setupcmdline

    public enum InnoSetupSilenceLevel
    {
        NONE,

        Silent,
        VerySilent,
    }

    public enum InnoSetupCloseApplications
    {
        NONE,

        Close,
        NoClose,
        ForceClose,
        NoForceClose,
    }

    public enum InnoSetupRestartApplications
    {
        NONE,

        Restart,
        NoRestart,
    }

    public struct InnoSetupInfo
    {
        public string Destination;
        public bool InitialPrompt;
        public InnoSetupSilenceLevel SilenceLevel;
        public bool SuppressMessageBoxes;
        public bool RestartAfterwards;
        public string LogPath;
        public InnoSetupCloseApplications CloseApplications;
        public InnoSetupRestartApplications RestartApplications;
        public List<string> Tasks;

        public static readonly InnoSetupInfo Default = new InnoSetupInfo
        {
            SuppressMessageBoxes = true,
            SilenceLevel = InnoSetupSilenceLevel.Silent,
            CloseApplications = InnoSetupCloseApplications.Close,
            Tasks = new List<string> { "!desktopicon" },
        };
    }

    public static partial class Global
    {
        public static ProcessStartInfo PrepareInnoSetupProcess(string installerPath, InnoSetupInfo info)
        {
            List<string> args = new List<string>();

            if(info.Destination != null)
                args.Add($"/DIR={info.Destination}");

            if(!info.InitialPrompt)
                args.Add("/SP-");

            switch(info.SilenceLevel)
            {
                case InnoSetupSilenceLevel.Silent: args.Add("/SILENT"); break;
                case InnoSetupSilenceLevel.VerySilent: args.Add("/VERYSILENT"); break;

                case InnoSetupSilenceLevel.NONE: break;

                default: throw new ArgumentException(nameof(info.SilenceLevel));
            }

            if(info.SuppressMessageBoxes)
                args.Add("/SUPPRESSMSGBOXES");

            if(info.LogPath != null)
                args.Add($"/LOG={info.LogPath}");

            switch(info.CloseApplications)
            {
                case InnoSetupCloseApplications.Close: args.Add("/CLOSEAPPLICATIONS"); break;
                case InnoSetupCloseApplications.NoClose: args.Add("/NOCLOSEAPPLICATIONS"); break;
                case InnoSetupCloseApplications.ForceClose: args.Add("/FORCECLOSEAPPLICATIONS"); break;
                case InnoSetupCloseApplications.NoForceClose: args.Add("/NOFORCECLOSEAPPLICATIONS"); break;

                case InnoSetupCloseApplications.NONE: break;

                default: throw new ArgumentException(nameof(info.CloseApplications));
            }

            switch(info.RestartApplications)
            {
                case InnoSetupRestartApplications.Restart: args.Add("/RESTARTAPPLICATIONS"); break;
                case InnoSetupRestartApplications.NoRestart: args.Add("/NORESTARTAPPLICATIONS"); break;

                case InnoSetupRestartApplications.NONE: break;

                default: throw new ArgumentException(nameof(info.RestartApplications));
            }

            if(info.Tasks != null && info.Tasks.Count > 0)
                args.Add($"/TASKS={string.Join(",", info.Tasks)}");

            ProcessStartInfo result = new ProcessStartInfo(installerPath)
            {
                Arguments = ToProcessArgumentsString(args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            //Console.WriteLine($"Arguments string: {result.Arguments}");

            return result;
        }
    }
}
