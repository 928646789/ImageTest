using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVGigE = MVAPI.MVGigE;
using MVImage = MVAPI.MVImage;
using MVSTATUS = MVAPI.MVSTATUS_CODES;
using System.Diagnostics;
using MVAPI;
using ZXing;

namespace 图像识别
{
    
    public partial class MV_E_EM : Form
    {
        /// <summary>
        /// 是否连续采集标识
        /// </summary>
        bool m_bRun = false;
        /// <summary>
        /// 图像句柄
        /// </summary>
        IntPtr m_hImage = IntPtr.Zero;
        /// <summary>
        /// 相机句柄
        /// </summary>
        IntPtr m_hCam = IntPtr.Zero;
        /// <summary>
        /// 像素格式
        /// </summary>
        MVAPI.MV_PixelFormatEnums m_PixelFormat;
        /// <summary>
        /// 采集图像数据委托
        /// </summary>
        MVAPI.MV_SNAPPROC StreamCBDelegate = null;
        /// <summary>
        /// 异步编程.用于将图像画到画布上面进行显示
        /// </summary>
        /// <returns></returns>
        public delegate int InvokeDraw();
        InvokeDraw invokeDraw = null;
        IAsyncResult ia = null;
        private List f1;
        public MV_E_EM(List f)
        {
            f1=f;
            InitializeComponent();
            f1.Hide();
        }

        private void MV_E_EM_FormClosed(object sender, FormClosedEventArgs e)
        {
            f1.Show();
        }

        private void butOpen_Click(object sender, EventArgs e)
        {
            int CamNum = 0;
            //获取相机个数
            MVSTATUS_CODES r = MVGigE.MVGetNumOfCameras(out CamNum);
            if (CamNum == 0)
            {
            MessageBox.Show("没有找到相机，请确认连接和相机IP设置");
            return;
            }
            //打开第0个相机
            r = MVGigE.MVOpenCamByIndex(0, out m_hCam);
            if (m_hCam == IntPtr.Zero)
            {
            if (r == MVSTATUS_CODES.MVST_ACCESS_DENIED)
            {
            MessageBox.Show("无法打开相机，可能正被别的软件控制");
            return;
            }
            }
            int w, h;
            //获取图像宽
            r = MVGigE.MVGetWidth(m_hCam, out w);
            if (CamNum == 0)
            {
            MessageBox.Show("取得图像宽度失败");
            return;
            }
            //获取图像高
            r = MVGigE.MVGetHeight(m_hCam, out h);
            if (CamNum == 0)
                {
            MessageBox.Show("取得图像高度失败");
            return;
            }
            //获取图像像素格式
            r = MVGigE.MVGetPixelFormat(m_hCam, out m_PixelFormat);
            if (CamNum == 0)
            {
            MessageBox.Show("取得图像颜色模式失败");
            return;
            }
            //创建图像
            if (m_PixelFormat == MVAPI.MV_PixelFormatEnums.PixelFormat_Mono8)
            m_hImage = MVAPI.MVImage.MVImageCreate(w, h, 8);
            else
            m_hImage = MVAPI.MVImage.MVImageCreate(w, h, 24);
            this.butOpen.Enabled = false;
            this.butGrab.Enabled = true;
            this.butClose.Enabled = false;
            
        }

