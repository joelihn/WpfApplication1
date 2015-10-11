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
using WpfApplication1.DAOModule;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for CTreatStatus.xaml
    /// </summary>
    public partial class CTreatStatus : UserControl
    {
        public ObservableCollection<TreatStatusData> Datalist = new ObservableCollection<TreatStatusData>();

        private int currnetIndex = -1;
        public CTreatStatus()
        {
            InitializeComponent();
            this.ListViewTreatType.ItemsSource = Datalist;
        }

        private void ListViewCTreatStatus_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var treatStatusDao = new TreatStatusDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = treatStatusDao.SelectTreatStatus(condition);
                    foreach (var type in list)
                    {
                        var treatStatusData = new TreatStatusData
                        {
                            Id = type.Id,
                            Activated = type.Activated,
                            Name = type.Name,
                            Description = type.Description
                        };
                        Datalist.Add(treatStatusData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private void ListViewCTreatStatus_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewTreatType.SelectedIndex >= 0)
            {
                currnetIndex = ListViewTreatType.SelectedIndex;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;

                NameTextBox.Text = Datalist[ListViewTreatType.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListViewTreatType.SelectedIndex].Description;
                if (Datalist[ListViewTreatType.SelectedIndex].Activated)
                {
                    this.RadioButton1.IsChecked = true;
                }
                else
                {
                    this.RadioButton2.IsChecked = true;
                }
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new TreatStatusDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectTreatStatus(condition);
                foreach (var pa in list)
                {
                    if (name.Equals(pa.Name))
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        //private void AddButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    try
        //    {
        //        if (this.NameTextBox.Text.Equals("") || !CheckNameIsExist(this.NameTextBox.Text))
        //        {
        //            var a = new RemindMessageBox1();
        //            a.remindText.Text = (string)FindResource("Message1001"); ;
        //            a.ShowDialog();
        //            return;
        //        }
        //        using (var treatStatusDao = new TreatStatusDao())
        //        {
        //            var treatStatus = new TreatStatus();
        //            treatStatus.Name = this.NameTextBox.Text;
        //            treatStatus.Activated = (bool) this.CheckBox1.IsChecked;
        //            treatStatus.Description = this.DescriptionTextBox.Text;
        //            int lastInsertId = -1;
        //            treatStatusDao.InsertTreatStatus(treatStatus, ref lastInsertId);
        //            //UI
        //            var treatStatusData = new TreatStatusData();
        //            treatStatusData.Id = treatStatus.Id;
        //            treatStatusData.Name = treatStatus.Name;
        //            treatStatusData.Activated = treatStatus.Activated;
        //            treatStatusData.Description = treatStatus.Description;
        //            Datalist.Add(treatStatusData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
        //    }
        //}

        //private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (ListViewTreatType.SelectedIndex == -1) return;

        //    if (this.NameTextBox.Text.Equals(""))
        //    {
        //        var a = new RemindMessageBox1();
        //        a.remindText.Text = (string)FindResource("Message1001"); ;
        //        a.ShowDialog();
        //        return;
        //    }
        //    //throw new NotImplementedException();
        //    using (var treatStatusDao = new TreatStatusDao())
        //    {
        //        var condition = new Dictionary<string, object>();
        //        condition["ID"] = Datalist[ListViewTreatType.SelectedIndex].Id;

        //        var fileds = new Dictionary<string, object>();
        //        fileds["NAME"] = NameTextBox.Text;
        //        fileds["DESCRIPTION"] = DescriptionTextBox.Text;
        //        treatStatusDao.UpdateTreatStatus(fileds, condition);
        //        RefreshData();
        //    }
        //}

        private void RefreshData()
        {
            try
            {
                using (var treatStatusDao = new TreatStatusDao())
                {
                    Datalist.Clear();

                    var condition = new Dictionary<string, object>();
                    var list = treatStatusDao.SelectTreatStatus(condition);
                    foreach (var pa in list)
                    {
                        var treatStatusData = new TreatStatusData();
                        treatStatusData.Id = pa.Id;
                        treatStatusData.Name = pa.Name;
                        treatStatusData.Activated = pa.Activated;
                        treatStatusData.Description = pa.Description;
                        Datalist.Add(treatStatusData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }


        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {

            if (ListViewTreatType.SelectedIndex == -1) return;

            //if (this.NameTextBox.Text.Equals(""))
            //{
            //    var a = new RemindMessageBox1();
            //    a.remindText.Text = (string)FindResource("Message1001"); ;
            //    a.ShowDialog();
            //    return;
            //}
            //throw new NotImplementedException();
            using (var treatStatusDao = new TreatStatusDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListViewTreatType.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                if ((bool) (this.RadioButton1.IsChecked))
                {
                    fileds["ACTIVATED"] = true;
                }else if ((bool) (this.RadioButton2.IsChecked))
                {
                    fileds["ACTIVATED"] = false;
                }

                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                treatStatusDao.UpdateTreatStatus(fileds, condition);
                int temp = this.ListViewTreatType.SelectedIndex;
                RefreshData();
                this.ListViewTreatType.SelectedIndex = temp;
            }
            this.ButtonApply.IsEnabled = false;

        }
        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {

            {
                this.ListViewTreatType.SelectedIndex = -1;
                this.ListViewTreatType.SelectedIndex = currnetIndex;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;
            }
        }
        //private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (ListViewTreatType.SelectedIndex == -1) return;
        //    //throw new NotImplementedException();
        //    using (var treatStatusDao = new TreatStatusDao())
        //    {
        //        treatStatusDao.DeleteTreatStatus(Datalist[ListViewTreatType.SelecButtonApply_OnClicktedIndex].Id);
        //        RefreshData();
        //    }
        //}

        private void CTreatStatus_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
            if (RadioButton1.IsChecked == true)
            {
                NameTextBox.IsEnabled = true;
                DescriptionTextBox.IsEnabled = true;
            }
            if (RadioButton2.IsChecked == true)
            {
                NameTextBox.IsEnabled = false;
                DescriptionTextBox.IsEnabled = false;
            }

        }

        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lb = (Label)sender;
            string bindingProperty = "";
            ListSortDirection sortDirection = ListSortDirection.Ascending;
            string strn = (string)(lb.Tag);
            if (strn == "0")
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
                bindingProperty = "Id";
            }
            else if (strn == "1")
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
                bindingProperty = "Activated";
            }
            else if (strn == "2")
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
                bindingProperty = "Name";
            }
            else if (strn == "3")
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
                bindingProperty = "Description";
            }
            
            SortDescriptionCollection sdc = ListViewTreatType.Items.SortDescriptions;
            if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }

            sdc.Add(new SortDescription(bindingProperty, sortDirection));
            var temp = new ObservableCollection<TreatStatusData>();
            for (int i = 0; i < ListViewTreatType.Items.Count; i++)
            {
                temp.Add((TreatStatusData)ListViewTreatType.Items[i]);
            }
            Datalist.Clear();
            Datalist = temp;
            ListViewTreatType.ItemsSource = Datalist;
            sdc.Clear();
        }
    }

    public class TreatStatusData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _description;
        private bool _activated;

        public TreatStatusData()
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

        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                OnPropertyChanged("Activated");
            }
        }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
