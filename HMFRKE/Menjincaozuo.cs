using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using MySql.Data.MySqlClient;

namespace HMFRKE
{
    public partial class Menjincaozuo : Form
    {
        SerialPort sp = new SerialPort(); //实例化串口对象
        byte[] rdbuffer = new byte[100];
        byte[] wrbuffer = new byte[100];
        byte[] PiccRequest = new byte[] { 0x07, 0x02, 0x41, 0x01, 0x52, 0xe8, 0x03 }; //请求命令数组， 字节类型
        byte[] PiccAnticoll = new byte[] { 0x08, 0x02, 0x42, 0x02, 0x93, 0x00, 0x26, 0x03 }; //防碰撞命令数组， 字节类型
        byte[] cardid = new byte[4];//声明字节类型变量，用来存放s50卡号。

        public string MJKaHao = "";
        public string MJGongHao = "";
        public string MJXinMing = "";
        public string MJTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        public string YX;


        public Menjincaozuo()
        {
            InitializeComponent();
            //------------------------自动获取串口号----------------------------------------
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
            button1.Enabled = true;
            button2.Enabled = false;//关闭串口禁用
            pictureBox1.Image = imageList1.Images[0];
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            /****************串口选择判断***************/
            if (comboBox1.Text.Length <= 1)
            {
                MessageBox.Show("请选择串口！！！！", "错误提示");
                return;//中止不会执行下面的代码
            }

            try //放置可能发生异常的程序代码， 进行监控
            {
                sp.PortName = comboBox1.Text;
                sp.BaudRate = 9600;
                sp.DataBits = 8;
                sp.ReadTimeout = 2000;
                sp.Parity = System.IO.Ports.Parity.None;
                sp.StopBits = System.IO.Ports.StopBits.One;
                sp.ReadBufferSize = 1024;
                sp.WriteBufferSize = 1024;
                sp.Open();


                label2_spzt.Text = "串口 " + comboBox1.Text + " 打开成功！ ";
                button1.Enabled = false;//禁用打开串口按钮
                sp.DiscardInBuffer();
                button2.Enabled = true;//开放 关闭串口按钮、请求、防碰撞、清除 等按钮

            }
            catch (Exception) //异常处理
            {
                MessageBox.Show("串口打开失败，请确认串口是否被其他软件占用！");

                throw;
            }

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sp.Close();
            label2_spzt.Text = "串口关闭！ ";
            button1.Enabled = true;//打开串口按钮启用
            button2.Enabled = false;//关闭串口按钮禁用
        }

        private void xsMenjin_Load(object sender, EventArgs e)
        {
            timer1.Start();  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
         label3_time.Text=DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");           
        }

