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
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    InfectType infectType = new InfectType();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    foreach (InfectType type in list)
                    {
                        InfectTypeData infectTypeData = new InfectTypeData();
                        infectTypeData.Name = type.Name;
                        infectTypeData.Description = type.Description;
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
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    InfectType infectType = new InfectType();
                    infectType.Name = this.NameTextBox.Text;
                    infectType.Description = this.DescriptionTextBox.Text;
                    int lastInsertId = -1;
                    infectTypeDao.InsertInfectType(infectType, ref lastInsertId);
                    //UI
                    InfectTypeData infectTypeData = new InfectTypeData();
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
          
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }

    public class InfectTypeData : INotifyPropertyChanged
    {
        private string _name;
        private string _description;

        public InfectTypeData()
        {
            Sequence = 0;
        }

        public int Sequence { get; set; }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("description");
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
