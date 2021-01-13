using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] arp_output;
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        private string[] get_arp_ip_interfce(string arp_ip_text)
        {
            List<string> ip_range = new List<string>();
            string split_ip;
            split_ip = arp_ip_text.Split(' ')[1];
            split_ip = split_ip.Split('.')[0] + '.' + split_ip.Split('.')[1] + '.' + split_ip.Split('.')[2];
            list2clear();
            foreach (string str in arp_output)
            {
                if (str.Contains(split_ip) && (!str.Contains("Interface")))
                {
                    if (str.Contains("ff-ff-ff-ff-ff-ff"))
                    {
                        break;
                    }
                    string[] split_ip2 = str.Trim().Split(' ');
                    ip_range.Add(split_ip2[0]);
                }
            }
            return ip_range.ToArray();
        }


        bool listbox_index_changed = false;
        string arp_ip_text;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listbox_index_changed = true;
            arp_ip_text = listBox1.Text;
            //listBox2.Items.Add(split_ip);
        }
     
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }
        static string output;
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            
        }
        private static System.Threading.Timer timer;
        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new System.Threading.Timer(_ => OnCallBack(), null, 1000 * 1, Timeout.Infinite); //in 10 seconds
        }
        delegate void list1clearCallback();

        private void list1clear()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox1.InvokeRequired)
            {
                list1clearCallback d = new list1clearCallback(list1clear);
                this.Invoke(d, new object[] {  });
            }
            else
            {
                listBox1.Items.Clear();
            }
        }
        delegate void list2clearCallback();

        private void list2clear()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox2.InvokeRequired)
            {
                list2clearCallback d = new list2clearCallback(list2clear);
                this.Invoke(d, new object[] { });
            }
            else
            {
                listBox2.Items.Clear();
            }
        }
        delegate void list1addCallback(string item);

        private void list1add(string item)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox1.InvokeRequired)
            {
                list1addCallback d = new list1addCallback(list1add);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                listBox1.Items.Add(item);
            }
        }

        delegate void list2addCallback(int index, string item);
        private void list2add(int index, string item)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox2.InvokeRequired)
            {
                list2addCallback d = new list2addCallback(list2add);
                this.Invoke(d, new object[] { index,item });
            }
            else
            {
                try
                {
                    listBox2.Items[index] = item;
                }
                catch
                {
                }
            }
        }

        delegate void list2addRangeCallback(string[] item);

        private void list2addrange(string[] item)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (listBox2.InvokeRequired)
            {
                list2addRangeCallback d = new list2addRangeCallback(list2addrange);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                listBox2.Items.AddRange(item);
            }
        }

        private void OnCallBack()
        {
            timer.Dispose();
            //timer1.Interval = 1000;
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "arp";
            p.StartInfo.Arguments = "-a";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output2 = p.StandardOutput.ReadToEnd();
            if (output == output2)
            {
                if (listbox_index_changed)
                {
                    list2addrange(get_arp_ip_interfce(arp_ip_text));
                    listbox_index_changed = false;
                }
                using (TcpClient tcpClient = new TcpClient())
                {
                    for (int i = 0; i < listBox2.Items.Count; i++)
                    {

                        string str_ip = listBox2.Items[i].ToString().Split('-')[0].Trim();

                        if (!listBox2.Items[i].ToString().Contains("-"))
                        {
                            list2add(i, str_ip + " -   Querying");
                        }
                        try
                        {
                           
                            if (!listBox2.Items[i].ToString().Contains("Port closed"))
                                tcpClient.Connect(str_ip, 3389);

                            var times = new List<double>();
                            //for (int j = 0; j < 4; j++)
                           // {
                                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                sock.Blocking = true;

                                var stopwatch = new Stopwatch();
                             
                                IPAddress ip = IPAddress.Parse(str_ip);// Dns.GetHostEntry(listBox2.Text).AddressList[0]; 
                                int port = 3389;

                                // Measure the Connect call only
                                stopwatch.Start();

                                sock.Connect(ip, port);
                                stopwatch.Stop();

                                double t = stopwatch.Elapsed.TotalMilliseconds;
                              //  Console.WriteLine("{0:0.00}ms", t);
                               // times.Add(t);

                                sock.Close();

                         //       Thread.Sleep(1000);
                            //}
                            list2add(i ,str_ip + " -   time = " +  (int)t + " ms");
                         //   Console.WriteLine("{0:0.00} {1:0.00} {2:0.00}", times.Min(), times.Max(), times.Average());
                            //Process.Start("mstsc", "/v:" + listBox2.Text);
                        }
                        catch (Exception)
                        {
                            list2add(i, str_ip + " -   Port closed");
                        }
                    }
                }
                timer = new System.Threading.Timer(_ => OnCallBack(), null, 1000 * 1, Timeout.Infinite); //in 10 seconds
                return;
            }
            output = output2;
            arp_output = output.Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            list1clear();
            foreach (string str in arp_output)
            {
                if (str.Contains("Interface"))
                    list1add(str);
            }

            //System.Diagnostics.Process.Start("route", "-f");
            //System.Diagnostics.Process.Start("ipconfig", "/release");
            //System.Diagnostics.Process.Start("ipconfig", "/renew");
            //System.Diagnostics.Process.Start("arp", "-d *");
            //System.Diagnostics.Process.Start("netsh", "interface ip delete arpcache");
            //System.Diagnostics.Process.Start(@"c:\windows\sysnative\nbtstat.exe", "-R");
            //System.Diagnostics.Process.Start(@"c:\windows\sysnative\nbtstat.exe", "-RR");
            //System.Diagnostics.Process.Start("ipconfig", "/flushdns");
            //System.Diagnostics.Process.Start("ipconfig", "/registerdns");
            p.WaitForExit();
            timer = new System.Threading.Timer(_ => OnCallBack(), null, 1000 * 1, Timeout.Infinite); //in 10 seconds
        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            if (!listBox2.Text.Contains("Port closed"))
            {
                Process p = new Process();
                string str_ip = listBox2.Text.Split('-')[0].Trim();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "mstsc";
                p.StartInfo.Arguments = "/v:" + str_ip;
                //p.StartInfo.CreateNoWindow = true;
                p.Start();
                // Process.Start("mstsc", "/v:" + listBox2.Text);


            }

        }
    }

    
}
