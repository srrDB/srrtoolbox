using srrtoolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static async Task<CookieConfig> Login(string username, string password)
        {
            string url = "https://www.srrdb.com/";

            Uri uri = new Uri(url);
            Uri baseAddress = new Uri(uri.Scheme + "://" + uri.Host); //ugly?

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;


            HttpClient client = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
                Timeout = TimeSpan.FromMinutes(60)
            };

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/account/login"))
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("login", "Login")
                });

                HttpResponseMessage httpResponseMessage = await client.PostAsync("/account/login", content);

                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();

                //found cookies
                if (responseCookies.Count(x => x.Name == "uid" || x.Name == "hash") >= 2)
                {
                    CookieConfig cc = new CookieConfig();

                    cc.uid = Convert.ToInt32(responseCookies.FirstOrDefault(x => x.Name == "uid").Value);
                    cc.hash = responseCookies.FirstOrDefault(x => x.Name == "hash").Value;

                    return cc;
                }
            }

            return null;
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

            long MBdl = 0;
            //TODO: calculate estimate left, check average length of the current records and use that to calculate

            IProgress<long> progress = new Progress<long>(value =>
            {
                double mbCalc = (int)value / 1000000;
                double mbReached = Math.Floor(mbCalc);

                if (mbReached > MBdl)
                {
                    MBdl = (int)mbReached;

                    Console.WriteLine("Downloaded " + MBdl.ToString() + " MB");
                }
            });

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.LocalPath))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    request.Headers.Add("Cookie", "uid=" + uid.ToString() + "; hash=" + uhash); //cookie auth

                    HttpResponseMessage httpResponseMessage = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                    using (Stream download = await httpResponseMessage.Content.ReadAsStreamAsync())
                    {
                        //await download.CopyToAsync(fs, 81920); //write to file
                        await download.CopyToAsyncWithProgress(fs, progress, default(CancellationToken), 81920); //write to file
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
