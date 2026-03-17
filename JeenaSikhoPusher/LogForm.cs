using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JeenaSikhoPusher
{
    public partial class LogForm : Form
    {
       
        public LogForm()
        {
            InitializeComponent();
            radGridView1.Click += new EventHandler(LogForm_Click);
        }

        private void SyncLog_Load(object sender, EventArgs e)
        {
            var list = ContextMenus.SyncLogList.Select(x => new { process_time = x.process_time, process_result = x.process_result, interval = x.interval });
            radGridView1.DataSource = list;
            txtmesssage.Text = ContextMenus.lastrun + "  : " + ContextMenus.lastmessage;
        }

        private void LogForm_Click(object sender, EventArgs e)
        {
            textBox1.Text = radGridView1.CurrentRow.Cells[1].Value.ToString();
        }
    }
}
