namespace Shark_Remote.Engine.Bot
{
    public class TelegramBot
    {
        public enum BotCommandType
        {
            NATIVE,
            PLUGIN,
            MENU,
            UNKNOWN,
        }

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
                tmp[1] = Settings.ReadVariable(tmp[0]);
                userCommand = userCommand.Replace($":{tmp[0]}:", tmp[1]);
            }
            return userCommand;
        }

        public static BotCommandType IsMyCommand(string botCommand)
        {
            foreach (string menu in Values.Bot.menus)
            {
                if (botCommand == menu)
                    return BotCommandType.MENU;
            }
            foreach (string command in Values.Bot.commands)
            {
                if (botCommand == command)
                    return BotCommandType.NATIVE;
            }
            foreach (string plugin in Values.GetAllPlugins())
            {
                if (!plugin.StartsWith("#app"))
                {
                    if (plugin.Contains($"/{botCommand}"))
                        return BotCommandType.PLUGIN;
                }
            }
            return BotCommandType.UNKNOWN;
        }
    }
}