        //刷卡进入
        private void button3_Click(object sender, EventArgs e)
        {
            getkahao();//请求防碰撞组合并获取卡号

            //1.查询卡号是否存在;
            string dtlength = DateTime.Now.ToString("yyyyMMdd");
            string rdsql = "select * from kaika_tb where kaika_tb.cardID = '" + MJKaHao + "'";
            List<LoginResult> r = MysqlHelp.GetKaiKas(rdsql);
            if (r != null && r.Count > 0)
            {
                //1.1提取查询到的数据其他列数据;

                MJXinMing = r[0].Name;
                MJGongHao = r[0].WorkerNO;


                //2.查询该卡是否为进入状态;
                string rdsqlformat = @"select * from menjinjilu_tb 
                                       where menjinjilu_tb.cardID = '{0}'
                                       and menjinjilu_tb.departureTime is NULL";
                string rdsql1 = string.Format(rdsqlformat, MJKaHao);
                List<MenjinjiluResult> rdmdr1 = MysqlHelp.GetMenJijilus(rdsql1);

                //2.1如果查询不到数据或者不在进入状态;
                if (rdmdr1 == null || //这是查询不到数据
                    (rdmdr1 != null && rdmdr1.Count > 0 && !rdmdr1[0].YX)) //这是有效状态
                {
                    //3查询当天是否有流水号信息记录;
                    string leftidsql = @"select distinct left(menjinjilu_tb.menjinjiluID,8) from menjinjilu_tb
                                        WHERE to_days(menjinjilu_tb.menjinjiluID) = to_days(now())";
                    List<SingleRowResult> s = MysqlHelp.GetSingleRows(leftidsql);
                    if (s == null)
                    {
                        //3.1如果当天没有流水号就执行开门并创建门禁记录信息写入到数据库;
                        YX = "1";
                        string laterID = "00000001";
                        string intofomat = @"insert into menjinjilu_tb values ('{0}{1}','{2}','{3}',NULL,'{4}')";
                        string intosql = string.Format(intofomat, dtlength, laterID, MJKaHao, MJTime, YX);
                        int a = MysqlHelp.ExcuteSql(intosql);
                        if (a > 0)
                        {
                            MessageBox.Show("成功进入");
                        }
                    }
                    else
                    {
                        string strmaxid = "";
                        //3.2如果当天有流水号记录,则查询当天最大流水号信息记录;
                        string rightidsql = @"select MAX(right(menjinjilu_tb.menjinjiluID,8)) as maxmenjinjiluID from menjinjilu_tb
                                            WHERE to_days(menjinjilu_tb.menjinjiluID) = to_days(now())";
                        List<SingleRowResult> s1 = MysqlHelp.GetSingleRows(rightidsql);
                        if (s1 != null && s1.Count > 0)
                        {

                            //2.4将流水号后8位进行自增长;
                            int maxID = 0;
                            maxID = int.Parse(s1[0].Value);
                            maxID++;
                            strmaxid = maxID.ToString().PadLeft(8, '0');

                        }
                        //3.3根据自增长流水号将新进入人员卡号和以及其他信息写入到数据库并执行开门;    
                        YX = "1";
                        string intofomat1 = @"insert into menjinjilu_tb values ('{0}{1}','{2}','{3}',NULL,'{4}')";
                        string intosql1 = string.Format(intofomat1, dtlength, strmaxid, MJKaHao, MJTime, YX);
                        int a = MysqlHelp.ExcuteSql(intosql1);
                        if (a > 0)
                        {
                            MessageBox.Show("成功进入");
                        }

                    }
                    textBox_SHJR.Text = "";
                    label5.Text = "开门成功！";
                    pictureBox1.Image = imageList1.Images[1];
                    label11_kahao.Text = MJKaHao.ToString();
                    label11_name.Text = MJXinMing.ToString();
                    label11_xuehao.Text = MJGongHao.ToString();


                }
                //2.2否则提示此人已进入;
                else
                {
                    MessageBox.Show("此人已进入");
                }
            }
        }

        //刷卡离开
        private void button6_Click(object sender, EventArgs e)
        {
            getkahao();//请求防碰撞组合并获取卡号

            //1.查询该卡是否存在;
            MJKaHao = textBox_SHLK.Text;
            string rdsql1 = "select * from kaika_tb where kaika_tb.cardID = '" + MJKaHao + "'";
            List<LoginResult> r = MysqlHelp.GetKaiKas(rdsql1);
            if (r != null & r.Count > 0)
            {
                //1.1提取查询到的其他列数据;

                MJXinMing = r[0].Name;
                MJGongHao = r[0].WorkerNO;

                //2.查询该卡是否为进入状态;
                string lkfomat = @"select * from menjinjilu_tb 
                                   where menjinjilu_tb.cardID = '{0}' 
                                   and menjinjilu_tb.departureTime is NULL";
                string lksql = string.Format(lkfomat, MJKaHao);
                List<MenjinjiluResult> m = MysqlHelp.GetMenJijilus(lksql);
                if (m != null && m.Count > 0)
                {

                    //2.1如果为进入状态将其修改为离开状态并填入离开时间;
                    string upfomat = @"update menjinjilu_tb 
                                       set menjinjilu_tb.departureTime = '{0}',menjinjilu_tb.YX = '0' 
                                       where menjinjilu_tb.cardID = '{1}' 
                                       and menjinjilu_tb.YX = '1'";
                    string upsql = string.Format(upfomat, MJTime, MJKaHao);
                    int b = MysqlHelp.ExcuteSql(upsql);
                    if (b > 0)
                    {
                        MessageBox.Show("成功离开");
                    }
                    textBox_SHLK.Text = "";
                    label5.Text = "开门成功！";
                    pictureBox1.Image = imageList1.Images[1];
                    label11_kahao.Text = MJKaHao.ToString();
                    label11_name.Text = MJXinMing.ToString();
                    label11_xuehao.Text = MJGongHao.ToString();
                }
                //2.2如不存在则返回提示框;
                else
                {
                    MessageBox.Show("此卡未进入,不能离开");
                }
            }
            //1.2否则提示该卡未启用;
            else
            {
                MessageBox.Show("该卡未启用");
            }
        }

