using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Shark_Remote.Helpers
{
    public class ProductMode
    {
        public static Image? GetBitmapScreenshot(string processName)
        {
            Image? img = null;
            Thread t = new(() =>
            {
                IntPtr handle = GetWindowHandle(processName);
                if (User32.IsIconic(handle))
                    User32.ShowWindowAsync(handle, User32.SHOWNORMAL);

                User32.SetForegroundWindow(handle);
                SendKeys.SendWait("%({PRTSC})");
                Thread.Sleep(200);
                img = Clipboard.GetImage();
                IntPtr clipWindow = User32.GetOpenClipboardWindow();
                User32.OpenClipboard(clipWindow);
                User32.EmptyClipboard();
                User32.CloseClipboard();
                Thread.Sleep(100);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            return img;
        }

        public static List<string> GetAllWindowHandleNames()
        {
            List<string> windowHandleNames = new();
            foreach (Process window in Process.GetProcesses())
            {
                window.Refresh();
                if (window.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(window.MainWindowTitle))
                    windowHandleNames.Add(window.ProcessName);
            }
            return windowHandleNames;
        }

        private static IntPtr GetWindowHandle(string name)
        {
            var process = Process.GetProcessesByName(name).FirstOrDefault();
            if (process != null && process.MainWindowHandle != IntPtr.Zero)
                return process.MainWindowHandle;
            return IntPtr.Zero;
        }

        private class User32
        {
            public const int SHOWNORMAL = 1;
            public const int SHOWMINIMIZED = 2;
            public const int SHOWMAXIMIZED = 3;

            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsIconic(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool CloseClipboard();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll")]
            public static extern bool EmptyClipboard();

            [DllImport("user32.dll")]
            public static extern IntPtr GetOpenClipboardWindow();
        }
    }
}