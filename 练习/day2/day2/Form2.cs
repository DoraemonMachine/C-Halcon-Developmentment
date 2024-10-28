using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Single_Form;
using HalconDotNet;
using System.Net;

namespace day2
{

    public partial class Form2 : Form
    {
        //调用单例
        Appvalue DL = Appvalue.GetAppvalue();
        //线程声明
        Thread thread = null;
        //定义全局一个接受图像变量
        HObject mImage = null;

        public Form2()
        {
            InitializeComponent();
            //设置窗口显示模式
            HOperatorSet.SetDraw(hWindowControl1.HalconWindow, "margin");
            //设置显示颜色
            HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
            //设置轮廓粗细
            HOperatorSet.SetLineWidth(hWindowControl1.HalconWindow, 2);
            //设置窗口文字大小
            DL.tool.set_display_font(hWindowControl1.HalconWindow, 18, "mono", "true", "false");
            
            
            //创建文件夹
            DL.seveparams.CreateDirectory(DL.seveparams.ConfigPath);
            //加载产品
            DL.seveparams.loadSystemini();

            if (DL.seveparams.NowProductName != "没有产品")
            {
                DL.Loadproduct(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
            }

        }

        /// <summary>
        /// 打开设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }


        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "启动")
            {
                button1.Text = "停止";
                DL.Camera.OpenCamera(hWindowControl1);
                thread = new Thread(ThreadImage);
                //设置成后台线程
                thread.IsBackground = true;
                //启动
                thread.Start();
               
            }
            else
            {
                button1.Text = "启动";
                //停止
                thread.Abort();
                DL.Camera.CloseCamera();
            }
        }

        /// <summary>
        /// 测量查找
        /// </summary>
        public void INspect()
        {
            try
            {
                if (DL.match.op)
                {
                    //定义成局部变量
                    HTuple refRow, refCol;
                    //查找模板
                    DL.match.FindMatch(mImage, DL.match.ho_ModelRegion, out DL.match.ho_TransContours,
                      DL.match.hv_ModelID, DL.match.hv_Start, DL.match.hv_Extent, DL.match.hv_MinScore, DL.match.hv_MatchNum,
                      DL.match.hv_MaxOverlap, out DL.match.hv_AlignmentHomMat2D, out DL.match.hv_Error);
                    //拟合圆
                    HOperatorSet.AffineTransPixel(DL.match.hv_AlignmentHomMat2D, DL.Measure.hv_ROIRows, DL.Measure.hv_ROICols, out refRow, out refCol);

                    DL.Measure.spoke(mImage, out DL.Measure.ho_Regions1, DL.Measure.hv_Elements,
                    DL.Measure.hv_DetectHeight, DL.Measure.hv_DetectWidth, DL.Measure.hv_Sigma, DL.Measure.hv_Threshold,
                    DL.Measure.hv_Transition, DL.Measure.hv_Select, refRow, refCol,
                    DL.Measure.hv_Direct, out DL.Measure.hv_ResultRow, out DL.Measure.hv_ResultColumn, out DL.Measure.hv_ArcType);

                    DL.Measure.pts_to_best_circle(out DL.Measure.ho_Circle, DL.Measure.hv_ResultRow, DL.Measure.hv_ResultColumn,
                    DL.Measure.hv_ActiveNum, DL.Measure.hv_ArcType, out DL.Measure.hv_RowCenter, out DL.Measure.hv_ColCenter,
                    out DL.Measure.hv_Radius, out DL.Measure.hv_StartPhi, out DL.Measure.hv_EndPhi, out DL.Measure.hv_PointOrder,
                    out DL.Measure.hv_ArcAngle);
                    DL.tool.Refresh(mImage, hWindowControl1);

                    HOperatorSet.DispObj(DL.Measure.ho_Circle, hWindowControl1.HalconWindow);
                    //通过像素单量得到实际直径
                    DL.tool.disp_message(hWindowControl1.HalconWindow, "直径" + "" + string.Format("{0:F4}", DL.Measure.hv_Scale * DL.Measure.hv_Radius.D * 2), "window", 20, 20, "red", "true");
                    //发给串口对象
                    DL.mySeeriaPort.Sendmessage("直径OK:" + " " + string.Format("{0:F4}", DL.Measure.hv_Scale * DL.Measure.hv_Radius.D * 2));
                    //发给网口
                    DL.network.Sendmessage("直径OK:" + " " + string.Format("{0:F4}", DL.Measure.hv_Scale * DL.Measure.hv_Radius.D * 2));
                }
                else
                {
                    MessageBox.Show("未设置");
                }
            }
            catch (HalconException ex)
            {

                MessageBox.Show(ex.GetErrorMessage());
            }

        }

        /// <summary>
        /// 线程
        /// </summary>
        public void ThreadImage()
        {
            
            while (true)
            {
                if (DL.mySeeriaPort.mQ.Count > 0)
                {
                    //跨线程
                    Invoke(new Action(() =>
                    {
                        textBox1.Text += DateTime.Now.ToString() + "....." + DL.mySeeriaPort.mQ.Peek() + "\r\n";
                        if (DL.mySeeriaPort.mQ.Dequeue() == "A1")
                        {
                            mImage = DL.Camera.MyGrabImage();
                            DL.tool.Refresh(mImage, hWindowControl1);
                            INspect();
                        }

                    }));
                }
                else if (DL.network.mQ.Count > 0)
                {
                    Invoke(new Action(() =>
                    {
                        textBox1.Text += DateTime.Now.ToString() + "....." + DL.network.mQ.Peek() + "\r\n";
                    if (DL.network.mQ.Dequeue() == "A1")
                    {
                        mImage = DL.Camera.MyGrabImage();
                        DL.tool.Refresh(mImage, hWindowControl1);
                        INspect();
                    }
                    }));
                }
                else
                {
                    Thread.Sleep(1000);
                    HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
                    DL.tool.disp_message(hWindowControl1.HalconWindow, "没有收到相关指令", "window", 20, 20, "red", "true");
                }
            }
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DL.seveparams.NowProductName!=null)
            {
                DL.seveparams.SaveSystemini();
            }
            
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "打开串口")
            {
                button2.Text = "关闭串口";
                DL.mySeeriaPort.Opencom();
            }
            else
            {
                button2.Text = "打开串口";
                DL.mySeeriaPort.Closecom();
            }
        }

        /// <summary>
        /// 打开网口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "打开网口")
            {
                button3.Text = "关闭网口";
                IPAddress ip = IPAddress.Parse(txtServer.Text);//ip地址,这是个静态方法不用实例直接点用
                int Port = int.Parse(txtPort.Text);//端口号
                DL.network.Open(ip,Port);
            }
            else
            {
                button3.Text = "打开网口";
                DL.network.Close();
            }
        }
    }
}
