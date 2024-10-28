using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace Single_Form
{
    /// <summary>
    /// 相机类
    /// </summary>
    class Camera
    {
        //相机句柄
        HTuple hv_AcqHandle;
        //相机打开标志
        bool CameIsOK = false;
        //相机触发
        public bool Trrigle;
        //相机曝光时间
        public HTuple ExpTime;
        //相机增益
        public HTuple Gain;
        //图像变量
        HObject mImage;

        /// <summary>
        /// 打开相机
        /// </summary>
        public void OpenCamera(HWindowControl windowControl)
        {
            if (!CameIsOK)
            {
                HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb",
-1, "false", "default", "[0] XiaoMi Webcam", 0, -1, out hv_AcqHandle);
                //局部变量 
                HTuple isva;
                //检测句柄是否有效
                HOperatorSet.TupleIsValidHandle(hv_AcqHandle, out isva);
                if (isva == 1)
                {
                    CameIsOK = true;
                    mImage = MyGrabImage();
                    HTuple Width, Heigth;
                    //获取图像大小
                    HOperatorSet.GetImageSize(mImage, out Width, out Heigth);
                    //
                    SetHwindowpart(ref windowControl, Width, Heigth); 
                }
            }
        }

        /// <summary>
        /// 设置图片大小随窗口的变化而变化
        /// </summary>
        /// <param name="windowControl"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        private void SetHwindowpart(ref HWindowControl windowControl, int imageWidth, int imageHeight)
        {
            //int mWidth = windowControl.Width;
            //int mHeight = windowControl.Height;
            //if (mWidth > 0 && mHeight > 0)
            //{
            //    //获取windowcontrol控件的长宽比例
            //    double mScale_Window = Convert.ToDouble(mWidth) / Convert.ToDouble(mHeight);

            //    //获取图像的长宽比例
            //    double mScale_Image = Convert.ToDouble(imageWidth) / Convert.ToDouble(imageHeight);

            int mW = windowControl.Width;
            int mH = windowControl.Height;
            if (mW > 0 && mH > 0)
            {
                double mScale_Window = Convert.ToDouble(mW) / Convert.ToDouble(mH);
                double mScale_Image = Convert.ToDouble(imageWidth) / Convert.ToDouble(imageHeight);
                double row1, column1, row2, column2;
                int mH_1 = Convert.ToInt32(mW / mScale_Image);
                System.Drawing.Rectangle rect = windowControl.ImagePart;

                if (mH_1 > mH)
                {
                    row1 = 0;
                    row2 = imageHeight;
                    double mImage_w = imageHeight * mScale_Window - imageWidth;
                    double mD_Image_W = Math.Abs(mImage_w / 2.0);
                    column1 = mD_Image_W;
                    column2 = imageWidth + mD_Image_W;

                    rect.X = -(int)Math.Round(mD_Image_W);
                    rect.Y = 0;
                    rect.Height = imageHeight;
                    rect.Width = (int)Math.Round(imageHeight * mScale_Window);
                }
                else
                {
                    column1 = 0;
                    column2 = imageWidth;
                    double mImage_h = Convert.ToDouble(imageWidth) / mScale_Window - imageHeight;
                    double mD_Image_H = Math.Abs(mImage_h / 2.0);
                    row1 = mD_Image_H;
                    row2 = imageHeight + mD_Image_H;

                    rect.X = 0;
                    rect.Y = -(int)Math.Round(mD_Image_H);
                    rect.Height = (int)Math.Round(Convert.ToDouble(imageWidth) / mScale_Window);
                    rect.Width = imageWidth;
                }
                windowControl.ImagePart = rect;
            }
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        public void CloseCamera()
        {
            if (CameIsOK)
            {
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                CameIsOK = false;
            }
        }

        /// <summary>
        /// 设置外触发
        /// </summary>
        /// <param name="Trrigle"></param>
        public void SetTriggerMode(bool trrigle)
        {
            try
            {
                if (CameIsOK)
                {
                    if (trrigle)
                    {
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
                    }
                    else
                    {
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 获得外触发
        /// </summary>
        public void GetTriggerMode()
        {
            HTuple trrigle;
            if (CameIsOK)
            { 
                    HOperatorSet.GetFramegrabberParam(hv_AcqHandle, "TriggerMode", out trrigle);
                if (trrigle.S == "no")
                {
                    Trrigle = true;
                }
                else
                {
                    Trrigle = false;
                }

            }
        }

        /// <summary>
        /// 设置曝光值
        /// </summary>
        /// <param name="time"></param>
        public void SetExposureTime(double time)
        {
            if (CameIsOK)
            {
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", time);
            }
        }

        /// <summary>
        /// 获得曝光值
        /// </summary>
        public void GetExposureTime()
        {
            if (CameIsOK)
            {
                HOperatorSet.GetFramegrabberParam(hv_AcqHandle, "ExposureTime",out ExpTime);
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="gain"></param>
        public void SetGain(double gain)
        {
            if (CameIsOK)
            {
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "Gain", gain);
            }
        }

        /// <summary>
        /// 获得增益
        /// </summary>
        public void GetGain()
        {
            if (CameIsOK)
            {
                HOperatorSet.GetFramegrabberParam(hv_AcqHandle, "Gain", out Gain);
            }
        }

        /// <summary>
        /// 采集图像
        /// </summary>
        /// <returns></returns>
        public HObject MyGrabImage()
        {
            try
            {
                HObject ho_Image = null;
                if (CameIsOK)
                {
                    HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                    //判断是否有值
                    if (!ho_Image.IsInitialized())
                    {
                        ho_Image = null;
                    }
                }
                return ho_Image;
            }
            catch (HalconException ex)
            {
                MessageBox.Show(ex.GetErrorMessage());
                throw;
            }
        }

        /// <summary>
        /// 保存相机参数
        /// </summary>
        /// <param name="ProductPath"></param>
        public void SaveHacameParams(string ProductPath)
        {
            //如果这个ini不存在，那么会新建这个文件，并且，将写的节点的键和值都创建
            IniAPI.INIWriteValue(ProductPath + "\\" + "product.ini", "Camera", "ExposureTime", Convert.ToString(ExpTime.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "Camera", "TriggerMode", Convert.ToString(Trrigle));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "Camera", "Gain", Convert.ToString(Gain.D));
        }

        /// <summary>
        /// 加载相机默认参数
        /// </summary>
        public void HaCameDefaultParams(string ProductPath)
        {
            string str_ExposureTime = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "Camera", "ExposureTime", "100");
            string str_TriggerMode = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "Camera", "TriggerMode", "false");
            string str_Gain = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "Camera", "Gain", "1.0");

            ExpTime = new HTuple(str_ExposureTime);
            Trrigle = Convert.ToBoolean(str_TriggerMode);
            Gain = new HTuple(str_Gain);

            //如果相机时打状态，将从ini文件中读取的相机参数设置到相机中去
            if (CameIsOK)
            {
                SetExposureTime(Convert.ToInt32(str_ExposureTime));
                SetGain(Convert.ToDouble(str_Gain));
                SetTriggerMode(Trrigle);
            }
        }
    }
}
