using System.Windows;
using System.Windows.Input;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// RemindMessageBox2.xaml 的交互逻辑
    /// </summary>
    public partial class RemindMessageBox2 : Window
    {
        public int remindflag;

        public RemindMessageBox2()
        {
            InitializeComponent();
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            remindflag = 2;
            Close();
        }
    }
}