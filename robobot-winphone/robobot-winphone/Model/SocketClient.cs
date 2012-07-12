using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace robobot_winphone.Model
{
    public class SocketClient
    {
        private static int MaxLength = 4;
        private static SocketClient instance;

        private Socket socket;
        private IPEndPoint endPoint;

        public ISensorView Subscriber { get; set; }

        public static SocketClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SocketClient();
                }
                return instance;
            }
        }

        private SocketClient() { }

        public void ConnectToDevice(IPAddress address, int port)
        {
            endPoint = new IPEndPoint(address, port);  
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = endPoint;
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                if (e.SocketError == SocketError.Success)
                {
                    LogManager.Log("Socket connected");
                }
            });

            try
            {
                socket.ConnectAsync(socketEventArg);
            }
            catch (SocketException)
            {
                LogManager.Log("Connection error");
            }
        }

        public void Disconnect()
        {
            if (socket == null)
            {
                return;
            }

            if (socket.Connected)
            {
                socket.Close();
                socket = null;
                LogManager.Log("Socket closed");
            }
            else
            {
                LogManager.Log("Socket isn't connected");
            }
        }

        public void SendData(byte[] data)
        {
            if (socket == null)
            {
                return;
            }

            if (socket.Connected)
            {
                var socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = endPoint;
                socketEventArg.SetBuffer(data, 0, data.Length);

                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError != SocketError.Success)
                    {
                        LogManager.Log(String.Format("Send error: {0}", e.SocketError));
                    }
                });

                socket.SendAsync(socketEventArg);          
            }
            else
            {
                LogManager.Log("Socket isn't connected");
            }
        }

        public bool IsConnected()
        {
            if ((socket != null) && (socket.Connected))
            {
                return true;
            }
            return false;
        }
        //Not tested

        //public Byte[] ReceiveData()
        //{
        //    if (socket.Connected)
        //    {
        //        var socketEventArg = new SocketAsyncEventArgs();

        //        socketEventArg.RemoteEndPoint = endPoint;
        //        var response = new Byte[MaxLength];
        //        socketEventArg.SetBuffer(new Byte[MaxLength], 0, MaxLength);

        //        socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
        //        {
        //            if (e.SocketError == SocketError.Success)
        //            {
        //                response = e.Buffer;
        //            }
        //            else
        //            {
        //                LogManager.Log("Socket error");
        //            }
        //        });

        //        socket.ReceiveAsync(socketEventArg);
        //        LogManager.Log("Data received");
        //        return response;
        //    }
        //    else
        //    {
        //        LogManager.Log("Socket isn't connected");
        //        return null;
        //    }
        //}
    }
}
