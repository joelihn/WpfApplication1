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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Init initContent;
        private Order orderContent;
        private Bed bedContent;
        private Config configContent;

        public MainWindow()
        {
            InitializeComponent();
            initContent = new Init(this);
            orderContent = new Order(this);
            bedContent = new Bed(this);
            configContent = new Config(this);
        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = initContent;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = orderContent;
        }

        private void BedButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = bedContent;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = configContent;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
