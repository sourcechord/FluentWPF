using SourceChord.FluentWPF.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public class AccentColors : ThemeHandler
    {
        [DllImport("uxtheme.dll", EntryPoint = "#95", CharSet = CharSet.Unicode)]
        internal static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [DllImport("uxtheme.dll", EntryPoint = "#96", CharSet = CharSet.Unicode)]
        internal static extern uint GetImmersiveColorTypeFromName(string name);
        [DllImport("uxtheme.dll", EntryPoint = "#98", CharSet = CharSet.Unicode)]
        internal static extern uint GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);


        private static readonly int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;



        static AccentColors()
        {
            AccentColors.Instance = new AccentColors();
            Initialize();
        }

        public AccentColors()
        {

        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                // 再取得
                Initialize();
            }

            return IntPtr.Zero;
        }

        public static Color GetColorByTypeName(string name)
        {
            var colorSet = GetImmersiveUserColorSetPreference(false, false);
            var colorType = GetImmersiveColorTypeFromName(name);
            var rawColor = GetImmersiveColorFromColorSetEx(colorSet, colorType, false, 0);

            var bytes = BitConverter.GetBytes(rawColor);
            return Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
        }


        #region Colors
        private static Color immersiveSystemAccent;
        public static Color ImmersiveSystemAccent
        {
            get { return immersiveSystemAccent; }
            private set { if (!object.Equals(immersiveSystemAccent, value)) { immersiveSystemAccent = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentDark1;
        public static Color ImmersiveSystemAccentDark1
        {
            get { return immersiveSystemAccentDark1; }
            private set { if (!object.Equals(immersiveSystemAccentDark1, value)) { immersiveSystemAccentDark1 = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentDark2;
        public static Color ImmersiveSystemAccentDark2
        {
            get { return immersiveSystemAccentDark2; }
            private set { if (!object.Equals(immersiveSystemAccentDark2, value)) { immersiveSystemAccentDark2 = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentDark3;
        public static Color ImmersiveSystemAccentDark3
        {
            get { return immersiveSystemAccentDark3; }
            private set { if (!object.Equals(immersiveSystemAccentDark3, value)) { immersiveSystemAccentDark3 = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentLight1;
        public static Color ImmersiveSystemAccentLight1
        {
            get { return immersiveSystemAccentLight1; }
            private set { if (!object.Equals(immersiveSystemAccentLight1, value)) { immersiveSystemAccentLight1 = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentLight2;
        public static Color ImmersiveSystemAccentLight2
        {
            get { return immersiveSystemAccentLight2; }
            private set { if (!object.Equals(immersiveSystemAccentLight2, value)) { immersiveSystemAccentLight2 = value; OnStaticPropertyChanged(); } }
        }

        private static Color immersiveSystemAccentLight3;
        public static Color ImmersiveSystemAccentLight3
        {
            get { return immersiveSystemAccentLight3; }
            private set { if (!object.Equals(immersiveSystemAccentLight3, value)) { immersiveSystemAccentLight3 = value; OnStaticPropertyChanged(); } }
        }
        #endregion


        #region Brushes
        private static Brush immersiveSystemAccentBrush;
        public static Brush ImmersiveSystemAccentBrush
        {
            get { return immersiveSystemAccentBrush; }
            private set { if (!object.Equals(immersiveSystemAccentBrush, value)) { immersiveSystemAccentBrush = value; OnStaticPropertyChanged(); } }
        }

        private static Brush immersiveSystemAccentDark1Brush;
        public static Brush ImmersiveSystemAccentDark1Brush
        {
            get { return immersiveSystemAccentDark1Brush; }
            private set { if (!object.Equals(immersiveSystemAccentDark1Brush, value)) { immersiveSystemAccentDark1Brush = value; OnStaticPropertyChanged(); } }
        }
        private static Brush immersiveSystemAccentDark2Brush;
        public static Brush ImmersiveSystemAccentDark2Brush
        {
            get { return immersiveSystemAccentDark2Brush; }
            private set { if (!object.Equals(immersiveSystemAccentDark2Brush, value)) { immersiveSystemAccentDark2Brush = value; OnStaticPropertyChanged(); } }
        }
        private static Brush immersiveSystemAccentDark3Brush;
        public static Brush ImmersiveSystemAccentDark3Brush
        {
            get { return immersiveSystemAccentDark3Brush; }
            private set { if (!object.Equals(immersiveSystemAccentDark3Brush, value)) { immersiveSystemAccentDark3Brush = value; OnStaticPropertyChanged(); } }
        }

        private static Brush immersiveSystemAccentLight1Brush;
        public static Brush ImmersiveSystemAccentLight1Brush
        {
            get { return immersiveSystemAccentLight1Brush; }
            private set { if (!object.Equals(immersiveSystemAccentLight1Brush, value)) { immersiveSystemAccentLight1Brush = value; OnStaticPropertyChanged(); } }
        }
        private static Brush immersiveSystemAccentLight2Brush;
        public static Brush ImmersiveSystemAccentLight2Brush
        {
            get { return immersiveSystemAccentLight2Brush; }
            private set { if (!object.Equals(immersiveSystemAccentLight2Brush, value)) { immersiveSystemAccentLight2Brush = value; OnStaticPropertyChanged(); } }
        }
        private static Brush immersiveSystemAccentLight3Brush;
        public static Brush ImmersiveSystemAccentLight3Brush
        {
            get { return immersiveSystemAccentLight3Brush; }
            private set { if (!object.Equals(immersiveSystemAccentLight3Brush, value)) { immersiveSystemAccentLight3Brush = value; OnStaticPropertyChanged(); } }
        }
        #endregion



        internal static void Initialize()
        {
            // 各種Color定義
            if (!SystemInfo.IsWin7())
            {
                ImmersiveSystemAccent = GetColorByTypeName("ImmersiveSystemAccent");
                ImmersiveSystemAccentDark1 = GetColorByTypeName("ImmersiveSystemAccentDark1");
                ImmersiveSystemAccentDark2 = GetColorByTypeName("ImmersiveSystemAccentDark2");
                ImmersiveSystemAccentDark3 = GetColorByTypeName("ImmersiveSystemAccentDark3");
                ImmersiveSystemAccentLight1 = GetColorByTypeName("ImmersiveSystemAccentLight1");
                ImmersiveSystemAccentLight2 = GetColorByTypeName("ImmersiveSystemAccentLight2");
                ImmersiveSystemAccentLight3 = GetColorByTypeName("ImmersiveSystemAccentLight3");
            }
            else
            {
                // Windows7の場合は、OSにテーマカラーの設定はないので、固定値を使用する。
                ImmersiveSystemAccent = (Color)ColorConverter.ConvertFromString("#FF2990CC");
                ImmersiveSystemAccentDark1 = (Color)ColorConverter.ConvertFromString("#FF2481B6");
                ImmersiveSystemAccentDark2 = (Color)ColorConverter.ConvertFromString("#FF2071A1");
                ImmersiveSystemAccentDark3 = (Color)ColorConverter.ConvertFromString("#FF205B7E");
                ImmersiveSystemAccentLight1 = (Color)ColorConverter.ConvertFromString("#FF2D9FE1");
                ImmersiveSystemAccentLight2 = (Color)ColorConverter.ConvertFromString("#FF51A5D6");
                ImmersiveSystemAccentLight3 = (Color)ColorConverter.ConvertFromString("#FF7BB1D0");
            }

            // ブラシ類の定義
            ImmersiveSystemAccentBrush = CreateBrush(ImmersiveSystemAccent);
            ImmersiveSystemAccentDark1Brush = CreateBrush(ImmersiveSystemAccentDark1);
            ImmersiveSystemAccentDark2Brush = CreateBrush(ImmersiveSystemAccentDark2);
            ImmersiveSystemAccentDark3Brush = CreateBrush(ImmersiveSystemAccentDark3);
            ImmersiveSystemAccentLight1Brush = CreateBrush(ImmersiveSystemAccentLight1);
            ImmersiveSystemAccentLight2Brush = CreateBrush(ImmersiveSystemAccentLight2);
            ImmersiveSystemAccentLight3Brush = CreateBrush(ImmersiveSystemAccentLight3);
        }

        internal static Brush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }


        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        protected static void OnStaticPropertyChanged([CallerMemberName]string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
