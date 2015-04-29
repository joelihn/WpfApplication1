using System;
using System.Collections.Generic;
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : UserControl
    {
        public MainWindow Basewindow;
        public ObservableCollection<OrderData> Orderdatalist = new ObservableCollection<OrderData>();
        public Order(MainWindow mainWindow)
        {
            InitializeComponent();

            Orderdatalist.Clear();
            ListView1.ItemsSource = Orderdatalist;

            OrderData orderdata = new OrderData();
            orderdata.Name = "张三";
            orderdata.Mon = "AM/HD|PM/HDF";
            orderdata.Tue = "AM/HD|PM/HDHP";
            orderdata.Wed = "AM/HDF|PM/HD";
            orderdata.Thu = "";
            orderdata.Fri = "AM/HD";
            orderdata.Sta = "|PM/HDF";
            orderdata.Sun = "AM/HD|PM/HDF";
            Orderdatalist.Add(orderdata);

            OrderData orderdata1 = new OrderData();
            orderdata1.Name = "李四";
            orderdata1.Mon = "AM/HD|PM/HDF";
            orderdata1.Tue = "AM/HD|PM/HDHP";
            orderdata1.Wed = "AM/HDF|PM/HD";
            orderdata1.Thu = "";
            orderdata1.Fri = "AM/HD";
            orderdata1.Sta = "|PM/HDF";
            orderdata1.Sun = "AM/HD|PM/HDF";

            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);
            Orderdatalist.Add(orderdata);
            Orderdatalist.Add(orderdata1);

            Basewindow = mainWindow;
        }
    }
    public class OrderData : INotifyPropertyChanged //这个是用户数据的数据源
    {
        private string _name;
        private string _mon;
        private string _tue;
        private string _wed;
        private string _thu;
        private string _fri;
        private string _sta;
        private string _sun;

        public OrderData()
        {
            Name = "";

        }

        public string Name { get; set; }

        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { set; get; }
        public string Fri { set; get; }
        public string Sta { set; get; }
        public string Sun { set; get; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    public sealed class BackgroundConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listView =
                ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            int index =
                listView.ItemContainerGenerator.IndexFromContainer(item);


            if (index % 2 == 0)
            {
                return "#FFDBDDEA";
            }
            else
            {
                return "#FFF1F1F1";
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
