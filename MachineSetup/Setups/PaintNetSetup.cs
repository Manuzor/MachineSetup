using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using HtmlAgilityPack;

using static Global;

namespace MachineSetup
{
  public class PaintNetSetup
  {
    public string DownloadBaseUrl = @"https://www.dotpdn.com/downloads";

    public void Run(SetupContext context)
    {
      HtmlWeb web = new HtmlWeb();
      HtmlDocument doc = web.Load($"{DownloadBaseUrl}/pdn.html");
      List<string> installers =
        doc.DocumentNode.Descendants("a")
        .Select(a => a.GetAttributeValue("href", string.Empty))
        .Where(s => s.EndsWith(".install.zip"))
        .ToList();

      installers.Sort();

      string url = $"{DownloadBaseUrl}/{installers.Last()}";
      string installerDir = Path.Combine(context.SavePath, "paint.net");
      string zipPath = Path.Combine(installerDir, GetFileNameFromUrl(url));
      context.DownloadFile("Paint.NET zipped installer", url, zipPath);

      string extractionDir = installerDir;
      ZipFile.ExtractToDirectory(zipPath, installerDir);

      string installerPath = null;
      foreach(string filePath in Directory.EnumerateFiles(extractionDir))
      {
        if(Path.GetExtension(filePath) == ".exe")
        {
          installerPath = filePath;
          break;
        }
      }
      Debug.Assert(installerPath != null);

      ProcessStartInfo processStartInfo = new ProcessStartInfo(installerPath)
      {
        Arguments = ToProcessArgumentsString("/auto"),
      };
      context.RunProcess(processStartInfo);
    }
  }
}
