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
using WpfApplication1.Utils;
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
        public ObservableCollection<BedPatientData> UnPatientList = new ObservableCollection<BedPatientData>();
        public ObservableCollection<BedPatientData> FixBedPatientList = new ObservableCollection<BedPatientData>();
        
        
        public ObservableCollection<BedInfo> BedInfoList = new ObservableCollection<BedInfo>();
        private ListBoxItem targetItemsControl;
        public List<DateTime> dtlist = new List<DateTime>();

        public Bed(MainWindow window)
        {
            InitializeComponent();

            Basewindow = window;
            this.PatientlistView.ItemsSource = BedPatientList;
            this.PatientListBox1.ItemsSource = BedPatientList;

            this.BedListBox.ItemsSource = BedInfoList;
            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();

            

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("所有");
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
            SexComboBox.SelectedIndex = 0;
            //InitDay();
            //InitWeekWithDate();
            //LoadPatientAreas();
        }

        private void InitPump()
        {
            PumpListPanel.Children.Clear();
            try
            {
                using (var machineTypeDao = new MachineTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = machineTypeDao.SelectMachineType(condition);
                    foreach (var pa in list)
                    {
                        var machineTypeData = new MachineTypeData();
                        machineTypeData.Id = pa.Id;
                        machineTypeData.Name = pa.Name;
                        machineTypeData.Description = pa.Description;

                        string bgColor = pa.BgColor;

                        if (bgColor != "" && bgColor != null)
                        {
                            Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                            machineTypeData.BgColor = bgBrush;
                        }

                        else
                            machineTypeData.BgColor = Brushes.Gray;

                        
                        Button btn = new Button();
                        btn.Style = this.FindResource("LabelStyleWithColor") as Style;
                        btn.Content = machineTypeData.Name;
                        btn.Background = machineTypeData.BgColor;
                        btn.Width = 50;
                        btn.Height = 20;
                        btn.Margin = new Thickness(2, 2, 2, 2);
                        btn.HorizontalAlignment = HorizontalAlignment.Right;


                        PumpListPanel.Children.Add(btn);
                        //Datalist.Add(machineTypeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CMachineType.xaml.cs:RefreshData exception messsage: " + ex.Message);
            }
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
                        treatMethodData.IsAvailable = pa.IsAvailable;
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
                        rect.Margin = new Thickness(0, 0, 0, 0);
                        Label label = new Label();
                        label.HorizontalContentAlignment = HorizontalAlignment.Center;
                        label.VerticalContentAlignment = VerticalAlignment.Center;
                        label.Content = treatMethodData.Name;
                        label.FontSize = 10;
                        label.Margin = new Thickness(0, 0, 0, 0);
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
                        
                        if( pa.IsAvailable == true )
                            TreatmentPanel.Children.Add(btn);

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
            if (weeknow < 0)
            {
                for (int n = 6; n >=0; n--)
                {
                    dtlist.Add(DateTime.Now.AddDays(-n));
                }
            }
            else
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

            //for (int n = 0; n < 7; n++)
            //{
            //    CurrentWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n);
            //    NextWeek.days[n].dateTime = DateTime.Now.AddDays(-weeknow + n + 7);
            //}
        }
        private void InitDay()
        {
            UncheckOtherToggleButton();
            int dayofweek = (int)DateTime.Now.DayOfWeek;
            switch (dayofweek)
            {
                case 0:
                    btn6.IsChecked = true;
                    BtnSun.IsChecked = true;
                    break;
                case 1:
                    btn0.IsChecked = true;
                    BtnMon.IsChecked = true;
                    break;
                case 2:
                    btn1.IsChecked = true;
                    BtnTue.IsChecked = true;
                    break;
                case 3:
                    btn2.IsChecked = true;
                    BtnWed.IsChecked = true;
                    break;
                case 4:
                    btn3.IsChecked = true;
                    BtnThe.IsChecked = true;
                    break;
                case 5:
                    btn4.IsChecked = true;
                    BtnFri.IsChecked = true;
                    break;
                case 6:
                    btn5.IsChecked = true;
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
                BedPatientList.Clear();
                
                List<Patient> list = complexDao.SelectPatient(condition, begin, end);
                foreach (Patient fmriPatient in list)
                {
                    var informatian = new BedPatientData();
                    informatian.Id = fmriPatient.Id;
                    informatian.PatientId = fmriPatient.PatientId;
                    informatian.Name = fmriPatient.Name;
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        condition.Clear();
                        condition["ID"] = informatian.InfectionType;
                        var arealist = infectTypeDao.SelectInfectType(condition);
                        if (arealist.Count == 1)
                        {
                            informatian.InfectionType = arealist[0].Name;
                        }
                    }
                    //informatian.InfectionType = fmriPatient.InfectTypeId;

                    string treatOrders = "";
                    string orders = fmriPatient.Orders;
                    if (!string.IsNullOrEmpty(orders))
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
                        informatian.ToolTips = treatOrders;
                    }
                    else
                    {
                        informatian.ToolTips = "";
                    }


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

        private void AutoDistributeFixedBed()
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
            DateTime dt1 = GetDate();

            List<BedPatientData> delPatients = new List<BedPatientData>();
            foreach (var patient in BedPatientList)
            //foreach (var patient in FixBedPatientList)
            {
                bool isFixedBed = false;
                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patient.Id;
                    var list = patientDao.SelectPatient(condition);
                    isFixedBed = list[0].IsFixedBed;
                }
                if (isFixedBed == false)
                    continue;

                long fixbedid = -1;
                
                /*using (var bedDao = new BedDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = fixBedId;
                    var list = bedDao.SelectBed(condition);
                }*/
                bool isAuto = true;
                bool isTemp = false;
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientId"] = patient.Id.ToString();
                    //condition["IsTemp"] = false;
                    condition["Date"] = dt1.Date.ToString("yyyy-MM-dd");
                    var list = scheduleDao.SelectScheduleTemplate(condition);
                    if (list.Count == 1)
                    {
                        isAuto = list[0].IsAuto;
                        fixbedid = list[0].BedId;
                        isTemp = list[0].IsTemp;
                    }

                }
                if (isAuto)
                {
                    using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                    {
                        Dictionary<string, object> condition = new Dictionary<string, object>();
                        condition["PatientId"] = patient.Id.ToString();
                        //condition["IsTemp"] = false;
                        condition["AmPmE"] = ampme;
                        var list = scheduleDao.SelectScheduleTemplate(condition);

                        foreach (var scheduleTemplate in list)
                        {
                            DateTime now = DateTime.Parse(scheduleTemplate.Date);
                            if (DateTime.Compare(now.Date, dt1.Date) < 0)
                            {
                                if (scheduleTemplate.BedId != -1)
                                {
                                    fixbedid = scheduleTemplate.BedId;
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var bed in BedInfoList)
                {
                    if (fixbedid != bed.Id)
                    {
                        continue;
                    }

                    if (bed.InfectionType == patient.InfectionType)
                    {
                        using (var methodDao = new TreatMethodDao())
                        {
                            int DoublePump = -1;
                            int SinglePump = -1;
                            var condition = new Dictionary<string, object>();
                            condition["NAME"] = patient.TreatMethod;
                            var list = methodDao.SelectTreatMethod(condition);
                            if (list.Count == 1)
                            {
                                if (list[0].DoublePump == true)
                                {
                                    DoublePump = 1;
                                }
                                if (list[0].SinglePump == true)
                                {
                                    SinglePump = 0;
                                }

                            }

                            if (bed.MachineTypeID != DoublePump && bed.MachineTypeID != SinglePump)
                            {
                                break;
                            }
                        }


                        if (bed.IsAvailable == true && bed.IsOccupy != true)
                        {
                            if (bed.PatientData == null)
                            {
                                delPatients.Add(patient);
                                UpdateBedId(patient.Id, dt1.Date, ampme, bed.Id, isAuto);
                                bed.PatientName = patient.Name + "\n" + patient.TreatMethod;
                                bed.PatientData = patient;
                                bed.IsTemp = isTemp;
                                break;
                            }
                        }
                        

                    }
                    
                }
            }

            foreach (var patient in delPatients)
            {
                BedPatientList.Remove(patient);
            }



            ///////////////////////////////////////
            /// 上边先给没有分过床的固定床位患者分床
            /// 下面给分过床的固定床位患者分床
            /// ///////////////////////////////////
            foreach (var patient in FixBedPatientList)
            {
                long fixbedid = -1;
                bool isAuto = true;
                bool isTemp = false;
                using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientId"] = patient.Id.ToString();
                    //condition["IsTemp"] = false;
                    condition["Date"] = dt1.Date.ToString("yyyy-MM-dd");
                    var list = scheduleDao.SelectScheduleTemplate(condition);
                    if (list.Count == 1)
                    {
                        isAuto = list[0].IsAuto;
                        fixbedid = list[0].BedId;
                        isTemp = list[0].IsTemp;
                    }

                }
                if (isAuto)
                {
                    using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                    {
                        Dictionary<string, object> condition = new Dictionary<string, object>();
                        condition["PatientId"] = patient.Id.ToString();
                        //condition["IsTemp"] = false;
                        condition["AmPmE"] = ampme;
                        var list = scheduleDao.SelectScheduleTemplate(condition);

                        foreach (var scheduleTemplate in list)
                        {
                            DateTime now = DateTime.Parse(scheduleTemplate.Date);
                            if (DateTime.Compare(now.Date, dt1.Date) < 0)
                            {
                                if (scheduleTemplate.BedId != -1)
                                {
                                    fixbedid = scheduleTemplate.BedId;
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (var bed in BedInfoList)
                {
                    if (fixbedid != bed.Id)
                    {
                        continue;
                    }

                    if (bed.InfectionType == patient.InfectionType)
                    {
                        using (var methodDao = new TreatMethodDao())
                        {
                            int DoublePump = -1;
                            int SinglePump = -1;
                            var condition = new Dictionary<string, object>();
                            condition["NAME"] = patient.TreatMethod;
                            var list = methodDao.SelectTreatMethod(condition);
                            if (list.Count == 1)
                            {
                                if (list[0].DoublePump == true)
                                {
                                    DoublePump = 1;
                                }
                                if (list[0].SinglePump == true)
                                {
                                    SinglePump = 0;
                                }

                            }

                            if (bed.MachineTypeID != DoublePump && bed.MachineTypeID != SinglePump)
                            {
                                break;
                            }
                        }


                        if (bed.IsAvailable == true && bed.IsOccupy != true)
                        {
                            if (bed.PatientData == null)
                            {
                                delPatients.Add(patient);
                                UpdateBedId(patient.Id, dt1.Date, ampme, bed.Id, isAuto);
                                bed.PatientName = patient.Name + "\n" + patient.TreatMethod;
                                bed.PatientData = patient;
                                bed.IsTemp = isTemp;
                                break;
                            }
                        }


                    }

                }
            }

            PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
            
            
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
                    if (bed.InfectionType == patient.InfectionType)
                    {
                        ////new added////
                        /*long patientInfectTypeId = -1;
                        using (PatientDao patientDao = new PatientDao())
                        {
                            var condition = new Dictionary<string, object>();
                            condition["ID"] = patient.Id;
                            var list = patientDao.SelectPatient(condition);
                            patientInfectTypeId = list[0].InfectTypeId;
                        }
                        long patientAreaInfectTypeId = -1;

                        using (BedDao bedDao = new BedDao())
                        {
                            var condition = new Dictionary<string, object>();
                            condition["ID"] = bed.Id;
                            var list = bedDao.SelectBed(condition);
                            long patientAreaId = list[0].PatientAreaId;

                            using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                            {
                                var condition2 = new Dictionary<string, object>();
                                condition2["ID"] = patientAreaId;
                                var list2 = patientAreaDao.SelectPatientArea(condition2);
                                patientAreaInfectTypeId = list2[0].InfectTypeId;

                            }
                        }

                        if (patientInfectTypeId != patientAreaInfectTypeId)
                        {
                            break;
                        }*/

                        /////////////////
                        using (var methodDao = new TreatMethodDao())
                        {
                            int DoublePump = -1;
                            int SinglePump = -1;
                            var condition = new Dictionary<string, object>();
                            condition["NAME"] = patient.TreatMethod;
                            var list = methodDao.SelectTreatMethod(condition);
                            if (list.Count == 1)
                            {
                                if (list[0].DoublePump == true)
                                {
                                    DoublePump = 1;
                                }
                                if (list[0].SinglePump == true)
                                {
                                    SinglePump = 0;
                                }

                            }

                            if (bed.MachineTypeID != DoublePump && bed.MachineTypeID != SinglePump)
                            {
                                continue;
                            }
                        }

                        if (bed.IsAvailable == true && bed.IsOccupy != true)
                        {
                            if (bed.PatientData == null)
                            {
                                delPatients.Add(patient);
                                UpdateBedId(patient.Id, dt1.Date, ampme, bed.Id);
                                bed.PatientName = patient.Name + "\n" + patient.TreatMethod;
                                bed.PatientData = patient;
                                break;
                            }
                        }

                    }
                }
            }

            foreach (var patient in delPatients)
            {
                BedPatientList.Remove(patient);
            }

            PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
            
        }
        private void BeginDatePicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void EndatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
        }
        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ListSortDirection sortDirection = ListSortDirection.Ascending;


            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column; //得到单击的列
                int columnflag = 0;
                columnflag = ((GridView)PatientlistView.View).Columns.IndexOf(clickedColumn);


                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    //string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path; //得到单击列所绑定的属性
                    string bindingProperty = "";
                    if (clickedColumn.Header is Grid & columnflag == 0)
                    {
                        if (Paixiflag[0] == 0)
                        {
                            Paixiflag[0] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[0] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientId";
                        
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 1)
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
                        bindingProperty = "Name";
                        
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 2)
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

                    SortDescriptionCollection sdc = PatientlistView.Items.SortDescriptions;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                        sdc.Clear();
                    }

                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                    var temp = new ObservableCollection<BedPatientData>();
                    for (int i = 0; i < PatientlistView.Items.Count; i++)
                    {
                        temp.Add((BedPatientData)PatientlistView.Items[i]);
                    }
                    BedPatientList.Clear();
                    BedPatientList = temp;
                    PatientlistView.ItemsSource = BedPatientList;
                    sdc.Clear();
                }
            }
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
            InitPump();
            InitDay();
            InitWeekWithDate();
            //throw new NotImplementedException();
            try
            {
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    InfectTypeComboBox.Items.Clear();
                    //InfectTypeComboBox.Items.Add("所有");
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
            
            
            LoadTratementConifg();
            LoadPatientAreas();
            RefreshData();
        }

        private void RefreshData()
        {
            string ampme = GetTime();
            DateTime dt = GetDate();
            long infecttype = GetInfectType();
            RefreshPatientList(dt.Date, ampme);
            RefreshBedList(infecttype);
            AutoDistributeFixedBed();
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

                case "btn0":
                    n = 0;
                    break;
                case "btn1":
                    n = 1;
                    break;
                case "btn2":
                    n = 2;
                    break;
                case "btn3":
                    n = 3;
                    break;
                case "btn4":
                    n = 4;
                    break;
                case "btn5":
                    n = 5;
                    break;
                case "btn6":
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

        private long GetInfectType()
        {
            string area = "";
            foreach (var i in InfectGrid.Children)
            {
                if ((i is ToggleButton) && (((ToggleButton)i).IsChecked == true))
                {
                    area = (string)((ToggleButton)i).Content;
                    break;
                }
            }

            try
            {
                /*using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["Name"] = type;
                    var arealist = infectTypeDao.SelectInfectType(condition);
                    if (arealist.Count == 1)
                    {
                        return arealist[0].Id;
                    }
                }
                */

                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["Name"] = area;
                    var arealist = patientAreaDao.SelectPatientArea(condition);
                    if (arealist.Count == 1)
                    {
                        return arealist[0].Id;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }


            return -1;
        }

        private void RefreshBedList( long areaId)
        {
            try
            {
                BedInfoList.Clear();
                using (BedDao bedDao = new BedDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PatientAreaId"] = areaId;
                    var list = bedDao.SelectBed(condition);
                    foreach (DAOModule.Bed bed in list)
                    {
                        BedInfo bedInfo = new BedInfo();
                        bedInfo.Id = bed.Id;
                        bedInfo.BedName = bed.Name;
                        bedInfo.IsAvailable = bed.IsAvailable;
                        bedInfo.IsOccupy = bed.IsOccupy;
                        bedInfo.PatientAreaID = bed.PatientAreaId;
                        bedInfo.MachineTypeID = bed.MachineTypeId;

                        //默认床不是零时床
                        bedInfo.IsTemp = false;
                        //bedInfo.IsTemp = bed.IsTemp;
                        DateTime then = GetDate();
                        if( then.Date >= DateTime.Now.Date )
                        if (bedInfo.IsAvailable == false )
                            continue;
                        /*using (var treatTypeDao = new TreatTypeDao())
                        {
                            condition.Clear();
                            condition["ID"] = bed.TreatTypeId;
                            var arealist = treatTypeDao.SelectTreatType(condition);
                            if (arealist.Count == 1)
                            {
                                bedInfo.TreatType = arealist[0].Name;
                            }
                        }*/
                        
                        /*
                        using (var bedinfodao = new BedInfoDao())
                        {
                            condition.Clear();
                            condition["Bed.Id"] = bed.Id;
                            var arealist = bedinfodao.SelectPatient(condition);
                            if (arealist.Count == 1)
                            {
                                bedInfo.InfectionType = arealist[0].Type;
                            }
                        }*/

                        using (var patientAreaDao = new PatientAreaDao())
                        {
                            condition.Clear();
                            condition["Id"] = areaId;
                            var arealist = patientAreaDao.SelectPatientArea(condition);
                            if (arealist.Count == 1)
                            {
                                if (arealist[0].Type == "0")
                                {
                                    bedInfo.InfectionType = "阴性";
                                }
                                else
                                {
                                    using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                                    {
                                        condition.Clear();
                                        condition["ID"] = arealist[0].InfectTypeId;
                                        var list2 = infectTypeDao.SelectInfectType(condition);
                                        if (list2.Count == 1)
                                        {
                                            bedInfo.InfectionType = list2[0].Name;
                                        }


                                    }
                                }


                               
                            }
                        }


                        using (var machineTypeDao = new MachineTypeDao())
                        {

                            condition.Clear();
                            condition["Id"] = bed.MachineTypeId;
                            var list3 = machineTypeDao.SelectMachineType(condition);
                            if (list3.Count == 1)
                            {
                                string co = list3[0].BgColor;
                                Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(co));
                                bedInfo.BedBrush = bgBrush;
                            }
                            
                        }


                        /*
                        if (bed.InfectTypeId == -1)
                        {
                            bedInfo.InfectionType = "阴性";
                        }
                        else
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = bed.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    bedInfo.InfectionType = arealist[0].Name;
                                }
                            }
                        }
                        */

                        
                        //初始化病床列表时需要将以排床的病人放入相应床上 

                        foreach (var bedPatientData in UnPatientList)
                        {
                            if (bedPatientData.BedId == bedInfo.Id)
                            {
                                bedInfo.PatientName = bedPatientData.Name + "\n" + bedPatientData.TreatMethod;
                                bedInfo.PatientData = bedPatientData;
                                ///////////////////////////////////////////////////////////
                                string ampme = GetTime();
                                DateTime dt = GetDate();
                                try
                                {
                                    using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                                    {
                                        condition.Clear();
                                        condition = new Dictionary<string, object>();
                                        condition["PatientId"] = bedPatientData.Id;
                                        condition["Date"] = dt.Date.ToString("yyyy-MM-dd");
                                        condition["AmPmE"] = ampme;
                                        condition["BedId"] = bedPatientData.BedId;
                                        var list11 = scheduleDao.SelectScheduleTemplate(condition);
                                        bedInfo.IsTemp = list11[0].IsTemp;

                                    }


                                }
                                catch (Exception ex)
                                {
                                    MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
                                }
                                ///////////////////////////////////////////////////////////////////////////
                            
                                BedPatientList.Remove(bedPatientData);
                                break;
                            }
                        }

                        


                        bedInfo.IsAvailable = bed.IsAvailable;
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
                UnPatientList.Clear();
                FixBedPatientList.Clear();
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
                                    patientInfo.Id = patientlist[0].Id; //
                                    patientInfo.Name = patientlist[0].Name;
                                    patientInfo.PatientId = patientlist[0].PatientId; //可能为空
                                    patientInfo.BedId = patient.BedId;
                                    patientInfo.TreatMethod = patient.Method;
                                    //TODO:需要通过method查询type Name
                                    /*using (TreatMethodDao treatMethodDao = new TreatMethodDao())
                                    {
                                        var condition1001 = new Dictionary<string, object>();
                                        condition1001["NAME"] = patient.Method;
                                        var list1001 = treatMethodDao.SelectTreatMethod(condition1001);

                                        using (TreatTypeDao treatTypeDao = new TreatTypeDao())
                                        {
                                            var condition1002 = new Dictionary<string, object>();
                                            condition1002["ID"] = list1001[0].TreatTypeId;
                                            var list1002 = treatTypeDao.SelectTreatType(condition1002);
                                            patientInfo.Type = list1002[0].Name;//超范围
                                        }
                                        
                                    }*/

                                    List<TreatOrder> TreatOrderList = new List<TreatOrder>();
                                    try
                                    {
                                        using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                                        {

                                            condition.Clear();

                                            condition["PATIENTID"] = patientInfo.Id;
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
                                            str += treatOrder.Seq;
                                            str += "/";
                                            str += treatOrder.Description;
                                            str += "\n";

                                            treatOrders += str;
                                        }
                                        treatOrders = treatOrders.Remove(treatOrders.LastIndexOf("\n"), 1);
                                        patientInfo.ToolTips = treatOrders;
                                    }
                                    else
                                    {
                                        patientInfo.ToolTips = null;
                                    }

                                    
                                    /*string treatOrders = "";
                                    string orders = patientlist[0].Orders;
                                    if (!string.IsNullOrEmpty(orders))
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
                                        patientInfo.ToolTips = treatOrders;
                                    }
                                    else
                                    {
                                        patientInfo.ToolTips = "";
                                    }*/

                                    using (var infectTypeDao = new InfectTypeDao())
                                    {
                                        condition.Clear();
                                        condition["ID"] = patientlist[0].InfectTypeId;
                                        var arealist = infectTypeDao.SelectInfectType(condition);
                                        if (arealist.Count == 1)
                                        {
                                            patientInfo.InfectionType = arealist[0].Name;
                                        }
                                        else
                                        {
                                            patientInfo.InfectionType = "阴性";
                                        }
                                    }

                                    //if (patient.BedId == -1 || patientlist[0].IsFixedBed)
                                    if (patient.BedId == -1)
                                    {

                                        BedPatientList.Add(patientInfo);//未排床的病人列表

                                    }
                                        
                                    else
                                    {
                                        if (patientlist[0].IsFixedBed)
                                        {
                                            FixBedPatientList.Add(patientInfo);//固定床位病人列表因为要继承上一次的床，而上一次的床可能已经修改，所以需要重新排
                                        }
                                        else
                                            UnPatientList.Add(patientInfo);//已排床的病人列表
                                    }




                                }
                            }
                            
                            
                        }

                        PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
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
                /*long treatid = -1;
                using (var typeDao = new TreatTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["Name"] = BedInfoList[index].TreatType;
                    var list = typeDao.SelectTreatType(condition);
                    if (list != null && list.Count == 1)
                    {
                        treatid = list[0].Id;
                    }
                }*/

                BedPatientData data = (BedPatientData)draggedItem;

                effects = System.Windows.DragDropEffects.None;
                //此处还需要检查病人是否属于这个病区，也就是感染类型是否对应
                if (BedInfoList[index].InfectionType == data.InfectionType )
                {
                    using (var methodDao = new TreatMethodDao())
                    {
                        int DoublePump = -1;
                        int SinglePump = -1;
                        var condition = new Dictionary<string, object>();
                        condition["NAME"] = data.TreatMethod;
                        var list = methodDao.SelectTreatMethod(condition);
                        if (list.Count == 1)
                        {
                            if (list[0].DoublePump == true)
                            {
                                DoublePump = 1;
                            }
                            if (list[0].SinglePump == true)
                            {
                                SinglePump = 0;
                            }

                        }

                        if (BedInfoList[index].MachineTypeID == DoublePump || BedInfoList[index].MachineTypeID == SinglePump)
                        {
                            effects = System.Windows.DragDropEffects.Move;
                        }
                    }


                    

                    /*long patientInfectTypeId = -1;
                    using (PatientDao patientDao = new PatientDao())
                    {
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = data.Id;
                        var list = patientDao.SelectPatient(condition);
                        patientInfectTypeId = list[0].InfectTypeId;
                    }

                    long patientAreaInfectTypeId = -1;

                    using (BedDao bedDao = new BedDao())
                    {
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = BedInfoList[index].Id;
                        var list = bedDao.SelectBed(condition);
                        long patientAreaId = list[0].PatientAreaId;

                         using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                        {
                            var condition2 = new Dictionary<string, object>();
                            condition2["ID"] = patientAreaId;
                            var list2 = patientAreaDao.SelectPatientArea(condition2);
                             patientAreaInfectTypeId = list2[0].InfectTypeId;

                        }
                    }


                    if (patientInfectTypeId == patientAreaInfectTypeId)
                    {
                        effects = System.Windows.DragDropEffects.Move;
                    }
                    else
                    {
                        effects = System.Windows.DragDropEffects.None;
                        //var a = new RemindMessageBox1();
                        //a.remindText.Text = "病区感染类型不匹配.";
                        //a.ShowDialog();
                    }*/
                }
                else
                {
                    effects = System.Windows.DragDropEffects.Scroll;
                    var a = new RemindMessageBox1();
                    a.remindText.Text = "感染类型不匹配.";
                    a.ShowDialog();
                }
            }
            e.Handled = true;
        }
        //SELECT * FROM (Bed INNER JOIN PatientRoom ON Bed.PatientRoomId=PatientRoom.Id) INNER JOIN InfectType ON PatientRoom.InfectTypeId=InfectType.Id
        private void UpdateBedId(long patientID, DateTime dateTime, string ampme , long bedid, bool isAuto = true )
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
                    if (bedid == -1)
                    {
                        fileds["ISTEMP"] = false;
                    }
                    fileds["ISAUTO"] = isAuto;
                    scheduleDao.UpdateScheduleTemplate(fileds, condition);
                    
                }


                /*bool isTemp = false;
                using (var bedDao = new BedDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = bedid;
                    var list = bedDao.SelectBed(condition);
                    isTemp = list[0].IsTemp;

                }

                if (isTemp == false )
                {
                    using (var patientDao = new PatientDao())
                    {
                        var fields = new Dictionary<string, object>();
                        fields["BEDID"] = bedid;
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = patientID;
                        patientDao.UpdatePatient(fields, condition);

                    }
                }*/

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
                            UpdateBedId(BedInfoList[index].PatientData.Id, dt.Date, ampme, -1, false);
                            BedPatientList.Add(BedInfoList[index].PatientData);
                        }
                        BedPatientData data = (BedPatientData)draggedItem;
                        BedInfoList[index].PatientName = data.Name + "\n" + data.TreatMethod;
                        BedInfoList[index].PatientData = data;
                        BedPatientList.Remove((BedPatientData)draggedItem);

                        DateTime dt1 = GetDate();
                        //UpdateBedId(data.Id, DateTime.Parse("2015-06-10"), ampme, BedInfoList[index].Id);
                        UpdateBedId(data.Id, dt1.Date, ampme, BedInfoList[index].Id, false);
                    }
                }
                if (effects == (System.Windows.DragDropEffects)DragDropEffects.None)
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = "治疗方法与机型不匹配.";
                    a.ShowDialog();
                }

            }

            PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
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
            if (BedInfoList[index].PatientData == null) return;
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
            if(BedInfoList[index].PatientData!= null)
            {
                DateTime dt = GetDate();
                //UpdateBedId(BedInfoList[index].PatientData.Id, DateTime.Parse("2015-06-10"), ampme, -1);
                UpdateBedId(BedInfoList[index].PatientData.Id, dt.Date, ampme, -1);
                BedInfoList[index].PatientData = null;
            }

            PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
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
        private void UncheckOtherToggleButton2(ToggleButton btn)
        {
            foreach (var i in InfectGrid.Children)
            {
                if ((i is ToggleButton) && i != btn)
                {
                    ((ToggleButton)i).IsChecked = false;
                }
            }
        }

        private void UncheckOtherToggleButton()
        {
            foreach (var i in GridDay.Children)
            {
                if ((i is ToggleButton))
                {
                    ((ToggleButton)i).IsChecked = false;
                }
            }
        }

        private void BtnInfect_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            if (btn.IsChecked == true)
            {
                UncheckOtherToggleButton2(btn);
            }
            else
            {
                btn.IsChecked = true;
                UncheckOtherToggleButton2(btn);
            }
            RefreshData();

        }

        private void ClearGrid()
        {
            List<UIElement> list = new List<UIElement>();
            foreach (UIElement i in InfectGrid.Children)
            {
                
                if ((i is ToggleButton))
                {
                    //((ToggleButton)i).IsChecked = false;
                    list.Add(i);
                    
                }
            }
            foreach (var element in list)
            {
                InfectGrid.Children.Remove(element);
            }
            
        }
        private void LoadPatientAreas()
        {
            try
            {
                //InfectGrid.Children.Clear();
                ClearGrid();
                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    var arealist = patientAreaDao.SelectPatientArea(condition);
                    /*if (arealist.Count == 1)
                    {
                        bedData.PatientArea = arealist[0].Name;
                    }*/
                    int n = 0;
                    foreach (var infectType in arealist)
                    {
                        ToggleButton btn = new ToggleButton();
                        btn.VerticalAlignment = VerticalAlignment.Top;
                        btn.Height = 50;
                        btn.Width = 122;
                        btn.Content = infectType.Name;
                        btn.Click += BtnInfect_OnClick;
                        btn.Template = this.FindResource("ToggleButtonControlTemplate3") as ControlTemplate;
                        //btn.Style = this.FindResource("ToggleButtonStyle") as Style;
                        /*Grid.SetColumn(btn, n);
                        Grid.SetRow(btn, 0);*/

                        InfectGrid.Children.Add(btn);
                        if (n == 0)
                            btn.IsChecked = true;
                        n++;
                    }
                }


            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            
        }

        private void BtnSortByName_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ListSortDirection sortDirection = ListSortDirection.Ascending;

            Button btn = (Button) sender;

            //Get clicked column
            int columnflag = 0;



            //Get binding property of clicked column
            //string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path; //得到单击列所绑定的属性
            string bindingProperty = "";
            if (btn.Uid == "0")
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
                bindingProperty = "Name";

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
            sdc.Clear();
            
            
        }

        private bool isCurrent = true;
        private void BtnCurrentWeek_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            isCurrent = true;
            dtlist.Clear();
            int weeknow = (int)DateTime.Now.DayOfWeek - 1;
            if (weeknow < 0)
            {
                for (int n = 6; n >= 0; n--)
                {
                    dtlist.Add(DateTime.Now.AddDays(-n));
                }
            }
            else
            {
                for (int n = 0; n < 7; n++)
                {
                    dtlist.Add(DateTime.Now.AddDays(-weeknow + n));
                }
            }

            btn0.Content = dtlist[0].ToString("MM-dd");
            btn1.Content = dtlist[1].ToString("MM-dd");
            btn2.Content = dtlist[2].ToString("MM-dd");
            btn3.Content = dtlist[3].ToString("MM-dd");
            btn4.Content = dtlist[4].ToString("MM-dd");
            btn5.Content = dtlist[5].ToString("MM-dd");
            btn6.Content = dtlist[6].ToString("MM-dd");
            RefreshData();
        }

        private void BtnNextWeek_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (isCurrent == false) return;
            for (int n = 0; n < 7; n++)
            {
                dtlist[n] = dtlist[n].AddDays(7);
            }

            btn0.Content = dtlist[0].ToString("MM-dd");
            btn1.Content = dtlist[1].ToString("MM-dd");
            btn2.Content = dtlist[2].ToString("MM-dd");
            btn3.Content = dtlist[3].ToString("MM-dd");
            btn4.Content = dtlist[4].ToString("MM-dd");
            btn5.Content = dtlist[5].ToString("MM-dd");
            btn6.Content = dtlist[6].ToString("MM-dd");
            RefreshData();
            isCurrent = false;
        }

        private void ChangeToTempBed_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                /*int index = BedListBox.SelectedIndex;
                if (index == -1) return;
                BedInfoList[index].IsTemp = !BedInfoList[index].IsTemp;

                using (var bedDao = new BedDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = BedInfoList[index].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["ISTEMP"] = BedInfoList[index].IsTemp;
                    bedDao.UpdateBed(fileds, condition);
                }*/

                int index = BedListBox.SelectedIndex;
                if (index == -1) return;
                if (BedInfoList[index].PatientData == null) return;//床上没有人
                //修改床是否零时床的属性
                BedInfoList[index].IsTemp = !BedInfoList[index].IsTemp;
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
                DateTime dt = GetDate();
                //UpdateBedId(BedInfoList[index].Id, DateTime.Parse("2015-06-10"), ampme, -1);
                //UpdateBedId(BedInfoList[index].PatientData.Id, dt.Date, ampme, -2);


                try
                {
                    using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                    {

                        Dictionary<string, object> condition = new Dictionary<string, object>();
                        condition["PatientId"] = BedInfoList[index].PatientData.Id;
                        condition["Date"] = dt.Date.ToString("yyyy-MM-dd");
                        condition["AmPmE"] = ampme;
                        condition["BedId"] = BedInfoList[index].Id;
                        var fileds = new Dictionary<string, object>();
                        fileds["IsTemp"] = BedInfoList[index].IsTemp;
                        scheduleDao.UpdateScheduleTemplate(fileds, condition);

                    }


                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In PatientSchedule.xaml.cs:GetPatientSchedule select patient exception messsage: " + ex.Message);
                }

                

            }

        }


        private void Bed_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                int index = BedListBox.SelectedIndex;
                if (index == -1) return;
                if (BedInfoList[index].PatientData == null) return;
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
                if (BedInfoList[index].PatientData != null)
                {
                    DateTime dt = GetDate();
                    //UpdateBedId(BedInfoList[index].PatientData.Id, DateTime.Parse("2015-06-10"), ampme, -1);
                    UpdateBedId(BedInfoList[index].PatientData.Id, dt.Date, ampme, -1, false);
                    BedInfoList[index].PatientData = null;
                    BedInfoList[index].IsTemp = false;//双击后将床位变为不是零时床
                }

                PatientCountLabel.Content = "待排患者(" + BedPatientList.Count + ")";
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
        private string _treatType;
        private Brush _itemBgBrush;
        private string _method;

        private static Dictionary<string, Color> TreatMethodDictionary = new Dictionary<string, Color>();
        public BedPatientData()
        {
            Name = "";
            ToolTips = "";
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

        public string TreatMethod
        {
            get { return _method; }
            set
            {
                _method = value;
                _itemBgBrush = new SolidColorBrush(StrColorConverter(_method));
                OnPropertyChanged("ItemBgBrush");
            }
        }
        public string InfectionType { get; set; }
        public string ToolTips { get; set; }

        public string Type
        {
            get { return _treatType; }
            set
            {
                _treatType = value;
                //_itemBgBrush = new SolidColorBrush(StrColorConverter(_treatType));
                OnPropertyChanged("Type");
                //OnPropertyChanged("ItemBgBrush");
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
                /*using (var typeDao = new TreatTypeDao())
                {
                    TreatMethodDictionary.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = typeDao.SelectTreatType(condition);
                    foreach (var pa in list)
                    {
                        var treatTypeData = new TreatTypeData();
                        treatTypeData.Id = pa.Id;
                        treatTypeData.Name = pa.Name;
                        //{
                        //    using (var treatTypeDao = new TreatTypeDao())
                        //    {
                        //        condition.Clear();
                        //        condition["ID"] = pa.TreatTypeId;
                        //        var arealist = treatTypeDao.SelectTreatType(condition);
                        //        if (arealist.Count == 1)
                        //        {
                        //            treatTypeData.Type = arealist[0].Name;
                        //        }
                        //    }
                        //}
                        string bgColor = pa.BgColor;
                        Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        if (bgColor != "" && bgColor != null)
                            treatTypeData.BgColor = bgBrush;
                        else
                            treatTypeData.BgColor = Brushes.LightGray;

                        //treatTypeData.IsAvailable = pa.IsAvailable;

                        treatTypeData.Description = pa.Description;
                        TreatMethodDictionary.Add(pa.Name, ((SolidColorBrush)treatTypeData.BgColor).Color);
                    }
                }*/


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
                        treatMethodData.IsAvailable = pa.IsAvailable;
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
        private Int64 _PatientAreaID;
        private Int64 _MachineTypeID;
        private int _roomID;
        private string _bedName;
        private string _patientName;
        private int _infcetionType;
        private string _treatType;
        private string _infectionType;

        private Brush _titleBrush;
        private Brush _bedBrush;
        private static Dictionary<string, Color> TreatTypeDictionary = new Dictionary<string, Color>();

        private bool _isAvliable;
        private bool _isOccupy;
        private bool _isTemp;

        public BedPatientData PatientData { get; set; }

        public BedInfo()
        {
            _bedName = "";
            _patientName = "";
            _treatType = "";
            _titleBrush = Brushes.GreenYellow;
            _bedBrush = Brushes.RoyalBlue;
            _isAvliable = false;
            _isOccupy = true;
            PatientData = null;
            LoadTreatType();

        }
        public Int64 PatientAreaID
        {
            get { return _PatientAreaID; }
            set
            {
                _PatientAreaID = value;
                //OnPropertyChanged("Id");
            }
        }

        public Int64 MachineTypeID
        {
            get { return _MachineTypeID; }
            set
            {
                _MachineTypeID = value;
                //OnPropertyChanged("Id");
            }
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

        public bool IsTemp
        {
            get { return _isTemp; }
            set
            {
                _isTemp = value;

                if (_isTemp == true )
                    _titleBrush = Brushes.Orange;
                else
                {
                    _titleBrush = Brushes.Gray;
                }
                OnPropertyChanged("TitleBrush");

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
            get { return _infectionType; }
            set
            {
                
                /*if((string)value == "阴性" )
                    _titleBrush = Brushes.GreenYellow;
                else
                {
                    _titleBrush = Brushes.Red;
                }*/
                _infectionType = value;
                //OnPropertyChanged("TitleBrush");
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

        public string TreatType
        {
            get { return _treatType; }
            set
            {
                _treatType = value;
                //_bedBrush = new SolidColorBrush(StrColorConverter(_treatType));
                OnPropertyChanged("TreatType");
                //OnPropertyChanged("BedBrush");
            }
        }

        public Brush BedBrush
        {
            get { return _bedBrush; }
            set
            {
                _bedBrush = value;
                OnPropertyChanged("BedBrush");
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

        private static void LoadTreatType()
        {
            try
            {

                using (var methodDao = new TreatTypeDao())
                {
                    TreatTypeDictionary.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = methodDao.SelectTreatType(condition);
                    foreach (var pa in list)
                    {
                        Brush b ;
                        string bgColor = pa.BgColor;
                        Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        if (bgColor != "" && bgColor != null)
                            b = bgBrush;
                        else
                            b = Brushes.LightGray;
                        TreatTypeDictionary.Add(pa.Name, ((SolidColorBrush)b).Color);
                    }
                }

                //using (var methodDao = new TreatMethodDao())
                //{
                //    TreatMethodDictionary.Clear();
                //    var condition = new Dictionary<string, object>();
                //    var list = methodDao.SelectTreatMethod(condition);
                //    foreach (var pa in list)
                //    {
                //        var treatMethodData = new TreatMethodData();
                //        treatMethodData.Id = pa.Id;
                //        treatMethodData.Name = pa.Name;
                //        {
                //            using (var treatTypeDao = new TreatTypeDao())
                //            {
                //                condition.Clear();
                //                condition["ID"] = pa.TreatTypeId;
                //                var arealist = treatTypeDao.SelectTreatType(condition);
                //                if (arealist.Count == 1)
                //                {
                //                    treatMethodData.Type = arealist[0].Name;
                //                }
                //            }
                //        }
                //        string bgColor = pa.BgColor;
                //        Brush bgBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(bgColor));
                //        if (bgColor != "" && bgColor != null)
                //            treatMethodData.BgColor = bgBrush;
                //        else
                //            treatMethodData.BgColor = Brushes.LightGray;

                //        treatMethodData.IsAvailable = pa.IsAvailable;
                //        treatMethodData.Description = pa.Description;
                //        TreatMethodDictionary.Add(pa.Name, ((SolidColorBrush)treatMethodData.BgColor).Color);
                //    }
                //}
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        public static string StrColorConverter(Brush brush)
        {
            Color color = ((SolidColorBrush)brush).Color;
            foreach (var v in TreatTypeDictionary)
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
            return TreatTypeDictionary[str];
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

                        treatMethodData.IsAvailable = pa.IsAvailable;
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
