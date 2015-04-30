using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
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
using WpfApplication1.LogModule;
using WpfApplication1.Utils;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly LogHelper Log = LogHelper.GetInstance();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
        private Init initContent;
        private Order orderContent;
        private Bed bedContent;
        private Config configContent;

        public MainWindow()
        {
            InitializeComponent();

            if (config.AppSettings.Settings["DataBasePath"].Value.Equals(""))
            {
                config.AppSettings.Settings["DataBasePath"].Value = System.Windows.Forms.Application.StartupPath;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

            ConstDefinition.DbStr = "Data Source=" + config.AppSettings.Settings["DataBasePath"].Value + "\\db.s3db;Version=3;";
            SQLiteConnection sqlConn = null;
            try
            {
                sqlConn = new SQLiteConnection(ConstDefinition.DbStr);
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

            initContent = new Init(this);
            orderContent = new Order(this);
            bedContent = new Bed(this);
            configContent = new Config(this);

        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = initContent;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = orderContent;
        }

        private void BedButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = bedContent;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            this.RightContent.Content = configContent;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
