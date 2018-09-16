using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidEmulatorController
{
    class Pad
    {
        public static void doWork()
        {
            AdbHandler.tap(ImageHandler.findImgLocation("D://pad.png", 5));
       }

        public static void start()
        {
            Thread t = new Thread(new ThreadStart(doWork));
            t.Start();
        }
    }
}
