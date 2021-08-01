<h1 align="center">
<img src="./logo.png" width="256"/><br />
FluentWPF
</h1>
<h4 align="center">Fluent Design System for WPF</h4>

## Overview

* Acrylic
  * AcrylicWindow
  * AcrylicBrush
* Reveal
  * Reveal styles for controls(Button/TextBox/ListBox)
* ParallaxView
* AccentColors

![Reveal](./docs/Reveal/demo.gif)

## Install

*NuGet Package*
```
Install-Package FluentWPF
```
https://nuget.org/packages/FluentWPF

### Preparation

Add XAML namespace.

```xml
xmlns:fw="clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF"
```

Add ResourceDictionary to App.xaml.

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
             <!--  FluentWPF Controls  -->
            <ResourceDictionary Source="pack://application:,,,/FluentWPF;component/Styles/Controls.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Usage

### Acrylic

#### AcrylicWindow

![AcrylicWindow](./docs/Acrylic/AcrylicWindow.gif)

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
        <TextBlock Margin="10"
                   HorizontalAlignment="Left"
                   VerticalAlignment="Top"
                   Text="This is AcrylicWindow"
                   TextWrapping="Wrap" />
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

##### Properties

|Property Name|Type|Description|
|-----|-----|-----|
|TintColor|Color|Gets or sets the color tint for the semi-transparent acrylic material.|
|TintOpacity|double|Gets or sets the degree of opacity of the color tint.|
|NoiseOpacity|double|Gets or sets the degree of opacity of the noise layer.|
|FallbackColor|Color|Gets or sets the color when window is inactive.|
|AcrylicWindowStyle|Normal </br> NoIcon </br> None|Gets or sets a value that indicates the style of the Acrylic Window.|

The difference between kind of AcrylicWindowStyle is demonstrated as follows,

|AcrylicWindowStyle="Normal"|AcrylicWindowStyle="NoIcon"|AcrylicWindowStyle="None"|
|-----|-----|-----|
|<img src="https://user-images.githubusercontent.com/14825436/58757611-25d06800-854a-11e9-8661-b79d9e249036.png" height="90"/>|<img src="https://user-images.githubusercontent.com/14825436/58757615-37b20b00-854a-11e9-9512-966c912b15bb.png" height="90"/>|<img src="https://user-images.githubusercontent.com/14825436/58757616-4bf60800-854a-11e9-85b3-bff1518849ec.png" height="90"/>|

##### Using as Attached Property

AcrylicWindow can also be used as an Attached Property.

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

#### AcrylicBrush

![AcrylicBrush](./docs/Acrylic/AcrylicBrush.gif)

```xml
<Window x:Class="FluentWPFSample.Views.AcrylicBrushSample"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:fw="clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF"
        xmlns:local="clr-namespace:FluentWPFSample.Views"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        Title="AcrylicBrushSample"
        Width="640"
        Height="480"
        mc:Ignorable="d">
    <Window.Resources>
        <BooleanToVisibilityConverter x:Key="booleanToVisibilityConverter" />
    </Window.Resources>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <Grid x:Name="grid" Background="White">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition />
            </Grid.ColumnDefinitions>
            <StackPanel>
                <Button Width="75" Margin="5" Content="Button" />
                <Button Width="75" Margin="5" Content="Button" />
                <Button Width="75" Margin="5" Content="Button" />
            </StackPanel>
            <Image Grid.Column="1"
                   Margin="5"
                   Source="/FluentWPFSample;component/Assets/Images/1.jpg" />
        </Grid>

        <Rectangle Grid.ColumnSpan="2"
                   Margin="40"
                   Fill="{fw:AcrylicBrush grid}"
                   Stroke="Black"
                   Visibility="{Binding IsChecked, ElementName=chkShowAcrylicLayer, Converter={StaticResource booleanToVisibilityConverter}}" />
        <CheckBox x:Name="chkShowAcrylicLayer"
                  Grid.Row="1"
                  Margin="5"
                  HorizontalAlignment="Left"
                  Content="Show Acrylic Rect"
                  IsChecked="True" />
    </Grid>
</Window>
```

### Reveal

Reveal effect for controls.

To use the reveal effect, set `fw:PointerTracker.Enabled="True"` on a parent container.

![Reveal](./docs/Reveal/Reveal.gif)
```xml
<Grid fw:PointerTracker.Enabled="True" Background="#01FFFFFF" Margin="3">
    <StackPanel>
        <Button Content="Button" HorizontalAlignment="Left" Margin="5" Width="75" Height="32"
                Style="{StaticResource ButtonRevealStyle}"/>

        <Button Content="Button" HorizontalAlignment="Left" Margin="5" Width="75" Height="32"
                Background="Transparent"
                Style="{StaticResource ButtonRevealStyle}"/>

        <TextBox HorizontalAlignment="Left" Height="23" Margin="5" Text="TextBox" Width="120"
             Style="{StaticResource TextBoxRevealStyle}"/>
    </StackPanel>
</Grid>
```


