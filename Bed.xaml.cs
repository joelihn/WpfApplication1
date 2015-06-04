using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using DragDropListBox;
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;
using System.Windows;
using System.Windows.Controls.Primitives;
using DragDropEffects = System.Windows.Forms.DragDropEffects;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Bed.xaml
    /// </summary>
    public partial class Bed : UserControl
    {
        public MainWindow Basewindow;
        public int selectoperation;
        //public ObservableCollection<BedData> Beddatalist = new ObservableCollection<BedData>();
        private DataFormat format = DataFormats.GetDataFormat("DragDropItemsControl");
        public ObservableCollection<BedPatientData> BedPatientList = new ObservableCollection<BedPatientData>();
        public ObservableCollection<BedInfo> BedInfoList = new ObservableCollection<BedInfo>();
        private ListBoxItem targetItemsControl;
        public Bed(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            this.PatientlistView.ItemsSource = BedPatientList;
            this.BedListBox.ItemsSource = BedInfoList;
            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("所有");
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
            SexComboBox.SelectedIndex = 0;
            InitDay();
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

        private void PatientlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void InquireButton_Click(object sender, RoutedEventArgs e)
        {
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
                condition["TREATSTATUSID"] = 1;
                condition["ISASSIGNED"] = 0;
                BedPatientList.Clear();
                
                List<Patient> list = complexDao.SelectPatient(condition, begin, end);
                foreach (Patient fmriPatient in list)
                {
                    var informatian = new BedPatientData();
                    informatian.Id = fmriPatient.Id;
                    informatian.PatientId = fmriPatient.PatientId;
                    informatian.Name = fmriPatient.Name;
                    BedPatientList.Add(informatian);
                    
                }
            }
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
                TimeRadioButton3.IsChecked = false;
            }
        }

        private void Bed_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
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
                MainWindow.Log.WriteInfoConsole("In Bed.xaml.cs:Bed_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

            BedPatientList.Clear();
            
            /*using (ComplexDao patientDao = new ComplexDao())
            {

                Dictionary<string, object> condition = new Dictionary<string, object>();
                //var list = patientDao.SelectPatient(condition);
                condition["TREATSTATUSID"] = 1;
                condition["ISASSIGNED"] = 0;
                var end = DateTime.Now;
                var begin = end.AddMonths(-1);
                List<Patient> list = patientDao.SelectPatient(condition, begin, end);

                foreach (Patient type in list)
                {
                    BedPatientData patientInfo = new BedPatientData();
                    patientInfo.Id = type.Id;
                    patientInfo.Name = type.Name;
                    patientInfo.PatientId = type.PatientId;


                    BedPatientList.Add(patientInfo);
                    
                }
            }*/
            GetPatientSchedule(DateTime.Now.Date, "AM");
            try
            {
                BedInfoList.Clear();
                using (BedDao bedDao = new BedDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = bedDao.SelectBed(condition);
                    InfectTypeComboBox.Items.Clear();
                    InfectTypeComboBox.Items.Add("所有");
                    foreach (DAOModule.Bed bed in list)
                    {
                        BedInfo bedInfo = new BedInfo();
                        bedInfo.BedName = bed.Name;
                        bedInfo.IsAvailable = bed.IsAvailable;
                        bedInfo.IsOccupy = bed.IsOccupy;
                        using (var treatTypeDao = new TreatMethodDao())
                        {
                            condition.Clear();
                            condition["ID"] = bed.TreatMethodId;
                            var arealist = treatTypeDao.SelectTreatMethod(condition);
                            if (arealist.Count == 1)
                            {
                                bedInfo.TreatMethod = arealist[0].Name;
                            }
                        }
                        BedInfoList.Add(bedInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Shedule.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

        }

        private void PreviewDragEnter_Event(object sender, DragEventArgs e)
        {
            targetItemsControl = (ListBoxItem)sender;
            targetItemsControl.IsSelected = true;
            int index = BedListBox.SelectedIndex;
            if (index == -1) return;

            object draggedItem = e.Data.GetData(this.format.Name);
            if (draggedItem != null)
            {
                e.Effects = System.Windows.DragDropEffects.Move;
            }
            e.Handled = true;
        }
        private void PreviewDrop_Event(object sender, DragEventArgs e)
        {
            targetItemsControl = (ListBoxItem)sender;
            targetItemsControl.IsSelected = true;

            int index = BedListBox.SelectedIndex;
            if (index == -1) return;
            object draggedItem = e.Data.GetData(this.format.Name);
            if (draggedItem != null)
            {
                if ((e.Effects  ==  (System.Windows.DragDropEffects) DragDropEffects.Move))
                {
                    if( BedInfoList[index].IsAvailable != true && BedInfoList[index].IsOccupy != true)
                    {
                        if (BedInfoList[index].PatientData != null)
                        {
                            BedPatientList.Add(BedInfoList[index].PatientData);
                        }
                        BedPatientData data = (BedPatientData)draggedItem;
                        BedInfoList[index].PatientName = data.Name;
                        BedInfoList[index].PatientData = data;
                        BedPatientList.Remove((BedPatientData)draggedItem);
                    }
                }

            }
            e.Handled = true;
        }
        private void PreviewDragOver_Event(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(this.format.Name);

            //DecideDropTarget(e);
            if (draggedItem != null)
            {
                /*ShowDraggedAdorner(e.GetPosition(this.topWindow));
                UpdateInsertionAdornerPosition();*/
            }
            e.Handled = true;
        }
        private void PreviewDragLeave_Event(object sender, DragEventArgs e)
        {
            object draggedItem = e.Data.GetData(this.format.Name);

            if (draggedItem != null)
            {
                //RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        private void MouseDoubleClick_Event(object sender, MouseButtonEventArgs e)
        {
            targetItemsControl = (ListBoxItem)sender;
            targetItemsControl.IsSelected = true;

            int index = BedListBox.SelectedIndex;
            if (index == -1) return;
            BedPatientList.Add(BedInfoList[index].PatientData);
            BedInfoList[index].PatientName = "";
            e.Handled = true;
        }

        private void BtnDay_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
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

        private void BtnAmPmE_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            if (btn.IsChecked == true)
            {
                UncheckOtherToggleButton1(btn);
            }
            else
            {
                btn.IsChecked = true;
                UncheckOtherToggleButton1(btn);
            }
            GetPatientSchedule(DateTime.Now.Date, (string)btn.Tag);


        }
        private void UncheckOtherToggleButton(ToggleButton btn)
        {
            foreach (var i in GridDay.Children)
            {
                if ((i is ToggleButton) && i != btn)
                {
                    ((ToggleButton)i).IsChecked = false;
                }
            }
        }
        private void UncheckOtherToggleButton1(ToggleButton btn)
        {
            foreach (var i in AmPmEGrid.Children)
            {
                if ((i is ToggleButton) && i != btn)
                {
                    ((ToggleButton)i).IsChecked = false;
                }
            }
        }

        private void GetPatientSchedule( DateTime dateTime , string ampme)
        {
            try
            {
                BedPatientList.Clear();
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["Date"] = "2015-06-10";//dateTime.ToString("yyyy-MM-dd");
                    condition["AmPmE"] = ampme;
                    var list = scheduleDao.SelectScheduleTemplate(condition);

                    if (list != null && list.Count != 0)
                    {
                        foreach (var patient in list)
                        {
                            using (var patientDao = new PatientDao())
                            {
                                condition.Clear();
                                condition["ID"] = patient.PatientId;
                                List<Patient> patientlist = patientDao.SelectPatient(condition);
                                if (patientlist.Count > 0)
                                {
                                    BedPatientData patientInfo = new BedPatientData();
                                    patientInfo.Id = patientlist[0].Id;
                                    patientInfo.Name = patientlist[0].Name;
                                    patientInfo.PatientId = patientlist[0].PatientId;
                                    BedPatientList.Add(patientInfo);
                                }
                            }
                        }
                        
                        /*var fileds = new Dictionary<string, object>();
                        condition["Date"] = day.dateTime.ToString("yyyy-MM-dd");
                        fileds["AMPME"] = day.Content;
                        fileds["METHOD"] = StrColorConverter(day.BgColor);
                        scheduleDao.UpdateScheduleTemplate(fileds, condition);*/
                    }
                           
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
            }
        }

    }

    public class Contact
    {
        public Contact(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public override string ToString() 
        { 
        return string.Format("Firstname: {0}, Lastname: {1}", Firstname, Lastname); 
        }
    } 

    public class BedPatientData : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private Int64 _id;

        public BedPatientData()
        {
            Name = "";

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

        public string Name { get; set; }
        public string PatientId { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    public class BedInfo : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private Int64 _id;
        private int _roomID;
        private string _bedName;
        private string _patientName;
        private int _infcetionType;
        private string _treatMethod;

        private Brush _titleBrush;
        private Brush _bedBrush;
        private static Dictionary<string, Color> TreatMethodDictionary = new Dictionary<string, Color>();

        private bool _isAvliable;
        private bool _isOccupy;

        public BedPatientData PatientData { get; set; }

        public BedInfo()
        {
            _bedName = "";
            _patientName = "";
            _treatMethod = "";
            _titleBrush = Brushes.GreenYellow;
            _bedBrush = Brushes.DimGray;
            _isAvliable = false;
            _isOccupy = true;
            PatientData = null;
            LoadTreatMethod();

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

        public bool IsOccupy
        {
            get { return _isOccupy; }
            set
            {
                _isOccupy = value;
                OnPropertyChanged("IsOccupy");
            }
        }

        public bool IsAvailable
        {
            get { return _isAvliable; }
            set
            {
                _isAvliable = value;
                if (_isAvliable == false)
                    _bedBrush = Brushes.DimGray;
                OnPropertyChanged("IsAvailable");
                OnPropertyChanged("BedBrush");
            }
        }

        public string BedName
        {
            get { return _bedName; }
            set
            {
                _bedName = value;
                OnPropertyChanged("BedName");
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

        public string TreatMethod
        {
            get { return _treatMethod; }
            set
            {
                _treatMethod = value;
                _bedBrush = new SolidColorBrush(StrColorConverter(_treatMethod));
                OnPropertyChanged("TreatMethod");
                OnPropertyChanged("BedBrush");
            }
        }

        public Brush BedBrush
        {
            get { return _bedBrush; }
            set
            {
                _bedBrush = value;
                OnPropertyChanged("TitleBrush");
            }
        }
        public Brush TitleBrush
        {
            get { return _titleBrush; }
            set
            {
                _titleBrush = value;
                OnPropertyChanged("TitleBrush");
            }
        }

        private static void LoadTreatMethod()
        {
            try
            {
                using (var methodDao = new TreatMethodDao())
                {
                    TreatMethodDictionary.Clear();
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

                        TreatMethodDictionary.Add(pa.Name, ((SolidColorBrush)treatMethodData.BgColor).Color);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        public static string StrColorConverter(Brush brush)
        {
            Color color = ((SolidColorBrush)brush).Color;
            foreach (var v in TreatMethodDictionary)
            {
                if (v.Value == color)
                    return v.Key;
            }
            return "";
        }
        public static Color StrColorConverter(string str)
        {
            if (str == "")
                return Colors.Transparent;
            return TreatMethodDictionary[str];
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

    public sealed class BackgroundConverter1 : IValueConverter
    {
        #region IValueConverter Members
        private static Dictionary<string, Color> TreatMethodDictionary = new Dictionary<string, Color>();
       
        public BackgroundConverter1()
        {
            LoadTreatMethod();
        }
        private static void LoadTreatMethod()
        {
            try
            {
                using (var methodDao = new TreatMethodDao())
                {
                    TreatMethodDictionary.Clear();
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

                        TreatMethodDictionary.Add(pa.Name, ((SolidColorBrush)treatMethodData.BgColor).Color);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        public static string StrColorConverter(Brush brush)
        {
            Color color = ((SolidColorBrush)brush).Color;
            foreach (var v in TreatMethodDictionary)
            {
                if (v.Value == color)
                    return v.Key;
            }
            return "";
        }
        public static Color StrColorConverter(string str)
        {
            if (str == "")
                return Colors.Transparent;
            return TreatMethodDictionary[str];
        }

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            string method = (string)value;
            return StrColorConverter(method);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            Brush method = (Brush)value;
            return StrColorConverter(method);
        }

        #endregion
    }

}
