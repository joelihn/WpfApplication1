using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ObservableCollection<PatientData> Datalist = new ObservableCollection<PatientData>();
        public MainWindow Basewindow;
        public PatientGroupPanel(MainWindow mainWindow)
        {
            InitializeComponent();
            Basewindow = mainWindow;
            this.ListBoxPatient.ItemsSource = Datalist;
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
            Datalist.Clear();
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
                                    var patientData = new PatientData();
                                    patientData.Id = patient.Id;
                                    patientData.Name = patient.Name;
                                    Datalist.Add(patientData);
                                }
                            }
                        }
                    }
                }
                
            }

        }

        private void ListBoxPatient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            MessageBox.Show(Datalist[this.ListBoxPatient.SelectedIndex].Id.ToString());

        }
    }

    public class PatientData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        public PatientData()
        {
        }

        public Int64 Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
