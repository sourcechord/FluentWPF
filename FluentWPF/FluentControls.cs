using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SourceChord.FluentWPF
{
    public class FluentControls : FrameworkElement
    {

        public static void SetHeadertext(UIElement element, string value)
        {
            element.SetValue(HeadertextProperty, value);
        }
        public static string GetHeadertext(UIElement element)
        {
            return (string)element.GetValue(HeadertextProperty);
        }


        public static readonly DependencyProperty HeadertextProperty = DependencyProperty.RegisterAttached("Headertext", typeof(string), typeof(FluentControls), new PropertyMetadata(""));





    }
}
