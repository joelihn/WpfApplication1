﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WpfApplication1.DAOModule;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// Interaction logic for CTreatTime.xaml
    /// </summary>
    public partial class CTreatTime : UserControl
    {
        public ObservableCollection<TreatTimeData> Datalist = new ObservableCollection<TreatTimeData>();

        private int currnetIndex = -1;


        public CTreatTime()
        {
            InitializeComponent();
            this.ListViewTreatTime.ItemsSource = Datalist;
        }

        private void DescriptionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }

        private void ListViewTreatTime_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                using (var treatTimeDao = new TreatTimeDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    var list = treatTimeDao.SelectTreatTime(condition);
                    foreach (var type in list)
                    {
                        var treatTimeData = new TreatTimeData();
                        treatTimeData.Id = type.Id;
                        treatTimeData.Name = type.Name;
                        treatTimeData.Activated = type.Activated;
                        treatTimeData.BeginTime = type.BeginTime;
                        treatTimeData.EndTime = type.EndTime;
                        treatTimeData.Description = type.Description;

                        Datalist.Add(treatTimeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatTime.xaml.cs:ListViewCTreatTime_OnLoaded exception messsage: " + ex.Message);
            }
        }

        private void ListViewTreatTime_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ListViewTreatTime.SelectedIndex >= 0)
            {
                currnetIndex = ListViewTreatTime.SelectedIndex;
                this.ButtonApply.IsEnabled = false;
                this.ButtonCancel.IsEnabled = false;

                NameTextBox.Text = Datalist[ListViewTreatTime.SelectedIndex].Name;
                DescriptionTextBox.Text = Datalist[ListViewTreatTime.SelectedIndex].Description;
                BeginTextBox.Text = Datalist[ListViewTreatTime.SelectedIndex].BeginTime;
                EndTextBox.Text = Datalist[ListViewTreatTime.SelectedIndex].EndTime;
                if (Datalist[ListViewTreatTime.SelectedIndex].Activated)
                {
                    this.RadioButton1.IsChecked = true;
                }
                else
                {
                    this.RadioButton2.IsChecked = true;
                }
            }
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {

            if (ListViewTreatTime.SelectedIndex == -1) return;

            //if (this.NameTextBox.Text.Equals(""))
            //{
            //    var a = new RemindMessageBox1();
            //    a.remindText.Text = (string)FindResource("Message1001"); ;
            //    a.ShowDialog();
            //    return;
            //}

            //throw new NotImplementedException();
            using (var treatTimeDao = new TreatTimeDao())
            {
                var condition = new Dictionary<string, object>();
                condition["ID"] = Datalist[ListViewTreatTime.SelectedIndex].Id;

                var fileds = new Dictionary<string, object>();
                
                fileds["NAME"] = NameTextBox.Text;
                fileds["BEGINTIME"] = BeginTextBox.Text;
                fileds["ENDTIME"] = EndTextBox.Text;
                if ((bool) (RadioButton1.IsChecked))
                {
                    fileds["ACTIVATED"] = true;
                }else if ((bool) (RadioButton2.IsChecked))
                {
                    fileds["ACTIVATED"] = false;
                }
                fileds["DESCRIPTION"] = DescriptionTextBox.Text;

                treatTimeDao.UpdateTreatTime(fileds, condition);
                int temp = this.ListViewTreatTime.SelectedIndex;
                RefreshData();
                this.ListViewTreatTime.SelectedIndex = temp;
            }
            this.ButtonApply.IsEnabled = false;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            {
                this.ListViewTreatTime.SelectedIndex = -1;
                this.ListViewTreatTime.SelectedIndex = currnetIndex;
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var treatTimeDao = new TreatTimeDao())
                {
                    Datalist.Clear();

                    var condition = new Dictionary<string, object>();
                    var list = treatTimeDao.SelectTreatTime(condition);
                    foreach (var pa in list)
                    {
                        var treatTimeData = new TreatTimeData();
                        treatTimeData.Id = pa.Id;
                        treatTimeData.Name = pa.Name;
                        treatTimeData.Activated = pa.Activated;
                        treatTimeData.BeginTime = pa.BeginTime;
                        treatTimeData.EndTime = pa.EndTime;
                        treatTimeData.Description = pa.Description;
                        Datalist.Add(treatTimeData);
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CTreatTime.xaml.cs:RefreshData exception messsage: " + ex.Message);
            }
        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e)
        {

            this.ButtonApply.IsEnabled = true;
            this.ButtonCancel.IsEnabled = true;
        }
    }

    public class TreatTimeData : INotifyPropertyChanged
    {
        private Int64 _id;
        private bool _activated;
        private string _name;
        private string _beginTime;
        private string _endTime;
        private string _description;
        public TreatTimeData()
        {
        }

        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                OnPropertyChanged("Activated");
            }
        }
        public Int64 Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string BeginTime
        {
            get { return _beginTime; }
            set
            {
                _beginTime = value;
                OnPropertyChanged("BeginTime");
            }
        }

        public string EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
