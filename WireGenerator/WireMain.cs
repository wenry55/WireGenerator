using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WireGenerator
{
    public partial class WireMain : Form
    {
        public WireMain()
        {
            InitializeComponent();
        }

        private void wireToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form frm = new WireBuilder();

            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();

        }

        // Add
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
