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
        public CPatientGroup()
        {
            InitializeComponent();
            this.ListViewPatientGroup.ItemsSource = Datalist;
            this.ListViewPatientGroupPara.ItemsSource = DatalistPara;
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

        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientGroup.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientGroupDao = new PatientGroupDao())
            {
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
        }



        private void RefreshDataPara()
        {
            try
            {
                using (var patientGroupParaDao = new PatientGroupParaDao())
                {
                    DatalistPara.Clear();

                    var condition = new Dictionary<string, object>();
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
        }


        private void ButtonParaNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.ListViewPatientGroup.SelectedIndex == -1)
                return;

            isNewPara = true;

            PatientGroupParaData patientGroupParaData = new PatientGroupParaData();
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
        }

        private void ButtonParaDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientGroupPara.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientGroupParaDao = new PatientGroupParaDao())
            {
                patientGroupParaDao.DeletePatientGroupPara(Datalist[ListViewPatientGroupPara.SelectedIndex].Id);
                RefreshDataPara();
            }

            this.ButtonParaNew.IsEnabled = true;
            this.ButtonParaDelete.IsEnabled = false;
            this.ButtonParaApply.IsEnabled = false;
            this.ButtonParaCancel.IsEnabled = false;
            isNewPara = false;
        }

        private void ButtonParaApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNewPara)
            {
                //throw new NotImplementedException();
                try
                {
                    int index = ListViewPatientGroupPara.SelectedIndex;
                    if (index == -1) return;

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
                        patientGroupPara.Left = DatalistPara[index].Left;
                        patientGroupPara.Key = DatalistPara[index].Key;
                        patientGroupPara.Symbol = DatalistPara[index].Symbol;
                        patientGroupPara.Value = DatalistPara[index].Value;
                        patientGroupPara.Right = DatalistPara[index].Right;
                        patientGroupPara.Logic = DatalistPara[index].Logic;
                        patientGroupPara.Description = DatalistPara[index].Description;
                        int lastInsertId = -1;
                        patientGroupParaDao.InsertPatientGroupPara(patientGroupPara, ref lastInsertId);
                        //UI
                        //PatientGroupData patientGroupData = new PatientGroupData();
                        //patientGroupData.Id = lastInsertId;
                        //patientGroupData.Name = patientGroup.Name;
                        //patientGroupData.Description = patientGroup.Description;

                        //Datalist.Add(patientGroupData);
                        DatalistPara[index].Id = lastInsertId;
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:ButtonParaApply_OnClick exception messsage: " + ex.Message);
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
                int index = ListViewPatientGroupPara.SelectedIndex;
                if (index == -1) return;

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
                    condition["ID"] = DatalistPara[index].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["LEFT"] = DatalistPara[index].Left;
                    fileds["KEY"] = DatalistPara[index].Key;
                    fileds["SYMBOL"] = DatalistPara[index].Symbol;
                    fileds["VALUE"] = DatalistPara[index].Value;
                    fileds["RIGHT"] = DatalistPara[index].Right;
                    fileds["LOGIC"] = DatalistPara[index].Logic;
                    fileds["DESCRIPTION"] = Datalist[index].Description;
                    patientGroupParaDao.UpdatePatientGroupPara(fileds, condition);
                    int temp = this.ListViewPatientGroupPara.SelectedIndex;
                    RefreshDataPara();
                    this.ListViewPatientGroupPara.SelectedIndex = temp;
                }

            }

            this.ButtonParaDelete.IsEnabled = true;
            this.ButtonParaApply.IsEnabled = false;
        }

        private void ButtonParaCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (isNewPara)
            {
                RefreshDataPara();

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
        }


        private void ListViewPatientGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currnetIndex = this.ListViewPatientGroup.SelectedIndex;
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
        

        public PatientGroupParaData()
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

        public Int64 GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
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
                OnPropertyChanged("Key");
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
