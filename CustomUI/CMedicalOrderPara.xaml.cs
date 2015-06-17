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
    /// Interaction logic for CInterval.xaml
    /// </summary>
    public partial class CMedicalOrderPara : UserControl
    {
        public ObservableCollection<MedicalOrderParaData> Datalist = new ObservableCollection<MedicalOrderParaData>();

        public CMedicalOrderPara()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
        }

        private void ListViewCInterval_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            //throw new NotImplementedException();
            try
            {
                using (var medicalOrderParaDao = new MedicalOrderParaDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = medicalOrderParaDao.SelectInterval(condition);
                    foreach (var type in list)
                    {
                        var medicalOrderParaData = new MedicalOrderParaData
                        {
                            Id = type.Id,
                            Name = type.Name,
                            Type=type.Type,
                            Count = type.Count,
                            Description = type.Description
                        };
                        Datalist.Add(medicalOrderParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CMedicalOrderPara.xaml.cs:ListViewCTreatType_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private void ListViewCInterval_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                ComboBoxType.Text = Datalist[ListView1.SelectedIndex].Type;
                CountTextBox.Text = Datalist[ListView1.SelectedIndex].Count.ToString();
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var medicalOrderParaDao = new MedicalOrderParaDao())
                {
                    var medicalOrderPara = new MedicalOrderPara();
                    medicalOrderPara.Name = this.NameTextBox.Text;
                    medicalOrderPara.Type = this.ComboBoxType.Text;
                    medicalOrderPara.Count = Int32.Parse(this.CountTextBox.Text);
                    medicalOrderPara.Description = this.DescriptionTextBox.Text;
                    int lastInsertId = -1;
                    medicalOrderParaDao.InsertInterval(medicalOrderPara, ref lastInsertId);
                    //UI
                    var medicalOrderParaData = new MedicalOrderParaData();
                    medicalOrderParaData.Id = medicalOrderPara.Id;
                    medicalOrderParaData.Name = medicalOrderPara.Name;
                    medicalOrderParaData.Type = medicalOrderPara.Type;
                    medicalOrderParaData.Count = medicalOrderPara.Count;
                    medicalOrderParaData.Description = medicalOrderPara.Description;
                    Datalist.Add(medicalOrderParaData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CMedicalOrderPara.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var medicalOrderParaDao = new MedicalOrderParaDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                    var fileds = new Dictionary<string, object>();
                    fileds["NAME"] = NameTextBox.Text;
                    fileds["TYPE"] = ComboBoxType.Text;
                    fileds["COUNT"] = Int32.Parse(CountTextBox.Text);
                    fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                    medicalOrderParaDao.UpdateInterval(fileds, condition);
                    RefreshData();
                }
            }
            catch (Exception ex )
            {

                MainWindow.Log.WriteInfoConsole("In CMedicalOrderPara.xaml.cs:UpdateButton_OnClick exception messsage: " + ex.Message);
            }
             
        }

          private void RefreshData()
        {
            try
            {
                using (var medicalOrderParaDao = new MedicalOrderParaDao())
                {
                    Datalist.Clear();

                    var condition = new Dictionary<string, object>();
                    var list = medicalOrderParaDao.SelectInterval(condition);
                    foreach (var pa in list)
                    {
                        var medicalOrderParaData = new MedicalOrderParaData();
                        medicalOrderParaData.Id = pa.Id;
                        medicalOrderParaData.Name = pa.Name;
                        medicalOrderParaData.Type = pa.Type;
                        medicalOrderParaData.Count = pa.Count;
                        medicalOrderParaData.Description = pa.Description;
                        Datalist.Add(medicalOrderParaData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CMedicalOrderPara.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
             using (var medicalOrderParaDao = new MedicalOrderParaDao())
            {
                medicalOrderParaDao.DeleteInterval(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
        }

        private void CMedicalOrderPara_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.ComboBoxType.Items.Clear();
            this.ComboBoxType.Items.Add("周");
            this.ComboBoxType.Items.Add("月");
        }

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                double num = 0;
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
            if (textBox.Text == "")
                textBox.Text = "0";
        }
    }

    public class MedicalOrderParaData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _type;
        private Int64 _count;
        private string _description;

        public MedicalOrderParaData()
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

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public Int64 Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
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
