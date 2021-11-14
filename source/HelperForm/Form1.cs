using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelperForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text;

            text = text.Replace(Environment.NewLine, " ");

            string newText = Regex.Replace(text, @"\s+", " ");

            textBox1.Text = newText;
        }
    }
}
