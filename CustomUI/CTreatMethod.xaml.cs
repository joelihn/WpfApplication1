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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.DAOModule;
using Label = System.Windows.Controls.Label;
using UserControl = System.Windows.Controls.UserControl;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for CTreatMethod.xaml
    /// </summary>
    public partial class CTreatMethod : UserControl
    {
        public ObservableCollection<TreatMethodData> Datalist = new ObservableCollection<TreatMethodData>();

        private int currnetIndex = -1;

        public CTreatMethod()
        {
            InitializeComponent();
            this.ListViewTreatMethod.ItemsSource = Datalist;
            RadioButton1.IsChecked = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void Buttonrectangle_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
            if (RadioButton2.IsChecked == true)
            {
                Buttonrectangle.IsEnabled = false;
                CheckBox1.IsEnabled = false;
                CheckBox2.IsEnabled = false;
            }
            if (RadioButton1.IsChecked == true)
            {
                Buttonrectangle.IsEnabled = true;
                CheckBox1.IsEnabled = true;
                CheckBox2.IsEnabled = true;
            }

        }

        //private void ListViewCTreatMethod_OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    #region refresh data list
        //    try
        //    {
        //        using (var methodDao = new TreatMethodDao())
        //        {
        //            Datalist.Clear();
        //            var condition = new Dictionary<string, object>();
        //            var list = methodDao.SelectTreatMethod(condition);
        //            foreach (var pa in list)
        //            {
        //                var treatMethodData = new TreatMethodData();
        //                treatMethodData.Id = pa.Id;
        //                treatMethodData.Name = pa.Name;
        //                {
        //                    using (var treatTypeDao = new TreatTypeDao())
        //                    {
        //                        condition.Clear();
        //                        condition["ID"] = pa.TreatTypeId;
        //                        var arealist = treatTypeDao.SelectTreatType(condition);
        //                        if (arealist.Count == 1)
        //                        {
        //                            treatMethodData.Type = arealist[0].Name;
        //                        }
        //                    }
        //                }
        //                string bgColor = pa.BgColor;
        //                if(bgColor != "" && bgColor != null)
        //                    treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
        //                else
        //                    treatMethodData.BgColor = Brushes.LightGray;
        //                treatMethodData.Description = pa.Description;
        //                treatMethodData.IsAvailable = pa.IsAvailable;
        //                Datalist.Add(treatMethodData);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
        //    }
        //    #endregion
        //}

        //private void ListViewCTreatMethod_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    if (ListView1.SelectedIndex >= 0)
        //    {
        //        NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
        //        ComboBoxTreatType.Text = Datalist[ListView1.SelectedIndex].Type;
        //        DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
        //        CheckBoxIsAvailable.IsChecked = Datalist[ListView1.SelectedIndex].IsAvailable;
        //        Buttonrectangle.Fill = Datalist[ListView1.SelectedIndex].BgColor;
        //    }
        //}

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new TreatMethodDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectTreatMethod(condition);
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
        //        using (var treatMethodDao = new TreatMethodDao())
        //        {
        //            var treatMethod = new TreatMethod();
        //            treatMethod.Name = this.NameTextBox.Text;

        //            var condition = new Dictionary<string, object>();
        //            using (var treatTypeDao = new TreatTypeDao())
        //            {
        //                condition.Clear();
        //                condition["Name"] = ComboBoxTreatType.Text;
        //                var arealist = treatTypeDao.SelectTreatType(condition);
        //                if (arealist.Count == 1)
        //                {
        //                    treatMethod.TreatTypeId = arealist[0].Id;
        //                }
        //            }
        //            treatMethod.Description = this.DescriptionTextBox.Text;
        //            treatMethod.BgColor = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();
        //            int lastInsertId = -1;
        //            treatMethodDao.InsertTreatMethod(treatMethod, ref lastInsertId);
        //            //UI
        //            TreatMethodData treatMethodData = new TreatMethodData();
        //            treatMethodData.Id = lastInsertId;
        //            treatMethodData.Name = treatMethod.Name;
        //            treatMethodData.Type = ComboBoxTreatType.Text;
        //            treatMethodData.Description = treatMethod.Description;
        //            treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(treatMethod.BgColor));
        //            treatMethodData.IsAvailable = treatMethod.IsAvailable;
        //            Datalist.Add(treatMethodData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
        //    }
        //}

        //private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (ListView1.SelectedIndex == -1) return;

        //    if (this.NameTextBox.Text.Equals("") )
        //    {
        //        var a = new RemindMessageBox1();
        //        a.remindText.Text = (string)FindResource("Message1001"); ;
        //        a.ShowDialog();
        //        return;
        //    }
        //    //throw new NotImplementedException();
        //    using (var treatMethodDao = new TreatMethodDao())
        //    {
        //        var condition = new Dictionary<string, object>();
        //        condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

        //        var fileds = new Dictionary<string, object>();
        //        fileds["NAME"] = NameTextBox.Text;

        //        var condition2 = new Dictionary<string, object>();
        //        using (var treatTypeDao = new TreatTypeDao())
        //        {
        //            condition2.Clear();
        //            condition2["Name"] = ComboBoxTreatType.Text;
        //            var arealist = treatTypeDao.SelectTreatType(condition2);
        //            if (arealist.Count == 1)
        //            {
        //                fileds["TREATTYPEID"] = arealist[0].Id;
        //            }
        //        }
        //        fileds["DESCRIPTION"] = DescriptionTextBox.Text;
        //        fileds["ISAVAILABLE"] = CheckBoxIsAvailable.IsChecked;
        //        fileds["BGCOLOR"] = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();

        //        var messageBox2 = new RemindMessageBox2();
        //        messageBox2.textBlock1.Text = "执行该操作将影响医嘱及排班";
        //        messageBox2.ShowDialog();
        //        if (messageBox2.remindflag == 1)
        //        {
        //            treatMethodDao.UpdateTreatMethod(fileds, condition);
        //            RefreshData();
        //        }

                
        //    }
        //}

        private void RefreshData()
        {
            try
            {
                using (var treatMethodDao = new TreatMethodDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = treatMethodDao.SelectTreatMethod(condition);
                    foreach (TreatMethod pa in list)
                    {
                        var treatMethodData = new TreatMethodData();
                        treatMethodData.Id = pa.Id;
                        treatMethodData.Name = pa.Name;
                        treatMethodData.SinglePump = pa.SinglePump;
                        treatMethodData.DoublePump = pa.DoublePump;
                        treatMethodData.IsAvailable = pa.IsAvailable;
                        treatMethodData.Description = pa.Description;
                        treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(pa.BgColor)); 
                        Datalist.Add(treatMethodData);
                    }
                }

                if (Datalist.Count != 0)
                    ListViewTreatMethod.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        //private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (ListView1.SelectedIndex == -1) return;
        //    //throw new NotImplementedException();
        //    using (var treatMethodDao = new TreatMethodDao())
        //    {
        //        treatMethodDao.DeleteTreatMethod(Datalist[ListView1.SelectedIndex].Id);
        //        RefreshData();
        //    }
        //}

        private void Button4rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dalog = new ColorDialog();
            if (dalog.ShowDialog() == DialogResult.OK)
            {

                this.ButtonApply.IsEnabled = true;
                this.ButtonCancel.IsEnabled = true;
                ((Rectangle)sender).Fill =
                    new SolidColorBrush(Color.FromRgb(dalog.Color.R, dalog.Color.G, dalog.Color.B));
            }
        }
        private void CTreatMethod_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            #region fill patientarea combox items
            //this.ComboBoxTreatType.Items.Clear();
            //try
            //{
            //    using (var treatTypeDao = new TreatTypeDao())
            //    {
            //        var condition = new Dictionary<string, object>();
            //        var list = treatTypeDao.SelectTreatType(condition);
            //        foreach (var pa in list)
            //        {
            //            this.ComboBoxTreatType.Items.Add(pa.Name);
            //        }
            //        if (list.Count > 0)
            //            this.ComboBoxTreatType.SelectedIndex = 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 1 exception messsage: " + ex.Message);
            //}
            #endregion

            if (Datalist.Count != 0)
                ListViewTreatMethod.SelectedIndex = 0;
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {

            if (ListViewTreatMethod.SelectedIndex == -1) return;

            if (this.NameTextBox.Text.Equals(""))
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message1001"); ;
                a.ShowDialog();
                return;
            }
            //throw new NotImplementedException();
            using (var treatMethodDao = new TreatMethodDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListViewTreatMethod.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                if ((bool) (this.CheckBox1.IsChecked))
                {
                    fileds["SINGLEPUMP"] = true;
                }
                else
                {
                    fileds["SINGLEPUMP"] = false;
                }
                if ((bool)(this.CheckBox2.IsChecked))
                {
                    fileds["DOUBLEPUMP"] = true;
                }
                else
                {
                    fileds["DOUBLEPUMP"] = false;
                }

                if ((bool) (this.RadioButton1.IsChecked))
                {
                    fileds["ISAVAILABLE"] = true;
                }else if ((bool) (this.RadioButton2.IsChecked))
                {
                    fileds["ISAVAILABLE"] = false;
                }

                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
               
                fileds["BGCOLOR"] = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();

                var messageBox2 = new RemindMessageBox2();
                messageBox2.textBlock1.Text = "执行该操作将影响医嘱及排班";
                messageBox2.ShowDialog();
                if (messageBox2.remindflag == 1)
                {
                    treatMethodDao.UpdateTreatMethod(fileds, condition);
                    int temp = this.ListViewTreatMethod.SelectedIndex;
                    RefreshData();
                    this.ListViewTreatMethod.SelectedIndex = temp;
                }


            }

            this.ButtonApply.IsEnabled = false;

        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.ListViewTreatMethod.SelectedIndex = -1;
            this.ListViewTreatMethod.SelectedIndex = currnetIndex;
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
            //else if (strn == "3")
            //{
            //    if (Paixiflag[3] == 0)
            //    {
            //        Paixiflag[3] = 1;
            //        sortDirection = ListSortDirection.Ascending;
            //    }
            //    else
            //    {
            //        Paixiflag[3] = 0;
            //        sortDirection = ListSortDirection.Descending;
            //    }
            //    bindingProperty = "BgColor";
            //}
            else if (strn == "4")
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
                bindingProperty = "SinglePump";
            }
            else if (strn == "5")
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
                bindingProperty = "DoublePump";
            }
            else if (strn == "6")
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
                bindingProperty = "Description";
            }
            SortDescriptionCollection sdc = ListViewTreatMethod.Items.SortDescriptions;
            if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }

            sdc.Add(new SortDescription(bindingProperty, sortDirection));
            var temp = new ObservableCollection<TreatMethodData>();
            for (int i = 0; i < ListViewTreatMethod.Items.Count; i++)
            {
                temp.Add((TreatMethodData)ListViewTreatMethod.Items[i]);
            }
            Datalist.Clear();
            Datalist = temp;
            ListViewTreatMethod.ItemsSource = Datalist;
            sdc.Clear();
        }

        private void ListViewTreatMethod_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (TreatMethodDao treatMethodDao = new TreatMethodDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = treatMethodDao.SelectTreatMethod(condition);
                    foreach (TreatMethod pa in list)
                    {
                        TreatMethodData treatMethodData = new TreatMethodData();
                        treatMethodData.Id = pa.Id;
                        treatMethodData.Name = pa.Name;
                        treatMethodData.SinglePump = pa.SinglePump;
                        treatMethodData.DoublePump = pa.DoublePump;
                        treatMethodData.IsAvailable = pa.IsAvailable;
                        string bgColor = pa.BgColor;

                        if (bgColor != "" && bgColor != null)
                        {
                            Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                            treatMethodData.BgColor = bgBrush;
                        }
                        else
                            treatMethodData.BgColor = Brushes.Gray;
                        treatMethodData.Description = pa.Description;
                        Datalist.Add(treatMethodData);
                    }
                }

                if (Datalist.Count != 0)
                    ListViewTreatMethod.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewTreatMethod_OnLoaded exception messsage: " + ex.Message);
            }

        }

        private void ListViewTreatMethod_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewTreatMethod.SelectedIndex >= 0)
            {
                currnetIndex = ListViewTreatMethod.SelectedIndex;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;


                NameTextBox.Text = Datalist[ListViewTreatMethod.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListViewTreatMethod.SelectedIndex].Description;
                if (Datalist[ListViewTreatMethod.SelectedIndex].IsAvailable)
                {
                    this.RadioButton1.IsChecked = true;
                }
                else 
                {
                    this.RadioButton2.IsChecked = true;
                }
                Buttonrectangle.Fill = Datalist[ListViewTreatMethod.SelectedIndex].BgColor;
                if (Datalist[ListViewTreatMethod.SelectedIndex].SinglePump)
                {
                    this.CheckBox1.IsChecked = true;
                }
                else
                {
                    this.CheckBox1.IsChecked = false;
                }
                if (Datalist[ListViewTreatMethod.SelectedIndex].DoublePump)
                {
                    this.CheckBox2.IsChecked = true;
                }
                else
                {
                    this.CheckBox2.IsChecked = false;
                }
                //this.ButtonApply.IsEnabled = true;
                //this.ButtonCancel.IsEnabled = true;
            }
        }


    }

    public class TreatMethodData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _type;//Need Delete
        private string _name;
        private bool _singlePump;
        private bool _doublePump;
        private string _description;
        private Brush _bgColor;
        private bool _isAvailable;

        public TreatMethodData()
        {
        }

        public Brush BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                OnPropertyChanged("RectColor");
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

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
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

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
            }
        }

        public bool SinglePump
        {
            get { return _singlePump; }
            set
            {
                _singlePump = value;
                OnPropertyChanged("SinglePump");
            }
        }

        public bool DoublePump
        {
            get { return _doublePump; }
            set
            {
                _doublePump = value;
                OnPropertyChanged("DoublePump");
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
