using SourceChord.FluentWPF.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        internal static void EnableBlur(Window win)
        {
            var windowHelper = new WindowInteropHelper(win);

            // ウィンドウに半透明のアクリル効果を適用する
            AcrylicHelper.EnableBlur(windowHelper.Handle);

            // タイトルバーの各種ボタンで利用するコマンドの設定
            win.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => { SystemCommands.CloseWindow(win); }));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => { SystemCommands.MinimizeWindow(win); }));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => { SystemCommands.MaximizeWindow(win); }));
            win.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => { SystemCommands.RestoreWindow(win); }));


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
            DependencyProperty.RegisterAttached("TintOpacity", typeof(double), typeof(AcrylicElement), new PropertyMetadata(0.6));




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
            DependencyProperty.RegisterAttached("NoiseOpacity", typeof(double), typeof(AcrylicElement), new PropertyMetadata(0.03));




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
            DependencyProperty.RegisterAttached("ExtendViewIntoTitleBar", typeof(bool), typeof(AcrylicElement), new PropertyMetadata(false));


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
            DependencyProperty.RegisterAttached("AcrylicWindowStyle", typeof(AcrylicWindowStyle), typeof(AcrylicElement), new PropertyMetadata(AcrylicWindowStyle.Normal));


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
            DependencyProperty.RegisterAttached("TitleBar", typeof(FrameworkElement), typeof(AcrylicElement), new PropertyMetadata(null));


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
            DependencyProperty.RegisterAttached("TitleBarMode", typeof(TitleBarMode), typeof(AcrylicElement), new PropertyMetadata(TitleBarMode.Default));
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
