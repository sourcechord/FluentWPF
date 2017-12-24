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
    /// ParallaxSample.xaml の相互作用ロジック
    /// </summary>
    public partial class ParallaxSample : Window
    {
        public List<string> Items { get; set; }

        public ParallaxSample()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Items = new List<string>();
            for (var i = 0; i < 100; i++)
            {
                this.Items.Add($"item{i:D3}");
            }
        }
    }
}
