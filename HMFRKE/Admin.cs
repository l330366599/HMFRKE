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
using MySql.Data.MySqlClient;

namespace HMFRKE
{
    public partial class Admin : Form
    {
        SerialPort sp = new SerialPort(); //实例化串口对象
        byte[] rdbuffer = new byte[100];
        byte[] wrbuffer = new byte[100];
        byte[] PiccRequest = new byte[] { 0x07, 0x02, 0x41, 0x01, 0x52, 0xe8, 0x03 }; //请求命令数组， 字节类型
        byte[] PiccAnticoll = new byte[] { 0x08, 0x02, 0x42, 0x02, 0x93, 0x00, 0x26, 0x03 }; //防碰撞命令数组， 字节类型
        byte[] cardid = new byte[4];//声明字节类型变量，用来存放s50卡号。
        //byte[] xuehao = new byte[10];
        //byte[] name = new byte[9];
        public string ADMKaHao = null;
        string ADMXinMing = null;
        public string ADMGongHao = null;
        public Boolean textboxKaHaoText = false;
        public Admin()
        {
            InitializeComponent();
            //------------------------自动获取串口号----------------------------------------
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
            button1.Enabled = true;
            button2.Enabled = false;//关闭串口禁用
        }

        private void button1_Click(object sender, EventArgs e)//打开串口
        {
            /****************串口选择判断***************/
            if(comboBox1.Text.Length<=1)
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

        private void button2_Click(object sender, EventArgs e)//串口关闭按钮
        {
            sp.Close();
            label2_spzt.Text = "串口关闭！ ";
            button1.Enabled = true;//打开串口按钮启用
            button2.Enabled = false;//关闭串口按钮禁用
        }

        private void button3_kaika_Click(object sender, EventArgs e)//确认开卡按钮
        {
            string Str_name;
            string Str_gonghao;
            string Str_kahao;
            Str_name = textBox1_name.Text.ToString();
            Str_gonghao = textBox2_gonghao.Text.ToString();
            Str_kahao = textBox3_kahao.Text.ToString();
            if (!string.IsNullOrEmpty(Str_name) && !string.IsNullOrEmpty(Str_gonghao) && textBox3_kahao.ForeColor != Color.LightGray)
            {
                ////********************************选择********************************
                //wrbuffer[0] = 0x0b;
                //wrbuffer[1] = 0x02;
                //wrbuffer[2] = 0x43;
                //wrbuffer[3] = 0x05;
                //wrbuffer[4] = 0x93;
                //wrbuffer[5] = cardid[0];
                //wrbuffer[6] = cardid[1];
                //wrbuffer[7] = cardid[2];
                //wrbuffer[8] = cardid[3];
                //wrbuffer[9] = bccCode(wrbuffer, 9);
                //wrbuffer[10] = 0x03;
                //sp.Write(wrbuffer, 0, 0x0b); //往串口写入命令
                //sp.Read(rdbuffer, 0, 1);
                //if (rdbuffer[0] == 0x07)
                //{
                //    for (int i = 1; i < 0x07; i++)
                //        sp.Read(rdbuffer, i, 1);
                //}
                //else
                //{
                //    sp.DiscardInBuffer();
                //    return;
                //}
                ////********************************验证********************************
                //wrbuffer[0] = 0x12;
                //wrbuffer[1] = 0x02;
                //wrbuffer[2] = 0x46;
                //wrbuffer[3] = 0x0c;
                //wrbuffer[4] = 0x60;
                //wrbuffer[5] = cardid[0]; //卡号
                //wrbuffer[6] = cardid[1];
                //wrbuffer[7] = cardid[2];
                //wrbuffer[8] = cardid[3];
                //wrbuffer[9] = 0xff; //秘钥
                //wrbuffer[10] = 0xff;
                //wrbuffer[11] = 0xff;
                //wrbuffer[12] = 0xff;
                //wrbuffer[13] = 0xff;
                //wrbuffer[14] = 0xff;
                //wrbuffer[15] = 0x04; //块号
                //wrbuffer[16] = bccCode(wrbuffer, 16);
                //wrbuffer[17] = 0x03;
                //sp.Write(wrbuffer, 0, 0x12); //往串口写入
                //sp.Read(rdbuffer, 0, 1);
                //if (rdbuffer[0] == 0x06)
                //{
                //    for (int i = 1; i < 0x06; i++)
                //        sp.Read(rdbuffer, i, 1);
                //}
                //else
                //{
                //    sp.DiscardInBuffer();
                //    return;
                //}
                ////********************************写数据********************************
                //name = Encoding.UTF8.GetBytes(textBox1_name.Text.Trim());
                //int t = Encoding.UTF8.GetByteCount(textBox1_name.Text);
                //xuehao = System.Text.Encoding.ASCII.GetBytes(textBox2_xuehao.Text.Trim());
                //int g = Encoding.UTF8.GetByteCount(textBox2_xuehao.Text);
                //MessageBox.Show(name.ToString());
                //if (t != 9 || g != 10)
                //{
                //    MessageBox.Show("请确认3位汉字姓名和5位完整学号");
                //    return;
                //}


                //wrbuffer[0] = 0x17;
                //wrbuffer[1] = 0x02;
                //wrbuffer[2] = 0x48;
                //wrbuffer[3] = 0x11;
                //wrbuffer[4] = 0x04;//块号

                //for (int i = 0; i < 10; i++) //存放5位学号
                //    wrbuffer[i + 5] = xuehao[i];
                //for (int i = 0; i < 9; i++) //存放9个字节的名字
                //    wrbuffer[i + 12] = name[i];
                //wrbuffer[21] = bccCode(wrbuffer, 21);
                //wrbuffer[22] = 0x03;
                //sp.Write(wrbuffer, 0, 0x17); //往串口写入写命令数据
                //System.Threading.Thread.Sleep(200);
                //sp.Read(rdbuffer, 0, 1);
                //if (rdbuffer[0] == 0x06)
                //{
                //    for (int i = 1; i < 0x06; i++)
                //        sp.Read(rdbuffer, i, 1);
                //    sp.DiscardInBuffer();
                //    label15_zhuangt.Text = "开卡操作成功！ ";
                //    DateTime.Now.ToShortTimeString();

                //}
                //else
                //{
                //   label15_zhuangt.Text = "开卡操作失败！ ";
                //    sp.DiscardInBuffer();
                //    return;
                //}   
                /**********************【查询是否已经开卡】***************************/
                string sql1 = "select * from kaika_tb where cardID='" + Str_kahao + "'";
                List<LoginResult> rd = MysqlHelp.GetKaiKas(sql1);
                if (rd != null && rd.Count > 0)
                {
                    MessageBox.Show("该卡号已存在！请勿重复开卡！");
                    return;
                }
                /**********************【开卡信息存入数据库】***************************/               
                string sql = "insert into kaika_tb values ('" + Str_kahao + "','" + Str_name + "','" + Str_gonghao + "')";
                int c = MysqlHelp.ExcuteSql(sql);
                try
                {
                    if (c > 0)
                    {
                        MessageBox.Show("      开卡成功" + "\n卡号:" + Str_kahao + 
                                                           "\n姓名:" + Str_name + 
                                                           "\n工号:" + Str_gonghao,"提示",
                                                           MessageBoxButtons.OK,
                                                           MessageBoxIcon.Information);                        
                        textBox1_name.Text = "";
                        textBox2_gonghao.Text = "";
                        textBox3_kahao.Text = "";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("添加失败，请重试");
                }
            }
            else
            {
                MessageBox.Show("请正确填写信息");
            }

        }


        private void textBox2_xuehao_KeyPress(object sender, KeyPressEventArgs e)//只能输入数字的方法
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }

        }

