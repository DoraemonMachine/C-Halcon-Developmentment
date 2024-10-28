using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Single_Form
{
    /// <summary>
    /// 串口类
    /// </summary>
    class MySeeriaPort
    {
        //队列
        public Queue<string> mQ = new Queue<string>();
        private SerialPort Myport = null;

        public MySeeriaPort()
        {
            // 串口名
            string portName = "COM1";
            //波特率
            int baudRate = 9600;
            //校验位
            Parity parity = Parity.None;
            //数据位
            int dataBits = 8;
            //停止位
            StopBits stopBits = StopBits.One;
            Myport = new SerialPort(portName, baudRate, parity, dataBits, stopBits); 
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Opencom()
        {
            try
            {
                if (!Myport.IsOpen)
                {
                    //打开串口
                    Myport.Open();
                    Myport.DataReceived += new SerialDataReceivedEventHandler(Recivemessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Closecom()
        {
            if (Myport.IsOpen)
            {
                //关闭串口
                Myport.Close();
                Myport.DataReceived -= new SerialDataReceivedEventHandler(Recivemessage);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Sendmessage(string str)
        {
            //调用字符表
            byte[] buffer = Encoding.Default.GetBytes(str);
            Myport.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void Recivemessage(object sender, SerialDataReceivedEventArgs e)
        {
            int l = Myport.BytesToRead; //获取接收缓冲区中数据的字节数。

            byte[] buffer = new byte[l];

            Myport.Read(buffer, 0, l); //输入缓冲区读取一些字节并将那些字节写入字节数组中指定的偏移量处

            string str = Encoding.Default.GetString(buffer); //将字节数组转成字符串

            mQ.Enqueue(str);
        }
    }
}
