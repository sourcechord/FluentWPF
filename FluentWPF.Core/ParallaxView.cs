using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SourceChord.FluentWPF
{
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
    ///     <MyNamespace:ParallaxView/>
    ///
    /// </summary>
    public class ParallaxView : ContentControl
    {
        static ParallaxView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ParallaxView), new FrameworkPropertyMetadata(typeof(ParallaxView)));
        }



        public double VerticalShift
        {
            get { return (double)GetValue(VerticalShiftProperty); }
            set { SetValue(VerticalShiftProperty, value); }
        }
        // Using a DependencyProperty as the backing store for VerticalShift.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalShiftProperty =
            DependencyProperty.Register("VerticalShift", typeof(double), typeof(ParallaxView), new PropertyMetadata(0.0));



        public double HorizontalShift
        {
            get { return (double)GetValue(HorizontalShiftProperty); }
            set { SetValue(HorizontalShiftProperty, value); }
        }
        // Using a DependencyProperty as the backing store for HorizontalShift.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalShiftProperty =
            DependencyProperty.Register("HorizontalShift", typeof(double), typeof(ParallaxView), new PropertyMetadata(0.0));


        public Thickness OffsetMargin
        {
            get { return (Thickness)GetValue(OffsetMarginProperty); }
            private set { SetValue(OffsetMarginProperty, value); }
        }
        // Using a DependencyProperty as the backing store for OffsetMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetMarginProperty =
            DependencyProperty.Register("OffsetMargin", typeof(Thickness), typeof(ParallaxView), new PropertyMetadata(new Thickness(0)));


        private void OnScrollUpdated(ScrollViewer scrollViewer)
        {
            var posX = scrollViewer.ScrollableWidth == 0 ? 0 : scrollViewer.HorizontalOffset / scrollViewer.ScrollableWidth;
            var posY = scrollViewer.ScrollableHeight == 0 ? 0 : scrollViewer.VerticalOffset / scrollViewer.ScrollableHeight;

            this.OffsetMargin = new Thickness(-posX * HorizontalShift, -posY * VerticalShift, 0, 0);
        }

        public UIElement Source
        {
            get { return (UIElement)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(UIElement), typeof(ParallaxView), new PropertyMetadata(null, OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Sourceが設定されたら、VisualTreeを辿りScrollViewerを探す。
            // ⇒見つかったScrollViewerの各種プロパティと、このParallaxViewのオフセット値をバインディングする。
            var parallax = d as ParallaxView;
            var ctrl = e.NewValue as FrameworkElement;
            ctrl.Loaded += (_, __) =>
            {
                var viewer = GetScrollViewer(ctrl);

                if (viewer != null)
                {
                    viewer.ScrollChanged += (sender, ___) => { parallax.OnScrollUpdated(sender as ScrollViewer); };
                    viewer.SizeChanged += (sender, ___) => { parallax.OnScrollUpdated(sender as ScrollViewer); };
                }
            };

        }


        private static ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            var viewer = obj as ScrollViewer ?? FindVisualChild<ScrollViewer>(obj);
            return viewer;
        }

        private static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }

    public class AddValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double sum = 0;
            foreach(var  v in values)
            {
                var isInvalid = v == DependencyProperty.UnsetValue;
                if (isInvalid) continue;

                var value = (double)v;
                sum += value;
            }
            return sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