        public void getkahao()//请求防碰撞组合并获取卡号
        {
            /********************************请求***********************************/
            sp.Write(PiccRequest, 0, PiccRequest.Length); //发送请求命令
            System.Threading.Thread.Sleep(100); //延时0.1秒， 单位是毫秒
            sp.Read(rdbuffer, 0, 1); //读应答信息
            if (rdbuffer[0] == 0x08)
            {
                sp.DiscardInBuffer();
            }
            else //由于模块问题（ 请求总是一次请求， 一次失败）， 当请求失败时， 只需重新发送请求命令，就会成功
            {
                sp.DiscardInBuffer();
                //********************************重新发送请求命令***************************
                sp.Write(PiccRequest, 0, PiccRequest.Length); //通过串口发送请求命令
                System.Threading.Thread.Sleep(100); //延时0.1秒， 单位是毫秒
                sp.DiscardInBuffer();
            }            /********************************防碰撞*******************************/
            sp.Write(PiccAnticoll, 0, PiccAnticoll.Length); //发送防碰撞命令
            System.Threading.Thread.Sleep(100); //延时0.1秒， 单位是毫秒
            sp.Read(rdbuffer, 0, 1); //读应答信息
            if (rdbuffer[0] == 0x0A)
            {
                for (int i = 1; i < 0x0A; i++)
                    sp.Read(rdbuffer, i, 1);
                //取出卡号， 存入数组cardid
                cardid[0] = rdbuffer[4];
                cardid[1] = rdbuffer[5];
                cardid[2] = rdbuffer[6];
                cardid[3] = rdbuffer[7];
                sp.DiscardInBuffer();
            }
            else
            {
                MessageBox.Show("未识别到卡片");
                sp.DiscardInBuffer();
                return;
            }
            MJKaHao = byteToHexStr(cardid, cardid.Length);
        }

