using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public class FluentTextBox : TextBox
    {


        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(FluentTextBox), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty PlaceHolderForegroundBrushProperty = DependencyProperty.Register("PlaceHolderForegroundBrush", typeof(Brush), typeof(FluentTextBox), new FrameworkPropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.Register("TextLength", typeof(int), typeof(FluentTextBox), new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty HeaderSizeProperty = DependencyProperty.Register("HeaderSize", typeof(double), typeof(FluentTextBox), new FrameworkPropertyMetadata(14.0));
        public static readonly DependencyProperty HeaderForegroundBrushProperty = DependencyProperty.Register("HeaderForegroundBrush", typeof(Brush), typeof(FluentTextBox), new FrameworkPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(FluentTextBox), new FrameworkPropertyMetadata(""));




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






        public FluentTextBox()
        {
            Style style = Application.Current.FindResource("FluentTextBoxRevealStyle") as Style;

            Style = style;
        }






        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
