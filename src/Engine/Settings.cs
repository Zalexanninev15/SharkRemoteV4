using Shark_Remote.Helpers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot.Types.ReplyMarkups;
using Tommy;

namespace Shark_Remote.Engine
{
    public class Values
    {
        public static string[] GetAllPlugins()
        {
            try
            {
                return File.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
            }
            catch (Exception ex)
            {
                return new string[]
                {
                    ex.Message,
                    ex.ToString()
                };
            }
        }

        public class Config
        {
            public static int VERSION { get; } = 1803;

            public static string ReadUserId()
            {
                try
                {
                    string user = Settings.Read("BOT", "admin");
                    if (user != null)
                        return user.Contains("|") ? user.Split('|')[1] : user;
                    else
                        return "none";
                }
                catch { return "none"; }
            }

            public static string ReadUsername(bool show_id = false)
            {
                try
                {
                    string user = Settings.Read("BOT", "admin");
                    if (user != "")
                    {
                        if (show_id)
                            return user;
                        else
                            return user.Contains("|") ? user.Split('|')[0] : user;
                    }
                    else
                        return "none";
                }
                catch { return "none"; }
            }
        }

        public class Bot
        {
            public static string[] commands { get; } = { "screen", "geo", "net", "ls", "lst", "wh",
            "power", "vget", "vset", "get", "set", "md", "clean", "run", "kill", "send", "apps", "kcmd",
            "kps", "tasks", "cat", "msg", "start", "file", "dir", "del", "rd", "ping", "sc", "tprint",
            "info", "bot", "touch", "curl", "input", "uptime", "usage", "move", "click", "dclick", "wheel", "pwg"};

            public static string[] menus { get; } = {
            "🗃 Файлы и папки",
            "🕹 Управление",
            "🌐 Сеть",
            "📜 Задачи",
            "📦+🪴 Пользовательские",
            "🤏 Другое",
            "📩 Отправка и сохранение"
            };
        }

        public class AppInfo
        {
            public static bool IsThisDevBuild()
            {
                string filePath = "Shark Remote.exe";
                X509Certificate2 theCertificate;
                try
                {
                    X509Certificate theSigner = X509Certificate.CreateFromSignedFile(filePath);
                    theCertificate = new X509Certificate2(theSigner);
                    return false;
                }
                catch { return true; }
            }

            public static string VersionLabel()
            {
                string dev = IsThisDevBuild() ? "-dev" : "";
                return $"Версия: {version}{dev}\r\nРазработчик: Zalexanninev15";
            }

            public static string startup_path { get; } = Application.StartupPath.EndsWith("\\") ? Application.StartupPath : $"{Application.StartupPath}\\";
            public static string version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            public static string service_path_tool { get; } = $"{startup_path}service\\";
            public static string WinSW_URL { get; } = "https://codeberg.org/Zalexanninev15/open-api/raw/branch/main/Shark%20Remote/WinSW.exe";
        }

        public class AppUI
        {
            public static bool use_forced_performance = false;
        }

        public class AppModes
        {
            public static bool preparing_enabled = false;
            public static bool mini = false;
            public static bool service = false;
        }

        public class AppHiddenParameters
        {
            public static void ReadHiddenParameters()
            {
                try
                {
                    string hidden_application_parameters = Settings.Read("OTHER", "hidden_application_parameters");
                    Set(hidden_application_parameters.Contains("|") ? hidden_application_parameters.Split('|') : new string[] { hidden_application_parameters });
                }
                catch { }
            }

            private static void Set(string[] parameters)
            {
                foreach (string parameter in parameters)
                {
                    switch (parameter.ToUpper())
                    {
                        case "P_ISL":
                            P_ISL = true;
                            break;

                        case "W_TOP":
                            W_TOP = true;
                            break;

                        case "W_HIDE":
                            W_HIDE = true;
                            break;

                        case "W_SM":
                            W_SM = true;
                            break;
                    }
                }
            }

            public static bool P_ISL = false;
            public static bool W_TOP = false;
            public static bool W_HIDE = false;
            public static bool W_SM = false;
        }
    }

    public class BotKeyboards
    {
        public static InlineKeyboardMarkup DocVarKeyboard = new InlineKeyboardMarkup(new[]
{
              new[]
              {
                   InlineKeyboardButton.WithUrl("ℹ Пользовательские переменные",
                   $"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#QAVA"),
              }
        });

        public static ReplyKeyboardMarkup MainKeyboard()
        {
            ReplyKeyboardMarkup keyboard = new(new[]
            {
                    new KeyboardButton[] { "🗃 Файлы и папки", "🕹 Управление" },
                    new KeyboardButton[] { "🌐 Сеть", "📜 Задачи" },
                    new KeyboardButton[] { "📦+🪴 Пользовательские", "🤏 Другое" },
                    new KeyboardButton[] { "📩 Отправка и сохранение" },
             });
            keyboard.ResizeKeyboard = true;
            return keyboard;
        }
    }

    public class Settings
    {
        public class MemoryValues
        {
            public static string token = "";
        }

        public enum TomlTypeRead
        {
            OnlyOne,
            All,
            OnlyOneKey
        }

        public static string Read(string section,
            string key,
            TomlTypeRead type = TomlTypeRead.OnlyOne,
            string sourcePath = @"settings\main.toml")
        {
            string for_return = "";
            using (StreamReader reader = File.OpenText($@"{FileSystem.GetDataPath()}\{sourcePath}"))
            {
                TomlTable table = TOML.Parse(reader);
                switch (type)
                {
                    case TomlTypeRead.OnlyOne:
                        {
                            for_return = table[section][key].ToString();
                            break;
                        }
                    case TomlTypeRead.All:
                        {
                            foreach (TomlNode node in table[section][key])
                                for_return += $"\\{node}";
                            break;
                        }
                    case TomlTypeRead.OnlyOneKey:
                        {
                            for_return = table[key];
                            break;
                        }
                }
            }
            return for_return;
        }

        public static string ReadVariable(string variable)
        {
            string[] settings = File.ReadAllLines($@"{FileSystem.GetDataPath()}\settings\variables.txt");
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].Contains(variable))
                    return settings[i].Replace($"{variable}=", "");
            }
            return "";
        }
    }
}