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
                MainWindow.Log.WriteInfoConsole("In CPatientGroup.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            //if (isNew)
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

            //        using (PatientAreaDao patientAreaDao = new PatientAreaDao())
            //        {
            //            PatientArea patientArea = new PatientArea();
            //            patientArea.Name = this.NameTextBox.Text;
            //            var condition = new Dictionary<string, object>();
            //            using (var infectTypeDao = new InfectTypeDao())
            //            {
            //                condition.Clear();
            //                condition["Name"] = InfectionComboBox.Text;
            //                var arealist = infectTypeDao.SelectInfectType(condition);
            //                if (arealist.Count == 1)
            //                {
            //                    patientArea.InfectTypeId = arealist[0].Id;
            //                }
            //                else
            //                {
            //                    patientArea.InfectTypeId = 1;
            //                }
            //            }
            //            if ((bool)this.RadioButton1.IsChecked)
            //                patientArea.Type = "0";
            //            else if ((bool)this.RadioButton2.IsChecked)
            //                patientArea.Type = "1";
            //            patientArea.Description = this.DescriptionTextBox.Text;
            //            patientArea.Position = this.PositionTextBox.Text;
            //            patientArea.Seq = int.Parse(this.SeqTextBox.Text);
            //            int lastInsertId = -1;
            //            patientAreaDao.InsertPatientArea(patientArea, ref lastInsertId);
            //            //UI
            //            PatientAreaData patientAreaData = new PatientAreaData();
            //            patientAreaData.Name = patientArea.Name;
            //            if ((bool)this.RadioButton1.IsChecked)
            //            {
            //                patientAreaData.InfectionType = "阴性";
            //                patientAreaData.Type = "0";
            //            }

            //            else if ((bool)this.RadioButton2.IsChecked)
            //                patientAreaData.Type = "1";

            //            patientAreaData.Description = patientArea.Description;
            //            patientAreaData.Position = patientArea.Position;
            //            patientAreaData.Seq = patientArea.Seq;

            //            Datalist.Add(patientAreaData);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:ButtonApply_OnClick exception messsage: " + ex.Message);
            //        return;
            //    }
            //    this.ButtonNew.IsEnabled = true;
            //    this.ButtonDelete.IsEnabled = true;
            //    this.ButtonApply.IsEnabled = true;
            //    this.ButtonCancel.IsEnabled = true;
            //}
            //else
            //{
            //    if (ListViewPatientArea.SelectedIndex == -1) return;

            //    if (this.NameTextBox.Text.Equals(""))
            //    {
            //        var a = new RemindMessageBox1();
            //        a.remindText.Text = (string)FindResource("Message1001"); ;
            //        a.ShowDialog();
            //        return;
            //    }

            //    //throw new NotImplementedException();
            //    using (var patientAreaDao = new PatientAreaDao())
            //    {
            //        var condition = new Dictionary<string, object>();
            //        condition["ID"] = Datalist[ListViewPatientArea.SelectedIndex].Id;

            //        var fileds = new Dictionary<string, object>();
            //        fileds["NAME"] = NameTextBox.Text;
            //        var condition2 = new Dictionary<string, object>();
            //        using (var infectTypeDao = new InfectTypeDao())
            //        {
            //            condition2.Clear();
            //            condition2["NAME"] = InfectionComboBox.Text;
            //            var arealist = infectTypeDao.SelectInfectType(condition2);
            //            if (arealist.Count == 1)
            //            {
            //                fileds["INFECTTYPEID"] = arealist[0].Id;
            //            }
            //            else
            //            {
            //                fileds["INFECTTYPEID"] = 1;
            //            }
            //        }
            //        if ((bool)this.RadioButton1.IsChecked)
            //            fileds["TYPE"] = "0";
            //        else if ((bool)this.RadioButton2.IsChecked)
            //            fileds["TYPE"] = "1";
            //        fileds["SEQ"] = SeqTextBox.Text;
            //        fileds["POSITION"] = PositionTextBox.Text;
            //        fileds["DESCRIPTION"] = DescriptionTextBox.Text;
            //        patientAreaDao.UpdatePatientArea(fileds, condition);
            //        int temp = this.ListViewPatientArea.SelectedIndex;
            //        RefreshData();
            //        this.ListViewPatientArea.SelectedIndex = temp;
            //    }
            //    isNew = false;
            //}
            //this.ButtonApply.IsEnabled = false;

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
        }

        private void ButtonParaDelete_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonParaApply_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonParaCancel_OnClick(object sender, RoutedEventArgs e)
        {

        }


        private void ListViewPatientGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isNew)
            {
                var messageBox2 = new RemindMessageBox2();
                messageBox2.textBlock1.Text = "是否保存当前行的修改？";
                messageBox2.ShowDialog();
                if (messageBox2.remindflag == 1)
                {

                }
                else
                {
                }
                isNew = false;
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
            //throw new NotImplementedException();
            try
            {
                using (var patientGroupParaDao = new PatientGroupParaDao())
                {
                    DatalistPara.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = patientGroupParaDao.SelectPatientGroupPara(condition);
                    foreach (var type in list)
                    {
                        var patientGroupParaData = new PatientGroupParaData
                        {
                            Id = type.Id,
                            GroupId = type.GroupId,//TODO
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
                MainWindow.Log.WriteInfoConsole("In CPatient.xaml.cs:ListViewPatientGroupPara_OnSelectionChanged exception messsage: " + ex.Message);
            }

        }

        private void ListViewPatientGroupPara_OnLoaded(object sender, RoutedEventArgs e)
        {
            

        }

        private void Para_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
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
