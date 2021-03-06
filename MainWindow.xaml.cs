﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CrashReporterDotNET;
using WpfApplication1.CustomUI;
using WpfApplication1.LogModule;
using WpfApplication1.Utils;
using Application = System.Windows.Forms.Application;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string,string> Key = new Dictionary<string, string>();
        public static Dictionary<string, string> Symbol = new Dictionary<string, string>();
        public static int ComboBoxPatientGroupIndex = 0;
        public static readonly LogHelper Log = LogHelper.GetInstance();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public PatientGroupPanel patientGroupPanel;
        public Init initContent;
        public Order orderContent;
        public Shedule sheduleContent;
        public Bed bedContent;
        public Config configContent;
        public Report reeportContent;
        public ObservableCollection<string> TopMenuCollection = new ObservableCollection<string>();
        
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            ReportCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
            Environment.Exit(0);
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ReportCrash(e.Exception);
        }

        private static void ReportCrash(Exception exception)
        {
            var reportCrash = new ReportCrash
            {
                ToEmail = "lijun.zhou@outlook.com"
            };

            reportCrash.Send(exception);
        }



        public MainWindow()
        {

            ConstDefinition.DbStr = "server=" + config.AppSettings.Settings["IpAddress"].Value + ";database=" + config.AppSettings.Settings["Database"].Value
                + ";uid=" + config.AppSettings.Settings["Username"].Value + ";pwd=" + config.AppSettings.Settings["Password"].Value + ";Timeout=10";
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(ConstDefinition.DbStr);
                sqlConn.Open();
            }
            catch (Exception e)
            {
                var messageBox1 = new RemindMessageBox1(true);
                messageBox1.remindText.Text = (string)FindResource("Message40");
                messageBox1.ShowDialog();
                Log.WriteErrorLog("数据路连接失败，请确认数据库文件和密码设置是否正确.", e);
                Close();
                return;
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }

            InitializeComponent();

            this.Title = config.AppSettings.Settings["Title"].Value.ToString();

            Key["姓名"]="NAME";
            Key["性别"] = "GENDER";
            Key["血型"] = "BLOODTYPE";
            Key["婚姻状况"] = "MARRIAGE";
            Key["感染情况"] = "INFECTTYPEID";
            Key["治疗状态"] = "TREATSTATUSID";
            Key["固定床位"] = "ISFIXEDBED";
            Key["所属分区"] = "AREAID";

            Symbol["大于"] = ">";
            Symbol["小于"] = "<";
            Symbol["等于"] = "=";
            Symbol["包含"] = "like";
            Symbol["不包含"] = "not like";

          
            Dispatcher.UnhandledException +=Dispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            //if (config.AppSettings.Settings["DataBasePath"].Value.Equals(""))
            //{
            //    config.AppSettings.Settings["DataBasePath"].Value = Application.StartupPath;
            //    config.Save(ConfigurationSaveMode.Modified);
            //    ConfigurationManager.RefreshSection("appSettings");
            //}


            //TODO test all database table access operation
            #region
            //using (var bedDao = new BedDao())
            //{
            //    var bed = new WpfApplication1.DAOModule.Bed();
            //    bed.PatientRoomId = 1;
            //    bed.Name = "床位1";
            //    bed.Type = 0;
            //    bed.IsAvailable = true;
            //    bed.IsOccupy = false;
            //    bed.Description = "描述";
            //    bed.Reserved = "保留字段";

            //    int lastInsertId = -1;
            //    bedDao.InsertBed(bed, ref lastInsertId);

            //    var condition = new Dictionary<string, object>();
            //    condition["NAME"] = "床位1";
            //    var list = bedDao.SelectBed(condition);

            //    var fields = new Dictionary<string, object>();
            //    fields["DESCRIPTION"] = "描述描述";
            //    bedDao.UpdateBed(fields, condition);

            //    bedDao.DeleteBed((int)list[0].Id);
            //}
            #endregion


            patientGroupPanel = new PatientGroupPanel(this);
            initContent = new Init(this);
            orderContent = new Order(this);
            sheduleContent = new Shedule(this);
            bedContent = new Bed(this);
            configContent = new Config(this);
            reeportContent = new Report(this);

            /*initContent.ButtonNew.Click += delegate
            {
                var a = new SignUP(this);
                a.ShowDialog();
            };*/

           /* if (ConstDefinition.Runlevel == 1)
                this.ConfigButton.IsEnabled = true;
            else
                this.ConfigButton.IsEnabled = false;*/
            this.RightContentR.Content = initContent;
            this.RightContentL.Content = patientGroupPanel;

            TopMenuListBox.ItemsSource = TopMenuCollection;
            TopMenuCollection.Add("登记");
            TopMenuCollection.Add("医嘱");
            TopMenuCollection.Add("排班");
            TopMenuCollection.Add("排床");
            TopMenuCollection.Add("设置");
            TopMenuCollection.Add("报表");
        }



        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContentR.Content = initContent;
            this.RightContentL.Content = patientGroupPanel;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContentR.Content = orderContent;
            this.RightContentL.Content = patientGroupPanel;
        }

        private void SheduleButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContentR.Content = sheduleContent;
            this.RightContentL.Content = patientGroupPanel;
        }

        private void BedButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContentA.Content = bedContent;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContentA.Content = configContent;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            var shutdownwindow = new Newshutdown(this);
            shutdownwindow.parent = this;
            shutdownwindow.ShowDialog();
        }

        private void listbox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedIndex == -1) return;
            switch (listBox.SelectedIndex)
            {
                case 0:
                    this.RightContentA.Visibility = Visibility.Hidden;
                    this.RightContentA.Content = "";
                    this.RightContentR.Content = initContent;
                    //initContent.ReLoad();
                    //patientGroupPanel.reLoaded();
                    this.RightContentL.Content = patientGroupPanel;
                    

                    break;
                case 1:
                    this.RightContentA.Visibility = Visibility.Hidden;
                    this.RightContentA.Content = "";
                    this.RightContentR.Content = orderContent;
                    //patientGroupPanel.reLoaded();
                    this.RightContentL.Content = patientGroupPanel;
                    break;
                case 2:
                    /*this.RightContentA.Visibility = Visibility.Hidden;
                    this.RightContentR.Content = sheduleContent;*/
                    this.RightContentA.Content = sheduleContent;
                    this.RightContentA.Visibility = Visibility.Visible;
                    this.RightContentL.Content = "";
                    this.RightContentR.Content = "";
                    //sheduleContent.Reload();
                    /*sheduleContent.PatientGroupComboBox.SelectedValue =
                        patientGroupPanel.ComboBoxPatientGroup.SelectedValue;*/

                    sheduleContent.PatientGroupComboBox.SelectedIndex =
                        patientGroupPanel.ComboBoxPatientGroup.SelectedIndex;
                    
                    /*this.RightContentL.Visibility = Visibility.Hidden;
                    this.RightContentR.Visibility = Visibility.Hidden;*/


                    //this.RightContentR.Content = sheduleContent;
                    //this.RightContentL.Content = patientGroupPanel;
                    break;
                case 3:
                    this.RightContentA.Content = bedContent;
                    this.RightContentA.Visibility = Visibility.Visible;
                    this.RightContentL.Content = "";
                    this.RightContentR.Content = "";
                    break;
                case 4:

                    this.RightContentA.Content = configContent;
                    this.RightContentA.Visibility = Visibility.Visible;
                    
                    this.RightContentL.Content = "";
                    this.RightContentR.Content = "";
                    break;
                case 5:
                    this.RightContentA.Content = reeportContent;
                    this.RightContentA.Visibility = Visibility.Visible;
                   
                    this.RightContentL.Content = "";
                    this.RightContentR.Content = "";
                    break;
            }
        }
    }
}