#### Styles

|Control|Style Name|
|-----|-----|
|Button|ButtonRevealStyle|
|Button|ButtonAccentRevealStyle|
|Button|ButtonRoundRevealStyle|
|Button|ButtonRoundAccentRevealStyle|
|TextBox|TextBoxRevealStyle|
|PasswordBox|PasswordBoxRevealStyle|
|ListBox|ListBoxRevealStyle|
|ComboBox|ComboBoxRevealStyle|


### ParallaxView

![ParallaxView](./docs/Controls/ParallaxView.gif)

```xml
<Grid>
    <fw:ParallaxView VerticalShift="200" HorizontalShift="200"
                     Source="{Binding ElementName=list}">
        <Image Source="/FluentWPFSample;component/Assets/Images/1.jpg" Stretch="UniformToFill"/>
    </fw:ParallaxView>
    <ListBox x:Name="list" Background="#88EEEEEE" ScrollViewer.CanContentScroll="False"
             ItemsSource="{Binding Items}"/>
</Grid>
```

#### Properties

|Property Name|Type|Description|
|-----|-----|-----|

### AccentColors

![Brushes](./docs/AccentColors/Brushes.png)

```xml
<StackPanel Margin="5">
    <StackPanel.Resources>
        <Style TargetType="Border">
            <Setter Property="Width" Value="120" />
            <Setter Property="Height" Value="120" />
            <Setter Property="Margin" Value="3" />
            <Setter Property="BorderBrush" Value="Black" />
            <Setter Property="BorderThickness" Value="1" />
        </Style>
        <Style TargetType="TextBlock">
            <Setter Property="TextWrapping" Value="Wrap" />
            <Setter Property="VerticalAlignment" Value="Bottom" />
            <Setter Property="FontSize" Value="14" />
        </Style>
    </StackPanel.Resources>
    <StackPanel Orientation="Horizontal" Margin="5">
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentBrush}">
            <TextBlock Text="ImmersiveSystemAccentBrush" />
        </Border>
    </StackPanel>
    <StackPanel Orientation="Horizontal" Margin="5">
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentLight1Brush}">
            <TextBlock Text="ImmersiveSystemAccentLight1Brush"/>
        </Border>
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentLight2Brush}">
            <TextBlock Text="ImmersiveSystemAccentLight2Brush"/>
        </Border>
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentLight3Brush}">
            <TextBlock Text="ImmersiveSystemAccentLight3Brush" />
        </Border>
    </StackPanel>
    <StackPanel Orientation="Horizontal" Margin="5">
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentDark1Brush}">
            <TextBlock Text="ImmersiveSystemAccentDark1Brush" Foreground="White"/>
        </Border>
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentDark2Brush}">
            <TextBlock Text="ImmersiveSystemAccentDark2Brush" Foreground="White"/>
        </Border>
        <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentDark3Brush}">
            <TextBlock Text="ImmersiveSystemAccentDark3Brush" Foreground="White"/>
        </Border>
    </StackPanel>
</StackPanel>
```


#### Accent Color
_Accent color depends on the accent color of the system._

|Sample|Color|Brush|
|-----|-----|-----|
|<div class="rect" style="background: #a6d8ff"></div>#a6d8ff|ImmersiveSystemAccentLight3|ImmersiveSystemAccentLight3Brush|
|<div class="rect" style="background: #76b9ed"></div>#76b9ed|ImmersiveSystemAccentLight2|ImmersiveSystemAccentLight2Brush|
|<div class="rect" style="background: #429ce3"></div>#429ce3|ImmersiveSystemAccentLight1|ImmersiveSystemAccentLight1Brush|
|<div class="rect" style="background: #0078d7"></div>#0078d7|ImmersiveSystemAccent|ImmersiveSystemAccentBrush|
|<div class="rect" style="background: #005a9e"></div>#005a9e|ImmersiveSystemAccentDark1|ImmersiveSystemAccentDark1Brush|
|<div class="rect" style="background: #004275"></div>#004275|ImmersiveSystemAccentDark2|ImmersiveSystemAccentDark2Brush|
|<div class="rect" style="background: #002642"></div>#002642|ImmersiveSystemAccentDark3|ImmersiveSystemAccentDark3Brush|

**Usage:**
```xml
 <Border Background="{x:Static fw:AccentColors.ImmersiveSystemAccentBrush}"/>
 <Border Background="{Binding Path=(fw:AccentColors.ImmersiveSystemAccentBrush)}"/>
```



#### Base Color

