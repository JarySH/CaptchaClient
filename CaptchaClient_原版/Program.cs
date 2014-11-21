using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CaptchaClient
{
    static class Program
    {
        public static List<string> BidNumbers = new List<string>();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //新建Login窗口（Login是自己定义的Form）
            Login Log = new Login();

            //使用模式对话框方法显示Log
            Log.ShowDialog();

            //DialogResult就是用来判断是否返回父窗体的
            if (Log.DialogResult == DialogResult.OK)
            {
                //在线程中打开主窗体
                Application.Run(new MainForm());
            }            


            //Application.Run(new MainForm());
        }
    }
}
