using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace Single_Form
{
    /// <summary>
    /// 匹配类
    /// </summary>
    class Match
    {
        //声明变量
        //模板区域
        public HObject ho_ModelRegion;
        //模板轮廓
        public HObject ho_ModelContours;
        //起始角度
        public HTuple hv_Start;
        //角度范围
        public HTuple hv_Extent;
        //模板句柄
        public HTuple hv_ModelID;
        //仿射变换后的轮廓
        public HObject ho_TransContours;
        //最小匹配分数
        public HTuple hv_MinScore;
        //匹配数量
        public HTuple hv_MatchNum;
        //重叠度
        public HTuple hv_MaxOverlap;
        //仿射变换矩阵
        public HTuple hv_AlignmentHomMat2D;
        //错误码
        public HTuple hv_Error;
        //用于判断
        public bool op = false;

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="ho_ModelRegion"></param>
        /// <param name="ho_ModelContours"></param>
        /// <param name="hv_Start"></param>
        /// <param name="hv_Extent"></param>
        /// <param name="hv_ModelID"></param>
        public void CreateMatch(HObject ho_Image, HObject ho_ModelRegion, out HObject ho_ModelContours,
            HTuple hv_Start, HTuple hv_Extent, out HTuple hv_ModelID)
        {




            // Local iconic variables 

            HObject ho_TemplateImage;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_TemplateImage);
            hv_ModelID = new HTuple();
            try
            {


                ho_TemplateImage.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_ModelRegion, out ho_TemplateImage);
                //
                //Matching 01: Create the shape model
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ModelID.Dispose();
                    HOperatorSet.CreateShapeModel(ho_TemplateImage, 7, hv_Start.TupleRad(), hv_Extent.TupleRad()
                        , (new HTuple(0.589)).TupleRad(), (new HTuple("point_reduction_low")).TupleConcat(
                        "no_pregeneration"), "use_polarity", ((new HTuple(10)).TupleConcat(19)).TupleConcat(
                        10), 3, out hv_ModelID);
                }

                //Matching 01: Get the model contour for transforming it later into the image
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                ho_TemplateImage.Dispose();


                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TemplateImage.Dispose();


                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// 绘制模板
        /// </summary>
        /// <param name="ho_ModelRegion"></param>
        /// <param name="hv_WindowHandle"></param>
        public void DrawMatch(out HObject ho_ModelRegion, HTuple hv_WindowHandle)
        {



            // Local control variables 

            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelRegion);
            try
            {
                hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                HOperatorSet.DrawRectangle2(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Phi,
                    out hv_Length1, out hv_Length2);
                ho_ModelRegion.Dispose();
                HOperatorSet.GenRectangle2(out ho_ModelRegion, hv_Row1, hv_Column1, hv_Phi,
                    hv_Length1, hv_Length2);

                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// 查找模板
        /// </summary>
        /// <param name="ho_Image1"></param>
        /// <param name="ho_ModelRegion"></param>
        /// <param name="ho_TransContours"></param>
        /// <param name="hv_ModelID"></param>
        /// <param name="hv_Start"></param>
        /// <param name="hv_Extent"></param>
        /// <param name="hv_MinScore"></param>
        /// <param name="hv_MatchNum"></param>
        /// <param name="hv_MaxOverlap"></param>
        /// <param name="hv_AlignmentHomMat2D"></param>
        /// <param name="hv_Error"></param>
        public void FindMatch(HObject ho_Image, HObject ho_ModelRegion, out HObject ho_TransContours,
            HTuple hv_ModelID, HTuple hv_StartAngle, HTuple hv_RangeAngle, HTuple hv_MinScore,
            HTuple hv_MatchNum, HTuple hv_MaxOverlap, out HTuple hv_AlignmentHomMat2D, out HTuple hv_Error)
        {




            // Local iconic variables 

            HObject ho_ModelContours;

            // Local control variables 

            HTuple hv_IsValid = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Score = new HTuple(), hv_I = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Area = new HTuple(), hv_RefRow = new HTuple();
            HTuple hv_RefColumn = new HTuple(), hv_Length = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TransContours);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            hv_AlignmentHomMat2D = new HTuple();
            hv_Error = new HTuple();
            try
            {
                //新增参数
                hv_Error.Dispose();
                hv_Error = 0;
                //判断模板句柄是否有效
                hv_IsValid.Dispose();
                HOperatorSet.TupleIsValidHandle(hv_ModelID, out hv_IsValid);

                if ((int)(new HTuple(hv_IsValid.TupleLess(1))) != 0)
                {
                    //无效句柄
                    hv_Error.Dispose();
                    hv_Error = 1;
                    ho_ModelContours.Dispose();

                    hv_IsValid.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Angle.Dispose();
                    hv_Score.Dispose();
                    hv_I.Dispose();
                    hv_HomMat2D.Dispose();
                    hv_Area.Dispose();
                    hv_RefRow.Dispose();
                    hv_RefColumn.Dispose();
                    hv_Length.Dispose();

                    return;
                }
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                //Matching 02: Find the model
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindShapeModel(ho_Image, hv_ModelID, hv_StartAngle.TupleRad(),
                        hv_RangeAngle.TupleRad(), hv_MinScore, hv_MatchNum, hv_MaxOverlap, "least_squares",
                        (new HTuple(7)).TupleConcat(1), 0.75, out hv_Row, out hv_Column, out hv_Angle,
                        out hv_Score);
                }

                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(0))) != 0)
                {
                    //查找模板失败
                    hv_Error.Dispose();
                    hv_Error = 3;
                    ho_ModelContours.Dispose();

                    hv_IsValid.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_Angle.Dispose();
                    hv_Score.Dispose();
                    hv_I.Dispose();
                    hv_HomMat2D.Dispose();
                    hv_Area.Dispose();
                    hv_RefRow.Dispose();
                    hv_RefColumn.Dispose();
                    hv_Length.Dispose();

                    return;
                }
                //Matching 02: Transform the model contours into the detected positions
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_HomMat2D.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Angle.TupleSelect(hv_I), 0, 0,
                            out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(
                            hv_I), out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    ho_TransContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours,
                        hv_HomMat2D);
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_TransContours, HDevWindowStack.GetActive());
                    }
                    // stop(...); only in hdevelop
                }
                //
                //Matching 02: Code for alignment of e.g. measurements
                //Matching 02: Calculate a hom_mat2d for each of the matching results
                hv_Area.Dispose(); hv_RefRow.Dispose(); hv_RefColumn.Dispose();
                HOperatorSet.AreaCenter(ho_ModelRegion, out hv_Area, out hv_RefRow, out hv_RefColumn);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_AlignmentHomMat2D.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_AlignmentHomMat2D);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dTranslate(hv_AlignmentHomMat2D, -hv_RefRow, -hv_RefColumn,
                            out ExpTmpOutVar_0);
                        hv_AlignmentHomMat2D.Dispose();
                        hv_AlignmentHomMat2D = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dRotate(hv_AlignmentHomMat2D, hv_Angle.TupleSelect(hv_I),
                            0, 0, out ExpTmpOutVar_0);
                        hv_AlignmentHomMat2D.Dispose();
                        hv_AlignmentHomMat2D = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dTranslate(hv_AlignmentHomMat2D, hv_Row.TupleSelect(hv_I),
                            hv_Column.TupleSelect(hv_I), out ExpTmpOutVar_0);
                        hv_AlignmentHomMat2D.Dispose();
                        hv_AlignmentHomMat2D = ExpTmpOutVar_0;
                    }
                    //Matching 02: Insert your code using the alignment here, e.g. code generated by
                    //Matching 02: the measure assistant with the code generation option
                    //Matching 02: 'Alignment Method' set to 'Affine Transformation'.
                }
                hv_Length.Dispose();
                HOperatorSet.TupleLength(hv_AlignmentHomMat2D, out hv_Length);
                if ((int)(new HTuple(hv_Length.TupleLess(6))) != 0)
                {
                    //小于6，ROI放射变换矩阵创建失败
                    hv_Error.Dispose();
                    hv_Error = 2;
                }
                ho_ModelContours.Dispose();

                hv_IsValid.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Score.Dispose();
                hv_I.Dispose();
                hv_HomMat2D.Dispose();
                hv_Area.Dispose();
                hv_RefRow.Dispose();
                hv_RefColumn.Dispose();
                hv_Length.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ModelContours.Dispose();

                hv_IsValid.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Score.Dispose();
                hv_I.Dispose();
                hv_HomMat2D.Dispose();
                hv_Area.Dispose();
                hv_RefRow.Dispose();
                hv_RefColumn.Dispose();
                hv_Length.Dispose();

                throw HDevExpDefaultException;
            }
        }

        /// <summary>
        /// 保存匹配参数
        /// </summary>
        public void SaveMatcheParam(string ProductPath)
        {
            //保存模板句柄和图像
            //HTuple Is_Valid = new HTuple();
            //HOperatorSet.TupleIsValidHandle(hv_ModelId, out Is_Valid);
            if (hv_ModelID != null)
            {
                if (ho_ModelRegion.IsInitialized())
                {
                    //保存模板句柄
                    HOperatorSet.WriteShapeModel(hv_ModelID, ProductPath + "\\" + "hv_ModelID.shm");
                    //保存模板图像
                    HOperatorSet.WriteRegion(ho_ModelRegion, ProductPath + "\\" + "ho_ModelRegion.hobj");
                }
            }
            //保存匹配控制参数
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_StartAngle", Convert.ToString(hv_Start.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_Angle", Convert.ToString(hv_Extent.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_NumMatches", Convert.ToString(hv_MatchNum.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_MaxOverlap", Convert.ToString(hv_MaxOverlap.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_MinScore", Convert.ToString(hv_MinScore.D));
            IniAPI.INIWriteValue(ProductPath + "\\product.ini", "MatchParam", "hv_Error", Convert.ToString(hv_Error.D));
        }

        /// <summary>
        /// 加载默认匹配参数
        /// </summary>
        public void MatchDefaultParams(string ProductPath)
        {
            //如果文件存在有我就读取，没有就不管
            if (System.IO.File.Exists(ProductPath + "\\" + "hv_ModelID.shm") && System.IO.File.Exists(ProductPath + "\\" + "ho_ModelRegion.hobj"))
            {
                //保存模板句柄
                HOperatorSet.ReadShapeModel(new HTuple(ProductPath + "\\" + "hv_ModelID.shm"), out hv_ModelID);
                //保存模板图像
                HOperatorSet.ReadRegion(out ho_ModelRegion, new HTuple(ProductPath + "\\" + "ho_ModelRegion.hobj"));
            }
            //如果ini文件中节点对应的键对应的值，有的话加载，没有话，我们自己设置的
            string str_StartAngle = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_StartAngle", "-45");
            string str_RangeAngle = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_RangeAngle", "90");
            string str_NumMatches = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_NumMatches", "1");
            string str_MaxOverlap = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_MaxOverlap", "0.6");
            string str_MinScore = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_MinScore", "0.5");
            string str_Error = IniAPI.INIGetStringValue(ProductPath + "\\product.ini", "MatchParam", "hv_Error", "0");

            hv_Start = Convert.ToDouble(str_StartAngle);
            hv_Extent = Convert.ToDouble(str_RangeAngle);
            hv_MatchNum = Convert.ToDouble(str_NumMatches);
            hv_MinScore = Convert.ToDouble(str_MinScore);
            hv_MaxOverlap = Convert.ToDouble(str_MaxOverlap);
            hv_Error = Convert.ToInt32(str_Error);
        }
    }

}

