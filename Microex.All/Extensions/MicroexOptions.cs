using Microex.All.AliyunOss;
using Microex.All.UEditor;

namespace Microex.All.Extensions
{
    public class MicroexOptions
    {
        //定义一个私有的静态全局变量来保存该类的唯一实例
        private static MicroexOptions _instance;
        //定义一个只读静态对象
        //且这个对象是在程序运行时创建的
        private static readonly object syncObject = new object();

        /// <summary>
        /// 构造函数必须是私有的
        /// 这样在外部便无法使用 new 来创建该类的实例
        /// </summary>
        private MicroexOptions()
        { }
        public AliyunOssOptions AliyunOssOptions { get; set; }
        public UEditorOptions UEditorOptions { get; set; }
        public static MicroexOptions Instance {
            get
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        //第二重 singleton == null
                        if (_instance == null)
                        {
                            _instance = new MicroexOptions();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
