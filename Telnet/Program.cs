using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Telnet
{
    internal class Program
    {
        const int PORT = 8080;
        static void Main(string[] args)
        {
            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddr = IPAddress.Any;
            IPEndPoint port = new IPEndPoint(ipaddr, PORT);
            socketListener.Bind(port);
            socketListener.Listen(10);
            Console.WriteLine($"Server startd on port {PORT}");
            Socket client = socketListener.Accept();
            Console.WriteLine($"New client connected ... {client.RemoteEndPoint}");

            byte[] buffer = new byte[128];
            int numberOfRecivedBytes = 0;

            while (true)
            {
                numberOfRecivedBytes = client.Receive(buffer);
                Console.WriteLine($"Resived bytes: {numberOfRecivedBytes}");
                Console.WriteLine($"Data send: {buffer}");

                string recivedData = Encoding.ASCII.GetString(buffer, 0, numberOfRecivedBytes);
                Console.WriteLine($"Recived data: {recivedData}");
                client.Send(buffer);                                   

                if(recivedData == "0")
                {
                    break;
                }

                Array.Clear(buffer, 0, buffer.Length);
                numberOfRecivedBytes = 0;
            }
        }
    }
}
