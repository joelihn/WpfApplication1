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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for SignUP.xaml
    /// </summary>
    public partial class SignUP : Window
    {
        public string name;
        public string sex;
        public string birthday;
        public string infectionType;
        public string treatmentStatus;
        public bool isFixBed;
        public string uid;
        public string area;
        public SignUP()
        {
            InitializeComponent();
        }
    }
}
