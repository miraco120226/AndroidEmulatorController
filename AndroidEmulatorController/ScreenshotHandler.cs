using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AndroidEmulatorController
{
    class ScreenshotHandler
    {
        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        private static extern int SendMessage(
         int hWnd,   // handle to destination window
         int Msg,    // message
         int wParam, // first message parameter
         int lParam // second message parameter
        );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);// handle to DC
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
        IntPtr hdc, // handle to DC
        int nWidth, // width of bitmap, in pixels
        int nHeight // height of bitmap, in pixels
        );
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(
        IntPtr hdc, // handle to DC
        IntPtr hgdiobj // handle to object
        );
        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(
        IntPtr hdc // handle to DC
        );
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
        IntPtr hwnd, // Window to copy,Handle to the window that will be copied.
        IntPtr hdcBlt, // HDC to print into,Handle to the device context. 
        UInt32 nFlags // Optional flags,Specifies the drawing options. It can be one of the following values. 
        );
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
        IntPtr hwnd // Window to copy,Handle to the window that will be copied.
        );

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_CLOSE = 0xF060;
        const int nox_topHeight = 30;
        const int width = 360;
        const int height = 600;

        public static Bitmap getWindowCapture()
        {
            string strclassName = Properties.Settings.Default.strclassName;
            string strWindowName = Properties.Settings.Default.strWindowName;
            int aaa = FindWindow(strclassName, strWindowName);
            return  GetWindowCapture(new IntPtr(aaa));
        }

        public static Bitmap GetWindowCapture(IntPtr hWnd)
        {
            IntPtr hscrdc = GetWindowDC(hWnd); //返回hWnd参数所指定的窗口的设备环境。
            var windowRect = Rectangle.Empty;
            GetWindowRect(hWnd, ref windowRect);
            int width = windowRect.Width - windowRect.X;
            int height = windowRect.Height - windowRect.Y;
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, width, height);//该函数创建与指定的设备环境相关的设备兼容的位图
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);//该函数创建一个与指定设备兼容的内存设备上下文环境（DC）
            SelectObject(hmemdc, hbitmap);//该函数选择一对象到指定的设备上下文环境中，该新对象替换先前的相同类型的对象
            PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);//删除用过的对象  
            DeleteDC(hmemdc);//删除用过的对象 

            Bitmap pic = new Bitmap(width, height - nox_topHeight); //建立圖片
            Graphics graphic = Graphics.FromImage(pic);//建立畫板
            graphic.DrawImage(bmp,
                     //將被切割的圖片畫在新圖片上面，第一個參數是被切割的原圖片
                     new Rectangle(0, 0, width, height - nox_topHeight),
                     //指定繪製影像的位置和大小，基本上是同pic大小
                     new Rectangle(0, nox_topHeight, width, height - nox_topHeight),
                     //指定被切割的圖片要繪製的部分
                     GraphicsUnit.Pixel);
            bmp.Dispose();
            graphic.Dispose();
            return pic;
        }

        
    }
}