        private void butGrab_Click(object sender, EventArgs e)
        {
            MVAPI.TriggerModeEnums mode;
            //获取相机触发模式
            MVGigE.MVGetTriggerMode(m_hCam, out mode);
            //如果相机不是连续采集模式
            if (mode != MVAPI.TriggerModeEnums.TriggerMode_Off)
            {
                //设置相机为连续采集模式
                MVGigE.MVSetTriggerMode(m_hCam, MVAPI.TriggerModeEnums.TriggerMode_Off);
            }
            //为StreamCBDelegate委托注册StreamCB方法
            StreamCBDelegate += new MVAPI.MV_SNAPPROC(StreamCB);
            //开始采集
            MVSTATUS_CODES r = MVGigE.MVStartGrab(m_hCam, StreamCBDelegate, this.Handle);
            this.butOpen.Enabled = false;
            this.butGrab.Enabled = false;
            this.butClose.Enabled = true;
            m_bRun = true;
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            //停止连续采集
            MVGigE.MVStopGrab(m_hCam);
            //关闭相机
            MVGigE.MVCloseCam(m_hCam);
            this.butOpen.Enabled = true;
            this.butGrab.Enabled = false;
            this.butClose.Enabled = false;
            m_bRun = false;
        }

        private void MV_E_EM_Load(object sender, EventArgs e)
        {
            MVSTATUS_CODES r;
            //函数库初始化
            r = MVGigE.MVInitLib();
            if (r != MVSTATUS_CODES.MVST_SUCCESS)
            {
                MessageBox.Show("函数库初始化失败！");
                return;
            }
            //查找连接计算机的相机
            r = MVGigE.MVUpdateCameraList();
            if (r != MVSTATUS_CODES.MVST_SUCCESS)
            {
                MessageBox.Show("查找连接计算机的相机失败！");
                return;
            }
            this.butOpen.Enabled = true;
            this.butGrab.Enabled = false;
            this.butClose.Enabled = false;

        }

        private void MV_E_EM_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bRun)
            {
                //停止采集
                MVGigE.MVStopGrab(m_hCam);
            }
            if (!this.butOpen.Enabled)
            {
                //关闭相机
                MVGigE.MVCloseCam(m_hCam);
            }
            //释放相机资源
            MVGigE.MVTerminateLib();
        }


        int DrawImage()
        {
            if (InvokeRequired)
            {
                if (ia == null)
                {
                    invokeDraw = DrawImage;
                    ia = this.BeginInvoke(invokeDraw);
                }
                else if (ia.IsCompleted)
                {
                    invokeDraw = DrawImage;
                    EndInvoke(ia);
                    ia = this.BeginInvoke(invokeDraw);
                }
                return 0;
            }
            if (m_hImage != IntPtr.Zero)
            {
                //将m_hImage图像画到this.Handle画布上面
                MVAPI.MVImage.MVImageDrawHwnd(m_hImage, this.Handle, 8, 40);
            }
            return 0;
        }

        int StreamCB(ref MVAPI.IMAGE_INFO pInfo, IntPtr UserVal)
            {
            //将原始帧转化为m_hImage图像格式
            MVGigE.MVInfo2Image(m_hCam, ref pInfo, m_hImage);
            pictureBox1.Image = ImageData2Bitmap(m_hImage);
            Bitmap remd = new Bitmap(pictureBox1.Image);
            BarcodeReader reader = new BarcodeReader();
            reader.Options.CharacterSet = "UTF-8";
            Result result = reader.Decode((remd));
            if (result != null)
            {
                MessageBox.Show(result.ToString());
            }
            //DrawImage();
            return 0;
            }

        public Bitmap ImageData2Bitmap(IntPtr hImage)
        {
            Bitmap bitmap = null;
            IntPtr ptrSrc = MVAPI.MVImage.MVImageGetBits(hImage);
            int nWidth = MVAPI.MVImage.MVImageGetWidth(hImage);
            int nHeight = MVAPI.MVImage.MVImageGetHeight(hImage);
            int bpp = MVImage.MVImageGetBPP(hImage);
            if (bpp == 8)
            {
                bitmap = new Bitmap(nWidth, nHeight, nWidth, System.Drawing.Imaging.
                PixelFormat.Format8bppIndexed, ptrSrc);
            }
            else
            {
                bitmap = new Bitmap(nWidth, nHeight, 3 * nWidth, System.Drawing.Imaging.
                PixelFormat.Format24bppRgb, ptrSrc);
            }
            return bitmap;
            }
    }
}
