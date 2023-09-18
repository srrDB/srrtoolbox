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

        public static async Task AddFileToRelease(string release, string filename, int uid, string uhash, string folder = "")
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new("https://www.srrdb.com")
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

                        HttpResponseMessage httpResponseMessage = client.Send(request);

                        string result = await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}
