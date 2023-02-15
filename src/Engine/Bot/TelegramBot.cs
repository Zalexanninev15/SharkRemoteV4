using Newtonsoft.Json.Linq;

using Shark_Remote.Helpers;

namespace Shark_Remote.Engine.Bot
{
    public class TelegramBot
    {
        [Flags]
        public enum BotCommandType
        {
            NATIVE,
            PLUGIN,
            UNKNOWN
        };

        public static string GetCommand(string commands) => commands.Contains('/') ? commands.ToLower().Split(' ')[0].Remove(0, 1) : commands;

        public static string ArgumentsAsText(string arguments) => arguments.Replace("\"", "");

        public static string GetArguments(string userCommand,
            string command)
        {
            userCommand = userCommand.Replace($"/{command} ", "").Replace($"/{command}", "");
            string[] tmp = { "", "" };
            if (userCommand.Contains(':'))
            {
                tmp[0] = userCommand.Split(':')[1];
                tmp[1] = AppSettings.ReadVariable(tmp[0]);
                userCommand = userCommand.Replace($":{tmp[0]}:", tmp[1]);
            }
            return userCommand;
        }

        public static bool IsMyUser(string botUsername)
        {
            string[] users = AppSettings.Read("bot", "users", AppSettings.TomlTypeRead.All).Split("\\");
            bool fileUsername = false;
            for (int i = 0; i < users.Length; i++)
            {
                if (botUsername == (users[i].StartsWith('@') ? users[i].Remove(0, 1) : users[i]))
                    fileUsername = true;
            }
            return fileUsername;
        }

        public static BotCommandType IsMyCommand(string botCommand)
        {
            BotCommandType valueCommand = BotCommandType.UNKNOWN;
            foreach (string command in AppValues.botCommands)
            {
                if (botCommand == command)
                {
                    valueCommand = BotCommandType.NATIVE;
                    break;
                }
            }
            foreach (string pl in AppValues.plugins())
            {
                if (!pl.StartsWith("#app"))
                {
                    if (pl.Contains($"/{botCommand}"))
                    {
                        valueCommand = BotCommandType.PLUGIN;
                        break;
                    }
                }
            }
            return valueCommand;
        }

        public static string ImgBB = "64e1ce4f46f31c22d00fe0b1ab43a9c0";

        public static async Task<string[]> GetGeoAsync(string ip)
        {
            try
            {
                string geo = await Network.HttpResponse($"https://ipinfo.io/{ip}/json?lang=ru");
                var obj = JObject.Parse(geo);
                string country_code = (string)obj["country"];
                string region = (string)obj["region"];
                string city = (string)obj["city"];
                string loc = (string)obj["loc"];
                string[] locs = loc.Split(',');
                string postal = (string)obj["postal"];
                string org = (string)obj["org"];
                string timezone = (string)obj["timezone"];
                string[] write_in_bot = { $"Публичный IP-адрес: <code>{ip}</code>" +
                        $"\nКод страны: <code>{country_code}</code>" +
                        $"\nОбласть: {region}" +
                        $"\nГород: {city}" +
                        $"\nШирота: <code>{locs[0]}</code>" +
                        $"\nДолгота: <code>{locs[1]}</code>" +
                        $"\nПочтовый индекс: <code>{postal}</code>" +
                        $"\nИнтернет-провайдер: <code>{org}</code>" +
                        $"\nЧасовой пояс: <code>{timezone}</code>",
                        locs[0], locs[1]};
                return write_in_bot;
            }
            catch
            {
                string[] write_in_bot = { "⚠️ Сервис не отвечает!\nПовторите попытку позже" };
                return write_in_bot;
            }
        }
    }
}