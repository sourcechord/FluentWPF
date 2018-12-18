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
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.RegisterAttached("HeaderText", typeof(string), typeof(Extensions), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHeaderText(UIElement element, string value)
        {
            element.SetValue(HeaderTextProperty, value);
        }
        public static string GetHeaderText(UIElement element)
        {
            return (string)element.GetValue(HeaderTextProperty);
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
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.RegisterAttached("PlaceHolderText", typeof(string), typeof(Extensions), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPlaceHolderText(UIElement element, string value)
        {
            element.SetValue(PlaceHolderTextProperty, value);
        }
        public static string GetPlaceHolderText(UIElement element)
        {
            return (string)element.GetValue(PlaceHolderTextProperty);
        }


        //PlaceHolder ForegroundBrush
        public static readonly DependencyProperty PlaceHolderForegroundBrushProperty = DependencyProperty.RegisterAttached("PlaceHolderForegroundBrush", typeof(Brush), typeof(Extensions), new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPlaceHolderForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(PlaceHolderForegroundBrushProperty, value);
        }
        public static Brush GetPlaceHolderForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(PlaceHolderForegroundBrushProperty);
        }

    }



}
