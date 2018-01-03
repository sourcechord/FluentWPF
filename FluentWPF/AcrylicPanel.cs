using System;
using System.Collections.Generic;
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
    ///     <MyNamespace:AcrylicPanel/>
    ///
    /// </summary>
    public class AcrylicPanel : ContentControl
    {
        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(AcrylicPanel), new PropertyMetadata(null));

        public FrameworkElement Source
        {
            get { return (FrameworkElement)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(FrameworkElement), typeof(AcrylicPanel), new PropertyMetadata(null));

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TintColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintColorProperty =
            DependencyProperty.Register("TintColor", typeof(Color), typeof(AcrylicPanel), new PropertyMetadata(Colors.White));

        public double TintOpacity
        {
            get { return (double)GetValue(TintOpacityProperty); }
            set { SetValue(TintOpacityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TintOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TintOpacityProperty =
            DependencyProperty.Register("TintOpacity", typeof(double), typeof(AcrylicPanel), new PropertyMetadata(0.0));

        public double NoiseOpacity
        {
            get { return (double)GetValue(NoiseOpacityProperty); }
            set { SetValue(NoiseOpacityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for NoiseOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoiseOpacityProperty =
            DependencyProperty.Register("NoiseOpacity", typeof(double), typeof(AcrylicPanel), new PropertyMetadata(0.03));



        static AcrylicPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicPanel), new FrameworkPropertyMetadata(typeof(AcrylicPanel)));
        }


        public AcrylicPanel()
        {
            this.Source = this;
        }

        bool _isChanged = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var rect = this.GetTemplateChild("rect") as Rectangle;
            if (rect != null)
            {
                rect.LayoutUpdated += (_, __) =>
                {
                    if (!this._isChanged)
                    {
                        this._isChanged = true;
                        BindingOperations.GetBindingExpressionBase(rect, Rectangle.RenderTransformProperty)?.UpdateTarget();

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this._isChanged = false;
                        }), System.Windows.Threading.DispatcherPriority.DataBind);
                    }
                };
            }
        }
    }
}
