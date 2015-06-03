using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace WpfApplication1.Utils
{
    public class ListFilesComparer : IComparer
    {
        #region IComparer Members

        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(y, x));
        }

        #endregion
    }

    public class ListFilesComparer2 : IComparer
    {
        #region IComparer Members

        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(x, y));
        }

        #endregion
    }

    /// <summary> Static methods as global functions </summary>
    /// <remarks>
    /// Set of tool functions.
    /// </remarks>
    /// 
    public static class Utils
    {
          //调用API函数
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        /// <summary> Format message box </summary>
        /// <remarks>
        /// Add the information of all parameters which will be changed caused by the modification.
        /// </remarks>
        /// 
        /// <param name="paraNames"> List of parameter's name </param>
        /// <param name="paraOldVal"> List of old value </param>
        /// <param name="paraNewVal"> List of new value </param>
        /// <returns> Message contained modification information </returns>
        public static string FormatMsgBox(List<string> paraNames, List<string> paraOldVal, List<string> paraNewVal)
        {
            string outputStr = null;
            outputStr = string.Format("You changed parameter {0} from {1} to {2}\n This modification will cause\n",
                                      paraNames[0], paraOldVal[0], paraNewVal[0]);

            for (int i = 1; i < paraNames.Count; i++)
            {
                string paraString = null;
                paraString = string.Format("the parameter {0} will be changed from {1} to {2}\n",
                                           paraNames[i], paraOldVal[i], paraNewVal[i]);
                outputStr += paraString;
            }

            string confirmStr;
            confirmStr = string.Format("Do you want to continue?");
            outputStr += confirmStr;

            // Message to be displayed.[htang, 08-11-27]
            return outputStr;
        }


        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="sourceDirName">源文件目录名</param>
        /// <param name="destDirName">目标文件目录名</param>
        /// <param name="copySubDirs">是否拷贝子目录</param>
        public static void DirectoryCopy(
            string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void delay(long delay_Time)
        {

            long stop_Value = 0;
            long start_Value = 0;
            long freq = 0;
            long n = 0;

            QueryPerformanceFrequency(ref freq); //获取CPU频率
            long count = delay_Time*freq/1000;
            QueryPerformanceCounter(ref start_Value); //获取初始前值

            while (n < count) //不能精确判定
            {
                QueryPerformanceCounter(ref stop_Value); //获取终止变量值
                n = stop_Value - start_Value;
            }
        }


        public static void SplitPath(string path, out string dir, out string name, out string ext)
        {
            int i = path.Length;
            while (i > 0)
            {
                //E:\JoeLab\XmlToCrypt\XmlToCryptTool\bin\Release
                char ch = path[i - 1];
                if (ch == '\\' || ch == '/' || ch == ':') break;
                i--;
            }
            dir = path.Substring(0, i);
            name = path.Substring(i);
            string[] temp = name.Split('.');
            ext = temp[temp.Length - 1];
        }

        public static void ListFiles(FileSystemInfo info, ref ArrayList fn)
        {
            if (!info.Exists)
            {
                fn = null;
                return;
            }

            var dir = info as DirectoryInfo;
            //不是目录
            if (dir == null)
            {
                fn = null;
                return;
            }

            FileSystemInfo[] files = dir.GetFileSystemInfos();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i] as FileInfo;
                //是文件
                if (file != null)
                {
                    fn.Add(file.Directory + "\\" + file.Name);
                }
                    //对于子目录，进行递归调用
                else
                    ListFiles(files[i], ref fn);
            }
        }

        public static void ListFiles(FileSystemInfo info, ref ArrayList fn, string ext)
        {
            if (!info.Exists)
            {
                fn = null;
                return;
            }

            var dir = info as DirectoryInfo;
            //不是目录
            if (dir == null)
            {
                fn = null;
                return;
            }

            FileSystemInfo[] files = dir.GetFileSystemInfos();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i] as FileInfo;
                //是文件
                if (file != null)
                {
                    if (file.Name.Trim().EndsWith(ext))
                    {
                        fn.Add(file.Directory + "\\" + file.Name);
                    }
                }
                    //对于子目录，进行递归调用
                else
                    ListFiles(files[i], ref fn, ext);
            }
        }


        public static bool CopyDirectory(string sourcePath, string destinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
                destinationPath = destinationPath.EndsWith(@"\") ? destinationPath : destinationPath + @"\";

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destinationPath) == false)
                        Directory.CreateDirectory(destinationPath);

                    foreach (string fls in Directory.GetFiles(sourcePath))
                    {
                        var flinfo = new FileInfo(fls);
                        flinfo.CopyTo(destinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(sourcePath))
                    {
                        var drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, destinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>Get the count of bits per pixel from a PixelFormat value</summary>
        /// <param name="format">One of the PixelFormat members beginning with "Format..." - all others are not supported</param>
        /// <returns>bit count</returns>
        public static Int16 ConvertPixelFormatToBitCount(PixelFormat format)
        {
            String formatName = format.ToString();
            if (formatName.Substring(0, 6) != "Format")
            {
                throw new Exception("Unknown pixel format: " + formatName);
            }

            formatName = formatName.Substring(6, 2);
            Int16 bitCount = 0;
            if (Char.IsNumber(formatName[1]))
            {
                //16, 32, 48
                bitCount = Int16.Parse(formatName);
            }
            else
            {
                //4, 8
                bitCount = Int16.Parse(formatName[0].ToString());
            }

            return bitCount;
        }

        /// <summary>Returns a PixelFormat value for a specific bit count</summary>
        /// <param name="bitCount">count of bits per pixel</param>
        /// <returns>A PixelFormat value for [bitCount]</returns>
        public static PixelFormat ConvertBitCountToPixelFormat(int bitCount)
        {
            String formatName;
            if (bitCount > 16)
            {
                formatName = String.Format("Format{0}bppRgb", bitCount);
            }
            else if (bitCount == 16)
            {
                formatName = "Format16bppRgb555";
            }
            else
            {
                // < 16
                formatName = String.Format("Format{0}bppIndexed", bitCount);
            }

            return (PixelFormat) Enum.Parse(typeof (PixelFormat), formatName);
        }

        public static string MakeTheFileName(string dir, string fileName)
        {
            int number = 1;
            string newFileNmae = fileName;
            while (File.Exists(dir + "\\" + newFileNmae))
            {
                newFileNmae = fileName + "_" + number;
                number++;
            }

            if (number == 1)
                newFileNmae = fileName;
            return newFileNmae;
        }

        public static List<string> GetMacAddress()
        {
            const int minMacAddrLength = 12;
            var macAddress = new List<string>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (!String.IsNullOrEmpty(tempMac) && tempMac.Length >= minMacAddrLength)
                {
                    macAddress.Add(tempMac);
                }
            }
            return macAddress;
        }

        /// <summary>
        /// MD5 encrypt a string and
        /// return as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string valueString)
        {
            string ret = String.Empty;
            //Setup crypto
            var md5Hasher = new MD5CryptoServiceProvider();
            //Get bytes
            byte[] data = Encoding.ASCII.GetBytes(valueString);
            //Encrypt
            data = md5Hasher.ComputeHash(data);
            //Convert from byte 2 hex
            for (int i = 0; i < data.Length; i++)
            {
                ret += data[i].ToString("x2").ToLower();
            }
            //Return encoded string
            return ret;
        }

        /// <summary>   
        /// 根据“精确进程名”结束进程   
        /// </summary>   
        /// <param name="strProcName">精确进程名</param>   
        public static void KillProc(string strProcName)
        {
            try
            {
                //精确进程名  用GetProcessesByName   
                foreach (Process p in Process.GetProcessesByName(strProcName))
                {
                    if (!p.CloseMainWindow())
                    {
                        p.Kill();
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>   
        /// 根据 模糊进程名 结束进程   
        /// </summary>   
        /// <param name="strProcName">模糊进程名</param>   
        public static void KillProcA(string strProcName)
        {
            try
            {
                //模糊进程名  枚举   
                //Process[] ps = Process.GetProcesses();  //进程集合   
                foreach (Process p in Process.GetProcesses())
                {
                    Console.WriteLine(p.ProcessName + p.Id);

                    if (p.ProcessName.IndexOf(strProcName) > -1) //第一个字符匹配的话为0，这与VB不同   
                    {
                        if (!p.CloseMainWindow())
                        {
                            p.Kill();
                        }
                    }
                }
            }
            catch
            {
            }
        }


        /// <summary>   
        /// 判断是否包含此字串的进程   模糊   
        /// </summary>   
        /// <param name="strProcName">进程字符串</param>   
        /// <returns>是否包含</returns>   
        public static bool SearchProcA(string strProcName)
        {
            try
            {
                //模糊进程名  枚举   
                //Process[] ps = Process.GetProcesses();  //进程集合   
                foreach (Process p in Process.GetProcesses())
                {
                    Console.WriteLine(p.ProcessName + p.Id);

                    if (p.ProcessName.IndexOf(strProcName) > -1) //第一个字符匹配的话为0，这与VB不同   
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>   
        /// 判断是否存在进程  精确   
        /// </summary>   
        /// <param name="strProcName">精确进程名</param>   
        /// <returns>是否包含</returns>   
        public static bool SearchProc(string strProcName)
        {
            try
            {
                //精确进程名  用GetProcessesByName   
                Process[] ps = Process.GetProcessesByName(strProcName);
                if (ps.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}