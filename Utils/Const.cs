using System.Windows.Forms;

namespace WpfApplication1.Utils
{
    /// <summary>
    /// 常量类型类
    /// </summary>
    public static class ConstDefinition
    {

        //数据库文件路径
        public static string DbStr;
        //数据库密码
        public static string DbPassword;
        //Storage SCP路径
        public static string StorageScpDir;
        //图像数据路径
        public static string ImageDataDir;

        //程序运行目录
        public static string RunningPath = Application.StartupPath;

        public static int TryVersionTimes = 30;

        public static int Runlevel = 0;
    }
}