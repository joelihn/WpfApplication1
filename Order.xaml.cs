﻿using System;
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
        public int[] Paixiflag = new int[6] { 0, 0, 0, 0, 0, 0 };
        public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public Order(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
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
                        ArrowPatientID.Visibility = Visibility.Visible;
                        ArrowID.Visibility = Visibility.Visible;
                        ArrowName.Visibility = Visibility.Hidden;
                        ArrowAge.Visibility = Visibility.Hidden;
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Visible;
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
                            ArrowInfectType.Source =
                                new BitmapImage(
                                    new Uri("pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png",
                                            UriKind.RelativeOrAbsolute));
                        }
                        else if (ArrowInfectType.Source.ToString() ==
                                 "pack://application:,,,/fMRISystem;component/Resources/ArrowUp.png")
                        {
                            ArrowInfectType.Source =
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        else if (ArrowInfectType.Source.ToString() ==
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        else if (ArrowInfectType.Source.ToString() ==
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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
                        ArrowInfectType.Visibility = Visibility.Hidden;
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

    }
}
