using Newtonsoft.Json;

namespace imgBBUploader
{
    public class Uploader
    {
        private static readonly string URL = "https://api.imgbb.com/1/upload";

        private string ApiKey;

        public Uploader(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey");
            ApiKey = apiKey;
        }

        public async Task<Response> UploadImageFileAsync(string filename, string name = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");
            if (string.IsNullOrWhiteSpace(name))
                name = Path.GetFileNameWithoutExtension(filename);
            var data = File.ReadAllBytes(filename);
            ByteArrayContent bytes = new ByteArrayContent(data);
            MultipartFormDataContent multiContent = new MultipartFormDataContent
            {
                { bytes, "image", name }
            };
            var route = URL + "?key=" + ApiKey;
            using var client = new HttpClient();
            var result = await client.PostAsync(route, multiContent);
            var json = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Response>(json);
            var error = JsonConvert.DeserializeObject<Error>(json);
            throw new Exception(error.error.message);
        }

        public async Task<Response> UploadFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("image", url) });
            var route = URL + "?key=" + ApiKey;
            using var client = new HttpClient();
            var result = await client.PostAsync(route, content);
            var json = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Response>(json);
            var error = JsonConvert.DeserializeObject<Error>(json);
            throw new Exception(error.error.message);
        }

        public async Task<bool> DeleteAsync(string deleteUrl)
        {
            throw new NotImplementedException();
        }
    }
}