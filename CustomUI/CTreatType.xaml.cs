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
using UserControl = System.Windows.Controls.UserControl;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for CTreatType.xaml
    /// </summary>
    public partial class CTreatType : UserControl
    {

        public ObservableCollection<TreatTypeData> Datalist = new ObservableCollection<TreatTypeData>();

        public CTreatType()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
        }

        private void ListViewCTreatType_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var treatTypeDao = new TreatTypeDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = treatTypeDao.SelectTreatType(condition);
                    foreach (var type in list)
                    {
                        var treatTypeData = new TreatTypeData();
                        treatTypeData.Id = type.Id;
                        treatTypeData.Name = type.Name;
                        treatTypeData.Description = type.Description;

                      

                        string bgColor = type.BgColor;

                        if (bgColor != "" && bgColor != null)
                        {
                            Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                            treatTypeData.BgColor = bgBrush;
                        }
                            
                        else
                            treatTypeData.BgColor = Brushes.Gray;

                        Datalist.Add(treatTypeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatType.xaml.cs:ListViewCTreatType_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private void ListViewCTreatType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
                Buttonrectangle.Fill = Datalist[ListView1.SelectedIndex].BgColor;

            }
        }

        private bool CheckNameIsExist(string name)
        {
            using (var bedDao = new TreatTypeDao())
            {
                var condition = new Dictionary<string, object>();
                var list = bedDao.SelectTreatType(condition);
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

                using (var treatTypeDao = new TreatTypeDao())
                {
                    var treatType = new TreatType();
                    treatType.Name = this.NameTextBox.Text;
                    treatType.Description = this.DescriptionTextBox.Text;
                    treatType.BgColor = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();
                    int lastInsertId = -1;
                    treatTypeDao.InsertTreatType(treatType, ref lastInsertId);
                    //UI
                    var treatTypeData = new TreatTypeData();
                    treatTypeData.Id = treatType.Id;
                    treatTypeData.Name = treatType.Name;
                    treatTypeData.Description = treatType.Description;

                    string bgColor = treatType.BgColor;
                    Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                    if (bgColor != "" && bgColor != null)
                        treatTypeData.BgColor = bgBrush;
                    else
                        treatTypeData.BgColor = Brushes.Gray;

                    Datalist.Add(treatTypeData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView1.SelectedIndex == -1) return;

            if (this.NameTextBox.Text.Equals("") )
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = (string)FindResource("Message1001"); ;
                a.ShowDialog();
                return;
            }

            //throw new NotImplementedException();
            using (var treatTypeDao = new TreatTypeDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;
                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                fileds["BGCOLOR"] = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();
                treatTypeDao.UpdateTreatType(fileds, condition);
                RefreshData();
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var treatTypeDao = new TreatTypeDao())
                {
                    Datalist.Clear();

                    var condition = new Dictionary<string, object>();
                    var list = treatTypeDao.SelectTreatType(condition);
                    foreach (var pa in list)
                    {
                        var treatTypeData = new TreatTypeData();
                        treatTypeData.Id = pa.Id;
                        treatTypeData.Name = pa.Name;
                        treatTypeData.Description = pa.Description;

                        string bgColor = pa.BgColor;

                        if (bgColor != "" && bgColor != null)
                        {
                            Brush bgBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                            treatTypeData.BgColor = bgBrush;
                        }
                            
                        else
                            treatTypeData.BgColor = Brushes.Gray;
                        Datalist.Add(treatTypeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatType.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListView1.SelectedIndex == -1) return;
            //throw new NotImplementedException();
            using (var treatTypeDao = new TreatTypeDao())
            {
                if (Datalist[ListView1.SelectedIndex].Id == 1)
                {
                    var a = new RemindMessageBox1();
                    a.remindText.Text = (string)FindResource("Message1002");
                    a.ShowDialog();
                    return;
                }
                treatTypeDao.DeleteTreatType(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
        }

        private void CTreatType_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void Button4rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dalog = new ColorDialog();
            if (dalog.ShowDialog() == DialogResult.OK)
            {
                ((Rectangle)sender).Fill =
                    new SolidColorBrush(Color.FromRgb(dalog.Color.R, dalog.Color.G, dalog.Color.B));
            }
        }
    }

    public class TreatTypeData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _description;
        private Brush _bgColor;
        public TreatTypeData()
        {
        }

        public Brush BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                OnPropertyChanged("BgColor");
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
