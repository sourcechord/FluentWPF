using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SourceChord.FluentWPF
{
    public enum ApplicationTheme
    {
        Light,
        Dark,
    }

    public enum WindowsTheme
    {
        Light,
        Dark,
    }

    public class SystemTheme : ThemeHandler
    {
        private static readonly int WM_WININICHANGE = 0x001A;
        private static readonly int WM_SETTINGCHANGE = WM_WININICHANGE;

        static SystemTheme()
        {
            SystemTheme.Instance = new SystemTheme();
            AppTheme = GetAppTheme();
            WindowsTheme = GetWindowsTheme();
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETTINGCHANGE)
            {
                var systemParmeter = Marshal.PtrToStringAuto(lParam);
                if (systemParmeter == "ImmersiveColorSet")
                {
                    // 再度レジストリから Dark/Lightの設定を取得
                    AppTheme = GetAppTheme();
                    WindowsTheme = GetWindowsTheme();
                    SystemTheme.ThemeChanged?.Invoke(null, null);

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private static ApplicationTheme GetAppTheme()
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            // キーが存在しないときはnullが返る
            if (regkey == null) return ApplicationTheme.Light;
            var intValue = (int)regkey.GetValue("AppsUseLightTheme", 1);

            return intValue == 0 ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        private static WindowsTheme GetWindowsTheme()
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            // キーが存在しないときはnullが返る
            if (regkey == null) return WindowsTheme.Light;
            var intValue = (int)regkey.GetValue("SystemUsesLightTheme", 1);

            return intValue == 0 ? WindowsTheme.Dark : WindowsTheme.Light;
        }

        private static ApplicationTheme appTheme;
        public static ApplicationTheme AppTheme
        {
            get { return appTheme; }
            private set { if (!object.Equals(appTheme, value)) { appTheme = value; OnStaticPropertyChanged(); } }
        }

        private static WindowsTheme windowsTheme;
        public static WindowsTheme WindowsTheme
        {
            get { return windowsTheme; }
            private set { if (!object.Equals(windowsTheme, value)) { windowsTheme = value; OnStaticPropertyChanged(); } }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }


        public static event EventHandler ThemeChanged;
    }
}
