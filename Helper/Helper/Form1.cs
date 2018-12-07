using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Helper
{
    public partial class Form1 : Form
    {
        OpenFileDialog ofd = new OpenFileDialog();
        SqlConnection conn = new SqlConnection("Data Source=RDP;Initial Catalog=cloud;Integrated Security=True");
        
        String path = "", fname = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.ShowDialog();
            textBox1.Text = ofd.FileName;
            path = textBox1.Text;
            fname = Path.GetFileName(path);

        }

        private void button2_Click(object sender, EventArgs e)
        {
             if (textBox1.Text == "")
            {
                MessageBox.Show("Choose the file", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (ofd.FileName == "")
            {
                MessageBox.Show("Select the File ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
             else if (ofd.FileName == "openFileDialog1")
             {
                 MessageBox.Show("Select the File ", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             }
             else
             {
                 FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                 byte[] buffer = new byte[fs.Length];
                 BinaryReader br = new BinaryReader(fs);
                 buffer = br.ReadBytes((Int32)fs.Length);
    
                 fs.Close();
                 conn.Open();
                 SqlCommand cmd = new SqlCommand("insert into fileupload(filename,filedata) values(@filename,@filedata)", conn);
                 cmd.Parameters.Add("@filename",SqlDbType.NVarChar, fname.Length).Value=fname;
                 cmd.Parameters.Add("@filedata",SqlDbType.Binary,buffer.Length).Value=buffer;
                 cmd.ExecuteNonQuery();
                 conn.Close();
                 textBox1.Text = "";
             }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 5654);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(ip);
            server.Listen(100);
            while (true)
            {
                Socket client = server.Accept();
                byte[] data = new byte[1024];
                int size = client.Receive(data);
                char[] chars = new char[size];
                int res = Encoding.UTF8.GetDecoder().GetChars(data, 0, size, chars, 0);
                String result = new String(chars);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select filedata from [cloud].[dbo].[fileupload] where filename=@filename", conn);
                cmd.Parameters.AddWithValue("@filename", fname);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    byte[] filedata =(byte[]) reader[0];
                    client.Send(filedata);
                   
                }
                client.Close();
                conn.Close();

            }

        }

       
    }
}
