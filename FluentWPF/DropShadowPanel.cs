using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace SourceChord.FluentWPF
{
    public class DropShadowPanel : Decorator
    {

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for BlurRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(DropShadowPanel), new PropertyMetadata(20.0));


        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DropShadowPanel), new PropertyMetadata(Colors.Black));


        public double Direction
        {
            get { return (double)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(double), typeof(DropShadowPanel), new PropertyMetadata(315.0));

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ShadowOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(DropShadowPanel), new PropertyMetadata(1.0));


        public RenderingBias RenderingBias
        {
            get { return (RenderingBias)GetValue(RenderingBiasProperty); }
            set { SetValue(RenderingBiasProperty, value); }
        }
        // Using a DependencyProperty as the backing store for RenderingBias.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderingBiasProperty =
            DependencyProperty.Register("RenderingBias", typeof(RenderingBias), typeof(DropShadowPanel), new PropertyMetadata(RenderingBias.Performance));


        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ShadowDepth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register("ShadowDepth", typeof(double), typeof(DropShadowPanel), new PropertyMetadata(0.0));


        private ContainerVisual _internalVisual;

        public DropShadowPanel()
        {

        }

        private ContainerVisual InternalVisual
        {
            get
            {
                if (this._internalVisual == null)
                {
                    this._internalVisual = new ContainerVisual();
                    AddVisualChild(this._internalVisual);
                }
                return this._internalVisual;
            }
        }

        private UIElement InternalChild
        {
            get
            {
                var children = this.InternalVisual.Children;
                if (children.Count != 0) return children[0] as UIElement;
                else return null;
            }
            set
            {
                var children = this.InternalVisual.Children;
                if (children.Count != 0) children.Clear();
                children.Add(value);
            }
        }

        public override UIElement Child
        {
            get { return this.InternalChild; }
            set
            {
                var old = this.InternalChild;

                if (old != value)
                {
                    // 古い要素をLogicalTreeから取り除く
                    RemoveLogicalChild(old);

                    var ic = this.CreateInternalVisual(value);

                    if (value != null)
                    {
                        AddLogicalChild(ic);
                    }

                    this.InternalChild = ic;

                    InvalidateMeasure();
                }
            }
        }

        protected internal UIElement CreateInternalVisual(UIElement value)
        {
            var effect = new System.Windows.Media.Effects.DropShadowEffect();
            BindingOperations.SetBinding(effect, DropShadowEffect.BlurRadiusProperty, new Binding("BlurRadius") { Source = this });
            BindingOperations.SetBinding(effect, DropShadowEffect.ColorProperty, new Binding("Color") { Source = this });
            BindingOperations.SetBinding(effect, DropShadowEffect.DirectionProperty, new Binding("Direction") { Source = this });
            BindingOperations.SetBinding(effect, DropShadowEffect.OpacityProperty, new Binding("ShadowOpacity") { Source = this });
            BindingOperations.SetBinding(effect, DropShadowEffect.RenderingBiasProperty, new Binding("RenderingBias") { Source = this });
            BindingOperations.SetBinding(effect, DropShadowEffect.ShadowDepthProperty, new Binding("ShadowDepth") { Source = this });
            var border = new Rectangle()
            {
                Fill = new VisualBrush(value)
                {
                    TileMode = TileMode.None,
                    Stretch = Stretch.None
                },
                Effect = effect
            };

            var grid = new Grid();
            grid.Children.Add(border);
            grid.Children.Add(value);

            return grid;
        }

        // 常に1を返しておく
        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return this.InternalVisual;
        }
    }
}
