﻿using System;
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

        public Shedule(MainWindow mainWindow)
        {
            InitializeComponent();

            //string color = (string)System.Windows.Application.Current.Resources["ysq"];

            InitDay();
            //InitCureTypeDictionary();
            SetBinding();
            //LoadOrderParaConfig();
            Basewindow = mainWindow;
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
                    BtnTue.IsChecked = true;
                    break;
                case 2:
                    BtnWed.IsChecked = true;
                    break;
                case 3:
                    BtnThe.IsChecked = true;
                    break;
                case 4:
                    BtnFri.IsChecked = true;
                    break;
                case 5:
                    BtnSta.IsChecked = true;
                    break;
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
                        condition["PATIENTID"] = patientID;
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
                                        treat.Type = details[1];
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

            ChangeButtonStauts(index, tag, e.ChangedButton);
            
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
        private void ChangeButtonStauts(int index, string tag, MouseButton mouseButton )
        {
            Point column = GetWeekAndDay(tag);
            int week = (int)column.X;
            int day = (int)column.Y;

            ListboxItemStatus listboxItem = ListboxItemStatusesList[index];
            string time;
            Brush type;

            if (week == 0)
            {
                //MessageBox.Show(listboxItem.CurrentWeek.days[day].dateTime.ToString());
                time = listboxItem.CurrentWeek.days[day].Content;
                type = listboxItem.CurrentWeek.days[day].BgColor;
            }
            else
            {
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
            }
            else
            {
                type = GetNextBrush(type);
                if (time == "")
                    return;
                if (week == 0)
                {
                    listboxItem.CurrentWeek.days[day].BgColor = type;
                }
                else
                {
                    listboxItem.NextWeek.days[day].BgColor = type;
                }
            }
            if (CheckOrders())
                listboxItem.Checks = "正常";
            else
            {
                listboxItem.Checks = "异常";
            }
            
            
            ListboxItemStatusesList[index] = listboxItem;
            ListBox1.Items.Refresh();

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
            LoadTratementConifg();
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
            ListboxItemStatus patient = ListboxItemStatusesList[index];
            if (ListboxItemStatusesList.Count == 0) return false;
            bool ret = true;
            if (TreatOrderList.Count == 0) return false;
            foreach (var treatOrder in TreatOrderList)
            {
                string treat = treatOrder.TreatMethod;
                int times = treatOrder.TreatTimes;
                int count = 0;
                for (int n = 0; n < 7; n++)
                {
                    if (treat == StrColorConverter(patient.CurrentWeek.days[n].BgColor) )
                        count++;
                    if (treat == StrColorConverter(patient.NextWeek.days[n].BgColor))
                        count++;

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
            try
            {
                //PatientList.Clear();
                ListboxItemStatusesList.Clear();
                using (PatientDao patientDao = new PatientDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientDao.SelectPatient(condition);
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
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    foreach (var v in ListboxItemStatusesList)
                    {
                        long patientID = v.PatientID;
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
                                
                                if(list!=null&&list.Count!=0)
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
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdatePatientSchedule();
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
            int weeknow = (int)DateTime.Now.DayOfWeek;
            for (int n = 0; n < 7; n++)
            {
                CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n);
                NextWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n + 7);
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
