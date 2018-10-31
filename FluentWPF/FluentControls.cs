using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SourceChord.FluentWPF
{
    public class TextBoxHelper
    {

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.RegisterAttached("HeaderText", typeof(string), typeof(TextBoxHelper), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHeaderText(UIElement element, string value)
        {
            element.SetValue(HeaderTextProperty, value);
        }
        public static string GetHeaderText(UIElement element)
        {
            return (string)element.GetValue(HeaderTextProperty);
        }



        //private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}



    }
}
