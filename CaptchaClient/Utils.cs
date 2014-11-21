using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


class Utils
{
    /// <summary>
    /// 判断是否为数字
    /// </summary>
    /// <param name="strNumber"></param>
    /// <returns></returns>
    public static bool IsNumber(string strNumber)
    {
        if (String.IsNullOrEmpty(strNumber))
        {
            return false;
        }

        for (int i = 0; i < strNumber.Length; i++)
        {
            if (strNumber[i] < '0' || strNumber[i] > '9')
                return false;
        }
        return true;
    }

    public static string Execute(string dosCommand, int outtime = 0)
    {
        string output = "";
        if (dosCommand != null && dosCommand != "")
        {
            Process process = new Process();//创建进程对象
            ProcessStartInfo startinfo = new ProcessStartInfo();//创建进程时使用的一组值，如下面的属性
            startinfo.FileName = "cmd.exe";//设定需要执行的命令程序
            //以下是隐藏cmd窗口的方法
            startinfo.Arguments = "/c" + dosCommand;//设定参数，要输入到命令程序的字符，其中"/c"表示执行完命令后马上退出
            startinfo.UseShellExecute = false;//不使用系统外壳程序启动
            startinfo.RedirectStandardInput = false;//不重定向输入
            startinfo.RedirectStandardOutput = true;//重定向输出，而不是默认的显示在dos控制台上
            startinfo.CreateNoWindow = true;//不创建窗口
            process.StartInfo = startinfo;

            try
            {
                if (process.Start())//开始进程
                {
                    if (outtime == 0)
                    { process.WaitForExit(); }
                    else
                    { process.WaitForExit(outtime); }
                    output = process.StandardOutput.ReadToEnd();//读取进程的输出
                }
            }
            catch
            {

            }
            finally
            {
                if (process != null)
                { process.Close(); }
            }
        }
        return output;
    }

    /// <summary>
    /// 32位MD5编码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetMd5Hash(string input)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] bytValue, bytHash;
        bytValue = System.Text.Encoding.UTF8.GetBytes(input);
        bytHash = md5.ComputeHash(bytValue);
        md5.Clear();
        string sTemp = "";
        for (int i = 0, lenth = bytHash.Length; i < lenth; i++)
        {
            sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return sTemp.ToLower();
    }

    /// <summary>
    /// 获得字符串中开始和结束字符串中间得值
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="s">开始</param>
    /// <param name="e">结束</param>
    /// <returns></returns> 
    public static string GetTagValue(string str, string s, string e)
    {
        Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
        return rg.Match(str).Value;
    }

    /// <summary>
    /// 获取标签中的值，不带标签括号，如&lt;Name&gt;中的Name为TagName
    /// </summary>
    /// <param name="str"></param>
    /// <param name="tagName"></param>
    /// <returns></returns>
    public static string GetTagValue(string Str, string TagName)
    {
        string tagStart = "<" + TagName + ">";
        string tagEnd = "</" + TagName + ">";
        return GetTagValue(Str, tagStart, tagEnd);
    }

    /// <summary>
    /// 异或byte[]数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] XORData(byte[] data)
    {
        byte[] buffer = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            buffer[i] = (byte)(data[i] ^ 0xFF);
        }
        return buffer;
    }

    /// <summary>
    /// 将字符串转换为DateTime
    /// </summary>
    /// <param name="strDateTime"></param>
    /// <param name="Format">字符串格式，如yyyyMMddHHmmss</param>
    /// <returns></returns>
    public static DateTime StringToDateTime(string strDateTime, string Format)
    {
        DateTime dt;
        try
        {
            dt = DateTime.ParseExact(strDateTime, Format, System.Globalization.CultureInfo.CurrentCulture);
        }
        catch
        {
            dt = DateTime.MinValue;
        }
        return dt;
    }

    /// <summary> 
    /// 获取第一个可用的端口号 
    /// </summary> 
    /// <returns></returns> 
    public static int GetFirstAvailablePort()
    {
        int MAX_PORT = 65535; //系统tcp/udp端口数最大是65535 
        int BEGIN_PORT = 50000;//从这个端口开始检测 

        for (int i = BEGIN_PORT; i < MAX_PORT; i++)
        {
            if (PortIsAvailable(i)) return i;
        }

        return -1;
    }

    /// <summary> 
    /// 获取操作系统已用的端口号 
    /// </summary> 
    /// <returns></returns> 
    public static IList PortIsUsed()
    {
        //获取本地计算机的网络连接和通信统计数据的信息 
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

        //返回本地计算机上的所有Tcp监听程序 
        IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

        //返回本地计算机上的所有UDP监听程序 
        IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

        //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。 
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        IList allPorts = new ArrayList();
        foreach (IPEndPoint ep in ipsTCP) allPorts.Add(ep.Port);
        foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
        foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

        return allPorts;
    }

    /// <summary> 
    /// 检查指定端口是否已用
    /// </summary> 
    /// <param name="port"></param> 
    /// <returns></returns> 
    public static bool PortIsAvailable(int port)
    {
        bool isAvailable = true;

        IList portUsed = PortIsUsed();

        foreach (int p in portUsed)
        {
            if (p == port)
            {
                isAvailable = false; break;
            }
        }

        return isAvailable;
    }

    /// <summary>
    /// 生成机器码
    /// </summary>
    /// <param name="strBidNumber"></param>
    /// <param name="strBidPassword"></param>
    /// <returns></returns>
    public static string GenerateMachineCode(string strBidNumber, string strBidPassword)
    {
        int BidNumber = Convert.ToInt32(strBidNumber);
        int BidPassword = Convert.ToInt32(strBidPassword);

        string strMachineCode = Convert.ToString((BidNumber * 10000 + BidPassword), 16).ToUpper();
        return strMachineCode;
    }

    /// <summary>
    /// 生成机器码
    /// </summary>
    /// <param name="strBidNumber"></param>
    /// <returns></returns>
    public static string GenerateMachineCode(string strBidNumber)
    {
        int BidNumber = Convert.ToInt32(strBidNumber);

        string strMachineCode = Convert.ToString((BidNumber), 16).ToUpper();
        return strMachineCode;
    }

    public static string UrlEncode(string str)
    {
        StringBuilder sb = new StringBuilder();
        byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
        for (int i = 0; i < byStr.Length; i++)
        {
            sb.Append(@"%" + Convert.ToString(byStr[i], 16));
        }

        return (sb.ToString());
    }
}