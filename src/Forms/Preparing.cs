using Microsoft.Win32;
using Shark_Remote.Engine;
using Shark_Remote.Helpers;
using System.Net;

using Tommy;
using VitNX2.UI.ControlsV2;
using VitNX3.Functions.Win32;

using Processes = VitNX3.Functions.AppsAndProcesses.Processes;

namespace Shark_Remote
{
    public partial class Preparing : Form
    {
        public string[] bot_settings = { "", "" };

        public Preparing()
        {
            InitializeComponent();
            Values.AppModes.preparing_enabled = true;
            label10.Text = Values.AppInfo.VersionLabel();
            ServicePointManager.SecurityProtocol = VitNX3.Functions.Web.Config.UseProtocols();
            Import.SetThreadExecutionState(Enums.EXECUTION_STATE.ES_CONTINUOUS |
                Enums.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                Enums.EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            Import.ReleaseCapture();
            ClientSize = new Size(UI.Window.Dpi(547), UI.Window.Dpi(409));
            Size = new Size(UI.Window.Dpi(547), UI.Window.Dpi(409));
            try
            {
                if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                    Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                else
                    Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            }
            catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
            Import.SetProcessDpiAwareness(Enums.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE);
            Opacity = 0;
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(sharkMessage.Handle); } catch { }
            System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
            launch.Tick += new EventHandler((sender, e) =>
            {
                if ((Opacity += 0.05d) == 1)
                    launch.Stop();
            });
            launch.Interval = 20;
            launch.Start();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            sharkOne.Checked = !sharkOne.Checked;
        }

        private void windowTitle_MouseDown(object sender, MouseEventArgs e)
        {
            Import.ReleaseCapture();
            Import.PostMessage(Handle,
                Constants.WM_SYSCOMMAND,
                Constants.DOMOVE, 0);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= Constants.CS_DROPSHADOW;
                return cp;
            }
        }

        private void second_Click(object sender, EventArgs e)
        {
            if (sharkOne.Checked)
            {
                visualProgress.Value = 35;
                preparingTab.SelectTab(1);
            }
            else
            {
                VitNX2_MessageBox.Show("Необходимо принять Условия использования!",
                 "Действие не выполнено",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning);
                visualProgress.Value = 0;
            }
        }

        private void first_Click(object sender, EventArgs e)
        {
            visualProgress.Value = 0;
            preparingTab.SelectTab(0);
        }

        private void secondSecond_Click(object sender, EventArgs e)
        {
            if (sharkOne.Checked)
            {
                if (Settings.MemoryValues.token != "")
                {
                    visualProgress.Value = 60;
                    preparingTab.SelectTab(2);
                }
                else
                {
                    VitNX2_MessageBox.Show("Необходимо вставить токен бота!",
                    "Действие не выполнено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    visualProgress.Value = 0;
                }
            }
            else
            {
                VitNX2_MessageBox.Show("Необходимо принять Условия использования!",
                "Действие не выполнено",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                visualProgress.Value = 0;
            }
        }

        private async void titleExit_Click(object sender, EventArgs e)
        {
            var exit = VitNX2_MessageBox.Show("Настройка Shark Remote будет отменена,\nжелаете закрыть Мастер настройки?",
            "Требуется подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
            if (exit == DialogResult.Yes)
            {
                //do
                //{
                //    Opacity -= 0.2;
                //    await Task.Delay(1);
                //}
                //while (Opacity > 0);
                try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.GetDataPath()); } catch { }
                Processes.KillNative($"Shark Remote.exe");
            }
        }

        private void titleExit_MouseEnter(object sender, EventArgs e)
        {
            titleExit.ForeColor = Color.Red;
        }

        private void titleExit_MouseLeave(object sender, EventArgs e)
        {
            titleExit.ForeColor = Color.FromArgb(183, 185, 191);
        }

        private void botFatherLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UI.OpenLink("https://t.me/botfather");
        }

