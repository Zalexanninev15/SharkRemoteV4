using LibreHardwareMonitor.Hardware;
using System.Management;
using VitNX2.UI.ControlsV2;
using WindowsInput;

namespace Shark_Remote.Helpers
{
    public class UI
    {
        public class Window
        {
            public static int Dpi(int input) => (int)Math.Round(input * 1.0);
        }

        public static bool IsWeakGPU()
        {
            try
            {
                ManagementObjectSearcher searcher11 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController");
                foreach (ManagementObject queryObj in searcher11.Get())
                {
                    string videoCardName = queryObj["Caption"].ToString();
                    if (videoCardName.ToLower().Contains("vm") || videoCardName.ToLower().Contains("vbox") || videoCardName.ToLower().Contains("virtual") || videoCardName.ToLower().Contains("intel"))
                        return true;
                }
                return false;
            }
            catch { return true; }
        }

        public static void OpenLink(string link)
        {
            if (VitNX3.Functions.AppsAndProcesses.Processes.OpenLink(link) == false)
            {
                VitNX2_MessageBox.Show("Невозможно открыть браузер,\nссылка скопирована в буфер обмена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(link);
            }
        }

        public static void MyButton_MouseLeave(object? sender, EventArgs? e)
        {
            VitNX2_Button? temp = sender as VitNX2_Button;
            temp.BorderColor = Color.FromArgb(26, 32, 48);
        }

        public static void MyButton_MouseEnter(object? sender, EventArgs? e)
        {
            VitNX2_Button? temp = sender as VitNX2_Button;
            temp.BorderColor = Color.FromArgb(39, 45, 59);
        }

        public static int SetMenuColorComboBox(string color_name)
        {
            switch (color_name)
            {
                case "default":
                    return 0;

                case "native":
                    return 1;

                case "keyboard":
                    return 2;

                case "unigram":
                    return 3;

                case "vivaldi":
                    return 4;

                case "github":
                    return 5;

                case "μtorrent":
                    return 6;

                case "happy_new_year":
                    return 7;

                case "happy_new_year_with_icons":
                    return 8;

                case "happy_new_year_with_icons_and_hide_log":
                    return 9;

                default:
                    return 0;
            }
        }
    }

    public class Keyboard
    {
        public static VirtualKeyCode ConvertToVirtualKeyCode(string key)
        {
            switch (key.ToUpper())
            {
                case "WIN":
                    return VirtualKeyCode.LWIN;

                case "CTRL":
                    return VirtualKeyCode.CONTROL;

                case "ALT":
                    return VirtualKeyCode.MENU;

                case "SHIFT":
                    return VirtualKeyCode.SHIFT;

                case "SPACE":
                    return VirtualKeyCode.SPACE;

                case "BACKSPACE":
                    return VirtualKeyCode.BACK;

                case "TAB":
                    return VirtualKeyCode.TAB;

                case "PRINT":
                    return VirtualKeyCode.PRINT;

                case "INSERT":
                    return VirtualKeyCode.INSERT;

                case "END":
                    return VirtualKeyCode.END;

                case "PAGEUP":
                    return VirtualKeyCode.PRIOR;

                case "PAGEDOWN":
                    return VirtualKeyCode.NEXT;

                case "CAPSLOCK":
                    return VirtualKeyCode.CAPITAL;

                case "ESC":
                    return VirtualKeyCode.ESCAPE;

                case "UP":
                    return VirtualKeyCode.UP;

                case "LEFT":
                    return VirtualKeyCode.LEFT;

                case "DOWN":
                    return VirtualKeyCode.DOWN;

                case "RIGHT":
                    return VirtualKeyCode.RIGHT;

                case "HOME":
                    return VirtualKeyCode.HOME;

                case "DEL":
                    return VirtualKeyCode.DELETE;

                case "PLAY":
                    return VirtualKeyCode.PLAY;

                case "PAUSE":
                    return VirtualKeyCode.PAUSE;

                case "F1":
                    return VirtualKeyCode.F1;

                case "F2":
                    return VirtualKeyCode.F2;

                case "F3":
                    return VirtualKeyCode.F3;

                case "F4":
                    return VirtualKeyCode.F4;

                case "F5":
                    return VirtualKeyCode.F5;

                case "F6":
                    return VirtualKeyCode.F6;

                case "F7":
                    return VirtualKeyCode.F7;

                case "F8":
                    return VirtualKeyCode.F8;

                case "F9":
                    return VirtualKeyCode.F9;

                case "F10":
                    return VirtualKeyCode.F10;

                case "F11":
                    return VirtualKeyCode.F11;

                case "F12":
                    return VirtualKeyCode.F12;

                case "1":
                    return VirtualKeyCode.NUMPAD1;

                case "2":
                    return VirtualKeyCode.NUMPAD2;

                case "3":
                    return VirtualKeyCode.NUMPAD3;

                case "4":
                    return VirtualKeyCode.NUMPAD4;

                case "5":
                    return VirtualKeyCode.NUMPAD5;

                case "6":
                    return VirtualKeyCode.NUMPAD6;

                case "7":
                    return VirtualKeyCode.NUMPAD7;

                case "8":
                    return VirtualKeyCode.NUMPAD8;

                case "9":
                    return VirtualKeyCode.NUMPAD9;

                case "0":
                    return VirtualKeyCode.NUMPAD0;

                case "A":
                    return VirtualKeyCode.VK_A;

                case "B":
                    return VirtualKeyCode.VK_B;

                case "C":
                    return VirtualKeyCode.VK_C;

                case "D":
                    return VirtualKeyCode.VK_D;

                case "E":
                    return VirtualKeyCode.VK_E;

                case "F":
                    return VirtualKeyCode.VK_F;

                case "G":
                    return VirtualKeyCode.VK_G;

                case "H":
                    return VirtualKeyCode.VK_H;

                case "I":
                    return VirtualKeyCode.VK_I;

                case "J":
                    return VirtualKeyCode.VK_J;

                case "K":
                    return VirtualKeyCode.VK_K;

                case "L":
                    return VirtualKeyCode.VK_L;

                case "M":
                    return VirtualKeyCode.VK_M;

                case "N":
                    return VirtualKeyCode.VK_N;

                case "O":
                    return VirtualKeyCode.VK_O;

                case "P":
                    return VirtualKeyCode.VK_P;

                case "Q":
                    return VirtualKeyCode.VK_Q;

                case "R":
                    return VirtualKeyCode.VK_R;

                case "S":
                    return VirtualKeyCode.VK_S;

                case "T":
                    return VirtualKeyCode.VK_T;

                case "U":
                    return VirtualKeyCode.VK_U;

                case "V":
                    return VirtualKeyCode.VK_V;

                case "W":
                    return VirtualKeyCode.VK_W;

                case "X":
                    return VirtualKeyCode.VK_X;

                case "Y":
                    return VirtualKeyCode.VK_Y;

                case "Z":
                    return VirtualKeyCode.VK_Z;

                default:
                    return VirtualKeyCode.SPACE;
            }
        }
    }

    public class HardwareUsage : IVisitor
    {
        public void VisitComputer(IComputer computer) => computer.Traverse(this);

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor)
        { }

        public void VisitParameter(IParameter parameter)
        { }
    }

    public class Service
    {
        public static bool IsInstalled()
        {
            ManagementObjectSearcher services = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
            ManagementObjectCollection services_collection = services.Get();
            foreach (ManagementObject service in services_collection)
            {
                if (Convert.ToString(service["Name"]) == "SharkRemoteDaemon")
                    return true;
            }
            return false;
        }
    }
}