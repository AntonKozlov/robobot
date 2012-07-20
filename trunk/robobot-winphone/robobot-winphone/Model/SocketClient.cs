using System;
using System.Net;
using System.Net.Sockets;

namespace robobot_winphone.Model
{
    public class SocketClient
    {
        private static int MaxLength = 4;
        private static SocketClient instance;

        private Socket socket;
        private IPEndPoint endPoint;

        public ISensorExecutor Subscriber { get; set; }

        public static SocketClient Instance
        {
            get { return instance ?? (instance = new SocketClient()); }
        }

        private SocketClient() { }

        public void ConnectToDevice(IPAddress address, int port)
        {
            if (socket != null)
            {
                socket.Dispose();
            }

            endPoint = new IPEndPoint(address, port);  
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var socketEventArg = new SocketAsyncEventArgs
                                     {
                                         RemoteEndPoint = endPoint
                                     };
            socketEventArg.Completed += delegate(object s, SocketAsyncEventArgs e)
                                            {
                                                if (e.SocketError == SocketError.Success)
                                                {
                                                    LogManager.Log("Socket connected");
                                                }
                                            };

            try
            {
                socket.ConnectAsync(socketEventArg);
            }
            catch (SocketException)
            {
                LogManager.Log("ConnectAsync error");
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
                socket.Dispose();
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
                var socketEventArg = new SocketAsyncEventArgs
                                         {
                                             RemoteEndPoint = endPoint
                                         };

                socketEventArg.SetBuffer(data, 0, data.Length);

                socketEventArg.Completed += delegate(object s, SocketAsyncEventArgs e)
                                                {
                                                    if (e.SocketError != SocketError.Success)
                                                    {
                                                        LogManager.Log(String.Format("Send error: {0}", e.SocketError));
                                                    }
                                                };

                try
                {
                    socket.SendAsync(socketEventArg);   
                }
                catch (SocketException)
                {
                    LogManager.Log("SendAsync error");
                }        
            }
            else
            {
                LogManager.Log("Socket isn't connected");
            }
        }

        public bool IsConnected()
        {
            return (socket != null) && (socket.Connected);
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
