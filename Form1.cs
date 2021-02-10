using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;

namespace TC
{
    public partial class timerCorne : Form
    {
        private int seconde = 240;
        private int ayugraId = 0;
        NamedPipeClientStream client;
        StreamReader sr;
        StreamWriter sw;
        DateTime startTime;

        private void init()
        {
            timer_lb.Text = "En attente de la corne..";
            //timer_lb.Text = ((int)secondes/60) + ":" + (seconde-((int)secondes/60));
        }

        async private void analyser()
        {
            await Task.Run(async () =>
            {
                using (sr)
                {
                    string returndata;
                    bool stop = false;
                    while ((returndata = sr.ReadLine()) != null && !stop)
                    {
                        //Console.WriteLine(returndata)
                        if (returndata.Contains("msgi 0 1183"))
                        {
                            stop = true;
                            Console.WriteLine(returndata);
                            startTime = DateTime.Now;
                            timer_lb.Invoke(new MethodInvoker(delegate { timer.Start(); }));
                            Console.WriteLine("Deb");
                        }
                    }
                }
            });
            
        }
        public timerCorne()
        {

            InitializeComponent();
            init();

            InitAsk ia = new InitAsk();
            ia.ShowDialog();
            if (Int32.Parse(ia.ayuid_tb.Text) != null)
            {
                ayugraId = Int32.Parse(ia.ayuid_tb.Text);

                client = new NamedPipeClientStream("AyugraPacketApi_" + ayugraId);
                client.Connect();

                sr = new StreamReader(client);
                sw = new StreamWriter(client);

                //timer.Start();

                analyser();
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingSeconds = seconde - elapsedSeconds;
            int mins = remainingSeconds / 60;
            int secs = remainingSeconds % 60;


            if (remainingSeconds <= 0)
            {
                startTime = DateTime.Now;
                //timer.Stop();
            }
            if (secs < 10) {

                timer_lb.Text = mins + ":0" + secs;
            } else
            {
                timer_lb.Text = mins + ":" + secs;
            }
            if (mins >= 1 && mins <= 2)
            {
                timer_lb.ForeColor = Color.White;
            } else if (mins == 3)
            {
                timer_lb.ForeColor = Color.Red;
            } else if (mins == 0)
            {
                timer_lb.ForeColor = Color.Orange;
            }
        }
    }
}
