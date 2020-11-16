using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
   

    /// <summary>
    /// MessageBox designed in fluent style.
    /// </summary>
    internal class AcrylicMessageBoxWindow : AcrylicWindow
    {
        static AcrylicMessageBoxWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicMessageBoxWindow), new FrameworkPropertyMetadata(typeof(AcrylicMessageBoxWindow)));
        }
        
        public AcrylicMessageBoxWindow(MessageBoxSetting messageBoxSetting)
        {
            MessageBoxSetting = messageBoxSetting ?? throw new ArgumentNullException(nameof(messageBoxSetting));
            Title = messageBoxSetting?.Caption ?? string.Empty;
            MessageBoxText = messageBoxSetting?.MessageBoxText ?? string.Empty;

            if (SetWindowOwner(messageBoxSetting.Owner))
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

        }

        private bool SetWindowOwner(object owner)
        {
            if (owner is IntPtr ownerHandle)
            {
                new WindowInteropHelper(this)
                {
                    Owner = ownerHandle
                };
                return true;
            }
            else if (owner is Window windowOwner)
            {
                Owner = windowOwner;
                return true;
            }

            return false;
        }

        public MessageBoxSetting MessageBoxSetting { get; }

        private const string PART_YESBUTTON = "yesButton";

        private const string PART_NOBUTTON = "noButton";

        private const string PART_OKBUTTON = "okButton";

        private const string PART_CANCELBUTTON = "cancelButton";
        private const string PART_MESSAGETEXT = "messageText";

        private ButtonBase _yesButton;
        private ButtonBase _noButton;
        private ButtonBase _okButton;
        private ButtonBase _cancelButton;
        private TextBlock _messageText;

        private static readonly MessageBoxButton[] _yesButtonVisibleStates = new MessageBoxButton[]
        {
            MessageBoxButton.YesNo,
            MessageBoxButton.YesNoCancel
        };

        private static readonly MessageBoxButton[] _noButtonVisibleStates = new MessageBoxButton[]
        {
            MessageBoxButton.YesNo,
            MessageBoxButton.YesNoCancel
        };

        private static readonly MessageBoxButton[] _okButtonVisibleStates = new MessageBoxButton[]
        {
            MessageBoxButton.OK,
            MessageBoxButton.OKCancel
        };

        private static readonly MessageBoxButton[] _cancelButtonVisibleStates = new MessageBoxButton[]
        {
            MessageBoxButton.OKCancel,
            MessageBoxButton.YesNoCancel
        };

        internal MessageBoxResult MessageBoxResult { get; private set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (!(newContent is FrameworkElement contentElement))
            {
                return;
            }
            
            _yesButton = LogicalTreeHelper.FindLogicalNode(contentElement,PART_YESBUTTON) as ButtonBase;
            _noButton = LogicalTreeHelper.FindLogicalNode(contentElement, PART_NOBUTTON) as ButtonBase;
            _okButton = LogicalTreeHelper.FindLogicalNode(contentElement, PART_OKBUTTON) as ButtonBase;
            _cancelButton = LogicalTreeHelper.FindLogicalNode(contentElement, PART_CANCELBUTTON) as ButtonBase;
            
            InitialMessageBoxButton(_yesButton, MessageBoxResult.Yes, MessageBoxSetting.YesButtonText,_yesButtonVisibleStates);
            InitialMessageBoxButton(_noButton, MessageBoxResult.No, MessageBoxSetting.NoButtonText,_noButtonVisibleStates);
            InitialMessageBoxButton(_okButton, MessageBoxResult.OK, MessageBoxSetting.OkButtonText,_okButtonVisibleStates);
            InitialMessageBoxButton(_cancelButton, MessageBoxResult.Cancel, MessageBoxSetting.CancelButtonText,_cancelButtonVisibleStates);

            _messageText = LogicalTreeHelper.FindLogicalNode(contentElement, PART_MESSAGETEXT) as TextBlock;
            if (_messageText != null)
            {
                _messageText.Text = MessageBoxSetting.MessageBoxText;
            }

            Icon = MessageBoxSetting.MessageBoxIcon;
            Title = MessageBoxSetting.Caption;
        }
        

        /// <summary>
        /// Initialize Button;
        /// </summary>
        /// <param name="messageBoxButton">Button to be intialized.</param>
        /// <param name="messageBoxResult"></param>
        /// <param name="messageButtonText"></param>
        private void InitialMessageBoxButton(ButtonBase messageBoxButton,MessageBoxResult messageBoxResult,string messageButtonText,MessageBoxButton[] visibleButtonStates)
        {
            if(messageBoxButton == null)
            {
                return;
            }

            void OnButtonClick(object sender,RoutedEventArgs e)
            {
                MessageBoxResult = messageBoxResult;
                Close();
            }

            messageBoxButton.Click -= OnButtonClick;
            messageBoxButton.Click += OnButtonClick;
            messageBoxButton.Content = messageButtonText;
            if (visibleButtonStates.Contains(MessageBoxSetting.MessageBoxButton))
            {
                messageBoxButton.Visibility = Visibility.Visible;
            }
            else
            {
                messageBoxButton.Visibility = Visibility.Collapsed;
            }
        }

        public string MessageBoxText
        {
            get { return (string)GetValue(MessageBoxTextProperty); }
            set { SetValue(MessageBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageBoxTextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageBoxTextProperty =
            DependencyProperty.Register(nameof(MessageBoxText), typeof(string), typeof(AcrylicMessageBoxWindow), new PropertyMetadata(string.Empty));

    }
}
