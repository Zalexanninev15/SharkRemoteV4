using Shark_Remote.Helpers;

using System.Reflection;

using Telegram.Bot.Types.ReplyMarkups;

using Tommy;

namespace Shark_Remote.Engine.Bot
{
    public class AppValues
    {
        public static string app_information = $"Версия: {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}\r\nРазработчик: Zalexanninev15";

        public static string[] botCommands { get; } = { "screen", "geo", "net", "ls", "lst", "wh",
        "power", "vget", "vset", "get", "set", "md", "clean", "run", "kill", "send", "apps", "killcmd",
        "killps", "tasks", "cat", "msg", "start", "battery", "file", "dir", "del", "rd", "ping", "sc", "tprint",
        "info", "bot", "touch", "curl", "input", "uptime", "usage", "move", "click", "dclick", "🗃 Файлы и папки", "🕹 Управление",
        "🌐 Сеть", "📜 Задачи", "📦+🪴 Пользовательские", "🤏 Другое", "📩 Отправка и сохранение"};

        public static InlineKeyboardMarkup keyboardMoreInfoQAVA = new InlineKeyboardMarkup(new[]
        {
              new[]
              {
                   InlineKeyboardButton.WithUrl("ℹ Пользовательские переменные",
                   $"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#QAVA"),
              }
        });

        public static bool miniMode { get; set; } = false;

        public static bool serviceMode { get; set; } = false;

        public static bool ProductMode { get; set; } = false;

        public static string botToken { get; set; } = "";

        public static ReplyKeyboardMarkup GetKeyboard()
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

        public static string[] plugins()
        {
            try
            {
                return File.ReadAllLines($@"{FileSystem.data_path_var}\plugins\installed.cfg");
            }
            catch (Exception ex)
            {
                return new string[]
                {
                    ex.Message, ex.ToString()
                };
            }
        }
    }

    public class AppSettings
    {
        public enum TomlTypeRead
        {
            OnlyOne,
            All,
            OnlyOneKey
        }

        public static string Read(string section, string key, TomlTypeRead type = TomlTypeRead.OnlyOne, string sourcePath = @"settings\main.toml")
        {
            try
            {
                string for_return = "";
                using (StreamReader reader = File.OpenText($@"{FileSystem.data_path_var}\{sourcePath}"))
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
            catch (Exception ex)
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"Файл настроек повреждён, либо настройки заданы неверно!\n\nError: {ex.Message}",
                "Ошибка чтения настроек",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return null;
            }
        }

        public static string ReadVariable(string variable)
        {
            string[] settings = File.ReadAllLines($@"{FileSystem.data_path_var}\settings\variables.txt");
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].Contains(variable))
                    return settings[i].Replace($"{variable}=", "");
            }
            return "";
        }
    }
}