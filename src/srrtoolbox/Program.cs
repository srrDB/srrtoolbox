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
            string configFile = "config.json";

            Configuration configuration = ConfigurationRoot.GetConfiguration(configFile);
            bool commandRan = false;

            //login to srrdb to get credentials cookie
            //TODO: check validity of the credentials
            if (string.IsNullOrEmpty(configuration.SrrDb.Auth.Cookie.hash))
            {
                //need to login
                CookieConfig cc = await Models.SrrDB.Functions.Login(configuration.SrrDb.Auth.Login.Username, configuration.SrrDb.Auth.Login.Password);

                //save cookie auth
                configuration.SrrDb.Auth.Cookie = cc;
                ConfigurationRoot.WriteConfig(configFile, configuration);
            }

            //download the latest dump if needed
            if (configuration.SrrDb.DumpGrabbedAt < DateTime.Now.AddDays(-7))
            {
                /*string downloadPath = "";

                //download new file
                if(!Path.IsPathRooted(configuration.SrrDb.DumpFile))
                {
                    downloadPath += Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                }

                downloadPath += configuration.SrrDb.DumpFile;*/

                await Models.SrrDB.Functions.DownloadSrrdbDump(configuration.SrrDb.DumpListUrl, configuration.SrrDb.DumpFile, configuration.SrrDb.Auth.Cookie.uid, configuration.SrrDb.Auth.Cookie.hash);

                //save new date
                configuration.SrrDb.DumpGrabbedAt = DateTime.Now;
                ConfigurationRoot.WriteConfig(configFile, configuration);
            }

            //read all releases from srrdb into memory, ~1.4 GB RAM usage
            string[] releases = File.ReadAllLines(configuration.SrrDb.DumpFile);

            //file list regex
            Regex regex = new Regex(@"(\w\.\w{1,40}\.xml(\.bz2)?)");

            foreach (string arg in args)
            {
                Match fileListMatch = regex.Match(arg);

                //parsing and exporting data from file lists
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