﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FluentWPFSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenAcrylicWindow(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicWindow();
            win.ShowDialog();
        }

        private void OpenAcrylicWindow2(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicWindow2();
            win.ShowDialog();
        }

        private void OpenAcrylicWindow3(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicWindow3();
            win.ShowDialog();
        }

        private void OpenAcrylicWindow4(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicWindow4();
            win.ShowDialog();
        }

        private void OpenAcrylicPanel(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicPanelSample();
            win.ShowDialog();
        }
        private void OpenAcrylicPanel2(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicPanelSample2();
            win.ShowDialog();
        }

        private void OpenAcrylicBrush(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicBrushSample();
            win.ShowDialog();
        }

        private void OpenAcrylicControls(object sender, RoutedEventArgs e)
        {
            var win = new Views.AcrylicControls();
            win.ShowDialog();
        }

        private void OpenReveal(object sender, RoutedEventArgs e)
        {
            var win = new Views.RevealStyles();
            win.ShowDialog();
        }

        private void OpenParallax(object sender, RoutedEventArgs e)
        {
            var win = new Views.ParallaxSample();
            win.ShowDialog();
        }

        private void OpenAccentColors(object sender, RoutedEventArgs e)
        {
            var win = new Views.AccentColorsSample();
            win.ShowDialog();
        }

        private void OpenLogo(object sender, RoutedEventArgs e)
        {
            var win = new Views.Logo();
            win.ShowDialog();
        }

        private void OpenControls(object sender, RoutedEventArgs e)
        {
            var win = new Views.Controls();
            win.ShowDialog();
        }

        private void OpenDropShadowPanel(object sender, RoutedEventArgs e)
        {
            var win = new Views.DropShadowPanelSample();
            win.ShowDialog();
        }

        private void OpenDropShadowPanel2(object sender, RoutedEventArgs e)
        {
            var win = new Views.DropShadowPanelSample2();
            win.ShowDialog();
        }

        private void OpenCalculatorSample(object sender, RoutedEventArgs e)
        {
            var win = new Views.Calculator();
            win.ShowDialog();
        }

        private void OpenMusicPlayerSample(object sender, RoutedEventArgs e)
        {
            var win = new Views.MusicPlayer();
            win.ShowDialog();
        }
    }
}
