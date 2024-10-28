using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using Single_Form;

namespace day2
{
    public partial class Form1 : Form
    {
        //调用单例
        Appvalue DL = Appvalue.GetAppvalue();
        //定义全局一个接受图像变量
        HObject mImage = null;
        //线程声明
        Thread thread = null;
        public Form1()
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

            //获取产品文件夹下的所以产品
            DL.seveparams.GetProductsName();
            if (Directory.Exists(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName))
            {
                //读取修改控件值
                ChangUI();
            }
        }
        /// <summary>
        /// 读取图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
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
                DL.tool.SetHwindowpart(ref hWindowControl1, Width, Heigth);
                //
                HOperatorSet.DispObj(mImage, hWindowControl1.HalconWindow);
                DL.Loadproduct(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
                //MessageBox.Show("打开成功");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown_MathNum_ValueChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 刷新屏幕
        /// </summary>
        public void Refresh()
        {
            HOperatorSet.ClearWindow(hWindowControl1.HalconWindow);
            HOperatorSet.DispObj(mImage, hWindowControl1.HalconWindow);
        }

        /// <summary>
        /// 显示结果图像
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="str"></param>
        public void ShowRsultImage(HObject Result, string str)
        {
            HOperatorSet.DispObj(Result, hWindowControl1.HalconWindow);
            DL.tool.disp_message(hWindowControl1.HalconWindow, str, "window", 20, 20, "red", "true");
        }

        /// <summary>
        /// 获取测量参数
        /// </summary>
        public void SetMeasureParams()
        {
            Invoke(new Action(() =>
            {
                //极性选择
                switch (comboBox_Polarity.Text)
                {
                    case "黑到白":
                        DL.Measure.hv_Transition = "positive";
                        break;
                    case "白到黑":
                        DL.Measure.hv_Transition = "negative";
                        break;
                    default:
                        DL.Measure.hv_Transition = "all";
                        break;
                }
                //点选择
                switch (comboBox_PointSelect.Text)
                {
                    case "第一个点":
                        DL.Measure.hv_Select = "first";
                        break;
                    case "最后一个点":
                        DL.Measure.hv_Select = "last";
                        break;
                    default:
                        DL.Measure.hv_Select = "max";
                        break;
                }
                DL.Measure.hv_Threshold = Convert.ToInt32(numericUpDown_Threshold.Value);
                DL.Measure.hv_Sigma = Convert.ToDouble(numericUpDown_Sigam.Value);
                DL.Measure.hv_Elements = Convert.ToDouble(numericUpDown_SearchNum.Value);

            }));

        }
        /// <summary>
        /// 获取匹配模块控件参数
        /// </summary>
        public void GetmatchParams()
        {
            //跨线程
            Invoke(new Action(() =>
            {
                DL.match.hv_Start = Convert.ToDouble(numericUpDown_StarAngle.Value);
                DL.match.hv_Extent = Convert.ToDouble(numericUpDown_ExtentAngle.Value);
                DL.match.hv_MinScore = Convert.ToDouble(numericUpDown_MathScore.Value);
                DL.match.hv_MatchNum = Convert.ToDouble(numericUpDown5.Value);
                DL.match.hv_MaxOverlap = Convert.ToDouble(numericUpDown_MathNum.Value);
            }));

        }

        /// <summary>
        /// 绘制模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (mImage != null)
            {
                //禁用按钮
                button2.Enabled = false;
                //1.确定调用方法
                //焦距控件
                hWindowControl1.Focus();
                //显示图像变量以及文字提醒
                DL.match.DrawMatch(out DL.match.ho_ModelRegion, hWindowControl1.HalconWindow);
                //IsInitialized()判断图像是否有初始化
                //2.确定方法是否执行成功
                if (DL.match.ho_ModelRegion.IsInitialized())
                {
                    Refresh();
                    //显示画出来的轮廓
                    ShowRsultImage(DL.match.ho_ModelRegion, "绘制模板成功");
                }
                else
                {
                    MessageBox.Show("绘制模板区域失败");
                }
            }
            else
            {
                MessageBox.Show("请打开图像");
            }

            button2.Enabled = true;
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //匹配参数
                GetmatchParams();
                DL.match.CreateMatch(mImage, DL.match.ho_ModelRegion, out DL.match.ho_ModelContours, DL.match.hv_Start, DL.match.hv_Extent, out DL.match.hv_ModelID);
                if (DL.match.ho_ModelContours.IsInitialized())
                {
                    Refresh();
                    //显示画出来的轮廓
                    ShowRsultImage(DL.match.ho_ModelContours, "创建模板成功");
                }
                else
                {
                    MessageBox.Show("创建模板失败");
                }
            }
            catch (HalconException ex)
            {
                //报halcon里的报错
                MessageBox.Show(ex.GetErrorMessage());
            }

        }

        /// <summary>
        /// 查找模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //匹配参数
                GetmatchParams();
                DL.match.FindMatch(mImage, DL.match.ho_ModelRegion, out DL.match.ho_TransContours,
              DL.match.hv_ModelID, DL.match.hv_Start, DL.match.hv_Extent, DL.match.hv_MinScore, DL.match.hv_MatchNum,
              DL.match.hv_MaxOverlap, out DL.match.hv_AlignmentHomMat2D, out DL.match.hv_Error);
                if (DL.match.ho_TransContours.IsInitialized())
                {
                    Refresh();
                    //显示画出来的轮廓
                    ShowRsultImage(DL.match.ho_TransContours, "查找模板成功");
                }
                else
                {
                    MessageBox.Show("查找模板失败");
                }
            }
            catch (HalconException ex)
            {
                //报halcon里的报错
                MessageBox.Show(ex.GetErrorMessage());
            }

        }

        /// <summary>
        /// 设置ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetRoI_Click(object sender, EventArgs e)
        {

            try
            {
                if (mImage != null)
                {
                    //焦距控件
                    hWindowControl1.Focus();
                    //测量参数
                    SetMeasureParams();
                    DL.Measure.draw_spoke3(mImage, out DL.Measure.ho_Regions, hWindowControl1.HalconWindow,
            DL.Measure.hv_Elements, DL.Measure.hv_DetectHeight, DL.Measure.hv_DetectWidth, out DL.Measure.hv_ROIRows,
            out DL.Measure.hv_ROICols, out DL.Measure.hv_Direct);
                    //2.确定方法是否执行成功，并且显示结果
                    if (DL.Measure.ho_Regions.IsInitialized())
                    {
                        //刷新图像
                        Refresh();
                        //显示图形变量以及文字提示
                        ShowRsultImage(DL.Measure.ho_Regions, "绘制ROI成功");
                    }
                    else
                    {
                        MessageBox.Show("绘制ROI失败");
                    }
                }
                else
                {
                    MessageBox.Show("请打开图像");
                }
            }
            catch (HalconException ex)
            {
                //报halcon里的报错
                MessageBox.Show(ex.GetErrorMessage());
                throw;
            }


        }

        /// <summary>
        /// 拟合圆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_FittingCircle_Click(object sender, EventArgs e)
        {
            try
            {
                //定义成局部变量
                HTuple refRow, refCol;
                //测量参数
                SetMeasureParams();
                //1.确定调用矩阵方法,确保参数有值，并且是仿射变换后的
                HOperatorSet.AffineTransPixel(DL.match.hv_AlignmentHomMat2D, DL.Measure.hv_ROIRows, DL.Measure.hv_ROICols, out refRow, out refCol);

                DL.Measure.spoke(mImage, out DL.Measure.ho_Regions1, DL.Measure.hv_Elements,
                DL.Measure.hv_DetectHeight, DL.Measure.hv_DetectWidth, DL.Measure.hv_Sigma, DL.Measure.hv_Threshold,
                DL.Measure.hv_Transition, DL.Measure.hv_Select, refRow, refCol,
                DL.Measure.hv_Direct, out DL.Measure.hv_ResultRow, out DL.Measure.hv_ResultColumn, out DL.Measure.hv_ArcType);

                DL.Measure.pts_to_best_circle(out DL.Measure.ho_Circle, DL.Measure.hv_ResultRow, DL.Measure.hv_ResultColumn,
                DL.Measure.hv_ActiveNum, DL.Measure.hv_ArcType, out DL.Measure.hv_RowCenter, out DL.Measure.hv_ColCenter,
                out DL.Measure.hv_Radius, out DL.Measure.hv_StartPhi, out DL.Measure.hv_EndPhi, out DL.Measure.hv_PointOrder,
                out DL.Measure.hv_ArcAngle);

                //2.确定方法是否执行成功，并且显示结果
                if (DL.Measure.ho_Circle.IsInitialized())
                {
                    //刷新图像
                    Refresh();
                    //显示图形变量以及文字提示
                    ShowRsultImage(DL.Measure.ho_Circle, "拟合圆成功");
                }
                else
                {
                    MessageBox.Show("拟合圆失败");
                }
            }
            catch (HalconException ex)
            {
                MessageBox.Show(ex.GetErrorMessage());
            }


        }

        /// <summary>
        /// 像素标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_PixelCalibration_Click(object sender, EventArgs e)
        {
            if (DL.Measure.hv_Radius != null)
            {
                DL.Measure.hv_Scale = Convert.ToDouble(numericUpDown_WuliD.Value) / (DL.Measure.hv_Radius.D * 2);
                textBox_pixD.Text = string.Format("{0:F4}", DL.Measure.hv_Scale);
            }
            else
            {
                MessageBox.Show("未按照步骤操作");
            }

        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (mImage != null && DL.match.hv_Error.I == 0)
                {
                    button4_Click(null, null);

                    if (DL.Measure.hv_Direct != null)
                    {
                        btn_FittingCircle_Click(null, null);
                        if (DL.Measure.hv_Radius != null)
                        {
                            decimal diameter = Convert.ToDecimal(DL.Measure.hv_Scale * DL.Measure.hv_Radius.D * 2);

                            //因为控件的类型是decimal，所以要转换
                            if (diameter < numericUpDown1.Value && diameter > numericUpDown2.Value)
                            {
                                Refresh();
                                HOperatorSet.DispObj(DL.Measure.ho_Circle, hWindowControl1.HalconWindow);
                                //通过像素单量得到实际直径
                                DL.tool.disp_message(hWindowControl1.HalconWindow, "直径OK:" + " " + string.Format("{0:F4}", DL.Measure.hv_Scale * DL.Measure.hv_Radius.D * 2), "window", 20, 20, "red", "true");
                                DL.match.op = true;
                            }
                            else
                            {
                                MessageBox.Show("直径不符合要求");
                            }
                        }
                    }
                }
            }
            catch (HalconException ex)
            {
                MessageBox.Show(ex.GetErrorMessage());
            }

        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            int i;
            if (button9.Text == "打开  相机")
            {
                button9.Text = "关闭  相机";
                DL.Camera.OpenCamera(hWindowControl1);
                mImage = DL.Camera.MyGrabImage();
                HTuple Width, Heigth;
                //获取图像大小
                HOperatorSet.GetImageSize(mImage, out Width, out Heigth);
                //
                DL.tool.SetHwindowpart(ref hWindowControl1, Width, Heigth);

                //获得相机参数，显示到控件上
                //DL.Camera.GetTriggerMode();
                //DL.Camera.GetExposureTime();
                //DL.Camera.GetGain();

                //checkBox_Trrigle.Checked = DL.Camera.Trrigle;
                //hScrollBar_Exposure.Value = (int)DL.Camera.ExpTime.D;
                //textBox1.Text = DL.Camera.ExpTime.D.ToString();
                //hScrollBar_Gain.Value = (int)DL.Camera.Gain.D;
                //textBox2.Text = DL.Camera.Gain.D.ToString();


            }
            else
            {
                button9.Text = "打开  相机";

                if (thread != null)
                {
                    thread.Abort();
                }
                DL.Camera.CloseCamera();
            }
        }

        /// <summary>
        /// 单帧采图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            mImage = DL.Camera.MyGrabImage();
            Refresh();
        }

        /// <summary>
        /// 连续采图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "连续采图")
            {
                button6.Text = "停止采图";
                thread = new Thread(ThreadImage);
                //设置成后台线程
                thread.IsBackground = true;
                //启动
                thread.Start();
            }
            else
            {
                button6.Text = "连续采图";
                //停止
                thread.Abort();
            }
        }

        /// <summary>
        /// 线程
        /// </summary>
        public void ThreadImage()
        {
            while (true)
            {
                mImage = DL.Camera.MyGrabImage();
                Refresh();
                //只有勾选上实时处理的时候我们才处理图像
                if (checkBox_realTime.Checked)
                {
                    //调用测量
                    button8_Click(null, null);
                }
            }
        }

        /// <summary>
        /// 外触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Trrigle_ClientSizeChanged(object sender, EventArgs e)
        {
            DL.Camera.SetTriggerMode(checkBox_Trrigle.Checked);
        }

        /// <summary>
        /// 曝光
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_Exposure_ValueChanged(object sender, EventArgs e)
        {
            DL.Camera.SetExposureTime(hScrollBar_Exposure.Value);
            textBox1.Text = hScrollBar_Exposure.Value.ToString();
        }

        /// <summary>
        /// 增益
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_Gain_ValueChanged(object sender, EventArgs e)
        {
            DL.Camera.SetGain(hScrollBar_Gain.Value);
            textBox2.Text = hScrollBar_Gain.Value.ToString();
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //停止
            if (thread != null)
            {
                thread.Abort();
            }

            DL.Camera.CloseCamera();

            DL.seveparams.SaveSystemini();
        }

        /// <summary>
        /// 新建产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_NewProdouct_Click(object sender, EventArgs e)
        {
            DL.seveparams.NowProductName = string.Format("{0}", textBox_newproduct.Text.Trim());

            if (!DL.seveparams.NowProductName.Equals("")) //判断输入的产品名称不为
            {
                if (!Directory.Exists(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName))
                {
                    if (MessageBox.Show("新建产品" + DL.seveparams.NowProductName, "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //创建产品文件夹
                        DL.seveparams.CreateDirectory(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
                        //获取产品文件夹下的所有产品名称
                        DL.seveparams.GetProductsName();
                        //跟新界面控件
                        ChangUI();
                    }
                }
                else
                {
                    MessageBox.Show("产品已经存在");
                }
            }
        }

        /// <summary>
        /// 保存产品数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否保持参数到产品" + DL.seveparams.NowProductName + "中", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetmatchParams();
                SetMeasureParams();
                DL.SaveProduct(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
            }

        }

        /// <summary>
        /// 修改界面参数
        /// </summary>
        public void ChangUI()
        {
            comboBox1.Items.Clear();
            //将获取产品的名称都放入combox控件中
            for (int i = 0; i < DL.seveparams.ProductNameList.Count; i++)
            {
                comboBox1.Items.Add(DL.seveparams.ProductNameList[i]);
            }
            //产品表
            textBox_newproduct.Text = DL.seveparams.NowProductName;
            comboBox1.Text = DL.seveparams.NowProductName;

            //相机界面参数修改
            //checkBox_Trrigle.Checked = DL.Camera.Trrigle;
            //曝光
            //hScrollBar_Exposure.Value = Convert.ToInt32(DL.Camera.ExpTime.D);
            //textBox1.Text = Convert.ToString(DL.Camera.ExpTime.D);
            //增益
            //textBox2.Text = Convert.ToString(DL.Camera.Gain.D);
            //hScrollBar_Gain.Value = Convert.ToInt32(DL.Camera.Gain.D);

            //匹配参数
            numericUpDown5.Value = Convert.ToDecimal(DL.match.hv_MaxOverlap.D);
            numericUpDown_MathScore.Value = Convert.ToDecimal(DL.match.hv_MinScore.D);
            numericUpDown_MathNum.Value = Convert.ToDecimal(DL.match.hv_MatchNum.D);
            numericUpDown_ExtentAngle.Value = Convert.ToDecimal(DL.match.hv_Extent.D);
            numericUpDown_StarAngle.Value = Convert.ToDecimal(DL.match.hv_Start.D);
            //测量参数
            numericUpDown_Sigam.Value = Convert.ToDecimal(DL.Measure.hv_Sigma.D);
            numericUpDown_Threshold.Value = Convert.ToDecimal(DL.Measure.hv_Threshold.D);
            numericUpDown_SearchNum.Value = Convert.ToDecimal(DL.Measure.hv_Elements.D);
            textBox_pixD.Text = Convert.ToString(DL.Measure.hv_Scale);

            string mTransition = "所有";
            string mSelect = "最强";

            switch (DL.Measure.hv_Transition.S)
            {
                case "positive":
                    mTransition = "黑到白";
                    break;
                case "negative":
                    mTransition = "白到黑";
                    break;
                default:
                    break;
            }
            switch (DL.Measure.hv_Select.S)
            {
                case "first":
                    mSelect = "第一个点";
                    break;
                case "last":
                    mSelect = "最后一个点";
                    break;
                default:
                    break;
            }
            comboBox_PointSelect.Text = mSelect;
            comboBox_Polarity.Text = mTransition;
        }

        /// <summary>
        /// 产品列表更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //误触发
            if (!comboBox1.Text.Equals(DL.seveparams.NowProductName))
            {
                textBox_newproduct.Text = comboBox1.Text;
                DL.seveparams.NowProductName = comboBox1.Text;
                //如果切换了产品，那么需要加载产品
                DL.Loadproduct(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
                //改变界面参数
                ChangUI();
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DeletProduct_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                DL.seveparams.DeleDirectory(DL.seveparams.ConfigPath + "\\" + comboBox1.Text);
                //获取文件夹中所有子文件夹，也就是产品名称
                DL.seveparams.GetProductsName();
                //改变界面参数
                ChangUI();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;
                    DL.Loadproduct(DL.seveparams.ConfigPath + "\\" + DL.seveparams.NowProductName);
                }
                else
                {
                    textBox_newproduct.Text = "";
                    comboBox1.Text = "";
                    MessageBox.Show("没有产品");
                }
            }
            else
            {
                MessageBox.Show("没有选择删除的产品");
            }


        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //SaveFileDialog SaveDg = new SaveFileDialog();
            //SaveDg.Title = "请选择图像保存路径";
            //SaveDg.Filter = "Image File(*.bmp)|*.bmp|图像文件(*.jpg)|*.jpg|Image File|*.tif|Image File(*.png)|*.txt|Image File(*.*)|*.*";
            // 获取当前时间并格式化为文件名的一部分
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // 将图像保存路径设置为 D 盘，并将文件名设为包含时间戳的名称
            string ImgSavePath = $@"D:\Images\{timestamp}.bmp"; // 你可以修改此路径
            try
            {

                //if (SaveDg.ShowDialog() == DialogResult.OK)
                //{
                    //string ImgSavePath = SaveDg.FileName;
                    if (!ImgSavePath.Equals(""))
                    {
                        HOperatorSet.WriteImage(mImage, "bmp", 0, ImgSavePath);
                        MessageBox.Show("保存成功！");
                    }
                //}

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存失败！" + ex.Message.ToString());
            }
        }

        private void numericUpDown_StarAngle_ValueChanged(object sender, EventArgs e)
        {

        }

        private void hScrollBar_Exposure_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
