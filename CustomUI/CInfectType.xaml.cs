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

        public CInfectType()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
        }

        private void ListViewCInfectType_OnLoaded(object sender, RoutedEventArgs e)
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
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void ListViewCInfectType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
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

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var infectTypeDao = new InfectTypeDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                infectTypeDao.UpdateInfectType(fileds, condition);
                RefreshData();
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

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var infectTypeDao = new InfectTypeDao())
            {
                infectTypeDao.DeleteInfectType(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
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
