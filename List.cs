using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 图像识别
{
    public partial class List : Form
    {
        private Form1 f1;
        public List(Form1 f)
        {
            InitializeComponent();
            f1=f;
            f1.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MV_E_EM MV_E_EM1 = new MV_E_EM(this);
            MV_E_EM1.Show();
            
        }

        private void List_FormClosed(object sender, FormClosedEventArgs e)
        {
            f1.Show();
        }
    }
}
