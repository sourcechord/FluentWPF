using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    ///     <MyNamespace:AcrylicMessageBox/>
    ///
    /// </summary>
    [TemplatePart(Name = PART_ButtonsArea, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_OkButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_YesButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NoButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CancelButton, Type = typeof(Button))]
    public class AcrylicMessageBox : AcrylicWindow
    {
        protected const string PART_ButtonsArea = "PART_ButtonsArea";
        protected const string PART_OkButton = "PART_OkButton";
        protected const string PART_YesButton = "PART_YesButton";
        protected const string PART_NoButton = "PART_NoButton";
        protected const string PART_CancelButton = "PART_CancelButton";

        private FrameworkElement _buttonsArea;
        private ButtonBase _okButton;
        private ButtonBase _yesButton;
        private ButtonBase _noButton;
        private ButtonBase _cancelButton;

        private ButtonBase _closeButton;

        static AcrylicMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicMessageBox), new FrameworkPropertyMetadata(typeof(AcrylicMessageBox)));
        }

        #region メッセージボックス表示用のstaticメソッド
        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return AcrylicMessageBox.Show(owner, messageBoxText, string.Empty);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return AcrylicMessageBox.Show(owner, messageBoxText, string.Empty, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
        {
            var msg = new AcrylicMessageBox()
            {
                Owner = Window.GetWindow(owner),
                Title = caption,
                Message = messageBoxText,
                ButtonType = button,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            msg.ShowDialog();

            return msg.Result;
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// メッセージボックス内に表示するテキストを取得または設定します。
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(AcrylicMessageBox), new PropertyMetadata(string.Empty));


        /// <summary>
        /// メッセージボックスに表示するボタンのタイプを取得または設定します。
        /// </summary>
        public MessageBoxButton ButtonType
        {
            get { return (MessageBoxButton)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ButtonType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(MessageBoxButton), typeof(AcrylicMessageBox), new PropertyMetadata(MessageBoxButton.OK, OnButtonTypeChanged));

        private static void OnButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var win = d as AcrylicMessageBox;
            if (win != null)
            {
                win.UpdateStates();
            }
        }

        /// <summary>
        /// メッセージボックスの結果を取得します。
        /// </summary>
        public MessageBoxResult Result
        {
            get { return (MessageBoxResult)GetValue(ResultProperty); }
            protected set { SetValue(ResultProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(MessageBoxResult), typeof(AcrylicMessageBox), new PropertyMetadata(MessageBoxResult.None));

        /// <summary>
        /// OKボタンに表示する文字列を取得または設定します。
        /// </summary>
        public object OkButtonContent
        {
            get { return (object)GetValue(OkButtonContentProperty); }
            set { SetValue(OkButtonContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for OkButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OkButtonContentProperty =
            DependencyProperty.Register("OkButtonContent", typeof(object), typeof(AcrylicMessageBox), new PropertyMetadata("OK"));

        /// <summary>
        /// Yesボタンに表示する文字列を取得または設定します。
        /// </summary>
        public object YesButtonContent
        {
            get { return (object)GetValue(YesButtonContentProperty); }
            set { SetValue(YesButtonContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for YesButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesButtonContentProperty =
            DependencyProperty.Register("YesButtonContent", typeof(object), typeof(AcrylicMessageBox), new PropertyMetadata("Yes"));

        /// <summary>
        /// Noボタンに表示する文字列を取得または設定します。
        /// </summary>
        public object NoButtonContent
        {
            get { return (object)GetValue(NoButtonContentProperty); }
            set { SetValue(NoButtonContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for NoButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoButtonContentProperty =
            DependencyProperty.Register("NoButtonContent", typeof(object), typeof(AcrylicMessageBox), new PropertyMetadata("No"));

        /// <summary>
        /// キャンセルボタンに表示する文字列を取得または設定します。
        /// </summary>
        public object CancelButtonContent
        {
            get { return (object)GetValue(CancelButtonContentProperty); }
            set { SetValue(CancelButtonContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CancelButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonContentProperty =
            DependencyProperty.Register("CancelButtonContent", typeof(object), typeof(AcrylicMessageBox), new PropertyMetadata("Cancel"));

        #endregion


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // ウィンドウの「閉じる」ボタン要素を取得
            this._closeButton = GetTemplateChild("btnCloseButton") as ButtonBase;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            // ボタンの押下イベントを登録
            var content = newContent as FrameworkElement;
            if (content == null) { return; }
            this._buttonsArea = LogicalTreeHelper.FindLogicalNode(content, PART_ButtonsArea) as FrameworkElement;
            if (this._buttonsArea != null)
            {
                this._buttonsArea.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(this.OnButtonClicked));

                // 各種ボタン要素を取得
                this._okButton = LogicalTreeHelper.FindLogicalNode(content, PART_OkButton) as ButtonBase;
                this._yesButton = LogicalTreeHelper.FindLogicalNode(content, PART_YesButton) as ButtonBase;
                this._noButton = LogicalTreeHelper.FindLogicalNode(content, PART_NoButton) as ButtonBase;
                this._cancelButton = LogicalTreeHelper.FindLogicalNode(content, PART_CancelButton) as ButtonBase;

                this.UpdateStates();
            }
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as ButtonBase;
            if (button == null)
                return;

            switch (button.Name)
            {
                case PART_OkButton:
                    this.Result = MessageBoxResult.OK;
                    break;
                case PART_YesButton:
                    this.Result = MessageBoxResult.Yes;
                    break;
                case PART_NoButton:
                    this.Result = MessageBoxResult.No;
                    break;
                case PART_CancelButton:
                    this.Result = MessageBoxResult.Cancel;
                    break;
                default:
                    this.Result = MessageBoxResult.None;
                    break;
            }

            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (this.Result == MessageBoxResult.None)
            {
                // メッセージボックスの「閉じる」ボタンが押された場合の処理
                switch (this.ButtonType)
                {
                    case MessageBoxButton.OK:
                        this.Result = MessageBoxResult.OK;
                        break;
                    case MessageBoxButton.OKCancel:
                        this.Result = MessageBoxResult.Cancel;
                        break;
                    case MessageBoxButton.YesNo:
                        // YesNoの動作の時は、必ず「Yes」「No」どちらかのボタンを押して選択されなければならない。
                        this.Result = MessageBoxResult.None;
                        throw new Exception();
                    case MessageBoxButton.YesNoCancel:
                        this.Result = MessageBoxResult.Cancel;
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// このコントロールのVisualStateを更新します
        /// </summary>
        private void UpdateStates()
        {
            if (this._okButton == null || this._yesButton == null ||
                this._noButton == null || this._cancelButton == null)
            {
                return;
            }

            switch (this.ButtonType)
            {
                case MessageBoxButton.OK:
                    this._okButton.Visibility = Visibility.Visible;
                    this._yesButton.Visibility = Visibility.Collapsed;
                    this._noButton.Visibility = Visibility.Collapsed;
                    this._cancelButton.Visibility = Visibility.Collapsed;
                    this._closeButton.IsEnabled = true;
                    break;
                case MessageBoxButton.OKCancel:
                    this._okButton.Visibility = Visibility.Visible;
                    this._yesButton.Visibility = Visibility.Collapsed;
                    this._noButton.Visibility = Visibility.Collapsed;
                    this._cancelButton.Visibility = Visibility.Visible;
                    this._closeButton.IsEnabled = true;
                    break;
                case MessageBoxButton.YesNo:
                    this._okButton.Visibility = Visibility.Collapsed;
                    this._yesButton.Visibility = Visibility.Visible;
                    this._noButton.Visibility = Visibility.Visible;
                    this._cancelButton.Visibility = Visibility.Collapsed;
                    this._closeButton.IsEnabled = false;
                    break;
                case MessageBoxButton.YesNoCancel:
                    this._okButton.Visibility = Visibility.Collapsed;
                    this._yesButton.Visibility = Visibility.Visible;
                    this._noButton.Visibility = Visibility.Visible;
                    this._cancelButton.Visibility = Visibility.Visible;
                    this._closeButton.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
