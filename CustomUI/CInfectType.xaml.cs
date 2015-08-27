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
    /// Interaction logic for CInfectType.xaml
    /// </summary>
    public partial class CInfectType : UserControl
    {
        public ObservableCollection<InfectTypeData> Datalist = new ObservableCollection<InfectTypeData>();

        private bool isNew = false;
        private int currnetIndex = -1;

        public CInfectType()
        {
            InitializeComponent();
            this.ListViewInfectType.ItemsSource = Datalist;
        }

        private void ListViewInfectType_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    Datalist.Clear();
                    var infectType = new InfectType();
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var type in list)
                    {
                        var infectTypeData = new InfectTypeData
                        {
                            Id = type.Id,
                            Name = type.Name,
                            Description = type.Description
                        };
                        Datalist.Add(infectTypeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private void ListViewInfectType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewInfectType.SelectedIndex >= 0)
            {
                currnetIndex = ListViewInfectType.SelectedIndex;
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;

                isNew = false;

                NameTextBox.Text = Datalist[ListViewInfectType.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListViewInfectType.SelectedIndex].Description;
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new InfectTypeDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectInfectType(condition);
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

        private void RefreshData()
        {
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    Datalist.Clear();
                    
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var pa in list)
                    {
                        var infectTypeData = new InfectTypeData();
                        infectTypeData.Id = pa.Id;
                        infectTypeData.Name = pa.Name;
                        infectTypeData.Description = pa.Description;
                        Datalist.Add(infectTypeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            isNew = true;
            NameTextBox.Text = "";
            DescriptionTextBox.Text = "";

            this.ButtonNew.IsEnabled = false;
            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewInfectType.SelectedIndex == -1) return;
            //throw new NotImplementedException();
            using (var infectTypeDao = new InfectTypeDao())
            {
                infectTypeDao.DeleteInfectType(Datalist[ListViewInfectType.SelectedIndex].Id);
                RefreshData();
            }

            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
            isNew = false;
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                //throw new NotImplementedException();
                try
                {
                    if (this.NameTextBox.Text.Equals("") || !CheckNameIsExist(this.NameTextBox.Text))
                    {
                        var a = new RemindMessageBox1();
                        a.remindText.Text = (string)FindResource("Message1001"); ;
                        a.ShowDialog();
                        return;
                    }
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        var infectType = new InfectType();
                        infectType.Name = this.NameTextBox.Text;
                        infectType.Description = this.DescriptionTextBox.Text;
                        int lastInsertId = -1;
                        infectTypeDao.InsertInfectType(infectType, ref lastInsertId);
                        //UI
                        var infectTypeData = new InfectTypeData();
                        infectTypeData.Id = infectType.Id;
                        infectTypeData.Name = infectType.Name;
                        infectTypeData.Description = infectType.Description;
                        Datalist.Add(infectTypeData);
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
                }
            }
            else
            {
                if (ListViewInfectType.SelectedIndex == -1) return;

                if (this.NameTextBox.Text.Equals(""))
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1001"); ;
                    a.ShowDialog();
                    return;
                }

                //throw new NotImplementedException();
                using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[ListViewInfectType.SelectedIndex].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["NAME"] = NameTextBox.Text;
                    fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                    infectTypeDao.UpdateInfectType(fileds, condition);
                    int temp = this.ListViewInfectType.SelectedIndex;
                    RefreshData();
                    this.ListViewInfectType.SelectedIndex = temp;
                }
                isNew = false;
            }
            this.ButtonApply.IsEnabled = false;

        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                NameTextBox.Text = "";
                DescriptionTextBox.Text = "";

                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = false;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;
                this.ListViewInfectType.SelectedIndex = -1;
                this.ListViewInfectType.SelectedIndex = currnetIndex;
            }
            else
            {
                this.ListViewInfectType.SelectedIndex = -1;
                this.ListViewInfectType.SelectedIndex = currnetIndex;
            }

        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

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
                bindingProperty = "Name";
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
                bindingProperty = "Description";
            }
            
            SortDescriptionCollection sdc = ListViewInfectType.Items.SortDescriptions;
            if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }

            sdc.Add(new SortDescription(bindingProperty, sortDirection));
            var temp = new ObservableCollection<InfectTypeData>();
            for (int i = 0; i < ListViewInfectType.Items.Count; i++)
            {
                temp.Add((InfectTypeData)ListViewInfectType.Items[i]);
            }
            Datalist.Clear();
            Datalist = temp;
            ListViewInfectType.ItemsSource = Datalist;
            sdc.Clear();
        }
    }

    public class InfectTypeData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _description;

        public InfectTypeData()
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
