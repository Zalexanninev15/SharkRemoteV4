using Shark_Remote.Helpers;
using VitNX3.Functions.Win32;

namespace Shark_Remote
{
    public partial class Downloader : Form
    {
        public static class Values
        {
            public static string url = "";
            public static string save_path = "";
            public static bool done = false;
        }

        public Downloader()
        {
            InitializeComponent();
            ControlBox = false;
            ClientSize = new Size(UI.Window.Dpi(228), UI.Window.Dpi(100));
            Size = new Size(UI.Window.Dpi(228), UI.Window.Dpi(100));
            try { VitNX3.Functions.WindowAndControls.Window.SetWindowsTenAndHighStyleForWinFormTitleToDark(Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height); } catch { }
            Import.SetProcessDpiAwareness(Enums.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE);
            Import.ReleaseCapture();
            visualProgress.Value = 0;
            DownloadFromLink();
        }

        private const int WS_CAPTION = 0x00C00000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.Style &= WS_CAPTION;
                return myCp;
            }
        }

        private async void DownloadFromLink()
        {
            using (HttpClient client = new HttpClient())
            {
                string filePath = Path.Combine(Path.GetDirectoryName(Values.save_path), $"{Path.GetFileName(Values.url)}");
                try
                {
                    using (var response = await client.GetAsync(Values.url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var contentLength = response.Content.Headers.ContentLength ?? -1L;

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, true))
                        {
                            var buffer = new byte[4096];
                            var totalBytesRead = 0L;
                            var bytesRead = 0;
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;
                                if (contentLength > 0)
                                {
                                    var percentage = (int)(totalBytesRead * 100 / contentLength);
                                    visualProgress.Value = percentage;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); }
                        catch (Exception) { }
                    }
                    VitNX2.UI.ControlsV2.VitNX2_MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка скачивания файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Values.done = true;
                    Close();
                }
            }
        }

        private void vitnX2_Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}