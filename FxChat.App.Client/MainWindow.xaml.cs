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

namespace FxChat.App.Client
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

        Socket socketSend;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
      
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //创建负责通讯的Socket
                //第一步创建一个开始监听的Socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //第二步创建Ip地址和端口号对象
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint point = new IPEndPoint(ip, 15000);
                socketSend.Connect(point);//通过ip 端口号定位一个要连接的服务器端

                Thread t = new Thread(Recive);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        void Recive()
        {
            while (true)
            {
                try
                {
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
                        string str = Encoding.UTF8.GetString(SocketHelper.Intanter.RemoveFbyte(buffer), 0, r - 1);
                        textBox.AppendText("\r\n");
                        textBox.AppendText(str);
                    }
                    //else if (buffer[0] == 1)
                    //{
                    //    string ImageName = SocketHelper.Intanter.ShowImgByByte(SocketHelper.Intanter.RemoveFbyte(buffer));
                    //    AddCbItem(ImageName);
                    //}

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string msg = $"  " + this.tb_name.Text + " ：" + this.rtb_sendmsg.Text.Trim();
                socketSend.Send(SocketHelper.Intanter.SendMessageToClient(sendTextBox.Text.Trim(), SocketHelper.MessageType.news));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
