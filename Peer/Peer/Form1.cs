using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Peer
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            msg("connecting to the server");
            TcpClient cs = new TcpClient("127.0.0.1", 5656);
            msg("connection established");
            NetworkStream ss = cs.GetStream();
            byte[] os = Encoding.ASCII.GetBytes(textBox1.Text);
            ss.Write(os, 0, os.Length);
            msg("requesting file");
            ss.Flush();
            byte[] filedata = new byte[10000 * 1024];
            ss.Read(filedata, 0, cs.ReceiveBufferSize);
            string data = Encoding.ASCII.GetString(filedata);
            string path=@"C:\Users\dotnet2\Documents\Visual Studio 2012\Projects\Peer\Peer\resources\";
            File.WriteAllText(Path.Combine(path,textBox1.Text), data);
            msg("file received");
            ss.Flush();
            cs.Close();
        }

        private void msg(String a)
        {
            richTextBox1.Text = a;
        }
       

    }
}
