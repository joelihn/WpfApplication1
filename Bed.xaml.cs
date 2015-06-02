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
using WpfApplication1.CustomUI;
using WpfApplication1.DAOModule;
using DragDropEffects = System.Windows.Forms.DragDropEffects;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Bed.xaml
    /// </summary>
    public partial class Bed : UserControl
    {
        public MainWindow Basewindow;
        public int selectoperation;
        //public ObservableCollection<BedData> Beddatalist = new ObservableCollection<BedData>();

        public ObservableCollection<BedPatientData> BedPatientList = new ObservableCollection<BedPatientData>();

        public Bed(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            this.PatientlistView.ItemsSource = BedPatientList;
            this.BedListBox.ItemsSource = BedPatientList;
            EndatePicker.Text = DateTime.Now.ToString();
            BeginDatePicker.Text = (DateTime.Now - TimeSpan.FromDays(3)).ToString();

            this.SexComboBox.Items.Clear();
            this.SexComboBox.Items.Add("所有");
            this.SexComboBox.Items.Add("男");
            this.SexComboBox.Items.Add("女");
            SexComboBox.SelectedIndex = 0;
        }

        private void PatientlistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void InquireButton_Click(object sender, RoutedEventArgs e)
        {
            var begin = new DateTime();
            var end = new DateTime();
            if (!BeginDatePicker.Text.Equals(""))
                begin = DateTime.Parse(BeginDatePicker.Text);
            else
                begin = DateTime.Now;
            if (!EndatePicker.Text.Equals(""))
                end = DateTime.Parse(EndatePicker.Text);
            else
                end = DateTime.Now;

            TimeSpan timeSpan = end - begin;
            if (timeSpan.Days > 90)
            {
                var a = new RemindMessageBox1();
                a.remindText.Text = "查询间隔最多不能超过90天.";
                a.ShowDialog();
                return;
            }

            using (var complexDao = new ComplexDao())
            {
                var condition = new Dictionary<string, object>();
                if (!NameTextBox.Text.Equals(""))
                    condition["NAME"] = NameTextBox.Text;
                //if (!IDTextBox.Text.Equals(""))
                //    condition["ID"] = IDTextBox.Text;
                if (!PatientIDTextBox.Text.Equals(""))
                    condition["PATIENTID"] = PatientIDTextBox.Text;
                condition["TREATSTATUSID"] = 1;
                condition["ISASSIGNED"] = 0;
                BedPatientList.Clear();

                List<Patient> list = complexDao.SelectPatient(condition, begin, end);
                foreach (Patient fmriPatient in list)
                {
                    var informatian = new BedPatientData();
                    informatian.Id = fmriPatient.Id;
                    informatian.PatientId = fmriPatient.PatientId;
                    informatian.Name = fmriPatient.Name;
                    BedPatientList.Add(informatian);
                }
            }
        }


        private void TimeRadioButton1_Click(object sender, RoutedEventArgs e)
        {
            selectoperation = 0;
            EndatePicker.SelectedDate = DateTime.Now;
            if (TimeRadioButton1.IsChecked == true)
            {
                BeginDatePicker.SelectedDate = DateTime.Parse("2012-01-01");
            }
            else if (TimeRadioButton2.IsChecked == true)
            {
                if (EndatePicker.SelectedDate != null)
                {
                    BeginDatePicker.SelectedDate = EndatePicker.SelectedDate - TimeSpan.FromDays(7);
                }
            }
            else if (TimeRadioButton3.IsChecked == true)
            {
                if (EndatePicker.SelectedDate != null)
                {
                    BeginDatePicker.SelectedDate = EndatePicker.SelectedDate - TimeSpan.FromDays(3);
                }
            }
            selectoperation = 1;
        }

        private void BeginDatePicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void EndatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
        }

        private void EndatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectoperation == 1)
            {
                TimeRadioButton1.IsChecked = false;
                TimeRadioButton2.IsChecked = false;
                TimeRadioButton3.IsChecked = false;
            }
        }

        private void Bed_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (InfectTypeDao infectTypeDao = new InfectTypeDao())
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>();
                    var list = infectTypeDao.SelectInfectType(condition);
                    InfectTypeComboBox.Items.Clear();
                    InfectTypeComboBox.Items.Add("所有");
                    foreach (InfectType type in list)
                    {
                        InfectTypeComboBox.Items.Add(type.Name);
                    }
                    InfectTypeComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Bed.xaml.cs:Bed_OnLoaded InfectType ComboxItem exception messsage: " + ex.Message);
            }

            BedPatientList.Clear();
            using (ComplexDao patientDao = new ComplexDao())
            {

                Dictionary<string, object> condition = new Dictionary<string, object>();
                //var list = patientDao.SelectPatient(condition);
                condition["TREATSTATUSID"] = 1;
                condition["ISASSIGNED"] = 0;
                var end = DateTime.Now;
                var begin = end.AddMonths(-1);
                List<Patient> list = patientDao.SelectPatient(condition, begin, end);

                foreach (Patient type in list)
                {
                    BedPatientData patientInfo = new BedPatientData();
                    patientInfo.Id = type.Id;
                    patientInfo.Name = type.Name;
                    patientInfo.PatientId = type.PatientId;


                    BedPatientList.Add(patientInfo);
                    //PatientList.Add(patientInfo);
                }
            }

        }

        private void Button_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(string));
            if (data == null) { return; }
            this.Button1.Content = data.ToString();
            this.PatientlistView.Items.Remove(data);
        }

        private void PatientlistView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        private void PatientlistView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        private void PatientlistView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.PatientlistView.Drop(this.PatientlistView.SelectedItem, DragDropEffects.Move);
        }
    }

    public class BedPatientData : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private Int64 _id;

        public BedPatientData()
        {
            Name = "";

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

        public string Name { get; set; }
        public string PatientId { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    public class BedInfo : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private Int64 _id;
        private int _roomID;
        private string _bedName;
        private string _patientName;
        private Brush _titleBrush;
        private Brush _bedBrush;
        private int _infcetionType;
        
        


        public BedInfo()
        {
            Name = "";

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

        public string Name { get; set; }
        public string PatientId { get; set; }

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
