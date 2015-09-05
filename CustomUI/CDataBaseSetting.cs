using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            DataBasePath.Text = config.AppSettings.Settings["DataBasePath"].Value;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            config.AppSettings.Settings["DataBasePath"].Value = DataBasePath.Text;
        }
        private void PathSelButton_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            if(folderDlg.ShowDialog() == DialogResult.OK)
                DataBasePath.Text = folderDlg.SelectedPath;
        }
        
    }

}
