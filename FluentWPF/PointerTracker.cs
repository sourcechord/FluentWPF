using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public static class PointerTracker
    {

        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }
        private static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }
        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("X", typeof(double), typeof(PointerTracker), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));


        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }
        private static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }
        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached("Y", typeof(double), typeof(PointerTracker), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));


        public static Point GetPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(PositionProperty);
        }
        private static void SetPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(PositionProperty, value);
        }
        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(PointerTracker), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.Inherits));


        public static bool GetIsEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnterProperty);
        }
        private static void SetIsEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnterProperty, value);
        }
        // Using a DependencyProperty as the backing store for IsEnter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnterProperty =
            DependencyProperty.RegisterAttached("IsEnter", typeof(bool), typeof(PointerTracker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));



        public static UIElement GetRootObject(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(RootObjectProperty);
        }
        private static void SetRootObject(DependencyObject obj, UIElement value)
        {
            obj.SetValue(RootObjectProperty, value);
        }
        // Using a DependencyProperty as the backing store for RootObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RootObjectProperty =
            DependencyProperty.RegisterAttached("RootObject", typeof(UIElement), typeof(PointerTracker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));




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
            DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(PointerTracker), new PropertyMetadata(false, OnEnabledChanged));

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as UIElement;
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;
            if (ctrl == null) return;

            // 無効になった場合の処理
            if (oldValue && !newValue)
            {
                ctrl.MouseEnter -= Ctrl_MouseEnter;
                ctrl.PreviewMouseMove -= Ctrl_PreviewMouseMove;
                ctrl.MouseLeave -= Ctrl_MouseLeave;

                ctrl.ClearValue(PointerTracker.RootObjectProperty);
            }


            // 有効になった場合の処理
            if (!oldValue && newValue)
            {
                ctrl.MouseEnter += Ctrl_MouseEnter;
                ctrl.PreviewMouseMove += Ctrl_PreviewMouseMove;
                ctrl.MouseLeave += Ctrl_MouseLeave;

                SetRootObject(ctrl, ctrl);
            }
        }

        private static void Ctrl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ctrl = sender as UIElement;
            if (ctrl != null)
            {
                SetIsEnter(ctrl, true);
            }
        }

        private static void Ctrl_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ctrl = sender as UIElement;
            if (ctrl != null && GetIsEnter(ctrl))
            {
                var pos = e.GetPosition(ctrl);

                SetX(ctrl, pos.X);
                SetY(ctrl, pos.Y);
                SetPosition(ctrl, pos);
            }
        }

        private static void Ctrl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ctrl = sender as UIElement;
            if (ctrl != null)
            {
                SetIsEnter(ctrl, false);
            }
        }
    }

    public class RelativePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(o => o == DependencyProperty.UnsetValue || o == null)) return new Point(0, 0);

            var parent = values[0] as UIElement;
            var ctrl = values[1] as UIElement;
            var pointerPos = (Point)values[2];
            var relativePos = parent.TranslatePoint(pointerPos, ctrl);

            return relativePos;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isEnter = (bool)value;
            var opacity = (double)parameter;
            return isEnter ? opacity : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RevealBrushExtension : MarkupExtension
    {
        public RevealBrushExtension()
        {

        }

        public Color Color { get; set; } = Colors.Black;
        public double Opacity { get; set; } = 1;

        public double Size { get; set; } = 100;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = pvt.TargetObject as DependencyObject;

            // 円形のグラデーション表示をするブラシを作成
            var bgColor = Color.FromArgb(0, this.Color.R, this.Color.G, this.Color.B);
            var brush = new RadialGradientBrush(this.Color, bgColor);
            brush.MappingMode = BrushMappingMode.Absolute;
            brush.RadiusX = this.Size;
            brush.RadiusY = this.Size;

            // カーソルが領域外にある場合は、透明にする。
            var opacityBinding = new Binding("Opacity")
            {
                Source = target,
                Path = new PropertyPath(PointerTracker.IsEnterProperty),
                Converter = new OpacityConverter(),
                ConverterParameter = this.Opacity
            };
            BindingOperations.SetBinding(brush, RadialGradientBrush.OpacityProperty, opacityBinding);

            // グラデーションの中心位置をバインディング
            var binding = new MultiBinding();
            binding.Converter = new RelativePositionConverter();
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(PointerTracker.RootObjectProperty) });
            binding.Bindings.Add(new Binding() { Source = target });
            binding.Bindings.Add(new Binding() { Source = target, Path = new PropertyPath(PointerTracker.PositionProperty) });

            BindingOperations.SetBinding(brush, RadialGradientBrush.CenterProperty, binding);
            BindingOperations.SetBinding(brush, RadialGradientBrush.GradientOriginProperty, binding);
            return brush;
        }
    }

    public class RevealElement
    {

        public static Brush GetMouseOverForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverForegroundProperty);
        }
        public static void SetMouseOverForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverForegroundProperty, value);
        }
        // Using a DependencyProperty as the backing store for MouseOverForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(RevealElement), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));


        public static Brush GetMouseOverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBackgroundProperty);
        }
        public static void SetMouseOverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBackgroundProperty, value);
        }
        // Using a DependencyProperty as the backing store for MouseOverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(RevealElement), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));




        public static double GetBorderRadius(DependencyObject obj)
        {
            return (double)obj.GetValue(BorderRadiusProperty);
        }
        public static void SetBorderRadius(DependencyObject obj, double value)
        {
            obj.SetValue(BorderRadiusProperty, value);
        }
        // Using a DependencyProperty as the backing store for BorderRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.RegisterAttached("BorderRadius", typeof(double), typeof(RevealElement), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits));



        public static double GetBorderOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(BorderOpacityProperty);
        }
        public static void SetBorderOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(BorderOpacityProperty, value);
        }
        // Using a DependencyProperty as the backing store for BorderOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderOpacityProperty =
            DependencyProperty.RegisterAttached("BorderOpacity", typeof(double), typeof(RevealElement), new PropertyMetadata(0.0));


        public static double GetMouseOverBorderOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(MouseOverBorderOpacityProperty);
        }
        public static void SetMouseOverBorderOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(MouseOverBorderOpacityProperty, value);
        }
        // Using a DependencyProperty as the backing store for MouseOverBorderOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderOpacityProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderOpacity", typeof(double), typeof(RevealElement), new PropertyMetadata(0.5));


        public static double GetPressBorderOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(PressBorderOpacityProperty);
        }
        public static void SetPressBorderOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(PressBorderOpacityProperty, value);
        }
        // Using a DependencyProperty as the backing store for PressBorderOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressBorderOpacityProperty =
            DependencyProperty.RegisterAttached("PressBorderOpacity", typeof(double), typeof(RevealElement), new PropertyMetadata(0.5));


        public static Brush GetPressTintBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressTintBrushProperty);
        }
        public static void SetPressTintBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressTintBrushProperty, value);
        }
        // Using a DependencyProperty as the backing store for PressTintBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressTintBrushProperty =
            DependencyProperty.RegisterAttached("PressTintBrush", typeof(Brush), typeof(RevealElement), new PropertyMetadata(Brushes.Gray));


    }
}
