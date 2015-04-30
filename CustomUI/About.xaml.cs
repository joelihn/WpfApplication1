using System;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public int remindflag;

        public About()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            remindflag = 1;
            Close();
        }

        private void TitleGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    System.Diagnostics.Process.Start(ConstDefinition.HelpFileDir + "\\功能磁共振临床应用软件项目用户手册.chm ");
            //}
            //catch (Exception ex)
            //{
            //    var cantOpenBox = new RemindMessageBox1();

            //    // 该段代码适应了帮助文件路径的长度。 [Hu Tai, 2012-5-15]
            //    cantOpenBox.remindText.Text = (string) FindResource("Message33") + ConstDefinition.HelpFileDir +
            //                                  "\\功能磁共振临床应用软件项目用户手册.chm ";
            //    cantOpenBox.MainGrid.Height =
            //        (int) (cantOpenBox.MainGrid.Height*(1 + (cantOpenBox.remindText.Text.Length - 30.0f)/180.0f));
            //    cantOpenBox.remindText.TextWrapping = TextWrapping.Wrap;
            //    cantOpenBox.ShowDialog();
            //    Window1.Log.WriteErrorLog("About.xaml.cs-Hyperlink_Click", ex);
            //    return;
            //}
        }
    }
}