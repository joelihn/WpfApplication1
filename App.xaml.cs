using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (Current.Resources.MergedDictionaries.Count == 0)
            {
                Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source =
                        new Uri(
                        System.Windows.Forms.Application.StartupPath +
                        "\\" + "CN.Xaml")
                });
            }
            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }
    }
}