        public static string byteToHexStr(byte[] bytes, int len) //字节转换成十六进制字符
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " "; //转换成2位的十六进制数字符
                }
            }
            return returnStr;
        }

        public static byte bccCode(byte[] command, int comlen)//校验码
        {
            uint bcc;
            bcc = 0;
            for (int i = 0; i < comlen; i++)
            {
                bcc ^= command[i];
            }
            bcc = ~bcc;

            return (byte)bcc;
        }



        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_SHJR_Click(object sender, EventArgs e)
        {
            //1.查询卡号是否存在;
            MJKaHao = textBox_SHJR.Text;
            string dtlength = DateTime.Now.ToString("yyyyMMdd");            
            string rdsql = "select * from kaika_tb where kaika_tb.cardID = '" + MJKaHao + "'";
            List<LoginResult> r = MysqlHelp.GetKaiKas(rdsql);
            if (r != null && r.Count>0)
            {
                //1.1提取查询到的数据其他列数据;

                MJXinMing = r[0].Name;
                MJGongHao = r[0].WorkerNO;


                //2.查询该卡是否为进入状态;
                string rdsqlformat = @"select * from menjinjilu_tb 
                                       where menjinjilu_tb.cardID = '{0}'
                                       and menjinjilu_tb.departureTime is NULL";
                string rdsql1 = string.Format(rdsqlformat, MJKaHao);
                List<MenjinjiluResult> rdmdr1 = MysqlHelp.GetMenJijilus(rdsql1);

                //2.1如果查询不到数据或者不在进入状态;
                if (rdmdr1 == null || //这是查询不到数据
                    (rdmdr1 != null && rdmdr1.Count >0  && !rdmdr1[0].YX)) //这是有效状态
                {
                    //3查询当天是否有流水号信息记录;
                    string leftidsql = @"select distinct left(menjinjilu_tb.menjinjiluID,8) from menjinjilu_tb
                                        WHERE to_days(menjinjilu_tb.menjinjiluID) = to_days(now())";
                    List<SingleRowResult> s = MysqlHelp.GetSingleRows(leftidsql);
                    if (s == null)
                    {
                        //3.1如果当天没有流水号就执行开门并创建门禁记录信息写入到数据库;
                        YX = "1";
                        string laterID = "00000001";
                        string intofomat = @"insert into menjinjilu_tb values ('{0}{1}','{2}','{3}',NULL,'{4}')";
                        string intosql = string.Format(intofomat, dtlength, laterID, MJKaHao, MJTime, YX);
                        int a = MysqlHelp.ExcuteSql(intosql);
                        if (a > 0)
                        {
                            MessageBox.Show("成功进入");
                        }
                    }
                    else
                    {
                        string strmaxid = "";
                        //3.2如果当天有流水号记录,则查询当天最大流水号信息记录;
                        string rightidsql = @"select MAX(right(menjinjilu_tb.menjinjiluID,8)) as maxmenjinjiluID from menjinjilu_tb
                                            WHERE to_days(menjinjilu_tb.menjinjiluID) = to_days(now())";
                        List<SingleRowResult> s1 = MysqlHelp.GetSingleRows(rightidsql);
                        if (s1 != null && s1.Count >0)
                        {

                                //2.4将流水号后8位进行自增长;
                            int maxID = 0;
                            maxID = int.Parse(s1[0].Value);
                            maxID++;
                            strmaxid = maxID.ToString().PadLeft(8, '0');

                        }
                        //3.3根据自增长流水号将新进入人员卡号和以及其他信息写入到数据库并执行开门;    
                        YX = "1";
                        string intofomat1 = @"insert into menjinjilu_tb values ('{0}{1}','{2}','{3}',NULL,'{4}')";
                        string intosql1 = string.Format(intofomat1, dtlength, strmaxid, MJKaHao, MJTime, YX);
                        int a = MysqlHelp.ExcuteSql(intosql1);
                        if (a > 0)
                        {
                            MessageBox.Show("成功进入");
                        }
                            
                    }
                    textBox_SHJR.Text = "";
                    label5.Text = "开门成功！";
                    pictureBox1.Image = imageList1.Images[1];
                    label11_kahao.Text = MJKaHao.ToString();
                    label11_name.Text = MJXinMing.ToString();
                    label11_xuehao.Text = MJGongHao.ToString();


                }
                //2.2否则提示此人已进入;
                else
                {
                    MessageBox.Show("此人已进入");
                }


            }
            //1.2否则提示该卡未启用;
            else
            {
                MessageBox.Show("该卡未启用");
            }            
        }

        private void button_SHLK_Click(object sender, EventArgs e)
        {
            //1.查询该卡是否存在;
            MJKaHao = textBox_SHLK.Text;
            string rdsql1 = "select * from kaika_tb where kaika_tb.cardID = '" + MJKaHao + "'";
            List<LoginResult> r = MysqlHelp.GetKaiKas(rdsql1);
            if (r != null && r.Count > 0)
            {
                //1.1提取查询到的其他列数据;

                MJXinMing = r[0].Name;
                MJGongHao = r[0].WorkerNO;

                //2.查询该卡是否为进入状态;
                string lkfomat = @"select * from menjinjilu_tb 
                                   where menjinjilu_tb.cardID = '{0}' 
                                   and menjinjilu_tb.departureTime is NULL";
                string lksql = string.Format(lkfomat, MJKaHao);
                List<MenjinjiluResult> m = MysqlHelp.GetMenJijilus(lksql);
                if (m != null && m.Count > 0)
                {

                    //2.1如果为进入状态将其修改为离开状态并填入离开时间;
                    string upfomat = @"update menjinjilu_tb 
                                       set menjinjilu_tb.departureTime = '{0}',menjinjilu_tb.YX = '0' 
                                       where menjinjilu_tb.cardID = '{1}' 
                                       and menjinjilu_tb.YX = '1'";
                    string upsql = string.Format(upfomat, MJTime, MJKaHao);
                    int b = MysqlHelp.ExcuteSql(upsql);
                    if (b > 0)
                    {
                        MessageBox.Show("成功离开");
                    }
                    textBox_SHLK.Text = "";
                    label5.Text = "开门成功！";
                    pictureBox1.Image = imageList1.Images[1];
                    label11_kahao.Text = MJKaHao.ToString();
                    label11_name.Text = MJXinMing.ToString();
                    label11_xuehao.Text = MJGongHao.ToString();
                }
                //2.2如不存在则返回提示框;
                else
                {
                    MessageBox.Show("此卡未进入,不能离开");
                }
            }
            //1.2否则提示该卡未启用;
            else
            {
                MessageBox.Show("该卡未启用");
            }
        }

        //常开
        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = imageList1.Images[1];
            label5.Text = "开门成功！";
            label11_kahao.Text = "";
            label11_name.Text = "";
            label11_xuehao.Text = "";
        }

        //关门
        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = imageList1.Images[0];
            label5.Text = "关门成功！";
            label11_kahao.Text = "";
            label11_name.Text = "";
            label11_xuehao.Text = "";
        }        
    }
}
