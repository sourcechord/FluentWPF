using System;
using System.Collections.Generic;
using System.Linq;
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

    public class SystemTheme : ThemeHandler
    {
        private static readonly int WM_WININICHANGE = 0x001A;
        private static readonly int WM_SETTINGCHANGE = WM_WININICHANGE;

        static SystemTheme()
        {
            SystemTheme.Instance = new SystemTheme();
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETTINGCHANGE)
            {
                var systemParmeter = Marshal.PtrToStringAuto(lParam);
                if (systemParmeter == "ImmersiveColorSet")
                {
                    // 再度レジストリから Dark/Lightの設定を取得
                    var theme = GetTheme();

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private static ApplicationTheme GetTheme()
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            // キーが存在しないときはnullが返る
            if (regkey == null) return ApplicationTheme.Light;

            var intValue = (int)regkey.GetValue("AppsUseLightTheme");
            Console.WriteLine(intValue);

            return intValue == 0 ? ApplicationTheme.Light : ApplicationTheme.Dark;
        }
    }
}
