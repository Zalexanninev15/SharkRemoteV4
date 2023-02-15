using System.Runtime.InteropServices;

namespace Shark_Remote.Helpers
{
    public class FileSystem
    {
        public static string data_path_var = GetDataPath();

        private static string GetDataPath()
        {
            if (File.Exists("storage.txt"))
            {
                try
                {
                    string data_path = File.ReadAllLines("storage.txt")[0].Replace("\"", "");
                    data_path = data_path.EndsWith("\\") ? data_path.Remove(data_path.Length - 1, 1) : data_path;
                    if (data_path == "")
                        return $@"{Application.StartupPath}\data";
                    else
                        return data_path;
                }
                catch
                {
                    return $@"{Application.StartupPath}\data";
                }
            }
            else
                return $@"{Application.StartupPath}\data";
        }

        public static List<string> ReturnRecursFList(string path)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] folders = Directory.GetDirectories(path);
                foreach (string folder in folders)
                    ls.Add("Папка: " + folder);
                string[] files = Directory.GetFiles(path);
                foreach (string filename in files)
                    ls.Add("Файл: " + filename);
            }
            catch (Exception e) { ls.Add(e.Message.Trim('\n')); }
            return ls;
        }
    }

    public sealed class DesktopWallpaper
    {
        private DesktopWallpaper()
        { }

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public static void Set(string sourceWallpaper)
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni);

            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            File.Copy(sourceWallpaper, tempPath, true);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}