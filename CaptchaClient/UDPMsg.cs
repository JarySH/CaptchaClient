using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class UDPMsg
{
    public delegate void ReceiveDelegate(UDPMsg msg);
    public static ReceiveDelegate receiveDelegate = null;

    public static int LimitPriceGlobal = 0;

    /// <summary>
    /// 当前最新收到的消息对象
    /// </summary>
    public static UDPMsg CurrentMsg { get; set; }
    /// <summary>
    /// UDP的Socket连接
    /// </summary>
    public static Socket sock;
    /// <summary>
    /// 拍卖会状态，A=首次出价阶段（蓝色），B=修改出价阶段（红色），C=非投标状态（黑色）
    /// </summary>
    public Char Status { get; private set; }
    /// <summary>
    /// 拍卖会标题
    /// </summary>
    public string Title { get; private set; }
    /// <summary>
    /// 投放额度
    /// </summary>
    public Int32 Quota { get; private set; }
    /// <summary>
    /// 警示价
    /// </summary>
    public Int32 LimitPrice { get; private set; }
    /// <summary>
    /// 目前已投标人数
    /// </summary>
    public Int32 People { get; private set; }
    /// <summary>
    /// 拍卖会开始时间
    /// </summary>
    public DateTime BeginTimeFull { get; private set; }
    /// <summary>
    /// 拍卖会结束时间
    /// </summary>
    public DateTime EndTimeFull { get; private set; }
    /// <summary>
    /// 首次出价开始时间
    /// </summary>
    public DateTime BeginTimeFirst { get; private set; }
    /// <summary>
    /// 首次出价结束时间
    /// </summary>
    public DateTime EndTimeFirst { get; private set; }
    /// <summary>
    /// 修改出价开始时间
    /// </summary>
    public DateTime BeginTimeSecond { get; private set; }
    /// <summary>
    /// 修改出价结束时间
    /// </summary>
    public DateTime EndTimeSecond { get; private set; }
    /// <summary>
    /// 系统目前时间
    /// </summary>
    public DateTime SystemTime { get; private set; }
    /// <summary>
    /// 目前最低可成交价
    /// </summary>
    public Int32 CurrentPrice { get; private set; }
    /// <summary>
    /// 最低可成交价出价时间
    /// </summary>
    public DateTime LastBidTime { get; private set; }
    /// <summary>
    /// 出价区间下限
    /// </summary>
    public Int32 LimitLow { get; private set; }
    /// <summary>
    /// 出价区间上限
    /// </summary>
    public Int32 LimitHigh { get; private set; }
    /// <summary>
    /// 数据包原始数据
    /// </summary>
    public string RawString { get; private set; }
    /// <summary>
    /// 消息接收时间
    /// </summary>
    public DateTime MsgTime { get; private set; }
    /// <summary>
    /// INFO中包含的字符串（C状态）
    /// </summary>
    public string Text { get; private set; }

    public UDPMsg(string strData)
    {
        RawString = strData;
        MsgTime = DateTime.Now;

        if (!strData.Contains("<INFO>"))
        {
            return;
        }

        // 提取INFO中的内容
        string strInfo = Utils.GetTagValue(strData, "<INFO>", "</INFO>");

        Status = strInfo[0];   // 投标会状态

        strInfo = strInfo.Substring(1, strInfo.Length - 1); // 状态字符去除

        if (Status == 'C')
        {
            Text = strInfo;
        }
        else if (Status == 'A' || Status == 'B')
        {
            string[] arrInfo = strInfo.Split('^');
            Title = arrInfo[0];
            Quota = Convert.ToInt32(arrInfo[1]);
            BeginTimeFull = Utils.StringToDateTime(arrInfo[3], "HH:mm");
            EndTimeFull = Utils.StringToDateTime(arrInfo[4], "HH:mm");
            BeginTimeFirst = Utils.StringToDateTime(arrInfo[5], "HH:mm");
            EndTimeFirst = Utils.StringToDateTime(arrInfo[6], "HH:mm");
            BeginTimeSecond = Utils.StringToDateTime(arrInfo[7], "HH:mm");
            EndTimeSecond = Utils.StringToDateTime(arrInfo[8], "HH:mm");
            SystemTime = Utils.StringToDateTime(arrInfo[9], "HH:mm:ss");

            if (Status == 'A')
            {
                LimitPrice = Convert.ToInt32(arrInfo[2]);
                People = Convert.ToInt32(arrInfo[10]);
                CurrentPrice = Convert.ToInt32(arrInfo[11]);
                LastBidTime = Utils.StringToDateTime(arrInfo[12], "HH:mm:ss");

                LimitPriceGlobal = LimitPrice;
            }
            else if (Status == 'B')
            {
                LimitPrice = LimitPriceGlobal;
                People = Convert.ToInt32(arrInfo[2]);
                CurrentPrice = Convert.ToInt32(arrInfo[10]);
                LastBidTime = Utils.StringToDateTime(arrInfo[11], "HH:mm:ss");
                LimitLow = Convert.ToInt32(arrInfo[12]);
                LimitHigh = Convert.ToInt32(arrInfo[13]);
            }
        }
        else
        {
            return;
        }
    }
    public static void SendUDPMsg(string MsgText)
    {
        if (!String.IsNullOrEmpty(MsgText))
        {
            string UDPServer = "www.wolai360.com";
            //string UDPServer = "localhost";
            int UDPPort = 9999;

            IPAddress ip = Dns.GetHostAddresses(UDPServer)[0];
            EndPoint RemoteEP = (EndPoint)(new IPEndPoint(ip, UDPPort));

            Socket sock = UDPMsg.sock;
            if (sock == null)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                UDPMsg.sock = sock;
            }
            byte[] SendData = Utils.XORData(Encoding.Default.GetBytes(MsgText));
            sock.SendTo(SendData, SendData.Length, SocketFlags.None, RemoteEP);

            // 写入日志
            LogHelper.WriteLine("UDP发送", "UDP_Send_" + DateTime.Now.ToString("yyyyMMdd"), sock.LocalEndPoint.ToString() + " -> " + RemoteEP.ToString() + Environment.NewLine + MsgText.Trim() + Environment.NewLine + "------------------------------------------------------------------------------------------------------", false);
            
        }
    }

    /// <summary>
    /// 开始UDP侦听线程
    /// </summary>
    public static void StartUDPThread()
    {
        Socket sock = UDPMsg.sock;
        if (sock == null)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            UDPMsg.sock = sock;
        }

        // 启动一个新线程用于侦听
        Thread ThreadListener = new Thread(new ParameterizedThreadStart((object index) =>
        {
            while (true)
            {
                int recv = 0;
                byte[] data = new byte[1024];
                IPEndPoint LocalEP = sock.LocalEndPoint as IPEndPoint;

                if (LocalEP == null)
                {
                    continue;
                }

                string LogFile = "UDP_Receive_" + DateTime.Now.ToString("yyyyMMdd");
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\tUDP服务已启动：{0}:{1}", LocalEP.Address, LocalEP.Port);
                LogHelper.WriteLine("UDP接收", LogFile, "UDP服务已启动：" + LocalEP.Address + ":" + LocalEP.Port, false);

                while (true)
                {
                    //得到客户机IP
                    EndPoint RecvRemote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                    try
                    {
                        recv = sock.ReceiveFrom(data, ref RecvRemote);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLine("UDP接收异常", "UDP_Exception" + DateTime.Now.ToString("yyyyMMdd"), ex.Message, false);

                        Console.WriteLine("UDP接收异常：" + ex.Message);
                        continue;
                    }

                    IPEndPoint RemoteEP = RecvRemote as IPEndPoint;
                    string strMsg = Encoding.Default.GetString(data, 0, recv);
                    //Console.WriteLine(DateTime.Now + "\t收到请求: {0}:{1}", RemoteEP.Address, RemoteEP.Port);
                    //Console.WriteLine(strMsg);

                    LogHelper.WriteLine("UDP接收", LogFile, "收到请求: " + RemoteEP.Address + ":" + RemoteEP.Port + Environment.NewLine + strMsg + Environment.NewLine + "------------------------------------------------------------------------------------------------------", false);

                    // 以下为SQLite代码
                    //string sql = String.Format("insert into [tbl_Receive] (Msg, LocalEP, RemoteEP, CreateTime) values ('{0}', '{1}', '{2}', '{3}')", strMsg.Trim(), LocalEP.ToString(), RemoteEP.ToString(), DateTime.Now.ToString("s"));                        
                    //SQLiteHelper.ExecuteNonQuery(sql);

                    try
                    {
                        UDPMsg msg = new UDPMsg(strMsg);
                        if (UDPMsg.CurrentMsg == null)
                        {
                            UDPMsg.CurrentMsg = msg;
                        }
                        else
                        {
                            if (msg.Status == 'C' || msg.SystemTime > UDPMsg.CurrentMsg.SystemTime)
                            {
                                UDPMsg.CurrentMsg = msg;
                            }
                        }
                        receiveDelegate(msg);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLine("UDP消息错误", "UDP_Error_" +DateTime.Now.ToString("yyyyMMdd") , ex.Message + Environment.NewLine + strMsg, false);
                    }
                }
            }
        }));
        ThreadListener.IsBackground = true;
        ThreadListener.Start();
    }
}