        private void getBotTokenNow_Click(object sender, EventArgs e)
        {
            try
            {
                string tmp = Clipboard.GetText();
                tmp = tmp.Replace("\"", "");
                if (tmp != "")
                {
                    if (!VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(tmp))
                    {
                        VitNX2_MessageBox.Show("Действительный токен не обнаружен!\nИспользуйте другой токен и бота, возможно у вас проблемы с Сетью",
                            "Ошибка проверки токена",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Settings.MemoryValues.token = "";
                        visualProgress.Value = 35;
                    }
                    else
                    {
                        Settings.MemoryValues.token = tmp;
                        bot_settings[0] = tmp;
                        VitNX2_MessageBox.Show("Токен принят и его работоспособность подтверждена!",
                            "Готово",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        visualProgress.Value = 60;
                        preparingTab.SelectTab(2);
                    }
                }
                else
                {
                    VitNX2_MessageBox.Show("Невозможно извлечь токен бота из буфера обмена!",
                    "Ошибка получения токена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    Settings.MemoryValues.token = "";
                    bot_settings[0] = "";
                }
            }
            catch
            {
                VitNX2_MessageBox.Show("Невозможно извлечь токен бота из буфера обмена!",
                    "Ошибка получения токена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Settings.MemoryValues.token = "";
                bot_settings[0] = "";
            }
        }

        private void setUsername_Click(object sender, EventArgs e)
        {
            string user = username.Texts;
            if (user != "")
            {
                user = user.Replace("\"", "");
                if (user.ToLower().StartsWith("https://t.me/"))
                    bot_settings[1] = user.Replace("https://t.me/".ToLower(), "").Replace("http://t.me/".ToLower(), "").Replace("t.me/".ToLower(), "");
                else if (user.ToLower().EndsWith(".t.me"))
                    bot_settings[1] = user.Replace(".t.me".ToLower(), "").Replace("https://".ToLower(), "").Replace("http://".ToLower(), "");
                else
                    bot_settings[1] = user.StartsWith('@') ? user.Remove(0, 1) : user;
                visualProgress.Value = 100;
                VitNX2_MessageBox.Show("Username добавлен в администраторы бота!",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                preparingTab.SelectTab(3);
                try
                {
                    TomlTable toml = new TomlTable
                    {
                        ["BOT"] =
                        {
                            ["token"] = bot_settings[0],
                            ["admin"] = bot_settings[1],
                            ["imgbb_api_key"] = ""
                        },
                        ["GEOLOCATION"] =
                        {
                            ["selected_service"] = 0,
                            ["ipgeolocationio_api_key"] = ""
                        },
                        ["PRINT_OPTIONS"] =
                        {
                            ["font"] = "Arial",
                            ["size"] = 10
                        },
                        ["UI"] =
                        {
                            ["use_rounded_window_frame_style"] = true,
                            ["use_window_mini_mode"] = true,
                            ["menu_color"] = "default",
                            ["use_window_transparency"] = false,
                            ["use_window_animation"] = true,
                            ["use_forced_performance"] = 0
                        },
                        ["OTHER"] =
                        {
                            ["config_version"] = Values.Config.VERSION,
                            ["hidden_bot_parameters"] = "",
                            ["hidden_application_parameters"] = ""
                        }
                    };
                    using (StreamWriter writer = File.CreateText($@"{FileSystem.GetDataPath()}\settings\main.toml"))
                    {
                        toml.WriteTo(writer);
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    VitNX2_MessageBox.Show($"{ex.Message}", "Ошибка записи настроек", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Processes.KillNative($"Shark Remote.exe");
                }
            }
            else
            {
                VitNX2_MessageBox.Show("Не указан username пользователя!",
                        "Ошибка получения username",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                bot_settings[1] = "";
                visualProgress.Value = 60;
            }
        }

        private void vitnX2_Button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void sharkOne_CheckedChanged(object sender, EventArgs e)
        {
            if (sharkOne.Checked)
            {
                visualProgress.Value = 35;
                preparingTab.SelectTab(1);
            }
        }

        private void Hider(bool force_true = false)
        {
            if (!force_true)
            {
                progressVisual.Visible = true;
                progress.Visible = true;
            }
            else
            {
                progressVisual.Visible = false;
                label12.Visible = false;
                progress.Visible = false;
            }
        }

        private void vitnX2_Button2_Click(object sender, EventArgs e)
        {
            Hider();
            vitnX2_Button2.Text = "Настраиваю...";
            Cursor.Current = Cursors.WaitCursor;
            Downloader.Values.url = Values.AppInfo.WinSW_URL;
            Downloader.Values.save_path = Values.AppInfo.service_path_tool;
            Downloader.Values.done = false;
            try
            {
                if (Network.InternetOk())
                {
                    if (Directory.Exists(Values.AppInfo.service_path_tool)) { try { VitNX3.Functions.FileSystem.Folder.DeleteForever(Values.AppInfo.service_path_tool); } catch { } }
                    Directory.CreateDirectory("service");
                    progress.Value = 35;
                    new Downloader().ShowDialog();
                    progress.Value = 60;
                    File.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", Properties.Resources.WinSW);
                    string t = File.ReadAllText($"{Values.AppInfo.service_path_tool}WinSW.xml");
                    t = t.Replace("<workingdirectory>Path to Shark Remote.exe here", $"<workingdirectory>{Values.AppInfo.startup_path}")
                        .Replace("<executable>Shark Remote", $"<executable>{Values.AppInfo.startup_path}Shark Remote.exe");
                    File.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", t);
                    Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"install", false);
                    progress.Value = 95;
                    using (RegistryKey SharkRemoteDaemon = Registry.LocalMachine.CreateSubKey(@"SYSTEM\ControlSet001\Control\Windows", true))
                        SharkRemoteDaemon.SetValue("NoInteractiveServices", 0);
                    Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"start", false);
                    progress.Value = 100;
                    vitnX2_Button2.Text = "Служба установлена!";
                    VitNX2_MessageBox.Show("Служба установлена, но требуется выход из приложения и\nзапуск службы вручную из Служб Windows (только в первый раз).\nНазвание: Shark Remote (SharkRemoteDaemon)", "Требуется ручная активация Службы Windows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Processes.KillNative("Shark Remote.exe");
                }
                else
                {
                    if (Directory.Exists(Values.AppInfo.service_path_tool))
                    {
                        progress.Value = 60;
                        File.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", Properties.Resources.WinSW);
                        string t = File.ReadAllText($"{Values.AppInfo.service_path_tool}WinSW.xml");
                        t = t.Replace("<workingdirectory>Path to Shark Remote.exe here", $"<workingdirectory>{Values.AppInfo.startup_path}");
                        File.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", t);
                        Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"install", false);
                        progress.Value = 95;
                        using (RegistryKey SharkRemoteDaemon = Registry.LocalMachine.CreateSubKey(@"SYSTEM\ControlSet001\Control\Windows", true))
                            SharkRemoteDaemon.SetValue("NoInteractiveServices", 0);
                        Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"start", false);
                        progress.Value = 100;
                        vitnX2_Button2.Text = "Служба установлена!";
                        VitNX2_MessageBox.Show("Служба установлена, но требуется выход из приложения и\nзапуск службы вручную из Служб Windows (только в первый раз).\nНазвание: Shark Remote (SharkRemoteDaemon)", "Требуется ручная активация Службы Windows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Processes.KillNative("Shark Remote.exe");
                    }
                    else
                    {
                        Hider(true);
                        VitNX2_MessageBox.Show("Установка Службы Windows в данный момент недоступна,\nпопробуйте позже из настроек приложения.\nСейчас воспользуйтесь кнопкой \"Запустить и использовать Shark Remote\",\nчтобы запустить приложение.", "Установка прервана", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                try { VitNX3.Functions.FileSystem.Folder.DeleteForever(Values.AppInfo.service_path_tool); } catch { }
                Hider(true);
                VitNX2_MessageBox.Show("Установка Службы Windows в данный момент недоступна, попробуйте позже из настроек приложения.\nСейчас воспользуйтесь кнопкой \"Запустить и использовать Shark Remote\",\nчтобы запустить приложение.\n\nОшибка: " + ex.Message, "Установка прервана", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Hider(true);
            Cursor.Current = Cursors.Default;
        }

        private void vitnX2_Button3_Click(object sender, EventArgs e)
        {
            try
            {
                username.Texts = Clipboard.GetText();
            }
            catch { }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new OtherSettings().ShowDialog();
        }
    }
}