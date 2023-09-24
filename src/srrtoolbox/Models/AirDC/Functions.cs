using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using ICSharpCode.SharpZipLib.BZip2;

namespace srrtoolbox.Models.AirDC
{
    public class Functions
    {
        private static List<ShareDirectory> ProcessDirectory(ShareDirectory targetDirectory)
        {
            List<ShareDirectory> releases = new List<ShareDirectory>();

            List<ShareDirectory> subdirectoryEntries = targetDirectory.Directory_;

            //recursive
            foreach (ShareDirectory subdirectory in subdirectoryEntries)
            {
                if (subdirectory.IsRelease)
                {
                    releases.Add(subdirectory);
                }

                releases.AddRange(ProcessDirectory(subdirectory));
            }

            return releases;
        }

        private static FileListing ProcessFileListXml(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            FileListing fileListing = null;

            using (FileStream source = fileInfo.OpenRead())
            {
                using (MemoryStream target = new MemoryStream())
                {
                    if (filename.EndsWith(".bz2"))
                    {
                        //decompress bz2 into memorystream
                        Console.WriteLine("Reading xml [" + filename + "]... (and decompressing bz2)");
                        BZip2.Decompress(source, target, false); //false makes sure that target is not disposed
                    }
                    else if (filename.EndsWith(".xml"))
                    {
                        Console.WriteLine("Reading xml [" + filename + "]...");
                        //read file into memorystream
                        byte[] bytes = new byte[source.Length];
                        source.Read(bytes, 0, (int)source.Length);
                        target.Write(bytes, 0, (int)source.Length);
                    }

                    target.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(typeof(FileListing));

                    Console.WriteLine("Deserializing xml...");
                    using (XmlReader reader = XmlReader.Create(target))
                    {
                        fileListing = (FileListing)serializer.Deserialize(reader);
                    }
                }
            }

            return fileListing;
        }

        public static List<FileListing> ProcessFileList(string[] filenames)
        {
            List<FileListing> fll = new List<FileListing>();

            foreach (string filename in filenames)
            {
                FileListing processed = ProcessFileListXml(filename);

                if (processed != null)
                {
                    fll.Add(processed);
                }
            }

            return fll;
        }

        public static List<FileListing> ProcessFileList(string filename)
        {
            return ProcessFileList(new string[] { filename });
        }

        public static List<ShareDirectory> ProcessDcFileList(List<FileListing> fll)
        {
            List<ShareDirectory> releases = new List<ShareDirectory>();

            if (fll != null && fll.Any())
            {
                //check each root dir for releases
                foreach (FileListing fl in fll)
                {
                    Console.WriteLine("Processing xml [" + fl.CID + "]...");

                    foreach (ShareDirectory dir in fl.Directory_)
                    {
                        releases.AddRange(ProcessDirectory(dir));
                    }
                }
            }

            return releases;
        }

        public static List<string> GetReleasesFromAutoSearchXml(string autoSearchFilePath)
        {
            string regExPattern = @"SearchString=\""(.*?)\""";

            string fileData = File.ReadAllText(autoSearchFilePath);
            List<string> releases = new List<string>();

            MatchCollection matches = Regex.Matches(fileData, regExPattern);

            foreach (Match match in matches)
            {
                releases.Add(match.Groups[1].ToString());
            }

            releases = releases.OrderBy(x => x).ToList();

            return releases;
        }

        //used to create bundles
        public static string GenerateRandomDigits(int length)
        {
            Random random = new Random();

            string s = string.Empty;

            for (int i = 0; i < length; i++)
            {
                s = string.Concat(s, random.Next(10).ToString());
            }

            return s;
        }

        public static string GenerateDummyTTH(int length)
        {
            Random random = new Random();

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static AutoSearchRoot ReadAutoSearch(string filename)
        {
            AutoSearchRoot asr = new AutoSearchRoot();

            FileInfo fileInfo = new FileInfo(filename);

            using (FileStream source = fileInfo.OpenRead())
            {
                using (MemoryStream target = new MemoryStream())
                {
                    Console.WriteLine("Reading xml [" + filename + "]...");
                    //read file into memorystream
                    byte[] bytes = new byte[source.Length];
                    source.Read(bytes, 0, (int)source.Length);
                    target.Write(bytes, 0, (int)source.Length);

                    target.Position = 0;
                    XmlSerializer serializer = new XmlSerializer(typeof(AutoSearchRoot));

                    Console.WriteLine("Deserializing xml...");
                    using (XmlReader reader = XmlReader.Create(target))
                    {
                        asr = (AutoSearchRoot)serializer.Deserialize(reader);
                    }
                }
            }

            return asr;
        }
    }
}
