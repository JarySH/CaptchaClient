namespace CaptchaClient
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSubmit = new System.Windows.Forms.Button();
            this.pbCaptcha = new System.Windows.Forms.PictureBox();
            this.tbImageCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRequestUDP = new System.Windows.Forms.Button();
            this.lbActionStatus = new System.Windows.Forms.Label();
            this.lbCurrentPrice = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbSystemTime = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbCodeStatus = new System.Windows.Forms.Label();
            this.lbMQStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbCaptcha)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(76, 128);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "回车提交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // pbCaptcha
            // 
            this.pbCaptcha.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pbCaptcha.Location = new System.Drawing.Point(53, 34);
            this.pbCaptcha.Name = "pbCaptcha";
            this.pbCaptcha.Size = new System.Drawing.Size(113, 33);
            this.pbCaptcha.TabIndex = 1;
            this.pbCaptcha.TabStop = false;
            // 
            // tbImageCode
            // 
            this.tbImageCode.Font = new System.Drawing.Font("Arial", 21F, System.Drawing.FontStyle.Bold);
            this.tbImageCode.Location = new System.Drawing.Point(53, 73);
            this.tbImageCode.MaxLength = 6;
            this.tbImageCode.Name = "tbImageCode";
            this.tbImageCode.Size = new System.Drawing.Size(113, 40);
            this.tbImageCode.TabIndex = 2;
            this.tbImageCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbImageCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbImageCode_KeyPress);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前系统时间：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnRequestUDP);
            this.groupBox1.Controls.Add(this.lbActionStatus);
            this.groupBox1.Controls.Add(this.lbCurrentPrice);
            this.groupBox1.Controls.Add(this.lbStatus);
            this.groupBox1.Controls.Add(this.lbSystemTime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 191);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "投标信息";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "投标阶段：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(9, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "操作状态：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "最低成交价：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRequestUDP
            // 
            this.btnRequestUDP.Location = new System.Drawing.Point(63, 151);
            this.btnRequestUDP.Name = "btnRequestUDP";
            this.btnRequestUDP.Size = new System.Drawing.Size(98, 23);
            this.btnRequestUDP.TabIndex = 0;
            this.btnRequestUDP.Text = "请求投标信息";
            this.btnRequestUDP.UseVisualStyleBackColor = true;
            this.btnRequestUDP.Click += new System.EventHandler(this.btnRequestUDP_Click);
            // 
            // lbActionStatus
            // 
            this.lbActionStatus.Location = new System.Drawing.Point(115, 90);
            this.lbActionStatus.Name = "lbActionStatus";
            this.lbActionStatus.Size = new System.Drawing.Size(118, 20);
            this.lbActionStatus.TabIndex = 3;
            this.lbActionStatus.Text = "等待服务器消息";
            this.lbActionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbCurrentPrice
            // 
            this.lbCurrentPrice.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentPrice.Location = new System.Drawing.Point(115, 120);
            this.lbCurrentPrice.Name = "lbCurrentPrice";
            this.lbCurrentPrice.Size = new System.Drawing.Size(118, 20);
            this.lbCurrentPrice.TabIndex = 3;
            this.lbCurrentPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(115, 60);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(118, 20);
            this.lbStatus.TabIndex = 3;
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbSystemTime
            // 
            this.lbSystemTime.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSystemTime.Location = new System.Drawing.Point(115, 30);
            this.lbSystemTime.Name = "lbSystemTime";
            this.lbSystemTime.Size = new System.Drawing.Size(118, 20);
            this.lbSystemTime.TabIndex = 3;
            this.lbSystemTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbCodeStatus);
            this.groupBox2.Controls.Add(this.pbCaptcha);
            this.groupBox2.Controls.Add(this.btnSubmit);
            this.groupBox2.Controls.Add(this.tbImageCode);
            this.groupBox2.Location = new System.Drawing.Point(258, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(226, 191);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "验证码";
            // 
            // lbCodeStatus
            // 
            this.lbCodeStatus.Location = new System.Drawing.Point(6, 158);
            this.lbCodeStatus.Name = "lbCodeStatus";
            this.lbCodeStatus.Size = new System.Drawing.Size(214, 23);
            this.lbCodeStatus.TabIndex = 3;
            this.lbCodeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMQStatus
            // 
            this.lbMQStatus.Location = new System.Drawing.Point(13, 213);
            this.lbMQStatus.Name = "lbMQStatus";
            this.lbMQStatus.Size = new System.Drawing.Size(471, 20);
            this.lbMQStatus.TabIndex = 3;
            this.lbMQStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 242);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbMQStatus);
            this.Name = "MainForm";
            this.Text = "验证码客户端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbCaptcha)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.PictureBox pbCaptcha;
        private System.Windows.Forms.TextBox tbImageCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCurrentPrice;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbSystemTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbCodeStatus;
        private System.Windows.Forms.Button btnRequestUDP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbActionStatus;
        private System.Windows.Forms.Label lbMQStatus;
    }
}

