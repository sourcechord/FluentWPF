using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SourceChord.FluentWPF.Animations
{
    public class BrushAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(Brush);

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return Brushes.Transparent;

            // From/Toが設定されていない場合は、デフォルト値を使う
            var originValue = this.From ?? defaultOriginValue as Brush;
            var dstValue = this.To ?? defaultDestinationValue as Brush;

            if (animationClock.CurrentProgress.Value == 0)
                return originValue;
            if (animationClock.CurrentProgress.Value == 1)
                return dstValue;

            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = originValue,
                Child = new Border()
                {
                    Background = dstValue,
                    Opacity = animationClock.CurrentProgress.Value,
                }
            });
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }

        //we must define From and To, AnimationTimeline does not have this properties
        public Brush From
        {
            get { return (Brush)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        public Brush To
        {
            get { return (Brush)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));
    }
}
