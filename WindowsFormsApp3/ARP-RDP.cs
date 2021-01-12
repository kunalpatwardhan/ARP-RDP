using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
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
            split_ip = listBox1.Text.Split(' ')[1];
            split_ip = split_ip.Split('.')[0] + '.' + split_ip.Split('.')[1] + '.' + split_ip.Split('.')[2];
            listBox2.Items.Clear();
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
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.AddRange( get_arp_ip_interfce(listBox1.Text));
            //listBox2.Items.Add(split_ip);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listBox2.Text.Contains("Port closed"))
                Process.Start("mstsc", "/v:" + listBox2.Text);


                  
            

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
                listBox2.Items[index] = item;
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

                using (TcpClient tcpClient = new TcpClient())
                {
                    for (int i = 0; i < listBox2.Items.Count; i++)
                    {
                        try
                        {
                            if (!listBox2.Items[i].ToString().Contains("Port closed"))
                                tcpClient.Connect(listBox2.Items[i].ToString(), 3389);
                            //Process.Start("mstsc", "/v:" + listBox2.Text);
                        }
                        catch (Exception)
                        {
                            list2add(listBox2.Items.IndexOf(listBox2.Items[i].ToString()) , listBox2.Items[i].ToString() + " - Port closed");
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

            p.WaitForExit();
            timer = new System.Threading.Timer(_ => OnCallBack(), null, 1000 * 1, Timeout.Infinite); //in 10 seconds
        }
    }

    
}
