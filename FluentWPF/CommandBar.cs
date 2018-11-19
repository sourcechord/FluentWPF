using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SourceChord.FluentWPF
{
    public class CommandBar : UserControl
    {
        //Dependency Properties
        public static readonly DependencyPropertyKey PrimaryCommandsProperty = DependencyProperty.RegisterReadOnly("PrimaryCommands", typeof(UIElementCollection), typeof(CommandBar), new PropertyMetadata());
        public static readonly DependencyPropertyKey MainContentProperty = DependencyProperty.RegisterReadOnly("MainContent", typeof(UIElementCollection), typeof(CommandBar), new PropertyMetadata());
        public static readonly DependencyPropertyKey IconProperty = DependencyProperty.RegisterReadOnly("Icon", typeof(UIElementCollection), typeof(CommandBar), new PropertyMetadata());

        public static readonly DependencyProperty CommandBarOrientationProperty = DependencyProperty.Register("CommandBarOrientation", typeof(Orientation), typeof(CommandBar), new UIPropertyMetadata(Orientation.Vertical, OnCommandBarOrientationPropertyChanged));

        public static readonly DependencyProperty SecondaryCommandsProperty = DependencyProperty.Register("SecondaryCommands", typeof(ContextMenu), typeof(CommandBar), new PropertyMetadata());

        static Color cmd_color = (Color)Application.Current.Resources["SystemControlAcrylicWindowFallbackColor"];
        public static readonly DependencyProperty CommandBarColorProperty = DependencyProperty.Register("CommandBarColor", typeof(SolidColorBrush), typeof(CommandBar), new PropertyMetadata(new SolidColorBrush(cmd_color)));



        //The command bar has three parts: Content, Primary Commands, Secondary Commands and More button
        Grid MainGrid;
        Grid Content_Grid;

        Grid IconHolder_More;

        ColumnDefinition Column_Content;
        ColumnDefinition Column_PrimaryCommands;
        ColumnDefinition Column_More;



        public Button button_more;


        StackPanel StackPanel_PrimaryCommands;



        bool isExpanded = false;


        private static void OnCommandBarOrientationPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CommandBar commandBar = (CommandBar)source;
            if ((Orientation)e.NewValue == Orientation.Vertical)
            {
                //default
                commandBar.button_more.BorderThickness = new Thickness(1, 1, 1, 1);
            }
            else
            {
                //horizontal
                //change the border size


                commandBar.button_more.BorderThickness = new Thickness(0, 0, 0, 0);

            }
        }


        public Orientation CommandBarOrientation
        {
            get
            {
                return (Orientation)GetValue(CommandBarOrientationProperty);
            }
            set
            {
                SetValue(CommandBarOrientationProperty, value);
                //change_orientation();
                //RaisePropertyChanged("CommandBarOrientation");
            }
        }



        public UIElementCollection PrimaryCommands
        {
            get
            {
                return (UIElementCollection)GetValue(PrimaryCommandsProperty.DependencyProperty);
            }
            set
            {
                SetValue(PrimaryCommandsProperty, value);
            }
        }


        public UIElementCollection MainContent
        {
            get
            {
                return (UIElementCollection)GetValue(MainContentProperty.DependencyProperty);
            }
            set
            {
                SetValue(MainContentProperty, value);
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


        public ContextMenu SecondaryCommands
        {
            get
            {
                return (ContextMenu)GetValue(SecondaryCommandsProperty);
            }
            set
            {
                SetValue(SecondaryCommandsProperty, value);
            }
        }


        public SolidColorBrush CommandBarColor
        {
            get
            {
                return (SolidColorBrush)GetValue(CommandBarColorProperty);
            }
            set
            {
                SetValue(CommandBarColorProperty, value);
            }
        }






        public CommandBar()
        {



            Height = 48;
            MainGrid = new Grid();
            AddChild(MainGrid);
            Content_Grid = new Grid();

            IconHolder_More = new Grid();
            IconHolder_More.Width = 20;
            IconHolder_More.Height = 20;




            //settings of the CommandBar

            MainGrid.Background = CommandBarColor;


            SystemTheme.ThemeChanged += new EventHandler((object obj, EventArgs args) =>
            {
                cmd_color = (Color)Application.Current.Resources["SystemControlAcrylicWindowFallbackColor"];
                CommandBarColor = new SolidColorBrush(cmd_color);
                MainGrid.Background = CommandBarColor;
            });



            //create three columns
            Column_Content = new ColumnDefinition();
            Column_Content.Width = new System.Windows.GridLength(100, System.Windows.GridUnitType.Star);


            Column_PrimaryCommands = new ColumnDefinition();
            Column_PrimaryCommands.Width = System.Windows.GridLength.Auto;


            Column_More = new ColumnDefinition();
            Column_More.Width = System.Windows.GridLength.Auto;


            MainGrid.ColumnDefinitions.Add(Column_Content);
            MainGrid.ColumnDefinitions.Add(Column_PrimaryCommands);
            MainGrid.ColumnDefinitions.Add(Column_More);


            //add experimental objects

            Style button_style = Application.Current.FindResource("ButtonRevealStyle") as Style;



            button_more = new Button();
            button_more.Background = Brushes.Transparent;
            button_more.Width = 48;
            button_more.Style = button_style;
            button_more.Click += Button_more_Click;
            button_more.Content = IconHolder_More;
            button_more.BorderThickness = new Thickness(1, 1, 1, 1);
            button_more.ToolTip = "See more";




            Grid.SetColumn(button_more, 2);
            //SetColumn(button_more, 2);
            Grid.SetRow(button_more, 0);

            MainGrid.Children.Add(button_more);


            //add experimental objects



            StackPanel_PrimaryCommands = new StackPanel();
            StackPanel_PrimaryCommands.Orientation = Orientation.Horizontal;
            //StackPanel_PrimaryCommands.Width = 100;


            Grid.SetColumn(StackPanel_PrimaryCommands, 1);
            Grid.SetRow(StackPanel_PrimaryCommands, 0);

            MainGrid.Children.Add(StackPanel_PrimaryCommands);

            Grid.SetColumn(Content_Grid, 0);
            Grid.SetRow(Content_Grid, 0);

            MainGrid.Children.Add(Content_Grid);


            PrimaryCommands = StackPanel_PrimaryCommands.Children;
            MainContent = Content_Grid.Children;

            Icon = IconHolder_More.Children;


            change_orientation();





        }




        private void change_orientation()
        {
            for (int index = 0; index < StackPanel_PrimaryCommands.Children.Count; index++)
            {
                try
                {
                    AppBarButton current_btn = (AppBarButton)StackPanel_PrimaryCommands.Children[index];

                    //change the orientation based on thje orientation
                    current_btn.ButtonOrientation = CommandBarOrientation;
                }
                catch (Exception)
                {

                }
            }
        }

        private async void Button_more_Click(object sender, RoutedEventArgs e)
        {
            process_command_movement();
        }

        private async void process_command_movement()
        {

            if (CommandBarOrientation == Orientation.Vertical)
            {
                //expand
                double high = 0;

                for (int index = 0; index < StackPanel_PrimaryCommands.Children.Count; index++)
                {
                    try
                    {
                        AppBarButton current_btn = (AppBarButton)StackPanel_PrimaryCommands.Children[index];

                        if (current_btn.GetHeight() > high)
                        {
                            high = current_btn.GetHeight();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }



                if (isExpanded == true)
                {
                    //close
                    isExpanded = false;

                    Collapse();

                    await Task.Delay(300);
                }
                else
                {
                    //open
                    isExpanded = true;

                    Expand(high);

                }



                for (int index = 0; index < StackPanel_PrimaryCommands.Children.Count; index++)
                {
                    try
                    {
                        AppBarButton current_btn = (AppBarButton)StackPanel_PrimaryCommands.Children[index];

                        current_btn.IsExpanded = isExpanded;
                    }
                    catch (Exception)
                    {

                    }
                }


                await Task.Delay(300);

                if (isExpanded == true)
                {
                    try
                    {


                        SecondaryCommands.Closed += new RoutedEventHandler(async (object obj, RoutedEventArgs args) =>
                        {
                            //close the command bar
                            isExpanded = false;
                            Collapse();

                            await Task.Delay(300);
                            for (int index = 0; index < StackPanel_PrimaryCommands.Children.Count; index++)
                            {
                                try
                                {
                                    AppBarButton current_btn = (AppBarButton)StackPanel_PrimaryCommands.Children[index];

                                    current_btn.IsExpanded = isExpanded;
                                }
                                catch (Exception)
                                {

                                }
                            }
                        });


                        //await Task.Delay(300);

                        if (VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            SecondaryCommands.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                        }
                        else
                        {
                            SecondaryCommands.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        }

                        SecondaryCommands.PlacementTarget = button_more;
                        SecondaryCommands.IsOpen = true;


                    }
                    catch (Exception) { }
                }

            }
            else
            {
                //show the secondary commands only
                try
                {


                    SecondaryCommands.Closed += new RoutedEventHandler(async (object obj, RoutedEventArgs args) =>
                    {
                        //close the command bar
                        isExpanded = false;
                        Collapse();

                        await Task.Delay(300);
                        for (int index = 0; index < StackPanel_PrimaryCommands.Children.Count; index++)
                        {
                            try
                            {
                                AppBarButton current_btn = (AppBarButton)StackPanel_PrimaryCommands.Children[index];

                                current_btn.IsExpanded = isExpanded;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    });


                    //await Task.Delay(300);

                    if (VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        SecondaryCommands.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                    }
                    else
                    {
                        SecondaryCommands.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    }

                    SecondaryCommands.PlacementTarget = button_more;
                    SecondaryCommands.IsOpen = true;


                }
                catch (Exception) { }



            }
            //get the highest 

        }



        private void Expand(double height)
        {
            QuarticEase ease = new QuarticEase();
            ease.EasingMode = EasingMode.EaseOut;

            DoubleAnimation animation = new DoubleAnimation(height, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            animation.EasingFunction = ease;

            BeginAnimation(HeightProperty, animation);


        }

        private void Collapse()
        {
            QuarticEase ease = new QuarticEase();
            ease.EasingMode = EasingMode.EaseOut;

            DoubleAnimation animation = new DoubleAnimation(48, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            animation.EasingFunction = ease;

            BeginAnimation(HeightProperty, animation);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}

