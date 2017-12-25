using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SourceChord.FluentWPF
{
    public class AcrylicBrushExtension : MarkupExtension
    {
        public string TargetName { get; set; }

        public Color TintColor { get; set; } = Colors.White;

        public double TintOpacity { get; set; } = 0.0;

        public double NoiseOpacity { get; set; } = 0.03;


        public AcrylicBrushExtension()
        {

        }

        public AcrylicBrushExtension(string target)
        {
            this.TargetName = target;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = pvt.TargetObject as FrameworkElement;

            var visualBrush = new VisualBrush()
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                ViewboxUnits = BrushMappingMode.Absolute,
            };
            var visualBinding = new Binding() { ElementName = this.TargetName };
            var transformBinding = new MultiBinding();
            transformBinding.Converter = new BrushTranslationConverter();
            transformBinding.Bindings.Add(new Binding() { ElementName = this.TargetName });
            transformBinding.Bindings.Add(new Binding() { Source = target });
            BindingOperations.SetBinding(visualBrush, VisualBrush.VisualProperty, visualBinding);


            var rect = new Rectangle()
            {
                Fill = visualBrush,
                Effect = new BlurEffect() { Radius = 100 }
            };

            var widthBinding = new Binding()
            {
                ElementName = this.TargetName,
                Path = new PropertyPath("ActualWidth")
            };
            BindingOperations.SetBinding(rect, Rectangle.WidthProperty, widthBinding);
            var heightBinding = new Binding()
            {
                ElementName = this.TargetName,
                Path = new PropertyPath("ActualHeight")
            };
            BindingOperations.SetBinding(rect, Rectangle.HeightProperty, heightBinding);

            // ぼかしレイヤーを、BitmapCacheを設定したContentControlに配置
            var visual = new ContentControl()
            {
                Content = rect,
                CacheMode = new BitmapCache(0.2)
            };

            var grid = new Grid();

            var background = new Rectangle();
            var bgBinding = new PriorityBinding();
            bgBinding.Bindings.Add(new Binding("Background") { ElementName = this.TargetName });
            bgBinding.Bindings.Add(new Binding("Fill") { ElementName = this.TargetName });
            BindingOperations.SetBinding(background, Rectangle.FillProperty, bgBinding);
            grid.Children.Add(background);

            grid.Children.Add(visual);

            var tintLayer = new Rectangle()
            {
                Fill = new SolidColorBrush(this.TintColor),
                Opacity = this.TintOpacity
            };
            grid.Children.Add(tintLayer);

            var imgNoise = new BitmapImage(new Uri(@"pack://application:,,,/FluentWPF;component/Assets/Images/noise.png"));
            var noiseLayer = new Rectangle()
            {
                Fill = new ImageBrush(imgNoise)
                {
                    TileMode = TileMode.Tile,
                    Stretch = Stretch.None,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Viewport = new Rect(0, 0, 128, 128),
                    Opacity = this.NoiseOpacity
                }
            };
            grid.Children.Add(noiseLayer);

            BindingOperations.SetBinding(grid, Grid.RenderTransformProperty, transformBinding);

            target.LayoutUpdated += (_, __) =>
            {
                BindingOperations.GetBindingExpressionBase(grid, Grid.RenderTransformProperty)?.UpdateTarget();
            };

            var brush = new VisualBrush(grid)
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                ViewboxUnits = BrushMappingMode.Absolute,
            };

            return brush;
        }
    }

    public class BrushTranslationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(o => o == DependencyProperty.UnsetValue || o == null)) return new Point(0, 0);

            var parent = values[0] as UIElement;
            var ctrl = values[1] as UIElement;
            //var pointerPos = (Point)values[2];
            var relativePos = parent.TranslatePoint(new Point(0, 0), ctrl);

            return new TranslateTransform(relativePos.X, relativePos.Y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
