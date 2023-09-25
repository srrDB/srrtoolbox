using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace srrtoolbox.Models
{
    //used to serialize new config file if broken or missing
    public class ConfigurationRoot
    {
        public Configuration Configuration { get; set; } = new Configuration();

        public static Configuration GetConfiguration(string configFile)
        {
            Configuration configuration = new Configuration();

            //settings
            if (File.Exists(configFile))
            {
                IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(configFile, optional: false);
                IConfiguration config;

                try
                {
                    config = builder.Build();
                }
                catch (InvalidDataException)
                {
                    //invalid config, let's fix it
                    new ConfigurationRoot().WriteConfig(configFile);

                    string jsonString = JsonSerializer.Serialize(new ConfigurationRoot(),
                            new JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText(configFile, jsonString);
                }

                config = builder.Build();

                configuration = config.GetSection("Configuration").Get<Configuration>() ?? new Configuration();
            }
            else
            {
                //write config file
                ConfigurationRoot writeConfig = new ConfigurationRoot();
                writeConfig.WriteConfig(configFile);

                configuration = writeConfig.Configuration;
            }

            return configuration;
        }

        public void WriteConfig(string filename)
        {
            //invalid config, let's fix it
            string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine("Config file missing/broken, writing new");

            File.WriteAllText(filename, jsonString);
        }
    }

    public class Configuration
    {
        public AirDCExport AirDCExport { get; set; } = new AirDCExport();

        public SrrDb SrrDb { get; set; } = new SrrDb();
    }

    public class SrrDb
    {
        public string CookieAuth { get; set; } = ""; //id,hash

        public string DumpFile { get; set; } = "releaselist.txt";

        public string ListDumpUrl { get; set; } = "https://www.srrdb.com/open/releaselist";

        //TODO: check common names if they also have ever had a scene release
        public string[] KnownP2PGroups { get; set; } = new string[]
        {
            "CDB",
            "Danishbits",
            "DAWGS",
            "DBRETAiL",
            "EGEN",
            "FiLMKiDS",
            "GRANiTEN",
            "GRANiTEN",
            "HTR",
            "JFF",
            "PANDEMONiUM",
            "PE2PE",
            "PTNK",
            "QUARK",
            "RAPiDCOWS",
            "ROCKETRACCOON",
            "TWA",
            "TWASERiES",
            "YOLO"
        };
    }

    public class AirDCExport
    {
        public string Format { get; set; } = "RawTxt";
    }
}
