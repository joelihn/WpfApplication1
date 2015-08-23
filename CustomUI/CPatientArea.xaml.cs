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
    /// Interaction logic for PatientArea.xaml
    /// </summary>
    public partial class CPatientArea : UserControl
    {
        public ObservableCollection<PatientAreaData> Datalist = new ObservableCollection<PatientAreaData>();
        public ObservableCollection<InfectTypeData> Datalist1 = new ObservableCollection<InfectTypeData>();

        public CPatientArea()
        {
            InitializeComponent();

            this.ListViewPatientArea.ItemsSource = Datalist;
            this.PationAreaInfos.ItemsSource = Datalist;
            //this.TypeComboBox.Items.Add("阴性");
            //this.TypeComboBox.Items.Add("阳性");
            this.TypeComboBox.Text = "阴性";
           
        }

        private void ListViewCPatientArea_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        PatientAreaData patientAreaData = new PatientAreaData();
                        patientAreaData.Id = pa.Id;
                        patientAreaData.Name = pa.Name;
                        patientAreaData.Type = pa.Type;

                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientAreaData.InfectionType = arealist[0].Name;
                                }
                            }
                        }

                        patientAreaData.Description = pa.Description;
                        Datalist.Add(patientAreaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
            InfectionComboBox.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    //Datalist1.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var type in list)
                    {
                        InfectionComboBox.Items.Add(type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }

        }

        private void RefreshData()
        {
            try
            {
                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        PatientAreaData patientAreaData = new PatientAreaData();
                        patientAreaData.Id = pa.Id;
                        patientAreaData.Name = pa.Name;
                        patientAreaData.Type = pa.Type;
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientAreaData.InfectionType = arealist[0].Name;
                                }
                            }
                        }
                        patientAreaData.Description = pa.Description;
                        Datalist.Add(patientAreaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }


        private void ListViewCPatientArea_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewPatientArea.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Name;
                TypeComboBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Type;
                InfectionComboBox.Text = Datalist[ListViewPatientArea.SelectedIndex].InfectionType;
                DescriptionTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Description;
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new PatientAreaDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectPatientArea(condition);
                foreach (var pa in list)
                {
                    if (name.Equals(pa.Name))
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                if (this.NameTextBox.Text.Equals("") || !CheckNameIsExist(this.NameTextBox.Text))
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1001"); ;
                    a.ShowDialog();
                    return;
                }

                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    PatientArea patientArea = new PatientArea();
                    patientArea.Name = this.NameTextBox.Text;
                    patientArea.Type = this.TypeComboBox.Text;
                    var condition = new Dictionary<string, object>();
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        condition.Clear();
                        condition["Name"] = InfectionComboBox.Text;
                        var arealist = infectTypeDao.SelectInfectType(condition);
                        if (arealist.Count == 1)
                        {
                            patientArea.InfectTypeId = arealist[0].Id;
                        }
                        else
                        {
                            patientArea.InfectTypeId = 1;
                        }
                    }
                    patientArea.Description = this.DescriptionTextBox.Text;
                    int lastInsertId = -1;
                    patientAreaDao.InsertPatientArea(patientArea, ref lastInsertId);
                    //UI
                    PatientAreaData patientAreaData = new PatientAreaData();
                    patientAreaData.Name = patientArea.Name;
                    patientAreaData.Type = patientArea.Type;
                    patientAreaData.InfectionType = InfectionComboBox.Text;
                    patientAreaData.Description = patientArea.Description;
                    Datalist.Add(patientAreaData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }

        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientArea.SelectedIndex == -1) return;

            if (this.NameTextBox.Text.Equals("") )
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message1001"); ;
                a.ShowDialog();
                return;
            }

            //throw new NotImplementedException();
            using (var patientAreaDao = new PatientAreaDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListViewPatientArea.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                fileds["TYPE"] = TypeComboBox.Text;
                var condition2 = new Dictionary<string, object>();
                using (var infectTypeDao = new InfectTypeDao())
                {
                    condition2.Clear();
                    condition2["Name"] = InfectionComboBox.Text;
                    var arealist = infectTypeDao.SelectInfectType(condition2);
                    if (arealist.Count == 1)
                    {
                        fileds["INFECTTYPEID"] = arealist[0].Id;
                    }
                    else
                    {
                        fileds["INFECTTYPEID"] = 1;
                    }
                }
                fileds["DESCRIPTION"] =DescriptionTextBox.Text;
                patientAreaDao.UpdatePatientArea(fileds,condition);
                RefreshData();
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientArea.SelectedIndex == -1) return;
            //throw new NotImplementedException();
            using (var patientAreaDao = new PatientAreaDao())
            {
                patientAreaDao.DeletePatientArea(Datalist[ListViewPatientArea.SelectedIndex].Id);
                RefreshData();
            }
        }

        private void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)this.TypeComboBox.SelectedItem;
            if (cbi.Content.Equals("阴性"))
            {
                this.InfectionComboBox.Text = "阴性";
                this.InfectionComboBox.IsEnabled = false;
            }
            else
            {
                this.InfectionComboBox.IsEnabled = true;
                this.InfectionComboBox.SelectedIndex = 1;
            }

            
            //var mSystemConfig = new ConfigModule.SystemConfig();
            //mSystemConfig.Write("RegistrationManner", "Enable", cbi.Content.ToString());
            //ConstDefinition.RegistrationManner = cbi.Content.ToString();
       

        }

        private void CPatientArea_OnLoaded(object sender, RoutedEventArgs e)
        {
            #region fill infecttype combox items
            this.InfectionComboBox.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var pa in list)
                    {
                        this.InfectionComboBox.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.InfectionComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:ListViewCPatientRoom_OnLoaded 2 exception messsage: " + ex.Message);
            }
            #endregion
        }

        
    }

    public class PatientAreaData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _type;   
        private string _infecttype;
        private string _description;
        private string _infectionType;

        public PatientAreaData()
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

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public string InfectionType
        {
            get { return _infectionType; }
            set
            {
                _infectionType = value;
                OnPropertyChanged("InfectionType");
            }
        }
        public string InfectType
        {
            get { return _infecttype; }
            set
            {
                _infecttype = value;
                OnPropertyChanged("InfectType");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
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
