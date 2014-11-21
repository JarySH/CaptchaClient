using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

class LogHelper
{
    private static Thread thread;
    private static DataTable LogTable = null;

    /// <summary>
    /// 写日志文件
    /// </summary>
    /// <param name="Event">日志事件名称</param>
    /// <param name="FileName">文件名（无需包含后缀名）</param>
    /// <param name="strText">记录内容</param>
    /// <param name="AddTicksSuffix">是否添加DateTime.Now.Ticks作为文件名后缀</param>
    public static void WriteLine(string Event, string FileName, string Text, bool AddTicksSuffix = true)
    {
        lock (LogTable)
        {
            // 将日志请求写入LogTable，进入队列等待线程处理
            DataRow drNew = LogTable.NewRow();
            drNew["Event"] = Event;
            drNew["FileName"] = FileName;
            drNew["Text"] = Text;
            drNew["AddTicksSuffix"] = AddTicksSuffix;
            LogTable.Rows.Add(drNew);
        }
    }

    /// <summary>
    /// 初始化LogTable和线程
    /// </summary>
    public static void Init()
    {
        if (LogTable == null)
        {
            LogTable = new DataTable();
            LogTable.Columns.AddRange(new DataColumn[]{
                new DataColumn("Event"),
                new DataColumn("FileName"),
                new DataColumn("Text"),
                new DataColumn("AddTicksSuffix")
            });
        }

        if (thread == null)
        {   // 启动线程，不影响原有程序运行
            thread = new Thread(new ThreadStart(() =>
            {
                // 该线程保持循环，不退出
                while (true)
                {
                    lock (LogTable)
                    {
                        if (LogTable.Rows.Count > 0)
                        {
                            DataRow dr = LogTable.Rows[0];
                            // 当出现异常时（如文件锁定），等待后不断重试
                            while (true)
                            {
                                try
                                {
                                    string Dir = Environment.CurrentDirectory + "/Logs/";
                                    if (!Directory.Exists(Dir))
                                    {
                                        Directory.CreateDirectory(Dir);
                                    }

                                    string strFullName = Dir + dr["FileName"];
                                    if (dr["AddTicksSuffix"].ToString() == "True")
                                    {
                                        strFullName += "_" + DateTime.Now.Ticks;
                                    }
                                    strFullName += ".txt";

                                    File.AppendAllText(strFullName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t[" + dr["Event"] + "] " + dr["Text"] + Environment.NewLine + Environment.NewLine);
                                    //Console.WriteLine("[" + dr["Event"] + "]" + dr["Text"]);

                                    // 从LogTable中删除本行
                                    LogTable.Rows.Remove(dr);

                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("写入日志异常正在重试：" + ex.Message);
                                    Thread.Sleep(50);
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }));
            thread.IsBackground = true;
            thread.Name = "日志线程";
            thread.Start();
        }
    }
}