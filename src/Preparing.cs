using Shark_Remote.Engine.Bot;
using Shark_Remote.Helpers;

using System.Net;

using Tommy;

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
            label10.Text = AppValues.app_information;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;
            Import.SetThreadExecutionState(Enums.EXECUTION_STATE.ES_CONTINUOUS |
                Enums.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                Enums.EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            Import.SetProcessDpiAwareness(Enums.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE);
            Import.ReleaseCapture();
            try
            {
                if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                    Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                else
                    Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            }
            catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
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
            if (VitNX3.Functions.Information.Internet.IsHaveInternet() != VitNX3.Functions.Information.Internet.INTERNET_STATUS.CONNECTED)
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Запуск невозможен, т.к. не обнаружено соедиение с Интернетом!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Processes.KillNative($"Shark Remote.exe");
            }
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
                selectedMenu.Location = new Point(191, 62);
                preparingTab.SelectTab(1);
            }
            else
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Необходимо принять Условия использования!",
                 "Действие не выполнено",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning);
        }

        private void first_Click(object sender, EventArgs e)
        {
            selectedMenu.Location = new Point(28, 62);
            preparingTab.SelectTab(0);
        }

        private void secondSecond_Click(object sender, EventArgs e)
        {
            if (sharkOne.Checked)
            {
                if (AppValues.botToken != "")
                {
                    selectedMenu.Location = new Point(377, 62);
                    preparingTab.SelectTab(2);
                }
                else VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Необходимо вставить токен бота!",
                    "Действие не выполнено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Необходимо принять лицензионное соглашение!",
                "Действие не выполнено",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private async void titleExit_Click(object sender, EventArgs e)
        {
            do
            {
                Opacity -= 0.2;
                await Task.Delay(1);
            }
            while (Opacity > 0);
            try { VitNX3.Functions.FileSystem.Folder.DeleteForever(FileSystem.data_path_var); } catch { }
            Processes.KillNative($"Shark Remote.exe");
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
            try
            {
                if (Processes.OpenLink("https://t.me/botfather") == false)
                    Clipboard.SetText("https://t.me/botfather");
            }
            catch
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Браузер для открытия ссылки не найден! Ссылка на @BotFather будет скопирована после закрытия данного сообщения.",
                    "Браузер по умолчанию не задан",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Clipboard.SetText("https://t.me/botfather");
            }
        }

        private async void getBotTokenNow_Click(object sender, EventArgs e)
        {
            string tmp = Clipboard.GetText();
            if (tmp != "")
            {
                try
                {
                    if (!VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(tmp))
                    {
                        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Токен нерабочий!",
                            "Ошибка проверки токена",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        AppValues.botToken = "";
                    }
                    else
                    {
                        AppValues.botToken = tmp;
                        bot_settings[0] = tmp;
                        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Токен принят и его работоспособность подтверждена!",
                            "Готово",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        selectedMenu.Location = new Point(377, 62);
                        preparingTab.SelectTab(2);
                    }
                }
                catch
                {
                    VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Действительный токен не обнаружен!",
                        "Ошибка проверки токена",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    AppValues.botToken = "";
                    bot_settings[0] = "";
                }
            }
            else
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Невозможно извлечь токен бота из буфера обмена!",
                    "Ошибка получения токена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                AppValues.botToken = "";
                bot_settings[0] = "";
            }
        }

        private void setUsername_Click(object sender, EventArgs e)
        {
            string user = username.Texts;
            if (user != "")
            {
                if (user.ToLower().StartsWith("https://t.me/"))
                    bot_settings[1] = user.Replace("https://t.me/".ToLower(), "").Replace("http://t.me/".ToLower(), "").Replace("t.me/".ToLower(), "");
                else if (user.ToLower().EndsWith(".t.me"))
                    bot_settings[1] = user.Replace(".t.me".ToLower(), "").Replace("https://".ToLower(), "").Replace("http://".ToLower(), "");
                else
                    bot_settings[1] = user.StartsWith('@') ? user.Remove(0, 1) : user;
                first.Visible = false;
                second.Visible = false;
                secondSecond.Visible = false;
                selectedMenu.Visible = false;
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Username добавлен в исключения бота!",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                preparingTab.SelectTab(3);
            }
            else
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Не указан username пользователя!",
                        "Ошибка получения username",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                bot_settings[1] = "";
            }
        }

        private void vitnX2_Button1_Click(object sender, EventArgs e)
        {
            try
            {
                TomlTable toml = new TomlTable
                {
                    ["bot"] =
                    {
                        ["token"] = bot_settings[0],
                        ["username"] = bot_settings[1],
                        //["users"] = new TomlNode[] { bot_settings[1] },
                    },
                    ["tprint"] =
                    {
                        ["font"] = "Arial",
                        ["size"] = 10
                    },
                    ["ui"] =
                    {
                        ["use_rounded_window_frame_style"] = true,
                        ["use_window_mini_mode"] = true,
                        ["menu_color"] = "default",
                        ["use_window_transparency"] = true,
                        ["use_window_animation"] = false,
                    }
                };
                using (StreamWriter writer = File.CreateText($@"{FileSystem.data_path_var}\settings\main.toml"))
                {
                    toml.WriteTo(writer);
                    writer.Flush();
                }
                MainForm secF = new MainForm();
                secF.Show();
                Hide();
            }
            catch (Exception ex) { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void sharkOne_CheckedChanged(object sender, EventArgs e)
        {
            if (sharkOne.Checked)
            {
                selectedMenu.Location = new Point(191, 62);
                preparingTab.SelectTab(1);
            }
        }

        private void vitnX2_Button2_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    TomlTable toml = new TomlTable
            //    {
            //        ["bot"] =
            //        {
            //            ["token"] = bot_settings[0],
            //            ["username"] = bot_settings[1],
            //            //["users"] = new TomlNode[] { bot_settings[1] },
            //        },
            //        ["tprint"] =
            //        {
            //            ["font"] = "Arial",
            //            ["size"] = 10
            //        },
            //        ["ui"] =
            //        {
            //            ["use_rounded_window_frame_style"] = true,
            //            ["use_window_mini_mode"] = true,
            //            ["menu_color"] = "default",
            //            ["use_window_transparency"] = true,
            //            ["use_window_animation"] = false,
            //        }
            //    };
            //    using (StreamWriter writer = File.CreateText($@"{FileSystem.data_path_var}\settings\main.toml"))
            //    {
            //        toml.WriteTo(writer);
            //        writer.Flush();
            //    }
            //    MainForm secF = new MainForm();
            //    secF.Show();
            //    Hide();
            //}
            //catch (Exception ex) { VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void vitnX2_Button3_Click(object sender, EventArgs e)
        {
            try
            {
                username.Texts = Clipboard.GetText();
            }
            catch { }
        }
    }
}