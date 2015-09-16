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
        //public ObservableCollection<PatientInfo> PatientList = new ObservableCollection<PatientInfo>();
        public CollectionViewSource PatientListViewSource = new CollectionViewSource();
        public ObservableCollection<TreatOrder> TreatOrderList = new ObservableCollection<TreatOrder>();

        public ObservableCollection<TreatMethodData> TreatMentList = new ObservableCollection<TreatMethodData>();
        public ObservableCollection<string> OrderParaList = new ObservableCollection<string>();
        public Order(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            //this.PatientlistView.ItemsSource = PatientList;
            this.MedicalOrderListBox.ItemsSource = TreatOrderList;
         
        }


        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            //ListSortDirection sortDirection = ListSortDirection.Ascending;


            //if (e.OriginalSource is GridViewColumnHeader)
            //{
            //    //Get clicked column
            //    GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column; //得到单击的列
            //    int columnflag = 0;
            //    columnflag = ((GridView)PatientlistView.View).Columns.IndexOf(clickedColumn);


            //    if (clickedColumn != null)
            //    {
            //        //Get binding property of clicked column
            //        //string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path; //得到单击列所绑定的属性
            //        string bindingProperty = "";
            //        if (clickedColumn.Header is Grid & columnflag == 0)
            //        {
            //            if (Paixiflag[0] == 0)
            //            {
            //                Paixiflag[0] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[0] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientId";
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Visible;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;

            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowID.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowID.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowID.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 1)
            //        {
            //            if (Paixiflag[1] == 0)
            //            {
            //                Paixiflag[1] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[1] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientPatientId";
            //            ArrowPatientID.Visibility = Visibility.Visible;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            //TODO: 20130923
            //            if (
            //              ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                  ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowPatientID.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowPatientID.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowPatientID.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 2)
            //        {
            //            if (Paixiflag[2] == 0)
            //            {
            //                Paixiflag[2] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[2] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientName";
            //            ArrowName.Visibility = Visibility.Visible;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowName.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowName.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowName.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 3)
            //        {
            //            if (Paixiflag[3] == 0)
            //            {
            //                Paixiflag[3] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[3] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientGender";
            //            ArrowSex.Visibility = Visibility.Visible;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;


            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowSex.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowSex.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowSex.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 4)
            //        {
            //            if (Paixiflag[4] == 0)
            //            {
            //                Paixiflag[4] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[4] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientDob";
            //            ArrowAge.Visibility = Visibility.Visible;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowAge.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowAge.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowAge.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 5)
            //        {
            //            if (Paixiflag[5] == 0)
            //            {
            //                Paixiflag[5] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[5] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientInfectType";
            //            ArrowInfectTypeId.Visibility = Visibility.Visible;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowInfectTypeId.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowInfectTypeId.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowInfectTypeId.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 6)
            //        {
            //            if (Paixiflag[6] == 0)
            //            {
            //                Paixiflag[6] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[6] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientTreatStatus";
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Visible;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowTreatStatusId.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowTreatStatusId.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowTreatStatusId.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 7)
            //        {
            //            if (Paixiflag[7] == 0)
            //            {
            //                Paixiflag[7] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[7] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientIsFixedBed";
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Visible;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowIsFixedBed.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowIsFixedBed.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowIsFixedBed.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 8)
            //        {
            //            if (Paixiflag[8] == 0)
            //            {
            //                Paixiflag[8] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[8] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientIsAssigned";
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Visible;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;

            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowIsAssigned.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowIsAssigned.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowIsAssigned.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 9)
            //        {
            //            if (Paixiflag[9] == 0)
            //            {
            //                Paixiflag[9] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[9] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientRegesiterDate";
            //            ArrowDate.Visibility = Visibility.Visible;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;

            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowDate.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowDate.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowDate.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else if (clickedColumn.Header is Grid && columnflag == 10)
            //        {
            //            if (Paixiflag[10] == 0)
            //            {
            //                Paixiflag[10] = 1;
            //                sortDirection = ListSortDirection.Ascending;
            //            }
            //            else
            //            {
            //                Paixiflag[10] = 0;
            //                sortDirection = ListSortDirection.Descending;
            //            }
            //            bindingProperty = "PatientDescription";
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;

            //            if (
            //                ((Image)((StackPanel)((Grid)clickedColumn.Header).Children[0]).Children[1]).Source.
            //                    ToString() == "pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png")
            //            {
            //                ArrowDate.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //            else if (ArrowDate.Source.ToString() ==
            //                     "pack://application:,,,/WpfApplication1;component/Resources/ArrowUp.png")
            //            {
            //                ArrowDate.Source =
            //                    new BitmapImage(
            //                        new Uri("pack://application:,,,/WpfApplication1;component/Resources/ArrowDown.png",
            //                                UriKind.RelativeOrAbsolute));
            //            }
            //        }
            //        else
            //        {
            //            ArrowDate.Visibility = Visibility.Hidden;
            //            ArrowAge.Visibility = Visibility.Hidden;
            //            ArrowSex.Visibility = Visibility.Hidden;
            //            ArrowID.Visibility = Visibility.Hidden;
            //            ArrowName.Visibility = Visibility.Hidden;
            //            ArrowPatientID.Visibility = Visibility.Hidden;
            //            ArrowInfectTypeId.Visibility = Visibility.Hidden;
            //            ArrowTreatStatusId.Visibility = Visibility.Hidden;
            //            ArrowIsAssigned.Visibility = Visibility.Hidden;
            //            ArrowIsFixedBed.Visibility = Visibility.Hidden;
            //            return;
            //        }


            //        SortDescriptionCollection sdc = PatientlistView.Items.SortDescriptions;
            //        if (sdc.Count > 0)
            //        {
            //            SortDescription sd = sdc[0];
            //            sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
            //            //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
            //            sdc.Clear();
            //        }

            //        sdc.Add(new SortDescription(bindingProperty, sortDirection));
            //        var temp = new ObservableCollection<PatientInfo>();
            //        for (int i = 0; i < PatientlistView.Items.Count; i++)
            //        {
            //            temp.Add((PatientInfo)PatientlistView.Items[i]);
            //        }
            //        PatientList.Clear();
            //        PatientList = temp;
            //        PatientlistView.ItemsSource = PatientList;
            //        sdc.Clear();
            //    }
            //}
        }

        private void Order_OnLoaded(object sender, RoutedEventArgs e)
        {
            OrderParaList.Clear();
            using (MedicalOrderParaDao medicalOrderParaDao = new MedicalOrderParaDao())
            {
                Dictionary<string, object> condition = new Dictionary<string, object>();
                var list = medicalOrderParaDao.SelectInterval(condition);

                foreach (var medicalOrderPara in list)
                {
                   
                    OrderParaList.Add(medicalOrderPara.Name);
                }
            }

            //throw new NotImplementedException();
            try
            {
                if (Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex == -1)
                    return;
                TreatOrderList.Clear();
                using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PATIENTID"] =
                        Basewindow.patientGroupPanel.Datalist[Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex]
                            .Id;
                    var list = medicalOrderDao.SelectMedicalOrder(condition);
                    
                    foreach (MedicalOrder medicalOrder in list)
                    {
                        TreatOrder treatOrder = new TreatOrder();
                        treatOrder.Id = medicalOrder.Id;
                        treatOrder.Activated = medicalOrder.Activated;
                        treatOrder.Seq = medicalOrder.Seq;
                        treatOrder.Plan = medicalOrder.Plan;
                        
                        treatOrder.TreatTimes = (int)medicalOrder.Times;
                        treatOrder.Description = medicalOrder.Description;

                        if (medicalOrder.MethodId!=-1)
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

        }

        public void RefreshData()
        {
            try
            {
                if (Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex == -1)
                    return;
                TreatOrderList.Clear();
                using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
                {

                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["PATIENTID"] =
                        Basewindow.patientGroupPanel.Datalist[Basewindow.patientGroupPanel.ListBoxPatient.SelectedIndex]
                            .Id;
                    var list = medicalOrderDao.SelectMedicalOrder(condition);

                    foreach (MedicalOrder medicalOrder in list)
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
        }

        private void CbTreatMathod_Initialized(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            foreach (var v in TreatMentList)
            {
                if(v.IsAvailable == true)
                cb.Items.Add(v.Name);
            }

        }

        private void CbOrderPara_Initialized(object sender, EventArgs e)
        {
            
            ComboBox cb = (ComboBox)sender;
            cb.Items.Clear();
            foreach (var v in OrderParaList)
            {
                cb.Items.Add(v);
            }

        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
            if (textBox.Text == "")
                textBox.Text = "0";
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            using (MedicalOrderDao medicalOrderDao = new MedicalOrderDao())
            {
                foreach (var order in TreatOrderList)
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    condition["ID"] = order.Id;
                    Dictionary<string, object> fileds = new Dictionary<string, object>();
                    fileds["ACTIVATED"] = order.Activated;

                    using (MedicalOrderParaDao medicalOrderParaDao = new MedicalOrderParaDao())
                    {
                        Dictionary<string, object> condition2 = new Dictionary<string, object>();
                        condition2["NAME"] = order.Type;
                        var list = medicalOrderParaDao.SelectInterval(condition2);
                        if ((list != null) && (list.Count != 0)) fileds["INTERVAL"] = list[0].Id;

                    }
                    fileds["TIMES"] = order.TreatTimes;
                    fileds["DESCRIPTION"] = order.Description;
                    medicalOrderDao.UpdateMedicalOrder(fileds, condition);
                }
            }

            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;

        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.RefreshData();

        }
    }

    public class TreatOrder : INotifyPropertyChanged
    {
        private Int64 _id;
        private int _patientId;
        private bool _activated;
        private string _seq;
        private string _plan;
        private string _method;
        private string _interval;
        private int _times;
        private string _description;

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

        public int PatientId
        {
            get { return _patientId; }
            set
            {
                _patientId = value;
                OnPropertyChanged("PatientId");
            }
        }

        public string Seq
        {
            get { return _seq; }
            set
            {
                _seq = value;
                OnPropertyChanged("Seq");
            }
        }
        public string Plan
        {
            get { return _plan; }
            set
            {
                _plan = value;
                OnPropertyChanged("Plan");
            }
        }

        public string TreatMethod
        {
            get { return _method; }
            set
            {
                _method = value;
                OnPropertyChanged("TreatMethod");
            }
        }

        public string Type
        {
            get { return _interval; }
            set
            {
                _interval = value;
                OnPropertyChanged("Type");
            }
        }

        public int TreatTimes
        {
            get { return _times; }
            set
            {
                _times = value;
                OnPropertyChanged("TreatTimes");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                OnPropertyChanged("Activated");
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
