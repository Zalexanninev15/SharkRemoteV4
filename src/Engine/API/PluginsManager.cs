using Shark_Remote.Engine.Bot;

namespace Shark_Remote.Engine.API
{
    public class PluginsManager
    {
        //public static string RunScript(string pluginName,
        //    string arguments = "",
        //    string command = "")
        //{
        //    string answer = "";
        //    try
        //    {
        //        Executor executor = new Script($@"{Application.StartupPath}\data\plugins\{pluginName}\main.ps1");
        //        executor.Execute();
        //        int argumentsPlugin = Convert.ToInt32(AppSettings.Read("", "arguments_count", AppSettings.TomlTypeRead.OnlyOneKey, @$"plugins\{pluginName}\main.manifest"));
        //        var PluginMain = (Func<string, string, string, string, string>)executor.Call("GetDelegate");
        //        if (argumentsPlugin > 0 && argumentsPlugin <= 4)
        //        {
        //            try
        //            {
        //                if (arguments != $"/{command}")
        //                {
        //                    string[] args = new string[4];
        //                    if (argumentsPlugin >= 2)
        //                    {
        //                        switch (argumentsPlugin)
        //                        {
        //                            case 2:
        //                                {
        //                                    args[0] = arguments.Split('|')[0];
        //                                    args[1] = arguments.Split('|')[1];
        //                                    break;
        //                                }
        //                            case 3:
        //                                {
        //                                    args[0] = arguments.Split('|')[0];
        //                                    args[1] = arguments.Split('|')[1];
        //                                    args[2] = arguments.Split('|')[2];
        //                                    break;
        //                                }
        //                            case 4:
        //                                {
        //                                    args[0] = arguments.Split('|')[0];
        //                                    args[1] = arguments.Split('|')[1];
        //                                    args[2] = arguments.Split('|')[2];
        //                                    args[3] = arguments.Split('|')[3];
        //                                    break;
        //                                }
        //                        }
        //                    }
        //                    else
        //                        args[0] = arguments;
        //                    answer = Convert.ToString(PluginMain(args[0], args[1], args[2], args[3]));
        //                }
        //                else
        //                    answer = "🔴 Не найдены аргументы для плагина!";
        //            }
        //            catch (Exception ex) { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show(ex.Message); }
        //        }
        //        else
        //            answer = Convert.ToString(PluginMain("", "", "", ""));
        //        executor.Dispose();
        //    }
        //    catch { answer = "🔴 Ошибка плагина!"; }
        //    return answer;
        //}

        public static string RunScriptPowerShell(string pluginName,
            string arguments = "",
            string command = "",
            bool background_worker = false)
        {
            string answer = "";
            try
            {
                var script = $@"{Application.StartupPath}\data\plugins\{pluginName}\main.ps1";
                int argumentsPlugin = Convert.ToInt32(AppSettings.Read("", "arguments_count",
                    AppSettings.TomlTypeRead.OnlyOneKey,
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
                                            args = arguments.Split('|')[0] + " " + arguments.Split('|')[1];
                                            break;
                                        }
                                    case 3:
                                        {
                                            args = arguments.Split('|')[0] + " " + arguments.Split('|')[1] + " " + arguments.Split('|')[2];
                                            break;
                                        }
                                    case 4:
                                        {
                                            args = arguments.Split('|')[0] + " " + arguments.Split('|')[1] + " " + arguments.Split('|')[2] + " " + arguments.Split('|')[3];
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                args = "None";
                            }
                            if (background_worker)
                            {
                                Functions.RunPowerShell(pluginName, args);
                            }
                            else
                            {
                                answer = Functions.RunPowerShell(pluginName, args);
                            }
                        }
                        else
                            answer = "🔴 Не найдены аргументы для плагина!";
                    }
                    catch (Exception ex) { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show(ex.Message); }
                }
                else
                {
                    if (background_worker)
                        Functions.RunPowerShell(pluginName);
                    else
                        answer = Functions.RunPowerShell(pluginName);
                }
            }
            catch { answer = "🔴 Ошибка плагина!"; }
            return answer;
        }

        public static string SearchPluginWithCommand(string command)
        {
            try
            {
                string[] plugins = File.ReadAllLines($@"{Application.StartupPath}\data\plugins\installed.cfg");
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