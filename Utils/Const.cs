using System.Windows.Forms;

namespace WpfApplication1.Utils
{
    /// <summary>
    /// ����������
    /// </summary>
    public static class ConstDefinition
    {

        //���ݿ��ļ�·��
        public static string DbStr;
        //���ݿ�����
        public static string DbPassword;
        //Storage SCP·��
        public static string StorageScpDir;
        //ͼ������·��
        public static string ImageDataDir;

        //��������Ŀ¼
        public static string RunningPath = Application.StartupPath;

        public static int TryVersionTimes = 30;

        public static int Runlevel = 0;
    }
}