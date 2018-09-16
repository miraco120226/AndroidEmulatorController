using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidEmulatorController
{
    public partial class Form1 : Form
    {
        const int fps = 60;
        public const string nox_path = "\""+ @"C:\Program Files (x86)\Nox\bin\"+"\"";
        
        int mouseDownX;
        int mouseDownY;

        public Form1()
        {
            InitializeComponent();
            Properties.Settings.Default.strclassName = "Qt5QWindowIcon";
            Properties.Settings.Default.strWindowName = "歐root";
            Properties.Settings.Default.Save();
            textBox1.Text = "歐root";
            textBox2.Text = "Qt5QWindowIcon";
            pictureBox1.Image = new Bitmap(1, 1);
            pictureBox1.Width = 360;
            pictureBox1.Height = 600;

            //backgroundWorker1.RunWorkerAsync();
            AdbHandler.setNoxPort(nox_path);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorker1.CancellationPending)
            {
                Thread.Sleep(1000/fps);
                backgroundWorker1.ReportProgress(0);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pictureBox1.Image.Dispose();
            try
            {
                Bitmap b = ScreenshotHandler.getWindowCapture();
                pictureBox1.Image = ImageHandler.imageResize(b,600.0/b.Height);
                b.Dispose();
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownX = e.X;
            mouseDownY = e.Y;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AdbHandler.tap(mouseDownX, mouseDownY);
        }




        private void printTime(string a, string b)
        {
            Console.WriteLine("time:" + (long.Parse(b) - long.Parse(a)));
            //string a = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            //string b = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            //printTime(a,b);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pad.start();
            //SendMessage(aaa, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread tap = new Thread(() => AdbHandler.swap(200, 300, 300, 400));
            tap.Start();
        }

        private void findImg(Image<Gray, byte> target)
        {
            Image<Gray, byte> img = new Image<Gray, byte>(ScreenshotHandler.getWindowCapture());
            Image<Gray, float> result = new Image<Gray, float>(img.Width, img.Height);
            result = img.MatchTemplate(target, TemplateMatchingType.CcorrNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(result, ref min, ref max, ref minp, ref maxp);
            Console.WriteLine(min + " " + max);
            CvInvoke.Rectangle(img, new Rectangle(maxp, new Size(target.Width, target.Height)), new MCvScalar(0, 0, 255), 3);
            pictureBox2.Image = img.ToBitmap();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdbHandler.tap(ImageHandler.findImgLocation("D://pad.png"));
        }
    }
}
