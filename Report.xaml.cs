using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        private MainWindow BaseWindow;
        public ObservableCollection<ReportData> Datalist = new ObservableCollection<ReportData>();

        public Report(MainWindow mainWindow)
        {
            InitializeComponent();
            BaseWindow = mainWindow;
            this.ReportListBox.ItemsSource = Datalist;
        }

        private void ButtonPrint_OnClick(object sender, RoutedEventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();
            printDialog.MaxPage = 100;
            if (printDialog.ShowDialog() == true)
            {

                this.ButtonPrint.Visibility = Visibility.Hidden;
                printDialog.PrintVisual(this, "");
                this.ButtonPrint.Visibility = Visibility.Visible;
            } 

            ////创建一个PrintDialog的实例
            //System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog();
            ////创建一个PrintDocument的实例
            //PrintDocument docToPrint = new PrintDocument();
            ////将事件处理函数添加到PrintDocument的PrintPage事件中
            //docToPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(docToPrint_PrintPage);
            ////把PrintDialog的Document属性设为上面配置好的PrintDocument的实例
            //dlg.Document = docToPrint; 
            ////根据用户的选择，开始打印
            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    docToPrint.Print();//开始打印
            //}
        }

        //设置打印机开始打印的事件处理函数
        private void docToPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Hello, world!", new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Regular), System.Drawing.Brushes.Black, 100, 100);
        }
    


        private void Report_OnLoaded(object sender, RoutedEventArgs e)
        {

            this.LabelDate.Content = DateTime.Now.ToString("yyyy-M-d dddd");

            try
            {
                using (var scheduleTemplateDao = new ScheduleTemplateDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    //condition["DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                    var list = scheduleTemplateDao.SelectScheduleTemplate(condition);
                    foreach (var type in list)
                    {
                        var rReportData = new ReportData();

                        rReportData.Id = type.Id;
                        using (PatientDao patientDao = new PatientDao())
                        {
                            var condition2 = new Dictionary<string, object>();
                            condition2["ID"] = type.PatientId;
                            var list2 = patientDao.SelectPatient(condition2);
                            if((list2!=null) && (list.Count>0))
                                rReportData.PatientName = list2[0].Name;
                        }
                        
                        rReportData.Time = type.AmPmE;
                        rReportData.Method = type.Method;
                        if (type.BedId== -1)
                            rReportData.BedId = "";
                        else
                        {
                            rReportData.BedId = type.BedId.ToString();
                        }
                        rReportData.Description = type.Description;
                        Datalist.Add(rReportData);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }
        }
    }


    public class ReportData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _patientName;
        private string _time;
        private string _method;
        private string _bedId;
        private string _description;

        public ReportData()
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

        public string BedId
        {
            get { return _bedId; }
            set
            {
                _bedId = value;
                OnPropertyChanged("BedId");
            }
        }

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                _patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        public string Method
        {
            get { return _method; }
            set
            {
                _method = value;
                OnPropertyChanged("Method");
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
