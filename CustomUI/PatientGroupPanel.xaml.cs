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
            ListBoxPatient.Items.Clear();
            using (var patientGroupDao = new PatientGroupDao())
            {
                var condition = new Dictionary<string, object>();
                condition["NAME"] = this.ComboBoxPatientGroup.SelectedItem;
                var list = patientGroupDao.SelectPatientGroup(condition);
                if (list.Count > 0)
                {
                    using (var patientGroupParaDao = new PatientGroupParaDao())
                    {
                        var conditionpara = new Dictionary<string, object>();
                        conditionpara["GROUPID"] = list[0].Id;
                        var listpara = patientGroupParaDao.SelectPatientGroupPara(conditionpara);

                        if (listpara.Count > 0)
                        {
                            using (var patientDao = new PatientDao())
                            {
                                var patientlist = patientDao.SelectPatientSpecial(listpara);
                                foreach (var patient in patientlist)
                                {
                                    ListBoxPatient.Items.Add(patient.Name);
                                }
                            }
                        }
                    }
                }
                
            }

        }

        private void ListBoxPatient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }
    }
}
