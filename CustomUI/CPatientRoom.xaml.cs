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
    /// Interaction logic for PatientRoom.xaml
    /// </summary>
    public partial class CPatientRoom : UserControl
    {
        public ObservableCollection<PatientRoomData> Datalist = new ObservableCollection<PatientRoomData>();


        public CPatientRoom()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
        }
        private void ListViewCPatientRoom_OnLoaded(object sender, RoutedEventArgs e)
        {
            #region refresh data list
            try
            {
                using (var patientRoomDao = new PatientRoomDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = patientRoomDao.SelectPatientRoom(condition);
                    foreach (var pa in list)
                    {
                        var patientRoomData = new PatientRoomData();
                        patientRoomData.Id = pa.Id;
                        patientRoomData.Name = pa.Name;
                        {
                            using (var patientAreaDao = new PatientAreaDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientAreaId;
                                var arealist = patientAreaDao.SelectPatientArea(condition);
                                if (arealist.Count == 1)
                                {
                                    patientRoomData.PatientArea = arealist[0].Name;
                                }
                            }
                        }
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientRoomData.InfectType = arealist[0].Name;
                                }
                            }
                        }
                        patientRoomData.Description = pa.Description;
                        Datalist.Add(patientRoomData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
            #endregion
        }

        private void ListViewCPatientRoom_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                ComboBoxPatientArea.Text = Datalist[ListView1.SelectedIndex].PatientArea;
                ComboBoxInfectType.Text = Datalist[ListView1.SelectedIndex].InfectType;
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var patientRoomDao = new PatientRoomDao())
                {
                    var patientRoom = new PatientRoom();
                    patientRoom.Name = this.NameTextBox.Text;

                    var condition = new Dictionary<string, object>();
                    using (var patientAreaDao = new PatientAreaDao())
                    {
                        condition.Clear();
                        condition["Name"] = ComboBoxPatientArea.Text;
                        var arealist = patientAreaDao.SelectPatientArea(condition);
                        if (arealist.Count == 1)
                        {
                            patientRoom.PatientAreaId = arealist[0].Id;
                        }
                    }
                    using (var infectTypeDao = new InfectTypeDao())
                    {
                        condition.Clear();
                        condition["Name"] = ComboBoxInfectType.Text;
                        var arealist = infectTypeDao.SelectInfectType(condition);
                        if (arealist.Count == 1)
                        {
                            patientRoom.InfectTypeId = arealist[0].Id;
                        }
                    }
                    patientRoom.Description = this.DescriptionTextBox.Text;
                    int lastInsertId = -1;
                    patientRoomDao.InsertPatientRoom(patientRoom, ref lastInsertId);
                    //UI
                    PatientRoomData patientRoomData = new PatientRoomData();
                    patientRoomData.Id = patientRoom.Id;
                    patientRoomData.Name = patientRoom.Name;
                    patientRoomData.PatientArea = ComboBoxPatientArea.Text;
                    patientRoomData.InfectType = ComboBoxInfectType.Text;
                    patientRoomData.Description = patientRoom.Description;
                    Datalist.Add(patientRoomData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var patientRoomDao = new PatientRoomDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;

                var condition2 = new Dictionary<string, object>();
                using (var patientAreaDao = new PatientAreaDao())
                {
                    condition2.Clear();
                    condition2["Name"] = ComboBoxPatientArea.Text;
                    var arealist = patientAreaDao.SelectPatientArea(condition2);
                    if (arealist.Count == 1)
                    {
                        fileds["PATIENTAREAID"] = arealist[0].Id;
                    }
                }
                using (var infectTypeDao = new InfectTypeDao())
                {
                    condition2.Clear();
                    condition2["Name"] = ComboBoxInfectType.Text;
                    var arealist = infectTypeDao.SelectInfectType(condition2);
                    if (arealist.Count == 1)
                    {
                        fileds["INFECTTYPEID"] = arealist[0].Id;
                    }
                }

                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                patientRoomDao.UpdatePatientRoom(fileds, condition);
                RefreshData();
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var patientRoomDao = new PatientRoomDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = patientRoomDao.SelectPatientRoom(condition);
                    foreach (PatientRoom pa in list)
                    {
                        var patientRoomData = new PatientRoomData();
                        patientRoomData.Id = pa.Id;
                        patientRoomData.Name = pa.Name;
                        {
                            using (var patientAreaDao = new PatientAreaDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.PatientAreaId;
                                var arealist = patientAreaDao.SelectPatientArea(condition);
                                if (arealist.Count == 1)
                                {
                                    patientRoomData.PatientArea = arealist[0].Name;
                                }
                            }
                        }
                        {
                            using (var infectTypeDao = new InfectTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.InfectTypeId;
                                var arealist = infectTypeDao.SelectInfectType(condition);
                                if (arealist.Count == 1)
                                {
                                    patientRoomData.InfectType = arealist[0].Name;
                                }
                            }
                        }
                        patientRoomData.Description = pa.Description;
                        Datalist.Add(patientRoomData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientArea.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }


        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var patientRoomDao = new PatientRoomDao())
            {
                patientRoomDao.DeletePatientRoom(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
        }

        private void CPatientRoom_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //throw new NotImplementedException();
            #region fill patientarea combox items
            this.ComboBoxPatientArea.Items.Clear();
            try
            {
                using (var patientAreaDao = new PatientAreaDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = patientAreaDao.SelectPatientArea(condition);
                    foreach (PatientArea pa in list)
                    {
                        this.ComboBoxPatientArea.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.ComboBoxPatientArea.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:ListViewCPatientRoom_OnLoaded 1 exception messsage: " + ex.Message);
            }
            #endregion

            #region fill infecttype combox items
            this.ComboBoxInfectType.Items.Clear();
            try
            {
                using (var infectTypeDao = new InfectTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (var pa in list)
                    {
                        this.ComboBoxInfectType.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.ComboBoxInfectType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CPatientRoom.xaml.cs:ListViewCPatientRoom_OnLoaded 2 exception messsage: " + ex.Message);
            }
            #endregion
        }
    }

    public class PatientRoomData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _patientArea;
        private string _infecttype;
        private string _description;

        public PatientRoomData()
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

        public string PatientArea
        {
            get { return _patientArea; }
            set
            {
                _patientArea = value;
                OnPropertyChanged("PatientArea");
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
