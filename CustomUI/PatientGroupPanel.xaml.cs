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

        private void UpdateGroupCount()
        {
            LabelCount.Content = "总共" + Datalist.Count + "人";
        }
        public void RefreshPatientGroupCombobox()
        {
            try
            {
                ComboBoxPatientGroup.Items.Clear();
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
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:RefreshPatientGroupCombobox exception messsage: " + ex.Message);
            }
        }

        private void ComboBoxPatientGroup_OnInitialized(object sender, EventArgs e)
        {
            try
            {
                ComboBoxPatientGroup.Items.Clear();
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
            UpdateGroupCount();
            //ListBoxPatient.SelectedIndex = Datalist.Count > 0 ? 1 : -1;

        }

        public void RefreshData()
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
                if (Datalist.Count > 0)
                {
                    this.ListBoxPatient.SelectedIndex = 0;
                }

            }
        }

        private void ListBoxPatient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListBoxPatient.SelectedIndex != -1)
            {
                using (PatientDao patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[this.ListBoxPatient.SelectedIndex].Id;
                    var list = patientDao.SelectPatient(condition);
                    if ((list != null) && (list.Count > 0))
                    {
                        Patient patient = list[0];
                        Basewindow.initContent.IDTextBox.Text = patient.PatientId.ToString();
                        Basewindow.initContent.NameTextBox.Text = patient.Name;
                        if (patient.Gender.Equals("男"))
                            Basewindow.initContent.RadioButton1.IsChecked = true;
                        else if(patient.Gender.Equals("女"))
                            Basewindow.initContent.RadioButton2.IsChecked = true;
                        try
                        {
                            Basewindow.initContent.DatePicker1.Text = DateTime.Parse(patient.Dob).ToString();
                        }
                        catch (Exception)
                        {

                            Basewindow.initContent.DatePicker1.Text = "";
                        }
                       
                        Basewindow.initContent.NationalityTextBox.Text = patient.Nationality;

                        if (patient.Gender.Equals("未婚"))
                            Basewindow.initContent.MarriageComboBox.SelectedIndex = 0;
                        else if (patient.Gender.Equals("已婚"))
                            Basewindow.initContent.MarriageComboBox.SelectedIndex = 1;

                        Basewindow.initContent.HeightTextBox.Text = patient.Height;
                        Basewindow.initContent.BloodTypeTextBox.Text = patient.BloodType;

                        if (patient.InfectTypeId == 0)
                        {
                            Basewindow.initContent.RadioButton5.IsChecked = true;
                        }
                        else
                        {
                            Basewindow.initContent.RadioButton6.IsChecked = true;
                            using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                            {
                                var condition1 = new Dictionary<string, object>();
                                condition1["ID"] = patient.InfectTypeId;
                                var list1 = infectTypeDao.SelectInfectType(condition1);
                                if ((list1 != null) && (list1.Count > 0))
                                {
                                    Basewindow.initContent.InfectTypeComboBox.Text = list1[0].Name;
                                }
                            }
                        }
                        if (patient.TreatStatusId == 0)
                        {
                            Basewindow.initContent.rbTreatStatus1.IsChecked = true;
                        }
                        else
                        {
                            Basewindow.initContent.rbTreatStatus2.IsChecked = true;
                            using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                            {
                                var condition1 = new Dictionary<string, object>();
                                condition1["ID"] = patient.TreatStatusId;
                                var list1 = treatStatusDao.SelectTreatStatus(condition1);
                                if ((list1 != null) && (list1.Count > 0))
                                {
                                    Basewindow.initContent.StatusComboBox.Text = list1[0].Name;
                                }
                            }
                        }

                        if (patient.IsFixedBed)
                            Basewindow.initContent.RadioButton3.IsChecked = true;
                        else 
                            Basewindow.initContent.RadioButton4.IsChecked = true;

                        using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                        {
                            var condition1 = new Dictionary<string, object>();
                            condition1["ID"] = patient.AreaId;
                            var list1 = patientAreaDao.SelectPatientArea(condition1);
                            if ((list1 != null) && (list1.Count > 0))
                            {
                                Basewindow.initContent.AreaComboBox.Text = list1[0].Name;
                            }
                        }

                        Basewindow.initContent.PatientIDTextBox.Text = patient.Id.ToString();
                        Basewindow.initContent.MobileTextBox.Text = patient.Mobile;
                        Basewindow.initContent.WeixinhaoTextBox.Text = patient.WeiXinHao;
                        Basewindow.initContent.PaymentTextBox.Text = patient.Payment;

                        Basewindow.initContent.ButtonApply.IsEnabled = false;
                        Basewindow.initContent.ButtonCancel.IsEnabled = false;
                        Basewindow.initContent.ButtonDelete.IsEnabled = true;

                        #region orderContent
                        Basewindow.orderContent.IDTextBox.Text = patient.PatientId.ToString();
                        Basewindow.orderContent.NameTextBox.Text = patient.Name;
                        if (patient.Gender.Equals("男"))
                            Basewindow.orderContent.RadioButton1.IsChecked = true;
                        else if (patient.Gender.Equals("女"))
                            Basewindow.orderContent.RadioButton2.IsChecked = true;
                        try
                        {
                            Basewindow.orderContent.DatePicker1.Text = DateTime.Parse(patient.Dob).ToString();
                        }
                        catch (Exception)
                        {

                            Basewindow.orderContent.DatePicker1.Text = "";
                        }

                        Basewindow.orderContent.NationalityTextBox.Text = patient.Nationality;

                        if (patient.Gender.Equals("未婚"))
                            Basewindow.orderContent.MarriageComboBox.SelectedIndex = 0;
                        else if (patient.Gender.Equals("已婚"))
                            Basewindow.orderContent.MarriageComboBox.SelectedIndex = 1;

                        Basewindow.orderContent.HeightTextBox.Text = patient.Height;
                        Basewindow.orderContent.BloodTypeTextBox.Text = patient.BloodType;

                        if (patient.InfectTypeId == 0)
                        {
                            Basewindow.orderContent.RadioButton5.IsChecked = true;
                        }
                        else
                        {
                            Basewindow.orderContent.RadioButton6.IsChecked = true;
                            using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                            {
                                var condition1 = new Dictionary<string, object>();
                                condition1["ID"] = patient.InfectTypeId;
                                var list1 = infectTypeDao.SelectInfectType(condition1);
                                if ((list1 != null) && (list1.Count > 0))
                                {
                                    Basewindow.orderContent.InfectTypeComboBox.Text = list1[0].Name;
                                }
                            }
                        }

                        using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                        {
                            var condition1 = new Dictionary<string, object>();
                            condition1["ID"] = patient.TreatStatusId;
                            var list1 = treatStatusDao.SelectTreatStatus(condition1);
                            if ((list1 != null) && (list1.Count > 0))
                            {
                                Basewindow.orderContent.StatusComboBox.Text = list1[0].Name;
                            }
                        }

                        if (patient.IsFixedBed)
                            Basewindow.orderContent.RadioButton3.IsChecked = true;
                        else
                            Basewindow.orderContent.RadioButton4.IsChecked = true;

                        using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                        {
                            var condition1 = new Dictionary<string, object>();
                            condition1["ID"] = patient.AreaId;
                            var list1 = patientAreaDao.SelectPatientArea(condition1);
                            if ((list1 != null) && (list1.Count > 0))
                            {
                                Basewindow.orderContent.AreaComboBox.Text = list1[0].Name;
                            }
                        }
                        Basewindow.orderContent.RefreshData();

                        Basewindow.orderContent.PatientIDTextBox.Text = patient.PatientId;
                        Basewindow.orderContent.MobileTextBox.Text = patient.Mobile;
                        Basewindow.orderContent.WeixinhaoTextBox.Text = patient.WeiXinHao;
                        Basewindow.orderContent.PaymentTextBox.Text = patient.Payment;
                        #endregion
                    }
                }
            }

        }

        public void RemoveData(long id)
        {
            try
            {
                foreach (PatientData patientData in Datalist)
                {
                    if (patientData.Id == id)
                    {
                        Datalist.Remove(patientData);
                        return;
                    }
                    
                }
                this.ListBoxPatient.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        public void reLoaded()
        {

            try
            {
                ComboBoxPatientGroup.Items.Clear();
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
                    ComboBoxPatientGroup.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:PatientGroupPanel_OnLoaded exception messsage: " + ex.Message);
            }
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
