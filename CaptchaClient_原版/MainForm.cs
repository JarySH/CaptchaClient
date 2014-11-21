using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CaptchaClient
{
    public partial class MainForm : Form
    {
        private MQObject mqObject;
        private string BidNumber;
        /// <summary>
        /// MQ连接状态
        /// -1 = 无法连接
        /// 0 = 未连接
        /// 200 = 正常
        /// 404 = 通道未找到 
        /// </summary>
        public static int MQStatus = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Program.BidNumbers.Count > 0)
            {
                BidNumber = Program.BidNumbers[0];      // 当前仅实现了1个投标号输入
                Text += "[" + BidNumber.Replace("ServerShared", "") + "]";
            }
            else
            {
                MessageBox.Show("登录状态不正确");
                return;
            }

            // 初始化日志
            LogHelper.Init();

            // 初始化MQ队列，放在线程中，如果失败则一直轮询
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        if (MQStatus == 0 && mqObject == null)
                        {
                            mqObject = new MQObject(BidNumber);
                            mqObject.receiveDelegate += ReceiveMessage;
                            mqObject.DoConsume();
                        }
                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            switch (MQStatus)
                            {
                                case 200:
                                    lbMQStatus.Text = "标书已登录，通信已建立";
                                    lbMQStatus.ForeColor = Color.Green;
                                    break;
                                case 404:
                                    lbMQStatus.Text = "标书尚未登录";
                                    lbMQStatus.ForeColor = Color.Red;
                                    break;
                                case -1:
                                    lbMQStatus.Text = "无法连接";
                                    lbMQStatus.ForeColor = Color.Red;
                                    break;
                            }                            
                        
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t[MQ初始化] " + ex.Message);
                        LogHelper.WriteLine("MQ初始化异常", "MQInit_", ex.Message);
                    }

                    Thread.Sleep(1000);
                }                
            }));
            thread.IsBackground = true;
            thread.Start();

            UDPMsg.receiveDelegate += ReceivePubInfo;

            // 发送UDP请求
            UDPMsg.SendUDPMsg("<TYPE>FORMAT</TYPE><BIDNO>" + BidNumber + "</BIDNO>");

            // 开始接收UDP
            UDPMsg.StartUDPThread();

            // 启动一个新线程作为界面轮询现成
            Thread ThreadUIUpdater = new Thread(new ThreadStart(() =>
            {
                DateTime dtNext = DateTime.Now.AddSeconds(90);  // 下一个UDP请求自动发送时间
                while (true)
                {
                    // 这个线程里加入自动发送国拍UDP请求的轮询
                    if (DateTime.Now >= dtNext)
                    {
                        UDPMsg.SendUDPMsg("<TYPE>CLIENT</TYPE><BIDNO>" + BidNumber + "</BIDNO>");
                        dtNext = DateTime.Now.AddSeconds(90);                       
                    }
                    Thread.Sleep(10000);
                }
            }));
            ThreadUIUpdater.IsBackground = true;
            ThreadUIUpdater.Start();
        }

        public delegate void ReceiveDelegate(string name);

        public void ReceiveMessage(string strMsg)
        {
            try
            {
                JObject jo = JObject.Parse(strMsg);
                string strAction = jo["Action"].ToString();
                if (strAction == "GetImageFinish")
                {
                    MemoryStream ms = new MemoryStream(Convert.FromBase64String(jo["ImageData"].ToString()));
                    Bitmap bmp = (Bitmap)Image.FromStream(ms);
                    pbCaptcha.Image = bmp;

                    mqObject.QueueServer = "Client" + jo["BidNumber"].ToString() + Utils.GenerateMachineCode(jo["BidNumber"].ToString());
                    mqObject.SN = jo["SN"].ToString();

                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        tbImageCode.Text = jo["ImageNumber"].ToString();
                        tbImageCode.Focus();
                        tbImageCode.SelectAll();

                        SetStatusText(lbActionStatus, "验证码获取成功", StatusType.Success);
                        SetStatusText(lbCodeStatus, "请立即输入验证码", StatusType.Normal);
                    }));

                    LogHelper.WriteLine("验证码接收", "Receive", strMsg);
                }
                else if (strAction == "GetImageStart")
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        lbActionStatus.Text = "正在获取验证码";
                        lbActionStatus.ForeColor = Color.Red;
                        lbCodeStatus.Text = "请准备好输入验证码";
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public delegate void PubInfoDelegate(UDPMsg msg);

        public void ReceivePubInfo(UDPMsg msg)
        {
            //Console.WriteLine(msg.RawString);

            this.BeginInvoke(new MethodInvoker(() =>
            {
                if (msg.Status == 'A' || msg.Status == 'B')
                {
                    lbSystemTime.Text = msg.SystemTime.ToString("HH:mm:ss");
                    lbCurrentPrice.Text = msg.CurrentPrice.ToString();
                    if (msg.Status == 'A')
                    {
                        lbStatus.Text = "首次出价阶段";
                        lbStatus.ForeColor = Color.Blue;
                    }
                    else
                    {
                        lbStatus.Text = "修改出价阶段";
                        lbStatus.ForeColor = Color.Red;
                    }
                }
                else if (msg.Status == 'C')
                {
                    lbStatus.Text = "非竞拍阶段";
                    lbStatus.ForeColor = Color.Black;
                    lbSystemTime.Text = "";
                    lbCurrentPrice.Text = "";
                }
                else
                {
                    SetStatusText(lbStatus, "未知状态", StatusType.Normal);
                    lbSystemTime.Text = "";
                    lbCurrentPrice.Text = "";
                }
            }));
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (pbCaptcha.Image == null)
            {
                return;
            }

            string strImageCode = tbImageCode.Text;
            if (Utils.IsNumber(strImageCode))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>()
                {
                    {"Action", "SubmitCode"},
                    {"ImageCode", strImageCode},
                    {"SN", mqObject.SN}
                };

                string json = JsonConvert.SerializeObject(dic);

                SetStatusText(lbCodeStatus, "正在提交验证码", StatusType.Normal);
                
                mqObject.SendMsg(json);     // 发送消息

                mqObject.DoAck();   // 确认上一条消息，开始接收下一条

                SetStatusText(lbCodeStatus, "验证码提交成功", StatusType.Success);

                SetStatusText(lbActionStatus, "等待服务器消息", StatusType.Normal);

                pbCaptcha.Image = null;

                tbImageCode.Text = "";

                LogHelper.WriteLine("验证码提交", "Submit", json);

            }
        }

        /// <summary>
        /// Label控件状态类型
        /// </summary>

        private enum StatusType
        {
            Success,
            Normal,
            Error
        };
        /// <summary>
        /// 设置控件文字及状态类型
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="strText">文字</param>
        /// <param name="statusType">状态类型</param>
        private void SetStatusText(Control control, string strText, StatusType statusType)
        {
            control.Text = strText;
            Color color = Color.Black;
            switch(statusType)
            {
                case StatusType.Normal:
                    color = Color.Black;
                    break;
                case StatusType.Success:
                    color = Color.Green;
                    break;
                case StatusType.Error:
                    color = Color.Red;
                    break;
            }
            control.ForeColor = color;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MQConn.Close();

            // 发送LOGOFF UDP请求
            UDPMsg.SendUDPMsg("<TYPE>LOGOFF</TYPE><BIDNO>" + BidNumber + "</BIDNO>");
        }

        private void tbImageCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 捕获回车
            if (e.KeyChar == (char)13)
            {
                btnSubmit.PerformClick();
            }
        }

        private void btnRequestUDP_Click(object sender, EventArgs e)
        {
            // 发送UDP请求
            UDPMsg.SendUDPMsg("<TYPE>CLIENT</TYPE><BIDNO>" + BidNumber + "</BIDNO>");
        }
    }
}