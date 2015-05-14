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
using System.Windows.Controls.Primitives;
using WpfApplication1.DataStructures;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : UserControl
    {
        public MainWindow Basewindow;
        
        public List<PatientSchedule> PatientScheduleList = new List<PatientSchedule>();
        public List<PatientItem> PatientItemList = new List<PatientItem>();

        public Dictionary<string ,string > IcomDictionary = new Dictionary<string, string>();
        
        
        public Order(MainWindow mainWindow)
        {
            InitializeComponent();
            //ListBox1.ItemsSource = PatientScheduleList;
            ListBox1.ItemsSource = PatientItemList;

           /* PatientSchedule schedule = new PatientSchedule();
            schedule.PatientID = 1;
            schedule.PatientName = "zhangsan";
            PatientScheduleList.Add(schedule);

            PatientSchedule schedule1 = new PatientSchedule();
            schedule1.PatientID = 2;
            schedule1.PatientName = "lisi";
            PatientScheduleList.Add(schedule1);*/

            PatientItem schedule = new PatientItem();
            schedule.PatientID = 1;
            schedule.PatientName = "zhangsan";
            schedule.SunNode0 = "/Resources/AM_HD.png";
            schedule.SunNode1 = "/Resources/E_HP.png";

            schedule.MonNode0 = "/Resources/AM_HD.png";
            schedule.MonNode1 = "/Resources/E_HP.png";

            schedule.TueNode0 = "/Resources/AM_HD.png";
            schedule.TueNode1 = "/Resources/E_HP.png";

            schedule.WedNode0 = "/Resources/AM_HD.png";
            schedule.WedNode1 = "/Resources/E_HP.png";

            schedule.ThuNode0 = "/Resources/AM_HD.png";
            schedule.ThuNode1 = "/Resources/E_HP.png";

            schedule.FriNode0 = "/Resources/AM_HD.png";
            schedule.FriNode1 = "/Resources/E_HP.png";

            schedule.SatNode0 = "/Resources/AM_HD.png";
            schedule.SatNode1 = "/Resources/E_HP.png";

            PatientItemList.Add(schedule);


            PatientItem schedule1 = new PatientItem();
            schedule1.PatientID = 2;
            schedule1.PatientName = "lisi";
            schedule1.SunNode0 = "/Resources/N_N.png";
            schedule1.SunNode1 = "/Resources/N_N.png";

            schedule1.MonNode0 = "/Resources/N_N.png";
            schedule1.MonNode1 = "/Resources/N_N.png";

            schedule1.TueNode0 = "/Resources/N_N.png";
            schedule1.TueNode1 = "/Resources/N_N.png";

            schedule1.WedNode0 = "/Resources/N_N.png";
            schedule1.WedNode1 = "/Resources/N_N.png";

            schedule1.ThuNode0 = "/Resources/N_N.png";
            schedule1.ThuNode1 = "/Resources/N_N.png";

            schedule1.FriNode0 = "/Resources/N_N.png";
            schedule1.FriNode1 = "/Resources/N_N.png";

            schedule1.SatNode0 = "/Resources/N_N.png";
            schedule1.SatNode1 = "/Resources/N_N.png";
            PatientItemList.Add(schedule1);


            string color = (string)System.Windows.Application.Current.Resources["ysq"];

            InitDay();

            Basewindow = mainWindow;
        }

        private void InitDay()
        {
            int dayofweek = (int)DateTime.Now.DayOfWeek;
            switch (dayofweek)
            {
                case 0:
                    BtnSun.IsChecked = true;
                    break;
                case 1:
                    BtnTue.IsChecked = true;
                    break;
                case 2:
                    BtnWed.IsChecked = true;
                    break;
                case 3:
                    BtnThe.IsChecked = true;
                    break;
                case 4:
                    BtnFri.IsChecked = true;
                    break;
                case 5:
                    BtnSta.IsChecked = true;
                    break;
            }
        }

        private void InitIcomDictionary()
        {
            IcomDictionary.Add("AM_HD", "/Resources/AM_HD.png");
            IcomDictionary.Add("AM_HDF", "/Resources/AM_HDF.png");
            IcomDictionary.Add("AM_HP", "/Resources/AM_HP.png");

            IcomDictionary.Add("PM_HD", "/Resources/PM_HD.png");
            IcomDictionary.Add("PM_HDF", "/Resources/PM_HDF.png");
            IcomDictionary.Add("PM_HP", "/Resources/PM_HP.png");

            IcomDictionary.Add("E_HD", "/Resources/E_HD.png");
            IcomDictionary.Add("E_HDF", "/Resources/E_HDF.png");
            IcomDictionary.Add("E_HP", "/Resources/E_HP.png");

        }

        private string GetNextItem(string curItem, MouseButton mouseButton)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(curItem);
            string ret = "";
            string[] str = name.Split('_');

            if (str.Length != 2)
                return ret;

            string time = str[0];
            string type = str[1];

            if (mouseButton == MouseButton.Left)
            {
                switch (time)
                {
                    case "AM":
                        time = "PM";
                        break;
                    case "PM":
                        time = "E";
                        break;
                    case "E":
                        time = "N";
                        break;
                    case "N":
                        time = "AM";
                        type = "HD";
                        break;
                    default:
                        time = "AM";
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case "HD":
                        type = "HDF";
                        break;
                    case "HDF":
                        type = "HP";
                        break;
                    case "HP":
                        type = "HD";
                        break;
                    case "N":
                        time = "AM";
                        type = "HD";
                        break;
                    default:
                        type = "HD";
                        break;
                }
            }

            if(time == "N")
                ret = "/Resources/N_N.png";
            else
                ret = "/Resources/" + time + "_" + type + ".png";
            return ret;
        }

        private void ButtonBase_OnClick(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button) sender;
            string tag = (string) btn.Tag;
            int index = ListBox1.SelectedIndex;
            if ( index == -1 ) 
                return;
            //string source = ((Image)btn.Content).Source.ToString();
            //MessageBox.Show(source);

            ChangeItems(index, tag, e.ChangedButton);
            /*if (e.ChangedButton == MouseButton.Left)
            {
                
                ChangeAmOrPm( index , tag);
                //MessageBox.Show( tag + " Left");
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                ChangeHemodialysisItem();
                MessageBox.Show( tag + " Right");
            }*/


            
        }

        private void ChangeItems(int index, string tag, MouseButton mouseButton)
        {
            string curItem = "";
            switch (tag)
            {
                case "Sun0":
                    curItem = PatientItemList[index].SunNode0;
                    PatientItemList[index].SunNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Sun1":
                    curItem = PatientItemList[index].SunNode1;
                    PatientItemList[index].SunNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Mon0":
                    curItem = PatientItemList[index].MonNode0;
                    PatientItemList[index].MonNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Mon1":
                    curItem = PatientItemList[index].MonNode1;
                    PatientItemList[index].MonNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Tue0":
                    curItem = PatientItemList[index].TueNode0;
                    PatientItemList[index].TueNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Tue1":
                    curItem = PatientItemList[index].TueNode1;
                    PatientItemList[index].TueNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Wed0":
                    curItem = PatientItemList[index].WedNode0;
                    PatientItemList[index].WedNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Wed1":
                    curItem = PatientItemList[index].WedNode1;
                    PatientItemList[index].WedNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Thu0":
                    curItem = PatientItemList[index].ThuNode0;
                    PatientItemList[index].ThuNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Thu1":
                    curItem = PatientItemList[index].ThuNode1;
                    PatientItemList[index].ThuNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Fri0":
                    curItem = PatientItemList[index].FriNode0;
                    PatientItemList[index].FriNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Fri1":
                    curItem = PatientItemList[index].FriNode1;
                    PatientItemList[index].FriNode1 = GetNextItem(curItem, mouseButton);
                    break;

                case "Sta0":
                    curItem = PatientItemList[index].SatNode0;
                    PatientItemList[index].SatNode0 = GetNextItem(curItem, mouseButton);
                    break;
                case "Sta1":
                    curItem = PatientItemList[index].SatNode1;
                    PatientItemList[index].SatNode1 = GetNextItem(curItem, mouseButton);
                    break;

                default:
                    break;

            }
            
            ListBox1.Items.Refresh();

            
        }

        private void ChangeHemodialysisItem()
        {
            
        }

        private void Tesetbtn_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    MessageBox.Show("left");
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    MessageBox.Show("right");
                }
            }
            //throw new NotImplementedException();
        }

        private void Tesetbtn_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                MessageBox.Show("left");
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                MessageBox.Show("right");
            }
        }

        private void BtnDay_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton) sender;
            if (btn.IsChecked == true)
            {
                UncheckOtherToggleButton(btn);
            }
            else
            {
                btn.IsChecked = true;
                UncheckOtherToggleButton(btn);
            }
        }

        private void UncheckOtherToggleButton(ToggleButton btn )
        {
            foreach (var i in GridDay.Children)
            {
                if ((i is ToggleButton)&& i!= btn)
                {
                    ((ToggleButton) i).IsChecked = false;
                }
            }
        }

    }

    public class PatientItem
    {
        public int PatientID { get; set; }
        public string PatientName { get; set; }

        public string SunNode0 { get; set; }
        public string SunNode1 { get; set; }



        public string MonNode0 { get; set; }
        public string MonNode1 { get; set; }



        public string TueNode0 { get; set; }
        public string TueNode1 { get; set; }


        public string WedNode0 { get; set; }
        public string WedNode1 { get; set; }



        public string ThuNode0 { get; set; }
        public string ThuNode1 { get; set; }


        public string FriNode0 { get; set; }
        public string FriNode1 { get; set; }


        public string SatNode0 { get; set; }
        public string SatNode1 { get; set; }




    }

    public struct ButtonNode
    {
        //public string path { get; set; }
        public HemodialysisItemIcon CurrentWeekStauts { get; set; }
        public AmOrPmIcon NetxweekStauts { get; set; }
    }

    public struct HemodialysisItemIcon
    {
        public HemodialysisItem hemodialysisItem { get; set; }
        public string iconSource { get; set; }
    }

    public struct AmOrPmIcon
    {
        public AmOrPm amOrPm { get; set; }
        public string iconSource { get; set; }
    }


    /*
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
    }*/
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
