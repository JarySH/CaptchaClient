using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CaptchaClient;

public class MQObject
{
    public string BidNumber { get; set; }
    
    private IModel channel;
    public IModel Channel
    {
        get
        {
            // 不断重试建立连接
            while (true)
            {
                try
                {
                    // 实例化时进行连接
                    if (channel == null || channel.IsClosed)
                    {
                        channel = MQConn.Connection.CreateModel();
                    }
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine("MQChannel", "MQChannel", ex.Message);
                    Console.WriteLine("[MQChannel]" + ex.Message);
                    Thread.Sleep(300);
                }
            }
            return channel;
        }
        set
        {
            channel = value;
        }
    }
    private Thread threadReceive;
    public string QueueServer;
    public string QueueClient;
    public delegate void ReceiveDelegate(string strMsg);
    public ReceiveDelegate receiveDelegate = null;
    private BasicDeliverEventArgs ea;
    public string SN { get; set; }

    public MQObject(string strBidNumber)
    {
        BidNumber = strBidNumber;

        if (BidNumber.StartsWith("ServerShared"))
        {
            QueueServer = String.Empty;
            QueueClient = BidNumber;
        }
        else
        {
            // 客户端通道名称与服务器相反
            QueueServer = "Client" + BidNumber + Utils.GenerateMachineCode(BidNumber);
            QueueClient = "Server" + BidNumber + Utils.GenerateMachineCode(BidNumber);           
        }

        // 客户端关闭了创建通道的权限，以下弃用
        //channel.QueueDeclare(QueueServer, false, false, false, null);   // 创建发送通道
        //channel.QueueDeclare(QueueClient, false, false, false, null);   // 创建接收通道
        
    }

    public void SendMsg(string strMsg)
    {
        var body = Encoding.UTF8.GetBytes(strMsg);

        Channel.BasicPublish("", QueueServer, null, body);
    }

    public string GetMsg()
    {
        var result = Channel.BasicGet(QueueClient, true);
        if (result == null)
        {
            return null;
        }
        else
        {
            return System.Text.UTF8Encoding.UTF8.GetString(result.Body);
        }
    }

    public void DoConsume()
    {
        // 独立通道启动接收消息线程，共享通道使用GetMsg方法单个获取消息
        threadReceive = new Thread(new ThreadStart(() =>
        {
            QueueingBasicConsumer consumer = null;
            bool Init = true;
            while (true)
            {
                try
                {                    
                    // 首次载入或通道关闭，则重新初始化
                    if (Init)
                    {
                        bool isNoAck = false;
                        if (!BidNumber.StartsWith("ServerShared"))
                        {
                            isNoAck = true;
                        }
                        else
                        {
                            Channel.BasicQos(0, 1, false);
                        }
                        consumer = new QueueingBasicConsumer(Channel);
                        Channel.BasicConsume(QueueClient, isNoAck, consumer);
                        Init = false;

                        Console.WriteLine("开始Consume");
                    }

                    MainForm.MQStatus = 200;    // MQ状态为已连接

                    if (ea != null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t[接收异常] " + ex.Message);
                    LogHelper.WriteLine("MQ接收异常", "MQReceive_", ex.Message);

                    if (ex.Message.Contains("NOT_FOUND - no queue"))
                    {
                        MainForm.MQStatus = 404;    // MQ状态为队列未找到
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                    
                    Init = true;    // 出现异常则重新初始化
                    continue;
                }

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                receiveDelegate(message);

                // 如果是独立通道，则需要把ea清空，否则循环将被跳过
                if (!BidNumber.StartsWith("ServerShared"))
                {
                    ea = null;
                }
            }
        }));
        threadReceive.IsBackground = true;
        threadReceive.Start();
    }

    public void DoAck()
    {
        if (ea != null)
        {
            Channel.BasicAck(ea.DeliveryTag, false);
            Console.WriteLine("Ack: " + ea.DeliveryTag);
            ea = null;
        }
    }
}