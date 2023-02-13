using System.Net;

namespace Shark_Remote.Helpers
{
    public class Network
    {
        public static async Task<string> HttpResponse(string line)
        {
            using (var net = new HttpClient())
            {
                var respponse = await net.GetAsync(line);
                return respponse.IsSuccessStatusCode ? await respponse.Content.ReadAsStringAsync() : null;
            }
        }

        public static bool InternetOk()
        {
            try
            {
                Dns.GetHostEntry("sharkremote.neocities.org");
                Dns.GetHostEntry("codeberg.org");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}