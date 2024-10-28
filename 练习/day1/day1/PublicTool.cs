using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace Single_Form
{
    class PublicTool
    {
        /// <summary>
        /// 设置图片大小随窗口的变化而变化
        /// </summary>
        /// <param name="windowControl"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        public void SetHwindowpart(ref HWindowControl windowControl, int imageWidth, int imageHeight)
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
    }
}
