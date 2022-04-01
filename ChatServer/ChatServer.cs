using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public class ChatServer
    {
        Socket serverSocket;
        IPEndPoint localIP;
        int _port = 8080;
        const int BUFFER = 1024;
        List<EndPoint> listOfClients;

        public int PORT { get { return _port; } set { _port = value; } }

        Form1 _mainForm;

        public ChatServer(Form1 mainForm)
        {
            _mainForm = mainForm;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localIP = new IPEndPoint(IPAddress.Any, _port);
            serverSocket.EnableBroadcast = true;
            listOfClients = new List<EndPoint>();
        }

        public void Recive()
        {
            try
            {
                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                socketAsyncEvent.SetBuffer(new byte[BUFFER], 0, BUFFER);
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
        }

        private void ReciveCallback(object? sender, SocketAsyncEventArgs e)
        {
            string command = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
            Debug.WriteLine($"Client ip {e.RemoteEndPoint}\tCommand text: {command}");

            if (command.Equals("[CHATDISCOVER]"))
            {
                if (!listOfClients.Contains(e.RemoteEndPoint))
                {
                    listOfClients.Add(e.RemoteEndPoint);
                    Debug.WriteLine($"Client count: {listOfClients.Count}\tIP: {e.RemoteEndPoint}\n===================>");
                }
                // Send data to client
                SendCommandToClient("[TEST]", e.RemoteEndPoint);

            }
            else if (listOfClients.Contains(e.RemoteEndPoint))
            {
                foreach (IPEndPoint ep in listOfClients)
                {
                    //if (!ep.Equals(e.RemoteEndPoint))
                    //{
                    //    // Send data to client
                    //    SendCommandToClient(command,ep);
                    //}
                    SendCommandToClient(command, ep);
                }
            }
            Recive();

        }

        private void SendCommandToClient(string command, EndPoint remoteEP)
        {
            if (string.IsNullOrEmpty(command) || remoteEP == null)
                return;

            SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
            socketAsyncEvent.RemoteEndPoint = remoteEP;

            var buffer = Encoding.ASCII.GetBytes(command);
            socketAsyncEvent.SetBuffer(buffer, 0, buffer.Length);
            socketAsyncEvent.Completed += SendMessage;
            serverSocket.SendToAsync(socketAsyncEvent);


            _mainForm.Invoke(new MethodInvoker(() =>
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine();
                message.AppendLine($"Client count: { listOfClients.Count}\tClient ip {socketAsyncEvent.RemoteEndPoint}");
                message.AppendLine($"{command}");
                message.AppendLine($"=======================>");
                _mainForm.ChatTextBox.Text += message.ToString();
            }));
        }

        private void SendMessage(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Sent data to client {e.RemoteEndPoint}");
        }
    }
}
