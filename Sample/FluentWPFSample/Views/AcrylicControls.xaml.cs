using SourceChord.FluentWPF;
using System;
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
using System.Windows.Shapes;

namespace FluentWPFSample.Views
{
    /// <summary>
    /// AcrylicControls.xaml の相互作用ロジック
    /// </summary>
    public partial class AcrylicControls
    {
        public AcrylicControls()
        {
            InitializeComponent();
        }


        private void ShowMessageBox_OK(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "This is MessageBox\nTest", "Title", MessageBoxButton.OK);
        }
        private void ShowMessageBox_OKCancel(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "This is MessageBox\nTest", "Title", MessageBoxButton.OKCancel);
        }
        private void ShowMessageBox_YesNoCancel(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "This is MessageBox\nTest", "Title", MessageBoxButton.YesNoCancel);
        }
        private void ShowMessageBox_YesNo(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "This is MessageBox\nTest", "Title", MessageBoxButton.YesNo);
        }



        private void ShowAcrylicMessageBox_OK(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(this, "This is AcrylicMessageBox\nTest", "Title", MessageBoxButton.OK);
        }
        private void ShowAcrylicMessageBox_OKCancel(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(this, "This is AcrylicMessageBox\nTest", "Title", MessageBoxButton.OKCancel);
        }
        private void ShowAcrylicMessageBox_YesNoCancel(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(this, "This is AcrylicMessageBox\nTest", "Title", MessageBoxButton.YesNoCancel);
        }
        private void ShowAcrylicMessageBox_YesNo(object sender, RoutedEventArgs e)
        {
            var result = AcrylicMessageBox.Show(this, "This is AcrylicMessageBox\nTest", "Title", MessageBoxButton.YesNo);
        }
    }
}
