using System.Drawing;

namespace WpfApplication1.ConfigModule
{
    /// <summary>
    ///   FacadeItems类，应用外观项
    /// </summary>
    public class FacadeItems
    {
        public Color PanelBackGroundColor { get; set; }//面板背景色
        public Color ButtonBackGroundColor { get; set; }//按钮背景色
        public Font Font { get; set; }//字体以及大小
        //TODO lots of items to be added...

        /// <summary>
        ///   构造函数，提供默认值
        /// </summary>
        public FacadeItems()
        {
            PanelBackGroundColor = Color.White;
            ButtonBackGroundColor = Color.Blue;
            Font = new Font("Times New Roman", 9f);
            //TODO lots of items to be added...
        }

        /// <summary>
        ///   重新加载新的外观文件，如果有缺失项，使用默认值
        /// </summary>
        public bool Reload(string fileName)
        {
            return true;
        }
    }

    /// <summary>
    ///   Facade类，应用外观类
    /// </summary>
    public class Facade
    {
        /// <summary>
        ///   外观类的单例实例
        /// </summary>
        private static Facade _facade;

        /// <summary>
        ///   保证外观单例线程安全的锁
        /// </summary>
        private static readonly object Lockhelper = new object();

        /// <summary>
        ///   成员变量，主要用来书写外观信息
        /// </summary>
        public  FacadeItems FacadeItems{ get; set;}

        /// <summary>
        ///   创建Facade实例，为了保证单例模式，将构造函数设为protected
        /// </summary>
        private Facade()
        {
            FacadeItems = new FacadeItems();
        }

        /// <summary>
        ///   返回单例Facade实例
        /// </summary>
        /// <returns>返回单例Facade实例类</returns>
        public static Facade GetInstance()
        {
            lock (Lockhelper)
            {
                if (_facade == null)
                {
                    _facade = new Facade();
                }
            }
            return _facade;
        }

        public bool Load(string fileName)
        {
            FacadeItems.Reload(fileName);
            DoApply();
            return true;
        }

        public bool Apply(string fileName)
        {
            FacadeItems.Reload(fileName);
            DoApply();
            return true;
        }

        /// <summary>
        ///  此函数负责将各个外观项应用到各个控件上
        /// </summary>
        public bool DoApply()
        {
            return true;
        }

        /// <summary>
        ///  保存用户新设计的外观文件
        /// </summary>
        public bool Save(string fileName)
        {
            return true;
        }
    }
}
