using FxChat.Domain;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FxChat.App.Server.ViewModels
{
    public class ServerMainViewModel : BindableBase
    {
        public string ServerIp { get; set; } = "127.0.0.1";

        public int ServerPort { get; set; } = 15000;

        public int MaxConnect { get; set; } = 4;

        private Socket serverSocket;

        public Socket ServerSocket
        {
            get => serverSocket;
            set
            {
                SetProperty(ref serverSocket, value);
            }
        }

        private string state = "未启动服务";

        public string State
        {
            get => state;
            set
            {
                SetProperty(ref state, value);
            }
        }

        private string inputMessage;

        public string InputMessage
        {
            get => inputMessage;
            set
            {
                SetProperty(ref inputMessage, value);
            }
        }


        public ObservableCollection<Socket> ConnectedSockets { get; set; } = new ObservableCollection<Socket>();

        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();


        public ServerMainViewModel()
        {
            StartServerCommand = new DelegateCommand(StartServer);
            SendMessageCommand = new DelegateCommand(SendMessage);
        }



        public DelegateCommand StartServerCommand { get; private set; }

        public DelegateCommand SendMessageCommand { get; private set; }

        private void StartServer()
        {
            if (ServerSocket != null) return;

            // 1 创建服务端Socket
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 2 Bind
            IPAddress iPAddress = IPAddress.Parse(ServerIp);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, ServerPort);
            ServerSocket.Bind(iPEndPoint);

            // 3 Listen
            ServerSocket.Listen(MaxConnect);


            State = "已启动";
            RemindMessage(ServerSocket.LocalEndPoint?.ToString() ?? "", EndPointType.Local, "服务已启动！");

            // 4 Accept
            Task.Run(() => Accept(ServerSocket));
        }

        private void SendMessage()
        {
            if (string.IsNullOrEmpty(InputMessage) || ConnectedSockets.Count == 0) return;

            var sendBytes = Encoding.UTF8.GetBytes(InputMessage);
            foreach (var connectedSocket in ConnectedSockets)
            {
                // 发送到连接到客户端
                connectedSocket.Send(sendBytes);
            }

            RemindMessage(ServerSocket.LocalEndPoint?.ToString() ?? "", EndPointType.Local, InputMessage);
            Application.Current.Dispatcher.Invoke(() => { InputMessage = ""; });
        }

        /// <summary>
        /// 等待客户端连接服务端
        /// </summary>
        /// <param name="serverSocket"></param>
        private void Accept(Socket serverSocket)
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                RemindClinetOnline(clientSocket);

                // Receive
                Task.Run(() => Receive(clientSocket));
            }
        }

        /// <summary>
        /// 接收客户端发送消息
        /// </summary>
        /// <param name="clinetSocket"></param>
        private void Receive(Socket clinetSocket)
        {
            byte[] data = new byte[1024 * 1024];

            while (clinetSocket.Connected)
            {
                //读取客户端发送过来的数据
                int readLeng = clinetSocket.Receive(data, 0, data.Length, SocketFlags.None);
                //客户端断开连接
                if (readLeng == 0)
                {
                    RemindClinetOffline(clinetSocket);
                    break;
                }

                string content = Encoding.UTF8.GetString(data, 0, readLeng);
                RemindMessage(clinetSocket.RemoteEndPoint?.ToString() ?? "", EndPointType.Remote, content);

                // 转发到各个连接的客户端
                var contentByte = Encoding.UTF8.GetBytes(content);
                foreach (var clientSocket in ConnectedSockets.Except(new List<Socket>() { clinetSocket }))
                {
                    clientSocket.Send(contentByte);
                }
            }
        }

        private void RemindClinetOnline(Socket connectedSocket)
        {
            RemindMessage(connectedSocket.RemoteEndPoint?.ToString() ?? "", EndPointType.Remote, "上线");

            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedSockets.Add(connectedSocket);
            });
        }

        private void RemindClinetOffline(Socket clientSocket)
        {
            RemindMessage(clientSocket.RemoteEndPoint?.ToString() ?? "", EndPointType.Remote, "下线");

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ConnectedSockets.Contains(clientSocket))
                {
                    ConnectedSockets.Remove(clientSocket);
                }
            });

            // 停止会话
            clientSocket.Shutdown(SocketShutdown.Both);
            // 关闭连接
            clientSocket.Close();
        }

        private void RemindMessage(string endPoint, EndPointType endPointType, string content)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(Message.BuildMessage(endPoint, endPointType, content));
            });
        }

    }
}
