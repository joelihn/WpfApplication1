using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Printing;
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
using WpfApplication1.DAOModule;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace WpfApplication1
{

    public class DataPaginator : DocumentPaginator
    {
        #region  属性及字段
        private DataTable dataTable;
        private Typeface typeFace;
        private double fontSize;
        private double margin;
        private int rowsPerPage;
        private int pageCount;
        private Size pageSize;
        public override Size PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                PaginateData();
            }
        }
        public override bool IsPageCountValid
        {
            get { return true; }
        }
        public override int PageCount
        {
            get { return pageCount; }
        }
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
        #endregion
        #region  构造函数相关方法
        //构造函数
        public DataPaginator(DataTable dt, Typeface typeface, int fontsize, double margin, Size pagesize)
        {
            this.dataTable = dt;
            this.typeFace = typeface;
            this.fontSize = fontsize;
            this.margin = margin;
            this.pageSize = pagesize;
            PaginateData();
        }
        /// <summary>
        /// 计算页数pageCount
        /// </summary>
        private void PaginateData()
        {
            //字符大小度量标准
            FormattedText ft = GetFormattedText("A");  //取"A"的大小计算行高等；
            //计算行数
            rowsPerPage = (int)((pageSize.Height - margin * 2) / ft.Height);
            //预留标题行
            rowsPerPage = rowsPerPage - 1;
            pageCount = (int)Math.Ceiling((double)dataTable.Rows.Count / rowsPerPage);
        }
        /// <summary>
        /// 格式化字符
        /// </summary>
        private FormattedText GetFormattedText(string text)
        {
            return GetFormattedText(text, typeFace);
        }
        /// <summary>
        /// 按指定样式格式化字符
        /// </summary>
        private FormattedText GetFormattedText(string text, Typeface typeFace)
        {
            return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, fontSize, Brushes.Black);
        }
        /// <summary>
        /// 获取对应页面数据并进行相应的打印设置
        /// </summary>
        public override DocumentPage GetPage(int pageNumber)
        {
            //设置列宽
            FormattedText ft = GetFormattedText("A");
            List<double> columns = new List<double>();
            int rowCount = dataTable.Rows.Count;
            int colCount = dataTable.Columns.Count;
            double columnWith = margin;
            columns.Add(columnWith);
            for (int i = 1; i < colCount; i++)
            {
                columnWith += ft.Width * 15;
                columns.Add(columnWith);
            }
            //获取页面对应行数
            int minRow = pageNumber * rowsPerPage;
            int maxRow = minRow + rowsPerPage;
            //绘制打印内容
            DrawingVisual visual = new DrawingVisual();
            Point point = new Point(margin, margin);
            using (DrawingContext dc = visual.RenderOpen())
            {
                Typeface columnHeaderTypeface = new Typeface(typeFace.FontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                //获取表头
                for (int i = 0; i < colCount; i++)
                {
                    point.X = columns[i];
                    ft = GetFormattedText(dataTable.Columns[i].Caption, columnHeaderTypeface);
                    dc.DrawText(ft, point);
                }
                dc.DrawLine(new Pen(Brushes.Black, 3), new Point(margin, margin + ft.Height), new Point(pageSize.Width - margin, margin + ft.Height));
                point.Y += ft.Height;
                //获取表数据
                for (int i = minRow; i < maxRow; i++)
                {
                    if (i > (rowCount - 1)) break;
                    for (int j = 0; j < colCount; j++)
                    {
                        point.X = columns[j];
                        string colName = dataTable.Columns[j].ColumnName;
                        ft = GetFormattedText(dataTable.Rows[i][colName].ToString());
                        dc.DrawText(ft, point);
                    }
                    point.Y += ft.Height;
                }
            }
            return new DocumentPage(visual);
        }
        #endregion
    }
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        private MainWindow BaseWindow;
        public ObservableCollection<ReportData> Datalist = new ObservableCollection<ReportData>();
        public ObservableCollection<string> PatientGroupComboBoxItems = new ObservableCollection<string>();

        public Report(MainWindow mainWindow)
        {
            InitializeComponent();
            BaseWindow = mainWindow;
            this.ReportListBox.ItemsSource = Datalist;
            PatientGroupComboBox.ItemsSource = PatientGroupComboBoxItems;
            AddSortRule();
            InitPatientGroupComboBox();

            DatePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }


        public void print(FrameworkElement ViewContainer)
        {
            FrameworkElement objectToPrint = ViewContainer as FrameworkElement;
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            if ((bool)printDialog.ShowDialog().GetValueOrDefault())
            {
                Mouse.OverrideCursor = Cursors.Wait;
                PrintCapabilities capabilities =
                            printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                double dpiScale = 300.0 / 96.0;
                FixedDocument document = new FixedDocument();
                try
                {
                    
                    objectToPrint.Width = capabilities.PageImageableArea.ExtentWidth;
                    objectToPrint.UpdateLayout();
                    objectToPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size size = new Size(capabilities.PageImageableArea.ExtentWidth,
                                             objectToPrint.DesiredSize.Height);
                    objectToPrint.Measure(size);
                    size = new Size(capabilities.PageImageableArea.ExtentWidth,
                                    objectToPrint.DesiredSize.Height);
                    objectToPrint.Measure(size);
                    objectToPrint.Arrange(new Rect(size));
                    // Convert the UI control into a bitmap at 300 dpi
                    double dpiX = 300;
                    double dpiY = 300;
                    RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(
                        capabilities.PageImageableArea.ExtentWidth * dpiScale),
                        Convert.ToInt32(objectToPrint.ActualHeight * dpiScale),
                        dpiX, dpiY, PixelFormats.Pbgra32);
                    bmp.Render(objectToPrint);
                    // Convert the RenderTargetBitmap into a bitmap we can more readily use
                    PngBitmapEncoder png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(bmp));
                    System.Drawing.Bitmap bmp2;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        png.Save(memoryStream);
                        bmp2 = new System.Drawing.Bitmap(memoryStream);
                    }
                    document.DocumentPaginator.PageSize =
                      new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                    // break the bitmap down into pages
                    int pageBreak = 0;
                    int previousPageBreak = 0;
                    int pageHeight =
                         Convert.ToInt32(capabilities.PageImageableArea.ExtentHeight * dpiScale);
                    while (pageBreak < bmp2.Height - pageHeight)
                    {
                        pageBreak += pageHeight;  // Where we thing the end of the page should be
                        // Keep moving up a row until we find a good place to break the page
                        while (!IsRowGoodBreakingPoint(bmp2, pageBreak)) pageBreak--;
                        PageContent pageContent = generatePageContent(bmp2, previousPageBreak,
                          pageBreak, document.DocumentPaginator.PageSize.Width,
                          document.DocumentPaginator.PageSize.Height, capabilities); document.Pages.Add(pageContent); previousPageBreak = pageBreak;
                    }
                    // Last Page
                    PageContent lastPageContent = generatePageContent(bmp2, previousPageBreak,
                      bmp2.Height, document.DocumentPaginator.PageSize.Width,
                      document.DocumentPaginator.PageSize.Height, capabilities);
                    document.Pages.Add(lastPageContent);
                }
                finally
                {
                    // Scale UI control back to the original so we don't effect what is on the screen
                    objectToPrint.Width = double.NaN;
                    objectToPrint.UpdateLayout();
                    objectToPrint.LayoutTransform = new ScaleTransform(1, 1);
                    Size size = new Size(capabilities.PageImageableArea.ExtentWidth,
                                         capabilities.PageImageableArea.ExtentHeight);
                    objectToPrint.Measure(size);
                    objectToPrint.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth,
                                          capabilities.PageImageableArea.OriginHeight), size));
                    Mouse.OverrideCursor = null;
                }
                printDialog.PrintDocument(document.DocumentPaginator, "Print Document Name");
                objectToPrint.Width = BaseWindow.RightContentA.ActualWidth;
            }
            
        }
        private PageContent generatePageContent(System.Drawing.Bitmap bmp, int top, int bottom, double pageWidth, double PageHeight, System.Printing.PrintCapabilities capabilities)
        {
            FixedPage printDocumentPage = new FixedPage();
            printDocumentPage.Width = pageWidth;
            printDocumentPage.Height = PageHeight;
            int newImageHeight = bottom - top;
            System.Drawing.Bitmap bmpPage = bmp.Clone(new System.Drawing.Rectangle(0, top, bmp.Width, newImageHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Create a new bitmap for the contents of this page  
            Image pageImage = new Image();
            BitmapSource bmpSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpPage.GetHbitmap(),
                    IntPtr.Zero,
                   System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(bmp.Width, newImageHeight));
            pageImage.Source = bmpSource;
            pageImage.VerticalAlignment = VerticalAlignment.Top;
            // Place the bitmap on the page
            printDocumentPage.Children.Add(pageImage);
            PageContent pageContent = new PageContent();
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(printDocumentPage);
            FixedPage.SetLeft(pageImage, capabilities.PageImageableArea.OriginWidth);
            FixedPage.SetTop(pageImage, capabilities.PageImageableArea.OriginHeight);
            pageImage.Width = capabilities.PageImageableArea.ExtentWidth;
            pageImage.Height = capabilities.PageImageableArea.ExtentHeight;
            return pageContent;
        }
        private bool IsRowGoodBreakingPoint(System.Drawing.Bitmap bmp, int row)
        {
            double maxDeviationForEmptyLine = 1627500;
            bool goodBreakingPoint = false;
            if (rowPixelDeviation(bmp, row) < maxDeviationForEmptyLine)
                goodBreakingPoint = true;
            return goodBreakingPoint;
        }
        private double rowPixelDeviation(System.Drawing.Bitmap bmp, int row)
        {
            int count = 0;
            double total = 0;
            double totalVariance = 0;
            double standardDeviation = 0;
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0,
                   bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int stride = bmpData.Stride;
            IntPtr firstPixelInImage = bmpData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)firstPixelInImage;
                p += stride * row;  // find starting pixel of the specified row
                for (int column = 0; column < bmp.Width; column++)
                {
                    count++;  //count the pixels
                    byte blue = p[0];
                    byte green = p[1];
                    byte red = p[3];
                    int pixelValue = System.Drawing.Color.FromArgb(0, red, green, blue).ToArgb();
                    total += pixelValue;
                    double average = total / count;
                    totalVariance += Math.Pow(pixelValue - average, 2);
                    standardDeviation = Math.Sqrt(totalVariance / count);
                    //go to next pixel
                    p += 3;
                }
            }
            bmp.UnlockBits(bmpData);
            return standardDeviation;
        }



        private DataTable GetDataTable()
        {
            DataTable table = new DataTable("Data Table");
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType,
            // ColumnName and add to DataTable.   
            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "id";
            column.Caption = "编号";
            column.ReadOnly = true;
            column.Unique = true;
            table.Columns.Add(column);

            // Create new DataColumn, set DataType,
            // ColumnName and add to DataTable.   
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Name";
            column.Caption = "患者姓名";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the Column to the DataColumnCollection.
            table.Columns.Add(column);
            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Time";
            column.AutoIncrement = false;
            column.Caption = "时间";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);
            //Create third column
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Method";
            column.AutoIncrement = false;
            column.Caption = "方法";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);

            //Create forth column
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "BedId";
            column.AutoIncrement = false;
            column.Caption = "床号";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);


            //Create forth column
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Description";
            column.AutoIncrement = false;
            column.Caption = "描述";
            column.ReadOnly = false;
            column.Unique = false;
            // Add the column to the table.
            table.Columns.Add(column);



            // Make the ID column the primary key column.
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = table.Columns["id"];
            table.PrimaryKey = PrimaryKeyColumns;
            // Instantiate the DataSet variable.
            //dataSet = new DataSet();
            // Add the new DataTable to the DataSet.
            //dataSet.Tables.Add(table);
            // Create three new DataRow objects and add
            // them to the DataTable
            //table.Rows.Add(new []{DateTime.Now.ToString("yyyy-MM-dd")});
            for (int i = 0; i < Datalist.Count; i++)
            {
                row = table.NewRow();
                row["id"] = i+1;
                row["Name"] = Datalist[i].PatientName;
                row["Time"] = Datalist[i].Time;
                row["Method"] = Datalist[i].Method;
                row["BedId"] = Datalist[i].BedId;
                row["Description"] = Datalist[i].Description;
                table.Rows.Add(row);
            }
            return table;
        }

        private void ButtonPrint_OnClick(object sender, RoutedEventArgs e)
        {
            this.print(this);
            //PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            //    DataTable dt = GetDataTable();
            //    try
            //    {
            //        DataPaginator dp = new DataPaginator(dt, new Typeface("SimSun"), 16, 96*0.75, new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
            //        printDialog.PrintDocument(dp, DateTime.Now.ToString("yyyy-MM-dd"));
            //    }
            //    catch
            //    {
            //        MessageBox.Show("无法打印！");
            //    }
            //} 
            //PrintDialog printDialog = new PrintDialog();
            //printDialog.MaxPage = 100;
            //if (printDialog.ShowDialog() == true)
            //{

            //    this.ButtonPrint.Visibility = Visibility.Hidden;
            //    printDialog.PrintVisual(this, "");
            //    this.ButtonPrint.Visibility = Visibility.Visible;
            //} 

            ////创建一个PrintDialog的实例
            //System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog();
            ////创建一个PrintDocument的实例
            //PrintDocument docToPrint = new PrintDocument();
            ////将事件处理函数添加到PrintDocument的PrintPage事件中
            //docToPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(docToPrint_PrintPage);
            ////把PrintDialog的Document属性设为上面配置好的PrintDocument的实例
            //dlg.Document = docToPrint;
            ////根据用户的选择，开始打印
            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    docToPrint.Print();//开始打印
            //}
        }

        //设置打印机开始打印的事件处理函数
        private void docToPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //e.Graphics.DrawString("Hello, world!", new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Regular), System.Drawing.Brushes.Black, 100, 100);

        }

        public void InitPatientGroupComboBox()
        {
            try
            {
                PatientGroupComboBoxItems.Clear();
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    var list = patientGroupDao.SelectPatientGroup(condition);
                    foreach (var type in list)
                    {
                        var patientGroupData = new PatientGroupData
                        {
                            Id = type.Id,
                            Name = type.Name,
                            Description = type.Description
                        };
                        PatientGroupComboBoxItems.Add(patientGroupData.Name);
                    }
                }

                if (PatientGroupComboBoxItems.Count != 0)
                    PatientGroupComboBox.SelectedIndex = 0;
                /*if (PatientGroupComboBoxItems.Count != 0)
                    this.PatientGroupComboBox.SelectedValue = Basewindow.patientGroupPanel.ComboBoxPatientGroup.SelectedValue;*/
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:ComboBoxPatientGroup_OnInitialized exception messsage: " + ex.Message);
            }

        }

        private void PatientGroupComboBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            QueryPatients();

        }

        private long GetAreaIdByBedId(long bedid)
        {
            using (var bedDao = new BedDao())
            {
                var condition = new Dictionary<string, object>();
                condition["Id"] = bedid;
                var bedlist = bedDao.SelectBed(condition);
                if (bedlist.Count == 1)
                {
                    long areaId = bedlist[0].PatientAreaId;
                    return areaId;
                }

            }
            return -1;
        }

        private List<Patient> GetPatientsInFixedDay( List<Patient> srcList, long areaid )
        {

            List<Patient> patients = new List<Patient>();
            using (var scheduleTemplateDao = new ScheduleTemplateDao())
            {
                var condition = new Dictionary<string, object>();
                condition["DATE"] = DatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");// DateTime.Now.ToString("yyyy-MM-dd");
                var list22 = scheduleTemplateDao.SelectScheduleTemplate(condition);

                foreach (var type in list22)
                {
                    if (type.BedId == -1) continue;

                    bool bExist = true;
                    foreach (var patient in patients)
                    {
                        if (patient.PatientId != type.PatientId.ToString())
                        {
                            bExist = false;
                            break;
                        }
                    }
                    if (!bExist)
                    {
                        long areaid1 = GetAreaIdByBedId(type.BedId);
                        if(areaid == areaid1)
                        {
                            using (PatientDao patientDao1 = new PatientDao())
                            {
                                var condition2 = new Dictionary<string, object>();
                                condition2["ID"] = type.PatientId;
                                var list2 = patientDao1.SelectPatient(condition2);
                                patients.Add(list2[0]);
                            }
                        }
                        
                    }


                    

                }
            }

            foreach (var patient in srcList)
            {
                if (!patients.Contains(patient))
                {
                    patients.Add(patient);
                }
            }
            return patients;
        }

        private string GetPatientArea(List<PatientGroupPara> listinParas)
        {
            foreach (var patientGroupPara in listinParas)
            {
                if (patientGroupPara.Key.Equals("所属分区"))
                {
                    return patientGroupPara.Value;
                }
            }
            return "";

        }

        private void QueryPatientsByArea( string areaName )
        {
            string areaid = "";
            using (var patientAreaDao = new PatientAreaDao())
            {
                var condition = new Dictionary<string, object>();
                condition["Name"] = areaName;
                var arealist = patientAreaDao.SelectPatientArea(condition);

                areaid = arealist[0].Id.ToString();
            }

            using (BedDao bedDao = new BedDao())
            {
                Dictionary<string, object> condition = new Dictionary<string, object>();
                condition["PatientAreaId"] = areaid;
                var list = bedDao.SelectBed(condition);
                foreach (DAOModule.Bed bed in list)
                {
                    using (var scheduleTemplateDao = new ScheduleTemplateDao())
                    {

                        condition.Clear();
                        condition = new Dictionary<string, object>();
                        condition["BedId"] = bed.Id;
                        condition["DATE"] = DatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");// DateTime.Now.ToString("yyyy-MM-dd");
                        var list22 = scheduleTemplateDao.SelectScheduleTemplate(condition);
                        foreach (var type in list22)
                        {
                            if (type.BedId == -1) continue;

                            var rReportData = new ReportData();

                            rReportData.Id = type.Id;
                            using (PatientDao patientDao1 = new PatientDao())
                            {
                                var condition2 = new Dictionary<string, object>();
                                condition2["ID"] = type.PatientId;
                                var list2 = patientDao1.SelectPatient(condition2);
                                if ((list2 != null) && (list.Count > 0))
                                {
                                    rReportData.PatientName = list2[0].Name;
                                    rReportData.Description = list2[0].Description;
                                }
                            }

                            rReportData.ShiftWork = type.AmPmE;
                            rReportData.Method = type.Method;
                            /*if (type.BedId == -1)
                                rReportData.BedId = "";
                            else
                            {
                                rReportData.BedId = type.BedId.ToString();
                            }*/

                            using (var bedDao1 = new BedDao())
                            {
                                condition.Clear();
                                condition["Id"] = type.BedId;
                                var bedlist = bedDao1.SelectBed(condition);
                                if (bedlist.Count == 1)
                                {
                                    long areaId = bedlist[0].PatientAreaId;
                                    rReportData.BedId = bedlist[0].Name;
                                    using (var patientAreaDao = new PatientAreaDao())
                                    {
                                        condition.Clear();
                                        condition["Id"] = areaId;
                                        var arealist = patientAreaDao.SelectPatientArea(condition);
                                        if (arealist.Count == 1)
                                        {
                                            rReportData.Area = arealist[0].Name;
                                        }

                                    }
                                }

                            }

                            Datalist.Add(rReportData);

                        }
                    }
                }
            }
            UpdateGroupCount();
        }

        private void QueryPatients()
        {
            try
            {
                int index = PatientGroupComboBox.SelectedIndex;
                if (index == -1) return;
                using (var patientGroupDao = new PatientGroupDao())
                {
                    var condition = new Dictionary<string, object>();
                    condition["NAME"] = PatientGroupComboBoxItems[index];
                    var list = patientGroupDao.SelectPatientGroup(condition);
                    if (list.Count > 0)
                    {
                        using (var patientGroupParaDao = new PatientGroupParaDao())
                        {
                            var conditionpara = new Dictionary<string, object>();
                            conditionpara["GROUPID"] = list[0].Id;
                            var listpara = patientGroupParaDao.SelectPatientGroupPara(conditionpara);

                            
                            if (listpara.Count > 0)
                            {
                                using (var patientDao = new PatientDao())
                                {
                                    var patientlist = patientDao.SelectPatientSpecial(listpara);

                                    string areaid = GetPatientArea(listpara);
                                    if (!areaid.Equals(""))
                                    {
                                        Datalist.Clear();
                                        QueryPatientsByArea(areaid);
                                        return;
                                    }


                                    Datalist.Clear();
                                    foreach (var patient in patientlist)
                                    {
                                        using (var scheduleTemplateDao = new ScheduleTemplateDao())
                                        {

                                            condition.Clear();
                                            condition = new Dictionary<string, object>();
                                            condition["PatientId"] = patient.Id;
                                            condition["DATE"] = DatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");// DateTime.Now.ToString("yyyy-MM-dd");
                                            var list22 = scheduleTemplateDao.SelectScheduleTemplate(condition);
                                            foreach (var type in list22)
                                            {
                                                if (type.BedId == -1) continue;

                                                var rReportData = new ReportData();

                                                rReportData.Id = type.Id;
                                                using (PatientDao patientDao1 = new PatientDao())
                                                {
                                                    var condition2 = new Dictionary<string, object>();
                                                    condition2["ID"] = type.PatientId;
                                                    var list2 = patientDao1.SelectPatient(condition2);
                                                    if ((list2 != null) && (list.Count > 0))
                                                    {
                                                        rReportData.PatientName = list2[0].Name;
                                                        rReportData.Description = list2[0].Description;
                                                    }
                                                }

                                                rReportData.ShiftWork = type.AmPmE;
                                                rReportData.Method = type.Method;
                                                /*if (type.BedId == -1)
                                                    rReportData.BedId = "";
                                                else
                                                {
                                                    rReportData.BedId = type.BedId.ToString();
                                                }*/

                                                using (var bedDao = new BedDao())
                                                {
                                                    condition.Clear();
                                                    condition["Id"] = type.BedId;
                                                    var bedlist = bedDao.SelectBed(condition);
                                                    if (bedlist.Count == 1)
                                                    {
                                                        long areaId = bedlist[0].PatientAreaId;
                                                        rReportData.BedId = bedlist[0].Name;
                                                        using (var patientAreaDao = new PatientAreaDao())
                                                        {
                                                            condition.Clear();
                                                            condition["Id"] = areaId;
                                                            var arealist = patientAreaDao.SelectPatientArea(condition);
                                                            if (arealist.Count == 1)
                                                            {
                                                                rReportData.Area = arealist[0].Name;
                                                            }

                                                        }
                                                    }

                                                }

                                                //rReportData.Description = patient.Description;
                                                Datalist.Add(rReportData);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                UpdateGroupCount();

            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In Init.xaml.cs:Init_OnLoaded select patient exception messsage: " + ex.Message);
            }
        }


        private void AddSortRule()
        {
            SortDescriptionCollection sdc = ReportListBox.Items.SortDescriptions;
            //SortDescriptionCollection sdc = new SortDescriptionCollection();
            /*if (sdc.Count > 0)
            {
                SortDescription sd = sdc[0];
                sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                //判断此列当前的排序方式:升序0,倒序1,并取反进行排序。
                sdc.Clear();
            }*/
            sdc.Clear();
            sdc.Add(new SortDescription("ShiftWork", ListSortDirection.Ascending));
            sdc.Add(new SortDescription("Area", ListSortDirection.Ascending));
            sdc.Add(new SortDescription("BedId", ListSortDirection.Ascending));
            sdc.Add(new SortDescription("PatientName", ListSortDirection.Ascending));
            sdc.Add(new SortDescription("Method", ListSortDirection.Ascending));

            var sdc1 = PatientGroupComboBox.Items.SortDescriptions;
            sdc1.Clear();
            sdc1.Add(new SortDescription("", ListSortDirection.Ascending));


        }
        private void UpdateGroupCount()
        {
            LabelCount.Content = "总共" + Datalist.Count + "人";

        }
        private void Report_OnLoaded(object sender, RoutedEventArgs e)
        {
            QueryPatients();
            this.LabelDate.Content = DateTime.Now.ToString("yyyy-M-d dddd");
            
            /*try
            {
                using (var scheduleTemplateDao = new ScheduleTemplateDao())
                {
                    Datalist.Clear();
                    var condition = new Dictionary<string, object>();
                    condition["DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                    var list = scheduleTemplateDao.SelectScheduleTemplate(condition);
                    foreach (var type in list)
                    {
                        if (type.BedId != -1)
                            continue;

                        var rReportData = new ReportData();

                        rReportData.Id = type.Id;
                        using (PatientDao patientDao = new PatientDao())
                        {
                            var condition2 = new Dictionary<string, object>();
                            condition2["ID"] = type.PatientId;
                            var list2 = patientDao.SelectPatient(condition2);
                            if((list2!=null) && (list.Count>0))
                                rReportData.PatientName = list2[0].Name;
                        }
                        
                        rReportData.ShiftWork = type.AmPmE;
                        rReportData.Method = type.Method;


                        using (var bedDao = new BedDao())
                        {
                            condition.Clear();
                            condition["Id"] = type.BedId;
                            var bedlist = bedDao.SelectBed(condition);
                            if (bedlist.Count == 1)
                            {
                                long areaId = bedlist[0].PatientAreaId;
                                rReportData.BedId = bedlist[0].Name;
                                using (var patientAreaDao = new PatientAreaDao())
                                {
                                    condition.Clear();
                                    condition["Id"] = areaId;
                                    var arealist = patientAreaDao.SelectPatientArea(condition);
                                    if (arealist.Count == 1)
                                    {
                                        rReportData.Area = arealist[0].Name;
                                    }

                                }
                            }

                        }
                        rReportData.Description = type.Description;
                        Datalist.Add(rReportData);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.Log.WriteInfoConsole("In CInfectType.xaml.cs:ListViewCInfectType_OnLoaded exception messsage: " + ex.Message);
            }*/
        }

        private void DatePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryPatients();
            this.LabelDate.Content = DatePicker1.SelectedDate.Value.ToString("yyyy-M-d dddd");

        }
    }


    public class ReportData : INotifyPropertyChanged
    {
        private Int64 _id;
        private string _patientName;
        private string _time;
        private string _method;
        private string _bedId;
        private string _description;
        private string _shiftWork;
        private string _area;


        public ReportData()
        {
        }

        public string ShiftWork
        {
            get { return _shiftWork; }
            set
            {
                _shiftWork = value;
                OnPropertyChanged("ShiftWork");
            }
        }

        public string Area
        {
            get { return _area; }
            set
            {
                _area = value;
                OnPropertyChanged("Area");
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

        public string BedId
        {
            get { return _bedId; }
            set
            {
                _bedId = value;
                OnPropertyChanged("BedId");
            }
        }

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                _patientName = value;
                OnPropertyChanged("PatientName");
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        public string Method
        {
            get { return _method; }
            set
            {
                _method = value;
                OnPropertyChanged("Method");
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
