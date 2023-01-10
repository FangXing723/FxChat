using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FxChat.App.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //循环监听连接的客户端
        Socket socketSend;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //第一步创建一个开始监听的Socket
                Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //第二步创建Ip地址和端口号对象
                IPAddress ip = IPAddress.Any;
                IPEndPoint point = new IPEndPoint(ip, 15000);
                //第三步让监听的Socket绑定Ip地址跟端口号
                socketWatch.Bind(point);
                //设置监听队列
                socketWatch.Listen(10);

                Thread t = new Thread(Listen);
                t.IsBackground = true;
                t.Start(socketWatch);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                //负责监听的Socket来接收客户端的连接，创建跟客户通讯的Socket
                socketSend = socketWatch.Accept();
                //将远程的连接客服端存储字典集合
                SocketHelper.Intanter.dicScoket.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
                //添加地址到下拉框与list集合中去
                connectList.Items.Add(socketSend.RemoteEndPoint.ToString());

                textBox.AppendText(socketSend.RemoteEndPoint.ToString() + "连接成功");

                Thread t = new Thread(Recive);
                t.IsBackground = true;
                t.Start(socketSend);
            }
        }

        //循环接收发送过来的数据
        void Recive(object o)
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                try
                {
                    string str = "";
                    //客户端连接成功后，服务器接收客服端发送过来的数据
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    //实际接收的有效字节
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    if (buffer[0] == 0)
                    {
                        str = socketSend.RemoteEndPoint.ToString() + ":\r\n" + "   " + Encoding.UTF8.GetString(SocketHelper.Intanter.RemoveFbyte(buffer), 0, r);

                    }
                    //else if (buffer[0] == 1)
                    //{
                    //    str = socketSend.RemoteEndPoint.ToString() + ":\r\n" + "   " + SocketHelper.Intanter.ShowImgByByte(SocketHelper.Intanter.RemoveFbyte(buffer));

                    //    SocketHelper.Intanter.SendMessage(buffer);
                    //}
                    byte[] newbuffer = SocketHelper.Intanter.SendMessageToClient(str);
                    SocketHelper.Intanter.SendMessage(newbuffer);

                    textBox.AppendText("\r\n");
                    textBox.AppendText(socketSend.RemoteEndPoint.ToString() + ":" + str);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            socketSend.Send(SocketHelper.Intanter.SendMessageToClient(sendTextBox.Text.Trim(), SocketHelper.MessageType.news));
        }
    }
}
