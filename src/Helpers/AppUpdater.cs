using Newtonsoft.Json.Linq;

using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Shark_Remote.Helpers
{
    public class AppUpdater
    {
        private static string current_version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public static async Task Upgrade()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (Network.InternetOk())
                    {
                        string update_json = await Network.HttpResponse("https://sharkremote.neocities.org/assets/update.json");
                        string new_version = GetVersion(update_json);
                        var odata = GetOther(update_json);
                        if (Convert.ToInt32(Convert.ToString(current_version, CultureInfo.InvariantCulture).Replace(".", "")) >= Convert.ToInt32(Convert.ToString(odata[1], CultureInfo.InvariantCulture).Replace(".", "")))
                        {
                            if (Convert.ToInt32(Convert.ToString(current_version, CultureInfo.InvariantCulture).Replace(".", "")) >= Convert.ToInt32(Convert.ToString(new_version, CultureInfo.InvariantCulture).Replace(".", "")))
                            {
                                // Simple update = 0
                                if (Convert.ToInt32(odata[0]) == 0)
                                {
                                    if (VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Доступна новая версия. Хотите обновиться?", "Доступна новая версия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        if (File.Exists("new.exe"))
                                            File.Delete("new.exe");
                                        using (var stream = await client.GetStreamAsync("https://codeberg.org/Voocfof/open-api/raw/branch/main/Shark%20Remote/Shark%20Remote.exe"))
                                        using (var file = new FileStream("new.exe", FileMode.CreateNew))
                                            await stream.CopyToAsync(file);
                                        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Сейчас будет произведено обновление до новой версии", "Требуется подтверждение", MessageBoxButtons.OK);
                                        Process.Start("new.exe", "--ctu-p0");
                                        Application.Exit();
                                    }
                                }
                                // Patch = 1
                                else if (Convert.ToInt32(odata[0]) == 1)
                                {
                                    if (File.Exists("new.exe"))
                                        File.Delete("new.exe");
                                    using (var stream = await client.GetStreamAsync("https://codeberg.org/Voocfof/open-api/raw/branch/main/Shark%20Remote/Shark%20Remote.exe"))
                                    using (var file = new FileStream("new.exe", FileMode.CreateNew))
                                        await stream.CopyToAsync(file);
                                    VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Сейчас будет произведено обновление до новой версии", "Требуется подтверждение", MessageBoxButtons.OK);
                                    Process.Start("new.exe", "--ctu-p0");
                                    Application.Exit();
                                }
                                // Broken update = 2
                            }
                            else
                                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("У вас последняя версия!", "Новых версий не обнаружено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Ваша версия более не поддерживается, обновление необходимо производить вручную (с полным удалением настроек)", "Ваша версия не поддерживается", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Нет доступа в Сеть!", "Ошибка проверки обновлений", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Нет доступа в Сеть!", "Ошибка проверки обновлений", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetVersion(string updateJson)
        {
            var obj = JObject.Parse(updateJson);
            return (string)obj["version"];
        }

        private static string[] GetOther(string updateJson)
        {
            var obj = JObject.Parse(updateJson);
            string type = (string)obj["type"];
            string minVersion = (string)obj["minimal_version_for_update"];
            return new string[] { type, minVersion };
        }
    }
}