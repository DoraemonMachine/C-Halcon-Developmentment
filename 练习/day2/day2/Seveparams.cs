using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Single_Form
{
    /// <summary>
    /// 数据保存类
    /// </summary>
    class Seveparams
    {
        //获取当前程序exe的路径,用来产生产品文件夹
        public string ExePath = AppDomain.CurrentDomain.BaseDirectory;
        //设置Config文件夹路径
        public string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config";
        //当前加载产品名称
        public string NowProductName;
        //定义一个System.Ini文件
        public string SysteminiPath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\System.ini";
        //list
        public List<string> ProductNameList = new List<string>();

        /// <summary>  Directory 文件夹类
        /// 创建文件夹
        /// 如果传入的是config路径，那么就会生成Config路径
        /// 如果传入的是产品路径，那么就会生成产品文件夹
        /// </summary>
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path)) //确定给定路径是否有现有文件夹
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 从ini文件中加载产品名称参数
        /// </summary>
        public void loadSystemini()
        {
            NowProductName = IniAPI.INIGetStringValue(SysteminiPath, "Product", "NowProductName", "");
        }

        /// <summary>
        /// 将产品名称参数保存到 ini文件中
        /// </summary>
        public void SaveSystemini()
        {
            if (NowProductName != null)
            {
                IniAPI.INIWriteValue(SysteminiPath, "Product", "NowProductName", NowProductName);
            }
            else
            {
                IniAPI.INIWriteValue(SysteminiPath, "Product", "NowProductName", "未创建产品");
            }
        }

        /// <summary>
        /// 获取文件夹中所有子文件夹，也就是产品名称
        /// </summary>
        public void GetProductsName()
        {
            //获取指定目录中的子目录的名称（包括其路径）,一个包含指定路径中子目录的完整名称（包括路径）的数组。
            string[] mProductsName = Directory.GetDirectories(ConfigPath);
            //先清空list集合
            ProductNameList.Clear();
            foreach (var item in mProductsName)
            {
                //包含指定路径中子目录的完整名称,然后我们同过分割的方式得到一个数组
                //取最后一个就是产品的名称
                string[] mName = item.Split('\\');
                ProductNameList.Add(mName[mName.Length - 1]);
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="path"></param>
        public void DeleDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
            }
        }
    }
}
