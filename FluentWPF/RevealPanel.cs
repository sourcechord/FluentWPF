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
    ///     <MyNamespace:RevealPanel/>
    ///
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public class RevealPanel : ContentControl
    {
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            private set { SetValue(IsPressedProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(RevealPanel), new PropertyMetadata(false));


        static RevealPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RevealPanel), new FrameworkPropertyMetadata(typeof(RevealPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateStates(false);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            this.CaptureMouse();
            this.UpdateStates(true);
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
                this.IsPressed = false;
            }
            this.UpdateStates(true);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.IsMouseCaptured && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                var pos = Mouse.PrimaryDevice.GetPosition(this);
                if (0 <= pos.X && pos.X <= this.ActualWidth &&
                    0 <= pos.Y && pos.Y <= this.ActualHeight)
                {
                    this.IsPressed = true;
                }
                else
                {
                    this.IsPressed = false;
                }
            }

            this.UpdateStates(true);
        }


        private void UpdateStates(bool useTransitions)
        {
            if (!this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
            }
            else if (this.IsPressed)
            {
                VisualStateManager.GoToState(this, "Pressed", useTransitions);
            }
            else if (this.IsMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
            }
        }
    }
}
