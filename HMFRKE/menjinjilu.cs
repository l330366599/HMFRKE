using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HMFRKE
{
    public partial class menjinjilu : Form
    {
        public menjinjilu()
        {
            InitializeComponent();            
        }
        Boolean textboxXMText = false;
        Boolean textboxGHText = false;        
        
        private void menjinjilu_Load(object sender, EventArgs e)
        {
            this.timer1.Start();  //运行实时更新;
            this.comboBox1.SelectedIndex = 1; //comboBox文本框默认显示设置;
            this.dateTimePicker_Start.Value = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            this.dateTimePicker_End.Value = DateTime.Now;
            /*******姓名文本框(textBox_XM)提示信息;*******/
            if (string.IsNullOrEmpty(textBox_XM.Text))
            {
                textBox_XM.Text = "默认:全部人";
                textBox_XM.ForeColor = Color.LightGray;
                textboxXMText = false;
            }

            /*******工号文本框(textBox_GH)提示信息;*******/
            if (string.IsNullOrEmpty(textBox_GH.Text))
            {
                textBox_GH.Text = "默认:全部工号";
                textBox_GH.ForeColor = Color.LightGray;
                textboxGHText = false;
            }
        }

        //查询按钮点击后根据判断条件提取数据;
        private void button1_Click(object sender, EventArgs e)
        {
            
            DateTime dStart = this.dateTimePicker_Start.Value.Date;
            string startDate = dStart.ToString("yyyy-MM-dd HH:mm:ss");//转成字符串
            DateTime dEnd = new DateTime(this.dateTimePicker_End.Value.Year, this.dateTimePicker_End.Value.Month, this.dateTimePicker_End.Value.Day, 23, 59, 59);
            string endDate = dEnd.ToString("yyyy-MM-dd HH:mm:ss");//转成字符串
            string MJXinMing = textBox_XM.Text.Trim().ToString();
            string MJGongHao = textBox_GH.Text.Trim().ToString();
            string StartTime = startDate.ToString();
            string EndTime = endDate.ToString();
            string sqlformat1 = @"select menjinjilu_tb.menjinjiluID AS 记录ID,
                                      kaika_tb.cardID AS 卡号,
                                      kaika_tb.KKName AS 姓名,
                                      kaika_tb.KKGongHao AS 工号,                            
                                      menjinjilu_tb.entryTime AS 进入时间,
                                      menjinjilu_tb.departureTime AS 离开时间,
                                      menjinjilu_tb.YX AS 当前是否在场
                                      from kaika_tb,menjinjilu_tb
                                      WHERE kaika_tb.cardID=menjinjilu_tb.cardID {0}{1}{2}{3}";

            string sqlformat2 = " and menjinjilu_tb.entryTime >= '"+ StartTime +"' and menjinjilu_tb.entryTime <= '"+ EndTime +"'";
            string sqlformat3 = (textBox_XM.ForeColor == Color.Black)?" and kaika_tb.KKName = '"+ MJXinMing +"'":"";
            string sqlformat4 = (textBox_GH.ForeColor == Color.Black) ?" and kaika_tb.KKGongHao = '" + MJGongHao +"'":"";
            string sqlformat5 = (comboBox1.SelectedIndex == 0) ? " and menjinjilu_tb.YX = '1'":"";

            string sql1 = String.Format(sqlformat1, sqlformat2, sqlformat3, sqlformat4, sqlformat5);
            DataSet ds = MysqlHelp.getAllData(sql1);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //工号文本框失去焦点事件;
        private void textBox_XM_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_XM.Text))
            {
                textBox_XM.Text = "默认:全部人";
                textBox_XM.ForeColor = Color.LightGray;
                textboxXMText = false;
            }
            else
                textboxXMText = true;
        }

        //工号文本框获取焦点事件;
        private void textBox_XM_Enter(object sender, EventArgs e)
        {
            if (textboxXMText == false)
            {
                textBox_XM.Text = "";
                textBox_XM.ForeColor = Color.Black;
            }
                
        }

        //工号文本框失去焦点事件;
        private void textBox_GH_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_GH.Text))
            {
                textBox_GH.Text = "默认:全部工号";
                textBox_GH.ForeColor = Color.LightGray;
                textboxGHText = false;
            }
            else
                textboxGHText = true;
        }

        //工号文本框获取焦点事件;
        private void textBox_GH_Enter(object sender, EventArgs e)
        {
            if (textboxGHText == false)
            {
                textBox_GH.Text = "";
                textBox_GH.ForeColor = Color.Black;
            }
        }

        //工号文本框限制条件(只能输入数字);
        private void textBox_GH_KeyPress(object sender, KeyPressEventArgs e)
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

        //转换表格第七列内容(1=是)(0=否);
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
             if (e.ColumnIndex == 6)
             {
                 int val = Convert.ToInt32(e.Value);
                 switch (val)
                 {
                     case 0:
                         e.Value = "否";
                         break;
                     case 1:
                         e.Value = "是";
                         break;
                 }
             }
        }

        //实时更新在场人数;
        private void timer1_Tick(object sender, EventArgs e)
        {         
            string count = "SELECT COUNT(*) as YX from menjinjilu_tb WHERE YX = '1'";
            int c = MysqlHelp.GetRowCount(count);
            if (c > 0)
            {
                label7.Text = c.ToString();
            }
            else
            {
                label7.Text = "0";
            }            
        }
    }
}
