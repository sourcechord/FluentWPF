using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public class FluentPasswordBox : TextBox
    {
        //Note: Functionality of masking is based from StackOverflow answer https://stackoverflow.com/questions/17407620/custom-masked-passwordbox-in-wpf



        /// <summary>
        ///   Dependency property to hold watermark for CustomPasswordBox
        /// </summary>
        public static readonly DependencyProperty SecurePasswordProperty = DependencyProperty.Register("SecurePassword", typeof(SecureString), typeof(FluentPasswordBox), new UIPropertyMetadata(new SecureString()));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(""));


        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty PlaceHolderForegroundBrushProperty = DependencyProperty.Register("PlaceHolderForegroundBrush", typeof(Brush), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.Register("TextLength", typeof(int), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty HeaderSizeProperty = DependencyProperty.Register("HeaderSize", typeof(double), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(14.0));
        public static readonly DependencyProperty HeaderForegroundBrushProperty = DependencyProperty.Register("HeaderForegroundBrush", typeof(Brush), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(FluentPasswordBox), new FrameworkPropertyMetadata(""));


        char password_char = '●';





        public FluentPasswordBox()
        {
            PreviewTextInput += OnPreviewTextInput;
            PreviewKeyDown += OnPreviewKeyDown;
            CommandManager.AddPreviewExecutedHandler(this, PreviewExecutedHandler);

            MaxLines = 1;
            AcceptsReturn = false;


            Style style = Application.Current.FindResource("FluentPasswordBoxRevealStyle") as Style;
            Style = style;


        }


        public new void Clear()
        {
            SecurePassword.Clear();
            Text = "";
        }



        public SecureString SecurePassword
        {
            get
            {
                return (SecureString)GetValue(SecurePasswordProperty);
            }

            set
            {
                SetValue(SecurePasswordProperty, value);

            }
        }

        public String Password
        {
            get
            {
                return SecureStringToString(SecurePassword);
            }
            set
            {

                SecurePassword.Clear();
                Text = "";
                CaretIndex = 0;
                AddToSecureString(value);
                SetValue(PasswordProperty, value);
                RaisePropertyChanged("Password");

            }
        }


        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set
            {
                SetValue(PlaceHolderTextProperty, value);
                RaisePropertyChanged("PlaceHolder");
            }
        }


        public Brush PlaceHolderForegroundBrush
        {
            get { return (Brush)GetValue(PlaceHolderForegroundBrushProperty); }
            set
            {
                SetValue(PlaceHolderForegroundBrushProperty, value);
                RaisePropertyChanged("PlaceHolderForegroundBrush");
            }
        }

        public int TextLength
        {
            get
            {
                string text_chars = Text;
                int counts = text_chars.Length;



                return counts;
            }
        }




        //header
        public double HeaderSize
        {

            get { return (int)GetValue(HeaderSizeProperty); }
            set
            {
                SetValue(HeaderSizeProperty, value);
                RaisePropertyChanged("HeaderSize");
            }
        }

        public Brush HeaderForegroundBrush
        {
            get { return (Brush)GetValue(HeaderForegroundBrushProperty); }
            set
            {
                SetValue(HeaderForegroundBrushProperty, value);
                RaisePropertyChanged("HeaderForegroundBrush");
            }
        }


        public string Header
        {
            get
            {
                return (string)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
                RaisePropertyChanged("Header");
            }
        }





        private static void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Command == ApplicationCommands.Copy ||
                executedRoutedEventArgs.Command == ApplicationCommands.Cut ||
                executedRoutedEventArgs.Command == ApplicationCommands.Paste)
            {
                executedRoutedEventArgs.Handled = true;
            }
        }

        /// <summary>
        ///   Method to handle PreviewTextInput events
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="textCompositionEventArgs">Event Text Arguments</param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            AddToSecureString(textCompositionEventArgs.Text);
            textCompositionEventArgs.Handled = true;
        }

        /// <summary>
        ///   Method to handle PreviewKeyDown events
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="keyEventArgs">Event Text Arguments</param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            Key pressedKey = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            switch (pressedKey)
            {
                case Key.Space:
                    AddToSecureString(" ");
                    keyEventArgs.Handled = true;
                    break;
                case Key.Back:
                case Key.Delete:
                    if (SelectionLength > 0)
                    {
                        RemoveFromSecureString(SelectionStart, SelectionLength);
                    }
                    else if (pressedKey == Key.Delete && CaretIndex < Text.Length)
                    {
                        RemoveFromSecureString(CaretIndex, 1);
                    }
                    else if (pressedKey == Key.Back && CaretIndex > 0)
                    {
                        int caretIndex = CaretIndex;
                        if (CaretIndex > 0 && CaretIndex < Text.Length)
                            caretIndex = caretIndex - 1;
                        RemoveFromSecureString(CaretIndex - 1, 1);
                        CaretIndex = caretIndex;
                    }

                    keyEventArgs.Handled = true;
                    break;
            }
        }


        private void AddToSecureString(string text)
        {
            if (SelectionLength > 0)
            {
                RemoveFromSecureString(SelectionStart, SelectionLength);
            }

            foreach (char c in text.Replace("\r", ""))
            {
                int caretIndex = CaretIndex;
                SecurePassword.InsertAt(caretIndex, c);

                if (caretIndex == Text.Length)
                {
                    Text = Text.Insert(caretIndex++, c.ToString());
                }
                else
                {
                    Text = Text.Insert(caretIndex++, password_char.ToString());
                }
                MaskAllDisplayText();
                CaretIndex = caretIndex;
            }
        }


        private void RemoveFromSecureString(int startIndex, int trimLength)
        {
            int caretIndex = CaretIndex;
            for (int i = 0; i < trimLength; ++i)
            {
                SecurePassword.RemoveAt(startIndex);
            }

            Text = Text.Remove(startIndex, trimLength);
            CaretIndex = caretIndex;
        }

        private void MaskAllDisplayText()
        {
            int caretIndex = CaretIndex;
            Text = new string(password_char, Text.Length);
            CaretIndex = caretIndex;
        }


        String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }


    }
}
