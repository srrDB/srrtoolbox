using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using static srrtoolbox.Models.Enums;
using srrtoolbox.Models;
using srrtoolbox.Models.AirDC;

namespace srrtoolbox
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Configuration configuration = ConfigurationRoot.GetConfiguration("config.json");
            bool commandRan = false;

            //file list regex
            Regex regex = new Regex(@"(\w\.\w{1,40}\.xml(\.bz2)?)");

            foreach (string arg in args)
            {
                Match fileListMatch = regex.Match(arg);

                //reading and exporting data from a file list
                if (fileListMatch.Success)
                {
                    ExportFormat exportFormat = (ExportFormat)Enum.Parse(typeof(ExportFormat), configuration.AirDCExport.Format);

                    string path = Path.GetDirectoryName(arg) + Path.DirectorySeparatorChar;
                    string fileName = Path.GetFileName(arg);

                    List<FileListing> fll = Functions.ProcessFileList(arg);
                    List<ShareDirectory> ReleaseList = Functions.ProcessDcFileList(fll);

                    if (exportFormat == ExportFormat.RawTxt)
                    {
                        string extractedDataPath = path + fileName + ".txt";

                        File.WriteAllLines(extractedDataPath, ReleaseList.Select(x => x.Name ?? string.Empty));
                    }
                    else if (exportFormat == ExportFormat.FlatJson)
                    {
                        string extractedDataPath = path + fileName + ".json";

                        string jsonString = JsonSerializer.Serialize(ReleaseList.Select(x => x.Name),
                            new JsonSerializerOptions { WriteIndented = true });

                        File.WriteAllText(extractedDataPath, jsonString);
                    }
                    else if (exportFormat == ExportFormat.StructuredJson)
                    {
                        string extractedDataPath = path + fileName + ".json";

                        //TODO: make the json prettier: skip subdirs etc.

                        string jsonString = JsonSerializer.Serialize(ReleaseList,
                            new JsonSerializerOptions { WriteIndented = true });

                        File.WriteAllText(extractedDataPath, jsonString);
                    }

                    commandRan = true;
                }
            }

            //print info
            if (!commandRan)
            {
                Version version = Assembly.GetEntryAssembly()?.GetName().Version;

                Console.WriteLine("Usage: srrtoolbox [input file]");
                Console.WriteLine("Version: " + version);
            }

            return;
        }
    }
}
