using Newtonsoft.Json.Linq;
using Shark_Remote.Engine;
using System.Globalization;
using System.Reflection;

namespace Shark_Remote.Helpers
{
    public class AppUpdater
    {
        public static string app_path = Assembly.GetEntryAssembly().Location.Replace(".dll", ".exe");
        public static bool working = false;

        public static async Task Upgrade()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (Network.InternetOk())
                    {
                        string update_json = await Network.HttpResponse("https://sharkremote.neocities.org/assets/update.json");
                        string[] new_version = GetJsonValues(update_json);
                        if (Convert.ToInt32(Convert.ToString(new_version[0], CultureInfo.InvariantCulture).Replace(".", "")) >= Convert.ToInt32(Convert.ToString(Values.AppInfo.version, CultureInfo.InvariantCulture).Replace(".", "")))
                        {
                            //if (Convert.ToInt32(Convert.ToString(current_version, CultureInfo.InvariantCulture).Replace(".", "")) < Convert.ToInt32(Convert.ToString(new_version, CultureInfo.InvariantCulture).Replace(".", "")))
                            //{
                            //    // Release = 0
                            //    if (Convert.ToInt32(odata[0]) == 0)
                            //    {
                            //        if (VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Доступна новая версия. Хотите обновиться?", "Доступна новая версия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            //        {
                            //            if (File.Exists($@"{FileSystem.GetDataPath()}\Shark Remote.exe"))
                            //                File.Delete($@"{FileSystem.GetDataPath()}\Shark Remote.exe");
                            //            VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Сейчас будет произведено обновление до новой версии", "Требуется подтверждение", MessageBoxButtons.OK);
                            //            using (var stream = await client.GetStreamAsync("https://codeberg.org/Zalexanninev15/open-api/raw/branch/main/Shark%20Remote/Shark%20Remote.exe"))
                            //            using (var file = new FileStream($@"{FileSystem.GetDataPath()}\Shark Remote.exe", FileMode.CreateNew))
                            //                await stream.CopyToAsync(file);
                            //            VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"Новая версия скачана. Чтобы её установить, переместите файл \"{FileSystem.GetDataPath()}\\Shark Remote.exe\" по пути \"{AppValues.AppInfo.startup_path}\" с заменой файла \"Shark Remote.exe\" на новый. После нажатия \"OK\" в Проводнике Windows будет открыта папка с файлом новой версии", "Обновление почти завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //            try { File.WriteAllText($@"{FileSystem.GetDataPath()}\Куда переместить Shark Remote новой версии.txt", Application.StartupPath); } catch { }
                            //        }
                            //    }
                            //    // Patch = 1
                            //    else if (Convert.ToInt32(odata[0]) == 1)
                            //    {
                            //        if (File.Exists($@"{FileSystem.GetDataPath()}\Shark Remote.exe"))
                            //            File.Delete($@"{FileSystem.GetDataPath()}\Shark Remote.exe");
                            //        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Сейчас будет произведено обновление до новой версии", "Требуется подтверждение", MessageBoxButtons.OK);
                            //        using (var stream = await client.GetStreamAsync("https://codeberg.org/Zalexanninev15/open-api/raw/branch/main/Shark%20Remote/Shark%20Remote.exe"))
                            //        using (var file = new FileStream($@"{FileSystem.GetDataPath()}\Shark Remote.exe", FileMode.CreateNew))
                            //            await stream.CopyToAsync(file);
                            //        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"Патч скачан. Чтобы его установить, переместите файл \"{FileSystem.GetDataPath()}\\Shark Remote.exe\" по пути \"{AppValues.AppInfo.startup_path}\" с заменой файла \"Shark Remote.exe\" на новый. После нажатия \"OK\" в Проводнике Windows будет открыта папка с файлом новой версии", "Обновление почти завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //        try { File.WriteAllText($@"{FileSystem.GetDataPath()}\Куда переместить Shark Remote новой версии.txt", Application.StartupPath); } catch { }
                            //    }
                            //    // Broken update = 2
                            //}
                            //else
                            //    VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("У вас последняя версия!", "Новых версий не обнаружено", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (!IsThisBoostyVersion(new_version[1]) || Settings.Read("OTHER", "app_h0") != "update.boosty_only=true")
                            {
                                // DOWNLOAD PREVIEW VERSION
                            }
                            else
                            {
                                // BOOSTY POST
                            }
                        }
                        else
                            VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("У вас последняя версия!", "Новых версий не обнаружено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show("Нет доступа с сервером!", "Проблема с подключением", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"Нет доступа в Сеть!\nПодробнее: {ex.Message}", "Ошибка проверки обновлений", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            working = false;
        }

        private static string[] GetJsonValues(string updateJson)
        {
            var obj = JObject.Parse(updateJson);
            return new string[] { (string)obj["preview_boosty_version"], (string)obj["boosty_version_rev"], (string)obj["boosty_post_link"] };
        }

        private static bool IsThisBoostyVersion(string boosty_md5)
        {
            string app_md5 = VitNX3.Functions.FileSystem.File.GetMD5("Shark Remote.exe");
            return app_md5.ToLower() == boosty_md5.ToLower();
        }
    }
}