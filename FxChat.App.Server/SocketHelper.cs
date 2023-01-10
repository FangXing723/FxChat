using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FxChat.App.Server
{
    public class SocketHelper
    {
        //将远程连接过来的客服端的IP地址和Socket存入集合
        public static SocketHelper Intanter = new SocketHelper();
        public Dictionary<string, Socket> dicScoket = new Dictionary<string, Socket>();
        public List<string> IPItem = new List<string>();
         
        public object ImageFormat { get; private set; }

        //消息类型枚举
        public enum MessageType
        {
            news,
            picture
        }

        /// <summary>
        ///  服务端接收数据后将数据发送给所有的客户端
        /// </summary>
        /// <param name="buffer">发送的消息字节</param>
        /// <param name="ms">消息类型</param>
        /// <returns></returns>
        public void SendMessage(byte[] buffer)
        {

            //获得用户在下拉框的IP地址
            var task1 = new Task(() =>
            {
                for (int i = 0; i < IPItem.Count; i++)
                {
                    string ip = IPItem[i].ToString();
                    dicScoket[ip].Send(buffer.ToArray());
                }
            });
            task1.Start();
        }
        /// <summary>
        ///  将消息转换成消息协议格式
        /// </summary>
        /// <param name="buffer">发送的消息字节</param>
        /// <param name="ms">消息类型</param>
        /// <returns></returns>
        public byte[] SendMessageToClient(string message, MessageType ms = MessageType.news)
        {
            List<byte> newbuffer = new List<byte>();
            byte[] buffer = new byte[0];
            switch (ms)
            {
                case MessageType.news:
                    newbuffer.Add(0);
                    buffer = Encoding.UTF8.GetBytes(message);
                    break;
                case MessageType.picture:
                    newbuffer.Add(1);
                    buffer = SaveImage(message);
                    break;
                default:
                    break;
            }
            newbuffer.AddRange(buffer);
            return newbuffer.ToArray();
        }
        public byte[] RemoveFbyte(byte[] buffer)
        {
            List<byte> newbuffer = buffer.ToList();
            newbuffer.RemoveAt(0);
            return newbuffer.ToArray();
        }
        /// <summary>
        /// 将图片以二进制流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] SaveImage(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }
        ///// <summary>
        ///// 现实二进制流代表的图片
        ///// </summary>
        ///// <param name="imgBytesIn"></param>
        //public string ShowImgByByte(byte[] imgBytesIn)
        //{
        //    lock (this)
        //    {
        //        string NewImageName = DateTime.Now.ToString("yyyy-mm-dd hh-mm-ss") + ".jpg";//ImageName(CenterId);//获得图片的名字
        //        string ImagePath = Environment.CurrentDirectory + $"\\Images\\";//@"F:/AQPXImageURL/" + NewImageName.ToString() + ".jpg";
        //        if (!Directory.Exists(ImagePath))
        //        {
        //            DirectoryInfo directoryInfo = new DirectoryInfo(ImagePath);
        //            directoryInfo.Create();
        //        }
        //        ImagePath += NewImageName.ToString();
        //        if (File.Exists(ImagePath))
        //        {
        //            return NewImageName;
        //        }
        //        MemoryStream ms = new MemoryStream(imgBytesIn);
        //        Bitmap bmp = new Bitmap(ms);
        //        bmp.Save(ImagePath, ImageFormat.Bmp);
        //        ms.Close();
        //        return NewImageName;
        //    }
        //    //return NewImageName;

        //    //pictureBox1.Image = Image.FromStream(ms);
        //}

    }

}
