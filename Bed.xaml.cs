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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Bed.xaml
    /// </summary>
    public partial class Bed : UserControl
    {
        public MainWindow Basewindow;
        public ObservableCollection<BedData> Beddatalist = new ObservableCollection<BedData>();
        public Bed(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;

            BedData bedData = new BedData();
            bedData.Name = "张三";
            Beddatalist.Add(bedData);

            BedData bedData1 = new BedData();
            bedData1.Name = "李四";

            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
            Beddatalist.Add(bedData);
            Beddatalist.Add(bedData1);
        }
    }

    public class BedData : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private string _name;

        public BedData()
        {
            Name = "";

        }

        public string Name { get; set; }

        //public string Mon { get; set; }
        //public string Tue { get; set; }
        //public string Wed { get; set; }
        //public string Thu { set; get; }
        //public string Fri { set; get; }
        //public string Sta { set; get; }
        //public string Sun { set; get; }

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
