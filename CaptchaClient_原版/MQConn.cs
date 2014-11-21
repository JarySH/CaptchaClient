using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public class MQConn
{
    private static ConnectionFactory factory = new ConnectionFactory() { HostName = "www.wolai360.com", UserName = "client", Password = "wlcl360", RequestedHeartbeat = 10 };
    private static IConnection connection;
    public static IConnection Connection
    {
        get
        {
            // 不断重试建立连接
            while (true)
            {
                try
                {
                    // 实例化时进行连接
                    if (connection == null || !connection.IsOpen)
                    {
                        connection = factory.CreateConnection();    // 建立连接
                    }
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLine("MQConn", "MQConn", ex.Message);
                    Console.WriteLine("[MQConn]" + ex.Message);
                    Thread.Sleep(300);
                }                
            }

            return connection;
        }
        set
        {
            connection = value;
        }
    }

    /// <summary>
    /// 关闭并清空MQ连接
    /// </summary>
    public static void Close()
    {
        if (connection != null && connection.IsOpen)
        {
            connection.Close();
        }
        connection = null;
    }
}