using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WSClientWPF_sample
{
    /// <summary>
    /// Window0.xaml の相互作用ロジック
    /// </summary>
    public partial class Window0 : Window
    {
        public Window0()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window1();
            w.Show();
        }
    }
}
