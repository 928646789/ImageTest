using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using ZXing;
using AForge.Math;
using System.Threading;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using AForge.Math.Geometry;
using GlyphRecognitionProto;


namespace 图像识别
{
    public partial class Form1 : Form
    {
        
        private const int CameraWidth = 640;  // constant Width
        private const int CameraHeight = 480; // constant Height

        private FilterInfoCollection cameras; //Collection of Cameras that connected to PC
        private VideoCaptureDevice device; //Current chosen device(camera) 
        private Dictionary<string, string> cameraDict = new Dictionary<string, string>();
        private Pen pen = new Pen(Brushes.Orange, 4); //is used for drawing rectangle around card
        private Font font = new Font("Tahoma", 15, FontStyle.Bold); //is used for writing string on card

        public Form1()
        {

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;//允许多线程操作界面控件
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            numericUpDown1.Value = 50;
            this.cameras = new FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
            int i = 1;
            foreach (AForge.Video.DirectShow.FilterInfo camera in this.cameras)
            {
                if (!this.cameraDict.ContainsKey(camera.Name))
                    this.cameraDict.Add(camera.Name, camera.MonikerString);
                else
                {
                    this.cameraDict.Add(camera.Name + "-" + i.ToString(), camera.MonikerString);
                    i++;
                }
            }
            this.comboBox1.DataSource = new List<string>(cameraDict.Keys); //Bind camera names to combobox

            if (this.comboBox1.Items.Count == 0)
                button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开")
            {
                comboBox2.Enabled = false;
                this.button1.Text = "关闭";
                this.device = new VideoCaptureDevice(this.cameraDict[comboBox1.SelectedItem.ToString()]);
                this.device.NewFrame += new NewFrameEventHandler(videoNewFrame);
                this.device.DesiredFrameSize = new Size(CameraWidth, CameraHeight);

                device.Start(); //Start Device
            }
            else
            {
                comboBox2.Enabled = true;
                this.StopCamera();
                button1.Text = "打开";
                this.pictureBox1.Image = null;
            }
        }

