# FluentWPF
FluentWPF provides Fluent Design System for WPF.

## Overview
* Acrylic
  * AcrylicWindow
  * AcrylicBrush
* Reveal
  * Reveal styles for controls(Button/TextBox/ListBox)
* ParallaxView
* AccentColors


## Install

*Nuget Package*
```
Install-Package FluentWPF
```
https://www.nuget.org/packages/FluentWPF/

### Preparation
Add xmlns to xaml code.

```xml
xmlns:fw="clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF"
```

Add ResourceDictionary to App.xaml
```xml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/FluentWPF;component/Styles/Controls.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
```

## Usage


### AcrylicWindow


```xml
<fw:AcrylicWindow x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        xmlns:fw="clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF"
        mc:Ignorable="d"
        Title="MainWindow" Height="300" Width="300">
    <Grid Background="#70FFFFFF">
        <TextBlock HorizontalAlignment="Left" Margin="32,49,0,0" TextWrapping="Wrap" Text="TextBlock" VerticalAlignment="Top"
                   Foreground="Gray"/>
    </Grid>
</fw:AcrylicWindow>
```

**Code Behind**
Remove base class definition.
```cs
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
```


#### Properties


#### Using as Attached Property

AcrylicWindow is able to use as attached property.

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        xmlns:fw="clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF"
        mc:Ignorable="d"
        Title="AcrylicWindow2" Height="300" Width="300"
        fw:AcrylicWindow.Enabled="True">
    <Grid>
        
    </Grid>
</Window>
```

**Code Behind**
```cs
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
```


### Reveal

Reveal effect for controls.


```xml
```

To use reveal effect, parent container have to set `fw:PointerTracker.Enabled="True"`.




|Control|Style Name|Description|
|-----|-----|-----|


### ParallaxView

### AccentColors


## Lisence
[MIT](LICENSE)
