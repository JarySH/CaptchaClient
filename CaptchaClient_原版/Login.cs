using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


public partial class Login : Form
{
    public Login()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        string strUserName = tbUserName.Text;
        string strUserPwd = tbUserPwd.Text;
        string strMachineName = System.Environment.MachineName;

        if (String.IsNullOrEmpty(strUserName))
        {
            MessageBox.Show("请输入用户名");
            tbUserName.Focus();
            return;
        }


        if (String.IsNullOrEmpty(strUserPwd))
        {
            MessageBox.Show("请输入密码");
            tbUserPwd.Focus();
            return;
        }
       
        // 拼装QueryString
        string strQuery = String.Empty;
        strQuery += "&UserName=" + Utils.UrlEncode(strUserName);
        strQuery += "&UserPwd=" + Utils.UrlEncode(strUserPwd);
        strQuery += "&MachineName=" + Utils.UrlEncode(strMachineName);

        string strUrl = "http://api.wolai360.com/API.aspx?Action=ClientLogin" + strQuery;
        //string strUrl = "http://localhost:12162/PaiPai/ClientLogin.aspx?" + strQuery;

        btnLogin.Enabled = false;
        Thread thread = new Thread(new ThreadStart(() =>
        {
            try
            {
                string response = HttpHelper.GetHttpResponseString(strUrl);

                JObject jo = JObject.Parse(response);

                if (jo["error"] != null)
                {
                    if (jo["error"].ToString() == "ok")
                    {
                        // 获取投标号
                        string[] BidNumbers = jo["BidNumbers"].ToString().Split(',');
                        CaptchaClient.Program.BidNumbers = BidNumbers.ToList();

                        // 关闭登录窗口，打开主窗口
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("登录失败：" + jo["error"]);
                        Console.WriteLine(response);
                    }
                }
                else
                {
                    MessageBox.Show("登录失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录异常：" + ex.Message);
            }

            this.BeginInvoke(new MethodInvoker(() =>
            {
                btnLogin.Enabled = true;
            }));
        }));
        thread.IsBackground = true;
        thread.Start();

        
    }

    private void tbUserPwd_KeyPress(object sender, KeyPressEventArgs e)
    {
        // 捕获回车
        if (e.KeyChar == (char)13)
        {
            btnLogin.PerformClick();
        }
    }
}