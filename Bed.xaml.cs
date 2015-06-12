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
        public List<DateTime> dtlist = new List<DateTime>();

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
            InitWeekWithDate();
        }

        private void LoadTratementConifg()
        {
            try
            {
                TreatmentPanel.Children.Clear();
                using (var methodDao = new TreatMethodDao())
                {
                    //Datalist.Clear();
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

                        StackPanel panel1 = new StackPanel();
                        panel1.Orientation = Orientation.Horizontal;

                        Rectangle rect = new Rectangle();
                        rect.Width = rect.Height = 10;
                        rect.Fill = bgBrush;
                        rect.HorizontalAlignment = HorizontalAlignment.Left;
                        rect.Margin = new Thickness(0, 0, 0, 0);
                        Label label = new Label();
                        label.HorizontalContentAlignment = HorizontalAlignment.Center;
                        label.VerticalContentAlignment = VerticalAlignment.Center;
                        label.Content = treatMethodData.Name;
                        label.FontSize = 10;
                        label.Margin = new Thickness(0, 0, 0, 0);
                        panel1.Children.Add(rect);
                        panel1.Children.Add(label);


                        TreatmentPanel.Children.Add(panel1);

                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        private void InitWeekWithDate()
        {
            dtlist.Clear();
            int weeknow = (int)DateTime.Now.DayOfWeek - 1;
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

        private void AutoClick(object sender, RoutedEventArgs e)
        {
            AutoDistributeBeds();
        }
        private void AutoDistributeBeds()
        {
            //BedPatientList
            //BedInfoList
            string ampme = "";
            foreach (var i in AmPmEGrid.Children)
            {
                if (i is ToggleButton)
                {
                    if (((ToggleButton)i).IsChecked == true)
                    {
                        ampme = (string)((ToggleButton)i).Tag;
                        break;
                    }
                }
            }
            DateTime dt1 = GetDate();

            List<BedPatientData> delPatients = new List<BedPatientData>();
            foreach (var patient in BedPatientList)
            {
                foreach (var bed in BedInfoList)
                {
                    if (bed.TreatMethod == patient.Method)
                    {
                        if (bed.IsAvailable != true && bed.IsOccupy != true)
                        {
                            if (bed.PatientData == null)
                            {
                                //BedPatientList.Remove(patient);
                                delPatients.Add(patient);
                                UpdateBedId(patient.Id, dt1.Date, ampme, bed.Id);
                                bed.PatientName = patient.Name;
                                bed.PatientData = patient;
                            }
                        }

                    }
                }
            }

            foreach (var patient in delPatients)
            {
                BedPatientList.Remove(patient);
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

            RefreshData();
            LoadTratementConifg();

        }

        private void RefreshData()
        {
            string ampme = GetTime();
            DateTime dt = GetDate();

            RefreshPatientList(dt.Date, ampme);
            RefreshBedList();
        }

        private DateTime GetDate()
        {
            ToggleButton toggleButton = new ToggleButton();
            foreach (var i in GridDay.Children)
            {
                if ((i is ToggleButton) && (((ToggleButton)i).IsChecked == true))
                {
                    //ampme = (string)((ToggleButton)i).Tag;
                    toggleButton = (ToggleButton)i;
                    break;
                }
            }
            int n = 0;
            switch (toggleButton.Name)
            {

                case "BtnMon":
                    n = 0;
                    break;
                case "BtnTue":
                    n = 1;
                    break;
                case "BtnWed":
                    n = 2;
                    break;
                case "BtnThe":
                    n = 3;
                    break;
                case "BtnFri":
                    n = 4;
                    break;
                case "BtnSta":
                    n = 5;
                    break;
                case "BtnSun":
                    n = 6;
                    break;
            }
            return dtlist[n];
        }

        private string GetTime()
        {
            string ampme = "";
            foreach (var i in AmPmEGrid.Children)
            {
                if ((i is ToggleButton) && (((ToggleButton)i).IsChecked == true))
                {
                    ampme = (string)((ToggleButton)i).Tag;
                    break;
                }
            }
            return ampme;
        }
        private void RefreshBedList()
        {
            try
            {
                BedInfoList.Clear();
                using (BedDao bedDao = new BedDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = bedDao.SelectBed(condition);
                    foreach (DAOModule.Bed bed in list)
                    {
                        BedInfo bedInfo = new BedInfo();
                        bedInfo.Id = bed.Id;
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

                        using (var bedinfodao = new BedInfoDao())
                        {
                            condition.Clear();
                            condition["Bed.Id"] = bed.Id;
                            var arealist = bedinfodao.SelectPatient(condition);
                            if (arealist.Count == 1)
                            {
                                bedInfo.InfectionType = arealist[0].Type;
                            }
                        }

                        foreach (var bedPatientData in BedPatientList)
                        {
                            if (bedPatientData.BedId == bedInfo.Id)
                            {
                                bedInfo.PatientName = bedPatientData.Name;
                                bedInfo.PatientData = bedPatientData;
                                BedPatientList.Remove(bedPatientData);
                                break;
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
        private void RefreshPatientList(DateTime dateTime, string ampme)
        {
            try
            {
                BedPatientList.Clear();
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    //condition["Date"] = "2015-06-10";//dateTime.ToString("yyyy-MM-dd");
                    condition["Date"] = dateTime.ToString("yyyy-MM-dd");
                    condition["AmPmE"] = ampme;
                    var list = scheduleDao.SelectScheduleTemplate(condition);
                    //ScheduleTemplate表中patientid 对应 Patient表中的ID
                    if (list != null && list.Count != 0)
                    {
                        foreach (var patient in list)
                        {
                            using (var patientDao = new PatientDao())
                            {
                                condition.Clear();
                                condition["ID"] = patient.PatientId;
                                List<Patient> patientlist = patientDao.SelectPatient(condition);
                                if (patientlist.Count == 1)
                                {
                                    BedPatientData patientInfo = new BedPatientData();
                                    patientInfo.Id = patientlist[0].Id;//
                                    patientInfo.Name = patientlist[0].Name;
                                    patientInfo.PatientId = patientlist[0].PatientId;//可能为空
                                    patientInfo.BedId = patient.BedId;
                                    patientInfo.Method = patient.Method;
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

        private System.Windows.DragDropEffects effects;
        private void PreviewDragEnter_Event(object sender, DragEventArgs e)
        {
            targetItemsControl = (ListBoxItem)sender;
            targetItemsControl.IsSelected = true;
            int index = BedListBox.SelectedIndex;
            if (index == -1) return;

            object draggedItem = e.Data.GetData(this.format.Name);
            if (draggedItem != null)
            {

                BedPatientData data = (BedPatientData)draggedItem;
                if(BedInfoList[index].TreatMethod == data.Method)
                //data.Method
                    effects = System.Windows.DragDropEffects.Move;
                else
                {
                    effects = System.Windows.DragDropEffects.None;
                }
            }
            e.Handled = true;
        }
        //SELECT * FROM (Bed INNER JOIN PatientRoom ON Bed.PatientRoomId=PatientRoom.Id) INNER JOIN InfectType ON PatientRoom.InfectTypeId=InfectType.Id
        private void UpdateBedId(long patientID, DateTime dateTime, string ampme , long bedid)
        {
            try
            {
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientId"] = patientID.ToString();
                    condition["Date"] = dateTime.ToString("yyyy-MM-dd");
                    condition["AmPmE"] = ampme;
                    var fileds = new Dictionary<string, object>();
                    fileds["BEDID"] = bedid;
                    scheduleDao.UpdateScheduleTemplate(fileds, condition);
                    
                }

                using (var patientDao = new PatientDao())
                {
                    var fields = new Dictionary<string, object>();
                    fields["BEDID"] = bedid;
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patientID;
                    patientDao.UpdatePatient(fields, condition);

                }

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
            }
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
                //BedPatientData data = (BedPatientData)draggedItem;
                if ((effects == (System.Windows.DragDropEffects)DragDropEffects.Move))
                {
                    if( BedInfoList[index].IsAvailable == true && BedInfoList[index].IsOccupy != true)
                    {
                        string ampme = "";
                        foreach (var i in AmPmEGrid.Children)
                        {
                            if (i is ToggleButton)
                            {
                                if (((ToggleButton)i).IsChecked == true)
                                {
                                    ampme = (string)((ToggleButton)i).Tag;
                                    break;
                                }
                            }
                        }
                        //test
                        if (BedInfoList[index].PatientData != null)
                        {
                            DateTime dt = GetDate();
                            //UpdateBedId(BedInfoList[index].Id, DateTime.Parse("2015-06-10"), ampme, -1);
                            UpdateBedId(BedInfoList[index].Id, dt.Date, ampme, -1);
                            BedPatientList.Add(BedInfoList[index].PatientData);
                        }
                        BedPatientData data = (BedPatientData)draggedItem;
                        BedInfoList[index].PatientName = data.Name;
                        BedInfoList[index].PatientData = data;
                        
                        BedPatientList.Remove((BedPatientData)draggedItem);

                        DateTime dt1 = GetDate();
                        //UpdateBedId(data.Id, DateTime.Parse("2015-06-10"), ampme, BedInfoList[index].Id);
                        UpdateBedId(data.Id, dt1.Date, ampme, BedInfoList[index].Id);
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

            string ampme = "";
            foreach (var i in AmPmEGrid.Children)
            {
                if (i is ToggleButton)
                {
                    if (((ToggleButton)i).IsChecked == true)
                    {
                        ampme = (string)((ToggleButton)i).Tag;
                        break;
                    }
                }
            }
            //test
            DateTime dt = GetDate();
            //UpdateBedId(BedInfoList[index].PatientData.Id, DateTime.Parse("2015-06-10"), ampme, -1);
            UpdateBedId(BedInfoList[index].PatientData.Id,dt.Date, ampme, -1);
            BedInfoList[index].PatientData = null;
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
            RefreshData();
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
            //RefreshPatientList(DateTime.Now.Date, (string)btn.Tag);
            RefreshData();

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
        private string _treatMethod;
        private Brush _itemBgBrush;
        private static Dictionary<string, Color> TreatMethodDictionary = new Dictionary<string, Color>();
        public BedPatientData()
        {
            Name = "";
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

        public Int64 BedId { get; set; }
        public string Name { get; set; }
        public string PatientId { get; set; }

        public string Method
        {
            get { return _treatMethod; }
            set
            {
                _treatMethod = value;
                _itemBgBrush = new SolidColorBrush(StrColorConverter(_treatMethod));
                OnPropertyChanged("Method");
                OnPropertyChanged("ItemBgBrush");
            }
        }

        public Brush ItemBgBrush
        {
            get { return _itemBgBrush; }
            set
            {
                _itemBgBrush = value;
                OnPropertyChanged("ItemBgBrush");
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


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
                        Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        if (bgColor != "" && bgColor != null)
                            treatMethodData.BgColor = bgBrush;
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
            _bedBrush = Brushes.RoyalBlue;
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

        public string InfectionType
        {
            get { return _bedName; }
            set
            {
                if((string)value == "阴性" )
                    _titleBrush = Brushes.GreenYellow;
                else
                {
                    _titleBrush = Brushes.Red;
                }
                OnPropertyChanged("TitleBrush");
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
                        Brush bgBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(bgColor));
                        if (bgColor != "" && bgColor != null)
                            treatMethodData.BgColor = bgBrush;
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
