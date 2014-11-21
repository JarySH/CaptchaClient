using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class HttpHelper
{
    /// <summary>  
    /// 创建GET方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="timeout">请求的超时时间</param>
    /// <returns></returns>  
    public static string GetHttpResponseString(string url, Encoding encoding, int timeout = 30000)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        ServicePointManager.ServerCertificateValidationCallback =
                delegate { return true; };
        //request.ServicePoint.BindIPEndPointDelegate = delegate { return new IPEndPoint(IPAddress.Parse("192.168.1.188"), 0); };
        request.Method = "GET";
        //request.ContentType = "text/html";
        //request.Accept = "text/html, */*";
        //request.UserAgent = "Mozilla/3.0 (compatible; Indy Library)";
        request.Proxy = null;
        request.Timeout = timeout;

        //if (cookies != null)
        //{
        //    request.CookieContainer = new CookieContainer();
        //    request.CookieContainer.Add(cookies);
        //}

        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
        {
            return reader.ReadToEnd();
        }
    }

    public static string GetHttpResponseString(string url, int timeout = 30000)
    {
        return GetHttpResponseString(url, Encoding.Default, timeout);
    }

    /// <summary>  
    /// 创建GET方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="timeout">请求的超时时间</param>
    /// <returns>HttpWebRequest对象</returns>  
    public static HttpWebRequest GetHttpWebRequest(string url, int timeout = 30000)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        ServicePointManager.ServerCertificateValidationCallback =
                delegate { return true; };
        //request.ServicePoint.BindIPEndPointDelegate = delegate { return new IPEndPoint(IPAddress.Parse("192.168.1.188"), 0); };
        request.Method = "GET";
        //request.ContentType = "text/html";
        //request.Accept = "text/html, */*";
        //request.UserAgent = "Mozilla/3.0 (compatible; Indy Library)";
        request.Proxy = null;
        request.Timeout = timeout;

        //if (cookies != null)
        //{
        //    request.CookieContainer = new CookieContainer();
        //    request.CookieContainer.Add(cookies);
        //}

        return request;
    }
}