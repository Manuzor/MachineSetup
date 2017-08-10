using MachineSetup;
using System;
using System.Diagnostics;
using System.IO;

public static partial class Global
{
    private static string _7zExePath;
    public static string SevenZipExePath
    {
        get
        {
            if(_7zExePath == null)
                _7zExePath = FindSevenZipExe();

            return _7zExePath;
        }
    }

    public static string FindSevenZipExe()
    {
        string result = null;

        string candidate = @"C:\Program Files\7-Zip\7z.exe";
        if(File.Exists(candidate))
        {
            result = candidate;
        }

        return result;
    }

    public static void ExtractSevenZip(SetupContext context, string archivePath, string destinationPath)
    {
        ProcessStartInfo start = new ProcessStartInfo(SevenZipExePath)
        {
            Arguments = ToProcessArgumentsString("e", archivePath, $"-o{destinationPath}"),
        };

        throw new NotImplementedException();
    }
}
