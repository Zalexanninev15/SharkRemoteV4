using Shark_Remote.Helpers;

namespace Shark_Remote.Engine.API
{
    public class PluginsManager
    {
        public static string RunPluginScript(string pluginName,
            string arguments = "",
            string command = "",
            bool background_worker = false,
            int plugin_type = 0)
        {
            string answer = "";
            try
            {
                int argumentsPlugin = Convert.ToInt32(Settings.Read("", "arguments_count",
                    Settings.TomlTypeRead.OnlyOneKey,
                    @$"plugins\{pluginName}\main.manifest"));
                if (argumentsPlugin > 0 && argumentsPlugin <= 4)
                {
                    try
                    {
                        if (arguments != $"/{command}")
                        {
                            string args = "";
                            if (argumentsPlugin >= 2)
                            {
                                switch (argumentsPlugin)
                                {
                                    case 1:
                                        {
                                            args = arguments;
                                            break;
                                        }
                                    case 2:
                                        {
                                            if (plugin_type == 0)
                                                args = arguments.Split('|')[0] + " " + arguments.Split('|')[1];
                                            if (plugin_type == 1)
                                                args = arguments.Split('|')[0] + "`" + arguments.Split('|')[1];
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (plugin_type == 0)
                                                args = arguments.Split('|')[0] + " " + arguments.Split('|')[1] + " " + arguments.Split('|')[2];
                                            if (plugin_type == 1)
                                                args = arguments.Split('|')[0] + "`" + arguments.Split('|')[1] + "`" + arguments.Split('|')[2];
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (plugin_type == 0)
                                                args = arguments.Split('|')[0] + " " + arguments.Split('|')[1] + " " + arguments.Split('|')[2] + " " + arguments.Split('|')[3];
                                            if (plugin_type == 1)
                                                args = arguments.Split('|')[0] + "`" + arguments.Split('|')[1] + "`" + arguments.Split('|')[2] + "`" + arguments.Split('|')[3];
                                            break;
                                        }
                                }
                            }
                            else
                                args = "None";
                            if (background_worker)
                            {
                                switch (plugin_type)
                                {
                                    case 0:
                                        Functions.RunPowerShell(pluginName, args);
                                        break;

                                    case 1:
                                        Functions.RunLua(pluginName, args, false);
                                        break;
                                }
                            }
                            else
                            {
                                switch (plugin_type)
                                {
                                    case 0:
                                        answer = Functions.RunPowerShell(pluginName, args);
                                        break;

                                    case 1:
                                        answer = Functions.RunLua(pluginName, args);
                                        break;
                                }
                            }
                        }
                        else
                            answer = "🔴 Не найдены аргументы для плагина!";
                    }
                    catch (Exception ex) { answer = ex.Message; }
                }
                else
                {
                    if (background_worker)
                    {
                        switch (plugin_type)
                        {
                            case 0:
                                Functions.RunPowerShell(pluginName);
                                break;

                            case 1:
                                Functions.RunLua(pluginName, resul_return: false);
                                break;
                        }
                    }
                    else
                    {
                        switch (plugin_type)
                        {
                            case 0:
                                answer = Functions.RunPowerShell(pluginName);
                                break;

                            case 1:
                                answer = Functions.RunLua(pluginName);
                                break;
                        }
                    }
                }
            }
            catch { answer = "🔴 Ошибка плагина!"; }
            return answer;
        }

        public static string SearchPluginWithCommand(string command)
        {
            try
            {
                string[] plugins = File.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
                foreach (string plugin in plugins)
                {
                    if (plugin.Contains($"/{command}"))
                        return plugin.Split(", ")[0].Remove(0, 4);
                }
            }
            catch { }
            return "";
        }
    }
}