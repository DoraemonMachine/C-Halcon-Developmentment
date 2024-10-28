using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Single_Form
{
    /// <summary>
    /// 网口类
    /// </summary>
    class Network
    {       
        //队列
        public Queue<string> mQ = new Queue<string>();
        private Socket socketliListen = null;
        Thread threadciver = null;//接收线程
        public Network()
        {
            socketliListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 打开网口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Port"></param>
        public void Open(IPAddress ip, int Port)
        {
            try
            {
                IPEndPoint ipport = new IPEndPoint(ip, Port);
                socketliListen.Connect(ipport);
                threadciver = new Thread(accept);
                threadciver.IsBackground = true;
                threadciver.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 关闭网口
        /// </summary>
        public void Close()
        {
            try
            {
                socketliListen.Close();
                threadciver.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void accept()
        {
            //Socket socketReciver = (Socket)s;//线程传参数
            while (true)
            {
                byte[] buffer = new byte[1024];
                int i = socketliListen.Receive(buffer);//返回值可以确定字节长度
                if (i > 0)//判断接收数据是否大于零
                {
                    string str = Encoding.Default.GetString(buffer, 0, i);//0代表从第零个字节开始，到第i个结束，这样就不会浪费内存
                    mQ.Enqueue(str);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        public void Sendmessage(string str)
        {
            socketliListen.Send(Encoding.Default.GetBytes(str));
        }

    }
}