|Light|Dark|Color|Brush|
|-----|-----|-----|-----|
|<div class="rect" style="background: #000000"></div>#000000|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|SystemBaseHighColor|SystemBaseHighColorBrush|
|<div class="rect" style="background: #333333"></div>#333333|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|SystemBaseMediumHighColor|SystemBaseMediumHighColorBrush|
|<div class="rect" style="background: #666666"></div>#666666|<div class="rect" style="background: #999999"></div>#999999|SystemBaseMediumColor|SystemBaseMediumColorBrush|
|<div class="rect" style="background: #999999"></div>#999999|<div class="rect" style="background: #666666"></div>#666666|SystemBaseMediumLowColor|SystemBaseMediumLowColorBrush|
|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|<div class="rect" style="background: #333333"></div>#333333|SystemBaseLowColor|SystemBaseLowColorBrush|

**Usage:**
```xml
 <Border Background="{DynamicResource SystemBaseHighColorBrush}"/>
```


#### Alt Color

|Light|Dark|Color|Brush|
|-----|-----|-----|-----|
|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|<div class="rect" style="background: #000000"></div>#000000|SystemAltHighColor|SystemAltHighColorBrush|
|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|<div class="rect" style="background: #333333"></div>#333333|SystemAltMediumHighColor|SystemAltMediumHighColorBrush|
|<div class="rect" style="background: #999999"></div>#999999|<div class="rect" style="background: #666666"></div>#666666|SytemAltMediumColor|SytemAltMediumColorBrush|
|<div class="rect" style="background: #666666"></div>#666666|<div class="rect" style="background: #999999"></div>#999999|SystemAltMediumLowColor|SystemAltMediumLowColorBrush|
|<div class="rect" style="background: #333333"></div>#333333|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|SystemAltLowColor|SystemAltLowColorBrush|

**Usage:**
```xml
 <TextBlock Foreground="{DynamicResource SystemAltHighColorBrush}"/>
```

#### Chrome Color

|Light|Dark|Color|Brush|
|-----|-----|-----|-----|
|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|<div class="rect" style="background: #767676"></div>#767676|SystemChromeHighColor|SystemChromeHighColorBrush|
|<div class="rect" style="background: #E6E6E6"></div>#E6E6E6|<div class="rect" style="background: #1F1F1F"></div>#1F1F1F|SytemAltMediumColor|SytemAltMediumColorBrush|
|<div class="rect" style="background: #F2F2F2"></div>#F2F2F2|<div class="rect" style="background: #2B2B2B"></div>#2B2B2B|SystemChromeMediumLowColor|SystemChromeMediumLowColorBrush|
|<div class="rect" style="background: #F2F2F2"></div>#F2F2F2|<div class="rect" style="background: #171717"></div>#171717|SystemChromeLowColor|SystemChromeLowColorBrush|

**Usage:**
```xml
 <Border Background="{DynamicResource SystemChromeMediumBrush}"/>
```

#### Opacity Color

Windows includes a set of colors that differ only by their opacities:

|Base Color|Opacity|Color|Brush|
|-----|-----|-----|-----|
|<div class="rect" style="background: #000000"></div>#000000|FF|SystemChromeBlackHighColor|SystemChromeBlackHighColorBrush|
|<div class="rect" style="background: #000000"></div>#000000|CC|SystemChromeBlackMediumColor|SystemChromeBlackMediumColor|
|<div class="rect" style="background: #000000"></div>#000000|66|SystemChromeBlackMediumLowColor|SystemChromeBlackMediumLowColorBrush|
|<div class="rect" style="background: #000000"></div>#000000|33|SystemChromeBlackLowColor|SystemChromeBlackLowColorBrush|
|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|33|SystemListMediumColor|SystemListMediumColorBrush|
|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|19|SystemListLowColor|SystemListLowColorBrush|


#### Other Colors
<style type="text/css" rel="styelsheet">
div.rect {
    width: 15px;
    height: 15px;
    display: inline-block;
}
</style> 

|Light|Dark|Color|Brush|
|-----|-----|-----|-----|
|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|<div class="rect" style="background: #FFFFFF"></div>#FFFFFF|SystemChromeWhiteColor|SystemChromeWhiteColorBrush|
|<div class="rect" style="background: #171717"></div>#171717|<div class="rect" style="background: #F2F2F2"></div>#F2F2F2|SystemChromeAltLowColor|SystemChromeAltLowColorBrush|
|<div class="rect" style="background: #CCCCCC"></div>#CCCCCC|<div class="rect" style="background: #333333"></div>#333333|SystemChromeDisabledHighColor|SystemChromeDisabledHighColorBrush|
|<div class="rect" style="background: #7A7A7A"></div>#7A7A7A|<div class="rect" style="background: #858585"></div>#858585|SystemChromeDisabledLowColor|SystemChromeDisabledLowColorBrush|




## License

[MIT License](LICENSE)
