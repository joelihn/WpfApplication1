using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication1.CustomUI
{
    /// <summary>
    /// newshutdown.xaml 的交互逻辑
    /// </summary>
    public partial class Newshutdown : Window
    {
        #region ExitWindows enum

        public enum ExitWindows : uint
        {
            EWX_LOGOFF = 0x00,
            EWX_SHUTDOWN = 0x01,
            EWX_REBOOT = 0x02,
            EWX_POWEROFF = 0x08,
            EWX_RESTARTAPPS = 0x40,
            EWX_FORCE = 0x04,
            EWX_FORCEIFHUNG = 0x10,
        }

        #endregion

        public MainWindow parent;

        public Newshutdown(MainWindow _parent)
        {
            InitializeComponent();
            parent = _parent;
            Top = 300;
            Left = 500;

            Owner = Application.Current.MainWindow;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ExitWindowsEx(ExitWindows uFlags,
                                                int dwReason);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle,
                                                    uint DesiredAccess,
                                                    out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true,
            CharSet = CharSet.Auto)]
        private static extern bool LookupPrivilegeValue(string lpSystemName,
                                                        string lpName,
                                                        out long lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
                                                         bool DisableAllPrivileges,
                                                         ref TOKEN_PRIVILEGES NewState,
                                                         int BufferLength,
                                                         IntPtr PreviousState,
                                                         IntPtr ReturnLength);


        public void AdjustToken()
        {
            const uint TOKEN_ADJUST_PRIVILEGES = 0x20;
            const uint TOKEN_QUERY = 0x8;
            const int SE_PRIVILEGE_ENABLED = 0x2;
            const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr procHandle = GetCurrentProcess();

            //取得令牌
            IntPtr tokenHandle;
            OpenProcessToken(procHandle,
                             TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
            //取得LUID
            var tp = new TOKEN_PRIVILEGES();
            tp.Attributes = SE_PRIVILEGE_ENABLED;
            tp.PrivilegeCount = 1;
            LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, out tp.Luid);
            //设定权限
            AdjustTokenPrivileges(
                tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        }


        private void TitleGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShutDownButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Log.WriteInfoLog("Shutdown fMRISystem at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            
            //设定强制关机
            AdjustToken();
            ExitWindowsEx(ExitWindows.EWX_POWEROFF | ExitWindows.EWX_FORCE, 0);
        }

        private void OnlyShutdownButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Log.WriteInfoLog("Shutdown fMRISystem at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
           
            Close();
            parent.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Nested type: TOKEN_PRIVILEGES

        [StructLayout(
            LayoutKind.Sequential, Pack = 1)]
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        #endregion
    }
}