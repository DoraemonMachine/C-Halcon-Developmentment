using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Single_Form
{/// <summary>
/// 单例类
/// </summary>
    class Appvalue
    {
        //单例模式:只让类实例化一次
        //1.将需要单例化的类拿到单例类中声明
        //工具类
        public PublicTool tool = null;
        //匹配类
        public Match match = null;
        //测量类
        public Measure Measure = null;
        //相机类
        public Camera Camera = null;
        //参数保存类
        public Seveparams seveparams = null;
        //串口类
        public MySeeriaPort mySeeriaPort = null;
        //网口类
        public Network network = null;




        //声明一个单例类对象
        private static Appvalue mAppvalue = null;
        private static readonly object O = new object();

        public static Appvalue GetAppvalue()
        {
            lock (O)
            {
                if (mAppvalue == null)
                {
                    mAppvalue = new Appvalue();
                }
                return mAppvalue;
            }

        }

        //2.将构造函数私有化
        private Appvalue()
        {
            //工具类
             tool = new PublicTool();
            //匹配类
             match = new Match();
            //测量类
             Measure = new Measure();
            //相机类
             Camera = new Camera();
            //参数保存类
            seveparams = new Seveparams();
            //串口类
            mySeeriaPort = new MySeeriaPort();
            //网口类
            network = new Network();
        }

        /// <summary>
        /// 保存参数到文件夹中
        /// </summary>
        /// <param name="ProductPath"></param>
        public void SaveProduct(string ProductPath)
        {
           // Camera.SaveHacameParams(ProductPath);
            match.SaveMatcheParam(ProductPath);
            Measure.SaveMeasureParam(ProductPath);
        }

        /// <summary>
        ///读取文件中参数
        /// </summary>
        /// <param name="ProductPath"></param>
        public void Loadproduct(string ProductPath)
        {
            match.op = true;
           // Camera.HaCameDefaultParams(ProductPath);
            match.MatchDefaultParams(ProductPath);
            Measure.MeasureDefaultParam(ProductPath);
        }
    }
}
