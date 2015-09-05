using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RadioButton = System.Windows.Controls.RadioButton;

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
        public DialogResult result;
        public SignUP()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            result = System.Windows.Forms.DialogResult.Cancel;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            result = System.Windows.Forms.DialogResult.OK;

            name = tbName.Text;
            if (rbM.IsChecked == true)
            {
                sex = "男";
            }
            else
            {
                sex = "女";
            }
            birthday = dpBirthday.Text;

            if (rbFixBed.IsChecked == true)
            {
                isFixBed = true;
            }
            else
            {
                isFixBed = false;
            }

            uid = tbUid.Text;
            area = cbArea.Text;



        }

        private void Window_Closed(object sender, EventArgs e)
        {
            result = System.Windows.Forms.DialogResult.Cancel;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (cbInfectionType == null) 
                return;
            RadioButton btn = (RadioButton)sender;
            if ((string)btn.Tag == "0")
            {
                cbInfectionType.IsEnabled = false;
               
            }
            else
            {
                cbInfectionType.IsEnabled = true;
            }
        }

        private void rbTreatStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (cbTreatMentStatus == null) 
                return; 
            RadioButton btn = (RadioButton)sender;
            if ((string)btn.Tag == "0")
            {
                cbTreatMentStatus.IsEnabled = false;
            }
            else
            {
                cbTreatMentStatus.IsEnabled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpBirthday.Text = DateTime.Now.ToString();
        }
    }
}
