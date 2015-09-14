using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;
using WpfApplication1.Utils;
using ListViewItem = System.Windows.Controls.ListViewItem;
using UserControl = System.Windows.Controls.UserControl;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Init.xaml
    /// </summary>
    public partial class Init : UserControl
    {
        public MainWindow Basewindow;
        public int NewOrEditFlag; //1是新建，2是编辑，3是查看
        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public int selectoperation;
        public Init(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
        }

        private void Init_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    InfectTypeComboBox.Items.Clear();
                    //InfectTypeComboBox.Items.Add("所有");
                    //InfectTypeComboBox.Items.Add("");
                    foreach (InfectType type in list)
                    {
                        InfectTypeComboBox.Items.Add(type.Name);
                    }
                    InfectTypeComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }


            try
            {
                using (var treatStatusDao = new TreatStatusDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = treatStatusDao.SelectTreatStatus(condition);
                    StatusComboBox.Items.Clear();
                    foreach (var type in list)
                    {
                        StatusComboBox.Items.Add(type.Name);
                    }
                    StatusComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded TreatStatus ComboxItem exception messsage: " + ex.Message);
            }

            try
            {
                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    AreaComboBox.Items.Clear();
                    foreach (var type in list)
                    {
                        AreaComboBox.Items.Add(type.Name);
                    }
                    AreaComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded TreatStatus ComboxItem exception messsage: " + ex.Message);
            }

            this.MarriageComboBox.Items.Clear();
            this.MarriageComboBox.Items.Add("未婚");
            this.MarriageComboBox.Items.Add("已婚");
            MarriageComboBox.SelectedIndex = 0;
        }

        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            //SignUP signUpDlg = new SignUP();
            //signUpDlg.ShowDialog();
            //if (signUpDlg.result == DialogResult.OK)
            //{
            //    using (PatientDao patientDao = new PatientDao())
            //    {
            //        Patient patient = new Patient();
            //        patient.Name = signUpDlg.name;
            //        patient.Gender = signUpDlg.sex;
            //        patient.Dob = signUpDlg.birthday;
            //        patient.InfectTypeId = signUpDlg.infectionTypeId;
            //        patient.TreatStatusId = signUpDlg.treatmentStatus;
            //        patient.IsFixedBed = signUpDlg.isFixBed;
            //        patient.PatientId = signUpDlg.uid;
            //        patient.AreaId = signUpDlg.areaId;

            //        int lastInsertId = -1;
            //        patientDao.InsertPatient(patient, ref lastInsertId);

            //        this.IDTextBox.Text = lastInsertId.ToString();
            //        this.NameTextBox.Text = patient.Name;
            //        if (patient.Gender.Equals("男"))
            //        {
            //            this.RadioButton1.IsChecked = true;
            //        }else if (patient.Gender.Equals("女"))
            //        {
            //            this.RadioButton2.IsChecked = true;
            //        }
            //        this.DatePicker1.Text = DateTime.Parse(patient.Dob).ToString();

            //        using (InfectTypeDao infectTypeDao = new InfectTypeDao())
            //        {
            //            var condition = new Dictionary<string, object>();
            //            condition["ID"] = patient.InfectTypeId;
            //            var list = infectTypeDao.SelectInfectType(condition);
            //            if (list != null) this.InfectTypeComboBox.Text = list[0].Name;
            //        }

            //        using (TreatStatusDao treatStatusDao = new TreatStatusDao())
            //        {
            //            var condition = new Dictionary<string, object>();
            //            condition["ID"] = patient.InfectTypeId;
            //            var list = treatStatusDao.SelectTreatStatus(condition);
            //            if (list != null) this.StatusComboBox.Text = list[0].Name;
            //        }

            //        PatientIDTextBox.Text = patient.PatientId;
            //        if (patient.IsFixedBed)
            //        {
            //            this.RadioButton3.IsChecked = true;
            //        }
            //        else
            //        {
            //            this.RadioButton4.IsChecked = true;
            //        }

            //        using (PatientAreaDao patientAreaDao = new PatientAreaDao())
            //        {
            //            var condition = new Dictionary<string, object>();
            //            condition["ID"] = patient.InfectTypeId;
            //            var list = patientAreaDao.SelectPatientArea(condition);
            //            if (list != null) this.AreaComboBox.Text = list[0].Name;
            //        }
            //    }
            //}
            //this.ButtonNew.IsEnabled = true;
            //this.ButtonDelete.IsEnabled = true;
            //this.ButtonApply.IsEnabled = false;
            //this.ButtonCancel.IsEnabled = false;
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            if(Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex==-1) return;

            this.ButtonNew.IsEnabled = true;
            this.ButtonDelete.IsEnabled = true;
            this.ButtonApply.IsEnabled = false;

            using (PatientDao patientDao= new PatientDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Basewindow.patientGroupPanel.Datalist[Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();

                fileds["NAME"] = NameTextBox.Text;

                if ((bool) RadioButton1.IsChecked)
                    fileds["GENDER"] = "男";
                else if ((bool)RadioButton2.IsChecked)
                    fileds["GENDER"] = "女";

                fileds["DOB"] = DatePicker1.Text;
                fileds["NATIONALITY"] = NationalityTextBox.Text;
                 fileds["MARRIAGE"] = MarriageComboBox.Text;
                 fileds["HEIGHT"] = HeightTextBox.Text;
                 fileds["BLOODTYPE"] = BloodTypeTextBox.Text;


                 if ((bool) RadioButton5.IsChecked)
                {
                    fileds["INFECTTYPEID"] = 0;
                }
                 else if ((bool)RadioButton6.IsChecked)
                {
                   
                    using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                    {
                        var condition1 = new Dictionary<string, object>();
                        condition1["NAME"] = InfectTypeComboBox.Text;
                        var list1 = infectTypeDao.SelectInfectType(condition1);
                        if ((list1 != null) && (list1.Count > 0))
                        {
                             fileds["INFECTTYPEID"] =  list1[0].Id;
                        }
                    }
                }

                using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                {
                    var condition1 = new Dictionary<string, object>();
                    condition1["NAME"] = StatusComboBox.Text;
                    var list1 = treatStatusDao.SelectTreatStatus(condition1);
                    if ((list1 != null) && (list1.Count > 0))
                    {
                        fileds["TREATSTATUSID"]  = list1[0].Id;
                    }
                }

                if ((bool) RadioButton3.IsChecked)
                    fileds["ISFIXEDBED"] = true;
                else if ((bool)RadioButton4.IsChecked)
                    fileds["ISFIXEDBED"] = false;

                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    var condition1 = new Dictionary<string, object>();
                    condition1["NAME"] = AreaComboBox.Text;
                    var list1 = patientAreaDao.SelectPatientArea(condition1);
                    if ((list1 != null) && (list1.Count > 0))
                    {
                        fileds["AREAID"] = list1[0].Id;
                    }
                }

                fileds["PATIENTID"] = PatientIDTextBox.Text;
                fileds["MOBILE"] = MobileTextBox.Text;
                fileds["WEIXINHAO"] = WeixinhaoTextBox.Text;
                fileds["PAYMENT"] = PaymentTextBox.Text;
               

                patientDao.UpdatePatient(fileds, condition);
            }
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            //if (ListViewBed.SelectedIndex == -1) return;
            if (Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientDao = new PatientDao())
            {
                patientDao.DeletePatient((int)(Basewindow.patientGroupPanel.Datalist[Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex].Id));
                Basewindow.patientGroupPanel.RemoveData(Basewindow.patientGroupPanel.Datalist[Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex].Id);
            }

            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            int temp = Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex;
            Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex = -1;
            Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex = temp;
        }


        private void RadioButton5_OnChecked(object sender, RoutedEventArgs e)
        {
            this.InfectTypeComboBox.IsEnabled = false;
           
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void RadioButton6_OnChecked(object sender, RoutedEventArgs e)
        {

            if ((bool)RadioButton6.IsChecked)
            {
                this.InfectTypeComboBox.IsEnabled = true;
                
            }
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void RadioButton2_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void DatePicker1_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void StatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void RadioButton3_OnChecked(object sender, RoutedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void RadioButton4_OnChecked(object sender, RoutedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }
    }

    public class PatientInfo : INotifyPropertyChanged
    {
        private Int64 _patientId;
        private string _patientName;
        private string _patientDob;
        private string _patientGender;
        private string _patientNationality;
        private string _patientMarriage;
        private string _patientHeight;
        private string _patientBloodType;
        private string _patientInfectType;
        private string _patientTreatStatus;
        private bool _patientIsFixedBed;
        private Int64 _patientAreaId;
        private string _patientMobile;
        private string _patientWeixinhao;
        private string _patientBedId;
        private bool _patientIsAssigned;
        private string _patientPayment;
        private string _patientRegesiterDate;
        private string _patientDescription;
        private string _patientPatientId;
        private string _patientAge;

        public Int64 PatientId
        {
            get { return _patientId; }
            set
            {
                _patientId = value;
                OnPropertyChanged("PatientId");
            }
        }
        public string PatientName
        {
            get { return _patientName; }
            set
            {
                _patientName = value;
                OnPropertyChanged("PatientName");
            }
        }
        public string PatientDob
        {
            get { return _patientDob; }
            set
            {
                _patientDob = value;
                OnPropertyChanged("PatientDob");
            }
        }
        public string PatientGender
        {
            get { return _patientGender; }
            set
            {
                _patientGender = value;
                OnPropertyChanged("PatientGender");
            }
        }
        public string PatientNationality
        {
            get { return _patientNationality; }
            set
            {
                _patientNationality = value;
                OnPropertyChanged("PatientNationality");
            }
        }
        public string PatientMarriage
        {
            get { return _patientMarriage; }
            set
            {
                _patientMarriage = value;
                OnPropertyChanged("PatientMarriage");
            }
        }
        public string PatientHeight
        {
            get { return _patientHeight; }
            set
            {
                _patientHeight = value;
                OnPropertyChanged("PatientHeight");
            }
        }
        public string PatientBloodType
        {
            get { return _patientBloodType; }
            set
            {
                _patientBloodType = value;
                OnPropertyChanged("PatientBloodType");
            }
        }
        public string PatientInfectType
        {
            get { return _patientInfectType; }
            set
            {
                _patientInfectType = value;
                OnPropertyChanged("PatientInfectType");
            }
        }   
        public string PatientTreatStatus
        {
            get { return _patientTreatStatus; }
            set
            {
                _patientTreatStatus = value;
                OnPropertyChanged("PatientTreatStatus");
            }
        }
        public bool PatientIsFixedBed
        {
            get { return _patientIsFixedBed; }
            set
            {
                _patientIsFixedBed = value;
                OnPropertyChanged("PatientIsFixedBed");
            }
        }
        public Int64 PatientAreaId
        {
            get { return _patientAreaId; }
            set
            {
                _patientAreaId = value;
                OnPropertyChanged("PatientAreaId");
            }
        }
        public string PatientAge
        {
            get
            {
                try
                {
                    int now = DateTime.Now.Date.Year;
                    int birth = DateTime.Parse(PatientDob).Year;

                    int age = now - birth;

                    return age.ToString();
                }
                catch (Exception)
                {

                    return "0";
                }

            }

        }

        public string PatientMobile
        {
            get { return _patientMobile; }
            set
            {
                _patientMobile = value;
                OnPropertyChanged("PatientMobile");
            }
        }
        public string PatientWeixinhao
        {
            get { return _patientWeixinhao; }
            set
            {
                _patientWeixinhao = value;
                OnPropertyChanged("PatientWeixinhao");
            }
        }

        public string PatientPayment
        {
            get { return _patientPayment; }
            set
            {
                _patientPayment = value;
                OnPropertyChanged("PatientPayment");
            }
        }

        public string PatientRegesiterDate
        {
            get { return _patientRegesiterDate; }
            set
            {
                _patientRegesiterDate = value;
                OnPropertyChanged("PatientRegesiterDate");
            }
        }

     

      
        public string PatientBedId
        {
            get { return _patientBedId; }
            set
            {
                _patientBedId = value;
                OnPropertyChanged("PatientBedId");
            }
        }
        public bool PatientIsAssigned
        {
            get { return _patientIsAssigned; }
            set
            {
                _patientIsAssigned = value;
                OnPropertyChanged("PatientIsAssigned");
            }
        }

        public string PatientDescription
        {
            get { return _patientDescription; }
            set
            {
                _patientDescription = value;
                OnPropertyChanged("PatientDescription");
            }
        }

        public string PatientPatientId
        {
            get { return _patientPatientId; }
            set
            {
                _patientPatientId = value;
                OnPropertyChanged("PatientPatientId");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
