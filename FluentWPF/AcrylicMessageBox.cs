using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    /// <summary>
    /// The setting for messagebox;
    /// </summary>
    public class MessageBoxSetting
    {
        /// <summary>
        /// The text shown on yes button;
        /// </summary>
        public string YesButtonText { get; set; } = "Yes";

        /// <summary>
        /// The text shown on no button;
        /// </summary>
        public string NoButtonText { get; set; } = "No";

        /// <summary>
        /// The text shown on ok button;
        /// </summary>
        public string OkButtonText { get; set; } = "Ok";

        /// <summary>
        /// The text shown on cancel button;
        /// </summary>
        public string CancelButtonText { get; set; } = "Cancel";

        /// <summary>
        /// The title of the messagebox;
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// The content shown on the messagebox;
        /// </summary>
        public string MessageBoxText { get; set; } = string.Empty;

        /// <summary>
        /// The messagebox button type;
        /// </summary>
        public MessageBoxButton MessageBoxButton { get; set; } = MessageBoxButton.OK;

        /// <summary>
        /// The icon shown on the titlebar of the messagebox;
        /// </summary>
        public ImageSource MessageBoxIcon { get; set; }

        /// <summary>
        /// The owner of the messagebox window,this could be a <see cref="Window"/> or a <see cref="IntPtr"/> that indicates a win32 window;
        /// </summary>
        public object Owner { get; set; }
    }

    /// <summary>
    /// Arcyliced messagebox;
    /// </summary>
    public sealed class AcrylicMessageBox
    {
        public static MessageBoxResult Show(string messageBoxText) => Show(messageBoxText, null);

        public static MessageBoxResult Show(string messageBoxText, ImageSource messageBoxIcon)
        {
            var setting = new MessageBoxSetting
            {
                MessageBoxText = messageBoxText,
                MessageBoxButton = MessageBoxButton.OK,
                MessageBoxIcon = messageBoxIcon
            };

            return Show(setting);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton messageBoxButton)
        {
            var setting = new MessageBoxSetting
            {
                MessageBoxText = messageBoxText,
                Caption = caption,
                MessageBoxButton = messageBoxButton,
            };

            return Show(setting);
        }

        public static MessageBoxResult Show(object owner, string messageBoxText, string caption, MessageBoxButton messageBoxButton)
        {
            var setting = new MessageBoxSetting
            {
                Owner = owner,
                MessageBoxText = messageBoxText,
                Caption = caption,
                MessageBoxButton = messageBoxButton
            };

            return Show(setting);
        }

        public static MessageBoxResult Show(MessageBoxSetting messageBoxSetting)
        {
            var window = new AcrylicMessageBoxWindow(messageBoxSetting);
            window.ShowDialog();
            return window.MessageBoxResult;
        }

    }
}