using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace Showing
{
    class Program
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]

        static extern bool ShowWindow(IntPtr hWnd, int mCmdShow);

        private static IPEndPoint showingEndPoint;
        private static UdpClient udpClient = new UdpClient();

        public static byte[] StringToByteArray(string hex, int ind)
        {
            hex = hex.Remove(0, 2);
            return Enumerable.Range(0, hex.Length)
        .Where(x => x % 2 == 0)
        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        .ToArray();
        }

        public static byte StringToByteArray(string hex)
        {
            hex = hex.Remove(0, 2);
            byte[] tmp = Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
            return tmp[1];
        }

        static void Main(string[] args)
        {
            var showingIp = ConfigurationManager.AppSettings.Get("showingIp");
            var showingPort = Convert.ToInt32(ConfigurationManager.AppSettings.Get("showingPort"));
            showingEndPoint = new IPEndPoint(IPAddress.Parse(showingIp), showingPort);
            Console.WriteLine($"showing: {showingEndPoint}");

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSours_NewFrame;
            videoSource.Start();

            Console.WriteLine("\n press Enter to hide the console");
            Console.ReadLine();
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }
        
        private static void VideoSours_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs) 
        {
            string configStringKey = ConfigurationManager.AppSettings.Get("vernum");

            byte secretKey = StringToByteArray(configStringKey);

            var bmp = new Bitmap(eventArgs.Frame, 800, 600);
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    //
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte)(bytes[i] ^ secretKey);
                    }
                    //
                    udpClient.Send(bytes, bytes.Length, showingEndPoint);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
    }
}
