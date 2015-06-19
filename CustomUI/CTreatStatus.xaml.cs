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

        public CTreatStatus()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
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
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
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

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
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
                using (var treatStatusDao = new TreatStatusDao())
                {
                    var treatStatus = new TreatStatus();
                    treatStatus.Name = this.NameTextBox.Text;
                    treatStatus.Description = this.DescriptionTextBox.Text;
                    int lastInsertId = -1;
                    treatStatusDao.InsertTreatStatus(treatStatus, ref lastInsertId);
                    //UI
                    var treatStatusData = new TreatStatusData();
                    treatStatusData.Id = treatStatus.Id;
                    treatStatusData.Name = treatStatus.Name;
                    treatStatusData.Description = treatStatus.Description;
                    Datalist.Add(treatStatusData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView1.SelectedIndex == -1) return;

            if (this.NameTextBox.Text.Equals(""))
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message1001"); ;
                a.ShowDialog();
                return;
            }
            //throw new NotImplementedException();
            using (var treatStatusDao = new TreatStatusDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                treatStatusDao.UpdateTreatStatus(fileds, condition);
                RefreshData();
            }
        }

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

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView1.SelectedIndex == -1) return;
            //throw new NotImplementedException();
            using (var treatStatusDao = new TreatStatusDao())
            {
                treatStatusDao.DeleteTreatStatus(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
        }

        private void CTreatStatus_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }

    public class TreatStatusData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _description;

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
