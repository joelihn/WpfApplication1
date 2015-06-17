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
        private CTreatStatus cTreatStatus;
        private CDataBaseSetting cDateBaseSetting;
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
            cTreatStatus = new CTreatStatus();
            cDateBaseSetting = new CDataBaseSetting();
            this.RightContent.Content = cPatientArea;
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
    }
}
