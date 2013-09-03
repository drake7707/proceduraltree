using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TreeGrowingAlgorithm
{
    public partial class StartUp : Form
    {
        public StartUp()
        {
            InitializeComponent();
        }

        private void btn2D_Click(object sender, EventArgs e)
        {
            Tree2DForm frm = new Tree2DForm();
            frm.Show();
        }

        private void btn3D_Click(object sender, EventArgs e)
        {
            Tree3DForm frm = new Tree3DForm();
            frm.Show();
        }
    }
}
