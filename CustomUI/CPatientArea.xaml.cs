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
    /// Interaction logic for PatientArea.xaml
    /// </summary>
    public partial class CPatientArea : UserControl
    {
        public ObservableCollection<PatientAreaData> Datalist = new ObservableCollection<PatientAreaData>();
        public ObservableCollection<InfectTypeData> Datalist1 = new ObservableCollection<InfectTypeData>();

        private bool isNew = false;
        private int currnetIndex = -1;

        public CPatientArea()
        {
            InitializeComponent();

            this.ListViewPatientArea.ItemsSource = Datalist;
           
        }

        private void ListViewCPatientArea_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        PatientAreaData patientAreaData = new PatientAreaData();
                        patientAreaData.Id = pa.Id;
                        patientAreaData.Name = pa.Name;
                        patientAreaData.Type = pa.Type;
                        patientAreaData.Position = pa.Position;
                        patientAreaData.Seq = pa.Seq;
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientAreaData.InfectionType = arealist[0].Name;
                                }
                            }
                        }
                        if (patientAreaData.Type.Equals("0"))
                        {
                            this.InfectionComboBox.IsEnabled = false;
                            this.RadioButton1.IsChecked = true;
                            patientAreaData.InfectionType = "阴性";
                        }

                        else if (patientAreaData.Type.Equals("1"))
                        {
                            this.InfectionComboBox.IsEnabled = true;
                            this.RadioButton2.IsChecked = true;
                        }
                        patientAreaData.Description = pa.Description;
                        Datalist.Add(patientAreaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
            InfectionComboBox.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    //Datalist1.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var type in list)
                    {
                        InfectionComboBox.Items.Add(type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }

        }

        private void RefreshData()
        {
            try
            {
                using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        PatientAreaData patientAreaData = new PatientAreaData();
                        patientAreaData.Id = pa.Id;
                        patientAreaData.Name = pa.Name;
                        patientAreaData.Type = pa.Type;
                        patientAreaData.Position = pa.Position;
                        patientAreaData.Seq = pa.Seq;
                       
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientAreaData.InfectionType = arealist[0].Name;
                                }
                            }
                        }
                        if (patientAreaData.Type.Equals("0"))
                        {
                            this.InfectionComboBox.IsEnabled = false;
                            this.RadioButton1.IsChecked = true;
                            patientAreaData.InfectionType = "阴性";
                        }

                        else if (patientAreaData.Type.Equals("1"))
                        {
                            this.InfectionComboBox.IsEnabled = true;
                            this.RadioButton2.IsChecked = true;
                        }
                        patientAreaData.Description = pa.Description;
                        Datalist.Add(patientAreaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }


        private void ListViewCPatientArea_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewPatientArea.SelectedIndex >= 0)
            {
                currnetIndex = ListViewPatientArea.SelectedIndex;
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;

                isNew = false;

                NameTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Name;
                InfectionComboBox.Text = Datalist[ListViewPatientArea.SelectedIndex].InfectionType;
                DescriptionTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Description;
                PositionTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Position;
                SeqTextBox.Text = Datalist[ListViewPatientArea.SelectedIndex].Seq.ToString();
                if (Datalist[ListViewPatientArea.SelectedIndex].Type.Equals("0"))
                {
                    this.InfectionComboBox.IsEnabled = false;
                    this.RadioButton1.IsChecked = true;
                }

                else if (Datalist[ListViewPatientArea.SelectedIndex].Type.Equals("1"))
                {
                    this.InfectionComboBox.IsEnabled = true;
                    this.RadioButton2.IsChecked = true;
                }
                //this.ButtonApply.IsEnabled = true;
                //this.ButtonCancel.IsEnabled = true;
            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new PatientAreaDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectPatientArea(condition);
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

        private void CPatientArea_OnLoaded(object sender, RoutedEventArgs e)
        {
            #region fill infecttype combox items
            this.InfectionComboBox.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var pa in list)
                    {
                        this.InfectionComboBox.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.InfectionComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:ListViewCPatientRoom_OnLoaded 2 exception messsage: " + ex.Message);
            }
            #endregion
        }


        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            isNew = true;
            NameTextBox.Text = "";
            InfectionComboBox.Text = "";
            DescriptionTextBox.Text = "";
            PositionTextBox.Text = "";
            SeqTextBox.Text = "";
            this.RadioButton1.IsChecked = true;

            this.ButtonNew.IsEnabled = false;
            this.ButtonDelete.IsEnabled = false;
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListViewPatientArea.SelectedIndex == -1) return;

            //throw new NotImplementedException();
            using (var patientAreaDao = new PatientAreaDao())
            {
                patientAreaDao.DeletePatientArea(Datalist[ListViewPatientArea.SelectedIndex].Id);
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

                    using (PatientAreaDao patientAreaDao = new PatientAreaDao())
                    {
                        PatientArea patientArea = new PatientArea();
                        patientArea.Name = this.NameTextBox.Text;
                        var condition = new Dictionary<string, object>();
                        using (var infectTypeDao = new InfectTypeDao())
                        {
                            condition.Clear();
                            condition["Name"] = InfectionComboBox.Text;
                            var arealist = infectTypeDao.SelectInfectType(condition);
                            if (arealist.Count == 1)
                            {
                                patientArea.InfectTypeId = arealist[0].Id;
                            }
                            else
                            {
                                patientArea.InfectTypeId = 1;
                            }
                        }
                        if ((bool)this.RadioButton1.IsChecked)
                            patientArea.Type = "0";
                        else if ((bool)this.RadioButton2.IsChecked)
                            patientArea.Type = "1";
                        patientArea.Description = this.DescriptionTextBox.Text;
                        patientArea.Position = this.PositionTextBox.Text;
                        patientArea.Seq = int.Parse(this.SeqTextBox.Text);
                        int lastInsertId = -1;
                        patientAreaDao.InsertPatientArea(patientArea, ref lastInsertId);
                        //UI
                        PatientAreaData patientAreaData = new PatientAreaData();
                        patientAreaData.Name = patientArea.Name;
                        if ((bool) this.RadioButton1.IsChecked)
                        {
                             patientAreaData.InfectionType = "阴性";
                             patientAreaData.Type = "0";
                        }
                        
                        else if((bool) this.RadioButton2.IsChecked)
                            patientAreaData.Type = "1";
                       
                        patientAreaData.Description = patientArea.Description;
                        patientAreaData.Position = patientArea.Position;
                        patientAreaData.Seq = patientArea.Seq;

                        Datalist.Add(patientAreaData);
                    }
                }
                catch (Exception ex)
                {
                    MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:ButtonApply_OnClick exception messsage: " + ex.Message);
                    return;
                }
                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = true;
                this.ButtonApply.IsEnabled = true;
                this.ButtonCancel.IsEnabled = true;
            }
            else
            {
                if (ListViewPatientArea.SelectedIndex == -1) return;

                if (this.NameTextBox.Text.Equals(""))
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1001"); ;
                    a.ShowDialog();
                    return;
                }

                //throw new NotImplementedException();
                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[ListViewPatientArea.SelectedIndex].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["NAME"] = NameTextBox.Text;
                    var condition2 = new Dictionary<string, object>();
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        condition2.Clear();
                        condition2["NAME"] = InfectionComboBox.Text;
                        var arealist = infectTypeDao.SelectInfectType(condition2);
                        if (arealist.Count == 1)
                        {
                            fileds["INFECTTYPEID"] = arealist[0].Id;
                        }
                        else
                        {
                            fileds["INFECTTYPEID"] = 1;
                        }
                    }
                    if ((bool)this.RadioButton1.IsChecked)
                        fileds["TYPE"] = "0";
                    else if ((bool)this.RadioButton2.IsChecked)
                        fileds["TYPE"] = "1";
                    fileds["SEQ"] = SeqTextBox.Text;
                    fileds["POSITION"] = PositionTextBox.Text;
                    fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                    patientAreaDao.UpdatePatientArea(fileds, condition);
                    int temp = this.ListViewPatientArea.SelectedIndex;
                    RefreshData();
                    this.ListViewPatientArea.SelectedIndex = temp;
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
                InfectionComboBox.Text = "";
                DescriptionTextBox.Text = "";
                PositionTextBox.Text = "";
                SeqTextBox.Text = "";
                this.RadioButton1.IsChecked = true;

                this.ButtonNew.IsEnabled = true;
                this.ButtonDelete.IsEnabled = false;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;
                this.ListViewPatientArea.SelectedIndex = -1;
                this.ListViewPatientArea.SelectedIndex = currnetIndex;
            }
            else
            {
                this.ListViewPatientArea.SelectedIndex = -1;
                this.ListViewPatientArea.SelectedIndex = currnetIndex;
            }

        }

        
        private void RadioButton2_OnChecked(object sender, RoutedEventArgs e)
        {
            if ((bool) RadioButton2.IsChecked)
            {
                this.InfectionComboBox.IsEnabled = true;
                this.InfectionComboBox.SelectedIndex = 0;
            }
            
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;

        }

        public int[] Paixiflag = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lb = (Label) sender;
            string bindingProperty = "";
            ListSortDirection sortDirection = ListSortDirection.Ascending;
            string strn = (string) (lb.Tag);
            if( strn == "0")
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
                bindingProperty = "InfectionType";
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
                bindingProperty = "Position";
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
            SortDescriptionCollection sdc = ListViewPatientArea.Items.SortDescriptions;
            if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }

            sdc.Add(new SortDescription(bindingProperty, sortDirection));
            var temp = new ObservableCollection<PatientAreaData>();
            for (int i = 0; i < ListViewPatientArea.Items.Count; i++)
            {
                temp.Add((PatientAreaData)ListViewPatientArea.Items[i]);
            }
            Datalist.Clear();
            Datalist = temp;
            ListViewPatientArea.ItemsSource = Datalist;
            sdc.Clear();
        }

        /*private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
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
                        bindingProperty = "Name";

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
                        bindingProperty = "InfectionType";

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
                    var temp = new ObservableCollection<BedPatientData>();
                    for (int i = 0; i < PatientlistView.Items.Count; i++)
                    {
                        temp.Add((BedPatientData)PatientlistView.Items[i]);
                    }
                    BedPatientList.Clear();
                    BedPatientList = temp;
                    PatientlistView.ItemsSource = BedPatientList;
                    sdc.Clear();
                }
            }
        }*/
    }

    public class PatientAreaData : INotifyPropertyChanged
    {
        private Int64 _id;
        private Int64 _seq;
        private string _name;
        private string _type;
        private string _position;
        private string _infecttype;
        private string _description;
        private string _infectionType;

        public PatientAreaData()
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

        public Int64 Seq
        {
            get { return _seq; }
            set
            {
                _seq = value;
                OnPropertyChanged("Seq");
            }
        }

        public string Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
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

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public string InfectionType
        {
            get { return _infectionType; }
            set
            {
                _infectionType = value;
                OnPropertyChanged("InfectionType");
            }
        }
        public string InfectType
        {
            get { return _infecttype; }
            set
            {
                _infecttype = value;
                OnPropertyChanged("InfectType");
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
