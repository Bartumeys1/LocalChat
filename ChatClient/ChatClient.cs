using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public class ChatClient
    {
        Socket serverSocket;
        IPEndPoint localIP;
        IPEndPoint localBroadcast;
        byte[]BUFFER = new byte[1024];

        ChatClientForm _mainForm;
        ListBox messageList;
        public ChatClient(int remotePort, int localPort , ListBox messageLis , ChatClientForm mainForm)
        {
            _mainForm = mainForm;
            messageList = messageLis;
            localBroadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);
            localIP = new IPEndPoint(IPAddress.Any, localPort);
            Send("[CHATDISCOVER]");
            Task recive = new Task(Recive);
            recive.Start();
        }


        public void Recive()
        {
            do
            {
                try
                {
                    SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                    socketAsyncEvent.SetBuffer(BUFFER, 0, BUFFER.Length);
                    socketAsyncEvent.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    if (!serverSocket.IsBound)
                    {
                        serverSocket.Bind(localIP);
                    }

                    socketAsyncEvent.Completed += ReciveCallback;

                    if (!serverSocket.ReceiveFromAsync(socketAsyncEvent))
                    {
                        Debug.WriteLine("Faled to recive data");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            } while (true);
        }

        private void ReciveCallback(object? sender, SocketAsyncEventArgs e)
        {
            _mainForm.Invoke(new MethodInvoker(() =>
            {
                string response = Encoding.ASCII.GetString(e.Buffer);
                string time = DateTime.Now.ToShortTimeString();
                messageList.Items.Add($"{time} {response}");
            }));
            Array.Clear(BUFFER,0,BUFFER.Length);
        }

        public void Send(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }

                if(serverSocket == null)
                {
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    serverSocket.EnableBroadcast = true;
                    serverSocket.Bind(localIP);
                }

                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                BUFFER = Encoding.ASCII.GetBytes(str);
                socketAsyncEvent.SetBuffer(BUFFER, 0, BUFFER.Length);
                socketAsyncEvent.RemoteEndPoint = localBroadcast;
                socketAsyncEvent.Completed += SendCallback;
                serverSocket.SendToAsync(socketAsyncEvent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void SendCallback(object? sender, SocketAsyncEventArgs e)
        {
            messageList.Items.Add($"Send to server {e.RemoteEndPoint} ..."); 
        }
    }
}
