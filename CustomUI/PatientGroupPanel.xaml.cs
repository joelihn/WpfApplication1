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
using WpfApplication1.DAOModule;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for PatientGroupPanel.xaml
    /// </summary>
    public partial class PatientGroupPanel : UserControl
    {

        public MainWindow Basewindow;
        public PatientGroupPanel(MainWindow mainWindow)
        {
            InitializeComponent();
            Basewindow = mainWindow;
            this.ComboBoxPatientGroup.SelectedIndex = MainWindow.ComboBoxPatientGroupIndex;
        }

        private void ComboBoxPatientGroup_OnInitialized(object sender, EventArgs e)
        {
            try
            {
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = patientGroupDao.SelectPatientGroup(condition);
                    foreach (var type in list)
                    {
                        var patientGroupData = new PatientGroupData
                        {
                            Id = type.Id,
                            Name = type.Name,
                            Description = type.Description
                        };
                        ComboBoxPatientGroup.Items.Add(patientGroupData.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:ComboBoxPatientGroup_OnInitialized exception messsage: " + ex.Message);
            }

        }

        private void ComboBoxPatientGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.ComboBoxPatientGroupIndex = this.ComboBoxPatientGroup.SelectedIndex;

        }
    }
}
