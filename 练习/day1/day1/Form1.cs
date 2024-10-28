using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using Single_Form;

namespace day1
{
    public partial class Form1 : Form
    {
        PublicTool tool = new PublicTool();
        //定义全局一个接受图像变量
        HObject mImage = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //HOperatorSet.ReadImage();
            //控制变量 HTuple 整数 小数 数组 句柄 字符串 图形变量 HObject 图像 区域 轮廓
            //图形数据都用HObjet来声明，控制类型都使用HTuple关键字来声明
            //图形变量和控制变量在使用前必须进行赋值，未赋值使用，会提示报错

            int i = 10;
            double d = 10.1;
            string str = "str";
            int[] arr = { 1,2,3,4,5};

            //第一种
            ////HTuple hv_i = new HTuple(i);
            //HTuple hv_d = new HTuple(d);
            //HTuple hv_str = new HTuple(str);
            //HTuple hv_arr = new HTuple(arr);

            //第二种（常用）
            HTuple hv_i = i;
            HTuple hv_d = d;
            HTuple hv_str =str;
            HTuple hv_arr =arr;

            //HTuple转C#
            //第一种
            //int A = hv_i.I;
            //double D = hv_d.D;
            //string Str = hv_str.S;
            //int[] Arr = hv_arr.IArr;
            //第二种
            int A = hv_i;
            double D = hv_d;
            string Str = hv_str;
            int[] Arr = hv_arr;

            //转换变量
            string Astr =Convert.ToString(hv_i.I);
            //总结：htuple变量中装的变是什么类型，直接转成对应类型
            //如果非要进行数据类型转换，先调用属性将其转成对应的C#类型，然后借助convert类


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = @"D:\A机器视觉\Halcon\案例\7 案例分析图片\4.案例分析图片\现场班-测量\齿轮测量\2hao20150107190908.bmp";
            //申明一个空的图像变量
            //HObject Image;
            //out获取输出变量
            //HOperatorSet.ReadImage(out Image, path);
            ////声明变量
            //HTuple width, height;
            ////获取图片的长和宽
            //HOperatorSet.GetImageSize(Image, out width, out height);
            ////设置窗体宽高
            //HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, height, width);
            ////用publictool这个类来找到长和宽，就可以随着长度和宽度变换
            //tool.SetHwindowpart(ref hWindowControl1, Width,Height);
            ////显示，（图像，窗口句柄）
            //HOperatorSet.DispObj(Image, hWindowControl1.HalconWindow)


            //用这个可以打开文件来选择
            OpenFileDialog Dlg = new OpenFileDialog();
            //获取或设置当前文件名筛选器字符串
            Dlg.Filter = "(*.bmp;*.png;*.jpg;*.jpeg;*.tif)|*.bmp";
            //是否允许选择多个文件
            Dlg.Multiselect = false;
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                //实例化图形变量用来存储图像
                mImage = new HObject();
                //将图片路径存储到变量中
                HOperatorSet.ReadImage(out mImage, Dlg.FileName);
                //定义两个变量用来接受图像的宽度和高度
                HTuple Width, Heigth;
                //获取图像大小
                HOperatorSet.GetImageSize(mImage, out Width, out Heigth);
                //
                HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
                //
                tool.SetHwindowpart(ref hWindowControl1, Width, Heigth);
                //
                HOperatorSet.DispObj(mImage, hWindowControl1.HalconWindow);
                MessageBox.Show("打开成功");
            }   
        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //申明一个空的图像变量
            HObject Image;
            HObject Connection;
            HObject SelectShape;
            HObject ReduceDomain;
            HOperatorSet.Threshold(mImage,out Image, 29, 200);
            HOperatorSet.Connection(Image,out Connection);
            HOperatorSet.SelectShape(Connection,out SelectShape,"area","and",70000,80000);
            HOperatorSet.ReduceDomain(mImage, SelectShape, out ReduceDomain);
            //用publictool这个类来找到长和宽，就可以随着长度和宽度变换
            tool.SetHwindowpart(ref hWindowControl1, Width, Height);
            //显示，（图像，窗口句柄）
            HOperatorSet.DispObj(ReduceDomain, hWindowControl1.HalconWindow);
        }
    }
}
