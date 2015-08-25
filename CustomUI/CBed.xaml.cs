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
    /// Interaction logic for BedUnit.xaml
    /// </summary>
    public partial class CBed : UserControl
    {
        public ObservableCollection<BedData> Datalist = new ObservableCollection<BedData>();

        private bool isNew = false;
        private int currnetIndex = -1;

        public CBed()
        {
            InitializeComponent();
            this.ListViewBed.ItemsSource = Datalist;
        }

        private void ListViewCBed_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            #region refresh data list
            try
            {
                using (var bedDao = new BedDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = bedDao.SelectBed(condition);
                    foreach (var pa in list)
                    {
                        var bedData = new BedData();
                        bedData.Id = pa.Id;
                        bedData.Name = pa.Name;
                        {
                            using (var patientAreaDao = new PatientAreaDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientAreaId;
                                var arealist = patientAreaDao.SelectPatientArea(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.PatientArea = arealist[0].Name;
                                }
                            }

                        }

                        {
                            using (var machineTypeDao = new MachineTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientAreaId;
                                var arealist = machineTypeDao.SelectMachineType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.MachineType = arealist[0].Name;
                                }
                            }

                        }

                        {
                            using (var treatMethodDao = new TreatTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.TreatTypeId;
                                var arealist = treatMethodDao.SelectTreatType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.TreatType = arealist[0].Name;
                                }
                            }
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.InfectType = arealist[0].Name;
                                }
                            }
                        }
                        bedData.IsAvailable = pa.IsAvailable;
                        bedData.IsOccupy = pa.IsOccupy;
                        bedData.IsTemp = pa.IsTemp;
                        bedData.Description = pa.Description;
                        Datalist.Add(bedData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CBed.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
            #endregion
        }

        private void ListViewCBed_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewBed.SelectedIndex >= 0)
            {
                currnetIndex = ListViewBed.SelectedIndex;
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;

                isNew = false;

                NameTextBox.Text = Datalist[ListViewBed.SelectedIndex].Name;
                ComboBoxPatientArea.Text = Datalist[ListViewBed.SelectedIndex].PatientArea;
                MachineTypeComboBox.Text = Datalist[ListViewBed.SelectedIndex].MachineType;
                if(Datalist[ListViewBed.SelectedIndex].IsAvailable)
                    IsAvilComboBox.Text = "可用";
                else
                    IsAvilComboBox.Text = "不可用";
                if (Datalist[ListViewBed.SelectedIndex].IsTemp)
                    IsTempComboBox.Text = "是";
                else
                    IsTempComboBox.Text = "否";
                DescriptionTextBox.Text = Datalist[ListViewBed.SelectedIndex].Description;
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new BedDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectBed(condition);
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

        private void RefreshData()
        {
            try
            {
                using (var bedDao = new BedDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = bedDao.SelectBed(condition);
                    foreach (DAOModule.Bed pa in list)
                    {
                        var bedData = new BedData();
                        bedData.Id = pa.Id;
                        bedData.Name = pa.Name;
                        {
                           /* using (var patientRoomDao = new PatientRoomDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientRoomId;
                                var arealist = patientRoomDao.SelectPatientRoom(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.PatientRoom = arealist[0].Name;
                                }
                            }*/

                            using (var patientAreaDao = new PatientAreaDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientAreaId;
                                var arealist = patientAreaDao.SelectPatientArea(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.PatientArea = arealist[0].Name;
                                }
                            }

                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.InfectType = arealist[0].Name;
                                }
                            }

                        }
                        {
                            using (var treatMethodDao = new TreatTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.TreatTypeId;
                                var arealist = treatMethodDao.SelectTreatType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedData.TreatType = arealist[0].Name;
                                }
                            }
                        }
                        bedData.IsAvailable = pa.IsAvailable;
                        bedData.IsOccupy = pa.IsOccupy;
                        bedData.Description = pa.Description;
                        Datalist.Add(bedData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CBed.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void CBed_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();


            #region fill infecttype combox items
            this.MachineTypeComboBox.Items.Clear();
            try
            {
                using (var machineTypeDao = new MachineTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = machineTypeDao.SelectMachineType(condition);
                    foreach (var pa in list)
                    {
                        //if(pa.IsAvailable == true )
                        this.MachineTypeComboBox.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.MachineTypeComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CBed.xaml.cs:ListViewCPatientRoom_OnLoaded 2 exception messsage: " + ex.Message);
            }
            #endregion

            #region fill patientarea combox items
            this.ComboBoxPatientArea.Items.Clear();
            try
            {
                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        this.ComboBoxPatientArea.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.ComboBoxPatientArea.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:ListViewCPatientRoom_OnLoaded 1 exception messsage: " + ex.Message);
            }
            #endregion

            #region fill infecttype combox items
            /*this.ComboBoxInfectType.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var pa in list)
                    {
                        this.ComboBoxInfectType.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.ComboBoxInfectType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:ListViewCPatientRoom_OnLoaded 2 exception messsage: " + ex.Message);
            }*/
            #endregion
        }

        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            isNew = true;
            NameTextBox.Text = "";
            ComboBoxPatientArea.Text = "";
            DescriptionTextBox.Text = "";
            MachineTypeComboBox.Text = "";
            IsAvilComboBox.Text = "";
            IsTempComboBox.Text = "";

            this.ButtonNew.IsEnabled = false;
            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                try
                {
                    if (this.NameTextBox.Text.Equals("") || !CheckNameIsExist(this.NameTextBox.Text))
                    {
                        var a = new RemindMessageBox1();
                        a.remindText.Text = (string)FindResource("Message1001"); ;
                        a.ShowDialog();
                        return;
                    }

                    using (var bedDao = new BedDao())
                    {
                        var bed = new DAOModule.Bed();
                        bed.Name = this.NameTextBox.Text;

                        var condition = new Dictionary<string, object>();
                        using (var machianTypeDao = new MachineTypeDao())
                        {
                            condition.Clear();
                            condition["Name"] = MachineTypeComboBox.Text;
                            var arealist = machianTypeDao.SelectMachineType(condition);
                            if (arealist.Count == 1)
                            {
                                bed.MachineTypeId = arealist[0].Id;
                            }
                        }
                        using (var patientAreaDao = new PatientAreaDao())
                        {
                            condition.Clear();
                            condition["Name"] = ComboBoxPatientArea.Text;
                            var arealist = patientAreaDao.SelectPatientArea(condition);
                            if (arealist.Count == 1)
                            {
                                bed.PatientAreaId = arealist[0].Id;
                            }
                        }

                        if (IsAvilComboBox.Text.Equals("可用"))
                            bed.IsAvailable = true;
                        else if (IsAvilComboBox.Text.Equals("不可用"))
                            bed.IsAvailable = false;

                        if (IsTempComboBox.Text.Equals("是"))
                            bed.IsTemp = true;
                        else if (IsTempComboBox.Text.Equals("否"))
                            bed.IsTemp = false;

                        bed.IsOccupy = false;
                        bed.Description = this.DescriptionTextBox.Text;
                        int lastInsertId = -1;
                        bedDao.InsertBed(bed, ref lastInsertId);
                        //UI
                        BedData bedData = new BedData();
                        bedData.Id = lastInsertId;
                        bedData.Name = bed.Name;
                        bedData.PatientArea = ComboBoxPatientArea.Text;
                        bedData.TreatType = "";
                        bedData.IsTemp = bed.IsTemp;
                        bedData.MachineType = MachineTypeComboBox.Text;
                        bedData.InfectType = "";
                        bedData.IsAvailable = bed.IsAvailable;
                        bedData.IsOccupy = bed.IsOccupy;
                        bedData.Description = bed.Description;
                        Datalist.Add(bedData);
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In CBed.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
                }
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = true;
                this.ButtonCancel.IsEnabled = true;
            }
            else
            {
                if (ListViewBed.SelectedIndex == -1) return;

                if (this.NameTextBox.Text.Equals(""))
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1001"); ;
                    a.ShowDialog();
                    return;
                }
                //throw new NotImplementedException();
                using (var bedDao = new BedDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[ListViewBed.SelectedIndex].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["NAME"] = NameTextBox.Text;

                    var condition2 = new Dictionary<string, object>();
                    
                    using (var machianTypeDao = new MachineTypeDao())
                    {
                        condition.Clear();
                        condition["Name"] = MachineTypeComboBox.Text;
                        var arealist = machianTypeDao.SelectMachineType(condition);
                        if (arealist.Count == 1)
                        {
                            fileds["MachineTypeId"] = arealist[0].Id;
                        }
                    }

                    using (var patientAreaDao = new PatientAreaDao())
                    {
                        condition2.Clear();
                        condition2["Name"] = ComboBoxPatientArea.Text;
                        var arealist = patientAreaDao.SelectPatientArea(condition2);
                        if (arealist.Count == 1)
                        {
                            fileds["PATIENTAREAID"] = arealist[0].Id;
                        }
                    }
                    if (IsAvilComboBox.Text.Equals("可用"))
                        fileds["ISAVAILABLE"] = true;
                    else if (IsAvilComboBox.Text.Equals("不可用"))
                        fileds["ISAVAILABLE"] = false;

                    if (IsTempComboBox.Text.Equals("是"))
                        fileds["ISTEMP"] = true;
                    else if (IsTempComboBox.Text.Equals("否"))
                        fileds["ISTEMP"] = false;

                    //fileds["ISOCCUPY"] = CheckBoxIsOccupy.IsChecked;
                    fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                    bedDao.UpdateBed(fileds, condition);
                    if (IsAvilComboBox.Text.Equals("不可用"))
                    {
                        using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                        {
                            scheduleDao.UpdateScheduleTemplate1("-1", Datalist[ListViewBed.SelectedIndex].Id.ToString(), DateTime.Now.Date);
                        }
                    }
                    RefreshData();
                }
                isNew = false;
            }
            this.ButtonApply.IsEnabled = false;
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewBed.SelectedIndex == -1) return;
            //throw new NotImplementedException();
            using (var bedDao = new BedDao())
            {
                bedDao.DeleteBed(Datalist[ListViewBed.SelectedIndex].Id);

                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    scheduleDao.UpdateScheduleTemplate1("-1", Datalist[ListViewBed.SelectedIndex].Id.ToString(), DateTime.Now.Date);
                }
                RefreshData();
            }

            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
            isNew = false;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                NameTextBox.Text = "";
                ComboBoxPatientArea.Text = "";
                DescriptionTextBox.Text = "";
                MachineTypeComboBox.Text = "";
                IsAvilComboBox.Text = "";
                IsTempComboBox.Text = "";

                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = false;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;
                this.ListViewBed.SelectedIndex = -1;
                this.ListViewBed.SelectedIndex = currnetIndex;
            }
            else
            {
                this.ListViewBed.SelectedIndex = -1;
                this.ListViewBed.SelectedIndex = currnetIndex;
            }

        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            

        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }
    }

    public class BedData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _treatType;
        private bool _isAvailable;
        private bool _isOccupy;
        private bool _isTemp;
        private string _description;
        private string _patientArea;
        private string _machineType;
        private string _infecttype;

        public BedData()
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

        /*public string PatientRoom
        {
            get { return _patientRoom; }
            set
            {
                _patientRoom = value;
                OnPropertyChanged("PatientRoom");
            }
        }*/

        public string TreatType
        {
            get { return _treatType; }
            set
            {
                _treatType = value;
                OnPropertyChanged("TreatType");
            }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
            }
        }

        public bool IsOccupy
        {
            get { return _isOccupy; }
            set
            {
                _isOccupy = value;
                OnPropertyChanged("Occupy");
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
        public string PatientArea
        {
            get { return _patientArea; }
            set
            {
                _patientArea = value;
                OnPropertyChanged("PatientArea");
            }
        }

        public string MachineType
        {
            get { return _machineType; }
            set
            {
                _machineType = value;
                OnPropertyChanged("MachineType");
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

        public bool IsTemp
        {
            get { return _isTemp; }
            set
            {
                _isTemp = value;
                OnPropertyChanged("IsTemp");
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