        private void textBox1_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键上
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

        private void button_CX_Click(object sender, EventArgs e)//查询
        {

          getkahao();
          if(ADMKaHao==null)
          {
              //没有识别到卡片将初始化
              string nul=" ";
              label8_kahao.Text = "未识别到卡片";
              label9_name.Text = nul;
              label10_gonghao.Text = nul;
              return;
          }
            string sql = "select * from kaika_tb where kaika_tb.cardID = '" + ADMKaHao + "'";//查询语句
            List<LoginResult> r = MysqlHelp.GetKaiKas(sql);

            if (r != null && r.Count >0)
            {
                ADMXinMing = r[0].Name;//获取学生名字
                ADMGongHao = r[0].WorkerNO;//获取学生学号
            }

            if (ADMXinMing==null)
            {
                MessageBox.Show("该卡片还没开通！");
                return;
            }
            //显示信息
            label8_kahao.Text = ADMKaHao;
            label9_name.Text = ADMXinMing;
            label10_gonghao.Text = ADMGongHao;            
        }

        public void getkahao( )//请求防碰撞组合并获取卡号
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
            ADMKaHao = byteToHexStr(cardid, cardid.Length);
        }

        private void button4_Click(object sender, EventArgs e)//返回
        {
            this.Close();
        }

        private void textBox3_kahao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }

        private void button_JSCX_Click(object sender, EventArgs e)
        {
            Search sch = new Search();
            DialogResult result = sch.ShowDialog();

            //MessageBox.Show(result.ToString());
            
            if (result == DialogResult.OK)
            {

                label10_gonghao.Text = sch.GongHao.ToString();
                label8_kahao.Text = sch.KaHao.ToString();
                label9_name.Text = sch.XingMing.ToString();
                //MessageBox.Show(sch.GongHao.ToString());
            }
            
        }

        private void button_duka_Click(object sender, EventArgs e)
        {
            getkahao();//请求防碰撞组合
            this.textBox3_kahao.Text = ADMKaHao;
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3_kahao.Text))
            {
                textBox3_kahao.Text = "如不读卡请输入卡号";
                textBox3_kahao.ForeColor = Color.LightGray;
                textboxKaHaoText = false;
            }
        }

        private void textBox3_kahao_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3_kahao.Text))
            {
                textBox3_kahao.Text = "如不读卡请输入卡号";
                textBox3_kahao.ForeColor = Color.LightGray;
                textboxKaHaoText = false;
            }
            else
            {
                textboxKaHaoText = true;
            }
        }

        private void textBox3_kahao_Enter(object sender, EventArgs e)
        {
            if (textboxKaHaoText == false)
            {
                textBox3_kahao.Text = "";
                textBox3_kahao.ForeColor = Color.Black;
            }
        }
    }
}
