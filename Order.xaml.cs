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
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : UserControl
    {
        public MainWindow Basewindow;
        public int selectoperation;
        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public ObservableCollection<TreatOrder> TreatOrderList = new ObservableCollection<TreatOrder>();

        public ObservableCollection<TreatMethodData> TreatMentList = new ObservableCollection<TreatMethodData>();
        public ObservableCollection<MedicalOrderParaData> OrderParaList = new ObservableCollection<MedicalOrderParaData>();
        public Order(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            this.PatientlistView.ItemsSource = PatientList;
            this.MedicalOrderlistView.ItemsSource = TreatOrderList;
            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();
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

        private void PatientlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientlistView.SelectedIndex >= 0)
            {
                TreatOrderList.Clear();
                int patientID = PatientlistView.SelectedIndex;
                try
                {

                    using (var patientDao = new PatientDao())
                    {
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = PatientList[patientID].PatientId;
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
                                        //string type = list1[0].Type;
                                        //string count = list1[0].Count.ToString();
                                        
                                        
                                        //treat.Type = details[1];
                                        treat.Type = list1[0].Name;
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
                if (!IDTextBox.Text.Equals(""))
                    condition["ID"] = IDTextBox.Text;
                if (!PatientIDTextBox.Text.Equals(""))
                    condition["PATIENTID"] = PatientIDTextBox.Text;
                condition["TREATSTATUSID"] = 1;
                PatientList.Clear();

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
                    //informatian.PatientIsFixedBed = fmriPatient.IsFixedBed;
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
                    informatian.PatientPatientId = fmriPatient.PatientId;
                    informatian.PatientName = fmriPatient.Name;
                    informatian.PatientRegesiterDate = fmriPatient.RegisitDate;
                    PatientList.Add(informatian);
                }
            }
        }

        private void PatientlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

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
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Visible;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;

                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowID.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
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
                        bindingProperty = "PatientPatientId";
                        ArrowPatientID.Visibility = Visibility.Visible;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        //TODO: 20130923
                        if (
                          ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                              ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowPatientID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowPatientID.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowPatientID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
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
                        bindingProperty = "PatientName";
                        ArrowName.Visibility = Visibility.Visible;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowName.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowName.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowName.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 3)
                    {
                        if (Paixiflag[3] == 0)
                        {
                            Paixiflag[3] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[3] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientGender";
                        ArrowSex.Visibility = Visibility.Visible;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;


                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowSex.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowSex.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowSex.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 4)
                    {
                        if (Paixiflag[4] == 0)
                        {
                            Paixiflag[4] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[4] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientDob";
                        ArrowAge.Visibility = Visibility.Visible;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowAge.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowAge.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowAge.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 5)
                    {
                        if (Paixiflag[5] == 0)
                        {
                            Paixiflag[5] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[5] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientInfectType";
                        ArrowInfectTypeId.Visibility = Visibility.Visible;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowInfectTypeId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowInfectTypeId.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowInfectTypeId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 6)
                    {
                        if (Paixiflag[6] == 0)
                        {
                            Paixiflag[6] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[6] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientTreatStatus";
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Visible;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowTreatStatusId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowTreatStatusId.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowTreatStatusId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 7)
                    {
                        if (Paixiflag[7] == 0)
                        {
                            Paixiflag[7] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[7] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientIsFixedBed";
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Visible;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowDate.Visibility = Visibility.Hidden;
                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowIsFixedBed.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowIsFixedBed.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowIsFixedBed.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 8)
                    {
                        if (Paixiflag[8] == 0)
                        {
                            Paixiflag[8] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[8] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientIsAssigned";
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Visible;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;

                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowIsAssigned.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowIsAssigned.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowIsAssigned.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 9)
                    {
                        if (Paixiflag[9] == 0)
                        {
                            Paixiflag[9] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[9] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientRegesiterDate";
                        ArrowDate.Visibility = Visibility.Visible;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;

                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowDate.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 10)
                    {
                        if (Paixiflag[10] == 0)
                        {
                            Paixiflag[10] = 1;
                            sortDirection = ListSortDirection.Ascending;
                        }
                        else
                        {
                            Paixiflag[10] = 0;
                            sortDirection = ListSortDirection.Descending;
                        }
                        bindingProperty = "PatientDescription";
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;

                        if (
                            ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
                                ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowDate.Source.ToString() ==
                                 "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        ArrowDate.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowSex.Visibility = Visibility.Hidden;
                        ArrowID.Visibility = Visibility.Hidden;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowPatientID.Visibility = Visibility.Hidden;
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
                        ArrowTreatStatusId.Visibility = Visibility.Hidden;
                        ArrowIsAssigned.Visibility = Visibility.Hidden;
                        ArrowIsFixedBed.Visibility = Visibility.Hidden;
                        return;
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
                    var temp = new ObservableCollection<PatientInfo>();
                    for (int i = 0; i < PatientlistView.Items.Count; i++)
                    {
                        temp.Add((PatientInfo)PatientlistView.Items[i]);
                    }
                    PatientList.Clear();
                    PatientList = temp;
                    PatientlistView.ItemsSource = PatientList;
                    sdc.Clear();
                }
            }
        }

        private void Order_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadTratementConifg();
            LoadOrderParaConfig();
            //throw new NotImplementedException();
            try
            {
                PatientList.Clear();
                using (ComplexDao patientDao = new ComplexDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["TREATSTATUSID"] = 1;
                    //var list = patientDao.SelectPatient(condition);
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
                        patientInfo.PatientRegesiterDate = type.RegisitDate;
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
                        patientInfo.PatientIsFixedBed = type.IsFixedBed;
                        patientInfo.PatientIsAssigned = type.IsAssigned;
                        patientInfo.PatientDescription = type.Description;
                        PatientList.Add(patientInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }

        }


        private void CbTreatMathod_Initialized(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            foreach (var v in TreatMentList)
            {
                cb.Items.Add(v.Name);
            }

        }

        private void CbOrderPara_Initialized(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            foreach (var v in OrderParaList)
            {
                cb.Items.Add(v.Name);
            }

        }

        private void LoadTratementConifg()
        {
            try
            {
                using (var methodDao = new TreatMethodDao())
                {
                    TreatMentList.Clear();
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

                        TreatMentList.Add(treatMethodData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        private void LoadOrderParaConfig()
        {
            try
            {
                using (var medicalOrderParaDao = new MedicalOrderParaDao())
                {
                    OrderParaList.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = medicalOrderParaDao.SelectInterval(condition);
                    foreach (var pa in list)
                    {
                        var medicalOrderParaData = new MedicalOrderParaData();
                        medicalOrderParaData.Id = pa.Id;
                        medicalOrderParaData.Name = pa.Name;
                        medicalOrderParaData.Type = pa.Type;
                        medicalOrderParaData.Count = pa.Count;
                        medicalOrderParaData.Description = pa.Description;
                        //Datalist.Add(medicalOrderParaData);
                        //OrderParaList.Add(pa.Name);
                        OrderParaList.Add(medicalOrderParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            int index = PatientlistView.SelectedIndex;
            if (index == -1) return;
            TreatOrderList.Add(new TreatOrder());
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveOrder();
        }

        private void SaveOrder()
        {
            int index = PatientlistView.SelectedIndex;
            if (index == -1) return;
            string MedicalOrder = "";
            foreach (var v in TreatOrderList)
            {
                var medicalOrderParaDao = new MedicalOrderParaDao();
                var condition1 = new Dictionary<string, object>();
                condition1["NAME"] = v.Type;
                var list1 = medicalOrderParaDao.SelectInterval(condition1);


                string str = v.TreatMethod + "/" + list1[0].Id + "/" + v.TreatTimes;
                MedicalOrder += str;
                MedicalOrder += "#";
            }
            try
            {
                long patientID = PatientList[index].PatientId;

                using (var patientDao = new PatientDao())
                {
                    var fields = new Dictionary<string, object>();
                    fields["ORDERS"] = MedicalOrder;
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = patientID;
                    patientDao.UpdatePatient(fields, condition);

                }

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In init.xaml.cs: ButtonNewSaveClick insert patient exception messsage: " + ex.Message);
            }
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            int index = MedicalOrderlistView.SelectedIndex;
            if ( index == -1) return;
            TreatOrderList.RemoveAt(index);
            SaveOrder();
        }

    }

    public class TreatOrder : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _treatMethod;
        private string _type;
        private int _treatTimes;

        public TreatOrder()
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

        public string TreatMethod
        {
            get { return _treatMethod; }
            set
            {
                _treatMethod = value;
                OnPropertyChanged("TreatMethod");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public int TreatTimes
        {
            get { return _treatTimes; }
            set
            {
                _treatTimes = value;
                OnPropertyChanged("TreatTimes");
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

    //public class TreatMent : INotifyPropertyChanged
    //{
    //    private string _treatMethod;

    //    public TreatMent()
    //    {
    //    }
    //    public TreatMent( string str )
    //    {
    //        _treatMethod = str;
    //    }

        
    //    public string TreatMethod
    //    {
    //        get { return _treatMethod; }
    //        set
    //        {
    //            _treatMethod = value;
    //            OnPropertyChanged("TreatMethod");
    //        }
    //    }
    //    #region INotifyPropertyChanged Members

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    #endregion

    //    private void OnPropertyChanged(String info)
    //    {
    //        if (PropertyChanged != null)
    //            PropertyChanged(this, new PropertyChangedEventArgs(info));
    //    }
    //}
}
