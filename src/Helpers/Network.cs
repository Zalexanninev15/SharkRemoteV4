using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Shark_Remote.Helpers
{
    public class Network
    {
        public class Addresses
        {
            public static string MAC()
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.OperationalStatus == OperationalStatus.Up && iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var physicalAddress = iface.GetPhysicalAddress();
                        if (physicalAddress != null && physicalAddress.ToString() != "")
                            return physicalAddress.ToString();
                    }
                }
                return "none";
            }

            public static string IPv4()
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.OperationalStatus == OperationalStatus.Up && iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var properties = iface.GetIPProperties();
                        foreach (var address in properties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                                return address.Address.ToString();
                        }
                    }
                }
                return "none";
            }

            public static string IPv6()
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.OperationalStatus == OperationalStatus.Up && iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var properties = iface.GetIPProperties();
                        foreach (var address in properties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                return address.Address.ToString();
                        }
                    }
                }
                return "none";
            }
        }

        public static async Task<string?> HttpResponse(string line)
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
                Dns.GetHostEntry("telegram.org");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<string[]> GetGeoAsync(string ip, int type, string api_key = "")
        {
            // Services:
            // 0 - ipinfo.io
            // 1 - ipwhois.io
            // 2 - ipgeolocation.io (with API key)
            string[] geo_services = new string[]
                {
                    "https://ipinfo.io/PUB_IP/json?lang=ru",
                    "https://ipwho.is/PUB_IP?lang=ru&output=json",
                    "https://api.ipgeolocation.io/ipgeo?apiKey=API_KEY&ip=PUB_IP&lang=ru"
                };
            string geo = await HttpResponse(geo_services[type].Replace("PUB_IP", ip).Replace("API_KEY", api_key));
            var obj = JObject.Parse(geo);
            string country_code = "";
            switch (type)
            {
                case 0:
                    country_code = (string)obj["country"];
                    break;

                case 1:
                    country_code = (string)obj["country_code"];
                    break;

                case 2:
                    country_code = (string)obj["country_code2"];
                    break;
            }
            string country = "";
            switch (type)
            {
                case 0:
                    country = country_code;
                    break;

                case 1:
                    country = (string)obj["country"];
                    break;

                case 2:
                    country = (string)obj["country_name"];
                    break;
            }
            string region = "";
            if (type == 2)
                region = (string)obj["state_prov"];
            else
                region = (string)obj["region"];
            string city = (string)obj["city"];
            string[] locs = new string[] { "", "" };
            if (type == 0)
            {
                string loc = (string)obj["loc"];
                locs = loc.Split(',');
            }
            else
                locs = new string[] { (string)obj["latitude"], (string)obj["longitude"] };
            string postal = "";
            if (type == 2)
                postal = (string)obj["zipcode"];
            else
                postal = (string)obj["postal"];
            string org = "";
            switch (type)
            {
                case 0:
                    org = (string)obj["org"];
                    break;

                case 1:
                    org = (string)obj["connection"]["org"];
                    break;

                case 2:
                    org = (string)obj["organization"];
                    break;
            }
            string timezone = "";
            switch (type)
            {
                case 0:
                    timezone = (string)obj["timezone"];
                    break;

                case 1:
                    timezone = (string)obj["timezone"]["id"];
                    break;

                case 2:
                    timezone = (string)obj["time_zone"]["name"];
                    break;
            }
            string[] write_in_bot = { $"Публичный IP-адрес: <code>{ip}</code>" +
                        $"\nКод страны: <code>{country_code}</code>" +
                        $"\nСтрана: {country}" +
                        $"\nОбласть: {region}" +
                        $"\nГород: {city}" +
                        $"\nШирота: <code>{locs[0]}</code>" +
                        $"\nДолгота: <code>{locs[1]}</code>" +
                        $"\nПочтовый индекс: <code>{postal}</code>" +
                        $"\nИнтернет-провайдер: <code>{org}</code>" +
                        $"\nЧасовой пояс: <code>{timezone.Replace(@"\", "")}</code>",
                        locs[0], locs[1]};
            return write_in_bot;
        }
    }
}