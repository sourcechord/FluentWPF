using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SourceChord.FluentWPF
{
    public class AppBarButton : Button
    {

        public StackPanel MainContent;
        public TextBlock Label_Text;

        bool isExpanded = false;

        public Grid Icon_Holder;




        public static readonly DependencyPropertyKey IconProperty = DependencyProperty.RegisterReadOnly("Icon", typeof(UIElementCollection), typeof(AppBarButton), new PropertyMetadata());
        public static readonly DependencyProperty ButtonOrientationProperty = DependencyProperty.Register("ButtonOrientation", typeof(Orientation), typeof(AppBarButton), new UIPropertyMetadata(Orientation.Vertical, OnButtonOrientationPropertyChanged));
        //public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(AppBarButton), new PropertyMetadata(true));


        private static void OnButtonOrientationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {

            AppBarButton appBarButton = (AppBarButton)source;

            appBarButton.MainContent.Orientation = (Orientation)e.NewValue;


            if ((Orientation)e.NewValue == Orientation.Horizontal)
            {
                appBarButton.MainContent.VerticalAlignment = VerticalAlignment.Center;
                appBarButton.MainContent.Margin = new Thickness(0, 10, 0, 0);

                appBarButton.Label_Text.Margin = new Thickness(5, 0, 0, 0);
                appBarButton.Width = double.NaN;
                appBarButton.MinWidth = 48;
                appBarButton.BorderThickness = new Thickness(0, 0, 0, 0);
                appBarButton.Icon_Holder.Margin = new System.Windows.Thickness(0, 0, 0, 0);


                if (appBarButton.Label_Text.Text == "")
                {
                    appBarButton.Label_Text.Visibility = Visibility.Collapsed;
                    appBarButton.Width = 40;
                }
                else
                {
                    appBarButton.Label_Text.Visibility = Visibility.Visible;
                }
            }
            else
            {
                appBarButton.Label_Text.Visibility = Visibility.Hidden;
                appBarButton.Width = 67;
                appBarButton.Icon_Holder.Margin = new System.Windows.Thickness(0, 7, 0, 5);
            }



        }





        public Orientation ButtonOrientation
        {
            get
            {
                return (Orientation)GetValue(ButtonOrientationProperty);
            }
            set
            {
                SetValue(ButtonOrientationProperty, value);
                //process_orientation();
                //RaisePropertyChanged("ButtonOrientation");
            }

        }


        public UIElementCollection Icon
        {
            get
            {
                return (UIElementCollection)GetValue(IconProperty.DependencyProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public string Label
        {
            get
            {
                return Label_Text.Text;
            }
            set
            {
                Label_Text.Text = value;
                ToolTip = value;
                RaisePropertyChanged("Label");
            }
        }


        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                process_orientation();
            }
        }


        public double GetHeight()
        {

            return 24 + Label_Text.ActualHeight + 20;
        }

        public new bool IsEnabled
        {
            get
            {
                return (bool)GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
                if (value == true)
                {
                    //reduce opacity of icon
                    Icon_Holder.Opacity = 1.0;
                    RaisePropertyChanged("IsEnabled");
                }
                else
                {
                    //full opacity
                    Icon_Holder.Opacity = 0.3;
                    RaisePropertyChanged("IsEnabled");
                }

            }
        }





        public AppBarButton()
        {
            Color cmd_color = (Color)Application.Current.Resources["SystemBaseHighColor"];
            Foreground = new SolidColorBrush(cmd_color);

            SystemTheme.ThemeChanged += new EventHandler((object obj, EventArgs args) =>
            {
                cmd_color = (Color)Application.Current.Resources["SystemBaseHighColor"];
                Foreground = new SolidColorBrush(cmd_color);
            });


            Style style = Application.Current.FindResource("ButtonRevealStyle") as Style;
            Style = style;
            Background = Brushes.Transparent;
            Margin = new Thickness(1, 0, 1, 0);
            BorderThickness = new Thickness(1, 1, 1, 1);


            MainContent = new StackPanel();
            Padding = new System.Windows.Thickness(10, 3, 10, 3);


            MainContent.Orientation = ButtonOrientation;

            Icon_Holder = new Grid();


            Icon_Holder.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Icon_Holder.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            Icon_Holder.Width = 20;
            Icon_Holder.Height = 20;
            Label_Text = new TextBlock();
            Label_Text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Label_Text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Label_Text.TextWrapping = TextWrapping.Wrap;
            Label_Text.TextAlignment = TextAlignment.Center;

            //Icon_Holder.Background = Brushes.Green;
            Label_Text.Text = Label;


            MainContent.Children.Add(Icon_Holder);
            MainContent.Children.Add(Label_Text);


            Icon = Icon_Holder.Children;


            Content = MainContent;

            ToolTip = Label;




            //defaults for the vertical orientation
            VerticalContentAlignment = VerticalAlignment.Top;
            Width = 67;
            Label_Text.Visibility = Visibility.Hidden;
            Icon_Holder.Margin = new System.Windows.Thickness(0, 7, 0, 5);












            // process_orientation();


        }




        public void process_orientation()
        {



            if (ButtonOrientation == Orientation.Horizontal)
            {
                if (Label_Text.Text.Length > 0)
                {
                    Icon_Holder.Margin = new System.Windows.Thickness(0, 0, 5, 0);

                    //always visible
                    Label_Text.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    Icon_Holder.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                    Label_Text.Visibility = System.Windows.Visibility.Collapsed;
                    //set the min width
                    Width = 48;
                }

            }
            else
            {
                //vertical
                Icon_Holder.Margin = new System.Windows.Thickness(0, 7, 0, 5);


                //fix the width
                Width = 67;

                //process expansion
                if (isExpanded == true)
                {
                    Label_Text.Visibility = System.Windows.Visibility.Visible;
                    //adjust the margin of the icon
                    //Icon_Holder.Margin = new System.Windows.Thickness(0, 10, 0, 5);
                }
                else
                {
                    Label_Text.Visibility = System.Windows.Visibility.Hidden;
                    //Icon_Holder.Margin = new System.Windows.Thickness(0, 10, 0, 5);
                }
            }
        }






        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
}
