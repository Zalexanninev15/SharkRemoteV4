using System.Diagnostics;

namespace Shark_Remote
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            //if (Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion.StartsWith("6") || Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion.StartsWith("8") || Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion.StartsWith("7") || VitNX3.Functions.Information.Windows.GetWindowsVersion().StartsWith("6") || VitNX3.Functions.Information.Windows.GetWindowsVersion().StartsWith("8") || VitNX3.Functions.Information.Windows.GetWindowsVersion().StartsWith("7"))
            //    VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Ваша система устарела и не поддерживается Shark Remote, пожалуйста обновитесь до Windows 10 или новее, чтобы использовать данное ПО", "Ваша версия системы не поддерживается");
            //else
            //{
            string exe_name = "Shark Remote.exe";
            if (new FileInfo(Application.ExecutablePath).Name == exe_name)
            {
                if (args.Length > 0)
                {
                    string new_exe = "new.exe";
                    switch (args[0])
                    {
                        case "--server":
                            Engine.Bot.AppValues.serviceMode = true;
                            Console.WriteLine($"App: Shark Remote v{Application.ProductVersion}\nPath: {Application.StartupPath}");
                            break;

                        case "--ctu-p0":
                            try
                            {
                                foreach (var ps in Process.GetProcessesByName(exe_name))
                                    ps.Kill();
                                File.Copy(new_exe, exe_name, true);
                                Process.Start(exe_name, "--ctu-p1");
                                Application.Exit();
                            }
                            catch { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Ошибка обновления: невозможно начать обновление", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            break;

                        case "--ctu-p1":
                            try
                            {
                                Thread.Sleep(1450);
                                File.Delete(new_exe);
                                ShowAppForm();
                            }
                            catch { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Ошибка обновления: невозможно закончить обновление", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            break;

                        case "--no-product-mode-tron":
                            Engine.Bot.AppValues.ProductMode = true;
                            break;
                    }
                }
                ShowAppForm();
            }
            //}
        }

        private static void ShowAppForm()
        {
            if (Directory.Exists("profile"))
                Directory.Delete("profile");
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");
            else
            {
                if (!File.Exists($@"{Application.StartupPath}\data\settings\main.toml"))
                {
                    try { VitNX3.Functions.FileSystem.Folder.DeleteForever("data"); } catch { }
                    Directory.CreateDirectory("data");
                }
            }
            if (Directory.Exists($@"{Application.StartupPath}\data\temp"))
                Directory.Delete($@"{Application.StartupPath}\data\temp", true);
            Directory.CreateDirectory($@"{Application.StartupPath}\data\temp");
            if (!Directory.Exists($@"{Application.StartupPath}\data\settings"))
                Directory.CreateDirectory($@"{Application.StartupPath}\data\settings");
            if (!File.Exists($@"{Application.StartupPath}\data\settings\variables.txt"))
                File.WriteAllText($@"{Application.StartupPath}\data\settings\variables.txt", "WPath=C:\\Windows\nS32=C:\\Windows\\System32\nPS=C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe\nEvil=https://google.com\nVideo=https://youtube.com\nrouterIP=192.168.0.1");
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            if (!File.Exists($@"{Application.StartupPath}\data\settings\main.toml"))
                Application.Run(new Preparing());
            else
                Application.Run(new MainForm());
        }
    }
}