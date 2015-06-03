using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using WpfApplication1.CustomUI;
using WpfApplication1.Utils;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);
        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32")]
        static extern bool OpenIcon(IntPtr hWnd);

        public static Byte[] Keys = { 0xD1 };

        private XmlDocument config_xmlDoc = null;

        /// <summary>
        /// 异或整数
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        private int EncryptInt(int times)
        {
            int result = 0;
            //int key = 235;
            //result = times ^ key;
            byte[] bb = new byte[4];

            bb[0] = (byte)(times);
            bb[1] = (byte)(times >> 8);
            bb[2] = (byte)(times >> 16);
            bb[3] = (byte)(times >> 24);

            for (int i = 0; i < bb.Length; i++)
            {
                //右移
                byte a = bb[i];
                byte b = bb[i];
                bb[i] = (byte)((a << (8 - 3)) | (b >> 3));

                int nPassLen = Keys.Length;
                //XOR
                bb[i] = (byte)(bb[i] ^ Keys[i % nPassLen]);
            }

            result = (int)(bb[0] | bb[1] << 8 | bb[2] << 16 | bb[3] << 24);

            return result;
        }

        private int DecryptInt(int times)
        {
            int result = 0;
            //int key = 235;
            //result = times ^ key;
            byte[] bb = new byte[4];

            bb[0] = (byte)(times);
            bb[1] = (byte)(times >> 8);
            bb[2] = (byte)(times >> 16);
            bb[3] = (byte)(times >> 24);

            for (int i = 0; i < bb.Length; i++)
            {
                int nPassLen = Keys.Length;

                //XOR
                bb[i] = (byte)(bb[i] ^ Keys[i % nPassLen]);

                //左移
                byte a = bb[i];
                byte b = bb[i];
                bb[i] = (byte)((a >> (8 - 3)) | (b << 3));
            }

            result = (int)(bb[0] | bb[1] << 8 | bb[2] << 16 | bb[3] << 24);

            return result;
        }

        public void Repopulate_Node_Of_Config_XMLDoc(string formname, string formnode, string nodename, string node_attr_tag, string node_attr_value)
        {

            if (config_xmlDoc != null)
            {
                XmlNodeList xmlNodeList = config_xmlDoc.SelectNodes(formname + "/" + formnode);
                bool b_found = false;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlAttribute atrribName = xmlNodeList[i].Attributes["name"];

                    if (atrribName == null)
                    {
                        XmlElement xe = (XmlElement)xmlNodeList[i];//转换类型

                        xe.SetAttribute("name", nodename);
                        if (!node_attr_tag.Equals("") /*&& !node_attr_value.Equals("")*/)
                        {
                            xe.SetAttribute(node_attr_tag, node_attr_value);
                        }

                        b_found = true;
                    }

                    else if (atrribName.Value.ToString().Equals(nodename))
                    {
                        XmlElement xe = (XmlElement)xmlNodeList[i];//转换类型

                        if (!node_attr_tag.Equals("") /*&& !node_attr_value.Equals("")*/)
                        {
                            xe.SetAttribute(node_attr_tag, node_attr_value);
                        }
                        b_found = true;
                    }
                }

                if (!b_found)
                {
                    XmlNode PRECISE_CFG = config_xmlDoc.SelectSingleNode(formname);
                    XmlElement xmlele = config_xmlDoc.CreateElement(formnode);
                    xmlele.SetAttribute("name", nodename);
                    if (!node_attr_tag.Equals("") /*&& !node_attr_value.Equals("")*/)
                    {
                        xmlele.SetAttribute(node_attr_tag, node_attr_value);
                    }
                    //xmlform.AppendChild(xmlele);

                    PRECISE_CFG.AppendChild(xmlele);
                }
                string preciseview_config_filename = System.Windows.Forms.Application.UserAppDataPath + "\\WpfApplication1\\lic.xml";
                preciseview_config_filename = preciseview_config_filename.Replace("\\\\", "\\");
                config_xmlDoc.Save(preciseview_config_filename);
            }
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            var other = FindWindow(null, "WpfApplicaiton1");
            if (other != IntPtr.Zero)
            {
                SetForegroundWindow(other);
                if (IsIconic(other))
                    OpenIcon(other);


                System.Windows.Application.Current.Shutdown();
                return;
            }

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

            //config_xmlDoc = new XmlDocument();
            //string configFilename = System.Windows.Forms.Application.UserAppDataPath + "\\WpfApplication1\\lic.xml";

            //if (File.Exists(configFilename))
            //{
            //    try
            //    {
            //        config_xmlDoc.Load(configFilename);
            //    }
            //    catch (Exception ee)
            //    {
            //        MessageBox.Show("Error occured:\n\n" + configFilename + " " + ee.Message);
            //        System.Windows.Application.Current.Shutdown();
            //        return;
            //    }

            //}
            //else
            //{
            //    if (!Directory.Exists(System.Windows.Forms.Application.UserAppDataPath + "\\WpfApplication1"))
            //        Directory.CreateDirectory(System.Windows.Forms.Application.UserAppDataPath + "\\WpfApplication1");
            //    FileStream config = File.Create(configFilename);
            //    byte[] rootNode = Encoding.UTF8.GetBytes("<System>\n</System>");
            //    config.Write(rootNode, 0, rootNode.Length);
            //    config.Close();

            //    config_xmlDoc.Load(configFilename);


            //    XmlNode SystemCfg = config_xmlDoc.SelectSingleNode("System");

            //    XmlNode setting = config_xmlDoc.SelectSingleNode("System/Setting");

            //    XmlElement xmlform = null;
            //    if (setting == null)
            //    {
            //        xmlform = config_xmlDoc.CreateElement("Setting");
            //    }
            //    else
            //    {
            //        xmlform = (XmlElement)setting;
            //    }

            //    XmlElement xmlele = config_xmlDoc.CreateElement("Item");
            //    xmlele.SetAttribute("name", "UT");
            //    int times = EncryptInt(0);
            //    xmlele.SetAttribute("value", times.ToString());
            //    xmlform.AppendChild(xmlele);

            //    SystemCfg.AppendChild(xmlform);

            //    config_xmlDoc.Save(configFilename);
            //}

            //List<string> macAddress = Utils.Utils.GetMacAddress();
            //bool fileNotExist = !File.Exists(ConstDefinition.RunningPath + "\\license.dat");

            //bool succ = false;

            ////Byte[] encryptBytes;
            //if (!fileNotExist)
            //{
            //    string str;
            //    using (var licfile = new StreamReader(ConstDefinition.RunningPath + "\\license.dat", true))
            //    {
            //        str = licfile.ReadLine();
            //    }
            //    foreach (string mac in macAddress)
            //    {
            //        string encryptStr = Utils.Utils.MD5Encrypt(mac+"0");

            //        if (str.Equals(encryptStr))
            //        {
            //            ConstDefinition.Runlevel = 0;
            //            succ = true;
            //            break;
            //        }
            //        encryptStr = Utils.Utils.MD5Encrypt(mac + "1");

            //        if (str.Equals(encryptStr))
            //        {
            //            ConstDefinition.Runlevel = 1;
            //            succ = true;
            //            break;
            //        }
            //    }
            //}

            //if (fileNotExist || !succ)
            //{
            //    // 试用次数
            //    bool flag = true;
            //    int times = 0;

            //    try
            //    {
            //        XmlDocument xmlDoc = this.config_xmlDoc;
            //        if (xmlDoc != null)
            //        {
            //            XmlNodeList xmlNodeList = xmlDoc.SelectNodes("System/Setting/Item");
            //            if (xmlNodeList != null && xmlNodeList.Count != 0)
            //            {
            //                for (int i = 0; i < xmlNodeList.Count; i++)
            //                {
            //                    XmlAttribute atrribName = xmlNodeList[i].Attributes["name"];
            //                    XmlAttribute atrribValue = xmlNodeList[i].Attributes["value"];

            //                    if ((atrribName != null) && (atrribValue != null) && (atrribName.Value.Equals("UT")))
            //                    {
            //                        times = Int32.Parse(atrribValue.Value);
            //                    }
            //                }
            //            }
            //        }

            //        times = DecryptInt(times);

            //        times++;

            //        if (times > ConstDefinition.TryVersionTimes)
            //        {
            //            times--;
            //            var messageBox1 = new RemindMessageBox1(true);
            //            messageBox1.remindText.Text = "您使用的是试用版，而且试用次数已用尽，如需继续使用请注册.";
            //            messageBox1.ShowDialog();
            //            Shutdown();
            //            return;
            //        }
            //        else if (times > 0)
            //        {
            //            int left = ConstDefinition.TryVersionTimes - times + 1;
            //            //var messageBox1 = new RemindMessageBox1(true);
            //            //messageBox1.remindText.Text = "您使用的是试用版，在您购买正式版之前还能试用 " + left + " 次.";
            //            //messageBox1.ShowDialog();
            //            times = EncryptInt(times);
            //            string formname = "System/Setting";
            //            Repopulate_Node_Of_Config_XMLDoc(formname, "Item", "UT", "value", times.ToString());
            //        }
            //        else
            //        {
            //            var messageBox1 = new RemindMessageBox1(true);
            //            messageBox1.remindText.Text = "您使用的是试用版，而且试用次数已用尽，如需继续使用请注册.";
            //            messageBox1.ShowDialog();
            //            Shutdown();
            //            return;
            //        }
            //    }
            //    catch(Exception exception)
            //    {
            //        MessageBox.Show(exception.Message);
            //        var messageBox1 = new RemindMessageBox1(true);
            //        messageBox1.remindText.Text = "您使用的是试用版，而且试用次数已用尽，如需继续使用请注册.";
            //        messageBox1.ShowDialog();
            //        Shutdown();
            //        return;
            //    }

            //}


            ////var s = new SplashScreen("SplashScreen.png");
            //var s = new SplashScreen("welcome.jpg");
            //s.Show(false, true);
            //s.Close(new TimeSpan(0, 0, 1));

            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }
    }
}
