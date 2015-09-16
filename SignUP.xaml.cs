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
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;
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
        public Int64 infectionTypeId;
        public Int64 treatmentStatus;
        public bool isFixBed;
        public string uid;
        public Int64 areaId;
        //public DialogResult result;

        public MainWindow Basewindow;

        public SignUP(MainWindow mainWindow)
        {
            InitializeComponent();
            Basewindow = mainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.Equals("") || tbUid.Text.Equals(""))
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = "患者名称和唯一识别码不能为空.";
                a.ShowDialog();
                return;
            }

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

            if ((bool)this.rbNegative1.IsChecked)
            {
                infectionTypeId = 0;
            }
            else if ((bool)this.rbNegative2.IsChecked)
            {
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["Name"] = InfectTypeComboBox.Text;
                    var list = infectTypeDao.SelectInfectType(condition);
                    if (list != null) infectionTypeId = list[0].Id;
                }
            }

            if ((bool)this.rbTreatStatus1.IsChecked)
            {
                treatmentStatus = 0;
            }
            else if ((bool)this.rbTreatStatus2.IsChecked)
            {
                using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["Name"] = StatusComboBox.Text;
                    var list = treatStatusDao.SelectTreatStatus(condition);
                    if (list != null) treatmentStatus = list[0].Id;
                }
            }

            if ((bool)this.rbFixBed1.IsChecked)
            {
                isFixBed = true;
            }
            else if ((bool)this.rbFixBed2.IsChecked)
            {
                isFixBed = false;
            }

            uid = tbUid.Text;

            using (PatientAreaDao patientAreaDao = new PatientAreaDao())
            {
                var condition = new Dictionary<string, object>();
                condition["Name"] = AreaComboBox.Text;
                var list = patientAreaDao.SelectPatientArea(condition);
                if (list != null) areaId = list[0].Id;
            }

            //result = System.Windows.Forms.DialogResult.OK;

            using (PatientDao patientDao = new PatientDao())
            {
                Patient patient = new Patient();
                patient.Name = name;
                patient.Gender = sex;
                patient.Dob = birthday;
                patient.InfectTypeId = infectionTypeId;
                patient.TreatStatusId = treatmentStatus;
                patient.IsFixedBed = isFixBed;
                patient.PatientId = uid;
                patient.AreaId = areaId;

                int lastInsertId = -1;
                patientDao.InsertPatient(patient, ref lastInsertId);

                InitMedicalOrderData(lastInsertId);

                Basewindow.initContent.IDTextBox.Text = lastInsertId.ToString();
                Basewindow.initContent.NameTextBox.Text = patient.Name;
                if (patient.Gender.Equals("男"))
                {
                    Basewindow.initContent.RadioButton1.IsChecked = true;
                }
                else if (patient.Gender.Equals("女"))
                {
                    Basewindow.initContent.RadioButton2.IsChecked = true;
                }
                Basewindow.initContent.DatePicker1.Text = DateTime.Parse(patient.Dob).ToString();

                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patient.InfectTypeId;
                    var list = infectTypeDao.SelectInfectType(condition);
                    if ((list != null) && (list.Count > 0)) Basewindow.initContent.InfectTypeComboBox.Text = list[0].Name;
                }

                using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patient.TreatStatusId;
                    var list = treatStatusDao.SelectTreatStatus(condition);
                    if ((list != null) && (list.Count > 0)) Basewindow.initContent.StatusComboBox.Text = list[0].Name;
                }

                Basewindow.initContent.PatientIDTextBox.Text = patient.PatientId;
                if (patient.IsFixedBed)
                {
                    Basewindow.initContent.RadioButton3.IsChecked = true;
                }
                else
                {
                    Basewindow.initContent.RadioButton4.IsChecked = true;
                }

                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patient.AreaId;
                    var list = patientAreaDao.SelectPatientArea(condition);
                    if ((list != null) && (list.Count > 0)) Basewindow.initContent.AreaComboBox.Text = list[0].Name;
                }
            }

            Basewindow.initContent.ButtonNew.IsEnabled = true;
            Basewindow.initContent.ButtonDelete.IsEnabled = true;
            Basewindow.initContent.ButtonApply.IsEnabled = false;
            Basewindow.initContent.ButtonCancel.IsEnabled = false;

            this.Close();
        }

        private void InitMedicalOrderData(int patientId)
        {
            using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
            {
                int lastInsertId = -1;
                MedicalOrder medicalOrder = new MedicalOrder();
                
                medicalOrder.PatientId = patientId;
                medicalOrder.Activated = true;
                medicalOrder.Plan = "方法";
                medicalOrder.Seq = "5";
                medicalOrder.MethodId = 4;
                medicalOrder.Interval = 1;
                medicalOrder.Times = 0;
                medicalOrder.Description = "";
                medicalOrderDao.InsertMedicalOrder(medicalOrder, ref lastInsertId);

                medicalOrder.PatientId = patientId;
                medicalOrder.Activated = true;
                medicalOrder.Plan = "方法";
                medicalOrder.Seq = "4";
                medicalOrder.MethodId = 3;
                medicalOrder.Interval = 1;
                medicalOrder.Times = 0;
                medicalOrder.Description = "";
                medicalOrderDao.InsertMedicalOrder(medicalOrder, ref lastInsertId);

                medicalOrder.PatientId = patientId;
                medicalOrder.Activated = true;
                medicalOrder.Plan = "方法";
                medicalOrder.Seq = "3";
                medicalOrder.MethodId = 2;
                medicalOrder.Interval = 1;
                medicalOrder.Times = 0;
                medicalOrder.Description = "";
                medicalOrderDao.InsertMedicalOrder(medicalOrder, ref lastInsertId);

                medicalOrder.PatientId = patientId;
                medicalOrder.Activated = true;
                medicalOrder.Plan = "方法";
                medicalOrder.Seq = "2";
                medicalOrder.MethodId = 1;
                medicalOrder.Interval = 1;
                medicalOrder.Times = 0;
                medicalOrder.Description = "";
                medicalOrderDao.InsertMedicalOrder(medicalOrder, ref lastInsertId);

                medicalOrder.PatientId = patientId;
                medicalOrder.Activated = true;
                medicalOrder.Plan = "频次";
                medicalOrder.Seq = "1";
                medicalOrder.MethodId = -1;
                medicalOrder.Interval = 1;
                medicalOrder.Times = 0;
                medicalOrder.Description = "";
                medicalOrderDao.InsertMedicalOrder(medicalOrder, ref lastInsertId);

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //result = System.Windows.Forms.DialogResult.Cancel;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (InfectTypeComboBox == null)
                return;
            RadioButton btn = (RadioButton)sender;
            if ((string)btn.Tag == "0")
            {
                InfectTypeComboBox.IsEnabled = false;

            }
            else
            {
                InfectTypeComboBox.IsEnabled = true;
            }
        }

        private void rbTreatStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox == null)
                return;
            RadioButton btn = (RadioButton)sender;
            if ((string)btn.Tag == "0")
            {
                StatusComboBox.IsEnabled = false;
            }
            else
            {
                StatusComboBox.IsEnabled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dpBirthday.Text = DateTime.Now.ToString();

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
        }
    }
}
