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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;
using WpfApplication1.Utils;

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
        public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public int selectoperation;
        public Init(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            this.PatientlistView.ItemsSource = PatientList;

            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();
            AddTimeDate.Text = "dateTimeString";

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("所有");
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
            SexComboBox.SelectedIndex = 0;
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

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            NewOrEditFlag = 1;
            grid1.IsEnabled = true;
            AddIDTextBox.Text = "";
            AddPatientIDTextBox.Text = "";
            Add_NameTextBox.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;
            AddAgeTextBox.IsEnabled = true;
            SexComboBox.IsEnabled = true;
            InfectTypeComboBox.IsEnabled = true;
            AddTimeDate.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;
            StatusComboBox.IsEnabled = true;

            NewButton.IsEnabled = false;
            CheckButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            InquireButton.IsEnabled = false;
            foreach (FrameworkElement a in TopGrid.Children)
            {
                a.IsEnabled = false;
            }

            //int lastInsertId = -1;
            //using (var patientDao = new PatientDao())
            //{
            //    var patient = new Patient();
            //    patient.Name = "";
            //    patient.PatientId = "";
            //    patient.RegisitDate = "";
            //    patientDao.InsertPatient(patient, ref lastInsertId);
            //}
            //AddIDTextBox.Text = lastInsertId.ToString("D8");
            NewButton.IsEnabled = false;
            CheckButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            //this.TopGrid.IsEnabled = false;
            PatientlistView.IsEnabled = false;

            AddPatientIDTextBox.Text = "";
            Add_NameTextBox.Text = "";
            AddAgeTextBox.Text = "";
            SexComboBox.SelectedIndex = 0;
            InfectTypeComboBox.SelectedIndex = 0;
            DescriptionTextBox.Text = "";
            StatusComboBox.SelectedIndex = 1;
            AddTimeDate.SelectedDate = DateTime.Now;
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (PatientlistView.SelectedIndex == -1)
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message17");
                a.ShowDialog();


                return;
            }
            else
            {
                NewOrEditFlag = 2;
                grid1.IsEnabled = true;

                AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId.ToString();

                Add_NameTextBox.IsEnabled = true;
                DescriptionTextBox.IsEnabled = true;
                MobileTextBox.IsEnabled = true;
                IsFixedBedCheckBox1.IsEnabled = true;
                AddAgeTextBox.IsEnabled = true;
                SexComboBox.IsEnabled = true;
                InfectTypeComboBox.IsEnabled = true;
                AddTimeDate.IsEnabled = true;
                DescriptionTextBox.IsEnabled = true;
                StatusComboBox.IsEnabled = true;
                AddSaveButton.Visibility = Visibility.Visible;


                NewButton.IsEnabled = false;
                CheckButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                InquireButton.IsEnabled = false;

                foreach (FrameworkElement a in TopGrid.Children)
                {
                    a.IsEnabled = false;
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientlistView.SelectedIndex == -1)
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message4");
                a.ShowDialog();
            }
            else
            {
                int i = PatientlistView.SelectedIndex;
                //if (((PatientInfo)PatientlistView.Items[i]).PatientId.Equals(Basewindow._stimulusLeft._patient.ID.Text))
                //{
                //    var remindMessageBox1 = new RemindMessageBox1();
                //    remindMessageBox1.remindText.Text = (string)FindResource("Message50");
                //    remindMessageBox1.ShowDialog();
                //    return;
                //}
                var confirmdialog = new RemindMessageBox3();
                //confirmdialog.textBlock1.Text = (string) FindResource("Message18");
                confirmdialog.ShowDialog();
                if (confirmdialog.remindflag == 2)
                {
                    return;
                }

                int id = int.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId.ToString());

                using (var patientDao = new PatientDao())
                {

                    if (confirmdialog.remindflag == 1)
                    {

                        var messageBox2 = new RemindMessageBox2();
                        messageBox2.textBlock1.Text =
                            (string)FindResource("Message67");
                        messageBox2.ShowDialog();
                        if (messageBox2.remindflag == 1)
                        {
                            //ysq 13.8.9 17:20
                            //删除文件 
                            //删除硬盘文件
                            Directory.Delete(ConstDefinition.ImageDataDir + "\\" +
                                                                              id.ToString("D8"), true);
                            patientDao.DeletePatient(id);
                        }
                    }
                    else if (confirmdialog.remindflag == 3)
                    {
                        patientDao.DeletePatient(id);
                    }

                }


                MainWindow.Log.WriteInfoLog("Delete the patient at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                                         " who`s id is: " + ((PatientInfo)PatientlistView.Items[i]).PatientId);

                DateTime begin = DateTime.Parse(BeginDatePicker.Text);
                DateTime end = DateTime.Parse(EndatePicker.Text);
                RefreshData(begin, end);
            }
        }

        private void RefreshData(DateTime begin, DateTime end)
        {
            PatientList.Clear();

            using (var complexDao = new ComplexDao())
            {
                var condition = new Dictionary<string, object>();
                List<Patient> list = complexDao.SelectPatient(condition, begin, end);
                foreach (Patient patient in list)
                {
                    var informatian = new PatientInfo();
                    informatian.PatientDob = patient.Dob;
                    informatian.PatientGender = patient.Gender;
                    informatian.PatientPatientId = patient.PatientId.ToString();
                    informatian.PatientMobile = patient.Mobile;
                    informatian.PatientDescription = patient.Description;
                    informatian.PatientId = patient.Id;
                    informatian.PatientName = patient.Name;
                    {
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition.Clear();
                            condition["ID"] = patient.InfectTypeId;
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
                            condition["ID"] = patient.TreatStatusId;
                            var arealist = treatStatusDao.SelectTreatStatus(condition);
                            if (arealist.Count == 1)
                            {
                                informatian.PatientTreatStatus = arealist[0].Name;
                            }
                        }
                    }

                    DateTime displaytime;
                    try
                    {
                        displaytime = DateTime.Parse(patient.RegisitDate);
                        informatian.PatientRegesiterDate = displaytime.ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log.WriteErrorLog("Init.xaml.cs-RefreshData", ex);
                    }

                    PatientList.Add(informatian);
                }
            }

            selectoperation = 1;
        }

        private void PatientlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientlistView.SelectedIndex >= 0)
            {
                Add_NameTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientName;
                AddAgeTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientDob;
                if (NewOrEditFlag != 1)
                {
                    AddIDTextBox.Text = "";
                    AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId.ToString();
                }
                else
                {
                    AddIDTextBox.Text = "";
                }
                try
                {
                    AddTimeDate.SelectedDate =
                        DateTime.Parse(PatientList[PatientlistView.SelectedIndex].PatientRegesiterDate);
                }
                catch (Exception)
                {
                    AddTimeDate.Text = (string)FindResource("Message21");
                    MainWindow.Log.WriteErrorLog(AddTimeDate.Text);
                }
                AddPatientIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientPatientId;
                MobileTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientMobile;
                InfectTypeComboBox1.Text = PatientList[PatientlistView.SelectedIndex].PatientInfectType;
                StatusComboBox.Text = PatientList[PatientlistView.SelectedIndex].PatientTreatStatus;
                IsFixedBedCheckBox1.IsChecked = PatientList[PatientlistView.SelectedIndex].PatientIsFixedBed;
                DescriptionTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientDescription;
                if (PatientList[PatientlistView.SelectedIndex].PatientGender == (string)FindResource("ManText"))
                {
                    SexComboBox1.SelectedIndex = 0;
                }
                else if (PatientList[PatientlistView.SelectedIndex].PatientGender == (string)FindResource("WomanText"))
                {
                    SexComboBox1.SelectedIndex = 1;
                }
                else
                {
                    SexComboBox1.SelectedIndex = -1;
                }

                //if (PatientList[PatientlistView.SelectedIndex].PatientHand == (string)FindResource("LeftText"))
                //{
                //    LishouComboBox.SelectedIndex = 0;
                //}
                //else if (PatientList[PatientlistView.SelectedIndex].PatientHand == (string)FindResource("rightText"))
                //{
                //    LishouComboBox.SelectedIndex = 1;
                //}
                //else
                //{
                //    LishouComboBox.SelectedIndex = -1;
                //}
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
                    PatientList.Add(informatian);
                }
            }
        }

        private void PatientlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;

            if (PatientlistView.SelectedIndex >= 0)
            {
                CheckButton_Click(CheckButton, new RoutedEventArgs());
            }
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

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientlistView.SelectedIndex == -1)
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message17");
                a.ShowDialog();
                return;
            }
            else
            {
                NewOrEditFlag = 3;
                grid1.IsEnabled = true;
                AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId.ToString();

                Add_NameTextBox.IsEnabled = false;
                DescriptionTextBox.IsEnabled = false;
                AddAgeTextBox.IsEnabled = false;
                SexComboBox.IsEnabled = false;
                MobileTextBox.IsEnabled = false;
                IsFixedBedCheckBox1.IsEnabled = false;
                InfectTypeComboBox.IsEnabled = false;
                StatusComboBox.IsEnabled = false;
                AddTimeDate.IsEnabled = false;
                DescriptionTextBox.IsEnabled = false;


                NewButton.IsEnabled = false;
                CheckButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                InquireButton.IsEnabled = false;
                AddSaveButton.Visibility = Visibility.Hidden;

                foreach (FrameworkElement a in TopGrid.Children)
                {
                    if (a.Name != "PatientlistView" && a.Name != "EditButton")
                    {
                        a.IsEnabled = false;
                    }
                    else
                    {
                        a.IsEnabled = true;
                    }
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

        private bool CheckPatientPatientIdValidity(string patientPatientId)
        {
            try
            {
                if (patientPatientId.Trim().Equals(string.Empty))
                    return false;
                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["PATIENTID"] = patientPatientId.Trim();
                    List<Patient> list =
                                patientDao.SelectPatient(condition);
                    if (list.Count > 0)
                        return false;
                    else
                    {
                        return true;
                    }
                }


            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteErrorLog("Init.xaml.cs-CheckPatientPatientIdValidity", ex);
                return false;
            }
        }

        private bool CheckPatientNameValidity(string name)
        {
            try
            {
                if (name.Trim().Equals(string.Empty))
                    return false;
                using (var patientDao = new PatientDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["NAME"] = name.Trim();
                    List<Patient> list =
                                patientDao.SelectPatient(condition);
                    if (list.Count > 0)
                        return false;
                    else
                    {
                        return true;
                    }
                }


            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteErrorLog("Init.xaml.cs-CheckPatientNameValidity", ex);
                return false;
            }
        }


        private void ButtonNewSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NewOrEditFlag == 1)
                {
                    if (!CheckPatientPatientIdValidity(AddPatientIDTextBox.Text))
                    {
                        var myRemindMessageBox = new RemindMessageBox1();
                        myRemindMessageBox.remindText.Text = (string)FindResource("Message76");
                        myRemindMessageBox.ShowDialog();
                        return;
                    }

                    if (!CheckPatientNameValidity(Add_NameTextBox.Text))
                    {
                        var myRemindMessageBox = new RemindMessageBox1();
                        myRemindMessageBox.remindText.Text = (string)FindResource("Message761");
                        myRemindMessageBox.ShowDialog();
                        return;
                    }

                    int lastInsertId = -1;
                    using (var patientDao = new PatientDao())
                    {
                        var patient = new Patient();
                        patient.Name = "";
                        patient.PatientId = "";
                        patient.RegisitDate = "";
                        patientDao.InsertPatient(patient, ref lastInsertId);
                    }
                    AddIDTextBox.Text = lastInsertId.ToString("D8");

                    using (var patientDao = new PatientDao())
                    {
                        var fields = new Dictionary<string, object>();
                        //TODO for lipeifeng get the value of UI controls, set them into fields
                        fields["NAME"] = Add_NameTextBox.Text;
                        fields["DESCRIPTION"] = DescriptionTextBox.Text;
                        fields["DOB"] = AddAgeTextBox.Text;
                        fields["PATIENTID"] = AddPatientIDTextBox.Text;
                        fields["GENDER"] = SexComboBox1.SelectedValue;
                        fields["MOBILE"] = MobileTextBox.Text;
                        //fields["INFECTTYPEID"] = InfectTypeComboBox.SelectedValue;
                        //fields["TREATSTATUSID"] = StatusComboBox.SelectedValue;
                        var condition2 = new Dictionary<string, object>();
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition2.Clear();
                            condition2["Name"] = InfectTypeComboBox1.Text;
                            var arealist = infectTypeDao.SelectInfectType(condition2);
                            if (arealist.Count == 1)
                            {
                                fields["INFECTTYPEID"] = arealist[0].Id;
                            }
                        }
                       
                        using (var treatStatusDao = new TreatStatusDao())
                        {
                            condition2.Clear();
                            condition2["Name"] = StatusComboBox.Text;
                            var arealist = treatStatusDao.SelectTreatStatus(condition2);
                            if (arealist.Count == 1)
                            {
                                fields["TREATSTATUSID"] = arealist[0].Id;
                            }
                        }

                        fields["ISFIXEDBED"] = IsFixedBedCheckBox1.IsChecked;
                        fields["ISASSIGNED"] = false;
                        if (AddTimeDate.SelectedDate != null)
                        {
                            var dateTime = (DateTime)AddTimeDate.SelectedDate;
                            fields["REGISITDATE"] = dateTime.ToString("yyyy-MM-dd");
                        }
                        fields["DESCRIPTION"] = DescriptionTextBox.Text;

                        var condition = new Dictionary<string, object>();
                        condition["ID"] = int.Parse(AddIDTextBox.Text);

                        patientDao.UpdatePatient(fields, condition);

                        MainWindow.Log.WriteInfoLog("Insert a new patient at " +
                                                 DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                                                 " who`s id is: " + AddIDTextBox.Text + ", name is: " +
                                                 Add_NameTextBox.Text
                                                 + ", age is: " + AddAgeTextBox.Text
                                                 + ", gender is: " + (string)SexComboBox.SelectedValue
                                                 + ", infecttype is: " + (string)InfectTypeComboBox.SelectedValue);

                        var newinformation = new PatientInfo
                        {
                            PatientName = Add_NameTextBox.Text,
                            PatientId = Int64.Parse(AddIDTextBox.Text),
                            PatientGender = (string)SexComboBox1.SelectedValue,
                            PatientDob = AddAgeTextBox.Text,
                            PatientRegesiterDate =
                                ((DateTime)AddTimeDate.SelectedDate).ToString("yyyy-MM-dd"),
                            PatientDescription = DescriptionTextBox.Text,
                            PatientInfectType = (string)InfectTypeComboBox1.SelectedValue,
                            PatientTreatStatus = (string)StatusComboBox.SelectedValue,

                            PatientPatientId = AddPatientIDTextBox.Text,
                            PatientMobile = MobileTextBox.Text,
                           
                        };



                        PatientList.Insert(0, newinformation);
                        PatientlistView.SelectedIndex = 0;
                    }

                    try
                    {
                        using (ScheduleTemplateDao scheduleDao = new ScheduleTemplateDao())
                        {
                            ScheduleTemplate scheduleTemplate = new ScheduleTemplate();
                            scheduleTemplate.PatientId = Int64.Parse(AddIDTextBox.Text);


                            int ret = -1;
                            scheduleDao.InsertScheduleTemplate(scheduleTemplate, ref ret);
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log.WriteInfoConsole("In init.xaml.cs: ButtonNewSaveClick insert patient exception messsage: " + ex.Message);
                    }

                }
                else if (NewOrEditFlag == 2)
                {
                    //if (!CheckPatientPatientIdValidity(AddPatientIDTextBox.Text))
                    //{
                    //    var myRemindMessageBox = new RemindMessageBox1();
                    //    myRemindMessageBox.remindText.Text = (string)FindResource("Message76");
                    //    myRemindMessageBox.ShowDialog();
                    //    return;
                    //}
                    var deleteOriginalReportRemind = new RemindMessageBox2();
                    // deleteOriginalReportRemind.MainGrid.Height = (int)(deleteOriginalReportRemind.MainGrid.Height * 1.15);
                    deleteOriginalReportRemind.textBlock1.Text = (string)FindResource("Message19") +
                                                                 (string)FindResource("Message20");
                    deleteOriginalReportRemind.ShowDialog();
                    if (deleteOriginalReportRemind.remindflag != 1)
                        return;
                    int i = PatientlistView.SelectedIndex;

                    using (var patientDao = new PatientDao())
                    {
                        var fields = new Dictionary<string, object>();
                        //TODO for lipeifeng get the value of UI controls, set them into fields
                        fields["NAME"] = Add_NameTextBox.Text;
                        fields["DESCRIPTION"] = DescriptionTextBox.Text;
                        fields["DOB"] = AddAgeTextBox.Text;
                        fields["GENDER"] = SexComboBox1.SelectedValue;
                       // fields["INFECTTYPE"] = InfectTypeComboBox.SelectedValue;
                        fields["PATIENTID"] = AddPatientIDTextBox.Text;
                        //fields["INFECTTYPE"] = InfectTypeComboBox.SelectedValue;
                        var condition2 = new Dictionary<string, object>();
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition2.Clear();
                            condition2["Name"] = InfectTypeComboBox1.Text;
                            var arealist = infectTypeDao.SelectInfectType(condition2);
                            if (arealist.Count == 1)
                            {
                                fields["INFECTTYPEID"] = arealist[0].Id;
                            }
                        }

                        using (var treatStatusDao = new TreatStatusDao())
                        {
                            condition2.Clear();
                            condition2["Name"] = StatusComboBox.Text;
                            var arealist = treatStatusDao.SelectTreatStatus(condition2);
                            if (arealist.Count == 1)
                            {
                                fields["TREATSTATUSID"] = arealist[0].Id;
                            }
                        }
                        fields["ISFIXEDBED"] = IsFixedBedCheckBox1.IsChecked;
                        //fields["ISASSIGNED"] = IsAssignedCheckBox.IsChecked;
                        DateTime dateTime = DateTime.Parse(AddTimeDate.Text);
                        fields["REGISITDATE"] = dateTime.ToString("yyyy-MM-dd");

                        fields["DESCRIPTION"] = DescriptionTextBox.Text;
                        //fields["PATIENT_ID"] = int.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId); 

                        var condition = new Dictionary<string, object>();
                        condition["ID"] = ((PatientInfo)PatientlistView.Items[i]).PatientId;
                        string Path = ConstDefinition.ImageDataDir + "\\" +
                                      ((PatientInfo)PatientlistView.Items[i]).PatientId.ToString("D8");
                        if (Directory.Exists(Path))
                        {
                            string PathRTF1 = ConstDefinition.ImageDataDir + "\\" +
                                              ((PatientInfo)PatientlistView.Items[i]).PatientId.ToString(
                                                  "D8") +
                                              "\\report.rtf";

                            File.Delete(PathRTF1);
                        }
                        patientDao.UpdatePatient(fields, condition);

                        MainWindow.Log.WriteInfoLog("Update the patient at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                                                 " who`s id is: " + ((PatientInfo)PatientlistView.Items[i]).PatientId +
                                                 ", name is: " + Add_NameTextBox.Text
                                                 + ", patientid is: " + AddPatientIDTextBox.Text
                                                 + ", age is: " + AddAgeTextBox.Text
                                                 + ", gender is: " + (string)SexComboBox.SelectedValue
                                                 + ", hand is: " + (string)InfectTypeComboBox.SelectedValue);

                        var newinformation = new PatientInfo
                        {
                            PatientName = Add_NameTextBox.Text,
                            PatientId = ((PatientInfo)PatientlistView.Items[i]).PatientId,
                            PatientGender = (string)SexComboBox1.SelectedValue,
                            PatientDob = AddAgeTextBox.Text,
                            PatientRegesiterDate = AddTimeDate.Text,
                            PatientDescription = DescriptionTextBox.Text,
                            PatientInfectType = (string)InfectTypeComboBox1.SelectedValue,
                            PatientTreatStatus = (string)StatusComboBox.SelectedValue,
                            PatientPatientId = AddPatientIDTextBox.Text,
                        };

                        PatientList[PatientlistView.SelectedIndex] = newinformation;
                        PatientlistView.SelectedIndex = i;
                        // 如果更新的和选定的是一样的，那要即时对选定的在页面中的表现进行更新 [Hu Tai, 2012-5-11]
                        //if (!String.IsNullOrEmpty(Basewindow._stimulusLeft._patient.ID.Text))
                        //{
                        //    if ((Int32.Parse(Basewindow._stimulusLeft._patient.ID.Text)) ==
                        //        (Int32.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId)))
                        //    {
                        //        Basewindow._stimulusLeft._patient.name.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientName;
                        //        Basewindow._stimulusLeft._patient.age.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientDob;
                        //        Basewindow.PatientId = uint.Parse(PatientList[PatientlistView.SelectedIndex].PatientId);
                        //        Basewindow._stimulusLeft._patient.ID.Text = Basewindow.PatientId.ToString("D8");
                        //        Basewindow._stimulusLeft._patient.PatientID.Text =
                        //         PatientList[PatientlistView.SelectedIndex].PatientPatientId;
                        //        Basewindow._stimulusLeft._patient.lishou.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientHand;
                        //        DescriptionTextBox.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientClinicDescription;
                        //        Basewindow._stimulusLeft._patient.sex.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientGender;
                        //        Basewindow._stimulusLeft._patient.description.Text =
                        //            PatientList[PatientlistView.SelectedIndex].PatientClinicDescription;

                        //        Basewindow.reportrtf.resetValue(ConstDefinition.ImageDataDir);
                        //    }
                        //}
                    }
                }
                NewOrEditFlag = 0;
                grid1.IsEnabled = false;

                NewButton.IsEnabled = true;
                CheckButton.IsEnabled = true;
                EditButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
                TopGrid.IsEnabled = true;
                PatientlistView.IsEnabled = true;
                foreach (FrameworkElement a in TopGrid.Children)
                {
                    a.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteErrorLog("Init.xaml.cs-ButtonNewSaveClick", ex);
                return;
            }
        }

        private void ButtonNewCancelClick(object sender, RoutedEventArgs e)
        {
            //查看时候不需要确认
            if (NewOrEditFlag == 3)
            {
                grid1.IsEnabled = true;
                AddSaveButton.Visibility = Visibility.Visible;

                InquireButton.IsEnabled = true;

                foreach (FrameworkElement a in TopGrid.Children)
                {
                    a.IsEnabled = true;
                }
                return;
            }


            var dialog = new RemindMessageBox2();
            dialog.textBlock1.Text = (string)FindResource("Message2");

            dialog.ShowDialog();
            if (dialog.remindflag == 2)
            {
                return;
            }

            grid1.IsEnabled = false;
            AddSaveButton.Visibility = Visibility.Visible;

            InquireButton.IsEnabled = true;

            if (NewOrEditFlag == 1)
            {
                int patientId = int.Parse(AddIDTextBox.Text);
                DeleteNotSavePatientData(patientId);
            }

            foreach (FrameworkElement a in TopGrid.Children)
            {
                a.IsEnabled = true;
            }
        }

        private void DeleteNotSavePatientData(int patientdId)
        {
            using (var patientDao = new PatientDao())
            {
                patientDao.DeletePatient(patientdId);
            }
        }

        private void Init_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
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
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

            try
            {
                PatientList.Clear();
                using (ComplexDao patientDao = new ComplexDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
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
                        PatientList.Add(patientInfo);
                    }
                }
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
                    InfectTypeComboBox1.Items.Clear();
                    foreach (InfectType type in list)
                    {
                        InfectTypeComboBox1.Items.Add(type.Name);
                    }
                    InfectTypeComboBox1.SelectedIndex = 0;
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

            this.SexComboBox1.Items.Clear();
            this.SexComboBox1.Items.Add("男");
            this.SexComboBox1.Items.Add("女");
            SexComboBox1.SelectedIndex = 0;
        }
    }

    public class PatientInfo : INotifyPropertyChanged
    {
        private Int64 _patientId;
        private string _patientName;
        private string _patientDob;
        private string _patientGender;
        private string _patientMobile;
        private string _patientRegesiterDate;
        private string _patientInfectType;
        private string _patientTreatStatus;
        private bool _patientIsFixedBed;
        private string _patientBedId;
        private bool _patientIsAssigned;
        private string _patientDescription;
        private string _patientPatientId;
     
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
        public string PatientMobile
        {
            get { return _patientMobile; }
            set
            {
                _patientMobile = value;
                OnPropertyChanged("PatientMobile");
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
