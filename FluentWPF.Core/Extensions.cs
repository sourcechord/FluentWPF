using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public static class Extensions
    {

        //header text
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(Extensions), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHeader(UIElement element, string value)
        {
            element.SetValue(HeaderProperty, value);
        }
        public static string GetHeader(UIElement element)
        {
            return (string)element.GetValue(HeaderProperty);
        }





        //header size
        public static readonly DependencyProperty HeaderSizeProperty = DependencyProperty.RegisterAttached("HeaderSize", typeof(double), typeof(Extensions), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHeaderSize(UIElement element, double value)
        {
            element.SetValue(HeaderSizeProperty, value);
        }
        public static double GetHeaderSize(UIElement element)
        {
            return (double)element.GetValue(HeaderSizeProperty);
        }



        //Header ForegroundBrush
        public static readonly DependencyProperty HeaderForegroundBrushProperty = DependencyProperty.RegisterAttached("HeaderForegroundBrush", typeof(Brush), typeof(Extensions), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHeaderForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(HeaderForegroundBrushProperty, value);
        }
        public static Brush GetHeaderForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(HeaderForegroundBrushProperty);
        }



        //Placeholder text
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(Extensions), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPlaceholderText(UIElement element, string value)
        {
            element.SetValue(PlaceholderTextProperty, value);
        }
        public static string GetPlaceholderText(UIElement element)
        {
            return (string)element.GetValue(PlaceholderTextProperty);
        }


        //Placeholder ForegroundBrush
        public static readonly DependencyProperty PlaceholderForegroundBrushProperty = DependencyProperty.RegisterAttached("PlaceholderForegroundBrush", typeof(Brush), typeof(Extensions), new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPlaceholderForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(PlaceholderForegroundBrushProperty, value);
        }
        public static Brush GetPlaceholderForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(PlaceholderForegroundBrushProperty);
        }

    }



}
