using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMFRKE
{
    public partial class Search : Form
    {
        public string KaHao = null;
        public string GongHao = null;
        public string XingMing = null;
        
        public Search()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //1.判断comboBox和textBox是否为空,如果都不为空则执行;
            if (!string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(textBox_CX.Text))
            {
                //2.如果comboBox的文本是(卡号:)则执行;
                if (comboBox1.Text == "卡号:")
                {
                    this.KaHao = textBox_CX.Text;
                    string sql = "Select * from kaika_tb where kaika_tb.cardID ='" + KaHao + "'";
                    List<LoginResult> r = MysqlHelp.GetKaiKas(sql);
                    if (r != null && r.Count> 0)
                    {
                        XingMing = r[0].Name;//获取学生名字
                        GongHao = r[0].WorkerNO;//获取学生学号
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("查无此信息");
                    }
                    
                }
                //3.如果comboBox的文本是(姓名:)则执行;
                else if (comboBox1.Text == "姓名:")
                {
                    this.XingMing = textBox_CX.Text;
                    string sql = "select * from kaika_tb where kaika_tb.KKName = '" + XingMing + "'";
                    List<LoginResult> r = MysqlHelp.GetKaiKas(sql);
                    if (r != null && r.Count > 0)
                    {
                        KaHao = r[0].ID;
                        GongHao = r[0].WorkerNO;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("查无此信息");
                    }

                }
                //4.如果comboBox的文本是(工号:)则执行;
                else if (comboBox1.Text == "工号:")
                {
                    this.GongHao = textBox_CX.Text;
                    string sql = "select * from kaika_tb where kaika_tb.KKGongHao = '" + GongHao + "'";
                    List<LoginResult> r = MysqlHelp.GetKaiKas(sql);

                    if (r != null && r.Count >0)
                    {
                        KaHao = r[0].ID;
                        XingMing = r[0].Name;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {                    
                        MessageBox.Show("查无此信息");
                    }
                }
            }
            //1.1否则输出提示消息;
            else
            {
                MessageBox.Show("请选择或输入正确信息");
            }        
            
        }

        private void Search_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;       //comboBox文本默认显示;
        }
    }
}
