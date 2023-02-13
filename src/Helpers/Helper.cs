using VitNX2.UI.ControlsV2;

namespace Shark_Remote.Helpers
{
    public class UI
    {
        public static void MyButton_MouseLeave(object sender, EventArgs e)
        {
            VitNX2_Button? temp = sender as VitNX2_Button;
            temp.BorderColor = Color.FromArgb(26, 32, 48);
        }

        public static void MyButton_MouseEnter(object sender, EventArgs e)
        {
            VitNX2_Button? temp = sender as VitNX2_Button;
            temp.BorderColor = Color.FromArgb(39, 45, 59);
        }
    }

    public class ModernDownloader
    {
        public static async Task GetFile(string fileName, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (Network.InternetOk())
                    {
                        using (var stream = await client.GetStreamAsync(url))
                        using (var file = new FileStream(fileName, FileMode.CreateNew))
                            await stream.CopyToAsync(file);
                    }
                    else
                        VitNX2_MessageBox.Show("Нет доступа в Сеть! [1]", "Ошибка скачивания", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                VitNX2_MessageBox.Show("Нет доступа в Сеть! [2]", "Ошибка скачивания", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //public async static Task GetMegaFile(string filename, string link, bool wait_to_end = false)
        //{
        //    var client = new MegaApiClient();
        //    client.LoginAnonymous();
        //    Uri link_uri = new Uri(link);
        //    await client.DownloadFileAsync(link_uri, filename);
        //    client.Logout();
        //    if ((!client.IsLoggedIn) && wait_to_end)
        //        downloaded = true;
        //    else
        //        downloaded = false;
        //}
    }
}