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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using WpfApplication1.CustomUI;
using WpfApplication1.DataStructures;
using WpfApplication1.DAOModule;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Orientation = System.Windows.Controls.Orientation;
using UserControl = System.Windows.Controls.UserControl;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Shedule.xaml
    /// </summary>
    public partial class Shedule : UserControl
    {
        public MainWindow Basewindow;
        
        public List<PatientSchedule> PatientScheduleList = new List<PatientSchedule>();

        public Dictionary<string, Color> CureTypeDictionary = new Dictionary<string, Color>();
        public Dictionary<string, Color> InvalidCureTypeDictionary = new Dictionary<string, Color>();
        public Dictionary<string, Color> AvilidCureTypeDictionary = new Dictionary<string, Color>();

        public List<ListboxItemStatus> ListboxItemStatusesList = new List<ListboxItemStatus>();
        public ObservableCollection<MedicalOrderParaData> OrderParaList = new ObservableCollection<MedicalOrderParaData>();
        //public ObservableCollection<MedicalOrderParaData> OrderParaList = new ObservableCollection<MedicalOrderParaData>();
        public ObservableCollection<TreatOrder> TreatOrderList = new ObservableCollection<TreatOrder>();
        public ObservableCollection<string> PatientGroupComboBoxItems = new ObservableCollection<string>();

        public ObservableCollection<MedicalOrder> MedicalOrders = new ObservableCollection<MedicalOrder>(); 

        public List<DateTime> dtlist = new List<DateTime>();
        public int selectoperation;
        private bool IsEditable = false;
        private List<int> ModifiedList;

        private int weekCount = 0;
        public Shedule(MainWindow mainWindow)
        {
            InitializeComponent();
            
            //string color = (string)System.Windows.Application.Current.Resources["ysq"];

            
            //InitCureTypeDictionary();
            SetBinding();
            //LoadOrderParaConfig();
            Basewindow = mainWindow;

            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("所有");
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
            SexComboBox.SelectedIndex = 0;
            ModifiedList = new List<int>();
            LoadTreatTimes();
        }

        private void InquireButton_Click(object sender, RoutedEventArgs e)
        {
            ListboxItemStatusesList.Clear();
            var begin = new DateTime();
            var end = new DateTime();
            if (!BeginDatePicker.Text.Equals(""))
                begin = DateTime.Parse(BeginDatePicker.Text);
            else
                begin = DateTime.Now;
            if (!EndatePicker.Text.Equals(""))
                end = DateTime.Parse(EndatePicker.Text);
            else
                end = DateTime.Now;

            TimeSpan timeSpan = end - begin;
            if (timeSpan.Days > 90)
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = "查询间隔最多不能超过90天.";
                a.ShowDialog();
                return;
            }

            using (var complexDao = new ComplexDao())
            {
                var condition = new Dictionary<string, object>();
                if (!NameTextBox.Text.Equals(""))
                    condition["NAME"] = NameTextBox.Text;
                //if (!IDTextBox.Text.Equals(""))
                //    condition["ID"] = IDTextBox.Text;
                if (!PatientIDTextBox.Text.Equals(""))
                    condition["PATIENTID"] = PatientIDTextBox.Text;

                if (SexComboBox.Text.Equals("男"))
                    condition["GENDER"] = "男";
                else if (SexComboBox.Text.Equals("女"))
                    condition["GENDER"] = "女";

                if (!InfectTypeComboBox.Text.Equals("所有"))
                {
                    using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                    {
                        var condition2 = new Dictionary<string, object>();
                        condition2["NAME"] = this.InfectTypeComboBox.Text;
                        var list2 = infectTypeDao.SelectInfectType(condition2);
                        condition["INFECTTYPEID"] = list2[0].Id;
                    }
                }

                condition["TREATSTATUSID"] = 1;
                List<Patient> list = complexDao.SelectPatient(condition, begin, end);
                foreach (Patient fmriPatient in list)
                {
                    var informatian = new PatientInfo();
                    informatian.PatientDob = fmriPatient.Dob;
                    informatian.PatientGender = fmriPatient.Gender;
                    informatian.PatientMobile = fmriPatient.Mobile;
                    informatian.PatientPatientId = fmriPatient.PatientId.ToString();
                    informatian.PatientDescription = fmriPatient.Description;
                    informatian.PatientId = fmriPatient.Id;
                    informatian.PatientPatientId = fmriPatient.PatientId;
                    informatian.PatientName = fmriPatient.Name;
                    informatian.PatientIsFixedBed = fmriPatient.IsFixedBed;
                    informatian.PatientRegesiterDate = fmriPatient.RegisitDate;
                    {
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition.Clear();
                            condition["ID"] = fmriPatient.InfectTypeId;
                            var arealist = infectTypeDao.SelectInfectType(condition);
                            if (arealist.Count == 1)
                            {
                                informatian.PatientInfectType = arealist[0].Name;
                            }
                        }
                    }
                    {
                        using (var treatStatusDao = new TreatStatusDao())
                        {
                            condition.Clear();
                            condition["ID"] = fmriPatient.TreatStatusId;
                            var arealist = treatStatusDao.SelectTreatStatus(condition);
                            if (arealist.Count == 1)
                            {
                                informatian.PatientTreatStatus = arealist[0].Name;
                            }
                        }
                    }

                    ListboxItemStatus status = new ListboxItemStatus();
                    status.PatientID = fmriPatient.Id;
                    status.PatientName = fmriPatient.Name;
                    PatientSchedule schedule = GetPatientSchedule((fmriPatient.Id));

                    //foreach (var day in status.CurrentWeek.days)
                    for (int n = 0; n < 7; n++)
                    {
                        foreach (var h in schedule.Hemodialysis)
                        {
                            if (h.dialysisTime.dateTime == status.CurrentWeek.days[n].dateTime.Date)
                            {
                                status.CurrentWeek.days[n].Content = h.dialysisTime.AmPmE;
                                status.CurrentWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                            }
                            if (h.dialysisTime.dateTime == status.NextWeek.days[n].dateTime.Date)
                            {
                                status.NextWeek.days[n].Content = h.dialysisTime.AmPmE;
                                status.NextWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                            }
                        }
                    }

                    //////////////////////////////
                    //status.Checks = CheckOrders(fmriPatient.Id);
                    /*if (CheckOrders(fmriPatient.Id))
                        status.Checks = "正常";
                    else
                    {
                        status.Checks = "异常";
                    }*/

                    /*if (CheckBed(fmriPatient.Id))
                        status.Bed = "正常";
                    else
                    {
                        status.Bed = "异常";
                    }*/

                    List<TreatOrder> TreatOrderList = new List<TreatOrder>();
                    try
                    {

                        using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                        {

                            condition.Clear();
                            condition["PATIENTID"] = informatian.PatientPatientId;
                            condition["ACTIVATED"] = true;
                            var list3 = medicalOrderDao.SelectMedicalOrder(condition);

                            foreach (MedicalOrder medicalOrder in list3)
                            {
                                TreatOrder treatOrder = new TreatOrder();
                                treatOrder.Id = medicalOrder.Id;
                                treatOrder.Activated = medicalOrder.Activated;
                                treatOrder.Seq = medicalOrder.Seq;
                                treatOrder.Plan = medicalOrder.Plan;

                                treatOrder.TreatTimes = (int)medicalOrder.Times;
                                treatOrder.Description = medicalOrder.Description;

                                if (medicalOrder.MethodId != -1)
                                {
                                    using (var treatMethodDao = new TreatMethodDao())
                                    {
                                        condition.Clear();
                                        condition["ID"] = (int)medicalOrder.MethodId;
                                        var arealist = treatMethodDao.SelectTreatMethod(condition);
                                        if (arealist.Count == 1)
                                        {
                                            treatOrder.TreatMethod = arealist[0].Name;
                                        }
                                    }
                                }
                                else
                                {
                                    treatOrder.TreatMethod = "NULL";
                                }
                                {
                                    using (var medicalOrderParaDao = new MedicalOrderParaDao())
                                    {
                                        condition.Clear();
                                        condition["ID"] = medicalOrder.Interval;
                                        var arealist = medicalOrderParaDao.SelectInterval(condition);
                                        if (arealist.Count == 1)
                                        {
                                            treatOrder.Type = arealist[0].Name;
                                        }
                                    }
                                }

                                TreatOrderList.Add(treatOrder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log.WriteInfoConsole("In Order.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
                    }

                    /*if (TreatOrderList.Count != 0)
                    {
                        foreach (var s in TreatOrderList)
                        {
                            s.
                            if (s != "")
                            {
                                string[] details = s.Split('/');
                                if (details.Count() == 3)
                                {
                                    var treat = new TreatOrder();
                                    treat.TreatMethod = details[0];

                                    var medicalOrderParaDao = new MedicalOrderParaDao();
                                    var condition1 = new Dictionary<string, object>();
                                    condition1["ID"] = details[1];
                                    var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                    string temporder;
                                    treat.Type = list1[0].Name;
                                    treat.TreatTimes = int.Parse(details[2]);
                                    temporder = treat.Type + "/" + treat.TreatTimes + "/" + treat.TreatMethod;
                                    treatOrders += temporder;
                                    treatOrders += "\n";
                                }
                            }
                        }
                        treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                        status.ToolTips = treatOrders;
                    }
                    else
                    {
                        status.ToolTips = "";
                    }*/


                    /*string treatOrders = "";
                    string orders = fmriPatient.Orders;
                    if (orders != "" && orders != null)
                    {
                        string[] order = orders.Split('#');
                        foreach (var s in order)
                        {
                            if (s != "")
                            {
                                string[] details = s.Split('/');
                                if (details.Count() == 3)
                                {
                                    var treat = new TreatOrder();
                                    treat.TreatMethod = details[0];

                                    var medicalOrderParaDao = new MedicalOrderParaDao();
                                    var condition1 = new Dictionary<string, object>();
                                    condition1["ID"] = details[1];
                                    var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                    string temporder;
                                    treat.Type = list1[0].Name;
                                    treat.TreatTimes = int.Parse(details[2]);
                                    temporder = treat.Type + "/" + treat.TreatTimes + "/" + treat.TreatMethod;
                                    treatOrders += temporder;
                                    treatOrders += "\n";
                                }
                            }
                        }
                        treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                        status.ToolTips = treatOrders;
                    }
                    else
                    {
                        status.ToolTips = "";
                    }*/



                    ListboxItemStatusesList.Add(status);
                }
            }
            ListBox1.Items.Refresh();
            RefreshStatistics();
        }

        private void TimeRadioButton1_Click(object sender, RoutedEventArgs e)
        {
            selectoperation = 0;
            EndatePicker.SelectedDate = DateTime.Now;
            if (TimeRadioButton1.IsChecked == true)
            {
                BeginDatePicker.SelectedDate = DateTime.Parse("2012-01-01");
            }
            else if (TimeRadioButton2.IsChecked == true)
            {
                if (EndatePicker.SelectedDate != null)
                {
                    BeginDatePicker.SelectedDate = EndatePicker.SelectedDate - TimeSpan.FromDays(7);
                }
            }
            else if (TimeRadioButton3.IsChecked == true)
            {
                if (EndatePicker.SelectedDate != null)
                {
                    BeginDatePicker.SelectedDate = EndatePicker.SelectedDate - TimeSpan.FromDays(3);
                }
            }
            selectoperation = 1;
        }


        private void InitDay()
        {
            int dayofweek = (int)DateTime.Now.DayOfWeek;
            switch (dayofweek)
            {
                case 0:
                    BtnSun.IsChecked = true;
                    btn6.IsChecked = true;
                    break;
                case 1:
                    BtnMon.IsChecked = true;
                    btn0.IsChecked = true;
                    break;
                case 2:
                    BtnTue.IsChecked = true;
                    btn1.IsChecked = true;
                    break;
                case 3:
                    BtnWed.IsChecked = true;
                    btn2.IsChecked = true;
                    break;
                case 4:
                    BtnThe.IsChecked = true;
                    btn3.IsChecked = true;
                    break;
                case 5:
                    BtnFri.IsChecked = true;
                    btn4.IsChecked = true;
                    break;
                case 6:
                    BtnSta.IsChecked = true;
                    btn5.IsChecked = true;
                    break;
            }
        }


        private List<string> GetUnCopyOrder(long _patientID)
        {
            List<string> ret = new List<string>();
            try
            {

                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = _patientID;
                    List<Patient> list = patientDao.SelectPatient(condition);
                    if (list.Count > 0)
                    {
                        string orders = list[0].Orders;
                        if (orders != null)
                        {
                            string[] order = orders.Split('#');
                            foreach (var s in order)
                            {
                                if (s != "")
                                {
                                    string[] details = s.Split('/');
                                    if (details.Count() == 3)
                                    {
                                        var treat = new TreatOrder();
                                        treat.TreatMethod = details[0];

                                        var medicalOrderParaDao = new MedicalOrderParaDao();
                                        var condition1 = new Dictionary<string, object>();
                                        condition1["ID"] = details[1];
                                        var list1 = medicalOrderParaDao.SelectInterval(condition1);


                                        treat.Type = list1[0].Type;
                                        if (treat.Type == "月")
                                            ret.Add(treat.TreatMethod);
                                    }
                                }
                            }
                        }
                    }
                }
                return ret;

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteErrorLog("Init.xaml.cs-CheckPatientPatientIdValidity", ex);
                return null;
            }
        }

        private void InitTreatOrderList( long _patientID )
        {
            TreatOrderList.Clear();

            try
            {

                using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["PATIENTID"] = _patientID;
                    condition["ACTIVATED"] = true;
                    var list3 = medicalOrderDao.SelectMedicalOrder(condition);

                    foreach (MedicalOrder medicalOrder in list3)
                    {
                        TreatOrder treatOrder = new TreatOrder();
                        treatOrder.Id = medicalOrder.Id;
                        treatOrder.Activated = medicalOrder.Activated;
                        treatOrder.Seq = medicalOrder.Seq;
                        treatOrder.Plan = medicalOrder.Plan;

                        treatOrder.TreatTimes = (int)medicalOrder.Times;
                        treatOrder.Description = medicalOrder.Description;

                        if (medicalOrder.MethodId != -1)
                        {
                            using (var treatMethodDao = new TreatMethodDao())
                            {
                                condition.Clear();
                                condition["ID"] = (int)medicalOrder.MethodId;
                                var arealist = treatMethodDao.SelectTreatMethod(condition);
                                if (arealist.Count == 1)
                                {
                                    treatOrder.TreatMethod = arealist[0].Name;
                                }
                            }
                        }
                        else
                        {
                            treatOrder.TreatMethod = "NULL";
                        }
                        {
                            using (var medicalOrderParaDao = new MedicalOrderParaDao())
                            {
                                condition.Clear();
                                condition["ID"] = medicalOrder.Interval;
                                var arealist = medicalOrderParaDao.SelectInterval(condition);
                                if (arealist.Count == 1)
                                {
                                    treatOrder.Type = arealist[0].Name;
                                }
                            }
                        }

                        TreatOrderList.Add(treatOrder);
                    }
                }


                /*using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = _patientID;
                    List<Patient> list = patientDao.SelectPatient(condition);
                    if (list.Count > 0)
                    {
                        string orders = list[0].Orders;
                        if (orders != null)
                        {
                            string[] order = orders.Split('#');
                            foreach (var s in order)
                            {
                                if (s != "")
                                {
                                    string[] details = s.Split('/');
                                    if (details.Count() == 3)
                                    {
                                        var treat = new TreatOrder();
                                        treat.TreatMethod = details[0];

                                        var medicalOrderParaDao = new MedicalOrderParaDao();
                                        var condition1 = new Dictionary<string, object>();
                                        condition1["ID"] = details[1];
                                        var list1 = medicalOrderParaDao.SelectInterval(condition1);


                                        //treat.Type = details[1];
                                        treat.Type = list1[0].Type;
                                        treat.TreatTimes = int.Parse(details[2]);
                                        TreatOrderList.Add(treat);
                                    }
                                }
                            }
                        }
                    }
                }*/


            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteErrorLog("Init.xaml.cs-CheckPatientPatientIdValidity", ex);
            }
        }

        private void PatientlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox1.SelectedIndex >= 0)
            {
                TreatOrderList.Clear();
                long patientID = ListboxItemStatusesList[ListBox1.SelectedIndex].PatientID;
                try
                {

                    using (var patientDao = new PatientDao())
                    {
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = patientID;
                        List<Patient> list = patientDao.SelectPatient(condition);
                        if (list.Count > 0)
                        {
                            string orders = list[0].Orders;
                            if (orders != null)
                            {
                                string[] order = orders.Split('#');
                                foreach (var s in order)
                                {
                                    if (s != "")
                                    {
                                        string[] details = s.Split('/');
                                        if (details.Count() == 3)
                                        {
                                            var treat = new TreatOrder();
                                            treat.TreatMethod = details[0];

                                            var medicalOrderParaDao = new MedicalOrderParaDao();
                                            var condition1 = new Dictionary<string, object>();
                                            condition1["ID"] = details[1];
                                            var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                        

                                            //treat.Type = details[1];
                                            treat.Type = list1[0].Type;
                                            treat.TreatTimes = int.Parse(details[2]);
                                            TreatOrderList.Add(treat);
                                        }
                                    }
                                }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteErrorLog("Init.xaml.cs-CheckPatientPatientIdValidity", ex);
                    //return false;
                }
                //UpdatePatientSchedule();
            }
            
            
        }

        private void InitCureTypeDictionary()
        {
            CureTypeDictionary.Add("HD", Colors.Red);
            CureTypeDictionary.Add("HP", Colors.BlueViolet);
            CureTypeDictionary.Add("HDF", Colors.Chartreuse);
            CureTypeDictionary.Add("xxx", Colors.Yellow);;

        }

        public string StrColorConverter( Color color)
        {
            foreach (var v in CureTypeDictionary)
            {
                if (v.Value == color)
                    return v.Key;
            }
            return "";
        }
        public string StrColorConverter(Brush brush)
        {
            Color color = ((SolidColorBrush)brush).Color;
            foreach (var v in CureTypeDictionary)
            {
                if (v.Value == color)
                    return v.Key;
            }
            return "";
        }
        public Color StrColorConverter(string str)
        {
            if (str == "")
                return Colors.Transparent;
            try
            {
                return CureTypeDictionary[str];
            }
            catch (Exception)
            {

                return Colors.Gray;
            }
            
        }
        
        private void ButtonBase_OnClick(object sender, MouseButtonEventArgs e)
        {


            if (IsEditable == false) return;
            DateTime dtTime1 = DateTime.Now;
            Button btn = (Button) sender;
            string tag = (string) btn.Tag;
            int index = ListBox1.SelectedIndex;
            if ( index == -1 ) 
                return;
            if (!ModifiedList.Contains(index))
            {
                ModifiedList.Add(index);
            }

            bool ret = ChangeButtonStauts(index, tag, e.ChangedButton);
            DateTime dtTime2 = DateTime.Now;
            TimeSpan tsSpan1 = dtTime2 - dtTime1;
            MainWindow.Log.WriteInfoLog("tsSpan1 is: " + tsSpan1.Milliseconds);
            if (ret)
            {
                //UpdatePatientSchedule( tag );
                DateTime dtTime3 = DateTime.Now;
                TimeSpan tsSpan2 = dtTime3 - dtTime2;
                MainWindow.Log.WriteInfoLog("tsSpan2 is: " + tsSpan2.Milliseconds);
                /////////
                //ListboxItemStatusesList[index].Checks = CheckOrders();

                //ListboxItemStatusesList[index].Bed = CheckBed();
                /*if (CheckOrders())
                    ListboxItemStatusesList[index].Checks = "正常";
                else
                {
                    ListboxItemStatusesList[index].Checks = "异常";
                }*/
                DateTime dtTime4 = DateTime.Now;
                TimeSpan tsSpan3 = dtTime4 - dtTime3;
                MainWindow.Log.WriteInfoLog("tsSpan3 is: " + tsSpan3.Milliseconds);
                DateTime dtTime5 = DateTime.Now;
                TimeSpan tsSpan4 = dtTime5 - dtTime4;
                MainWindow.Log.WriteInfoLog("tsSpan4 is: " + tsSpan4.Milliseconds);
            }
                
        }

        private Point GetWeekAndDay( string tag )
        {
            Point point = new Point(0,0);
            switch (tag)
            {
                case "Mon0":
                    point = new Point(0, 0);
                    break;
                case "Tue0":
                    point = new Point(0, 1);
                    break;
                case "Wed0":
                    point = new Point(0, 2);
                    break;
                case "Thu0":
                    point = new Point(0, 3);
                    break;
                case "Fri0":
                    point = new Point(0, 4);
                    break;
                case "Sta0":
                    point = new Point(0, 5);
                    break;
                case "Sun0":
                    point = new Point(0, 6);
                    break;


                case "Mon1":
                    point = new Point(1, 0);
                    break;
                case "Tue1":
                    point = new Point(1, 1);
                    break;
                case "Wed1":
                    point = new Point(1, 2);
                    break;
                case "Thu1":
                    point = new Point(1, 3);
                    break;
                case "Fri1":
                    point = new Point(1, 4);
                    break;
                case "Sta1":
                    point = new Point(1, 5);
                    break;
                case "Sun1":
                    point = new Point(1, 6);
                    break;
                default:
                    break;
            }
            return point;
        }

        private Brush GetNextBrush( Brush brush)
        {
            if (AvilidCureTypeDictionary.Count == 0) return brush;
            Color color = ((SolidColorBrush)brush).Color;
            int n = 0;
            foreach (var v in AvilidCureTypeDictionary)
            {
                if (v.Value == color)
                {
                    break;
                }
                n++;
            }

            int count = n+1;
            if (count >= AvilidCureTypeDictionary.Count)
            {
                count = 0;
            }

            n = 0;
            foreach (var v in AvilidCureTypeDictionary)
            {
                if (n == count)
                {
                    return new SolidColorBrush(v.Value);
                }
                n++;
            }
            return null;

        }

        //private List<string> TreatTimeList = new List<string>();
        private int[] treateTimeiInts = new int[4];
        private void LoadTreatTimes()
        {
            try
            {
                treateTimeiInts[0] = 0;
                treateTimeiInts[1] = 0;
                treateTimeiInts[2] = 0;
                treateTimeiInts[3] = 1;
                using (var treatTimeDao = new TreatTimeDao())
                {
                    //TreatTimeList.Clear();
                    var condition = new Dictionary<string, object>();
                    condition["Activated"] = true;
                    var list = treatTimeDao.SelectTreatTime(condition);
                    foreach (var type in list)
                    {
                        var treatTimeData = new TreatTimeData();
                        treatTimeData.Id = type.Id;
                        treatTimeData.Name = type.Name;
                        treatTimeData.Activated = type.Activated;
                        treatTimeData.BeginTime = type.BeginTime;
                        treatTimeData.EndTime = type.EndTime;
                        treatTimeData.Description = type.Description;

                        if (type.Name == "AM")
                            treateTimeiInts[0] = 1;
                        else if (type.Name == "PM")
                            treateTimeiInts[1] = 1;
                        else if (type.Name == "Evening")
                            treateTimeiInts[2] = 1;

                        //TreatTimeList.Add(type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatTime.xaml.cs:ListViewCTreatTime_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private string GetNextTime(string time)
        {
            /*string ret = "";
            switch (time)
            {
                case "AM":
                    ret = "PM";
                    break;
                case "PM":
                    ret = "E";
                    break;
                case "E":
                    ret = "";
                    break;
                case "":
                    ret = "AM";
                    break;
                default:
                    ret = "";
                    break;
                    
            }

            return ret;*/
            int flag = 0;
            switch (time)
            {
                case "AM":
                    flag = 0;
                    break;
                case "PM":
                    flag = 1;
                    break;
                case "E":
                    flag = 2;
                    break;
                case "":
                    flag = 3;
                    break;

            }
            flag += 1;
            if (flag == 4) flag = 0;
            int retInt = -1;
            for (int n = flag; n < 4; n++)
            {
                if (treateTimeiInts[n] == 1)
                {
                    retInt = n;
                    break;
                }
            }
            if (retInt == -1)
            {
                for (int n = 0; n < flag; n++)
                {
                    if (treateTimeiInts[n] == 1)
                    {
                        retInt = n;
                    }
                }
            }

            if (retInt == 0)
                return "AM";
            else if (retInt == 1)
                return "PM";
            else if (retInt == 2)
                return "E";
            else
                return "";

        }
        private bool ChangeButtonStauts(int index, string tag, MouseButton mouseButton )
        {
            Point column = GetWeekAndDay(tag);
            int week = (int)column.X;
            int day = (int)column.Y;

            ListboxItemStatus listboxItem = ListboxItemStatusesList[index];
            string time;
            Brush type;

            if (week == 0)
            {
                if (DateTime.Compare(listboxItem.CurrentWeek.days[day].dateTime.Date, DateTime.Now.Date) < 0)
                {
                    return true;
                }
                //MessageBox.Show(listboxItem.CurrentWeek.days[day].dateTime.ToString());
                time = listboxItem.CurrentWeek.days[day].Content;
                type = listboxItem.CurrentWeek.days[day].BgColor;
            }
            else
            {
                if (DateTime.Compare(listboxItem.NextWeek.days[day].dateTime.Date, DateTime.Now.Date) < 0)
                {
                    return true;
                }
                //MessageBox.Show(listboxItem.NextWeek.days[day].dateTime.ToString());
                time = listboxItem.NextWeek.days[day].Content;
                type = listboxItem.NextWeek.days[day].BgColor;
            }

            if (mouseButton == MouseButton.Left)
            {
                time = GetNextTime(time);
                if (week == 0)
                {
                    listboxItem.CurrentWeek.days[day].Content = time;
                }
                else
                {
                    listboxItem.NextWeek.days[day].Content = time;
                }
                if (time == "")
                {
                    if (week == 0)
                    {
                        listboxItem.CurrentWeek.days[day].BgColor = Brushes.LightGray;
                    }
                    else
                    {
                        listboxItem.NextWeek.days[day].BgColor = Brushes.LightGray;
                    }
                    
                }
                if (time == "AM")
                {
                    if (week == 0)
                    {
                        listboxItem.CurrentWeek.days[day].BgColor = new SolidColorBrush(AvilidCureTypeDictionary.Values.First());
                    }
                    else
                    {
                        listboxItem.NextWeek.days[day].BgColor = new SolidColorBrush(AvilidCureTypeDictionary.Values.First());
                    }

                }
            }
            else
            {
                type = GetNextBrush(type);
                if (time == "")
                    return false;
                if (week == 0)
                {
                    listboxItem.CurrentWeek.days[day].BgColor = type;
                }
                else
                {
                    listboxItem.NextWeek.days[day].BgColor = type;
                }
            }
            
            
            
            ListboxItemStatusesList[index] = listboxItem;
            ListBox1.Items.Refresh();
            RefreshStatistics();


            
            return true;
        }


        private void BtnDay_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton) sender;
            if (btn.IsChecked == true)
            {
                UncheckOtherToggleButton(btn);
            }
            else
            {
                btn.IsChecked = true;
                UncheckOtherToggleButton(btn);
            }
        }

        private void UncheckOtherToggleButton(ToggleButton btn )
        {
            foreach (var i in GridDay.Children)
            {
                if ((i is ToggleButton)&& i!= btn)
                {
                    ((ToggleButton) i).IsChecked = false;
                }
            }
        }

        public void SetBinding()
        {
            ListBox1.ItemsSource = ListboxItemStatusesList;
            PatientGroupComboBox.ItemsSource = PatientGroupComboBoxItems;
            //InitPatientGroupComboBox();
            CopySchedule();
            LoadTratementConifg();
            InitWeekWithDate();
            //ListboxItemStatus status = new ListboxItemStatus();
            //status.PatientID = 1;
            //status.PatientName = "zhangsan";

            //Week week = new Week();
            //week.days[0].Content = "AM";
            //week.days[0].BgColor = Brushes.SeaGreen;

            //Week week1 = new Week();
            //week1.days[0].Content = "PM";
            //week1.days[0].BgColor = Brushes.GreenYellow;

            //status.CurrentWeek = week;
            //status.NextWeek = week1;
            //ListboxItemStatusesList.Add(status);


            //ListboxItemStatus status1 = new ListboxItemStatus();
            //status1.PatientID = 1;
            //status1.PatientName = "lisi";
            //ListboxItemStatusesList.Add(status1);

        }

        private void LoadTratementConifg()
        {
            try
            {
                sheet.Children.Clear();
                using (var methodDao = new TreatMethodDao())
                {
                    //Datalist.Clear();
                    CureTypeDictionary.Clear();
                    AvilidCureTypeDictionary.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = methodDao.SelectTreatMethod(condition);
                    foreach (var pa in list)
                    {
                        var treatMethodData = new TreatMethodData();
                        treatMethodData.Id = pa.Id;
                        treatMethodData.Name = pa.Name;
                        {
                            using (var treatTypeDao = new TreatTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.TreatTypeId;
                                var arealist = treatTypeDao.SelectTreatType(condition);
                                if (arealist.Count == 1)
                                {
                                    treatMethodData.Type = arealist[0].Name;
                                }
                            }
                        }
                        string bgColor = pa.BgColor;
                        Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        if (bgColor != "" && bgColor != null)
                            treatMethodData.BgColor = bgBrush;
                        else
                            treatMethodData.BgColor = Brushes.LightGray;


                        treatMethodData.Description = pa.Description;

                        /*StackPanel panel1 = new StackPanel();
                        panel1.Orientation = Orientation.Horizontal;
                            
                        Rectangle rect = new Rectangle();
                        rect.Width = rect.Height = 10;
                        rect.Fill = bgBrush;
                        rect.HorizontalAlignment = HorizontalAlignment.Left;
                        Label label = new Label();
                        label.HorizontalContentAlignment = HorizontalAlignment.Center;
                        label.VerticalContentAlignment = VerticalAlignment.Center;
                        label.Content = treatMethodData.Name;
                        label.FontSize = 10;
                        panel1.Children.Add(rect);
                        panel1.Children.Add(label);*/

                        Button btn = new Button();
                        btn.Style = this.FindResource("LabelStyleWithColor") as Style;
                        btn.Content = treatMethodData.Name;
                        btn.Background = bgBrush;
                        btn.Width = 50;
                        btn.Height = 20;
                        btn.Margin = new Thickness(2, 2, 2, 2);
                        btn.HorizontalAlignment = HorizontalAlignment.Left;


                        //sheet.Children.Add(panel1);
                        if (pa.IsAvailable == true)
                        {
                            sheet.Children.Add(btn);
                            AvilidCureTypeDictionary.Add(pa.Name, ((SolidColorBrush)treatMethodData.BgColor).Color);
                        }
                            
                        CureTypeDictionary.Add(pa.Name, ((SolidColorBrush)treatMethodData.BgColor).Color);
                        //Datalist.Add(treatMethodData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        private string CheckOrders()
        {
            int index = ListBox1.SelectedIndex;
            if (index == -1) return "";
            
            if (ListboxItemStatusesList.Count == 0) return "";
            ListboxItemStatus patient = ListboxItemStatusesList[index];
            PatientSchedule schedule = GetPatientSchedule(patient.PatientID);
            InitTreatOrderList(patient.PatientID);
            bool ret = true;
            if (TreatOrderList.Count == 0) return "";
            /*
            List<string> treats = new List<string>();
            foreach (var treatOrder in TreatOrderList)
            {
                treats.Add(treatOrder.TreatMethod);
            }*/
            foreach (var treatOrder in TreatOrderList)
            {
                string treat = treatOrder.TreatMethod;
                string type = treatOrder.Type;
                int seq = int.Parse(treatOrder.Seq);
                //int times = treatOrder.TreatTimes;
                int count = 0;
                if (type == "单周")
                {
                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    int currentSeq = 0;
                    int nextSeq = 0;
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date.AddDays(6)) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                currentSeq++;

                        }

                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date.AddDays(7)) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                nextSeq++;
                        }
                    }
                    if (currentSeq != seq || nextSeq != seq)
                    {
                        return treat;
                        //return false;
                    }

                }
                else if (type == "单月")
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int dure = DateTime.DaysInMonth(year, month);
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-day + 1);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-day + dure);


                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }
                    if (count != seq)
                    {
                        return treat;
                        //return false;
                    }
                }
                else//双周
                {
                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }

                    if (count != seq)
                    {
                        return treat;
                        //return false;
                    }
                }
            }
            return "";

        }
        //
        private bool CheckOrdersNew(long _PatientID)
        {
            PatientSchedule schedule = GetPatientSchedule(_PatientID);
            bool ret = true;
            InitTreatOrderList(_PatientID);
            if (TreatOrderList.Count == 0) return false;
            List<string> treats = new List<string>();
            foreach (var treatOrder in TreatOrderList)
            {
                treats.Add(treatOrder.TreatMethod);
            }
            foreach (var treatOrder in TreatOrderList)
            {
                string treat = treatOrder.TreatMethod;
                string type = treatOrder.Type;
                int times = treatOrder.TreatTimes;
                int count = 0;
                if (type == "单周")
                {
                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem && treats.Contains(v.hemodialysisItem))
                                count++;
                            else if (!treats.Contains(v.hemodialysisItem))
                            {
                                return false;
                            }
                        }
                    }
                }
                else if (type == "单月")
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int dure = DateTime.DaysInMonth(year, month);
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-day + 1);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-day + dure);


                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem && treats.Contains(v.hemodialysisItem))
                                count++;
                            else if (!treats.Contains(v.hemodialysisItem))
                            {
                                return false;
                            }
                        }
                    }
                }
                else//双周
                {

                }
                if (count != times)
                {
                    ret = false;
                }
            }
            return ret;

        }


        private string CheckOrders(long _PatientID )
        {
            PatientSchedule schedule = GetPatientSchedule(_PatientID);
            
            InitTreatOrderList(_PatientID);
            if (TreatOrderList.Count == 0) return "正常";

/*
            List<string> treats = new List<string>();
            foreach (var treatOrder in TreatOrderList)
            {
                treats.Add(treatOrder.TreatMethod);
            }*/

            int frequency = 0;
            int singleWeekFrequency = 0;
            int doubleWeekFrequency = 0;
            int singleMonthFrequency = 0;
            List<string> treatList = new List<string>();
            foreach (var treatOrder in TreatOrderList)
            {
                if (treatOrder.TreatMethod == "NULL")
                    treatList.Add("HD");
                else
                {
                    treatList.Add(treatOrder.TreatMethod);
                }
            }


            foreach (var treatOrder in TreatOrderList)
            {
                //string treat = treatOrder.TreatMethod;
                //int times = treatOrder.TreatTimes;
                //int count = 0;
                //for (int n = 0; n < 7; n++)
                //{
                //    if (treat == StrColorConverter(patient.CurrentWeek.days[n].BgColor) )
                //        count++;
                //    if (treat == StrColorConverter(patient.NextWeek.days[n].BgColor))
                //        count++;

                //}
                //if (count != times)
                //{
                //    ret = false;
                //}
                

                string treat = treatOrder.TreatMethod;
                string type = treatOrder.Type;
                int seq = treatOrder.TreatTimes;
                //int times = treatOrder.TreatTimes;
                int count = 0;

                if (treatOrder.Plan == "频次")
                {
                    //frequency = treatOrder.TreatTimes;

                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            //if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }

                    if (count != treatOrder.TreatTimes)
                    {
                        return "频次";
                        //return false;
                    }

                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (!treatList.Contains(v.hemodialysisItem))
                            {
                                return "异常";
                            }
                            
                        }
                    }

                    continue;
                }


                if (type == "单周")
                {
                    /*for (int n = 0; n < 7; n++)
                    {
                        if (treat == StrColorConverter(patient.CurrentWeek.days[n].BgColor))
                            count++;
                        if (treat == StrColorConverter(patient.NextWeek.days[n].BgColor))
                            count++;

                    }*/
                    /*int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int dure = DateTime.DaysInMonth(year, month);*/
                    
                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    int currentSeq = 0;
                    int nextSeq = 0;
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date.AddDays(6)) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                currentSeq++;

                        }

                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date.AddDays(7)) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                nextSeq++;
                        }
                    }
                    if (currentSeq != seq || nextSeq != seq)
                    {
                        return treat;
                        //return false;
                    }
                    /*else
                    {
                        singleWeekFrequency = currentSeq + nextSeq;
                    }*/

                }
                else if (type == "单月")
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int dure = DateTime.DaysInMonth(year, month);
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-day + 1);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-day + dure);


                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }
                    if (count != seq)
                    {
                        return treat;
                        //return false;
                    }
                    /*else
                    {
                        singleMonthFrequency = seq;
                    }*/
                }
                else//双周
                {
                    int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                    if (dayofweek == -1) dayofweek = 6;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }

                    if (count != seq)
                    {
                        return treat;
                        //return false;
                    }
                    /*else
                    {
                        singleMonthFrequency = seq;
                    }*/
                }
            }

            return "正常";

            /*if (frequency != (singleMonthFrequency + doubleWeekFrequency + singleWeekFrequency))
            {
                return "HD";
            }
            else
            {
                return "正常";
            }*/
            

        }


        private string CheckBed(long _patientID)
        {
            try
            {
                int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
                if (dayofweek == -1) dayofweek = 6;
                DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 13);

                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientId"] = _patientID.ToString();
                    var list = scheduleDao.SelectScheduleTemplate(condition);
                    DateTime ret = new DateTime();
                    int n = 0;
                    foreach (ScheduleTemplate type in list)
                    {
                        DateTime dt = DateTime.Parse(type.Date);
                        if (DateTime.Compare(dt.Date, dtFrom.Date) >= 0 &&
                    DateTime.Compare(dt.Date, dtTo.Date) <= 0)
                        {
                            //if(type.PatientId == 16)
                            if (type.BedId == -1)
                            {
                                if (n == 0)
                                    ret = dt;
                                else
                                {
                                    if (DateTime.Compare(dt.Date, ret.Date) <= 0)
                                    {
                                        ret = dt;
                                    }
                                    
                                }
                                n++;
                                //ret = dt;
                                //return dt.ToString("MM-dd");
                            }
                                
                        }

                    }
                    if (n == 0)
                        return "正常";
                    else
                        return ret.ToString("MM-dd");
                }
                return null;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
                return null;
            }
        }

        public void InitPatientGroupComboBox()
        {
            try
            {
                PatientGroupComboBoxItems.Clear();
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
                        PatientGroupComboBoxItems.Add(patientGroupData.Name);
                    }
                }

                /*if (PatientGroupComboBoxItems.Count != 0)
                    PatientGroupComboBox.SelectedIndex = 0;*/
                if (PatientGroupComboBoxItems.Count != 0)
                this.PatientGroupComboBox.SelectedValue = Basewindow.patientGroupPanel.ComboBoxPatientGroup.SelectedValue;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:ComboBoxPatientGroup_OnInitialized exception messsage: " + ex.Message);
            }

        }

        public void Reload()
        {
            InitWeekWithDate();
            InitDay();
            LoadTratementConifg();
            ListBox1.SelectedIndex = -1;
            InitPatientGroupComboBox();
            LoadTreatTimes();
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
                MainWindow.Log.WriteInfoConsole("In Shedule.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }
            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeekVisible = Visibility.Hidden;

            }
            IsEditable = false;
            ListBox1.Items.Refresh();
            RefreshStatistics();
            ButtonCancel.IsEnabled = false;
            ButtonApply.IsEnabled = false;
            ButtonEdit.IsEnabled = true;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitWeekWithDate();
            InitDay();
            //GetPatientSchedule(0);
            LoadTratementConifg();

            ListBox1.SelectedIndex = -1;
            InitPatientGroupComboBox();
            LoadTreatTimes();
            //CopySchedule();
            //return;
            /*
             * try
            {
                //PatientList.Clear();
                ListboxItemStatusesList.Clear();
                using (ComplexDao patientDao = new ComplexDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    //var list = patientDao.SelectPatient(condition);
                    condition["TREATSTATUSID"] = 1;
                    var end = DateTime.Now;
                    var begin = end.AddMonths(-1);
                    List<Patient> list = patientDao.SelectPatient(condition, begin, end);

                    foreach (Patient type in list)
                    {
                        PatientInfo patientInfo = new PatientInfo();
                        patientInfo.PatientId = type.Id;
                        patientInfo.PatientName = type.Name;
                        patientInfo.PatientDob = type.Dob;
                        patientInfo.PatientPatientId = type.PatientId;
                        patientInfo.PatientGender = type.Gender;
                        patientInfo.PatientMobile = type.Mobile;
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = type.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientInfo.PatientInfectType = arealist[0].Name;
                                }
                            }
                        }
                        {
                            using (var treatStatusDao = new TreatStatusDao())
                            {
                                condition.Clear();
                                condition["ID"] = type.TreatStatusId;
                                var arealist = treatStatusDao.SelectTreatStatus(condition);
                                if (arealist.Count == 1)
                                {
                                    patientInfo.PatientTreatStatus = arealist[0].Name;
                                }
                            }
                        }
                        patientInfo.PatientRegesiterDate = type.RegisitDate;
                        patientInfo.PatientIsFixedBed = type.IsFixedBed;
                        patientInfo.PatientIsAssigned = type.IsAssigned;
                        patientInfo.PatientDescription = type.Description;

                        ListboxItemStatus status = new ListboxItemStatus();
                        status.PatientID = patientInfo.PatientId;
                        status.PatientName = patientInfo.PatientName;
                        PatientSchedule schedule = GetPatientSchedule(patientInfo.PatientId);

                        //foreach (var day in status.CurrentWeek.days)
                        for( int n = 0; n < 7; n++ )
                        {
                            foreach (var h in schedule.Hemodialysis)
                            {
                                if (h.dialysisTime.dateTime == status.CurrentWeek.days[n].dateTime.Date)
                                {
                                    status.CurrentWeek.days[n].Content = h.dialysisTime.AmPmE;
                                    status.CurrentWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                }
                                if (h.dialysisTime.dateTime == status.NextWeek.days[n].dateTime.Date)
                                {
                                    status.NextWeek.days[n].Content = h.dialysisTime.AmPmE;
                                    status.NextWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                }
                            }
                        }
                        if (CheckOrders(patientInfo.PatientId))
                            status.Checks = "正常";
                        else
                        {
                            status.Checks = "异常";
                        }

                        if (CheckBed(patientInfo.PatientId))
                            status.Bed = "正常";
                        else
                        {
                            status.Bed = "异常";
                        }



                        string treatOrders = "";
                        string orders = type.Orders;
                        if (orders != "" && orders != null)
                        {
                            string[] order = orders.Split('#');
                            foreach (var s in order)
                            {
                                if (s != "")
                                {
                                    string[] details = s.Split('/');
                                    if (details.Count() == 3)
                                    {
                                        var treat = new TreatOrder();
                                        treat.TreatMethod = details[0];

                                        var medicalOrderParaDao = new MedicalOrderParaDao();
                                        var condition1 = new Dictionary<string, object>();
                                        condition1["ID"] = details[1];
                                        var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                        string temporder;
                                        treat.Type = list1[0].Name;
                                        treat.TreatTimes = int.Parse(details[2]);
                                        temporder = treat.Type + "/" + treat.TreatTimes + "/" + treat.TreatMethod;
                                        treatOrders += temporder;
                                        treatOrders += "\n";
                                    }
                                }
                            }
                            treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                            status.ToolTips = treatOrders;
                        }
                        else
                        {
                            status.ToolTips = "";
                        }


                        ListboxItemStatusesList.Add(status);
                        //PatientList.Add(patientInfo);
                    }
                }
                ListBox1.Items.Refresh();
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }*/

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
                MainWindow.Log.WriteInfoConsole("In Shedule.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

            


            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeekVisible = Visibility.Hidden;

            }
            IsEditable = false;
            ListBox1.Items.Refresh();
                RefreshStatistics();
            ButtonCancel.IsEnabled = false;
            ButtonApply.IsEnabled = false;
            ButtonEdit.IsEnabled = true;

        }


        private PatientSchedule GetPatientSchedule(long _patientID)
        {
            try
            {
                PatientSchedule patientSchedule = new PatientSchedule(_patientID);

                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientId"] = _patientID.ToString();
                    var list = scheduleDao.SelectScheduleTemplate(condition);
                    foreach (ScheduleTemplate type in list)
                    {
                        Hemodialysy hemodialysy = new Hemodialysy();

                        /*DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                        dtFormat.ShortDatePattern = "yyyy-MM-dd";

                        datetdt = Convert.ToDateTime(type.Date, dtFormat);*/
                        DateTime dt = DateTime.Parse(type.Date);

                        hemodialysy.dialysisTime = new DialysisTime(dt, type.AmPmE);
                        hemodialysy.hemodialysisItem = type.Method;

                        patientSchedule.Hemodialysis.Add(hemodialysy);
                    }
                    return patientSchedule;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
                return null;
            }
        }

        private void UpdatePatientSchedule( )
        {
            try
            {
                /*Point column = GetWeekAndDay(tag);
                int weeks = (int)column.X;
                int days = (int)column.Y;*/
                foreach (var line in ModifiedList)
                {
                    long selectPatientID = ListboxItemStatusesList[line].PatientID;
                    using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                    {
                        foreach (var v in ListboxItemStatusesList)
                        {
                            long patientID = v.PatientID;
                            if (selectPatientID == patientID)
                            {
                                bool fixbed = false;
                                long bedid = -1;
                                var condition = new Dictionary<string, object>();
                                using (var patientDao = new PatientDao())
                                {
                                    condition.Clear();
                                    condition["ID"] = patientID;
                                    List<Patient> patientlist = patientDao.SelectPatient(condition);
                                    if (patientlist.Count == 1)
                                    {
                                        BedPatientData patientInfo = new BedPatientData();
                                        fixbed = patientlist[0].IsFixedBed;
                                        bedid = patientlist[0].BedId;

                                    }
                                }

                                //var condition = new Dictionary<string, object>();
                                condition.Clear();
                                condition["PatientID"] = patientID;

                                foreach (var day in v.CurrentWeek.days)
                                //if(weeks == 0)
                                {
                                    //var day = v.CurrentWeek.days[days];
                                    if (day.Content != "" && day.Content != null)
                                    {
                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                        condition1["PatientId"] = patientID.ToString();
                                        condition1["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                        var list = scheduleDao.SelectScheduleTemplate(condition1);

                                        if (list != null && list.Count != 0)
                                        {

                                            var fileds = new Dictionary<string, object>();
                                            //fileds["DATE"] = day.dateTime.Date;
                                            condition["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                            fileds["AMPME"] = day.Content;
                                            fileds["METHOD"] = StrColorConverter(day.BgColor);
                                            /*if(fixbed == true)
                                                fileds["BEDID"] = bedid;
                                            else*/
                                                //fileds["BEDID"] = -1;
                                            if (list[0].AmPmE != day.Content)
                                                fileds["BEDID"] = -1;
                                            scheduleDao.UpdateScheduleTemplate(fileds, condition);
                                        }
                                        else
                                        {
                                            ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                            scheduleTemplate.PatientId = patientID;
                                            scheduleTemplate.Date = day.dateTime.ToString("yyyy-MM-dd");
                                            scheduleTemplate.AmPmE = day.Content;
                                            scheduleTemplate.Method = StrColorConverter(day.BgColor);
                                            if(fixbed == true)
                                            scheduleTemplate.BedId = bedid;
                                            else
                                            {
                                                scheduleTemplate.BedId = -1;
                                            }
                                            int ret = -1;
                                            scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);
                                        }
                                    }
                                    else if (day.Content == "")
                                    {
                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                        condition1["PatientId"] = patientID.ToString();
                                        condition1["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                        var list = scheduleDao.SelectScheduleTemplate(condition1);
                                        foreach (var l in list)
                                        {
                                            scheduleDao.DeleteScheduleTemplate((int)l.Id);
                                        }
                                    }
                                }

                                foreach (var day in v.NextWeek.days)
                                //if (weeks == 1)
                                {
                                    //var day = v.CurrentWeek.days[days];
                                    if (day.Content != "" && day.Content != null)
                                    {
                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                        condition1["PatientId"] = patientID.ToString();
                                        condition1["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                        var list = scheduleDao.SelectScheduleTemplate(condition1);

                                        if (list != null && list.Count != 0)
                                        {
                                            var fileds = new Dictionary<string, object>();
                                            //fileds["DATE"] = day.dateTime.Date;
                                            condition["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                            fileds["AMPME"] = day.Content;
                                            fileds["METHOD"] = StrColorConverter(day.BgColor);
                                            /*if (fixbed == true)
                                                fileds["BEDID"] = bedid;
                                            else*/
                                            {
                                                fileds["BEDID"] = -1;
                                            }
                                            scheduleDao.UpdateScheduleTemplate(fileds, condition);
                                        }
                                        else
                                        {
                                            ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                            scheduleTemplate.PatientId = patientID;
                                            scheduleTemplate.Date = day.dateTime.ToString("yyyy-MM-dd");
                                            scheduleTemplate.AmPmE = day.Content;
                                            scheduleTemplate.Method = StrColorConverter(day.BgColor);
                                            if (fixbed == true)
                                                scheduleTemplate.BedId = bedid;
                                            else
                                                scheduleTemplate.BedId = -1;
                                            int ret = -1;

                                            scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);
                                        }
                                    }
                                    else if (day.Content == "")
                                    {
                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                        condition1["PatientId"] = patientID.ToString();
                                        condition1["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                                        var list = scheduleDao.SelectScheduleTemplate(condition1);
                                        foreach (var l in list)
                                        {
                                            scheduleDao.DeleteScheduleTemplate((int)l.Id);
                                        }
                                    }
                                }
                                string dt = CheckBed(patientID);
                                if (dt == null)
                                {
                                    ListboxItemStatusesList[line].Bed = "";
                                }
                                else
                                {
                                    ListboxItemStatusesList[line].Bed = dt;
                                }

                                ListboxItemStatusesList[line].Checks = CheckOrders(patientID);

                                /*if (CheckBed(patientID))
                                    ListboxItemStatusesList[line].Bed = "正常";
                                else
                                {
                                    ListboxItemStatusesList[line].Bed = "异常";
                                }*/
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
            }
        }


        private void BeginDatePicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void EndatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
        }

        private void EndatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectoperation == 1)
            {
                TimeRadioButton1.IsChecked = false;
                TimeRadioButton2.IsChecked = false;
                TimeRadioButton3.IsChecked = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //UpdatePatientSchedule();
            //RefreshStatistics();
            CopySchedule();
        }
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            weekCount--;
            if (weekCount < 0)
            {
                ButtonEdit.IsEnabled = false;
            }
            else if( weekCount == 0)
            {
                ButtonEdit.IsEnabled = true;
                btnNextWeek.IsEnabled = true;
            }
            PreWeek();
        }


        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            weekCount++;
            if (weekCount < 0)
            {
                ButtonEdit.IsEnabled = false;
            }
            else if (weekCount == 0)
            {
                ButtonEdit.IsEnabled = true;
                btnNextWeek.IsEnabled = true;
            }
            else if( weekCount == 1)
            {
                ButtonEdit.IsEnabled = false;
                btnNextWeek.IsEnabled = false;
            }
            else
            {
                btnNextWeek.IsEnabled = false;
                weekCount = 1;
                return;
            }

            NextWeek();
            
        }

        private void PreWeek()
        {
            //int index = ListBox1.SelectedIndex;
            //if (index == -1) return;
            //ListboxItemStatus listboxItem = ListboxItemStatusesList[index];
            //listboxItem.PreWeek();
            foreach (var v in ListboxItemStatusesList)
            {
                v.PreWeek();
            }
            RefreshListbox();
            PreWeekOnLabel();
            RefreshStatistics();
        }

        private void PreWeekOnLabel()
        {
            for (int n = 0; n < 7; n++)
            {
                dtlist[n] = dtlist[n].AddDays(-7);
            }

            lable0.Content = dtlist[0].ToString("MM-dd");
            lable1.Content = dtlist[1].ToString("MM-dd");
            lable2.Content = dtlist[2].ToString("MM-dd");
            lable3.Content = dtlist[3].ToString("MM-dd");
            lable4.Content = dtlist[4].ToString("MM-dd");
            lable5.Content = dtlist[5].ToString("MM-dd");
            lable6.Content = dtlist[6].ToString("MM-dd");

            btn0.Content = dtlist[0].ToString("MM-dd");
            btn1.Content = dtlist[1].ToString("MM-dd");
            btn2.Content = dtlist[2].ToString("MM-dd");
            btn3.Content = dtlist[3].ToString("MM-dd");
            btn4.Content = dtlist[4].ToString("MM-dd");
            btn5.Content = dtlist[5].ToString("MM-dd");
            btn6.Content = dtlist[6].ToString("MM-dd");
        }

        private void NextWeek()
        {
            //int index = ListBox1.SelectedIndex;
            //if (index == -1) return;

            //if (ListboxItemStatusesList[0].CurrentWeek.days[0].dateTime.AddDays(7))
            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeek1();
            }

            RefreshListbox();
            NextWeekOnLabel();
            RefreshStatistics();
        }

        private void NextWeekOnLabel()
        {
            for (int n = 0; n < 7; n++)
            {
                dtlist[n] = dtlist[n].AddDays(7);
            }

            lable0.Content = dtlist[0].ToString("MM-dd");
            lable1.Content = dtlist[1].ToString("MM-dd");
            lable2.Content = dtlist[2].ToString("MM-dd");
            lable3.Content = dtlist[3].ToString("MM-dd");
            lable4.Content = dtlist[4].ToString("MM-dd");
            lable5.Content = dtlist[5].ToString("MM-dd");
            lable6.Content = dtlist[6].ToString("MM-dd");

            btn0.Content = dtlist[0].ToString("MM-dd");
            btn1.Content = dtlist[1].ToString("MM-dd");
            btn2.Content = dtlist[2].ToString("MM-dd");
            btn3.Content = dtlist[3].ToString("MM-dd");
            btn4.Content = dtlist[4].ToString("MM-dd");
            btn5.Content = dtlist[5].ToString("MM-dd");
            btn6.Content = dtlist[6].ToString("MM-dd");
            
        }
        private void InitWeekWithDate()
        {
            weekCount = 0;
            dtlist.Clear();
            int weeknow = (int)DateTime.Now.DayOfWeek-1;
            if (weeknow == -1)
                weeknow = 6;

            /*if (weeknow < 0)
            {
                for (int n = 0; n < 7; n++)
                {
                    dtlist.Add(DateTime.Now.AddDays(-weeknow + n));
                }
            }
            else*/
            {
                for (int n = 0; n < 7; n++)
                {
                    dtlist.Add(DateTime.Now.AddDays(-weeknow + n));
                }
            }

            lable0.Content = dtlist[0].ToString("MM-dd");
            lable1.Content = dtlist[1].ToString("MM-dd");
            lable2.Content = dtlist[2].ToString("MM-dd");
            lable3.Content = dtlist[3].ToString("MM-dd");
            lable4.Content = dtlist[4].ToString("MM-dd");
            lable5.Content = dtlist[5].ToString("MM-dd");
            lable6.Content = dtlist[6].ToString("MM-dd");

            btn0.Content = dtlist[0].ToString("MM-dd");
            btn1.Content = dtlist[1].ToString("MM-dd");
            btn2.Content = dtlist[2].ToString("MM-dd");
            btn3.Content = dtlist[3].ToString("MM-dd");
            btn4.Content = dtlist[4].ToString("MM-dd");
            btn5.Content = dtlist[5].ToString("MM-dd");
            btn6.Content = dtlist[6].ToString("MM-dd");

            btnPreWeek.IsEnabled = true;
            btnNextWeek.IsEnabled = true;
            //for (int n = 0; n < 7; n++)
            //{
            //    CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n);
            //    NextWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n + 7);
            //}
        }

        private void RefreshListbox()
        {
            try
            {

                foreach (var v in ListboxItemStatusesList)
                {
                    PatientSchedule schedule = GetPatientSchedule(v.PatientID);

                    for (int n = 0; n < 7; n++)
                    {
                        foreach (var h in schedule.Hemodialysis)
                        {

                            {
                                v.CurrentWeek.days[n].Content = "";
                                v.CurrentWeek.days[n].BgColor = Brushes.LightGray;
                            }

                            {
                                v.NextWeek.days[n].Content = "";
                                v.NextWeek.days[n].BgColor = Brushes.LightGray;
                            }
                        }
                    }

                    //foreach (var day in status.CurrentWeek.days)
                    for (int n = 0; n < 7; n++)
                    {
                        foreach (var h in schedule.Hemodialysis)
                        {
                            if (h.dialysisTime.dateTime == v.CurrentWeek.days[n].dateTime.Date)
                            {
                                v.CurrentWeek.days[n].Content = h.dialysisTime.AmPmE;
                                v.CurrentWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                            }
                            //else
                            //{
                            //    v.CurrentWeek.days[n].Content = "";
                            //    v.CurrentWeek.days[n].BgColor = Brushes.LightGray;
                            //}
                            if (h.dialysisTime.dateTime == v.NextWeek.days[n].dateTime.Date)
                            {
                                v.NextWeek.days[n].Content = h.dialysisTime.AmPmE;
                                v.NextWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                            }
                            //else
                            //{
                            //    v.NextWeek.days[n].Content = "";
                            //    v.NextWeek.days[n].BgColor = Brushes.LightGray;
                            //}
                        }
                    }
                }


                ListBox1.Items.Refresh();
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }
        }

        /*private void RefreshStatistics()
        {
            int dayofweek = (int)DateTime.Now.DayOfWeek;
            //dayofweek = 0;
            AmPanel.Children.Clear();
            Label lbam = new Label();
            lbam.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbam.VerticalContentAlignment = VerticalAlignment.Center;
            lbam.Width = 40;
            lbam.Content = "AM";
            AmPanel.Children.Add(lbam);
            Dictionary<string, int> AmDictionary = Statistics("AM", dayofweek);
            foreach (var v in AmDictionary)
            {
                Rectangle rect = new Rectangle();
                rect.Width = rect.Height = 28;
                rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(v.Key);
                rect.HorizontalAlignment = HorizontalAlignment.Left;
                AmPanel.Children.Add(rect);

                Label label = new Label();
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.Width = 40;
                label.Content = v.Value;
                AmPanel.Children.Add(label);
            }


            PmPanel.Children.Clear();
            Label lbpm = new Label();
            lbpm.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbpm.VerticalContentAlignment = VerticalAlignment.Center;
            lbpm.Width = 40;
            lbpm.Content = "PM";
            PmPanel.Children.Add(lbpm);
            Dictionary<string, int> PmDictionary = Statistics("PM", dayofweek);
            foreach (var v in PmDictionary)
            {
                Rectangle rect = new Rectangle();
                rect.Width = rect.Height = 28;
                rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(v.Key);
                rect.HorizontalAlignment = HorizontalAlignment.Left;
                PmPanel.Children.Add(rect);

                Label label = new Label();
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.Width = 40;
                label.Content = v.Value;
                PmPanel.Children.Add(label);
            }

            EPanel.Children.Clear();
            Label lbe = new Label();
            lbe.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbe.VerticalContentAlignment = VerticalAlignment.Center;
            lbe.Width = 40;
            lbe.Content = "E";
            EPanel.Children.Add(lbe);
            Dictionary<string, int> EDictionary = Statistics("E", dayofweek);
            foreach (var v in EDictionary)
            {
                Rectangle rect = new Rectangle();
                rect.Width = rect.Height = 28;
                rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(v.Key);
                rect.HorizontalAlignment = HorizontalAlignment.Left;
                EPanel.Children.Add(rect);

                Label label = new Label();
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.Width = 40;
                label.Content = v.Value;
                EPanel.Children.Add(label);
            }
            

        }*/

        private void CopySchedule( )
        {
            try
            {

                int dayOfWeek = (int)DateTime.Now.DayOfWeek;

                //return;
                if (dayOfWeek != 1) return;

                DateTime dtFrom = DateTime.Now.AddDays(-7);
                DateTime dtTo = DateTime.Now.AddDays(-1);
                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    //condition["TREATSTATUSID"] = 0;//在治
                    List<Patient> patientslist = patientDao.SelectPatient(condition);
                    if (patientslist.Count == 0) return;
                    ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao();
                    foreach (var patient in patientslist)
                    {
                        //if (patient.Id != 17) continue;
                        PatientSchedule schedule = GetPatientSchedule(patient.Id);
                        
                        //只要双周就复制
                        //List<string> uncopyed =  GetUnCopyOrder(patient.Id);
                        //List<Hemodialysy> newlist = new List<Hemodialysy>();
                        foreach (var v in schedule.Hemodialysis)
                        {
                        
                            if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 && DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                            {
                                //Hemodialysy newHemodialysy = new Hemodialysy();
                                //newHemodialysy = v;
                                //newHemodialysy.dialysisTime.dateTime = v.dialysisTime.dateTime.AddDays(14);
                                //newlist.Add(newHemodialysy);
                                //schedule.Hemodialysis.Add(newHemodialysy);

                                if (v.hemodialysisItem == "HD")
                                {
                                    //取当前的排班信息
                                    Dictionary<string, object> condition2 = new Dictionary<string, object>();
                                    condition2["PatientId"] = patient.Id;
                                    condition2["Date"] = v.dialysisTime.dateTime.ToString("yyyy-MM-dd");
                                    var list22 = scheduleDao.SelectScheduleTemplate(condition2);
                                    if (list22.Count == 1)
                                    {
                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                        condition1["PatientId"] = patient.Id;
                                        condition1["Date"] = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                                        var list = scheduleDao.SelectScheduleTemplate(condition1);
                                        if (list == null || list.Count == 0)
                                        {
                                            ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                            scheduleTemplate = list22[0];
                                            scheduleTemplate.Date = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");

                                            int ret = -1;
                                            scheduleDao.InsertScheduleTemplate(list22[0], ref ret);
                                        }
                                    }
                                }
                                else
                                

                                using (var methodDao = new TreatMethodDao())
                                {
                                    condition.Clear();
                                    condition = new Dictionary<string, object>();
                                    condition["NAME"] = v.hemodialysisItem;//获取治疗方法id
                                    var list1 = methodDao.SelectTreatMethod(condition);
                                    if (list1.Count == 1)
                                    {
                                        
                                        using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                                        {
                                            condition.Clear();
                                            condition["PATIENTID"] = patient.Id;//病人id
                                            condition["METHODID"] = list1[0].Id;//治疗方法id
                                            var list2 = medicalOrderDao.SelectMedicalOrder(condition);//获取医嘱类型 1 单周， 2 双周， 3月
                                            if (list2.Count == 1)
                                            {
                                                if (list2[0].Interval != 3) //周类型
                                                {
                                                    //取当前的排班信息
                                                    Dictionary<string, object> condition2 = new Dictionary<string, object>();
                                                    condition2["PatientId"] = patient.Id;
                                                    condition2["Date"] = v.dialysisTime.dateTime.ToString("yyyy-MM-dd");
                                                    var list22 = scheduleDao.SelectScheduleTemplate(condition2);
                                                    if (list22.Count == 1)
                                                    {
                                                        Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                                        condition1["PatientId"] = patient.Id;
                                                        condition1["Date"] = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                                                        var list = scheduleDao.SelectScheduleTemplate(condition1);
                                                        if (list == null || list.Count == 0)
                                                        {
                                                            /*ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                                            scheduleTemplate.PatientId = patient.Id;
                                                            scheduleTemplate.Date = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                                                            scheduleTemplate.AmPmE = v.dialysisTime.AmPmE;
                                                            scheduleTemplate.Method = v.hemodialysisItem;
                                                            scheduleTemplate.BedId = -1;
                                                            int ret = -1;
                                                            scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);*/
                                                            /*ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                                            scheduleTemplate.PatientId = patient.Id;
                                                            scheduleTemplate.Date = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                                                            scheduleTemplate.AmPmE = v.dialysisTime.AmPmE;
                                                            scheduleTemplate.Method = v.hemodialysisItem;
                                                            scheduleTemplate.BedId = -1;*/
                                                            ///////////////////////////////////////////
                                                            /// 需要重新拷贝日期
                                                            /// //////////////////////////////////////
                                                            /// 
                                                            /// 
                                                            ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                                            scheduleTemplate = list22[0];
                                                            scheduleTemplate.Date = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");

                                                            int ret = -1;
                                                            scheduleDao.InsertScheduleTemplate(list22[0], ref ret);
                                                        }
                                                    }
                                                    
                                                }
                                                else//月类型 
                                                {
                                                    Dictionary<string, object> condition3 = new Dictionary<string, object>();
                                                    condition3["PatientId"] = patient.Id;
                                                    condition3["Method"] = v.hemodialysisItem;
                                                    var list33 = scheduleDao.SelectScheduleTemplate(condition3);
                                                    bool bCopy = false;
                                                    DateTime copyTime = new DateTime();
                                                    foreach (var aa in list33)
                                                    {
                                                        DateTime dt = DateTime.Parse(aa.Date);
                                                        //if (IsInPreMonth(dt))
                                                        {
                                                            DateTime dt1 = dt.AddDays(28);
                                                            if (IsInCurrentMonth(dt1))
                                                            {
                                                                //if (IsInCurrentWeek(dt1))
                                                                {
                                                                    bCopy = true;
                                                                    copyTime = dt1;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DateTime dt2 = dt1.AddDays(7);
                                                                //if (IsInCurrentWeek(dt2))
                                                                {
                                                                    bCopy = true;
                                                                    copyTime = dt2;
                                                                }
                                                            }

                                                        }
                                                    }
                                                    
                                                    if(bCopy)
                                                    {
                                                        Dictionary<string, object> condition2 = new Dictionary<string, object>();
                                                        condition2["PatientId"] = patient.Id;
                                                        condition2["Date"] = v.dialysisTime.dateTime.ToString("yyyy-MM-dd");
                                                        var list22 = scheduleDao.SelectScheduleTemplate(condition2);
                                                        if (list22.Count == 1)
                                                        {
                                                            Dictionary<string, object> condition1 = new Dictionary<string, object>();
                                                            condition1["PatientId"] = patient.Id;
                                                            condition1["Date"] = copyTime.Date.ToString("yyyy-MM-dd");
                                                            var list = scheduleDao.SelectScheduleTemplate(condition1);
                                                            if (list == null || list.Count == 0)
                                                            {
                                                                ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                                                scheduleTemplate = list22[0];
                                                                scheduleTemplate.Date = copyTime.Date.ToString("yyyy-MM-dd");
                                                                int ret = -1;
                                                                scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);
                                                            }
                                                            else
                                                            {
                                                                ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                                                scheduleTemplate = list22[0];
                                                                scheduleTemplate.Date = copyTime.Date.ToString("yyyy-MM-dd");
                                                                int ret = -1;
                                                                condition.Clear();
                                                                condition["PatientId"] = scheduleTemplate.PatientId;
                                                                condition["Date"] = copyTime.Date.ToString("yyyy-MM-dd");

                                                                var fields = new Dictionary<string, object>();
                                                                fields["Method"] = scheduleTemplate.Method;

                                                                scheduleDao.UpdateScheduleTemplate(fields, condition);
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }

                                }



                                

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }



        }

        private bool IsInPreMonth(DateTime dt)
        {

            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            DateTime dtFrom = lastMonth.AddDays(1 - lastMonth.Day);
            DateTime dtTo = lastMonth.AddDays(1 - lastMonth.Day).AddMonths(1).AddDays(-1);


            /*DateTime dtNow = DateTime.Now;
            int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            DateTime dtFrom = dtNow.AddDays(-dtNow.Day + 1).AddMonths(-1);
            DateTime dtTo = dtNow.AddDays(-dtNow.Day + days ).AddMonths(-1);*/
            if (DateTime.Compare(dt.Date, dtFrom.Date) >= 0 && DateTime.Compare(dt.Date, dtTo.Date) <= 0)
            {
                return true;
            }
            else
                return false;
        }
        private bool IsInCurrentMonth( DateTime dt )
        {
            DateTime dtNow = DateTime.Now; 
            int days = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            DateTime dtFrom = dtNow.AddDays(-dtNow.Day + 1);
            DateTime dtTo = dtFrom.AddDays(days - 1);
            if (DateTime.Compare(dt.Date, dtFrom.Date) >= 0 && DateTime.Compare(dt.Date, dtTo.Date) <= 0)
            {
                return true;
            }
            else
                return false;

        }

        private bool IsInCurrentWeek(DateTime dt)
        {
            int dayofweek = (int)DateTime.Now.DayOfWeek - 1;
            if (dayofweek == -1) dayofweek = 6;
            DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
            DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 6);

            if (DateTime.Compare(dt.Date, dtFrom.Date) >= 0 && DateTime.Compare(dt.Date, dtTo.Date) <= 0)
            {
                return true;
            }
            else
                return false;
        }

        private void RefreshStatistics()
        {
            StatisticsGrid.Children.Clear();

            Label label0 = new Label();
            label0.HorizontalContentAlignment = HorizontalAlignment.Center;
            label0.VerticalContentAlignment = VerticalAlignment.Center;
            //label0.Height = label0.Width = 15;
            label0.Content = "AM";
            label0.HorizontalAlignment = HorizontalAlignment.Center;
            label0.VerticalAlignment = VerticalAlignment.Center;

            Label label1 = new Label();
            label1.HorizontalContentAlignment = HorizontalAlignment.Center;
            label1.VerticalContentAlignment = VerticalAlignment.Center;
            //label1.Height = label1.Width = 15;
            label1.Content = "PM";
            label1.HorizontalAlignment = HorizontalAlignment.Center;
            label1.VerticalAlignment = VerticalAlignment.Center;

            Label label2 = new Label();
            label2.HorizontalContentAlignment = HorizontalAlignment.Center;
            label2.VerticalContentAlignment = VerticalAlignment.Center;
            //label2.Height = label2.Width = 15;
            label2.Content = "E";
            label2.HorizontalAlignment = HorizontalAlignment.Center;
            label2.VerticalAlignment = VerticalAlignment.Center;

            Border border = new Border();
            border.BorderBrush = Brushes.DodgerBlue;
            border.BorderThickness = new Thickness(0, 1, 0, 1);
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Stretch;

            StatisticsGrid.Children.Add(label0);
            StatisticsGrid.Children.Add(label1);
            StatisticsGrid.Children.Add(label2);
            StatisticsGrid.Children.Add(border);

            label0.SetValue(Grid.RowProperty, 0);
            label0.SetValue(Grid.ColumnProperty, 0);
            label0.SetValue(Grid.ColumnSpanProperty, 2);

            label1.SetValue(Grid.RowProperty, 1);
            label1.SetValue(Grid.ColumnProperty, 0);
            label1.SetValue(Grid.ColumnSpanProperty, 2);

            label2.SetValue(Grid.RowProperty, 2);
            label2.SetValue(Grid.ColumnProperty, 0);
            label2.SetValue(Grid.ColumnSpanProperty, 2);

            border.SetValue(Grid.RowProperty, 1);
            border.SetValue(Grid.ColumnProperty, 0);
            border.SetValue(Grid.ColumnSpanProperty, 20);

            int dayofweek = (int)DateTime.Now.DayOfWeek;
            string ampme = "";
            for(int o = 0; o< 3; o++)
            {
                if (o == 0)
                    ampme = "AM";
                else if (o == 1)
                    ampme = "PM";
                else
                    ampme = "E";
                for (int m = 0; m < 2; m++)
                {
                    if (m == 1 && IsEditable == false) continue;
                    for (int n = 0; n < 7; n++)
                    {
                        Dictionary<string, int> dictionary = Statistics(ampme, n, m);//ampme=="AM"&&n==5&&m==0
                        if (dictionary == null)
                            continue;
                        StackPanel panel = new StackPanel();
                        int DoublePump = 0;
                        int SinglePump = 0;
                        StackPanel panel1 = new StackPanel();
                        panel1.Orientation = Orientation.Vertical;
                        foreach (var v in dictionary)
                        {
                            
                            
                            /*Rectangle rect = new Rectangle();
                            rect.Width = rect.Height = 15;
                            rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(v.Key);
                            rect.HorizontalAlignment = HorizontalAlignment.Left;
                            Label label = new Label();
                            label.HorizontalContentAlignment = HorizontalAlignment.Center;
                            label.VerticalContentAlignment = VerticalAlignment.Center;
                            label.Content = v.Value;

                            panel1.Children.Add(rect);
                            panel1.Children.Add(label);*/

                            Brush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(v.Key);
                            string mathod = StrColorConverter(brush);


                            using (var methodDao = new TreatMethodDao())
                            {
                                
                                var condition = new Dictionary<string, object>();
                                condition["NAME"] = mathod;
                                var list = methodDao.SelectTreatMethod(condition);
                                if (list.Count == 1)
                                {
                                    if (list[0].DoublePump == true && list[0].SinglePump == false)
                                    {
                                        DoublePump+=v.Value;
                                    }
                                    else if (list[0].SinglePump == true && list[0].DoublePump == false)
                                    {
                                        SinglePump += v.Value;
                                    }
                                    else if (list[0].DoublePump == true && list[0].SinglePump == true)
                                    {
                                        SinglePump += v.Value;
                                    }

                                }
                            }


                            /*Label label = new Label();
                            label.HorizontalContentAlignment = HorizontalAlignment.Center;
                            label.VerticalContentAlignment = VerticalAlignment.Center;
                            label.Content = mathod + "/" + v.Value;

                            //panel1.Children.Add(rect);
                            panel1.Children.Add(label);

                            panel.Children.Add(panel1);*/

                        }
                        Label slabel = new Label();
                        slabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                        slabel.VerticalContentAlignment = VerticalAlignment.Center;
                        slabel.Content = "单泵" + SinglePump;
                        panel1.Children.Add(slabel);

                        Label dlabel = new Label();
                        dlabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                        dlabel.VerticalContentAlignment = VerticalAlignment.Center;
                        dlabel.Content = "双泵" + DoublePump;
                        panel1.Children.Add(dlabel);


                        panel.Children.Add(panel1);


                        StatisticsGrid.Children.Add(panel);

                        panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        panel.VerticalAlignment = VerticalAlignment.Stretch;
                        panel.SetValue(Grid.RowProperty, o);
                        panel.SetValue(Grid.ColumnProperty, n*2 + 2 + m);

                    }
                }
            }
        }
        private Dictionary<string, int> Statistics(string condition, int dayofweek )
        {
            try
            {
                
                //Dictionary<Brush, int> dictionary = new Dictionary<Brush, int>();
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                foreach (var v in ListboxItemStatusesList)
                {
                    //PatientSchedule schedule = GetPatientSchedule(v.PatientID);
                    string ampme = v.CurrentWeek.days[dayofweek].Content;
                    //Brush bgBrush = v.CurrentWeek.days[dayofweek].BgColor;
                    string bgBrush = v.CurrentWeek.days[dayofweek].BgColor.ToString();

                    if (ampme == condition)
                    {
                        if (dictionary.ContainsKey(bgBrush) == false)
                        {
                            dictionary.Add(bgBrush, 1);
                        }
                        else
                            dictionary[bgBrush]++;
                    }
                    //v.CurrentWeek.days[dayofweek].BgColor = Brushes.LightGray;
                    //v.NextWeek.days[dayofweek].Content = "";
                    //v.NextWeek.days[dayofweek].BgColor = Brushes.LightGray;
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Statistics.xaml.cs:Statistics exception messsage: " + ex.Message);
                return null;
            }
        }

        private Dictionary<string, int> Statistics(string condition, int dayofweek, int preOrNext)
        {
            try
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                foreach (var v in ListboxItemStatusesList)
                {
                    string ampme = "";
                    string bgBrush = "";
                    if (preOrNext == 0)
                    {
                        ampme = v.CurrentWeek.days[dayofweek].Content;
                        bgBrush = v.CurrentWeek.days[dayofweek].BgColor.ToString();
                    }
                    else
                    {
                        ampme = v.NextWeek.days[dayofweek].Content;
                        bgBrush = v.NextWeek.days[dayofweek].BgColor.ToString();
                    }

                    if (ampme == condition)
                    {
                        if (dictionary.ContainsKey(bgBrush) == false)
                        {
                            dictionary.Add(bgBrush, 1);
                        }
                        else
                            dictionary[bgBrush]++;
                    }

                }
                return dictionary;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Statistics.xaml.cs:Statistics exception messsage: " + ex.Message);
                return null;
            }
        }

        private void ListSortByName(string rule)
        {
            if (!string.IsNullOrEmpty(rule) && (!rule.ToLower().Equals("desc") || !rule.ToLower().Equals("asc")))
            {
                try
                {
                    ListboxItemStatusesList.Sort(
                        delegate(ListboxItemStatus info1, ListboxItemStatus info2)
                        {
                            /*Type t1 = info1.GetType();
                            Type t2 = info2.GetType();
                            PropertyInfo pro1 = t1.GetProperty(field);
                            PropertyInfo pro2 = t2.GetProperty(field);
                            return rule.ToLower().Equals("asc") ?
                                pro1.GetValue(info1.CurrentWeek.days[n].Content, null).ToString().CompareTo(pro2.GetValue(info2.CurrentWeek.days[n].Content, null).ToString()) :
                                pro2.GetValue(info2.CurrentWeek.days[n].Content, null).ToString().CompareTo(pro1.GetValue(info1.CurrentWeek.days[n].Content, null).ToString());*/



                            return rule.ToLower().Equals("asc") ?
                            info1.PatientName.CompareTo(info2.PatientName) :
                            info2.PatientName.CompareTo(info1.PatientName);


                        });

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            } //Console.WriteLine("ruls is wrong");

        }

        private void ListSort(string rule, int week, int day)
        {
            if (!string.IsNullOrEmpty(rule) && (!rule.ToLower().Equals("desc") || !rule.ToLower().Equals("asc")))
            {
                try
                {
                    ListboxItemStatusesList.Sort(
                        delegate(ListboxItemStatus info1, ListboxItemStatus info2)
                        {
                            /*Type t1 = info1.GetType();
                            Type t2 = info2.GetType();
                            PropertyInfo pro1 = t1.GetProperty(field);
                            PropertyInfo pro2 = t2.GetProperty(field);
                            return rule.ToLower().Equals("asc") ?
                                pro1.GetValue(info1.CurrentWeek.days[n].Content, null).ToString().CompareTo(pro2.GetValue(info2.CurrentWeek.days[n].Content, null).ToString()) :
                                pro2.GetValue(info2.CurrentWeek.days[n].Content, null).ToString().CompareTo(pro1.GetValue(info1.CurrentWeek.days[n].Content, null).ToString());*/


                            if (week == 0)
                            {
                                return rule.ToLower().Equals("asc") ?
                                info1.CurrentWeek.days[day].Content.CompareTo(info2.CurrentWeek.days[day].Content) :
                                info2.CurrentWeek.days[day].Content.CompareTo(info1.CurrentWeek.days[day].Content);
                            }
                            else
                            {
                                return rule.ToLower().Equals("asc") ?
                                info1.NextWeek.days[day].Content.CompareTo(info2.NextWeek.days[day].Content) :
                                info2.NextWeek.days[day].Content.CompareTo(info1.NextWeek.days[day].Content);
                            }
                            
                        });

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            } //Console.WriteLine("ruls is wrong");

        }
        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static bool de = false;
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            string tag = (string)btn.Tag;

            if (tag != "name")
            {
                string[] tags = tag.Split('_');

                int week = int.Parse(tags[0]);
                int day = int.Parse(tags[1]);
                if (de == false)
                {
                    ListSort("asc", week, day);
                    de = true;
                }
                else
                {
                    ListSort("desc", week, day);
                    de = false;
                }
            }
            else
            {
                if (de == false)
                {
                    ListSortByName("asc");
                    de = true;
                }
                else
                {
                    ListSortByName("desc");
                    de = false;
                }
            }
            ListBox1.Items.Refresh();


            ListSortDirection sortDirection = ListSortDirection.Ascending;
            /*ListSortDirection sortDirection = ListSortDirection.Ascending;

            Button btn = (Button)sender;

            string tag = (string)btn.Tag;
            //Get binding property of clicked column
            //string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path; //得到单击列所绑定的属性
            string bindingProperty = "";
            if (tag == "name")
            {
                if (Paixiflag[1] == 0)
                {
                    Paixiflag[1] = 1;
                    sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    Paixiflag[1] = 0;
                    sortDirection = ListSortDirection.Descending;
                }
                bindingProperty = "PatientName";

            }
            else if (btn.Uid == "1")
            {
                if (Paixiflag[2] == 0)
                {
                    Paixiflag[2] = 1;
                    sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    Paixiflag[2] = 0;
                    sortDirection = ListSortDirection.Descending;
                }
                bindingProperty = "InfectionType";

            }

            SortDescriptionCollection sdc = PatientListBox1.Items.SortDescriptions;
            if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }

            sdc.Add(new SortDescription(bindingProperty, sortDirection));
            var temp = new ObservableCollection<BedPatientData>();
            for (int i = 0; i < PatientListBox1.Items.Count; i++)
            {
                temp.Add((BedPatientData)PatientListBox1.Items[i]);
            }
            BedPatientList.Clear();
            BedPatientList = temp;
            PatientListBox1.ItemsSource = BedPatientList;
            sdc.Clear();*/
        }

        private void HideOutherImages(Button btn)
        {
            foreach (var i in SortGrid.Children)
            {
                if( i is Grid)
                {

                    foreach (var j in ((Grid)i).Children)
                    {
                        if ((j is Button) && j != btn)
                        {
                            Image img = FindChild((Button) j);
                            img.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        public static Image FindChild(DependencyObject p)
        {
            Image f = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(p); i++)
            {
                var ch = VisualTreeHelper.GetChild(p, i);
                Image childType = ch as Image;
                if (childType == null)
                {
                    f = FindChild(ch);
                    if (f != null) break;
                }
                else
                {
                    var e = ch as FrameworkElement;

                    if (e != null)
                    {
                        f = (Image)ch;
                        break;
                    }
                }
            }
            return f;
        }


        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            //ListboxItemStatusesList

            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeekVisible = Visibility.Visible;
                
            }
            IsEditable = true;
            ListBox1.Items.Refresh();
            RefreshStatistics();
            ButtonCancel.IsEnabled = true;
            ButtonApply.IsEnabled = true;
            ButtonEdit.IsEnabled = false;

            btnPreWeek.IsEnabled = false;
            btnNextWeek.IsEnabled = false;
            PatientGroupComboBox.IsEnabled = false;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {

            var messageBox2 = new RemindMessageBox2();
            messageBox2.textBlock1.Text = "退出不会保存当前更改，是否退出！";
            messageBox2.ShowDialog();
            if (messageBox2.remindflag != 1)
            {
                return;
            }
            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeekVisible = Visibility.Hidden;

            }
            IsEditable = false;
            RefreshShedule();
            RefreshStatistics();
            //ListboxItemStatusesList.Clear();
            //Reload();
            ButtonCancel.IsEnabled = false;
            ButtonApply.IsEnabled = false;
            ButtonEdit.IsEnabled = true;

            btnPreWeek.IsEnabled = true;
            btnNextWeek.IsEnabled = true;
            PatientGroupComboBox.IsEnabled = true;
        }
        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var v in ListboxItemStatusesList)
            {
                v.NextWeekVisible = Visibility.Hidden;

            }
            IsEditable = false;
            UpdatePatientSchedule();
            ListBox1.Items.Refresh();
            RefreshStatistics();
            ModifiedList.Clear();
            ButtonCancel.IsEnabled = false;
            ButtonApply.IsEnabled = false;
            ButtonEdit.IsEnabled = true;

            btnPreWeek.IsEnabled = true;
            btnNextWeek.IsEnabled = true;
            PatientGroupComboBox.IsEnabled = true;
        }

        public void RefreshShedule()
        {
            try
            {
                int index = PatientGroupComboBox.SelectedIndex;
                if (index == -1) return;
                ListboxItemStatusesList.Clear();
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["NAME"] = PatientGroupComboBoxItems[index];
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

                                        PatientInfo patientInfo = new PatientInfo();
                                        patientInfo.PatientId = patient.Id;
                                        patientInfo.PatientName = patient.Name;
                                        patientInfo.PatientDob = patient.Dob;
                                        patientInfo.PatientPatientId = patient.PatientId;
                                        patientInfo.PatientGender = patient.Gender;
                                        patientInfo.PatientMobile = patient.Mobile;
                                        {
                                            using (var infectTypeDao = new InfectTypeDao())
                                            {
                                                condition.Clear();
                                                condition["ID"] = patient.InfectTypeId;
                                                var arealist = infectTypeDao.SelectInfectType(condition);
                                                if (arealist.Count == 1)
                                                {
                                                    patientInfo.PatientInfectType = arealist[0].Name;
                                                }
                                            }
                                        }
                                        {
                                            using (var treatStatusDao = new TreatStatusDao())
                                            {
                                                condition.Clear();
                                                condition["ID"] = patient.TreatStatusId;
                                                var arealist = treatStatusDao.SelectTreatStatus(condition);
                                                if (arealist.Count == 1)
                                                {
                                                    patientInfo.PatientTreatStatus = arealist[0].Name;
                                                }
                                            }
                                        }
                                        patientInfo.PatientRegesiterDate = patient.RegisitDate;
                                        patientInfo.PatientIsFixedBed = patient.IsFixedBed;
                                        patientInfo.PatientIsAssigned = patient.IsAssigned;
                                        patientInfo.PatientDescription = patient.Description;

                                        ListboxItemStatus status = new ListboxItemStatus();
                                        status.PatientID = patientInfo.PatientId;
                                        status.PatientName = patientInfo.PatientName;
                                        PatientSchedule schedule = GetPatientSchedule(patientInfo.PatientId);

                                        //foreach (var day in status.CurrentWeek.days)
                                        for (int n = 0; n < 7; n++)
                                        {
                                            foreach (var h in schedule.Hemodialysis)
                                            {
                                                if (h.dialysisTime.dateTime == status.CurrentWeek.days[n].dateTime.Date)
                                                {
                                                    status.CurrentWeek.days[n].Content = h.dialysisTime.AmPmE;
                                                    status.CurrentWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                                }
                                                if (h.dialysisTime.dateTime == status.NextWeek.days[n].dateTime.Date)
                                                {
                                                    status.NextWeek.days[n].Content = h.dialysisTime.AmPmE;
                                                    status.NextWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                                }
                                            }
                                        }

                                        /*string order = CheckOrders(patientInfo.PatientId);
                                        if (order == null)
                                        {
                                            status.Checks = "";
                                        }
                                        else
                                        {
                                            status.Checks = order;
                                        }*/
                                        status.Checks = CheckOrders(patientInfo.PatientId);
                                        /*if (CheckOrders(patientInfo.PatientId))
                                            status.Checks = "正常";
                                        else
                                        {
                                            status.Checks = "异常";
                                        }*/

                                        //status.Checks = "world";
                                        string dt = CheckBed(patientInfo.PatientId);
                                        if (dt == null)
                                        {
                                            status.Bed = "";
                                        }
                                        else
                                        {
                                            status.Bed = dt;
                                        }
                                        //status.Bed = "hello";
                                        /*if (CheckBed(patientInfo.PatientId))
                                            status.Bed = "正常";
                                        else
                                        {
                                            status.Bed = "异常";
                                        }*/


                                        List<TreatOrder> TreatOrderList = new List<TreatOrder>();
                                        try
                                        {
                                            using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                                            {

                                                condition.Clear();

                                                condition["PATIENTID"] = patient.Id;
                                                condition["ACTIVATED"] = true;
                                                var list3 = medicalOrderDao.SelectMedicalOrder(condition);

                                                foreach (MedicalOrder medicalOrder in list3)
                                                {
                                                    TreatOrder treatOrder = new TreatOrder();
                                                    treatOrder.Id = medicalOrder.Id;
                                                    treatOrder.Activated = medicalOrder.Activated;
                                                    treatOrder.Seq = medicalOrder.Seq;
                                                    treatOrder.Plan = medicalOrder.Plan;

                                                    treatOrder.TreatTimes = (int)medicalOrder.Times;
                                                    treatOrder.Description = medicalOrder.Description;

                                                    if (medicalOrder.MethodId != -1)
                                                    {
                                                        using (var treatMethodDao = new TreatMethodDao())
                                                        {
                                                            condition.Clear();
                                                            condition["ID"] = (int)medicalOrder.MethodId;
                                                            var arealist = treatMethodDao.SelectTreatMethod(condition);
                                                            if (arealist.Count == 1)
                                                            {
                                                                treatOrder.TreatMethod = arealist[0].Name;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        treatOrder.TreatMethod = "NULL";
                                                    }
                                                    {
                                                        using (var medicalOrderParaDao = new MedicalOrderParaDao())
                                                        {
                                                            condition.Clear();
                                                            condition["ID"] = medicalOrder.Interval;
                                                            var arealist = medicalOrderParaDao.SelectInterval(condition);
                                                            if (arealist.Count == 1)
                                                            {
                                                                treatOrder.Type = arealist[0].Name;
                                                            }
                                                        }
                                                    }

                                                    TreatOrderList.Add(treatOrder);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MainWindow.Log.WriteInfoConsole("In Order.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
                                        }

                                        string treatOrders = "";

                                        if (TreatOrderList.Count != 0)
                                        {
                                            foreach (var treatOrder in TreatOrderList)
                                            {
                                                string str = "";
                                                if (treatOrder.Plan == "频次")
                                                {
                                                    str = "频次";
                                                }
                                                else
                                                {
                                                    str = treatOrder.TreatMethod;
                                                }
                                                str += "/";
                                                str += treatOrder.Type;
                                                str += "/";
                                                str += treatOrder.TreatTimes;
                                                str += "/";
                                                str += treatOrder.Description;
                                                str += "\n";

                                                treatOrders += str;
                                            }
                                            treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                                            status.ToolTips = treatOrders;
                                        }
                                        else
                                        {
                                            status.ToolTips = null;
                                        }


                                        /*string treatOrders = "";
                                        string orders = patient.Orders;
                                        if (orders != "" && orders != null)
                                        {
                                            string[] order = orders.Split('#');
                                            foreach (var s in order)
                                            {
                                                if (s != "")
                                                {
                                                    string[] details = s.Split('/');
                                                    if (details.Count() == 3)
                                                    {
                                                        var treat = new TreatOrder();
                                                        treat.TreatMethod = details[0];

                                                        var medicalOrderParaDao = new MedicalOrderParaDao();
                                                        var condition1 = new Dictionary<string, object>();
                                                        condition1["ID"] = details[1];
                                                        var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                                        string temporder;
                                                        treat.Type = list1[0].Name;
                                                        treat.TreatTimes = int.Parse(details[2]);
                                                        temporder = treat.Type + "/" + treat.TreatTimes + "/" + treat.TreatMethod;
                                                        treatOrders += temporder;
                                                        treatOrders += "\n";
                                                    }
                                                }
                                            }
                                            treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                                            status.ToolTips = treatOrders;
                                        }
                                        else
                                        {
                                            status.ToolTips = "";
                                        }*/


                                        ListboxItemStatusesList.Add(status);
                                    }
                                }
                            }
                        }
                    }

                }

                ListBox1.Items.Refresh();
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }
        }
        private void PatientGroupComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                int index = PatientGroupComboBox.SelectedIndex;
                if (index == -1) return;
                ListboxItemStatusesList.Clear();
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["NAME"] = PatientGroupComboBoxItems[index];
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
                                    
                                        PatientInfo patientInfo = new PatientInfo();
                                        patientInfo.PatientId = patient.Id;
                                        patientInfo.PatientName = patient.Name;
                                        patientInfo.PatientDob = patient.Dob;
                                        patientInfo.PatientPatientId = patient.PatientId;
                                        patientInfo.PatientGender = patient.Gender;
                                        patientInfo.PatientMobile = patient.Mobile;
                                        {
                                            using (var infectTypeDao = new InfectTypeDao())
                                            {
                                                condition.Clear();
                                                condition["ID"] = patient.InfectTypeId;
                                                var arealist = infectTypeDao.SelectInfectType(condition);
                                                if (arealist.Count == 1)
                                                {
                                                    patientInfo.PatientInfectType = arealist[0].Name;
                                                }
                                            }
                                        }
                                        {
                                            using (var treatStatusDao = new TreatStatusDao())
                                            {
                                                condition.Clear();
                                                condition["ID"] = patient.TreatStatusId;
                                                var arealist = treatStatusDao.SelectTreatStatus(condition);
                                                if (arealist.Count == 1)
                                                {
                                                    patientInfo.PatientTreatStatus = arealist[0].Name;
                                                }
                                            }
                                        }
                                        patientInfo.PatientRegesiterDate = patient.RegisitDate;
                                        patientInfo.PatientIsFixedBed = patient.IsFixedBed;
                                        patientInfo.PatientIsAssigned = patient.IsAssigned;
                                        patientInfo.PatientDescription = patient.Description;

                                        ListboxItemStatus status = new ListboxItemStatus();
                                        status.PatientID = patientInfo.PatientId;
                                        status.PatientName = patientInfo.PatientName;
                                        PatientSchedule schedule = GetPatientSchedule(patientInfo.PatientId);

                                        //foreach (var day in status.CurrentWeek.days)
                                        for (int n = 0; n < 7; n++)
                                        {
                                            foreach (var h in schedule.Hemodialysis)
                                            {
                                                if (h.dialysisTime.dateTime == status.CurrentWeek.days[n].dateTime.Date)
                                                {
                                                    status.CurrentWeek.days[n].Content = h.dialysisTime.AmPmE;
                                                    status.CurrentWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                                }
                                                if (h.dialysisTime.dateTime == status.NextWeek.days[n].dateTime.Date)
                                                {
                                                    status.NextWeek.days[n].Content = h.dialysisTime.AmPmE;
                                                    status.NextWeek.days[n].BgColor = new SolidColorBrush(StrColorConverter(h.hemodialysisItem));
                                                }
                                            }
                                        }

                                        /*string order = CheckOrders(patientInfo.PatientId);
                                        if (order == null)
                                        {
                                            status.Checks = "";
                                        }
                                        else
                                        {
                                            status.Checks = order;
                                        }*/
                                        status.Checks = CheckOrders(patientInfo.PatientId);
                                        /*if (CheckOrders(patientInfo.PatientId))
                                            status.Checks = "正常";
                                        else
                                        {
                                            status.Checks = "异常";
                                        }*/

                                        //status.Checks = "world";
                                        string dt = CheckBed(patientInfo.PatientId);
                                        if (dt == null)
                                        {
                                            status.Bed = "";
                                        }
                                        else
                                        {
                                            status.Bed = dt;
                                        }
                                        //status.Bed = "hello";
                                        /*if (CheckBed(patientInfo.PatientId))
                                            status.Bed = "正常";
                                        else
                                        {
                                            status.Bed = "异常";
                                        }*/


                                        List<TreatOrder> TreatOrderList = new List<TreatOrder>();
                                        try
                                        {
                                            using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                                            {

                                                condition.Clear();

                                                condition["PATIENTID"] = patient.Id;
                                                condition["ACTIVATED"] = true;
                                                var list3 = medicalOrderDao.SelectMedicalOrder(condition);

                                                foreach (MedicalOrder medicalOrder in list3)
                                                {
                                                    TreatOrder treatOrder = new TreatOrder();
                                                    treatOrder.Id = medicalOrder.Id;
                                                    treatOrder.Activated = medicalOrder.Activated;
                                                    treatOrder.Seq = medicalOrder.Seq;
                                                    treatOrder.Plan = medicalOrder.Plan;

                                                    treatOrder.TreatTimes = (int)medicalOrder.Times;
                                                    treatOrder.Description = medicalOrder.Description;

                                                    if (medicalOrder.MethodId != -1)
                                                    {
                                                        using (var treatMethodDao = new TreatMethodDao())
                                                        {
                                                            condition.Clear();
                                                            condition["ID"] = (int)medicalOrder.MethodId;
                                                            var arealist = treatMethodDao.SelectTreatMethod(condition);
                                                            if (arealist.Count == 1)
                                                            {
                                                                treatOrder.TreatMethod = arealist[0].Name;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        treatOrder.TreatMethod = "NULL";
                                                    }
                                                    {
                                                        using (var medicalOrderParaDao = new MedicalOrderParaDao())
                                                        {
                                                            condition.Clear();
                                                            condition["ID"] = medicalOrder.Interval;
                                                            var arealist = medicalOrderParaDao.SelectInterval(condition);
                                                            if (arealist.Count == 1)
                                                            {
                                                                treatOrder.Type = arealist[0].Name;
                                                            }
                                                        }
                                                    }

                                                    TreatOrderList.Add(treatOrder);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MainWindow.Log.WriteInfoConsole("In Order.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
                                        }

                                        string treatOrders = "";

                                        if (TreatOrderList.Count!= 0)
                                        {
                                            foreach (var treatOrder in TreatOrderList)
                                            {
                                                string str = "";
                                                if (treatOrder.Plan == "频次")
                                                {
                                                    str = "频次";
                                                }
                                                else
                                                {
                                                    str = treatOrder.TreatMethod;
                                                }
                                                str += "/";
                                                str += treatOrder.Type;
                                                str += "/";
                                                str += treatOrder.TreatTimes;
                                                str += "/";
                                                str += treatOrder.Description;
                                                str += "\n";

                                                treatOrders += str;
                                            }
                                            treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                                            status.ToolTips = treatOrders;
                                        }
                                        else
                                        {
                                            status.ToolTips = null;
                                        }
                                        
                                        
                                        /*string treatOrders = "";
                                        string orders = patient.Orders;
                                        if (orders != "" && orders != null)
                                        {
                                            string[] order = orders.Split('#');
                                            foreach (var s in order)
                                            {
                                                if (s != "")
                                                {
                                                    string[] details = s.Split('/');
                                                    if (details.Count() == 3)
                                                    {
                                                        var treat = new TreatOrder();
                                                        treat.TreatMethod = details[0];

                                                        var medicalOrderParaDao = new MedicalOrderParaDao();
                                                        var condition1 = new Dictionary<string, object>();
                                                        condition1["ID"] = details[1];
                                                        var list1 = medicalOrderParaDao.SelectInterval(condition1);
                                                        string temporder;
                                                        treat.Type = list1[0].Name;
                                                        treat.TreatTimes = int.Parse(details[2]);
                                                        temporder = treat.Type + "/" + treat.TreatTimes + "/" + treat.TreatMethod;
                                                        treatOrders += temporder;
                                                        treatOrders += "\n";
                                                    }
                                                }
                                            }
                                            treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                                            status.ToolTips = treatOrders;
                                        }
                                        else
                                        {
                                            status.ToolTips = "";
                                        }*/


                                        ListboxItemStatusesList.Add(status);
                                    }
                                }
                            }
                        }
                    }
                
                }
                
                ListBox1.Items.Refresh();
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }

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
                MainWindow.Log.WriteInfoConsole("In Shedule.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

            RefreshStatistics();
        }
    }



    public class ListboxItemStatus:INotifyPropertyChanged
    {
        public long PatientID
        {
            get { return patientID; }
            set { patientID = value; }
        }

        public Brush PatientNameBgColor
        {
            get { return new SolidColorBrush(Color.FromArgb(255,61, 193, 109 )); }
            //set { patientName = value; }
        }
        public string PatientName
        {
            get { return patientName; }
            set
            {
                patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        public string Checks
        {
            get { return checks; }
            set
            {
                checks = value;
                OnPropertyChanged("Checks");
            }
        }
        public string Bed
        {
            get { return bed; }
            set
            {
                bed = value;
                OnPropertyChanged("Bed");
            }
        }

        public string ToolTips
        {
            get { return _tooltops; }
            set
            {
                _tooltops = value;
                OnPropertyChanged("ToolTips");
            }
       }

        public Week CurrentWeek
        {
            get { return _currentWeek; }
            set
            {
                _currentWeek = value;
                OnPropertyChanged("CurrentWeek");
            }
        }

        public Week NextWeek
        {
            get { return _nextWeek; }
            set
            {
                _nextWeek = value;
                OnPropertyChanged("NextWeek");

            }
        }

        public Visibility NextWeekVisible
        {
            get { return _nextWeekVisible; }
            set
            {
                _nextWeekVisible = value;
                OnPropertyChanged("NextWeekVisible");

            }
       }

        public Visibility CurrentWeekVisible
        {
            get { return _currentWeekVisible; }
            set
            {
                _currentWeekVisible = value;
                OnPropertyChanged("CurrentWeekVisible");
            }
        }

        private long patientID;
        private string patientName;
        private string checks;
        private string bed;
        private Visibility _currentWeekVisible;
        private Visibility _nextWeekVisible;
        private Week _currentWeek;
        private Week _nextWeek;
        private string _tooltops;




        public ListboxItemStatus()
        {
            CurrentWeek = new Week();
            NextWeek = new Week();
            InitWeekWithDate();
            CurrentWeekVisible = Visibility.Visible;
            NextWeekVisible = Visibility.Hidden;
        }

        private void InitWeekWithDate()
        {
            int weeknow = (int)DateTime.Now.DayOfWeek-1;
            if (weeknow == -1)
                weeknow = 6;

            //if (weeknow < 0)
            //{
            //    for (int n = 6; n >=0; n--)
            //    {
            //        CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-n);
            //        NextWeek.days[n].dateTime = DateTime.Now.AddDays(-n + 7);
            //    }
            //}
            //else
            {
                for (int n = 0; n < 7; n++)
                {
                    CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n);
                    NextWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n + 7);
                }
            }
        }

        public void PreWeek()
        {
            for (int n = 0; n < 7; n++)
            {
                CurrentWeek.days[n].dateTime = CurrentWeek.days[n].dateTime.AddDays(-7);
                NextWeek.days[n].dateTime = NextWeek.days[n].dateTime.AddDays(-7);
            }
        }

        public void NextWeek1()
        {
            for (int n = 0; n < 7; n++)
            {
                CurrentWeek.days[n].dateTime = CurrentWeek.days[n].dateTime.AddDays(7);
                NextWeek.days[n].dateTime = NextWeek.days[n].dateTime.AddDays(7);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class Week
    {
/*
        public ButtonStatus day0 { get; set; }
        public ButtonStatus day1 { get; set; }
        public ButtonStatus day2 { get; set; }
        public ButtonStatus day3 { get; set; }
        public ButtonStatus day4 { get; set; }
        public ButtonStatus day5 { get; set; }
        public ButtonStatus day6 { get; set; }
        public ButtonStatus day7 { get; set; }*/

        public List<ButtonStatus> days { get; set; }

        public Week()
        {
/*
            this.day0 = new ButtonStatus();
            this.day1 = new ButtonStatus();
            this.day2 = new ButtonStatus();
            this.day3 = new ButtonStatus();
            this.day4 = new ButtonStatus();
            this.day5 = new ButtonStatus();
            this.day6 = new ButtonStatus();
            this.day7 = new ButtonStatus();*/
            this.days = new List<ButtonStatus>(7);
            for (int n = 0; n < 7; n++)
            {
                days.Add(new ButtonStatus());
            }
            //days[0].Content = "AM";

        }

    }
    public class ButtonStatus
    {

        public string Content { get; set; }
        public Brush BgColor { get; set; }
        public DateTime dateTime { get; set; }

        public ButtonStatus(string _content, Color _backColor)
        {
            this.Content = _content;
            this.BgColor = new SolidColorBrush(_backColor);
        }

        public ButtonStatus()
        {
            this.Content = "";
            //this.BgColor = new SolidColorBrush(Colors.Transparent);
            this.BgColor = new SolidColorBrush(Colors.LightGray);
        }


    }


    /*
    public class OrderData : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private string _name;
        private string _mon;
        private string _tue;
        private string _wed;
        private string _thu;
        private string _fri;
        private string _sta;
        private string _sun;

        public OrderData()
        {
            Name = "";

        }

        public string Name { get; set; }

        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { set; get; }
        public string Fri { set; get; }
        public string Sta { set; get; }
        public string Sun { set; get; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }*/
    public sealed class BackgroundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listView =
                ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            int index =
                listView.ItemContainerGenerator.IndexFromContainer(item);


            if (index % 2 == 0)
            {
                return "#FFDBDDEA";
            }
            else
            {
                return "#FFF1F1F1";
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
