using VitNX3.Functions.Win32;

namespace Shark_Remote.Win64
{
    public class Keyboard
    {
        public static void KeyDown(Keys vKey)
        {
            Import.keybd_event((byte)vKey, 0,
                (int)Enums.KEYEVENTF.KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            Import.keybd_event((byte)vKey, 0,
                (int)Enums.KEYEVENTF.KEYEVENTF_EXTENDEDKEY |
                (int)Enums.KEYEVENTF.KEYEVENTF_KEYUP, 0);
        }

        public static void WindowsKeyboardEventsAPI(int status, string keys = "none")
        {
            switch (status)
            {
                case 0:
                    {
                        KeyDown(Keys.LWin);
                        KeyDown(Keys.D);
                        KeyUp(Keys.LWin);
                        KeyUp(Keys.D);
                        break;
                    }
            }
        }
    }

    //if (!usbDevice.Caption.Contains("USB-концентратор") && !usbDevice.Caption.StartsWith("Составное")
    //    && !usbDevice.Caption.StartsWith("USB-устройство ввода") && !usbDevice.DeviceID.StartsWith(@"USBSTOR\"))
    //    returnString += $"Устройство: {usbDevice.Caption}\nDeviceID: {usbDevice.DeviceID}\n\n";
}