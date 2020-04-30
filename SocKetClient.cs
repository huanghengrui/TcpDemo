using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public class SocKetClient
    {
        public Socket c_socket;
        public IPEndPoint point = null;
        public IAsyncResult asyncResult;
        public int socKetPwd = 0;   //通讯密码
        public const int PROTOCOLKEY = 0x18181818;  //协议token
        public byte[] Header = new byte[32];  //头协议
        public const int LEN = 4;
        public const int HEADERLEN = 32;

        public int ReceiveBufferSize = 409600; //接收的包的最大容量
        
        public TextBox reciveText = new TextBox();

        public string rfile = System.AppDomain.CurrentDomain.BaseDirectory + "ReciveData.txt"; //保存单次收到的json数据
        public string sfile = System.AppDomain.CurrentDomain.BaseDirectory + "SendData.txt";   //保存单次发送的json数据
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="connetIp"></param>
        /// <param name="connetPort"></param>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public int Start(string connetIp, int connetPort, int pwd, TextBox textBox = null)
        {
            int index = 0;
            
            try
            {
                socKetPwd = pwd;
                reciveText = textBox;
                //创建通信的Socket
                c_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(connetIp);
                point = new IPEndPoint(ip, connetPort);
               
                c_socket.SendTimeout = 20000;
                c_socket.ReceiveTimeout = 20000;
         
                //连接到对应的IP地址和端口号
                c_socket.Connect(point);
                
                index = 1;        
            }
            catch (Exception E)
            {
                index = -1;
                reciveText.AppendText("Connection failed!\r\n" + E.Message);
            }
            return index;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if(c_socket != null)
            {
                if (c_socket.Connected)
                {
                    c_socket.Shutdown(SocketShutdown.Both);
                    c_socket.Close();
                }
            }
            
        }

        /// <summary>
        /// 添加头数据到要发送的内容的前面，默认32字节
        /// </summary>
        /// <param name="sendBuff"></param>
        /// <returns></returns>
        public byte[] GetSendBuff(byte[] sendBuff)
        {
            byte[] newSendBuff = new byte[HEADERLEN + sendBuff.Length];
            byte[] buf = new byte[4];
            if (ConvertIntToByteArray(sendBuff.Length, ref buf))
                Buffer.BlockCopy(buf, 0,
                                Header, 0,
                                LEN);
            if (ConvertIntToByteArray(PROTOCOLKEY, ref buf))
                Buffer.BlockCopy(buf, 0,
                                Header, 4,
                                LEN);
            if (ConvertIntToByteArray(socKetPwd, ref buf))
                Buffer.BlockCopy(buf, 0,
                                Header, 8,
                                LEN);
            Buffer.BlockCopy(Header, 0,
                               newSendBuff, 0,
                               HEADERLEN);
            Buffer.BlockCopy(sendBuff, 0,
                               newSendBuff, HEADERLEN,
                               sendBuff.Length);
            return newSendBuff;
        }

        /// <summary>
        /// int类型转为byte[]类型
        /// </summary>
        /// <param name="m"></param>
        /// <param name="arry"></param>
        /// <returns></returns>
        public static bool ConvertIntToByteArray(Int32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;

            arry[0] = (byte)(m & 0xFF);
            arry[1] = (byte)((m & 0xFF00) >> 8);
            arry[2] = (byte)((m & 0xFF0000) >> 16);
            arry[3] = (byte)((m >> 24) & 0xFF);

            return true;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SendData(byte[] buffer, ref StringBuilder jsonStringBuilder)
        {
            int index = 0;
            try
            { 
                #region 保存发送的数据
                FileStream fsWrite = null;
                if (!File.Exists(sfile))
                {
                    using (fsWrite = new FileStream(sfile, FileMode.Create))
                    {
                        fsWrite.Write(buffer, 32, buffer.Length - 32);
                    }
                    fsWrite.Close();
                }
                else
                {
                    using (fsWrite = new FileStream(sfile, FileMode.Append))
                    {
                        fsWrite.Write(buffer, 32, buffer.Length - 32);
                    }
                    fsWrite.Close();
                }
                #endregion
               
                try
                {
                    c_socket.Send(buffer);
                }
                catch
                {
                    return -1;
                }
                Thread.Sleep(100);
                Recive(ref jsonStringBuilder);
                index = 1;
               
            }
            catch (Exception E)
            {
                
                index = -1;
                reciveText.AppendText(E.Message);
            }
            finally
            {
                Close();
            }
            return index;
        }
        /// <summary>
        /// 接收信息
        /// </summary>
        public void Recive(ref StringBuilder jsonStringBuilder)
        {
            int numberValidBytes = 0;
            byte[] buffer = null;
            byte[] newbuffer = null;
            int bufCount = 0;
            int bufRecCount = 0;
            string token = "";
            string reciveData = "";
            FileStream fsWrite = null;
            while (true)
            {
                try
                {
                   
                    if (!c_socket.Connected)
                    {
                        break;
                    }
                    buffer = new byte[ReceiveBufferSize];
                    numberValidBytes = c_socket.Receive(buffer); 
                    //实际接收到的有效字节数
                    if (numberValidBytes <= 0)
                    {
                        break;
                    }
                    else      //表示收到的是数据
                    {

                        if (jsonStringBuilder.Length < 1)
                        {
                            if (numberValidBytes >= 32)
                            {
                                byte[] bufLength = new byte[4];
                                Array.Copy(buffer, 4, bufLength, 0, 4);
                                token = bufLength[0].ToString("X") + bufLength[1].ToString("X") + bufLength[2].ToString("X") + bufLength[3].ToString("X");

                                if (token == "18181818")     //对应0x18181818
                                {
                                    Array.Copy(buffer, 0, bufLength, 0, 4);
                                    bufCount = BitConverter.ToInt32(bufLength, 0);
                                    if (numberValidBytes > 32)
                                    {
                                        numberValidBytes = numberValidBytes - 32;
                                        newbuffer = new byte[numberValidBytes];
                                        Array.Copy(buffer, 32, newbuffer, 0, numberValidBytes);
                                        buffer = newbuffer;
                                    }
                                    else
                                        continue;
                                }
                            }
                        }
                        reciveData = Encoding.UTF8.GetString(buffer, 0, numberValidBytes);
                        bufRecCount += numberValidBytes;
                        if (reciveData.Length > 0)
                        {
                            #region 保存数据到根目录的TXT文件
                            if (!File.Exists(rfile))
                            {
                                using (fsWrite = new FileStream(rfile, FileMode.Create))
                                {
                                    fsWrite.Write(buffer, 0, numberValidBytes);
                                }
                                fsWrite.Close();
                            }
                            else
                            {
                                using (fsWrite = new FileStream(rfile, FileMode.Append))
                                {
                                    fsWrite.Write(buffer, 0, numberValidBytes);
                                }
                                fsWrite.Close();
                            }
                            #endregion
                            if (jsonStringBuilder.Length < 1)
                            {
                                jsonStringBuilder = new StringBuilder(reciveData);
                            }
                            else
                            {
                                jsonStringBuilder.Append(reciveData);
                            }

                            if (bufCount == bufRecCount) //接收一包数据完成
                            {
                                break;
                            }
                        }
                    }
                }
                catch(Exception E)
                {
                    reciveText.AppendText("The machine has been disconnected!\r\n" + E.Message);
                    break;
                }
            }
        }
    }
}
