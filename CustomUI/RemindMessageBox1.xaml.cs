using System.Windows;
using System.Windows.Input;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// RemindMessageBox1.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class RemindMessageBox1 : Window
    {
        public int remindflag;

        public RemindMessageBox1(bool ret = false)
        {
            InitializeComponent();
            if (!ret)
                Owner = Application.Current.MainWindow;
        }

        private void TitleGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            remindflag = 1;
            Close();
        }
    }
}