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
    public partial class mainCD : Form
    {
        public mainCD()
        {
            InitializeComponent();
        }
        public static Form Frm1;
        public static Form Frm2;
        public static Form Frm3;

        //点击按钮输出门禁刷卡窗体;
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if(Frm1 == null || Frm1.IsDisposed)
            {
                Frm1 = new Menjincaozuo();
                Frm1.MdiParent = this;
                Frm1.Show();
                Frm1.Dock = DockStyle.Fill;
            }
            else
            {
                Frm1.Activate();
            }
            
        }

        //点击按钮输出门禁报表记录窗体;
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Frm2 == null || Frm2.IsDisposed)
            {
                Frm2 = new menjinjilu();
                Frm2.MdiParent = this;
                Frm2.Show();
                Frm2.Dock = DockStyle.Fill;
            }
            else
            {
                Frm2.Activate();
            }
                
        }

        //点击按钮输出开卡和查询卡窗体;
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (Frm3 == null || Frm3.IsDisposed)
            {
                Frm3 = new Admin();
                Frm3.MdiParent = this;
                Frm3.Show();
                Frm3.Dock = DockStyle.Fill;
            }
            else
            {
                Frm3.Activate();
            }
            
        }

        private void mainCD_Load(object sender, EventArgs e)
        {

        }
    }
}
