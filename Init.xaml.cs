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
    /// Interaction logic for Init.xaml
    /// </summary>
    public partial class Init : UserControl
    {
        public MainWindow Basewindow;
        public Init(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
        }
    }
}
