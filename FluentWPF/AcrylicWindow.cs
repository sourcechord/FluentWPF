using SourceChord.FluentWPF.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace SourceChord.FluentWPF
{
    public enum AcrylicAccentState
    {
        Default             = -1,
        Disabled            = 0,
        Gradient            = 1,
        TransparentGradient = 2,
        BlurBehind          = 3,
        AcrylicBlurBehind   = 4,
    }

    public enum AcrylicWindowStyle
    {
        Normal,
        NoIcon,
        None,
    }

    public enum TitleBarMode
    {
        Default,
        Extend,
    }

    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SourceChord.FluentWPF"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SourceChord.FluentWPF;assembly=SourceChord.FluentWPF"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:AcrylicWindow/>
    ///
    /// </summary>
    public class AcrylicWindow : Window
    {
        static AcrylicWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicWindow), new FrameworkPropertyMetadata(typeof(AcrylicWindow)));

            TintColorProperty = AcrylicElement.TintColorProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.Inherits));
            TintOpacityProperty = AcrylicElement.TintOpacityProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(0.6, FrameworkPropertyMetadataOptions.Inherits));
            NoiseOpacityProperty = AcrylicElement.NoiseOpacityProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(0.03, FrameworkPropertyMetadataOptions.Inherits));
            FallbackColorProperty = AcrylicElement.FallbackColorProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.Inherits));
            AcrylicAccentStateProperty = AcrylicElement.AcrylicAccentStateProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(AcrylicAccentState.Default, FrameworkPropertyMetadataOptions.Inherits));
            ExtendViewIntoTitleBarProperty = AcrylicElement.ExtendViewIntoTitleBarProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
            AcrylicWindowStyleProperty = AcrylicElement.AcrylicWindowStyleProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(AcrylicWindowStyle.Normal, FrameworkPropertyMetadataOptions.Inherits));
            TitleBarProperty = AcrylicElement.TitleBarProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            TitleBarModeProperty = AcrylicElement.TitleBarModeProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(TitleBarMode.Default, FrameworkPropertyMetadataOptions.Inherits));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnableBlur(this);

            var caption = this.GetTemplateChild("captionGrid") as FrameworkElement;
            if (caption != null)
            {
                caption.SizeChanged += (s, e) =>
                {
                    var chrome = WindowChrome.GetWindowChrome(this);
                    chrome.CaptionHeight = e.NewSize.Height;
                };
            }
        }

        private static readonly int WM_ENTERSIZEMOVE = 0x0231;
        private static readonly int WM_EXITSIZEMOVE = 0x0232;
        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        internal static ConditionalWeakTable<Window, CommandBinding> _closeCommandTable = new ConditionalWeakTable<Window, CommandBinding>();
        internal static ConditionalWeakTable<Window, CommandBinding> _minimizeCommandTable = new ConditionalWeakTable<Window, CommandBinding>();
        internal static ConditionalWeakTable<Window, CommandBinding> _maximizeCommandTable = new ConditionalWeakTable<Window, CommandBinding>();
        internal static ConditionalWeakTable<Window, CommandBinding> _restoreCommandTable = new ConditionalWeakTable<Window, CommandBinding>();

        internal static void EnableBlur(Window win)
        {
            var windowHelper = new WindowInteropHelper(win);

            var source = HwndSource.FromHwnd(windowHelper.Handle);
            source.AddHook(WndProc);

            // ウィンドウに半透明のアクリル効果を適用する
            var state = AcrylicWindow.GetAcrylicAccentState(win);
            SetBlur(win, state);

            // タイトルバーの各種ボタンで利用するコマンドの設定
            var closeBinding = new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => { SystemCommands.CloseWindow(win); });
            var minimizeBinding = new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => { SystemCommands.MinimizeWindow(win); });
            var maximizeBinding = new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => { SystemCommands.MaximizeWindow(win); });
            var restoreBinding = new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => { SystemCommands.RestoreWindow(win); });
            win.CommandBindings.Add(closeBinding);
            win.CommandBindings.Add(minimizeBinding);
            win.CommandBindings.Add(maximizeBinding);
            win.CommandBindings.Add(restoreBinding);

            _closeCommandTable.Add(win, closeBinding);
            _minimizeCommandTable.Add(win, minimizeBinding);
            _maximizeCommandTable.Add(win, maximizeBinding);
            _restoreCommandTable.Add(win, restoreBinding);


            // WPFのSizeToContentのバグ対策
            // (WindowChrome使用時に、SizeToContentのウィンドウサイズ計算が正しく行われない)
            void onContentRendered(object sender, EventArgs e)
            {
                if (win.SizeToContent != SizeToContent.Manual)
                {
                    win.InvalidateMeasure();
                }

                win.ContentRendered -= onContentRendered;
            }
            win.ContentRendered += onContentRendered;
        }

        internal static void DisableBlur(Window win)
        {
            var windowHelper = new WindowInteropHelper(win);

            var source = HwndSource.FromHwnd(windowHelper.Handle);
            source.RemoveHook(WndProc);

            // アクリル効果を解除する
            SetBlur(win, AcrylicAccentState.Disabled);

            // コマンド解除
            if (_closeCommandTable.TryGetValue(win, out var closeBinding))
            {
                win.CommandBindings.Remove(closeBinding);
                _closeCommandTable.Remove(win);
            }

            if (_minimizeCommandTable.TryGetValue(win, out var minimizeBinding))
            {
                win.CommandBindings.Remove(minimizeBinding);
                _minimizeCommandTable.Remove(win);
            }

            if (_maximizeCommandTable.TryGetValue(win, out var maximizeBinding))
            {
                win.CommandBindings.Remove(maximizeBinding);
                _maximizeCommandTable.Remove(win);
            }

            if (_restoreCommandTable.TryGetValue(win, out var restoreBinding))
            {
                win.CommandBindings.Remove(restoreBinding);
                _restoreCommandTable.Remove(win);
            }
        }

        internal static void SetBlur(Window win, AcrylicAccentState state, AccentFlagsType style = AccentFlagsType.Window)
        {
            var windowHelper = new WindowInteropHelper(win);

            var value = AcrylicHelper.SelectAccentState(state);
            AcrylicHelper.SetBlur(windowHelper.Handle, style, value);
        }

        protected static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_ENTERSIZEMOVE)
            {
                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                var state = AcrylicWindow.GetAcrylicAccentState(win);
                var value = AcrylicHelper.SelectAccentState(state);
                if (value == AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND)
                {
                    AcrylicWindow.SetBlur(win, AcrylicAccentState.BlurBehind);
                    InvalidateRect(hwnd, IntPtr.Zero, true);
                }
            }
            else if (msg == WM_EXITSIZEMOVE)
            {
                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                var state = AcrylicWindow.GetAcrylicAccentState(win);
                AcrylicWindow.SetBlur(win, state);
                InvalidateRect(hwnd, IntPtr.Zero, true);
            }

            return IntPtr.Zero;
        }

        #region Dependency Property

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintColorProperty;
        public static Color GetTintColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(AcrylicElement.TintColorProperty);
        }

        public static void SetTintColor(DependencyObject obj, Color value)
        {
            obj.SetValue(AcrylicElement.TintColorProperty, value);
        }


        public double TintOpacity
        {
            get { return (double)GetValue(TintOpacityProperty); }
            set { SetValue(TintOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TintOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintOpacityProperty;
        public static double GetTintOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(AcrylicElement.TintOpacityProperty);
        }

        public static void SetTintOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(AcrylicElement.TintOpacityProperty, value);
        }



        public double NoiseOpacity
        {
            get { return (double)GetValue(NoiseOpacityProperty); }
            set { SetValue(NoiseOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoiseOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoiseOpacityProperty;
        public static double GetNoiseOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(AcrylicElement.NoiseOpacityProperty);
        }

        public static void SetNoiseOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(AcrylicElement.NoiseOpacityProperty, value);
        }


        public Color FallbackColor
        {
            get { return (Color)GetValue(FallbackColorProperty); }
            set { SetValue(FallbackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FallbackColorProperty;
        public static Color GetFallbackColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(AcrylicElement.FallbackColorProperty);
        }

        public static void SetFallbackColor(DependencyObject obj, Color value)
        {
            obj.SetValue(AcrylicElement.FallbackColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for AcrylicAccentState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicAccentStateProperty;
        public static AcrylicAccentState GetAcrylicAccentState(DependencyObject obj)
        {
            return (AcrylicAccentState)obj.GetValue(AcrylicElement.AcrylicAccentStateProperty);
        }

        public static void SetAcrylicAccentState(DependencyObject obj, AcrylicAccentState value)
        {
            obj.SetValue(AcrylicElement.AcrylicAccentStateProperty, value);
        }

        public bool ExtendViewIntoTitleBar
        {
            get { return (bool)GetValue(ExtendViewIntoTitleBarProperty); }
            set { SetValue(ExtendViewIntoTitleBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtendViewIntoTitleBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty;
        public static bool GetExtendViewIntoTitleBar(DependencyObject obj)
        {
            return (bool)obj.GetValue(AcrylicElement.ExtendViewIntoTitleBarProperty);
        }

        public static void SetExtendViewIntoTitleBar(DependencyObject obj, bool value)
        {
            obj.SetValue(AcrylicElement.ExtendViewIntoTitleBarProperty, value);
        }


        public AcrylicWindowStyle AcrylicWindowStyle
        {
            get { return (AcrylicWindowStyle)GetValue(AcrylicWindowStyleProperty); }
            set { SetValue(AcrylicWindowStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcrylicWindowStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicWindowStyleProperty;
        public static AcrylicWindowStyle GetAcrylicWindowStyle(DependencyObject obj)
        {
            return (AcrylicWindowStyle)obj.GetValue(AcrylicElement.AcrylicWindowStyleProperty);
        }

        public static void SetAcrylicWindowStyle(DependencyObject obj, AcrylicWindowStyle value)
        {
            obj.SetValue(AcrylicElement.AcrylicWindowStyleProperty, value);
        }

        public FrameworkElement TitleBar
        {
            get { return (FrameworkElement)GetValue(TitleBarProperty); }
            set { SetValue(TitleBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBarProperty;
        public static FrameworkElement GetTitleBar(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(AcrylicElement.TitleBarProperty);
        }

        public static void SetTitleBar(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(AcrylicElement.TitleBarProperty, value);
        }

        public TitleBarMode TitleBarMode
        {
            get { return (TitleBarMode)GetValue(TitleBarModeProperty); }
            set { SetValue(TitleBarModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleBarMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBarModeProperty;
        public static TitleBarMode GetTitleBarMode(DependencyObject obj)
        {
            return (TitleBarMode)obj.GetValue(AcrylicElement.TitleBarModeProperty);
        }

        public static void SetTitleBarMode(DependencyObject obj, TitleBarMode value)
        {
            obj.SetValue(AcrylicElement.TitleBarModeProperty, value);
        }

        #endregion


        #region Attached Property


        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for Enabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(AcrylicWindow), new PropertyMetadata(false, OnEnableChanged));

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as Window;
            if (win == null) { return; }

            var value = (bool)e.NewValue;
            if (value)
            {
                var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/FluentWPF;component/Styles/Window.xaml") };
                var style = dic["AcrylicWindowStyle"] as Style;
                win.Style = style;

                win.Loaded += (_, __) => { EnableBlur(win); };
                if(win.IsLoaded) EnableBlur(win);
            }
            else
            {
                win.Style = null;
                DisableBlur(win);
            }
        }
        #endregion
    }

    internal class AcrylicElement
    {


        public static Color GetTintColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(TintColorProperty);
        }

        public static void SetTintColor(DependencyObject obj, Color value)
        {
            obj.SetValue(TintColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for TintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintColorProperty =
            DependencyProperty.RegisterAttached("TintColor", typeof(Color), typeof(AcrylicElement), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.Inherits));




        public static double GetTintOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(TintOpacityProperty);
        }

        public static void SetTintOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(TintOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for TintOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintOpacityProperty =
            DependencyProperty.RegisterAttached("TintOpacity", typeof(double), typeof(AcrylicElement), new FrameworkPropertyMetadata(0.6, FrameworkPropertyMetadataOptions.Inherits));




        public static double GetNoiseOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(NoiseOpacityProperty);
        }

        public static void SetNoiseOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(NoiseOpacityProperty, value);
        }

        // Using a DependencyProperty as the backing store for NoiseOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoiseOpacityProperty =
            DependencyProperty.RegisterAttached("NoiseOpacity", typeof(double), typeof(AcrylicElement), new FrameworkPropertyMetadata(0.03, FrameworkPropertyMetadataOptions.Inherits));




        public static Color GetFallbackColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(FallbackColorProperty);
        }

        public static void SetFallbackColor(DependencyObject obj, Color value)
        {
            obj.SetValue(FallbackColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FallbackColorProperty =
            DependencyProperty.RegisterAttached("FallbackColor", typeof(Color), typeof(AcrylicElement), new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.Inherits));


        public static AcrylicAccentState GetAcrylicAccentState(DependencyObject obj)
        {
            return (AcrylicAccentState)obj.GetValue(AcrylicAccentStateProperty);
        }

        public static void SetAcrylicAccentState(DependencyObject obj, AcrylicAccentState value)
        {
            obj.SetValue(AcrylicAccentStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for AcrylicAccentState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicAccentStateProperty =
            DependencyProperty.RegisterAttached("AcrylicAccentState", typeof(AcrylicAccentState), typeof(AcrylicElement), new FrameworkPropertyMetadata(AcrylicAccentState.Default, FrameworkPropertyMetadataOptions.Inherits, OnAcrylicAccentStateChanged));

        private static void OnAcrylicAccentStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var win = d as Window;
            if (win == null) { return; }

            var isAcrylic = win is AcrylicWindow || AcrylicWindow.GetEnabled(win);

            var newValue = (AcrylicAccentState)e.NewValue;
            var oldValue = (AcrylicAccentState)e.OldValue;
            if (isAcrylic && newValue != oldValue)
            {
                AcrylicWindow.SetBlur(win, newValue);
            }
        }

        public static bool GetExtendViewIntoTitleBar(DependencyObject obj)
        {
            return (bool)obj.GetValue(ExtendViewIntoTitleBarProperty);
        }

        public static void SetExtendViewIntoTitleBar(DependencyObject obj, bool value)
        {
            obj.SetValue(ExtendViewIntoTitleBarProperty, value);
        }

        // Using a DependencyProperty as the backing store for ExtendViewIntoTitleBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtendViewIntoTitleBarProperty =
            DependencyProperty.RegisterAttached("ExtendViewIntoTitleBar", typeof(bool), typeof(AcrylicElement), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));


        public static AcrylicWindowStyle GetAcrylicWindowStyle(DependencyObject obj)
        {
            return (AcrylicWindowStyle)obj.GetValue(AcrylicWindowStyleProperty);
        }

        public static void AcrylicWindowStyleBar(DependencyObject obj, AcrylicWindowStyle value)
        {
            obj.SetValue(AcrylicWindowStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for AcrylicWindowStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicWindowStyleProperty =
            DependencyProperty.RegisterAttached("AcrylicWindowStyle", typeof(AcrylicWindowStyle), typeof(AcrylicElement), new FrameworkPropertyMetadata(AcrylicWindowStyle.Normal, FrameworkPropertyMetadataOptions.Inherits));


        public static FrameworkElement GetTitleBar(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(TitleBarProperty);
        }

        public static void SetTitleBar(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(TitleBarProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBarProperty =
            DependencyProperty.RegisterAttached("TitleBar", typeof(FrameworkElement), typeof(AcrylicElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));


        public static TitleBarMode GetTitleBarMode(DependencyObject obj)
        {
            return (TitleBarMode)obj.GetValue(TitleBarModeProperty);
        }

        public static void SetTitleBarMode(DependencyObject obj, TitleBarMode value)
        {
            obj.SetValue(TitleBarModeProperty, value);
        }

        // Using a DependencyProperty as the backing store for TitleBarMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBarModeProperty =
            DependencyProperty.RegisterAttached("TitleBarMode", typeof(TitleBarMode), typeof(AcrylicElement), new FrameworkPropertyMetadata(TitleBarMode.Default, FrameworkPropertyMetadataOptions.Inherits));
    }


    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var col = (Color)value;
                return new SolidColorBrush(col);
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
