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

namespace FxChat.App.Client.ViewModels
{
    public class ClientMainViewModel : BindableBase
    {
        public string ServerIp { get; set; } = "127.0.0.1";

        public int ServerPort { get; set; } = 15000;

        private Socket clientSocket;

        public Socket ClientSocket
        {
            get => clientSocket;
            set
            {
                SetProperty(ref clientSocket, value);
            }
        }

        private string state = "未连接服务";

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


        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();


        public ClientMainViewModel()
        {
            ConnectServerCommand = new DelegateCommand(ConnectServer);
            DisconnectServerCommand = new DelegateCommand(DisconnectServer);
            SendMessageCommand = new DelegateCommand(SendMessage);
        }



        public DelegateCommand ConnectServerCommand { get; private set; }
        public DelegateCommand DisconnectServerCommand { get; private set; }

        public DelegateCommand SendMessageCommand { get; private set; }

        private void ConnectServer()
        {
            if (ClientSocket != null) return;

            // 1 创建服务端Socket
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 2 Connect
            IPAddress iPAddress = IPAddress.Parse(ServerIp);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, ServerPort);
            ClientSocket.Connect(iPEndPoint);

            //RemindClinetOnline(ClientSocket);
            Application.Current.Dispatcher.Invoke(() => { State = "已连接"; });
            RemindMessage(clientSocket?.LocalEndPoint?.ToString() ?? "", EndPointType.Local, "启动连接");


            // 4 Accept
            Task.Run(() => Receive(ClientSocket));
        }

        private void DisconnectServer()
        {
            if (ClientSocket == null) return;

            RemindMessage(ClientSocket.LocalEndPoint?.ToString() ?? "", EndPointType.Local, "断开连接");
            Application.Current.Dispatcher.Invoke(() => { State = "未连接"; });


            // 停止会话
            ClientSocket.Shutdown(SocketShutdown.Both);
            // 关闭连接
            ClientSocket.Close();
            ClientSocket = null;
        }

        private void SendMessage()
        {
            if (ClientSocket == null || string.IsNullOrEmpty(InputMessage)) return;

            // 发送到连接到客户端
            var sendBytes = Encoding.UTF8.GetBytes(InputMessage);
            ClientSocket.Send(sendBytes);

            RemindMessage(ClientSocket?.LocalEndPoint?.ToString() ?? "", EndPointType.Local, InputMessage);
            Application.Current.Dispatcher.Invoke(() => { InputMessage = ""; });
        }


        /// <summary>
        /// 接收客户端发送消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private void Receive(Socket clientSocket)
        {
            byte[] data = new byte[1024 * 1024];
            while (true)
            {
                //读取客户端发送过来的数据
                int readLeng = ClientSocket.Receive(data, 0, data.Length, SocketFlags.None);

                //客户端断开连接，在客户端关闭命令发出后，会再接收到一次服务端发来的信息
                if (readLeng == 0) break;

                string content = Encoding.UTF8.GetString(data, 0, readLeng);
                RemindMessage(clientSocket?.RemoteEndPoint?.ToString() ?? "", EndPointType.Remote, content);
            }
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
