using System;
using System.IO;
using System.Runtime.CompilerServices;
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
                    WriteConfig(configFile, new ConfigurationRoot());

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
                ConfigurationRoot newConfig = new ConfigurationRoot();
                WriteConfig(configFile, newConfig);

                configuration = newConfig.Configuration;
            }

            return configuration;
        }

        private static void WriteConfig(string filename, ConfigurationRoot croot)
        {
            string jsonString = JsonSerializer.Serialize(croot, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine("Saving config");

            File.WriteAllText(filename, jsonString);
        }

        //wrapper
        public static void WriteConfig(string filename, Configuration configuration)
        {
            WriteConfig(filename, new ConfigurationRoot { Configuration = configuration });
        }
    }

    public class Configuration
    {
        public AirDCExport AirDCExport { get; set; } = new AirDCExport();

        public SrrDb SrrDb { get; set; } = new SrrDb();
    }

    public class SrrDb
    {
        public Auth Auth { get; set; } = new Auth();

        public string DumpFile { get; set; } = "releaselist.txt";

        public string DumpListUrl { get; set; } = "https://www.srrdb.com/open/releaselist";

        public DateTime DumpGrabbedAt { get; set; } = new DateTime(2020, 1, 1);

        //TODO: check some common names if they also have ever had a scene release
        public string[] KnownP2PGroups { get; set; } = new string[]
        {
            "CDB",
            "Danishbits",
            "DAWGS",
            "DBRETAiL",
            "EGEN",
            "ESiR",
            "FiLMKiDS",
            "GRANiTEN",
            "GRANiTEN",
            "HDChina",
            "HTR",
            "JFF",
            "KraBBA",
            "m0nkrus",
            "P2P",
            "PANDEMONiUM",
            "PE2PE",
            "PTNK",
            "QUARK",
            "RAPiDCOWS",
            "ROCKETRACCOON",
            "RTBYTES",
            "TWA",
            "TWASERiES",
            "WiKi",
            "YOLO"
        };
    }

    public class Auth
    {
        public CookieConfig Cookie { get; set; } = new CookieConfig();

        public LoginConfig Login { get; set; } = new LoginConfig();
    }

    public class CookieConfig
    {
        public int uid { get; set; }

        public string hash { get; set; }
    }

    public class LoginConfig
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class AirDCExport
    {
        public string Format { get; set; } = "RawTxt";
    }
}
