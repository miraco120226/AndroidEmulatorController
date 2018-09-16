using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidEmulatorController
{
    class AdbHandler
    {
        const string sendeventCmd = "sendevent /dev/input/event7";
        static string devicesPath = "";

        public static void setNoxPort(string nox_path)
        {
            string output = CommandLineHandler.doCommand(nox_path + "nox_adb devices");
            string pat = @"127.0.0.1:62\d\d\d	device";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            Match m = r.Match(output);
            string line = "";
            while (m.Success)
            {
                line = m.Groups[0].ToString();
                m = m.NextMatch();
            }

            devicesPath = "127.0.0.1:" + line.Replace("	device", "").Replace("127.0.0.1:", "");
        }

        private static void sendDrapStart(int x, int y)
        {
            List<string> list = new List<string>();
            list.Add("1 330 1");
            list.Add("3 58 1");
            list.Add("3 53 " + x);
            list.Add("3 54 " + y);
            list.Add("0 2 0");
            list.Add("0 0 0");
            foreach (string c in list)
            {
                CommandLineHandler.doNoOutputCommand(Form1.nox_path + "nox_adb -s " + devicesPath + " shell " + sendeventCmd + " " + c);
            }
        }

        private static void sendDrapFinish()
        {
            List<string> list = new List<string>();
            list.Add("0 2 0");
            list.Add("0 0 0");
            list.Add("1 330 0");
            list.Add("3 58 0");
            list.Add("3 53 0");
            list.Add("3 54 0");
            list.Add("0 2 0");
            list.Add("0 0 0");
            foreach (string c in list)
            {
                CommandLineHandler.doNoOutputCommand(Form1.nox_path + "nox_adb -s " + devicesPath + " shell " + sendeventCmd + " " + c);
            }
        }

        public static void swap(int x1, int y1, int x2, int y2)
        {
            List<string> code123 = new List<string>();
            for (int x = x1; x < x2; x += 10)
            {
                code123.Add("3 53 " + x);
                code123.Add("3 54 " + y1);
                code123.Add("0 2 0");
                code123.Add("0 0 0");
            }

            for (int y = y1; y < y2; y += 10)
            {
                code123.Add("3 53 " + x2);
                code123.Add("3 54 " + y);
                code123.Add("0 2 0");
                code123.Add("0 0 0");
            }

            sendDrapStart(x1, y1);
            Thread.Sleep(100);
            foreach (string c in code123)
            {
                CommandLineHandler.doNoOutputCommand(Form1.nox_path + "nox_adb -s " + devicesPath + " shell sendevent /dev/input/event7 " + c);
                Thread.Sleep(100);
                Console.WriteLine("sendevent / dev / input / event7 " + c);
            }
            Thread.Sleep(300);
            sendDrapFinish();
        }

        public static void tap(int x, int y)
        {
            string cmd = Form1.nox_path + "nox_adb -s " + devicesPath + " shell input tap " + x + " " + y;
            Thread tap = new Thread(() => CommandLineHandler.doNoOutputCommand(cmd));
            tap.Start();
        }

        public static void tap(Point p)
        {
            string cmd = Form1.nox_path + "nox_adb -s " + devicesPath + " shell input tap " + p.X + " " + p.Y;
            Thread tap = new Thread(() => CommandLineHandler.doNoOutputCommand(cmd));
            tap.Start();
        }
    }
}
