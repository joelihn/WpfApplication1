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
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using WpfApplication1.CustomUI;
using WpfApplication1.DataStructures;
using WpfApplication1.DAOModule;

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
        
        public List<ListboxItemStatus> ListboxItemStatusesList = new List<ListboxItemStatus>();
        //public ObservableCollection<MedicalOrderParaData> OrderParaList = new ObservableCollection<MedicalOrderParaData>();
        public ObservableCollection<TreatOrder> TreatOrderList = new ObservableCollection<TreatOrder>();
        public List<DateTime> dtlist = new List<DateTime>();

        public int selectoperation;
        public Shedule(MainWindow mainWindow)
        {
            InitializeComponent();
            
            //string color = (string)System.Windows.Application.Current.Resources["ysq"];

            InitDay();
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

                if (!InfectTypeComboBox.Text.Equals("所有"))
                {

                    var condition2 = new Dictionary<string, object>();
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        condition2.Clear();
                        condition2["Name"] = InfectTypeComboBox.Text;
                        var arealist = infectTypeDao.SelectInfectType(condition2);
                        if (arealist.Count == 1)
                        {
                            condition["INFECTTYPEID"] = arealist[0].Id;
                        }
                    }
                    
                }
                

                if (!SexComboBox.Text.Equals("所有"))
                    condition["GENDER"] = SexComboBox.Text;

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
                    status.PatientID = (fmriPatient.Id);
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

                    if (CheckOrders(fmriPatient.Id))
                        status.Checks = "正常";
                    else
                    {
                        status.Checks = "异常";
                    }
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
                    break;
                case 1:
                    BtnMon.IsChecked = true;
                    break;
                case 2:
                    BtnTue.IsChecked = true;
                    break;
                case 3:
                    BtnWed.IsChecked = true;
                    break;
                case 4:
                    BtnThe.IsChecked = true;
                    break;
                case 5:
                    BtnFri.IsChecked = true;
                    break;
                case 6:
                    BtnSta.IsChecked = true;
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

                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = _patientID;
                    List<Patient> list = patientDao.SelectPatient(condition);
                    if (list.Count > 0)
                    {
                        string orders = list[0].Orders;
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
            return CureTypeDictionary[str];
        }
        
        private void ButtonBase_OnClick(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button) sender;
            string tag = (string) btn.Tag;
            int index = ListBox1.SelectedIndex;
            if ( index == -1 ) 
                return;

            bool ret = ChangeButtonStauts(index, tag, e.ChangedButton);
            if (ret)
            {
                UpdatePatientSchedule();
                if (CheckOrders())
                    ListboxItemStatusesList[index].Checks = "正常";
                else
                {
                    ListboxItemStatusesList[index].Checks = "异常";
                }

                ListBox1.Items.Refresh();
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
            Color color = ((SolidColorBrush)brush).Color;
            int n = 0;
            foreach (var v in CureTypeDictionary)
            {
                if (v.Value == color)
                {
                    break;
                }
                n++;
            }

            int count = n+1;
            if (count >= CureTypeDictionary.Count )
            {
                count = 0;
            }

            n = 0;
            foreach (var v in CureTypeDictionary)
            {
                if (n == count)
                {
                    return new SolidColorBrush(v.Value);
                }
                n++;
            }
            return null;

        }

        private string GetNextTime(string time)
        {
            string ret = "";
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

            return ret;
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
                        listboxItem.CurrentWeek.days[day].BgColor = new SolidColorBrush(CureTypeDictionary.Values.First());
                    }
                    else
                    {
                        listboxItem.NextWeek.days[day].BgColor = new SolidColorBrush(CureTypeDictionary.Values.First());
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
                using (var methodDao = new TreatMethodDao())
                {
                    //Datalist.Clear();
                    CureTypeDictionary.Clear();
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
                        if (bgColor != "" && bgColor != null)
                            treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        else
                            treatMethodData.BgColor = Brushes.LightGray;


                        treatMethodData.Description = pa.Description;

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

        private bool CheckOrders()
        {
            int index = ListBox1.SelectedIndex;
            if (index == -1) return false;
            
            if (ListboxItemStatusesList.Count == 0) return false;
            ListboxItemStatus patient = ListboxItemStatusesList[index];
            PatientSchedule schedule = GetPatientSchedule(patient.PatientID);
            bool ret = true;
            if (TreatOrderList.Count == 0) return false;
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
                int times = treatOrder.TreatTimes;
                int count = 0;
                if( type == "周" )
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
                    int dayofweek = (int) DateTime.Now.DayOfWeek - 1;
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 14);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }
                }
                else if (type == "月")
                {
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int dure = DateTime.DaysInMonth(year, month);
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-day+1);
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
                }
                if (count != times)
                {
                    ret = false;
                }
            }
            return ret;

        }

        private bool CheckOrders(long _PatientID )
        {
            PatientSchedule schedule = GetPatientSchedule(_PatientID);
            bool ret = true;
            InitTreatOrderList(_PatientID);
            if (TreatOrderList.Count == 0) return false;
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
                int times = treatOrder.TreatTimes;
                int count = 0;
                if (type == "周")
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
                    DateTime dtFrom = DateTime.Now.Date.AddDays(-dayofweek);
                    DateTime dtTo = DateTime.Now.Date.AddDays(-dayofweek + 14);
                    foreach (var v in schedule.Hemodialysis)
                    {
                        if (DateTime.Compare(v.dialysisTime.dateTime, dtFrom.Date) >= 0 &&
                            DateTime.Compare(v.dialysisTime.dateTime, dtTo.Date) <= 0)
                        {
                            if (treat == v.hemodialysisItem)
                                count++;
                        }
                    }
                }
                else if (type == "月")
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
                }
                if (count != times)
                {
                    ret = false;
                }
            }
            return ret;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //GetPatientSchedule(0);
            LoadTratementConifg();

            ListBox1.SelectedIndex = -1;
            //CopySchedule();
            //return;
            try
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
                        ListboxItemStatusesList.Add(status);
                        //PatientList.Add(patientInfo);
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
                    InfectTypeComboBox.Items.Add("所有");
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

        private void UpdatePatientSchedule()
        {
            try
            {
                 long selectPatientID = ListboxItemStatusesList[ListBox1.SelectedIndex].PatientID;
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    foreach (var v in ListboxItemStatusesList)
                    {
                        long patientID = v.PatientID;
                        if (selectPatientID == patientID)
                        {
                            var condition = new Dictionary<string, object>();
                            condition["PatientID"] = patientID;

                            foreach (var day in v.CurrentWeek.days)
                            {
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
                                        scheduleDao.UpdateScheduleTemplate(fileds, condition);
                                    }
                                    else
                                    {
                                        ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                        scheduleTemplate.PatientId = patientID;
                                        scheduleTemplate.Date = day.dateTime.ToString("yyyy-MM-dd");
                                        scheduleTemplate.AmPmE = day.Content;
                                        scheduleTemplate.Method = StrColorConverter(day.BgColor);
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
                            {
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
                                        scheduleDao.UpdateScheduleTemplate(fileds, condition);
                                    }
                                    else
                                    {
                                        ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                        scheduleTemplate.PatientId = patientID;
                                        scheduleTemplate.Date = day.dateTime.ToString("yyyy-MM-dd");
                                        scheduleTemplate.AmPmE = day.Content;
                                        scheduleTemplate.Method = StrColorConverter(day.BgColor);
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
            PreWeek();
        }


        private void Button_Click2(object sender, RoutedEventArgs e)
        {
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
        }

        private void NextWeek()
        {
            //int index = ListBox1.SelectedIndex;
            //if (index == -1) return;
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
            
        }
        private void InitWeekWithDate()
        {
            dtlist.Clear();
            int weeknow = (int)DateTime.Now.DayOfWeek-1;
            for (int n = 0; n < 7; n++)
            {
                dtlist.Add(DateTime.Now.AddDays(-weeknow + n));
            }

            lable0.Content = dtlist[0].ToString("MM-dd");
            lable1.Content = dtlist[1].ToString("MM-dd");
            lable2.Content = dtlist[2].ToString("MM-dd");
            lable3.Content = dtlist[3].ToString("MM-dd");
            lable4.Content = dtlist[4].ToString("MM-dd");
            lable5.Content = dtlist[5].ToString("MM-dd");
            lable6.Content = dtlist[6].ToString("MM-dd");
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
            int dayOfWeek = (int)DateTime.Now.DayOfWeek;

            //return;
            if (dayOfWeek != 1) return;

            DateTime dtFrom = DateTime.Now.AddDays(-7);
            DateTime dtTo = DateTime.Now.AddDays(-1);
            using (var patientDao = new PatientDao())
            {
                var condition = new Dictionary<string, object>();
                condition["TREATSTATUSID"] = 1;
                List<Patient> patientslist = patientDao.SelectPatient(condition);
                if (patientslist.Count == 0) return;
                ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao();
                foreach (var patient in patientslist)
                {
                    //if (patient.Id != 17) continue;
                    PatientSchedule schedule = GetPatientSchedule(patient.Id);
                    List<string> uncopyed =  GetUnCopyOrder(patient.Id);
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

                            Dictionary<string, object> condition1 = new Dictionary<string, object>();
                            condition1["PatientId"] = patient.Id;
                            condition1["Date"] = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                            var list = scheduleDao.SelectScheduleTemplate(condition1);
                            if( list == null || list.Count == 0)
                            {
                                ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                                scheduleTemplate.PatientId = patient.Id;
                                scheduleTemplate.Date = v.dialysisTime.dateTime.AddDays(14).ToString("yyyy-MM-dd");
                                scheduleTemplate.AmPmE = v.dialysisTime.AmPmE;
                                scheduleTemplate.Method = v.hemodialysisItem;
                                int ret = -1;
                                if (!uncopyed.Contains(scheduleTemplate.Method))
                                    scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);
                            }

                        }
                    }
                }
            }

            



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
                    for (int n = 0; n < 7; n++)
                    {
                        Dictionary<string, int> dictionary = Statistics(ampme, n, m);
                        StackPanel panel = new StackPanel();
                        foreach (var v in dictionary)
                        {
                            StackPanel panel1 = new StackPanel();
                            panel1.Orientation = Orientation.Horizontal;
                            
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
                            Label label = new Label();
                            label.HorizontalContentAlignment = HorizontalAlignment.Center;
                            label.VerticalContentAlignment = VerticalAlignment.Center;
                            label.Content = mathod + "/" + v.Value;

                            //panel1.Children.Add(rect);
                            panel1.Children.Add(label);

                            panel.Children.Add(panel1);

                        }

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
    }



    public class ListboxItemStatus:INotifyPropertyChanged
    {
        public long PatientID
        {
            get { return patientID; }
            set { patientID = value; }
        }
        public string PatientName
        {
            get { return patientName; }
            set { patientName = value; }
        }

        public string Checks
        {
            get { return checks; }
            set { checks = value; }
        }

        public Week CurrentWeek { get; set; }
        public Week NextWeek { get; set; }
        
        private long patientID;
        private string patientName;
        private string checks;

        public ListboxItemStatus()
        {
            CurrentWeek = new Week();
            NextWeek = new Week();
            InitWeekWithDate();
        }

        private void InitWeekWithDate()
        {
            int weeknow = (int)DateTime.Now.DayOfWeek-1;
            for (int n = 0; n < 7; n++)
            {
                CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n);
                NextWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n + 7);
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
