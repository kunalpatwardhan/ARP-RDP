using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string split_ip;
            split_ip = listBox1.Text.Split(' ')[1];
            split_ip = split_ip.Split('.')[0] + '.' + split_ip.Split('.')[1] + '.' + split_ip.Split('.')[2];
            listBox2.Items.Clear();
            foreach (string str in arp_output)
            {
                if (str.Contains(split_ip) && (!str.Contains("Interface")))
                {
                    if(str.Contains("ff-ff-ff-ff-ff-ff"))
                    {
                        break;
                    }
                    string[] split_ip2 = str.Trim().Split(' ');
                    listBox2.Items.Add(split_ip2[0]);
                }
            }
            //listBox2.Items.Add(split_ip);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listBox2.Text.Contains("Port closed"))
                Process.Start("mstsc", "/v:" + listBox2.Text);


                  
            

        }
        string output;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
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
                            listBox2.Items[listBox2.Items.IndexOf(listBox2.Items[i].ToString())] = listBox2.Items[i].ToString() + " - Port closed";
                        }
                    }
                }
                return;
            }
            output = output2;
            arp_output = output.Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            foreach (string str in arp_output)
            {
                if (str.Contains("Interface"))
                    listBox1.Items.Add(str);
            }

            p.WaitForExit();
            
        }
    }
}
