using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.DAOModule;
using UserControl = System.Windows.Controls.UserControl;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for CInfectType.xaml
    /// </summary>
    public partial class CDataBaseSetting : UserControl
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        
        public CDataBaseSetting()
        {
            InitializeComponent();
            IpAddressTextBox.Text = config.AppSettings.Settings["IpAddress"].Value;
            UserTextBox.Text = config.AppSettings.Settings["Username"].Value;
            PasswordTextBox.Text = config.AppSettings.Settings["Password"].Value;
            DatabaseNameTextBox.Text = config.AppSettings.Settings["Database"].Value;
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            config.AppSettings.Settings["IpAddress"].Value = IpAddressTextBox.Text;
            config.AppSettings.Settings["Username"].Value = UserTextBox.Text;
            config.AppSettings.Settings["Password"].Value = PasswordTextBox.Text;
            config.AppSettings.Settings["Database"].Value = DatabaseNameTextBox.Text;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
        }
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            IpAddressTextBox.Text = config.AppSettings.Settings["IpAddress"].Value;
            UserTextBox.Text = config.AppSettings.Settings["Username"].Value;
            PasswordTextBox.Text = config.AppSettings.Settings["Password"].Value;
            DatabaseNameTextBox.Text = config.AppSettings.Settings["Database"].Value;
            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
        private void PathSelButton_OnClick(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            //if(folderDlg.ShowDialog() == DialogResult.OK)
            //    DataBasePath.Text = folderDlg.SelectedPath;
        }
        
    }

}
