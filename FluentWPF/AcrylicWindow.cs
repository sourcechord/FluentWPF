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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetCursorPos(out POINT lpPoint);

        private static readonly int WM_SIZE = 0x0005;
        private static readonly int WM_WINDOWPOSCHANGING = 0x0046;
        private static readonly int WM_SYSCOMMAND = 0x112;

        private static readonly int WM_ENTERSIZEMOVE = 0x0231;
        private static readonly int WM_EXITSIZEMOVE = 0x0232;
        private static readonly int WM_GETMINMAXINFO = 0x0024;
        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("shcore.dll")]
        public static extern int GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        public enum MONITOR_DPI_TYPE : int
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI,
            MDT_RAW_DPI,
            MDT_DEFAULT
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        private static readonly int MONITOR_DEFAULTTOPRIMARY = 1;
        private static readonly int MONITOR_DEFAULTTONEAREST = 2;

        private enum SysCommands : int
        {
            SC_RESTORE = 0xF120,
        }

        private enum SizeEvents : int
        {
            SIZE_RESTORED = 0,
            SIZE_MAXIMIZED = 2,
        }

        private enum WindowRestoringState
        {
            Default,    // Restored
            Restoring,
            RestoreMove,
        }

        private class AcrylicWindowInternalState
        {
            public WindowRestoringState RestoringState { get; set; }
            public CommandBinding CloseCommand { get; set; }
            public CommandBinding MinimizeCommand { get; set; }
            public CommandBinding MaximizeCommand { get; set; }
            public CommandBinding RestoreCommand { get; set; }
        }

        private static ConditionalWeakTable<Window, AcrylicWindowInternalState> _internalStateTable = new ConditionalWeakTable<Window, AcrylicWindowInternalState>();

        internal static void EnableBlur(Window win)
        {
            // ウィンドウに半透明のアクリル効果を適用する
            var state = AcrylicWindow.GetAcrylicAccentState(win);
            SetBlur(win, state);

            if (!_internalStateTable.TryGetValue(win, out var _))
            {
                var windowHelper = new WindowInteropHelper(win);
                var source = HwndSource.FromHwnd(windowHelper.Handle);
                source.AddHook(WndProc);

                // タイトルバーの各種ボタンで利用するコマンドの設定
                var closeBinding = new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => { SystemCommands.CloseWindow(win); });
                var minimizeBinding = new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => { SystemCommands.MinimizeWindow(win); });
                var maximizeBinding = new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => { SystemCommands.MaximizeWindow(win); });
                var restoreBinding = new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => { SystemCommands.RestoreWindow(win); });
                win.CommandBindings.Add(closeBinding);
                win.CommandBindings.Add(minimizeBinding);
                win.CommandBindings.Add(maximizeBinding);
                win.CommandBindings.Add(restoreBinding);

                var internalState = new AcrylicWindowInternalState()
                {
                    RestoringState = WindowRestoringState.Default,
                    CloseCommand = closeBinding,
                    MinimizeCommand = minimizeBinding,
                    MaximizeCommand = maximizeBinding,
                    RestoreCommand = restoreBinding,
                };
                _internalStateTable.Add(win, internalState);


                // フルスクリーン状態だったら、ウィンドウサイズの訂正をする
                if (win.WindowState == WindowState.Maximized)
                {
                    FixMaximizedWindowSize(windowHelper.Handle);
                }

                // WPFのSizeToContentのバグ対策
                // (WindowChrome使用時に、SizeToContentのウィンドウサイズ計算が正しく行われない)
                void onContentRendered(object sender, EventArgs e)
                {
                    if (win.SizeToContent != SizeToContent.Manual)
                    {
                        win.InvalidateMeasure();
                    }

                    win.ContentRendered -= onContentRendered;
                    InvalidateRect(windowHelper.Handle, IntPtr.Zero, true);
                }
                win.ContentRendered += onContentRendered;
                InvalidateRect(windowHelper.Handle, IntPtr.Zero, true);
            }
        }

        internal static void DisableBlur(Window win)
        {
            // アクリル効果を解除する
            SetBlur(win, AcrylicAccentState.Disabled);

            
            if (_internalStateTable.TryGetValue(win, out var internalState))
            {
                var windowHelper = new WindowInteropHelper(win);
                var source = HwndSource.FromHwnd(windowHelper.Handle);
                source.RemoveHook(WndProc);

                // コマンド解除
                win.CommandBindings.Remove(internalState.CloseCommand);
                win.CommandBindings.Remove(internalState.MinimizeCommand);
                win.CommandBindings.Remove(internalState.MaximizeCommand);
                win.CommandBindings.Remove(internalState.RestoreCommand);

                _internalStateTable.Remove(win);
            }
        }

        internal static void SetBlur(Window win, AcrylicAccentState state, AccentFlagsType style = AccentFlagsType.Window)
        {
            var windowHelper = new WindowInteropHelper(win);

            var value = AcrylicHelper.SelectAccentState(state);
            AcrylicHelper.SetBlur(windowHelper.Handle, style, value);
        }

        internal static void FixMaximizedWindowSize(IntPtr hwnd)
        {
            var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
            if (win != null && _internalStateTable.TryGetValue(win, out var internalState))
            {
                internalState.RestoringState = WindowRestoringState.Default;

                win.WindowStyle = WindowStyle.None;

                var hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                var monitorInfo = new MONITORINFO
                {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFO))
                };

                GetMonitorInfo(hMonitor, ref monitorInfo);
                var workingRectangle = monitorInfo.rcWork;
                var monitorRectangle = monitorInfo.rcMonitor;

                var x = workingRectangle.left;
                var y = workingRectangle.top;
                var width = Math.Abs(workingRectangle.right - workingRectangle.left);
                var height = Math.Abs(workingRectangle.bottom - workingRectangle.top) - 1;
                MoveWindow(hwnd, x, y, width, height, true);
            }
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

                if (win != null && _internalStateTable.TryGetValue(win, out var internalState))
                {
                    internalState.RestoringState = WindowRestoringState.Default;
                }
            }
            else if (msg == WM_GETMINMAXINFO)
            {
                handled = true;

                var hMonitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                var monitorInfo = new MONITORINFO
                {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFO))
                };

                GetMonitorInfo(hMonitor, ref monitorInfo);
                var info = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                var workingRectangle = monitorInfo.rcWork;
                var monitorRectangle = monitorInfo.rcMonitor;

                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                if (win != null)
                {
                    GetDpiForMonitor(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);

                    var maxWidth = win.MaxWidth / 96.0 * dpiX;
                    var maxHeight = win.MaxHeight / 96.0 * dpiY;
                    var minWidth = win.MinWidth / 96.0 * dpiX;
                    var minHeight = win.MinHeight / 96.0 * dpiY;

                    // ウィンドウ最大化時の位置とサイズを指定
                    info.ptMaxPosition.x = Math.Abs(workingRectangle.left - monitorRectangle.left);
                    info.ptMaxPosition.y = Math.Abs(workingRectangle.top - monitorRectangle.top);
                    info.ptMaxSize.x = (int)Math.Min(Math.Abs(workingRectangle.right - monitorRectangle.left), maxWidth);
                    info.ptMaxSize.y = (int)Math.Min(Math.Abs(workingRectangle.bottom - monitorRectangle.top), maxHeight);

                    // ウィンドウのリサイズ可能サイズを設定
                    info.ptMaxTrackSize.x = (int)Math.Min(info.ptMaxTrackSize.x, maxWidth);
                    info.ptMaxTrackSize.y = (int)Math.Min(info.ptMaxTrackSize.y, maxHeight);
                    info.ptMinTrackSize.x = (int)Math.Max(info.ptMinTrackSize.x, minWidth);
                    info.ptMinTrackSize.y = (int)Math.Max(info.ptMinTrackSize.y, minHeight);

                    Marshal.StructureToPtr(info, lParam, true);
                } 
                return IntPtr.Zero;
            }
            else if (msg == WM_SIZE && wParam == (IntPtr)SizeEvents.SIZE_MAXIMIZED)
            {
                FixMaximizedWindowSize(hwnd);
            }
            else if (msg == WM_SIZE && wParam == (IntPtr)SizeEvents.SIZE_RESTORED)
            {
                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                if (win != null && _internalStateTable.TryGetValue(win, out var internalState))
                {
                    win.ClearValue(WindowStyleProperty);
                    if (win.WindowState == WindowState.Maximized &&
                        internalState.RestoringState == WindowRestoringState.Default)
                    {
                        internalState.RestoringState = WindowRestoringState.RestoreMove;
                    }
                    if (internalState.RestoringState == WindowRestoringState.Restoring)
                    {
                        internalState.RestoringState = WindowRestoringState.Default;
                    }
                }
            }
            else if (msg == WM_SYSCOMMAND && wParam == (IntPtr)SysCommands.SC_RESTORE)
            {
                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                if (win != null && _internalStateTable.TryGetValue(win, out var internalState))
                {
                    internalState.RestoringState = WindowRestoringState.Restoring;
                }
            }
            else if (msg == WM_WINDOWPOSCHANGING)
            {
                var win = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                if (win != null && _internalStateTable.TryGetValue(win, out var internalState))
                {
                    var pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    if (internalState.RestoringState == WindowRestoringState.RestoreMove &&
                        !(pos.x == 0 && pos.y == 0 && pos.cx == 0 && pos.cy == 0))
                    {
                        GetCursorPos(out var cur);
                        pos.y = cur.y - 8;
                        Marshal.StructureToPtr(pos, lParam, true);
                    }
                }
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
                win.SetResourceReference(FrameworkElement.StyleProperty, "AcrylicWindowStyle");

                win.Loaded += (_, __) =>
                {
                    EnableBlur(win);
                    var windowHelper = new WindowInteropHelper(win);
                    InvalidateRect(windowHelper.Handle, IntPtr.Zero, true);
                };
                if (win.IsLoaded)
                {
                    EnableBlur(win);
                    var windowHelper = new WindowInteropHelper(win);
                    InvalidateRect(windowHelper.Handle, IntPtr.Zero, true);
                }
            }
            else
            {
                win.Style = null;
                win.ClearValue(FrameworkElement.StyleProperty);
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
