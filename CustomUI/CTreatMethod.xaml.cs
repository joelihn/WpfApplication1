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
    /// Interaction logic for CTreatMethod.xaml
    /// </summary>
    public partial class CTreatMethod : UserControl
    {
        public ObservableCollection<TreatMethodData> Datalist = new ObservableCollection<TreatMethodData>();


        public CTreatMethod()
        {
            InitializeComponent();
            this.ListView1.ItemsSource = Datalist;
        }

        private void ListViewCTreatMethod_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            #region refresh data list
            try
            {
                using (var methodDao = new TreatMethodDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = methodDao.SelectTreatMethod(condition);
                    foreach (var pa in list)
                    {
                        var treatMethodData = new TreatMethodData();
                        treatMethodData.Id = pa.Id;
                        treatMethodData.Name = pa.Name;
                        {
                            using (var treatTypeDao = new TreatTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.TreatTypeId;
                                var arealist = treatTypeDao.SelectTreatType(condition);
                                if (arealist.Count == 1)
                                {
                                    treatMethodData.Type = arealist[0].Name;
                                }
                            }
                        }
                        string bgColor = pa.BgColor;
                        if(bgColor != "" && bgColor != null)
                            treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor));
                        else
                            treatMethodData.BgColor = Brushes.LightGray;
                        treatMethodData.Description = pa.Description;
                        treatMethodData.IsAvailable = pa.IsAvailable;
                        Datalist.Add(treatMethodData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 3 exception messsage: " + ex.Message);
            }
            #endregion
        }

        private void ListViewCTreatMethod_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListView1.SelectedIndex >= 0)
            {
                NameTextBox.Text = Datalist[ListView1.SelectedIndex].Name;
                ComboBoxTreatType.Text = Datalist[ListView1.SelectedIndex].Type;
                DescriptionTextBox.Text = Datalist[ListView1.SelectedIndex].Description;
                CheckBoxIsAvailable.IsChecked = Datalist[ListView1.SelectedIndex].IsAvailable;
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var treatMethodDao = new TreatMethodDao())
                {
                    var treatMethod = new TreatMethod();
                    treatMethod.Name = this.NameTextBox.Text;

                    var condition = new Dictionary<string, object>();
                    using (var treatTypeDao = new TreatTypeDao())
                    {
                        condition.Clear();
                        condition["Name"] = ComboBoxTreatType.Text;
                        var arealist = treatTypeDao.SelectTreatType(condition);
                        if (arealist.Count == 1)
                        {
                            treatMethod.TreatTypeId = arealist[0].Id;
                        }
                    }
                    treatMethod.Description = this.DescriptionTextBox.Text;
                    treatMethod.BgColor = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();
                    int lastInsertId = -1;
                    treatMethodDao.InsertTreatMethod(treatMethod, ref lastInsertId);
                    //UI
                    TreatMethodData treatMethodData = new TreatMethodData();
                    treatMethodData.Id = treatMethod.Id;
                    treatMethodData.Name = treatMethod.Name;
                    treatMethodData.Type = ComboBoxTreatType.Text;
                    treatMethodData.Description = treatMethod.Description;
                    treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(treatMethod.BgColor));
                    treatMethodData.IsAvailable = treatMethod.IsAvailable;
                    Datalist.Add(treatMethodData);
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var treatMethodDao = new TreatMethodDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListView1.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                fileds["NAME"] = NameTextBox.Text;

                var condition2 = new Dictionary<string, object>();
                using (var treatTypeDao = new TreatTypeDao())
                {
                    condition2.Clear();
                    condition2["Name"] = ComboBoxTreatType.Text;
                    var arealist = treatTypeDao.SelectTreatType(condition2);
                    if (arealist.Count == 1)
                    {
                        fileds["TREATTYPEID"] = arealist[0].Id;
                    }
                }
                fileds["DESCRIPTION"] = DescriptionTextBox.Text;
                fileds["ISAVAILABLE"] = CheckBoxIsAvailable.IsChecked;
                fileds["BGCOLOR"] = ((SolidColorBrush)Buttonrectangle.Fill).Color.ToString();

                var messageBox2 = new RemindMessageBox2();
                messageBox2.textBlock1.Text = "执行该操作将影响医嘱及排班";
                messageBox2.ShowDialog();
                if (messageBox2.remindflag == 1)
                {
                    treatMethodDao.UpdateTreatMethod(fileds, condition);
                    RefreshData();
                }

                
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var treatMethodDao = new TreatMethodDao())
                {
                    Datalist.Clear();
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = treatMethodDao.SelectTreatMethod(condition);
                    foreach (TreatMethod pa in list)
                    {
                        var treatMethodData = new TreatMethodData();
                        treatMethodData.Id = pa.Id;
                        treatMethodData.Name = pa.Name;
                        {
                            using (var treatTypeDao = new TreatTypeDao())
                            {
                                condition.Clear();
                                condition["ID"] = pa.TreatTypeId;
                                var arealist = treatTypeDao.SelectTreatType(condition);
                                if (arealist.Count == 1)
                                {
                                    treatMethodData.Type = arealist[0].Name;
                                }
                            }
                        }
                        treatMethodData.IsAvailable = pa.IsAvailable;
                        treatMethodData.Description = pa.Description;
                        treatMethodData.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(pa.BgColor)); 
                        Datalist.Add(treatMethodData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:AddButton_OnClick exception messsage: " + ex.Message);
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            using (var treatMethodDao = new TreatMethodDao())
            {
                treatMethodDao.DeleteTreatMethod(Datalist[ListView1.SelectedIndex].Id);
                RefreshData();
            }
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
        private void CTreatMethod_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            #region fill patientarea combox items
            this.ComboBoxTreatType.Items.Clear();
            try
            {
                using (var treatTypeDao = new TreatTypeDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = treatTypeDao.SelectTreatType(condition);
                    foreach (var pa in list)
                    {
                        this.ComboBoxTreatType.Items.Add(pa.Name);
                    }
                    if (list.Count > 0)
                        this.ComboBoxTreatType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatMethod.xaml.cs:ListViewCPatientRoom_OnLoaded 1 exception messsage: " + ex.Message);
            }
            #endregion
        }
    }

    public class TreatMethodData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _name;
        private string _type;
        private string _description;
        private Brush _bgColor;
        private bool _isAvailable;

        public TreatMethodData()
        {
        }

        public Brush BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                OnPropertyChanged("RectColor");
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

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
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
