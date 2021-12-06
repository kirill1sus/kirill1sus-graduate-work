using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Watching
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //public static byte[] StringToByteArray(string hex, int ind)
        //{
        //    hex = hex.Remove(0, 2);
        //        return Enumerable.Range(0, hex.Length)
        //    .Where(x => x % 2 == 0)
        //    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        //    .ToArray();
        //}

        //public static byte StringToByteArray(string hex)
        //{
        //    hex = hex.Remove(0, 2);
        //    byte[] tmp = Enumerable.Range(0, hex.Length)
        //    .Where(x => x % 2 == 0)
        //    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        //    .ToArray();
        //    return tmp[1];
        //}

        private async void Form1_Load(object sender, EventArgs e)
        {
            //string configStringKey =ConfigurationManager.AppSettings.Get("vernum");

            //byte secretKey = StringToByteArray(configStringKey);
            var port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("port"));

            var client = new UdpClient(port);
            while (true)
            {
                var date = await client.ReceiveAsync();
                using (var ms = new MemoryStream(date.Buffer)) 
                {
                    pictureBox1.Image = new Bitmap(ms);
                }
                Text = $"Bytes receved:{date.Buffer.Length * sizeof(byte) } ";
            }
        }



        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            MessageBox.Show(string.Join("\n", host.AddressList.
                Where(i => i.AddressFamily == AddressFamily.InterNetwork)));
        }
    }
}
