using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FluentWPFSample.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxSample.xaml
    /// </summary>
    public partial class MessageBoxSample : Window
    {
        public MessageBoxSample()
        {
            InitializeComponent();
        }

        private void WriteResult(MessageBoxResult messageBoxResult, [CallerMemberName] string callerName = null)
        {
            outputTextBox.Text += $"MessageBoxResult received:{messageBoxResult.ToString()},\tcallerName:{callerName},{Environment.NewLine}";
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show("Hello,This is a message from MessageBoxSample");
            WriteResult(result);
        }

        private void OnYesNoClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(
                "A new package just arrived,receive it or not?",
                "Notification",
                MessageBoxButton.YesNo
            );
            WriteResult(result);
        }

        private void OnOkCancelClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(
                "The bomb is being planted,are you sure?",
                "Remember to think it twice",
                MessageBoxButton.OKCancel
            );

            WriteResult(result);
        }

        private void OnYesNoCancelClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(
                "Do you want to leave without saving the document?",
                "Wait a minute",
                MessageBoxButton.YesNoCancel
            );

            WriteResult(result);
        }

        private void OnOkWithIconClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(
                "Hello,This is message from github.",
                new BitmapImage(new Uri("pack://application:,,,/FluentWPFSample;component/Assets/Images/github.png", UriKind.RelativeOrAbsolute))
            );

            WriteResult(result);
        }

        private void OnCustomYesNoCancelTextClick(object sender, RoutedEventArgs e)
        {
            var setting = new MessageBoxSetting
            {
                MessageBoxText = "こんいちわ！",
                YesButtonText = "はい",
                NoButtonText = "いいえ",
                CancelButtonText = "キャンセル",
                MessageBoxButton = MessageBoxButton.YesNoCancel
            };
            var result = AcrylicMessageBox.Show(setting);

            WriteResult(result);
        }

        private void OnOkWithOwnerClick(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(
                this,
                "This is a message from owner.",
                Title, MessageBoxButton.OK
            );

            WriteResult(result);
        }
    }
}