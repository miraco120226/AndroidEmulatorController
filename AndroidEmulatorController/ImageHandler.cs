using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidEmulatorController
{
    class TimerObj
    {
        public int ms;
        public bool doneFlag = false;
        BackgroundWorker bgw = new BackgroundWorker();

        public TimerObj(int s)
        {
            ms = s;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
        }

        public void start()
        {
            bgw.RunWorkerAsync();
        }

        public void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(ms);
            doneFlag = true;
        }
    }

    class ImageHandler
    {
        public static Bitmap imageResize(Bitmap originImage, Double times)
        {
            int width = Convert.ToInt32(originImage.Width * times);
            int height = Convert.ToInt32(originImage.Height * times);

            Bitmap resizedbitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(originImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, originImage.Width, originImage.Height), GraphicsUnit.Pixel);
            g.Dispose();

            return resizedbitmap;
        }
        /*
                 public static void findImg(Image<Gray, byte> target)
                {
                    Image<Gray, byte> screenshot = new Image<Gray, byte>(ScreenshotHandler.getWindowCapture());
                    Image<Gray, float> result = new Image<Gray, float>(screenshot.Width, screenshot.Height);
                    result = screenshot.MatchTemplate(target, TemplateMatchingType.CcorrNormed);
                    double min = 0, max = 0;
                    Point maxp = new Point(0, 0);
                    Point minp = new Point(0, 0);
                    CvInvoke.MinMaxLoc(result, ref min, ref max, ref minp, ref maxp);
                    Console.WriteLine(min + " " + max);
                    CvInvoke.Rectangle(screenshot, new Rectangle(maxp, new Size(target.Width, target.Height)), new MCvScalar(0, 0, 255), 3);
                    pictureBox2.Image = screenshot.ToBitmap();

                }
                */
        public static Point findImgLocation(string path)
        {
            Image<Gray, byte> target = new Image<Gray, byte>(path);
            Image<Gray, byte> screenshot = new Image<Gray, byte>(ScreenshotHandler.getWindowCapture());
            Image<Gray, float> result = new Image<Gray, float>(screenshot.Width, screenshot.Height);
            result = screenshot.MatchTemplate(target, TemplateMatchingType.CcorrNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(result, ref min, ref max, ref minp, ref maxp);
            Console.WriteLine(min + " " + max);

            screenshot.Dispose();
            result.Dispose();
            
            if (max<0.99)
            {
                throw new TargetNotFoundException();
            }
            return new Point(maxp.X + target.Width / 2, maxp.Y + target.Height / 2);
        }

        public static Point findImgLocation(string path, double second)
        {
            Point p = new Point();
            TimerObj to = new TimerObj(Convert.ToInt32(second *1000));
            to.start();
            while (p.Equals(new Point(0,0)) && !to.doneFlag)
            {
                try
                {
                    p = findImgLocation(path);
                }
                catch { }
                Thread.Sleep(100);
            }
            if (p.Equals(new Point(0, 0)))
            {
                throw new TargetNotFoundException();
            }
            return p;
        }

    }
}
