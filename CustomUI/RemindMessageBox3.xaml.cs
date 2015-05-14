using System.Windows;
using System.Windows.Input;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// RemindMessageBox3.xaml 的交互逻辑
    /// </summary>
    public partial class RemindMessageBox3 : Window
    {
        public int remindflag;

        public RemindMessageBox3(bool ret = false)
        {
            InitializeComponent();
            if (!ret)
                Owner = Application.Current.MainWindow;
        }

        private void TitleGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == true)
                remindflag = 1;
            else
                remindflag = 3;
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            remindflag = 2;
            Close();
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            //OK.IsEnabled = true;
        }
    }
}
