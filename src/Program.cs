using Shark_Remote.Helpers;

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
                        case "--server":
                            Engine.Bot.AppValues.serviceMode = true;
                            Console.WriteLine($"App: Shark Remote v{Application.ProductVersion}\nPath: {Application.StartupPath}");
                            break;

                        case "--no-product-mode-tron":
                            Engine.Bot.AppValues.ProductMode = true;
                            ShowAppForm();
                            break;
                    }
                }
                else
                    ShowAppForm();
            }
        }

        private static void ShowAppForm()
        {
            if (!Directory.Exists(FileSystem.data_path_var))
                Directory.CreateDirectory(FileSystem.data_path_var);
            else
            {
                if (!File.Exists($@"{FileSystem.data_path_var}\settings\main.toml"))
                {
                    try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.data_path_var); } catch { }
                    Directory.CreateDirectory(FileSystem.data_path_var);
                }
            }
            if (Directory.Exists($@"{FileSystem.data_path_var}\temp"))
                Directory.Delete($@"{FileSystem.data_path_var}\temp", true);
            Directory.CreateDirectory($@"{FileSystem.data_path_var}\temp");
            if (!Directory.Exists($@"{FileSystem.data_path_var}\settings"))
                Directory.CreateDirectory($@"{FileSystem.data_path_var}\settings");
            if (!File.Exists($@"{FileSystem.data_path_var}\settings\variables.txt"))
                File.WriteAllText($@"{FileSystem.data_path_var}\settings\variables.txt", "WPath=C:\\Windows\nS32=C:\\Windows\\System32\nPS=C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe\nEvil=https://google.com\nVideo=https://youtube.com\nrouterIP=192.168.0.1");
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            if (File.Exists($@"{FileSystem.data_path_var}\Shark Remote.exe"))
            {
                try
                {
                    Processes.KillNative("Shark Remote.exe");
                }
                catch { }
                File.Delete($@"{FileSystem.data_path_var}\Shark Remote.exe");
            }
            if (!File.Exists($@"{FileSystem.data_path_var}\settings\main.toml"))
                Application.Run(new Preparing());
            else
                Application.Run(new MainForm());
        }
    }
}