using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : UserControl
    {
        public MainWindow Basewindow;
        private CPatientArea cPatientArea;
        private CPatientRoom cPatientRoom;
        private CBed cBed;
        private CInfectType cInfectType;
        private CMedicalOrderPara cMedicalOrderPara;
        private CTreatType cTreatType;
        private CTreatMethod cTreatMethod;
        private CTreatTime cTreatTime;
        private CTreatStatus cTreatStatus;
        private CDataBaseSetting cDateBaseSetting;

        public ObservableCollection<string> ConfigMenuCollection = new ObservableCollection<string>();
        

        public Config(MainWindow window)
        {
            InitializeComponent();
            Basewindow = window;
            cPatientArea = new CPatientArea();
            cPatientRoom = new CPatientRoom();
            cBed = new CBed();
            cInfectType = new CInfectType();
            cMedicalOrderPara = new CMedicalOrderPara();
            cTreatType = new CTreatType();
            cTreatMethod = new CTreatMethod();
            cTreatTime = new CTreatTime();
            cTreatStatus = new CTreatStatus();
            cDateBaseSetting = new CDataBaseSetting();
            this.RightContent.Content = cPatientArea;
        }

        private void InitConfigMenu()
        {
            ConifgMenuListBox.ItemsSource = ConfigMenuCollection;
            ConfigMenuCollection.Clear();
            ConfigMenuCollection.Add("病区设置");
            ConfigMenuCollection.Add("床位设置");
            ConfigMenuCollection.Add("感染类型");
            ConfigMenuCollection.Add("机器类型");
            ConfigMenuCollection.Add("治疗方法");
            ConfigMenuCollection.Add("治疗班次");
            ConfigMenuCollection.Add("治疗状态");
            ConfigMenuCollection.Add("数据库设置");
            ConfigMenuCollection.Add("患者组");
        }
        private void PatientAreaButton_OnClick(object sender, RoutedEventArgs e)
        {

            this.RightContent.Content = cPatientArea;
        }

        private void PatientRoomButton_OnClick(object sender, RoutedEventArgs e)
        {

            this.RightContent.Content = cPatientRoom;
        }

        private void BedButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cBed;

        }

        private void InfectTypeButton_OnClick(object sender, RoutedEventArgs e)
        {

            this.RightContent.Content = cInfectType;
        }

        private void MedicalOrderParaButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cMedicalOrderPara;

        }

        private void TreatTypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cTreatType;

        }

        private void TreatMethodButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cTreatMethod;

        }

        private void TreatStatusButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cTreatStatus;

        }
        private void DataBasesetting_OnClick(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = cDateBaseSetting;

        }

        private void ConifgMenuListBox_Loaded(object sender, RoutedEventArgs e)
        {
            InitConfigMenu();
        }


        private void listbox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedIndex == -1) return;
            switch (listBox.SelectedIndex)
            {
                case 0:
                    this.RightContent.Content = cPatientArea;
                    break;
                case 1:
                    this.RightContent.Content = cBed;
                    break;
                case 2:
                    this.RightContent.Content = cInfectType;
                    break;
                case 3:
                    this.RightContent.Content = cTreatType;
                    break;
                case 4:
                    this.RightContent.Content = cTreatMethod;
                    break;
                case 5:
                    this.RightContent.Content = cTreatTime;
                    break;
                case 6:
                    this.RightContent.Content = cTreatStatus;
                    break;
                case 7:
                    this.RightContent.Content = cTreatStatus;
                    break;
                case 8:
                    this.RightContent.Content = cTreatStatus;
                    break;

            }
        }


    }
}
