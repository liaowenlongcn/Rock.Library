using Rock.Work.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rock.Work
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnChangeId_Click(object sender, EventArgs e)
        {
            this.btnChangeId.Enabled = false;
            AccountHelper.OldUserIdToNew();
            MessageBox.Show("转换完成！");
            this.btnChangeId.Enabled = true;
        }
    }
}
