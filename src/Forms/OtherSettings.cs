using Shark_Remote.Engine;
using Shark_Remote.Helpers;
using Tommy;
using VitNX2.UI.ControlsV2;
using VitNX3.Functions.Win32;

namespace Shark_Remote
{
    public partial class OtherSettings : Form
    {
        public static string TempFont = "Arial";
        public static int TempSize = 10;
        public static string ipgeolocation_api_key = "";
        public static string imgbb_api_key = "";

        public OtherSettings()
        {
            InitializeComponent();
            ClientSize = new Size(UI.Window.Dpi(566), UI.Window.Dpi(395));
            Size = new Size(UI.Window.Dpi(566), UI.Window.Dpi(395));
            try { VitNX3.Functions.WindowAndControls.Window.SetWindowsTenAndHighStyleForWinFormTitleToDark(Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height); } catch { }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(comboBox1.Handle); } catch { }
            Import.SetProcessDpiAwareness(Enums.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE);
            Import.ReleaseCapture();
            imgbb_api_key = Settings.Read("BOT", "imgbb_api_key");
            comboBox1.SelectedIndex = Convert.ToInt32(Settings.Read("GEOLOCATION", "selected_service"));
            ipgeolocation_api_key = Settings.Read("GEOLOCATION", "ipgeolocationio_api_key");
            TempFont = Settings.Read("PRINT_OPTIONS", "font");
            TempSize = Convert.ToInt32(Settings.Read("PRINT_OPTIONS", "size"));
            checkBox1.Checked = Convert.ToBoolean(Settings.Read("UI", "use_rounded_window_frame_style"));
            checkBox2.Checked = Convert.ToBoolean(Settings.Read("UI", "use_window_mini_mode"));
            comboBox2.SelectedIndex = UI.SetMenuColorComboBox(Settings.Read("UI", "menu_color"));
            checkBox3.Checked = Convert.ToBoolean(Settings.Read("UI", "use_window_transparency"));
            checkBox4.Checked = Convert.ToBoolean(Settings.Read("UI", "use_window_animation"));
            forcedPerformanceMode.Checked = Convert.ToInt32(Settings.Read("UI", "use_forced_performance")) == 1 ? true : false;
            textBox1.Text = Settings.Read("OTHER", "hidden_bot_parameters");
            textBox2.Text = Settings.Read("OTHER", "hidden_application_parameters");
        }

        private void vitnX2_Button5_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.ShowColor = false;
                fd.ShowEffects = false;
                fd.ShowHelp = false;
                FontConverter fc = new FontConverter();
                if (TempFont != "" && TempSize != 0)
                    fd.Font = new Font(TempFont, TempSize, FontStyle.Regular);
                else
                    fd.Font = new Font("Arial", 10.0F, FontStyle.Regular);
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    TempFont = fc.ConvertToString(fd.Font).Split(';')[0];
                    TempSize = (int)Math.Round(fd.Font.Size);
                }
                else
                {
                    TempFont = "Arial";
                    TempSize = 10;
                }
            }
        }

        private void vitnX2_Button3_Click(object sender, EventArgs e)
        {
            string admin = Settings.Read("BOT", "admin");
            string menu_color;
            Values.AppModes.mini = checkBox2.Checked;
            int ipgeo_int = 0;
            if (comboBox2.SelectedItem != null)
                menu_color = comboBox2.SelectedItem.ToString().
                    Replace("C учётом акцентного цвета в Windows", "native").
                    Replace("По умолчанию", "default").
                    Replace("Новый год 2023 с иконками", "happy_new_year_with_icons").
                    Replace("Новый год 2023 (полный вариант)", "happy_new_year_with_icons_and_hide_log").
                    Replace("Новый год 2023", "happy_new_year");
            else
            {
                comboBox2.SelectedIndex = 0;
                menu_color = "default";
            }
            if (comboBox1.SelectedItem != null)
            {
                switch (comboBox1.SelectedItem.ToString())
                {
                    case "ipinfo.io":
                        ipgeo_int = 0;
                        break;

                    case "ipwhois.io":
                        ipgeo_int = 1;
                        break;

                    case "ipgeolocation.io (требуется API ключ)":
                        ipgeo_int = 2;
                        break;
                }
            }
            else
            {
                comboBox1.SelectedIndex = 0;
                VitNX2_MessageBox.Show("Т.к. не был выбран сервис для определения геолокации,\nавтоматически выбран ipinfo.io", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ipgeo_int = 0;
            }
            if (ipgeo_int == 2 && ipgeolocation_api_key == "")
                VitNX2_MessageBox.Show("Для работы сервиса ipgeolocation.io требуется API ключ!", "Ошибка применения настроек", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                try
                {
                    TomlTable toml = new TomlTable
                    {
                        ["BOT"] =
                        {
                            ["token"] = Settings.MemoryValues.token,
                            ["admin"] = admin,
                            ["imgbb_api_key"] = imgbb_api_key
                        },
                        ["GEOLOCATION"] =
                        {
                            ["selected_service"] = ipgeo_int,
                            ["ipgeolocationio_api_key"] = ipgeolocation_api_key
                        },
                        ["PRINT_OPTIONS"] =
                        {
                            ["font"] = TempFont,
                            ["size"] = TempSize
                        },
                        ["UI"] =
                        {
                            ["use_rounded_window_frame_style"] = checkBox1.Checked,
                            ["use_window_mini_mode"] = checkBox2.Checked,
                            ["menu_color"] = menu_color,
                            ["use_window_transparency"] = checkBox3.Checked,
                            ["use_window_animation"] = checkBox4.Checked,
                            ["use_forced_performance"] = forcedPerformanceMode.Checked ? 1 : 0
                        },
                        ["OTHER"] =
                        {
                            ["config_version"] = Values.Config.VERSION,
                            ["hidden_bot_parameters"] = textBox1.Text,
                            ["hidden_application_parameters"] = textBox2.Text
                        }
                    };
                    using (StreamWriter writer = File.CreateText($@"{FileSystem.GetDataPath()}\settings\main.toml"))
                    {
                        toml.WriteTo(writer);
                        writer.Flush();
                    }
                    VitNX2_MessageBox.Show("Настройки успешно применены!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                catch (Exception ex) { VitNX2_MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка записи настроек", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void vitnX2_Button1_Click(object sender, EventArgs e)
        {
            try
            {
                ipgeolocation_api_key = Clipboard.GetText();
                VitNX2_MessageBox.Show("API ключ принят!",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch { }
        }

        private void vitnX2_Button2_Click(object sender, EventArgs e)
        {
            try
            {
                imgbb_api_key = Clipboard.GetText();
                VitNX2_MessageBox.Show("API ключ принят!",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vitnX2_Button1.Visible = (comboBox1.SelectedItem.ToString() == "ipgeolocation.io (требуется API ключ)") ? true : false;
        }

        private void forcedPerformanceMode_CheckedChanged(object sender, EventArgs e)
        {
            if (forcedPerformanceMode.Checked)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
                checkBox2.Checked = true;
                checkBox2.Enabled = false;
                checkBox3.Checked = false;
                checkBox3.Enabled = false;
                checkBox4.Checked = false;
                checkBox4.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
            }
        }
    }
}