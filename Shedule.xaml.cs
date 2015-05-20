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

        //public ObservableCollection<ListboxItemStatus> ListboxItemStatusesList = new ObservableCollection<ListboxItemStatus>();
        public List<ListboxItemStatus> ListboxItemStatusesList = new List<ListboxItemStatus>();
        public Shedule(MainWindow mainWindow)
        {
            InitializeComponent();

            //string color = (string)System.Windows.Application.Current.Resources["ysq"];

            InitDay();
            //InitCureTypeDictionary();
            SetBinding();

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

        private void InitCureTypeDictionary()
        {
            CureTypeDictionary.Add("HD", Colors.Red);
            CureTypeDictionary.Add("HP", Colors.BlueViolet);
            CureTypeDictionary.Add("HDF", Colors.Chartreuse);

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

       /* private string GetNextItem(string mark, MouseButton mouseButton)
        {
            ListboxItemStatus listboxItem = ListboxItemStatusesList[index];
            //string name = System.IO.Path.GetFileNameWithoutExtension(curItem);
            //string ret = "";
            string[] str = mark.Split('/');

            if (str.Length != 2)
                return ret;

            string time = str[0];
            string type = str[1];

            if (mouseButton == MouseButton.Left)
            {
                switch (time)
                {
                    case "AM":
                        time = "PM";
                        break;
                    case "PM":
                        time = "E";
                        break;
                    case "E":
                        time = "N";
                        break;
                    case "N":
                        time = "AM";
                        type = "HD";
                        break;
                    default:
                        time = "AM";
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case "HD":
                        type = "HDF";
                        break;
                    case "HDF":
                        type = "HP";
                        break;
                    case "HP":
                        type = "HD";
                        break;
                    case "N":
                        time = "AM";
                        type = "HD";
                        break;
                    default:
                        type = "HD";
                        break;
                }
            }

            if(time == "N")
                ret = "/Resources/N_N.png";
            else
                ret = "/Resources/" + time + "_" + type + ".png";
            return ret;
        }*/

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
                time = listboxItem.CurrentWeek.days[day].Content;
                type = listboxItem.CurrentWeek.days[day].BgColor;
            }
            else
            {
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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
        public Week CurrentWeek { get; set; }
        public Week NextWeek { get; set; }
        
        private long patientID;
        private string patientName;

        public ListboxItemStatus()
        {
            CurrentWeek = new Week();
            NextWeek = new Week();

        }

        /*public ButtonStatus SunNode0 { get; set; }
        public ButtonStatus SunNode1 { get; set; }

        public ButtonStatus MonNode0 { get; set; }
        public ButtonStatus MonNode1 { get; set; }

        public ButtonStatus TueNode0 { get; set; }
        public ButtonStatus TueNode1 { get; set; }

        public ButtonStatus WedNode0 { get; set; }
        public ButtonStatus WedNode1 { get; set; }

        public ButtonStatus ThuNode0 { get; set; }
        public ButtonStatus ThuNode1 { get; set; }

        public ButtonStatus FriNode0 { get; set; }
        public ButtonStatus FriNode1 { get; set; }

        public ButtonStatus SatNode0 { get; set; }
        public ButtonStatus SatNode1 { get; set; }*/


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
