using Shark_Remote.Engine;
using Shark_Remote.Helpers;
using VitNX2.UI.ControlsV2;
using VitNX3.Functions.AppsAndProcesses;

namespace Shark_Remote
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            ArgParser(args);
        }

        private static void ArgParser(string[] args)
        {
            string exe_name = "Shark Remote.exe";
            if (new FileInfo(Application.ExecutablePath).Name == exe_name)
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "--run-preparing":
                            Values.AppModes.preparing_enabled = true;
                            break;

                        case "--daemon":
                            Values.AppModes.service = true;
                            Console.WriteLine($"App: Shark Remote\nVersion: {Values.AppInfo.version}\nPath: {Values.AppInfo.startup_path}");
                            break;
                    }
                }
                ShowAppForm();
            }
        }

        private static void ShowAppForm()
        {
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.SetCompatibleTextRenderingDefault(false);
            if (!Network.InternetOk())
            {
                VitNX2_MessageBox.Show("Запуск невозможен, т.к. не обнаружено соедиение с Интернетом!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Processes.KillNative($"Shark Remote.exe");
            }
            if (!File.Exists("storage.txt"))
                File.WriteAllText("storage.txt", $@"{Values.AppInfo.startup_path}data");
            if (!File.Exists($@"{FileSystem.GetDataPath()}\settings\main.toml") || Values.AppModes.preparing_enabled)
            {
                try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.GetDataPath()); } catch { }
                Directory.CreateDirectory(FileSystem.GetDataPath());
            }
            else
            {
                try
                {
                    if (Convert.ToInt32(Settings.Read("OTHER", "config_version")) != Values.Config.VERSION)
                    {
                        MessageBox.Show("Конфигурационный файл Shark Remote устарел и не совместим с текущей версией.\nСейчас будет запущен Мастер настройки для перенастройки Shark Remote.\nРезервная копия текущей папки \"data\" будет создана на Рабочем столе", "Файл настроек устарел", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Values.AppModes.preparing_enabled = true;
                        VitNX3.Functions.FileSystem.Folder.Copy(FileSystem.GetDataPath(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Shark Remote Data Backup"));
                        try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.GetDataPath()); } catch { }
                        Directory.CreateDirectory(FileSystem.GetDataPath());
                    }
                }
                catch
                {
                    MessageBox.Show("Конфигурационный файл Shark Remote устарел и не совместим с текущей версией.\nСейчас будет запущен Мастер настройки для перенастройки Shark Remote\nРезервная копия текущей папки \"data\" будет создана на Рабочем столе", "Файл настроек устарел", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Values.AppModes.preparing_enabled = true;
                    VitNX3.Functions.FileSystem.Folder.Copy(FileSystem.GetDataPath(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Shark Remote Data Backup"));
                    try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.GetDataPath()); } catch { }
                    Directory.CreateDirectory(FileSystem.GetDataPath());
                }
            }
            if (Directory.Exists($@"{FileSystem.GetDataPath()}\temp"))
                Directory.Delete($@"{FileSystem.GetDataPath()}\temp", true);
            Directory.CreateDirectory($@"{FileSystem.GetDataPath()}\temp");
            if (!Directory.Exists($@"{FileSystem.GetDataPath()}\settings"))
                Directory.CreateDirectory($@"{FileSystem.GetDataPath()}\settings");
            if (!File.Exists($@"{FileSystem.GetDataPath()}\settings\variables.txt"))
                File.WriteAllText($@"{FileSystem.GetDataPath()}\settings\variables.txt", "WPath=C:\\Windows\nS32=C:\\Windows\\System32\nPS=C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe\nEvil=https://google.com\nVideo=https://youtube.com\nrouterIP=192.168.0.1");
            if (!File.Exists($@"{FileSystem.GetDataPath()}\settings\main.toml") || Values.AppModes.preparing_enabled)
                Application.Run(new Preparing());
            else
            {
                if (UI.IsWeakGPU() && (Convert.ToInt32(Settings.Read("UI", "use_forced_performance")) == 0) && !Values.AppModes.service)
                {
                    if (VitNX2_MessageBox.Show("Возможны проблемы с производительностью из-за\nвидеокарты (GPU), желаете включить режим производительности?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var s = File.ReadAllText($@"{FileSystem.GetDataPath()}\settings\main.toml");
                        s = s.Replace("use_forced_performance = 0", "use_forced_performance = 1");
                        File.WriteAllText($@"{FileSystem.GetDataPath()}\settings\main.toml", s);
                    }
                    else
                    {
                        var s = File.ReadAllText($@"{FileSystem.GetDataPath()}\settings\main.toml");
                        s = s.Replace("use_forced_performance = 0", "use_forced_performance = 2");
                        File.WriteAllText($@"{FileSystem.GetDataPath()}\settings\main.toml", s);
                    }
                }
                Values.AppHiddenParameters.ReadHiddenParameters();
                Application.Run(new MainForm());
            }
        }
    }
}