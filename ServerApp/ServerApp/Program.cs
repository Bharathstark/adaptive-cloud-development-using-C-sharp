using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 5656);
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                server.Bind(ip);
                server.Listen(100);
                while (true)
                {
                    Socket client = server.Accept();
                    Console.WriteLine("accepted" + client);
                    Console.WriteLine("");
                    byte[] data = new byte[1024];
                    int size = client.Receive(data);
                    Console.WriteLine("received request");
                    char[] chars=new char[size];
                    int res = Encoding.UTF8.GetDecoder().GetChars(data,0,size,chars,0);
                    String result = new String(chars);
                    String path = @"C:\Users\dotnet2\Documents\Visual Studio 2012\Projects\ServerApp\ServerApp\Resources\" + result;
                    if (File.Exists(path))
                    {
                        Console.WriteLine("file found...sending to peer");
                        byte[] filedata = File.ReadAllBytes(path);
                        client.Send(filedata);
                    }
                    else
                    {
                        Console.Write("file not found.....contacting helper");
                        TcpClient cs = new TcpClient("127.0.0.1", 5654);
                        NetworkStream ss = cs.GetStream();
                        Console.WriteLine("connection established with helper...");
                        byte[] os = Encoding.ASCII.GetBytes(result);
                        Console.WriteLine("sending request...helper");
                        ss.Write(os, 0, os.Length);
                        ss.Flush();
                        byte[] filedat = new byte[cs.ReceiveBufferSize];
                        ss.Read(filedat, 0, cs.ReceiveBufferSize);
                        Console.Write("receiving file and forwarding");
                        string dat = Encoding.ASCII.GetString(filedat);
                        string pat = @"C:\Users\dotnet2\Documents\Visual Studio 2012\Projects\ServerApp\ServerApp\Resources";
                        File.WriteAllText(Path.Combine(pat,result), dat);
                        client.Send(filedat);
                        ss.Flush();
                        cs.Close();
                    }
                    Console.WriteLine(path);
                   
                }
                server.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }
    }
}
