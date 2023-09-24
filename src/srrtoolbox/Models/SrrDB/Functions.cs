using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace srrtoolbox.Models.SrrDB
{
    public class Functions
    {
        public async static Task<bool> AddFileStructureToReleases(string uploadFilesPath, int uid, string uhash)
        {
            if (!uploadFilesPath.EndsWith(@"\"))
                uploadFilesPath = uploadFilesPath + @"\";

            string[] releases = Directory.GetDirectories(uploadFilesPath);

            foreach (string releaseDir in releases)
            {
                string release = Path.GetFileName(releaseDir);
                string[] filesToUpload = Directory.GetFiles(uploadFilesPath + release);

                foreach (string filename in filesToUpload)
                {
                    //TODO: add error handling
                    await AddFileToRelease(release, filename, uid, uhash);

                    Console.WriteLine("Added file to release [" + release + "], [" + filename + "]");
                }
            }

            return true;
        }

        public static async Task<bool> DownloadSrrdbDump(string url, string filename, int uid, string uhash)
        {
            Uri uri = new Uri(url);
            Uri baseAddress = new Uri(uri.Scheme + "://" + uri.Host); //ugly?

            //required to work in .NET 4.8
            //https://stackoverflow.com/a/13287224
            HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };

            HttpClient client = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
                Timeout = TimeSpan.FromMinutes(60)
            };

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.LocalPath))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    request.Headers.Add("Cookie", "uid=" + uid.ToString() + "; hash=" + uhash); //cookie auth

                    HttpResponseMessage httpResponseMessage = await client.SendAsync(request);

                    using (Stream download = await httpResponseMessage.Content.ReadAsStreamAsync())
                    {
                        await download.CopyToAsync(fs, 81920); //write to file
                    }

                    return true;
                }
            }
        }

        public static async Task AddFileToRelease(string release, string filename, int uid, string uhash, string folder = "")
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://www.srrdb.com")
            };

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/release/add/" + release))
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    request.Headers.Add("Cookie", "uid=" + uid.ToString() + "; hash=" + uhash);

                    using (MultipartFormDataContent content = new MultipartFormDataContent
                    {
                        {
                            new StringContent(""), "add"
                        },
                        {
                            new StreamContent(stream), "file[]", Path.GetFileName(filename)
                        }
                    })
                    {
                        request.Content = content;

                        HttpResponseMessage httpResponseMessage = await client.SendAsync(request);

                        string result = await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}
