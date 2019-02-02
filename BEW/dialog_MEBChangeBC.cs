using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BEW
{
    public partial class dialog_MEBChangeBC : Form
    {
        public dialog_MEBChangeBC(ref string text)
        {
            InitializeComponent();
            this.richTextBox_MEBChangeBC.Text = text;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {

        }

        public string FileText
        {
            get { return this.richTextBox_MEBChangeBC.Text; }
        }
    }
}
