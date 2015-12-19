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
    /// Interaction logic for CPatientGroup.xaml
    /// </summary>
    public partial class CPatientGroup : UserControl
    {

        public ObservableCollection<PatientGroupData> Datalist = new ObservableCollection<PatientGroupData>();
        public ObservableCollection<PatientGroupParaData> DatalistPara = new ObservableCollection<PatientGroupParaData>();

        private bool isNew = false;
        private int currnetIndex = -1;

        private bool isNewPara = false;
        private int currnetIndexPara = -1;

        public MainWindow Basewindow;
        public CPatientGroup(MainWindow mainWindow)
        {
            InitializeComponent();
            Basewindow = mainWindow;
            this.ListViewPatientGroup.ItemsSource = Datalist;
            this.ListViewPatientGroupPara.ItemsSource = DatalistPara;
            //Datalist.CollectionChanged += Datalist_CollectionChanged;
            //DatalistPara.CollectionChanged += DatalistPara_CollectionChanged;
        }

        void Datalist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Datalist.Count == 0)
            {
                this.ButtonDelete.IsEnabled = false;
            }
            else
            {
                this.ButtonDelete.IsEnabled = true;
            }
        }

        void DatalistPara_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (DatalistPara.Count == 0)
            {
                ButtonParaNew.IsEnabled = false;
                ButtonDelete.IsEnabled = false;
                ButtonApply.IsEnabled = false;
                ButtonCancel.IsEnabled = false;
            }
            //throw new NotImplementedException();
        }

        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            isNew = true;

            PatientGroupData patientGroupData = new PatientGroupData();
            patientGroupData.Name = "";
            patientGroupData.Description = "";
            Datalist.Add(patientGroupData);

            this.ButtonNew.IsEnabled = false;
            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

            ParSettingGrid.IsEnabled = false;
            ListViewPatientGroupPara.IsEnabled = false;

            ListViewPatientGroup.SelectedIndex = Datalist.Count - 1;
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientGroup.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientGroupDao = new PatientGroupDao())
            {
                Basewindow.patientGroupPanel.ComboBoxPatientGroup.Items.Remove(Datalist[ListViewPatientGroup.SelectedIndex].Name);
                Basewindow.sheduleContent.PatientGroupComboBoxItems.Remove(Datalist[ListViewPatientGroup.SelectedIndex].Name);

                patientGroupDao.DeletePatientGroup(Datalist[ListViewPatientGroup.SelectedIndex].Id);
                RefreshData();
            }
            


            this.ButtonNew.IsEnabled = true;
            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;
            isNew = false;
        }

        private void RefreshData()
        {
            try
            {
                using (var patientGroupDao = new PatientGroupDao())
                {
                    Datalist.Clear();

                    var condition = new Dictionary<string, object>();
                    var list = patientGroupDao.SelectPatientGroup(condition);
                    foreach (var pa in list)
                    {
                        var patientGroupData = new PatientGroupData();
                        patientGroupData.Id = pa.Id;
                        patientGroupData.Name = pa.Name;
                        patientGroupData.Description = pa.Description;
                        Datalist.Add(patientGroupData);
                    }
                }
                Basewindow.patientGroupPanel.RefreshPatientGroupCombobox();
                Basewindow.sheduleContent.InitPatientGroupComboBox();

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:RefreshData exception messsage: " + ex.Message);
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var patientGroupDao = new PatientGroupDao())
            {
                var condition = new Dictionary<string, object>();
                var list = patientGroupDao.SelectPatientGroup(condition);
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


        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                //throw new NotImplementedException();
                try
                {
                    int index = ListViewPatientGroup.SelectedIndex;
                    if (index == -1) return;

                    if (Datalist[index].Name.Equals("") || !CheckNameIsExist(Datalist[index].Name))
                    {
                        var a = new RemindMessageBox1();
                        a.remindText.Text = (string)FindResource("Message1001"); ;
                        a.ShowDialog();
                        return;
                    }

                    using (PatientGroupDao patientGroupDao = new PatientGroupDao())
                    {
                        PatientGroup patientGroup = new PatientGroup();
                        patientGroup.Name = Datalist[index].Name;
                        patientGroup.Description = Datalist[index].Description;
                        int lastInsertId = -1;
                        patientGroupDao.InsertPatientGroup(patientGroup, ref lastInsertId);
                        //UI
                        //PatientGroupData patientGroupData = new PatientGroupData();
                        //patientGroupData.Id = lastInsertId;
                        //patientGroupData.Name = patientGroup.Name;
                        //patientGroupData.Description = patientGroup.Description;

                        //Datalist.Add(patientGroupData);
                        var patientData = new PatientData();
                        patientData.Id = lastInsertId;
                        patientData.Name = patientGroup.Name;
                        //Basewindow.patientGroupPanel.Datalist.Add(patientData);
                        
                        Basewindow.patientGroupPanel.ComboBoxPatientGroup.Items.Add(patientGroup.Name);
                        Basewindow.sheduleContent.PatientGroupComboBoxItems.Add(patientGroup.Name);

                        Datalist[index].Id = lastInsertId;
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:ButtonApply_OnClick exception messsage: " + ex.Message);
                    return;
                }
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = true;
                this.ButtonCancel.IsEnabled = true;
                isNew = false;
            }
            else
            {
                int index = ListViewPatientGroup.SelectedIndex;
                if (index == -1) return;

                if (this.Datalist[index].Name.Equals(""))
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1001"); ;
                    a.ShowDialog();
                    return;
                }

                //throw new NotImplementedException();
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[index].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["NAME"] = Datalist[index].Name;
                    fileds["DESCRIPTION"] = Datalist[index].Description;
                    patientGroupDao.UpdatePatientGroup(fileds, condition);
                    int temp = this.ListViewPatientGroup.SelectedIndex;
                    RefreshData();
                    this.ListViewPatientGroup.SelectedIndex = temp;
                }
               
            }

            this.ButtonDelete.IsEnabled = true;

            this.ButtonApply.IsEnabled = false;
            this.ButtonCancel.IsEnabled = false;

            ParSettingGrid.IsEnabled = true;
            ListViewPatientGroupPara.IsEnabled = true;
        }



        private void RefreshDataPara(int index)
        {
            try
            {
                using (var patientGroupParaDao = new PatientGroupParaDao())
                {
                    DatalistPara.Clear();

                    var condition = new Dictionary<string, object>();
                    condition["GROUPID"] = index;
                    var list = patientGroupParaDao.SelectPatientGroupPara(condition);
                    foreach (var pa in list)
                    {
                        var patientGroupParaData = new PatientGroupParaData();
                        patientGroupParaData.Id = pa.Id;
                        patientGroupParaData.GroupId = pa.GroupId;
                        patientGroupParaData.Left = pa.Left;
                        patientGroupParaData.Key = pa.Key;
                        patientGroupParaData.Symbol = pa.Symbol;
                        patientGroupParaData.Value = pa.Value;
                        patientGroupParaData.Right = pa.Right;
                        patientGroupParaData.Logic = pa.Logic;
                        patientGroupParaData.Description = pa.Description;
                        DatalistPara.Add(patientGroupParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:RefreshDataPara exception messsage: " + ex.Message);
            }
        }
        private UIElement GetListViewCellControl(int rowIndex, int cellIndex)
        {
            // rowIndex 和 cellIndex 基於 0.
            // 首先应得到 ListViewItem, 毋庸置疑, 所有可视UI 元素都继承了UIElement:
            UIElement u = ListViewPatientGroupPara.ItemContainerGenerator.ContainerFromIndex(rowIndex) as UIElement;
            if (u == null) return null;

            // 然后在 ListViewItem 元素树中搜寻 单元格:
            while ((u = (VisualTreeHelper.GetChild(u, 0) as UIElement)) != null)
                if (u is GridViewRowPresenter) 
                    return VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(u, cellIndex), 0) as UIElement;

            return u;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                RefreshData();

                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = false;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;
                this.ListViewPatientGroup.SelectedIndex = -1;
                this.ListViewPatientGroup.SelectedIndex = currnetIndex;
                isNew = false;
            }
            else
            {
                this.ListViewPatientGroup.SelectedIndex = -1;
                this.ListViewPatientGroup.SelectedIndex = currnetIndex;
            }

            ParSettingGrid.IsEnabled = true;
            ListViewPatientGroupPara.IsEnabled = true;
        }


        private void ButtonParaNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.ListViewPatientGroup.SelectedIndex == -1)
                return;

            isNewPara = true;

            PatientGroupParaData patientGroupParaData = new PatientGroupParaData();
            patientGroupParaData.Id = -1;
            patientGroupParaData.GroupId = Datalist[this.ListViewPatientGroup.SelectedIndex].Id;
            patientGroupParaData.Left = "";
            patientGroupParaData.Key = "";
            patientGroupParaData.Symbol = "";
            patientGroupParaData.Value = "";
            patientGroupParaData.Right = "";
            patientGroupParaData.Logic = "";
            patientGroupParaData.Description = "";
            DatalistPara.Add(patientGroupParaData);

            this.ButtonParaNew.IsEnabled = false;
            this.ButtonParaDelete.IsEnabled = false;
            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;

            GroupSettingGrid.IsEnabled = false;
            ListViewPatientGroup.IsEnabled = false;

            ParSettingGrid.IsEnabled = true;
            ListViewPatientGroupPara.IsEnabled = true;

        }

        private void ButtonParaDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientGroupPara.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientGroupParaDao = new PatientGroupParaDao())
            {
                patientGroupParaDao.DeletePatientGroupPara(DatalistPara[ListViewPatientGroupPara.SelectedIndex].Id);
                RefreshDataPara((int)Datalist[ListViewPatientGroup.SelectedIndex].Id);
            }

            this.ButtonParaNew.IsEnabled = true;
            this.ButtonParaDelete.IsEnabled = false;
            this.ButtonParaApply.IsEnabled = false;
            this.ButtonParaCancel.IsEnabled = false;
            isNewPara = false;
        }

        private void ButtonParaApply_OnClick(object sender, RoutedEventArgs e)
        {
            for (int index = 0; index < DatalistPara.Count; index++)
            {
                PatientGroupParaData patientGroupParaData = DatalistPara[index];
                if (patientGroupParaData.Id == -1)
                {
                    //throw new NotImplementedException();
                    try
                    {
                        //int index = ListViewPatientGroupPara.SelectedIndex;
                        //if (index == -1) return;

                        //if (DatalistPara[index].Name.Equals("") || !CheckNameIsExist(DatalistPara[index].Name))
                        //{
                        //    var a = new RemindMessageBox1();
                        //    a.remindText.Text = (string)FindResource("Message1001"); ;
                        //    a.ShowDialog();
                        //    return;
                        //}

                        using (PatientGroupParaDao patientGroupParaDao = new PatientGroupParaDao())
                        {
                            PatientGroupPara patientGroupPara = new PatientGroupPara();
                            patientGroupPara.GroupId = Datalist[ListViewPatientGroup.SelectedIndex].Id;
                            patientGroupPara.Left = patientGroupParaData.Left;
                            patientGroupPara.Key = patientGroupParaData.Key;
                            patientGroupPara.Symbol = patientGroupParaData.Symbol;
                            patientGroupPara.Value = patientGroupParaData.Value;
                            patientGroupPara.Right = patientGroupParaData.Right;
                            patientGroupPara.Logic = patientGroupParaData.Logic;
                            patientGroupPara.Description = patientGroupParaData.Description;
                            int lastInsertId = -1;
                            patientGroupParaDao.InsertPatientGroupPara(patientGroupPara, ref lastInsertId);
                            //UI
                            //PatientGroupData patientGroupData = new PatientGroupData();
                            //patientGroupData.Id = lastInsertId;
                            //patientGroupData.Name = patientGroup.Name;
                            //patientGroupData.Description = patientGroup.Description;

                            //Datalist.Add(patientGroupData);
                            patientGroupParaData.Id = lastInsertId;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.Log.WriteInfoConsole(
                            "In CPatientGroup.xaml.cs:ButtonParaApply_OnClick exception messsage: " + ex.Message);
                        return;
                    }
                    this.ButtonParaNew.IsEnabled = true;
                    this.ButtonParaDelete.IsEnabled = true;
                    this.ButtonParaApply.IsEnabled = true;
                    this.ButtonParaCancel.IsEnabled = true;
                    isNewPara = false;
                }
                else
                {
                    //int index = ListViewPatientGroupPara.SelectedIndex;
                    //if (index == -1)
                    //{
                    //    this.ButtonParaApply.IsEnabled = false;
                    //    ButtonParaCancel.IsEnabled = false;
                    //    return;
                    //}

                    //if (this.DatalisParat[index].Name.Equals(""))
                    //{
                    //    var a = new RemindMessageBox1();
                    //    a.remindText.Text = (string)FindResource("Message1001"); ;
                    //    a.ShowDialog();
                    //    return;
                    //}

                    //throw new NotImplementedException();
                    using (var patientGroupParaDao = new PatientGroupParaDao())
                    {
                        var condition = new Dictionary<string, object>();
                        condition["ID"] = patientGroupParaData.Id;

                        var fileds = new Dictionary<string, object>();
                        fileds["LEFT"] = patientGroupParaData.Left;
                        fileds["KEY"] = patientGroupParaData.Key;
                        fileds["SYMBOL"] = patientGroupParaData.Symbol;
                        fileds["VALUE"] = patientGroupParaData.Value;
                        fileds["RIGHT"] = patientGroupParaData.Right;
                        fileds["LOGIC"] = patientGroupParaData.Logic;
                        fileds["DESCRIPTION"] = patientGroupParaData.Description;
                        patientGroupParaDao.UpdatePatientGroupPara(fileds, condition);
                        //int temp = this.ListViewPatientGroupPara.SelectedIndex;
                        //RefreshDataPara((int) Datalist[ListViewPatientGroup.SelectedIndex].Id);
                        //this.ListViewPatientGroupPara.SelectedIndex = temp;
                    }
                }
            }


            this.ButtonParaDelete.IsEnabled = true;
            this.ButtonParaApply.IsEnabled = false;
            ButtonParaCancel.IsEnabled = false;

            GroupSettingGrid.IsEnabled = true;
            ListViewPatientGroup.IsEnabled = true;

            Basewindow.patientGroupPanel.RefreshData();
            Basewindow.reeportContent.InitPatientGroupComboBox();

        }

        private void ButtonParaCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientGroup.SelectedIndex == -1)
            {
                ButtonParaApply.IsEnabled = false;
                ButtonParaCancel.IsEnabled = false;

                GroupSettingGrid.IsEnabled = true;
                ListViewPatientGroup.IsEnabled = true;

                //ButtonParaNew.IsEnabled = true;
                return;
            }
            if (isNewPara)
            {
                RefreshDataPara((int)Datalist[ListViewPatientGroup.SelectedIndex].Id);

                this.ButtonParaNew.IsEnabled = true;
                this.ButtonParaDelete.IsEnabled = false;
                this.ButtonParaApply.IsEnabled = false;
                this.ButtonParaCancel.IsEnabled = false;
                this.ListViewPatientGroupPara.SelectedIndex = -1;
                this.ListViewPatientGroupPara.SelectedIndex = currnetIndexPara;
                isNewPara = false;
            }
            else
            {
                this.ListViewPatientGroupPara.SelectedIndex = -1;
                this.ListViewPatientGroupPara.SelectedIndex = currnetIndexPara;
            }
            ButtonParaApply.IsEnabled = false;
            ButtonParaCancel.IsEnabled = false;

            GroupSettingGrid.IsEnabled = true;
            ListViewPatientGroup.IsEnabled = true;
        }


        private void ListViewPatientGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currnetIndex = this.ListViewPatientGroup.SelectedIndex;
                if (currnetIndex == -1)
                {
                    this.ButtonDelete.IsEnabled = false;
                    this.ButtonParaNew.IsEnabled = false;
                }
                else
                {
                    this.ButtonDelete.IsEnabled = true;
                    this.ButtonParaNew.IsEnabled = true;
                }

                using (var patientGroupParaDao = new PatientGroupParaDao())
                {
                    DatalistPara.Clear();
                    var condition = new Dictionary<string, object>();
                    condition["GROUPID"] = Datalist[this.ListViewPatientGroup.SelectedIndex].Id;
                    var list = patientGroupParaDao.SelectPatientGroupPara(condition);
                    foreach (var type in list)
                    {
                        var patientGroupParaData = new PatientGroupParaData
                        {
                            Id = type.Id,
                            GroupId = type.GroupId,
                            Left = type.Left,
                            Key = type.Key,
                            Symbol = type.Symbol,
                            Value = type.Value,
                            Right = type.Right,
                            Logic = type.Logic,
                            Description = type.Description
                        };
                        DatalistPara.Add(patientGroupParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:ListViewPatientGroup_OnSelectionChanged exception messsage: " + ex.Message);
            }

        }

        private void ListViewPatientGroup_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var patientGroupDao = new PatientGroupDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = patientGroupDao.SelectPatientGroup(condition);
                    foreach (var type in list)
                    {
                        var patientGroupData = new PatientGroupData
                        {
                            Id = type.Id,
                            Name = type.Name,
                            Description = type.Description
                        };
                        Datalist.Add(patientGroupData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatient.xaml.cs:ListViewPatientGroup_OnLoaded exception messsage: " + ex.Message);
            }

        }

        private void ListViewPatientGroupPara_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currnetIndexPara = this.ListViewPatientGroupPara.SelectedIndex;
            if (currnetIndexPara!=-1)
                ButtonParaDelete.IsEnabled = true;
            else
            {
                ButtonParaDelete.IsEnabled = false;
            }
            ////throw new NotImplementedException();
            //try
            //{
            //    using (var patientGroupParaDao = new PatientGroupParaDao())
            //    {
            //        DatalistPara.Clear();
            //        var condition = new Dictionary<string, object>();
            //        var list = patientGroupParaDao.SelectPatientGroupPara(condition);
            //        foreach (var type in list)
            //        {
            //            var patientGroupParaData = new PatientGroupParaData
            //            {
            //                Id = type.Id,
            //                GroupId = type.GroupId,//TODO
            //                Left = type.Left,
            //                Key = type.Key,
            //                Symbol = type.Symbol,
            //                Value = type.Value,
            //                Right = type.Right,
            //                Logic = type.Logic,
            //                Description = type.Description
            //            };
            //            DatalistPara.Add(patientGroupParaData);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MainWindow.Log.WriteInfoConsole("In CPatient.xaml.cs:ListViewPatientGroupPara_OnSelectionChanged exception messsage: " + ex.Message);
            //}

        }

        private void ListViewPatientGroupPara_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.ListViewPatientGroup.SelectedIndex == -1)
                return;

            try
            {
                using (var patientGroupParaDao = new PatientGroupParaDao())
                {
                    DatalistPara.Clear();
                    var condition = new Dictionary<string, object>();
                    condition["GROUPID"] = Datalist[this.ListViewPatientGroup.SelectedIndex].Id;
                    var list = patientGroupParaDao.SelectPatientGroupPara(condition);
                    foreach (var type in list)
                    {
                        var patientGroupParaData = new PatientGroupParaData
                        {
                            Id = type.Id,
                            GroupId = type.GroupId,
                            Left = type.Left,
                            Key = type.Key,
                            Symbol = type.Symbol,
                            Value = type.Value,
                            Right = type.Right,
                            Logic = type.Logic,
                            Description = type.Description
                        };
                        DatalistPara.Add(patientGroupParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:ListViewPatientGroupPara_OnLoaded exception messsage: " + ex.Message);
            }

        }

        private void Para_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void TextBoxBase_OnTextChanged_Para(object sender, TextChangedEventArgs e)
        {

            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var aa = textBox.DataContext;
                ListViewPatientGroup.SelectedItem = aa;
            }
        }

        private void UIElement_OnGotFocus_Para(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var aa = textBox.DataContext;
                ListViewPatientGroupPara.SelectedItem = aa;
            }

            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var aa = comboBox.DataContext;
                ListViewPatientGroupPara.SelectedItem = aa;
            }
        }

        private void ComboBoxKey_OnInitialized(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            cb.Items.Add("姓名");
            cb.Items.Add("性别");
            cb.Items.Add("血型");
            cb.Items.Add("婚姻状况");
            cb.Items.Add("感染情况");
            cb.Items.Add("治疗状态");
            cb.Items.Add("固定床位");
            cb.Items.Add("所属分区");
        }

        private void ComboBoxKey_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedItem.Equals("姓名"))
            {
                //this.ListViewPatientGroupPara
            }
            else
            {
                
            }

            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;
        
        }


        private void ComboBoxSymbol_OnInitialized(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            cb.Items.Add("大于");
            cb.Items.Add("小于");
            cb.Items.Add("等于");
            cb.Items.Add("包含");
            cb.Items.Add("不包含");
        }

        private void ComboBoxSymbol_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListViewPatientGroupPara.SelectedIndex = 
            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;
        }

        private void ComboBoxValue_OnInitialized(object sender, EventArgs e)
        {

        }

        private void ComboBoxValue_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;

        }

        private void FrameworkElement_OnInitialized(object sender, EventArgs e)
        {

            ComboBox cb = (ComboBox)sender;
            cb.Items.Add("and");
            cb.Items.Add("or");
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            this.ButtonParaApply.IsEnabled = true;
            this.ButtonParaCancel.IsEnabled = true;

        }

        private void CPatientGroup_OnLoaded(object sender, RoutedEventArgs e)
        {
            

        }
    }

    public class PatientGroupParaData : INotifyPropertyChanged
    {
        private Int64 _id;
        private Int64 _groupId;
        private string _left;
        private string _key;
        private string _symbol;
        private string _value;
        private string _right;
        private string _logic;
        private string _description;
        private List<string> _details;
        private bool _isEditable;
        

        public PatientGroupParaData()
        {
            _details = new List<string>();

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

        public Int64 GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
            }
        }

        public bool IsEditabel
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                OnPropertyChanged("IsEditabel");
            }
        }

        public string Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged("Left");
            }
        }

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                FillValues(_key);
                OnPropertyChanged("Details");
                OnPropertyChanged("Key");
            }
        }


        private void FillValues( string key )
        {
            IsEditabel = false;
            using (var patientDao = new PatientDao())
            {
                var condition = new Dictionary<string, object>();
                List<Patient> patientslist = patientDao.SelectPatient(condition);
                switch (key)
                {
                    case "姓名":
                        IsEditabel = true;
                        /*_details.Clear();
                        foreach (var patient in patientslist)
                        {
                            _details.Add(patient.Name);
                        }*/
                        break;
                    case "性别":
                        _details.Clear();
                        _details.Add("男");
                        _details.Add("女");
                        break;
                    case "血型":
                        _details.Clear();
                        _details.Add("O");
                        _details.Add("A");
                        _details.Add("B");
                        _details.Add("AB");

                        break;
                    case "婚姻状况":
                        _details.Clear();
                        _details.Add("已婚");
                        _details.Add("未婚");
                        break;
                    case "感染情况":
                        _details.Clear();
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition.Clear();
                            var arealist = infectTypeDao.SelectInfectType(condition);
                            foreach (var infectType in arealist)
                            {
                                _details.Add(infectType.Name);
                            }
                            _details.Add("阴性");
                        }
                        break;
                    case "治疗状态":
                        _details.Clear();
                        using (TreatStatusDao treatStatusDao = new TreatStatusDao())
                        {
                            var condition1 = new Dictionary<string, object>();
                            var list1 = treatStatusDao.SelectTreatStatus(condition1);
                            foreach (var treatStatuse in list1)
                            {
                                _details.Add(treatStatuse.Name);
                            }
                            _details.Add("在治");
                        }

                        break;
                    case "固定床位":
                        _details.Clear();
                        _details.Add("FALSE");
                        _details.Add("TRUE");
                        break;
                    case "所属分区":
                        _details.Clear();
                        using (var patientAreaDao = new PatientAreaDao())
                        {
                            condition.Clear();
                            var list = patientAreaDao.SelectPatientArea(condition);
                            foreach (var type in list)
                            {
                                _details.Add(type.Name);
                            }
                        }

                        break;

                }

                
            }
        }

        public string Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                OnPropertyChanged("Symbol");
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public List<string> Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
            }
        }

        public string Right
        {
            get { return _right; }
            set
            {
                _right = value;
                OnPropertyChanged("Right");
            }
        }

        public string Logic
        {
            get { return _logic; }
            set
            {
                _logic = value;
                OnPropertyChanged("Logic");
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

    public class PatientGroupData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _description;

        public PatientGroupData()
        {
        }


        public long Id
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