        private void StopCamera()
        {
            if (device != null && device.IsRunning)
            {
                device.SignalToStop(); //stop device
                device.WaitForStop();
                device = null;
            }
        }
        bool wait = false;
        private void videoNewFrame(object sender, NewFrameEventArgs args)
        {

            Bitmap temp = args.Frame.Clone() as Bitmap;
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    
                    break;
                case 1:
                    temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                    break;
                case 2:
                    temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                    temp = new Threshold((int)numericUpDown1.Value).Apply(temp);
                    break;
                case 3:
                    temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                    AForge.Imaging.Filters.CannyEdgeDetector filter = new AForge.Imaging.Filters.CannyEdgeDetector();
                    filter.ApplyInPlace(temp);
                    break;
                case 4:
                    for (int i = 0; i < (int)numericUpDown2.Value; i++)
                        temp = new Dilatation().Apply(temp);
                    break;
                case 5:
                    for (int i = 0; i < (int)numericUpDown2.Value;i++ )
                        temp = new Erosion().Apply(temp);
                    break;
                case 6:
                    temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                    AForge.Imaging.Filters.BradleyLocalThresholding filter1 = new AForge.Imaging.Filters.BradleyLocalThresholding();
                    filter1.ApplyInPlace(temp);
                    break;

            }
            
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    {
                        BarcodeReader reader = new BarcodeReader();
                        reader.Options.CharacterSet = "UTF-8";
                        Result result = reader.Decode(temp);
                        if (result != null)
                        {
                            if (wait == false)
                            {
                                MessageBox.Show(result.ToString());
                                wait = true;
                            }


                        }
                        else
                            wait = false;
                        break;
                    }
                case 1:
                    {
                        /*Bitmap pImg = MakeGrayscale3((Bitmap)temp);
                        using (ZBar.ImageScanner scanner = new ZBar.ImageScanner())
                        {
                            scanner.SetConfiguration(ZBar.SymbolType.None, ZBar.Config.Enable, 0);
                            scanner.SetConfiguration(ZBar.SymbolType.CODE39, ZBar.Config.Enable, 1);
                            scanner.SetConfiguration(ZBar.SymbolType.CODE128, ZBar.Config.Enable, 1);

                            List<ZBar.Symbol> symbols = new List<ZBar.Symbol>();
                            symbols = scanner.Scan((System.Drawing.Image)pImg);
                            if (symbols != null && symbols.Count > 0)
                            {
                                string result = string.Empty;
                                symbols.ForEach(s => result += s.Data);
                                if (wait == false)
                                {
                                    MessageBox.Show(result);
                                    wait = true;
                                }
                            }
                            else
                                wait = false;
                        }*/
                        break;
                    }
                case 2:
                    {
                        break;
                    } 
                case 3:
                    break;
            }
            this.pictureBox1.Image = ResizeBitmap(temp);
            

        }

        private Bitmap ResizeBitmap(Bitmap bmp)
        {
            ResizeBilinear resizer = new ResizeBilinear(pictureBox1.Width, pictureBox1.Height);

            return resizer.Apply(bmp);
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
               new float[][] 
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "处理中";           
            switch(comboBox4.SelectedIndex)
            {

                case 0:
                    {
                        Bitmap temp = (Bitmap)pictureBox1.Image;
                        OilPainting filter3 = new OilPainting(10);
                        // apply the filter
                        filter3.ApplyInPlace(temp);
                        this.pictureBox2.Image = ResizeBitmap(temp);
                        break;
                    }
                case 1:
                    {
                        Bitmap temp = (Bitmap)pictureBox1.Image;
                        temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                        DifferenceEdgeDetector edgeDetector = new DifferenceEdgeDetector();
                        temp = edgeDetector.Apply(temp);
                        temp = new Threshold((int)numericUpDown1.Value).Apply(temp);

                        //FillHoles filter2 = new FillHoles();
                        //filter2.MaxHoleHeight = MinHeight;
                        //filter2.MaxHoleWidth = MaxWidth;
                        //filter2.CoupledSizeFiltering = false;
                        // apply the filter
                        //temp = filter2.Apply(temp);
                        //HorizontalRunLengthSmoothing hrls = new HorizontalRunLengthSmoothing(40);
                        // apply the filter
                        //hrls.ApplyInPlace(temp);

                        /*AForge.Imaging.Filters.BlobsFiltering filter = new AForge.Imaging.Filters.BlobsFiltering();
                        // 设置过滤条件（对象长、宽至少为70）
                        filter.CoupledSizeFiltering = true;
                        filter.MaxWidth = (int)numericUpDown3.Value;
                        filter.MaxHeight = (int)numericUpDown4.Value;
                        filter.MinWidth = (int)numericUpDown5.Value;
                        filter.MinHeight = (int)numericUpDown6.Value;
                        filter.ApplyInPlace(temp);*/



                        BlobCounter blobCounter = new BlobCounter();

                        blobCounter.MinHeight = 32;
                        blobCounter.MinWidth = 32;
                        blobCounter.FilterBlobs = true;
                        blobCounter.ObjectsOrder = ObjectsOrder.Size;

                        // 4 - find all stand alone blobs
                        blobCounter.ProcessImage(temp);
                        Blob[] blobs = blobCounter.GetObjectsInformation();
                        SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                        
                        List<IntPoint> corners = null;
                        List<IntPoint> corners2 = null;
                        for (int i = 0, n = blobs.Length; i < n; i++)
                        {
                            List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                            // does it look like a quadrilateral ?
                            if (shapeChecker.IsQuadrilateral(edgePoints, out corners))
                            {
                                // get edge points on the left and on the right side
                                List<IntPoint> leftEdgePoints, rightEdgePoints;
                                blobCounter.GetBlobsLeftAndRightEdges(blobs[i],
                                    out leftEdgePoints, out rightEdgePoints);
                                listBox1.DataSource =leftEdgePoints;
                                listBox2.DataSource=rightEdgePoints;
                            }
                            
                        }
                        //listBox1.DataSource = corners;
                        //listBox2.DataSource = corners2;
                        this.pictureBox1.Image = temp;
                        break;
                    }
                    
                case 2:
                    {
                        Bitmap bt2 = new Bitmap(@"D:\TCL条码\截图01.bmp");
                        Bitmap bt1 = new Bitmap(@"D:\TCL条码\截图03.bmp");
                        //Bitmap bt1 = new Bitmap(pictureBox2.Image);
                        ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.80f);
                        //基于一定的相似性阈值获得匹配块
                        TemplateMatch[] matchings = tm.ProcessImage(bt1, bt2);
                        BitmapData data = bt1.LockBits(
                            new Rectangle(0, 0, bt1.Width, bt1.Height),
                            ImageLockMode.ReadWrite, bt1.PixelFormat);
                        foreach (TemplateMatch m in matchings)
                        {
                            Drawing.Rectangle(data, m.Rectangle, Color.Red);
                        }
                        bt1.UnlockBits(data);
                        pictureBox2.Image = bt1;
                        break;
                    }
                case 3:
                    {
                        Bitmap bt2 = new Bitmap(@"D:\TCL条码\Canny算法.png");
                        AForge.Imaging.Filters.BlobsFiltering filter = new AForge.Imaging.Filters.BlobsFiltering();
                        // 设置过滤条件（对象长、宽至少为70）
                        filter.CoupledSizeFiltering = true;
                        filter.MaxWidth = (int)numericUpDown3.Value;
                        filter.MaxHeight = (int)numericUpDown4.Value;
                        filter.MinWidth = (int)numericUpDown5.Value;
                        filter.MinHeight = (int)numericUpDown6.Value;
                        filter.ApplyInPlace(bt2);
                        pictureBox1.Image = bt2;
                        byte[] RESULT = BitmapToBytes(bt2);
                        break;
                    }
                case 4:
                    {
                        Bitmap temp = (Bitmap)pictureBox1.Image;
                        temp = new Grayscale(0.2125, 0.7154, 0.0721).Apply(temp);
                        AForge.Imaging.Filters.CannyEdgeDetector filter = new AForge.Imaging.Filters.CannyEdgeDetector();
                        filter.ApplyInPlace(temp);
                        pictureBox2.Image = temp;
                        break;
                    }

            }
            
            button2.Text = "处理";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }


        System.Drawing.Point start; //画框的起始点
        System.Drawing.Point end;//画框的结束点<br data-filtered="filtered">
        bool blnDraw;//判断是否绘制<br data-filtered="filtered">
        System.Drawing.Point tempEndPoint;
        Rectangle rect;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
            Invalidate();
            blnDraw = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnDraw)
            {
                if (e.Button != MouseButtons.Left)//判断是否按下左键
                    return;
                tempEndPoint = e.Location; //记录框的位置和大小
                rect.Location = new System.Drawing.Point(
                Math.Min(start.X, tempEndPoint.X),
                Math.Min(start.Y, tempEndPoint.Y));
                rect.Size = new Size(
                Math.Abs(start.X - tempEndPoint.X),
                Math.Abs(start.Y - tempEndPoint.Y));
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            
            Bitmap bit = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            using (Graphics g = Graphics.FromImage(bit))
            {
                g.DrawImage(pictureBox1.Image, new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height), new Rectangle(start.X, start.Y, Math.Abs(start.X - tempEndPoint.X), Math.Abs(start.Y - tempEndPoint.Y)), GraphicsUnit.Pixel);
            }
            pictureBox2.Image = bit;
            blnDraw = false; //结束绘制
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (blnDraw)
            {
                if (pictureBox1.Image != null)
                {
                    if (rect != null && rect.Width > 0 && rect.Height > 0)
                    {
                        e.Graphics.DrawRectangle(new Pen(Color.Red, 3), rect);//重新绘制颜色为红色
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List list1 = new List(this);
            list1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "D:\\TCL条码";
            openFileDialog1.ShowDialog();
            string name=System.IO.Path.GetFileName(openFileDialog1.FileName);
            pictureBox1.Image = new Bitmap(System.IO.Path.GetDirectoryName(openFileDialog1.FileName)+"\\"+name);
        }

        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }   
    }
}
