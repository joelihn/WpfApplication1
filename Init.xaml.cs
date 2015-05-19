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
        public int[] Paixiflag = new int[6] { 0, 0, 0, 0, 0, 0 };
        public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public int selectoperation;
        public Init(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            this.PatientlistView.ItemsSource = PatientList;
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
            SexComboBox.Text = "";
            InfectTypeComboBox.Text = "";
            DescriptionTextBox.Text = "";
            StatusComboBox.Text = "";
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

                AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId;

                Add_NameTextBox.IsEnabled = true;
                DescriptionTextBox.IsEnabled = true;
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

                int id = int.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId);

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
                    informatian.PatientId = patient.Id.ToString("D8");
                    informatian.PatientName = patient.Name;
                    informatian.PatientInfectTypeId = patient.InfectTypeId;
                    informatian.PatientTreatStatusId = patient.TreatStatusId;

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
                    AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId;
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

                DescriptionTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientDescription;
                if (PatientList[PatientlistView.SelectedIndex].PatientGender == (string)FindResource("ManText"))
                {
                    SexComboBox.SelectedIndex = 0;
                }
                else if (PatientList[PatientlistView.SelectedIndex].PatientGender == (string)FindResource("WomanText"))
                {
                    SexComboBox.SelectedIndex = 1;
                }
                else
                {
                    SexComboBox.SelectedIndex = -1;
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
                    informatian.PatientId = fmriPatient.Id.ToString("D8");
                    informatian.PatientPatientId = fmriPatient.PatientId;
                    informatian.PatientName = fmriPatient.Name;
                    informatian.PatientRegesiterDate = fmriPatient.RegisitDate;
                    informatian.PatientInfectTypeId = fmriPatient.InfectTypeId;
                    informatian.PatientTreatStatusId = fmriPatient.TreatStatusId;
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
                        ArrowPatientID.Visibility = Visibility.Visible;
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowID.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 1)
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
                        ArrowPatientID.Visibility = Visibility.Visible;
                        ArrowID.Visibility = Visibility.Visible;
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
                              ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowPatientID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowPatientID.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowPatientID.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 2)
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowName.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowName.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowName.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 3)
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowSex.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowSex.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowSex.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 4)
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowAge.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowAge.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowAge.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 5)
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
                        bindingProperty = "PatientHand";
                        ArrowInfectTypeId.Visibility = Visibility.Hidden;
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowInfectTypeId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowInfectTypeId.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowInfectTypeId.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 5)
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
                        bindingProperty = "PatientHand";
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowIsFixedBed.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowInfectTypeId.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowIsFixedBed.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 5)
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
                        bindingProperty = "PatientHand";
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowIsAssigned.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowInfectTypeId.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowIsAssigned.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                    }
                    else if (clickedColumn.Header is Grid && columnflag == 8)
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
                                ToString() == "pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowDate.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowDate.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowDown.png",
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
                AddIDTextBox.Text = PatientList[PatientlistView.SelectedIndex].PatientId;

                Add_NameTextBox.IsEnabled = false;
                DescriptionTextBox.IsEnabled = false;
                AddAgeTextBox.IsEnabled = false;
                SexComboBox.IsEnabled = false;
                InfectTypeComboBox.IsEnabled = false;
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
                        fields["GENDER"] = SexComboBox.SelectedValue;
                        fields["MOBILE"] = MobileTextBox.Text;
                        fields["INFECTTYPE"] = InfectTypeComboBox.SelectedValue;
                        fields["ISFIXEDBED"] = IsFixedBedCheckBox.IsChecked;
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
                            PatientId = AddIDTextBox.Text,
                            PatientGender = (string)SexComboBox.SelectedValue,
                            PatientDob = AddAgeTextBox.Text,
                            PatientRegesiterDate =
                                ((DateTime)AddTimeDate.SelectedDate).ToString("yyyy-MM-dd"),
                            PatientDescription = DescriptionTextBox.Text,
                            PatientInfectTypeId = (string)InfectTypeComboBox.SelectedValue,
                            PatientPatientId = AddPatientIDTextBox.Text,
                            PatientMobile = MobileTextBox.Text,
                           
                        };


                        PatientList.Insert(0, newinformation);
                        PatientlistView.SelectedIndex = 0;
                    }
                }
                else if (NewOrEditFlag == 2)
                {
                    if (!CheckPatientPatientIdValidity(AddPatientIDTextBox.Text))
                    {
                        var myRemindMessageBox = new RemindMessageBox1();
                        myRemindMessageBox.remindText.Text = (string)FindResource("Message76");
                        myRemindMessageBox.ShowDialog();
                        return;
                    }
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
                        fields["GENDER"] = SexComboBox.SelectedValue;
                        fields["INFECTTYPE"] = InfectTypeComboBox.SelectedValue;
                        fields["PATIENTID"] = AddPatientIDTextBox.Text;
                        fields["INFECTTYPE"] = InfectTypeComboBox.SelectedValue;
                        fields["ISFIXEDBED"] = IsFixedBedCheckBox.IsChecked;
                        fields["ISASSIGNED"] = IsAssignedCheckBox.IsChecked;
                        DateTime dateTime = DateTime.Parse(AddTimeDate.Text);
                        fields["REGISITDATE"] = dateTime.ToString("yyyy-MM-dd");

                        fields["DESCRIPTION"] = DescriptionTextBox.Text;
                        //fields["PATIENT_ID"] = int.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId); 

                        var condition = new Dictionary<string, object>();
                        condition["ID"] = int.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId);
                        string Path = ConstDefinition.ImageDataDir + "\\" +
                                      Int32.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId).ToString("D8");
                        if (Directory.Exists(Path))
                        {
                            string PathRTF1 = ConstDefinition.ImageDataDir + "\\" +
                                              Int32.Parse(((PatientInfo)PatientlistView.Items[i]).PatientId).ToString(
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
                            PatientGender = (string)SexComboBox.SelectedValue,
                            PatientDob = AddAgeTextBox.Text,
                            PatientRegesiterDate = AddTimeDate.Text,
                            PatientDescription = DescriptionTextBox.Text,
                            PatientInfectTypeId = (string)InfectTypeComboBox.SelectedValue,
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

            try
            {
                PatientList.Clear();
                using (PatientDao patientDao = new PatientDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientDao.SelectPatient(condition);
                    foreach (Patient type in list)
                    {
                        PatientInfo patientInfo = new PatientInfo();
                        patientInfo.PatientName = type.Name;
                        patientInfo.PatientDob = type.Dob;
                        patientInfo.PatientPatientId = type.PatientId;
                        patientInfo.PatientGender = type.Gender;
                        patientInfo.PatientMobile = type.Mobile;
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
                    InfectTypeComboBox.Items.Clear();
                    foreach (InfectType type in list)
                    {
                        InfectTypeComboBox.Items.Add(type.Name);
                    }
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
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded TreatStatus ComboxItem exception messsage: " + ex.Message);
            }

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
        }
    }

    public class PatientInfo : INotifyPropertyChanged
    {
        private string _patiendid;
        public string PatientName { get; set; }
        public string PatientDob { get; set; }
        public string PatientGender { get; set; }
        public string PatientMobile { get; set; }
        public string PatientRegesiterDate { get; set; }
        public Int64 PatientInfectTypeId { get; set; }
        public Int64 PatientTreatStatusId { get; set; }
        public bool PatientIsFixedBed { get; set; }
        public string PatientBedId { get; set; }
        public bool PatientIsAssigned { get; set; }
        public string PatientDescription { get; set; }
        public string PatientPatientId { get; set; }
        public string PatientId
        {
            get { return _patiendid; }
            set
            {
                _patiendid = value;
                OnPropertyChanged("PatientId");
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
